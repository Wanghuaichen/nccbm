﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="zgys_list.aspx.cs" Inherits="NCCBM.data.Report.zgys.zgys_list" %>
<%@ Register Src="../../../Menu/ActionControl.ascx" TagName="ActionControl" TagPrefix="uc1" %>
<%@ Register TagPrefix="cc1" Namespace="X1.Pager" Assembly="X1.Pager" %>
<%@ Register TagPrefix="cc1" Namespace="NCCBM" Assembly="NCCBM" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>    
    
    <link  href="../../../css/jquery-ui-1.8.20.custom.css" rel="stylesheet" type="text/css" />
    <link  href="../../../css/superTables.css" rel="stylesheet" type="text/css" />    
    <script src="../../../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../../js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
    <script src="../../../js/jquery-ui.zh-CN.js" type="text/javascript"></script>
    <script src="../../../js/superTables.js" type="text/javascript"></script>
    <script src="../../../js/jquery.superTable.js" type="text/javascript"></script>
    <script runat="server" language="c#">
        public void GridViewRowBound(object sender,GridViewRowEventArgs e)
        {
            NCCBM.EXGridView.GridViewRowBound(sender, e);
        } 
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:GridView ID="GridView2" runat="server" Width="840px"
            AutoGenerateColumns="False" RowStyle-HorizontalAlign="Center"
            CssClass="Table100" AllowPaging="true" 
            OnPageIndexChanging="Page_IndexChanging" 
            OnRowCommand="GridView2_RowCommand" 
            OnRowDataBound="GridViewRowBound" ShowHeader="true">

            <PagerSettings FirstPageText="首页" LastPageText="尾页" Mode="NumericFirstLast" NextPageText="下一页"
                PageButtonCount="25" PreviousPageText="上一页"  Position="Bottom"/>

            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <RowStyle CssClass="tableLine" />
            <EditRowStyle BackColor="#9EB6CE" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle CssClass="tableLineHeader" />
            <AlternatingRowStyle CssClass="tableLineAlternating" />
            <Columns>
                <asp:BoundField DataField="id" HeaderText="序号" ItemStyle-Width="50px" ItemStyle-Font-Size="Small"/>
                <asp:BoundField DataField="lururiqi" HeaderText="录入日期" ItemStyle-Width="80px" ItemStyle-Font-Size="Small" DataFormatString="{0:yyyy-MM-dd}"/>
                <asp:BoundField DataField="qukuai" HeaderText="区块" ItemStyle-Width="100px" ItemStyle-Font-Size="Small"/>
                <asp:BoundField DataField="taoguanzhiliangwenti" HeaderText="套管质量问题" ItemStyle-Width="100px" ItemStyle-Font-Size="Small"/>
                <asp:BoundField DataField="xiayu" HeaderText="下雨及其影响天数" ItemStyle-Width="100px" ItemStyle-Font-Size="Small"/>
                <asp:BoundField DataField="gongnongguanxi" HeaderText="工农关系影响天数" ItemStyle-Width="100px" ItemStyle-Font-Size="Small"/>
                <asp:BoundField DataField="cheliangweixiu" HeaderText="车辆维修天数" ItemStyle-Width="100px" ItemStyle-Font-Size="Small"/>
                <asp:BoundField DataField="jingchangbanqian" HeaderText="井场搬迁" ItemStyle-Width="100px" ItemStyle-Font-Size="Small"/>
                <asp:BoundField DataField="dengdaijingtaibanqian" HeaderText="等待井台搬迁" ItemStyle-Width="100px" ItemStyle-Font-Size="Small"/>
                <asp:BoundField DataField="beishuipeiye" HeaderText="备水配液" ItemStyle-Width="100px" ItemStyle-Font-Size="Small"/>
                <asp:BoundField DataField="beizhu" HeaderText="备注" ItemStyle-Width="50px" ItemStyle-Font-Size="Small"/>
                

                <asp:TemplateField HeaderText="操作" ShowHeader="False" Visible="False" ItemStyle-Width="50px" ItemStyle-Font-Size="Small">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnEdit" runat="server" CausesValidation="False" Text="修改" CommandName="xiugai"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:TemplateField HeaderText="操作" ShowHeader="False" Visible="False" ItemStyle-Width="50px" ItemStyle-Font-Size="Small">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnDel" runat="server" CausesValidation="False" Text="删除" CommandName="shanchu" OnClientClick="JavaScript:return confirm('确定要删除吗？')"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField> 
            </Columns>
        </asp:GridView>
    </form>
</body>
</html>