app.controller('RegisterCtrl', function ($location, Api, Toast) {
  var vm = this;
  vm.f = { vaiTro: 'UngVien' };
  vm.dangKy = function () {
    vm.loi = '';
    if (vm.f.matKhau !== vm.f.nhapLai) { vm.loi = 'Mật khẩu nhập lại không khớp'; return; }
    vm.dangGui = true;
    Api.post('/api/xacthuc/dang-ky', {
      email: vm.f.email, matKhau: vm.f.matKhau, vaiTro: vm.f.vaiTro, hoTen: vm.f.hoTen, soDienThoai: vm.f.soDienThoai
    }).then(function () {
      Toast.ok('Đăng ký thành công! Hãy đăng nhập.');
      $location.url('/dang-nhap');
    }, function (e) { vm.loi = Api.loi(e); }).finally(function () { vm.dangGui = false; });
  };
});