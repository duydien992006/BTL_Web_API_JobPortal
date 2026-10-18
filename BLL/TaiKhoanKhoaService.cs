using BLL.Interfaces;
using DAL.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    // Được gọi MỖI request có token. Cache 30 giây để không truy vấn DB liên tục,
    // nên lệnh khóa/mở khóa có hiệu lực trong tối đa 30 giây
    public class TaiKhoanKhoaService : ITaiKhoanKhoaService
    {
        private readonly INguoiDungRepository _repo;
        private readonly IMemoryCache _cache;

        public TaiKhoanKhoaService(INguoiDungRepository repo, IMemoryCache cache)
        {
            _repo = repo;
            _cache = cache;
        }

        public async Task<bool> BiKhoaAsync(int nguoiDungId)
        {
            string khoa = "taikhoan-bi-khoa:" + nguoiDungId;
            bool biKhoa;
            if (_cache.TryGetValue(khoa, out biKhoa)) return biKhoa;

            biKhoa = await _repo.BiKhoaAsync(nguoiDungId);
            _cache.Set(khoa, biKhoa, TimeSpan.FromSeconds(30));
            return biKhoa;
        }
    }
}
