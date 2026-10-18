using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public static class TrangThaiHopLe
    {
        public static readonly string[] Don = { "DaNop", "DangXemXet", "PhongVan", "DeNghi", "TrungTuyen", "UngVienTuChoi", "TuChoi", "DaRut" };
        public static readonly string[] Tin = { "ChoDuyet", "DaDuyet", "TuChoi", "DaDong" };

        // Rỗng = không lọc. Có giá trị mà sai thì báo 400 kèm danh sách giá trị đúng
        public static string ChuanHoa(string giaTri, string[] hopLe)
        {
            if (string.IsNullOrWhiteSpace(giaTri)) return null;
            string g = giaTri.Trim();
            if (!hopLe.Contains(g))
                throw new NghiepVuException("Giá trị lọc trạng thái không hợp lệ. Chấp nhận: " + string.Join(", ", hopLe));
            return g;
        }
    }
}
