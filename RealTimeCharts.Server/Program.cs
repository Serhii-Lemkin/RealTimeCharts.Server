using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using RealTimeCharts.Server.HubConfig;
using RealTimeCharts.Server.Models.config;
using RealTimeCharts.Server.Services;
using RealTimeCharts.Server.TimerFeatures;
using System.Text;
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

builder.Services.AddSingleton<IAuthService, AuthService>();

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience= false,
            ValidateLifetime = false,
        };
    });
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .SetIsOriginAllowed(origin => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
builder.Services.AddSingleton<TimerManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChartHub>("/chart");
app.MapHub<InviteHub>("/invitehub");
app.MapHub<MessageHub>("/messagehub");
app.MapHub<TicTacToeHub>("/ticktacktoehub");

app.Run();
