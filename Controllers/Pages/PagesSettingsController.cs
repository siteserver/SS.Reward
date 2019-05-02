using System;
using System.Web.Http;
using SiteServer.Plugin;
using SS.Reward.Core;

namespace SS.Reward.Controllers.Pages
{
    [RoutePrefix("pages/settings")]
    public class PagesSettingsController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            var request = Context.AuthenticatedRequest;
            var siteId = request.GetQueryInt("siteId");

            if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Main.PluginId))
            {
                return Unauthorized();
            }

            return Ok(new
            {
                Value = Context.ConfigApi.GetConfig<ConfigInfo>(Main.PluginId, siteId)
            });
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = Context.AuthenticatedRequest;
                var siteId = request.GetQueryInt("siteId");
                if (!request.IsAdminLoggin || !request.AdminPermissions.HasSitePermissions(siteId, Main.PluginId))
                {
                    return Unauthorized();
                }

                var configInfo = request.GetPostObject<ConfigInfo>();

                Context.ConfigApi.SetConfig(Main.PluginId, siteId, configInfo);

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
