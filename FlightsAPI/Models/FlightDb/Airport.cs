using System;
using System.Collections.Generic;
using NpgsqlTypes;

namespace FlightsAPI.Models.FlightDb;

public partial class dbAirport
{
    /// <summary>
    /// Airport code
    /// </summary>
    public string? AirportCode { get; set; }

    /// <summary>
    /// Airport name
    /// </summary>
    public string? AirportName { get; set; }

    /// <summary>
    /// City
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Airport coordinates (longitude and latitude)
    /// </summary>
    public NpgsqlPoint? Coordinates { get; set; }

    /// <summary>
    /// Airport time zone
    /// </summary>
    public string? Timezone { get; set; }
}
