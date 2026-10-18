using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Range(1, int.MaxValue, ErrorMessage = LoiValidate.IdHopLe)]
        public int TinTuyenDungId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = LoiValidate.IdHopLe)]
        public int HoSoId { get; set; }
    }

    public class DoiTrangThaiDonModel
    {
        [Range(1, int.MaxValue, ErrorMessage = LoiValidate.IdHopLe)]
        public int Id { get; set; }

        [Required(ErrorMessage = LoiValidate.BatBuoc)]
        [StringLength(30, ErrorMessage = LoiValidate.QuaDai)]
        public string TrangThaiMoi { get; set; }
    }
}
