using FlightsAPI.Infrastructure.ExternalApis;
using System.Text.Json;
using FlightsAPI.Apis;
using FlightsAPI.Infrastructure.ExternalApis.Interfaces;
using FlightsAPI.Domain.Interfaces;
using System.Text.Json.Serialization;
using FlightsAPI.Domain;
using FlightsAPI.Infrastructure.DataBases;
using FlightsAPI.Infrastructure.DataBases.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonSerializerOptions>(options =>
{
	options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
	options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	options.PropertyNameCaseInsensitive = true;
	options.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<AmadeusOptions>(builder.Configuration.GetSection("Amadeus"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddHttpClient<ISimpleClient<HttpResponseMessage>, SimpleClient>();
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


app.Run();