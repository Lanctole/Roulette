formattedNamesToIdMap = [];

function createWheel(mediaName) {
    const mediaArray = Object.entries(mediaName).map(([Id, name]) => ({ Id: Number(Id), name }));
    const names = mediaArray.map(item => item.name);
    const formattedNamesArray = splitAndFormatStrings(names);
    const formattedNames = mediaArray.map((item, index) => ({
        Id: item.Id,
        formattedName: formattedNamesArray[index]
    }));
    formattedNamesToIdMap = mediaArray.reduce((map, item, index) => {
            map[formattedNamesArray[index]] = item.Id;
            return map;
        },
        {});
    const segments = formattedNames.map(nameObj => ({
        'fillStyle': "#c0c0c0",
        'text': nameObj.formattedName
    }));

    canvas = document.getElementById("rouletteCanvas");
    clickedWhatId = document.getElementById("winnerId");
    canvasWidth = canvas.width;
    canvasHeight = canvas.height;

    theWheel = new Winwheel({
        'canvasId': "rouletteCanvas",
        'outerRadius': canvasHeight / 2 - 20,
        'numSegments': 5,
        'textFontSize': 10,
        'innerRadius': 20,
        'responsive': true,
        'textAlignment': "center",
        'textFontFamily': "Georgia",
        'rotationAngle': -30,
        'fillStyle': "#e7706f",
        'lineWidth': 3,
        'segments': segments,
        'animation':
        {
            'type': "spinToStop",
            'duration': 3,
            'spins': 5,
            'callbackFinished': "winAnimation(getIdByText)",
            'callbackAfter': "drawPointer()"
        }

    });

    drawPointer();

    canvas.onclick = function(e) {
        resetSegmentColours();
        const clickedSegment = theWheel.getSegmentAt(e.clientX, e.clientY);
        if (clickedSegment) {
            clickedSegment.fillStyle = "yellow";
            theWheel.draw();
            drawPointer();
            clickedWhatId.innerText = getIdByText(clickedSegment.text);
        }
        signalizeAboutWinner(getIdByText(clickedSegment.text));
    };

    function resetSegmentColours() {
        for (let x = 1; x < theWheel.segments.length; x++) {
            theWheel.segments[x].fillStyle = "#c0c0c0";
        }
        theWheel.draw();
        drawPointer();
        clickedWhatId.innerText = "";
    }

    function splitAndFormatStrings(strings) {
        return strings.map(str => {
            const words = str.split(" ");

            function wrapLongWord(word) {
                const maxLength = 13;
                const wrapped = [];
                while (word.length > maxLength) {
                    wrapped.push(word.slice(0, maxLength) + "-");
                    word = word.slice(maxLength);
                }
                wrapped.push(word);
                return wrapped.join("\n");
            }

            let lines = [];
            let currentLine = "";

            words.forEach(word => {
                const wrappedWord = word.length > 13 ? wrapLongWord(word) : word;
                if ((currentLine + (currentLine ? " " : "") + wrappedWord).length <= 16) {
                    currentLine += (currentLine ? " " : "") + wrappedWord;
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
                lines[3] += "...";
            }
            return lines.join("\n");
        });
    }

}

function getIdByText(text) {
    return formattedNamesToIdMap[text] || null;
}

let dotNetObjectRef;

window.registerDotNetObject = function (dotNetObject) {
    dotNetObjectRef = dotNetObject;
};
window.signalizeAboutWinner = async function (id) {
    if (dotNetObjectRef) {
        try {
            const result = await dotNetObjectRef.invokeMethodAsync("ChangeID", id);
        } catch (error) {
            console.error('Error invoking .NET method:', error);
        }
    } else {
        console.error('DotNetObjectReference is not set.');
    }
};


async function winAnimation(getIdByText) {
    const winningSegmentNumber = theWheel.getIndicatedSegmentNumber();
    theWheel.segments[winningSegmentNumber].fillStyle = "yellow";
    theWheel.draw();
    drawPointer();
    const winningSegment = theWheel.getIndicatedSegment();
    clickedWhatId.innerText = getIdByText(winningSegment.text);
    id = getIdByText(winningSegment.text);
    var value = "C# Method called from JavaScript with parameter";
    signalizeAboutWinner(id);
}


function drawPointer() {
    ctx = theWheel.ctx;
    canvasWidth = canvas.width;
    canvasHeight = canvas.height;

    ctx.lineWidth = 2;
    ctx.strokeStyle = "black";
    ctx.fillStyle = "black";

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