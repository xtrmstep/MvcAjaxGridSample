(function($) {
    $.fn.ajaxGrid = function(options) {
        var settings = $.extend({
            // defaults
            url: {
                sort: "",
                page: ""
            },
            selectors: {
                columnsSelector: "[data-column]",
                pagingSelector: "[data-page]",
                optionsSelector: "[data-role='options']",
                filterSelector: "[data-role='filter']",
                bodySelector: "[data-role='body']",
                dialogSelector: "[data-role='dialog']"
            }
        }, options);
        var $this = $(this);

        return this.each(function() {
            var $filter = $this.find(settings.selectors.filterSelector);
            var $gridBody = $this.find(settings.selectors.bodySelector);
            var $dialog = $this.find(settings.selectors.dialogSelector);

            initSorting();
            initPaging();
            initFilter($filter);
            initRecordControls($gridBody);
            initDialog($dialog);
        });

        function initSorting() {
            $this.on("click", settings.selectors.columnsSelector, function() {
                var $column = $(this);
                var columnName = $column.data("column");
                var sortAscending = $column.data("sorting");
                var gridOptions = $this.find(settings.selectors.optionsSelector).val();
                var dto = $.postify({
                    sorting: { name: columnName, ascending: sortAscending },
                    options: gridOptions
                });
                $.post(settings.url.sort, dto)
                    .success(function(xhr, status, err) {
                        $this.html(xhr);
                    })
                    .error(function(xhr, status, err) {
                        if (xhr.responseText)
                            alert(xhr.responseText);
                        else
                            alert("Error is occurred.");
                    });
            });
        }

        function initFilter($filter) {
            
        }

        function initPaging() {
            $this.on("click", settings.selectors.pagingSelector, function () {
                var $pageButton = $(this);
                var pageIndex = $pageButton.data("page");
                var gridOptions = $this.find(settings.selectors.optionsSelector).val();
                var dto = $.postify({
                    index: pageIndex,
                    options: gridOptions
                });
                $.post(settings.url.page, dto)
                    .success(function (xhr, status, err) {
                        $this.html(xhr);
                    })
                    .error(function (xhr, status, err) {
                        if (xhr.responseText)
                            alert(xhr.responseText);
                        else
                            alert("Error is occurred.");
                    });
            });
        }

        function initRecordControls($gridBody) {
            
        }

        function initDialog() {
            
        }
    };
}(jQuery));


//$("[data-page]").click(function() {
//    var gridOptions = $("#gridOptions").val();
//    var data = { options: gridOptions };
//    if (data.pageIndex == "up" || data.pageIndex == "down") {
//        var currentPageIndex = $("[data-role='pageIndex']").val();
//        currentPageIndex = currentPageIndex + (data.pageIndex == "up" ? 1 : -1);
//        data.pageIndex = currentPageIndex;
//    }
//    $.post("@Url.Action("GoToPage", "Home")", data)
//        .success(function(xhr, status, err) {
//            $("[data-role='data']").html("");
//        });

//});

//function ShowEditDialog() {
//    $('#editBookDialog').modal();
//}
//function SuccessSaveEditDialog(data, status, xhr) {
//    $('#editBookDialog').modal('hide');
//}
//function FailureSaveEditDialog(xhr, status, err) {
//    if (status == "error")
//    {
//        if (xhr.responseJSON) {
//            $(".form-group").removeClass('has-error');
//            $(".form-group input").tooltip('destroy');
//            for (var i = 0; i < xhr.responseJSON.length; i++) {
//                var name = xhr.responseJSON[i].Field;
//                var errorMessage = xhr.responseJSON[i].ErrorMessage;
//                var selectorGroup = "[role='editor-" + name + "']";
//                $(selectorGroup).addClass('has-error');
//                var selectorEditor = "input[name='" + name + "']";
//                $(selectorEditor)
//                    .attr("data-toggle", "tooltip")
//                    .attr("data-placement", "top")
//                    .attr("title", errorMessage);
//                $(selectorEditor).tooltip();
//            }
//        } else
//            if (xhr.responseText) {
//            alert(xhr.responseText);
//        }
//    }
//}
function GetAntiForgeryToken() {
    var tokenField = $("input[type='hidden'][name$='RequestVerificationToken']");
    if (tokenField.length == 0) {
        return null;
    } else {
        return {
            name: tokenField[0].name,
            value: tokenField[0].value
        };
    }
}

$.ajaxPrefilter(
    function (options, localOptions, jqXHR) {
        if (options.type !== "GET") {
            var token = GetAntiForgeryToken();
            if (token !== null) {
                if (options.data.indexOf("X-Requested-With") === -1) {
                    options.data = "X-Requested-With=XMLHttpRequest" + ((options.data === "") ? "" : "&" + options.data);
                }
                options.data = options.data + "&" + token.name + '=' + token.value;
            }
        }
    }
 );

$.postify = function(value) {
    var result = {};

    var buildResult = function(object, prefix) {
        for (var key in object) {

            var postKey = isFinite(key)
                ? (prefix != "" ? prefix : "") + "[" + key + "]"
                : (prefix != "" ? prefix + "." : "") + key;

            switch (typeof (object[key])) {
                case "number":
                case "string":
                case "boolean":
                    result[postKey] = object[key];
                    break;

                case "object":
                    if (object[key].toUTCString)
                        result[postKey] = object[key].toUTCString().replace("UTC", "GMT");
                    else {
                        buildResult(object[key], postKey != "" ? postKey : key);
                    }
            }
        }
    };

    buildResult(value, "");

    return result;
};