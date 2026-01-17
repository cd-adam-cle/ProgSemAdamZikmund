export default {
  parser: '@typescript-eslint/parser',
  parserOptions: {
    ecmaVersion: 2021,
    sourceType: 'module',
    ecmaFeatures: {
      jsx: true
    }
  },
  env: {
    browser: true,
    es2021: true,
    node: true
  },
  extends: [
    'eslint:recommended'
  ],
  rules: {
    'no-unused-vars': 'warn',
    'no-console': 'off',
    'no-undef': 'error',
    'no-unreachable': 'error'
  }
};
