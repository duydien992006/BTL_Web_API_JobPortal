app.controller('NtUngVienCtrl', function ($routeParams, $window, Api, Toast, Helper) {
  var vm = this;
  vm.tinId = $routeParams.id;
  vm.trangThai = '';
  vm.kq = { danhSach: [] };
  vm.dsTrangThai = ['DaNop', 'DangXemXet', 'PhongVan', 'DeNghi', 'TrungTuyen', 'UngVienTuChoi', 'TuChoi', 'DaRut'];

  Api.get('/api/tintuyendung/' + vm.tinId).then(function (t) { vm.tin = t; }, Api.bao);
  vm.tai = function (t) {
    Api.get('/api/donungtuyen/theo-tin/' + vm.tinId, { trangThai: vm.trangThai || undefined, trang: t || 1, kichThuoc: 10 })
      .then(function (kq) { vm.kq = kq; }, Api.bao);
  };
  vm.tai(1);

  vm.taiCv = function (d) { Api.taiVe('/api/hoso/' + d.hoSoId + '/tai-ve', d.tenFileGoc).catch(Api.bao); };

  vm.doiTrangThai = function (d, moi) {
    var hoi = moi === 'TuChoi' ? 'Từ chối ứng viên ' + d.hoTen + '?' : 'Chuyển đơn của ' + d.hoTen + ' sang "Đang xem xét"?';
    if (!$window.confirm(hoi)) return;
    Api.put('/api/donungtuyen/doi-trang-thai', { id: d.id, trangThaiMoi: moi }).then(function () {
      Toast.ok('Đã cập nhật trạng thái');
      vm.tai(vm.kq.trang);
      if (vm.m && vm.m.donId === d.id) vm.napChiTiet();
    }, Api.bao);
  };

  // ----- Cửa sổ quản lý 1 đơn -----
  vm.quanLy = function (d) {
    vm.m = { donId: d.id, hoTen: d.hoTen, lich: {}, offer: {} };
    vm.napChiTiet();
  };
  vm.napChiTiet = function () {
    Api.get('/api/donungtuyen/chi-tiet/' + vm.m.donId).then(function (ct) { vm.m.ct = ct; }, Api.bao);
  };
  vm.coLichDat = function () {
    return !!(vm.m && vm.m.ct && vm.m.ct.lichPhongVan.some(function (l) { return l.ketQua === 'Dat'; }));
  };
  vm.taoLich = function () {
    Api.post('/api/lichphongvan/tao-lich', {
      donUngTuyenId: vm.m.donId, thoiGianPhongVan: vm.m.lich.thoiGian, diaDiemPhongVan: vm.m.lich.diaDiem
    }).then(function () {
      Toast.ok('Đã hẹn lịch phỏng vấn'); vm.m.lich = {}; vm.napChiTiet(); vm.tai(vm.kq.trang);
    }, Api.bao);
  };
  vm.ghiKetQua = function (l, kq) {
    Api.put('/api/lichphongvan/' + l.id + '/ket-qua', null, { ketQua: kq })
      .then(function () { Toast.ok('Đã ghi kết quả phỏng vấn'); vm.napChiTiet(); }, Api.bao);
  };
  vm.guiOffer = function () {
    Api.post('/api/denghituyendung/gui', {
      donUngTuyenId: vm.m.donId, mucLuongDeNghi: vm.m.offer.luong, ngayBatDauLamViec: Helper.ngayUtc(vm.m.offer.ngay)
    }).then(function () {
      Toast.ok('Đã gửi đề nghị tuyển dụng'); vm.m.offer = {}; vm.napChiTiet(); vm.tai(vm.kq.trang);
    }, Api.bao);
  };
});