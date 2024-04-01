using System;
using System.Collections.Generic;

namespace FlightsAPI.Models.FlightDb;

public partial class dbRoute
{
    /// <summary>
    /// Flight number
    /// </summary>
    public string? FlightNo { get; set; }

    /// <summary>
    /// Code of airport of departure
    /// </summary>
    public string? DepartureAirport { get; set; }

    /// <summary>
    /// Name of airport of departure
    /// </summary>
    public string? DepartureAirportName { get; set; }

    /// <summary>
    /// City of departure
    /// </summary>
    public string? DepartureCity { get; set; }

    /// <summary>
    /// Code of airport of arrival
    /// </summary>
    public string? ArrivalAirport { get; set; }

    /// <summary>
    /// Name of airport of arrival
    /// </summary>
    public string? ArrivalAirportName { get; set; }

    /// <summary>
    /// City of arrival
    /// </summary>
    public string? ArrivalCity { get; set; }

    /// <summary>
    /// Aircraft code, IATA
    /// </summary>
    public string? AircraftCode { get; set; }

    /// <summary>
    /// Scheduled duration of flight
    /// </summary>
    public TimeSpan? Duration { get; set; }

    /// <summary>
    /// Days of week on which flights are scheduled
    /// </summary>
    public List<int>? DaysOfWeek { get; set; }
}
