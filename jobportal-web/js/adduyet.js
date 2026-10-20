app.controller('AdDuyetCtrl', function (Api, Toast, $window) {
  var vm = this;
  vm.tai = function () {
    Api.get('/api/tintuyendung/cho-duyet').then(function (ds) { vm.tin = ds; }, Api.bao);
    Api.get('/api/congty/cho-xac-minh').then(function (ds) { vm.congTy = ds; }, Api.bao);
  };
  vm.tai();
  vm.duyet = function (t, ok) {
    if (!ok && !$window.confirm('Từ chối tin "' + t.tieuDe + '"?')) return;
    Api.put('/api/tintuyendung/' + t.id + '/duyet', null, { duyet: ok })
      .then(function () { Toast.ok(ok ? 'Đã duyệt tin' : 'Đã từ chối tin'); vm.tai(); }, Api.bao);
  };
  vm.xacMinh = function (c) {
    Api.put('/api/congty/' + c.id + '/xac-minh', null, { daXacMinh: true })
      .then(function () { Toast.ok('Đã xác minh công ty'); vm.tai(); }, Api.bao);
  };
});