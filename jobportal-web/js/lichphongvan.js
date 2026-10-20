app.controller('LichCtrl', function ($window, Api, Auth, Toast) {
  var vm = this;
  vm.laNTD = Auth.laNTD();
  vm.tai = function () { Api.get('/api/lichphongvan/cua-toi').then(function (ds) { vm.ds = ds; }, Api.bao); };
  vm.tai();
  vm.sapToi = function (l) { return l.ketQua === 'ChoKetQua' && new Date(l.thoiGianPhongVan) >= new Date(); };
  vm.ghiKetQua = function (l, kq) {
    if (!$window.confirm('Ghi kết quả "' + (kq === 'Dat' ? 'Đạt' : 'Không đạt') + '" cho buổi phỏng vấn này?')) return;
    Api.put('/api/lichphongvan/' + l.id + '/ket-qua', null, { ketQua: kq })
      .then(function () { Toast.ok('Đã ghi kết quả'); vm.tai(); }, Api.bao);
  };
});