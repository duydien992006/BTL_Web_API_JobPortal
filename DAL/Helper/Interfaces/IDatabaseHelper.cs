using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Helper.Interfaces
{
    public interface IDatabaseHelper
    {
        Task<IEnumerable<T>> QueryAsync<T>(string spName, object param = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string spName, object param = null);
        Task<int> ExecuteAsync(string spName, object param = null);
        Task<T> ExecuteScalarAsync<T>(string spName, object param = null);
        Task<KetQuaKep<T1, T2>> QueryMultipleAsync<T1, T2>(string spName, object param = null);
        DataTable TaoBangId(IEnumerable<int> danhSachId);
        Task<T> QueryMultipleAsync<T>(string spName, object param, Func<SqlMapper.GridReader, Task<T>> docKetQua);
    }

    public class KetQuaKep<T1, T2>
    {
        public T1 KetQua1 { get; set; }
        public List<T2> KetQua2 { get; set; }
    }
}
