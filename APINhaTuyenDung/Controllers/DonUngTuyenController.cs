using BLL.Interfaces;
using Common.Extensions;
using Common.Problems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace APINhaTuyenDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonUngTuyenController : ControllerBase
    {
        private readonly IDonUngTuyenBusiness _business;
        public DonUngTuyenController(IDonUngTuyenBusiness business) { _business = business; }

        [HttpPut("doi-trang-thai")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> DoiTrangThai([FromBody] DoiTrangThaiDonModel model)
        {
            var kq = await _business.DoiTrangThaiAsync(model, User.LayNguoiDungHienTai());
            return this.KetQua(kq);
        }

        [HttpGet("theo-tin/{tinId:int}")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> LayTheoTin(int tinId)
        {
            return Ok(await _business.LayTheoTinAsync(tinId, User.LayNguoiDungHienTai()));
        }
    }
}
