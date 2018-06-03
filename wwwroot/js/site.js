var SelectedItem;
var preText;
var InputText;
var FinalText;
var ControlSelected = 0;
var myArray = new Array();


$(document).ready(function () {



   //Permite estableces unas características de edición de las tablas que se muestran en la aplicación
    //y unos eventos asociados al cambio de celda o aceptación.

    $(".EditableName, .EditableNameProducts, .EditableNameItems, .EditableNameChange").click(function (event) {
       
        //Establezco la posicion actual

        if (ControlSelected === 0) {
            SelectedItem = $(this);
            ControlSelected = 1;
        }
        else
        {
            alert("Es necesario Aceptar cambios con 'INTRO' o con 'ESC'");
            SelectedItem.html(preText);
            ControlSelected = 1;
        }

        //valor antiguo que habia de datos html
         preText = SelectedItem.html();

        //nueva variable que voy a meter
         InputText = $("<input type='text'/>");
        
        SelectedItem.html("");

        InputText
            .width("100%")
            .val(preText)
            .appendTo(SelectedItem)
            .focus()
            .select();
           
        InputText.keyup(function (event) {
            if (13 === event.which) { // press ENTER-key
               FinalText = $(this).val();
               SelectedItem.html(FinalText);
               ControlSelected = 0;
            }
            else if (27 === event.which) {  // press ESC-key
                SelectedItem.html(preText);
                ControlSelected = 0;
            }
        });

        //inhabilitamos segundo click dentro del cuadro inputtext
        InputText.click(function () {
            return false;
        });
    });
});

//Eventos asociados al boton aceptar o cancelar

$(document).ready(function () {



    //restablece los datos como estaban de origen

    $("#Cancelchanges").click(function (event) {
        if (confirm("¡Atención!, Los cambios no guardados se perderán"))
            SelectedItem.html(preText);
        else
            return;
    });

    //Elimina las másquinas seleccionadas una vez que se ha pulsado el botón correspondiente

    $('.DeleteMachine').on('click', function () {

        
            // aviso de que se va a borrar

            if (confirm("¡Atención!, se va a borrar la siguiente máquina" + $(this).val())) {

                myArray.push("-1", "eliminar", "eliminar", "eliminar","eliminar", null);

                $(this).parents("tr").find("td").each(function () {
                    myArray.push($(this).text());
                });

                $(this).parent().parent().remove();

                $.ajax({
                    type: 'POST',
                    url: '/Machine/SaveChangeData',
                    traditional: true,
                    dataType: "json",
                    data: {
                        arrayofvalues: myArray,
                    }
                });
                //limpiamos el array
                myArray = [];
            }
    });

    //Elimina los elementos o productos seleccionados

    $('.DeleteItems').on('click', function () {


        // aviso de que se va a borrar

        if (confirm("¡Atención!, se va a borrar el siguiente producto" + $(this).val())) {

            myArray.push("-1", "eliminar", "eliminar");

            $(this).parents("tr").find("td").each(function () {
                myArray.push($(this).text());
            });

            $(this).parent().parent().remove();

            $.ajax({
                type: 'POST',
                url: '/Machine/SaveChangeDataItems',
                traditional: true,
                dataType: "json",
                data: {
                    arrayofvalues: myArray,
                }
            });
            //limpiamos el array
            myArray = [];
        }
    });

    //Actualiza los cambios del cambio en monedas de cada máquina

    $('#SaveChangesChange').on('click', function () {

        if (confirm("Los cambios se van a guardar" + $(this).val())) {

            $("#tableDataChange tr td").each(function () {
                myArray.push($(this).text());
            });

            $.ajax({
                type: 'POST',
                url: '/Machine/SaveChangeDataChange',
                traditional: true,
                dataType: "json",
                data: {
                    arrayofvalues: myArray,
                }
            });
            //limpiamos el array
            myArray = [];
        }
    });

    //actualiza los cambios realizados sobre los items de productos existentes

    $('#SavechangesItems').on('click', function () {

        if (confirm("Los cambios se van a guardar" + $(this).val())) {

            myArray.push("1", "Update", "Update");

            $("#tableDataItems tr td").each(function () {
                myArray.push($(this).text());
            });

            $.ajax({
                type: 'POST',
                url: '/Machine/SaveChangeDataItems',
                traditional: true,
                dataType: "json",
                data: {
                    arrayofvalues: myArray,
                }
            });
            //limpiamos el array
            myArray = [];
        }
    });

    //Anañe una nueva fila en el campo de nuevos productos existentes

    $("#AddNewItemRow").click(function (event) {

        if ($("#InputTextNewItem").val().length > 0) {

            myArray.push("-2", "new", "new");
            myArray.push($("#InputTextNewItem").val(), $("#InputTextNewItem").val(), $("#InputTextNewItem").val());

            $.ajax({
            type: 'POST',
            url: '/Machine/SaveChangeDataItems',
            traditional: true,
            dataType: "json",
            data: {
                arrayofvalues: myArray,
            },
                success: function (response) {
                    if (response.success)
                    {
                        location.reload();
                    }
                }
            });

            //limpiamos el array
            myArray = [];
        }
    });

    //Actualiza los cambios de una partial view cuando se pulsa sobre un producto

    $('.Item').click(function (event) {

        $.ajax({
            type: 'POST',
            url: '/Machine/PartialItemsView',
            traditional: true,
            data: {
                arrayofvalues: $(this).attr("id")
            },
            success: function (data) {
                location.reload();

            }
        });
    });

    //Actualiza la partial view cuando se pulsa sobre una máquina para mostrar el cambio del que dispone

    $('.Change').click(function (event) {

        $.ajax({
            type: 'POST',
            url: '/Machine/PartialChangeView',
            traditional: true,
            data: {
                arrayofvalues: $(this).attr("id")
            },
            success: function (data) {
                location.reload();
            }
        });
    }); 

    //Actualiza la partial view con los mensajes en la que el usuario actual es origen o destinatario de los mensajes

    $('.MessageUser').click(function (event) {

        $.ajax({
            type: 'POST',
            url: '/Messages/PartialMessageView',
            traditional: true,
            data: {
                arrayofvalues: $(this).attr("id")
            },
            success: function (data) {
                location.reload();
            }
        });
    });

    //Actualiza la partial view con los productos al pulsar sobre una máquina

    $('.Product').click(function (event) {

        $.ajax({
            type: 'POST',
            url: '/Machine/PartialSlotsView',
            traditional: true,
            data: {
                arrayofvalues: $(this).attr("id")
            },
            success: function (data) {
                location.reload();
            }

        }); 
    });
    
    //Actualiza la partial view con la gráfica de estadísticas sobre los productos

    $('.ItemsStats').click(function (event) {

        $.ajax({
            type: 'POST',
            url: '/Stats/PartialProductsStats',
            traditional: true,
            data: {
                arrayofvalues: $(this).attr("id")
            },
            success: function (data) {
                location.reload();
            }

        });
    });

    //Actualiza la partial view con la gráfica de estadísticas sobre las máquinas

    $('.ProductStats').click(function (event) {

        $.ajax({
            type: 'POST',
            url: '/Stats/PartialMachineStats',
            traditional: true,
            data: {
                arrayofvalues: $(this).attr("id")
            },
            success: function (data) {
                location.reload();
            }

        });
    });

    //Guarda los cambios realizados sobre una máquina

    $("#Savechanges").click(function (event) {


        if (confirm("¡Atención!, Los datos se van a guardar")) {
            var sThisVal;
            myArray.push("0", "editar", "editar", "editar", "editar", null);

            $("#tableData tr td").each(function () {

                myArray.push($(this).text());

            });

            $.ajax({
                type: 'POST',
                url: '/Machine/SaveChangeData',
                traditional: true,
                dataType: "json",
                data: {
                    arrayofvalues: myArray,
                }
            });
            //limpiamos el array
            myArray = [];
        }
        else
            return;
    });  


    //Actualiza los cambios realizados sobre los productos

    $("#SavechangesProducts").click(function (event) {


        if (confirm("¡Atención!, Los datos se van a guardar")) {
            var sThisVal;
            myArray.push(null, "0", "editar", "editar", "editar", "editar", "editar");

            $("#tableDataProducts tr td").each(function ()
            {
                //Si es un item del combo de producto lo inserto, sino, inserto el valor de la fila td
                if ($(this).attr("value") === "productComboBoxs")
                {
                    myArray.push($(this).find(":selected").val());
                }
                else
                {
                    myArray.push($(this).text());
                }
            });

            $.ajax({
                type: 'POST',
                url: '/Machine/SaveChangeDataProducts',
                traditional: true,
                dataType: "json",
                data: {
                    arrayofvalues: myArray,
                },
                success: function (data) {
                    location.reload();
                }
            });
            //limpiamos el array
            myArray = [];
        }
        else
            return;
    });


    
    //Actualiza los cambios realizado sobre el stock de las máquinas

    $("#SavechangesItemsStock").click(function (event) {


        if (confirm("¡Atención!, Los datos se van a guardar")) {
            var sThisVal;
            myArray.push("0", "Editar", "Editar", "Editar", "Editar", "Editar");

        
               
            $("#tableDataItemsStock tr td").each(function () {

                    myArray.push($(this).text());

                });
      

            $.ajax({
                type: 'POST',
                url: '/Machine/SaveChangeDataStockProducts',
                traditional: true,
                dataType: "json",
                data: {
                    arrayofvalues: myArray,
                },
                success: function (data) {
                    location.reload();
                }
            });
            //limpiamos el array
            myArray = [];
        }
        else
            return;
    });
});

