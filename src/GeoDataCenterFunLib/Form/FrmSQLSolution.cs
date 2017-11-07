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
    public partial class FrmSQLSolution : DevComponents.DotNetBar.Office2007Form
    {
        private IMapControlDefault m_MapControlDefault;
        private IMap m_pMap;
        private IFeatureLayer m_pCurrentLayer;
        ///���ҵ������ڳ�ʼ�������ֵ�   20111011  Zq add
        private IWorkspace m_Workspace = null;
        private IDictionary<string, string> _DicSolution = null;
        private IDictionary<string, string> _DicDescription = null;
        public SysCommon.BottomQueryBar _QueryBar
        {
            get;
            set;
        }
        private string LayerID;
        string strConfigPath = Application.StartupPath + "\\..\\res\\xml\\�ְ�Ų�ѯ.xml";
        string strLayerTreePath = Application.StartupPath + "\\..\\res\\xml\\��ʱͼ����0.xml";   
        public FrmSQLSolution(IMapControlDefault pMapControl, IWorkspace pWorkspace)
        {
            m_MapControlDefault = pMapControl;
            m_pMap = pMapControl.Map;
            ///��ʼ�������ֵ�  20111011  Zq add
            m_Workspace = pWorkspace;

            InitializeComponent();
        }
        private void DropTable(string TableName, IWorkspace pW)
        {
            try
            {
                pW.ExecuteSQL("Drop table " + TableName);
            }
            catch
            { }
        }
        private void InitSqlSolution()
        {
            cmboxSqlSolution.Items.Clear();
            IFeatureWorkspace pFeawks = m_Workspace as IFeatureWorkspace;
            DropTable("TempTable0", m_Workspace);

            try
            {

                m_Workspace.ExecuteSQL("create table TempTable0 as select Layerid,id,longinuser,solutionname,description,condition from  SQLSOLUTION  where (longinuser='" + Plugin.ModuleCommon.AppUser.Name + "' or isshare=1) and layerid='" + LayerID + "'");
                if (_DicSolution == null)
                {
                    _DicSolution = new Dictionary<string, string>();
                }
                if (_DicDescription == null)
                {
                    _DicDescription = new Dictionary<string, string>();
                }
                ITable pTable = pFeawks.OpenTable("TempTable0");
                int indexField = pTable.Fields.FindField("solutionname");
                int indexCondition = pTable.Fields.FindField("condition");
                int indexDescription = pTable.Fields.FindField("description");
                ICursor pCursor = pTable.Search(null, false);
                IRow pRow = pCursor.NextRow();
                while (pRow != null)
                {
                    if (pRow.get_Value(indexField) != null)
                    {
                        string strName=pRow.get_Value(indexField).ToString();
                        cmboxSqlSolution.Items.Add(strName);
                        string strCondition=pRow.get_Value(indexCondition).ToString();
                        string strDescription = pRow.get_Value(indexDescription).ToString();
                        if (!_DicSolution.ContainsKey(strName))
                        {
                            _DicSolution.Add(strName, strCondition);
                        }
                        if (!_DicDescription.ContainsKey(strName))
                        {
                            _DicDescription.Add(strName, strDescription);
                        }
                    }
                    pRow = pCursor.NextRow();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
                pCursor = null;
                DropTable("TempTable0", m_Workspace);
            }
            catch
            { }
            cmboxSqlSolution.Text = "ѡ���ѯ����";

        }

        private void FrmSQLQuery_Load(object sender, EventArgs e)
        {
            this.richTextExpression.Text = "";
            SysCommon.ModSysSetting.CopyLayerTreeXmlFromDataBase(Plugin.ModuleCommon.TmpWorkSpace, strLayerTreePath);
            SysCommon.ModSysSetting.CopyConfigXml(Plugin.ModuleCommon.TmpWorkSpace, "����ְߺ�", strConfigPath);
            XmlDocument pXmlDoc = new XmlDocument();
            pXmlDoc.Load(strConfigPath);
            string strSearch = "//QueryConfig/QueryItem[@ItemText=" + "'����ְߺŲ�ѯ'" + "]";
            XmlNode pNode = pXmlDoc.SelectSingleNode(strSearch);
            if (pNode == null)
            {
                return;
            }
            XmlNodeList pNodeList = pNode.SelectNodes(".//LayerItem");
            string LinBanLayerKey = "";
            string LinBanNodetxt = "";
            if (pNodeList.Count > 0)
            {
                XmlNode pXZnode = pNodeList[0];
                LinBanLayerKey = pXZnode.Attributes["NodeKey"].Value;//�ְ�ͼ����
                LinBanNodetxt = pXZnode.Attributes["NodeText"].Value;
            }
            if (LinBanLayerKey != "")
            {
                m_pCurrentLayer = new FeatureLayerClass();
                m_pCurrentLayer.FeatureClass = SysCommon.ModSysSetting.GetFeatureClassByNodeKey(Plugin.ModuleCommon.TmpWorkSpace, strLayerTreePath, LinBanLayerKey);
                LayerID = LinBanLayerKey;
            }

            if (m_pCurrentLayer.FeatureClass != null)
            {
                cmblayersel.Text = LinBanNodetxt;
                m_pCurrentLayer.Name = LinBanNodetxt;// xisheng 20111122 �Զ����ѯ������BUG�޸�
            }


            //ygc ��ʼ��ͼ����
            if (SysCommon.ModField._DicFieldName.Keys.Count == 0)
            {
                SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");
            }
            string LayerTreePath = System.Windows.Forms.Application.StartupPath + "\\..\\res\\xml\\��ѯͼ����tmp.xml"; //ͼ��Ŀ¼�ļ�·��
            //��ʼ��ͼ�����б�
            if (File.Exists(strLayerTreePath))
            {

                XmlDocument LayerTreeXmldoc = new XmlDocument();

                LayerTreeXmldoc.Load(strLayerTreePath);
                advTreeLayers.Nodes.Clear();

                //��ȡXml�ĸ��ڵ㲢��Ϊ���ڵ�ӵ�UltraTree��
                XmlNode xmlnodeRoot = LayerTreeXmldoc.DocumentElement;
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
                LayerTreeXmldoc = null;
            }
            try
            {
                System.IO.File.Delete(LayerTreePath);
            }
            catch
            { }

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

                //��ȡ��ǰͼ��� featureclass
                vProgress.SetProgress("��ȡ��ǰͼ��");
                IFeatureClass pFeatClass = m_pCurrentLayer.FeatureClass;

                //�����ѯ������
                IQueryFilter pQueryFilter = new QueryFilterClass();
                //��ֵ��������
                vProgress.SetProgress("�����ѯ����������ֵ��ѯ����");
                pQueryFilter.WhereClause = whereClause;

                //��ֵ��ѯ��ʽ,�ɲ�ѯ��ʽ��combo���
                esriSelectionResultEnum pSelectionResult;
                pSelectionResult = esriSelectionResultEnum.esriSelectionResultNew;

                //���в�ѯ�����������ʾ����
                vProgress.SetProgress("���ڲ�ѯ���������Ľ��");
                //frmQuery frm = new frmQuery(m_MapControlDefault);
                //frm.FillData(m_pCurrentLayer, pQueryFilter, pSelectionResult);
                _QueryBar.m_pMapControl = m_MapControlDefault;
                _QueryBar.EmergeQueryData(m_MapControlDefault.Map, m_pCurrentLayer, pQueryFilter, pSelectionResult,vProgress);
                try
                {
                    DevComponents.DotNetBar.Bar pBar = _QueryBar.Parent.Parent as DevComponents.DotNetBar.Bar;
                    if (pBar != null)
                    {
                        pBar.AutoHide = false;
                        //pBar.SelectedDockTab = 1;
                        int tmpindex = pBar.Items.IndexOf("dockItemDataCheck");
                        pBar.SelectedDockTab = tmpindex;
                    }
                }
                catch
                { }
            }
            //frm.Show();
            catch
            { }
            finally
            {
                vProgress.Close();
                this.Hide();
                this.Dispose(true);
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
            //Plugin.SelectLayerByTree frm = new Plugin.SelectLayerByTree(1);
            //SysCommon.ModSysSetting.CopyLayerTreeXmlFromDataBase(Plugin.ModuleCommon.TmpWorkSpace, frm._LayerTreePath);
            // if (frm.ShowDialog() == DialogResult.OK)
            //{
            //    if (frm.m_NodeKey.Trim() != "")
            //    {
            //        m_pCurrentLayer=new FeatureLayerClass();
            //         m_pCurrentLayer.FeatureClass = SysCommon.ModSysSetting.GetFeatureClassByNodeKey(Plugin.ModuleCommon.TmpWorkSpace, frm._LayerTreePath, frm.m_NodeKey);
            //         LayerID = frm.m_NodeKey;
            //    }

            //    if (m_pCurrentLayer.FeatureClass != null)
            //    {

            //        cmblayersel.Text = frm.m_NodeText;
            //        m_pCurrentLayer.Name = frm.m_NodeText;// xisheng 20111122 �Զ����ѯ������BUG�޸�
            //    }
            //}

            this.advTreeLayers.Width = this.cmblayersel.Width;
            this.advTreeLayers.Visible = true;
            this.advTreeLayers.Focus();
        }
        //����ѡ��ͼ��ʹͼ�����ı�ʱ xs 20111119
        private void cmblayersel_TextChanged(object sender, EventArgs e)
        {
            if (m_pCurrentLayer == null||m_pCurrentLayer.FeatureClass==null)
                return;

            IFeatureClass pFeatureClass = m_pCurrentLayer.FeatureClass; //��ȡҪ����ļ���

            //added by chulili 2012-10-23 ��ʼ��ͼ��Ĳ�ѯ��������ղ�ѯ����
            InitSqlSolution();
            richTextExpression.Text = "";
        }


        private void cmboxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strname = cmboxSqlSolution.SelectedItem.ToString();
            if (_DicSolution.ContainsKey(strname))
            {
                richTextExpression.Text = _DicSolution[strname];
            }
            if (_DicDescription.ContainsKey(strname))
            {
                labelDescription.Text=_DicDescription[strname];
            }
        }

        private void FrmSQLQuery_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_DicSolution != null)
            {
                _DicSolution.Clear();
                _DicSolution = null;
            }
            if (_DicDescription != null)
            {
                _DicDescription.Clear();
                _DicDescription = null;
            }
        }

        private void advTreeLayers_Leave(object sender, EventArgs e)
        {
            this.advTreeLayers.Visible = false;
        }

        private void FrmSQLSolution_Click(object sender, EventArgs e)
        {
            this.advTreeLayers.Visible = false;
        }

        private void labelDescription_Click(object sender, EventArgs e)
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

        private void labelX3_Click(object sender, EventArgs e)
        {
            this.advTreeLayers.Visible = false;
        }

        private void advTreeLayers_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            DealSelectNodeEX();
        }
        private void DealSelectNodeEX()
        {
            LayerID = "";
            if (this.advTreeLayers.SelectedNode == null || advTreeLayers.SelectedNode.Tag==null)
                return;
            if (advTreeLayers.SelectedNode.Tag.ToString() != "Layer")//����Ҷ�ӽڵ� ����
            {
                return;
            }

            LayerID = GetNodeKey(advTreeLayers.SelectedNode);
            if (string.IsNullOrEmpty(LayerID))
                return;

            this.advTreeLayers.Visible = false;

            m_pCurrentLayer = new FeatureLayerClass();
            IFeatureClass pClass = SysCommon.ModSysSetting.GetFeatureClassByNodeKey(Plugin.ModuleCommon.TmpWorkSpace, strLayerTreePath, LayerID);
            if (pClass != null)
            {
                m_pCurrentLayer.FeatureClass = pClass;
                cmblayersel.Text = advTreeLayers.SelectedNode.Text;
                m_pCurrentLayer.Name = advTreeLayers.SelectedNode.Text;// xisheng 20111122 �Զ����ѯ������BUG�޸�

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


    }
}