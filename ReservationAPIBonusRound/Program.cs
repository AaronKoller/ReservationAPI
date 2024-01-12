using ReservationAPIBonusRound.Models.AppSettings;
using ReservationAPIBonusRound.Repositories;
using ReservationAPIBonusRound.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appSettings.json");
builder.Services.AddOptions<AppSettingsOptions>().BindConfiguration("");

builder.Services.AddControllers();

builder.Services.AddSingleton<IProviderRepository, SuperSimpleProviderRepository>();
builder.Services.AddTransient<IProviderService, ProviderService>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();

namespace ReservationAPIBonusRound
{
    public partial class Program { }
}
