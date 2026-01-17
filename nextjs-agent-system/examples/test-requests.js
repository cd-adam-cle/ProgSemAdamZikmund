import MultiAgentSystem from '../system.js';
import logger from '../utils/logger.js';

const TEST_REQUESTS = [
  {
    name: 'Simple Blog Homepage',
    request: 'Create a simple blog homepage with a list of posts',
    description: 'Basic blog layout with post cards'
  },
  {
    name: 'Contact Form',
    request: 'Create a contact form component with API endpoint',
    description: 'Form with validation and API route'
  },
  {
    name: 'Todo App',
    request: 'Create a todo app with add, delete, and toggle functionality',
    description: 'Interactive todo list with local state'
  },
  {
    name: 'User Dashboard',
    request: 'Create a dashboard with user profile and settings page',
    description: 'Multi-page dashboard layout'
  },
  {
    name: 'Product Catalog',
    request: 'Create a product catalog with grid layout and search',
    description: 'E-commerce product listing'
  }
];

async function runTest(testCase) {
  logger.header(`🧪 Test: ${testCase.name}`);
  logger.info('Test', testCase.description);
  logger.info('Test', `Request: "${testCase.request}"`);

  const system = new MultiAgentSystem();

  try {
    const result = await system.run(testCase.request);

    if (result.success) {
      logger.success('Test', `✓ ${testCase.name} - PASSED`);
      return { ...testCase, success: true, result };
    } else {
      logger.error('Test', `✗ ${testCase.name} - FAILED`);
      return { ...testCase, success: false, result };
    }
  } catch (error) {
    logger.error('Test', `✗ ${testCase.name} - ERROR: ${error.message}`);
    return { ...testCase, success: false, error: error.message };
  }
}

async function runAllTests() {
  logger.header('🚀 Running Test Suite');
  logger.info('Test Suite', `Total tests: ${TEST_REQUESTS.length}`);

  const results = [];

  for (let i = 0; i < TEST_REQUESTS.length; i++) {
    const testCase = TEST_REQUESTS[i];

    logger.section(`\nTest ${i + 1}/${TEST_REQUESTS.length}`);

    const result = await runTest(testCase);
    results.push(result);

    // Wait between tests to avoid rate limiting
    if (i < TEST_REQUESTS.length - 1) {
      logger.info('Test', 'Waiting 5 seconds before next test...');
      await new Promise(resolve => setTimeout(resolve, 5000));
    }
  }

  // Summary
  logger.header('📊 Test Summary');

  const passed = results.filter(r => r.success).length;
  const failed = results.length - passed;

  logger.info('Summary', `Total: ${results.length}`);
  logger.success('Summary', `Passed: ${passed}`);

  if (failed > 0) {
    logger.error('Summary', `Failed: ${failed}`);
  }

  logger.raw('\nResults:\n');
  results.forEach((result, idx) => {
    const status = result.success ? '✓' : '✗';
    const color = result.success ? '\x1b[32m' : '\x1b[31m';
    console.log(`  ${color}${status}\x1b[0m ${result.name}`);

    if (result.result) {
      console.log(`     Files: ${result.result.filesCreated}`);
      console.log(`     Cost: $${result.result.costs.total.toFixed(4)}`);
      console.log(`     Time: ${result.result.duration}s`);
    }
  });

  logger.raw('\n' + '='.repeat(60) + '\n');
}

async function runSingleTest(testIndex) {
  if (testIndex < 0 || testIndex >= TEST_REQUESTS.length) {
    logger.error('Test', `Invalid test index. Choose 0-${TEST_REQUESTS.length - 1}`);
    return;
  }

  const testCase = TEST_REQUESTS[testIndex];
  await runTest(testCase);
}

// CLI Interface
async function main() {
  const args = process.argv.slice(2);

  if (args.length === 0) {
    console.log(`
🧪 Test Suite for Next.js Multi-Agent System

Usage:
  node examples/test-requests.js all         # Run all tests
  node examples/test-requests.js <index>     # Run specific test (0-${TEST_REQUESTS.length - 1})
  node examples/test-requests.js list        # List all tests

Available tests:
${TEST_REQUESTS.map((t, i) => `  ${i}. ${t.name} - ${t.description}`).join('\n')}
`);
    process.exit(0);
  }

  const command = args[0];

  if (command === 'list') {
    console.log('\nAvailable Tests:\n');
    TEST_REQUESTS.forEach((test, idx) => {
      console.log(`${idx}. ${test.name}`);
      console.log(`   Request: "${test.request}"`);
      console.log(`   Description: ${test.description}\n`);
    });
  } else if (command === 'all') {
    await runAllTests();
  } else {
    const testIndex = parseInt(command, 10);
    if (isNaN(testIndex)) {
      logger.error('Test', 'Invalid command. Use: all, list, or a test index (0-4)');
      process.exit(1);
    }
    await runSingleTest(testIndex);
  }
}

main().catch(error => {
  logger.error('Test', `Fatal error: ${error.message}`);
  console.error(error);
  process.exit(1);
});
