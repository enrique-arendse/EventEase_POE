using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventEase_POE.Migrations
{
    /// <inheritdoc />
    public partial class CreateBookingDetailsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW [dbo].[vw_BookingDetails] AS
                SELECT 
                    b.BookingId,
                    b.VenueId,
                    b.EventId,
                    b.BookingDate,
                    v.VenueName,
                    v.Location AS VenueLocation,
                    v.Capacity AS VenueCapacity,
                    v.ImageUrl AS VenueImageUrl,
                    e.EventName,
                    e.EventDate,
                    e.Description AS EventDescription,
                    e.ImageUrl AS EventImageUrl
                FROM dbo.Bookings b
                INNER JOIN dbo.Venues v ON b.VenueId = v.VenueId
                INNER JOIN dbo.Events e ON b.EventId = e.EventId
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS [dbo].[vw_BookingDetails]");
        }
    }
}
