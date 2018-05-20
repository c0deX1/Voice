function nul(e){
	return (e===undefined || e===null)
}
function empty(e){
	if(typeof e==='Array'){
		for(var i in e){
			return false
		}
	}
	return true
}
function sizeof(a){
	var count=0
	for(var i in a){
		if(a.hasOwnProperty(i)){++count}
	}
	return count
}
function htmlspecialchars(a){
	var map={
		'&': '&amp;',
		'<': '&lt;',
		'>': '&gt;',
		'"': '&quot;',
		"'": '&#039;'
	}
	return a.replace(/[&<>"']/g,function(m){return map[m]})
}
$ROOT=$ROOT || ''
$USER=$USER || {
	ID:0
}
var emptyFunc=function(){}
/*
{
	url
	data
	[type]=POST
	[dataType]=json
	success
	error
	complete
}
*/
ajax=function(e){
	var success=e.success || emptyFunc
	var error=e.error || emptyFunc
	var complete=e.complete || emptyFunc
	delete e.success
	delete e.error
	delete e.complete
	e.type=e.type || 'POST'
	e.dataType=e.dataType || 'json'
	if(e.dataType==='json'){
		e.success=function(a){
			if(a.error){
				alert(a.error)
				error(a)
			} else{
				success(a)
			}
			complete(a)
		}
	}
	return $.ajax(e)
}