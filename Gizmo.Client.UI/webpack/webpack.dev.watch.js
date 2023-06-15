const { merge } = require("webpack-merge");
const common = require("./webpack.common.js");

module.exports = merge(common, {
  mode: "development",
  devtool: "inline-source-map",
  watch: true,
  watchOptions: {
    ignored: /node_modules/,
    aggregateTimeout: 500, // Delay before reloading
    poll: 1000, // Check for changes every second
    stdin: true, // Stop watching when stdin stream has ended
  },
});
