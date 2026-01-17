import { ChromaClient } from 'chromadb';
import OpenAI from 'openai';
import fs from 'fs/promises';
import path from 'path';
import { LLM_CONFIG, PRICING } from '../../config/llm-config.js';
import { SYSTEM_CONFIG } from '../../config/system-config.js';
import logger from '../../utils/logger.js';
import costTracker from '../../utils/cost-tracker.js';

class RAGSystem {
  constructor() {
    this.client = null;
    this.collection = null;
    this.openai = new OpenAI({ apiKey: LLM_CONFIG.openai.apiKey });
    this.collectionName = 'nextjs_docs';
    this.initialized = false;
  }

  async initialize() {
    if (this.initialized) {
      return;
    }

    try {
      logger.info('RAG', 'Initializing ChromaDB...');

      // Initialize ChromaDB client
      this.client = new ChromaClient();

      // Try to get existing collection or create new one
      try {
        this.collection = await this.client.getCollection({
          name: this.collectionName
        });
        logger.success('RAG', 'Loaded existing vector database');
      } catch (error) {
        // Collection doesn't exist, create it
        this.collection = await this.client.createCollection({
          name: this.collectionName,
          metadata: { description: 'Next.js 14 documentation' }
        });
        logger.success('RAG', 'Created new vector database');

        // Load documentation
        await this.loadDocumentation();
      }

      this.initialized = true;
      logger.success('RAG', 'RAG system initialized');
    } catch (error) {
      logger.error('RAG', `Failed to initialize: ${error.message}`);
      logger.info('RAG', 'Continuing without RAG - code generation will work without documentation context');
      this.initialized = false;
      // Don't throw error - allow system to continue without RAG
    }
  }

  async loadDocumentation() {
    try {
      const docsDir = path.join(SYSTEM_CONFIG.knowledgeBaseDir, 'nextjs-docs');
      const files = await fs.readdir(docsDir);

      const documents = [];
      const embeddings = [];
      const ids = [];
      const metadatas = [];

      logger.info('RAG', `Loading ${files.length} documentation files...`);

      for (const file of files) {
        if (!file.endsWith('.txt')) continue;

        const filePath = path.join(docsDir, file);
        const content = await fs.readFile(filePath, 'utf-8');

        // Split into chunks if content is too large
        const chunks = this.splitIntoChunks(content, 1000);

        for (let i = 0; i < chunks.length; i++) {
          const chunk = chunks[i];
          const docId = `${file.replace('.txt', '')}_${i}`;

          documents.push(chunk);
          ids.push(docId);
          metadatas.push({
            source: file,
            chunk: i,
            total_chunks: chunks.length
          });

          // Create embedding
          const embedding = await this.createEmbedding(chunk);
          embeddings.push(embedding);
        }
      }

      // Add to collection
      await this.collection.add({
        ids: ids,
        embeddings: embeddings,
        documents: documents,
        metadatas: metadatas
      });

      logger.success('RAG', `Loaded ${documents.length} document chunks`);
    } catch (error) {
      logger.error('RAG', `Failed to load documentation: ${error.message}`);
      throw error;
    }
  }

  splitIntoChunks(text, maxLength) {
    const chunks = [];
    const paragraphs = text.split('\n\n');
    let currentChunk = '';

    for (const paragraph of paragraphs) {
      if (currentChunk.length + paragraph.length > maxLength && currentChunk.length > 0) {
        chunks.push(currentChunk.trim());
        currentChunk = paragraph;
      } else {
        currentChunk += (currentChunk ? '\n\n' : '') + paragraph;
      }
    }

    if (currentChunk) {
      chunks.push(currentChunk.trim());
    }

    return chunks;
  }

  async createEmbedding(text) {
    try {
      const response = await this.openai.embeddings.create({
        model: LLM_CONFIG.embedding.model,
        input: text,
        dimensions: LLM_CONFIG.embedding.dimensions
      });

      // Track cost
      const tokens = response.usage.total_tokens;
      const cost = costTracker.trackEmbedding(tokens);

      return response.data[0].embedding;
    } catch (error) {
      logger.error('RAG', `Failed to create embedding: ${error.message}`);
      throw error;
    }
  }

  async retrieve(query, topK = 3) {
    try {
      if (!this.initialized) {
        await this.initialize();
      }

      // If still not initialized after trying, return empty results
      if (!this.initialized || !this.collection) {
        logger.info('RAG', 'Skipping document retrieval - RAG not available');
        return [];
      }

      logger.thinking('RAG', `Searching for relevant docs: "${query.substring(0, 50)}..."`);

      // Create query embedding
      const queryEmbedding = await this.createEmbedding(query);

      // Query collection
      const results = await this.collection.query({
        queryEmbeddings: [queryEmbedding],
        nResults: topK
      });

      const documents = results.documents[0].map((doc, idx) => ({
        content: doc,
        source: results.metadatas[0][idx].source,
        distance: results.distances[0][idx]
      }));

      logger.success('RAG', `Retrieved ${documents.length} relevant documents`);

      return documents;
    } catch (error) {
      logger.error('RAG', `Failed to retrieve documents: ${error.message}`);
      return [];
    }
  }

  async addDocuments(docs) {
    try {
      if (!this.initialized) {
        await this.initialize();
      }

      const documents = [];
      const embeddings = [];
      const ids = [];
      const metadatas = [];

      for (let i = 0; i < docs.length; i++) {
        const doc = docs[i];
        const docId = `custom_${Date.now()}_${i}`;

        documents.push(doc.content);
        ids.push(docId);
        metadatas.push(doc.metadata || { source: 'custom' });

        const embedding = await this.createEmbedding(doc.content);
        embeddings.push(embedding);
      }

      await this.collection.add({
        ids: ids,
        embeddings: embeddings,
        documents: documents,
        metadatas: metadatas
      });

      logger.success('RAG', `Added ${documents.length} custom documents`);
    } catch (error) {
      logger.error('RAG', `Failed to add documents: ${error.message}`);
      throw error;
    }
  }

  async clear() {
    try {
      if (this.client && this.collection) {
        await this.client.deleteCollection({ name: this.collectionName });
        logger.success('RAG', 'Cleared vector database');
        this.initialized = false;
        this.collection = null;
      }
    } catch (error) {
      logger.error('RAG', `Failed to clear database: ${error.message}`);
      throw error;
    }
  }

  formatContextForPrompt(documents) {
    return documents.map((doc, idx) =>
      `[Document ${idx + 1} - ${doc.source}]\n${doc.content}`
    ).join('\n\n---\n\n');
  }
}

export default new RAGSystem();
