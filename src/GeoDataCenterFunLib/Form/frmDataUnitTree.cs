using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

namespace GeoDataCenterFunLib
{
    //���ݵ�Ԫ��
    public partial class frmDataUnitTree : DevComponents.DotNetBar.Office2007Form
    {
        public frmDataUnitTree()
        {
            InitializeComponent();
            InitDataUnitTree();
        }
        //private TreeNode m_node;//ȫ�ֱ���ѡ��ǰ�ڵ�
        public int flag;
        public void InitDataUnitTree()
        {//�� ���ݵ�Ԫ�� �л�ȡ��Ϣ
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strDispLevel = dIndex.GetXmlElementValue("UnitTree", "tIsDisp");//�Ƿ���м���ʼ�������ݵ�Ԫ��
            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            OleDbConnection mycon = new OleDbConnection(constr);   //����OleDbConnection����ʵ�����������ݿ�
            string strExp = "";
            strExp = "select * from " + "���ݵ�Ԫ��";
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            try
            {
                mycon.Open();

                //����datareader   ���������ӵ���     
                OleDbDataReader aReader = aCommand.ExecuteReader();

                DataUnitTree.Nodes.Clear();
                TreeNode tparent;
                tparent = new TreeNode();
                tparent.Text = "����������";
                tparent.Tag = 0;

                TreeNode tRoot;
                tRoot = new TreeNode();
                tRoot = tparent;
                TreeNode tNewNode;
                TreeNode tNewNodeClild;
                TreeNode tNewLeafNode;
                DataUnitTree.Nodes.Add(tparent);
                DataUnitTree.ExpandAll();
                while (aReader.Read())
                {
                    //�����������������1���Ǹ��ڵ�
                    //�˴�Ĭ�϶��Ѿ����������������ά���������ֵ�ά��������ʵ��
                    if (aReader["���ݵ�Ԫ����"].ToString().Equals("1")) //ʡ���ڵ�
                    {
                        //if (strDispLevel.Equals("0"))
                        //{
                            tNewNode = new TreeNode();
                            tNewNode.Text = aReader["��������"].ToString();
                            tNewNode.Name = aReader["��������"].ToString();
                            tparent.Nodes.Add(tNewNode);
                            tparent.Expand();
                            tRoot = tNewNode;
                            tNewNode.Tag = 1;
                            tNewNode.ImageIndex = 17;
                            tNewNode.SelectedImageIndex = 17;
                        //}

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
                        tNewNodeClild.ImageIndex = 17;
                        tNewNodeClild.SelectedImageIndex = 17;
                    }
                    else if (aReader["���ݵ�Ԫ����"].ToString().Equals("3"))//�ؼ��ڵ�
                    {
                        tNewLeafNode = new TreeNode();
                        tNewLeafNode.Text = aReader["��������"].ToString();
                        tNewLeafNode.Name = aReader["��������"].ToString();
                        tRoot.Nodes.Add(tNewLeafNode);
                        tNewLeafNode.Tag = 3;
                        tNewLeafNode.ImageIndex = 17;
                        tNewLeafNode.SelectedImageIndex = 17;
                    }
                    else
                    {
                        tNewNodeClild = new TreeNode();
                        tNewNodeClild.Text = aReader["��������"].ToString();
                        tNewNodeClild.Name = aReader["��������"].ToString();
                        tparent.Nodes.Add(tNewNodeClild);
                        tparent.Expand();
                        tNewNodeClild.Tag = 1;
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

        private void DataUnitTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node!= null)
            {
                if (flag == 0)
                {

                    if (Convert.ToInt32(e.Node.Tag) != 0)
                    frmAnalyseInLibMap.Node = e.Node;
                }
                else if (flag == 1)
                    frmDataReduction.Node = e.Node;
                else if (flag == 2)
                    frmDocRedution.Node = e.Node;
                else
                {
                    if (Convert.ToInt32(e.Node.Tag) != 3)
                    {
                        MessageBox.Show("��ѡ��һ���ؼ��ڵ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                        frmDocUpload.Node = e.Node;
                }
            }

            this.Close();
        }
        //ȷ��
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (DataUnitTree.SelectedNode != null)
            {
                if (flag == 0)
                {
                    if (Convert.ToInt32(DataUnitTree.SelectedNode.Tag) != 0)
                    frmAnalyseInLibMap.Node = DataUnitTree.SelectedNode;
                }
                else if (flag == 1)
                    frmDataReduction.Node = DataUnitTree.SelectedNode;
                else if (flag == 2)
                    frmDocRedution.Node = DataUnitTree.SelectedNode;
                else
                {
                    if (Convert.ToInt32(DataUnitTree.SelectedNode.Tag) != 3)
                    {
                        MessageBox.Show("��ѡ��һ���ؼ��ڵ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                        frmDocUpload.Node = DataUnitTree.SelectedNode;
                }
               
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();

        }
    }
}