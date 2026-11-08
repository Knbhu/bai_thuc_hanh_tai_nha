const sass = require('sass');

module.exports = {
    css: {
        loaderOptions: {
            sass: { implementation: sass }
        }
    },
    devServer: {
        proxy: {
            '/api': {
                target: 'http://localhost:8099',
                changeOrigin: true
            },
            '/login': {
                target: 'http://localhost:6060',
                changeOrigin: true,
                pathRewrite: { '^/login': '/api' }
            },
            '/agentsChat': {
                target: 'http://localhost:4099',
                changeOrigin: true,
                ws: true
            }
        }
    }
};
