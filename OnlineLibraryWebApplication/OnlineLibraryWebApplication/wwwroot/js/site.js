// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var maxLength = 250;
var description = document.getElementById('book-description');
var readMoreBtn = document.getElementById('read-more');
var fullText = description.innerHTML;

if (fullText.length > maxLength) {
    var shortenedText = fullText.substring(0, maxLength) + '...';
description.innerHTML = shortenedText;
readMoreBtn.style.display = 'block';
} else {
    readMoreBtn.style.display = 'none';
}

function toggleDescription() {
    if (readMoreBtn.innerHTML === 'Читати далі') {
    description.innerHTML = fullText;
readMoreBtn.innerHTML = 'Згорнути';
    } else {
    description.innerHTML = shortenedText;
readMoreBtn.innerHTML = 'Читати далі';
    }
}

$(document).ready(function () {
    $(window).scroll(function () {
        var scroll = $(window).scrollTop();

        if (scroll >= $(window).height()) {
            $('body').addClass('pastel-background');
        } else {
            $('body').removeClass('pastel-background');
        }
    });
});

$(document).ready(function () {
    $('#reviewsCarousel').carousel({
        interval: false
    });
});

function addToLibrary() {
    var bookId = $("#bookId").val();

    $.ajax({
        url: "/Possessions/AddToLibrary/" + bookId,
        type: "POST",
        success: function () {
            alert("Книга додана до бібліотеки!");
        },
        error: function () {
            alert("Помилка під час додавання до бібліотеки.");
        }
    });
}

