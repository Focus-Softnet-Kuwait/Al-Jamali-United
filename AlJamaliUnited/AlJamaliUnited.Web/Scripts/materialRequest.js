//Get today's date in the format YYYY-MM-DD
const today = new Date().toISOString().split('T')[0];




$(document).ready(function () {

    //DataTable initialization
    var table = $('#dataTable').DataTable({

        "dom": '<"row"<"col-md-6"l><"col-md-6"f>>Brt<"row"<"col-md-6"i><"col-md-6"p>>',
        "buttons": [{
            extend: 'excel',
            text: 'Export to Excel',
            className: 'btn btn-primary',
            exportOptions: {
                columns:[0,1,2,3,4,5,6]
            }
        }],
        columns: [
            { data: "ItemCode" },
            { data: "Description" },
            { data: "QtySold" },
            { data: "StockQty" },
            { data: "Unit" },
            { data: "ItemSellingRate" },
            { data: "Cost" },
            { data: "Warehouse" }

        ],
    });
    $('#BtnLoad').on('click', function (e) {
        debugger
        e.preventDefault();
        console.log('Button clicked!');
        // Rest of your code...
    }); 
});
// Event listener for the "Close Screen" button
$('#closeScreenBtn').click(function () {
    debugger
    Focus8WAPI.gotoHomePage();
});
            //Handle form submission

//$('#BtnLoad').on('click', function (e) {
//    e.preventDefault();
//    $('#divLoader').show();
//    var toDate = $('#ToDate').val();

//    // Make an AJAX request to the controller action method.
//    $.ajax({
//        url: '/AlJamaliUnited/MaterialRequest/LoadData',      //Use a relative URL
//        type: 'Get',
//        data: {
//            toDate: toDate
//        },
//        success: function (data) {
//            debugger
//            console.log(data);
//            $('#divLoader').hide();

//        },
//        error: function (xhr, textStatus, errorThrown) {
//            debugger
//            console.log('Error', textStatus, errorThrown);
//        },
//        complete: function () {
//            // Hide the loader regardless of success or error
//            $('#divLoader').hide();
//        }

//    });
//});