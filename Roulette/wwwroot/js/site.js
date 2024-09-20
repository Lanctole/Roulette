function resizeCanvas() {
    const canvas = document.getElementById('rouletteCanvas');
    if (canvas) {
        if (window.innerWidth > 500) {
            var width = window.innerWidth * 0.3;
            var height = width;
            canvas.width = width;
            canvas.height = height;
        } else {
            var width = window.innerWidth * 0.9;
            var height = width;
            canvas.width = width;
            canvas.height = height;
        }

    }
}

window.getTooltipPlacement = function () {
    return window.innerWidth <= 768 ? 'right' : 'bottomLeft';
};