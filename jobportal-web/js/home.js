app.controller('HomeCtrl', function ($location, Api, Auth, KyNang) {
  var vm = this;
  vm.tuKhoa = ''; vm.diaDiem = '';
  vm.tim = function () {
    var q = {};
    if (vm.tuKhoa) q.tuKhoa = vm.tuKhoa;
    if (vm.diaDiem) q.diaDiem = vm.diaDiem;
    $location.path('/viec-lam').search(q);
  };
  KyNang.tatCa().then(function (ds) { vm.kyNang = ds.slice(0, 10); });
  Api.post('/api/tintuyendung/tim-kiem', { trang: 1, kichThuoc: 8 }).then(function (kq) { vm.moiNhat = kq.danhSach; });
  Api.get('/api/congty', { trang: 1, kichThuoc: 8 }).then(function (kq) { vm.congTy = kq.danhSach; });
  if (Auth.laUV()) Api.get('/api/tintuyendung/goi-y').then(function (ds) { vm.goiY = ds; }, function () {});
});