using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    // Exception "có kiểm soát": message an toàn để trả thẳng cho client
    public class AppException : Exception
    {
        public int StatusCode { get; }
        public AppException(string message, int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class NghiepVuException : AppException
    {
        public NghiepVuException(string message) : base(message, 400) { }
    }

    public class KhongTimThayException : AppException
    {
        public KhongTimThayException(string message) : base(message, 404) { }
    }

    public class KhongCoQuyenException : AppException
    {
        public KhongCoQuyenException(string message) : base(message, 403) { }
    }

    public class XungDotDuLieuException : AppException
    {
        public XungDotDuLieuException(string message) : base(message, 409) { }
    }
}
