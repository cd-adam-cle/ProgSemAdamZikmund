#!/usr/bin/env node

import fs from 'fs/promises';
import path from 'path';
import { fileURLToPath } from 'url';

const __dirname = path.dirname(fileURLToPath(import.meta.url));

console.log('\n🔍 Verifying Next.js Multi-Agent System Setup...\n');

const checks = {
  passed: 0,
  failed: 0,
  warnings: 0
};

async function checkFile(filePath, description) {
  try {
    await fs.access(filePath);
    console.log(`✅ ${description}`);
    checks.passed++;
    return true;
  } catch {
    console.log(`❌ ${description}`);
    checks.failed++;
    return false;
  }
}

async function checkDirectory(dirPath, description) {
  try {
    const stat = await fs.stat(dirPath);
    if (stat.isDirectory()) {
      console.log(`✅ ${description}`);
      checks.passed++;
      return true;
    } else {
      console.log(`❌ ${description} (exists but not a directory)`);
      checks.failed++;
      return false;
    }
  } catch {
    console.log(`❌ ${description}`);
    checks.failed++;
    return false;
  }
}

async function checkEnvFile() {
  try {
    const content = await fs.readFile('.env', 'utf-8');
    const hasOpenAI = content.includes('OPENAI_API_KEY=sk-');
    const hasAnthropic = content.includes('ANTHROPIC_API_KEY=sk-ant-');

    if (hasOpenAI && hasAnthropic) {
      console.log('✅ .env file with API keys configured');
      checks.passed++;
      return true;
    } else {
      console.log('⚠️  .env file exists but API keys may not be configured');
      checks.warnings++;
      return false;
    }
  } catch {
    console.log('⚠️  .env file not found (copy from .env.example)');
    checks.warnings++;
    return false;
  }
}

async function verify() {
  console.log('📋 Configuration Files:');
  await checkFile('package.json', 'package.json');
  await checkFile('.env.example', '.env.example');
  await checkFile('.gitignore', '.gitignore');
  await checkEnvFile();

  console.log('\n🤖 Agent Files:');
  await checkFile('agents/coordinator/coordinator-agent.js', 'Coordinator Agent');
  await checkFile('agents/coordinator/prompts.js', 'Coordinator Prompts');
  await checkFile('agents/generator/code-generator-agent.js', 'Code Generator Agent');
  await checkFile('agents/generator/rag-system.js', 'RAG System');
  await checkFile('agents/generator/prompts.js', 'Generator Prompts');
  await checkFile('agents/validator/validator-agent.js', 'Validator Agent');
  await checkFile('agents/validator/eslint.config.js', 'ESLint Config');

  console.log('\n⚙️  Configuration:');
  await checkFile('config/llm-config.js', 'LLM Configuration');
  await checkFile('config/system-config.js', 'System Configuration');

  console.log('\n🛠️  Utilities:');
  await checkFile('utils/logger.js', 'Logger');
  await checkFile('utils/cost-tracker.js', 'Cost Tracker');
  await checkFile('utils/file-manager.js', 'File Manager');

  console.log('\n📚 Knowledge Base:');
  await checkFile('knowledge-base/nextjs-docs/app-router.txt', 'App Router Docs');
  await checkFile('knowledge-base/nextjs-docs/server-components.txt', 'Server Components Docs');
  await checkFile('knowledge-base/nextjs-docs/api-routes.txt', 'API Routes Docs');
  await checkFile('knowledge-base/nextjs-docs/data-fetching.txt', 'Data Fetching Docs');
  await checkFile('knowledge-base/nextjs-docs/styling.txt', 'Styling Docs');

  console.log('\n🎯 Main System:');
  await checkFile('system.js', 'Main System Orchestrator');
  await checkFile('examples/test-requests.js', 'Test Suite');

  console.log('\n📁 Directories:');
  await checkDirectory('output', 'Output directory');
  await checkDirectory('knowledge-base/embeddings', 'Embeddings directory');

  console.log('\n📖 Documentation:');
  await checkFile('README.md', 'README');
  await checkFile('QUICK-START.md', 'Quick Start Guide');
  await checkFile('PROJECT-SUMMARY.md', 'Project Summary');

  // Check if node_modules exists
  console.log('\n📦 Dependencies:');
  try {
    await fs.access('node_modules');
    console.log('✅ node_modules exists (dependencies installed)');
    checks.passed++;
  } catch {
    console.log('⚠️  node_modules not found (run: npm install)');
    checks.warnings++;
  }

  // Summary
  console.log('\n' + '='.repeat(60));
  console.log('📊 Verification Summary:');
  console.log('='.repeat(60));
  console.log(`✅ Passed:   ${checks.passed}`);
  console.log(`❌ Failed:   ${checks.failed}`);
  console.log(`⚠️  Warnings: ${checks.warnings}`);
  console.log('='.repeat(60));

  if (checks.failed === 0 && checks.warnings <= 2) {
    console.log('\n🎉 Setup verification PASSED!');
    console.log('\nNext steps:');
    console.log('1. Ensure API keys are in .env file');
    console.log('2. Run: npm install (if not done)');
    console.log('3. Test: node system.js "Create a simple homepage"');
  } else if (checks.failed > 0) {
    console.log('\n❌ Setup verification FAILED!');
    console.log('Please check the missing files above.');
  } else {
    console.log('\n⚠️  Setup mostly complete with minor warnings.');
    console.log('Please review the warnings above.');
  }

  console.log('');
}

verify().catch(err => {
  console.error('Error during verification:', err);
  process.exit(1);
});
