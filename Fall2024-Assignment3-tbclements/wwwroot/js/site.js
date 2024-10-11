// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    //get the selectedmember from the hidden field.
    var selectedmembers = $("#selectedmemebers").val().split(",");
    //set the value in the dropdownlist.
    $("#Members").select2({
        multiple: true,
    });
    $('#Members').val(selectedmembers).trigger('change');
});