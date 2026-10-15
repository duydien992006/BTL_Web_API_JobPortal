using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace APINguoiDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NguoiDungController : ControllerBase
    {
        private readonly INguoiDungBusiness _business;
        public NguoiDungController(INguoiDungBusiness business) { _business = business; }

        [HttpGet("thong-tin-ca-nhan")]
        public async Task<IActionResult> ThongTinCaNhan()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            int id = claim != null ? int.Parse(claim.Value) : 0;
            var tt = await _business.LayThongTinCaNhanAsync(id);
            if (tt == null) return NotFound();
            return Ok(tt);
        }

        [HttpPut("cap-nhat")]
        public async Task<IActionResult> CapNhat([FromBody] CapNhatNguoiDungModel model)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "Id");
            int id = claim != null ? int.Parse(claim.Value) : 0;
            model.Id = id;   // chỉ cho phép tự sửa thông tin của chính mình, không cho sửa người khác
            var kq = await _business.CapNhatAsync(model);
            if (kq.ThanhCong) return Ok(kq);
            return NotFound(kq);
        }
    }
}
