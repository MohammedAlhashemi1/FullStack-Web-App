using ICA09.Net.Classes;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(x => x.AllowAnyHeader()
                  .AllowAnyMethod()
                  .SetIsOriginAllowed(origin => true));

// -------------------- GET STUDENTS --------------------
app.MapGet("/students", () =>
{
    var data = Stored.GetStudents();
    return new { students = data };
});

// -------------------- GET CLASSES FOR ONE STUDENT --------------------
app.MapGet("/students/{id}/classes", (int id) =>
{
    var data = Stored.GetClasses(id);
    return new { classes = data };
});

// -------------------- GET ALL CLASSES --------------------
app.MapGet("/classes", () =>
{
    var data = Stored.GetAllClasses();
    return new { classes = data };
});

// -------------------- INSERT STUDENT --------------------
app.MapPost("/students/add", (NewStudent s) =>
{
     if (string.IsNullOrWhiteSpace(s.FirstName))
        return Results.Json(new { error = "First name cannot be empty." });

    if (string.IsNullOrWhiteSpace(s.LastName))
        return Results.Json(new { error = "Last name cannot be empty." });

    if (s.SchoolId <= 0)
        return Results.Json(new { error = "School ID must be a positive number." });

    if (s.ClassIds == null || s.ClassIds.Count == 0)
        return Results.Json(new { error = "You must select at least one class." });

    try
    {
        string msg = Stored.InsertStudent(s.FirstName, s.LastName, s.SchoolId, s.ClassIds);
        return Results.Json(new { message = msg });
    }
    catch (Exception ex)
    {
        return Results.Json(new { error = "Server error: " + ex.Message });
    }
});

// -------------------- UPDATE STUDENT --------------------
app.MapPut("/students/update", (StudentUpdate s) =>
{
    int school = int.Parse(s.SchoolId);
    string msg = Stored.UpdateStudent(s.StudentId, s.FirstName, s.LastName, school);
    return new { message = msg };
});

// -------------------- DELETE STUDENT --------------------
app.MapDelete("/students/{id}", (int id) =>
{
    string msg = Stored.DeleteStudent(id);
    return new { message = msg };
});

app.Run();

record StudentUpdate(int StudentId, string FirstName, string LastName, string SchoolId);
record NewStudent(string FirstName, string LastName, int SchoolId, List<int> ClassIds);