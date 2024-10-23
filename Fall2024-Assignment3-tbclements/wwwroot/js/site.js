// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//get the selectedmember from the hidden field.
var selectedmembers = $("#selectedmemebers").val();
if (selectedmembers != null) {
    selectedmembers = selectedmembers.split(',');
} else {
    selectedmembers = [];
}
//set the value in the dropdownlist.
$("#Members").select2({
    multiple: true,
});
$('#Members').val(selectedmembers).trigger('change');

$(".dt").DataTable({
    select: false,
    lengthChange: false,
    pageLength: 4
});

$(".indDT").DataTable({
    select: false,
    lengthChange: false,
    pageLength: 10,
    columnDefs: [{ orderable: false, targets: 0 }]
}).order([1, 'asc']).draw();