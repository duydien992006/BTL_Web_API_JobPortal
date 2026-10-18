using Common.Exceptions;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public static class QuyenHelper
    {
        // Không tồn tại -> 404
        public static T PhaiTonTai<T>(T doiTuong, string thongDiep) where T : class
        {
            if (doiTuong == null) throw new KhongTimThayException(thongDiep);
            return doiTuong;
        }

        // Phải là chủ sở hữu (hoặc Admin nếu choPhepAdmin = true) -> không đạt thì 403
        public static void PhaiLaChuSoHuu(int chuSoHuuId, NguoiDungHienTai nguoi, bool choPhepAdmin = false)
        {
            if (choPhepAdmin && nguoi.LaAdmin) return;
            if (chuSoHuuId <= 0 || chuSoHuuId != nguoi.Id)
                throw new KhongCoQuyenException("Bạn không có quyền thao tác trên dữ liệu này");
        }

        // Một trong hai bên của đơn ứng tuyển (ứng viên hoặc nhà tuyển dụng của đơn đó), hoặc Admin
        public static void PhaiLaBenLienQuan(ThongTinSoHuuModel don, NguoiDungHienTai nguoi)
        {
            if (nguoi.LaAdmin) return;
            bool laUngVien = don.UngVienId > 0 && don.UngVienId == nguoi.Id;
            bool laNhaTuyenDung = don.NhaTuyenDungId > 0 && don.NhaTuyenDungId == nguoi.Id;
            if (!laUngVien && !laNhaTuyenDung)
                throw new KhongCoQuyenException("Bạn không có quyền xem dữ liệu của đơn ứng tuyển này");
        }
    }
}
