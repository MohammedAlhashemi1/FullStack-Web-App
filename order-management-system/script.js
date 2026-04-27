var wsUrl = "http://localhost:5026";

/* ==============================
   AJAX WRAPPER
============================== */
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

/* ==============================
   PAGE LOAD
============================== */
$(document).ready(function () {

    LoadLocations();
    LoadItems();

    $("#locationList").change(function () {
        LoadOrders();
    });

    $("#btnPlace").click(ProcessOrder);
});

/* ==============================
   LOAD LOCATIONS
============================== */
function LoadLocations() {

    CallAJAX(wsUrl + "/locations", {}, "GET", "json",
        LoadLocationsSuccess, AjaxError);
}

function LoadLocationsSuccess(data) {

    var html = "<option value=''>Select...</option>";

    for (var i = 0; i < data.locations.length; i++) {
        let loc = data.locations[i];
        html += "<option value='" + loc.locationId + "'>" + loc.locationName + "</option>";
    }

    $("#locationList").html(html);
    $("#pickupList").html(html);
}

/* ==============================
   LOAD ITEMS
============================== */
function LoadItems() {

    CallAJAX(wsUrl + "/items", {}, "GET", "json",
        LoadItemsSuccess, AjaxError);
}

function LoadItemsSuccess(data) {

    var html = "<option value=''>Select...</option>";

    for (var i = 0; i < data.items.length; i++) {
        let it = data.items[i];
        html += "<option value='" + it.itemId + "'>" + it.itemName + "</option>";
    }

    $("#itemList").html(html);
}

/* ==============================
   LOAD ORDERS
============================== */
function LoadOrders() {

    var cid = $("#customerId").val().trim();
    var loc = $("#locationList").val();

    if (cid === "")
        return;

    CallAJAX(wsUrl + "/orders",
        { customerId: cid, locationId: loc },
        "GET", "json",
        LoadOrdersSuccess, AjaxError);
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

    var html = "<table>";
    html += "<tr><th>Order</th><th>Date</th><th>Payment</th><th>Item</th><th>Price</th><th>Qty</th><th>Del</th></tr>";

    for (var i = 0; i < list.length; i++) {

        var o = list[i];

        html += "<tr>";
        html += "<td>" + o.orderId + "</td>";
        html += "<td>" + o.orderDate + "</td>";
        html += "<td>" + o.paymentMethod + "</td>";
        html += "<td>" + o.itemName + "</td>";
        html += "<td>" + o.itemPrice + "</td>";
        html += "<td>" + o.itemCount + "</td>";
        html += "<td><button class='delBtn' data-id='" + o.orderId + "'>Delete</button></td>";
        html += "</tr>";
    }

    html += "</table>";
    $("#ordersArea").html(html);

    $(".delBtn").click(function () {
        DeleteOrder($(this).data("id"));
    });
}

/* ==============================
   DELETE ORDER
============================== */
function DeleteOrder(id) {

    CallAJAX(wsUrl + "/orders/" + id, {}, "DELETE", "json",
        function () {
            LoadOrders();
        },
        AjaxError);
}

/* ==============================
   PLACE / UPDATE DECIDER
============================== */
function ProcessOrder() {
    if ($("#orderId").val().trim() === "")
        PlaceOrder();
    else
        UpdateOrder();
}

/* ==============================
   PLACE ORDER
============================== */
function PlaceOrder() {

    var d = {};
    d["CustomerId"] = parseInt($("#procCustomerId").val());
    d["LocationId"] = parseInt($("#pickupList").val());
    d["ItemId"] = parseInt($("#itemList").val());
    d["ItemCount"] = parseInt($("#itemCount").val());
    d["Payment"] = $("#paymentList").val();

    CallAJAX(wsUrl + "/orders/add",
        d, "POST", "json",
        PlaceOrderSuccess, AjaxError);
}

function PlaceOrderSuccess(data) {

    if (data.error) {
        $("#orderMsg").text(data.error);
        return;
    }

    $("#orderMsg").text("Order placed. Ready in " + data.readyMins + " mins.");

    $("#orderId").val(data.orderId);
    $("#btnPlace").text("Update Order");
}

/* ==============================
   UPDATE ORDER
============================== */
function UpdateOrder() {

    var d = {};
    d["OrderId"] = parseInt($("#orderId").val());
    d["ItemId"] = parseInt($("#itemList").val());
    d["ItemCount"] = parseInt($("#itemCount").val());
    d["Payment"] = $("#paymentList").val();

    CallAJAX(wsUrl + "/orders/update",
        d, "PUT", "json",
        UpdateOrderSuccess, AjaxError);
}

function UpdateOrderSuccess(data) {

    if (data.error) {
        $("#orderMsg").text(data.error);
        return;
    }

    $("#orderMsg").text("Order updated.");
    $("#btnPlace").prop("disabled", true);
}