using DAL.Helper.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Helper
{
    public class DatabaseHelper : IDatabaseHelper
    {
        private const int NguongChamMs = 500;
        private readonly string _connectionString;
        private readonly ILogger<DatabaseHelper> _logger;

        public DatabaseHelper(IConfiguration configuration, ILogger<DatabaseHelper> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        // Hàm trung tâm: mở kết nối + đo thời gian + log. Lỗi vẫn ném lên (Global Handler sẽ log)
        private async Task<T> ChayAsync<T>(string spName, Func<SqlConnection, Task<T>> thucThi)
        {
            var dongHo = Stopwatch.StartNew();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    return await thucThi(connection);
                }
            }
            finally
            {
                dongHo.Stop();
                if (dongHo.ElapsedMilliseconds >= NguongChamMs)
                    _logger.LogWarning("Stored Procedure chạy chậm: {SpName} mất {ElapsedMs} ms", spName, dongHo.ElapsedMilliseconds);
                else
                    _logger.LogDebug("Stored Procedure {SpName} hoàn tất trong {ElapsedMs} ms", spName, dongHo.ElapsedMilliseconds);
            }
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string spName, object param = null)
            => ChayAsync(spName, c => c.QueryAsync<T>(spName, param, commandType: CommandType.StoredProcedure));

        public Task<T> QueryFirstOrDefaultAsync<T>(string spName, object param = null)
            => ChayAsync(spName, c => c.QueryFirstOrDefaultAsync<T>(spName, param, commandType: CommandType.StoredProcedure));

        // Các SP UPDATE/DELETE đều SELECT số dòng ảnh hưởng ở cuối nên dùng ExecuteScalar
        public Task<int> ExecuteAsync(string spName, object param = null)
            => ChayAsync(spName, c => c.ExecuteScalarAsync<int>(spName, param, commandType: CommandType.StoredProcedure));

        public Task<T> ExecuteScalarAsync<T>(string spName, object param = null)
            => ChayAsync(spName, c => c.ExecuteScalarAsync<T>(spName, param, commandType: CommandType.StoredProcedure));

        public Task<KetQuaKep<T1, T2>> QueryMultipleAsync<T1, T2>(string spName, object param = null)
            => ChayAsync(spName, async c =>
            {
                using (var multi = await c.QueryMultipleAsync(spName, param, commandType: CommandType.StoredProcedure))
                {
                    var ketQua1 = await multi.ReadFirstOrDefaultAsync<T1>();
                    var ketQua2 = (await multi.ReadAsync<T2>()).ToList();
                    return new KetQuaKep<T1, T2> { KetQua1 = ketQua1, KetQua2 = ketQua2 };
                }
            });

        public DataTable TaoBangId(IEnumerable<int> danhSachId)
        {
            var bang = new DataTable();
            bang.Columns.Add("Id", typeof(int));
            if (danhSachId != null)
            {
                foreach (var id in danhSachId) bang.Rows.Add(id);
            }
            return bang;
        }
    }
}
