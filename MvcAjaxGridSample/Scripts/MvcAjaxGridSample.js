(function($) {
    $.fn.ajaxGrid = function(options) {
        var settings = $.extend({
            // defaults
            url: {
                sort: "",
                page: "",
                filter: "",
                edit: "",
                delete: "",
                save: ""
            },
            selectors: {
                columnsSelector: "[data-column]",

                pagingSelector: "[data-page]",

                filterApplySelector: "[data-role='filterApply']",
                filterClearSelector: "[data-role='filterClear']",
                filterInputsSelector: "[data-filter]",

                optionsSelector: "[data-role='options']",

                recordEditSelector: "[data-role='recordEdit']",
                recordDeleteSelector: "[data-role='recordDelete']",

                dialogContainerSelector: "[data-role='editDialog']",
                dialogSaveSelector: "[data-role='editSave']"
            }
        }, options);
        var $this = $(this);

        return this.each(function() {
            initSorting();
            initPaging();
            initFilter();
            initRecordActions();
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

        function initFilter() {
            // apply filter
            $this.on("click", settings.selectors.filterApplySelector, function () {
                var $filters = $this.find(settings.selectors.filterInputsSelector);
                var filterArray = new Array();
                $filters.each(function() {
                    var filterInput = $(this);
                    var fieldName = filterInput.data("filter");
                    var fieldValue = filterInput.val();
                    filterArray.push({ name: fieldName, value: fieldValue });
                });
                var gridOptions = $this.find(settings.selectors.optionsSelector).val();
                var dto = $.postify({
                    filter: filterArray,
                    options: gridOptions
                });
                $.post(settings.url.filter, dto)
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
            // clear filter
            $this.on("click", settings.selectors.filterClearSelector, function () {
                var gridOptions = $this.find(settings.selectors.optionsSelector).val();
                var dto = $.postify({
                    options: gridOptions
                });
                $.post(settings.url.filter, dto)
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

        function initRecordActions() {
            // delete items
            $this.on("click", settings.selectors.recordDeleteSelector, function() {
                if (confirm("Are you sure?")) {
                    var recordId = $(this).data("id");
                    var gridOptions = $this.find(settings.selectors.optionsSelector).val();
                    var dto = $.postify({
                        id: recordId,
                        options: gridOptions
                    });
                    $.post(settings.url.delete, dto)
                        .success(function(xhr, status, err) {
                            $this.html(xhr);
                        })
                        .error(function(xhr, status, err) {
                            if (xhr.responseText)
                                alert(xhr.responseText);
                            else
                                alert("Error is occurred.");
                        });
                }
            });
            // edit items
            $this.on("click", settings.selectors.recordEditSelector, function() {
                var recordId = $(this).data("id");
                var dto = $.postify({
                    id: recordId
                });
                $.post(settings.url.edit, dto)
                    .success(function (xhr, status, err) {
                        var $dialogContainer = $(settings.selectors.dialogContainerSelector);
                        $dialogContainer.html(xhr);
                        var $dialog = $dialogContainer.find("#dialog");
                        $dialog.modal();
                    })
                    .error(function(xhr, status, err) {
                        if (xhr.responseText)
                            alert(xhr.responseText);
                        else
                            alert("Error is occurred.");
                    });
            });

            $(document).on("click", settings.selectors.dialogSaveSelector, function () {
                var $dialogContainer = $(settings.selectors.dialogContainerSelector);
                var dialogData = getPostData($dialogContainer);
                var gridOptions = $this.find(settings.selectors.optionsSelector).val();

                var dto = $.postify({
                    bookEditViewModel: dialogData,
                    options: gridOptions
                });
                $.post(settings.url.save, dto)
                    .success(function (xhr, status, err) {
                        var $dialog = $dialogContainer.find("#dialog");
                        $this.html(xhr);
                        $dialog.modal('hide');
                    })
                    .error(function (xhr, status, err) {
                        if (status == "error")
                        {
                            if (xhr.responseJSON) {
                                $(".form-group").removeClass('has-error');
                                $(".form-group input").tooltip('destroy');
                                for (var i = 0; i < xhr.responseJSON.length; i++) {
                                    var name = xhr.responseJSON[i].Field.replace("bookEditViewModel.", "");
                                    var errorMessage = xhr.responseJSON[i].ErrorMessage;
                                    var selectorGroup = "[data-role='editor-" + name + "']";
                                    $(selectorGroup).addClass('has-error');
                                    var selectorEditor = "input[name='" + name + "']";
                                    $(selectorEditor)
                                        .attr("data-toggle", "tooltip")
                                        .attr("data-placement", "top")
                                        .attr("title", errorMessage);
                                    $(selectorEditor).tooltip();
                                }
                            } else
                                if (xhr.responseText) {
                                alert(xhr.responseText);
                            }
                        }
                    });
            });
        }

        function getPostData($element) {
            var data = {};
            if ($element) {
                var $inputs = $element.find("input");
                $inputs.each(function() {
                    var name = this.name;
                    var val = this.value;
                    if (name && val)
                        data[name] = val;
                });
            }
            return data;
        }
    };
}(jQuery));

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