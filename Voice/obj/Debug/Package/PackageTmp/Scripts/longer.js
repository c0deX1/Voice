var canvas;

var maxFreqGraph = 4500;
var maxdBGraph = 120;
var maxHz = 3000;
var minHz = 100;
var maxdB = 50;
var mindB = 10;

var $state = 'wait'

var average = mindB + (maxdB - mindB) / 2;
var $mic;
var $recorder;
var $fft;
var $waveform;
var fps = 30
var startTime = new Date().getTime()
var $startFreq // стартовая эталонная частота
var $freq // текущая эталонная частота
var $line
var $need_freq = 150
var $freq_in_sec = 50
var $need_secs = $need_freq / $freq_in_sec // секунд на одно ребро
var $freq_offset = 20 // нормализация отклонения
var $cnt
var $good_cnt
var myMatrix;

function todB(value) {

    return Math.round(maxdBGraph * (value / canvas.height))
}
function toHz(value) {

    return Math.round(maxFreqGraph * (value / canvas.width))
}
function drawAxes(_cnvs) {
    var _ctx = canvas.getContext('2d')
    _ctx.lineWidth = 2;
    _ctx.moveTo(0, 0)
    _ctx.lineTo(0, canvas.height)
    _ctx.lineTo(canvas.width, canvas.height)
}
function drawPoints(_cnvs) {
    var _ctx = _cnvs.getContext('2d');
    var point = 0;
    _ctx.font = "10px Arial";
    var step = _cnvs.height / 7;
    var _height = _cnvs.height;
    var _width = _cnvs.width;

    for (var i = 0; i < 8; i++) {
        _ctx.moveTo(0, _height - point)
        _ctx.lineTo(4, _height - point)
        point += Math.round(step);
        _ctx.fillText(todB(point), 5, _height - point + 7);
    }
    point -= Math.round(step);
    _ctx.font = "20px Arial";
    _ctx.fillText("A, дБ", 30, _height - point + 17);
    _ctx.font = "10px Arial";

    point = 0;
    for (var i = 0; i < 8; i++) {
        _ctx.moveTo(point, _height)
        _ctx.lineTo(point, _height - 4)
        point += Math.round(step);
        _ctx.fillText(toHz(point), point - 30, _height - 5);
    }
    _ctx.font = "20px Arial";
    _ctx.fillText("f, Гц", _width - 50, _height - 30);
}

$("#canvas").click(() => {
    canvas = $('#canvas')[0];
    canvas.width = 512;
    canvas.height = 512;
    var _ctx = canvas.getContext('2d')
    _ctx.beginPath()
    drawAxes(canvas)
    drawPoints(canvas)
    //_ctx.fillRect(canvas.width * i / maxFreqGraph, (canvas.height - (canvas.height * item / maxdBGraph)), 5, 5)
    //_ctx.rect(canvas.width * minHz / maxFreqGraph, (canvas.height - canvas.height * (maxdBGraph - maxdB) / maxdBGraph), canvas.width * (maxHz - minHz) / maxFreqGraph, canvas.height * (maxdB - mindB) / maxdBGraph);
    _ctx.rect(canvas.width * minHz / maxFreqGraph, (canvas.height * (maxdBGraph - maxdB) / maxdBGraph), canvas.width * (maxHz - minHz) / maxFreqGraph, canvas.height * (maxdB - mindB) / maxdBGraph);
    console.log()
    _ctx.stroke()
})
$(document).ready(() => {
    canvas = $('#canvas')[0];
    canvas.width = 512;
    canvas.height = 512;
    var _ctx = canvas.getContext('2d')
    _ctx.beginPath()
    drawAxes(canvas)
    drawPoints(canvas)
    _ctx.stroke()
})
$("#valueSet").click(() => {
    maxHz = $("#maxHz").val();
    minHz = $("#minHz").val();;
    maxdB = $("#maxdB").val();;
    mindB = $("#mindB").val();;
})
function matrixArray(rows, columns) {
    var arr = new Array();
    for (var i = 0; i < rows; i++) {
        arr[i] = new Array();
        for (var j = 0; j < columns; j++) {
            arr[i][j] = average;
        }
    }
    return arr;
}

myMatrix = matrixArray(3, 2000); //[0] - min, [1]-max, [2]-Changed flag(???)
function findnextchnged(index_from) {
    var flag = index_from + 1;
    if (myMatrix[2][flag] != 1)
        flag++;
    else
        return flag;
}
function matrixtocanvas(_cnvs, changed) {
    var _ctx = _cnvs.getContext('2d');
    var _height = _cnvs.height;
    var _width = _cnvs.width;
    _ctx.clearRect(0, 0, _width, _height)
    _ctx.beginPath()
    drawAxes(canvas)
    drawPoints(canvas);
    if (myMatrix[0][changed] != myMatrix[1][changed]) {
        //console.log("dbchanged: " + changed +  " mat[o][i]: " + myMatrix[0][changed] +  " mat[1][i]: " + myMatrix[1][changed])
    }

    _ctx.moveTo(0, _height - myMatrix[0][0]);
    myMatrix[0].forEach((item, i) => {
        if (myMatrix[2][i] === 1)
            _ctx.fillRect(canvas.width * i / maxFreqGraph, (_height - canvas.height * item / maxdBGraph), 3, 3) //_ctx.lineTo(i, _height - item)

    });
    _ctx.moveTo(0, _height - myMatrix[1][0]);
    myMatrix[1].forEach((item, i) => {
        if (myMatrix[2][i] === 1)
            _ctx.fillRect(canvas.width * i / maxFreqGraph, (canvas.height - (canvas.height * item / maxdBGraph)), 5, 5)
        //_ctx.lineTo(i, _height - item)
    });

    _ctx.stroke();
}
// SETUP
function setup() {
    $mic = new p5.AudioIn()
    $mic.start()
    $fft = new p5.FFT()
    $fft.setInput($mic)
}

var startBtn = $('#startBtn').click(() => {
    if ($state === 'record') {
        $state = 'wait'
        startBtn.html('Start')
    }
    else {
        setup()
        $state = 'record'
        startBtn.html('Stop')
        //myMatrix.length = 0;
    }
    var $interval = setInterval(function () {//main function
        curTime = new Date().getTime() - startTime

        var vol = $mic.getLevel(0.01) // уровень громкости
        var spectrum = $fft.analyze() // разложение в спектр
        var waveform = $fft.waveform();// разложение в волну 
        var freqs = spectrum.length
        var offset = 10 // кол-во частот слева и справа при нахождении максималного среднего
        var sumprev = 0;
        function findFf() {
            var i = 1;
            if (spectrum)
                while (spectrum[i] <= spectrum[i + 1]) {
                    i++;
                }
            return i;
        }

        for (var i = 0; i < offset; ++i) {
            sumprev += spectrum[i]
        }
        var sumnext = 0
        for (var i = offset + 1; i <= offset + offset; ++i) {
            sumnext += spectrum[i]
        }
        var maxfreq = offset
        var maxavrg = (sumprev + spectrum[offset] + sumnext) / (offset * 2 + 1)
        for (var i = offset; i < freqs - offset - 1; ++i) {
            var avrg = (sumprev + spectrum[i] + sumnext) / (offset * 2 + 1)
            if (avrg > maxavrg) {
                maxfreq = i
                maxavrg = avrg
            }
            sumprev += -spectrum[i - offset] + spectrum[i]
            sumnext += -spectrum[i + 1] + spectrum[i + 1 + offset]
        }

        if ($state === 'wait') {
            $startFreq = maxfreq
        }
        else if ($state === 'record') {
            if ($line === 2) {
                $freq = $startFreq
            } else {
                $freq = $startFreq + $need_freq * Math.sin((curTime / 1000 / ($need_secs * 2)) * Math.PI)
            }
        }
        var currentHz = Math.round(20000 * (maxfreq / 1024))
        var currentdB = Math.round(vol * 160);
        //console.log(maxfreq);

        if ($state === 'record') {
            if (myMatrix[2][currentHz] == average) //Test DSV
            {
                myMatrix[0][currentHz] = average;
                myMatrix[1][currentHz] = average;
                myMatrix[2][currentHz] = 1;
                // matrixtocanvas(currentHz);
                //console.log("currentHz: " + currentHz + " 0: " + myMatrix[0][currentHz] + " 1: "+ myMatrix[1][currentHz]); //Test init
            }
            else {
                if (currentdB > mindB)
                    myMatrix[0][currentHz] = (myMatrix[0][currentHz] > currentdB) ? (currentdB) : (myMatrix[0][currentHz]);
                if (currentdB < maxdB)
                    myMatrix[1][currentHz] = (myMatrix[1][currentHz] < currentdB) ? (currentdB) : (myMatrix[1][currentHz]);
                // matrixtocanvas(currentHz);
                //console.log("currentHz: " + currentHz + " 0: " + myMatrix[0][currentHz] + " 1: "+ myMatrix[1][currentHz]);	//Test change
            }
            // document.getElementById('canvas')
            matrixtocanvas(canvas, currentHz);
            $('#output').html(
                'Громкость ~ ' + currentdB + ' дБ<br/>\
		Высота голоса: '+ currentHz + ' Гц <br/>\
		Частота основного тона: ' + 20000 * findFf() / 1024 + ' Гц')
        }// TODO: findFf to Hz
    }, 1000 / fps)
});