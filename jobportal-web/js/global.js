// ============ js/global.js ============
// Khai báo ứng dụng + mọi thứ dùng chung cho các trang.
// Các file js của từng trang dùng biến `app` khai báo ở đây.
var app = angular.module('jobportal', []);

// Địa chỉ Gateway (Backend). Đổi ở đây nếu Gateway chạy cổng khác
app.constant('API_URL', 'http://localhost:7000');

// Tham số của trang hiện tại (vd: /viec-lam/5 -> { id: '5' }). Bộ định tuyến bên dưới sẽ điền vào đây
app.value('$routeParams', {});

app.config(function ($locationProvider, $httpProvider, $qProvider) {
  $locationProvider.hashPrefix('');                      // đường dẫn dạng  #/viec-lam
  if ($qProvider.errorOnUnhandledRejections) $qProvider.errorOnUnhandledRejections(false);
  $httpProvider.interceptors.push('AuthInterceptor');
});

// =====================================================================
// 1. DANH SÁCH TRANG: đường dẫn -> file giao diện + controller
//    dangNhap: true  = cần đăng nhập      vaiTro: [...] = chỉ các vai trò này
// =====================================================================
app.constant('ROUTES', (function () {
  var UV = ['UngVien'], NTD = ['NhaTuyenDung'], AD = ['QuanTriVien'];
  return [
    { path: '/', view: 'home', ctrl: 'HomeCtrl' },
    { path: '/viec-lam', view: 'jobs', ctrl: 'JobsCtrl' },
    { path: '/viec-lam/:id', view: 'job-detail', ctrl: 'JobDetailCtrl' },
    { path: '/cong-ty', view: 'companies', ctrl: 'CompaniesCtrl' },
    { path: '/mau-cv', view: 'templates', ctrl: 'TemplatesCtrl' },
    { path: '/dang-nhap', view: 'login', ctrl: 'LoginCtrl' },
    { path: '/dang-ky', view: 'register', ctrl: 'RegisterCtrl' },
    { path: '/quen-mat-khau', view: 'forgot', ctrl: 'ForgotCtrl' },
    { path: '/dat-lai-mat-khau', view: 'forgot', ctrl: 'ForgotCtrl' },
    { path: '/thong-bao', view: 'notifications', ctrl: 'ThongBaoCtrl', dangNhap: true },
    { path: '/tai-khoan', view: 'account', ctrl: 'TaiKhoanCtrl', dangNhap: true },
    { path: '/lich-phong-van', view: 'interviews', ctrl: 'LichCtrl', vaiTro: ['UngVien', 'NhaTuyenDung'] },
    { path: '/ung-vien/ho-so', view: 'profile-cv', ctrl: 'HoSoCtrl', vaiTro: UV },
    { path: '/ung-vien/don-ung-tuyen', view: 'applications', ctrl: 'DonCtrl', vaiTro: UV },
    { path: '/nha-tuyen-dung/dashboard', view: 'emp-dashboard', ctrl: 'NtDashboardCtrl', vaiTro: NTD },
    { path: '/nha-tuyen-dung/cong-ty', view: 'emp-company', ctrl: 'NtCongTyCtrl', vaiTro: NTD },
    { path: '/nha-tuyen-dung/tin', view: 'emp-jobs', ctrl: 'NtTinCtrl', vaiTro: NTD },
    { path: '/nha-tuyen-dung/tin/moi', view: 'emp-job-form', ctrl: 'NtTinFormCtrl', vaiTro: NTD },
    { path: '/nha-tuyen-dung/tin/:id/sua', view: 'emp-job-form', ctrl: 'NtTinFormCtrl', vaiTro: NTD },
    { path: '/nha-tuyen-dung/tin/:id/ung-vien', view: 'emp-applicants', ctrl: 'NtUngVienCtrl', vaiTro: NTD },
    { path: '/admin/dashboard', view: 'adm-dashboard', ctrl: 'AdDashboardCtrl', vaiTro: AD },
    { path: '/admin/duyet', view: 'adm-review', ctrl: 'AdDuyetCtrl', vaiTro: AD },
    { path: '/admin/nguoi-dung', view: 'adm-users', ctrl: 'AdNguoiDungCtrl', vaiTro: AD },
    { path: '/admin/danh-muc', view: 'adm-catalog', ctrl: 'AdDanhMucCtrl', vaiTro: AD }
  ];
})());

// =====================================================================
// 2. BỘ ĐỊNH TUYẾN NHỎ (thay cho ngRoute): <main app-view></main>
//    Khi địa chỉ sau dấu # đổi -> tìm trang tương ứng -> tải views/xxx.html -> chạy controller
// =====================================================================
app.directive('appView', function ($location, $templateRequest, $compile, $controller, $rootScope, $routeParams, ROUTES, Auth, Toast) {
  return {
    restrict: 'A',
    link: function (scope, el) {
      var viewScope = null, lanTai = 0;

      function timRoute(path) {
        for (var i = 0; i < ROUTES.length; i++) {
          var ten = [];
          var mau = '^' + ROUTES[i].path.replace(/:(\w+)/g, function (m, t) { ten.push(t); return '([^/]+)'; }) + '/?$';
          var kq = new RegExp(mau).exec(path);
          if (kq) {
            var tham = {};
            for (var j = 0; j < ten.length; j++) tham[ten[j]] = decodeURIComponent(kq[j + 1]);
            return { route: ROUTES[i], tham: tham };
          }
        }
        return null;
      }

      function napTrang() {
        var tim = timRoute($location.path());
        if (!tim) { $location.url('/').replace(); return; }
        var r = tim.route;

        // Chặn vào trang cần đăng nhập / sai vai trò (bảo mật thật nằm ở Backend)
        if ((r.dangNhap || r.vaiTro) && !Auth.daDangNhap()) {
          $rootScope.quayLai = $location.url();              // đăng nhập xong quay lại đúng trang này
          Toast.loi('Vui lòng đăng nhập để tiếp tục');
          $location.url('/dang-nhap').replace();
          return;
        }
        if (r.vaiTro && !Auth.laVaiTro(r.vaiTro)) {
          Toast.loi('Bạn không có quyền truy cập trang này');
          $location.url('/').replace();
          return;
        }

        // Điền tham số trang: phần trên đường dẫn (:id) + phần sau dấu ? (tuKhoa=...)
        for (var k in $routeParams) delete $routeParams[k];
        angular.extend($routeParams, $location.search(), tim.tham);

        var luot = ++lanTai;
        $templateRequest('views/' + r.view + '.html').then(function (html) {
          if (luot !== lanTai) return;                       // người dùng đã chuyển sang trang khác
          if (viewScope) viewScope.$destroy();
          viewScope = scope.$new();
          el.html(html);
          viewScope.vm = $controller(r.ctrl, { $scope: viewScope });   // trong giao diện dùng vm.xxx
          $compile(el.contents())(viewScope);
          window.scrollTo(0, 0);
        }, function () {
          Toast.loi('Không tải được giao diện trang: views/' + r.view + '.html');
        });
      }

      $rootScope.$on('$locationChangeSuccess', napTrang);
    }
  };
});

// =====================================================================
// 3. DỊCH VỤ DÙNG CHUNG
// =====================================================================

// ----- Thông báo nổi -----
app.factory('Toast', function ($rootScope, $timeout) {
  $rootScope.toasts = [];
  function hien(loai, noiDung) {
    var t = { loai: loai, noiDung: noiDung };
    $rootScope.toasts.push(t);
    $timeout(function () {
      var i = $rootScope.toasts.indexOf(t);
      if (i >= 0) $rootScope.toasts.splice(i, 1);
    }, 4500);
  }
  return { ok: function (nd) { hien('ok', nd); }, loi: function (nd) { hien('loi', nd); } };
});

// ----- Gọi API: mọi request đều đi qua đây -----
app.factory('Api', function ($http, API_URL, Toast) {
  function chay(cfg) { return $http(cfg).then(function (r) { return r.data; }); }
  function boNull(d) { return d === null ? undefined : d; }

  var svc = {
    get:  function (url, params) { return chay({ method: 'GET', url: API_URL + url, params: params }); },
    post: function (url, data, params) { return chay({ method: 'POST', url: API_URL + url, data: boNull(data), params: params }); },
    put:  function (url, data, params) { return chay({ method: 'PUT', url: API_URL + url, data: boNull(data), params: params }); },
    del:  function (url, params) { return chay({ method: 'DELETE', url: API_URL + url, params: params }); },

    // Tải file lên (multipart/form-data)
    upload: function (url, formData) {
      return $http.post(API_URL + url, formData, {
        transformRequest: angular.identity,
        headers: { 'Content-Type': undefined }
      }).then(function (r) { return r.data; });
    },

    // Tải file về
    taiVe: function (url, tenFile) {
      return $http.get(API_URL + url, { responseType: 'blob' }).then(function (r) {
        var link = document.createElement('a');
        var u = window.URL.createObjectURL(r.data);
        link.href = u;
        link.download = tenFile || 'tai-ve';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(u);
      });
    },

    // Biến mọi kiểu lỗi của Backend thành 1 câu tiếng Việt
    loi: function (e) {
      if (!e) return 'Có lỗi xảy ra';
      if (e.status === -1 || e.status === 0)
        return 'Không kết nối được máy chủ. Hãy kiểm tra Gateway (' + API_URL + ') đã chạy chưa.';
      var d = e.data;
      if (d && d.errors) {
        var ds = [];
        angular.forEach(d.errors, function (arr) { ds = ds.concat(arr); });
        if (ds.length) return ds.join(' • ');
      }
      if (d && d.detail) return d.detail + (e.status >= 500 && d.traceId ? ' [mã lỗi: ' + d.traceId + ']' : '');
      if (e.status === 401) return 'Bạn cần đăng nhập';
      if (e.status === 403) return 'Bạn không có quyền thực hiện thao tác này';
      if (e.status === 404) return 'Không tìm thấy dữ liệu';
      return 'Có lỗi xảy ra (' + e.status + ')';
    }
  };
  svc.bao = function (e) { Toast.loi(svc.loi(e)); };       // dùng nhanh:  .then(ok, Api.bao)
  return svc;
});

// ----- Đăng nhập, lưu JWT, phân quyền -----
app.factory('Auth', function ($window, $rootScope, Api) {
  var ls = $window.localStorage;
  var token = ls.getItem('jp_token'), user = null, het = 0;

  function hanToken(t) {
    try {
      var p = JSON.parse(atob(t.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')));
      return (p.exp || 0) * 1000;
    } catch (e) { return 0; }
  }
  try { user = JSON.parse(ls.getItem('jp_user')); } catch (e) { user = null; }
  if (token) het = hanToken(token);

  var svc = {
    token: function () { return token; },
    nguoiDung: function () { return user; },
    daDangNhap: function () { return !!token && het > Date.now(); },
    laVaiTro: function (ds) { return !!user && ds.indexOf(user.vaiTro) >= 0; },
    laUV: function () { return svc.laVaiTro(['UngVien']); },
    laNTD: function () { return svc.laVaiTro(['NhaTuyenDung']); },
    laAdmin: function () { return svc.laVaiTro(['QuanTriVien']); },
    trangChu: function () {
      if (svc.laNTD()) return '/nha-tuyen-dung/dashboard';
      if (svc.laAdmin()) return '/admin/dashboard';
      return '/';
    },
    dangNhap: function (email, matKhau) {
      return Api.post('/api/xacthuc/dang-nhap', { email: email, matKhau: matKhau }).then(function (kq) {
        token = kq.duLieu.token;
        het = hanToken(token);
        user = { hoTen: kq.duLieu.hoTen, vaiTro: kq.duLieu.vaiTro };
        ls.setItem('jp_token', token);
        ls.setItem('jp_user', JSON.stringify(user));
        $rootScope.$broadcast('auth:doi');
        return user;
      });
    },
    dangXuat: function () {
      token = null; user = null; het = 0;
      ls.removeItem('jp_token'); ls.removeItem('jp_user');
      $rootScope.$broadcast('auth:doi');
    },
    capNhatTen: function (hoTen) {
      if (user) { user.hoTen = hoTen; ls.setItem('jp_user', JSON.stringify(user)); }
    }
  };
  return svc;
});

// ----- Tự gắn JWT vào mọi request, tự đăng xuất khi gặp 401 -----
app.factory('AuthInterceptor', function ($q, $window, $injector, API_URL) {
  return {
    request: function (config) {
      var t = $window.localStorage.getItem('jp_token');
      if (t && config.url.indexOf(API_URL) === 0) config.headers.Authorization = 'Bearer ' + t;
      return config;
    },
    responseError: function (r) {
      var daCoToken = !!$window.localStorage.getItem('jp_token');
      if (r.status === 401 && daCoToken && r.config && r.config.url.indexOf('/api/xacthuc/') === -1) {
        $injector.get('Auth').dangXuat();
        $injector.get('Toast').loi('Phiên đăng nhập đã hết hạn hoặc tài khoản không còn hiệu lực, vui lòng đăng nhập lại');
        $injector.get('$location').url('/dang-nhap');
      }
      return $q.reject(r);
    }
  };
});

// ----- Danh sách kỹ năng (tải 1 lần, dùng lại) -----
app.factory('KyNang', function (Api) {
  var cache = null;
  return {
    tatCa: function () {
      if (!cache) {
        cache = Api.get('/api/kynang');
        cache.catch(function () { cache = null; });
      }
      return cache;
    },
    lamMoi: function () { cache = null; }
  };
});

// ----- Tiện ích -----
app.factory('Helper', function () {
  return {
    // Ngày chọn từ <input type="date"> -> 00:00 UTC cùng ngày, tránh bị lùi 1 ngày do múi giờ
    ngayUtc: function (d) {
      return d ? new Date(Date.UTC(d.getFullYear(), d.getMonth(), d.getDate())) : null;
    },
    max: function (ds, truong) {
      var m = 1;
      angular.forEach(ds || [], function (x) { if (x[truong] > m) m = x[truong]; });
      return m;
    }
  };
});

// =====================================================================
// 4. BỘ LỌC + THÀNH PHẦN GIAO DIỆN DÙNG CHUNG
// =====================================================================
app.constant('TRANG_THAI', {
  DaNop: ['Đã nộp', 'blue'], DangXemXet: ['Đang xem xét', 'orange'], PhongVan: ['Phỏng vấn', 'purple'],
  DeNghi: ['Đã nhận offer', 'purple'], TrungTuyen: ['Trúng tuyển', 'green'], UngVienTuChoi: ['Đã từ chối offer', 'gray'],
  TuChoi: ['Bị từ chối', 'red'], DaRut: ['Đã rút đơn', 'gray'],
  ChoDuyet: ['Chờ duyệt', 'orange'], DaDuyet: ['Đang tuyển', 'green'], DaDong: ['Đã đóng', 'gray'],
  ChoKetQua: ['Chờ kết quả', 'orange'], Dat: ['Đạt', 'green'], KhongDat: ['Không đạt', 'red'], DaHuy: ['Đã hủy', 'gray'],
  DaGui: ['Chờ phản hồi', 'purple'], DaChapNhan: ['Đã chấp nhận', 'green'], DaTuChoi: ['Đã từ chối', 'red'],
  UngVien: ['Ứng viên', 'blue'], NhaTuyenDung: ['Nhà tuyển dụng', 'green'], QuanTriVien: ['Quản trị viên', 'purple']
});

app.filter('nhan', function (TRANG_THAI) {
  return function (k) { return (TRANG_THAI[k] && TRANG_THAI[k][0]) || k; };
});
app.filter('mau', function (TRANG_THAI) {
  return function (k) { return (TRANG_THAI[k] && TRANG_THAI[k][1]) || 'gray'; };
});
app.filter('chuDau', function () {
  return function (s) { return s ? String(s).trim().charAt(0).toUpperCase() : '?'; };
});
app.filter('luong', function () {
  function trieu(n) { return (n / 1000000).toLocaleString('vi-VN', { maximumFractionDigits: 1 }); }
  return function (tu, den) {
    if (!tu && !den) return 'Thỏa thuận';
    if (tu && den) return trieu(tu) + ' - ' + trieu(den) + ' triệu';
    return 'Từ ' + trieu(tu || den) + ' triệu';
  };
});

// <pager trang="..." tong="..." on-chon="vm.tai(t)">
app.directive('pager', function () {
  return {
    restrict: 'E',
    scope: { trang: '=', tong: '=', onChon: '&' },
    template:
      '<div class="pager" ng-if="tong > 1">' +
      '<button ng-disabled="trang <= 1" ng-click="chon(trang - 1)">‹</button>' +
      '<button ng-repeat="t in ds()" ng-class="{on: t === trang}" ng-click="chon(t)">{{t}}</button>' +
      '<button ng-disabled="trang >= tong" ng-click="chon(trang + 1)">›</button></div>',
    link: function (scope) {
      scope.ds = function () {
        var den = Math.min(scope.tong, Math.max(1, scope.trang - 2) + 4);
        var tu = Math.max(1, den - 4), a = [];
        for (var i = tu; i <= den; i++) a.push(i);
        return a;
      };
      scope.chon = function (t) {
        if (t < 1 || t > scope.tong || t === scope.trang) return;
        scope.onChon({ t: t });
      };
    }
  };
});

// <input type="file" file-model="vm.file">
app.directive('fileModel', function ($parse) {
  return {
    restrict: 'A',
    link: function (scope, el, attrs) {
      var gan = $parse(attrs.fileModel).assign;
      el.on('change', function () {
        scope.$apply(function () { gan(scope, el[0].files[0] || null); });
      });
    }
  };
});

// <job-card job="j">
app.directive('jobCard', function () {
  return {
    restrict: 'E',
    scope: { job: '=' },
    templateUrl: 'views/job-card.html',
    link: function (scope) {
      scope.conNgay = function (d) {
        var n = Math.ceil((new Date(d) - new Date()) / 86400000);
        return n < 0 ? 0 : n;
      };
    }
  };
});

// ----- Thanh trên cùng (dùng ở index.html) -----
app.controller('HeaderCtrl', function ($scope, $location, $interval, Auth, Api) {
  var h = this;
  h.auth = Auth;
  h.chuaDoc = 0;
  h.active = function (p) { return $location.path().indexOf(p) === 0; };
  h.dangXuat = function () { Auth.dangXuat(); $location.url('/dang-nhap'); };

  function taiThongBao() {
    if (!Auth.daDangNhap()) { h.chuaDoc = 0; return; }
    Api.get('/api/thongbao', { trang: 1, kichThuoc: 1 }).then(function (kq) { h.chuaDoc = kq.soChuaDoc; }, function () {});
  }
  taiThongBao();
  $interval(taiThongBao, 60000);
  $scope.$on('auth:doi', taiThongBao);
  $scope.$on('thongbao:doi', taiThongBao);
});