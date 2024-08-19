using TourGuide.LibrairiesWrappers;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services;
using TourGuide.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRewardsService, RewardsService>();
builder.Services.AddScoped<ITourGuideService, TourGuideService>();
builder.Services.AddScoped<IGpsUtil, GpsUtilWrapper>();
builder.Services.AddScoped<IRewardCentral, RewardCentralWrapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// NOTE: if testmode is enabled then it will create 100 internal users for testing.
// Note:userName = $"internalUser{i}"
// Note: user = new User(Guid.NewGuid(), userName, "000", $"{userName}@tourGuide.com"); */