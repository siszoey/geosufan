using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.DirectoryServices;

//*********************************************************************************
//** �ļ�����frmDataUpload.cs
//** CopyRight (c) �人������Ϣ���̼������޹�˾�����������
//** �����ˣ�ϯʤ
//** ��  �ڣ�20011-03-15
//** �޸��ˣ�
//** ��  �ڣ�
//** ��  ����
//**
//** ��  ����1.0
//*********************************************************************************
namespace GeoDataCenterFunLib
{
    public partial class frmNewDOCDirectory : DevComponents.DotNetBar.Office2007Form
    {
        public frmNewDOCDirectory()
        {
            InitializeComponent();
        }
        const String constIISWebSiteRoot = "IIS://localhost/W3SVC/1/ROOT";
        private void btnServer_Click(object sender, EventArgs e)
        {

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxDatasouce.Text = folderBrowserDialog.SelectedPath;
            }

        }

        private void textBoxDatasouce_TextChanged(object sender, EventArgs e)
        {
            //string text=textBoxDatasouce.Text;
            //ComBoxName.Text = text.Substring(text.LastIndexOf(@"\") + 1);
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            try
            {
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string strExp = "";
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
                if (ComBoxName.Text != "" && textBoxDatasouce.Text != "")
                {
                    //if (ComBoxName.Items.Contains(ComBoxName.Text))
                    //{
                    //    MessageBox.Show("����Ŀ¼���Ѵ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    return;
                    //}
                    string virtualDirName = ComBoxName.Text;//����Ŀ¼����
                    string physicalPath = @textBoxDatasouce.Text;
                    DirectoryEntry root = new DirectoryEntry(constIISWebSiteRoot);
                    if (GetVirtualDirectory(constIISWebSiteRoot, ComBoxName.Text))
                    {


                        foreach (DirectoryEntry tmpDir in root.Children)
                        {
                            if (virtualDirName == tmpDir.Name)
                            {
                                tmpDir.Properties["Path"][0] = physicalPath;
                                tmpDir.Invoke("AppCreate", true);
                                tmpDir.Properties["AccessRead"][0] = true;
                                tmpDir.Properties["AccessWrite"][0] = true;
                                tmpDir.Properties["AccessExecute"][0] = true;
                                //tbEntry.Properties["Access"]
                                tmpDir.Properties["ContentIndexed"][0] = true; ;
                                tmpDir.Properties["DefaultDoc"][0] = "";
                                tmpDir.Properties["AppFriendlyName"][0] = virtualDirName;
                                tmpDir.Properties["AccessScript"][0] = true;
                                tmpDir.Properties["DontLog"][0] = true;
                                // tbEntry.Properties["AuthFlags"][0] = 0;
                                tmpDir.Properties["AuthFlags"][0] = 1;
                                //tbEntry.Properties["DirBrower"]
                                tmpDir.CommitChanges();
                            }
                        }

                    }
                    else
                    {
                        DirectoryEntry tbEntry = root.Children.Add(virtualDirName, root.SchemaClassName);
                        tbEntry.Properties["Path"][0] = physicalPath;
                        tbEntry.Invoke("AppCreate", true);
                        tbEntry.Properties["AccessRead"][0] = true;
                        tbEntry.Properties["AccessWrite"][0] = true;
                        tbEntry.Properties["AccessExecute"][0] = true;
                        //tbEntry.Properties["Access"]
                        tbEntry.Properties["ContentIndexed"][0] = true; ;
                        tbEntry.Properties["DefaultDoc"][0] = "";
                        tbEntry.Properties["AppFriendlyName"][0] = virtualDirName;
                        tbEntry.Properties["AccessScript"][0] = true;
                        tbEntry.Properties["DontLog"][0] = true;
                        // tbEntry.Properties["AuthFlags"][0] = 0;
                        tbEntry.Properties["AuthFlags"][0] = 1;
                        //tbEntry.Properties["DirBrower"]
                        tbEntry.CommitChanges();
                    }
                    strExp = "insert into �ĵ�����Դ��Ϣ�� values('" + ComBoxName.Text + "','" + physicalPath + "')";
                }
                if (strExp != "")
                {
                    GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                    db.ExcuteSqlFromMdb(strCon, strExp);
                    MessageBox.Show("�����ɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("�����Ϊ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }



        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void frmNewDOCDirectory_Load(object sender, EventArgs e)
        {
            try
            {
                DirectoryEntry root = new DirectoryEntry(constIISWebSiteRoot);
                foreach (DirectoryEntry tmpDir in root.Children)
                {
                    if (tmpDir.Properties.Contains("Path"))//�жϴ治����·��
                        ComBoxName.Items.Add(tmpDir.Name);
                }
            }
            catch
            {
                MessageBox.Show("û���ҵ�IISĿ¼,��������IISĿ¼��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }

        }

        //ɾ������Դ
        private void btnDel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ComBoxName.Items.Contains(ComBoxName.Text))
                {
                    MessageBox.Show("����Ŀ¼�������ڣ��޷�ɾ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (!GetVirtualDirectory(constIISWebSiteRoot, ComBoxName.Text))
                {

                    MessageBox.Show("����Ŀ¼�������ڣ��޷�ɾ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string strExp = "";
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
                strExp = "delete * from �ĵ�����Դ��Ϣ�� where ����Ŀ¼��='" + ComBoxName.Text + "'";
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                db.ExcuteSqlFromMdb(strCon, strExp);
                strExp = "delete * from �ĵ������Ϣ�� where �ĵ�����Ŀ¼='" + ComBoxName.Text + "'";
                db.ExcuteSqlFromMdb(strCon, strExp);
                DirectoryEntry deRoot = new DirectoryEntry(constIISWebSiteRoot);
                deRoot.RefreshCache();
                DirectoryEntry Dirport = deRoot.Children.Find(ComBoxName.Text, "IIsWebVirtualDir");
                //deRoot.Invoke("AppDelete", true);
                deRoot.Children.Remove(Dirport);
                deRoot.CommitChanges();
                deRoot.Close();
                ComBoxName.Items.Remove(ComBoxName.Text);
                MessageBox.Show("ɾ���ɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        //���Ҷ�Ӧ������Ŀ¼ 
        private Boolean GetVirtualDirectory(string WebSiteName, string VirtualDirectory)
        {
            DirectoryEntry root = new DirectoryEntry(WebSiteName);
            try
            {
                foreach (DirectoryEntry tmpDir in root.Children)
                {
                    try
                    {
                        if (VirtualDirectory == tmpDir.Name)
                        {
                            return true;
                        }
                    }
                    catch { }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        //ѡ��ͬ����Ŀ¼����ʾ��ͬ·��
        private void ComBoxName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                if (GetVirtualDirectory(constIISWebSiteRoot, ComBoxName.Text))
                {
                    DirectoryEntry root = new DirectoryEntry(constIISWebSiteRoot);
                    foreach (DirectoryEntry tmpDir in root.Children)
                    {
                        if (ComBoxName.Text == tmpDir.Name)
                        {
                            textBoxDatasouce.Text = tmpDir.Properties["Path"][0].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
    }
}