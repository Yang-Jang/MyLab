using Microsoft.EntityFrameworkCore;
using Bai1.Models;

namespace Bai1.Data
{
   public class ReservationContext : DbContext
    {
        public ReservationContext(DbContextOptions<ReservationContext> options)
            : base(options)
        {
        }

        public DbSet<Reservation> Reservations { get; set; }
    }
}