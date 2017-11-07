using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using SysCommon;
using SysCommon.Gis;
using SysCommon.Error;
using ESRI.ArcGIS.Geodatabase;

namespace GeoDatabaseManager
{
    public partial class frmDBSet : DevComponents.DotNetBar.Office2007Form
    {
        private enumWSType wsType;                 //���������ݿ�����
        public enumWSType WsType
        {
            set { wsType = value; }
            get { return wsType; }
        }

        private SysGisDataSet gisDataSet;           //���ݿ������

        /// <summary>
        /// ������
        /// </summary>
        private IWorkspace _Workspace;
        public IWorkspace WorkSpace
        {
            get { return _Workspace; }
            set { _Workspace = value; }
        }

        /// <summary>
        /// ���ƿ�
        /// </summary>
        private IWorkspace _CurWorkspace;
        public IWorkspace CurWorkSpace
        {
            get { return _CurWorkspace; }
            set { _CurWorkspace = value; }
        }

        //string server;              //������
        //string service ;            //������
        //string dataBase ;           //���ݿ�
        //string user ;               //�û���
        //string password ;           //����
        //string version ;            //�汾

        public frmDBSet()
        {
            InitializeComponent();
        }

        /// <summary>
        /// �õ������ռ�
        /// </summary>
        /// <param name="eError"></param>
        /// <returns></returns>
        public IWorkspace GetWorkspace(string server,string service,string dataBase,string user,string password,string version,enumWSType type,out Exception eError)
        {
            eError = null;
            bool result = false;
            
            if (gisDataSet == null)
            {
                gisDataSet = new SysGisDataSet();
            }

            try
            {
                switch (type)
                {
                    case enumWSType.SDE:
                        result=gisDataSet.SetWorkspace(server, service, dataBase, user, password, version, out eError);
                        break;
                    case enumWSType.PDB:
                    case enumWSType.GDB:
                        result=gisDataSet.SetWorkspace(server, wsType,out eError);
                        break;
                    default:
                        break;
                }
                if (result)
                {
                    return gisDataSet.WorkSpace;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                eError = ex;
                return null;
            }
        }

        private void frmDBSet_Load(object sender, EventArgs e)
        {
            //�ж���ʵ�������
            try
            {
                if (File.Exists(Mod.v_ConfigPath))
                {
                    //������
                    string strServer, strType, strInstance, strDatabase, strUser, strPassword, strVersion;
                    SysCommon.Authorize.AuthorizeClass.GetConnectInfo(Mod.v_ConfigPath, out strServer, out strInstance, out strDatabase, out strUser, out strPassword, out strVersion, out strType);
                    this.ConSet.DatabaseType = strType;
                    this.ConSet.Server = strServer;
                    this.ConSet.Service = strInstance;
                    this.ConSet.DataBase = strDatabase;
                    this.ConSet.User = strUser;
                    this.ConSet.Password = strPassword;
                    this.ConSet.Version = strVersion;
                }
            }
            catch 
            {
                
            }
        }

        private void buttonXOK_Click(object sender, EventArgs e)
        {
            Exception eError = null;
            if (_Workspace == null)
            {
                _Workspace = this.ConSet.GetWks();
            }
            if (_Workspace == null)
            {
                MessageBox.Show("�޷�����ָ�����ݿ⣬����������Ϣ��","��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //�����ʽ��������Ϣ
            string strServer = "";
            string strSevice = "";
            string strDatabase = "";
            string strUser = "";
            string strPass = "";
            string strVersion = "";
            string strType = "";

            enumWSType WsCurType = enumWSType.SDE;
            //if (_CurWorkspace == null)
            //{
            //    SysCommon.Authorize.AuthorizeClass.GetCurWks(_Workspace, out strServer, out strSevice, out strDatabase, out strUser, out strPass, out strVersion, out strType);

            //    if (strType.ToUpper() == "ORACLE" || strType.ToUpper() == "SQUSERVER")
            //    {
            //        WsCurType = enumWSType.SDE;
            //    }
            //    else if (strType.ToUpper() == "ACCESS")
            //    {
            //        WsCurType = enumWSType.PDB;
            //    }
            //    else if (strType.ToUpper() == "FILE")
            //    {
            //        WsCurType = enumWSType.GDB;
            //    }

            //    Exception Err = null;
            //    _CurWorkspace = GetWorkspace(strServer, strSevice, strDatabase, strUser, strPass, strVersion, WsCurType, out Err);
            //}

            if (_Workspace != null)
            {
                //�������л�
                System.Collections.Hashtable conset = new System.Collections.Hashtable();
                ////������
                conset.Add("dbtype", this.ConSet.DatabaseType);
                conset.Add("server", this.ConSet.Server);
                conset.Add("service", this.ConSet.Service);
                conset.Add("database", this.ConSet.DataBase);
                conset.Add("user", this.ConSet.User);
                conset.Add("password", this.ConSet.Password);
                conset.Add("version", this.ConSet.Version);

                SysCommon.Authorize.AuthorizeClass.Serialize(conset, Mod.v_ConfigPath);

                //��һ�¹�����
                Mod.Server = this.ConSet.Server;
                Mod.Instance = this.ConSet.Service;
                Mod.Database = this.ConSet.DataBase;
                Mod.User = this.ConSet.User;
                Mod.Password = this.ConSet.Password;
                Mod.Version = this.ConSet.Version;
                Mod.dbType = this.ConSet.DatabaseType;
                Mod.TempWks = _Workspace;

                //��ʼ��ϵͳ���� added by chulili 20110531
                IWorkspaceFactory pWorkspaceFactory = null;
                IWorkspace pWorkspace = null;
                ESRI.ArcGIS.esriSystem.IPropertySet pPropertySet = new ESRI.ArcGIS.esriSystem.PropertySetClass();
                //��ȡ��ʼ��ϵͳ���õı��ģ��
                string dataPath = Application.StartupPath + "\\..\\Template\\DbInfoTemplate.gdb";
                pPropertySet.SetProperty("DATABASE", dataPath);
                pWorkspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.FileGDBWorkspaceFactoryClass();
                pWorkspace = pWorkspaceFactory.Open(pPropertySet, 0);
                if (pWorkspace != null)
                {

                    try//�ӱ�����xisheng 20110817 ������ʧ�ܵ�����Ϣ����Ҫ�ҵ���
                    { //�Ƿ񸲸ǣ�false  ���ԭ����ϵͳ���ñ��򲻸���
                        ModuleOperator.InitSystemByXML(pWorkspace, _Workspace, false);
                    }
                    catch(Exception err)
                    {

                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "ϵͳά�����ʼ��ʧ�ܣ�");
                        _Workspace = null;//����ʧ�ܺ󣬽�ԭ�����ÿ�����Ϊ�� xisheng 20110817 
                        return;
                    }
                }//end  added by chulili 20110531

                this.DialogResult = DialogResult.OK;

            }
            else
            {
                if (eError != null)
                {
                    ErrorHandle.ShowInform("��ʾ", eError.Message);
                }
            }
        }

        private void buttonXCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}