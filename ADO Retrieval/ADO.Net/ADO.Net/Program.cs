using ICA08.Net.My_Class;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
var app = builder.Build();

app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(_ => true)
);


// ===============================================
// GET STUDENTS
// ===============================================
app.MapGet("/students", () =>
{
    var data = Classes.GetStudents();
    return new { students = data };
});


// ===============================================
// GET CLASSES FOR ONE STUDENT
// ===============================================
app.MapGet("/classes/{id}", (int id) =>
{
    var data = Classes.GetClasses(id);
    return new { classes = data };
});


app.Run();


