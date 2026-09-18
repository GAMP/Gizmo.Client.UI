const path = require("path");
const CopyWebpackPlugin = require("copy-webpack-plugin");

module.exports = {
  entry: {
    client_internal_code: "./src/js/internal.js",
    client_external_code: "./src/js/external.js",
    client_api_code: "./src/js/api.js",
    client_internal_style: "./src/scss/main.scss",
    client_external_style: "./src/scss/external.scss"
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
        {
          from: path.resolve(__dirname, "../node_modules/flag-icons/flags/4x3"),
          to: path.resolve(__dirname, "../wwwroot/img/flags"),
        },
        // Fonts and icons are self-hosted files rather than part of the style bundle:
        // club machines can be offline, so nothing may be fetched from a CDN. They are
        // copied here, licence texts included, so a clean build produces a complete skin
        // instead of leaving them to be placed by hand.
        {
          from: "src/vendor",
          to: path.resolve(__dirname, "../wwwroot/vendor"),
        },
        {
          from: path.resolve(__dirname, "../node_modules/flag-icons/LICENSE"),
          to: path.resolve(__dirname, "../wwwroot/img/flags/LICENSE-flag-icons.txt"),
          toType: "file",
        },
        {
          from: "THIRD-PARTY-NOTICES.md",
          to: path.resolve(__dirname, "../wwwroot/THIRD-PARTY-NOTICES.md"),
          toType: "file",
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
