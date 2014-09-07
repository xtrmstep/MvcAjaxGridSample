function ShowEditDialog() {
    $('#editBookDialog').modal();
}
function SuccessSaveEditDialog(data, status, xhr) {
    $('#editBookDialog').modal('hide');
}
function FailureSaveEditDialog(xhr, status, err) {
    if (status == "error")
    {
        if (xhr.responseJSON) {
            $(".form-group").removeClass('has-error');
            $(".form-group input").tooltip('destroy');
            for (var i = 0; i < xhr.responseJSON.length; i++) {
                var name = xhr.responseJSON[i].Field;
                var errorMessage = xhr.responseJSON[i].ErrorMessage;
                var selectorGroup = "[role='editor-" + name + "']";
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
}
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