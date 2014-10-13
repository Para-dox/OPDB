

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

function getAdvancedSearchForm(e) {
    debugger;
    var url = $(e).attr('data-url');
    var id = url.split("/Home/_PartialViewLoad?view=_")[1];
    
    if (id == "Alcance") {
        $('.active').removeClass("active");
        $('#alcance').addClass("active");
    } else if (id == "Actividades") {
        $('.active').removeClass("active");
        $('#actividades').addClass("active");
    } else if (id == "Escuelas") {
        $('.active').removeClass("active");
        $('#escuelas').addClass("active");
    } else if (id == "Unidades") {
        $('.active').removeClass("active");
        $('#unidades').addClass("active");
    } else {
        $('.active').removeClass("active");
        $('#usuarios').addClass("active");
    }

    $('#searchForm').load(url);
}

