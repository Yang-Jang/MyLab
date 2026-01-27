using Microsoft.AspNetCore.Mvc;
using Bai2.Models;
using System.Collections.Generic;

namespace Bai2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repo;

        public ProductController(IProductRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                message = "Tải danh sách sản phẩm thành công.",
                count = _repo.GetAll().Count(),
                data = _repo.GetAll()
            });
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var p = _repo.GetById(id);
            if (p == null) return NotFound(new { message = $"Không tìm thấy sản phẩm ID: {id}" });
            return Ok(new { message = "Tìm thấy sản phẩm.", data = p });
        }

        [HttpPost]
        public IActionResult Post([FromBody] Product p)
        {
            var newProduct = _repo.Add(p);
            return Ok(new
            {
                message = "Thêm sản phẩm mới thành công!",
                newId = newProduct.Id,
                data = newProduct 
            });
        }

        [HttpPut]
        public IActionResult Put([FromBody] Product p)
        {
            var updatedProduct = _repo.Update(p);
            if (updatedProduct == null)
            {
                return NotFound(new { message = $"Không tìm thấy ID: {p.Id} để cập nhật." });
            }

            return Ok(new
            {
                message = "Cập nhật sản phẩm thành công!",
                data = updatedProduct 
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var deletedProduct = _repo.Delete(id);
            if (deletedProduct == null)
            {
                return NotFound(new { message = $"Không tìm thấy ID: {id} để xóa." });
            }

            return Ok(new
            {
                message = "Xóa sản phẩm thành công!",
                data = deletedProduct 
            });
        }
    }
}