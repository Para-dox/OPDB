

function getValue() {
    var outreachEntity = document.getElementById('outreachEntity');
    var user = document.getElementById('user');
    var faculty = document.getElementById('faculty');
    var collegeStudent = document.getElementById('collegeStudent');
    var grade = document.getElementById('grade');

    if (document.getElementById('userType').value == 3) {

        if(outreachEntity != null)
            outreachEntity.removeAttribute("hidden", "hidden");

        if(user != null)
            user.setAttribute("hidden", "hidden");
    }

    else {
        if (user != null)
            user.removeAttribute("hidden", "hidden");

        if (outreachEntity != null)
            outreachEntity.setAttribute("hidden", "hidden");
    }

    if (document.getElementById('userType').value == 4) {
        if(faculty != null)
            faculty.removeAttribute("hidden", "hidden");

        if(collegeStudent != null)
            collegeStudent.setAttribute("hidden", "hidden");

        if(grade != null)
            grade.setAttribute("hidden", "hidden");
    }
    else if (document.getElementById('userType').value == 5) {
        if(collegeStudent != null)
            collegeStudent.removeAttribute("hidden", "hidden");
        if(faculty != null)
            faculty.setAttribute("hidden", "hidden");
        if(grade != null)
            grade.setAttribute("hidden", "hidden");
    }
    else if (document.getElementById('userType').value == 6 || document.getElementById('userType').value == 7) {
        if(grade != null)
            grade.removeAttribute("hidden", "hidden");
        if(faculty != null)
            faculty.setAttribute("hidden", "hidden");
        if(collegeStudent != null)
            collegeStudent.setAttribute("hidden", "hidden");
    }
    else {
        if(collegeStudent != null)
            collegeStudent.setAttribute("hidden", "hidden");
        if(faculty != null)
            faculty.setAttribute("hidden", "hidden");
        if(grade != null)
            grade.setAttribute("hidden", "hidden");
    }
}

function removeRecord(e, table) {
    var url = $(e).attr('data-url');
    if (confirm("Este record será removido del sistema. ¿Esta seguro(a) que desea proceder?")) {
        $.post(url, null, function (e) { alert(e);});
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
    var instructions = $(e).attr('data-validation');
    var url = $(e).attr('data-url');
    var title = $(e).attr('data-title');
    var calendar = $('fc-view-container');
    
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
    var url = $(e).attr('data-url');
    var id = $(e).attr('id');

    if (id.search("active") != -1 || id.search("pending") != -1 || id.search("removed") != -1)
    {
        document.getElementById('managementTableInner').innerHTML = "";
        document.getElementsByTagName("loadinginner")[0].innerHTML = "<div style='text-align: center; width: 100%; height: 100%;'><i class='fa fa-spinner fa-spin' style='font-size: 10em; color: gray; margin-top: 5%;'></i></div>";
    }
    else
    {
        document.getElementById('managementTables').innerHTML = "";
        document.getElementsByTagName("loading")[0].innerHTML = "<div style='text-align: center; width: 100%; height: 100%;'><i class='fa fa-spinner fa-spin' style='font-size: 10em; color: gray; margin-top: 5%;'></i></div>";
        
    }            
    
    
    var urlIDs = ["outreachURL", "activeOutreachURL", "pendingOutreachURL", "removedOutreachURL", "activitiesURL", "schoolsURL", "activeSchoolsURL", "removedSchoolsURL", "resourcesURL", "unitsURL", "usersURL", "activeUsersURL", "removedUsersURL", "reportsURL"];
    var liIDs = ["outreachAdmin", "activeOutreach", "pendingOutreach", "removedOutreach", "activitiesAdmin", "schoolsAdmin", "activeSchools", "removedSchools", "resourcesAdmin", "unitsAdmin", "usersAdmin", "activeUsers", "removedUsers", "reportsAdmin"];
    
    for (var i = 0; i < urlIDs.length; i++) {

        if (id == urlIDs[i])
        {
            if (id.search("active") != -1 || id.search("pending") != -1 || id.search("removed") != -1) {
                $('.sub-nav').find('.active').removeClass("active");               
            }
            else {
                $('#adminBar').find('.active').removeClass("active");
            }
            $('#' + liIDs[i]).addClass("active");            
            break;
        }

    }

    if (url == "false")
        url = $('#' + id).attr('data-url');

    if (id.search("active") != -1 || id.search("pending") != -1 || id.search("removed") != -1) {
        $('#managementTableInner').load(url);
    }
    else {
        $('#managementTables').load(url);
    }

    
    
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

function approve(e) {
    var url = $(e).attr('data-url');

    $.post(url);
    sleep(1000);

    window.location.reload();
}

function getAffiliateValue() {
    if (document.getElementById('affiliateTypes').value == "School") {
        document.getElementById('schools').removeAttribute("hidden", "hidden");
        document.getElementById('outreachEntities').setAttribute("hidden", "hidden");
        $('#outreachEntity').val("");
        document.getElementById('units').setAttribute("hidden", "hidden");
        $('#unit').val("");
    } else if (document.getElementById('affiliateTypes').value == "Outreach Entity") {
        document.getElementById('outreachEntities').removeAttribute("hidden", "hidden");
        document.getElementById('schools').setAttribute("hidden", "hidden");
        $('#school').val("");
        document.getElementById('units').setAttribute("hidden", "hidden");
        $('#unit').val("");
    } else if (document.getElementById('affiliateTypes').value == "Unit") {
        document.getElementById('units').removeAttribute("hidden", "hidden");
        document.getElementById('outreachEntities').setAttribute("hidden", "hidden");
        $('#outreachEntity').val("");
        document.getElementById('schools').setAttribute("hidden", "hidden");
        $('#school').val("");
    } else {
        document.getElementById('schools').setAttribute("hidden", "hidden");
        $('#school').val("");
        document.getElementById('units').setAttribute("hidden", "hidden");
        $('#unit').val("");
        document.getElementById('outreachEntities').setAttribute("hidden", "hidden");
        $('#outreachEntity').val("");
    }
}

function approve(e) {
    var url = $(e).attr('data-url');

    $.post(url);
    sleep(1000);

    window.location.reload();
}

function getActivityType() {
    if (document.getElementById('activityType').value == 3) {
        document.getElementById('dynamic').setAttribute("hidden", "hidden");
        $('#activityDynamic').val("");
    } else {
        document.getElementById('dynamic').removeAttribute("hidden", "hidden");
    }
}

function compareDate(e) {
    var id = $(e).attr('id');
    var dateParts = document.getElementById(id).value.split('/');
    var selectedDate = dateParts[1] + '/' + dateParts[0] + '/' + dateParts[2];
    selectedDate = new Date(selectedDate);    
    var currentDate = new Date((new Date()).setHours(0, 0, 0, 0));

    if(selectedDate <= currentDate)
        alert("La fecha escogida es menor o igual que hoy.")

}

function displayReturn() 
{
    if (sessionStorage.getItem('Route') != undefined && sessionStorage.getItem('Route') != "") {
        var details = document.getElementById('details');
        var admin = document.getElementById('admin');
        var list = document.getElementById('list');
        var home = document.getElementById('home');

        if (sessionStorage.getItem('Route') == "admin") {
            admin.removeAttribute("hidden", "hidden");
            list.setAttribute("hidden", "hidden");
            if(details != null)
                details.setAttribute("hidden", "hidden");
            if (home != null)
                home.setAttribute("hidden", "hidden");
        } else if (sessionStorage.getItem('Route') == "list") {
            list.removeAttribute("hidden", "hidden");
            admin.setAttribute("hidden", "hidden");
            if (details != null)
                details.setAttribute("hidden", "hidden");
            if (home != null)
                home.setAttribute("hidden", "hidden");        
        } else if (sessionStorage.getItem('Route') == "details") {
            if (details != null)
                details.removeAttribute("hidden", "hidden");
            admin.setAttribute("hidden", "hidden");
            if(list != null)
                list.setAttribute("hidden", "hidden");
            if (home != null)
                home.setAttribute("hidden", "hidden");            
        } else if (sessionStorage.getItem('Route') == "home") {
            if (home != null)
                home.removeAttribute("hidden", "hidden");
            admin.setAttribute("hidden", "hidden");
            if (list != null)
                list.setAttribute("hidden", "hidden");
            if (details != null)
                details.setAttribute("hidden", "hidden");
        }


    }
}

function activityReturn() {
    if (sessionStorage.getItem('Activity') != undefined && sessionStorage.getItem('Activity') != "") {
        var home = document.getElementById('home');
        var details = document.getElementById('details');
        var admin = document.getElementById('admin');
        var list = document.getElementById('list');
        var calendar = document.getElementById('calendar');
        var interest = document.getElementById('interest');

        if (sessionStorage.getItem('Activity') == "admin") {
            admin.removeAttribute("hidden", "hidden");
            list.setAttribute("hidden", "hidden");
            home.setAttribute("hidden", "hidden");
            details.setAttribute("hidden", "hidden");
            calendar.setAttribute("hidden", "hidden");
            interest.setAttribute("hidden", "hidden");
        } else if (sessionStorage.getItem('Activity') == "list") {
            list.removeAttribute("hidden", "hidden");
            admin.setAttribute("hidden", "hidden");
            home.setAttribute("hidden", "hidden");
            details.setAttribute("hidden", "hidden");
            calendar.setAttribute("hidden", "hidden");
            interest.setAttribute("hidden", "hidden");
        } else if (sessionStorage.getItem('Activity') == "home") {
            home.removeAttribute("hidden", "hidden");
            admin.setAttribute("hidden", "hidden");
            list.setAttribute("hidden", "hidden");
            details.setAttribute("hidden", "hidden");
            calendar.setAttribute("hidden", "hidden");
            interest.setAttribute("hidden", "hidden");
        } else if (sessionStorage.getItem('Activity') == "details") {
            details.removeAttribute("hidden", "hidden");
            admin.setAttribute("hidden", "hidden");
            home.setAttribute("hidden", "hidden");
            list.setAttribute("hidden", "hidden");
            calendar.setAttribute("hidden", "hidden");
            interest.setAttribute("hidden", "hidden");
        } else if (sessionStorage.getItem('Activity') == "calendar") {
            calendar.removeAttribute("hidden", "hidden");
            details.setAttribute("hidden", "hidden");
            admin.setAttribute("hidden", "hidden");
            home.setAttribute("hidden", "hidden");
            list.setAttribute("hidden", "hidden");
            interest.setAttribute("hidden", "hidden");
        } else if (sessionStorage.getItem('Activity') == "interest") {
            interest.removeAttribute("hidden", "hidden");
            calendar.setAttribute("hidden", "hidden");
            details.setAttribute("hidden", "hidden");
            admin.setAttribute("hidden", "hidden");
            home.setAttribute("hidden", "hidden");
            list.setAttribute("hidden", "hidden");
        }


    }
}

function cancelReturn() {
    if (sessionStorage.getItem('Cancel') != undefined && sessionStorage.getItem('Cancel') != "") {
        var details = document.getElementById('details');
        var admin = document.getElementById('admin');

        if (sessionStorage.getItem('Cancel') == "admin") {
            admin.removeAttribute("hidden", "hidden");
            details.setAttribute("hidden", "hidden");
            
        } else if (sessionStorage.getItem('Cancel') == "details") {
            details.removeAttribute("hidden", "hidden");
            admin.setAttribute("hidden", "hidden");
       } 


    }
}
