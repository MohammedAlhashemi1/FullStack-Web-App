using ICA10.NET;
using ICA10.NET.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(origin => true)
);

/* =========================================
   GET LOCATIONS
========================================= */
app.MapGet("/locations", () =>
{
    return Results.Json(new { locations = Restaurant.GetLocations() });
});

/* =========================================
   GET ORDERS
========================================= */
app.MapGet("/orders", (string customerId, string locationId) =>
{
    int cid, lid;
    if (!int.TryParse(customerId, out cid))
        return Results.Json(new { error = "Invalid Customer Id." });
    if (!int.TryParse(locationId, out lid))
        return Results.Json(new { error = "Invalid Location Id." });


    var result = Restaurant.GetOrders(cid, lid);
    return Results.Json(result);
});


app.Run();