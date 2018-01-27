using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SS.Reward.Core;
using SS.Reward.Model;

namespace SS.Reward.Pages
{
    public class PageSettings : Page
    {
        public Literal LtlMessage;
        public DropDownList DdlIsEnabled;
        public PlaceHolder PhEnabled;
        public TextBox TbDefaultAmount;
        public TextBox TbDescription;

        private int _siteId;
        private ConfigInfo _configInfo;

        public void Page_Load(object sender, EventArgs e)
        {
            _siteId = Convert.ToInt32(Request.QueryString["siteId"]);

            if (!Main.AdminApi.IsSiteAuthorized(_siteId))
            {
                HttpContext.Current.Response.Write("<h1>未授权访问</h1>");
                HttpContext.Current.Response.End();
                return;
            }

            _configInfo = Main.GetConfigInfo(_siteId);

            if (IsPostBack) return;

            Utils.SelectListItems(DdlIsEnabled, _configInfo.IsEnabled.ToString());
            PhEnabled.Visible = _configInfo.IsEnabled;
            TbDefaultAmount.Text = _configInfo.DefaultAmount.ToString("N2");
            TbDescription.Text = _configInfo.Description;
        }

        public void DdlIsEnabled_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhEnabled.Visible = Convert.ToBoolean(DdlIsEnabled.SelectedValue);
        }

        public void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            _configInfo.IsEnabled = Convert.ToBoolean(DdlIsEnabled.SelectedValue);
            _configInfo.DefaultAmount = Convert.ToDecimal(TbDefaultAmount.Text);
            _configInfo.Description = TbDescription.Text;

            Main.ConfigApi.SetConfig(_siteId, _configInfo);
            LtlMessage.Text = Utils.GetMessageHtml("文章打赏设置修改成功！", true);
        }
    }
}