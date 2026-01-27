using Microsoft.AspNetCore.Mvc;
using Bai2.Models;
using Microsoft.AspNetCore.JsonPatch; 
using System.Collections.Generic;

namespace Bai2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private IRepository repository;

        public ReservationController(IRepository repo) => repository = repo;

        [HttpGet]
        public IEnumerable<Reservation> Get() => repository.Reservations;

        [HttpGet("{id}")]
        public ActionResult<Reservation> Get(int id)
        {
            var res = repository[id];
            if (res == null)
                return NotFound($"Không tìm thấy ID: {id}"); 
            return Ok(res);
        }

        [HttpPost]
        public ActionResult Post([FromBody] Reservation res)
        {
            var newRes = repository.AddReservation(new Reservation
            {
                Name = res.Name,
                StartLocation = res.StartLocation,
                EndLocation = res.EndLocation
            });
            return Ok($"Thêm mới thành công! ID mới là: {newRes.Id}");
        }

        [HttpPut]
        public ActionResult Put([FromBody] Reservation res)
        {
            repository.UpdateReservation(res);
            return Ok($"Đã cập nhật (PUT) thành công cho ID: {res.Id}");
        }

        [HttpPatch("{id}")]
        public ActionResult Patch(int id, [FromBody] JsonPatchDocument<Reservation> patchDoc)
        {
            var res = repository[id];
            if (res == null)
                return NotFound($"Không tìm thấy ID: {id} để sửa (Patch)");

            if (patchDoc != null)
            {
                patchDoc.ApplyTo(res);
                return Ok($"Đã sửa đổi (PATCH) thành công cho ID: {id}");
            }
            return BadRequest("Dữ liệu Patch không hợp lệ");
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (repository[id] == null)
                 return NotFound($"Không tìm thấy ID: {id} để xóa");

            repository.DeleteReservation(id);
            return Ok($"Đã xóa thành công ID: {id}");
        }
    }
}