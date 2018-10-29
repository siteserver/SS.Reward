using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Reward.Controls;
using SS.Reward.Core;
using SS.Reward.Model;
using SS.Reward.Provider;

namespace SS.Reward.Pages
{
	public class PageRecords : Page
	{
	    public Literal LtlMessage;

        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnDelete;

        private int _siteId;

        public static string GetRedirectUrl(int siteId)
        {
            return $"{nameof(PageRecords)}.aspx?siteId={siteId}";
        }

		public void Page_Load(object sender, EventArgs e)
		{
		    var request = SiteServer.Plugin.Context.GetCurrentRequest();
            _siteId = request.GetQueryInt("siteId");

            if (!request.AdminPermissions.HasSitePermissions(_siteId, Main.PluginId))
            {
                Response.Write("<h1>未授权访问</h1>");
                Response.End();
                return;
            }

            if (!string.IsNullOrEmpty(Request.QueryString["delete"]) &&
                !string.IsNullOrEmpty(Request.QueryString["idCollection"]))
            {
                var array = Request.QueryString["idCollection"].Split(',');
                var list = array.Select(s => Convert.ToInt32(s)).ToList();
                RecordDao.Delete(list);
                LtlMessage.Text = Utils.GetMessageHtml("删除成功！", true);
            }

            SpContents.ControlToPaginate = RptContents;
            SpContents.ItemsPerPage = 20;
            SpContents.SelectCommand = RecordDao.GetSelectString(_siteId);
            SpContents.SortField = nameof(RecordInfo.Id);
            SpContents.SortMode = "DESC";
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            if (IsPostBack) return;

            SpContents.DataBind();

            BtnDelete.Attributes.Add("onclick", Utils.ReplaceNewline($@"
var ids = [];
$(""input[name='idCollection']:checked"").each(function () {{
    ids.push($(this).val());}}
);
if (ids.length > 0){{
    {Utils.SwalWarning("删除记录", "此操作将删除所选记录，确定吗？", "取 消", "删 除",
                $"location.href='{GetRedirectUrl(_siteId)}&delete={true}&idCollection=' + ids.join(',')")}
}} else {{
    {Utils.SwalError("请选择需要删除的记录！", string.Empty)}
}}
;return false;", string.Empty));

        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var channelId = Utils.EvalInt(e.Item.DataItem, nameof(RecordInfo.ChannelId));
            var contentId = Utils.EvalInt(e.Item.DataItem, nameof(RecordInfo.ContentId));
            var message = Utils.EvalString(e.Item.DataItem, nameof(RecordInfo.Message));
            var amount = Utils.EvalDecimal(e.Item.DataItem, nameof(RecordInfo.Amount));
            var isPaied = Utils.EvalBool(e.Item.DataItem, nameof(RecordInfo.IsPaied));
            var addDate = Utils.EvalDateTime(e.Item.DataItem, nameof(RecordInfo.AddDate));

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlMessage = (Literal)e.Item.FindControl("ltlMessage");
            var ltlAmount = (Literal)e.Item.FindControl("ltlAmount");
            var ltlStatus = (Literal)e.Item.FindControl("ltlStatus");
            var ltlAddDate = (Literal)e.Item.FindControl("ltlAddDate");

            ltlTitle.Text = $@"<a href=""{SiteServer.Plugin.Context.ContentApi.GetContentUrl(_siteId, channelId, contentId)}"" target=""_blank"">{SiteServer.Plugin.Context.ContentApi.GetContentValue(_siteId, channelId, contentId, "Title")}</a>";
            ltlMessage.Text = message;
            ltlAmount.Text = amount.ToString("N2");
            ltlStatus.Text = isPaied ? "已支付" : "未支付";
            ltlAddDate.Text = addDate.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
