jQuery.SHP.Answer = function (mod) {
    mod = mod.toLowerCase();

    if (mod == "SetUser" || mod == "SecurityQuestions" || mod == "delete") {
        var successMsg, errorMsg, validate;

        if (mod == "delete") {
            validate = false;
            successMsg = "deleted";
            errorMsg = "deleting";
        }
        else {
            validate = true;
            successMsg = "saved";
            errorMsg = "saving";
        }

        jQuery("#frmSetUser").submit(function (e) {
            e.preventDefault();

            if (validate && !jQuery(this).valid())
                return;

            sLoading();
            jQuery.post(this.action, validate ? jQuery(this).serialize() : null)
                .done(function (result) {
                    if (result) {
                        hModal(getCurrentModalId());
                        msgSuccess(`Record ${successMsg}.`);
                    }
                    else
                        msgError(`Error ${errorMsg} the record.`);
                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    msgError(errorThrown);
                })
                .always(function () {
                    hLoading();
                });
        });
    }

    if (mod == "SetUser") {
        $("#UserId").focus();
    }
    //else if (mod == "details" || mod == "delete") {
    //    $("#bodyForm input[type=text]").prop("readonly", true);
    //}
};

function validateUserId(userDn) {
    //var userId = document.forms["formSetUser"]["UserId"].value;

    if (userDn == "" || userDn == null) {
        alert("Must be filled out!");
        return false;
    } else {
        _getSecurityQuestios(userDn);
    }
};

function _getSecurityQuestios(userDn) {

    jQuery.ajax({
        cache: false,
        type: "GET",
        url: "/Answers/getSecurityQuestios",
        data: { "userDn": userDn },
        success: function (result) {
            alert(result);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Internal Error!!");
        }
    });
};

function ValidationUserId() {
    jQuery('#Send').attr("disabled", true);
    var y = 0;
    var textVal = document.getElementById('UserId').value;
    if (textVal == "") {
        document.getElementById('UserId').className = "invalid";
        document.getElementById('Validation').innerHTML = "";
        var text = document.createTextNode("The fields can not be empty");
        document.getElementById('Validation').appendChild(text);
        jQuery('#Send').attr("disabled", false);
        return false;
    }
    else{
        return true;
    }
};

function ValidationEvent() {
    $('#Send').attr("disabled", true);
    var questionCount = document.getElementById('UltimoItem').value;
    var y = 0;
    for (var i = 1; i <= questionCount; i++) {
        var textVal = document.getElementById('AnswerDesc_' + i).value;
        if (textVal == "") {
            y = y + 1;
            document.getElementById('AnswerDesc_' + i).className = "invalid";
            document.getElementById('Validation').innerHTML = "";
            var text = document.createTextNode("The fields can not be empty");
            document.getElementById('Validation').appendChild(text);
            $('#Send').attr("disabled", false);
            return false;
            break;
        }
        if (y == 0 && i == questionCount) {
            return true;
        }
    }
};