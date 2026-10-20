app.controller('NtTinFormCtrl', function ($routeParams, $location, Api, Toast, KyNang) {
  var vm = this;
  vm.id = $routeParams.id;
  vm.sua = !!vm.id;
  vm.f = {}; vm.chon = {};
  KyNang.tatCa().then(function (ds) { vm.kyNang = ds; });

  if (vm.sua) {
    Api.get('/api/tintuyendung/' + vm.id).then(function (t) {
      vm.f = { congTyId: t.congTyId, tieuDe: t.tieuDe, moTaCongViec: t.moTaCongViec, diaDiem: t.diaDiem,
               mucLuongTu: t.mucLuongTu, mucLuongDen: t.mucLuongDen, ngayHetHan: new Date(t.ngayHetHan) };
    }, Api.bao);
  } else {
    Api.get('/api/congty/cua-toi').then(function (ds) {
      vm.congTy = ds;
      var dau = ds.filter(function (c) { return c.daXacMinh; })[0];
      if (dau) vm.f.congTyId = dau.id;
    }, Api.bao);
  }

  vm.luu = function () {
    vm.loi = '';
    if (vm.f.mucLuongTu > vm.f.mucLuongDen) { vm.loi = 'Mức lương từ phải nhỏ hơn hoặc bằng mức lương đến'; return; }
    var han = new Date(vm.f.ngayHetHan);
    han.setHours(23, 59, 59, 0);
    var body = { congTyId: vm.f.congTyId, tieuDe: vm.f.tieuDe, moTaCongViec: vm.f.moTaCongViec, diaDiem: vm.f.diaDiem,
                 mucLuongTu: vm.f.mucLuongTu || 0, mucLuongDen: vm.f.mucLuongDen || 0, ngayHetHan: han };
    if (!vm.sua) {
      var ids = [];
      angular.forEach(vm.chon, function (v, k) { if (v) ids.push(Number(k)); });
      if (!ids.length) { vm.loi = 'Hãy chọn ít nhất 1 kỹ năng yêu cầu'; return; }
      body.danhSachKyNangId = ids;
    }
    vm.dangGui = true;
    var p = vm.sua ? Api.put('/api/tintuyendung/' + vm.id, body) : Api.post('/api/tintuyendung', body);
    p.then(function () {
      Toast.ok(vm.sua ? 'Đã cập nhật tin' : 'Đã đăng tin, vui lòng chờ quản trị viên duyệt');
      $location.url('/nha-tuyen-dung/tin');
    }, function (e) { vm.loi = Api.loi(e); }).finally(function () { vm.dangGui = false; });
  };
});