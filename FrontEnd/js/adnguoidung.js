app.controller('AdNguoiDungCtrl', function (Api, Toast) {
  var vm = this;
  vm.f = { tuKhoa: '', vaiTro: '' };
  vm.kq = { danhSach: [] };
  vm.tai = function (t) {
    Api.get('/api/quan-ly-nguoi-dung', { tuKhoa: vm.f.tuKhoa || undefined, vaiTro: vm.f.vaiTro || undefined, trang: t || 1, kichThuoc: 10 })
      .then(function (kq) { vm.kq = kq; }, Api.bao);
  };
  vm.tai(1);
  vm.khoa = function (u) { vm.m = { u: u, lyDo: '' }; };
  vm.xacNhanKhoa = function () {
    Api.put('/api/quan-ly-nguoi-dung/' + vm.m.u.id + '/khoa', null, { khoa: true, lyDo: vm.m.lyDo })
      .then(function (kq) { Toast.ok(kq.thongDiep); vm.m = null; vm.tai(vm.kq.trang); }, Api.bao);
  };
  vm.moKhoa = function (u) {
    Api.put('/api/quan-ly-nguoi-dung/' + u.id + '/khoa', null, { khoa: false })
      .then(function (kq) { Toast.ok(kq.thongDiep); vm.tai(vm.kq.trang); }, Api.bao);
  };
});