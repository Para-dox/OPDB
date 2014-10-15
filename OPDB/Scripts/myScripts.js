

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

function removeRecord(e, table) {
    var url = $(e).attr('data-url');
    if (confirm("Este record será removido del sistema. ¿Esta seguro(a) que desea proceder?")) {
        $.post(url);
        sleep(1000);
        window.location.reload();
    }

    $(table).dataTable().fnDraw();

}

function getAdvancedSearchForm(e) {
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


function sleep(milliseconds) {
    var start = new Date().getTime();
    for (var i = 0; i < 1e7; i++) {
        if ((new Date().getTime() - start) > milliseconds) {
            break;
        }
    }
}

function loadResourceAction(e) {
    var url = $(e).attr('data-url');
    $('#resourceModal').load(url, function () {
        $(this).dialog('open');
    });
}


