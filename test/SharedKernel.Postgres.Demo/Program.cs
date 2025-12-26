using GridifyExtensions.Extensions;
using SharedKernel.Postgres.Demo;
using SharedKernel.Postgres.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddPostgresContext<MyDbContext>(builder.Configuration.GetConnectionString("Postgres")!);
builder.AddGridify();


builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.Run();