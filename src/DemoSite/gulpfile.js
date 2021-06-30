var gulp = require('gulp'); 

gulp.task('copy', function (done) {
    gulp.src('../CD.UmbracoForms.DotdigitalWorkflow/App_Plugins/CD.*/**/*.*')
        .pipe(gulp.dest('./App_Plugins'));
    done();
});