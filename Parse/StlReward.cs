using System;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SiteServer.Plugin;
using SS.Payment.Core;
using SS.Reward.Model;
using ThoughtWorks.QRCode.Codec;

namespace SS.Reward.Parse
{
    public static class StlReward
    {
        public const string ElementName = "stl:reward";

        public const string AttributeWeixinName = "weixinName";

        public static object ApiPay(IRequest request)
        {
            var siteId = request.GetPostInt("siteId");
            var channelId = request.GetPostInt("channelId");
            var contentId = request.GetPostInt("contentId");
            var amount = request.GetPostDecimal("amount");
            var channel = request.GetPostString("channel");
            var message = request.GetPostString("message");
            var isMobile = request.GetPostBool("isMobile");
            var successUrl = request.GetPostString("successUrl");
            var orderNo = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
            successUrl += "&orderNo=" + orderNo;

            var paymentApi = new PaymentApi(siteId);

            var recordInfo = new RecordInfo
            {
                PublishmentSystemId = siteId,
                ChannelId = channelId,
                ContentId = contentId,
                Message = message,
                Amount = amount,
                OrderNo = orderNo,
                IsPaied = false,
                AddDate = DateTime.Now
            };
            Main.RecordDao.Insert(recordInfo);

            if (channel == "alipay")
            {
                return isMobile
                    ? paymentApi.ChargeByAlipayMobi("文章打赏", amount, orderNo, successUrl)
                    : paymentApi.ChargeByAlipayPc("文章打赏", amount, orderNo, successUrl);
            }
            if (channel == "weixin")
            {
                var notifyUrl = Main.Instance.PluginApi.GetPluginApiUrl(nameof(ApiWeixinNotify), orderNo);
                var url = HttpUtility.UrlEncode(paymentApi.ChargeByWeixin("文章打赏", amount, orderNo, notifyUrl));
                var qrCodeUrl =
                    $"{Main.Instance.PluginApi.GetPluginApiUrl(nameof(ApiQrCode))}?qrcode={url}";
                return new
                {
                    qrCodeUrl,
                    orderNo
                };
            }
            if (channel == "jdpay")
            {
                return paymentApi.ChargeByJdpay("文章打赏", amount, orderNo, successUrl);
            }

            return null;
        }

        public static HttpResponseMessage ApiQrCode(IRequest request)
        {
            var response = new HttpResponseMessage();

            var qrcode = request.GetQueryString("qrcode");
            var qrCodeEncoder = new QRCodeEncoder
            {
                QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
                QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M,
                QRCodeVersion = 0,
                QRCodeScale = 4
            };

            //将字符串生成二维码图片
            var image = qrCodeEncoder.Encode(qrcode, Encoding.Default);

            //保存为PNG到内存流  
            var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);

            response.Content = new ByteArrayContent(ms.GetBuffer());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        public static HttpResponseMessage ApiWeixinNotify(IRequest request, string orderNo)
        {
            var siteId = request.GetPostInt("siteId");
            var paymentApi = new PaymentApi(siteId);

            var response = new HttpResponseMessage();

            bool isPaied;
            string responseXml;
            paymentApi.NotifyByWeixin(request.HttpRequest, out isPaied, out responseXml);
            if (isPaied)
            {
                Main.RecordDao.UpdateIsPaied(orderNo);
            }

            response.Content = new StringContent(responseXml);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        public static object ApiPaySuccess(IRequest request)
        {
            var orderNo = request.GetPostString("orderNo");
            
            Main.RecordDao.UpdateIsPaied(orderNo);

            return null;
        }

        public static object ApiWeixinInterval(IRequest request)
        {
            var orderNo = request.GetPostString("orderNo");

            var isPaied = Main.RecordDao.IsPaied(orderNo);

            return new
            {
                isPaied
            };
        }

        public static string Parse(IParseContext context)
        {
            var config = Main.Instance.GetConfigInfo(context.SiteId);
            if (!config.IsEnabled || context.ChannelId == 0 || context.ContentId == 0) return string.Empty;

            var weixinName = string.Empty;

            foreach (var attriName in context.StlAttributes.Keys)
            {
                var value = context.StlAttributes[attriName];
                if (Utils.EqualsIgnoreCase(attriName, AttributeWeixinName))
                {
                    weixinName = Main.Instance.ParseApi.ParseAttributeValue(value, context);
                }
            }

            string template;
            if (!string.IsNullOrEmpty(context.StlInnerHtml))
            {
                template = Main.Instance.ParseApi.Parse(context.StlInnerHtml, context);
            }
            else
            {
                if (!string.IsNullOrEmpty(weixinName))
                {
                    weixinName = $@"<p style=""text-align: center"">{weixinName}</p>";
                }
                template = $@"
<div class=""detail_zan"">
    {config.Description}
    <div @click=""open"" class=""detail_btn"">
        赞赏支持
    </div>
</div>
<div class=""mask1_bg mask1_bg_cut"" v-show=""isReward || isWxQrCode || isRewardSuccess"" @click=""isReward = isWxQrCode = isRewardSuccess = false""></div>
<div class=""detail_alert detail_alert_cut"" v-show=""isReward"">
  <div class=""close"" @click=""isReward = isWxQrCode = isRewardSuccess = false""></div>
  <div class=""alert_input"">
    赞赏金额（元）:<input type=""text"" v-model.number=""amount"" />
  </div>
  <div class=""alert_textarea"">
    <textarea v-model=""message"" placeholder=""留言""></textarea>
  </div>
  <div class=""pay_list"">
    <p>支付方式</p>
    <ul>
        <li v-show=""(isAlipayPc && !isMobile) || (isAlipayMobi && isMobile)"" :class=""{{ pay_cut: channel === 'alipay' }}"" @click=""channel = 'alipay'"" class=""channel_alipay""><b></b></li>
        <li v-show=""isWeixin"" :class=""{{ pay_cut: channel === 'weixin' }}"" @click=""channel = 'weixin'"" class=""channel_weixin""><b></b></li>
        <li v-show=""isJdpay"" :class=""{{ pay_cut: channel === 'jdpay' }}"" @click=""channel = 'jdpay'"" class=""channel_jdpay""><b></b></li>
    </ul>
    <div class=""mess_text""></div>
    <a href=""javascript:;"" @click=""pay"" class=""pay_go"">立即支付</a>
  </div>
</div>
<div class=""detail_alert detail_alert_cut"" v-show=""isWxQrCode"">
  <div class=""close"" @click=""isReward = isWxQrCode = isRewardSuccess = false""></div>
  <div class=""pay_list"">
    <p style=""text-align: center"">打开手机微信，扫一扫下面的二维码，即可完成支付</p>
    {weixinName}
    <p style=""margin-left: 195px;margin-bottom: 80px;""><img :src=""qrCodeUrl"" style=""width: 200px;height: 200px;""></p>
  </div>
</div>
<div class=""detail_alert detail_alert_cut"" v-show=""isRewardSuccess"">
  <div class=""close"" @click=""isReward = isWxQrCode = isRewardSuccess = false""></div>
  <div class=""pay_list"">
    <p style=""text-align: center"">支付成功，谢谢支持</p>
    <div class=""mess_text""></div>
    <a href=""javascript:;"" @click=""isReward = isWxQrCode = isRewardSuccess = false"" class=""pay_go"">关闭</a>
  </div>
</div>
";
            }

            var elementId = "el-" + Guid.NewGuid();
            var vueId = "v" + Guid.NewGuid().ToString().Replace("-", string.Empty);
            var styleUrl = Main.Instance.PluginApi.GetPluginUrl("assets/css/style.css");
            var jqueryUrl = Main.Instance.PluginApi.GetPluginUrl("assets/js/jquery.min.js");
            var vueUrl = Main.Instance.PluginApi.GetPluginUrl("assets/js/vue.min.js");
            var deviceUrl = Main.Instance.PluginApi.GetPluginUrl("assets/js/device.min.js");
            var apiPayUrl = Main.Instance.PluginApi.GetPluginApiUrl(nameof(ApiPay));
            var apiPaySuccessUrl = Main.Instance.PluginApi.GetPluginApiUrl(nameof(ApiPaySuccess));
            var successUrl = Main.Instance.ParseApi.GetCurrentUrl(context) + "?isRewardSuccess=" + true;
            var apiWeixinIntervalUrl = Main.Instance.PluginApi.GetPluginApiUrl(nameof(ApiWeixinInterval));

            var paymentApi = new PaymentApi(context.SiteId);

            return $@"
<link rel=""stylesheet"" type=""text/css"" href=""{styleUrl}"" />
<script type=""text/javascript"" src=""{jqueryUrl}""></script>
<script type=""text/javascript"" src=""{vueUrl}""></script>
<script type=""text/javascript"" src=""{deviceUrl}""></script>
<div id=""{elementId}"">
    {template}
</div>
<script type=""text/javascript"">
    var match = location.search.match(new RegExp(""[\?\&]isRewardSuccess=([^\&]+)"", ""i""));
    var isRewardSuccess = (!match || match.length < 1) ? false : true;
    var {vueId} = new Vue({{
        el: '#{elementId}',
        data: {{
            amount: {config.DefaultAmount:N2},
            message: '',
            isAlipayPc: {paymentApi.IsAlipayPc.ToString().ToLower()},
            isAlipayMobi: {paymentApi.IsAlipayMobi.ToString().ToLower()},
            isWeixin: {paymentApi.IsWeixin.ToString().ToLower()},
            isJdpay: {paymentApi.IsJdpay.ToString().ToLower()},
            isMobile: device.mobile(),
            channel: 'alipay',
            isReward: false,
            isWxQrCode: false,
            isRewardSuccess: isRewardSuccess,
            qrCodeUrl: ''
        }},
        methods: {{
            open: function () {{
                this.isReward = true;
            }},
            weixinInterval: function(orderNo) {{
                var $this = this;
                var interval = setInterval(function(){{
                    $.ajax({{
                        url : ""{apiWeixinIntervalUrl}"",
                        type: ""POST"",
                        data: JSON.stringify({{orderNo: orderNo}}),
                        contentType: ""application/json; charset=utf-8"",
                        dataType: ""json"",
                        success: function(data)
                        {{
                            if (data.isPaied) {{
                                clearInterval(interval);
                                $this.isReward = $this.isWxQrCode = false;
                                $this.isRewardSuccess = true;
                            }}
                        }},
                        error: function (err)
                        {{
                            var err = JSON.parse(err.responseText);
                            console.log(err.message);
                        }}
                    }});
                }}, 3000);
            }},
            pay: function () {{
                if (this.amount == 0) return;
                var $this = this;
                var data = {{
                    siteId: '{context.SiteId}',
                    channelId: {context.ChannelId},
                    contentId: {context.ContentId},
                    amount: this.amount,
                    channel: this.channel,
                    message: this.message,
                    isMobile: this.isMobile,
                    successUrl: ""{successUrl}""
                }};
                $.ajax({{
                    url : ""{apiPayUrl}"",
                    type: ""POST"",
                    data: JSON.stringify(data),
                    contentType: ""application/json; charset=utf-8"",
                    dataType: ""json"",
                    success: function(charge)
                    {{
                        if ($this.channel === 'weixin') {{
                            $this.isReward = false;
                            $this.isWxQrCode = true;
                            $this.qrCodeUrl = charge.qrCodeUrl;
                            $this.weixinInterval(charge.orderNo);
                        }} else {{
                            document.write(charge);
                        }}
                    }},
                    error: function (err)
                    {{
                        var err = JSON.parse(err.responseText);
                        console.log(err.message);
                    }}
                }});
            }}
        }}
    }});
    
    match = location.search.match(new RegExp(""[\?\&]orderNo=([^\&]+)"", ""i""));
    var orderNo = (!match || match.length < 1) ? '' : decodeURIComponent(match[1]);
    if (isRewardSuccess) {{
        $(document).ready(function(){{
            $.ajax({{
                url : ""{apiPaySuccessUrl}"",
                type: ""POST"",
                data: JSON.stringify({{
                    orderNo: orderNo
                }}),
                contentType: ""application/json; charset=utf-8"",
                dataType: ""json"",
                success: function(data)
                {{
                    console.log(data);
                }},
                error: function (err)
                {{
                    var err = JSON.parse(err.responseText);
                    console.log(err.message);
                }}
            }});
        }});
    }}
</script>
";
        }
    }
}
