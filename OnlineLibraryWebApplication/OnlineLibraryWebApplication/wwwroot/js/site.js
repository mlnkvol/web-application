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
    var bookId = document.getElementById('bookId').value;

    fetch('/Books/AddToLibrary', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-CSRF-TOKEN': document.querySelector('input[name="__RequestVerificationToken"]').value // Якщо ви використовуєте захист від CSRF-атак
        },
        body: JSON.stringify({
            bookId: bookId
        })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Виникла помилка: ' + response.status);
            }
            return response.json();
        })
        .then(data => {
            alert(data); // Повідомлення про успішне додавання книги до бібліотеки
        })
        .catch(error => {
            console.error('Помилка:', error);
            alert('Сталася помилка при спробі додати книгу до бібліотеки.');
        });
}


document.getElementById('exportButton').addEventListener('click', async () => {
    const format = document.getElementById('exportFormat').value;
    try {
        const response = await fetch(`/Books/Export?format=${format}`, { 
            method: 'GET'
        });
        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `books.${format}`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
    } catch (error) {
        console.error('Помилка експорту:', error);
    }
});


