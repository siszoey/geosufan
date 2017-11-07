using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.OleDb;
using System.Collections.Generic;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using System.Xml;
using System.IO;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataManagementTools;
using System.Runtime.Serialization.Formatters.Binary;
using SysCommon.Error;
using System.Threading;

namespace GeoDBIntegration
{

    /// <summary>
    /// ���ߣ�yjl
    /// ���ڣ�20110716
    /// ˵�������ݿⱸ�ݺͻ�ԭ
    /// </summary
    public partial class frmOraBk_Rc : DevComponents.DotNetBar.Office2007Form
    {
        private string strColName = "����";
        private string m_strTemplate = "";
        IWorkspace pWks=null;
        IFeatureWorkspace pFW = null;
        XmlDocument xDoc = null;
        XmlNodeList xnl = null;
        private string userID = "";
        private string connStr;
        //sde
        private IWorkspace pSDEWs = null;
        private IWorkspace pGDBWs = null;//file gdb

        //oracle��ԭ
        private string files = "";//dmp files
        private string logfile = "";
        
        public frmOraBk_Rc()
        {
            InitializeComponent();
            rdoFull.Checked = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //FrmLogin fmLgon = new FrmLogin();
            //fmLgon.ShowDialog();
            //if (!fmLgon.isLogin)
            //    this.Close();
            //else
            //{
            //    userID = fmLgon.Login;
            //    connStr = fmLgon.ConStr;
 
            //}
            gPOraCon.Dock = DockStyle.Fill;
            gPOraBak.Dock = DockStyle.Fill;
            gPOraRc.Dock = DockStyle.Fill;
            gPSDEcon.Dock = DockStyle.Fill;
            gPSDEds.Dock = DockStyle.Fill; ;
            gPSdeRc.Dock = DockStyle.Fill;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            Process pBat = new Process();
            pBat.StartInfo.FileName = Application.StartupPath + "\\bkpFull.bat";
            pBat.StartInfo.UseShellExecute = true;
            pBat.StartInfo.CreateNoWindow = true;
            pBat.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (pBat.Start())
                lblBottom.Text = "���ݳɹ���";
            else
                lblBottom.Text = "����ʧ�ܣ�";
           
           
            
        }
       


        private void buttonX1_Click(object sender, EventArgs e)
        {
            
        }

     


        private void LstAllLyrFile(IWorkspace pWks)
        {
            this.lstLyrFile.Items.Clear();
            IFeatureWorkspace pFeaWks = pWks as IFeatureWorkspace;
            if (pFeaWks == null) return;

            IEnumDatasetName pEnumFeaCls = pWks.get_DatasetNames(esriDatasetType.esriDTFeatureClass);
            IDatasetName pFeaClsName = pEnumFeaCls.Next();
            while (pFeaClsName != null)
            {
                ListViewItem lvi = this.lstLyrFile.Items.Add(pFeaClsName.Name);

                lvi.Tag = pFeaWks.OpenFeatureClass(pFeaClsName.Name);
                pFeaClsName = pEnumFeaCls.Next();
            }

            IEnumDatasetName pEnumDataNames = pWks.get_DatasetNames(esriDatasetType.esriDTFeatureDataset);
            IDatasetName pDatasetName = pEnumDataNames.Next();
            while (pDatasetName != null)
            {
                IEnumDatasetName pSubNames = pDatasetName.SubsetNames;
                IDatasetName pSubName = pSubNames.Next();
                while (pSubName != null)
                {
                    ListViewItem lvi=this.lstLyrFile.Items.Add(pSubName.Name);
                    lvi.Tag = pFeaWks.OpenFeatureClass(pFeaClsName.Name);
                    pSubName = pSubNames.Next();
                }

                pDatasetName = pEnumDataNames.Next();
            }

            for (int i = 0; i < this.lstLyrFile.Items.Count; i++)
            {
                this.lstLyrFile.Items[i].Checked = false;
            }
        }

        private void lstLyrFile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                foreach (ListViewItem item in this.lstLyrFile.Items)
                {
                    item.Selected = true;
                }
            }
        }

        private void lstLyrFile_ColumnClick(object sender, ColumnClickEventArgs e)
        {

        }

        private void InitListViewStyle(ListView plv)
        {
            // ʧ����ʱ��������ѡ��
            plv.HideSelection = false;

            // Set the view to show details.
            plv.View = View.Details;
            // Allow the user to edit item text.
            plv.LabelEdit = false;
            // Allow the user to rearrange columns.
            plv.AllowColumnReorder = true;
            // Display check boxes.
            plv.CheckBoxes = true;   //_heluyuan_20071117_modify
            // Select the item and subitems when selection is made.
            plv.FullRowSelect = true;
            // Sort the items in the list in ascending order.
            plv.Sorting = SortOrder.Ascending;

            plv.Columns.Add(strColName, -2, HorizontalAlignment.Center);
            //plv.Columns.Add(strAliasName, -2, HorizontalAlignment.Left);
        }

        private void cmbTarget_Click(object sender, EventArgs e)
        {

          
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lstLyrFile_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmdSource_Click(object sender, EventArgs e)
        {
           
           
        }
        private IWorkspace GetWorkspace(string sFilePath, int wstype)
        {
            pWks = null;

            try
            {
                IPropertySet pPropSet = new PropertySetClass();
                switch (wstype)
                {
                    case 1:
                        AccessWorkspaceFactory pAccessFact = new AccessWorkspaceFactoryClass();
                        pPropSet.SetProperty("DATABASE", sFilePath);
                        pWks = pAccessFact.Open(pPropSet, 0);
                        pAccessFact = null;
                        break;
                    case 2:
                        FileGDBWorkspaceFactoryClass pFileGDBFact = new FileGDBWorkspaceFactoryClass();
                        pPropSet.SetProperty("DATABASE", sFilePath);
                        pWks = pFileGDBFact.Open(pPropSet, 0);
                        pFileGDBFact = null;
                        break;
                    case 3:
                        IWorkspaceFactory shpWF = new ShapefileWorkspaceFactoryClass();
                        pWks = shpWF.OpenFromFile(sFilePath, 0);
                        shpWF = null;
                        break;
                }
                pPropSet = null;
                return pWks;
            }
            catch
            {
                return null;
            }
        }

        private void btnRecover_Click(object sender, EventArgs e)
        {
            Process pBat = new Process();
            pBat.StartInfo.FileName = Application.StartupPath + "\\rcrFull.bat";
            pBat.StartInfo.UseShellExecute = true;
            pBat.StartInfo.CreateNoWindow = true;
            pBat.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (pBat.Start())
                lblBottom.Text = "��ԭ�ɹ���";
            else
                lblBottom.Text = "��ԭʧ�ܣ�";
        }
       
        private void btnOraBkOK_Click(object sender, EventArgs e)
        {
            if (txtDmpPath.Text == "")
                return;
            
            rTxtLog.Text = "";
            string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");//.Replace(':','_').Replace(' ','_');
            string dmpPath = txtDmpPath.Text + "\\" + date;
            if (!Directory.Exists(dmpPath))
                Directory.CreateDirectory(dmpPath);
            //����Ŀ¼����¶���
            OleDbConnection conn = new OleDbConnection(connStr);
            conn.Open();
            OleDbCommand pCmd = conn.CreateCommand();
            pCmd.CommandText = "create or replace directory SDElogicbakdir as '" + dmpPath + "'";
            try
            {
                pCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
                return;
            }
            finally
            {
                conn.Close();
            }
            //����Ŀ¼����end
            string expdp = userID;//"expdp " + 
            if (rdoFull.Checked)
                expdp += " FULL=Y ";
            else
            {
                if (lstUsr.CheckedItems.Count == 0)
                    return;
                List<string> users = new List<string>();
                expdp += " schemas=(";
                for (int i = 0; i < lstUsr.Items.Count;i++)
                {
                    ListViewItem lvi = lstUsr.Items[i];
                    if (lvi.Checked)
                    {
                        expdp += lvi.Text + ",";
                        users.Add(lvi.Text);
                    }
                }
                Stream stmp = File.Open(dmpPath + "\\expdpUsers.dat", FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stmp, users);
                stmp.Close();//�����û����б�Ϊ���ܻ�ԭ��ʱ����ʾ

                expdp = expdp.Substring(0, expdp.Length - 1);
                expdp += ")";
            }
         
            string pams = " directory=SDElogicbakdir  dumpfile=expdp" + date + @"_%U.dmp,";//����bat��Ҫ2��%
              //for(int num=1;num<2;num++)//�ļ��б�_01.dmp,_02.dmp,...
              //{
              //    pams +="expdp" + date + @"_" + num.ToString("0#") + ".dmp,";
              //}
              //pams = pams.Substring(0, pams.Length - 1);
              pams += "  logfile=expdp" + date + ".log ";
              logfile = dmpPath + "\\expdp" + date + ".log ";//Ϊ����Ӧ�鿴��־��ť
            expdp += pams;
            //string  batPath=Application.StartupPath + "\\expdpOra.bat";
            //Stream s = new System.IO.FileStream(batPath, FileMode.Create);
            //StreamWriter sw = new StreamWriter(s);
            ////sw.Encoding = Encoding.ASCII;
            //sw.WriteLine(expdp);
            //sw.Close();
            //s.Close();
            lblBottom.Text = "���ڱ���...";
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            //Process pBat = new Process();
            pBat.StartInfo.FileName = "expdp";
            pBat.StartInfo.Arguments = expdp;
            //pBat.StartInfo.UseShellExecute = false;
            pBat.StartInfo.CreateNoWindow = false;
            //pBat.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pBat.StartInfo.RedirectStandardOutput = false;
            pBat.Start();
            //this.Height = 557;
            //Application.DoEvents();
            while (!pBat.WaitForExit(0))
            {
                this.Enabled = false;
                //rTxtLog.AppendText(pBat.StandardOutput.ReadLine() + "\r\n");
                //rTxtLog.ScrollToCaret();
            }
            //rTxtLog.AppendText(pBat.StandardOutput.ReadToEnd());
            //rTxtLog.ScrollToCaret();
            pBat.Close();
            lblBottom.Text = "������ɡ�";
            Application.DoEvents();
            this.Enabled = true;
            this.Cursor = Cursors.Default;
            btnLookLog.Visible = true;
        }

        private void rdoFull_CheckedChanged(object sender, EventArgs e)
        {
            lstUsr.Items.Clear();
            if (rdoFull.Checked)
                lstUsr.Enabled = false;
        }

        private void rdoUsr_CheckedChanged(object sender, EventArgs e)
        {
            lstUsr.Items.Clear();
            if (rdoUsr.Checked)
            {
                lstUsr.Enabled = true;
                OleDbConnection pOC = new OleDbConnection(connStr);
                if (connStr == null)
                {
                    MessageBox.Show("�������ӵ����ݿ�!", "��ʾ", MessageBoxButtons.OK,
                          MessageBoxIcon.Information);
                    return;
                }
                pOC.Open();
                //OleDbCommand pCmd = pOC.CreateCommand();
                OleDbDataReader pODR = GetReader(pOC, "select username from all_users");
                while (pODR.Read())
                {
                    if (pODR.GetString(0).ToUpper() == "SDE")//Ĭ��ѡ��SDE�û�
                        lstUsr.Items.Add(pODR.GetString(0)).Checked = true;
                    else
                        lstUsr.Items.Add(pODR.GetString(0));
                }
                pODR.Close();
                pOC.Close();
            }
        }
        private bool existUser(string user)
        {
            List<string> tmp = new List<string>();
            OleDbConnection pOC = new OleDbConnection(connStr);
            pOC.Open();
            //OleDbCommand pCmd = pOC.CreateCommand();
            OleDbDataReader pODR = GetReader(pOC, "select username from all_users");
            while (pODR.Read())
            {
                tmp.Add(pODR.GetString(0).ToUpper());
                
            }
            pODR.Close();
            pOC.Close();
            if (tmp.Contains(user))//Ĭ��ѡ��SDE�û�
                    return true;
            else
                    return false;
            
        }
        //�������ܣ���ȡ�α�
        //������������ݿ�����  sql��� //�������������sql���򿪵ı���α�
        public static OleDbDataReader GetReader(OleDbConnection conn, string sqlstr)
        {
            OleDbCommand comm = conn.CreateCommand();
            comm.CommandText = sqlstr;
            OleDbDataReader myreader;
            try
            {
                myreader = comm.ExecuteReader();
                return myreader;
            }
            catch (System.Exception e)
            {
                e.Data.Clear();
                return null;
            }
        }

        private void btnDmpPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            //folder.ShowNewFolderButton = false;
            folder.Description = "�����ļ���";
            if (folder.ShowDialog() == DialogResult.OK)
            {
                string str = folder.SelectedPath;
                this.txtDmpPath.Text = str;
                //OleDbConnection conn = new OleDbConnection(connStr);
                //conn.Open();
                //OleDbCommand pCmd = conn.CreateCommand();
                //pCmd.CommandText = "create directory logicbakdir as '" + txtDmpPath.Text + "'";
                //try
                //{
                //    pCmd.ExecuteNonQuery();
                //}
                //catch
                //{ }
            }
        }

        private void btnOraBkCancel_Click(object sender, EventArgs e)
        {
            gPOraCon.BringToFront();
            lblBottom.Text = "";
            btnLookLog.Visible = false;
            btnLookLogRc.Visible = false;
        }

        private void tabControl1_TabIndexChanged(object sender, EventArgs e)
        {
         
        }
        /// <summary>
        /// ����SDE������
        /// </summary>
        /// <param name="sServer">��������</param>
        /// <param name="sService">������</param>
        /// <param name="sDatabase">���ݿ���(SQLServer)</param>
        /// <param name="sUser">�û���</param>
        /// <param name="sPassword">����</param>
        /// <param name="strVersion">SDE�汾</param>
        /// <returns>�������Exception</returns>
        public IWorkspace SetWorkspace(string sServer, string sService, string sDatabase, string sUser, string sPassword, string strVersion)
        {
            //eError = null;
            IPropertySet pPropSet = new PropertySetClass();
            IWorkspaceFactory pSdeFact = new SdeWorkspaceFactoryClass();
            pPropSet.SetProperty("SERVER", sServer);
            pPropSet.SetProperty("INSTANCE", sService);
            pPropSet.SetProperty("DATABASE", sDatabase);
            pPropSet.SetProperty("USER", sUser);
            pPropSet.SetProperty("PASSWORD", sPassword);
            pPropSet.SetProperty("VERSION", strVersion);

            try
            {
                return  pSdeFact.Open(pPropSet, 0);
             
            }
            catch (Exception eX)
            {
                //********************************
                //guozheng added  system exception log
                //if (SysCommon.Log.Module.SysLog == null)
                //    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                //SysCommon.Log.Module.SysLog.Write(eX);
                //********************************
                
                return null;
            }
        }
        private void btnCloseLog_Click(object sender, EventArgs e)
        {
            this.Height = 331;
        }

        private void btnSelAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstUsr.Items.Count; i++)
            {
                ListViewItem lvi = lstUsr.Items[i];
                if (!lvi.Checked)
                    lvi.Checked=true;
            }
        }

        private void btnSelReverse_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstUsr.Items.Count; i++)
            {
                ListViewItem lvi = lstUsr.Items[i];
                if (lvi.Checked)
                    lvi.Checked = false;
                else
                    lvi.Checked = true;
            }
        }

        private void btnGDB_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            //folder.ShowNewFolderButton = false;
            folder.Description = "�����ļ���";
            if (folder.ShowDialog() == DialogResult.OK)
            {
                string str = folder.SelectedPath;
                this.txtGdbPath.Text = str;
                
            }
        }

        private void tabItemSDE_Click(object sender, EventArgs e)
        {
           
        }

        private void btnSdeBakOk_Click(object sender, EventArgs e)
        {
            if (lstLyrFile.SelectedItems.Count == 0)
                return;
            lblSDE.Text = "";
            this.Cursor = Cursors.WaitCursor;
            this.Enabled = false;
            
            //ESRI.ArcGIS.Geoprocessor.Geoprocessor _geoPro = new ESRI.ArcGIS.Geoprocessor.Geoprocessor();
            ////ESRI.ArcGIS.Geoprocessing.IGeoProcessor _geoPro = new ESRI.ArcGIS.Geoprocessing.GeoProcessorClass();
            //ITrackCancel pTrackCancel = new TrackCancelClass();
            //IGPEnvironmentManager pgpEnv = new GPEnvironmentManager();
            //IGPMessages pGpMessages; //= _geoPro.Validate(parameters, false, pgpEnv);
            //IGPComHelper pGPCOMHelper = new GpDispatch();

            ////�����ǹؼ����������ֵ�Ļ�����ô�ͻᱨ��
            //IGPEnvironmentManager pEnvMgr = pGPCOMHelper.EnvironmentManager;
            //pgpEnv.PersistAll = true;
            //pGpMessages = new GPMessagesClass();
            //_geoPro.OverwriteOutput = true;
            SysCommon.CProgress pgss = new SysCommon.CProgress("���ڱ���SDE����...");
            pgss.EnableCancel = false;
            pgss.ShowDescription = false;
            pgss.FakeProgress = true;
            pgss.TopMost = true;
            pgss.ShowProgress();
            Application.DoEvents();
            try
            {
                string date = DateTime.Now.ToString().Replace(':', '_').Replace(' ', '_');
                IWorkspace pGDBWs = CreateFileGDBWorkSpace("gdbbak" + date + ".gdb");
                for (int i = 0; i < lstLyrFile.Items.Count; i++)
                {
                    ListViewItem lvi = lstLyrFile.Items[i];
                    if (!lvi.Checked)
                        continue;
                    IFeatureWorkspace pSDEFW = pSDEWs as IFeatureWorkspace;
                    IFeatureDataset pFD = pSDEFW.OpenFeatureDataset(lvi.Text);

                    ISpatialReference pSR = (pFD as IGeoDataset).SpatialReference;
                    IFeatureWorkspace pGDBFW = pGDBWs as IFeatureWorkspace;
                    //CopyPasteGDBData.CopyPasteGeodatabaseData(pSDEWs, pGDBWs, lvi.Text, esriDatasetType.esriDTFeatureDataset);
                    //IFeatureDataset pGDBfd = pGDBFW.CreateFeatureDataset(lvi.Text.Split('.')[1], pSR);
                    //CopyFeatures copyFea = new CopyFeatures();
                    //IEnumDataset pED = pFD.Subsets;
                    //IDataset pD = pED.Next();
                    //while (pD != null)
                    //{
                    //    lblSDE.Text = "���ڱ���Ҫ�ؼ�" + pFD.Name + "�µ�Ҫ����" + pD.Name;
                    //    Application.DoEvents();
                    //    string pdName = pD.Name.Split('.')[1];
                    //    //copyFea.in_features = pD;
                    //    //copyFea.out_feature_class = pGDBWs.PathName + "\\" + pGDBfd.Name + "\\" + pdName;
                    //    //_geoPro.Execute(copyFea, pTrackCancel);
                        
                    //    pD = pED.Next();
                    //}
                    //copyFea = null;
                    
                }
                
                lblSDE.Text = "������ɣ�";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

            }
            finally
            {
                pgss.Close();
                //_geoPro = null;
                this.Cursor = Cursors.Default;
                this.Enabled = true;
                Application.DoEvents();
            }
        }
        
        public IWorkspace CreateFileGDBWorkSpace(string filename)
        {
           
            IWorkspaceFactory pWorkspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactoryClass();
            
            IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create("" + txtGdbPath.Text + "", "" + filename + "", null, 0);
            IName name = (ESRI.ArcGIS.esriSystem.IName)pWorkspaceName;

            IWorkspace fileGDB_workspace = (IWorkspace)name.Open();
            return fileGDB_workspace;
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void buttonX1_Click_1(object sender, EventArgs e)
        {

            if (txtUsr.Text == "" || txtPwd.Text == "" || txtSrv.Text == "")
            {
                lblTestOraCon.Text = "����д�������Ӳ�����";
                return;
            }
            try
            {
                connStr = "Provider=OraOLEDB.Oracle;Data Source=" + txtSrv.Text + ";User Id=" + txtUsr.Text
                    + ";Password=" + txtPwd.Text + ";OLEDB.NET=True;";
                OleDbConnection pConn = new OleDbConnection(connStr);
                pConn.Open();
                pConn.Close();
                //isValid = true;
                userID = txtUsr.Text + "/" + txtPwd.Text + "@" + txtSrv.Text;
                lblTestOraCon.Text = "���ӳɹ���";
                if (rdoBackup.Checked)
                {
                    gPOraBak.BringToFront();
                    rdoUsr.Checked = true;
                }
                else
                    gPOraRc.BringToFront();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK,
                         MessageBoxIcon.Information);

            }

          
        }

        private void buttonX1_MouseLeave(object sender, EventArgs e)
        {
            lblTestOraCon.Text = "";
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            if (txtPC.Text == "" || txtSDESrvs.Text == "" || txtDB.Text == "" || txtUr.Text == "" || txtPswd.Text == "" || txtVersion.Text == "")
            {
                lblSDETest.Text ="����д�������Ӳ�����";
                return;
            }

            pSDEWs = SetWorkspace(txtPC.Text, txtSDESrvs.Text, txtDB.Text, txtUr.Text, txtPswd.Text, txtVersion.Text);
            if (pSDEWs == null)
            {
                lblSDETest.Text ="����ʧ�ܣ��������Ӳ�����";
                this.Cursor = Cursors.Default;
                return;
            }
            if (rdoSdeBak.Checked)
            {
                this.Cursor = Cursors.WaitCursor;
                lstLyrFile.Items.Clear();

                IEnumDatasetName pEDN = pSDEWs.get_DatasetNames(esriDatasetType.esriDTFeatureDataset);
                IDatasetName pDN = pEDN.Next();
                while (pDN != null)
                {
                    lstLyrFile.Items.Add(pDN.Name);
                    pDN = pEDN.Next();
                }
                lstLyrFile.Refresh();
                this.Cursor = Cursors.Default;
                gPSDEds.BringToFront();
            }
            else
            {
                gPSdeRc.BringToFront();
            }
        }

        private void btnTSde_Leave(object sender, EventArgs e)
        {
            lblSDETest.Text = "";
        }
        private void executeSQL(string sql)
        {
            OleDbConnection conn = new OleDbConnection(connStr);
            conn.Open();
            OleDbCommand pCmd = conn.CreateCommand();
            pCmd.CommandText = sql;
            try
            {
                pCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
                
            }
            finally
            {
                conn.Close();
            }
        }
        private void btnBakDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "��ԭĿ¼��";
            if (folder.ShowDialog() == DialogResult.OK)
            {
                lstRcUsr.Items.Clear();
                lstRcUsr.Items.Add("SDE").SubItems.Add("SDE");
                string str = folder.SelectedPath;
                this.txtRcDir.Text = str;
                //����Ŀ¼����¶���
                OleDbConnection conn = new OleDbConnection(connStr);
                conn.Open();
                OleDbCommand pCmd = conn.CreateCommand();
                pCmd.CommandText = "create or replace directory SDElogicbakdir as '" + txtRcDir.Text + "'";
                try
                {
                    pCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
                    
                }
                finally
                {
                    conn.Close();
                    
                }
                //����Ŀ¼����end
                try
                {
                    Stream stmp = File.Open(txtRcDir.Text + "\\expdpUsers.dat", FileMode.Open);
                    BinaryFormatter bf = new BinaryFormatter();
                    object o = bf.Deserialize(stmp);
                    List<string> users = o as List<string>;
                    stmp.Close();
                    foreach (string usr in users)
                    {
                        if(usr!="SDE")
                        lstRcUsr.Items.Add(usr).SubItems.Add(usr);

                    }
                }
                catch(Exception ex)
                {
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
                    
                }
                string[] fs = Directory.GetFiles(txtRcDir.Text.Trim(), "*.dmp", SearchOption.TopDirectoryOnly);
                System.Array.Sort(fs);
                foreach (string s in fs)
                {
                    files += System.IO.Path.GetFileName(s) + ",";
                }
                files = files.Substring(0, files.Length - 1);
            }
        }

        private void rdoFullRc_CheckedChanged(object sender, EventArgs e)
        {
          
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
           
            
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lstRcUsr.SelectedItems)
            {
                lstRcUsr.Items.Remove(lvi);
            }
        }

        private void btnClr_Click(object sender, EventArgs e)
        {
            lstRcUsr.Items.Clear();
        }

        private void btnRc_Click(object sender, EventArgs e)
        {
            if (txtRcDir.Text == "")
                return;
            if (MessageBox.Show("��ȷ��û���û���ʹ��SDE���ݣ�", "��ʾ", MessageBoxButtons.YesNo,
                         MessageBoxIcon.Information) != DialogResult.Yes)
                return;
            this.Cursor = Cursors.WaitCursor;
            rTxtLog.Text = "";
            btnStopSde_Click(sender, e);//ֹͣSDE����
            Thread.Sleep(1000);
            lblBottom.Text = "���ڴ������ݿ��û�...";
            Application.DoEvents();
            string impdp =userID + " directory=SDElogicbakdir";// "impdp " + 
            if (lstRcUsr.Items.Count > 0)
            {
                impdp += " schemas=(";
                string frmUsrs = "", toUsrs = "";
                for (int i = 0; i < lstRcUsr.Items.Count; i++)
                {
                    ListViewItem lvi = lstRcUsr.Items[i];
                    frmUsrs += lvi.Text + ",";
                    if (existUser(lvi.Text))
                    {
                        executeSQL("drop user " + lvi.Text + " cascade");
                        lblBottom.Text = "����ɾ���û���" + lvi.Text + "...";
                        Application.DoEvents();
                    }
                    //toUsrs += lvi.SubItems[1].Text + ",";
                }
                frmUsrs = frmUsrs.Substring(0, frmUsrs.Length - 1);
                //toUsrs = toUsrs.Substring(0, toUsrs.Length - 1);
                impdp += frmUsrs;// +" touser=" + toUsrs;
                impdp += ")";
            }
           
            impdp += " dumpfile=" + files;
            string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string pams =" logfile=impdp" + date + ".log ";
            logfile = txtRcDir.Text + "\\impdp" + date + ".log ";//Ϊ����Ӧ�鿴��־��ť
            impdp += pams;
            //Stream s = new System.IO.FileStream(Application.StartupPath + "\\impdpUsr.bat", FileMode.Create);
            //StreamWriter sw = new StreamWriter(s);
            ////sw.Encoding = Encoding.ASCII;
            //sw.WriteLine(impdp);
            //sw.Close();
            //s.Close();
            //Process pBat = new Process();
            lblBottom.Text = "���ڻ�ԭ...";
            Application.DoEvents();
            pBat.StartInfo.FileName = "impdp";//Application.StartupPath + "\\impdpUsr.bat";
            pBat.StartInfo.Arguments = impdp;
            //pBat.StartInfo.UseShellExecute = false;
            pBat.StartInfo.CreateNoWindow = false;
            //pBat.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pBat.StartInfo.RedirectStandardOutput = false;
            pBat.Start();
            //this.Height = 557;
            //Application.DoEvents();
            while (!pBat.WaitForExit(0))
            {
                this.Enabled = false;
                //rTxtLog.AppendText(pBat.StandardOutput.ReadLine() + "\r\n");
                //rTxtLog.ScrollToCaret();
            }
            //rTxtLog.AppendText(pBat.StandardOutput.ReadToEnd());
            //rTxtLog.ScrollToCaret();
            pBat.Close();
            btnStartSde_Click(sender, e);//����SDE����
            this.Enabled = true;
            this.Cursor = Cursors.Default;
            btnLookLogRc.Visible = true;
        }

        private void btnTOra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)//֧�ֻس���
                buttonX1_Click_1(sender, e);
        }

        private void btnOraRcEsc_Click(object sender, EventArgs e)
        {
            gPOraCon.BringToFront();
            lblBottom.Text = "";
            btnLookLog.Visible = false;
            btnLookLogRc.Visible = false;
        }

        private void btnStopSde_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
           

            string imp = "net stop \"arcsde service(esri_sde)\"";
            string batPath=Application.StartupPath + "\\RcPrefix.bat";
            Stream s = new System.IO.FileStream(batPath, FileMode.Create);
            StreamWriter sw = new StreamWriter(s);
            //sw.Encoding = Encoding.ASCII;
            sw.WriteLine(imp);
            sw.Close();
            s.Close();
            //Process pBat = new Process();
            pBat.StartInfo.FileName = batPath;
            //pBat.StartInfo.UseShellExecute = false;
            pBat.StartInfo.CreateNoWindow = true;
            //pBat.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pBat.StartInfo.RedirectStandardOutput = true;
            pBat.Start();
            //this.Height = 557;
            //Application.DoEvents();
            string outPut = "";
            while (!pBat.WaitForExit(0))
            {
                this.Enabled = false;

                //rTxtLog.AppendText(pBat.StandardOutput.ReadLine() + "\r\n");
                //rTxtLog.ScrollToCaret();
            }
            pBat.Close();
            Application.DoEvents();
            Process[] pro=Process.GetProcessesByName("giomgr");//����.exe
            if (pro.Length == 0)
                this.lblBottom.Text = "�����ѳɹ�ֹͣ��";
            else
                this.lblBottom.Text = "����δ��ֹͣ��";
            this.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        private void btnStartSde_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;


            string imp = "net start \"arcsde service(esri_sde)\"";
            string batPath = Application.StartupPath + "\\RcPrefix.bat";
            Stream s = new System.IO.FileStream(batPath, FileMode.Create);
            StreamWriter sw = new StreamWriter(s);
            //sw.Encoding = Encoding.ASCII;
            sw.WriteLine(imp);
            sw.Close();
            s.Close();
            //Process pBat = new Process();
            pBat.StartInfo.FileName = batPath;
            //pBat.StartInfo.UseShellExecute = false;
            pBat.StartInfo.CreateNoWindow = true;
            //pBat.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pBat.StartInfo.RedirectStandardOutput = false;
            pBat.Start();
            //this.Height = 557;
            //Application.DoEvents();
            while (!pBat.WaitForExit(0))
            {
                this.Enabled = false;
                //rTxtLog.AppendText(pBat.StandardOutput.ReadLine() + "\r\n");
                //rTxtLog.ScrollToCaret();
            }
            pBat.Close();
            Process[] pro = Process.GetProcessesByName("giomgr");//����.exe
            if (pro.Length == 0)
                this.lblBottom.Text = "����δ�ɹ��������볢���ڿ�����忪����";
            else
                this.lblBottom.Text = "����ɹ���������ԭ��ɣ�";
            this.Enabled = true;
            this.Cursor = Cursors.Default;
        }

        private void btnReCreateUsr_Click(object sender, EventArgs e)
        {
            string ruSqlTmp = Application.StartupPath + "\\..\\template\\RcUser.sql";
            string ruSqlTo = Application.StartupPath+"\\RcUser.sql";
            if (!File.Exists(ruSqlTmp))
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ؽ��û��ű��ļ���ʧ��");
                return;
            }
            //File.Copy(ruSqlTmp, ruSqlTo, true);
            this.Cursor = Cursors.WaitCursor;


            string imp = "sqlplus " + userID + " @RcUser.sql > RcUser.log";
            string batPath = Application.StartupPath + "\\RcUser.bat";
            Stream srcS = new System.IO.FileStream(ruSqlTmp, FileMode.Open);
            StreamReader srcR = new StreamReader(srcS);
            string srcSql = srcR.ReadToEnd();
            srcR.Close();
            srcS.Close();//�ͷŶ�ģ��sql�ļ��ķ���
            Stream s = new System.IO.FileStream(ruSqlTo, FileMode.Create);
            StreamWriter sw = new StreamWriter(s);
            foreach(ListViewItem lvi in lstRcUsr.Items)
            {
                string tmpSql=srcSql;
                tmpSql=tmpSql.Replace("sde",lvi.SubItems[1].Text);
                tmpSql = tmpSql.Replace("pwd", "s");
                sw.Write(tmpSql);
            }
            //sw.Encoding = Encoding.ASCII;
            sw.WriteLine("quit");
            sw.Close();
            s.Close();
            s = new System.IO.FileStream(batPath, FileMode.Create);
            sw = new StreamWriter(s);
            sw.WriteLine(imp);
            sw.Close();
            s.Close();
            //Process pBat = new Process();
            pBat.StartInfo.FileName = batPath;
            //pBat.StartInfo.UseShellExecute = false;
            pBat.StartInfo.CreateNoWindow = true;
            //pBat.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pBat.StartInfo.RedirectStandardOutput = false;
            pBat.Start();
            //this.Height = 557;
            //Application.DoEvents();
            while (!pBat.WaitForExit(0))
            {
                this.Enabled = false;
                //rTxtLog.AppendText(pBat.StandardOutput.ReadLine() + "\r\n");
                //rTxtLog.ScrollToCaret();
            }
        
            //rTxtLog.AppendText(pBat.StandardOutput.ReadToEnd());
            //rTxtLog.ScrollToCaret();
            pBat.Close();
            string ruSqlLog = Application.StartupPath + "\\RcUser.log";
            pBat.StartInfo.FileName = "notepad.exe";
            pBat.StartInfo.Arguments = ruSqlLog;
            pBat.Start();//���ؽ��û���־�ļ�
            //while (!pBat.WaitForExit(0))
            //{
            //    this.Enabled = false;
            //    //rTxtLog.AppendText(pBat.StandardOutput.ReadLine() + "\r\n");
            //    //rTxtLog.ScrollToCaret();
            //}
            //pBat.Close();
            this.Enabled = true;
            this.Cursor = Cursors.Default;

            
        }

        private void btnSdeRcPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            //folder.ShowNewFolderButton = false;
            folder.Description = "��ԭ��gdb·����";
            if (folder.ShowDialog() == DialogResult.OK)
            {
                string str = folder.SelectedPath;
                this.txtSdeRcPath.Text = str;

            }
            IWorkspaceFactory pGDBWf=new FileGDBWorkspaceFactoryClass();
            pGDBWs = pGDBWf.OpenFromFile(txtSdeRcPath.Text, 0);
            this.Cursor = Cursors.WaitCursor;
            lstSdeRcDs.Items.Clear();
            if (pGDBWs == null)
            {
                lblBottom.Text = "����ʧ�ܣ�����·����";
                this.Cursor = Cursors.Default;
                return;
            }
            IEnumDatasetName pEDN = pGDBWs.get_DatasetNames(esriDatasetType.esriDTFeatureDataset);
            IDatasetName pDN = pEDN.Next();
            while (pDN != null)
            {
                lstSdeRcDs.Items.Add(pDN.Name);
                pDN = pEDN.Next();
            }
            lstSdeRcDs.Refresh();
            this.Cursor = Cursors.Default;
        }

        private void btnLookLog_Click(object sender, EventArgs e)
        {
            lookLog(logfile);
            btnLookLog.Visible = false;
        }
        private void lookLog(string filename)
        {
            try
            {
                if(filename=="")
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", "��־�ļ������ڣ�");
                else
                    Process.Start("notepad.exe", filename);
            }
            catch (Exception ex)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
            }

 
        }

        private void btnLookLogRc_Click(object sender, EventArgs e)
        {
            lookLog(logfile);
            btnLookLogRc.Visible = false;
        }

        private void btnTOra_Leave(object sender, EventArgs e)
        {

        }

      
      

    }
}