// Scaffold-DbContext "YOUR_CONNECTION_STRING_HERE";
//Microsoft.entitiyframeworkcore.sqlserver 9.0.11 Project
//Microsoft.entitiyframeworkcore.tools
//Manage console


using ICA11.NET;
using ICA11.NET.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(origin => true)
);

/* ============================
   LOAD LOCATIONS
============================ */
app.MapGet("/locations", () =>
{
    return Results.Json(new { locations = Restaurant.GetLocations() });
});

/* ============================
   LOAD ITEMS
============================ */
app.MapGet("/items", () =>
{
    return Results.Json(new { items = Restaurant.GetItems() });
});

/* ============================
   LOAD ORDERS (ICA10)
============================ */
app.MapGet("/orders", (string customerId, string locationId) =>
{
    if (!int.TryParse(customerId, out int cid))
        return Results.Json(new { error = "Invalid customer ID." });
    if (!int.TryParse(locationId, out int lid))
        return Results.Json(new { error = "Invalid location ID." });

    var result = Restaurant.GetOrders(cid, lid);
    return Results.Json(result);
});

/* ============================
   DELETE ORDER
============================ */
app.MapDelete("/orders/{id}", (int id) =>
{
    var result = Restaurant.DeleteOrder(id);
    return Results.Json(result);
});

/* ============================
   PLACE ORDER
============================ */
app.MapPost("/orders/add", (OrderData d) =>
{
    var result = Restaurant.AddOrder(d);
    return Results.Json(result);
});

/* ============================
   UPDATE ORDER
============================ */
app.MapPut("/orders/update", (OrderUpdate u) =>
{
    var result = Restaurant.UpdateOrder(u);
    return Results.Json(result);
});

app.Run();


public record OrderData(int CustomerId, int LocationId, int ItemId, int ItemCount, string Payment);
public record OrderUpdate(int OrderId, int ItemId, int ItemCount, string Payment);
