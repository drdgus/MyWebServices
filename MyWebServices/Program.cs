using Microsoft.EntityFrameworkCore;
using MyWebServices.Core.DataAccess;
using MyWebServices.Core.DataAccess.Repositories;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ParagraphsDbContext>(options =>
  options.UseSqlite("Filename=paragraphs.db"));
builder.Services.AddTransient<UserRepository>();


var app = builder.Build();

if (app.Environment.IsProduction())
{
    var filePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "StylesForRenderedHTML.css");
    if (File.Exists(filePath) == false)
    {
        File.WriteAllText(filePath, @".renderedHTML .centertext{
    text-align: center;
} 
.renderedHTML table, .renderedHTML th, .renderedHTML td{
    border: 1px solid black;
    border-collapse: collapse;
}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ParagraphsDbContext>();
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

#if DEBUG
app.Use(async (context, next) =>
{
    // Do work that doesn't write to the Response.
    string logStr = $"[{DateTime.Now}]\n\t" +
        $"---Request method---:\n\t" +
        $"[{context.Request.Method}] {context.Request.Path}\n\t" +
        $"---Request Headers---:\n\t" +
        String.Join("\n\t", context.Request.Headers.ToList()) + "\n\t" +
        $"---Request parametrs---:\n\t[" + (context.Request.QueryString.Value == "" ? "Empty" : context.Request.QueryString.Value) + "]";

    var req = context.Request;
    var bodyStr = string.Empty;

    // Allows using several time the stream in ASP.Net Core
    req.EnableBuffering();

    // Arguments: Stream, Encoding, detect encoding, buffer size 
    // AND, the most important: keep stream opened
    using (StreamReader reader
              = new StreamReader(req.Body, Encoding.UTF8, true, 20000, true))
    {
        bodyStr = reader.ReadToEndAsync().Result;
    }

    // Rewind, so the core is not lost when it looks the body for the request
    req.Body.Position = 0;

    using (var requestStream = new MemoryStream())
    {
        logStr += $"\n\t---Request Body---:\n\t[{(bodyStr.Length == 0 ? "Empty" : bodyStr)}]";
    }
    app.Logger.LogInformation(logStr);

    await next.Invoke();
    // Do logging or other work that doesn't write to the Response.
});
#endif

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
