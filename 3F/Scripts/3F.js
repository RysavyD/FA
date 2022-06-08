function ShowWaitDialog() {
    bootbox.dialog({
        message: "Zpracovávám ....",
        closeButton: false,
    });
};

function HideWaitDialog() {
    bootbox.hideAll();
};

function MakePostCall(url, data, callback) {
    ShowWaitDialog();
    $.ajax({
        type: "POST",
        url: url,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify(data),
        success: function (result) {
            if (result == ""){
                if (callback == undefined)
                    location.reload(true);
                else
                    callback();
            }
            else {
                HideWaitDialog();
                alert(result);
                eventLoginClick = true;
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            HideWaitDialog();
            alert("Nastala chyba \n\n" + xhr.responseText);
            eventLoginClick = true;
        }
    });
};

function GeneratePagination(selector, callback, items, startPage, pageSize) {
    if (items == 0)
        return;
    console.log(items);
    console.log("pagesize:" + (pageSize || 10));
    $(selector).twbsPagination({
        totalPages: Math.ceil(items / (pageSize || 10)),
        visiblePages: 10,
        startPage: (startPage) ? startPage : 1,
        onPageClick: function (event, page) {
            callback(page);
        }
    });
};

function MakeGetCall(selector, url, page, callback) {
    $(selector + "ErrorMessage").hide();
    $(selector + "Spinner").show();
    $(selector + "LoadBox").show();
    $(selector).empty();

    $.getJSON(url, { page: page })
                        .done(function (data) {
                            $(selector + "LoadBox").hide();
                            var templateWithData = Mustache.to_html($("#" + data.Template).html(), data);
                            $(selector).html(templateWithData);
                            $(selector + "Title").text(data.Name);
                            if (callback) {
                                callback(data);
                            }
                        })
                        .fail(function () {
                            $(selector + "Spinner").hide();
                            $(selector + "ErrorMessage").show();
                            $(selector + "LoadBox").show();
                        });
}

function FilterItems(text, itemsSelector, countSelector) {
    var allObjects = $(itemsSelector);
    if (text) {
        allObjects.each(function () {
            ($(this).filter("[data-filter*='" + text + "']").length != 0) ?
                $(this).show() :
                $(this).hide();
        });

        if (countSelector) {
            $(countSelector).text($(itemsSelector + ":visible").length);
        }
    } else {
        allObjects.show();
        if (countSelector) {
            $(countSelector).text(allObjects.length);
        }
    }
}