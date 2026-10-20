app.controller('NtTinCtrl', function ($window, Api, Toast) {
  var vm = this;
  vm.trangThai = '';
  vm.kq = { danhSach: [] };
  vm.tai = function (t) {
    Api.get('/api/tintuyendung/cua-toi', { trangThai: vm.trangThai || undefined, trang: t || 1, kichThuoc: 10 })
      .then(function (kq) { vm.kq = kq; }, Api.bao);
  };
  vm.tai(1);
  vm.dong = function (j) {
    if (!$window.confirm('Đóng tin "' + j.tieuDe + '"?')) return;
    Api.put('/api/tintuyendung/' + j.id + '/dong').then(function () { Toast.ok('Đã đóng tin'); vm.tai(vm.kq.trang); }, Api.bao);
  };
  vm.xoa = function (j) {
    if (!$window.confirm('Xóa tin "' + j.tieuDe + '"?')) return;
    Api.del('/api/tintuyendung/' + j.id).then(function () { Toast.ok('Đã xóa tin'); vm.tai(vm.kq.trang); }, Api.bao);
  };
});