$("input").keyup(function () {
    clearErrors();

});

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

function clearErrors() {
    $("#SuccessNotifications").hide();
   
}