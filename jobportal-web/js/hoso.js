app.controller('HoSoCtrl', function ($window, Api, Toast, KyNang) {
  var vm = this;
  vm.f = { soNam: 0 }; vm.chon = {}; vm.file = null;
  KyNang.tatCa().then(function (ds) { vm.kyNang = ds; });

  vm.tai = function () { Api.get('/api/hoso/cua-toi').then(function (ds) { vm.ds = ds; }, Api.bao); };
  vm.tai();

  vm.upload = function () {
    var ids = [];
    angular.forEach(vm.chon, function (v, k) { if (v) ids.push(k); });
    if (!vm.file) { Toast.loi('Hãy chọn file CV (PDF, DOC hoặc DOCX)'); return; }
    if (!ids.length) { Toast.loi('Hãy chọn ít nhất 1 kỹ năng'); return; }

    var fd = new FormData();
    fd.append('file', vm.file);
    fd.append('soNamKinhNghiem', vm.f.soNam || 0);
    fd.append('danhSachKyNangId', ids.join(','));

    vm.dangGui = true;
    Api.upload('/api/hoso/tai-len', fd).then(function () {
      Toast.ok('Tải CV thành công');
      vm.file = null; vm.chon = {};
      document.getElementById('fileCv').value = '';
      vm.tai();
    }, Api.bao).finally(function () { vm.dangGui = false; });
  };
  vm.taiVe = function (h) { Api.taiVe('/api/hoso/' + h.id + '/tai-ve', h.tenFileGoc).catch(Api.bao); };
  vm.xoa = function (h) {
    if (!$window.confirm('Xóa CV "' + h.tenFileGoc + '"?')) return;
    Api.del('/api/hoso/' + h.id).then(function () { Toast.ok('Đã xóa CV'); vm.tai(); }, Api.bao);
  };
});