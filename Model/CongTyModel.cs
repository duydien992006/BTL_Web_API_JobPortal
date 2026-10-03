using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CongTyModel
    {
        public int Id { get; set; }
        public string TenCongTy { get; set; }
        public string MoTa { get; set; }
        public string LogoUrl { get; set; }
        public string DiaChi { get; set; }
        public int NguoiDaiDienId { get; set; }
        public bool DaXacMinh { get; set; }
    }
}
