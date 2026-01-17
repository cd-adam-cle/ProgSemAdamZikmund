#!/usr/bin/env node

import logger from './utils/logger.js';
import costTracker from './utils/cost-tracker.js';
import fileManager from './utils/file-manager.js';
import coordinatorAgent from './agents/coordinator/coordinator-agent.js';
import codeGeneratorAgent from './agents/generator/code-generator-agent.js';
import validatorAgent from './agents/validator/validator-agent.js';
import ragSystem from './agents/generator/rag-system.js';
import { SYSTEM_CONFIG } from './config/system-config.js';

class MultiAgentSystem {
  constructor() {
    this.startTime = null;
  }

  async run(userRequest) {
    this.startTime = Date.now();

    logger.header('🚀 Multi-Agent System Starting...');
    logger.info('System', `Request: "${userRequest}"`);

    try {
      // Initialize RAG system
      await ragSystem.initialize();

      // Step 1: Coordinator analyzes request
      logger.section('\n📋 Phase 1: Request Analysis');
      const coordinatorResult = await coordinatorAgent.analyze(userRequest);

      if (!coordinatorResult.success) {
        throw new Error(`Coordinator failed: ${coordinatorResult.error}`);
      }

      const plan = coordinatorResult.plan;
      console.log(coordinatorAgent.formatPlanSummary(plan));

      // Create project name from request
      const projectName = this.generateProjectName(userRequest);
      logger.info('System', `Project name: ${projectName}`);

      // Check if project already exists
      if (await fileManager.projectExists(projectName)) {
        logger.error('System', `Project "${projectName}" already exists!`);
        logger.info('System', 'Please delete the existing project or choose a different name.');
        return this.generateReport(false, 'Project already exists', 0, null, 0);
      }

      // Create project directory
      const projectPath = await fileManager.createProject(projectName);
      logger.success('System', `Created project directory: ${projectPath}`);

      // Step 2: Generate code
      logger.section('\n💻 Phase 2: Code Generation');
      const generatorResult = await codeGeneratorAgent.generateMultiple(plan.tasks);

      if (!generatorResult.success) {
        throw new Error(`Code generation failed: ${generatorResult.error}`);
      }

      console.log(codeGeneratorAgent.formatGenerationSummary(generatorResult.results));

      // Step 3: Write files to disk
      logger.section('\n📝 Phase 3: Writing Files');
      const allFiles = codeGeneratorAgent.getAllFiles(generatorResult.results);
      const allDependencies = generatorResult.dependencies;

      // Setup project boilerplate
      logger.info('System', 'Setting up project structure...');
      await fileManager.setupProjectBoilerplate(projectPath, allDependencies);
      logger.success('System', 'Created boilerplate files');

      // Write generated files
      let filesWritten = 0;
      for (const file of allFiles) {
        try {
          await fileManager.writeFile(projectPath, file.path, file.content);
          logger.success('System', `✓ ${file.path}`);
          filesWritten++;
        } catch (error) {
          logger.error('System', `✗ ${file.path}: ${error.message}`);
        }
      }

      logger.success('System', `Wrote ${filesWritten} files to disk`);

      // Step 4: Validate code
      logger.section('\n🔍 Phase 4: Validation');
      const validationResult = await validatorAgent.validate(projectPath, allFiles);

      console.log(validatorAgent.formatValidationReport(validationResult));

      // Generate final report
      const duration = ((Date.now() - this.startTime) / 1000).toFixed(1);
      const success = validationResult.overallStatus === 'passed';

      return this.generateReport(
        success,
        null,
        filesWritten,
        projectPath,
        duration
      );

    } catch (error) {
      logger.error('System', `Fatal error: ${error.message}`);
      const duration = ((Date.now() - this.startTime) / 1000).toFixed(1);
      return this.generateReport(false, error.message, 0, null, duration);
    }
  }

  generateProjectName(request) {
    // Convert request to kebab-case project name
    const name = request
      .toLowerCase()
      .replace(/[^a-z0-9\s-]/g, '')
      .replace(/\s+/g, '-')
      .substring(0, 50);

    return name || 'nextjs-project';
  }

  generateReport(success, error, filesCreated, projectPath, duration) {
    const costs = costTracker.getCosts();

    logger.header('📊 EXECUTION REPORT');

    if (success) {
      logger.success('System', 'Status: SUCCESS');
    } else {
      logger.error('System', 'Status: FAILED');
      if (error) {
        logger.error('System', `Error: ${error}`);
      }
    }

    if (projectPath) {
      logger.info('System', `Project: ${projectPath}`);
    }

    logger.info('System', `Files: ${filesCreated} created`);

    costTracker.printSummary();

    logger.info('System', `Time: ${duration}s`);

    // Save to history if configured
    if (SYSTEM_CONFIG.saveHistory && projectPath) {
      costTracker.saveHistory({
        request: projectPath,
        success: success,
        filesCreated: filesCreated
      });
    }

    if (success && projectPath) {
      logger.section('\n🎉 Project Ready!');
      logger.info('System', 'Next steps:');
      logger.raw(`  1. cd ${projectPath}`);
      logger.raw(`  2. npm install`);
      logger.raw(`  3. npm run dev`);
      logger.raw(`  4. Open http://localhost:3000\n`);
    }

    logger.raw('='.repeat(60) + '\n');

    return {
      success,
      error,
      filesCreated,
      projectPath,
      costs,
      duration
    };
  }
}

// CLI Interface
async function main() {
  const args = process.argv.slice(2);

  if (args.length === 0) {
    console.log(`
🤖 Next.js Multi-Agent System

Usage:
  node system.js "Your request here"

Examples:
  node system.js "Create a simple blog homepage with posts"
  node system.js "Create a contact form with API endpoint"
  node system.js "Create a todo app with local storage"

The system will:
  1. Analyze your request (GPT-4)
  2. Generate Next.js code (Claude Sonnet)
  3. Validate the code (ESLint)
  4. Create a ready-to-run project

Output will be in: ./output/your-project-name/
`);
    process.exit(0);
  }

  const userRequest = args.join(' ');
  const system = new MultiAgentSystem();

  try {
    await system.run(userRequest);
  } catch (error) {
    logger.error('System', `Unhandled error: ${error.message}`);
    console.error(error);
    process.exit(1);
  }
}

// Run if called directly
if (import.meta.url === `file://${process.argv[1]}`) {
  main();
}

export default MultiAgentSystem;
