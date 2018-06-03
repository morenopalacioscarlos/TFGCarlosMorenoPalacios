//Guarda las nuevas asugnaciones de máquinas a un usuario

var myArray = new Array();

$(document).ready(function () {

    $("#SavechangesMachineAssign").click(function (event) {


        if (confirm("¡Atención!, Las nuevas asignaciones se van a realizar")) {
            var sThisVal;
            myArray.push("editar", "editar", "editar");

            $("#tableDataAssingMachine tr td").each(function () {
                //Si es un item del combo de producto lo inserto, sino, inserto el valor de la fila td
                if ($(this).attr("value") === "assignMachineComboBox") {
                    myArray.push($(this).find(":selected").val());
                }
                else {
                    myArray.push($(this).text());
                }
            });

            $.ajax({
                type: 'POST',
                url: '/Users/SaveChangeMachineAssign',
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