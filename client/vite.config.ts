import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

import path from "path";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  if (mode === "development") {
    return {
      plugins: [react()],
      alias: {
        "../../theme.config": path.join(
          __dirname,
          "./src/semantic-ui/theme.config"
        ),
      },
      server: {
        proxy: {
          "/api": {
            target: "https://localhost:5001",
            changeOrigin: true,
            secure: false,
            rewrite: (path) => path.replace(/^\/api/, ""),
          },
        },
      },
    };
  } else {
    return {
      plugins: [react()],
      build: {
        outDir: "./build",
      },
    };
  }
});
