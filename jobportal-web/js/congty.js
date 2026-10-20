app.controller('CompaniesCtrl', function (Api) {
  var vm = this;
  vm.tai = function (t) { Api.get('/api/congty', { trang: t || 1, kichThuoc: 12 }).then(function (kq) { vm.kq = kq; }, Api.bao); };
  vm.tai(1);
});