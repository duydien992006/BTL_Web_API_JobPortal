using DAL.Helper.Interfaces;
using DAL.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class BaoCaoRepository : IBaoCaoRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        public BaoCaoRepository(IDatabaseHelper dbHelper) { _dbHelper = dbHelper; }

        public async Task<BaoCaoTongQuanModel> LayTongQuanAsync()
        {
            return await _dbHelper.QueryFirstOrDefaultAsync<BaoCaoTongQuanModel>("sp_baocao_tongQuan");
        }
    }
}
