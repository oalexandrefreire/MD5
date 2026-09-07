(function () {
    var menuButton = document.querySelector('.menu-toggle');
    var navigation = document.querySelector('#main-nav');

    if (menuButton && navigation) {
        menuButton.addEventListener('click', function () {
            var isOpen = navigation.classList.toggle('is-open');
            menuButton.setAttribute('aria-expanded', String(isOpen));
        });

        navigation.querySelectorAll('a').forEach(function (link) {
            link.addEventListener('click', function () {
                navigation.classList.remove('is-open');
                menuButton.setAttribute('aria-expanded', 'false');
            });
        });
    }

    document.querySelectorAll('[data-copy]').forEach(function (button) {
        button.addEventListener('click', function () {
            var originalLabel = button.textContent;
            navigator.clipboard.writeText(button.getAttribute('data-copy')).then(function () {
                button.textContent = 'Copiado';
                window.setTimeout(function () { button.textContent = originalLabel; }, 1600);
            });
        });
    });
})();
