$(window).on("load", function () {
    document.getElementsByTagName("input").value = "";

    document.getElementById("preventAlphabet").addEventListener("keypress", function (evt) {

        if (evt.which >= 97 && evt.which <= 122) {
            evt.preventDefault();
        }
    });


});
function clear_errors() {
    $("span.text-danger").html("");
    $("#clearImage").trigger("click");
}
$(document).on('click', '#close-preview', function () {
    $('.image-preview').popover('hide');
    // Hover befor close the preview
    $('.image-preview').hover(
        function () {
            $('.image-preview').popover('show');
        },
         function () {
             $('.image-preview').popover('hide');
         }
    );
});

$(function () {
    // Create the close button
    var closebtn = $('<button/>', {
        type: "button",
        text: 'x',
        id: 'close-preview',
        style: 'font-size: initial;',
    });
    closebtn.attr("class", "close pull-right");
    // Set the popover default content
    $('.image-preview').popover({
        trigger: 'manual',
        html: true,
        title: "<strong>Preview</strong>" + $(closebtn)[0].outerHTML,
        content: "There's no image",
        placement: 'bottom'
    });
    // Clear event
    $('.image-preview-clear').click(function () {
        $('.image-preview').attr("data-content", "").popover('hide');
        $('.image-preview-filename').val("");
        $('.image-preview-clear').hide();
        $('.image-preview-input input:file').val("");
        $(".image-preview-input-title").text("Browse");
    });
    // Create the preview image
    $(".image-preview-input input:file").change(function () {
        var AllowedFileTypes = ["image/jpeg", "image/png", "image/jpg"];
        var img = $('<img/>', {
            id: 'dynamic',
            width: 250,
            height: 200
        });
       
        var file = this.files[0];
        if (AllowedFileTypes.indexOf(file.type) == -1) {
            $("#InvalidImageError").html("Only .Jpeg & .Png File Accepted");
            $("#clearImage").trigger("click");
            return false;
        }
        $("#InvalidImageError").html(" ");
            var reader = new FileReader();
            // Set preview image into the popover data-content
            reader.onload = function (e) {
                $(".image-preview-input-title").text("Change");
                $(".image-preview-clear").show();
                $(".image-preview-filename").val(file.name);
                img.attr('src', e.target.result);
                $(".image-preview").attr("data-content", $(img)[0].outerHTML).popover("show");
            }
            reader.readAsDataURL(file);
        
      });
});
