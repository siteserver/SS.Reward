using SiteServer.Plugin;
using SS.Reward.Controllers;

namespace SS.Reward.Core
{
    public static class UrlUtils
    {
        public static string GetStatusUrl(string apiUrl)
        {
            return $"{apiUrl}/{Main.PluginId}/reward/{nameof(RewardController.Status)}";
        }

        public static string GetPayUrl(string apiUrl)
        {
            return $"{apiUrl}/{Main.PluginId}/reward/{nameof(RewardController.Pay)}";
        }

        public static string GetPaySuccessUrl(string apiUrl)
        {
            return $"{apiUrl}/{Main.PluginId}/reward/{nameof(RewardController.PaySuccess)}";
        }

        public static string GetSuccessUrl(IParseContext context)
        {
            return Context.ParseApi.GetCurrentUrl(context) + "?isPaymentSuccess=" + true;
        }

        public static string GetWxPayNotifyUrl(string apiUrl, string orderNo, int siteId)
        {
            return $"{apiUrl}/{Main.PluginId}/reward/{nameof(RewardController.WxPayNotify)}?orderNo={orderNo}&siteId={siteId}";
        }

        public static string GetWxPayQrCodeUrl(string apiUrl, string url)
        {
            return $"{apiUrl}/{Main.PluginId}/reward/{nameof(RewardController.QrCode)}?qrcode={url}";
        }

        public static string GetWxPayIntervalUrl(string apiUrl)
        {
            return $"{apiUrl}/{Main.PluginId}/payment/{nameof(RewardController.WxPayInterval)}";
        }

        public static string GetAssetsUrl(string relatedUrl)
        {
            return $"{Context.PluginApi.GetPluginUrl(Main.PluginId)}/assets/payment/{relatedUrl}";
        }
    }
}
