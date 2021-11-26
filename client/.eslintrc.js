module.exports = {
  env: {
    browser: true,
    commonjs: true,
    es2021: true
  },
  extends: ["plugin:react/recommended", "standard"],
  parser: "@typescript-eslint/parser",
  parserOptions: {
    ecmaFeatures: {
      jsx: true
    },
    ecmaVersion: 13
  },
  plugins: ["react", "@typescript-eslint"],
  rules: {
    quotes: [2, "double"],
    "space-before-function-paren": ["error", "never"],
    semi: ["warn", "always"],
    "no-use-before-define": "off",
    "@typescript-eslint/no-use-before-define": ["error"]
  }
};
