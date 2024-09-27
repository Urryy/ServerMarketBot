using Microsoft.EntityFrameworkCore;
using ServerMarketBot;
using ServerMarketBot.Dto;
using ServerMarketBot.Repository.Impl;
using ServerMarketBot.Repository.Interfaces;
using ServerMarketBot.Services.Impl;
using ServerMarketBot.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel();

string connectionToDb = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<DatabaseContext>(opt => opt.UseSqlServer(connectionToDb));

builder.Services.Configure<TeamsSettings>(builder.Configuration.GetSection("Teams"));

builder.Services.AddSingleton<TelegramBot>();
builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient<ITelegramBotService, TelegramBotService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.Services.GetRequiredService<TelegramBot>().LaunchAsync().Wait();

app.UseRouting();

app.MapControllers();

app.Run();
