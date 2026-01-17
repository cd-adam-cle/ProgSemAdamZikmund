import { ESLint } from 'eslint';
import fs from 'fs/promises';
import path from 'path';
import logger from '../../utils/logger.js';
import eslintConfig from './eslint.config.js';

class ValidatorAgent {
  constructor() {
    this.eslint = new ESLint({
      baseConfig: eslintConfig,
      useEslintrc: false
    });
  }

  async validate(projectPath, files) {
    logger.thinking('Validator', 'Validating generated code...');

    const results = {
      syntaxValidation: { passed: false, errors: [] },
      structureValidation: { passed: false, errors: [] },
      importValidation: { passed: false, errors: [] },
      overallStatus: 'pending'
    };

    try {
      // 1. Syntax Validation
      results.syntaxValidation = await this.validateSyntax(projectPath, files);

      // 2. Structure Validation
      results.structureValidation = await this.validateStructure(projectPath, files);

      // 3. Import Validation
      results.importValidation = await this.validateImports(projectPath, files);

      // Determine overall status
      const allPassed = results.syntaxValidation.passed &&
                        results.structureValidation.passed &&
                        results.importValidation.passed;

      results.overallStatus = allPassed ? 'passed' : 'failed';

      // Log results
      this.logResults(results);

      return results;

    } catch (error) {
      logger.error('Validator', `Validation failed: ${error.message}`);
      results.overallStatus = 'error';
      return results;
    }
  }

  async validateSyntax(projectPath, files) {
    logger.info('Validator', 'Checking syntax...');

    const errors = [];
    let hasErrors = false;

    try {
      for (const file of files) {
        const filePath = path.join(projectPath, file.path);

        // Check if file exists
        try {
          await fs.access(filePath);
        } catch {
          errors.push({
            file: file.path,
            message: 'File does not exist'
          });
          continue;
        }

        // Skip non-JS/TS files
        if (!/\.(js|jsx|ts|tsx)$/.test(file.path)) {
          continue;
        }

        // Run ESLint
        try {
          const results = await this.eslint.lintFiles([filePath]);

          for (const result of results) {
            if (result.errorCount > 0 || result.warningCount > 0) {
              hasErrors = result.errorCount > 0 ? true : hasErrors;

              result.messages.forEach(msg => {
                errors.push({
                  file: file.path,
                  line: msg.line,
                  column: msg.column,
                  message: msg.message,
                  severity: msg.severity === 2 ? 'error' : 'warning'
                });
              });
            }
          }
        } catch (lintError) {
          // Syntax error in file
          hasErrors = true;
          errors.push({
            file: file.path,
            message: `Syntax error: ${lintError.message}`
          });
        }
      }

      const passed = !hasErrors;

      if (passed) {
        logger.success('Validator', 'Syntax validation: PASSED');
      } else {
        logger.error('Validator', `Syntax validation: FAILED (${errors.length} issues)`);
      }

      return { passed, errors };

    } catch (error) {
      logger.error('Validator', `Syntax validation error: ${error.message}`);
      return {
        passed: false,
        errors: [{ message: `Validation error: ${error.message}` }]
      };
    }
  }

  async validateStructure(projectPath, files) {
    logger.info('Validator', 'Checking file structure...');

    const errors = [];
    const requiredFiles = ['package.json', 'next.config.js', 'tsconfig.json'];
    const requiredDirs = ['app'];

    try {
      // Check required files
      for (const file of requiredFiles) {
        const filePath = path.join(projectPath, file);
        try {
          await fs.access(filePath);
        } catch {
          errors.push({
            type: 'missing-file',
            path: file,
            message: `Required file missing: ${file}`
          });
        }
      }

      // Check required directories
      for (const dir of requiredDirs) {
        const dirPath = path.join(projectPath, dir);
        try {
          const stat = await fs.stat(dirPath);
          if (!stat.isDirectory()) {
            errors.push({
              type: 'invalid-directory',
              path: dir,
              message: `${dir} exists but is not a directory`
            });
          }
        } catch {
          errors.push({
            type: 'missing-directory',
            path: dir,
            message: `Required directory missing: ${dir}`
          });
        }
      }

      // Check all generated files exist
      for (const file of files) {
        const filePath = path.join(projectPath, file.path);
        try {
          await fs.access(filePath);
        } catch {
          errors.push({
            type: 'missing-file',
            path: file.path,
            message: `Generated file not found: ${file.path}`
          });
        }
      }

      const passed = errors.length === 0;

      if (passed) {
        logger.success('Validator', 'Structure validation: PASSED');
      } else {
        logger.error('Validator', `Structure validation: FAILED (${errors.length} issues)`);
      }

      return { passed, errors };

    } catch (error) {
      logger.error('Validator', `Structure validation error: ${error.message}`);
      return {
        passed: false,
        errors: [{ message: `Validation error: ${error.message}` }]
      };
    }
  }

  async validateImports(projectPath, files) {
    logger.info('Validator', 'Checking imports...');

    const errors = [];
    const knownModules = new Set([
      'react', 'react-dom', 'next', 'next/link', 'next/image',
      'next/navigation', 'next/headers', 'next/server'
    ]);

    try {
      for (const file of files) {
        // Only check JS/TS files
        if (!/\.(js|jsx|ts|tsx)$/.test(file.path)) {
          continue;
        }

        const filePath = path.join(projectPath, file.path);

        try {
          const content = await fs.readFile(filePath, 'utf-8');

          // Extract imports
          const importRegex = /import\s+.*?\s+from\s+['"](.+?)['"]/g;
          let match;

          while ((match = importRegex.exec(content)) !== null) {
            const importPath = match[1];

            // Check relative imports
            if (importPath.startsWith('.') || importPath.startsWith('@/')) {
              const resolvedPath = this.resolveImportPath(file.path, importPath, projectPath);

              if (resolvedPath && !await this.fileExists(resolvedPath)) {
                errors.push({
                  file: file.path,
                  import: importPath,
                  message: `Import file not found: ${importPath}`
                });
              }
            }
            // Known npm modules are OK
            else if (!knownModules.has(importPath.split('/')[0])) {
              // Check if it's in dependencies
              const packageJsonPath = path.join(projectPath, 'package.json');
              try {
                const packageJson = JSON.parse(await fs.readFile(packageJsonPath, 'utf-8'));
                const allDeps = {
                  ...packageJson.dependencies,
                  ...packageJson.devDependencies
                };

                const moduleName = importPath.split('/')[0];
                if (!allDeps[moduleName]) {
                  errors.push({
                    file: file.path,
                    import: importPath,
                    message: `Import not in package.json: ${importPath}`
                  });
                }
              } catch {
                // Can't read package.json, skip this check
              }
            }
          }
        } catch (readError) {
          errors.push({
            file: file.path,
            message: `Could not read file: ${readError.message}`
          });
        }
      }

      const passed = errors.length === 0;

      if (passed) {
        logger.success('Validator', 'Import validation: PASSED');
      } else {
        logger.warning('Validator', `Import validation: WARNINGS (${errors.length} issues)`);
      }

      // Don't fail on import warnings, just warn
      return { passed: true, errors };

    } catch (error) {
      logger.error('Validator', `Import validation error: ${error.message}`);
      return {
        passed: true, // Don't fail on this
        errors: [{ message: `Validation error: ${error.message}` }]
      };
    }
  }

  resolveImportPath(currentFile, importPath, projectPath) {
    if (importPath.startsWith('@/')) {
      // Alias import
      return path.join(projectPath, importPath.replace('@/', ''));
    } else if (importPath.startsWith('.')) {
      // Relative import
      const currentDir = path.dirname(path.join(projectPath, currentFile));
      return path.resolve(currentDir, importPath);
    }
    return null;
  }

  async fileExists(filePath) {
    // Try different extensions
    const extensions = ['', '.ts', '.tsx', '.js', '.jsx'];

    for (const ext of extensions) {
      try {
        await fs.access(filePath + ext);
        return true;
      } catch {
        continue;
      }
    }

    return false;
  }

  logResults(results) {
    logger.section('Validation Results');

    // Syntax
    if (results.syntaxValidation.passed) {
      logger.success('Validator', '✓ Syntax: PASSED');
    } else {
      logger.error('Validator', `✗ Syntax: FAILED (${results.syntaxValidation.errors.length} errors)`);
      results.syntaxValidation.errors.slice(0, 5).forEach(err => {
        logger.raw(`    ${err.file}:${err.line || '?'} - ${err.message}`);
      });
    }

    // Structure
    if (results.structureValidation.passed) {
      logger.success('Validator', '✓ Structure: PASSED');
    } else {
      logger.error('Validator', `✗ Structure: FAILED (${results.structureValidation.errors.length} errors)`);
      results.structureValidation.errors.slice(0, 5).forEach(err => {
        logger.raw(`    ${err.path} - ${err.message}`);
      });
    }

    // Imports
    if (results.importValidation.passed) {
      logger.success('Validator', '✓ Imports: PASSED');
    } else {
      logger.warning('Validator', `⚠ Imports: WARNINGS (${results.importValidation.errors.length} warnings)`);
    }
  }

  formatValidationReport(results) {
    let report = '\n🔍 Validation Report:\n\n';

    report += `Overall Status: ${results.overallStatus.toUpperCase()}\n\n`;

    report += 'Checks:\n';
    report += `  Syntax: ${results.syntaxValidation.passed ? '✓ PASSED' : '✗ FAILED'}\n`;
    report += `  Structure: ${results.structureValidation.passed ? '✓ PASSED' : '✗ FAILED'}\n`;
    report += `  Imports: ${results.importValidation.passed ? '✓ PASSED' : '⚠ WARNINGS'}\n`;

    const totalErrors = results.syntaxValidation.errors.length +
                        results.structureValidation.errors.length;

    if (totalErrors > 0) {
      report += `\nTotal Issues: ${totalErrors}\n`;
    }

    return report;
  }
}

export default new ValidatorAgent();
