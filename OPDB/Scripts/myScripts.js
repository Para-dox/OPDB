

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
    debugger;
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
    
    
    var urlIDs = ["outreachURL", "activeOutreachURL", "pendingOutreachURL", "removedOutreachURL", "activitiesURL", "schoolsURL", "activeSchoolsURL", "removedSchoolsURL", "resourcesURL", "unitsURL", "usersURL", "activeUsersURL", "removedUsersURL"];
    var liIDs = ["outreachAdmin", "activeOutreach", "pendingOutreach", "removedOutreach", "activitiesAdmin", "schoolsAdmin", "activeSchools", "removedSchools", "resourcesAdmin", "unitsAdmin", "usersAdmin", "activeUsers", "removedUsers"];
    
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

//function getUserTypeValue() {
//    if (document.getElementById('userTypes').value == 4) {
//        document.getElementById('faculty').removeAttribute("hidden", "hidden");
//        document.getElementById('collegeStudent').setAttribute("hidden", "hidden");
//        document.getElementById('grade').setAttribute("hidden", "hidden");
//    }
//    else if (document.getElementById('userTypes').value == 5) {
//        document.getElementById('collegeStudent').removeAttribute("hidden", "hidden");
//        document.getElementById('faculty').setAttribute("hidden", "hidden");
//        document.getElementById('grade').setAttribute("hidden", "hidden");
//    }
//    else if (document.getElementById('userTypes').value == 7) {
//        document.getElementById('grade').removeAttribute("hidden", "hidden");
//        document.getElementById('faculty').setAttribute("hidden", "hidden");
//        document.getElementById('collegeStudent').setAttribute("hidden", "hidden");
//    }
//    else {
//        document.getElementById('collegeStudent').setAttribute("hidden", "hidden");
//        document.getElementById('faculty').setAttribute("hidden", "hidden");
//        document.getElementById('grade').setAttribute("hidden", "hidden");
//    }
//}

function approve(e) {
    var url = $(e).attr('data-url');

    $.post(url);
    sleep(1000);

    window.location.reload();
}

function getAffiliateValue() {
    debugger;
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
    debugger;
    var id = $(e).attr('id');
    var dateParts = document.getElementById(id).value.split('/');
    var selectedDate = dateParts[1] + '/' + dateParts[0] + '/' + dateParts[2];
    selectedDate = new Date(selectedDate);    
    var currentDate = new Date((new Date()).setHours(0, 0, 0, 0));

    if(selectedDate <= currentDate)
        alert("La fecha escogida es menor o igual que hoy.")

}
