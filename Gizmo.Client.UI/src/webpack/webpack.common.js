const path = require("path");
const CopyWebpackPlugin = require("copy-webpack-plugin");

module.exports = {
  watch: true,
  watchOptions: {
    ignored: /node_modules/,
    aggregateTimeout: 500, // Delay before reloading
    poll: 1000, // Check for changes every second
    stdin: true, // Stop watching when stdin stream has ended
  },
  entry: {
    client_internal_code: "../js/internal.js",
    client_external_code: "../js/external.js",
    client_api_code: "../js/api.js",
    client_internal_style: "../scss/main.scss",
    client_external_style: "../scss/external.scss",
    webcomponents_code:
      "../../../Submodules/Gizmo.Web.Components/src/js/main.js",
    webcomponents_style:
      "../../../Submodules/Gizmo.Web.Components/src/scss/main.scss",
  },
  plugins: [
    new CopyWebpackPlugin({
      patterns: [
        {
          from: "../html",
          to: path.resolve(__dirname, "../../wwwroot/"),
          // globOptions: {
          //   ignore: ["**/index.html"]
          // },
        },
        {
          from: "../img",
          to: path.resolve(__dirname, "../../wwwroot/img"),
        },
      ],
    }),
  ],
  resolve: {
    modules: [path.resolve(__dirname, "node_modules")],
  },
  module: {
    rules: [
      {
        test: /\.(scss|css)$/i,
        use: ["style-loader", "css-loader", "sass-loader"],
      },
      {
        test: /\.(woff(2)?|ttf|eot)$/,
        type: "asset/resource",
        generator: {
          filename: "./font-family/[name][ext]",
        },
      },
    ],
  },
  performance: {
    hints: false,
  },
  output: {
    filename: "[name].js",
    path: path.resolve(__dirname, "../../wwwroot/"),
    clean: true,
  },
};
