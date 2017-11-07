using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using System.IO;
using ESRI.ArcGIS.esriSystem;

namespace GeoDataCenterFunLib
{
    public partial class frmSelectRaster : DevComponents.DotNetBar.Office2007Form
    {
        public frmSelectRaster()
        {
            InitializeComponent();
        }
        public string value="";
        bool flag = true;
        private void frmSelectRaster_Load(object sender, EventArgs e)
        {
            
            if(value!=""&&value.Contains("\\"))
            {
                string[] arrname = value.Split('\\');
                comboBoxSource.Text = arrname[0];
                comboBoxCatalog.Text = arrname[1];
            }
        }

        private void LoadComboBox()
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string strExp = "select ����Դ���� from ��������Դ��";
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            List<string> list = db.GetDataReaderFromMdb(strCon, strExp);
            for (int i = 0; i < list.Count; i++)
            {
                comboBoxSource.Items.Add(list[i]);//��������Դ�б��
            }
            if (list.Count > 0)
            {
                comboBoxSource.SelectedIndex = 0;//Ĭ��ѡ���һ��
            }
        }

        /// <summary>
        /// �õ�����Դ��ַ
        /// </summary>
        /// <param name="str">����Դ����</param>
        /// <returns></returns>
        private string GetSourcePath(string str)
        {
            try
            {
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                string strExp = "select ���ݿ� from ��������Դ�� where ����Դ����='" + str + "'";
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                string strname = db.GetInfoFromMdbByExp(strCon, strExp);
                return strname;
            }
            catch { return ""; }
        }

        /// <summary>
        /// �õ����ݿ�ռ� Added by xisheng 2011.04.28
        /// </summary>
        /// <param name="str">����Դ����</param>
        /// <returns>�����ռ�</returns>
        private IWorkspace GetWorkspace(string str)
        {
            try
            {
                IWorkspace pws = null;
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                string strExp = "select * from ��������Դ�� where ����Դ����='" + str + "'";
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
                string type = dt.Rows[0]["����Դ����"].ToString();
                if (type.Trim() == "GDB")
                {
                    IWorkspaceFactory pWorkspaceFactory;
                    pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                    pws = pWorkspaceFactory.OpenFromFile(dt.Rows[0]["���ݿ�"].ToString(), 0);
                }
                else if (type.Trim() == "SDE")
                {
                    IWorkspaceFactory pWorkspaceFactory;
                    pWorkspaceFactory = new SdeWorkspaceFactoryClass();

                    //PropertySet
                    IPropertySet pPropertySet;
                    pPropertySet = new PropertySetClass();
                    pPropertySet.SetProperty("Server", dt.Rows[0]["������"].ToString());
                    pPropertySet.SetProperty("Database", dt.Rows[0]["���ݿ�"].ToString());
                    pPropertySet.SetProperty("Instance", "esri_sde");//"port:" + txtService.Text
                    pPropertySet.SetProperty("user", dt.Rows[0]["�û�"].ToString());
                    pPropertySet.SetProperty("password", dt.Rows[0]["����"].ToString());
                    pPropertySet.SetProperty("version", "sde.DEFAULT");
                    pws = pWorkspaceFactory.Open(pPropertySet, 0);

                }
                return pws;
            }
            catch
            {
                return null;
            }
        }
        private void comboBoxSource_SelectedIndexChanged(object sender, EventArgs e)
        {

            comboBoxCatalog.Items.Clear();
            comboBoxCatalog.Text = "";
            if (comboBoxSource.Text.Trim() != "")
            {
                //IWorkspaceFactory Pwf = new FileGDBWorkspaceFactoryClass();
                //if(!Directory.Exists(GetSourcePath(comboBoxSource.Text)))
                //    return;
                //IWorkspace pWorkspace = (IWorkspace)(Pwf.OpenFromFile(GetSourcePath(comboBoxSource.Text), 0));
                IWorkspace pWorkspace = GetWorkspace(comboBoxSource.Text);
                if (pWorkspace == null) return;
                IEnumDataset enumDataset = pWorkspace.get_Datasets(esriDatasetType.esriDTRasterCatalog) as IEnumDataset;
                IDataset dataset = enumDataset.Next();
                while (dataset != null)
                {
                    comboBoxCatalog.Items.Add(dataset.Name);
                    dataset = enumDataset.Next();
                }
                if (comboBoxCatalog.Items.Count > 0)
                {
                    comboBoxCatalog.SelectedIndex = 0;
                }
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (comboBoxSource.Text.Trim() == "")
            {
                MessageBox.Show("����Դ����Ϊ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (comboBoxCatalog.Text == "")
            {
                MessageBox.Show("դ��Ŀ¼����Ϊ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            value = comboBoxSource.Text + "\\" + comboBoxCatalog.Text;
            this.DialogResult = DialogResult.OK;
            this.Hide();
            this.Dispose(true);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
            this.Dispose(true);
           
        }

        private void comboBoxSource_Click(object sender, EventArgs e)
        {
            if (flag)
            {
                this.Cursor = Cursors.WaitCursor;
                LoadComboBox();
                this.Cursor = Cursors.Default;
            }
            flag = false;
        }

    }
}