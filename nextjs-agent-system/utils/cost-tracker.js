import fs from 'fs/promises';
import { PRICING } from '../config/llm-config.js';

class CostTracker {
  constructor() {
    this.costs = {
      coordinator: 0,
      generator: 0,
      embedding: 0,
      total: 0
    };
    this.history = [];
  }

  calculateOpenAICost(model, inputTokens, outputTokens) {
    const pricing = PRICING.openai[model];
    if (!pricing) {
      throw new Error(`Unknown OpenAI model: ${model}`);
    }
    return (inputTokens * pricing.input) + (outputTokens * pricing.output);
  }

  calculateAnthropicCost(model, inputTokens, outputTokens) {
    const pricing = PRICING.anthropic[model];
    if (!pricing) {
      throw new Error(`Unknown Anthropic model: ${model}`);
    }
    return (inputTokens * pricing.input) + (outputTokens * pricing.output);
  }

  calculateEmbeddingCost(model, tokens) {
    const pricing = PRICING.openai[model];
    if (!pricing) {
      throw new Error(`Unknown embedding model: ${model}`);
    }
    return tokens * pricing.input;
  }

  trackCoordinator(inputTokens, outputTokens) {
    const cost = this.calculateOpenAICost('gpt-4-turbo-preview', inputTokens, outputTokens);
    this.costs.coordinator += cost;
    this.costs.total += cost;
    return cost;
  }

  trackGenerator(inputTokens, outputTokens) {
    const cost = this.calculateAnthropicCost('claude-sonnet-4-5-20250929', inputTokens, outputTokens);
    this.costs.generator += cost;
    this.costs.total += cost;
    return cost;
  }

  trackEmbedding(tokens) {
    const cost = this.calculateEmbeddingCost('text-embedding-3-small', tokens);
    this.costs.embedding += cost;
    this.costs.total += cost;
    return cost;
  }

  getCosts() {
    return { ...this.costs };
  }

  reset() {
    this.costs = {
      coordinator: 0,
      generator: 0,
      embedding: 0,
      total: 0
    };
  }

  async saveHistory(requestInfo) {
    const entry = {
      timestamp: new Date().toISOString(),
      request: requestInfo.request,
      costs: { ...this.costs },
      success: requestInfo.success,
      filesCreated: requestInfo.filesCreated
    };

    this.history.push(entry);

    try {
      await fs.writeFile('cost-history.json', JSON.stringify(this.history, null, 2));
    } catch (error) {
      console.error('Failed to save cost history:', error.message);
    }
  }

  async loadHistory() {
    try {
      const data = await fs.readFile('cost-history.json', 'utf-8');
      this.history = JSON.parse(data);
    } catch (error) {
      // File doesn't exist or is invalid, start with empty history
      this.history = [];
    }
  }

  printSummary() {
    console.log('\n💰 Cost Breakdown:');
    console.log(`   Coordinator (GPT-4):     $${this.costs.coordinator.toFixed(4)}`);
    console.log(`   Generator (Claude):      $${this.costs.generator.toFixed(4)}`);
    console.log(`   Embeddings (OpenAI):     $${this.costs.embedding.toFixed(4)}`);
    console.log(`   Total:                   $${this.costs.total.toFixed(4)}`);
  }
}

export default new CostTracker();
