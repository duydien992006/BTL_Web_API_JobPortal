app.controller('AdDashboardCtrl', function (Api, Helper) {
  var vm = this;
  Api.get('/api/baocao/tong-quan').then(function (t) { vm.t = t; }, Api.bao);
  Api.get('/api/baocao/pipeline').then(function (p) {
    vm.p = p;
    vm.maxDon = Helper.max(p.donTheoTrangThai, 'soLuong');
    vm.maxNgay = Helper.max(p.donTheoNgay, 'soDon');
  }, Api.bao);
  vm.pt = function (v, max) { return Math.round(v / max * 100); };
});