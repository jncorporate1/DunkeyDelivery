function ClearProductDiv(id, name) {

    $("#productDetails").html("");
    // Clear Prodcut Div to reload new products on click

}
function SetCategory(id, name) {

    //changing active class from one li to other li on click

    $('.menu-list').children().removeClass('active');
    $("#item_" + id).addClass("active");

    $("#categoryName").text(name);
}

function openCity(evt, cityName) {
    // Declare all variables
    var i, tabcontent, tablinks;

    // Get all elements with class="tabcontent" and hide them
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }

    // Get all elements with class="tablinks" and remove the class "active"
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }

    // Show the current tab, and add an "active" class to the button that opened the tab
    document.getElementById(cityName).style.display = "block";
    evt.currentTarget.className += " active";


}
// Get the element with id="defaultOpen" and click on it
document.getElementById("defaultOpen").click();