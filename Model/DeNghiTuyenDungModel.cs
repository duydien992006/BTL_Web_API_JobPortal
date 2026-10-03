using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DeNghiTuyenDungModel
    {
        public int Id { get; set; }
        public int DonUngTuyenId { get; set; }
        public decimal MucLuongDeNghi { get; set; }
        public DateTime NgayBatDauLamViec { get; set; }
        public string TrangThai { get; set; }
    }
}
