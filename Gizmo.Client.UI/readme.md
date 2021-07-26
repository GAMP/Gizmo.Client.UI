
# gulp installation steps:-

* for scss/js complilation we are using gulp.

* go to your command line and run below commands for gulp setup.

 npm init
 npm install
 npm install --global gulp-cli
 npm install node-sass gulp-sass --save-dev
 run gulp 

* After gulp installation done run (gulp) command you can see gulp running.

# Override variable / typography according to new theme/requirements in web project steps:-

* we are using (Gizmo.Web.Components) library inside (Gizmo.Client.UI) web project.
* all our component (button,card,input), variables, typography classes comes from (Gizmo.Web.Components)
  which is common/global and we use (Gizmo.Web.Components) in our multiple web projects.

* so, we do not make any project specific changes inside the (Gizmo.Web.Components).
  becuase as we know this is common/global. So if we do any change inside the (Gizmo.Web.Components)
  it will effect all web projects where we are using (Gizmo.Web.Components).

* To resolve this problem, We are using override approach.
  We override variables/classes in specific project.

  for this, go to (Gizmo.Client.UI) > wwwroot > scss folder.
  inside scss folder we see _variable.scss and _typography.scss file.
  where we can override variable/typography according to project requirements.
  It will not effect (Gizmo.Web.Components) and other web projects too.
  and woking with proper way.

 
  


  

