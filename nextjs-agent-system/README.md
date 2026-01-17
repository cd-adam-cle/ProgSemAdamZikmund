# Next.js Multi-Agent System

A production-ready multi-agent system that uses AI to automatically generate complete Next.js 14 applications. The system orchestrates three specialized AI agents to analyze requests, generate code, and validate output.

## Features

- **Coordinator Agent (GPT-4)**: Analyzes user requests and creates detailed execution plans
- **Code Generator Agent (Claude Sonnet 4.5)**: Generates production-ready Next.js 14 code with TypeScript
- **Validator Agent (Rule-based)**: Validates syntax, structure, and imports
- **RAG System**: Retrieves relevant Next.js documentation to improve code generation
- **Cost Tracking**: Monitors and reports API usage costs
- **Automatic Project Setup**: Creates complete Next.js projects with all boilerplate

## Architecture

```
User Request
     ↓
Coordinator Agent (GPT-4)
     ↓ (Execution Plan)
Code Generator Agent (Claude) + RAG System
     ↓ (Generated Code)
File Manager (Writes to disk)
     ↓
Validator Agent (ESLint, Structure checks)
     ↓
Ready-to-run Next.js Project
```

## Prerequisites

- Node.js 20 or higher
- OpenAI API key (for GPT-4 and embeddings)
- Anthropic API key (for Claude Sonnet)

## Installation

1. Clone the repository:
```bash
cd nextjs-agent-system
```

2. Install dependencies:
```bash
npm install
```

3. Create `.env` file:
```bash
cp .env.example .env
```

4. Add your API keys to `.env`:
```
OPENAI_API_KEY=sk-...
ANTHROPIC_API_KEY=sk-ant-...
```

## Usage

### Basic Usage

Generate a Next.js project from a natural language request:

```bash
node system.js "Create a simple blog homepage with posts"
```

The system will:
1. Analyze your request
2. Create an execution plan
3. Generate TypeScript/React code
4. Validate the code
5. Create a complete project in `./output/`

### Example Requests

```bash
# Simple blog
node system.js "Create a blog homepage with a list of posts"

# Contact form with API
node system.js "Create a contact form with email validation and API endpoint"

# Todo application
node system.js "Create a todo app with add, delete, and toggle functionality"

# User dashboard
node system.js "Create a dashboard with user profile and settings"

# E-commerce catalog
node system.js "Create a product catalog with grid layout and search"
```

### Running Tests

Test the system with predefined requests:

```bash
# List all tests
node examples/test-requests.js list

# Run a specific test
node examples/test-requests.js 0

# Run all tests
node examples/test-requests.js all
```

## Project Structure

```
nextjs-agent-system/
├── agents/
│   ├── coordinator/           # GPT-4 request analyzer
│   │   ├── coordinator-agent.js
│   │   └── prompts.js
│   ├── generator/             # Claude code generator
│   │   ├── code-generator-agent.js
│   │   ├── rag-system.js     # Vector database for docs
│   │   └── prompts.js
│   └── validator/             # Code validator
│       ├── validator-agent.js
│       └── eslint.config.js
├── config/
│   ├── llm-config.js          # API keys and model settings
│   └── system-config.js       # System configuration
├── knowledge-base/
│   └── nextjs-docs/           # Next.js documentation
│       ├── app-router.txt
│       ├── server-components.txt
│       ├── api-routes.txt
│       ├── data-fetching.txt
│       └── styling.txt
├── utils/
│   ├── logger.js              # Colored console logging
│   ├── cost-tracker.js        # API cost tracking
│   └── file-manager.js        # File system operations
├── output/                    # Generated projects
├── examples/
│   └── test-requests.js       # Test suite
└── system.js                  # Main orchestrator
```

## How It Works

### 1. Coordinator Agent (GPT-4)

Analyzes the user's request and creates a structured execution plan:

- Breaks down complex requests into tasks
- Identifies required files and components
- Determines frontend vs backend needs
- Lists necessary npm dependencies
- Provides best practice recommendations

**Output**: JSON execution plan with tasks, files, and dependencies

### 2. RAG System (ChromaDB + OpenAI Embeddings)

Retrieves relevant Next.js documentation:

- Creates vector embeddings of Next.js docs
- Performs semantic search based on task requirements
- Provides context to the Code Generator
- Improves code quality and accuracy

### 3. Code Generator Agent (Claude Sonnet)

Generates production-ready code:

- Uses RAG context for accurate Next.js patterns
- Generates complete TypeScript files
- Follows Next.js 14 App Router conventions
- Uses Tailwind CSS for styling
- Includes proper error handling
- Adds Server/Client component directives

**Output**: Complete code files with descriptions

### 4. File Manager

Creates the project structure:

- Sets up Next.js project boilerplate
- Writes all generated files to disk
- Creates package.json with dependencies
- Adds configuration files (tsconfig, tailwind, etc.)
- Creates root layout and global styles

### 5. Validator Agent

Validates the generated code:

- **Syntax validation**: Uses ESLint to check for errors
- **Structure validation**: Ensures required files exist
- **Import validation**: Checks for missing imports
- Reports issues with file and line numbers

## Generated Project Structure

Each generated project follows Next.js 14 conventions:

```
output/your-project/
├── app/
│   ├── layout.tsx          # Root layout
│   ├── page.tsx            # Homepage
│   ├── globals.css         # Global styles
│   └── api/                # API routes (if needed)
├── components/             # React components
├── package.json
├── tsconfig.json
├── tailwind.config.js
├── postcss.config.js
├── next.config.js
└── .gitignore
```

## Running Generated Projects

After generation, run your project:

```bash
cd output/your-project-name
npm install
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) in your browser.

## Cost Tracking

The system tracks all API costs:

- GPT-4 Turbo: $0.01/1K input, $0.03/1K output
- Claude Sonnet 4.5: $0.003/1K input, $0.015/1K output
- OpenAI Embeddings: $0.00002/1K tokens

Costs are displayed after each request and saved to `cost-history.json`.

**Typical costs per project:**
- Simple project: $0.02 - $0.05
- Medium project: $0.05 - $0.15
- Complex project: $0.15 - $0.30

## Configuration

### LLM Settings

Edit `config/llm-config.js` to customize:

```javascript
export const LLM_CONFIG = {
  openai: {
    model: 'gpt-4-turbo-preview',
    temperature: 0.3,
    maxTokens: 2000
  },
  anthropic: {
    model: 'claude-sonnet-4-5-20250929',
    temperature: 0.3,
    maxTokens: 4000
  }
};
```

### System Settings

Edit `config/system-config.js`:

```javascript
export const SYSTEM_CONFIG = {
  outputDir: './output',
  maxRetries: 3,
  validateBeforeWrite: true,
  saveHistory: true
};
```

## Troubleshooting

### API Key Errors

```
Error: Invalid API key
```

**Solution**: Check your `.env` file has valid API keys.

### ChromaDB Connection Issues

```
Error: Failed to connect to ChromaDB
```

**Solution**: Ensure ChromaDB is installed: `npm install chromadb`

### Rate Limiting

```
Error: Rate limit exceeded
```

**Solution**: Wait a few minutes between requests or upgrade your API plan.

### Project Already Exists

```
Error: Project already exists
```

**Solution**: Delete the existing project in `output/` or choose a different request.

## Advanced Usage

### Custom Knowledge Base

Add your own documentation to `knowledge-base/nextjs-docs/`:

```bash
echo "Your custom docs" > knowledge-base/nextjs-docs/custom.txt
```

The RAG system will automatically index new files.

### Programmatic Usage

Use the system in your own scripts:

```javascript
import MultiAgentSystem from './system.js';

const system = new MultiAgentSystem();
const result = await system.run('Create a blog homepage');

console.log(result.success);
console.log(result.projectPath);
```

### Adding Custom Agents

Extend the system by adding new agents in `agents/`:

1. Create agent class
2. Implement required methods
3. Add to system orchestrator

## Limitations

- Generates frontend and simple backend only
- No database integration (can add with custom requests)
- No authentication setup (can be requested)
- Limited to Next.js 14 patterns
- Requires internet connection for API calls

## Best Practices

1. **Be specific**: "Create a blog with posts and categories" is better than "Create a blog"
2. **Include details**: Mention specific features, styling preferences, or components
3. **One feature at a time**: Start simple, then iterate
4. **Review generated code**: The system is powerful but should be reviewed
5. **Test locally**: Always run and test generated projects

## Examples

### Simple Blog

```bash
node system.js "Create a blog homepage with a hero section and a grid of 6 blog post cards"
```

**Generated:**
- Homepage with responsive grid
- Blog post card component
- Tailwind styling
- TypeScript types

### Contact Form with API

```bash
node system.js "Create a contact form with name, email, and message fields, and an API endpoint to handle submissions"
```

**Generated:**
- Contact form component (Client Component)
- Form validation
- API route for POST requests
- Success/error states

### Dashboard

```bash
node system.js "Create a dashboard with a sidebar navigation and pages for profile and settings"
```

**Generated:**
- Layout with sidebar
- Multiple pages (routes)
- Navigation component
- Responsive design

## Contributing

This is a research project. Feel free to:

- Report bugs
- Suggest features
- Improve prompts
- Add documentation

## License

MIT License - feel free to use in your projects!

## Acknowledgments

- Built with OpenAI GPT-4, Anthropic Claude, and ChromaDB
- Inspired by Next.js 14 and modern AI agent patterns
- Uses Next.js, React, TypeScript, and Tailwind CSS

## Support

For issues or questions:
1. Check the troubleshooting section
2. Review example requests
3. Ensure API keys are valid
4. Check API rate limits

---

**Built with AI, for AI-assisted development.**

Generate complete Next.js applications with natural language!
