using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class HoSoModel
    {
        public int Id { get; set; }
        public int UngVienId { get; set; }
        public string TenFileGoc { get; set; }
        public string DuongDanFile { get; set; }
        public int SoNamKinhNghiem { get; set; }
        public DateTime NgayTaiLen { get; set; }
        public List<KyNangModel> DanhSachKyNang { get; set; } = new List<KyNangModel>();
    }

    public class TaoHoSoModel
    {
        public int UngVienId { get; set; }
        public string TenFileGoc { get; set; }
        public string DuongDanFile { get; set; }
        public int SoNamKinhNghiem { get; set; }
        public List<int> DanhSachKyNangId { get; set; } = new List<int>();
    }
}
