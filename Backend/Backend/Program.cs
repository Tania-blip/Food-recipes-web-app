using WebApplication1;
using WebApplication1.Properties;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(
    options => options.WithOrigins("http://localhost:52871").AllowAnyMethod()
);

app.UseAuthorization();

app.MapControllers();

// load config from appsettings.json
var (uri, user, password) = Config.UnpackNeo4jConfig();
// connect to Neo4J and Verify Connectivity
await MyNeo4j.InitDriverAsync(uri, user, password);

Console.WriteLine("Database initialized");

app.Run();