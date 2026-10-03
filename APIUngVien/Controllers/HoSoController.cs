using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIUngVien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoSoController : ControllerBase
    {
        private readonly IHoSoBusiness _business;
        public HoSoController(IHoSoBusiness business) { _business = business; }

        [HttpPost("tai-len")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> TaiLen(IFormFile file, [FromForm] int soNamKinhNghiem, [FromForm] string danhSachKyNangId)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            int ungVienId = claim != null ? int.Parse(claim.Value) : 0;

            List<int> kyNangIds = new List<int>();
            if (!string.IsNullOrEmpty(danhSachKyNangId))
                kyNangIds = danhSachKyNangId.Split(',').Select(int.Parse).ToList();

            var kq = await _business.TaoAsync(file, ungVienId, soNamKinhNghiem, kyNangIds);
            if (kq.ThanhCong) return Ok(kq);
            return BadRequest(kq);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> LayChiTiet(int id)
        {
            var hoSo = await _business.LayChiTietAsync(id);
            if (hoSo == null) return NotFound();
            return Ok(hoSo);
        }

        [HttpGet("cua-toi")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> DanhSachCuaToi()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            int ungVienId = claim != null ? int.Parse(claim.Value) : 0;
            var ds = await _business.LayDanhSachTheoUngVienAsync(ungVienId);
            return Ok(ds);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> Xoa(int id)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            int ungVienId = claim != null ? int.Parse(claim.Value) : 0;
            var kq = await _business.XoaAsync(id, ungVienId);
            if (kq.ThanhCong) return Ok(kq);
            return BadRequest(kq);
        }
    }
}
