$(document).ready(function () {
    var maxLength = 250;
    var description = document.getElementById('description');
    var readMoreBtn = document.getElementById('read-more');
    var fullText, shortenedText;

    if (description) {
        fullText = description.innerHTML;
        if (fullText.length > maxLength) {
            shortenedText = fullText.substring(0, maxLength) + '...';
            description.innerHTML = shortenedText;
            if (readMoreBtn) {
                readMoreBtn.style.display = 'inline';
            }
        } else {
            if (readMoreBtn) {
                readMoreBtn.style.display = 'none';
            }
        }
    }

    window.toggleDescription = function () {
        if (!description || !readMoreBtn) return;

        if (readMoreBtn.innerHTML === 'Читати далі') {
            description.innerHTML = fullText;
            readMoreBtn.innerHTML = 'Згорнути';
        } else {
            description.innerHTML = shortenedText;
            readMoreBtn.innerHTML = 'Читати далі';
        }
    };

    window.addToLibrary = function (bookTitle) {
        var bookId = document.getElementById('bookId').value;
        var token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        if (!bookId || !token) {
            console.error('bookId або token не знайдені:', { bookId, token });
            alert('Помилка: не вдалося отримати дані книги або токен.');
            return;
        }

        fetch('/Possessions/AddToLibrary', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({ id: bookId }) // Перевірте, чи контролер чекає саме 'id'
        })
            .then(response => {
                console.log('Response status:', response.status); // Логування статусу
                if (!response.ok) {
                    throw new Error(`Помилка: ${response.status} - ${response.statusText}`);
                }
                return response.text();
            })
            .then(data => {
                console.log('Response data:', data); // Логування даних
                $('#addToLibraryModal').modal('show');
                $('#modalMessage').html(`Книга <strong>${bookTitle}</strong> успішно додана до <a href="/Possessions/Index">бібліотеки</a>✅`);
            })
            .catch(error => {
                console.error('Error:', error);
                alert(`Сталася помилка при додаванні книги до бібліотеки: ${error.message}`);
            });
    };

    $(window).scroll(function () {
        var scroll = $(window).scrollTop();
        if (scroll >= $(window).height()) {
            $('body').addClass('pastel-background');
        } else {
            $('body').removeClass('pastel-background');
        }
    });

    $('#reviewsCarousel').carousel({
        interval: false
    });
});