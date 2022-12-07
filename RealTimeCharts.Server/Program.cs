using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RealTimeCharts.Server.HubConfig;
using RealTimeCharts.Server.Models.config;
using RealTimeCharts.Server.Services;
using RealTimeCharts.Server.TimerFeatures;
using TalkBack.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<UsersDBSettings>(
    builder.Configuration.GetSection(nameof(UsersDBSettings)));
builder.Services.AddSingleton<IUsersDBSettings>(
    sp => sp.GetRequiredService<IOptions<UsersDBSettings>>().Value);
builder.Services.AddSingleton<IMongoClient>(
    sp => new MongoClient(builder.Configuration.GetValue<string>("UsersDBSettings:ConnectionString")));
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
builder.Services.AddSingleton<TimerManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChartHub>("/chart");

app.Run();
