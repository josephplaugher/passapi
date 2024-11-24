using System;
using Microsoft.EntityFrameworkCore;
using passapi.data;
using passapi.Interfaces;
using passapi.Services;

const string DevCorsPolicy = "DevCorsPolicy";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IBaseUploadService, BaseUploadService>();
builder.Services.AddScoped<ITestResultUploadService, TestResultUploadService>();
builder.Services.AddCors(options => 
{
    options.AddPolicy(name: DevCorsPolicy,
    policy => 
    {
        policy.AllowAnyHeader();
        policy.AllowAnyOrigin();
        policy.AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints?.MapControllers();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();