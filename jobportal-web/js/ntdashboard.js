app.controller('NtDashboardCtrl', function (Api, Helper) {
  var vm = this;
  Api.get('/api/dashboard/nha-tuyen-dung').then(function (d) {
    vm.d = d;
    vm.max = Helper.max(d.pipeline, 'soLuong');
  }, Api.bao);
  vm.pt = function (v) { return Math.round(v / vm.max * 100); };
});