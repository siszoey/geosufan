using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using System.Data.OracleClient;

namespace GDBM
{
    public class frmMain : SysCommon.BaseRibbonForm
    {
        //�ȴ�����
        private frmLoadProgress _frmTemp;
        /// <summary>
        /// ����������������
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private bool _Res=false;
        /// <summary>
        /// ������������ʹ�õ���Դ��
        /// </summary>
        /// <param name="disposing">���Ӧ�ͷ��й���Դ��Ϊ true������Ϊ false��</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        public frmMain()
        {
            //���̵߳ȴ���֪�����ھ���������
            //while (!this.IsHandleCreated)
            //{
            //    ;
            //}
            InitializeComponent();

            //frmLogin frmLogin = new frmLogin();
            //string name = "";
            //if (frmLogin.ShowDialog() == DialogResult.OK)
            //{

                //�ȴ�����
                _frmTemp = new frmLoadProgress();
                _frmTemp.Show(this);

                Plugin.ModuleCommon.SysLogInfoChnaged += new SysCommon.SysLogInfoChangedHandle(SysLogInfoChnaged);
                //���ؽ�����Ϣ
                //int pGroupFuncID=21;//��ɫID��Ϣ
                //name = frmLogin.txtUser.Text;
                //InitialFrm(name);
                InitialFrm();

                _frmTemp.Close();
                
            //}
            //else
            //{
            //    return;
            //}
        }
        public frmMain(string userName, string UserPassword, string userType)
        {
            //���̵߳ȴ���֪�����ھ���������
            //while (!this.IsHandleCreated)
            //{
            //    ;
            //}
            InitializeComponent();

            //frmLogin frmLogin = new frmLogin();
            //string name = "";
            //if (frmLogin.ShowDialog() == DialogResult.OK)
            //{

            //�ȴ�����
            _frmTemp = new frmLoadProgress();
            _frmTemp.Show(this);

            Plugin.ModuleCommon.SysLogInfoChnaged += new SysCommon.SysLogInfoChangedHandle(SysLogInfoChnaged);
            //���ؽ�����Ϣ
            //int pGroupFuncID=21;//��ɫID��Ϣ
            //name = frmLogin.txtUser.Text;
            //InitialFrm(name);
            InitialFrm();

            _frmTemp.Close();
            //}
            //else
            //{
            //    return;
            //}

            // *=====================================================
            // *���ܣ�������Ϻ���ݲ�ͬ���û����뵽��ͬ���ӽ���ȥ
            // *���������Ƿ�
            // *ʱ�䣺20110520
            string pSysName = "";           //��ϵͳ����
            string pSysCaption = "";        //��ϵͳCaption
            if (userType == UserTypeEnum.SuperAdmin.GetHashCode().ToString())
            {
                //��������Ա,���ù�����ϵͳ
                pSysName = "GeoUserManager.ControlGisSysSetting";
            }
            else if (userType == UserTypeEnum.Admin.GetHashCode().ToString())
            {
                //����Ա�����ɹ�����ϵͳ
                pSysName = "GeoDBIntegration.ControlDBIntegrationTool";
            }
            else if (userType == UserTypeEnum.CommonUser.GetHashCode().ToString())
            {
                //��ͨ�û������Ĺ�����ϵͳ
                pSysName = "GeoSysUpdate.ControlSysUpdate";
            }
            //����Name�����ϵͳ��caption
            XmlDocument sysXml = new XmlDocument();
            sysXml.Load(Mod.m_SysXmlPath);
            XmlNode sysNode = sysXml.SelectSingleNode("//Main//System[@Name='" + pSysName + "']");
            if (sysNode == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������NameΪ" + pSysName + "��ϵͳ");
                return;
            }
            pSysCaption = (sysNode as XmlElement).GetAttribute("Caption").Trim();  //caption

            //�����������Ĺ�����ϵͳ����
            ModuleOperator.InitialForm(pSysName, pSysCaption);

            //������־ enter feature Db Log
            
            if (Mod.v_SysLog != null)
            {
                List<string> Pra = new List<string>();
                Mod.v_SysLog.Write("�������ù�����ϵͳ", Pra, DateTime.Now);
            }
        }
        /// <summary>
        ///��ʼ������
        /// </summary>
        /// <param name="pGroupFunc">��ID</param>
        private void InitialFrm()//(string name)
        {
            //��ȡϵͳȨ��XML
            XmlDocument docXml = null;
            //docXml = Mod.v_SystemXml;
            //if (docXml == null&& name == "Admin")
            //{
                docXml = new XmlDocument();
                if (!File.Exists(Mod.m_SysXmlPath)) return;
                docXml.Load(Mod.m_SysXmlPath);

                List<string> lstUserPrivilegeID = new List<string>();
                lstUserPrivilegeID = Mod.v_ListUserPrivilegeID;

                Plugin.ModuleCommon.ListUserPrivilegeID = Mod.v_ListUserPrivilegeID;
                Plugin.ModuleCommon.ListUserdataPriID = Mod.v_ListUserdataPriID;

            _frmTemp.SysInfo = "��ȡϵͳ���ܲ����...";
            _frmTemp.RefreshLable();

            //�Ӳ���ļ����л�ȡ����ӿڶ���
            Plugin.Parse.PluginHandle pluginHandle = new Plugin.Parse.PluginHandle();
            pluginHandle.PluginFolderPath = Mod.m_PluginFolderPath;
            Plugin.Parse.PluginCollection pluginCol = pluginHandle.GetPluginFromDLL();
            Mod.m_PluginCol = pluginCol;
            //��ʼ������ܶ���
            Mod.v_AppForm = new Plugin.Application.AppForm(this,null, docXml, null, null, pluginCol, Mod.m_ResPath);
            //������ݿ�������Ϣ
            Plugin.Application.ICustomWks tempWks = new Plugin.Application.ICustomWks();
            tempWks.Wks = Mod.TempWks;
            tempWks.Server = Mod.Server;
            tempWks.Service = Mod.Instance;
            tempWks.Database = Mod.Database;
            tempWks.User = Mod.User;
            tempWks.PassWord = Mod.Password;
            tempWks.Version = Mod.Version;
            tempWks.DBType = Mod.dbType;
            Mod.v_AppForm.TempWksInfo = tempWks;

            Plugin.Application.ICustomWks curWks = new Plugin.Application.ICustomWks();
            curWks.Wks = Mod.CurWks;
            curWks.Server = Mod.CurServer;
            curWks.Service = Mod.CurInstance;
            curWks.Database = Mod.CurDatabase;
            curWks.User = Mod.CurUser;
            curWks.PassWord = Mod.CurPassword;
            curWks.Version = Mod.CurVersion;
            curWks.DBType = Mod.CurdbType;
            Mod.v_AppForm.CurWksInfo = curWks;
            //�����������ȡ���
            Plugin.ModuleCommon.IntialModuleCommon(lstUserPrivilegeID,docXml, Mod.m_ResPath, pluginCol, Mod.m_LogPath);
            //����XML���ز������
            Plugin.ModuleCommon.LoadFormByXmlNode(Mod.v_AppForm as Plugin.Application.IApplicationRef);
            SysCommon.SysLogInfoChangedEvent newEvent = new SysCommon.SysLogInfoChangedEvent("��������...");
            SysLogInfoChnaged(null, newEvent);
            //����XML���ز������
            Plugin.ModuleCommon.LoadData(Mod.v_AppForm as Plugin.Application.IApplicationRef);
        }
        private void SysLogInfoChnaged(object sender, SysCommon.SysLogInfoChangedEvent e)
        {
            _frmTemp.SysInfo = "";
            _frmTemp.RefreshLable();
            _frmTemp.SysInfo = e.Information;
            _frmTemp.RefreshLable();
        }
        #region Windows ������ƴ���
        /// <summary>
        /// �������
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.SuspendLayout();
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(523, 342);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.ResumeLayout(false);

        }
        #endregion
        private void frmMain_Load(object sender, EventArgs e)
        {
            if (_Res)
            {
                Application.Exit();
            }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SysCommon.FrmExit pFrm = new SysCommon.FrmExit();
            DialogResult pRes = pFrm.ShowDialog();
            pFrm = null;
            if (pRes == DialogResult.Yes)
            {
                Plugin.LogTable.Writelog("�˳�ϵͳ");//yjl��¼�û��˳�ϵͳ
                Application.ExitThread();
                Application.Exit();
                System.Diagnostics.Process[] pro = System.Diagnostics.Process.GetProcessesByName("GDBM");
                foreach (System.Diagnostics.Process pc in pro)
                {
                    pc.Kill();
                }
            }
            else if (pRes == DialogResult.No)
            {
                Plugin.LogTable.Writelog("�л���ϵͳ");
                string exepath = Application.StartupPath;
                string strExecutablePath = Application.ExecutablePath;
                Application.ExitThread();
                Application.Exit();
                string picPath;
                System.Diagnostics.Process p = new System.Diagnostics.Process(); 
                picPath = string.Concat(System.IO.Path.GetDirectoryName(strExecutablePath), "\\GDBM.exe");
                System.Diagnostics.Process.Start(picPath);
            }
            else
            {
                e.Cancel = true;
            }

        }
    }
}
