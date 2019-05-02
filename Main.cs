using System.Collections.Generic;
using SiteServer.Plugin;
using SS.Reward.Core;

namespace SS.Reward
{
    public class Main : PluginBase
    {
        public const string PluginId = "SS.Reward";

        public static RecordRepository RecordRepository;

        public override void Startup(IService service)
        {
            RecordRepository = new RecordRepository();

            service
                .AddDatabaseTable(RecordRepository.TableName, RecordRepository.TableColumns)
                .AddSiteMenu(siteId => new Menu
                {
                    Text = "文章打赏",
                    IconClass = "ion-social-yen",
                    Menus = new List<Menu>
                    {
                        new Menu
                        {
                            Text = "文章打赏记录",
                            Href = "pages/records.html"
                        },
                        new Menu
                        {
                            Text = "文章打赏设置",
                            Href = "pages/settings.html"
                        }
                    }
                })
                .AddStlElementParser(StlReward.ElementName, StlReward.Parse);

            //service.RestApiGet += Service_RestApiGet;
            //service.RestApiPost += Service_RestApiPost;
        }

        //private object Service_RestApiGet(object sender, RestApiEventArgs args)
        //{
        //    if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlReward.ApiQrCode)))
        //    {
        //        return StlReward.ApiQrCode(args.Request);
        //    }

        //    return new HttpResponseMessage(HttpStatusCode.NotFound);

        //}

        //private object Service_RestApiPost(object sender, RestApiEventArgs args)
        //{
        //    var request = args.Request;

        //    if (!string.IsNullOrEmpty(args.RouteResource) && !string.IsNullOrEmpty(args.RouteId))
        //    {
        //        if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlReward.ApiWeixinNotify)))
        //        {
        //            return StlReward.ApiWeixinNotify(request, args.RouteId);
        //        }
        //    }
        //    else if (!string.IsNullOrEmpty(args.RouteResource))
        //    {
        //        if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlReward.ApiPay)))
        //        {
        //            return StlReward.ApiPay(request);
        //        }
        //        if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlReward.ApiPaySuccess)))
        //        {
        //            return StlReward.ApiPaySuccess(request);
        //        }
        //        if (Utils.EqualsIgnoreCase(args.RouteResource, nameof(StlReward.ApiWeixinInterval)))
        //        {
        //            return StlReward.ApiWeixinInterval(request);
        //        }
        //    }

        //    return new HttpResponseMessage(HttpStatusCode.NotFound);
        //}
    }
}