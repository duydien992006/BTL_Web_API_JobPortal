using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; }
        public string MaLoi { get; }

        public AppException(string message, int statusCode = 400, string maLoi = "LOI_NGHIEP_VU")
            : base(message)
        {
            StatusCode = statusCode;
            MaLoi = maLoi;
        }
    }

    public class NghiepVuException : AppException
    {
        public NghiepVuException(string message) : base(message, 400, "LOI_NGHIEP_VU") { }
    }

    public class KhongTimThayException : AppException
    {
        public KhongTimThayException(string message) : base(message, 404, "KHONG_TIM_THAY") { }
    }

    public class KhongCoQuyenException : AppException
    {
        public KhongCoQuyenException(string message) : base(message, 403, "KHONG_CO_QUYEN") { }
    }

    public class XungDotDuLieuException : AppException
    {
        public XungDotDuLieuException(string message) : base(message, 409, "XUNG_DOT_DU_LIEU") { }
    }
}
