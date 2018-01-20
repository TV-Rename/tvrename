// Run jquery Slideshow on the div class="fade"
$(function(){
$('.fade > :gt(0)').hide();
	setInterval(function(){$('.fade > :first-child').fadeOut().next().fadeIn().end().appendTo('.fade');}, 2500);
});