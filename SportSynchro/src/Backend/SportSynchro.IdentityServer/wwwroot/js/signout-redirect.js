(function () {
    var seconds = 3;
    var el = document.getElementById("countdown");

    if (!el) return;

    var interval = setInterval(function () {
        seconds--;
        el.textContent = seconds;

        if (seconds <= 0) {
            clearInterval(interval);
            window.location.href = "http://localhost:5173";
        }
    }, 1000);
})();
