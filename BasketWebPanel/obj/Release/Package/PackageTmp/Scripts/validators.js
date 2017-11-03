$('#btnDeleteNo').on("click", function (e) {
    $('#deleteModal').modal('hide');
    var lblError = $("#lblErrorOnDelete");
    lblError.text("");
    lblError.hide();
});

$('#btnDeleteClose').on("click", function (e) {
    $('#deleteModal').modal('hide');
    var lblError = $("#lblErrorOnDelete");
    lblError.text("");
    lblError.hide();
});

// CSRF (XSRF) security
function addAntiForgeryToken(data) {
    //if the object is undefined, create a new one.
    if (!data) {
        data = {};
    }
    //add token
    var tokenInput = $('input[name=__RequestVerificationToken]');
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
};

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

function OnSuccessInsertion() {
    //$("#success-alert").append("Store updated successfully.");
    $("#success-alert").fadeTo(2000, 500).slideUp(500, function () {
        $("#success-alert").slideUp(500);
    });
}



function OnFailedInsertion(XMLHttpRequest, textStatus, errorThrown) {
    if (XMLHttpRequest.status == 404) {
        $("#ImageError").text(errorThrown);
        $("#clearImage").trigger("click");
    }
    else {
        $("#error-alert span").text(errorThrown);
        $("#error-alert").fadeTo(2000, 500).slideUp(500, function () {
            $("#error-alert").slideUp(500);
        });
    }
}


function OnStoreChange() {
    var url = 'Categories/FetchCategories'
    var categories = $('#Category_ParentCategoryId');
    $.getJSON(url, { StoreId: $("#Category_Store_Id").val() }, function (response) {
        categories.empty();
        $.each(response, function (index, item) {
            categories.append($('<option></option>').text(item.Name).val(item.Id));
        });
    })
}


function ClearValidationErrorMessages()
{
    $("span.text-danger , .field-validation-valid").html("");
}
