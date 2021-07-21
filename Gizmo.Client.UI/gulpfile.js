/// <binding BeforeBuild='sass' ProjectOpened='watch' />
'use strict';

const { src, dest } = require('gulp');


var gulp = require('gulp');
var sass = require('gulp-sass')(require('sass'));
var connect = require('gulp-connect');
var gcmq = require('gulp-group-css-media-queries');
var cleanCSS = require('gulp-clean-css');
var autoprefixer = require('gulp-autoprefixer');
var concat = require('gulp-concat');

sass.compiler = require('sass');

//sass compilation function
function sassCompile() {
    return src('wwwroot/scss/main.scss')
        .pipe(sass().on('error', sass.logError))
        .pipe(autoprefixer({
            cascade: false,
            overrideBrowserslist: ['last 2 versions']
        }))
        .pipe(gcmq())
        .pipe(dest('wwwroot/css'))
        .pipe(cleanCSS({
            level: 1
        }))
        .pipe(connect.reload());s
}

//js file concat function
function scripts() {
    return src(['wwwroot/js/*.js', 'wwwroot/fontawesome/js/all.js'])
        .pipe(concat('all.js'))
        .pipe(dest('wwwroot'))
        .pipe(connect.reload());
}

//sass file change watch function
function watchTask() {
    gulp.watch('wwwroot/scss/*.scss', sassCompile);
    gulp.watch('wwwroot/scss/*/*.scss', sassCompile);
    gulp.watch('wwwroot/js/*.js', scripts);   
    gulp.watch('wwwroot/fontawesome/js/all.js', scripts);
}

//define our default sass compilation task
gulp.task('sass', sassCompile);

//create a watch task responsible of compiling sass on any source file changes
gulp.task("watch", watchTask);

//this watch task will run on project open and will monitor sass file changes
gulp.task('default', watchTask);
