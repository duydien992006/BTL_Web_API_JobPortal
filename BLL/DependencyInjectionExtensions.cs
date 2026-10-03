using BLL.Interfaces;
using DAL;
using DAL.Helper;
using DAL.Helper.Interfaces;
using DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddJobPortalServices(this IServiceCollection services)
        {
            services.AddScoped<IDatabaseHelper, DatabaseHelper>();

            services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
            services.AddScoped<ICongTyRepository, CongTyRepository>();
            services.AddScoped<IKyNangRepository, KyNangRepository>();
            services.AddScoped<ITinTuyenDungRepository, TinTuyenDungRepository>();
            services.AddScoped<IHoSoRepository, HoSoRepository>();
            services.AddScoped<IDonUngTuyenRepository, DonUngTuyenRepository>();
            services.AddScoped<ILichPhongVanRepository, LichPhongVanRepository>();
            services.AddScoped<IDeNghiTuyenDungRepository, DeNghiTuyenDungRepository>();
            services.AddScoped<IThongBaoRepository, ThongBaoRepository>();
            services.AddScoped<IBaoCaoRepository, BaoCaoRepository>();

            services.AddScoped<INguoiDungBusiness, NguoiDungBusiness>();
            services.AddScoped<ICongTyBusiness, CongTyBusiness>();
            services.AddScoped<IKyNangBusiness, KyNangBusiness>();
            services.AddScoped<ITinTuyenDungBusiness, TinTuyenDungBusiness>();
            services.AddScoped<IHoSoBusiness, HoSoBusiness>();
            services.AddScoped<IDonUngTuyenBusiness, DonUngTuyenBusiness>();
            services.AddScoped<ILichPhongVanBusiness, LichPhongVanBusiness>();
            services.AddScoped<IDeNghiTuyenDungBusiness, DeNghiTuyenDungBusiness>();
            services.AddScoped<IThongBaoBusiness, ThongBaoBusiness>();
            services.AddScoped<IBaoCaoBusiness, BaoCaoBusiness>();

            return services;
        }
    }
}
