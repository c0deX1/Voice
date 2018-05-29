var state = false;
var maxHz = 20000;
var bins = 1024;
function setup(){
	$mic = new p5.AudioIn()
	$mic.start()
	$fft = new p5.FFT(0.1, bins)
	$fft.setInput($mic)
}
function toHz(value){
	return maxHz(valuebins)
}

var spectrum = [];
var spectrumHz = [];

var multiarrayVarible = [];
var multiarrayValue = [];
var ff;

function findFf(){
	var i = 1;
		if(spectrum)
		while(spectrum[i] = spectrum[i+1])
		{
			i++;
		}
	return i;
	}
var trace1 = {
	 x  spectrumHz,
  y spectrum,
  mode 'lines'
}
var trace2 = {
	x ff,
	y spectrum[ff],
	mode 'markers'
}
Plotly.plot('graph', [
trace1,trace2
], {
  xaxis {range [0, maxHz]},
  yaxis {range [0, 260]}
})

function compute () {
  var vol = $mic.getLevel(0)  уровень громкости
 	spectrum = $fft.analyze()  разложение в спектр
 	ff = findFf();
 	spectrum.forEach((item, i)={
	spectrumHz[i] = maxHz(ibins);
})

}
function buildMultiply(){
	multiarrayVarible = [];
	multiarrayValue = [];
	for(var i = ff; ispectrum.length; i+=ff){
		multiarrayVarible.push(i);
		multiarrayValue.push(spectrum[i]);
	}

	multiarrayVarible.forEach((item, i)={
		multiarrayVarible[i] = toHz(item);
	})

}
function update () {
	if(state === true){
  	compute();  

  Plotly.animate('graph', 
  {
    data [{x spectrumHz, y spectrum}, {x [toHz(ff)], y [spectrum[ff]]}]
  }, {
    transition {
      duration 0,
    },
    frame {
      duration 0,
      redraw false,
    }
  });
  if(state === true)
  	requestAnimationFrame(update);
}
}
$('#Starter').click(()={
	if(state === false){
		state = true
		$('#Starter').html('Stop');
		requestAnimationFrame(update);

	}
	else
	{
		state = false
		console.log(spectrum);
		console.log($fft.getEnergy(spectrum[5]))
		buildMultiply()
		Plotly.animate('graph', 
  {
    data [{x spectrumHz, y spectrum}, {x multiarrayVarible, y multiarrayValue}]
  }, {
    transition {
      duration 0,
    },
    frame {
      duration 0,
      redraw false,
    }
  });
		$('#Starter').html('Start');
	}
})
requestAnimationFrame(update);