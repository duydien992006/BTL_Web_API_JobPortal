using BLL.Interfaces;
using Common.Problems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace APINhaTuyenDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TinTuyenDungController : ControllerBase
    {
        private readonly ITinTuyenDungBusiness _business;
        public TinTuyenDungController(ITinTuyenDungBusiness business) { _business = business; }

        [HttpPost]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> Tao([FromBody] TaoTinTuyenDungModel model)
        {
            var kq = await _business.TaoAsync(model);
            return this.KetQua(kq);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> CapNhat(int id, [FromBody] TaoTinTuyenDungModel model)
        {
            var kq = await _business.CapNhatAsync(id, model);
            return this.KetQua(kq, 404);
        }

        [HttpPut("{id:int}/duyet")]
        [Authorize(Roles = "QuanTriVien")]
        public async Task<IActionResult> Duyet(int id, [FromQuery] bool duyet)
        {
            var kq = await _business.DuyetAsync(id, duyet);
            return this.KetQua(kq, 404);
        }

        [HttpPut("{id:int}/dong")]
        [Authorize(Roles = "NhaTuyenDung")]
        public async Task<IActionResult> Dong(int id)
        {
            var kq = await _business.DongAsync(id);
            return this.KetQua(kq, 404);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "NhaTuyenDung,QuanTriVien")]
        public async Task<IActionResult> Xoa(int id)
        {
            var kq = await _business.XoaAsync(id);
            return this.KetQua(kq, 404);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> LayChiTiet(int id)
        {
            var tin = await _business.LayChiTietAsync(id);
            if (tin == null) return NotFound();
            return Ok(tin);
        }

        [HttpPost("tim-kiem")]
        public async Task<IActionResult> TimKiem([FromBody] TimKiemTinModel model)
        {
            var ds = await _business.TimKiemAsync(model);
            return Ok(ds);
        }

        [HttpGet("goi-y")]
        [Authorize(Roles = "UngVien")]
        public async Task<IActionResult> GoiY()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            int ungVienId = claim != null ? int.Parse(claim.Value) : 0;
            var ds = await _business.GoiYAsync(ungVienId);
            return Ok(ds);
        }
    }
}
