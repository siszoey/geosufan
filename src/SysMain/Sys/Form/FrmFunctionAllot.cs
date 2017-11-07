using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Collections;
using ESRI.ArcGIS.esriSystem;
using System.Data.OracleClient;
using System.Data.OleDb;

namespace GeoDatabaseManager
{
    public partial class FrmFunctionAllot : DevComponents.DotNetBar.Office2007Form
    {
        Dictionary<string,int> dicUserGroup = new Dictionary<string,int>();//�û�����Ϣ��id��Name��
        XmlDocument docXml = new XmlDocument();
        XmlNode SysNode = null;
        private string pDbType = "";//���ݿ�����
       private string pConStr = "";//���ݿ������ַ���


        public FrmFunctionAllot()
        {
            InitializeComponent();
            GetConInfo();
            if(loadRoleTable()==false ) return ;
            this.comboBoxRole.SelectedIndex = 0;

            //��ȡϵͳȨ��XML
            if (!File.Exists(Mod.m_SysXmlPath)) return;
            docXml.Load(Mod.m_SysXmlPath);
            SysNode = docXml.SelectSingleNode(".//Root//Main//System");

            ReadXMl();
        }

        /// <summary>
        /// ������ݿ����ӵĲ���
        /// </summary>
        private void GetConInfo()
        {
            StreamReader sr = new StreamReader(Mod.filestr);
            pDbType = sr.ReadLine();
            pConStr = sr.ReadLine();
        }

        /// <summary>
        /// �������ݿ�
        /// </summary>
        /// <param name="pSysTable">Ҫ���ӵ����ݿ�</param>
        /// <param name="DBtype">���ݿ�����</param>
        /// <param name="strConnectionString">���ݿ������ַ���</param>
       private void  SetDbCon(SysCommon.DataBase.SysTable pSysTable, string DBtype, string strConnectionString, out Exception eError)
        {
            eError = null;
            if (strConnectionString == "")
                return;
            try
            {
                if (DBtype.Trim() == "ACCESS")
                {
                    pSysTable.SetDbConnection(strConnectionString+";Mode=Share Deny None;Persist Security Info=False", SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out eError);
                }
                else if (DBtype.Trim() == "ORACLE")
                {
                    pSysTable.SetDbConnection(strConnectionString, SysCommon.enumDBConType.ORACLE, SysCommon.enumDBType.ORACLE, out eError);
                }
                else if (DBtype.Trim() == "SQL")
                {
                    pSysTable.SetDbConnection(strConnectionString, SysCommon.enumDBConType.SQL, SysCommon.enumDBType.SQLSERVER, out eError);
                }
            }
            catch (Exception ex)
            {
                eError = ex;
                return;
            }
        }
        //���ؽ�ɫȨ�ޱ�
        private bool loadRoleTable()
        {
            Exception eError = null;

             SysCommon.DataBase.SysTable pSysTable = new SysCommon.DataBase.SysTable();
             SetDbCon(pSysTable, pDbType ,pConStr, out eError);
            if (eError != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��Ϣ��ʾ", eError.Message);
                return false ;
            }
            DataTable tempTable = pSysTable.GetTable("ice_UserGroupinfo", out eError);
            if (tempTable == null||tempTable.Rows.Count==0) return false ;
            for (int i = 0; i < tempTable.Rows.Count; i++)
            {
                int pUserGroupID = int.Parse(tempTable.Rows[i][0].ToString());//��ID
                string pUserGroupName = tempTable.Rows[i][1].ToString();//������
                dicUserGroup.Add(pUserGroupName,pUserGroupID);
                this.comboBoxRole.Items.Add(pUserGroupName);
            }
            return true;
        }

        private void ReadXMl()
        {
            ReadMainXMl(SysNode);
            ReadToolBarXML(SysNode);
            ReadContextMenuXML(SysNode);
        }
        /// <summary>
        /// ��XML�ж�ȡ���˵��ڵ�
        /// </summary>
        /// <param name="SysNode"></param>
        private void ReadMainXMl(XmlNode SysNode)
        {
            this.node1.Text = "���˵�";//node1��ʾ������
            this.node1.Expanded = true;//node����չ����
            this.node1.CheckBoxVisible = false;//node1�Ƿ���ʾcheckBox
            this.node1.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
            this.node1.FullRowBackground = true;
            //this.advTreeMainPurview.Nodes.AddRange(new DevComponents.AdvTree.Node[] { this.node1 });//��node1��ӵ�����
            #region ͨ����ȡxml�ļ���̬�Ľ����˵��ڵ���ӵ�node1����
            XmlNodeList mainFuncNodeList = SysNode.SelectNodes(".//RibbonTabItem");//��õ�һ���ڵ㼯��
            XmlElement nodeToElem = null;
            string pCaption = "";//Caption����ֵ
            string pName = "";//Name����ֵ

            #region ������˵��ڵ��Լ��ӽڵ�
            //����xml�ļ��е�һ���ӽڵ㣬���������Ϣ���ӵ��������ڵ���
            foreach (XmlNode oneFunc in mainFuncNodeList)
            {
                //��ӵ�һ���ӽڵ�
                //RibbonTabItem�ӽڵ�
                nodeToElem = oneFunc as XmlElement;//���ڵ�ת��ΪԪ�����ͣ��Ա��ȡ�ڵ������
                pCaption = nodeToElem.GetAttribute("Caption").Trim();//�ڵ��Caption����
                pName = nodeToElem.GetAttribute("Name").Trim();//�ڵ��Name����
                DevComponents.AdvTree.Node nodeFunc = new DevComponents.AdvTree.Node();//����һ��node����
                //��node���������Ը�ֵ
                nodeFunc.Text = pCaption + "," + pName;//��xml�ж����������Ը�����node��Text����
                nodeFunc.CheckBoxVisible = true;
                nodeFunc.Checked = false;
                nodeFunc.Expanded = true;
                nodeFunc.NodeClick += new System.EventHandler(TreeNodeClick);//����node��nodeclick�¼�
                this.node1.Nodes.AddRange(new DevComponents.AdvTree.Node[] { nodeFunc });//����node������node1�ڵ����
                XmlNodeList nodeRibbonBar = oneFunc.SelectNodes(".//RibbonBar");//��õڶ����ڵ㼯��
                //����xml�ļ��еڶ����ӽڵ㣬���������Ϣ���ӵ��������ڵ���
                foreach (XmlNode oneNodeRibbon in nodeRibbonBar)
                {
                    //��ӵڶ����ӽڵ㣬ͬ��
                    //RibbonBar�ӽڵ�
                    nodeToElem = oneNodeRibbon as XmlElement;
                    pCaption = nodeToElem.GetAttribute("Caption").Trim();
                    pName = nodeToElem.GetAttribute("Name").Trim();
                    DevComponents.AdvTree.Node subNodeRibb = new DevComponents.AdvTree.Node();//����һ��node����
                    //��node���������Ը�ֵ
                    subNodeRibb.Text = pCaption + "," + pName;
                    subNodeRibb.CheckBoxVisible = true;
                    subNodeRibb.Checked = false;
                    subNodeRibb.Expanded = true;
                    subNodeRibb.NodeClick += new System.EventHandler(TreeNodeClick);//nodeclick�¼�
                    nodeFunc.Nodes.AddRange(new DevComponents.AdvTree.Node[] { subNodeRibb });//���ýڵ㸽���ڵ�һ���ڵ����
                    XmlNodeList nodeSubItemList = oneNodeRibbon.SelectNodes(".//SubItems");//��xml�ļ��л�ȡ�������ڵ㼯��
                    //����xml�ļ��е������ӽڵ㣬���������Ϣ���ӵ��������ڵ���
                    foreach (XmlNode nodeSubItem in nodeSubItemList)
                    {
                        //��ӵ������ӽڵ�
                        // ButtonItem�ӽڵ�
                        //��ýڵ������ֵ
                        nodeToElem = nodeSubItem as XmlElement;
                        pCaption = nodeToElem.GetAttribute("Caption").Trim();
                        pName = nodeToElem.GetAttribute("Name").Trim();
                        DevComponents.AdvTree.Node nodeButtonItem = new DevComponents.AdvTree.Node();//����һ��node�ڵ�
                        //��node���������Ը�ֵ
                        nodeButtonItem.Text = pCaption + "," + pName;
                        nodeButtonItem.CheckBoxVisible = true;
                        nodeButtonItem.Expanded = true;
                        nodeButtonItem.Checked = false;
                        nodeButtonItem.NodeClick += new System.EventHandler(TreeNodeClick);//nodeclick�¼�
                        //���Ľڵ㸽���ڵڶ���������
                        subNodeRibb.Nodes.AddRange(new DevComponents.AdvTree.Node[] { nodeButtonItem });//���ButtonItem�ڵ�
                    }
                }
            }
            #endregion
            #endregion
        }
        /// <summary>
        /// ��XML�ж�ȡ�������ڵ�
        /// </summary>
        /// <param name="SysNode"></param>
        private void ReadToolBarXML(XmlNode SysNode)
        {
            this.node2.Text = "������";
            this.node2.Expanded = true;
            this.node2.CheckBoxVisible = false;
            this.node2.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
            this.node2.FullRowBackground = true;
            //this.advTreeToolbar.Nodes.AddRange(new DevComponents.AdvTree.Node[] { this.node2 });
            XmlNodeList toolBarNodeList = SysNode.SelectNodes(".//ToolBar");
            XmlElement nodeToElem = null;
            string pCaption = "";//Caption����ֵ
            string pName = "";//Name����ֵ

            #region ��ӹ������ڵ��Լ��ӽڵ�
            foreach (XmlNode toolBarNode in toolBarNodeList)
            {//RibbonTabItem�ӽڵ�
                nodeToElem = toolBarNode as XmlElement;
                pCaption = nodeToElem.GetAttribute("Caption").Trim();
                pName = nodeToElem.GetAttribute("Name").Trim();
                DevComponents.AdvTree.Node toolBarNd = new DevComponents.AdvTree.Node();
                toolBarNd.Text = pCaption + "," + pName;
                toolBarNd.CheckBoxVisible = true;
                toolBarNd.Expanded = true;
                toolBarNd.Checked = false;
                toolBarNd.NodeClick+=new System.EventHandler(TreeNodeClick);//����nodeclick�¼�
                this.node2.Nodes.AddRange(new DevComponents.AdvTree.Node[] { toolBarNd });
                XmlNodeList nodeButtonList = toolBarNode.SelectNodes(".//SubItems");
                foreach (XmlNode nodeButton in nodeButtonList)
                {//RibbonBar�ӽڵ�
                    nodeToElem = nodeButton as XmlElement;
                    pCaption = nodeToElem.GetAttribute("Caption").Trim();
                    pName = nodeToElem.GetAttribute("Name").Trim();
                    DevComponents.AdvTree.Node nodeBtn = new DevComponents.AdvTree.Node();//���toolBar�ڵ�
                    nodeBtn.Text = pCaption + "," + pName;
                    nodeBtn.CheckBoxVisible = true;
                    nodeBtn.Expanded = true;
                    nodeBtn.Checked = false;
                    nodeBtn.NodeClick+=new System.EventHandler(TreeNodeClick);//����nodeclick�¼�
                    toolBarNd.Nodes.AddRange(new DevComponents.AdvTree.Node[] { nodeBtn });//���Button�ڵ�
                }
            }
            #endregion
        }
        /// <summary>
        /// ��XML�ж�ȡ�Ҽ��˵��ڵ�
        /// </summary>
        /// <param name="SysNode"></param>
        private void ReadContextMenuXML(XmlNode SysNode)
        {
            this.node3.Text = "�Ҽ��˵�";
            this.node3.Expanded = true;
            this.node3.CheckBoxVisible = false;
            this.node3.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
            this.node3.FullRowBackground = true;
            //this.advTreeContextMenuBar.Nodes.AddRange(new DevComponents.AdvTree.Node[] { this.node3 });
            XmlNodeList contextMenuNodeList = SysNode.SelectNodes(".//ContextMenuBar");
            XmlElement nodeToElem = null;
            string pCaption = "";//Caption����ֵ
            string pName = "";//Name����ֵ

            #region ����Ҽ��˵��ڵ��Լ��ӽڵ�
            foreach (XmlNode contextMenuNode in contextMenuNodeList)
            {//contextMenuBar�ӽڵ�
                nodeToElem = contextMenuNode as XmlElement;
                pCaption = nodeToElem.GetAttribute("Caption").Trim();
                pName = nodeToElem.GetAttribute("Name").Trim();
                DevComponents.AdvTree.Node menuBarnode = new DevComponents.AdvTree.Node();
                menuBarnode.Text = pCaption + "," + pName;
                menuBarnode.CheckBoxVisible = true;
                menuBarnode.Expanded = true;
                menuBarnode.Checked = false;
                menuBarnode.NodeClick += new System.EventHandler(TreeNodeClick);//nodeclick�¼�
                this.node3.Nodes.AddRange(new DevComponents.AdvTree.Node[] { menuBarnode });
                XmlNodeList nodeButtonList = contextMenuNode.SelectNodes(".//SubItems");//���contextMenuBar�ڵ�
                foreach (XmlNode oneNodeButton in nodeButtonList)
                {//ButtonItem�ӽڵ�
                    nodeToElem = oneNodeButton as XmlElement;
                    pCaption = nodeToElem.GetAttribute("Caption").Trim();
                    pName = nodeToElem.GetAttribute("Name").Trim();
                    DevComponents.AdvTree.Node subNodeBtn = new DevComponents.AdvTree.Node();
                    subNodeBtn.Text = pCaption + "," + pName;
                    subNodeBtn.CheckBoxVisible = true;
                    subNodeBtn.Expanded = true;
                    subNodeBtn.Checked = false;
                    subNodeBtn.NodeClick += new System.EventHandler(TreeNodeClick);//nodeclick�¼�
                    menuBarnode.Nodes.AddRange(new DevComponents.AdvTree.Node[] { subNodeBtn });//���ButtonItem�ڵ�
                }
            }
            #endregion
        }

        /// <summary>
        /// ���node�ڵ㣬����check����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeNodeClick(object sender, EventArgs e)
        {
            DevComponents.AdvTree.Node pNode = sender as DevComponents.AdvTree.Node;//��õ���Ľڵ�
            pNode.Checked = !pNode.Checked;//���ڵ��ѡ��״̬�÷�
            DevComponents.AdvTree.Node nodeParent = new DevComponents.AdvTree.Node();//�������ڵ����
            DevComponents.AdvTree.NodeCollection  nodeChilds = new DevComponents.AdvTree.NodeCollection();//�����ӽڵ㼯�ϱ���
            if (pNode.Checked == true)
            {
                #region ���ӽڵ㱻ѡ�У��������ڵĸ��ڵ����ѡ��
                nodeParent = pNode.Parent;
                while (nodeParent != null && nodeParent.CheckBoxVisible == true)
                {
                    if (nodeParent.Checked == false)
                    {
                        nodeParent.Checked = true;
                    }
                    nodeParent = nodeParent.Parent;
                }
                #endregion
                #region �����ڵ㱻ѡ��,�����е��ӽڵ����ѡ��
                setNodeChildVisble(pNode, true);
                #endregion
            }
            else if (pNode.Checked == false)
            {
                //�����ڵ�Ϊfalse,�������е��ӽڵ��Ϊfalse;
                setNodeChildVisble(pNode, false);
            }
        }
        /// <summary>
        /// ���ݸ��ڵ��visible���ԣ������ӽڵ��visible����
        /// </summary>
        /// <param name="pNode">���ڵ�</param>
        /// <param name="visible">���ڵ�visible����ֵ</param>
        private void setNodeChildVisble(DevComponents.AdvTree.Node pNode,bool visible)
        {
            DevComponents.AdvTree.NodeCollection nodeChilds = new DevComponents.AdvTree.NodeCollection();//�����ӽڵ㼯�ϱ���
            nodeChilds = pNode.Nodes;
            //�����ӽڵ�ʱ,���ӽڵ�ȫ��ѡ��
            if (nodeChilds.Count > 0)
            {
                for (int i = 0; i < nodeChilds.Count; i++)
                {
                    DevComponents.AdvTree.Node nodeChild = nodeChilds[i];
                    nodeChild.Checked = visible;
                    setNodeChildVisble(nodeChild, visible);
                }
            }
        }
        /// <summary>
        /// ����advTree�Ľڵ��visible����ֵ���޸�XML�Ľڵ��visible���Ժ�Enable����---������˵�
        /// </summary>
        private void UpdateXmlBaseOnMainTree()
        {
            //����RibbonTab��visible��enable����
            DevComponents.AdvTree.NodeCollection pNodeColl = this.node1.Nodes;//��á����˵����������ڵ�node1���ӽڵ㼯��
            for (int i = 0; i < pNodeColl.Count; i++)
            {
                DevComponents.AdvTree.Node pNode = pNodeColl[i];//������˵����������ڵ�ĵ�һ���ӽڵ�
                int index = pNode.Text.IndexOf(",");//��á����˵����������ڵ��һ���ӽڵ�������������Ķ��ŵ�����ֵ���Ա��øýڵ������
                string pName = pNode.Text.Substring(index + 1);//ͨ�����ŵ�����ֵ��ý������ڵ������Name���Ա�������xml��ȡ�Ľڵ�Ƚ�
                XmlNodeList RibbonTabNodeList = SysNode.SelectNodes(".//RibbonTabItem");//���xml�ļ������˵��ڵ�ļ���
                //����xml�ļ������˵��ӽڵ�
                foreach (XmlNode RibbonTabNode in RibbonTabNodeList)
                {
                    XmlElement RibbonTabElem = RibbonTabNode as XmlElement;//���ڵ����ͽ���ת�����Ա��ýڵ������ֵ
                    string mName = RibbonTabElem.GetAttribute("Name").Trim();//��ýڵ��Name����ֵ
                    if (pName == mName)//���������еĽڵ��Name��xml�нڵ��Name����ֵ���бȽ�,�ҵ���������еĽڵ�ƥ���xml�ļ��еĶ�Ӧ�ڵ�
                    {
                        //���ݽ������нڵ�Ĺ�ѡ���������xml�ļ��ж�ӦRibbonTab�ڵ��RibbonBar��visible��enable����
                        if (pNode.Checked == true)
                        {
                            RibbonTabElem.SetAttribute("Enabled", "true");
                            RibbonTabElem.SetAttribute("Visible", "true");
                        }
                        else if (pNode.Checked == false)
                        {
                            RibbonTabElem.SetAttribute("Enabled", "false");
                            RibbonTabElem.SetAttribute("Visible", "false");
                            #region �����ڵ��visible��enable����Ϊfalse,����������е��ӽڵ��visible��enable����ҲΪfalse
                            XmlNodeList fNodeList = RibbonTabNode.SelectNodes(".//RibbonBar");//���xml�ļ���RibbonBar�ڵ���ӽڵ㼯��
                            //�����ӽڵ㣬���������ӽڵ��visible��enable��������Ϊfalse
                            foreach (XmlNode fNode in fNodeList)
                            {
                                XmlElement fElme = fNode as XmlElement;
                                fElme.SetAttribute("Enabled", "false");
                                fElme.SetAttribute("Visible", "false");
                                XmlNodeList ffNodeList = fNode.SelectNodes(".//SubItems");//���xml�ļ���SubItems�ڵ���ӽڵ㼯��
                                //�����ӽڵ㣬���������ӽڵ��visible��enable��������Ϊfalse
                                foreach (XmlNode ffNode in ffNodeList)
                                {
                                    XmlElement ffElem = ffNode as XmlElement;
                                    ffElem.SetAttribute("Enabled", "false");
                                    ffElem.SetAttribute("Visible", "false");
                                }
                            }
                            break;
                            #endregion
                        }
                        #region ����RibbonBar��visible��enable����
                        DevComponents.AdvTree.NodeCollection ppNodeColl = pNode.Nodes;//��á����˵����������ڵ�ڶ����ӽڵ㼯��
                        //�����ڶ����ӽڵ�
                        for (int j = 0; j < ppNodeColl.Count; j++)
                        {
                            DevComponents.AdvTree.Node ppNode = ppNodeColl[j];
                            int pIndex = ppNode.Text.IndexOf(",");//ͬ���øýڵ�Ķ�������
                            string ppName = ppNode.Text.Substring(pIndex + 1);//ͨ��������ýڵ������
                            XmlNodeList RibbonBarNodeList = RibbonTabNode.SelectNodes(".//RibbonBar");//��ȡXML�ļ��������Ӧ�ڶ����ڵ�Ľڵ㼯��
                            //����xml�ļ��еĵڶ����ڵ�
                            foreach (XmlNode RibbonBar in RibbonBarNodeList)
                            {
                                XmlElement RibbonBarElem = RibbonBar as XmlElement;
                                string mmName = RibbonBarElem.GetAttribute("Name").Trim();//��ö����ڵ��Name����
                                //ͨ�������ҵ�xml������Ӧ�Ľڵ�
                                if (ppName == mmName)
                                {
                                    //���ݽ����ϵĹ�ѡ���������xml�ļ��нڵ������ֵ
                                    if (ppNode.Checked == true)
                                    {
                                        RibbonBarElem.SetAttribute("Enabled", "true");
                                        RibbonBarElem.SetAttribute("Visible", "true");
                                    }
                                    else if (ppNode.Checked == false)
                                    {
                                        RibbonBarElem.SetAttribute("Enabled", "false");
                                        RibbonBarElem.SetAttribute("Visible", "false");
                                        #region �����ڵ��visible��enable����Ϊfalse,����������е��ӽڵ��visible��enable����ҲΪfalse
                                       //��xml�ļ��������ӽڵ����������δfalse
                                        XmlNodeList fffNodeList = RibbonBar.SelectNodes(".//SubItems");
                                        //����ButtonItem��visible��enable����
                                        foreach (XmlNode fffNode in fffNodeList)
                                        {
                                            XmlElement fffElem = fffNode as XmlElement;
                                            fffElem.SetAttribute("Enabled", "false");
                                            fffElem.SetAttribute("Visible", "false");
                                        }
                                        break;
                                        #endregion
                                    }
                                    #region ����ButtonItem��visible��enable����
                                    DevComponents.AdvTree.NodeCollection pppNodeColl = ppNode.Nodes;//��á����˵����������ڵ�������ӽڵ㼯��
                                    //�����������е������ӽڵ�
                                    for (int k = 0; k < pppNodeColl.Count; k++)
                                    {
                                        DevComponents.AdvTree.Node pppNode = pppNodeColl[k];
                                        int ppIndex = pppNode.Text .IndexOf(",");
                                        string pppName = pppNode.Text.Substring(ppIndex + 1);//��ý������ӽڵ������
                                        XmlNodeList ButtonNodeList = RibbonBar.SelectNodes(".//SubItems");//��ȡxml�ļ��е������ӽڵ㼯��
                                        foreach (XmlNode ButtonNode in ButtonNodeList)
                                        {
                                            XmlElement ButtonElem = ButtonNode as XmlElement;
                                            string mmmName = ButtonElem.GetAttribute("Name").Trim();//���xml�ļ��нڵ������ֵ
                                            //ͨ���ȽϽ����ϵ����ƺ�xml�ļ����ܽڵ�����ƣ���xml�ļ����ҵ�������Ͻڵ��Ӧ�ڵ㣬
                                            //�����ݽ���Ĺ�ѡ���������xml�ļ��нڵ������ֵ
                                            if (pppName == mmmName)
                                            {
                                                //����SubItems�ڵ������
                                                if (pppNode.Checked == true)
                                                {
                                                    ButtonElem.SetAttribute("Enabled", "true");
                                                    ButtonElem.SetAttribute("Visible", "true");
                                                }
                                                else if (pppNode.Checked == false)
                                                {
                                                    ButtonElem.SetAttribute("Enabled", "false");
                                                    ButtonElem.SetAttribute("Visible", "false");
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    #endregion
                                    break;
                                }
                                
                            }
                        }
                        #endregion
                        break;
                    }
                }
               
            }
        }
        /// <summary>
        /// ����advTree�Ľڵ��visible����ֵ���޸�XML�Ľڵ��visible���Ժ�Enable����---��Թ�����
        /// </summary>
        private void UpdateXmlBaseOnToolBarTree()
        {
            //����ToolBar��visible��enable����
            DevComponents.AdvTree.NodeCollection pNodeColl = this.node2.Nodes;//node1�������ӽڵ�
            for (int i = 0; i < pNodeColl.Count; i++)
            {
                DevComponents.AdvTree.Node pNode = pNodeColl[i];
                int index = pNode.Text .IndexOf(",");//���ڵ��϶��ŵ�����
                string pName = pNode.Text.Substring(index + 1);//���ڵ��Name
                XmlNodeList ToolBarNodeList = SysNode.SelectNodes(".//ToolBar");//���ToolBar�ڵ㼯��
                foreach (XmlNode ToolBarNode in ToolBarNodeList)
                {//����ToolBar������
                    XmlElement ToolBarElem = ToolBarNode as XmlElement;//��ToolBar�ڵ�ת��ΪToolBarԪ��
                    string mName = ToolBarElem.GetAttribute("Name").Trim();//ͨ��ToolBarԪ�ػ��ToolBar��Name����ֵ
                    if (pName == mName)//������������ڵ������Name��ToolBar��Name����ֵһ���������ToolBar�ڵ���������Ը�ֵ
                    {
                        if (pNode.Checked == true)
                        {
                            ToolBarElem.SetAttribute("Enabled", "true");
                            ToolBarElem.SetAttribute("Visible", "true");
                        }
                        else if (pNode.Checked == false)
                        {
                            ToolBarElem.SetAttribute("Enabled", "false");
                            ToolBarElem.SetAttribute("Visible", "false");
                            #region �����ڵ�ToolBar��visible��enable����Ϊfalse,����������е��ӽڵ��visible��enable����ҲΪfalse
                            XmlNodeList fNodeList = ToolBarNode.SelectNodes(".//SubItems");
                            //����SubItems��visible��enable����
                            foreach (XmlNode fNode in fNodeList)
                            {
                                XmlElement fElme = fNode as XmlElement;
                                fElme.SetAttribute("Enabled", "false");
                                fElme.SetAttribute("Visible", "false");
                            }
                            break;
                            #endregion
                        }
                        #region ����SubItems��visible��enable����
                        DevComponents.AdvTree.NodeCollection ppNodeColl = pNode.Nodes;//pNode�������ӽڵ�
                        for (int j = 0; j < ppNodeColl.Count; j++)
                        {
                            DevComponents.AdvTree.Node ppNode = ppNodeColl[j];
                            int pIndex = ppNode.Text.IndexOf(",");
                            string ppName = ppNode.Text.Substring(pIndex + 1);
                            XmlNodeList ButtonNodeList = ToolBarNode.SelectNodes(".//SubItems");
                            foreach (XmlNode ButtonNode in ButtonNodeList)
                            {
                                XmlElement ButtonElem = ButtonNode as XmlElement;
                                string mmName = ButtonElem.GetAttribute("Name").Trim();
                                //ͨ�������ҵ�xml������Ӧ�Ľڵ�
                                if (ppName == mmName)
                                {
                                    //����SubItems�ڵ������
                                    if (ppNode.Checked == true)
                                    {
                                        ButtonElem.SetAttribute("Enabled", "true");
                                        ButtonElem.SetAttribute("Visible", "true");
                                    }
                                    else if (ppNode.Checked == false)
                                    {
                                        ButtonElem.SetAttribute("Enabled", "false");
                                        ButtonElem.SetAttribute("Visible", "false");
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion
                        break;
                    }
                }

            }
        }
        /// <summary>
        /// ����advTree�Ľڵ��visible����ֵ���޸�XML�Ľڵ��visible���Ժ�Enable����---����Ҽ��˵�
        /// </summary>
        private void UpdateXmlBaseOnContextMenuTree()
        {
            //����ContextMenu��visible��enable����
            DevComponents.AdvTree.NodeCollection pNodeColl = this.node3.Nodes;//node1�������ӽڵ�
            for (int i = 0; i < pNodeColl.Count; i++)
            {
                DevComponents.AdvTree.Node pNode = pNodeColl[i];
                int index = pNode.Text .IndexOf(",");//������ڵ��϶��ŵ�����
                string pName = pNode.Text .Substring(index + 1);//������ڵ��Name
                XmlNodeList ContextMenuNodeList = SysNode.SelectNodes(".//ContextMenuBar");//���ContextMenuBar�ڵ���
                foreach (XmlNode ContextMenuNode in ContextMenuNodeList)
                {//����ToolBar������
                    XmlElement ContextMenuElem = ContextMenuNode as XmlElement;//��ContextMenu�ڵ�ת��ΪContextMenuԪ��
                    string mName = ContextMenuElem.GetAttribute("Name").Trim();//ͨ��ContextMenuԪ�ػ��ContextMenu��Name����ֵ
                    if (pName == mName)//������������ڵ������Name��ContextMenu��Name����ֵһ���������ContextMenu�ڵ���������Ը�ֵ
                    {
                        if (pNode.Checked == true)
                        {
                            ContextMenuElem.SetAttribute("Enabled", "true");
                            ContextMenuElem.SetAttribute("Visible", "true");
                        }
                        else if (pNode.Checked == false)
                        {
                            ContextMenuElem.SetAttribute("Enabled", "false");
                            ContextMenuElem.SetAttribute("Visible", "false");
                            #region �����ڵ�ContextMenuElem��visible��enable����Ϊfalse,����������е��ӽڵ��visible��enable����ҲΪfalse
                            XmlNodeList fNodeList = ContextMenuNode.SelectNodes(".//SubItems");
                            //����SubItems��visible��enable����
                            foreach (XmlNode fNode in fNodeList)
                            {
                                XmlElement fElme = fNode as XmlElement;
                                fElme.SetAttribute("Enabled", "false");
                                fElme.SetAttribute("Visible", "false");
                            }
                            break;
                            #endregion
                        }
                        #region ����SubItems��visible��enable����
                        DevComponents.AdvTree.NodeCollection ppNodeColl = pNode.Nodes;//pNode�������ӽڵ�
                        for (int j = 0; j < ppNodeColl.Count; j++)
                        {
                            DevComponents.AdvTree.Node ppNode = ppNodeColl[j];
                            int pIndex = ppNode.Text .IndexOf(",");
                            string ppName = ppNode.Text .Substring(pIndex + 1);
                            XmlNodeList ButtonNodeList = ContextMenuNode.SelectNodes(".//SubItems");
                            foreach (XmlNode ButtonNode in ButtonNodeList)
                            {
                                XmlElement ButtonElem = ButtonNode as XmlElement;
                                string mmName = ButtonElem.GetAttribute("Name").Trim();
                                //ͨ�������ҵ�xml������Ӧ�Ľڵ�
                                if (ppName == mmName)
                                {
                                    //����SubItems�ڵ������
                                    if (ppNode.Checked == true)
                                    {
                                        ButtonElem.SetAttribute("Enabled", "true");
                                        ButtonElem.SetAttribute("Visible", "true");
                                    }
                                    else if (ppNode.Checked == false)
                                    {
                                        ButtonElem.SetAttribute("Enabled", "false");
                                        ButtonElem.SetAttribute("Visible", "false");
                                    }
                                    break;
                                }
                            }
                        }
                        #endregion
                        break;
                    }
                }

            }
        }

        //��������
        private void btnSave_Click(object sender, EventArgs e)
        {
            UpdateXmlBaseOnMainTree();
            UpdateXmlBaseOnToolBarTree();
            UpdateXmlBaseOnContextMenuTree();

            string roleName = this.comboBoxRole.Text.Trim();//�û�������
            int roleID = dicUserGroup[roleName];//�û���ID

            #region �������ݿ⣬��BLOB�ֶβ��뵽���ݿ���
            byte[] XmlBt = System.Text.Encoding.Default.GetBytes(docXml.InnerXml);//��xmlDocumentת��Ϊ��������
            string updateStr = "update ice_UserGroupinfo set G_purview=:G_purview where G_id =" + roleID;
            if (pDbType == "ORACLE")
            {
                OracleConnection pConn = new OracleConnection(pConStr);
                try
                {
                    pConn.Open();

                    #region ����¼�����ɫȨ�ޱ���
                    OracleCommand OracleCmd = new OracleCommand(updateStr, pConn);
                    //OracleCmd.CommandType = CommandType.Text;
                    OracleCmd.CommandText = updateStr;//����SQL���������ݿ�

                    OracleCmd.Parameters.Add("G_purview", System.Data.OracleClient.OracleType.Blob, XmlBt.Length);
                    OracleCmd.Parameters[0].Value = XmlBt;

                    OracleCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("����", ex.Message);
                    return;
                }
            }
            if (pDbType == "ACCESS")
            {
                OleDbConnection pOleConn = new OleDbConnection(pConStr);
                try
                {
                    pOleConn.Open();
                    #region ����ɫ��Ϣ�����û���Ȩ�ޱ���
                    OleDbCommand oledbCmd = new OleDbCommand(updateStr, pOleConn);
                    //oledbCmd.CommandText = updateStr;//����SQL���������ݿ�
                    oledbCmd.Parameters.Add("G_purview", System.Data.OleDb.OleDbType.Binary, XmlBt.Length);
                    oledbCmd.Parameters[0].Value = XmlBt;
                    oledbCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("����", ex.Message);
                    return;
                }
            }
            #endregion

            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ɹ�");

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Application.Exit();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Application.Exit();
            this.Close();
        }
    }
}