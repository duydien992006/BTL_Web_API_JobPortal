app.controller('TaiKhoanCtrl', function (Api, Toast, Auth) {
  var vm = this;
  vm.mk = {};
  Api.get('/api/nguoidung/thong-tin-ca-nhan').then(function (u) {
    vm.u = u; vm.f = { hoTen: u.hoTen, soDienThoai: u.soDienThoai };
  }, Api.bao);

  vm.luu = function () {
    Api.put('/api/nguoidung/cap-nhat', vm.f).then(function () {
      Auth.capNhatTen(vm.f.hoTen); Toast.ok('Đã cập nhật thông tin');
    }, Api.bao);
  };
  vm.doiMatKhau = function () {
    vm.loi = '';
    if (vm.mk.moi !== vm.mk.nhapLai) { vm.loi = 'Mật khẩu nhập lại không khớp'; return; }
    Api.put('/api/nguoidung/doi-mat-khau', { matKhauCu: vm.mk.cu, matKhauMoi: vm.mk.moi })
      .then(function () { Toast.ok('Đổi mật khẩu thành công'); vm.mk = {}; }, function (e) { vm.loi = Api.loi(e); });
  };
});