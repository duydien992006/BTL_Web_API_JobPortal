app.controller('JobDetailCtrl', function ($routeParams, Api, Auth, Toast) {
  var vm = this;
  vm.auth = Auth;
  vm.id = $routeParams.id;

  Api.get('/api/tintuyendung/' + vm.id).then(function (tin) {
    vm.tin = tin;
    Api.get('/api/congty/' + tin.congTyId).then(function (ct) { vm.congTy = ct; });
  }, function () { vm.khongCo = true; });

  vm.hetHan = function () { return vm.tin && new Date(vm.tin.ngayHetHan) < new Date(); };
  vm.moNop = function () {
    vm.hienNop = true;
    Api.get('/api/hoso/cua-toi').then(function (ds) { vm.hoSo = ds; if (ds.length) vm.hoSoId = ds[0].id; }, Api.bao);
  };
  vm.nop = function () {
    vm.dangNop = true;
    Api.post('/api/donungtuyen/nop-don', { tinTuyenDungId: Number(vm.id), hoSoId: vm.hoSoId })
      .then(function () { Toast.ok('Nộp đơn ứng tuyển thành công!'); vm.hienNop = false; vm.daNop = true; }, Api.bao)
      .finally(function () { vm.dangNop = false; });
  };
});