using BLL.Interfaces;
using DAL.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BaoCaoBusiness : IBaoCaoBusiness
    {
        private readonly IBaoCaoRepository _repo;
        public BaoCaoBusiness(IBaoCaoRepository repo) { _repo = repo; }
        public async Task<BaoCaoTongQuanModel> LayTongQuanAsync() { return await _repo.LayTongQuanAsync(); }
    }
}
