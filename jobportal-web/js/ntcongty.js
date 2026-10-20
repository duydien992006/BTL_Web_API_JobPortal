app.controller('NtCongTyCtrl', function ($window, Api, Toast) {
  var vm = this;
  vm.tai = function () { Api.get('/api/congty/cua-toi').then(function (ds) { vm.ds = ds; }, Api.bao); };
  vm.tai();
  vm.moi = function () { vm.f = {}; vm.hien = true; };
  vm.sua = function (c) { vm.f = angular.copy(c); vm.hien = true; };
  vm.luu = function () {
    var body = { tenCongTy: vm.f.tenCongTy, moTa: vm.f.moTa, logoUrl: vm.f.logoUrl, diaChi: vm.f.diaChi };
    var p = vm.f.id ? Api.put('/api/congty/' + vm.f.id, body) : Api.post('/api/congty', body);
    p.then(function () {
      Toast.ok(vm.f.id ? 'Đã cập nhật công ty' : 'Đã tạo công ty, vui lòng chờ quản trị viên xác minh');
      vm.hien = false; vm.tai();
    }, Api.bao);
  };
  vm.xoa = function (c) {
    if (!$window.confirm('Xóa công ty "' + c.tenCongTy + '"?')) return;
    Api.del('/api/congty/' + c.id).then(function () { Toast.ok('Đã xóa công ty'); vm.tai(); }, Api.bao);
  };
});