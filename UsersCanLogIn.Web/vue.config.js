const path = require('path');

module.exports = {
    devServer: {
        proxy: 'http://localhost:51051'
    },
    outputDir: path.resolve(__dirname, '../UsersCanLogIn.API/wwwroot')
}