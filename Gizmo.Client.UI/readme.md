# Gizmo.Client.UI

# Gulp installation steps:

* For scss/js compilation we are using gulp. The following packages are necessary:
  gulp gulp-autoprefixer gulp-clean-css gulp-concat gulp-connect gulp-group-css-media-queries
  gulp-less gulp-rename gulp-sass gulp-sourcemaps gulp-minify postcss node-sass
  and they are installed as dev-dependencies with Node.js 18.12.1.

* To install gulp-cli globally open the command line
  and run the following command:
  
 npm install --global gulp-cli

* Under both Gizmo.Client.UI and Gizmo.Web.Components directories open command line
  and run the following command to install required packages:
  
 npm install

* After installation you can run "gulp" command to start watching for file changes in scss/js directories.

* Visual Studio 2022 - Task Runner Explorer
  If Task Runner Explorer cannot load Gulpfile.js with error: ".. could not find a binding for your current environment: ...",
  you can try to move the (PATH) above the (VSInstalledExternalTools) under Tools->Options...->Projects and Solutions->Web Package Management
  and restart Visual Studio.


# Override variable / typography according to new theme/requirements in web project steps:

* we are using (Gizmo.Web.Components) library inside (Gizmo.Client.UI) web project.
* all our component (button,card,input), variables, typography classes comes from (Gizmo.Web.Components)
  which is common/global and we use (Gizmo.Web.Components) in our multiple web projects.

* so, we do not make any project specific changes inside the (Gizmo.Web.Components).
  becuase as we know this is common/global. So if we do any change inside the (Gizmo.Web.Components)
  it will effect all web projects where we are using (Gizmo.Web.Components).

* To resolve this problem, We are using override approach.
  We override variables/classes in specific project.

  for this, go to (Gizmo.Client.UI) > src > scss folder.
  inside scss folder we see _variable.scss and _typography.scss file.
  where we can override variable/typography according to project requirements.
  It will not effect (Gizmo.Web.Components) and other web projects too.
  and woking with proper way.