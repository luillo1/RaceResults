import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  if (mode === "development") {
    return {
      plugins: [react()],
      server: {
        proxy: {
          "/api": {
            target: "https://localhost:5001",
            changeOrigin: true,
            secure: false,
            rewrite: (path) => path.replace(/^\/api/, "")
          }
        }
      }
    };
  } else {
    return {
      plugins: [react()],
      build: {
        outDir: "./build"
      }
    };
  }
});
