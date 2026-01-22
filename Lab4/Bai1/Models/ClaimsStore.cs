using System.Security.Claims;

namespace Bai1.Models
{
    public static class ClaimsStore
    {
        public static List<Claim> GetAllClaims()
        {
            return new List<Claim>()
            {
                // Các Claim chức năng (giữ lại từ Bài 1 nếu muốn dùng lẻ)
                new Claim("CreateProduct", "Create Product"),
                new Claim("Edit Product", "Edit Product"),
                new Claim("Delete Product", "Delete Product"),

                // --- CÁC CLAIM PHÂN QUYỀN (QUAN TRỌNG CHO BÀI 2) ---
                new Claim("Admin", "Admin Role"),  // Check vào đây để set quyền Admin
                new Claim("Sales", "Sales Role")   // Check vào đây để set quyền Sales
            };
        }
    }
}