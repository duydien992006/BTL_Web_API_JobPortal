using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Model;
namespace Common.Problems
{
    public static class ControllerExtensions
    {
        // Thành công  -> 200 + PhanHoiModel (như cũ)
        // Thất bại    -> ProblemDetails, "detail" là ThongDiep của Business
        public static IActionResult KetQua(this ControllerBase controller, PhanHoiModel kq, int maLoiHttp = 400)
        {
            if (kq.ThanhCong) return controller.Ok(kq);

            var pd = new ProblemDetails { Status = maLoiHttp, Detail = kq.ThongDiep };
            ProblemDetailsHelper.DienMacDinh(controller.HttpContext, pd);

            return new ObjectResult(pd)
            {
                StatusCode = maLoiHttp,
                ContentTypes = { ProblemDetailsHelper.ContentType }
            };
        }
    }
}
