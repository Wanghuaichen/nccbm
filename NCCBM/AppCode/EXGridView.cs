using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections;
using System.Text;
using MR.Cdecl;

namespace NCCBM
{
    public class EXGridView : GridView
    {
        public event EventHandler OnCreateHeader;
        public void oInit()
        {
            trIndex = new ArrayList();
            trRule = new ArrayList();
            trReplaceUrl = new ArrayList();
            trUIType = new ArrayList();
            trText = new ArrayList();
            trSize = new ArrayList();
            ViewState["trMetaData"] = new DataSet();
        }
        /// <summary>
        /// initial page to the control page
        /// </summary>
        public EXGridView()
        {
            System.Globalization.CultureInfo cultureInfo;
            cultureInfo = new System.Globalization.CultureInfo("zh-cn");
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;

            page = (System.Web.UI.Page)HttpContext.Current.Handler;//call the callback page
        }

        public static void GridViewRowBound(object sender, GridViewRowEventArgs e)
        {
            //�˴��������ӹ���ǰ����ɫ��ɫ
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.EmptyDataRow)
            {
                //����Զ������ԣ�������ƹ���ʱ���ø��еı���ɫΪ"6699ff",������ԭ����ɫ
                e.Row.Attributes.Add("onmouseover", "currentClass=this.className;this.className='tableLineSelected'");
                //����Զ������ԣ����������ʱ��ԭ���еı���ɫ
                e.Row.Attributes.Add("onmouseout", "this.className=currentClass");
            }
        }

        #region ����
        /// <summary>
        /// use this property to store the count of column that will be hidden before create blank row
        /// </summary>
        public int ColumnHideCount = 1;

        /// <summary>                                                                                                                                                                                                                                                                                                                                                                    
        /// �Ƿ����û��߽�ֹ��������                                                                                                                                                                                                                                                                                                                                                         
        /// </summary>                                                                                                                                                                                                                                                                                                                                                                       
        [
        Description("�Ƿ����ö���������"),
        Category("����"),
        DefaultValue("false"),
        ]
        public bool AllowMultiColumnSorting
        {
            get
            {
                object o = ViewState["EnableMultiColumnSorting"];
                return (o != null ? (bool)o : false);
            }
            set
            {
                AllowSorting = true;
                ViewState["EnableMultiColumnSorting"] = value;
            }
        }
        /// <summary>
        /// using all page config to sort all data
        /// </summary>
        [
        Description("�Ƿ����ö�ҳ������"),
        Category("����"),
        DefaultValue("false"),
        ]
        public bool AllowAllPage
        {
            get
            {
                object o = ViewState["EnableAllPageSorting"];
                return (o != null ? (bool)o : false);
            }
            set { ViewState["EnableAllPageSorting"] = value; }
        }

        /// <summary>
        /// ����ʱ��ʾͼ��
        /// </summary>
        [
        Description("����ʱ��ʾͼ��"),
        Category("����"),
        Editor("System.Web.UI.Design.UrlEditor", typeof(System.Drawing.Design.UITypeEditor)),
        DefaultValue(""),

        ]
        public string SortAscImageUrl
        {
            get
            {
                object o = ViewState["SortImageAsc"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["SortImageAsc"] = value;
            }
        }

        /// <summary>
        /// ����ʱ��ʾͼ��                                                                                                                                                                                                                                                                                                                                                                   
        /// </summary>                                                                                                                                                                                                                                                                                                                                                                       
        [
        Description("����ʱ��ʾͼ��"),
        Category("����"),
        Editor("System.Web.UI.Design.UrlEditor", typeof(System.Drawing.Design.UITypeEditor)),
        DefaultValue(""),
        ]
        public string SortDescImageUrl
        {
            get
            {
                object o = ViewState["SortImageDesc"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["SortImageDesc"] = value;
            }
        }

        /// <summary>
        /// ���������ַ���                                                                                                                                                                                                                                                                                                                                                                   
        /// </summary>                                                                                                                                                                                                                                                                                                                                                                       
        [
        Description("�����ַ���"),
        Category("����"),
        DefaultValue(""),
        ]
        public string SortExpression1
        {
            get
            {
                object o = ViewState["SortExpression1"];
                return (o != null ? o.ToString() : "");
            }
            set
            {
                ViewState["SortExpression1"] = value;
            }
        }

        #endregion

        #region ���Ĺ�ϵ����
        private ArrayList _trIndex;
        /// <summary>
        /// �����й�����к��ڱ���е�˳��
        /// </summary>
        public ArrayList trIndex
        {
            get
            { _trIndex = (ArrayList)ViewState["trIndex"]; return _trIndex; }
            set
            {
                ViewState["trIndex"] = value;
            }
        }

        private ArrayList _trRule;
        /// <summary>
        /// �����й�����к������ֶεĹ���
        /// </summary>
        public ArrayList trRule
        {
            get
            {
                _trRule = (ArrayList)ViewState["trRule"];
                return _trRule;
            }
            set
            {
                ViewState["trRule"] = value;
            }
        }

        private ArrayList _trUIType;//FieldUIType,_trRuleImageUrl
        /// <summary>
        /// �����й�����к������ֶε�UI��������
        /// </summary>
        public ArrayList trUIType
        {
            get
            { _trUIType = (ArrayList)ViewState["trUIType"]; return _trUIType; }
            set
            {
                ViewState["trUIType"] = value;
            }
        }

        private ArrayList _trText;
        /// <summary>
        /// �����й�����к������ֶε�ƥ��Ĭ��ֵ
        /// </summary>
        public ArrayList trText
        {
            get
            { _trText = (ArrayList)ViewState["trText"]; return _trText; }
            set
            {
                ViewState["trText"] = value;
            }
        }

        private ArrayList _trReplaceUrl;
        /// <summary>
        /// �����й�����к������ֶε�ƥ���滻ֵ
        /// </summary>
        public ArrayList trReplaceUrl
        {
            get
            { _trReplaceUrl = (ArrayList)ViewState["trReplaceUrl"]; return _trReplaceUrl; }
            set
            {
                ViewState["trReplaceUrl"] = value;
            }
        }

        /// <summary>
        /// �Գ������ı���������,title��ʾȫ����
        /// </summary>
        public ArrayList trSize
        {
            get
            { _trSize = (ArrayList)ViewState["trSize"]; return _trSize; }
            set
            {
                ViewState["trSize"] = value;
            }
        }

        private ArrayList _trSize;
        private DataSet metaData = new DataSet();

        /// <summary>
        /// get data to show metadata in page sucn as (M,W)={man,woman}
        /// </summary>
        public DataSet MetaData
        {
            get
            {
                metaData = (DataSet)ViewState["trMetaData"]; return metaData;
            }
            set
            {
                ViewState["trMetaData"] = value;
            }
        }

        #endregion

        #region �ܱ����ķ���
        /// <summary>
        /// ���������ֶεķ�������
        /// </summary>
        protected SortDirection _SortDirection
        {
            get
            {
                object o = ViewState["_SortDirection"];
                return (o != null ? (SortDirection)o : SortDirection.Descending);
            }
            set
            {
                ViewState["_SortDirection"] = value;
            }
        }

        /// <summary>                                                                                                                                                                                                                                                                                                                                                                    
        ///  ��ȡ������ʽ                                                                                                                                                                                                                                                                                                                                                                  
        /// </summary>                                                                                                                                                                                                                                                                                                                                                                       
        protected string GetSortExpression(GridViewSortEventArgs e)
        {
            string[] sortColumns = null;
            //��Ϊ������һ������asc�����Բ��ø���ֵ����ֵ��OnSorting������
            //if (SortExpression1 == "") SortExpression1 = e.SortExpression + " DESC ";
            string sortAttribute = SortExpression1;

            if (sortAttribute != String.Empty)
            {
                sortColumns = sortAttribute.Split(",".ToCharArray());
            }
            if (sortAttribute.IndexOf(e.SortExpression) > 0 || sortAttribute.StartsWith(e.SortExpression))
            {
                sortAttribute = ModifySortExpression(sortColumns, e.SortExpression);
            }
            else
            {
                sortAttribute += String.Concat(",", e.SortExpression, " ASC ");
            }
            return sortAttribute.TrimStart(",".ToCharArray()).TrimEnd(",".ToCharArray());

        }

        /// <summary>                                                                                                                                                                                                                                                                                                                                                                    
        ///  �޸�����˳��                                                                                                                                                                                                                                                                                                                                                                    
        /// </summary>                                                                                                                                                                                                                                                                                                                                                                       
        protected string ModifySortExpression(string[] sortColumns, string sortExpression)
        {
            string ascSortExpression = String.Concat(sortExpression, " ASC ");
            string descSortExpression = String.Concat(sortExpression, " DESC ");

            for (int i = 0; i < sortColumns.Length; i++)
            {

                if (ascSortExpression.Trim().ToUpper().Equals(sortColumns[i].Trim().ToUpper()))//if pass by url,' 'is trim by http
                {
                    sortColumns[i] = descSortExpression;
                }

                else if (descSortExpression.Trim().ToUpper().Equals(sortColumns[i].Trim().ToUpper()))
                {
                    Array.Clear(sortColumns, i, 1);
                }
            }

            return String.Join(",", sortColumns).Replace(",,", ",").TrimStart(",".ToCharArray());

        }

        /// <summary>                                                                                                                                                                                                                                                                                                                                                                    
        ///  ��ȡ��ǰ�ı��ʽ����ѡ�н�������                                                                                                                                                                                                                                                                                                                                                
        /// </summary>                                                                                                                                                                                                                                                                                                                                                                       
        protected void SearchSortExpression(string[] sortColumns, string sortColumn, out string sortOrder, out int sortOrderNo)
        {
            sortOrder = "";
            sortOrderNo = -1;
            for (int i = 0; i < sortColumns.Length; i++)
            {
                if (sortColumns[i].ToUpper().StartsWith(sortColumn.ToUpper() + " "))//fixed this bug,because fields like so and sou both has so as start
                {
                    sortOrderNo = i + 1;
                    if (AllowMultiColumnSorting)
                    {
                        sortOrder = sortColumns[i].Substring(sortColumn.Length).Trim();
                    }
                    else
                    {//�жϵ����ֶ�ʱ�����
                        sortOrder = ((_SortDirection == SortDirection.Ascending) ? "ASC" : "DESC");
                    }
                }
            }
        }
        #endregion

        /// <summary>                                                                                                                                                                                                                                                                                                                                                                    
        ///  �����������ͼƬ                                                                                                                                                                                                                                                                                                                                                              
        /// </summary>                                                                                                                                                                                                                                                                                                                                                                       
        protected void DisplaySortOrderImages(string sortExpression, GridViewRow dgItem)
        {
            string[] sortColumns = sortExpression.Split(",".ToCharArray());

            for (int i = 0; i < dgItem.Cells.Count; i++)
            {
                if (dgItem.Cells[i].Controls.Count > 0 && dgItem.Cells[i].Controls[0] is LinkButton)
                {
                    string sortOrder;
                    int sortOrderNo;
                    string column = ((LinkButton)dgItem.Cells[i].Controls[0]).CommandArgument;
                    SearchSortExpression(sortColumns, column, out sortOrder, out sortOrderNo);
                    if (sortOrderNo > 0)
                    {
                        string sortImgLoc = (sortOrder.Equals("ASC") ? SortAscImageUrl : SortDescImageUrl);

                        if (sortImgLoc != String.Empty)
                        {
                            Image imgSortDirection = new Image();
                            imgSortDirection.ImageUrl = sortImgLoc;
                            dgItem.Cells[i].Controls.Add(imgSortDirection);

                        }
                        else
                        {

                            if (AllowMultiColumnSorting)
                            {
                                Literal litSortSeq = new Literal();
                                litSortSeq.Text = sortOrderNo.ToString();
                                dgItem.Cells[i].Controls.Add(litSortSeq);

                            }
                        }
                    }
                }
            }
        }

        #region ��д����
        private bool _check = false;
        /// <summary>
        /// using this property to hidden the checkbox in list
        /// </summary>
        public bool CheckBoxHiddened
        {
            get { return _check; }
            set { _check = value; }
        }
        protected override void OnSorting(GridViewSortEventArgs e)
        {

            if (AllowMultiColumnSorting)
            {
                e.SortExpression = GetSortExpression(e);
                SortExpression1 = e.SortExpression;
            }
            else
            {
                if (_SortDirection == SortDirection.Ascending)
                    _SortDirection = SortDirection.Descending;
                else
                    _SortDirection = SortDirection.Ascending;
                if (e.SortDirection == SortDirection.Descending)
                    _SortDirection = SortDirection.Descending;
                if (e.SortExpression.LastIndexOf(' ') != -1)
                    _SortDirection = e.SortDirection;
                else
                    e.SortDirection = _SortDirection;
                SortExpression1 = e.SortExpression.Split(' ')[0] + " " + ((_SortDirection == SortDirection.Ascending) ? "ASC" : "DESC");
            }
            if (AllowSorting)//ֻ�е�ʹ�����򣬲ŵ���ǰ�÷���
                base.OnSorting(e);
        }

        public void HanderSort1(GridViewSortEventArgs e)
        {

            if (AllowSorting)
            {//ֻ�е�ʹ�����򣬲ŵ���ǰ�÷���
                _SortDirection = SortExpression1.IndexOf(" ASC") > 0 ? SortDirection.Ascending : SortDirection.Descending;
                base.OnSorting(e);
            }
        }

        public void HanderSort(GridViewSortEventArgs e)
        {
            this.OnSorting(e);
        }

        protected override void OnRowCreated(GridViewRowEventArgs e)
        {
            if (this.OnCreateHeader != null)
            {
                this.OnCreateHeader(this, e);
                e.Row.Cells[2].CssClass = "hidden";
                return;
            }
            if (e.Row.RowType == DataControlRowType.Header)
            {
                if (SortExpression1 != String.Empty)
                {
                    DisplaySortOrderImages(SortExpression1, e.Row);
                    this.CreateRow(0, 0, DataControlRowType.EmptyDataRow, DataControlRowState.Normal);
                }
            }
            e.Row.Cells[2].CssClass = "hidden";//must has a key ,this index is key
            base.OnRowCreated(e);
        }

        /// <summary>
        /// ��GridView��ʾ��껮������
        /// </summary>
        /// <param name="sender">Ĭ�ϴ��ݵ�GridView</param>
        /// <param name="e">���¼�����</param>
        protected override void OnRowDataBound(GridViewRowEventArgs e)
        {
            //�˴��������ӹ���ǰ����ɫ��ɫ
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.EmptyDataRow)
            {
                //����Զ������ԣ�������ƹ���ʱ���ø��еı���ɫΪ"6699ff",������ԭ����ɫ
                e.Row.Attributes.Add("onmouseover", "currentClass=this.className;this.className='tableLineSelected'");
                //����Զ������ԣ����������ʱ��ԭ���еı���ɫ
                e.Row.Attributes.Add("onmouseout", "this.className=currentClass");
            }
            if (CheckBoxHiddened) e.Row.Cells[1].CssClass = "hidden";//hidden the check
            //��Ź���
            if (e.Row.RowIndex != -1)
            {
                try
                {//�������dataview��
                    DataSet ds;
                    DataView dv;
                    string text;
                    int _fieldindex;
                    string _fieldname;
                    if (this.DataSource is DataSet)
                    {
                        ds = this.DataSource as DataSet;
                        e.Row.Cells[0].Text = ds.Tables[0].Rows[e.Row.RowIndex]["RowNo"].ToString();
                    }
                    else
                        if (this.DataSource is DataView)
                        {
                            dv = this.DataSource as DataView;
                            e.Row.Cells[0].Text = (dv.Table.Rows[e.Row.RowIndex]["RowNo"] == DBNull.Value) ? "" : dv.Table.Rows[e.Row.RowIndex]["RowNo"].ToString();
                        }


                    if (e.Row.Cells[0].Text == "")//�Զ����Ŀ���ȥ��ϵͳĬ�ϵ�ģ��ؼ�
                    {
                        int _cspan = e.Row.Cells.Count;
                        e.Row.Controls.Clear();
                        for (int _i = 1; _i < _cspan; _i++)//������һ��ID��
                        {
                            TableCell tc = new TableCell();
                            //tc.ColumnSpan = _cspan;
                            tc.Controls.Add(new LiteralControl("&nbsp;"));
                            e.Row.Controls.Add(tc);
                        }
                        if (CheckBoxHiddened) e.Row.Cells[1].CssClass = "hidden";//hidden the check
                    }
                    else
                    {
                        if (e.Row.Cells.Count > 3)
                        {
                            Regex regex1 = new Regex(@"<[^>]*>", RegexOptions.IgnoreCase);
                            for (int k = 2; k < e.Row.Cells.Count; k++)
                            {//this continue let loop count lesser
                                bool bnext = false;
                                for (int m = 0; m < trIndex.Count; m++)
                                    if (k == (int)trIndex[m]) { bnext = true; break; }
                                if (bnext) continue;//only that no template field use this rule
                                e.Row.Cells[k].Text = regex1.Replace(e.Row.Cells[k].Text, "");//here is question that some need B font but other not need
                                e.Row.Cells[k].Text = e.Row.Cells[k].Text.Replace("\n", "<br>");//show enter
                            }

                            for (int k = 0; k < trIndex.Count; k++)
                            {
                                string metaname = trReplaceUrl[k].ToString().Split(",".ToCharArray())[0];
                                int sizeLimit = (int)trSize[k];
                                _fieldindex = (int)trIndex[k];
                                _fieldname = (this.Columns[_fieldindex] as BoundField).DataField;
                                text = e.Row.Cells[_fieldindex].Text;
                                sizeLimit = (sizeLimit == 0) ? int.MaxValue : sizeLimit;
                                if (text.Length > sizeLimit)
                                {
                                    e.Row.Cells[_fieldindex].Text = text.Substring(0, sizeLimit).Replace("\\n", "<br>") + "...";
                                    e.Row.Cells[_fieldindex].ToolTip = text;
                                }

                                if (trText[k].ToString() != "")//trText[k].ToString() == e.Row.Cells[_fieldindex].Text &&
                                {//modify here to add meta data to screen

                                    try//field value is bad
                                    {
                                        DataRow[] rows;
                                        if (text == "&nbsp;")
                                        {//do nothing here bacause no value in text,ver2:later we found null is use in such(no data)hint,add by roo

                                            rows = MetaData.Tables[k].Select(metaname + "=''");
                                            if (rows.Length > 0)
                                            {
                                                e.Row.Cells[_fieldindex].Text = rows[0][1].ToString();
                                            }
                                        }
                                        else
                                        {
                                            switch (trUIType[k].ToString())
                                            {
                                                case "NAMEVALUECOLLECTION"://is name collection control
                                                    string[] objIDs = text.Split(';');
                                                    e.Row.Cells[_fieldindex].Text = "";//set null
                                                    foreach (string s in objIDs)
                                                    {
                                                        rows = MetaData.Tables[k].Select(metaname + "='" + s + "'");
                                                        if (rows.Length > 0)
                                                        {
                                                            e.Row.Cells[_fieldindex].Text += rows[0][1].ToString() + ";";
                                                        }
                                                    }
                                                    e.Row.Cells[_fieldindex].Text = e.Row.Cells[_fieldindex].Text.TrimEnd(';');

                                                    break;
                                                default:
                                                    rows = MetaData.Tables[k].Select(metaname + "='" + text + "'");
                                                    if (rows.Length > 0)
                                                    {
                                                        e.Row.Cells[_fieldindex].Text = rows[0][1].ToString();
                                                    }
                                                    else
                                                    {
                                                        e.Row.Cells[_fieldindex].Text = text;
                                                    }
                                                    break;
                                            }

                                        }
                                        //complicate condition text template+rule
                                        if (text.Length > sizeLimit) { e.Row.Cells[_fieldindex].ToolTip = e.Row.Cells[_fieldindex].Text; e.Row.Cells[_fieldindex].Text = e.Row.Cells[_fieldindex].Text.Substring(0, sizeLimit).Replace("\\n", "<br>") + "..."; }
                                        if (trRule[k].ToString() != "")
                                        {
                                            metaname = trRule[k].ToString().Replace("<No.>", this.DataKeys[e.Row.RowIndex].Value.ToString());
                                            metaname = metaname.Replace("<Text.>", e.Row.Cells[_fieldindex].Text);
                                            e.Row.Cells[_fieldindex].Text = "<a href=javascript:OpenNode('" + this.DataKeys[e.Row.RowIndex].Value.ToString() + "','" + _fieldname + "')>" + metaname + "</a>";
                                        }
                                    }
                                    catch (Exception ex) { }

                                }
                                else
                                {
                                    if (trRule[k].ToString() != "")
                                    {
                                        metaname = trRule[k].ToString().Replace("<No.>", this.DataKeys[e.Row.RowIndex].Value.ToString());
                                        metaname = metaname.Replace("<Text.>", e.Row.Cells[_fieldindex].Text);
                                        e.Row.Cells[_fieldindex].Text = "<a href=javascript:OpenNode('" + this.DataKeys[e.Row.RowIndex].Value.ToString() + "','" + _fieldname + "')>" + metaname + "</a>";
                                    }
                                    else
                                        e.Row.Cells[_fieldindex].Text = "<a href=javascript:OpenNode('" + this.DataKeys[e.Row.RowIndex].Value.ToString() + "','" + _fieldname + "') title='" + text.Replace("\\n", "&#13;&#10;") + "'>" + e.Row.Cells[_fieldindex].Text + "</a>";

                                }
                                if ((this.Columns[_fieldindex] as BoundField).ApplyFormatInEditMode)
                                { //has attachment

                                    //e.Row.Cells[_fieldindex].Text += GetAttachmentInfo(this.DataKeys[e.Row.RowIndex].Value.ToString());
                                }
                            }

                            //e.Row.Cells[3].Text = "<a href=javascript:OpenNode('" + this.DataKeys[e.Row.RowIndex].Value.ToString() + "')>" + e.Row.Cells[3].Text + "</a>";
                        }
                    }
                }
                catch (Exception ex) { }
            }
        }
        

        #endregion

        #region public method call by action
        /// <summary>
        /// ������ͼ�Ŀ���,���ܼ����ݵ�TABLE�ٰ󶨣�����������ɸѡ����ͼ��Զ����ָ��λ�ã�
        /// ��ʹ��ʽλ������������ʾҲ��ɸѡλ�ù̶�,�����ڴ˴���û�м�����¼�
        /// </summary>
        /// <param name="index"></param>
        public void CreateBlankViewRows(int index)
        {
            GridViewRow row = this.CreateRow(index, index, DataControlRowType.DataRow, DataControlRowState.Normal);

            Table tb = this.Controls[0] as Table;
            TableCell tc;
            int _cspan = this.Rows[0].Cells.Count;

            for (int _i = ColumnHideCount; _i < _cspan; _i++)//������һ��ID��,using ColumnHideCount instead by roo,2008-7-11
            {
                tc = new TableCell();
                tc.Controls.Add(new LiteralControl("&nbsp;"));
                row.Cells.Add(tc);
            }
            row.CssClass = (index % 2 == 0 ? this.RowStyle.CssClass : this.AlternatingRowStyle.CssClass);
            //if (tb.Rows[index - 1].CssClass == this.RowStyle.CssClass)
            //    row.CssClass  = this.AlternatingRowStyle.CssClass;
            //else
            //    row.CssClass = this.RowStyle.CssClass;
            tb.Rows.Add(row);
            //this.Context.Response.Write(this.Rows.Count + ";");
        }
        #endregion


        //for list
        #region list ����

        private string sqlSearchCondition = "";
        private string sqlOrderCondition = "";
        private string tablePrimaryKey = "ID";
        private System.Web.UI.Page page;
        private string sort;
        private string fields = "*";
        /// <summary>
        /// default sort string for list
        /// </summary>
        public string DefaultSort
        {
            get
            { return sort; }
            set
            { sort = value; }
        }
        /// <summary>
        /// sql search condition string to get data for special user
        /// </summary>
        public string SqlSearchCondition
        {
            get
            {
                return sqlSearchCondition;
            }
            set
            {
                sqlSearchCondition = value;
            }
        }

        /// <summary>
        /// sql order by
        /// </summary>
        public string SqlOrderCondition
        {
            get
            { return sqlOrderCondition; }
            set
            { sqlOrderCondition = value; }
        }
        /// <summary>
        /// the primary key of table ,can use any name of key
        /// </summary>
        public string TablePrimaryKey
        {
            get
            {
                return tablePrimaryKey;
            }
            set
            {
                tablePrimaryKey = value;
            }
        }

        #endregion

        /// <summary>
        /// ���洢���̵Ĳ�����ֵ
        /// </summary>
        /// <param name="tableName">����</param>
        /// <param name="currentPage">��ǰҳ����</param>
        /// <param name="pageSize">ÿҳ����</param>
        /// <returns>����2������ļ���</returns>
        private DataCollection SelfTableParameter(string tableName, int currentPage, int pageSize)
        {
            string names = "TName,TKey,TGetFields,TWhere,TOrder,CurrPage,Lines";//TName,TKey,TGetFields,TWhere,CurrPage,Lines
            DataCollection datas = new DataCollection();

            datas.Add(tableName);//�����ֶ�����˳����Ӿ�������
            datas.Add(tablePrimaryKey);//add table primary key name
            datas.Add(fields + ",RowNo,RecordCount,PageCount");//count is fixed field
            datas.Add(GetSqlOffOrder(SqlSearchCondition)[0]);//use SqlSearchCondition instead of SqlBase.Sqlfilter
            datas.Add(sqlOrderCondition);
            datas.Add(currentPage + 1);
            datas.Add(pageSize);
            return STD.DBA.GetCollectionDatas(names, datas);

        }

        /// <summary>
        /// get sql syntax no order syntax
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string[] GetSqlOffOrder(string sql)
        {
            string[] sqlLists = sql.Split(new string[] { "order by" }, StringSplitOptions.None);
            return sqlLists;
        }

        /// <summary>
        /// self bind data ,call by public method
        /// </summary>
        /// <param name="tablename">DataSet</param>
        /// <param name="templateGridView">GridView</param>
        /// <param name="templatePageBar">PageBar</param>
        public void AutoBindData(string tablename, X1.Pager.PageBar templatePageBar)
        {
            Int32 rsCount = 0, rsPageCount = 0;
            StringBuilder cols = new StringBuilder();
            cols.Append(tablePrimaryKey);//uid,name,ddd
            for (int i = 3; i < this.Columns.Count; i++)
            {//1 is no. 2 is check,3 is uid,4 is normal
                if (tablePrimaryKey != this.Columns[i].SortExpression && this.Columns[i].SortExpression!="")//exclude uid field,by roo.2009-7-10
                {
                    cols.Append(",");
                    cols.Append(this.Columns[i].SortExpression);
                }
            }
            fields = cols.ToString();
            DataSet ds = STD.DBA.SqlRecordSet(SelfTableParameter(tablename, templatePageBar.CurrentPageIndex,
                this.AllowPaging ? this.PageSize : Int32.MaxValue), "mytable");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count != 0)//�ж���û�д򿪼�¼
            {
                //������
                rsCount = Convert.ToInt32(ds.Tables[0].Rows[0]["RecordCount"]);
                rsPageCount = Convert.ToInt32(ds.Tables[0].Rows[0]["PageCount"]);
                //GridView1.PageCount = rsPageCount;
                int addCount = this.PageSize - ds.Tables[0].Rows.Count;

                if (this.AllowSorting && DefaultSort != null)//has sort
                {
                    DataView dvMainView = new DataView();
                    dvMainView = ds.Tables[0].DefaultView;
                    dvMainView.Sort = DefaultSort;
                    dvMainView.AllowNew = true;
                    addCount = this.PageSize - dvMainView.Table.Rows.Count;
                    this.DataSource = dvMainView;
                    this.DataBind();
                    for (int i = 0; i < addCount; i++)
                    {
                        //dvMainView.AddNew();    
                        this.CreateBlankViewRows(i + dvMainView.Table.Rows.Count);
                    }
                }
                else
                {
                    for (int i = 0; i < addCount; i++)
                    {
                        DataRow row = ds.Tables[0].NewRow();
                        ds.Tables[0].Rows.Add(row);
                    }
                    this.DataSource = ds;
                    this.DataBind();
                }

                //ds.Tables[0].Columns[0].RemoveAt(0);

                ShowStats(rsCount, templatePageBar);//show pagebar
            }
            else
            {
                //ds.Tables.Add();������
                if (!page.IsPostBack) //�״�ʹ��GRIDVIEW�γ�ҳ��Ҫ���ؿձ�ͷ�Ϳ���,RowNo����GridViewRowBound�γ��к�
                {
                    ds = STD.DBA.SqlRecordSet("select 1 as RowNo," + fields + " from " + tablename + " where 1<>1", "mytable");
                    int addCount = this.PageSize - ds.Tables[0].Rows.Count;
                    for (int i = 0; i < addCount; i++)
                    {
                        DataRow row = ds.Tables[0].NewRow();
                        ds.Tables[0].Rows.Add(row);
                    }
                    this.DataSource = ds;
                    this.DataBind();
                }
                for (int i = 0; i < this.Rows.Count; i++)
                {
                    for (int j = 0; j < this.Rows[i].Cells.Count; j++) this.Rows[i].Cells[j].Text = "";
                }

                ShowStats(rsCount, templatePageBar);
            }
            /*foreach (DataColumn  item in ds.Tables[0].Columns)
	            {
                    //Response.Write(item.Caption);
	            }*/
        }

        /// <summary>
        /// ��ʾ��ҳ�������ݵ���ҳ��
        /// </summary>
        /// <param name="rscount">��¼����</param>
        /// <param name="targetGridView">�󶨱��</param>
        /// <param name="targetPageBar">�󶨵ķ�ҳ����</param>
        void ShowStats(int rsCount,X1.Pager.PageBar targetPageBar)
        {
            targetPageBar.Visible = true;//show the list page bar
            //PageBar1.CurrentPageIndex = (int)GridView1.PageIndex;
            targetPageBar.RecordCount = rsCount;// ((DataSet)GridView1.DataSource).Tables[0].Rows.Count;
            targetPageBar.PageSize = this.PageSize;
            if (!this.AllowPaging && rsCount > this.PageSize)
                targetPageBar.PageSize = rsCount;

        }


        /// <summary>
        /// ����Ҫ������������ݣ�ֻ��ʹ��VIEW���ܼ�¼�з�ҳ״̬�Ķ��������־,add by roo. 
        /// ��������Ϊ�˽���ռ�¼������ɼ�¼ֻ���к������ݵ�����
        /// </summary>
        /// <param name="tablename">����ı������ѯ���</param>
        /// <param name="templateGridView">������</param>
        /// <param name="templatePageBar">��ҳ��</param>
        /// <param name="blnUseView">ʹ����ͼģʽ</param>
        public void AutoBindData(string tablename, X1.Pager.PageBar templatePageBar, bool blnUseView)
        {
            Int32 rsCount = 0, rsPageCount = 0;
            int columnCount = 0;
            DataView dvMainView = null;
            if (blnUseView)
            {
                StringBuilder cols = new StringBuilder();
                cols.Append(tablePrimaryKey);//uid,name,ddd
                for (int i = 3; i < this.Columns.Count; i++)
                {//1 is no. 2 is check,3 is uid,4 is normal
                    if (tablePrimaryKey != this.Columns[i].SortExpression)//exclude uid field,by roo.2009-7-10
                    {
                        cols.Append(",");
                        cols.Append(this.Columns[i].SortExpression);
                    }
                }
                fields = cols.ToString();
                DataSet ds = STD.DBA.SqlRecordSet(SelfTableParameter(tablename, templatePageBar.CurrentPageIndex,
                    this.AllowPaging ? this.PageSize : Int32.MaxValue), "mytable");
                int addCount;

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)//�ж���û�д򿪼�¼
                {
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        rsCount = Convert.ToInt32(ds.Tables[0].Rows[0]["RecordCount"]);
                        rsPageCount = Convert.ToInt32(ds.Tables[0].Rows[0]["PageCount"]);
                        dvMainView = ds.Tables[0].DefaultView;
                        if (this.SortExpression1 != "") //��Ϊ�գ���¼���������
                        {
                            dvMainView.Sort = this.SortExpression1;
                        }

                        dvMainView.AllowNew = true;


                    }
                    //������
                    addCount = this.PageSize - dvMainView.Table.Rows.Count;
                    this.DataSource = dvMainView;//������������ͼ
                    //if (tablename != "currentWorkflowPipe.EffectTableName" )//�ж��Ƿ��������
                    //{
                    //    columnCount = this.Columns.Count;
                    //    for (int i = 2; i < columnCount; i++)//�Ƴ�����ź�ѡ�����
                    //        this.Columns.RemoveAt(2);
                    //    BindTitle(templateGridView, tablename);
                    //    currentWorkflowPipe.EffectTableName = tablename;//�任����
                    //    //must remember ,here we after this event we must call save method save the pipe
                    //}
                    //ds.Tables[0].Columns[0].RemoveAt(0);
                    this.DataBind();
                    ShowStats(rsCount, templatePageBar);
                    for (int i = 0; i < addCount; i++)
                    {
                        //dvMainView.AddNew();    
                        this.CreateBlankViewRows(i + dvMainView.Table.Rows.Count);
                    }
                }
                else
                {
                    //ds.Tables.Add();������,���Բ���Ҫ������
                    if (!page.IsPostBack) //�״�ʹ��GRIDVIEW�γ�ҳ��Ҫ���ؿձ�ͷ�Ϳ���,RowNo����GridViewRowBound�γ��к�
                    {
                        ds = STD.DBA.SqlRecordSet("select 1 as RowNo," + fields + " from " + tablename + " where 1<>1", "mytable");//this sql also can run in ODP of oracle,but not pl-sql dev.
                        addCount = this.PageSize - ds.Tables[0].Rows.Count;
                        for (int i = 0; i < addCount; i++)
                        {
                            DataRow row = ds.Tables[0].NewRow();
                            ds.Tables[0].Rows.Add(row);
                        }
                        this.DataSource = ds;
                        //if (tablename != currentWorkflowPipe.EffectTableName  == EffectGridView)//�ж��Ƿ��������
                        //{
                        //    columnCount = templateGridView.Columns.Count;
                        //    for (int i = 2; i < columnCount; i++)//�Ƴ�����ź�ѡ�����
                        //        templateGridView.Columns.RemoveAt(2);
                        //    BindTitle(templateGridView, tablename);
                        //    currentWorkflowPipe.EffectTableName = tablename;//�任����
                        //}
                        this.DataBind();
                    }
                    for (int i = 0; i < this.Rows.Count; i++)
                    {
                        for (int j = 0; j < this.Rows[i].Cells.Count; j++) this.Rows[i].Cells[j].Text = "";
                    }

                    addCount = this.PageSize - this.Rows.Count;
                    for (int i = 0; i < addCount; i++)
                    {
                        //dvMainView.AddNew();    
                        this.CreateBlankViewRows(i + this.Rows.Count);
                    }

                    ShowStats(rsCount, templatePageBar);
                }
            }
        }

        /// <summary>
        /// ��̬�����ݵ���,add by roo.luo 
        /// �����Ϳ����ö������һ��ҳ����ʹ�����������
        /// </summary>
        /// <param name="targetGridView">�б����</param>
        /// <param name="tableName">Ҫȡ̧ͷ�ı�</param>
        public void AutoBindTitle(string fieldcaptions,string fieldnames)
        {
            this.AutoGenerateColumns = false;
            BoundField field = new BoundField();
            field.HeaderText = TablePrimaryKey;
            field.DataField = TablePrimaryKey;
            //field.Visible = false ;//ID��������¼���޸ı�־������ʾ��ҳ���� bind to GridView1_RowCreated so we can use id in client
            //field.SortExpression = "EmployeeID";
            //3 is the coloumn of real data,0 is key hide inside 1 is index,2 is checkbox
            int k = 3;
            string sort = null;
            bool useParentTemplate = true;
            this.Columns.Add(field);

            string[] sfieldcaptions = fieldcaptions.Split(',');
            string[] sfieldnames = fieldnames.Split(',');
            for (int i = 0; i < sfieldcaptions.Length; i++)
            {
                field = new BoundField();
                field.HeaderText = sfieldcaptions[i];//must has value,because fd tag is inherit from fields
                field.DataField = sfieldnames[i];
                field.SortExpression = sfieldnames[i];
                field.Visible = true;

                this.Columns.Add(field);

            }
        }

    }

    interface IActioner
    {
        void AddAction();
        void DeleteAction();
        void ModifyAction();
        void QueryAction();
        void PrintExcel();
        void DoAct1();
        void DoAct2();
        void DoAct3();
        void DoAct4();
        void DoAct5();
    }
}
