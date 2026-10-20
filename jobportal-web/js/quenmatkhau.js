app.controller('ForgotCtrl', function ($routeParams, $location, Api, Toast) {
  var vm = this;
  vm.datLai = !!$routeParams.token;
  vm.f = { email: $routeParams.email || '', token: $routeParams.token || '' };

  vm.guiYeuCau = function () {
    vm.loi = ''; vm.thongBao = '';
    Api.post('/api/xacthuc/quen-mat-khau', { email: vm.f.email })
      .then(function (kq) { vm.thongBao = kq.thongDiep; }, function (e) { vm.loi = Api.loi(e); });
  };
  vm.doiMatKhau = function () {
    vm.loi = '';
    if (vm.f.matKhauMoi !== vm.f.nhapLai) { vm.loi = 'Mật khẩu nhập lại không khớp'; return; }
    Api.post('/api/xacthuc/dat-lai-mat-khau', { email: vm.f.email, token: vm.f.token, matKhauMoi: vm.f.matKhauMoi })
      .then(function () { Toast.ok('Đặt lại mật khẩu thành công, hãy đăng nhập'); $location.url('/dang-nhap'); },
            function (e) { vm.loi = Api.loi(e); });
  };
});