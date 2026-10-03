using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ThongBaoModel
    {
        public int Id { get; set; }
        public int NguoiDungId { get; set; }
        public string NoiDung { get; set; }
        public bool DaDoc { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
