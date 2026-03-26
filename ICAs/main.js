var wsUrl = "https://localhost:7089";
$(document).ready(function () {
    LoadStudents();
});


/* -------------------------------------------------------
   AJAX WRAPPER
--------------------------------------------------------*/
function CallAJAX(url, data, type, dataType, fxnSuccess, fxnError) {
    let ajaxOptions = {};
    ajaxOptions["url"] = url;
    ajaxOptions["data"] = data;
    ajaxOptions["type"] = type;
    ajaxOptions["dataType"] = dataType;
    $.ajax(ajaxOptions).done(fxnSuccess).fail(fxnError);
}
function AjaxError(a, b, c) {
    console.log("AJAX ERROR:", b, c);
}


/* -------------------------------------------------------
   LOAD STUDENTS
--------------------------------------------------------*/
function LoadStudents() {
    CallAJAX(
        wsUrl + "/students",
        {},                 
        "GET",           
        "json",
        LoadStudentsSuccess,
        AjaxError
    );
}


function LoadStudentsSuccess(data) {
    var students = data.students;
    var html = "<table class='styled-table'>";
    html += "<tr><th>Get Students</th><th>Student ID</th><th>First Name</th><th>Last Name</th><th>School Id</th></tr>";
    for (var i = 0; i < students.length; i++) {
        var s = students[i];
        html += "<tr>";
        html += "<td><button class='btnGetClasses' data-id='" + s.studentId + "'>Retrieve Class Info</button></td>";
        html += "<td>" + s.studentId + "</td>";
        html += "<td>" + s.firstName + "</td>";
        html += "<td>" + s.lastName + "</td>";
        html += "<td>" + s.schoolId + "</td>";
        html += "</tr>";
    }
    html += "<tr><td colspan='5'>Retrieved : " + students.length + " student records</td></tr>";
    html += "</table>";
    $("#studentsArea").html(html);
    $("#classesArea").html("");


    $(".btnGetClasses").click(function () {
        var studentId = $(this).data("id");
        LoadClasses(studentId);
    });
}


/* -------------------------------------------------------
   LOAD CLASSES (REST)
--------------------------------------------------------*/
function LoadClasses(studentId) {
    CallAJAX(
        wsUrl + "/classes/" + studentId,
        {},                 // no data needed
        "GET",              // REST GET
        "json",
        LoadClassesSuccess,
        AjaxError
    );
}



function LoadClassesSuccess(data) {
    var classes = data.classes;
    if (classes.length === 0) {
        $("#classesArea").html("<p>No class records found for this student.</p>");
        return;
    }
    var html = "<table class='styled-table'>";
    html += "<tr>";
    html += "<th>Class ID</th>";
    html += "<th>Description</th>";
    html += "<th>Days</th>";
    html += "<th>Start Date</th>";
    html += "<th>Instructor ID</th>";
    html += "<th>First Name</th>";
    html += "<th>Last Name</th>";
    html += "</tr>";

    for (var i = 0; i < classes.length; i++) {
        var c = classes[i];
        html += "<tr>";
        html += "<td>" + c.classId + "</td>";
        html += "<td>" + c.classDesc + "</td>";
        html += "<td>" + c.days + "</td>";
        html += "<td>" + c.startDate + "</td>";
        html += "<td>" + c.instructorId + "</td>";
        html += "<td>" + c.instructorFirstName + "</td>";
        html += "<td>" + c.instructorLastName + "</td>";
        html += "</tr>";
    }
    html += "</table>";
    $("#classesArea").html(html);
}