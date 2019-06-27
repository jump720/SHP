function _UnlockUser(userDn) {

    jQuery.ajax({
        cache: false,
        type: "GET",
        url: "/LDAPAutenticador/UnlockUser",
        data: { "userDn": userDn },
        success: function (result) {
            alert(result);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Internal Error!!");
        }
    });
};

function _savePassword(userDn, oldPass, newPass) {

    jQuery.ajax({
        cache: false,
        type: "GET",
        url: "/LDAPAutenticador/changePassword",
        data: { "userDn": userDn, "oldPass": oldPass, "newPass": newPass },
        success: function (result) {
            alert(result);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Internal Error!");
        }
    });
};

function _resetPassword(userDn, newPassword) {

    jQuery.ajax({
        cache: false,
        type: "GET",
        url: "/LDAPAutenticador/resetPassword",
        data: { "userDn": userDn, "newPassword": newPassword },
        success: function (result) {
            location.href = "/";
            alert(result);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert("Internal Error!");
        }
    });
};

function validateChangePass(userDn) {
    var passOld = document.forms["formChangePass"]["passOld"].value;
    var passNew = document.forms["formChangePass"]["passNew"].value;
    var passConfirm = document.forms["myForm"]["passConfirm"].value;

    if (passOld == "" || passNew == "" || passConfirm == "") {
        alert("Must be filled out!");
        return false;
    } else if (passNew != passConfirm) {
        alert("The new password must be the same as the confirmation password");
        return false;
    } else {
        _savePassword(userDn, passOld, passNew);
    }
};

function validateResetPass(userDn) {
    var passReset = document.forms["formResetPass"]["passReset"].value;

    if (passReset == "") {
        alert("Must be filled out!");
        return false;
    } else {
        _resetPassword(userDn, passReset);
    }
};