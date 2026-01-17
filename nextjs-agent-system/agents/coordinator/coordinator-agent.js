import OpenAI from 'openai';
import { LLM_CONFIG } from '../../config/llm-config.js';
import { SYSTEM_CONFIG } from '../../config/system-config.js';
import logger from '../../utils/logger.js';
import costTracker from '../../utils/cost-tracker.js';
import { COORDINATOR_SYSTEM_PROMPT, createCoordinatorPrompt } from './prompts.js';

class CoordinatorAgent {
  constructor() {
    this.openai = new OpenAI({ apiKey: LLM_CONFIG.openai.apiKey });
    this.model = LLM_CONFIG.openai.model;
    this.temperature = LLM_CONFIG.openai.temperature;
    this.maxTokens = LLM_CONFIG.openai.maxTokens;
  }

  async analyze(userRequest, retryCount = 0) {
    try {
      logger.thinking('Coordinator', 'Analyzing user request...');

      const messages = [
        { role: 'system', content: COORDINATOR_SYSTEM_PROMPT },
        { role: 'user', content: createCoordinatorPrompt(userRequest) }
      ];

      const response = await this.openai.chat.completions.create({
        model: this.model,
        messages: messages,
        temperature: this.temperature,
        max_tokens: this.maxTokens,
        response_format: { type: 'json_object' }
      });

      const result = response.choices[0].message.content;
      const usage = response.usage;

      // Track cost
      const cost = costTracker.trackCoordinator(
        usage.prompt_tokens,
        usage.completion_tokens
      );

      logger.success('Coordinator', `Analysis complete`);
      logger.cost('Coordinator', cost);

      // Parse and validate response
      const plan = JSON.parse(result);

      if (!this.validatePlan(plan)) {
        throw new Error('Invalid plan format received from coordinator');
      }

      logger.info('Coordinator', `Identified ${plan.tasks.length} tasks (${plan.analysis.complexity} complexity)`);

      return {
        success: true,
        plan: plan,
        usage: usage,
        cost: cost
      };

    } catch (error) {
      logger.error('Coordinator', `Analysis failed: ${error.message}`);

      // Retry logic
      if (retryCount < SYSTEM_CONFIG.maxRetries) {
        logger.warning('Coordinator', `Retrying... (${retryCount + 1}/${SYSTEM_CONFIG.maxRetries})`);
        await this.delay(SYSTEM_CONFIG.retryDelay);
        return this.analyze(userRequest, retryCount + 1);
      }

      return {
        success: false,
        error: error.message,
        plan: null
      };
    }
  }

  validatePlan(plan) {
    // Check if plan has required structure
    if (!plan.analysis || !plan.tasks || !Array.isArray(plan.tasks)) {
      return false;
    }

    if (!plan.analysis.summary || !plan.analysis.complexity) {
      return false;
    }

    // Validate each task
    for (const task of plan.tasks) {
      if (!task.type || !task.description || !task.files || !Array.isArray(task.files)) {
        return false;
      }
    }

    return true;
  }

  delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  formatPlanSummary(plan) {
    let summary = `\n📋 Execution Plan:\n`;
    summary += `\nComplexity: ${plan.analysis.complexity.toUpperCase()}\n`;
    summary += `Estimated Files: ${plan.analysis.estimatedFiles}\n`;
    summary += `\n${plan.analysis.summary}\n`;

    summary += `\n📝 Tasks:\n`;
    plan.tasks.forEach((task, idx) => {
      summary += `\n${idx + 1}. ${task.component || task.description}\n`;
      summary += `   Type: ${task.type}\n`;
      summary += `   Files: ${task.files.join(', ')}\n`;
      if (task.dependencies && task.dependencies.length > 0) {
        summary += `   Dependencies: ${task.dependencies.join(', ')}\n`;
      }
    });

    if (plan.recommendations && plan.recommendations.length > 0) {
      summary += `\n💡 Recommendations:\n`;
      plan.recommendations.forEach((rec, idx) => {
        summary += `   ${idx + 1}. ${rec}\n`;
      });
    }
    return summary;
  }
}

export default new CoordinatorAgent();
