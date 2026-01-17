import Anthropic from '@anthropic-ai/sdk';
import { LLM_CONFIG } from '../../config/llm-config.js';
import { SYSTEM_CONFIG } from '../../config/system-config.js';
import logger from '../../utils/logger.js';
import costTracker from '../../utils/cost-tracker.js';
import ragSystem from './rag-system.js';
import { GENERATOR_SYSTEM_PROMPT, createGeneratorPrompt } from './prompts.js';

class CodeGeneratorAgent {
  constructor() {
    this.anthropic = new Anthropic({ apiKey: LLM_CONFIG.anthropic.apiKey });
    this.model = LLM_CONFIG.anthropic.model;
    this.temperature = LLM_CONFIG.anthropic.temperature;
    this.maxTokens = LLM_CONFIG.anthropic.maxTokens;
  }

  async generate(task, retryCount = 0) {
    try {
      logger.thinking('CodeGenerator', `Generating code for: ${task.component || task.description}`);

      // Retrieve relevant documentation
      const query = `${task.description} ${task.type} ${task.files.join(' ')}`;
      const docs = await ragSystem.retrieve(query, 3);
      const relevantDocs = ragSystem.formatContextForPrompt(docs);

      logger.success('CodeGenerator', `Retrieved ${docs.length} relevant documentation chunks`);

      // Generate code using Claude
      const response = await this.anthropic.messages.create({
        model: this.model,
        max_tokens: this.maxTokens,
        temperature: this.temperature,
        system: GENERATOR_SYSTEM_PROMPT,
        messages: [
          {
            role: 'user',
            content: createGeneratorPrompt(task, relevantDocs)
          }
        ]
      });

      const content = response.content[0].text;
      const usage = response.usage;

      // Track cost
      const cost = costTracker.trackGenerator(
        usage.input_tokens,
        usage.output_tokens
      );

      logger.success('CodeGenerator', 'Code generation complete');
      logger.cost('CodeGenerator', cost);

      // Parse response
      let result;
      try {
        // Try to extract JSON if there's extra text
        const jsonMatch = content.match(/\{[\s\S]*\}/);
        if (jsonMatch) {
          result = JSON.parse(jsonMatch[0]);
        } else {
          result = JSON.parse(content);
        }
      } catch (parseError) {
        logger.error('CodeGenerator', 'Failed to parse JSON response');
        throw new Error('Invalid JSON response from generator');
      }

      if (!this.validateResult(result)) {
        throw new Error('Invalid result format from generator');
      }

      logger.info('CodeGenerator', `Generated ${result.files.length} files`);

      return {
        success: true,
        result: result,
        usage: usage,
        cost: cost
      };

    } catch (error) {
      logger.error('CodeGenerator', `Generation failed: ${error.message}`);

      // Retry logic
      if (retryCount < SYSTEM_CONFIG.maxRetries) {
        logger.warning('CodeGenerator', `Retrying... (${retryCount + 1}/${SYSTEM_CONFIG.maxRetries})`);
        await this.delay(SYSTEM_CONFIG.retryDelay);
        return this.generate(task, retryCount + 1);
      }

      return {
        success: false,
        error: error.message,
        result: null
      };
    }
  }

  async generateMultiple(tasks) {
    const results = [];
    const allDependencies = new Set();

    for (let i = 0; i < tasks.length; i++) {
      const task = tasks[i];
      logger.section(`Task ${i + 1}/${tasks.length}: ${task.component || task.description}`);

      const result = await this.generate(task);

      if (result.success) {
        results.push(result.result);

        // Collect dependencies
        if (result.result.dependencies) {
          result.result.dependencies.forEach(dep => allDependencies.add(dep));
        }
      } else {
        logger.error('CodeGenerator', `Failed to generate code for task ${i + 1}`);
        return {
          success: false,
          error: result.error,
          results: results,
          dependencies: Array.from(allDependencies)
        };
      }
    }

    return {
      success: true,
      results: results,
      dependencies: Array.from(allDependencies)
    };
  }

  validateResult(result) {
    if (!result.files || !Array.isArray(result.files)) {
      return false;
    }

    for (const file of result.files) {
      if (!file.path || !file.content) {
        return false;
      }
    }

    return true;
  }

  delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  getAllFiles(results) {
    const allFiles = [];

    for (const result of results) {
      if (result.files) {
        allFiles.push(...result.files);
      }
    }

    return allFiles;
  }

  getAllDependencies(results) {
    const allDeps = new Set();

    for (const result of results) {
      if (result.dependencies) {
        result.dependencies.forEach(dep => allDeps.add(dep));
      }
    }

    return Array.from(allDeps);
  }

  formatGenerationSummary(results) {
    let summary = `\n📦 Generation Summary:\n`;
    summary += `\nTasks completed: ${results.length}\n`;

    const allFiles = this.getAllFiles(results);
    summary += `Files generated: ${allFiles.length}\n`;

    summary += `\n📄 Generated Files:\n`;
    allFiles.forEach((file, idx) => {
      summary += `   ${idx + 1}. ${file.path}\n`;
      if (file.description) {
        summary += `      ${file.description}\n`;
      }
    });

    const allDeps = this.getAllDependencies(results);
    if (allDeps.length > 0) {
      summary += `\n📦 Dependencies:\n`;
      allDeps.forEach((dep, idx) => {
        summary += `   ${idx + 1}. ${dep}\n`;
      });
    }

    return summary;
  }
}

export default new CodeGeneratorAgent();
