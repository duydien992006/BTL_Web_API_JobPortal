using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public static class PhanTrangHelper
    {
        public const int KichThuocMacDinh = 10;
        public const int KichThuocToiDa = 50;

        public static int Trang(int trang)
        {
            return trang < 1 ? 1 : trang;
        }

        public static int KichThuoc(int kichThuoc)
        {
            if (kichThuoc < 1) return KichThuocMacDinh;
            return kichThuoc > KichThuocToiDa ? KichThuocToiDa : kichThuoc;
        }
    }
}
