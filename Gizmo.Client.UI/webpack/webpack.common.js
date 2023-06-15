const path = require("path");
const CopyWebpackPlugin = require("copy-webpack-plugin");

module.exports = {
  entry: {
    client_internal_code: "./src/js/internal.js",
    client_external_code: "./src/js/external.js",
    client_api_code: "./src/js/api.js",
    client_internal_style: "./src/scss/main.scss",
    client_external_style: "./src/scss/external.scss",
    webcomponents_code: "../Submodules/Gizmo.Web.Components/src/js/main.js",
    webcomponents_style:
      "../Submodules/Gizmo.Web.Components/src/scss/main.scss",
  },
  plugins: [
    new CopyWebpackPlugin({
      patterns: [
        {
          from: "src/html",
          to: path.resolve(__dirname, "../wwwroot/"),
        },
        {
          from: "src/img",
          to: path.resolve(__dirname, "../wwwroot/img"),
        },
      ],
    }),
  ],
  resolve: {
    modules: [path.resolve(__dirname, "../node_modules")],
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
    path: path.resolve(__dirname, "../wwwroot/"),
    clean: true,
  },
};
