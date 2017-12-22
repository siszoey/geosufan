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
    public class frmMain : Fan.Common.BaseRibbonForm
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
            InitializeComponent();
            //�ȴ�����
            _frmTemp = new frmLoadProgress();
            _frmTemp.Show(this);
            Fan.Plugin.ModuleCommon.SysLogInfoChnaged += new Fan.Common.SysLogInfoChangedHandle(SysLogInfoChnaged);
            //���ؽ�����Ϣ
            InitialFrm();
            _frmTemp.Close();
        }
        /// <summary>
        ///��ʼ������
        /// </summary>
        /// <param name="pGroupFunc">��ID</param>
        private void InitialFrm()
        {
            XmlDocument docXml = null;
            docXml = new XmlDocument();
            if (!File.Exists(Mod.m_SysXmlPath)) return;
            docXml.Load(Mod.m_SysXmlPath);
            List<string> lstUserPrivilegeID = new List<string>();

            lstUserPrivilegeID = Mod.v_ListUserPrivilegeID;
            Fan.Plugin.ModuleCommon.ListUserPrivilegeID = Mod.v_ListUserPrivilegeID;
            Fan.Plugin.ModuleCommon.ListUserdataPriID = Mod.v_ListUserdataPriID;
            _frmTemp.SysInfo = "��ȡϵͳ���ܲ����...";
            _frmTemp.RefreshLable();
            //�Ӳ���ļ����л�ȡ����ӿڶ���
            Fan.Plugin.Parse.PluginHandle pluginHandle = new Fan.Plugin.Parse.PluginHandle();
            pluginHandle.PluginFolderPath = Mod.m_PluginFolderPath;
            Fan.Plugin.Parse.PluginCollection pluginCol = pluginHandle.GetPluginFromDLL();
            Mod.m_PluginCol = pluginCol;
            //��ʼ������ܶ���
            Mod.v_AppForm = new Fan.Plugin.Application.AppForm(this, null, docXml, null, null, pluginCol, Mod.m_ResPath);
            //������ݿ�������Ϣ
            Fan.Plugin.Application.ICustomWks tempWks = new Fan.Plugin.Application.ICustomWks();
            tempWks.Wks = Mod.TempWks;
            tempWks.Server = Mod.Server;
            tempWks.Service = Mod.Instance;
            tempWks.Database = Mod.Database;
            tempWks.User = Mod.User;
            tempWks.PassWord = Mod.Password;
            tempWks.Version = Mod.Version;
            tempWks.DBType = Mod.dbType;
            Mod.v_AppForm.TempWksInfo = tempWks;

            Fan.Plugin.Application.ICustomWks curWks = new Fan.Plugin.Application.ICustomWks();
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
            Fan.Plugin.ModuleCommon.IntialModuleCommon(lstUserPrivilegeID, docXml, Mod.m_ResPath, pluginCol, Mod.m_LogPath);
            //����XML���ز������
            Fan.Plugin.ModuleCommon.LoadFormByXmlNode(Mod.v_AppForm as Fan.Plugin.Application.IApplicationRef);
            Fan.Common.SysLogInfoChangedEvent newEvent = new Fan.Common.SysLogInfoChangedEvent("��������...");
            SysLogInfoChnaged(null, newEvent);
            //����XML���ز������
            Fan.Plugin.ModuleCommon.LoadData(Mod.v_AppForm as Fan.Plugin.Application.IApplicationRef);
        }
        private void SysLogInfoChnaged(object sender, Fan.Common.SysLogInfoChangedEvent e)
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1196, 690);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void frmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
