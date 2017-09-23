
    function addClass(id) {
        $("input.search-field").removeAttr("id");
        $(id).attr('id', 'currentSearch');

    }
document.getElementById("PhoneNumber").addEventListener("keypress", function (evt) {

    if (evt.which >= 97 && evt.which <= 122) {
        evt.preventDefault();
    }
});


    navigator.geolocation.getCurrentPosition(function (position) {
        var latlng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
        geocoder = new google.maps.Geocoder();
        geocoder.geocode({ 'latLng': latlng }, function (results, status) {
            $(".search-field").val(results[0].formatted_address);

        });

    });
function GetLocationAddress() {

    $(".alert").html("");
    var places = new google.maps.places.Autocomplete(document.getElementById("currentSearch"));

    //google.maps.event.addListener(places, 'place_changed', function () {
    //   var place = places.getPlace();
    //    var address = place.formatted_address;

    //    var latitude = place.geometry.location.lat();
    //    var longitude = place.geometry.location.lng();
    //    $("#lat").val(latitude);
    //    $("#lng").val(longitude);

    //var mesg = "Address: " + address;
    //mesg += "\nLatitude: " + latitude;
    //mesg += "\nLongitude: " + longitude;
    //alert(mesg);
    //});
}


function SearchStore(StoreType, Address, FieldClass,Level) {

    if (Address.length < 1) {
        $("." + FieldClass).html("<p style='font-size:14px;color:red;'>Address field is required</p>").show().delay(3000).fadeOut();

    } else {
        //var Address=$("input[tabindex='0']").attr("value");
        //var lat=$("#lat").val();
        //var lng=$("#lng").val();
        //if (lat.length < 1 || lng.length < 1) {
        //    lat = 0;
        //    lng = 0;
        //}
        //$address = $(".search-field").val();
       //var url = 'SearchByAddress?search=test&Address=123&Level=00';
       // var url =  '@Url.Action("Home","SearchByAddress",new {Area="User",Type="test",Address="123",Level="00"})';
        var url = '@Html.Raw(@Url.Action("Home", "SearchByAddress",new {Area="User",Type="test",Address="123",Level="00"}))';
        url = url.replace("test", StoreType);
        url = url.replace('123', Address);
        url = url.replace('00', Level);
        window.location.href = url;

        //$.ajax({
        //    type: "GET",
        //    //contentType: "application/json; charset=utf-8",
        //    url: "Home/SearchByAddress",
        //    data: { Type: StoreType, Address: Address },
        //    success: function (result) {
        //        console.log(result);
        //    }
        //});

    }
}