app.controller('JobsCtrl', function ($routeParams, Api, KyNang) {
  var vm = this;
  vm.luongOpts = [{ v: 0, t: 'Tất cả' }, { v: 5000000, t: 'Từ 5 triệu' }, { v: 10000000, t: 'Từ 10 triệu' },
                  { v: 15000000, t: 'Từ 15 triệu' }, { v: 20000000, t: 'Từ 20 triệu' }, { v: 30000000, t: 'Từ 30 triệu' }];
  vm.loc = { tuKhoa: $routeParams.tuKhoa || '', diaDiem: $routeParams.diaDiem || '', luongToiThieu: 0 };
  vm.chon = {};
  vm.kq = { danhSach: [], tongSo: 0, trang: 1, tongSoTrang: 0 };
  ($routeParams.kyNang || '').split(',').forEach(function (id) { if (id) vm.chon[id] = true; });
  KyNang.tatCa().then(function (ds) { vm.kyNang = ds; });

  vm.tim = function (trang) {
    var ids = [];
    angular.forEach(vm.chon, function (v, k) { if (v) ids.push(Number(k)); });
    vm.dangTai = true;
    Api.post('/api/tintuyendung/tim-kiem', {
      tuKhoa: vm.loc.tuKhoa, diaDiem: vm.loc.diaDiem, luongToiThieu: vm.loc.luongToiThieu || null,
      danhSachKyNangId: ids, trang: trang || 1, kichThuoc: 10
    }).then(function (kq) { vm.kq = kq; }, Api.bao).finally(function () { vm.dangTai = false; });
  };
  vm.xoaLoc = function () { vm.loc = { tuKhoa: '', diaDiem: '', luongToiThieu: 0 }; vm.chon = {}; vm.tim(1); };
  vm.tim(1);
});