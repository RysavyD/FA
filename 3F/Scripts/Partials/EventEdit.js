$(document).ready(function () {
    $("#toggleSubCategories").click(function () {
        if ($("#subCategories").is(':visible')) {
            $("#subCategories").hide(400);
            $("#toggleSubCategories").text("Rozbalit");
        } else {
            $("#subCategories").show(400);
            $("#toggleSubCategories").text("Skrýt");
        }
    });
});