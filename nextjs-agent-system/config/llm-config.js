import dotenv from 'dotenv';
dotenv.config();

export const LLM_CONFIG = {
  openai: {
    apiKey: process.env.OPENAI_API_KEY,
    model: 'gpt-4-turbo-preview',
    temperature: 0.3,
    maxTokens: 2000
  },
  anthropic: {
    apiKey: process.env.ANTHROPIC_API_KEY,
    model: 'claude-sonnet-4-5-20250929',
    temperature: 0.3,
    maxTokens: 4000
  },
  embedding: {
    model: 'text-embedding-3-small',
    dimensions: 1536
  }
};

export const PRICING = {
  openai: {
    'gpt-4-turbo-preview': {
      input: 0.01 / 1000,  // $0.01 per 1K tokens
      output: 0.03 / 1000   // $0.03 per 1K tokens
    },
    'text-embedding-3-small': {
      input: 0.00002 / 1000  // $0.00002 per 1K tokens
    }
  },
  anthropic: {
    'claude-sonnet-4-5-20250929': {
      input: 0.003 / 1000,   // $0.003 per 1K tokens
      output: 0.015 / 1000   // $0.015 per 1K tokens
    }
  }
};
