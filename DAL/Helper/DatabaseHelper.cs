using DAL.Helper.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DAL.Helper
{
    public class DatabaseHelper : IDatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string spName, object param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<T>(spName, param, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string spName, object param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<T>(spName, param, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> ExecuteAsync(string spName, object param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.ExecuteScalarAsync<int>(spName, param, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(string spName, object param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.ExecuteScalarAsync<T>(spName, param, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<KetQuaKep<T1, T2>> QueryMultipleAsync<T1, T2>(string spName, object param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var multi = await connection.QueryMultipleAsync(spName, param, commandType: CommandType.StoredProcedure))
                {
                    var ketQua1 = await multi.ReadFirstOrDefaultAsync<T1>();
                    var ketQua2 = (await multi.ReadAsync<T2>()).ToList();
                    return new KetQuaKep<T1, T2> { KetQua1 = ketQua1, KetQua2 = ketQua2 };
                }
            }
        }

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
