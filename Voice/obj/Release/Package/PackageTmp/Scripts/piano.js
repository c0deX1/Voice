var width = 500, octave = ['A', 'A#', 'B', 'C', 'C#', 'D', 'D#', 'E', 'F', 'F#', 'G', 'G#'], id = "", div, whitekeys=0, keys = [],context = window.AudioContext ? new AudioContext() : new webkitAudioContext();
parent: for (var i = 0; i < 8; i++) {
	for (var j = 0; j < 12; j++) {
		if ((i * 12) + j >= 88) break parent;
		div = document.createElement('div');
		div.id = (octave[j][1] == '#') ? octave[j][0] + ((((i * 12) + j + 9) / 12)|0) + 's' : octave[j] + ((((i * 12) + j + 9) / 12)|0);
		div.class = 'pianoButton';
		if (j % 12 == 1 || j % 12 == 4 || j % 12 == 6 || j % 12 == 9 || j % 12 == 11) {
			div.setAttribute('style', 'border:1px solid black; position:absolute; background-color:black; left:' + ((width / 50 * whitekeys) - (width / 200)) + 'px; width:' + width/100 + 'px; height: 100px; z-index:1;');}
		else {
			div.setAttribute('style', 'border:1px solid black; position:absolute; background-color:white; left:' + (width / 50 * whitekeys) + 'px; width:' + width/50 + 'px; height:150px;');
			whitekeys++;
		}
		$(".q").append(div);}}
document.body.addEventListener('click', play);
function play(e) {
	if($(e.target.parentElement).attr('class') == 'q'){
	var oldColor;
	if(e.target.style.backgroundColor != 'red') oldColor = e.target.style.backgroundColor;
	e.target.style.backgroundColor = 'red';
	var controctave = { 'C': 32.703, 'Cs': 34.648, 'D': 36.708, 'Ds': 38.891, 'E': 41.203, 'F': 43.654, 'Fs': 46.249, 'G': 48.999, 'Gs': 51.913, 'A': 55, 'As': 58.27, 'B': 61.735}, osc = context.createOscillator();
	osc.frequency.value = e.target.id[2] == 's' ? controctave[e.target.id[0] + 's'] * Math.pow(2, (e.target.id[1]|0) - 1) : controctave[e.target.id[0]] * Math.pow(2, (e.target.id[1]|0) - 1);
	osc.type = "square";
	osc.connect(context.destination);
	osc.start(0);
	setTimeout(function() {
	    osc.stop(0);
	    osc.disconnect(context.destination);
	    e.target.style.backgroundColor = oldColor;
	}, 1000 / 2);}
}