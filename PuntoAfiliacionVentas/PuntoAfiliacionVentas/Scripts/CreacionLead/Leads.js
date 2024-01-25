$(document).ready(function () {
    CargaComboServicios()
    LimpiarCampos()

});



function InsertaLead() {
    let txtNombre = document.getElementById('txtNombre').value;
    let txtDireccion = document.getElementById('txtDireccion').value;
    let txtTelefono1 = document.getElementById('txtTelefono1').value;
    let txtTelefono2 = document.getElementById('txtTelefono2').value;
    let txtCorreo = document.getElementById('txtCorreo').value;
    let cboServios = document.getElementById('cboServios').options[document.getElementById('cboServios').selectedIndex].text; //Obtener texto seleccionado

    if (!txtNombre) {
        swal({
            title: 'El nombre es obligatorio.',
            type: 'error',
            padding: '2em'
        });
        return;
    }

    if (!txtTelefono1) {
        swal({
            title: 'El telefono1 es obligatorio',
            type: 'error',
            padding: '2em'
        });
        return;
    }

    if (!txtCorreo) {
        swal({
            title: 'El correo es obligatorio',
            type: 'error',
            padding: '2em'
        });
        return;
    }
    if (!txtDireccion) {
        swal({
            title: 'La dirección es obligatoria',
            type: 'error',
            padding: '2em'
        });
        return;
    }


    $.ajax({
        type: "POST",
        url: "/Home/InsertaLead",
        data: { Nombre: txtNombre, Correo: txtCorreo, Telefono1: txtTelefono1, Telefono2: txtTelefono2, Direccion: txtDireccion, ServiciosInteres: cboServios },
        dataType: "json",
        success: function (data) {

            if (data == "OK") {
                swal({
                    title: 'Se ha insertado el Lead Correctamente' ,
                    type: 'success',
                    padding: '2em'
                });
                LimpiarCampos()
            } else {
                alert("Error");
            }
        },
        complete: function () {
            $("#load_screen").hide();
        }
    });

}

function CargaComboServicios()
{
    let cboServios = document.getElementById('cboServios'); //Obtener texto seleccionado
    $.ajax({
        type: "POST",
        url: "/Home/PaquetesCombos",
        dataType: "json",
        success: function (data) {

            var datos = JSON.parse(data);
           
            $(datos).each(function () {
                var option = $(document.createElement('option'));

                option.text(this.DESCRIPCIO);
                option.val(this.DESCRIPCIO);

                $("#cboServios").append(option);

            });
        },
        complete: function () {
            $("#load_screen").hide();
        }
    });

}

function LimpiarCampos() {
    document.getElementById('txtNombre').value = "";
    document.getElementById('txtDireccion').value = "";
    document.getElementById('txtTelefono1').value = "";
    document.getElementById('txtTelefono2').value = "";
    document.getElementById('txtCorreo').value = "";

}