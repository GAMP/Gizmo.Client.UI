# Gizmo.Web.Components

## Gulp installation steps:

* For scss/js compilation we are using gulp. The following packages are necessary:
  * gulp gulp-autoprefixer
  * gulp-clean-css gulp-concat
  * gulp-connect 
  * gulp-group-css-media-queries
  * gulp-less
  * gulp-rename
  * gulp-sass
  * gulp-sourcemaps
  * gulp-minify
  * postcss
  * node-sass

  and they are installed as dev-dependencies with Node.js 18.12.1.

* To install gulp-cli globally open the command line
  and run the following command:

  ```console
  npm install --global gulp-cli
  ```

* Under both Gizmo.Client.UI and Gizmo.Web.Components directories open command line
  and run the following command to install required packages:

  ```console 
  npm install
  ```

* After installation you can run "gulp" command to start watching for file changes in scss/js directories.

* Visual Studio 2022 - Task Runner Explorer
  If Task Runner Explorer cannot load Gulpfile.js with error: ".. could not find a binding for your current environment: ...",
  you can try to move the (PATH) above the (VSInstalledExternalTools) under Tools->Options...->Projects and Solutions->Web Package Management
  and restart Visual Studio.

