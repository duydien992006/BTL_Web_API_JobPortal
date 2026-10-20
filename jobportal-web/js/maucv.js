app.controller('TemplatesCtrl', function (Api) {
  var vm = this;
  Api.get('/api/mau-cv').then(function (ds) { vm.ds = ds; }, Api.bao);
  vm.taiVe = function (m) { Api.taiVe('/api/mau-cv/' + m.id + '/tai-ve', m.tenFileGoc).catch(Api.bao); };
});