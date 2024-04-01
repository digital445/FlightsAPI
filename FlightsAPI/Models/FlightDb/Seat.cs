using System;
using System.Collections.Generic;

namespace FlightsAPI.Models.FlightDb;

/// <summary>
/// Seats
/// </summary>
public partial class Seat
{
    /// <summary>
    /// Aircraft code, IATA
    /// </summary>
    public string AircraftCode { get; set; } = null!;

    /// <summary>
    /// Seat number
    /// </summary>
    public string SeatNo { get; set; } = null!;

    /// <summary>
    /// Travel class
    /// </summary>
    public string FareConditions { get; set; } = null!;

    public virtual AircraftsDatum AircraftCodeNavigation { get; set; } = null!;
}
