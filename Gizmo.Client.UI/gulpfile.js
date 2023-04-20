/// <binding BeforeBuild='comiple' ProjectOpened='watch' />
'use strict';

const { src, dest } = require('gulp');
var gulp = require('gulp');
var connect = require('gulp-connect');
var gcmq = require('gulp-group-css-media-queries');
var cleanCSS = require('gulp-clean-css');
var autoprefixer = require('gulp-autoprefixer');
var rename = require('gulp-rename');
var sourcemaps = require('gulp-sourcemaps');
var minify = require('gulp-minify');

//create sass compiler
var nodeSass = require('node-sass');
var gulpSass = require("gulp-sass");
var sass = gulpSass(nodeSass);

//compile all function
function compileAll(cb) {
    sassCompile();
    jsCompile();
    cb();
}

//sass compilation function
function sassCompile() {
    return src('src/scss/main.scss')
        .pipe(sourcemaps.init())
        .pipe(sass().on('error', sass.logError))
        .pipe(autoprefixer({
            cascade: false,
            overrideBrowserslist: ['last 2 versions']
        }))
        .pipe(gcmq())
        .pipe(sourcemaps.write('.'))
        .pipe(dest('wwwroot/css'))
        .pipe(rename("main.min.css"))
        .pipe(cleanCSS({
            level: 1
        }))
        .pipe(rename("main.min.css"))
        .pipe(sourcemaps.write('.'))
        .pipe(dest('wwwroot/css'))
        .pipe(connect.reload());
}

//js compilation function
function jsCompile() {
    return src('src/js/**/*.js')
        .pipe(minify({
            ext: {
                min: '.min.js'
            },
            ignoreFiles: ['-min.js','interop.js']
        }))
        .pipe(dest('wwwroot/js'));
}

//sass file change watch function
function watchTask() {
    gulp.watch('src/scss/*.scss', sassCompile);
    gulp.watch('src/scss/**/*.scss', sassCompile);
    gulp.watch('src/js/*.js', jsCompile);
    gulp.watch('src/js/**/*.js', jsCompile);
}

//this watch task will run on project open and will monitor sass file changes
gulp.task('default', watchTask);

//create a watch task responsible of compiling sass on any source file changes
gulp.task("watch", watchTask);

//define our default compilation task
gulp.task('comiple', compileAll);

//sass compilation task
gulp.task('sass', sassCompile);

//java script compilation task
gulp.task('js', jsCompile);
