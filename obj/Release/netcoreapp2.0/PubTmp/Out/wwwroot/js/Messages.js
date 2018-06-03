$(document).ready(function () {

    //Realiza el autoscroll al cargar la página de los mensajes de manera que se muestren siempre los últimos
    //mensajes

    $('#autoscroll').animate({
        scrollTop: $('#autoscroll').get(0).scrollHeight
    }, 0);
});