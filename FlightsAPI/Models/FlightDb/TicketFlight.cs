using System;
using System.Collections.Generic;

namespace FlightsAPI.Models.FlightDb;

/// <summary>
/// Flight segment
/// </summary>
public partial class TicketFlight
{
    /// <summary>
    /// Ticket number
    /// </summary>
    public string TicketNo { get; set; } = null!;

    /// <summary>
    /// Flight ID
    /// </summary>
    public int FlightId { get; set; }

    /// <summary>
    /// Travel class
    /// </summary>
    public string FareConditions { get; set; } = null!;

    /// <summary>
    /// Travel cost
    /// </summary>
    public decimal Amount { get; set; }

    public virtual BoardingPass? BoardingPass { get; set; }

    public virtual Flight Flight { get; set; } = null!;

    public virtual Ticket TicketNoNavigation { get; set; } = null!;
}
