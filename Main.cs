using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using SiteServer.Plugin;
using SS.Reward.Core;
using SS.Reward.Model;
using SS.Reward.Pages;
using SS.Reward.Parse;
using SS.Reward.Provider;

namespace SS.Reward
{
    public class Main : IPlugin
    {
        public static IDataApi DataApi { get; private set; }
        public static IConfigApi ConfigApi { get; private set; }
        public static IParseApi ParseApi { get; private set; }
        public static IFilesApi FilesApi { get; private set; }
        public static IAdminApi AdminApi { get; private set; }
        public static IContentApi ContentApi { get; private set; }

        public static DatabaseType DatabaseType { get; private set; }
        public static string ConnectionString { get; private set; }

        public static Dao Dao { get; private set; }
        public static RecordDao RecordDao { get; private set; }

        private static readonly Dictionary<int, ConfigInfo> ConfigInfoDict = new Dictionary<int, ConfigInfo>();

        public static ConfigInfo GetConfigInfo(int siteId)
        {
            if (!ConfigInfoDict.ContainsKey(siteId))
            {
                ConfigInfoDict[siteId] = ConfigApi.GetConfig<ConfigInfo>(siteId) ?? new ConfigInfo();
            }
            return ConfigInfoDict[siteId];
        }

        public void Startup(IContext context, IService service)
        {
            DataApi = context.DataApi;
            ConfigApi = context.ConfigApi;
            ParseApi = context.ParseApi;
            FilesApi = context.FilesApi;
            AdminApi = context.AdminApi;
            ContentApi = context.ContentApi;

            DatabaseType = context.Environment.DatabaseType;
            ConnectionString = context.Environment.ConnectionString;

            Dao = new Dao();
            RecordDao = new RecordDao();

            service
                .AddDatabaseTable(RecordDao.TableName, RecordDao.Columns)
                .AddSiteMenu(siteId => new Menu
                {
                    Text = "文章打赏",
                    IconClass = "ion-social-yen",
                    Menus = new List<Menu>
                    {
                        new Menu
                        {
                            Text = "文章打赏记录",
                            Href = $"{nameof(PageRecords)}.aspx"
                        },
                        new Menu
                        {
                            Text = "文章打赏设置",
                            Href = $"{nameof(PageSettings)}.aspx"
                        }
                    }
                })
                .AddStlElementParser(StlReward.ElementName, StlReward.Parse)
                .AddJsonPost((request, name) =>
                {
                    if (Utils.EqualsIgnoreCase(name, nameof(StlReward.ApiPay)))
                    {
                        return StlReward.ApiPay(request);
                    }
                    if (Utils.EqualsIgnoreCase(name, nameof(StlReward.ApiPaySuccess)))
                    {
                        return StlReward.ApiPaySuccess(request);
                    }
                    if (Utils.EqualsIgnoreCase(name, nameof(StlReward.ApiWeixinInterval)))
                    {
                        return StlReward.ApiWeixinInterval(request);
                    }

                    return null;
                })
                .AddHttpGet((request, name) =>
                {
                    if (Utils.EqualsIgnoreCase(name, nameof(StlReward.ApiQrCode)))
                    {
                        return StlReward.ApiQrCode(request);
                    }

                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                })
                .AddHttpPost((request, name, id) =>
                {
                    if (Utils.EqualsIgnoreCase(name, nameof(StlReward.ApiWeixinNotify)))
                    {
                        return StlReward.ApiWeixinNotify(request, id);
                    }

                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                });
        }
    }
}