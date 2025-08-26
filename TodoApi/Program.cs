using Microsoft.EntityFrameworkCore;
using TodoApi.Repositories;
using TodoApi.Middleware;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Filters;

var builder = WebApplication.CreateBuilder(args);
builder
    .Services.AddDbContext<TodoContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("TodoContext"))
    )
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(c =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    })
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            if (context.ModelState.ErrorCount == 0)
            {
                return new BadRequestObjectResult("Request body is required.");
            }

            return new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
        };
    });

// Service registration
builder.Services.AddScoped<ITodoListRepository, TodoListRepository>();
builder.Services.AddScoped<ITodoItemRepository, TodoItemRepository>();
builder.Services.AddScoped<ValidateTodoListExistsAttribute>();

// Logging
var enableVerboseEf = builder.Configuration.GetValue("Logging:EnableVerboseEF", false);
var includeScopes = builder.Configuration.GetValue("Logging:Console:IncludeScopes", true);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(o =>
{
    o.IncludeScopes = includeScopes;
    o.SingleLine = false;
    o.TimestampFormat = "hh:mm:ss ";
    o.UseUtcTimestamp = true;
});
builder.Logging.AddFilter(
    "Microsoft.EntityFrameworkCore.Database.Command",
    enableVerboseEf ? LogLevel.Information : LogLevel.Warning);


var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

// Run migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoContext>();
    db.Database.Migrate();
}

app.Run();