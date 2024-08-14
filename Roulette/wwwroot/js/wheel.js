function createWheel(mediaName) {
    elements = mediaName;
    names = [];
    elements.forEach(element => {
        names.push(element);
    });
    formattedNames = splitAndFormatStrings(names);
    canvas = document.getElementById('rouletteCanvas');
    clickedWhat = document.getElementById('clickedWhat');
    canvasWidth = canvas.width;
    canvasHeight = canvas.height;

    theWheel = new Winwheel({
        'canvasId': 'rouletteCanvas',
        'outerRadius': canvasHeight / 2 - 20,
        'numSegments': 5,
        'textFontSize': 10,
        'innerRadius': 20,
        'responsive': true,
        'textAlignment': 'center',
        'textFontFamily': 'Georgia',
        'rotationAngle': -30,
        'fillStyle': '#e7706f',
        'lineWidth': 3,
        'segments':
            [
                { 'fillStyle': '#c0c0c0', 'text': formattedNames[0] },
                { 'fillStyle': '#c0c0c0', 'text': formattedNames[1] },
                { 'fillStyle': '#c0c0c0', 'text': formattedNames[2] },
                { 'fillStyle': '#c0c0c0', 'text': formattedNames[3] },
                { 'fillStyle': '#c0c0c0', 'text': formattedNames[4] }
            ],
        'animation':
        {
            'type': 'spinToStop',
            'duration': 3,
            'spins': 5,
            'callbackFinished': 'winAnimation()',
            'callbackAfter': 'drawPointer()'
        }
        
    });

    drawPointer();

    canvas.onclick = function (e) {
        resetSegmentColours();
        let clickedSegment = theWheel.getSegmentAt(e.clientX, e.clientY);
        if (clickedSegment) {
            clickedSegment.fillStyle = 'yellow';
            theWheel.draw();
            drawPointer();
            clickedWhat.innerText = clickedSegment.text;
        }
    }

    function resetSegmentColours() {
        for (let x = 1; x < theWheel.segments.length; x++) {
            theWheel.segments[x].fillStyle = '#c0c0c0';
        }
        theWheel.draw();
        drawPointer();
        clickedWhat.innerText = "";
    }

    function splitAndFormatStrings(strings) {
        return strings.map(str => {
            let words = str.split(' ');
            function wrapLongWord(word) {
                const maxLength = 13;
                let wrapped = [];
                while (word.length > maxLength) {
                    wrapped.push(word.slice(0, maxLength) + '-');
                    word = word.slice(maxLength);
                }
                wrapped.push(word);
                return wrapped.join('\n');
            }

            let lines = [];
            let currentLine = '';

            words.forEach(word => {
                let wrappedWord = word.length > 13 ? wrapLongWord(word) : word;
                if ((currentLine + (currentLine ? ' ' : '') + wrappedWord).length <= 16) {
                    currentLine += (currentLine ? ' ' : '') + wrappedWord;
                } else {
                    lines.push(currentLine);
                    currentLine = wrappedWord;
                }
            });

            if (currentLine) {
                lines.push(currentLine);
            }

            if (lines.length > 4) {
                lines = lines.slice(0, 4);
                lines[3] += '...'; 
            }
            return lines.join('\n');
        });
    }
    
}
function winAnimation() {
    
    let winningSegmentNumber = theWheel.getIndicatedSegmentNumber();
    for (let x = 1; x < theWheel.segments.length; x++) {
        theWheel.segments[x].fillStyle = '#c0c0c0';
    }
    theWheel.segments[winningSegmentNumber].fillStyle = 'yellow';
    theWheel.draw();
    drawPointer();
}

function alertPrize() {
    let winningSegment = theWheel.getIndicatedSegment();
    alert("You have won " + winningSegment.text + "!");
}

function drawPointer() {
    ctx = theWheel.ctx;
    canvasWidth = canvas.width;
    canvasHeight = canvas.height;

    ctx.lineWidth = 2;
    ctx.strokeStyle = 'black';
    ctx.fillStyle = 'black';

    const pointerWidth = 20;
    const pointerHeight = 20;
    const pointerX = (canvasWidth / 2) - (pointerWidth / 2);
    const pointerY = -4;

    ctx.beginPath();
    ctx.moveTo(pointerX, pointerY);
    ctx.lineTo(pointerX + pointerWidth, pointerY);
    ctx.lineTo(pointerX + (pointerWidth / 2), pointerY + pointerHeight);
    ctx.lineTo(pointerX, pointerY);
    ctx.stroke();
    ctx.fill();
    ctx.restore();
}

function startSpin(mediaName) {
    createWheel(mediaName);
    theWheel.stopAnimation(false);
    theWheel.rotationAngle = theWheel.rotationAngle % 360;
    theWheel.startAnimation();
}