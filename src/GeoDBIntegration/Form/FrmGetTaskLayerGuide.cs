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
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System.Data.OracleClient;
using ESRI.ArcGIS.Display;
using SysCommon.Authorize;

namespace GeoDBIntegration
{
    public partial class FrmGetTaskLayerGuide : DevComponents.DotNetBar.Office2007Form
    {
        private Plugin.Application.IAppDBIntegraRef m_AppGIS;               //������Ӧ��APP
        private Plugin.Application.IAppFormRef m_AppForm;               //������Ӧ��APP
        private IGeometry m_Geometry;
        private IWorkspace m_SDEWs = null;//////////////////////�û�������Workspace
        private IWorkspace m_SourceWs = null;/////////////////����Χ����Դ��Workspace
        private IFeatureClass m_SDETaskLayer = null;/////////////����ķ�Χͼ�㣨SDE�ϵģ�
        private IFeatureClass m_SourceTaskLayer = null;//////////Դ������Χͼ��
        string m_sSDERangeLayerName = "RANGE";
        private string m_DBProjectID = string.Empty;/////////////���ݿ⹤��ID
        private string m_iRoleId ="";/////////////////////////////��ҵԱ��Ӧ�Ľ�ɫID
        //cyf 20110603 add
        //private string pUserId = "";                         //��ҵԱID
        //end

        public IGeometry DrawGeometry
        {
            set
            {
                m_Geometry = value;
            }
        }
        public FrmGetTaskLayerGuide(Plugin.Application.IAppDBIntegraRef pAppGIS, IWorkspace in_SDEWs, string in_DBID)
        {
            InitializeComponent();
            //cyf  201100603  add;
            m_AppGIS = pAppGIS;
            if (m_AppGIS == null) return;
            m_AppForm = m_AppGIS as Plugin.Application.IAppFormRef;
            if (m_AppForm == null) return;
            //end
            this.cmb_Type.Items.Add("SDE");
            this.cmb_Type.Items.Add("PDB");
            this.cmb_Type.Items.Add("GDB");
            this.cmb_Type.SelectedIndex = 1;
            this.m_SDEWs = in_SDEWs;
            this.radioInputNewLayer.Checked = true;
            this.m_DBProjectID = in_DBID;
            this.RadioSDELayer.Checked = true;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void RadioBtnSelectRange_Click(object sender, EventArgs e)
        {

        }

        private void RadioBtnSelectRange_CheckedChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// ����Դ���͸ı�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cmb_Type.Text == "SDE")
            {
                this.txtServer.Enabled = true;
                this.txt_servername.Enabled = true;
                this.txtDataBase.Enabled = true;
                this.btnServer.Enabled = false;
                this.txtVersion.Enabled = true;
                this.txtVersion.Text = "SDE.DEFAULT";
                this.txtUser.Enabled = true;
                this.txtPassWord.Enabled = true;
            }
            else if (this.cmb_Type.Text == "GDB" || this.cmb_Type.Text == "PDB")
            {
                this.txtServer.Enabled = false;
                this.txt_servername.Enabled = false;
                this.txtDataBase.Enabled = true;
                this.btnServer.Enabled = true;
                this.txtVersion.Enabled = false;
                this.txtVersion.Text = "";
                this.txtUser.Enabled = false;
                this.txtPassWord.Enabled = false;
            }
        }

        private void RadioSDELayer_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioInputNewLayer.Checked == true)
                this.groupBox1.Enabled = true;
            else
                this.groupBox1.Enabled = false;
        }

        private void radioInputNewLayer_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioInputNewLayer.Checked == true)
                this.groupBox1.Enabled = true;
            else
                this.groupBox1.Enabled = false;
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            switch (this.cmb_Type.Text.Trim())
            {
                case "SDE":

                    break;

                case "GDB":
                    FolderBrowserDialog pFolderBrowser = new FolderBrowserDialog();
                    if (pFolderBrowser.ShowDialog() == DialogResult.OK)
                    {
                        txtDataBase.Text = pFolderBrowser.SelectedPath;
                    }
                    break;

                case "PDB":
                    OpenFileDialog saveFile = new OpenFileDialog();
                    saveFile.Title = "��PDB����";
                    saveFile.Filter = "PDB����(*.mdb)|*.mdb";
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        txtDataBase.Text = saveFile.FileName;
                    }
                    break;
                default:
                    break;
            }
        }






        private void btn1_Next_Click(object sender, EventArgs e)
        {
            //tabControl1.SelectedIndex = 1;
            Exception ex=null;
            if (this.RadioSDELayer.Checked)
            {
                ///////SDE���Ѿ����ڵķ�Χͼ��///////
                try
                {
                    this.m_SDETaskLayer = (this.m_SDEWs as IFeatureWorkspace).OpenFeatureClass(this.m_sSDERangeLayerName);
                    tabControl1.SelectedIndex = 2;
                    IFeatureLayer SDERangeLayer = new FeatureLayerClass();
                    SDERangeLayer.FeatureClass = this.m_SDETaskLayer;
                    this.axMapControl2.AddLayer(SDERangeLayer as ILayer);
                    //////////��Ⱦ///////////////
                    if (this.axMapControl2.LayerCount > 0)
                    {
                        IFeatureLayer GetFirstLayer = this.axMapControl2.get_Layer(0) as IFeatureLayer;
                        SetDataUpdateSymbol(GetFirstLayer);
                    }
                    this.axMapControl2.Refresh();
                    //cyf 20110603  modify��������Ŀ��Ϣ
                    GetProjectInfo(out ex);
                    if (ex != null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ��Ŀ��Ϣʧ�ܣ�" + ex.Message);
                        return;
                    }
                    //GetAllUser(ModuleData.v_AppConnStr, out ex);
                    //if (ex != null)
                    //{
                    //    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ��ҵԱ��Ϣʧ�ܣ�" + ex.Message);
                    //    return;
                    //}
                    //end
                    
                }
                catch (Exception eError)
                {
                    if (null == ModuleData.v_SysLog) ModuleData.v_SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModuleData.v_SysLog.Write(eError);
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "SDE�л�ȡ����Χͼ�㣺" + this.m_sSDERangeLayerName + " ʧ��");
                    return;
                }
            }
            else
            {
                ///////����Դ�е����ͼ��///////
                this.m_SourceWs = GetDBInfo(this.cmb_Type.Text.Trim(), this.txtDataBase.Text, this.txtServer.Text, this.txt_servername.Text, this.txtUser.Text, this.txtPassWord.Text, this.txtVersion.Text, out ex) as IWorkspace;
                if (ex != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��������Դʧ�ܣ�" + ex.Message);
                    return;
                }
                if (this.m_SourceWs == null) return;
                tabControl1.SelectedIndex = 1;
                ////////��Դ�����е�ͼ����ʾ���б���///////
                IEnumDataset pEnumDataSet = this.m_SourceWs.get_Datasets(esriDatasetType.esriDTFeatureClass);
                IFeatureClass GetLayerInSource = pEnumDataSet.Next() as IFeatureClass;
                while (GetLayerInSource != null)
                {
                    if (GetLayerInSource.ShapeType != esriGeometryType.esriGeometryPolygon)
                    {
                        ////////ֻ���ض���ε�ͼ��
                        GetLayerInSource = pEnumDataSet.Next() as IFeatureClass;
                        continue;
                    }
                    this.combox_SelectLayer.Items.Add((GetLayerInSource as IDataset).Name);
                    GetLayerInSource = pEnumDataSet.Next() as IFeatureClass;
                }
                if (this.combox_SelectLayer.Items.Count > 0) this.combox_SelectLayer.SelectedIndex = 0;
            }
        }


        /// <summary>
        /// �����������������ռ䣨����ԭ�д��룩
        /// </summary>
        /// <param name="strType">���ͣ�SDE,PDB,GDB</param>
        /// <param name="strDB">���ݿ�·��</param>
        /// <param name="strServer">������</param>
        /// <param name="strInstance">ʵ����</param>
        /// <param name="strUser">�û���</param>
        /// <param name="strPassword">����</param>
        /// <param name="strVersion">�汾</param>
        /// <param name="ex">�����������Ϣ</param>
        /// <returns>IWorkspace���󣬳�����NULL</returns>
        public static object GetDBInfo(string strType, string strDB, string strServer, string strInstance, string strUser, string strPassword, string strVersion, out Exception ex)
        {
            ex = null;
            try
            {
                IPropertySet pPropSet = null;
                switch (strType.Trim().ToLower())
                {
                    case "pdb":
                        pPropSet = new PropertySetClass();
                        AccessWorkspaceFactory pAccessFact = new AccessWorkspaceFactoryClass();
                        if (!File.Exists(strDB))
                        {
                            FileInfo filePDB = new FileInfo(strDB);
                            pAccessFact.Create(filePDB.DirectoryName, filePDB.Name, null, 0);
                        }
                        pPropSet.SetProperty("DATABASE", strDB);
                        IWorkspace pdbWorkspace = pAccessFact.Open(pPropSet, 0);
                        pAccessFact = null;
                        return pdbWorkspace;

                    case "gdb":
                        pPropSet = new PropertySetClass();
                        FileGDBWorkspaceFactoryClass pFileGDBFact = new FileGDBWorkspaceFactoryClass();
                        if (!Directory.Exists(strDB))
                        {
                            DirectoryInfo dirGDB = new DirectoryInfo(strDB);
                            pFileGDBFact.Create(dirGDB.Parent.FullName, dirGDB.Name, null, 0);
                        }
                        pPropSet.SetProperty("DATABASE", strDB);
                        IWorkspace gdbWorkspace = pFileGDBFact.Open(pPropSet, 0);
                        pFileGDBFact = null;
                        return gdbWorkspace;

                    case "sde":
                        pPropSet = new PropertySetClass();
                        IWorkspaceFactory pSdeFact = new SdeWorkspaceFactoryClass();
                        pPropSet.SetProperty("SERVER", strServer);
                        pPropSet.SetProperty("INSTANCE", strInstance);
                        pPropSet.SetProperty("DATABASE", strDB);
                        pPropSet.SetProperty("USER", strUser);
                        pPropSet.SetProperty("PASSWORD", strPassword);
                        pPropSet.SetProperty("VERSION", strVersion);
                        IWorkspace sdeWorkspace = pSdeFact.Open(pPropSet, 0);
                        pSdeFact = null;
                        return sdeWorkspace;

                    case "access":
                        System.Data.Common.DbConnection dbCon = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strDB);
                        dbCon.Open();
                        return dbCon;

                    //case "oracle":
                    //    string strOracle = "Data Source=" + strDB + ";Persist Security Info=True;User ID=" + strUser + ";Password=" + strPassword + ";Unicode=True";
                    //    System.Data.Common.DbConnection dbConoracle = new OracleConnection(strOracle);
                    //    dbConoracle.Open();
                    //    return dbConoracle;

                    //case "sql":
                    //    string strSql = "Data Source=" + strDB + ";Initial Catalog=" + strInstance + ";User ID=" + strUser + ";Password=" + strPassword;
                    //    System.Data.Common.DbConnection dbConsql = new SqlConnection(strSql);
                    //    dbConsql.Open();
                    //    return dbConsql;

                    default:
                        break;
                }

                return null;
            }
            catch (Exception e)
            {
                //******************************************
                //ϵͳ������־
                if (ModuleData.v_SysLog == null) ModuleData.v_SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                ModuleData.v_SysLog.Write(e);
                //******************************************
                ex = e;
                return null;
            }
        }

        private void btn2_Pre_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void axMapControl1_OnAfterDraw(object sender, IMapControlEvents2_OnAfterDrawEvent e)
        {

        }

        /// <summary>
        /// Դͼ���б���ͼ��ı䣬ͼ����ʾ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combox_SelectLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sGetLayerName = this.combox_SelectLayer.Text;
            try
            {
                this.axMapControl1.ClearLayers();
                IFeatureClass Getlayer = (this.m_SourceWs as IFeatureWorkspace).OpenFeatureClass(sGetLayerName);
                IFeatureLayer pFeaLayer = new FeatureLayerClass();
                pFeaLayer.FeatureClass = Getlayer;
                ILayer AddLayer = pFeaLayer as ILayer;
                this.axMapControl1.AddLayer(AddLayer);
                this.m_SourceTaskLayer = Getlayer;
                this.axMapControl1.Refresh();
                this.m_SourceTaskLayer = Getlayer;
            }
            catch
            {

            }
        }

        /// <summary>
        /// ��Դͼ���Ҫ�ص��뵽SDE�У�������һ������Ľ׶�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn2_next_Click(object sender, EventArgs e)
        {
            Exception ex=null;
            if (this.m_SourceTaskLayer == null) return;
            if (this.m_SDETaskLayer == null)
            {
                try
                {
                    this.m_SDETaskLayer = (this.m_SDEWs as IFeatureWorkspace).OpenFeatureClass(this.m_sSDERangeLayerName);
                }
                catch(Exception eError)
                {
                    if (null == ModuleData.v_SysLog) ModuleData.v_SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModuleData.v_SysLog.Write(eError);
                }
            }
            //////////����������еķ�ΧҪ��//////
            //OracleConnection Con =Con = new OracleConnection(ModuleData.v_AppConnStr);
            try
            {
                //cyf 20110603 modify
                /////////////////��շ�����Ϣ/////////////////           
                //if (Con.State == ConnectionState.Closed) Con.Open();
                //string sDeleteSql = "DELETE FROM updateinfo WHERE PROJECTID=" + this.m_DBProjectID;
                //OracleCommand Comm = new OracleCommand(sDeleteSql, Con);
                //Comm.ExecuteNonQuery();
                string sDeleteSql = "DELETE FROM updateinfo WHERE PROJECTID=" + this.m_DBProjectID;
                if (ModuleData.TempWks == null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ϵͳά����ʧ�ܣ�");
                    return;
                }
                ModuleData.TempWks.ExecuteSQL(sDeleteSql);
                //end
                //cyf 20110603 delete:ȡ��ע��汾
                ////////////////////�ж��Ƿ����˰汾////////////////////
                //IVersionedObject pVersion = this.m_SDETaskLayer as IVersionedObject;
                //if (!pVersion.IsRegisteredAsVersioned) pVersion.RegisterAsVersioned(true);
                ////////////////////////////////////////////////
                //end
                IFeatureCursor pFeaCur = this.m_SDETaskLayer.Search(null, false);
                IFeature pGetFea = pFeaCur.NextFeature();
                IWorkspaceEdit SDEWs = (this.m_SDETaskLayer as IDataset).Workspace as IWorkspaceEdit;
                //SDEWs.StartEditing(false);
                //SDEWs.StartEditOperation();
                bool bSave = false;
                while (pGetFea != null)
                {
                    pGetFea.Delete();
                    pGetFea.Store();
                    bSave = true;
                    pGetFea = pFeaCur.NextFeature();
                }
                //SDEWs.StopEditOperation();
                //SDEWs.StopEditing(bSave);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaCur);
                ///////////////////////////
            }
            catch (Exception eError)
            {
                if (ModuleData.v_SysLog == null) ModuleData.v_SysLog = new SysCommon.Log.clsWriteSystemFunctionLog(); ModuleData.v_SysLog.Write(eError);
            }
            //finally
            //{
            //    if (Con.State == ConnectionState.Open) Con.Close();
            //}
            ////////Ҫ��ת��////////////
            try
            {
                //cyf 20110603 delete:ȡ��ע��汾
                ////////////////////�ж��Ƿ����˰汾////////////////////
                //IVersionedObject pVersion = this.m_SDETaskLayer as IVersionedObject;
                //if (!pVersion.IsRegisteredAsVersioned) pVersion.RegisterAsVersioned(true);
                ////////////////////////////////////////////////
                //end
                IFeatureCursor pFeaCur = this.m_SourceTaskLayer.Search(null, false);
                IFeature pGetFea = pFeaCur.NextFeature();
               // int RangeId = 0;
                IWorkspaceEdit SDEWs = (this.m_SDETaskLayer as IDataset).Workspace as IWorkspaceEdit;
                //SDEWs.StartEditing(false);
                //SDEWs.StartEditOperation();
                bool bSave = false;
                while (pGetFea != null)
                {
                    //RangeId += 1;
                    IFeature NewFeature = this.m_SDETaskLayer.CreateFeature();
                    NewFeature.Shape = pGetFea.ShapeCopy;
                    int FieldIndex = NewFeature.Fields.FindField("RANGEID");
                    if (FieldIndex > 0) NewFeature.set_Value(FieldIndex, NewFeature.OID);
                    FieldIndex = NewFeature.Fields.FindField("assign");
                    if (FieldIndex > 0) NewFeature.set_Value(FieldIndex, 0);
                    NewFeature.Store();
                    pGetFea = pFeaCur.NextFeature();
                    bSave = true;
                }
                //SDEWs.StopEditOperation();
                //SDEWs.StopEditing(bSave);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaCur);
            }
            catch (Exception eError)
            {
                if (ModuleData.v_SysLog == null) ModuleData.v_SysLog = new SysCommon.Log.clsWriteSystemFunctionLog(); ModuleData.v_SysLog.Write(eError);
            }
            tabControl1.SelectedIndex = 2;
            /////////
            IFeatureLayer SDERangeLayer = new FeatureLayerClass();
            SDERangeLayer.FeatureClass = this.m_SDETaskLayer;
            this.axMapControl2.AddLayer(SDERangeLayer as ILayer);            
            
            //cyf 20110603 modify:������Ŀ��Ϣ
            GetProjectInfo(out ex);
            //GetAllUser(ModuleData.v_AppConnStr, out ex);
            if (ex != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ��Ŀ��Ϣʧ�ܣ�" + ex.Message);
                return;
            }
            //////////��Ⱦ///////////////
            if (this.axMapControl2.LayerCount > 0)
            {
                IFeatureLayer GetFirstLayer = this.axMapControl2.get_Layer(0) as IFeatureLayer;
                SetDataUpdateSymbol(GetFirstLayer);
            }
            this.axMapControl2.Refresh();
        }

        /*
        /// <summary>
        ///��ȡϵͳά�����е���ҵԱ��Ϣ�������б����ṩ��Χ����
        /// </summary>
        /// <param name="in_OracleDBConnectstr"></param>
        /// <param name="ex"></param>
        private void GetAllUser(string in_OracleDBConnectstr, out Exception ex)
        {
            ex = null;
            if (string.IsNullOrEmpty(in_OracleDBConnectstr)) { ex = new Exception("ϵͳά����������Ϣ����Ϊ��"); return; }
            ///////////////��ȡ���ݿ��е��û���Ϣ��ʾ��Combox��///////////
            OracleConnection Con = new OracleConnection(in_OracleDBConnectstr);
            try
            {
                Con.Open();
                string SQL = "SELECT ROLETYPEID FROM roletypeinfo WHERE ROLETYPE=1";/////1Ϊ��ͨ�û�
                OracleDataAdapter DataAdapter = new OracleDataAdapter(SQL, Con);
                DataTable GetTable = new DataTable();
                DataAdapter.Fill(GetTable);
                m_iRoleId = Convert.ToInt32(GetTable.Rows[0][0].ToString());
                SQL = "SELECT U.USERID,U.USERNAME, U.SEX,U.JOB FROM userbaseinfo U JOIN userrolerelationinfo R  ON (U.USERID=R.userid)JOIN roletypeinfo X ON(R.ROLEID=X.ROLETYPEID) WHERE X.ROLETYPE=" + m_iRoleId.ToString();
                DataAdapter = new OracleDataAdapter(SQL, Con);
                GetTable = new DataTable();
                DataAdapter.Fill(GetTable);
                this.comboBox_USER.DataSource = GetTable;
                this.comboBox_USER.DisplayMember = "USERNAME";
                this.comboBox_USER.ValueMember = "USERID";
            }
            catch (Exception eError)
            {
                ex = eError;
                if (ModuleData.v_SysLog == null) ModuleData.v_SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                ModuleData.v_SysLog.Write(eError);
                return;
            }
            finally
            {
                if (Con.State == ConnectionState.Open) Con.Close();
            }
        }
        */


        /// <summary>
        /// cyf 20110603 add:��ȡϵͳά�����е���ҵԱ��Ϣ�������б����ṩ��Χ����
        /// </summary>
        /// <param name="proIDStr">��ĿID</param>
        /// <param name="ex">�쳣</param>
        private void GetAllUser(string proIDStr,out Exception ex)
        {
            ex = null;
            if (ModuleData.TempWks==null) { ex = new Exception("ϵͳά����������Ϣ����Ϊ��"); return; }
            ///////////////��ȡ���ݿ��е��û���Ϣ��ʾ��Combox��///////////
            try
            {
                //��ѯ��񣬻�ȡ�û���Ϣ
                IFeatureWorkspace pFeaWs = ModuleData.TempWks as IFeatureWorkspace;
                if (pFeaWs != null)
                {
                    IQueryDef pQueryDef = pFeaWs.CreateQueryDef();
                    pQueryDef.Tables = "role,user_role,user_info";
                    pQueryDef.SubFields = "user_info.USERID,user_info.NAME,role.ROLEID";
                    pQueryDef.WhereClause = "role.ROLEID=user_role.ROLEID and  user_role.USERID=user_info.USERID and role.TYPEID='"+EnumRoleType.��ҵԱ.GetHashCode().ToString()+"' and  " + "role.PROJECTID='" + proIDStr + "'";//+ EnumRoleType.��ҵԱ.GetHashCode().ToString() + "'
                    ICursor pCursor = pQueryDef.Evaluate();
                    if (pCursor == null)
                    {
                        ex = new Exception("��ѯ�û���Ϣʧ�ܣ�");
                        return;
                    }
                      //������ʱ�ı�������Ѷ��ѯ������Ŀ��Ϣ
                    DataTable mTable = new DataTable();
                    mTable.Columns.Add("USERID", Type.GetType("System.String"));
                    mTable.Columns.Add("USERNAME", Type.GetType("System.String"));

                    IRow pRow = pCursor.NextRow();
                    //������ѯ�������е��У�����Ŀ��Ϣ�����������б����
                    while (pRow != null)
                    {
                        string mUserIDStr = pRow.get_Value(0).ToString().Trim();        //��ĿID
                        string mUserNameStr = pRow.get_Value(1).ToString().Trim();      //��Ŀ����

                        //����ʱ���в�������
                        DataRow pDtRow = mTable.NewRow();
                        pDtRow[0] = mUserIDStr;
                        pDtRow[1] = mUserNameStr;
                        mTable.Rows.Add(pDtRow);

                        pRow = pCursor.NextRow();
                    }
                    //�ͷ��α�
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
                    //�������б������ʾ
                    this.comboBox_USER.DataSource = mTable;
                    this.comboBox_USER.DisplayMember = "USERNAME";
                    this.comboBox_USER.ValueMember = "USERID";

                    if (this.comboBox_USER.Items.Count > 0)
                    {
                        this.comboBox_USER.SelectedIndex = 0;
                    }
                }
            } catch (Exception eError)
            {
                ex = eError;
                if (ModuleData.v_SysLog == null) ModuleData.v_SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                ModuleData.v_SysLog.Write(eError);
                return;
            }
            
        }
        
        /// <summary>
        /// cyf 20110603 add :��ȡ��¼�û�����Ŀ��Ϣ
        /// </summary>
        /// <param name="outError">�쳣</param>
        private void GetProjectInfo(out Exception outError)
        {
            outError = null;
            if (ModuleData.TempWks == null) { outError = new Exception("ϵͳά����������Ϣ����Ϊ��"); return; }
            ///////////////��ȡ���ݿ��е��û���Ϣ��ʾ��Combox��///////////
            try
            {
                //��ѯ��񣬻�ȡ�û���Ϣ
                IFeatureWorkspace pFeaWs = ModuleData.TempWks as IFeatureWorkspace;
                if (pFeaWs != null)
                {
                    //��õ�¼�û���Ӧ�Ĺ�����Ŀ��Ϣ
                    List<Role> LstRole = m_AppForm.LstRoleInfo;
                    if (LstRole == null)
                    {
                        outError = new Exception("��ѯ��Ŀ��Ϣ��ʧ�ܣ�");
                        return;
                    }
                    string proIDWhere = "";           //��ĿID�����ַ���
                    foreach (Role pRole in LstRole)
                    {
                        string proIDStr = pRole.PROJECTID;   //��ĿID
                        proIDWhere += "'" + proIDStr + "',";
                    }
                    if (proIDWhere != "")
                    {
                        proIDWhere = proIDWhere.Substring(0, proIDWhere.Length - 1);
                    }

                    IQueryDef pQueryDef = pFeaWs.CreateQueryDef();
                    pQueryDef.Tables = "projectgroup";
                    pQueryDef.SubFields = "PROJECTID,PROJECTNAME";
                    if (proIDWhere != "")
                    {
                        pQueryDef.WhereClause = "PROJECTID in (" + proIDWhere + ")";
                    }
                    else
                    {
                        outError = new Exception("��ȡ��¼�û�����Ŀ��Ϣʧ�ܣ�");
                        return;
                    }
                    ICursor pCursor = pQueryDef.Evaluate();
                    if (pCursor == null)
                    {
                        outError = new Exception("��ѯ��Ŀ��Ϣʧ�ܣ�");
                        return;
                    }
                    //������ʱ�ı�������Ѷ��ѯ������Ŀ��Ϣ
                    DataTable mTable = new DataTable();
                    mTable.Columns.Add("PROJECTID", Type.GetType("System.String"));
                    mTable.Columns.Add("PROJECTNAME", Type.GetType("System.String"));

                    IRow pRow = pCursor.NextRow();
                    //������ѯ�������е��У�����Ŀ��Ϣ�����������б����
                    while (pRow != null)
                    {
                        string mProIDStr = pRow.get_Value(0).ToString().Trim();        //��ĿID
                        string mProNameStr = pRow.get_Value(1).ToString().Trim();      //��Ŀ����

                        //����ʱ���в�������
                        DataRow pDtRow = mTable.NewRow();
                        pDtRow[0] = mProIDStr;
                        pDtRow[1] = mProNameStr;
                        mTable.Rows.Add(pDtRow);

                        pRow = pCursor.NextRow();
                    }
                    //�ͷ��α�
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
                    //�������б������ʾ
                    this.cmbProject.DataSource = mTable;
                    this.cmbProject.DisplayMember = "PROJECTNAME";
                    this.cmbProject.ValueMember = "PROJECTID";

                    if (this.cmbProject.Items.Count > 0)
                    {
                        this.cmbProject.SelectedIndex = 0;
                    }
                }
            } catch(Exception eError)
            {
                outError = eError;
                if (ModuleData.v_SysLog == null) ModuleData.v_SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                ModuleData.v_SysLog.Write(eError);
                return;
            }
        }

        /// <summary>
        /// ��Χ�ķ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //cyf  20110603 add
           if( ModuleData.TempWks == null)
           {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ϵͳά����ʧ�ܣ�");
                return;
           }
            IFeatureWorkspace pFeaWs=ModuleData.TempWks as IFeatureWorkspace;
            if(pFeaWs==null) 
            {
                 SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ϵͳά����ʧ�ܣ�");
                return;
            }
            //end
            if (this.axMapControl2.LayerCount <= 0)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ط�Χͼ��");
                return;
            }
            if (string.IsNullOrEmpty(this.comboBox_USER.Text.Trim()))
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��һ��������ҵ��Χ����ҵԱ");
                this.comboBox_USER.Focus(); 
                return;
            }
            try
            {
                IFeatureLayer GetLayer = this.axMapControl2.get_Layer(0) as IFeatureLayer;
                IFeatureClass GetFeaCls = GetLayer.FeatureClass;
                IFeatureSelection fealSel = GetLayer as IFeatureSelection;
                string sUserName = this.comboBox_USER.Text;
                string sUserID = this.comboBox_USER.SelectedValue.ToString();
                //cyf 20110603 delete:
                ////////////////////�ж��Ƿ����˰汾////////////////////
                //IVersionedObject pVersion = this.m_SDETaskLayer as IVersionedObject;
                //if (!pVersion.IsRegisteredAsVersioned) pVersion.RegisterAsVersioned(true);
                //end
                ////////////////////////////////////////////////
                //cyf  20110603  modify
                ///////ϵͳά�������//////
                //OracleConnection Con = new OracleConnection(ModuleData.v_AppConnStr);
                IWorkspaceEdit SDEWs = (this.m_SDETaskLayer as IDataset).Workspace as IWorkspaceEdit;
                int iUserFieldIndex = m_SDETaskLayer.Fields.FindField("USERNAME");
                //SDEWs.StartEditing(false);
                //SDEWs.StartEditOperation();
                //bool bSave = false;
                 if (fealSel.SelectionSet.Count != 0)
                 {
                     //Con.Open();
                     IEnumIDs pEnumIDs = fealSel.SelectionSet.IDs;
                     int id = pEnumIDs.Next();
                     while (id != -1)
                     {
                         IFeature GetFea = GetFeaCls.GetFeature(id);
                         string sRangeID=string.Empty;
                         int iAssign = 0;
                         int Index=GetFea.Fields.FindField("RANGEID");
                         if (Index<0) sRangeID="null";
                         else sRangeID=GetFea.get_Value(Index).ToString();
                         Index = GetFea.Fields.FindField("assign");
                         try
                         {
                             iAssign = Convert.ToInt32(GetFea.get_Value(Index).ToString());
                         }
                         catch
                         {
                             iAssign = 0;
                         }
                         ///////�����������Ϣд�����ݿ�/////
                         //cyf 20110603 add:��ȡupdateinfo�ļ�¼�����ֵ
                         int oidMax = 0;
                         ITable pTable = pFeaWs.OpenTable("updateinfo");
                         if (pTable.RowCount(null) == 0)
                         {
                             oidMax = 1;
                         }
                         else
                         {
                             IDataStatistics pDtSta = new DataStatisticsClass();
                             pDtSta.Field = "OBJECTID";
                             pDtSta.Cursor = pTable.Search(null, false);
                             IStatisticsResults pStaRes = null;
                             pStaRes = pDtSta.Statistics;
                             oidMax = (int)pStaRes.Maximum;
                             oidMax = oidMax + 1;
                         }
                         //end
                         string SQL = string.Empty;
                         if (iAssign == 0)
                             SQL = "INSERT INTO updateinfo(OBJECTID,PROJECTID ,RANGEID,ROLEID,USERID ) values(" + oidMax + "," + this.m_DBProjectID + "," + sRangeID + ",'" + this.m_iRoleId.ToString() + "','" + sUserID + "')";
                         else
                             SQL = "UPDATE updateinfo SET PROJECTID=" + this.m_DBProjectID + "," + "RANGEID=" + sRangeID + "," + "ROLEID='" + this.m_iRoleId.ToString() + "'," + "USERID='" + sUserID + "' WHERE "+"PROJECTID=" + this.m_DBProjectID + " AND " + "RANGEID=" + sRangeID ;
                        //cyf 20110603 modify
                             ModuleData.TempWks.ExecuteSQL(SQL);
                         //end
                         //OracleCommand Com = new OracleCommand(SQL, Con);
                         //Com.ExecuteNonQuery();
                         id = pEnumIDs.Next();
                         GetFea.set_Value(Index, 1);
                         //*********��¼������û�**************
                         if (iUserFieldIndex >= 0) GetFea.set_Value(iUserFieldIndex, this.comboBox_USER.Text.Trim());
                         GetFea.Store();
                         //bSave = true;
                     }
                     //SDEWs.StopEditOperation();
                     //SDEWs.StopEditing(bSave);
                     SysCommon.Error.ErrorHandle.ShowInform("��ʾ", "�������");
                     SetDataUpdateSymbol(GetLayer);
                     this.axMapControl2.Refresh();
                 }
            }
            catch(Exception eError)
            {
                if (ModuleData.v_SysLog != null) ModuleData.v_SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                ModuleData.v_SysLog.Write(eError);
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�������" + eError.Message);
            }
            
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// ����ˢ�º���Ⱦ�ѷ����Ҫ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axMapControl2_OnAfterDraw(object sender, IMapControlEvents2_OnAfterDrawEvent e)
        {
            try
            {
               //IFeatureLayer GetLayer = this.axMapControl2.get_Layer(0) as IFeatureLayer;
               //if (GetLayer.FeatureClass.Fields.FindField("assign") < 0) return;
               //SetDataUpdateSymbol(GetLayer);
            }
            catch
            {
            }
        }
        private void SetDataUpdateSymbol(IFeatureLayer pFeatureLayer)
        {
            //if (pFeatureLayer == null || strFieldName == string.Empty) return;
            //Dictionary<string, string> dicFieldValue = new Dictionary<string, string>();
            //Dictionary<string, ISymbol> dicFieldSymbol = new Dictionary<string, ISymbol>();

            //ISymbol pSymbol = null;
            //dicFieldValue.Add("1", "�ѷ���");
            //pSymbol = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 35, 254, 7);
            //dicFieldSymbol.Add("1", pSymbol);

            ////dicFieldValue.Add("2", "�޸�");
            ////pSymbol = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 38, 254, 7);
            ////dicFieldSymbol.Add("2", pSymbol);

            ////dicFieldValue.Add("3", "ɾ��");
            ////pSymbol = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 254, 7, 7); ;
            ////dicFieldSymbol.Add("3", pSymbol);

            //dicFieldValue.Add("0", "δ����");
            //pSymbol = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 178, 178, 178); ;
            //dicFieldSymbol.Add("0", pSymbol);

            //SetLayerUniqueValueRenderer(pFeatureLayer, strFieldName, dicFieldValue, dicFieldSymbol, false);
            string sCurrentUserName = this.comboBox_USER.Text.Trim();//////////////��ǰѡ���û�
            string sUserFieldName = "USERNAME";
            string sAssignFieldName = "assign";
            int iUserIndex = pFeatureLayer.FeatureClass.Fields.FindField(sUserFieldName);
            int iAssignIndex = pFeatureLayer.FeatureClass.Fields.FindField(sAssignFieldName);
            if (iUserIndex < 0 || iAssignIndex < 0) return;/////û���ҵ���Ⱦ��ͼ��
            ISymbol pSymbolNull = null;//δ�仯
            pSymbolNull = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 178, 178, 178);

            ISymbol pSymbolOtherUser= null;//�����û���Ⱦ
            pSymbolOtherUser = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 35, 255, 255);

            ISymbol pSymbolCurrentUser = null;//�����û���Ⱦ
            pSymbolCurrentUser = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, 38, 254, 7);

            IFeatureClass pFeatCls = pFeatureLayer.FeatureClass;
            IUniqueValueRenderer pUniqueValueRenderer = new UniqueValueRendererClass();
            pUniqueValueRenderer.FieldCount = 2;/////////////////////////////////////////
            pUniqueValueRenderer.FieldDelimiter = "|";
            pUniqueValueRenderer.set_Field(0, sUserFieldName);          //������Ⱦɫ
            pUniqueValueRenderer.set_Field(1, sAssignFieldName);
            pUniqueValueRenderer.DefaultSymbol = pSymbolNull;
            pUniqueValueRenderer.UseDefaultSymbol = true;////////////////////////////////
            string[] GetUnirueValues = GetUniqueValue(pFeatureLayer.FeatureClass, sUserFieldName);
            if (GetUnirueValues.Length <= 0) return;
            ///////////////��ȡ�漴ɫ//////////////
            //IRandomColorRamp pColorRandom = new RandomColorRampClass();
            //pColorRandom.StartHue = 40;
            //pColorRandom.EndHue = 120;
            //pColorRandom.MinValue = 65;
            //pColorRandom.MaxValue = 90;
            //pColorRandom.MinSaturation = 25;
            //pColorRandom.MaxSaturation = 45;
            //pColorRandom.Size = GetUnirueValues.Length;
            //pColorRandom.Seed = 23;
            //bool IsCreateRamp = false;
            //pColorRandom.CreateRamp(out IsCreateRamp);
            //IEnumColors pEnumColors = pColorRandom.Colors;
            foreach (string GetUserName in GetUnirueValues)
            {
                ///////��Ⱦ�����û�
                //ISymbol pSymbolUser = null;//δ�仯
               // pSymbolUser = CreateSymbol(pFeatureLayer.FeatureClass.ShapeType, pEnumColors.Next());
                if (GetUserName == sCurrentUserName) continue;
                pUniqueValueRenderer.AddValue(GetUserName + "|1", "", pSymbolOtherUser);
               // pUniqueValueRenderer.AddValue(GetVersion.ToString() + "|1", "", pSymbol1);
            }
            //////��Ⱦ��ǰ�û�////
            pUniqueValueRenderer.AddValue(sCurrentUserName + "|1", "", pSymbolCurrentUser);
            IGeoFeatureLayer pGeoFeatLay = pFeatureLayer as IGeoFeatureLayer;
            if (pGeoFeatLay != null) pGeoFeatLay.Renderer = pUniqueValueRenderer as IFeatureRenderer;

        }
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="pGeometryType"></param>
        /// <param name="intR"></param>
        /// <param name="intG"></param>
        /// <param name="intB"></param>
        /// <returns></returns>
        private ISymbol CreateSymbol(esriGeometryType pGeometryType, int intR, int intG, int intB)
        {
            ISymbol pSymbol = null;
            ISimpleLineSymbol pSimpleLineSymbol = null;
            IColor pColor = GetRGBColor(intR, intG, intB);
            switch (pGeometryType)
            {
                case esriGeometryType.esriGeometryPolygon:
                    pSimpleLineSymbol = new SimpleLineSymbolClass();
                    pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                    pSimpleLineSymbol.Color = GetRGBColor(156, 156, 156);
                    pSimpleLineSymbol.Width = 0.01;
                    ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbolClass();
                    pSimpleFillSymbol.Outline = pSimpleLineSymbol;
                    pSimpleFillSymbol.Color = pColor;
                    pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
                    pSymbol = pSimpleFillSymbol as ISymbol;
                    break;
                case esriGeometryType.esriGeometryPoint:
                    ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
                    pSimpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                    pSimpleMarkerSymbol.Color = pColor;
                    pSimpleMarkerSymbol.Size = 2;
                    pSymbol = pSimpleMarkerSymbol as ISymbol;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    pSimpleLineSymbol = new SimpleLineSymbolClass();
                    pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                    pSimpleLineSymbol.Color = pColor;
                    pSimpleLineSymbol.Width = 0.1;
                    pSymbol = pSimpleLineSymbol as ISymbol;
                    break;
            }

            return pSymbol;
        }
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="pGeometryType"></param>
        private ISymbol CreateSymbol(esriGeometryType pGeometryType, IColor in_Color)
        {
            ISymbol pSymbol = null;
            ISimpleLineSymbol pSimpleLineSymbol = null;
            IColor pColor = in_Color;
            switch (pGeometryType)
            {
                case esriGeometryType.esriGeometryPolygon:
                    pSimpleLineSymbol = new SimpleLineSymbolClass();
                    pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                    pSimpleLineSymbol.Color = GetRGBColor(156, 156, 156);
                    pSimpleLineSymbol.Width = 0.01;
                    ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbolClass();
                    pSimpleFillSymbol.Outline = pSimpleLineSymbol;
                    pSimpleFillSymbol.Color = pColor;
                    pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
                    pSymbol = pSimpleFillSymbol as ISymbol;
                    break;
                case esriGeometryType.esriGeometryPoint:
                    ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
                    pSimpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                    pSimpleMarkerSymbol.Color = pColor;
                    pSimpleMarkerSymbol.Size = 2;
                    pSymbol = pSimpleMarkerSymbol as ISymbol;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    pSimpleLineSymbol = new SimpleLineSymbolClass();
                    pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                    pSimpleLineSymbol.Color = pColor;
                    pSimpleLineSymbol.Width = 0.1;
                    pSymbol = pSimpleLineSymbol as ISymbol;
                    break;
            }

            return pSymbol;
        }


        /// <summary>
        /// ����ͼ����ȾUniqueValueRenderer
        /// </summary>
        /// <param name="pFeatLay">��Ⱦͼ��</param>
        /// <param name="strFieldName">��Ⱦ�ֶ�</param>
        /// <param name="dicFieldValue">��Ⱦֵ��(�ֶ�ֵ,��Ⱦ����)</param>
        /// <param name="dicFieldSymbol">��ȾSymbol��(�ֶ�ֵ,Symbol)</param>
        private void SetLayerUniqueValueRenderer(IFeatureLayer pFeatLay, string strFieldName, Dictionary<string, string> dicFieldValue, Dictionary<string, ISymbol> dicFieldSymbol, bool bUseDefaultSymbol)
        {
            if (pFeatLay == null || strFieldName == string.Empty || dicFieldValue == null || dicFieldSymbol == null) return;
            IFeatureClass pFeatCls = pFeatLay.FeatureClass;
            IUniqueValueRenderer pUniqueValueRenderer = new UniqueValueRendererClass();
            pUniqueValueRenderer.FieldCount = 1;
            pUniqueValueRenderer.set_Field(0, strFieldName);
            if (bUseDefaultSymbol == true)
            {
                pUniqueValueRenderer.UseDefaultSymbol = true;
            }
            else
            {
                pUniqueValueRenderer.UseDefaultSymbol = false;
            }
            foreach (KeyValuePair<string, string> keyValue in dicFieldValue)
            {
                if (dicFieldSymbol.ContainsKey(keyValue.Key))
                {
                    pUniqueValueRenderer.AddValue(keyValue.Key, "", dicFieldSymbol[keyValue.Key]);
                    pUniqueValueRenderer.set_Label(keyValue.Key, keyValue.Value);
                }
            }

            IGeoFeatureLayer pGeoFeatLay = pFeatLay as IGeoFeatureLayer;
            if (pGeoFeatLay != null) pGeoFeatLay.Renderer = pUniqueValueRenderer as IFeatureRenderer;
        }
        /// <summary>
        /// ��ȡRGB
        /// </summary>
        /// <param name="lngR"></param>
        /// <param name="lngG"></param>
        /// <param name="lngB"></param>
        /// <returns></returns>
        private IRgbColor GetRGBColor(int lngR, int lngG, int lngB)
        {
            IRgbColor rgbColor = new RgbColorClass();
            rgbColor.Red = lngR;
            rgbColor.Green = lngG;
            rgbColor.Blue = lngB;
            rgbColor.UseWindowsDithering = false;

            return rgbColor;
        }

        private void btn_test_Click(object sender, EventArgs e)
        {

        }
         /// <summary>
         /// ��ȡһ��Ҫ�ؼ�ĳ���ֶε�Ψһֵ
         /// </summary>
         /// <param name="pFeatureClass">IFeatureClass����</param>
         /// <param name="strFld">��ȡΨһֵ���ֶ�</param>
         /// <returns>string[]Ψһֵ�б�</returns>
����     public static string[] GetUniqueValue(IFeatureClass pFeatureClass,string strFld)
����     {
����         //�õ�IFeatureCursor�α�
����         IFeatureCursor pCursor=pFeatureClass.Search(null,false);   ����
����         //coClass����ʵ������
����         IDataStatistics pdata=new DataStatisticsClass();
             pdata.Field = strFld;
             pdata.Cursor = pCursor as ICursor;   ����
����         //ö��Ψһֵ
             IEnumerator pEnumVar = pdata.UniqueValues; ����
����         //��¼����
             int RecordCount = pdata.UniqueValueCount;  ����
����         //�ַ�����
����         string[] strValue=new string[RecordCount]; 
����         pEnumVar.Reset();   ���� 
����         int i=0; ����
����         while(pEnumVar.MoveNext())
����         {
����            strValue[i++]=pEnumVar.Current.ToString();
����         } ���� 
����         return strValue;
          }

        /// <summary>
        /// ȡ��һ������ķ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancleAss_Click(object sender, EventArgs e)
        {
            if (this.axMapControl2.LayerCount <= 0)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ط�Χͼ��");
                return;
            }
            try
            {
                IFeatureLayer GetLayer = this.axMapControl2.get_Layer(0) as IFeatureLayer;
                IFeatureClass GetFeaCls = GetLayer.FeatureClass;
                IFeatureSelection fealSel = GetLayer as IFeatureSelection;
                string sUserName = this.comboBox_USER.Text;
                string sUserID = this.comboBox_USER.SelectedValue.ToString();
                //cyf  20110603 modify
                ////////////////////�ж��Ƿ����˰汾////////////////////
                //IVersionedObject pVersion = this.m_SDETaskLayer as IVersionedObject;
                //if (!pVersion.IsRegisteredAsVersioned) pVersion.RegisterAsVersioned(true);
                ////////////////////////////////////////////////
                //end
                //cyf 20110603 modify
                //OracleConnection Con = new OracleConnection(ModuleData.v_AppConnStr);
                IWorkspaceEdit SDEWs = (this.m_SDETaskLayer as IDataset).Workspace as IWorkspaceEdit;
                int iUserFieldIndex = m_SDETaskLayer.Fields.FindField("USERNAME");
                //SDEWs.StartEditing(false);
                //SDEWs.StartEditOperation();
                //bool bSave = false;
                if (fealSel.SelectionSet.Count != 0)
                {
                    //Con.Open();
                    IEnumIDs pEnumIDs = fealSel.SelectionSet.IDs;
                    int id = pEnumIDs.Next();
                    while (id != -1)
                    {
                        //////////��ԭ�з�����Ϣ���
                        IFeature GetFea = GetFeaCls.GetFeature(id);
                        string sRangeID = string.Empty;
                        int iAssign = 0;
                        int Index = GetFea.Fields.FindField("RANGEID");
                        if (Index < 0) sRangeID = "null";
                        else sRangeID = GetFea.get_Value(Index).ToString();
                        Index = GetFea.Fields.FindField("assign");
                        if (Index >= 0) GetFea.set_Value(Index, 0);
                        ///////ɾ�����еķ����¼/////
                        string SQL = string.Empty;
                        SQL = "DELETE FROM updateinfo WHERE PROJECTID=" + this.m_DBProjectID + " AND " + "RANGEID=" + sRangeID;
                        //cyf 20110603 modify
                        ModuleData.TempWks.ExecuteSQL(SQL);
                        //end
                        //OracleCommand Com = new OracleCommand(SQL, Con);
                        //Com.ExecuteNonQuery();
                        id = pEnumIDs.Next();
                        //*********��շ�����û�**************
                        if (iUserFieldIndex >= 0) GetFea.set_Value(iUserFieldIndex, "");
                        GetFea.Store();
                        //bSave = true;
                    }
                    //SDEWs.StopEditOperation();
                    //SDEWs.StopEditing(bSave);
                    //end
                    SysCommon.Error.ErrorHandle.ShowInform("��ʾ", "ȡ���������");
                    SetDataUpdateSymbol(GetLayer);
                    this.axMapControl2.Refresh();
                }
            }
            catch (Exception eError)
            {
                if (ModuleData.v_SysLog != null) ModuleData.v_SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                ModuleData.v_SysLog.Write(eError);
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "ȡ���������" + eError.Message);
            }
        }

        private void comboBox_USER_SelectedIndexChanged(object sender, EventArgs e)
        {
            //cyf 20110603 add:����û���Ӧ�Ľ�ɫID
            string pUserIdStr = this.comboBox_USER.SelectedValue.ToString().Trim();  //�û�ID
            IFeatureWorkspace pFeaWs = ModuleData.TempWks as IFeatureWorkspace;
            if (pFeaWs == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ϵͳά����ʧ�ܣ�");
                return;
            }
            IQueryDef pQueryDef = pFeaWs.CreateQueryDef();
            pQueryDef.Tables = "user_role,role";
            pQueryDef.SubFields = "user_role.ROLEID";
            pQueryDef.WhereClause = "user_role.USERID='" + pUserIdStr + "' and role.TYPEID='"+EnumRoleType.��ҵԱ.GetHashCode().ToString()+"'";
            ICursor pCursor = pQueryDef.Evaluate();
            if (pCursor == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ�û���Ϣʧ�ܣ�");
                return;
            }
            IRow pRow = pCursor.NextRow();
            if (pRow != null)
            {
                m_iRoleId = pRow.get_Value(0).ToString();
            }
            //end
            //////////��Ⱦ///////////////
            if (this.axMapControl2.LayerCount > 0)
            {
                IFeatureLayer GetFirstLayer = this.axMapControl2.get_Layer(0) as IFeatureLayer;
                SetDataUpdateSymbol(GetFirstLayer);
            }
            this.axMapControl2.Refresh();
        }
        
        //cyf  20110603  add
        private void cmbProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            Exception outError=null;
            string proIdStr = this.cmbProject.SelectedValue.ToString().Trim();  //��Ŀ��ϢID
            //��ȡ�������û���Ϣ
            GetAllUser(proIdStr, out outError);
            if (outError != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������ҵԱ��Ϣʧ�ܣ�ԭ��" + outError.Message);
                return;
            }
        }
    }

         

}