using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;
using System.Data.OleDb;

namespace GeoDBConfigFrame
{
    public partial class ModifyDataUnitControl : DevComponents.DotNetBar.Office2007Form
    {
        public TreeNode m_NewRootNode;  //toTreeView�ĸ��ڵ�

        public ModifyDataUnitControl()
        {
            InitializeComponent();

            InitFromTree();

            InitToTree();
        }


        //��ʼ������
        public void InitFromTree()
        {
            //�� ���ݵ�Ԫ�� �л�ȡ��Ϣ
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            OleDbConnection mycon = new OleDbConnection(constr);   //����OleDbConnection����ʵ�����������ݿ�
            string strExp = "";
            strExp = "select * from " + "��׼���ݵ�Ԫ��";
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            try
            {
                mycon.Open();

                //����datareader   ���������ӵ���     
                OleDbDataReader aReader = aCommand.ExecuteReader();

                FromTreeView.Nodes.Clear();
                TreeNode tRoot;
                tRoot = new TreeNode();
                tRoot.Text = "���ݵ�Ԫ";
                tRoot.Tag = 0;
                tRoot.ImageIndex = 0;
                tRoot.SelectedImageIndex = 0;
                TreeNode tparent;
                tparent = new TreeNode();
                tparent = tRoot;

                TreeNode tCityNode;
                tCityNode = new TreeNode();
                tCityNode = tRoot;

                TreeNode tNewNode;
                TreeNode tNewNodeClild;
                TreeNode tNewLeafNode;

                FromTreeView.Nodes.Add(tRoot);
                FromTreeView.ExpandAll();
                while (aReader.Read())
                {
                    //�����������������1���Ǹ��ڵ�
                    //�˴�Ĭ�϶��Ѿ����������������ά���������ֵ�ά��������ʵ��
                    if (aReader["���ݵ�Ԫ����"].ToString().Equals("1")) //ʡ���ڵ�
                    {
                          tNewNode = new TreeNode();
                          tNewNode.Text = aReader["��������"].ToString();
                          tNewNode.Name = aReader["��������"].ToString();
                          tRoot.Nodes.Add(tNewNode);
                          tparent = tNewNode;
                          tNewNode.Tag = 1;
                          tNewNode.ImageIndex = 1;
                          tNewNode.SelectedImageIndex = 1;

                    }
                    else if (aReader["���ݵ�Ԫ����"].ToString().Equals("2")) //�м��ڵ�
                    {
                        tNewNodeClild = new TreeNode();
                        tNewNodeClild.Text = aReader["��������"].ToString();
                        tNewNodeClild.Name = aReader["��������"].ToString();
                        tparent.Nodes.Add(tNewNodeClild);
                        tNewNodeClild.Tag = 2;
                        tCityNode = tNewNodeClild;
                        tNewNodeClild.ImageIndex = 1;
                        tNewNodeClild.SelectedImageIndex = 1;
                    }
                    else if (aReader["���ݵ�Ԫ����"].ToString().Equals("3"))//�ؼ��ڵ�
                    {
                        tNewLeafNode = new TreeNode();
                        tNewLeafNode.Text = aReader["��������"].ToString();
                        tNewLeafNode.Name = aReader["��������"].ToString();
                        tCityNode.Nodes.Add(tNewLeafNode);
                        tNewLeafNode.Tag = 3;
                        tNewLeafNode.ImageIndex = 1;
                        tNewLeafNode.SelectedImageIndex = 1;
                    }
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

        public void InitToTree()
        {
            //�� ���ݵ�Ԫ�� �л�ȡ��Ϣ
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strDispLevel = dIndex.GetXmlElementValue("UnitTree", "tIsDisp");//�Ƿ���м���ʼ�������ݵ�Ԫ��
            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            OleDbConnection mycon = new OleDbConnection(constr);   //����OleDbConnection����ʵ�����������ݿ�
            string strExp = "";
            strExp = "select * from ���ݵ�Ԫ��";
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            try
            {
                mycon.Open();

                //����datareader   ���������ӵ���     
                OleDbDataReader aReader = aCommand.ExecuteReader();

                ToTreeView.Nodes.Clear();
                TreeNode tparent;
                tparent = new TreeNode();
                tparent.Text = "���ݵ�Ԫ";
                tparent.Tag = 0;
                tparent.ImageIndex = 0;
                tparent.SelectedImageIndex = 0;
                TreeNode tRoot;
                tRoot = new TreeNode();
                tRoot = tparent;
                TreeNode tNewNode;
                TreeNode tNewNodeClild;
                TreeNode tNewLeafNode;
                ToTreeView.Nodes.Add(tparent);
                ToTreeView.ExpandAll();
                while (aReader.Read())
                {
                    //�����������������1���Ǹ��ڵ�
                    //�˴�Ĭ�϶��Ѿ����������������ά���������ֵ�ά��������ʵ��
                    if (aReader["���ݵ�Ԫ����"].ToString().Equals("1")) //ʡ���ڵ�
                    {
                        if (strDispLevel.Equals("0"))
                        {
                            tNewNode = new TreeNode();
                            tNewNode.Text = aReader["��������"].ToString();
                            tNewNode.Name = aReader["��������"].ToString();
                            tparent.Nodes.Add(tNewNode);
                            tparent.Expand();
                            tparent = tNewNode;
                            tNewNode.Tag = 1;
                            tNewNode.ImageIndex = 1;
                            tNewNode.SelectedImageIndex = 1;
                        }

                    }
                    else if (aReader["���ݵ�Ԫ����"].ToString().Equals("2")) //�м��ڵ�
                    {
                        tNewNodeClild = new TreeNode();
                        tNewNodeClild.Text = aReader["��������"].ToString();
                        tNewNodeClild.Name = aReader["��������"].ToString();
                        tparent.Nodes.Add(tNewNodeClild);
                        tparent.Expand();
                        tRoot = tNewNodeClild;
                        tNewNodeClild.Tag = 2;
                        tNewNodeClild.ImageIndex = 1;
                        tNewNodeClild.SelectedImageIndex = 1;
                    }
                    else if (aReader["���ݵ�Ԫ����"].ToString().Equals("3"))//�ؼ��ڵ�
                    {
                        tNewLeafNode = new TreeNode();
                        tNewLeafNode.Text = aReader["��������"].ToString();
                        tNewLeafNode.Name = aReader["��������"].ToString();
                        tRoot.Nodes.Add(tNewLeafNode);
                        tNewLeafNode.Tag = 3;
                        tNewLeafNode.ImageIndex = 1;
                        tNewLeafNode.SelectedImageIndex = 1;
                    }
                    else
                    {
                        tNewNodeClild = new TreeNode();
                        tNewNodeClild.Text = aReader["��������"].ToString();
                        tNewNodeClild.Name = aReader["��������"].ToString();
                        tparent.Nodes.Add(tNewNodeClild);
                        tparent.Expand();
                        tNewNodeClild.Tag = 1;
                        tNewNodeClild.ImageIndex = 1;
                        tNewNodeClild.SelectedImageIndex = 1;
                    }
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

        //����
        private void btnIn_Click(object sender, EventArgs e)
        {
            //�� ���ݵ�Ԫ�� �л�ȡ��Ϣ
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strDispLevel = dIndex.GetXmlElementValue("UnitTree", "tIsDisp");//�Ƿ���м���ʼ�������ݵ�Ԫ��
            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select �������� from ���ݵ�Ԫ�� where ���ݵ�Ԫ����='1'";
            GeoDataCenterDbFun db=new GeoDataCenterDbFun();
            string strProvince = db.GetInfoFromMdbByExp(constr, strExp);

            //��ȡ��ǰ����ڵ� ʡ���нڵ�
            //ToTreeView.Nodes.Clear();
            TreeNode tFromSelNode = FromTreeView.SelectedNode;   
            if (tFromSelNode != null)
            {
                //��ʡ���ڵ� ���������е�
                if (tFromSelNode.Tag.Equals(1))
                {
                    ToTreeView.Nodes.Clear();
                    //��ȡ��ʡ���ڵ�������ӽڵ�
                    TreeNode tRootNode;
                    TreeNode tCityNode;
                    TreeNode tCountyNode;
                    tRootNode = new TreeNode();
                    tRootNode.Name = tFromSelNode.Name;
                    tRootNode.Text = tFromSelNode.Text;
                    tRootNode.Tag = tFromSelNode.Tag;
                    tRootNode.ImageIndex = tFromSelNode.ImageIndex;
                    ToTreeView.Nodes.Add(tRootNode);
                    m_NewRootNode = tRootNode;

                    int iNodeCount = tFromSelNode.GetNodeCount(false);
                    tCityNode = new TreeNode();
                    TreeNode tOldCityNode = tFromSelNode.FirstNode;
                    tCityNode.Name = tOldCityNode.Name;
                    tCityNode.Text = tOldCityNode.Text;
                    tCityNode.Tag = tOldCityNode.Tag;
                    tCityNode.ImageIndex = tOldCityNode.ImageIndex;
                    while (iNodeCount >0)
                    {
                        tRootNode.Nodes.Add(tCityNode);
                        int iLeafCount = tOldCityNode.GetNodeCount(false);
                        tCountyNode =  new TreeNode();
                        TreeNode tOldCountyNode = tOldCityNode.FirstNode;
                        tCountyNode.Name = tOldCountyNode.Name;
                        tCountyNode.Text = tOldCountyNode.Text;
                        tCountyNode.Tag = tOldCountyNode.Tag;
                        tCountyNode.ImageIndex = tOldCountyNode.ImageIndex;
                        tCountyNode.ExpandAll();
                        while (iLeafCount > 0)
                        {
                            tCityNode.Nodes.Add(tCountyNode);
                            if (tOldCountyNode != tOldCityNode.LastNode)
                            {
                                tOldCountyNode = tOldCountyNode.NextNode;
                                tCountyNode = new TreeNode();
                                tCountyNode.Name = tOldCountyNode.Name;
                                tCountyNode.Text = tOldCountyNode.Text;
                                tCountyNode.Tag = tOldCountyNode.Tag;
                                tCountyNode.ImageIndex = tOldCountyNode.ImageIndex;
                                tCountyNode.ExpandAll();
                            }
                            
                            iLeafCount--;
                        }

                        if (tOldCityNode != tFromSelNode.LastNode)
                        {
                            tOldCityNode = tOldCityNode.NextNode;
                            tCityNode = new TreeNode();
                            tCityNode.Name = tOldCityNode.Name;
                            tCityNode.Text = tOldCityNode.Text;
                            tCityNode.Tag = tOldCityNode.Tag;
                            tCityNode.ImageIndex = tOldCityNode.ImageIndex;
                            tCityNode.ExpandAll();
                        }
                        iNodeCount--;
                        tRootNode.Expand();
                       
                    }
                   // ToTreeView.Refresh();
                    
                }
                else if (tFromSelNode.Tag.Equals(2)) //���м��ڵ� Ҫ����ʡ��
                {
                    //��ȡʡ���ڵ� Ȼ����м��ڵ�����ӽڵ�
                    TreeNode tParentNode = tFromSelNode.Parent;
                    TreeNode tRootNode=null;
                    TreeNode tCityNode=null;
                    TreeNode tCountyNode=null;
                    if (ToTreeView.Nodes.Count>=1)
                    {
                        if (strProvince != (tParentNode.Text) && ToTreeView.Nodes[0].Text != tParentNode.Text)//�Ǹ�ʡ�ڵ�
                        {
                            ToTreeView.Nodes.Clear();
                            tRootNode = new TreeNode();
                            //����ʡ�ڵ�
                            tRootNode.Name = tParentNode.Name;
                            tRootNode.Text = tParentNode.Text;
                            tRootNode.Tag = tParentNode.Tag;
                            tRootNode.ImageIndex = tParentNode.ImageIndex;
                            ToTreeView.Nodes.Add(tRootNode);
                        }
                        else
                        {
                            tRootNode = ToTreeView.Nodes[0];
                        }
                        if (ToTreeView.Nodes[0].Nodes.Count > 0)//�Ѿ����нڵ�
                        {
                            foreach (TreeNode node in ToTreeView.Nodes[0].Nodes)
                            {
                                if ((node.Text.Trim() == tFromSelNode.Text.Trim()))//�нڵ��Ѵ��ڣ�
                                {
                                    DialogResult result = MessageBox.Show("�Ƿ��滻���нڵ�?", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                    if (result == DialogResult.No)//���滻
                                    {
                                        return;//������Ӧ��ֱ�ӷ���
                                    }
                                    else//�滻
                                    {
                                        node.Remove();//�Ƴ��ýڵ�
                                    }
                                }
                            }
                        }

                        tCityNode = new TreeNode();
                        //�����нڵ�
                        tCityNode.Name = tFromSelNode.Name;
                        tCityNode.Text = tFromSelNode.Text;
                        tCityNode.Tag = tFromSelNode.Tag;
                        tCityNode.ImageIndex = tFromSelNode.ImageIndex;
                        tRootNode.Nodes.Add(tCityNode);
                        tRootNode.Expand();
                    }
                    else
                    {
                        tRootNode = new TreeNode();
                        //����ʡ�ڵ�
                        tRootNode.Name = tParentNode.Name;
                        tRootNode.Text = tParentNode.Text;
                        tRootNode.Tag = tParentNode.Tag;
                        tRootNode.ImageIndex = tParentNode.ImageIndex;
                        ToTreeView.Nodes.Add(tRootNode);;

                        tCityNode = new TreeNode();
                        //�����нڵ�
                        tCityNode.Name = tFromSelNode.Name;
                        tCityNode.Text = tFromSelNode.Text;
                        tCityNode.Tag = tFromSelNode.Tag;
                        tCityNode.ImageIndex = tFromSelNode.ImageIndex;
                        tRootNode.Nodes.Add(tCityNode);
                        tRootNode.Expand();
                    }
                    m_NewRootNode = tRootNode;

                    //�����ؼ��ڵ�
                    int iNodeCount = tFromSelNode.GetNodeCount(false);
                    tCountyNode = new TreeNode();
                    TreeNode tOldCountyNode = tFromSelNode.FirstNode;
                    if (tOldCountyNode != null)
                    {
                        tCountyNode.Name = tOldCountyNode.Name;
                        tCountyNode.Text = tOldCountyNode.Text;
                        tCountyNode.Tag = tOldCountyNode.Tag;
                        tCountyNode.ImageIndex = tOldCountyNode.ImageIndex;
                    }
                    while (iNodeCount > 0)
                    {
                        tCityNode.Nodes.Add(tCountyNode);
                        tCountyNode = new TreeNode();
                        if (tOldCountyNode != tFromSelNode.LastNode)
                        {
                            tOldCountyNode = tOldCountyNode.NextNode;
                            tCountyNode.Name = tOldCountyNode.Name;
                            tCountyNode.Text = tOldCountyNode.Text;
                            tCountyNode.Tag = tOldCountyNode.Tag;
                            tCountyNode.ImageIndex = tOldCountyNode.ImageIndex;
                        }
                        iNodeCount--;
                    }
                }
                else if (tFromSelNode.Tag.Equals(3))  //�ؼ��ڵ�
                {
                    //��ȡʡ���ڵ� Ȼ����м��ڵ�����ӽڵ�
                    TreeNode tParentNode = tFromSelNode.Parent.Parent;
                    TreeNode tRootNode = null;
                    TreeNode tCityNode = null;
                    TreeNode tCountyNode = null;
                    if (ToTreeView.Nodes.Count >= 1)
                    {
                        if (strProvince != (tParentNode.Text) && ToTreeView.Nodes[0].Text != tParentNode.Text)//�Ǹ�ʡ�ڵ�
                        {
                            ToTreeView.Nodes.Clear();
                            tRootNode = new TreeNode();
                            //����ʡ�ڵ�
                            tRootNode.Name = tParentNode.Name;
                            tRootNode.Text = tParentNode.Text;
                            tRootNode.Tag = tParentNode.Tag;
                            tRootNode.ImageIndex = tParentNode.ImageIndex;
                            ToTreeView.Nodes.Add(tRootNode);
                        }
                        else
                        {
                            tRootNode = ToTreeView.Nodes[0];
                        }
                        if (ToTreeView.Nodes[0].Nodes.Count > 0)//�Ѿ����нڵ�
                        {
                            foreach (TreeNode node in ToTreeView.Nodes[0].Nodes)
                            {
                                if ((node.Text.Trim() == tFromSelNode.Parent.Text.Trim()))//�нڵ��Ѵ��ڣ�
                                {
                                    tCityNode = node;
                                }
                            }
                           
                        }
                        if (tCityNode == null)//û�и��нڵ㣬���һ���нڵ�
                        {
                            tCityNode = new TreeNode();
                            //�����нڵ�
                            tCityNode.Name = tFromSelNode.Parent.Name;
                            tCityNode.Text = tFromSelNode.Parent.Text;
                            tCityNode.Tag = tFromSelNode.Parent.Tag;
                            tCityNode.ImageIndex = tFromSelNode.Parent.ImageIndex;
                            tRootNode.Nodes.Add(tCityNode);
                            tRootNode.ExpandAll();
                            tCityNode.ExpandAll();
                        }

                    }
                    else
                    {
                        tRootNode = new TreeNode();
                        //����ʡ�ڵ�
                        tRootNode.Name = tParentNode.Name;
                        tRootNode.Text = tParentNode.Text;
                        tRootNode.Tag = tParentNode.Tag;
                        tRootNode.ImageIndex = tParentNode.ImageIndex;
                        ToTreeView.Nodes.Add(tRootNode);

                        tCityNode = new TreeNode();
                        //�����нڵ�
                        tCityNode.Name = tFromSelNode.Parent.Name;
                        tCityNode.Text = tFromSelNode.Parent.Text;
                        tCityNode.Tag = tFromSelNode.Parent.Tag;
                        tCityNode.ImageIndex = tFromSelNode.Parent.ImageIndex;
                        tRootNode.Nodes.Add(tCityNode);
                    }
                    
                    m_NewRootNode = tRootNode;
                    tRootNode.Expand();
                    //�����ؼ��ڵ�
                    if (tCityNode.Nodes.Count > 0)
                    {
                        foreach (TreeNode node in tCityNode.Nodes)
                        {
                            if ((node.Text.Trim() == tFromSelNode.Text.Trim()))//�ؽڵ��Ѵ��ڣ�
                            {
                                MessageBox.Show("�ýڵ��Ѵ���!", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }
                    tCountyNode = new TreeNode();
                    tCountyNode.Name = tFromSelNode.Name;
                    tCountyNode.Text = tFromSelNode.Text;
                    tCountyNode.Tag = tFromSelNode.Tag;
                    tCountyNode.ImageIndex = tFromSelNode.ImageIndex;
                    tCountyNode.ExpandAll();
                    tCityNode.Nodes.Add(tCountyNode);
                    tCityNode.ExpandAll();
                    tCountyNode.ExpandAll();

                }
            }
        }

        //�ѵ�ǰ�����ݵ�Ԫд�� �����ݵ�Ԫ��
        public void WriteDataIntoDataUnitTbl()
        {
            if(ToTreeView.GetNodeCount(true) > 0)  //�м�¼�͸���
            {
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                OleDbConnection mycon = new OleDbConnection(constr);   //����OleDbConnection����ʵ�����������ݿ�
                string strExp = "";
                strExp = "delete * from " + "���ݵ�Ԫ��";
                OleDbCommand aCommand = new OleDbCommand(strExp, mycon);

                try
                {
                    mycon.Open();

                    //ɾ����¼    
                    int  iRows = aCommand.ExecuteNonQuery();

                    //�����¼
                    SearchAllTreeNod(mycon,m_NewRootNode);

                    //�ر�����,�����Ҫ     
                    mycon.Close();
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                }
       
            }
        }

        //�ݹ�����������ڵ�
        public void  SearchAllTreeNod( OleDbConnection mycon,TreeNode   node) 
        {
            if (node == null)
                return;
            string strExp = "";
            strExp = "insert into ���ݵ�Ԫ�� (��������,��������,���ݵ�Ԫ����) values ('" + node.Name + "','" + node.Text + "','" + node.Tag + "')";
          
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            try
            {
                //ɾ����¼    
                int iRows = aCommand.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //id ���±��
    /*        strExp = "alter table [���ݵ�Ԫ��] alter column [ID] counter (1,1)";
            OleDbCommand aCommandbh = new OleDbCommand(strExp, mycon);
            try
            {
                //���±��    
                 aCommandbh.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
*/
            foreach(TreeNode   findNode   in   node.Nodes) 
            { 
                 //�������ȡn������ 
                 SearchAllTreeNod(mycon,findNode); 
            } 
        } 


        //���
        private void btnOut_Click(object sender, EventArgs e)
        {
            //ToTreeView.Nodes.Clear();
            //if (ToTreeView.SelectedNode.Nodes > 0)
            //{
            //    ToTreeView.SelectedNode.Nodes.Clear();
            //}
            try
            {
                ToTreeView.SelectedNode.Remove();
            }
            catch { }

        }

        //ȷ��
        private void btnOk_Click(object sender, EventArgs e)
        {
            //�ѵ�ǰ�����ݵ�Ԫд�� �����ݵ�Ԫ��
            WriteDataIntoDataUnitTbl();
            MessageBox.Show("�������!","��ʾ");
            this.DialogResult = DialogResult.OK;
            this.Hide();
            this.Dispose(true);
        }

        //ȡ��
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
            this.Dispose(true);
        }

        private void FromTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            labelCode.Text = "";
            if (FromTreeView.SelectedNode != e.Node)
            {
                if (FromTreeView.SelectedNode != null)
                {
                    FromTreeView.SelectedNode.ForeColor = Color.Black;
                }
            }

            FromTreeView.SelectedNode = e.Node;
            FromTreeView.SelectedNode.ForeColor = Color.Red;

            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select �������� from  ��׼���ݵ�Ԫ�� where ��������='" + FromTreeView.SelectedNode.Text + "' and ���ݵ�Ԫ����=" + FromTreeView.SelectedNode.Tag+ "";
            GeoDataCenterDbFun db=new GeoDataCenterDbFun();
            labelCode.Text = db.GetInfoFromMdbByExp(strCon, strExp);
           
        }

        private void ToTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            labelCode.Text = "";
            if (ToTreeView.SelectedNode != e.Node)
                ToTreeView.SelectedNode = e.Node;
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select �������� from  ��׼���ݵ�Ԫ�� where ��������='" + ToTreeView.SelectedNode.Text + "' and ���ݵ�Ԫ����=" + ToTreeView.SelectedNode.Tag + "";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            labelCode.Text = db.GetInfoFromMdbByExp(strCon, strExp);
        }
    }
}