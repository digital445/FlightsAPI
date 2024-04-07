using FlightsAPI.Infrastructure.ExternalApis;
using System.Text.Json;
using FlightsAPI.Apis;
using FlightsAPI.Infrastructure.ExternalApis.Interfaces;
using FlightsAPI.Domain.Interfaces;
using System.Text.Json.Serialization;
using FlightsAPI.Domain;
using FlightsAPI.Infrastructure.DataBases;
using FlightsAPI.Infrastructure.DataBases.Interfaces;
using Microsoft.EntityFrameworkCore;
using FlightsAPI.MapperProfiles;
using Microsoft.Extensions.Options;
using FlightsAPI.Caching;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonSerializerOptions>(options =>
{
	options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
	options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	options.PropertyNameCaseInsensitive = true;
	options.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<AmadeusOptions>(builder.Configuration.GetSection("Amadeus"));
builder.Services.Configure<FlightDbOptions>(builder.Configuration.GetSection("FlightDb"));
string? dbConnectionString = builder.Configuration["FlightDb:ConnectionString"];

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOutputCache(options => 
	options.AddPolicy("CachePost", CustomCachingPolicy.Instance)
);
//Add AutoMapper profiles manually because of using DI in one profile
builder.Services.AddAutoMapper((provider, cfg) =>
{
	cfg.AddProfile(new FlightQueryProfile());
	cfg.AddProfile(new FlightOfferProfile(provider.GetService<IOptions<FlightDbOptions>>()!));
	cfg.AddProfile(new BookingOrderProfile());
	cfg.AddProfile(new BookingResultProfile());
}, assemblies: null);
builder.Services.AddHttpClient<ISimpleClient<HttpResponseMessage>, SimpleClient>();
builder.Services.AddDbContext<FlightDbContext>(opt => opt.UseNpgsql(dbConnectionString));
builder.Services.AddScoped<ISimpleClient<HttpResponseMessage>, SimpleClient>();
builder.Services.AddScoped<IAmadeusClient, AmadeusClient>();
builder.Services.AddScoped<IAmadeusAdapter, AmadeusAdapter>();
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IFlightService, FlightAggregationService>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("/api/v1/flights")
	.WithTags("Flights API")
	.MapFlightsApi();

app.UseOutputCache();

app.Run();