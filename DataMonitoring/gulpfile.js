/// <binding BeforeBuild='css, min:css' />

// Requis
var gulp = require('gulp');
var uglify = require('gulp-uglify');

var plugins = require('gulp-load-plugins')(); // tous les plugins de package.json
var source = "styles";
var destination = "wwwroot";

gulp.task('css', function () {
    return gulp.src([source + '/smartadmin.scss', source + '/smartadmin-plugins.scss'])
    .pipe(plugins.sass())
    .pipe(plugins.csscomb())
    .pipe(plugins.cssbeautify({ indent: '  ' }))
    //.pipe(plugins.autoprefixer())
    .pipe(gulp.dest(destination + '/css/' ));
});

gulp.task('min:css', function () {
    return gulp.src([destination + '/css/smartadmin.css', destination + '/css/smartadmin-skins.css', destination + '/css/smartadmin-plugins.css', destination + '/css/smartadmin-angular-next.css'])
    .pipe(plugins.csso())
    .pipe(plugins.rename({
      suffix: '.min'
    }))
    .pipe(gulp.dest(destination + '/css/'));
});

gulp.task('watch', () => {

  gulp.watch(source + '/*.scss', ['css']);
  gulp.watch(destination + "/css/smartadmin.css", ["min:css"]);
});

// Gulp task to minify JavaScript files
gulp.task('scripts', function () {
  return gulp.src(destination + '/js/gauge.js')
    // Minify the file
    .pipe(uglify())
    .pipe(plugins.rename({
      suffix: '.min'
    }))
    // Output
    .pipe(gulp.dest(destination + '/js'))
});



