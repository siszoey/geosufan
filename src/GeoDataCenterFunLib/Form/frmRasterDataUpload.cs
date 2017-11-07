using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;
using SysCommon;
using SysCommon.Gis;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geometry;

//*********************************************************************************
//** �ļ�����frmRasterDataUpload.cs
//** CopyRight (c) �人������Ϣ���̼������޹�˾�����������
//** �����ˣ�ϯʤ
//** ��  �ڣ�20011-04-11
//** �޸��ˣ�
//** ��  �ڣ�
//** ��  ����
//**
//** ��  ����1.0
//*********************************************************************************

namespace GeoDataCenterFunLib
{
    public partial class frmRasterDataUpload : DevComponents.DotNetBar.Office2007Form
    {
        public frmRasterDataUpload()
        {
            InitializeComponent();
            
        }

        private void frmRasterDataUpload_Load(object sender, EventArgs e)
        {
           
            string strExp = "select ����Դ���� from ��������Դ��";
            string mypath = m_dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            List<string> list = db.GetDataReaderFromMdb(strCon, strExp);
            for (int ii = 0; ii < list.Count; ii++)
            {
                comboBoxSource.Items.Add(list[ii]);//��������Դ�б��
            }
            if (list.Count > 0)
            {
                comboBoxSource.SelectedIndex = 0;//Ĭ��ѡ���һ��
            }
        }

        GetDataTreeInitIndex m_dIndex = new GetDataTreeInitIndex();//ȡ��·������
        frmDataReduction fdr=new frmDataReduction();//ɾ��ʱ��������������ɾ����ķ���
        //SysGisDataSet ds = new SysGisDataSet();
        OpenFileDialog OpenFile;
        int i = 0;
        bool m_success=false;
        bool m_newfile;

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
        //դ��������⵽GDB���ݿ�ķ���
        private void ImportRasterToNewWorkSpace(string file, string outfilename)
        {
            try
            {
                string ExportFileShortName = outfilename;
                if (file == "") { return; }
                int Index = file.LastIndexOf("\\");
                string ImportFileName = file.Substring(Index + 1);
                string ImportFilePath = System.IO.Path.GetDirectoryName(file);
                //�򿪴��ڵ�GDB�����ռ�
                //IWorkspaceFactory Pwf = new FileGDBWorkspaceFactoryClass();
                //IWorkspace pWorkspace = Pwf.OpenFromFile(GetSourcePath(comboBoxSource.Text), 0);
                IWorkspace pWorkspace = GetWorkspace(comboBoxSource.Text);
                IWorkspace2 pWorkspace2 = (IWorkspace2)pWorkspace;
                
                //�ж�Ҫ���Ƿ���ڣ������ڽ�ɾ��Դ�ļ�
                if (pWorkspace2.get_NameExists(esriDatasetType.esriDTRasterDataset, ImportFileName))
                {
                    if (m_newfile == true)
                    {
                        IRasterWorkspaceEx pRWs = pWorkspace as IRasterWorkspaceEx;
                        IDataset pDataset = pRWs.OpenRasterDataset(ImportFileName) as IDataset;
                        pDataset.CanDelete();
                        pDataset.Delete();
                        pDataset = null;
                    }
                    else
                    {
                        MessageBox.Show("������ͬ�ļ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        m_success = false;
                        return;
                    }
                }
                //IWorkspaceFactory pWorkspaceFactory = new RasterWorkspaceFactoryClass();
                //IWorkspace pWs = pWorkspaceFactory.OpenFromFile(ImportFilePath, 0);
                //IRasterDataset pRasterDs = null;
                //IRasterWorkspace pRasterWs;
                //IRasterWorkspaceEx pRasterEx=pWs as IRasterWorkspaceEx;
                //pRasterWs = pWs as IRasterWorkspace;
                //pRasterDs = pRasterWs.OpenRasterDataset(ImportFileName);
                //ISaveAs2 saveAs2 = (ISaveAs2)pRasterDs;
                ITrackCancel pTrackCancel = new TrackCancelClass();
                IRasterCatalogLoader pRasterload = new RasterCatalogLoaderClass();
                pRasterload.Workspace = pWorkspace;
                pRasterload.EnableBuildStatistics = true;
                pRasterload.StorageDef = new RasterStorageDefClass();
                pRasterload.StorageDef.CompressionType = esriRasterCompressionType.esriRasterCompressionLZ77;
                pRasterload.StorageDef.PyramidLevel = 9;
                pRasterload.StorageDef.CompressionQuality = 50;
                pRasterload.StorageDef.TileHeight = 128;
                pRasterload.StorageDef.TileWidth = 128;
                pRasterload.Projected = true;
                
                //����դ�����ݵ�catalog����
                pRasterload.Load(comboBoxCatalog.Text.Trim(), ImportFilePath, pTrackCancel);
                
                //��������դ������
                #region
                //IRasterStorageDef rasterStorageDef = new RasterStorageDefClass();
                //IRasterStorageDef2 rasterStorageDef2 = (IRasterStorageDef2)rasterStorageDef;
                ////դ��ѹ�����ݸ�ʽ�趨
                //string[] str = file.Split('.');
                //switch (str[1].ToLower())
                //{
                //    case "jpg":
                //        rasterStorageDef2.CompressionType = esriRasterCompressionType.esriRasterCompressionJPEG2000;
                //        break;
                //    case "sid":case "img":
                //        rasterStorageDef2.CompressionType = esriRasterCompressionType.esriRasterCompressionLZ77;
                //        break;
                //}
               
                //rasterStorageDef2.CompressionQuality = 50;
                //rasterStorageDef2.Tiled = true;
                //rasterStorageDef2.TileHeight = 128;
                //rasterStorageDef2.TileWidth = 128;
                //saveAs2.SaveAsRasterDataset(outfilename, pWorkspace, "gdb", rasterStorageDef2);
                #endregion

                m_success = true;
            }
            catch(Exception ex)
            {
                m_success = false;
            }
        }      
          
     


        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (comboBoxCatalog.Items.Count==0)
            {
                MessageBox.Show("����Դ��û��դ��Ŀ¼����������դ��Ŀ¼!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            OpenFile = new OpenFileDialog();
            OpenFile.Filter = "դ������|*.jpg;*.bmp;*.tif;*.sid;*.img;";
            OpenFile.Multiselect = true;  
             
            //��SHP�ļ�
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {

                foreach (string file in OpenFile.FileNames)
                {
                    for (int j = 0; j < i; j++)
                    {
                        string strExist = listView.Items[j].Text.Trim();
                        if (strExist.CompareTo(file) == 0)
                        {
                            MessageBox.Show("�ļ��Ѵ������б���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Cursor = Cursors.Default;
                            return;
                        }

                    }
                  
                    {
                        listView.Items.Add(file);
                        listView.Items[i].SubItems.Add("�ȴ����");
                        listView.Items[i].Checked = true;
                        i++;
                    }
                }
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                if (item.Checked)
                {
                    listView.Items.Remove(item);
                    i--;
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            listView.Items.Clear();
            i = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
          
            bool _check=false;
            foreach (ListViewItem item in listView.Items)
            {   
                
                string []str=item.Text.Split('.');
                str[1] = str[1].Substring(0, 3);
                if (item.Checked&&item.SubItems[1].Text=="�ȴ����")
                { 
                    _check = true;
                    item.SubItems[1].Text = "�������";
                    listView.Refresh();
                    ImportRasterToNewWorkSpace(item.Text,item.Text);
                    if(m_success)
                    item.SubItems[1].Text = "������";
                    else
                    item.SubItems[1].Text = "���ʧ��";
                    listView.Refresh();
                }
            }
            if (_check ==false)
                MessageBox.Show("��ѡ��Ҫ�����ļ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("���������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        //ȫѡ��ť
        private void btnAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView.Items.Count; i++)
            {
                listView.Items[i].Checked = true;
            }

        }
        //��ѡ��ť
        private void btnInverse_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView.Items.Count; i++)
            {
                if (listView.Items[i].Checked == false)
                {
                    listView.Items[i].Checked = true;
                    //datagwSource.Rows[i].Selected = true;
                }
                else
                {
                    listView.Items[i].Checked = false;
                    //datagwSource.Rows[i].Selected = false;
                }
            }
        }

        //��ʾդ��Ŀ¼
        private void comboBoxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxCatalog.Items.Clear();
            comboBoxCatalog.Text = "";

          
            //��ʼ��������
            SysCommon.CProgress vProgress = new SysCommon.CProgress("���ڼ���դ��Ŀ¼");
            vProgress.EnableCancel = false;
            vProgress.ShowDescription = true;
            vProgress.FakeProgress = true;
            vProgress.TopMost = true;
            vProgress.ShowProgress();

            //IWorkspaceFactory Pwf = new FileGDBWorkspaceFactoryClass();
            //IWorkspace pWorkspace = (IWorkspace)(Pwf.OpenFromFile(GetSourcePath(comboBoxSource.Text), 0));
            IWorkspace pWorkspace = GetWorkspace(comboBoxSource.Text);
            if (pWorkspace == null)
            {
                MessageBox.Show("����Դ�ռ䲻����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                vProgress.Close();
                this.Activate();
                return;
            }
            //��ȡ���е�raster catalogĿ¼
            comboBoxCatalog.Items.Clear();
            IEnumDataset enumDataset = pWorkspace.get_Datasets(esriDatasetType.esriDTRasterCatalog) as IEnumDataset;
            IDataset dataset = enumDataset.Next();
            while (dataset != null)
            {

                string catalog = dataset.Name;
                if (!comboBoxCatalog.Items.Contains(catalog))
                {
                    comboBoxCatalog.Items.Add(catalog);
                }
                dataset = enumDataset.Next();
            }
            if (comboBoxCatalog.Items.Count != 0)
                comboBoxCatalog.SelectedIndex = 0;
            vProgress.Close();
            this.Activate();
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
                    pPropertySet.SetProperty("Instance", "5151");//"port:" + txtService.Text
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

      
    }
}