var wsUrl = "http://localhost:5209";

/* =====================================================
   AJAX WRAPPER (ICA09 style)
===================================================== */
function CallAJAX(url, data, type, dataType, success, error) {

    let opt = {};
    opt["url"] = url;
    opt["type"] = type;
    opt["dataType"] = dataType;

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
    LoadLocations();

    // ONLY the location change loads orders (as per ICA10 instructions)
    $("#locations").change(function () {
        LoadOrders();
    });
});

/* =====================================================
   LOAD LOCATIONS
===================================================== */
function LoadLocations() {

    CallAJAX(wsUrl + "/locations",
        {},
        "GET",
        "json",
        LoadLocationsSuccess,
        AjaxError
    );
}

function LoadLocationsSuccess(data) {

    var list = data.locations;
    var html = "<option value=''>Select Location</option>";

    for (var i = 0; i < list.length; i++) {
        var l = list[i];
        html += "<option value='" + l.locationId + "'>" +
            l.locationName +
            "</option>";
    }

    $("#locations").html(html);
}

/* =====================================================
   LOAD ORDERS
===================================================== */
function LoadOrders() {

    var cid = $("#customerId").val().trim();
    var loc = $("#locations").val();

    if (cid === "") {
        alert("Customer Id is required.");
        $("#customerId").focus();
        return;
    }

    if (loc === "") {
        $("#ordersArea").html("");
        return;
    }

    var sendData = {};
    sendData["customerId"] = cid;
    sendData["locationId"] = loc;

    CallAJAX(wsUrl + "/orders",
        sendData,
        "GET",
        "json",
        LoadOrdersSuccess,
        AjaxError
    );
}

function LoadOrdersSuccess(data) {

    if (data.error) {
        $("#ordersArea").html("<p>" + data.error + "</p>");
        return;
    }

    var list = data.orders;

    if (list.length === 0) {
        $("#ordersArea").html("<p>No records found.</p>");
        return;
    }

    var html = "<h3>Orders placed by " + data.customerName +
               " at location: " + data.locationName + "</h3>";

    html += "<table>";
    html += "<tr>" +
        "<th>Order Id</th>" +
        "<th>Order Date</th>" +
        "<th>Payment Method</th>" +
        "<th>Item Name</th>" +
        "<th>Item Price</th>" +
        "<th>Item Count</th>" +
        "</tr>";

    for (var i = 0; i < list.length; i++) {
        var o = list[i];

        html += "<tr>";
        html += "<td>" + o.orderId + "</td>";
        html += "<td>" + o.orderDate + "</td>";
        html += "<td>" + o.paymentMethod + "</td>";
        html += "<td>" + o.itemName + "</td>";
        html += "<td>" + o.itemPrice + "</td>";
        html += "<td>" + o.itemCount + "</td>";
        html += "</tr>";
    }

    html += "</table>";

    $("#ordersArea").html(html);
}