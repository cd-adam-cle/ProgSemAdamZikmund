# Quick Start Guide

Get your Next.js Multi-Agent System running in 5 minutes!

## Step 1: Install Dependencies

```bash
npm install
```

This will install:
- `openai` - GPT-4 API client
- `@anthropic-ai/sdk` - Claude API client
- `chromadb` - Vector database for RAG
- `chalk` - Colored terminal output
- `dotenv` - Environment variables
- `eslint` - Code validation

## Step 2: Configure API Keys

Create a `.env` file in the root directory:

```bash
cp .env.example .env
```

Edit `.env` and add your API keys:

```
OPENAI_API_KEY=sk-proj-...
ANTHROPIC_API_KEY=sk-ant-...
```

### Get API Keys

**OpenAI (GPT-4 + Embeddings):**
1. Go to [platform.openai.com](https://platform.openai.com)
2. Sign up or log in
3. Go to API Keys section
4. Create new secret key
5. Copy and paste into `.env`

**Anthropic (Claude):**
1. Go to [console.anthropic.com](https://console.anthropic.com)
2. Sign up or log in
3. Go to API Keys
4. Create new key
5. Copy and paste into `.env`

## Step 3: Test the System

Run a simple test request:

```bash
node system.js "Create a simple homepage with a hero section"
```

You should see:
1. Coordinator analyzing the request
2. Code Generator creating files
3. Validator checking the code
4. Success report with project location

## Step 4: Check the Output

```bash
cd output/create-a-simple-homepage-with-a-hero-section
ls -la
```

You should see:
- `app/` directory with page.tsx and layout.tsx
- `components/` (if any were generated)
- `package.json`
- `tsconfig.json`
- `tailwind.config.js`
- Other Next.js config files

## Step 5: Run Your Generated Project

```bash
npm install
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) in your browser!

## Example Requests to Try

### Beginner
```bash
node system.js "Create a simple about page"
```

### Intermediate
```bash
node system.js "Create a blog homepage with 6 post cards in a grid"
```

### Advanced
```bash
node system.js "Create a contact form with name, email, and message fields, plus an API endpoint"
```

## Understanding the Output

When you run a request, you'll see:

```
🚀 Multi-Agent System Starting...

[10:30:15] 🤔 [Coordinator] Analyzing user request...
[10:30:18] ✅ [Coordinator] Analysis complete: 2 tasks identified
[10:30:18] 💰 [Coordinator] Cost: $0.0089

[10:30:18] 🤔 [CodeGenerator] Generating code for: Homepage
[10:30:18] 📚 [RAG] Retrieved 3 relevant docs
[10:30:25] ✅ [CodeGenerator] Created: app/page.tsx
[10:30:25] 💰 [CodeGenerator] Cost: $0.0342

[10:30:25] 🔍 [Validator] Validating generated code...
[10:30:27] ✅ [Validator] Syntax: PASSED
[10:30:27] ✅ [Validator] Structure: PASSED

============================================================
  📊 EXECUTION REPORT
============================================================
✅ Status: SUCCESS
📁 Project: output/create-a-simple-homepage/
📄 Files: 2 created
💰 Total Cost: $0.0431
⏱️  Time: 12.3s
============================================================
```

## Troubleshooting

### "Invalid API Key" Error

**Problem:** API key is missing or invalid

**Solution:**
1. Check `.env` file exists
2. Verify API keys are correct
3. Ensure no extra spaces or quotes
4. Check API key permissions (need GPT-4 access for OpenAI)

### "Rate Limit Exceeded"

**Problem:** Too many API requests

**Solution:**
1. Wait 60 seconds
2. Check your API usage limits
3. Upgrade your API plan if needed

### "ChromaDB Connection Error"

**Problem:** ChromaDB not installed properly

**Solution:**
```bash
npm install chromadb --save
```

### "Project Already Exists"

**Problem:** A project with the same name was already generated

**Solution:**
1. Delete the existing project: `rm -rf output/project-name`
2. Or modify your request to generate a different name

### ESLint Errors

**Problem:** Generated code has syntax errors

**Solution:**
1. Check the validation report
2. The system should retry automatically
3. If persists, try simplifying your request

## Tips for Best Results

### Be Specific
❌ Bad: "Create a website"
✅ Good: "Create a homepage with a hero section, feature cards, and footer"

### Include Details
❌ Bad: "Create a form"
✅ Good: "Create a contact form with name, email, and message fields with validation"

### One Feature at a Time
❌ Bad: "Create a full e-commerce site with cart, checkout, and admin"
✅ Good: "Create a product catalog page with grid layout"

### Mention Styling Preferences
❌ Bad: "Create a button"
✅ Good: "Create a blue primary button with hover effects"

## Cost Management

Typical costs per request:

| Complexity | Files | Cost Range |
|------------|-------|------------|
| Simple     | 1-3   | $0.02-$0.05 |
| Medium     | 4-8   | $0.05-$0.15 |
| Complex    | 9+    | $0.15-$0.30 |

View cost history:
```bash
cat cost-history.json
```

## Next Steps

1. **Explore Examples:**
   ```bash
   node examples/test-requests.js list
   node examples/test-requests.js 0
   ```

2. **Read Full Documentation:**
   - See `README.md` for architecture details
   - Check `knowledge-base/nextjs-docs/` for available patterns

3. **Customize the System:**
   - Edit `config/llm-config.js` for model settings
   - Add docs to `knowledge-base/nextjs-docs/`
   - Modify prompts in `agents/*/prompts.js`

4. **Build Real Projects:**
   - Start with simple components
   - Iterate and refine
   - Review and customize generated code
   - Deploy to Vercel or other platforms

## Common Workflows

### Building a Blog

```bash
# Step 1: Homepage
node system.js "Create a blog homepage with post grid"

# Step 2: Post page
node system.js "Create a blog post page with title, content, and author"

# Step 3: API
node system.js "Create an API endpoint to fetch blog posts"
```

### Building a Landing Page

```bash
node system.js "Create a landing page with hero section, features grid, pricing table, and contact form"
```

### Building a Dashboard

```bash
node system.js "Create a dashboard layout with sidebar navigation and stats cards"
```

## Support

If you run into issues:

1. Check this guide
2. Read the error messages carefully
3. Review `README.md` troubleshooting section
4. Ensure API keys have proper permissions
5. Check your API rate limits and billing

## Success Checklist

- ✅ Dependencies installed (`npm install`)
- ✅ `.env` file created with valid API keys
- ✅ Test request runs successfully
- ✅ Generated project in `output/` directory
- ✅ Can run `npm install && npm run dev` in generated project
- ✅ Can open http://localhost:3000 and see the result

You're ready to generate Next.js applications with AI! 🚀

---

**Have fun building!**
