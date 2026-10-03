using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DonUngTuyenModel
    {
        public int Id { get; set; }
        public int TinTuyenDungId { get; set; }
        public int HoSoId { get; set; }
        public int UngVienId { get; set; }
        public string TrangThai { get; set; }
        public DateTime NgayUngTuyen { get; set; }
    }

    public class NopDonModel
    {
        public int TinTuyenDungId { get; set; }
        public int HoSoId { get; set; }
    }

    public class DoiTrangThaiDonModel
    {
        public int Id { get; set; }
        public string TrangThaiMoi { get; set; }
    }
}
