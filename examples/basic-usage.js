/**
 * BASIC USAGE EXAMPLE
 *
 * This example demonstrates how to use the Multi-Agent System
 * to build a Next.js feature
 */

// Import all agents
const ArchonAgent = require('../agents/archon/archon-agent');
const FrontendAgent = require('../agents/frontend/frontend-agent');
const BackendAgent = require('../agents/backend/backend-agent');
const TestingAgent = require('../agents/testing/testing-agent');
const CodeReviewAgent = require('../agents/code-review/code-review-agent');
const DocumentationAgent = require('../agents/documentation/documentation-agent');

/**
 * Initialize the Multi-Agent System
 */
async function initializeAgentSystem() {
  console.log('🚀 Initializing Multi-Agent System...\n');

  // Create the Archon (manager)
  const archon = new ArchonAgent();

  // Create specialized agents
  const frontendAgent = new FrontendAgent();
  const backendAgent = new BackendAgent();
  const testingAgent = new TestingAgent();
  const codeReviewAgent = new CodeReviewAgent();
  const documentationAgent = new DocumentationAgent();

  // Register all agents with Archon
  archon.registerAgent(frontendAgent);
  archon.registerAgent(backendAgent);
  archon.registerAgent(testingAgent);
  archon.registerAgent(codeReviewAgent);
  archon.registerAgent(documentationAgent);

  console.log('✅ All agents registered\n');

  return archon;
}

/**
 * Example 1: Build a simple user dashboard
 */
async function example1_BuildUserDashboard() {
  console.log('=' .repeat(60));
  console.log('EXAMPLE 1: Build a User Dashboard');
  console.log('='.repeat(60) + '\n');

  const archon = await initializeAgentSystem();

  // User request
  const userRequest = "Create a user dashboard page with an API to fetch user data";

  // Process the request
  const report = await archon.processRequest(userRequest);

  console.log('\n📊 Final Report:', report);

  return report;
}

/**
 * Example 2: Add authentication to the application
 */
async function example2_AddAuthentication() {
  console.log('\n\n');
  console.log('='.repeat(60));
  console.log('EXAMPLE 2: Add Authentication');
  console.log('='.repeat(60) + '\n');

  const archon = await initializeAgentSystem();

  // User request
  const userRequest = "Add user authentication with login page and auth API";

  // Process the request
  const report = await archon.processRequest(userRequest);

  console.log('\n📊 Final Report:', report);

  return report;
}

/**
 * Example 3: Create a blog feature
 */
async function example3_CreateBlogFeature() {
  console.log('\n\n');
  console.log('='.repeat(60));
  console.log('EXAMPLE 3: Create Blog Feature');
  console.log('='.repeat(60) + '\n');

  const archon = await initializeAgentSystem();

  // User request
  const userRequest = "Create a blog with posts component, database models, and API routes";

  // Setup some initial context
  archon.updateContext('database', 'PostgreSQL');
  archon.updateContext('authentication', true);

  // Process the request
  const report = await archon.processRequest(userRequest);

  console.log('\n📊 Final Report:', report);

  return report;
}

/**
 * Example 4: Check system status
 */
async function example4_CheckStatus() {
  console.log('\n\n');
  console.log('='.repeat(60));
  console.log('EXAMPLE 4: System Status');
  console.log('='.repeat(60) + '\n');

  const archon = await initializeAgentSystem();

  // Get system status
  const status = archon.getStatus();

  console.log('📊 System Status:');
  console.log(JSON.stringify(status, null, 2));

  return status;
}

/**
 * Run all examples
 */
async function runAllExamples() {
  try {
    // Run Example 1
    await example1_BuildUserDashboard();

    // Wait a bit between examples
    await new Promise(resolve => setTimeout(resolve, 1000));

    // Run Example 2
    await example2_AddAuthentication();

    // Wait a bit
    await new Promise(resolve => setTimeout(resolve, 1000));

    // Run Example 3
    await example3_CreateBlogFeature();

    // Wait a bit
    await new Promise(resolve => setTimeout(resolve, 1000));

    // Run Example 4
    await example4_CheckStatus();

    console.log('\n\n✅ All examples completed successfully!');
  } catch (error) {
    console.error('\n❌ Error running examples:', error);
  }
}

// Run examples if this file is executed directly
if (require.main === module) {
  runAllExamples();
}

// Export for use in other files
module.exports = {
  initializeAgentSystem,
  example1_BuildUserDashboard,
  example2_AddAuthentication,
  example3_CreateBlogFeature,
  example4_CheckStatus
};
