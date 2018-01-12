using System;
using Fan.Plugin.Parse;
using Fan.Common;
using Fan.Plugin;
using DevExpress.XtraBars.Ribbon;
using Fan.Plugin.Interface;

namespace GDBM
{
    public class frmMain : BaseRibbonForm
    {
        //�ȴ�����
        private frmLoadProgress _frmTemp;
        /// <summary>
        /// ����������������
        /// </summary>
        private System.ComponentModel.IContainer components = null;
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
            m_MainPluginUI = new PluginUI(Mod.m_LoginUser);
            m_MainPluginUI.SysLogInfoChnaged += new SysLogInfoChangedHandle(SysLogInfoChnaged);
            this.mainRibbon.SelectedPageChanged += MainRibbon_SelectedPageChanged;
            //���ؽ�����Ϣ
            InitialFrm();
            _frmTemp.Close();
        }
        /// <summary>
        /// RibbonPageѡ����Ƿ������Ӧ�������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainRibbon_SelectedPageChanged(object sender, EventArgs e)
        {
            RibbonControl ribbonControl = sender as RibbonControl;
            RibbonPage seletPage = ribbonControl.SelectedPage;
            if (seletPage == null) return;
            string strFName = seletPage.Name;
            if (m_MainPluginUI.m_AllPlugin.dicPlugins.ContainsKey(strFName))
            {
                IPlugin plugin = m_MainPluginUI.m_AllPlugin.dicPlugins[strFName];
                if (plugin is ICommandRef)
                {
                    (plugin as ICommandRef).OnClick();
                }
            }
        }
        PluginUI m_MainPluginUI = default(PluginUI);
        private void InitialSysConfig()
        {
            Mod.m_sysConfig = new SysConfig(Mod.m_SysDbOperate);
            this.Text = Mod.m_sysConfig.SystemName;
        }
        /// <summary>
        ///��ʼ������
        /// </summary>
        private void InitialFrm()
        {
            _frmTemp.SysInfo = "��ȡϵͳ���ܲ����...";
            _frmTemp.RefreshLable();
            //�Ӳ���ļ����л�ȡ����ӿڶ���
            PluginHandle pluginHandle = new PluginHandle(Mod.m_PluginFolderPath);
            PluginCollection pluginCol = pluginHandle.GetPluginFromDLL();
            //��ʼ������ܶ���
            Mod.v_AppForm = new Fan.Plugin.Application.AppForm(this, pluginCol);
            //�����������ȡ���
            m_MainPluginUI.IntialModuleCommon(Mod.m_LoginUser, pluginCol);
            //�����û�Ȩ�޼�ֵ����
            string strMessage=m_MainPluginUI.LoadForm(Mod.v_AppForm as Fan.Plugin.Application.IApplicationRef);
            if (!string.IsNullOrEmpty(strMessage))
            {
                LogManage.WriteLog(strMessage);
            }
            SysLogInfoChangedEvent newEvent = new SysLogInfoChangedEvent("��������...");
            SysLogInfoChnaged(null, newEvent);
            //����XML���ز������
            m_MainPluginUI.LoadData(Mod.v_AppForm as Fan.Plugin.Application.IApplicationRef);
        }
        private void SysLogInfoChnaged(object sender, SysLogInfoChangedEvent e)
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
            this.IsMdiContainer = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new EventHandler(frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
        private void frmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
