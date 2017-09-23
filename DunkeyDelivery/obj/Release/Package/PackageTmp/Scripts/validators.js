

function ValidateForm(result) {
    if (!result.success) {

        $('#errorBlock').html('');

        if ($("#CodeBrand").val() == '' || $("#CodeCategory").val() == '' || $("#CodePrice").val() == '') {
            $("#errorBlock").append('<p>*Code 1 is required</p>');
            $("#errorBlock").append('<p>*Code 2 is required</p>');
            $("#errorBlock").append('<p>*Code 3 is required</p>');
        }

        return false;
    }
}

function ValidateInsertForm(result) {
    if (!result.success) {

        $('#errorBlock').html('');

        if ($("#CodeBrand").val() == '' || $("#CustomerId").val() == '') {
            $("#errorBlock").append('<p>*Code 1 is required</p>');
            $("#errorBlock").append('<p>*CustomerId is required</p>');
        }

        return false;
    }
}
// success insertion message for admin panel 
function OnSuccessInsertion() {
    //$("#success-alert").append("Store updated successfully.");
    $("#success-alert").fadeTo(2000, 500).slideUp(500, function () {
        $("#success-alert").slideUp(500);
    });
}

function OnFailedInsertion(XMLHttpRequest, textStatus, errorThrown) {

    $("#error-alert span").text(errorThrown);
    $("#error-alert").fadeTo(2000, 500).slideUp(500, function () {
        $("#error-alert").slideUp(500);
    });
}





function AddProduct_OnStoreChange() {

    var url = 'Product/FetchCategories'
    var categories = $('#Category_Id');
    $.getJSON(url, { StoreId: $("#Store_Id").val() }, function (response) {
        categories.empty();
        $.each(response, function (index, item) {
            categories.append($('<option></option>').text(item.Name).val(item.Id));
        });
    })
}

function OnStoreChange() {
    var url = 'FetchCategories'
    var categories = $('#ParentCategoryId');
    $.getJSON(url, { StoreId: $("#Store_Id").val() }, function (response) {
        categories.empty();
        $.each(response, function (index, item) {
            categories.append($('<option></option>').text(item.Name).val(item.Id));
        });
    })
}