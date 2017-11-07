using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;
using System.Data.OleDb;
using System.IO;

namespace GeoDBConfigFrame
{
    public partial class AddGroupForm : DevComponents.DotNetBar.Office2007Form
    {
        public string m_mypath;
        public string m_strSubName;
        public AddGroupForm()
        {
            InitializeComponent();

            //��ʼ��fromtreeview����
            InitFromTreeView();
            
        }

        //��ʼ��fromtreeview����
        public void InitFromTreeView()
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            m_mypath = dIndex.GetDbInfo();

            //�ж��ļ��Ƿ����
            if (!File.Exists(m_mypath))
                return;

            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            OleDbConnection mycon = new OleDbConnection(constr);   //����OleDbConnection����ʵ�����������ݿ�
            string strExp = "";
            strExp = "select ����,���� from ��׼ͼ����Ϣ�� order by ҵ��С�����";
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            try
            {
                mycon.Open();
                TreeNode tNewNode;
                //����datareader   ���������ӵ���     
                OleDbDataReader aReader = aCommand.ExecuteReader();
                while (aReader.Read())
                {

                    tNewNode = new TreeNode();
                    tNewNode.Text = aReader["����"].ToString();
                    tNewNode.Name = aReader["����"].ToString();
                    fromtreeView.Nodes.Add(tNewNode);
                    fromtreeView.ExpandAll();
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

        //�������ڵ�
        private TreeNode FindNode( TreeNode tnParent, string strValue )
        {
            if( tnParent == null ) 
                return null;
            if( tnParent.Text == strValue ) 
                return tnParent;
     
            TreeNode tnRet = null;
            foreach( TreeNode tn in tnParent.Nodes )
            {
                tnRet = FindNode( tn, strValue );
                if( tnRet != null ) 
                    break;
            }
            return tnRet;
        }


        private void btnIn_Click(object sender, EventArgs e)
        {
            if (fromtreeView.SelectedNode != null)
            {
                //�жϽڵ��Ƿ������fromTreeView��
                bool bFlag = false;
                for (int ii = 0; ii < totreeView.Nodes.Count;ii++ )
                {
                    if (totreeView.Nodes[ii].Name.Equals(fromtreeView.SelectedNode.Name))
                    {
                        bFlag = true;
                        break;
                    }
                }

                if (bFlag == false)
                {
                    TreeNode tSleNode = null;
                    tSleNode = new TreeNode();
                    tSleNode.Text = fromtreeView.SelectedNode.Text;
                    tSleNode.Name = fromtreeView.SelectedNode.Name;
                    totreeView.Nodes.Add(tSleNode);
                }        
                totreeView.Refresh();
            }
        }

        private void btnOut_Click(object sender, EventArgs e)
        {
            if (totreeView.SelectedNode != null)
            {
                totreeView.Nodes.Remove(totreeView.SelectedNode);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (textBoxGroupName.Text.Trim().Equals(""))
            {
                MessageBox.Show("������������!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            m_strSubName = textBoxGroupName.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Hide();
            this.Dispose(true);
        }

        private void butnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
            this.Dispose(true);
        }
    }
}