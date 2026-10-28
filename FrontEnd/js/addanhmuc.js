app.controller('AdDanhMucCtrl', function ($window, Api, Toast, KyNang) {
  var vm = this;
  vm.f = {}; vm.file = null; vm.tenKyNang = '';

  vm.taiMau = function () { Api.get('/api/mau-cv').then(function (ds) { vm.mau = ds; }, Api.bao); };
  vm.taiKyNang = function () { KyNang.lamMoi(); KyNang.tatCa().then(function (ds) { vm.kyNang = ds; }); };
  vm.taiMau(); vm.taiKyNang();

  vm.themMau = function () {
    if (!vm.file) { Toast.loi('Hãy chọn file mẫu CV'); return; }
    var fd = new FormData();
    fd.append('file', vm.file);
    fd.append('tenMau', vm.f.tenMau || '');
    fd.append('moTa', vm.f.moTa || '');
    Api.upload('/api/mau-cv', fd).then(function () {
      Toast.ok('Đã thêm mẫu CV');
      vm.f = {}; vm.file = null; document.getElementById('fileMau').value = ''; vm.taiMau();
    }, Api.bao);
  };
  vm.xoaMau = function (m) {
    if (!$window.confirm('Xóa mẫu "' + m.tenMau + '"?')) return;
    Api.del('/api/mau-cv/' + m.id).then(function () { Toast.ok('Đã xóa mẫu CV'); vm.taiMau(); }, Api.bao);
  };
  vm.themKyNang = function () {
    Api.post('/api/kynang', { tenKyNang: vm.tenKyNang }).then(function () {
      Toast.ok('Đã thêm kỹ năng'); vm.tenKyNang = ''; vm.taiKyNang();
    }, Api.bao);
  };
});