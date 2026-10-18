using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    // {0} = tên trường, {1}/{2} = giới hạn. Dùng làm ErrorMessage trong các attribute validate
    public static class LoiValidate
    {
        public const string BatBuoc = "{0} là bắt buộc";
        public const string QuaDai = "{0} tối đa {1} ký tự";
        public const string DoDai = "{0} phải từ {2} đến {1} ký tự";    // StringLength có MinimumLength
        public const string QuaNhieu = "{0} tối đa {1} phần tử";
        public const string Khoang = "{0} phải nằm trong khoảng {1} đến {2}";
        public const string KhongAm = "{0} phải là số không âm";
        public const string SoDuong = "{0} phải lớn hơn 0";
        public const string IdHopLe = "{0} không hợp lệ";
        public const string Email = "Email không đúng định dạng";
    }
}
