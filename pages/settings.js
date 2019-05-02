var $api = axios.create({
  baseURL: utils.getQueryString('apiUrl') + '/' + utils.getQueryString('pluginId') + '/pages/settings/',
  params: {
    siteId: utils.getQueryInt('siteId')
  },
  withCredentials: true
});

var data = {
  pageLoad: false,
  pageAlert: null,
  configInfo: null
};

var methods = {
  apiGet: function () {
    var $this = this;

    $api.get('').then(function (response) {
      var res = response.data;

      $this.configInfo = res.value;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  },

  apiSubmit: function () {
    var $this = this;

    utils.loading(true);
    $api.post('', $this.configInfo).then(function (response) {
      var res = response.data;

      swal2({
        toast: true,
        type: 'success',
        title: "设置保存成功",
        showConfirmButton: false,
        timer: 2000
      });
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validate().then(function (result) {
      if (result) {
        $this.apiSubmit();
      }
    });
  },

  btnNavClick: function (pageName) {
    location.href = utils.getPageUrl(pageName);
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    this.apiGet();
  }
});
