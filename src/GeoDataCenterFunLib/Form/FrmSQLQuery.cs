using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using System.Xml;
using System.IO;

namespace GeoDataCenterFunLib
{
    public partial class FrmSQLQuery : DevComponents.DotNetBar.Office2007Form
    {
        private IMapControlDefault m_MapControlDefault;
        private IMap m_pMap;
        private IFeatureLayer m_pCurrentLayer;
        ///���ҵ������ڳ�ʼ�������ֵ�   20111011  Zq add
        private IWorkspace m_Workspace = null;
        public string _LayerTreePath = System.Windows.Forms.Application.StartupPath + "\\..\\res\\xml\\��ѯͼ����.xml"; //ͼ��Ŀ¼�ļ�·��
        private XmlDocument _LayerTreeXmldoc = null;
        private Dictionary<string, string> _DicLayerList = null;
        public SysCommon.BottomQueryBar _QueryBar
        {
            get;
            set;
        }
        private string LayerID;
        public FrmSQLQuery(IMapControlDefault pMapControl, IWorkspace pWorkspace)
        {
            m_MapControlDefault = pMapControl;
            m_pMap = pMapControl.Map;
            ///��ʼ�������ֵ�  20111011  Zq add
            m_Workspace = pWorkspace;
            if (SysCommon.ModField._DicFieldName.Count == 0)
            {
                SysCommon.ModField.InitNameDic(m_Workspace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");
            }

            InitializeComponent();
        }
        private void btnOperateClick(DevComponents.DotNetBar.ButtonX button)
        {
            richTextExpression.Text += button.Text.Trim()+" ";
        }
        #region �������ĵ���¼�
        private void btnBigger_Click(object sender, EventArgs e)
        {
             btnOperateClick(btnBigger);
        }

        private void btnSmaller_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnSmaller );
        }

        private void btnEqual_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnEqual );
        }

        private void btnBiggerEqual_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnBiggerEqual );
        }

        private void btnSmallerEqual_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnSmallerEqual );
        }

        private void btnNotEqual_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnNotEqual );
        }

        private void btn1Ultra_Click(object sender, EventArgs e)
        {
            btnOperateClick(btn1Ultra );
        }

        private void btn2Ultra_Click(object sender, EventArgs e)
        {
            btnOperateClick(btn2Ultra );
        }

        private void btnPercent_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnPercent );
        }

        private void btnIs_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnIs );
        }

        private void btnOr_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnOr );
        }

        private void btnNot_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnNot );
        }

        private void btnLike_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnLike );
        }

        private void btnAnd_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnAnd );
        }
        #endregion

        private void FrmSQLQuery_Load(object sender, EventArgs e)
        {
            this.richTextExpression.Text = "";
            //ͼ��ѡ�����ݼ���
            //�ı�ͼ��ѡ��ʽ ����֮ǰ���� xisheng 20111119
            //IFeatureLayer pFeaTureLayer;
            //DevComponents.Editors.ComboItem item;
            //for (int iIndex = 0; iIndex < m_pMap.LayerCount; iIndex++)
            //{
            //    ILayer pLayer = m_pMap.get_Layer(iIndex);
            //    if (pLayer is IFeatureLayer)
            //    {
            //        pFeaTureLayer = pLayer as IFeatureLayer;
            //        item = new DevComponents.Editors.ComboItem();
            //        item.Text = pFeaTureLayer.Name;
            //        item.Tag = pFeaTureLayer;
            //        this.cmblayersel.Items.Add(item);//
            //    }
            //    else
            //    {
            //        if (pLayer is ICompositeLayer)
            //        {
            //            ICompositeLayer pCLayer=pLayer as ICompositeLayer;
            //            for (int j = 0; j < pCLayer.Count; j++)
            //            {
            //                ILayer pTemLayer = pCLayer.get_Layer(j);
            //                if (pTemLayer is IFeatureLayer)
            //                {
            //                    pFeaTureLayer = pTemLayer as IFeatureLayer;
            //                    item = new DevComponents.Editors.ComboItem();
            //                    item.Text = pFeaTureLayer.Name;
            //                    item.Tag = pFeaTureLayer;
            //                    this.cmblayersel.Items.Add(item);//
            //                }
            //            }
            //        }
            //    }
            //}
            //if (this.cmblayersel.Items.Count > 0)
            //{
            //    this.cmblayersel.SelectedIndex = 0;
            //    this.cmblayersel.Sorted = true;
            //}
            cmblayersel.Text = "���ѡ���ѯͼ��";

            //ѡ��ģʽ���ݼ���
            object[] objArray = new object[5];
            objArray[0] = "������ѡ����";
            objArray[1] = "����һ���µ�ѡ����";
            objArray[2] = "��ӵ���ǰѡ����";
            objArray[3] = "�ӵ�ǰѡ�������Ƴ�";
            objArray[4] = "�ӵ�ǰѡ������ѡ��";

            this.cmbselmode.Items.AddRange(objArray);//
            this.cmbselmode.SelectedIndex = 0;

            
            //��ʼ���ֶ��б�
            ColumnHeader newColumnHeader1 = new ColumnHeader();
            newColumnHeader1.Text = "Name";
            //newColumnHeader1.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.listViewField.Columns.Add(newColumnHeader1);

            //��ʼ��ֵ�б�
            SysCommon.ModSysSetting.CopyLayerTreeXmlFromDataBase(Plugin.ModuleCommon.TmpWorkSpace, _LayerTreePath);
            ColumnHeader newColumnHeader2 = new ColumnHeader();
            newColumnHeader2.Text = "Value";
            //newColumnHeader2.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.listViewValue.Columns.Add(newColumnHeader2);


            //ygc ��ʼ��ͼ����
             if (SysCommon.ModField._DicFieldName.Keys.Count == 0)
            {
                SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");
            }
            if (_DicLayerList == null)
            {
                _DicLayerList = new Dictionary<string, string>();
            }
            //��ʼ��ͼ�����б�
            if (File.Exists(_LayerTreePath))
            {
                if (_LayerTreeXmldoc == null)
                {
                    _LayerTreeXmldoc = new XmlDocument();
                }
                _LayerTreeXmldoc.Load(_LayerTreePath);
                advTreeLayers.Nodes.Clear();

                //��ȡXml�ĸ��ڵ㲢��Ϊ���ڵ�ӵ�UltraTree��
                XmlNode xmlnodeRoot = _LayerTreeXmldoc.DocumentElement;
                XmlElement xmlelementRoot = xmlnodeRoot as XmlElement;

                xmlelementRoot.SetAttribute("NodeKey", "Root");
                string sNodeText = xmlelementRoot.GetAttribute("NodeText");

                //�������趨���ĸ��ڵ�
                DevComponents.AdvTree.Node treenodeRoot = new DevComponents.AdvTree.Node();
                treenodeRoot.Name = "Root";
                treenodeRoot.Text = sNodeText;

                treenodeRoot.Tag = "Root";
                treenodeRoot.DataKey = xmlelementRoot;
                treenodeRoot.Expanded = true;
                this.advTreeLayers.Nodes.Add(treenodeRoot);

                treenodeRoot.Image = this.ImageList.Images["Root"];
                InitLayerTreeByXmlNode(treenodeRoot, xmlnodeRoot);
            }
        }
        
        //�ı�ͼ��ѡ��ʽ ����֮ǰ���� xisheng 20111119
        private void cmblayersel_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (this.cmblayersel.SelectedItem == null) return;
            //DevComponents.Editors.ComboItem item = this.cmblayersel.SelectedItem as DevComponents.Editors.ComboItem;                        //��ȡѡ����
            //if (item == null) return;

            //m_pCurrentLayer = item.Tag as IFeatureLayer;      //ע���ڳ�ʼ��ʱ����������������һ��

            //this.listViewField.Items.Clear();                             //��Ϊ�ֶ�ֵ����ͼ����ı䣬���
            //this.listViewValue.Items.Clear();

            //if (m_pCurrentLayer == null)                                     //������ѡ��ͼ�� ����ʾ
            //{
            //    MessageBox.Show("��ѡͼ����ڴ���", "��ʾ");
            //    return;
            //}

            //IFeatureClass pFeatureClass = m_pCurrentLayer.FeatureClass; //��ȡҪ����ļ���

            ////ѭ���õ�ÿһ���ֶβ������ֶ����ͽ��ж�Ӧ�Ĳ���
            ////�˴��Ժ�Ҫ�������Ӹ����жϺͶ�Ӧ����
            //for (int iIndex = 0; iIndex < pFeatureClass.Fields.FieldCount; iIndex++)
            //{
            //    IField pField = pFeatureClass.Fields.get_Field(iIndex);
            //    switch (pField.Type)
            //    {
            //        case esriFieldType.esriFieldTypeSmallInteger:
            //        case esriFieldType.esriFieldTypeInteger:
            //        case esriFieldType.esriFieldTypeSingle:
            //        case esriFieldType.esriFieldTypeDouble:
            //        case esriFieldType.esriFieldTypeString:
            //            ListViewItem newItem = new ListViewItem(new string[] { pField.AliasName + "��" + SysCommon.ModField.GetChineseNameOfField(pField.AliasName)+"��" });
            //            newItem.Tag =pField.Name;
            //            //////�����ֶ���������ʾ����
            //            newItem.ToolTipText = SysCommon.ModField.GetChineseNameOfField(pField.AliasName);
            //            this.listViewField.Items.Add(newItem);
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }
        //�ֶ��б�˫���¼������ֶ����Ƽ��뵽richTextExpression��
        private void listViewField_MouseDoubleClick(object sender, MouseEventArgs e)
        {
           ListViewItem currentFieldItem= this.listViewField.GetItemAt(e.Location.X, e.Location.Y);
             if (this.listViewField.SelectedItems.Count > 1)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ֻѡ��һ���ֶ�");
            }
            if (currentFieldItem == null) return;
            if (currentFieldItem.Selected == true)
            {
                string sValue = currentFieldItem.Tag.ToString();
                this.richTextExpression.Text = this.richTextExpression.Text + sValue +" " ;
            }           
        }

        //ֵ�б�˫���¼������ֶε�ֵ���뵽richTextExpression��
        private void listViewValue_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem currentValueItem = this.listViewValue.GetItemAt(e.Location.X, e.Location.Y);

            if (m_pCurrentLayer == null || currentValueItem == null) return;
            string sValue = currentValueItem.Text.Trim();
            if (sValue.Contains("[") && sValue.Contains("]"))
            {
                sValue = currentValueItem.Tag.ToString();
            }
            string sFieldName = this.listViewField.SelectedItems[0].Tag.ToString();
            if (sFieldName == "")
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ֶ���");
            }

            int iFieldIndex = m_pCurrentLayer.FeatureClass.Fields.FindField(sFieldName);
            IField pField = m_pCurrentLayer.FeatureClass.Fields.get_Field(iFieldIndex);

            //
            string tempValue = SysCommon.ModXZQ.GetCode(Plugin.ModuleCommon.TmpWorkSpace, sFieldName, sValue);
            if (pField.VarType > 1 && pField.VarType < 6)
            {
                this.richTextExpression.Text = this.richTextExpression.Text + tempValue + " ";
            }
            else
            {
                this.richTextExpression.Text = this.richTextExpression.Text + "'" + tempValue + "'";
            }
        }

        //ͼ��ѡ�� ���ܱ༭
        private void cmblayersel_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        //ѡ��ʽ ���ܱ༭
        private void cmbselmode_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        private bool DisplayUniqueValue(string strFieldName)
        {
            this.listViewValue.Items.Clear();
            if (!File.Exists(SysCommon.ModField._MatchFieldValuepath))
                return false;
            //��ȡ�����ļ�
            XmlDocument pXmldoc = new XmlDocument();
            pXmldoc.Load(SysCommon.ModField._MatchFieldValuepath);
            string strSearch = "//MatchFieldConfig/Field[@FieldName='"+strFieldName +"']";
            XmlNode pNode = pXmldoc.SelectSingleNode(strSearch);
            if (pNode == null)
                return false;
            string strTableName = pNode.Attributes["TableName"].Value.ToString();
            SysCommon.Gis.SysGisTable sysTable = new SysCommon.Gis.SysGisTable(m_Workspace );
            Exception exError = null;
            List<Dictionary<string, object>> lstDicData = sysTable.GetRows(strTableName, "", out exError);
            if (lstDicData == null)
                return false;
            try
            {
                if (lstDicData.Count > 0)
                {
                    for (int i = 0; i < lstDicData.Count; i++)
                    {
                        string strName = "";
                        string strAliasName = "";
                        if (lstDicData[i]["CODE"] != null)
                            strName = lstDicData[i]["CODE"].ToString();
                        if (lstDicData[i]["NAME"] != null)
                            strAliasName = lstDicData[i]["NAME"].ToString();
                        //����������������ӵ��ֵ���
                        if ((!strName.Equals("")) && (!strAliasName.Equals("")))
                        {
                            ListViewItem newItem = new ListViewItem(new string[] { strName+"["+strAliasName +"]" });
                            newItem.Tag = strName;
                            this.listViewValue.Items.Add(newItem);
                        }
                    }
                    return true;
                }
            }
            catch
            { }
            return false;
            
        }
        //��ʾ���ܵ�ֵ
        private void btnDisplayValue_Click(object sender, EventArgs e)
        {
            //�����ڵ�ǰ�� ���� ������ ѡ���ֶ�ʱ����
            if (m_pCurrentLayer == null || this.listViewField.SelectedItems.Count  == 0) return;

            string sFieldName = this.listViewField.SelectedItems[0].Tag.ToString();         //��ȡѡ������ַ���
            bool bRes=DisplayUniqueValue(sFieldName);
            if (bRes)
            {
                return;
            }
            //string sFieldName = listBox_Field.SelectedValue.ToString();                   //��ȡѡ������ַ���

            IFeatureClass pFeatureClass = m_pCurrentLayer.FeatureClass;                     //�õ�Ҫ�ؼ���
            if (pFeatureClass == null) return;

            try
            {

                //ע����ȷʹ��FeatureLayer��FeatureClass
                //IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);  //��������ѯ�õ�ȫ����Feature
                IFeatureCursor pFeatureCursor = m_pCurrentLayer.Search(null, false);
                //ygc 20130326 ��ӻ�ȡ�ֵ�����ȡΨһֵ
                List<string> listValue = new List<string>();                        //����ΨһҪ�� ֵ�ļ��� 
                listValue = SysCommon.ModXZQ.GetListChineseName(Plugin.ModuleCommon.TmpWorkSpace, sFieldName);
                if (listValue.Count != 0)
                {
                    for (int t = 0; t < listValue.Count; t++)
                    {
                        ListViewItem newItem = new ListViewItem(new string[] { listValue[t] });
                        this.listViewValue.Items.Add(newItem);
                    }
                }
                else
                {
                    IFeature pFeature = pFeatureCursor.NextFeature();                   //ȡ��Ҫ��ʵ��

                    this.listViewValue.Items.Clear();

                    while (pFeature != null)
                    {
                        int iFieldIndex = pFeature.Fields.FindField(sFieldName);            //�õ��ֶ�ֵ������
                        object objValue = pFeature.get_Value(iFieldIndex);

                        if (objValue != null)
                        {
                            string sValue = objValue.ToString();
                            if (!string.IsNullOrEmpty(sValue))
                            {
                                if (!listValue.Contains(sValue))
                                {
                                    if (listValue.Count > 200)                           //�Ƿ񳬹���������¼
                                    {
                                        MessageBox.Show("����ֵ�ļ�¼�Ѿ�����200���������ټ�����ʾ!", "��ʾ");
                                        break;
                                    }

                                    listValue.Add(sValue);
                                    ListViewItem newItem = new ListViewItem(new string[] { sValue });
                                    this.listViewValue.Items.Add(newItem);
                                }
                            }
                        }
                        pFeature = pFeatureCursor.NextFeature();
                    }
                }
                listViewValue.Sort();
                this.listViewValue.Update();                                         //����
            }
            catch (Exception ex)
            {
                MessageBox.Show("��ȡ�ֶ�ֵ�������󣬴���ԭ��Ϊ" + ex.Message, "��ʾ");
            }
        }

        //������ʽ
        private void btnClearExpression_Click(object sender, EventArgs e)
        {
            this.richTextExpression.ResetText();
        }
        //��֤���ʽ
        private void btnValidateExpression_Click(object sender, EventArgs e)
        {
            if (this.richTextExpression.Text.Trim() == "")
            {
                MessageBox.Show("���ʽΪ�գ���������ʽ��", "��ʾ");
                return;
            }
            string whereClause = this.richTextExpression.Text.Trim();

            CheckExpression(whereClause, true);
        }

        //���ؽ����
        private void btnLoadResults_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();

            dlgOpenFile.FilterIndex = 2;
            dlgOpenFile.Title = "��ѡ����Ҫ���صĽ����";
            dlgOpenFile.Filter = "All Files (*.*)|*.*|Text Files(*.exp)|*.exp";

            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                richTextExpression.LoadFile(dlgOpenFile.FileName);
            }
        }
        //��������
        private void btnSaveResults_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlgSaveFile = new SaveFileDialog();

            dlgSaveFile.FilterIndex = 2;
            dlgSaveFile.Title = "��ָ������������·��";
            dlgSaveFile.Filter = "All Files (*.*)|*.*|Text Files(*.exp)|*.exp";

            if (dlgSaveFile.ShowDialog() == DialogResult.OK)
            {
                richTextExpression.SaveFile(dlgSaveFile.FileName);
            }
        }
        //ʾ��
        private void btnSample_Click(object sender, EventArgs e)
        {
            //��յ�ǰ�ı��ʽ��
            this.richTextExpression.ResetText();

            if (m_pCurrentLayer == null) return;

            //�ֶο�����ֶ���
            if (this.listViewField.Items.Count > 1)
            {
                //��ȡ��ǰ��IFeatureClass,Ȼ��ȡ��Feature��ָ��
                IFeatureClass pFeatureClass = m_pCurrentLayer.FeatureClass;
                //ע����ȷʹ��FeatureLayer��FeatureClass
                //IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);

                for (int i = 0; i < this.listViewField.Items.Count; i++)
                {
                    IFeatureCursor pFeatureCursor = m_pCurrentLayer.Search(null, false);
                    //����ȡ��ÿһ��feature
                    IFeature pFeature = pFeatureCursor.NextFeature();
                    while (pFeature != null)
                    {
                        string sValue;
                        string sFieldName = this.listViewField.Items[i].Tag.ToString();
                        int iIndex = pFeatureClass.Fields.FindField(sFieldName);

                        IField pField = pFeatureClass.Fields.get_Field(iIndex);

                        //�������ͺ�bolb���͵�ֵ ��Ϊ"shape"
                        if (pField.Type == esriFieldType.esriFieldTypeGeometry || pField.Type == esriFieldType.esriFieldTypeBlob)
                        {
                            sValue = "shape";
                        }
                        else
                        {
                            //һֱѭ����ȡ����ȷ��ֵ
                            sValue = pFeature.get_Value(iIndex).ToString();
                            if (!string.IsNullOrEmpty(sValue))
                            {
                                this.richTextExpression.Text = sFieldName + " = " + "'" + sValue + "'";
                                if (CheckExpression(this.richTextExpression.Text.Trim(), false) == true) return;
                            }
                        }
                        pFeature = pFeatureCursor.NextFeature();
                    }
                }
            }
        }
        /// У����ʽ�Ƿ�Ϸ�
        private bool CheckExpression(string sExpression, bool bShow)
        {
            if (m_pCurrentLayer == null) return false;

            //��ȡ��ǰͼ��� featureclass
            IFeatureClass pFeatCls = m_pCurrentLayer.FeatureClass;
            //�����ѯ������
            IQueryFilter pFilter = new QueryFilterClass();

            try
            {
                //��ֵ��������
                pFilter.WhereClause = sExpression;
                //�õ���ѯ���
                //ע����ȷʹ��FeatureLayer��FeatureClass
                //IFeatureCursor pFeatCursor = pFeatCls.Search(pFilter, false);
                IFeatureCursor pFeatCursor = m_pCurrentLayer.Search(pFilter, false);
                //ȡ�õ�һ��feature
                IFeature pFeat = pFeatCursor.NextFeature();
                if (pFeat != null)
                {
                    if (bShow == true)
                    {
                        MessageBox.Show("���ʽ��ȷ!", "��ʾ");
                    }
                    return true;
                }
                else
                {
                    if (bShow == true)
                    {
                        MessageBox.Show("�˱��ʽ��������Ҫ��,������ʽ", "��ʾ");
                    }
                    return false;
                }
            }
            catch
            {
                if (bShow == true)
                {
                    MessageBox.Show("�˱��ʽ��������Ҫ��,������ʽ", "��ʾ");
                }
                return false;
            }
        }
       //�˳���
        private void FrmSQLQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Dispose(true);
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
           //�ӽ����� xisheng 2011.06.28
            SysCommon.CProgress vProgress = new SysCommon.CProgress("������");
            vProgress.EnableCancel = true;
            vProgress.EnableUserCancel(true);


            vProgress.ShowDescription = true;
            vProgress.FakeProgress = true;
            vProgress.TopMost = true;
            vProgress.ShowProgress();
            vProgress.SetProgress("��ʼ��ѯ");

            bool bRes = false;  //�Ƿ��ѯ������
            this.DialogResult = DialogResult.OK;
           
            //û�е�ǰͼ��ֱ���˳�
            if (m_pCurrentLayer == null)
            {
                vProgress.Close();// ����  20110705  ���
                return;
            }
            try
            {
                string whereClause = this.richTextExpression.Text.Trim();
                if (CheckExpression(whereClause, false) == false)   //changed by chulili 20120818 ������ʾ��
                {
                    vProgress.Close();// ����  20110705  ���
                    MessageBox.Show("�˱��ʽ��������Ҫ��,������ʽ", "��ʾ");
                    return;

                }
                //��ȡ��ǰͼ��� featureclass
                vProgress.SetProgress("��ȡ��ǰͼ��");
                IFeatureClass pFeatClass = m_pCurrentLayer.FeatureClass;

                //�����ѯ������
                IQueryFilter pQueryFilter = new QueryFilterClass();
                //��ֵ��������
                vProgress.SetProgress("�����ѯ����������ֵ��ѯ����");
                pQueryFilter.WhereClause = whereClause;

                //��ֵ��ѯ��ʽ,�ɲ�ѯ��ʽ��combo���
                esriSelectionResultEnum pSelectionResult = esriSelectionResultEnum.esriSelectionResultNew;
                bool bSelection = true;
                switch (this.cmbselmode.SelectedItem.ToString())
                {
                    case ("������ѡ����"):
                        bSelection = false;
                        break;
                    case ("����һ���µ�ѡ����"):
                        pSelectionResult = esriSelectionResultEnum.esriSelectionResultNew;
                        break;
                    case ("��ӵ���ǰѡ����"):
                        pSelectionResult = esriSelectionResultEnum.esriSelectionResultAdd;
                        break;
                    case ("�ӵ�ǰѡ�������Ƴ�"):
                        pSelectionResult = esriSelectionResultEnum.esriSelectionResultSubtract;
                        break;
                    case ("�ӵ�ǰѡ������ѡ��"):
                        pSelectionResult = esriSelectionResultEnum.esriSelectionResultAnd;
                        break;
                    default:
                        vProgress.Close();// ����  20110705  ���
                        return;
                }


                //���в�ѯ�����������ʾ����
                vProgress.SetProgress("���ڲ�ѯ���������Ľ��");
                //frmQuery frm = new frmQuery(m_MapControlDefault);
                //frm.FillData(m_pCurrentLayer, pQueryFilter, pSelectionResult);
                _QueryBar.m_pMapControl = m_MapControlDefault;
                if (!bSelection)
                {
                    _QueryBar.EmergeQueryData(m_pCurrentLayer, pQueryFilter, vProgress);
                }
                else
                {
                    _QueryBar.EmergeQueryData(m_MapControlDefault.Map, m_pCurrentLayer, pQueryFilter, pSelectionResult, vProgress);
                }
                try
                {
                    DevComponents.DotNetBar.Bar pBar = _QueryBar.Parent.Parent as DevComponents.DotNetBar.Bar;
                    if (pBar != null)
                    {
                        pBar.AutoHide = false;
                        int tmpindex = pBar.Items.IndexOf("dockItemDataCheck");
                        pBar.SelectedDockTab = tmpindex;
                    }
                }
                catch
                { }
                bRes = true;
            }
            //frm.Show();
            catch
            { }
            finally
            {
                vProgress.Close();
                if (bRes)
                {
                    this.Hide();
                    this.Dispose(true);
                }
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Dispose(true);
        }
        //����Click�¼�����Ŀ¼ѡ��ͼ�� xisheng 20111119
        private void cmblayersel_Click(object sender, EventArgs e)
        {            
            //this.Width = this.advTreeLayerList.Left + this.advTreeLayerList.Width + 5;
            //this.advTreeLayers.Location = new Point(this.cmblayersel.Location.X+this.groupPanel1.Location.X , this.cmblayersel.Location.Y+this.groupPanel1.Location.Y);
            this.advTreeLayers.Width = this.cmblayersel.Width;
            this.advTreeLayers.Visible = true;
            this.advTreeLayers.Focus();

        }
        //����ѡ��ͼ��ʹͼ�����ı�ʱ xs 20111119
        private void cmblayersel_TextChanged(object sender, EventArgs e)
        {
            if (m_pCurrentLayer == null||m_pCurrentLayer.FeatureClass==null)
                return;
            this.listViewField.Items.Clear();                             //��Ϊ�ֶ�ֵ����ͼ����ı䣬���
            this.listViewValue.Items.Clear();

            IFeatureClass pFeatureClass = m_pCurrentLayer.FeatureClass; //��ȡҪ����ļ���

            //ѭ���õ�ÿһ���ֶβ������ֶ����ͽ��ж�Ӧ�Ĳ���
            //�˴��Ժ�Ҫ�������Ӹ����жϺͶ�Ӧ����
            for (int iIndex = 0; iIndex < pFeatureClass.Fields.FieldCount; iIndex++)
            {
                IField pField = pFeatureClass.Fields.get_Field(iIndex);
                switch (pField.Type)
                {
                    case esriFieldType.esriFieldTypeSmallInteger:
                    case esriFieldType.esriFieldTypeInteger:
                    case esriFieldType.esriFieldTypeSingle:
                    case esriFieldType.esriFieldTypeDouble:
                    case esriFieldType.esriFieldTypeString:
                        ListViewItem newItem = new ListViewItem(new string[] { pField.AliasName + "��" + SysCommon.ModField.GetChineseNameOfField(pField.AliasName) + "��" });
                        newItem.Tag = pField.Name;
                        //////�����ֶ���������ʾ����
                        newItem.ToolTipText = SysCommon.ModField.GetChineseNameOfField(pField.AliasName);
                        this.listViewField.Items.Add(newItem);
                        break;
                    default:
                        break;
                }
            }
        }
        private void btnSaveCondition_Click(object sender, EventArgs e)
        {
            FrmSaveSQLSolution frmSave = new FrmSaveSQLSolution(m_Workspace);
            frmSave.m_TableName = "SQLSOLUTION";
            frmSave.m_Condition = richTextExpression.Text;
            if (LayerID == "" || LayerID == null)
            {
                MessageBox.Show("��ѡ��Ҫ��ѯ��ͼ�㣡","��ʾ");
                return;
            }
            if (richTextExpression.Text == "")
            {
                MessageBox.Show("�������ѯ������","��ʾ");
                return;
            }
            frmSave.m_LayerID = LayerID;
            frmSave.m_layerName = m_pCurrentLayer.Name;
            frmSave.ShowDialog();
        }
        private void btnOpenConditon_Click(object sender, EventArgs e)
        {
            if (LayerID == "" || LayerID == null)
            {
                MessageBox.Show("��ѡ���ѯ��ͼ�����ݣ�","��ʾ");
                return;
            }
            if (m_Workspace == null) return;
            FrmOpenSQLCondition frmOpen = new FrmOpenSQLCondition(m_Workspace);
            frmOpen.m_TableName = "SQLSOLUTION";
            frmOpen.m_LayerId = LayerID;
            if (frmOpen.ShowDialog () == DialogResult.OK)
            {
                richTextExpression.Text = "";
                richTextExpression.Text = frmOpen.m_Condition;
            }
        }

        //���������ļ���ʾͼ����
        private void InitLayerTreeByXmlNode(DevComponents.AdvTree.Node treenode, XmlNode xmlnode)
        {

            for (int iChildIndex = 0; iChildIndex < xmlnode.ChildNodes.Count; iChildIndex++)
            {
                XmlElement xmlElementChild = xmlnode.ChildNodes[iChildIndex] as XmlElement;
                if (xmlElementChild == null)
                {
                    continue;
                }
                else if (xmlElementChild.Name == "ConfigInfo")
                {
                    continue;
                }
                //��Xml�ӽڵ��"NodeKey"��"NodeText"�������������ӽڵ�
                string sNodeKey = xmlElementChild.GetAttribute("NodeKey");
                if (!Plugin.ModuleCommon.ListUserdataPriID.Contains(sNodeKey))
                {
                    continue;
                }
                string sNodeText = xmlElementChild.GetAttribute("NodeText");

                DevComponents.AdvTree.Node treenodeChild = new DevComponents.AdvTree.Node();
                treenodeChild.Name = sNodeKey;
                treenodeChild.Text = sNodeText;

                treenodeChild.DataKey = xmlElementChild;
                treenodeChild.Tag = xmlElementChild.Name;


                treenode.Nodes.Add(treenodeChild);

                //�ݹ�
                if (xmlElementChild.Name != "Layer")
                {
                    InitLayerTreeByXmlNode(treenodeChild, xmlElementChild as XmlNode);
                }

                InitializeNodeImage(treenodeChild);
            }

        }
        /// <summary>
        /// ͨ������ڵ��tag��ѡ���Ӧ��ͼ��        
        /// </summary>
        /// <param name="treenode"></param>
        private void InitializeNodeImage(DevComponents.AdvTree.Node treenode)
        {
            switch (treenode.Tag.ToString())
            {
                case "Root":
                    treenode.Image = this.ImageList.Images["Root"];
                    treenode.CheckBoxVisible = false;
                    break;
                case "SDE":
                    treenode.Image = this.ImageList.Images["SDE"];
                    break;
                case "PDB":
                    treenode.Image = this.ImageList.Images["PDB"];
                    break;
                case "FD":
                    treenode.Image = this.ImageList.Images["FD"];
                    break;
                case "FC":
                    treenode.Image = this.ImageList.Images["FC"];
                    break;
                case "TA":
                    treenode.Image = this.ImageList.Images["TA"];
                    break;
                case "DIR":
                    treenode.Image = this.ImageList.Images["DIR"];
                    //treenode.CheckBoxVisible = false;
                    break;
                case "DataDIR":
                    treenode.Image = this.ImageList.Images["DataDIRHalfOpen"];
                    break;
                case "DataDIR&AllOpened":
                    treenode.Image = this.ImageList.Images["DataDIROpen"];
                    break;
                case "DataDIR&Closed":
                    treenode.Image = this.ImageList.Images["DataDIRClosed"];
                    break;
                case "DataDIR&HalfOpened":
                    treenode.Image = this.ImageList.Images["DataDIRHalfOpen"];
                    break;
                case "Layer":
                    XmlNode xmlnodeChild = (XmlNode)treenode.DataKey;
                    if (xmlnodeChild != null && xmlnodeChild.Attributes["FeatureType"] != null)
                    {
                        string strFeatureType = xmlnodeChild.Attributes["FeatureType"].Value;

                        switch (strFeatureType)
                        {
                            case "esriGeometryPoint":
                                treenode.Image = this.ImageList.Images["_point"];
                                break;
                            case "esriGeometryPolyline":
                                treenode.Image = this.ImageList.Images["_line"];
                                break;
                            case "esriGeometryPolygon":
                                treenode.Image = this.ImageList.Images["_polygon"];
                                break;
                            case "esriFTAnnotation":
                                treenode.Image = this.ImageList.Images["_annotation"];
                                break;
                            case "esriFTDimension":
                                treenode.Image = this.ImageList.Images["_Dimension"];
                                break;
                            case "esriGeometryMultiPatch":
                                treenode.Image = this.ImageList.Images["_MultiPatch"];
                                break;
                            default:
                                treenode.Image = this.ImageList.Images["Layer"];
                                break;
                        }
                    }
                    else
                    {
                        treenode.Image = this.ImageList.Images["Layer"];
                    }
                    break;
                case "RC":
                    treenode.Image = this.ImageList.Images["RC"];
                    break;
                case "RD":
                    treenode.Image = this.ImageList.Images["RD"];
                    break;
                case "SubType":
                    treenode.Image = this.ImageList.Images["SubType"];
                    break;
                default:
                    break;
            }//end switch
        }

        private void advTreeLayerList_Click(object sender, EventArgs e)
        {

        }
        //ͨ��NODE �õ�NODYKEY
        private string GetNodeKey(DevComponents.AdvTree.Node Node)
        {
           // labelErr.Text = "";
            XmlNode xmlnode = (XmlNode)Node.DataKey;
            XmlElement xmlelement = xmlnode as XmlElement;
            string strDataType = "";
            if (xmlelement.HasAttribute("DataType"))
            {
                strDataType = xmlnode.Attributes["DataType"].Value;
            }
            if (strDataType == "RD" || strDataType == "RC")//��Ӱ������ ����
            {
               // labelErr.Text = "��ѡ��ʸ�����ݽ��в���!";
                return "";
            }
                if (xmlelement.HasAttribute("IsQuery"))
                {
                    if (xmlelement["IsQuery"].Value == "False")
                    {
                       // labelErr.Text = "��ͼ�㲻�ɲ�ѯ!";
                        return "";
                    }
                }
            if (xmlelement.HasAttribute("NodeKey"))
            {
                return xmlelement.GetAttribute("NodeKey");
              
            }
            return "";

        }

        private void btnLayerOK_Click(object sender, EventArgs e)
        {
            DealSelectNode();
        }

        private void btnLayerCancel_Click(object sender, EventArgs e)
        {
            this.Width = this.advTreeLayerList.Left+2;
        }
        private void DealSelectNode()
        {
            LayerID = "";
            if (advTreeLayerList.SelectedNode == null)
                return;
            if (advTreeLayerList.SelectedNode.Tag.ToString() != "Layer")//����Ҷ�ӽڵ� ����
            {
                return;
            }

            GetNodeKey(advTreeLayerList.SelectedNode);
            if (string.IsNullOrEmpty(LayerID))
                return;
            m_pCurrentLayer = new FeatureLayerClass();
            IFeatureClass pClass=SysCommon.ModSysSetting.GetFeatureClassByNodeKey(Plugin.ModuleCommon.TmpWorkSpace, _LayerTreePath, LayerID);
            if (pClass != null)
            {
                m_pCurrentLayer.FeatureClass = pClass;
                cmblayersel.Text = advTreeLayerList.SelectedNode.Text;
                m_pCurrentLayer.Name = advTreeLayerList.SelectedNode.Text;// xisheng 20111122 �Զ����ѯ������BUG�޸�

            }
            this.Width = this.advTreeLayerList.Left + 7;
        }
        private void DealSelectNodeEX()
        {
            LayerID = "";
            if (this.advTreeLayers.SelectedNode == null)
                return;
            if (advTreeLayers.SelectedNode.Tag.ToString() != "Layer")//����Ҷ�ӽڵ� ����
            {
                return;
            }

            LayerID=GetNodeKey(advTreeLayers.SelectedNode);
            if (string.IsNullOrEmpty(LayerID))
                return;

            this.advTreeLayers.Visible = false;

            m_pCurrentLayer = new FeatureLayerClass();
            IFeatureClass pClass = SysCommon.ModSysSetting.GetFeatureClassByNodeKey(Plugin.ModuleCommon.TmpWorkSpace, _LayerTreePath, LayerID);
            if (pClass != null)
            {
                m_pCurrentLayer.FeatureClass = pClass;
                cmblayersel.Text = advTreeLayers.SelectedNode.Text;
                m_pCurrentLayer.Name = advTreeLayers.SelectedNode.Text;// xisheng 20111122 �Զ����ѯ������BUG�޸�

            }
        }
        private void advTreeLayerList_NodeDoubleClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            DealSelectNode();
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            groupPanelFewer.Visible = false;
        }

        private void btnEqual2_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnEqual);
        }

        private void advTreeLayers_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            DealSelectNodeEX();
        }
        #region  ѡ��ͼ�����ͼʧȥ����
        private void FrmSQLQuery_MouseClick(object sender, MouseEventArgs e)
        {
            this.advTreeLayers.Visible = false;
        }

        private void advTreeLayers_Leave(object sender, EventArgs e)
        {
            this.advTreeLayers.Visible = false;
        }
        private void groupPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            this.advTreeLayers.Visible = false;
        }

        private void groupPanel2_MouseClick(object sender, MouseEventArgs e)
        {
            this.advTreeLayers.Visible = false;
        }

        private void groupPanelFewer_MouseClick(object sender, MouseEventArgs e)
        {
            this.advTreeLayers.Visible = false;
        }

        private void groupPanel4_MouseClick(object sender, MouseEventArgs e)
        {
            this.advTreeLayers.Visible = false;
        }

        private void groupPanel5_MouseClick(object sender, MouseEventArgs e)
        {
            this.advTreeLayers.Visible = false;
        }      
        private void labelX1_Click(object sender, EventArgs e)
        {
            this.advTreeLayers.Visible = false;
        }

        private void labelX2_Click(object sender, EventArgs e)
        {
            this.advTreeLayers.Visible = false;
        }

        private void groupPanel1_Click(object sender, EventArgs e)
        {
            this.advTreeLayers.Visible = false;
        }
        #endregion
    }
}