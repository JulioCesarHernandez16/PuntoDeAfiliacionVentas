$(document).ready(function () {
    CargaCasosLead();
});


function CargaCasosLead() {
    $.ajax({
        type: "GET",
        url: "/Casos/CargaCasosLead",
        data: {},
        dataType: "json",
        success: function (data) {
            CargaTablaCasos(data);


        },
        complete: function () {
            $("#load_screen").hide();
        }
    });


}

function CargaTablaCasos(Data) {


    if ($.fn.DataTable.isDataTable('#tblCasos')) {
        $('#tblCasos').DataTable().destroy();
    }

    tblConsulta = $('#tblCasos').DataTable({
        searching: true,
        dom: '<"row"<"col-md-12"<"row"<"col-md-6"B><"col-md-6"f> > ><"col-md-12"rt> <"col-md-12"<"row"<"col-md-5"i><"col-md-7"p>>> >',
        buttons: {
            buttons: [
                { extend: 'excel', className: 'btn' }
            ]
        },
        responsive: true,
        data: Data,
        pageLength: true,
        lengthMenu: true,
        paging: true,
        info: true,
        "lengthMenu": [5, 10, 20, 50],
        "pageLength": 5,
        "columns": [
            { data: "ID" },
            { data: "NOMBRE_COMPLETO" },
            {
                data: "FECHA_CREACION",
                render: function (data, type, row, meta) {
                    // Create a Date object from the date string
                    var date = new Date(data);

                    // Format the date to display only the date without the time
                    var formattedDate = date.toLocaleDateString();

                    return formattedDate;
                },
            },
            {
                render: function (data, type, row, meta) {
                    if (row.ESTADO == '1') {

                        return '<span class="badge badge-success" style ="width: 100px;">CREADO</span>';

                    } if (row.ESTADO == '2') {
                        return '<span class="badge badge-danger" style ="width: 100px;">VENTA</span>';

                    }
                    if (row.ESTADO == '3') {
                        return '<span class="badge badge-danger" style ="width: 100px;">NO VENTA</span>';

                    }
                    if (row.ESTADO == '4') {
                        return '<span class="badge badge-danger" style ="width: 100px;">ASIGNADO</span>';

                    }
                    if (row.ESTADO == '5') {
                        return '<span class="badge badge-danger" style ="width: 100px;">PAGADO E INSTALADO</span>';

                    }
                },
            },
           
        ],
        "language": { "url": "../Content/Spanish.json" }
    });

}
