function ShowEditDialog() {
    $('#editBookDialog').modal();
}
function SuccessSaveEditDialog(data, status, xhr) {
    $('#editBookDialog').modal('hide');
}
function FailureSaveEditDialog(xhr, status, err) {
    if (xhr.status == 500) // internal server error
    {
        //if (xhr.responseJSON) {

        //} else
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