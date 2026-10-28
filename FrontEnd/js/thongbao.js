app.controller('ThongBaoCtrl', function ($rootScope, Api) {
  var vm = this;
  vm.kq = { danhSach: [] };
  vm.tai = function (t) { Api.get('/api/thongbao', { trang: t || 1, kichThuoc: 10 }).then(function (kq) { vm.kq = kq; }, Api.bao); };
  vm.tai(1);
  vm.doc = function (n) {
    if (n.daDoc) return;
    Api.put('/api/thongbao/' + n.id + '/da-doc').then(function () {
      n.daDoc = true; vm.kq.soChuaDoc--;
      $rootScope.$broadcast('thongbao:doi');
    }, Api.bao);
  };
});