<%@ Page Language="C#" Inherits="SS.Reward.Pages.PageSettings" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <link href="assets/plugin-utils/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/plugin-utils.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/plugin-utils/css/ionicons.min.css" rel="stylesheet" type="text/css" />
  </head>

  <body>
    <div style="padding: 20px 0;">

      <div class="container">
        <form id="form" runat="server" class="form-horizontal">

          <div class="row">
            <div class="card-box">
              <div class="row">
                <div class="col-lg-10">
                  <h4 class="m-t-0 header-title"><b>文章打赏设置</b></h4>
                  <p class="text-muted font-13 m-b-30">
                    在此设置文章打赏功能
                  </p>
                </div>
              </div>

              <asp:Literal id="LtlMessage" runat="server" />

              <div class="form-horizontal">

                <div class="form-group">
                  <label class="col-sm-3 control-label">是否启用文章打赏</label>
                  <div class="col-sm-3">
                    <asp:DropDownList ID="DdlIsEnabled" OnSelectedIndexChanged="DdlIsEnabled_SelectedIndexChanged" AutoPostBack="true" class="form-control"
                      runat="server">
                      <asp:ListItem Text="启用" Value="True" Selected="True" />
                      <asp:ListItem Text="禁用" Value="False" />
                    </asp:DropDownList>
                  </div>
                  <div class="col-sm-6">
                    <span class="help-block">启用后打赏按钮将出现在您的文章底部</span>
                  </div>
                </div>

                <asp:PlaceHolder id="PhEnabled" runat="server">
                  <div class="form-group">
                    <label class="col-sm-3 control-label">默认接受打赏金额</label>
                    <div class="col-sm-3">
                      <div class="input-group">
                        <span class="input-group-addon"><i class="ion-social-yen"></i></span>
                        <asp:TextBox id="TbDefaultAmount" class="form-control" runat="server"></asp:TextBox>
                        <span class="input-group-addon">元</span>
                      </div>
                    </div>
                    <div class="col-sm-6">
                      <asp:RequiredFieldValidator ControlToValidate="TbDefaultAmount" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                      />
                      <asp:RegularExpressionValidator ControlToValidate="TbDefaultAmount" runat="server" ValidationExpression="^[1-9]\d*(\.\d+)?$"
                        ErrorMessage="请输入正确的金额" foreColor="red"></asp:RegularExpressionValidator>
                    </div>
                  </div>

                  <div class="form-group">
                    <label class="col-sm-3 control-label">打赏描述</label>
                    <div class="col-sm-3">
                      <asp:TextBox id="TbDescription" TextMode="MultiLine" class="form-control" runat="server"></asp:TextBox>
                    </div>
                    <div class="col-sm-6">
                      <span class="help-block"></span>
                    </div>
                  </div>
                </asp:PlaceHolder>

                <div class="form-group m-b-0">
                  <div class="col-sm-offset-3 col-sm-9">
                    <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
                  </div>
                </div>

              </div>
            </div>
          </div>

        </form>
      </div>
    </div>
  </body>

  </html>