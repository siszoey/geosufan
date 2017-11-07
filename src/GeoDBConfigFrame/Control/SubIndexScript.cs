using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;
using System.Data.OleDb;
using System.Xml;
using System.IO;

namespace GeoDBConfigFrame
{
    public partial class SubIndexScript : DevComponents.DotNetBar.Office2007Form
    {
        public string m_mypath = null;
        public string m_xmlPath;
        public XmlDocument m_xmldoc;
        public TreeNode m_tMapNode;
        private TreeNode m_LastDragNode = null;
        string m_Typecode = "";
        public SubIndexScript()
        {
            InitializeComponent();

            //��ʼ��listview�б�
            InitListView();


        }

        //private void btnOk_Click(object sender, EventArgs e)
        //{
            
        //    this.DialogResult = DialogResult.OK;
        //    this.Hide();
        //    this.Dispose(true);
        //}

        private void butnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
            this.Dispose(true);
        }

        public void InitListView()
        {

            listViewControl.View = View.Details;

            //�� ���ݵ�Ԫ�� �л�ȡ��Ϣ
            ListViewItem lItem;
            ListViewItem.ListViewSubItem lSubItem;
            ListViewItem.ListViewSubItem lSubItemSecond;
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            m_mypath = dIndex.GetDbInfo();
            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            OleDbConnection mycon = new OleDbConnection(constr);   //����OleDbConnection����ʵ�����������ݿ�
            string strExp = "";
            strExp = "select ר������,����,�ű��ļ� from ��׼ר����Ϣ��";
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            try
            {
                mycon.Open();

                //����datareader   ���������ӵ���     
                OleDbDataReader aReader = aCommand.ExecuteReader();
                while (aReader.Read())
                {
                    lItem = new ListViewItem();
                    lItem.Text = aReader["ר������"].ToString();

                    lSubItem = new ListViewItem.ListViewSubItem();
                    lSubItem.Text = aReader["����"].ToString();
                    lItem.SubItems.Add(lSubItem);

                    lSubItemSecond = new ListViewItem.ListViewSubItem();
                    lSubItemSecond.Text = aReader["�ű��ļ�"].ToString();
                    lItem.SubItems.Add(lSubItemSecond);

                    listViewControl.Items.Add(lItem);
                }
                //�ر�reader����     
                aReader.Close();

                //�ر�����,�����Ҫ     
                mycon.Close();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }



        private void listViewControl_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //listview�ڵ㷢���任 ���ظ�������ͼ
        private void listViewControl_MouseClick(object sender, MouseEventArgs e)
        {

            //�ڵ㷢���仯
            ListView lvControl = (ListView)sender;
            if (lvControl.SelectedItems.Count == 0) return;
            m_Typecode = lvControl.SelectedItems[0].Text;
            string strIndexName = lvControl.SelectedItems[0].SubItems[2].Text;
            LoadTreeView(strIndexName);//�������б�
        }

        private void LoadTreeView(string strIndexName)
        {
            try
            {
                //������ؼ�
                treeViewControl.Nodes.Clear();

                //��ȡ�����ļ���ʼ������ͼ
                string strModFile = Application.StartupPath + "\\..\\Template\\" + strIndexName;

                //�ж��ļ��Ƿ���� 
                if (File.Exists(strModFile))
                {
                    //����xml�ļ�
                    m_xmldoc = new XmlDocument();
                    m_xmldoc.Load(strModFile);
                    m_xmlPath = strModFile;

                    //���ڵ�
                    TreeNode tparent;
                    tparent = new TreeNode();
                    tparent.Text = "��ͼ�ĵ�";
                    tparent.Tag = 0;
                    treeViewControl.Nodes.Add(tparent);
                    treeViewControl.ExpandAll();
                    tparent.ImageIndex = 0;
                    tparent.SelectedImageIndex = 0;


                    TreeNode tNewNode;
                    string strTblName = "";
                    string strRootName = "";
                    string strSearchRoot = "//GisMap";
                    XmlNode xmlNodeRoot = m_xmldoc.SelectSingleNode(strSearchRoot);
                    XmlElement xmlElentRoot = (XmlElement)xmlNodeRoot;
                    strRootName = xmlElentRoot.GetAttribute("sItemName");

                    m_tMapNode = new TreeNode();
                    m_tMapNode.Text = strRootName;
                    m_tMapNode.Tag = 1;
                    tparent.Nodes.Add(m_tMapNode);
                    tparent.ExpandAll();
                    m_tMapNode.ImageIndex = 1;
                    m_tMapNode.SelectedImageIndex = 1;

                    //������ӵ�һ���ӽڵ� SubGroup
                    string strSearch = "//SubGroup";
                    XmlNodeList xmlNdList = m_xmldoc.SelectNodes(strSearch);
                    foreach (XmlNode xmlChild in xmlNdList)
                    {
                        strTblName = "";
                        XmlElement xmlElent = (XmlElement)xmlChild;
                        strTblName = xmlElent.GetAttribute("sItemName");
                        tNewNode = new TreeNode();
                        tNewNode.Text = strTblName;
                        tNewNode.Tag = 2;
                        m_tMapNode.Nodes.Add(tNewNode);
                        m_tMapNode.ExpandAll();
                        tNewNode.ImageIndex = 2;
                        tNewNode.SelectedImageIndex = 2;

                        //��������ӽڵ�
                        AddLeafItem(tNewNode, xmlChild);
                    }

                    /*  //��������xml�ļ�
                        treeViewControl.Nodes.Add(new TreeNode(xDoc.DocumentElement.Name));
                        TreeNode tNode = new TreeNode();
                        tNode = (TreeNode)treeViewControl.Nodes[0];  
                        addTreeNode(xDoc.DocumentElement, tNode);*/

                    treeViewControl.ExpandAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        //���Ҷ�ӽڵ�
        public void AddLeafItem(TreeNode treeNode, XmlNode xmlNode)
        {
            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string layer = "";
            strExp = "select �ؼ�ͼ�� from ��׼ר����Ϣ�� where ר������='" + m_Typecode + "'";
            layer = db.GetInfoFromMdbByExp(constr, strExp);
            strExp = "select ���� from ��׼ͼ����Ϣ�� where ����='" + layer + "'";
            layer = db.GetInfoFromMdbByExp(constr, strExp);

            if (treeNode != null && xmlNode != null)
            {
                TreeNode tNewNode;
                string strLayerDescribed = ""; //ͼ������ ����ͼ�ߵ�
                string strFileName = "";
                XmlNodeList xmlNdList;
                xmlNdList = xmlNode.ChildNodes;
                foreach (XmlNode xmlChild in xmlNdList)
                {
                    strLayerDescribed = "";
                    XmlElement xmlElent = (XmlElement)xmlChild;
                    strLayerDescribed = xmlElent.GetAttribute("sDemo");  //����
                    strFileName = xmlElent.GetAttribute("sItemName");    //����

                    //�޸�sfile����
                    tNewNode = new TreeNode();
                    tNewNode.Text = strLayerDescribed;
                    tNewNode.Name = strFileName;
                    tNewNode.Tag = 3;
                    treeNode.Nodes.Add(tNewNode);
                    tNewNode.ImageIndex = 3;
                    tNewNode.SelectedImageIndex = 3;
                    if (strFileName.Trim().CompareTo(layer.Trim()) == 0)
                        tNewNode.ForeColor = Color.Red;
                }
                treeNode.ExpandAll();
            }
        }

        //�ݹ���ӽڵ�
        private void addTreeNode(XmlNode xmlNode, TreeNode treeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList xNodeList;
            if (xmlNode.HasChildNodes)
            {
                xNodeList = xmlNode.ChildNodes;
                for (int x = 0; x <= xNodeList.Count - 1; x++)
                {
                    xNode = xmlNode.ChildNodes[x];
                    treeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = treeNode.Nodes[x];
                    addTreeNode(xNode, tNode);
                }
            }
            else
                treeNode.Text = xmlNode.OuterXml.Trim();
        }

        private void treeViewControl_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeViewControl.SelectedNode = e.Node;
            if (e.Button == MouseButtons.Right)
            {
                if (e.Node.Tag.Equals(0))//���ڵ㵯�������˵�
                {
                    System.Drawing.Point ClickPoint = treeViewControl.PointToScreen(new System.Drawing.Point(e.X, e.Y));
                    contextMenuSub.Show(ClickPoint);
                }
                else
                {

                    System.Drawing.Point ClickPoint = treeViewControl.PointToScreen(new System.Drawing.Point(e.X, e.Y));
                    contextMenuTree.Show(ClickPoint);
                    if (e.Node.Tag.Equals(3))//�������Ҷ�ӽڵ�
                    {
                        MenuItemEditLayer.Visible = true;
                        MenuItemMainLayer.Visible = true;
                        MenuItemCanceMainLayer.Visible = true;
                        toolStripSeparator2.Visible = true;
                        if (!e.Node.ForeColor.Equals(Color.Red))
                            MenuItemCanceMainLayer.Enabled = false;
                    }
                    else
                    {
                        MenuItemEditLayer.Visible = false;
                        MenuItemMainLayer.Visible = false;
                        MenuItemCanceMainLayer.Visible = false;
                        toolStripSeparator2.Visible = false;
                    }
                }
            }
        }

        //���ר��
        private void MenuItemAddSub_Click(object sender, EventArgs e)
        {
            SubAttForm dlg = new SubAttForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            { 
                string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                OleDbConnection mycon = new OleDbConnection(constr);   //����OleDbConnection����ʵ�����������ݿ�
                string strExp = "select count(*) from ��׼ר����Ϣ�� where ר������='"+dlg.strSubCode+"'";
                GeoDataCenterDbFun db=new GeoDataCenterDbFun();
                int count=db.GetCountFromMdb(constr,strExp);
                if (count > 0)
                {
                    MessageBox.Show("ר���Ѵ��ڣ����޸�ר�����ͣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //��ǰlistview���һ����¼
                ListViewItem lvItem;
                ListViewItem.ListViewSubItem lSubItem;
                ListViewItem.ListViewSubItem lSubItemSecond;
                lvItem = new ListViewItem();
                lvItem.Text = dlg.strSubCode;

                lSubItem = new ListViewItem.ListViewSubItem();
                lSubItem.Text = dlg.strSubName;
                lvItem.SubItems.Add(lSubItem);

                lSubItemSecond = new ListViewItem.ListViewSubItem();
                lSubItemSecond.Text = dlg.strIndexFile;
                lvItem.SubItems.Add(lSubItemSecond);

                listViewControl.Items.Add(lvItem);
                listViewControl.Refresh();

                //��ȡ��ֵ��ӵ�����׼ר����Ϣ����
               
                strExp = "insert into ��׼ר����Ϣ�� (ר������,����,�ű��ļ�,��ͼ�����ļ�) values('" + dlg.strSubCode + "','" + dlg.strSubName + "','" + dlg.strIndexFile + "','" + dlg.strMapSymIndexFile  + "')";
                OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
                try
                {
                    mycon.Open();

                    //�����¼    
                    int iRows = aCommand.ExecuteNonQuery();

                    //�ر�����,�����Ҫ     
                    mycon.Close();
                }
                catch (System.Exception err)
                {
                    Console.WriteLine(err.Message);
                }

                //��\TemplateĿ¼�����ɶ�Ӧ�ļ�
                string strModFile = Application.StartupPath + "\\..\\Template\\StandardBlank.xml";
                string strIndexFile = Application.StartupPath + "\\..\\Template\\" + dlg.strIndexFile;
                if (!File.Exists(strIndexFile))
                {
                    File.Copy(strModFile, strIndexFile, true);
                }

                //�����ļ����޸�GisMap ItemName=""
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(strIndexFile);
                string strSearchRoot = "//GisMap";
                XmlNode xmlNodeRoot = xmldoc.SelectSingleNode(strSearchRoot);
                XmlElement xmlElentRoot = (XmlElement)xmlNodeRoot;
                xmlElentRoot.SetAttribute("sItemName", dlg.strSubName);
                xmldoc.Save(strIndexFile);
                m_Typecode = dlg.strSubCode;
                LoadTreeView(dlg.strIndexFile);
            }
        }

        //�޸�ר��
        private void MenuItemModifySub_Click(object sender, EventArgs e)
        {
            if (listViewControl.SelectedItems.Count <= 0)
            {
                MessageBox.Show("��ѡ��Ҫ�޸ĵ�ר��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //��ȡ���ݿ�����Ϣ ����
            //��ȡ��ֵ��ӵ�����׼ר����Ϣ����
            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            OleDbConnection mycon = new OleDbConnection(constr);   //����OleDbConnection����ʵ�����������ݿ�
            string strExp = "";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            //��ȡ�ڵ�����
            string strSubOldName = null;
            string strSubOldCode = null;
            string strSubOldIndexFile = null;
            string strSubOldMapFile = null;

            SubAttForm dlg = new SubAttForm();
            dlg.strSubCode = listViewControl.SelectedItems[0].Text;
            strSubOldCode = listViewControl.SelectedItems[0].Text;
            dlg.strSubName = listViewControl.SelectedItems[0].SubItems[1].Text;
            strSubOldName = listViewControl.SelectedItems[0].SubItems[1].Text;
            dlg.strIndexFile = listViewControl.SelectedItems[0].SubItems[2].Text;
            strSubOldIndexFile = listViewControl.SelectedItems[0].SubItems[2].Text;
            strExp="select ��ͼ�����ļ� from ��׼ר����Ϣ�� where " + "ר������ = '" + strSubOldCode + "' ";
            strSubOldMapFile= dlg.strMapSymIndexFile = db.GetInfoFromMdbByExp(constr, strExp);
            dlg.SetFormTextBoxAtt();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //û�����仯
                if (strSubOldCode.Equals(dlg.strSubCode) && strSubOldName.Equals(dlg.strSubName) && strSubOldIndexFile.Equals(dlg.strIndexFile)&&strSubOldMapFile.Equals(dlg.strMapSymIndexFile))
                    return;

                //�޸�listview
                listViewControl.SelectedItems[0].Text = dlg.strSubCode;
                listViewControl.SelectedItems[0].SubItems[1].Text = dlg.strSubName;
                listViewControl.SelectedItems[0].SubItems[2].Text = dlg.strIndexFile;
                listViewControl.Refresh();
            }

            
         //   strExp = "update ��׼ר����Ϣ�� set ר������ = '" + dlg.strSubCode + "'," + "���� = '" + dlg.strSubName + "' where " + "ר������ = '" + strSubOldCode + "'";
            strExp = "update ��׼ר����Ϣ�� set ר������ = '" + dlg.strSubCode + "'," + "���� = '" + dlg.strSubName + "'," + "�ű��ļ� = '" + dlg.strIndexFile + "'," + "��ͼ�����ļ� = '" + dlg.strMapSymIndexFile + "' where " + "ר������ = '" + strSubOldCode + "'";
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            try
            {
                mycon.Open();

                //g���¼�¼    
                int iRows = aCommand.ExecuteNonQuery();

                //�ر�����,�����Ҫ     
                mycon.Close();
            }
            catch (System.Exception err)
            {
                Console.WriteLine(err.Message);
            }
            strExp = "update ��ͼ�����Ϣ�� set ר������='" + dlg.strSubCode + "' where " + "ר������ = '" + strSubOldCode + "'";
            db.ExcuteSqlFromMdb(constr,strExp);
            string strModFile = Application.StartupPath + "\\..\\Template\\StandardBlank.xml";
            string strIndexFile = Application.StartupPath + "\\..\\Template\\" + dlg.strIndexFile;
            if (strSubOldIndexFile.CompareTo(dlg.strIndexFile) != 0)
            {
                if (!File.Exists(strIndexFile))
                {
                    File.Copy(strModFile, strIndexFile, true);
                }
            }
            if (strSubOldName.CompareTo(dlg.strSubName) != 0)  //�޸���ר���������޸Ķ�Ӧ��xml�ļ�
            {
                //�����ļ����޸�GisMap ItemName=""
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(strIndexFile);
                string strSearchRoot = "//GisMap";
                XmlNode xmlNodeRoot = xmldoc.SelectSingleNode(strSearchRoot);
                XmlElement xmlElentRoot = (XmlElement)xmlNodeRoot;
                xmlElentRoot.SetAttribute("sItemName", dlg.strSubName);
                xmldoc.Save(strIndexFile);
            }

        }

        //ɾ��ר��
        private void MenuItemDelSub_Click(object sender, EventArgs e)
        {
            if (listViewControl.SelectedItems.Count <= 0)
            {
                MessageBox.Show("��ѡ��Ҫɾ����ר��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string strTip = "ȷ��ɾ����" + listViewControl.SelectedItems[0].SubItems[1].Text + "��";
            if (MessageBox.Show(strTip, "��ʾ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                return;

            //��ȡ��ֵ��ӵ�����׼ר����Ϣ����
            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            OleDbConnection mycon = new OleDbConnection(constr);   //����OleDbConnection����ʵ�����������ݿ�
            string strExp = "";
            strExp = "delete  from ��׼ר����Ϣ�� where ר������ = '" + listViewControl.SelectedItems[0].Text + "'";
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            try
            {
                mycon.Open();

                //ɾ����¼    
                int iRows = aCommand.ExecuteNonQuery();
             
                //�ر�����,�����Ҫ     
                mycon.Close();
            }
            catch (System.Exception err)
            {
                Console.WriteLine(err.Message);
            }
            strExp = "delete  from ��ͼ�����Ϣ�� where ר������ = '" + listViewControl.SelectedItems[0].Text + "'";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            db.ExcuteSqlFromMdb(constr, strExp);
            //ɾ���ű��ļ�
            string strIndexFile = Application.StartupPath + "\\..\\Template\\" + listViewControl.SelectedItems[0].SubItems[2].Text;
            if (File.Exists(strIndexFile))
            {
                File.Delete(strIndexFile);
            }

            //ɾ��listview
            listViewControl.Items.Remove(listViewControl.SelectedItems[0]);

            treeViewControl.Nodes.Clear();
        }

        //�����
        private void MenuItemTreeAddGroup_Click(object sender, EventArgs e)
        {
            AddGroupForm dlg = new AddGroupForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //�޸�����ͼ
                TreeNode tNewGroupNode = new TreeNode();
                tNewGroupNode.Name = "��" + dlg.m_strSubName + "��";
                tNewGroupNode.Text = "��" + dlg.m_strSubName + "��";
                tNewGroupNode.Tag = 2;
                tNewGroupNode.ImageIndex = 2;
                tNewGroupNode.SelectedImageIndex = 2;
                m_tMapNode.Nodes.Add(tNewGroupNode);
                m_tMapNode.ExpandAll();

                //�޸�xml�ļ�
                string strSearchRoot = "//GisMap";
                XmlNode xmlNodeRoot = m_xmldoc.SelectSingleNode(strSearchRoot);
                XmlElement xmlElentRoot = (XmlElement)xmlNodeRoot;
                XmlElement xmlElemGroup = m_xmldoc.CreateElement("SubGroup");
                string strGroupName = "��" + dlg.m_strSubName + "��";
                xmlElemGroup.SetAttribute("sItemName", strGroupName);
                xmlElemGroup.SetAttribute("sType", "GROUP");
                xmlElentRoot.AppendChild(xmlElemGroup);


                //��ȡ����ͼ���б�
                TreeNode tNewNode = null;
                foreach (TreeNode fCurNode in dlg.totreeView.Nodes)
                {
                    
                    XmlNode boolnode = m_xmldoc.SelectSingleNode("//Layer[@sItemName='" +  fCurNode.Text+ "']");
                    if (boolnode != null)
                    {
                        MessageBox.Show(fCurNode.Text+"ͼ���Ѵ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        continue;
                    }
                    XmlElement xmlElemt = m_xmldoc.CreateElement("Layer");
                    xmlElemt.SetAttribute("sDemo", fCurNode.Text);
                    xmlElemt.SetAttribute("sItemName", fCurNode.Text);
                    

                    //��� �ļ����� ҵ��������  ҵ��С�����
                    string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                    OleDbConnection mycon = new OleDbConnection(constr);   //����OleDbConnection����ʵ�����������ݿ�
                    string strExp = "";
                    strExp = "select  * from ��׼ͼ����Ϣ�� where ���� = '" + fCurNode.Text + "'";
                    OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
                    strExp = "select ͼ����� from ��׼ר����Ϣ�� where ר������='" + m_Typecode + "'";
                    GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                    string layer = db.GetInfoFromMdbByExp(constr, strExp);
                    try
                    {
                        mycon.Open();

                        //����datareader   ���������ӵ���     
                        OleDbDataReader aReader = aCommand.ExecuteReader();
                        while (aReader.Read())
                        {
                            xmlElemt.SetAttribute("sFile", aReader["����"].ToString());
                            xmlElemt.SetAttribute("sBigClass", aReader["ҵ��������"].ToString());
                            xmlElemt.SetAttribute("sSubClass", aReader["ҵ��С�����"].ToString());


                            if (layer == "")
                                layer = aReader["����"].ToString();
                            else if(!GetExists(aReader["����"].ToString(),layer))
                                layer += "/" + aReader["����"].ToString();//����ͼ�����
                        }

                        //�ر�reader����     
                        aReader.Close();

                        //�ر�����,�����Ҫ     
                        mycon.Close();
                        //���±�׼ר����Ϣ��
                        strExp = "update ��׼ר����Ϣ�� set ͼ�����='" + layer + "' where ר������='" + m_Typecode + "'";
                        db.ExcuteSqlFromMdb(constr, strExp);
                    }
                    catch (System.Exception err)
                    {

                    }
                    //������
                    tNewNode = new TreeNode();
                    tNewNode.Text = fCurNode.Text;
                    tNewNode.Name = fCurNode.Name;
                    tNewNode.Tag = 3;
                    tNewNode.ImageIndex = 3;
                    tNewNode.SelectedImageIndex = 3;
                    tNewGroupNode.Nodes.Add(tNewNode);
                    tNewGroupNode.ExpandAll();

                    xmlElemGroup.AppendChild(xmlElemt);

                }
                treeViewControl.Refresh();

                //����xml�ļ�
                m_xmldoc.Save(m_xmlPath);
            }
        }

        //���ͼ��������Ƿ����ͼ��
        private bool GetExists(string layer,string layers)
        {
            bool exist=false;
            if (layers.Contains("/"))
            {
                string[] arrlayer = layers.Split('/');
                for (int ii = 0; ii < arrlayer.Length; ii++)
                {
                    if (arrlayer[ii].Trim() == layer)
                    {
                        exist = true;
                        break;
                    }
                }
            }
            else
            {
                if (layers.Trim() == layer)
                    exist = true;
            }
            return exist;
        }
        //���ͼ��
        private void MenuItemAddLayer_Click(object sender, EventArgs e)
        {
            string strCurSubName = null;
            TreeNode tparentNode = null;
            if (treeViewControl.SelectedNode.Tag.Equals(2))
            {
                strCurSubName = treeViewControl.SelectedNode.Text;
                tparentNode = treeViewControl.SelectedNode;
            }
            if (treeViewControl.SelectedNode.Tag.Equals(3))
            {
                strCurSubName = treeViewControl.SelectedNode.Parent.Text;
                tparentNode = treeViewControl.SelectedNode.Parent;
            }
            AddLayerForm dlg = new AddLayerForm(m_xmldoc, strCurSubName);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                

                //����xml�ļ�
                string strSearchRoot = "//SubGroup[@sItemName = '" + strCurSubName + "']";
                XmlNode xmlNodeRoot = m_xmldoc.SelectSingleNode(strSearchRoot);
                XmlElement xmlElemGroup = (XmlElement)xmlNodeRoot;
                XmlNode boolnode = m_xmldoc.SelectSingleNode("//Layer[@sItemName='" + dlg.m_strLayerName + "']");
                if (boolnode != null)
                {
                    MessageBox.Show("��ͼ���Ѵ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                XmlElement xmlElemt = m_xmldoc.CreateElement("Layer");
                xmlElemt.SetAttribute("sDemo", dlg.m_strLayerDescri);
                xmlElemt.SetAttribute("sItemName", dlg.m_strLayerName);
                xmlElemt.SetAttribute("sDispScale", dlg.m_strScale);
                xmlElemt.SetAttribute("sDiaphaneity", dlg.m_strTransp);

                //��� �ļ����� ҵ��������  ҵ��С�����
                string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                OleDbConnection mycon = new OleDbConnection(constr);   //����OleDbConnection����ʵ�����������ݿ�
                string strExp = "";
                strExp = "select  * from ��׼ͼ����Ϣ�� where ���� = '" + dlg.m_strLayerName + "'";
                OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
                strExp = "select ͼ����� from ��׼ר����Ϣ�� where ר������='"+m_Typecode+"'";
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                string layer = db.GetInfoFromMdbByExp(constr, strExp);
                try
                {
                    mycon.Open();

                    //����datareader   ���������ӵ���     
                    OleDbDataReader aReader = aCommand.ExecuteReader();
                    while (aReader.Read())
                    {
                        xmlElemt.SetAttribute("sFile", aReader["����"].ToString());
                        xmlElemt.SetAttribute("sBigClass", aReader["ҵ��������"].ToString());
                        xmlElemt.SetAttribute("sSubClass", aReader["ҵ��С�����"].ToString());
                        
                        if (layer == "")
                            layer = aReader["����"].ToString();
                        else if (!GetExists(aReader["����"].ToString(), layer))
                            layer += "/" + aReader["����"].ToString();//����ͼ�����
                    }

                    //�ر�reader����     
                    aReader.Close(); 
                    //�ر�����,�����Ҫ     
                    mycon.Close();

                    //���±�׼ר����Ϣ��
                    strExp = "update ��׼ר����Ϣ�� set ͼ�����='" + layer + "' where ר������='" + m_Typecode + "'";
                    db.ExcuteSqlFromMdb(constr, strExp);
                }

                catch (System.Exception err)
                {
                    MessageBox.Show(err.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                   
                }
                //������
                TreeNode tNewNode = new TreeNode();
                tNewNode.Text = dlg.m_strLayerDescri;
                tNewNode.Name = dlg.m_strLayerName;
                tNewNode.Tag = 3;
                tNewNode.ImageIndex = 3;
                tNewNode.SelectedImageIndex = 3;
                tparentNode.Nodes.Add(tNewNode);
                tparentNode.ExpandAll();
                treeViewControl.Refresh();
               if(xmlElemGroup!=null)
                xmlElemGroup.AppendChild(xmlElemt);

                //����xml�ļ�
                m_xmldoc.Save(m_xmlPath);
            }
        }


        //���±�׼ͼ����Ϣ��
        private void UpdateLayersInfo()
        {
            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string layer = "";
            string strSearchRoot = "//Layer";
            XmlNodeList xmlNodelist = m_xmldoc.SelectNodes(strSearchRoot);
            foreach (XmlNode node in xmlNodelist)
            {
                XmlElement xmlelment = node as XmlElement;
                layer += xmlelment.GetAttribute("sFile") + "/";
            }
            if(layer.Contains("/"))
                layer=layer.Substring(0,layer.LastIndexOf("/"));
            strExp = "update ��׼ר����Ϣ�� set ͼ�����='" + layer + "' where ר������='" + m_Typecode + "'";
            db.ExcuteSqlFromMdb(constr, strExp);
           
        }

        //ɾ���ڵ�
        private void MenuItemTreeDelLayer_Click(object sender, EventArgs e)
        {
          
            if (treeViewControl.SelectedNode == null)
                return;
            string strSelItemName = treeViewControl.SelectedNode.Text;
            if (treeViewControl.SelectedNode.Tag.Equals(2))
            {
                string strTip = strSelItemName + "����������ͼ��Ҳ��һ��ɾ��!";
                if (MessageBox.Show(strTip, "��ʾ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    string strSearchRoot = "//SubGroup[@sItemName='" + strSelItemName + "']";
                    XmlNode xmlNodeRoot = m_xmldoc.SelectSingleNode(strSearchRoot);
                    if (xmlNodeRoot != null)
                    {
                        xmlNodeRoot.RemoveAll();
                        xmlNodeRoot.ParentNode.RemoveChild(xmlNodeRoot);
                    }

                    //ɾ���ӽڵ� ɾ���ýڵ�
                    treeViewControl.Nodes.Remove(treeViewControl.SelectedNode);
                }
            }
            else
            {
                //�޸Ķ�Ӧ��xml�ļ�
                string strSearchRoot = "//Layer[@sItemName='" + strSelItemName + "']";
                XmlNode xmlNodeRoot = m_xmldoc.SelectSingleNode(strSearchRoot);
                if (xmlNodeRoot != null)
                {
                    xmlNodeRoot.RemoveAll();
                    xmlNodeRoot.ParentNode.RemoveChild(xmlNodeRoot);
                }

                //ɾ���ڵ�
                treeViewControl.Nodes.Remove(treeViewControl.SelectedNode);
            }
            UpdateLayersInfo();//����ͼ����Ϣ��
            m_xmldoc.Save(m_xmlPath);
        }

        //����϶����ڵ�
        private void treeViewControl_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        private void treeViewControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void treeViewControl_DragDrop(object sender, DragEventArgs e)
        {
            if (m_LastDragNode != null)
            {
                m_LastDragNode.BackColor = SystemColors.Window;//ȡ����һ�������õĽڵ������ʾ   

                m_LastDragNode.ForeColor = SystemColors.WindowText;
                m_LastDragNode = null;
            }
            //����Ϸ��еĽڵ�  
            TreeNode moveNode = null;
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                moveNode = (TreeNode)(e.Data.GetData(typeof(TreeNode)));
            }
            else
            {
                MessageBox.Show("error");
            }
            if (moveNode.Level != 3)
                return;
            //�����������ȷ��Ҫ�ƶ�����Ŀ��ڵ�  
            Point pt;
            TreeNode targeNode;
            pt = ((TreeView)(sender)).PointToClient(new Point(e.X, e.Y));
            targeNode = treeViewControl.GetNodeAt(pt);

            //���Ŀ��ڵ����ӽڵ������Ϊͬ���ڵ�,��֮��ӵ��¼��ڵ��δ��  
            TreeNode NewMoveNode = (TreeNode)moveNode.Clone();
            switch (targeNode.Level)
            {
                case 3:
                    if (targeNode.Parent.Parent != moveNode.Parent.Parent)
                        return;
                    targeNode.Parent.Nodes.Insert(targeNode.Index, NewMoveNode);
                    break;
                case 2:
                    if (targeNode.Parent != moveNode.Parent.Parent)
                        return;
                    targeNode.Nodes.Insert(targeNode.Nodes.Count, NewMoveNode);
                    break;
            }

            //���µ�ǰ�϶��Ľڵ�ѡ��  
            treeViewControl.SelectedNode = NewMoveNode;

            //չ��Ŀ��ڵ�,������ʾ�Ϸ�Ч��  
            targeNode.Expand();

            //�Ƴ��ϷŵĽڵ�  
            moveNode.Remove();
        }

        private void treeViewControl_DragOver(object sender, DragEventArgs e)
        {


            //�������ͣ�� TreeView �ؼ���ʱ��չ���ÿؼ��е� TreeNode   
            TreeNode moveNode = null;
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                moveNode = (TreeNode)(e.Data.GetData(typeof(TreeNode)));
            }
            else
            {
                MessageBox.Show("error");
            }
            if (moveNode.Level != 3)
                return;

            Point p = this.treeViewControl.PointToClient(new Point(e.X, e.Y));

            TreeNode tn = this.treeViewControl.GetNodeAt(p);

            //�����Ϸ�Ŀ��TreeNode�ı���ɫ   


            if (m_LastDragNode != null && tn != m_LastDragNode)
            {
                m_LastDragNode.BackColor = SystemColors.Window;//ȡ����һ�������õĽڵ������ʾ   

                m_LastDragNode.ForeColor = SystemColors.WindowText;
            }
            else
            {
                moveNode.BackColor = SystemColors.Window;//ȡ����һ�������õĽڵ������ʾ   

                moveNode.ForeColor = SystemColors.WindowText;
            }

            if (m_LastDragNode != tn)
            {
                tn.BackColor = SystemColors.Highlight;

                tn.ForeColor = SystemColors.HighlightText;

            }
            m_LastDragNode = tn;




        }

        //�༭ͼ��
        private void MenuItemEditLayer_Click(object sender, EventArgs e)
        {
            string strCurSubName = null;//��ڵ��text
            TreeNode tparentNode = null;
            TreeNode tNode = null;

            strCurSubName = treeViewControl.SelectedNode.Parent.Text;
            tparentNode = treeViewControl.SelectedNode.Parent;
            tNode = treeViewControl.SelectedNode;
            string stroldText = tNode.Name;//��¼ԭ�нڵ�����
            AddLayerForm dlg = new AddLayerForm(m_xmldoc, tNode);
            if (dlg.ShowDialog() == DialogResult.OK)
            {

                tNode.Text = dlg.m_strLayerDescri;
                tNode.Name = dlg.m_strLayerName;
                tNode.Tag = 3;
                tNode.ImageIndex = 3;
                tNode.SelectedImageIndex = 3;
                tparentNode.ExpandAll();
                treeViewControl.Refresh();

                //����xml�ļ�
                //ɾ��ԭ�нڵ�
                string strSearch = "//Layer[@sItemName='" + stroldText + "']";
                XmlNode xmlNode = m_xmldoc.SelectSingleNode(strSearch);
                //xmlNode.ParentNode.RemoveChild(xmlNode);//
                //string strSearchRoot = "//SubGroup[@sItemName = '" + strCurSubName + "']";
                //XmlNode xmlNodeRoot = m_xmldoc.SelectSingleNode(strSearchRoot);
                //XmlElement xmlElemGroup = (XmlElement)xmlNodeRoot;
                //XmlElement xmlElemt= m_xmldoc.CreateElement("Layer");
                XmlElement xmlElemt = xmlNode as XmlElement;
                xmlElemt.SetAttribute("sDemo", tNode.Text);
                xmlElemt.SetAttribute("sDispScale", dlg.m_strScale);
                xmlElemt.SetAttribute("sDiaphaneity", dlg.m_strTransp);
                xmlElemt.SetAttribute("sItemName", tNode.Name);
                //xmlElemGroup.AppendChild(xmlElemt);

                //��� �ļ����� ҵ��������  ҵ��С�����
                string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                OleDbConnection mycon = new OleDbConnection(constr);   //����OleDbConnection����ʵ�����������ݿ�
                string strExp = "";
                strExp = "select  * from ��׼ͼ����Ϣ�� where ���� = '" + tNode.Name + "'";
                OleDbCommand aCommand = new OleDbCommand(strExp, mycon);

                strExp = "select ͼ����� from ��׼ר����Ϣ�� where ר������='" + m_Typecode + "'";
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                string layer = db.GetInfoFromMdbByExp(constr, strExp);
                try
                {
                    mycon.Open();

                    //����datareader   ���������ӵ���     
                    OleDbDataReader aReader = aCommand.ExecuteReader();
                    while (aReader.Read())
                    {
                        xmlElemt.SetAttribute("sFile", aReader["����"].ToString());
                        xmlElemt.SetAttribute("sBigClass", aReader["ҵ��������"].ToString());
                        xmlElemt.SetAttribute("sSubClass", aReader["ҵ��С�����"].ToString());
                        
                        if (layer == "")
                            layer = aReader["����"].ToString();
                        else if (!GetExists(aReader["����"].ToString(), layer))
                            layer += "/" + aReader["����"].ToString();//����ͼ�����
                    }

                    //�ر�reader����     
                    aReader.Close();

                    //�ر�����,�����Ҫ     
                    mycon.Close();

                    //���±�׼ר����Ϣ��
                    strExp = "update ��׼ר����Ϣ�� set ͼ�����='" + layer + "' where ר������='" + m_Typecode + "'";
                    db.ExcuteSqlFromMdb(constr, strExp);
                }

                catch (System.Exception err)
                {

                }

                //����xml�ļ�
                m_xmldoc.Save(m_xmlPath);
            }
        }

        //���óɹؼ�ͼ��
        private void MenuItemMainLayer_Click(object sender, EventArgs e)
        {
            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string layer = "";
           // strExp = "select �ؼ�ͼ�� from ��׼ר����Ϣ�� where ר������='" + m_Typecode + "'";
           //string  Mainlayer = db.GetInfoFromMdbByExp(constr, strExp);
            
            layer = treeViewControl.SelectedNode.Name;
            strExp = "select ���� from ��׼ͼ����Ϣ�� where ���� ='" + layer + "'";
            layer = db.GetInfoFromMdbByExp(constr, strExp);
            strExp = "update ��׼ר����Ϣ�� set �ؼ�ͼ��='" + layer + "' where ר������='" + m_Typecode + "'";
            db.ExcuteSqlFromMdb(constr, strExp);
            ChangeColor();
            treeViewControl.SelectedNode.ForeColor = Color.Red;
            MenuItemCanceMainLayer.Enabled = true;
            treeViewControl.ExpandAll();
            treeViewControl.Refresh();

        }
         //ȡ���ؼ�ͼ��
        private void MenuItemCanceMainLayer_Click(object sender, EventArgs e)
        {
            if (treeViewControl.SelectedNode.ForeColor.Equals(Color.Red))
            {
                treeViewControl.SelectedNode.ForeColor = Color.Black;
                string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                string strExp = "";
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                strExp = "update ��׼ר����Ϣ�� set �ؼ�ͼ��='' where ר������='" + m_Typecode + "'";
                db.ExcuteSqlFromMdb(constr, strExp);
            }
            MenuItemCanceMainLayer.Enabled = true;
            treeViewControl.Refresh();
        }
        
       //�ı����ڵ���ɫ
        private void ChangeColor()
        {
            try
            {
                foreach (TreeNode node in treeViewControl.Nodes[0].Nodes)
                {
                    foreach (TreeNode child in node.Nodes)
                    {
                        foreach (TreeNode childnode in child.Nodes)
                        {
                            childnode.ForeColor = SystemColors.WindowText;
                        }
                    }
                }
            }
            catch
            { }
        }
    

    }
}