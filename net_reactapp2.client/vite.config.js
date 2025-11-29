import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
    plugins: [react()],
    server: {
        proxy: {
            '/api': {
                target: 'https://localhost:7139',
                changeOrigin: true,
                secure: false,
                rewrite: (path) => {
                    const separator = '/';
                    const startingIndex = 2;
                    const newPath = `${separator}${path.split(separator).slice(startingIndex).join(separator)}`;
                    return newPath;
                }
            }
        }
    }
})
