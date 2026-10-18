using BLL.Interfaces;
using Common.Extensions;
using Common.Problems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace APIUngVien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonUngTuyenController : ControllerBase
    {
        private readonly IDonUngTuyenBusiness _business;
        public DonUngTuyenController(IDonUngTuyenBusiness business) { _business = business; }

        [HttpPost("nop-don")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> NopDon([FromBody] NopDonModel model)
        {
            var kq = await _business.NopDonAsync(model, User.LayNguoiDungHienTai().Id);
            return this.KetQua(kq);
        }

        [HttpGet("cua-toi")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> DonCuaToi()
        {
            return Ok(await _business.LayTheoUngVienAsync(User.LayNguoiDungHienTai().Id));
        }

        [HttpGet("chi-tiet/{id:int}")]
        [Authorize]
        public async Task<IActionResult> ChiTiet(int id)
        {
            return Ok(await _business.LayChiTietAsync(id, User.LayNguoiDungHienTai()));
        }
    }
}
