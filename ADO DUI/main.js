var wsUrl = "https://localhost:7245";

/* =====================================================
   AJAX WRAPPER
===================================================== */
function CallAJAX(url, data, type, dataType, success, error) {

    let opt = {};
    opt["url"] = url;
    opt["type"] = type;
    opt["dataType"] = dataType;

    // For POST and PUT we must JSON.stringify + send proper header
    if (type === "POST" || type === "PUT") {
        opt["data"] = JSON.stringify(data);
        opt["contentType"] = "application/json";
    } else {
        opt["data"] = data;
    }

    $.ajax(opt).done(success).fail(error);
}

function AjaxError(a, b, c) {
    console.log("AJAX ERROR:", b, c);
}

/* =====================================================
   PAGE LOAD
===================================================== */
$(document).ready(function () {
    LoadStudents();
    LoadClassList();
    $("#btnAddStudent").click(AddStudent);
});

/* =====================================================
   LOAD STUDENTS
===================================================== */
function LoadStudents() {

    CallAJAX(wsUrl + "/students", {}, "GET", "json",
        LoadStudentsSuccess, AjaxError);
}

function LoadStudentsSuccess(data) {

    var list = data.students;

    var html = "<table class='styled-table'>";
    html += "<tr><th>Get</th><th>ID</th><th>First</th><th>Last</th><th>School</th><th>Action</th></tr>";

    for (var i = 0; i < list.length; i++) {

        var s = list[i];

        html += "<tr>";
        html += "<td><button class='btnGetClasses' data-id='" + s.studentId + "'>Retrieve Class Info</button></td>";
        html += "<td>" + s.studentId + "</td>";
        html += "<td class='fn'>" + s.firstName + "</td>";
        html += "<td class='ln'>" + s.lastName + "</td>";
        html += "<td class='sc'>" + s.schoolId + "</td>";

        html += "<td>";
        html += "<button class='btnDelete' data-id='" + s.studentId + "'>Delete</button> ";
        html += "<button class='btnEdit' data-id='" + s.studentId + "'>Edit</button>";
        html += "</td>";

        html += "</tr>";
    }

    html += "</table>";

    $("#studentsArea").html(html);
    $("#classesArea").html("");

    $(".btnGetClasses").click(function () {
        LoadClasses($(this).data("id"));
    });
}

/* =====================================================
   LOAD CLASSES
===================================================== */
function LoadClasses(id) {

    CallAJAX(wsUrl + "/students/" + id + "/classes",
        {}, "GET", "json", LoadClassesSuccess, AjaxError);
}

function LoadClassesSuccess(data) {

    var list = data.classes;

    if (list.length === 0) {
        $("#classesArea").html("<p>No class records.</p>");
        return;
    }

    var html = "<table class='styled-table'>";
    html += "<tr><th>ID</th><th>Desc</th><th>Days</th><th>Start</th></tr>";

    for (var i = 0; i < list.length; i++) {
        var c = list[i];

        html += "<tr>";
        html += "<td>" + c.classId + "</td>";
        html += "<td>" + c.classDesc + "</td>";
        html += "<td>" + c.days + "</td>";
        html += "<td>" + c.startDate + "</td>";
        html += "</tr>";
    }

    html += "</table>";
    $("#classesArea").html(html);
}

/* =====================================================
   DELETE STUDENT
===================================================== */
$(document).on("click", ".btnDelete", function () {

    let id = $(this).data("id");

    CallAJAX(wsUrl + "/students/" + id,
        {}, "DELETE", "json",
        function () { LoadStudents(); },
        AjaxError);
});

/* =====================================================
   EDIT STUDENT
===================================================== */
$(document).on("click", ".btnEdit", function () {

    var row = $(this).closest("tr");
    var id = $(this).data("id");

    var fn = row.find(".fn").text();
    var ln = row.find(".ln").text();
    var sc = row.find(".sc").text();

    row.find(".fn").html("<input class='editFN' value='" + fn + "'>");
    row.find(".ln").html("<input class='editLN' value='" + ln + "'>");
    row.find(".sc").html("<input class='editSC' value='" + sc + "'>");

    $(this).parent().html(
        "<button class='btnUpdate' data-id='" + id + "'>Update</button> " +
        "<button class='btnCancel'>Cancel</button>"
    );
});

$(document).on("click", ".btnCancel", function () {
    LoadStudents();
});

/* =====================================================
   UPDATE STUDENT
===================================================== */
$(document).on("click", ".btnUpdate", function () {

    var id = $(this).data("id");
    var row = $(this).closest("tr");

    var fn = row.find(".editFN").val();
    var ln = row.find(".editLN").val();
    var sc = row.find(".editSC").val();

    var d = {};
    d["StudentId"] = id;
    d["FirstName"] = fn;
    d["LastName"] = ln;
    d["SchoolId"] = sc;

    CallAJAX(wsUrl + "/students/update",
        d, "PUT", "json",
        function () { LoadStudents(); },
        AjaxError);
});

/* =====================================================
   LOAD CLASS LIST
===================================================== */
function LoadClassList() {

    CallAJAX(wsUrl + "/classes",
        {}, "GET", "json",
        LoadClassListSuccess, AjaxError);
}

function LoadClassListSuccess(data) {
    var list = data.classes;
    var html = "";

    for (var i = 0; i < list.length; i++) {
        html += "<option value='" + list[i].classId + "'>" +
            list[i].classId + " - " + list[i].classDesc +
            "</option>";
    }

    $("#addClasses").html(html);
}

/* =====================================================
   ADD STUDENT
===================================================== */
function AddStudent()
{
    var d = {};
    d["FirstName"] = $("#addFirst").val();
    d["LastName"]  = $("#addLast").val();
    d["SchoolId"]  = parseInt($("#addSchool").val());
    d["ClassIds"] = $("#addClasses").val();



    console.log("Sending:", d);

    CallAJAX(wsUrl + "/students/add", d, "POST", "json", AddSuccess, AjaxError);
}




function AddSuccess(data) {

    if (data.error) {
        $("#addStatus").text(data.error);
        return;
    }

    $("#addStatus").text("");   // clear previous errors
    $("#addStatus").text(data.message);

    LoadStudents();
}