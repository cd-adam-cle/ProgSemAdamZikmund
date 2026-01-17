import chalk from 'chalk';
import fs from 'fs/promises';
import path from 'path';

class Logger {
  constructor(enableFileLogging = false) {
    this.enableFileLogging = enableFileLogging;
    this.logFile = 'system.log';
  }

  getTimestamp() {
    const now = new Date();
    return now.toLocaleTimeString('en-US', { hour12: false });
  }

  async writeToFile(message) {
    if (this.enableFileLogging) {
      try {
        await fs.appendFile(this.logFile, `${message}\n`);
      } catch (error) {
        // Silently fail if file logging fails
      }
    }
  }

  info(agent, message) {
    const timestamp = this.getTimestamp();
    const output = `[${timestamp}] ℹ️  [${agent}] ${message}`;
    console.log(chalk.blue(output));
    this.writeToFile(output);
  }

  success(agent, message) {
    const timestamp = this.getTimestamp();
    const output = `[${timestamp}] ✅ [${agent}] ${message}`;
    console.log(chalk.green(output));
    this.writeToFile(output);
  }

  error(agent, message) {
    const timestamp = this.getTimestamp();
    const output = `[${timestamp}] ❌ [${agent}] ${message}`;
    console.log(chalk.red(output));
    this.writeToFile(output);
  }

  warning(agent, message) {
    const timestamp = this.getTimestamp();
    const output = `[${timestamp}] ⚠️  [${agent}] ${message}`;
    console.log(chalk.yellow(output));
    this.writeToFile(output);
  }

  thinking(agent, message) {
    const timestamp = this.getTimestamp();
    const output = `[${timestamp}] 🤔 [${agent}] ${message}`;
    console.log(chalk.cyan(output));
    this.writeToFile(output);
  }

  cost(agent, amount) {
    const timestamp = this.getTimestamp();
    const output = `[${timestamp}] 💰 [${agent}] Cost: $${amount.toFixed(4)}`;
    console.log(chalk.magenta(output));
    this.writeToFile(output);
  }

  header(title) {
    const line = '='.repeat(60);
    console.log(chalk.bold.cyan(`\n${line}`));
    console.log(chalk.bold.cyan(`  ${title}`));
    console.log(chalk.bold.cyan(`${line}\n`));
    this.writeToFile(`\n${line}\n  ${title}\n${line}\n`);
  }

  section(title) {
    console.log(chalk.bold.white(`\n${title}`));
    this.writeToFile(`\n${title}`);
  }

  raw(message) {
    console.log(message);
    this.writeToFile(message);
  }
}

export default new Logger();
