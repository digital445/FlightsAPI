using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace FlightsAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "bookings");

            migrationBuilder.CreateTable(
                name: "aircrafts_data",
                schema: "bookings",
                columns: table => new
                {
                    aircraft_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false, comment: "Aircraft code, IATA"),
                    model = table.Column<string>(type: "jsonb", nullable: false, comment: "Aircraft model"),
                    range = table.Column<int>(type: "integer", nullable: false, comment: "Maximal flying distance, km")
                },
                constraints: table =>
                {
                    table.PrimaryKey("aircrafts_pkey", x => x.aircraft_code);
                },
                comment: "Aircrafts (internal data)");

            migrationBuilder.CreateTable(
                name: "airports_data",
                schema: "bookings",
                columns: table => new
                {
                    airport_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false, comment: "Airport code"),
                    airport_name = table.Column<string>(type: "jsonb", nullable: false, comment: "Airport name"),
                    city = table.Column<string>(type: "jsonb", nullable: false, comment: "City"),
                    coordinates = table.Column<NpgsqlPoint>(type: "point", nullable: false, comment: "Airport coordinates (longitude and latitude)"),
                    timezone = table.Column<string>(type: "text", nullable: false, comment: "Airport time zone")
                },
                constraints: table =>
                {
                    table.PrimaryKey("airports_data_pkey", x => x.airport_code);
                },
                comment: "Airports (internal data)");

            migrationBuilder.CreateTable(
                name: "bookings",
                schema: "bookings",
                columns: table => new
                {
                    book_ref = table.Column<string>(type: "character(6)", fixedLength: true, maxLength: 6, nullable: false, comment: "Booking number"),
                    book_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Booking _date"),
                    total_amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, comment: "Total booking cost")
                },
                constraints: table =>
                {
                    table.PrimaryKey("bookings_pkey", x => x.book_ref);
                },
                comment: "Bookings");

            migrationBuilder.CreateTable(
                name: "seats",
                schema: "bookings",
                columns: table => new
                {
                    aircraft_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false, comment: "Aircraft code, IATA"),
                    seat_no = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false, comment: "Seat number"),
                    fare_conditions = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, comment: "Travel class")
                },
                constraints: table =>
                {
                    table.PrimaryKey("seats_pkey", x => new { x.aircraft_code, x.seat_no });
                    table.ForeignKey(
                        name: "seats_aircraft_code_fkey",
                        column: x => x.aircraft_code,
                        principalSchema: "bookings",
                        principalTable: "aircrafts_data",
                        principalColumn: "aircraft_code",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Seats");

            migrationBuilder.CreateTable(
                name: "flights",
                schema: "bookings",
                columns: table => new
                {
                    flight_id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('flights_flight_id_seq'::regclass)", comment: "Flight ID"),
                    flight_no = table.Column<string>(type: "character(6)", fixedLength: true, maxLength: 6, nullable: false, comment: "Flight number"),
                    scheduled_departure = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Scheduled departure time"),
                    scheduled_arrival = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, comment: "Scheduled arrival time"),
                    departure_airport = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false, comment: "Airport of departure"),
                    arrival_airport = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false, comment: "Airport of arrival"),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Flight status"),
                    aircraft_code = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false, comment: "Aircraft code, IATA"),
                    actual_departure = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Actual departure time"),
                    actual_arrival = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true, comment: "Actual arrival time")
                },
                constraints: table =>
                {
                    table.PrimaryKey("flights_pkey", x => x.flight_id);
                    table.ForeignKey(
                        name: "flights_aircraft_code_fkey",
                        column: x => x.aircraft_code,
                        principalSchema: "bookings",
                        principalTable: "aircrafts_data",
                        principalColumn: "aircraft_code");
                    table.ForeignKey(
                        name: "flights_arrival_airport_fkey",
                        column: x => x.arrival_airport,
                        principalSchema: "bookings",
                        principalTable: "airports_data",
                        principalColumn: "airport_code");
                    table.ForeignKey(
                        name: "flights_departure_airport_fkey",
                        column: x => x.departure_airport,
                        principalSchema: "bookings",
                        principalTable: "airports_data",
                        principalColumn: "airport_code");
                },
                comment: "Flights");

            migrationBuilder.CreateTable(
                name: "tickets",
                schema: "bookings",
                columns: table => new
                {
                    ticket_no = table.Column<string>(type: "character(13)", fixedLength: true, maxLength: 13, nullable: false, comment: "Ticket number"),
                    book_ref = table.Column<string>(type: "character(6)", fixedLength: true, maxLength: 6, nullable: false, comment: "Booking number"),
                    passenger_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Passenger ID"),
                    passenger_name = table.Column<string>(type: "text", nullable: false, comment: "Passenger name"),
                    contact_data = table.Column<string>(type: "jsonb", nullable: true, comment: "Passenger contact information")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tickets_pkey", x => x.ticket_no);
                    table.ForeignKey(
                        name: "tickets_book_ref_fkey",
                        column: x => x.book_ref,
                        principalSchema: "bookings",
                        principalTable: "bookings",
                        principalColumn: "book_ref");
                },
                comment: "Tickets");

            migrationBuilder.CreateTable(
                name: "ticket_flights",
                schema: "bookings",
                columns: table => new
                {
                    ticket_no = table.Column<string>(type: "character(13)", fixedLength: true, maxLength: 13, nullable: false, comment: "Ticket number"),
                    flight_id = table.Column<int>(type: "integer", nullable: false, comment: "Flight ID"),
                    fare_conditions = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, comment: "Travel class"),
                    amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, comment: "Travel cost")
                },
                constraints: table =>
                {
                    table.PrimaryKey("ticket_flights_pkey", x => new { x.ticket_no, x.flight_id });
                    table.ForeignKey(
                        name: "ticket_flights_flight_id_fkey",
                        column: x => x.flight_id,
                        principalSchema: "bookings",
                        principalTable: "flights",
                        principalColumn: "flight_id");
                    table.ForeignKey(
                        name: "ticket_flights_ticket_no_fkey",
                        column: x => x.ticket_no,
                        principalSchema: "bookings",
                        principalTable: "tickets",
                        principalColumn: "ticket_no");
                },
                comment: "Flight segment");

            migrationBuilder.CreateTable(
                name: "boarding_passes",
                schema: "bookings",
                columns: table => new
                {
                    ticket_no = table.Column<string>(type: "character(13)", fixedLength: true, maxLength: 13, nullable: false, comment: "Ticket number"),
                    flight_id = table.Column<int>(type: "integer", nullable: false, comment: "Flight ID"),
                    boarding_no = table.Column<int>(type: "integer", nullable: false, comment: "Boarding pass number"),
                    seat_no = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false, comment: "Seat number")
                },
                constraints: table =>
                {
                    table.PrimaryKey("boarding_passes_pkey", x => new { x.ticket_no, x.flight_id });
                    table.ForeignKey(
                        name: "boarding_passes_ticket_no_fkey",
                        columns: x => new { x.ticket_no, x.flight_id },
                        principalSchema: "bookings",
                        principalTable: "ticket_flights",
                        principalColumns: new[] { "ticket_no", "flight_id" });
                },
                comment: "Boarding passes");

            migrationBuilder.CreateIndex(
                name: "boarding_passes_flight_id_boarding_no_key",
                schema: "bookings",
                table: "boarding_passes",
                columns: new[] { "flight_id", "boarding_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "boarding_passes_flight_id_seat_no_key",
                schema: "bookings",
                table: "boarding_passes",
                columns: new[] { "flight_id", "seat_no" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "flights_flight_no_scheduled_departure_key",
                schema: "bookings",
                table: "flights",
                columns: new[] { "flight_no", "scheduled_departure" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_flights_aircraft_code",
                schema: "bookings",
                table: "flights",
                column: "aircraft_code");

            migrationBuilder.CreateIndex(
                name: "IX_flights_arrival_airport",
                schema: "bookings",
                table: "flights",
                column: "arrival_airport");

            migrationBuilder.CreateIndex(
                name: "IX_flights_departure_airport",
                schema: "bookings",
                table: "flights",
                column: "departure_airport");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_flights_flight_id",
                schema: "bookings",
                table: "ticket_flights",
                column: "flight_id");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_book_ref",
                schema: "bookings",
                table: "tickets",
                column: "book_ref");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "boarding_passes",
                schema: "bookings");

            migrationBuilder.DropTable(
                name: "seats",
                schema: "bookings");

            migrationBuilder.DropTable(
                name: "ticket_flights",
                schema: "bookings");

            migrationBuilder.DropTable(
                name: "flights",
                schema: "bookings");

            migrationBuilder.DropTable(
                name: "tickets",
                schema: "bookings");

            migrationBuilder.DropTable(
                name: "aircrafts_data",
                schema: "bookings");

            migrationBuilder.DropTable(
                name: "airports_data",
                schema: "bookings");

            migrationBuilder.DropTable(
                name: "bookings",
                schema: "bookings");
        }
    }
}
