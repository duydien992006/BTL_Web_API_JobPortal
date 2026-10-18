using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class KyNangModel
    {
        public int Id { get; set; }
        public string TenKyNang { get; set; }
    }

    public class TaoKyNangModel
    {
        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(100, ErrorMessage = LoiValidate.QuaDai)]
        public string TenKyNang { get; set; }
    }
}
