using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add your DbContext here
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 39)))); // Adjust version as necessary

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        //builder => builder.AllowAnyOrigin()
         builder => builder.WithOrigins("http://localhost:5173")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});


builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAllOrigins");
app.MapPost("/api/auth/login", async (LoginDto request, AppDbContext db, IPasswordHasher passwordHasher) =>
{
    // Use the password hasher service
    string hashedPassword = passwordHasher.HashPassword(request.Password); 

    // Check if the user exists and the password matches
    var user = await db.User.SingleOrDefaultAsync(u => u.Username == request.Username && u.PasswordHash == hashedPassword);
    
    if (user != null)
    {
        return Results.Ok(new { message = "Login successful!" });
    }
    else
    {
        return Results.Unauthorized();
    }
}).WithName("Login");





// Task CRUD operations
app.MapGet("/tasks", async (AppDbContext db) =>
{
    return await db.TaskItem.ToListAsync(); // Get all tasks
}).WithName("GetTasks");

app.MapGet("/tasks/{id}", async (int id, AppDbContext db) =>
{
    return await db.TaskItem.FindAsync(id)
        is TaskItem task
            ? Results.Ok(task) // Return the task if found
            : Results.NotFound(); // Return 404 if not found
}).WithName("GetTaskById");

app.MapPost("/tasks", async (TaskItem task, AppDbContext db) =>
{
    db.TaskItem.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{task.Id}", task); // Return created task
}).WithName("CreateTask");

app.MapPut("/tasks/{id}", async (int id, TaskItem updatedTask, AppDbContext db) =>
{
    var task = await db.TaskItem.FindAsync(id);

    if (task is null) return Results.NotFound();

    task.Title = updatedTask.Title; // Update properties
    task.IsCompleted = updatedTask.IsCompleted;

    await db.SaveChangesAsync();
    return Results.NoContent(); // Return 204 No Content
}).WithName("UpdateTask");

app.MapDelete("/tasks/{id}", async (int id, AppDbContext db) =>
{
    var task = await db.TaskItem.FindAsync(id);

    if (task is null) return Results.NotFound();

    db.TaskItem.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent(); // Return 204 No Content
}).WithName("DeleteTask");

// User CRUD operations
app.MapGet("/users", async (AppDbContext db) =>
{
    return await db.User.ToListAsync(); // Get all users
}).WithName("GetUsers");

app.MapGet("/users/{id}", async (int id, AppDbContext db) =>
{
    return await db.User.FindAsync(id)
        is AppUser user
            ? Results.Ok(user) // Return the user if found
            : Results.NotFound(); // Return 404 if not found
}).WithName("GetUserById");

app.MapPost("/users", async (AppUser user, AppDbContext db) =>
{
    db.User.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user); // Return created user
}).WithName("CreateUser");

app.MapPut("/users/{id}", async (int id, AppUser updatedUser, AppDbContext db) =>
{
    var user = await db.User.FindAsync(id);

    if (user is null) return Results.NotFound();

    user.Username = updatedUser.Username; // Update properties
    user.PasswordHash = updatedUser.PasswordHash;
    user.Email = updatedUser.Email;

    await db.SaveChangesAsync();
    return Results.NoContent(); // Return 204 No Content
}).WithName("UpdateUser");

app.MapDelete("/users/{id}", async (int id, AppDbContext db) =>
{
    var user = await db.User.FindAsync(id);

    if (user is null) return Results.NotFound();

    db.User.Remove(user);
    await db.SaveChangesAsync();
    return Results.NoContent(); // Return 204 No Content
}).WithName("DeleteUser");

app.Run();
