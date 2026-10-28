app.controller('LoginCtrl', function ($rootScope, $location, Auth, Api, Toast) {
  var vm = this;
  vm.f = {};
  vm.dangNhap = function () {
    vm.dangGui = true; vm.loi = '';
    Auth.dangNhap(vm.f.email, vm.f.matKhau).then(function (u) {
      Toast.ok('Xin chào ' + u.hoTen + '!');
      var dich = $rootScope.quayLai || Auth.trangChu();
      $rootScope.quayLai = null;
      $location.url(dich);
    }, function (e) { vm.loi = Api.loi(e); }).finally(function () { vm.dangGui = false; });
  };
});