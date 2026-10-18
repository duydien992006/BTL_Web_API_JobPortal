using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CongTyModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(200, ErrorMessage = LoiValidate.QuaDai)]
        public string TenCongTy { get; set; }

        [StringLength(5000, ErrorMessage = LoiValidate.QuaDai)]
        public string MoTa { get; set; }

        [StringLength(500, ErrorMessage = LoiValidate.QuaDai)]
        public string LogoUrl { get; set; }

        [StringLength(300, ErrorMessage = LoiValidate.QuaDai)]
        public string DiaChi { get; set; }

        public int NguoiDaiDienId { get; set; }   // bị bỏ qua, luôn lấy từ token
        public bool DaXacMinh { get; set; }
    }
}
