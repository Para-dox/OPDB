

function getValue() {
    if (document.getElementById('userType').value == 3) {
        document.getElementById('outreachEntity').removeAttribute("hidden", "hidden");
        document.getElementById('user').setAttribute("hidden", "hidden");
    }
    else {
        document.getElementById('user').removeAttribute("hidden", "hidden");
        document.getElementById('outreachEntity').setAttribute("hidden", "hidden");
    }
}

function removeRecord(e) {
    debugger;
    var url = $(e).attr('data-url');
    if (confirm("Este record será removido del sistema. ¿Esta seguro(a) que desea proceder?")) {
        $.post(url);
        window.location.reload();

    }

}

