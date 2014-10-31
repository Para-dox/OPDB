

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

function DisplayModal(e) {
    modalClose();
    var instructions = $(e).attr('data-validation');
    var url = $(e).attr('data-url');
    var title = $(e).attr('data-title');
    
    if (instructions == "false")
        $('#instructions').attr("hidden", true);
    else 
        $('#instructions').attr("hidden", false);
    

    $.ajax({
        type: 'POST',
        url: url,
        dataType: "html",
        contentType: "application/html; charset=utf-8",

        success: function (data) {
            $('#ajax-modal').find('.modal-header').find('h3').html(title);
            $('#ajax-modal').find('.modal-body').html(data);
            $('#ajax-modal').modal('show');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.status);
            alert(thrownError);
        }
    });
}

function modalClose() {
    $('#validation li').empty();
    $('#ajax-modal').modal('hide');
}

function loadAdminView(e) {
    document.getElementById('managementTables').innerHTML = "";
    document.getElementsByTagName("loading")[0].innerHTML = "<div style='text-align: center; width: 100%; height: 100%;'><i class='fa fa-spinner fa-spin' style='font-size: 10em; color: gray; margin-top: 5%;'></i></div>";
    var url = $(e).attr('data-url');
    var id = $(e).attr('id');
    var urlIDs = ["outreachURL", "activitiesURL", "schoolsURL", "resourcesURL", "unitsURL", "usersURL", "removedOutreachURL", "removedSchoolsURL", "removedUsersURL"];
    var liIDs = ["outreachAdmin", "activitiesAdmin", "schoolsAdmin", "resourcesAdmin", "unitsAdmin", "usersAdmin", "removed", "removed", "removed"];
    
    for (var i = 0; i < urlIDs.length; i++) {

        if (id == urlIDs[i])
        {
            $('.active').removeClass("active");
            $('#' + liIDs[i]).addClass("active");
            break;
        }

    }

    $('#managementTables').load(url);
    
}

function showInterest(e) {
    var url = $(e).attr('data-url');
    $.post(url);
    sleep(1000);
    window.location.reload();

}

function loadRightDetailsView(e) {
    document.getElementById('detailsRight').innerHTML = "";
    document.getElementsByTagName("loading")[0].innerHTML = "<div style='text-align: center; width: 100%; height: 100%;'><i class='fa fa-spinner fa-spin' style='font-size: 10em; color: gray; margin-top: 5%;'></i></div>";
    var url = $(e).attr('data-url');
    var id = $(e).attr('id');

    if (id == "verNotas") {
        $('#verNotas').attr("hidden", true);
        $('#verContactos').attr("hidden", false);
    }
    else {
        $('#verContactos').attr("hidden", true);
        $('#verNotas').attr("hidden", false);
    }

    $('#detailsRight').load(url);

}

function restoreRecord(e, table) {
    var url = $(e).attr('data-url');
    if (confirm("Este record será restaurado al sistema. ¿Esta seguro(a) que desea proceder?")) {
        $.post(url);
        sleep(1000);
        window.location.reload();
    }

    $(table).dataTable().fnDraw();

}