using System;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Reward.Core;
using ThoughtWorks.QRCode.Codec;

namespace SS.Reward.Controllers
{
    [RoutePrefix("reward")]
    public class RewardController : ApiController
    {
        [HttpGet, Route(nameof(QrCode))]
        public HttpResponseMessage QrCode()
        {
            var request = Context.AuthenticatedRequest;

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

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(ms.GetBuffer());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            return response;
        }

        //public IHttpActionResult ApiGet(IRequest request)
        //{
        //    return new
        //    {
        //        request.IsUserLoggin
        //    };
        //}

        [HttpPost, Route(nameof(Status))]
        public IHttpActionResult Status()
        {
            var request = Context.AuthenticatedRequest;

            return Ok(new
            {
                Value = request.IsUserLoggin
            });
        }

        [HttpPost, Route(nameof(Pay))]
        public IHttpActionResult Pay()
        {
            var request = Context.AuthenticatedRequest;
            var repository = new RecordRepository();

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

            var paymentApi = new SS.Payment.Core.PaymentApi(siteId);

            var recordInfo = new RecordInfo
            {
                SiteId = siteId,
                ChannelId = channelId,
                ContentId = contentId,
                Message = message,
                Amount = amount,
                OrderNo = orderNo,
                IsPayed = false,
                AddDate = DateTime.Now
            };
            repository.Insert(recordInfo);

            var redirectUrl = string.Empty;
            var wxPayQrCodeUrl = string.Empty;
            var wxPayOrderNo = string.Empty;

            if (channel == "AliPay")
            {
                redirectUrl = paymentApi.ChargeByAliPay("文章打赏", amount, orderNo, successUrl);
            }
            if (channel == "WxPay")
            {
                var apiUrl = Context.Environment.ApiUrl;

                wxPayOrderNo = orderNo;
                var notifyUrl = UrlUtils.GetWxPayNotifyUrl(apiUrl, wxPayOrderNo, siteId);
                var url = HttpUtility.UrlEncode(paymentApi.ChargeByWxPay("文章打赏", amount, orderNo, notifyUrl));
                wxPayQrCodeUrl = UrlUtils.GetWxPayQrCodeUrl(apiUrl, url);
            }

            return Ok(new
            {
                Value = redirectUrl,
                WxPayQrCodeUrl = wxPayQrCodeUrl,
                WxPayOrderNo = wxPayOrderNo
            });
        }

        [HttpPost, Route(nameof(WxPayNotify))]
        public static HttpResponseMessage WxPayNotify()
        {
            var request = Context.AuthenticatedRequest;
            var repository = new RecordRepository();

            var siteId = request.GetQueryInt("siteId");
            var orderNo = request.GetQueryString("orderNo");

            var paymentApi = new SS.Payment.Core.PaymentApi(siteId);

            paymentApi.NotifyByWxPay(HttpContext.Current.Request, out var isPayed, out var responseXml);
            //var filePath = Path.Combine(Main.PhysicalApplicationPath, "log.txt");
            //File.WriteAllText(filePath, responseXml);
            if (isPayed)
            {
                repository.UpdateIsPayed(orderNo);
            }

            var response = new HttpResponseMessage {Content = new StringContent(responseXml)};
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        [HttpPost, Route(nameof(PaySuccess))]
        public IHttpActionResult PaySuccess()
        {
            var request = Context.AuthenticatedRequest;
            var repository = new RecordRepository();

            var orderNo = request.GetPostString("orderNo");

            repository.UpdateIsPayed(orderNo);

            return Ok(new
            {
                Value = true
            });
        }

        [HttpGet, Route(nameof(WxPayInterval))]
        public IHttpActionResult WxPayInterval()
        {
            var request = Context.AuthenticatedRequest;
            var repository = new RecordRepository();

            var orderNo = request.GetQueryString("orderNo");

            var isPayed = repository.IsPayed(orderNo);

            return Ok(new
            {
                Value = isPayed
            });
        }
    }
}
