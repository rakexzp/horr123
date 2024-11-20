// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
window.addEventListener('scroll', () => {
    const scrollPosition = window.scrollY;
    const headerHeight = headerImage.offsetHeight;
    const opacity = 1 - (scrollPosition / headerHeight);
    headerImage.style.opacity = opacity;
    if (scrollPosition > 0) {
        headerImage.style.backgroundColor = '#fff';
    } else {
        headerImage.style.backgroundColor = 'transparent';
    }
});
$('a[href="#index-needle"]').on('click', function() {
    var target = $(this).attr('href');
    $('html, body').animate({
        scrollTop: $(target).offset().top
    }, 1000);
});
// Write your JavaScript code.
