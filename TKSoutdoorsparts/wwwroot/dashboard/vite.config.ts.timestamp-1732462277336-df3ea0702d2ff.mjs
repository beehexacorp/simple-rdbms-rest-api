// vite.config.ts
import { fileURLToPath, URL } from "node:url";
import { defineConfig } from "file:///config/workspaces/simpleodbcapi/TKSoutdoorsparts/wwwroot/dashboard/node_modules/vite/dist/node/index.js";
import vue from "file:///config/workspaces/simpleodbcapi/TKSoutdoorsparts/wwwroot/dashboard/node_modules/@vitejs/plugin-vue/dist/index.mjs";
import vueJsx from "file:///config/workspaces/simpleodbcapi/TKSoutdoorsparts/wwwroot/dashboard/node_modules/@vitejs/plugin-vue-jsx/dist/index.mjs";
import vueDevTools from "file:///config/workspaces/simpleodbcapi/TKSoutdoorsparts/wwwroot/dashboard/node_modules/vite-plugin-vue-devtools/dist/vite.mjs";
import nightwatchPlugin from "file:///config/workspaces/simpleodbcapi/TKSoutdoorsparts/wwwroot/dashboard/node_modules/vite-plugin-nightwatch/index.js";
import Components from "file:///config/workspaces/simpleodbcapi/TKSoutdoorsparts/wwwroot/dashboard/node_modules/unplugin-vue-components/dist/vite.js";
import tsconfigPaths from "file:///config/workspaces/simpleodbcapi/TKSoutdoorsparts/wwwroot/dashboard/node_modules/vite-tsconfig-paths/dist/index.js";
import svgLoader from "file:///config/workspaces/simpleodbcapi/TKSoutdoorsparts/wwwroot/dashboard/node_modules/vite-svg-loader/index.js";
import { AntDesignVueResolver } from "file:///config/workspaces/simpleodbcapi/TKSoutdoorsparts/wwwroot/dashboard/node_modules/unplugin-vue-components/dist/resolvers.js";
import * as dotenv from "file:///config/workspaces/simpleodbcapi/TKSoutdoorsparts/wwwroot/dashboard/node_modules/dotenv/lib/main.js";
var __vite_injected_original_import_meta_url = "file:///config/workspaces/simpleodbcapi/TKSoutdoorsparts/wwwroot/dashboard/vite.config.ts";
dotenv.config();
console.log("Base Directory", process.env.VITE_BASE || "/dashboard/dist/");
console.log("Port", Number(process.env.VITE_PORT) || 3e3);
console.log(fileURLToPath(new URL("./src", __vite_injected_original_import_meta_url)));
var vite_config_default = defineConfig({
  base: "/dashboard",
  server: {
    port: Number(process.env.VITE_PORT) || 3e3
  },
  plugins: [
    vue(),
    vueJsx(),
    tsconfigPaths(),
    svgLoader(),
    vueDevTools(),
    nightwatchPlugin(),
    Components({
      resolvers: [
        AntDesignVueResolver({
          importStyle: false,
          // css in js
          resolveIcons: true
          // Ensure icons are resolved on-demand
        })
      ]
    })
  ],
  build: {
    rollupOptions: {
      onwarn(warning, warn) {
        if (warning.code === "UNRESOLVED_IMPORT" || warning.message.includes("@microsoft/signalr")) {
          return;
        }
        warn(warning);
      },
      output: {
        manualChunks(id) {
          if (id.includes("node_modules")) {
            if (id.includes("ant-design-vue")) {
              return "ant-design-vue";
            }
            if (id.includes("@vue")) {
              return "vue-core";
            }
            if (id.includes("moment")) {
              return "moment";
            }
            if (id.includes("lodash")) {
              return "lodash";
            }
            if (id.includes("axios")) {
              return "axios";
            }
            const namespaceMatch = id.match(/node_modules\/@([^/]+)\//);
            if (namespaceMatch) {
              return `vendor-${namespaceMatch[1]}`;
            }
            const libraryMatch = id.match(/node_modules\/([^/]+)\//);
            if (libraryMatch) {
              return `vendor-${libraryMatch[1]}`;
            }
            return "vendor";
          }
        },
        entryFileNames: "assets/[name]-[hash].js",
        // File naming format for chunks
        chunkFileNames: "assets/[name]-[hash].js",
        // File naming format for shared chunks
        assetFileNames: "assets/[name]-[hash][extname]"
        // File naming format for static assets
      }
    },
    outDir: "dist",
    // Ensure the output folder matches the base
    assetsDir: "assets",
    sourcemap: process.env.NODE_ENV !== "production"
    // Enable sourcemaps in development
  },
  optimizeDeps: {
    include: ["ant-design-vue"],
    exclude: ["@ant-design/icons-vue"],
    esbuildOptions: {
      target: "esnext",
      splitting: true,
      format: "esm"
    }
  },
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", __vite_injected_original_import_meta_url))
    }
  }
});
export {
  vite_config_default as default
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsidml0ZS5jb25maWcudHMiXSwKICAic291cmNlc0NvbnRlbnQiOiBbImNvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9kaXJuYW1lID0gXCIvY29uZmlnL3dvcmtzcGFjZXMvc2ltcGxlb2RiY2FwaS9US1NvdXRkb29yc3BhcnRzL3d3d3Jvb3QvZGFzaGJvYXJkXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ZpbGVuYW1lID0gXCIvY29uZmlnL3dvcmtzcGFjZXMvc2ltcGxlb2RiY2FwaS9US1NvdXRkb29yc3BhcnRzL3d3d3Jvb3QvZGFzaGJvYXJkL3ZpdGUuY29uZmlnLnRzXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ltcG9ydF9tZXRhX3VybCA9IFwiZmlsZTovLy9jb25maWcvd29ya3NwYWNlcy9zaW1wbGVvZGJjYXBpL1RLU291dGRvb3JzcGFydHMvd3d3cm9vdC9kYXNoYm9hcmQvdml0ZS5jb25maWcudHNcIjtpbXBvcnQgeyBmaWxlVVJMVG9QYXRoLCBVUkwgfSBmcm9tICdub2RlOnVybCdcbmltcG9ydCB7IGRlZmluZUNvbmZpZyB9IGZyb20gJ3ZpdGUnXG5pbXBvcnQgdnVlIGZyb20gJ0B2aXRlanMvcGx1Z2luLXZ1ZSdcbmltcG9ydCB2dWVKc3ggZnJvbSAnQHZpdGVqcy9wbHVnaW4tdnVlLWpzeCdcbmltcG9ydCB2dWVEZXZUb29scyBmcm9tICd2aXRlLXBsdWdpbi12dWUtZGV2dG9vbHMnXG5pbXBvcnQgbmlnaHR3YXRjaFBsdWdpbiBmcm9tICd2aXRlLXBsdWdpbi1uaWdodHdhdGNoJ1xuaW1wb3J0IENvbXBvbmVudHMgZnJvbSAndW5wbHVnaW4tdnVlLWNvbXBvbmVudHMvdml0ZSdcbmltcG9ydCB0c2NvbmZpZ1BhdGhzIGZyb20gJ3ZpdGUtdHNjb25maWctcGF0aHMnXG5pbXBvcnQgc3ZnTG9hZGVyIGZyb20gJ3ZpdGUtc3ZnLWxvYWRlcidcbmltcG9ydCB7IEFudERlc2lnblZ1ZVJlc29sdmVyIH0gZnJvbSAndW5wbHVnaW4tdnVlLWNvbXBvbmVudHMvcmVzb2x2ZXJzJ1xuaW1wb3J0ICogYXMgZG90ZW52IGZyb20gJ2RvdGVudidcblxuLy8gTG9hZCBlbnZpcm9ubWVudCB2YXJpYWJsZXMgbWFudWFsbHlcbmRvdGVudi5jb25maWcoKVxuXG5jb25zb2xlLmxvZygnQmFzZSBEaXJlY3RvcnknLCBwcm9jZXNzLmVudi5WSVRFX0JBU0UgfHwgJy9kYXNoYm9hcmQvZGlzdC8nKVxuY29uc29sZS5sb2coJ1BvcnQnLCBOdW1iZXIocHJvY2Vzcy5lbnYuVklURV9QT1JUKSB8fCAzMDAwKVxuY29uc29sZS5sb2coZmlsZVVSTFRvUGF0aChuZXcgVVJMKCcuL3NyYycsIGltcG9ydC5tZXRhLnVybCkpKVxuXG4vLyBodHRwczovL3ZpdGUuZGV2L2NvbmZpZy9cbmV4cG9ydCBkZWZhdWx0IGRlZmluZUNvbmZpZyh7XG4gIGJhc2U6ICcvZGFzaGJvYXJkJyxcbiAgc2VydmVyOiB7XG4gICAgcG9ydDogTnVtYmVyKHByb2Nlc3MuZW52LlZJVEVfUE9SVCkgfHwgMzAwMCxcbiAgfSxcbiAgcGx1Z2luczogW1xuICAgIHZ1ZSgpLFxuICAgIHZ1ZUpzeCgpLFxuICAgIHRzY29uZmlnUGF0aHMoKSxcbiAgICBzdmdMb2FkZXIoKSxcbiAgICB2dWVEZXZUb29scygpLFxuICAgIG5pZ2h0d2F0Y2hQbHVnaW4oKSxcbiAgICBDb21wb25lbnRzKHtcbiAgICAgIHJlc29sdmVyczogW1xuICAgICAgICBBbnREZXNpZ25WdWVSZXNvbHZlcih7XG4gICAgICAgICAgaW1wb3J0U3R5bGU6IGZhbHNlLCAvLyBjc3MgaW4ganNcbiAgICAgICAgICByZXNvbHZlSWNvbnM6IHRydWUsIC8vIEVuc3VyZSBpY29ucyBhcmUgcmVzb2x2ZWQgb24tZGVtYW5kXG4gICAgICAgIH0pLFxuICAgICAgXSxcbiAgICB9KSxcbiAgXSxcbiAgYnVpbGQ6IHtcbiAgICByb2xsdXBPcHRpb25zOiB7XG4gICAgICBvbndhcm4od2FybmluZywgd2Fybikge1xuICAgICAgICAvLyBTdXBwcmVzcyBzcGVjaWZpYyB3YXJuaW5nc1xuICAgICAgICBpZiAoXG4gICAgICAgICAgd2FybmluZy5jb2RlID09PSAnVU5SRVNPTFZFRF9JTVBPUlQnIHx8XG4gICAgICAgICAgd2FybmluZy5tZXNzYWdlLmluY2x1ZGVzKCdAbWljcm9zb2Z0L3NpZ25hbHInKVxuICAgICAgICApIHtcbiAgICAgICAgICByZXR1cm5cbiAgICAgICAgfVxuICAgICAgICB3YXJuKHdhcm5pbmcpIC8vIERlZmF1bHQgYmVoYXZpb3IgZm9yIG90aGVyIHdhcm5pbmdzXG4gICAgICB9LFxuICAgICAgb3V0cHV0OiB7XG4gICAgICAgIG1hbnVhbENodW5rcyhpZCkge1xuICAgICAgICAgIGlmIChpZC5pbmNsdWRlcygnbm9kZV9tb2R1bGVzJykpIHtcbiAgICAgICAgICAgIC8vIEhhbmRsZSBzaGFyZWQgdXRpbGl0aWVzIG9mIGFudC1kZXNpZ24tdnVlXG4gICAgICAgICAgICAvLyBEeW5hbWljYWxseSBzcGxpdCBhbnQtZGVzaWduLXZ1ZSBjb21wb25lbnRzXG4gICAgICAgICAgICBpZiAoaWQuaW5jbHVkZXMoJ2FudC1kZXNpZ24tdnVlJykpIHtcbiAgICAgICAgICAgICAgcmV0dXJuICdhbnQtZGVzaWduLXZ1ZSdcbiAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgLy8gRm9yY2Ugc3BsaXR0aW5nIG9mIFZ1ZSBjb3JlIGxpYnJhcmllc1xuICAgICAgICAgICAgaWYgKGlkLmluY2x1ZGVzKCdAdnVlJykpIHtcbiAgICAgICAgICAgICAgcmV0dXJuICd2dWUtY29yZSdcbiAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgLy8gRm9yY2Ugc3BsaXR0aW5nIG9mIG90aGVyIGxhcmdlIGRlcGVuZGVuY2llc1xuICAgICAgICAgICAgaWYgKGlkLmluY2x1ZGVzKCdtb21lbnQnKSkge1xuICAgICAgICAgICAgICByZXR1cm4gJ21vbWVudCdcbiAgICAgICAgICAgIH1cbiAgICAgICAgICAgIGlmIChpZC5pbmNsdWRlcygnbG9kYXNoJykpIHtcbiAgICAgICAgICAgICAgcmV0dXJuICdsb2Rhc2gnXG4gICAgICAgICAgICB9XG4gICAgICAgICAgICBpZiAoaWQuaW5jbHVkZXMoJ2F4aW9zJykpIHtcbiAgICAgICAgICAgICAgcmV0dXJuICdheGlvcydcbiAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgLy8gR3JvdXAgZGVwZW5kZW5jaWVzIGZyb20gdGhlIHNhbWUgbmFtZXNwYWNlXG4gICAgICAgICAgICBjb25zdCBuYW1lc3BhY2VNYXRjaCA9IGlkLm1hdGNoKC9ub2RlX21vZHVsZXNcXC9AKFteL10rKVxcLy8pXG4gICAgICAgICAgICBpZiAobmFtZXNwYWNlTWF0Y2gpIHtcbiAgICAgICAgICAgICAgcmV0dXJuIGB2ZW5kb3ItJHtuYW1lc3BhY2VNYXRjaFsxXX1gXG4gICAgICAgICAgICB9XG5cbiAgICAgICAgICAgIC8vIEdyb3VwIG90aGVyIGxpYnJhcmllcyBieSB0aGVpciBmb2xkZXJcbiAgICAgICAgICAgIGNvbnN0IGxpYnJhcnlNYXRjaCA9IGlkLm1hdGNoKC9ub2RlX21vZHVsZXNcXC8oW14vXSspXFwvLylcbiAgICAgICAgICAgIGlmIChsaWJyYXJ5TWF0Y2gpIHtcbiAgICAgICAgICAgICAgcmV0dXJuIGB2ZW5kb3ItJHtsaWJyYXJ5TWF0Y2hbMV19YFxuICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICAvLyBEZWZhdWx0IHZlbmRvciBjaHVuayBmb3IgcmVtYWluaW5nIGxpYnJhcmllc1xuICAgICAgICAgICAgcmV0dXJuICd2ZW5kb3InXG4gICAgICAgICAgfVxuICAgICAgICB9LFxuICAgICAgICBlbnRyeUZpbGVOYW1lczogJ2Fzc2V0cy9bbmFtZV0tW2hhc2hdLmpzJywgLy8gRmlsZSBuYW1pbmcgZm9ybWF0IGZvciBjaHVua3NcbiAgICAgICAgY2h1bmtGaWxlTmFtZXM6ICdhc3NldHMvW25hbWVdLVtoYXNoXS5qcycsIC8vIEZpbGUgbmFtaW5nIGZvcm1hdCBmb3Igc2hhcmVkIGNodW5rc1xuICAgICAgICBhc3NldEZpbGVOYW1lczogJ2Fzc2V0cy9bbmFtZV0tW2hhc2hdW2V4dG5hbWVdJywgLy8gRmlsZSBuYW1pbmcgZm9ybWF0IGZvciBzdGF0aWMgYXNzZXRzXG4gICAgICB9LFxuICAgIH0sXG4gICAgb3V0RGlyOiAnZGlzdCcsIC8vIEVuc3VyZSB0aGUgb3V0cHV0IGZvbGRlciBtYXRjaGVzIHRoZSBiYXNlXG4gICAgYXNzZXRzRGlyOiAnYXNzZXRzJyxcbiAgICBzb3VyY2VtYXA6IHByb2Nlc3MuZW52Lk5PREVfRU5WICE9PSAncHJvZHVjdGlvbicsIC8vIEVuYWJsZSBzb3VyY2VtYXBzIGluIGRldmVsb3BtZW50XG4gIH0sXG4gIG9wdGltaXplRGVwczoge1xuICAgIGluY2x1ZGU6IFsnYW50LWRlc2lnbi12dWUnXSxcbiAgICBleGNsdWRlOiBbJ0BhbnQtZGVzaWduL2ljb25zLXZ1ZSddLFxuICAgIGVzYnVpbGRPcHRpb25zOiB7XG4gICAgICB0YXJnZXQ6ICdlc25leHQnLFxuICAgICAgc3BsaXR0aW5nOiB0cnVlLFxuICAgICAgZm9ybWF0OiAnZXNtJyxcbiAgICB9LFxuICB9LFxuICByZXNvbHZlOiB7XG4gICAgYWxpYXM6IHtcbiAgICAgICdAJzogZmlsZVVSTFRvUGF0aChuZXcgVVJMKCcuL3NyYycsIGltcG9ydC5tZXRhLnVybCkpLFxuICAgIH0sXG4gIH0sXG59KVxuIl0sCiAgIm1hcHBpbmdzIjogIjtBQUEyWCxTQUFTLGVBQWUsV0FBVztBQUM5WixTQUFTLG9CQUFvQjtBQUM3QixPQUFPLFNBQVM7QUFDaEIsT0FBTyxZQUFZO0FBQ25CLE9BQU8saUJBQWlCO0FBQ3hCLE9BQU8sc0JBQXNCO0FBQzdCLE9BQU8sZ0JBQWdCO0FBQ3ZCLE9BQU8sbUJBQW1CO0FBQzFCLE9BQU8sZUFBZTtBQUN0QixTQUFTLDRCQUE0QjtBQUNyQyxZQUFZLFlBQVk7QUFWc04sSUFBTSwyQ0FBMkM7QUFheFIsY0FBTztBQUVkLFFBQVEsSUFBSSxr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
