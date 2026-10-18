using BLL.Interfaces;
using Common.Problems;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace APINguoiDung.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XacThucController : ControllerBase
    {
        private readonly INguoiDungBusiness _business;
        private readonly IMatKhauBusiness _matKhauBusiness;

        // CHỈ ĐƯỢC CÓ 1 CONSTRUCTOR PUBLIC
        public XacThucController(INguoiDungBusiness business, IMatKhauBusiness matKhauBusiness)
        {
            _business = business;
            _matKhauBusiness = matKhauBusiness;
        }

        [HttpPost("dang-ky")]
        public async Task<IActionResult> DangKy([FromBody] DangKyModel model)
        {
            return this.KetQua(await _business.DangKyAsync(model));
        }

        [HttpPost("dang-nhap")]
        public async Task<IActionResult> DangNhap([FromBody] DangNhapModel model)
        {
            return this.KetQua(await _business.DangNhapAsync(model), 401);
        }

        [HttpPost("quen-mat-khau")]
        public async Task<IActionResult> QuenMatKhau([FromBody] QuenMatKhauModel model)
        {
            return this.KetQua(await _matKhauBusiness.QuenMatKhauAsync(model));
        }

        [HttpPost("dat-lai-mat-khau")]
        public async Task<IActionResult> DatLaiMatKhau([FromBody] DatLaiMatKhauModel model)
        {
            return this.KetQua(await _matKhauBusiness.DatLaiMatKhauAsync(model));
        }
    }
}
