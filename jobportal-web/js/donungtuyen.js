app.controller('DonCtrl', function ($window, Api, Toast) {
  var vm = this;
  vm.trangThai = '';
  vm.kq = { danhSach: [] };
  vm.dsTrangThai = ['DaNop', 'DangXemXet', 'PhongVan', 'DeNghi', 'TrungTuyen', 'UngVienTuChoi', 'TuChoi', 'DaRut'];

  vm.tai = function (t) {
    Api.get('/api/donungtuyen/cua-toi', { trangThai: vm.trangThai || undefined, trang: t || 1, kichThuoc: 8 })
      .then(function (kq) { vm.kq = kq; }, Api.bao);
  };
  vm.tai(1);

  vm.coTheRut = function (d) { return ['DaNop', 'DangXemXet', 'PhongVan'].indexOf(d.trangThai) >= 0; };
  vm.rut = function (d) {
    if (!$window.confirm('Rút đơn ứng tuyển "' + d.tieuDe + '"? Bạn sẽ không nộp lại được tin này.')) return;
    Api.put('/api/donungtuyen/' + d.id + '/rut').then(function () { Toast.ok('Đã rút đơn'); vm.tai(vm.kq.trang); }, Api.bao);
  };
  vm.phanHoi = function (d, chap) {
    if (!$window.confirm(chap ? 'Chấp nhận offer này?' : 'Từ chối offer này?')) return;
    Api.put('/api/denghituyendung/' + d.deNghiId + '/phan-hoi', null, { chapNhan: chap })
      .then(function () { Toast.ok(chap ? 'Chúc mừng bạn đã trúng tuyển!' : 'Đã từ chối offer'); vm.tai(vm.kq.trang); }, Api.bao);
  };
  vm.xem = function (d) {
    vm.hien = true; vm.ct = null;
    Api.get('/api/donungtuyen/chi-tiet/' + d.id).then(function (ct) { vm.ct = ct; }, function (e) { vm.hien = false; Api.bao(e); });
  };
});