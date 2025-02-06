using System;
using Microsoft.EntityFrameworkCore;
using passapi.data;
using passapi.Interfaces;
using passapi.Services;
using Microsoft.AspNetCore.HttpOverrides;

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
        policy.AllowAnyHeader()
        .AllowAnyOrigin()
        .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseCors(DevCorsPolicy);
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
}

app.UseEndpoints(endpoints =>
{
    endpoints?.MapControllers();
});

app.UseHttpsRedirection();
app.Run();