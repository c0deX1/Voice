;(function() {
var width = 512
var height = 650
var canvas = $('#canvas')[0]
canvas.width = width
canvas.height = height
var ctx = canvas.getContext('2d')

var cx = width / 2
var cy = height / 2
var triangle

;(function(){
	// Сторона равностороннего треугольника
	var a = Math.min(width, height) * 0.7
	triangle = {
		0: {x: cx - a / 2, y: cy + a / 2 * Math.sin(Math.PI / 3), sin: -Math.sin(Math.PI / 3), cos: Math.cos(Math.PI / 3), angle: -60},
		1: {x: cx, y: cy - a / 2 * Math.sin(Math.PI / 3), sin: Math.sin(Math.PI / 3), cos: Math.cos(Math.PI / 3), angle: 60},
		2: {x: cx + a / 2, y: cy + a / 2 * Math.sin(Math.PI / 3), sin: 0, cos: -1, angle: -180},
		a: a,
		median: a * Math.sin(Math.PI / 3)
	}
})()
function degs_to_rads (degs){ return degs / (180/Math.PI) }
function rads_to_degs (rads){ return rads * (180/Math.PI) }
var $mic;
var $recorder;
var $fft;
var $waveform;

function setup(){
	$mic = new p5.AudioIn()
	$mic.start()
	$fft = new p5.FFT()
	$fft.setInput($mic)
}

setup()
function clear(){
	ctx.clearRect(0, 0, canvas.width, canvas.height)
}
// DRAW TRIANGLE
function drawTriangle(){
	ctx.beginPath()
	ctx.moveTo(triangle[0].x, triangle[0].y)
	ctx.lineTo(triangle[1].x, triangle[1].y)
	ctx.lineTo(triangle[2].x, triangle[2].y)
	ctx.closePath()
	ctx.stroke()
}

function HSVtoRGB(h, s, v) {
    var r, g, b, i, f, p, q, t;
    if (arguments.length === 1) {
        s = h.s, v = h.v, h = h.h;
    }
    i = Math.floor(h * 6);
    f = h * 6 - i;
    p = v * (1 - s);
    q = v * (1 - f * s);
    t = v * (1 - (1 - f) * s);
    switch (i % 6) {
        case 0: r = v, g = t, b = p; break;
        case 1: r = q, g = v, b = p; break;
        case 2: r = p, g = v, b = t; break;
        case 3: r = p, g = q, b = v; break;
        case 4: r = t, g = p, b = v; break;
        case 5: r = v, g = p, b = q; break;
    }
    return {
        r: Math.round(r * 255),
        g: Math.round(g * 255),
        b: Math.round(b * 255)
    };
}
function drawCircle(line, time, relation){
	var radius = 20
	ctx.save()
	var degs = degs_to_rads(triangle[line].angle)
	var cos = triangle[line].cos
	var sin = triangle[line].sin
	cos = Math.cos(degs)
	sin = Math.sin(degs)
	ctx.setTransform(cos, sin, -sin, cos, triangle[line].x, triangle[line].y)
	var rgb = HSVtoRGB(120 / 360 * (1 - Math.abs(relation)), 1, 1)
	ctx.fillStyle = 'rgb('+ rgb.r +', '+ rgb.g +', '+ rgb.b +')'
	ctx.beginPath()
	ctx.arc(
		triangle.a * time,
		relation * radius * 2,
		radius,
		0,
		2 * Math.PI,
		false
	)
	ctx.fill()
	ctx.restore()
	ctx.font = '20px Arial'
	ctx.textAlign = 'center'
	ctx.fillText((relation < 0 ? 'Держите тембр выше' : 'Держите темб ниже'), cx, cy + triangle.median / 4)
}

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

var $interval = setInterval(function(){//main function
	curTime = new Date().getTime() - startTime
	if ($state === 'record'){
		// Смена состояний на костылях
		if ($line === 0){
			if (curTime / 1000 >= $need_secs){
				$line = 1
			}
		} else if ($line === 1){
			if (curTime / 1000 >= $need_secs * 2){
				$line = 2
			}
		}
		if (curTime / 1000 > $need_secs * 3){
			$state = 'wait'
			startBtn.html('Start')
			$('#result').html('Результат: '+ Math.round($good_cnt / $cnt * 100) +'%')
			return
		}
	}
	clear()
	drawTriangle()
	var vol = $mic.getLevel() // уровень громкости
	var spectrum = $fft.analyze() // разложение в спектр
	var waveform = $fft.waveform();// разложение в волну 
	
	//console.log(waveform);
	var freqs = spectrum.length
	var offset = 10 // кол-во частот слева и справа при нахождении максималного среднего
	var sumprev = 0;
function findFf(){
	var i = 1;
		if(spectrum)
		while(spectrum[i] <= spectrum[i+1])
		{
			i++;
		}
	return i;
	}
	for (var i = 0; i < offset; ++i){
		sumprev += spectrum[i]
	}
	var sumnext = 0
	for (var i = offset + 1; i <= offset + offset; ++i){
		sumnext += spectrum[i]
	}
	var maxfreq = offset
	var maxavrg = (sumprev + spectrum[offset] + sumnext) / (offset * 2 + 1)
	for (var i = offset; i < freqs - offset - 1; ++i){
		var avrg = (sumprev + spectrum[i] + sumnext) / (offset * 2 + 1)
		if (avrg > maxavrg){
			maxfreq = i
			maxavrg = avrg
		}
		sumprev += -spectrum[i - offset] + spectrum[i]
		sumnext += -spectrum[i + 1] + spectrum[i + 1 + offset]
	}
	if ($state === 'wait'){
		$startFreq = maxfreq
	}
	else if ($state === 'record'){
		if ($line === 2){
			$freq = $startFreq
		} else{
			$freq = $startFreq + $need_freq * Math.sin((curTime / 1000 / ($need_secs * 2)) * Math.PI)
		}
	}
	//console.log($startFreq, maxfreq, $freq, maxfreq - $freq)
	if ($state === 'record'){
		++$cnt
		// подсчёт "Результат"
		if (Math.abs((maxfreq - $freq) / $freq_offset) < 0.2){
			++$good_cnt
		}
		drawCircle(
			$line, 
			(curTime % ($need_secs * 1000)) / ($need_secs * 1000), 
			(maxfreq < $freq ? 
				(maxfreq + $freq_offset < $freq ? -1 : (maxfreq - $freq) / $freq_offset) : 
				(maxfreq - $freq_offset > $freq ? 1 : (maxfreq - $freq) / $freq_offset)
			)
		)
	}
	var currentHz =  Math.round(20000 * (maxfreq / 1024))
	var currentdB = vol * 100;
		$('#output').html(
'Громкость ~ '+ currentdB+ ' дБ<br/>\
Высота голоса: '+currentHz  +' Гц <br/>\
Частота основного тона: ' + 20000*findFf()/1024 + ' Гц') // TODO: findFf to Hz
}, 1000 / fps)
var $state = 'wait'
var startBtn = $('#startBtn').click(function(){
	if ($state === 'wait'){
		$state = 'record'
		//$recorder.record(soundFile)
		startBtn.html('Stop')
		$line = 0
		$freq = $startFreq
		$need_freq = Math.min(50, $startFreq * 0.5)
		startTime = new Date().getTime()
		$cnt = 0
		$good_cnt = 0
		$('#result').html('')
	}
	else if ($state === 'record'){
		$state = 'wait'
		//$recorder.stop()
		startBtn.html('Start')
	}
})
})()