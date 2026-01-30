using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bai2.Data;
using Bai2.Models;

namespace Bai2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ReservationContext _context;

        public ReservationsController(ReservationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _context.Reservations.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
               Console.WriteLine($"Không tìm thấy ID = {id}");
                return NotFound(new { message = $"Không tìm thấy dữ liệu có ID = {id}" }); 
            }
            return reservation;
        }

        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(Reservation reservation)
        {
            Console.WriteLine($"Đang thêm mới: {reservation.Name} - {reservation.StartLocation}...");
            
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Đã thêm thành công");
            return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, Reservation reservation)
        {
            if (id != reservation.Id)
            {
                Console.WriteLine("ID trên URL và Body không khớp nhau!");
                return BadRequest(new { message = "ID không khớp!" });
            }

            _context.Entry(reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("Đã cập nhật xong.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound(new { message = "Không tìm thấy ID để cập nhật" });
                }
                else
                {
                    throw;
                }
            }
            return Ok(new { message = "Cập nhật thành công!", data = reservation });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                Console.WriteLine($"Không tìm thấy ID = {id} để xóa.");
                return NotFound(new { message = $"Không tìm thấy dữ liệu có ID = {id} để xóa." });
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Đã xóa thành công ID = {id}");
            return Ok(new { message = "Xóa thành công!", data = reservation });
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
