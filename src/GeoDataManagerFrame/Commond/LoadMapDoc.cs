using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;

//���ص�ͼ�ĵ�
namespace GeoDataManagerFrame
{
    public class LoadMapDoc : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
        private Plugin.Application.IAppFormRef m_frmhook;
        public LoadMapDoc()
        {
            base._Name = "GeoDataManagerFrame.LoadMapDoc";
            base._Caption = "���ص�ͼ�ĵ�";
            base._Tooltip = "���ص�ͼ�ĵ�";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "���ص�ͼ�ĵ�";
        }

        public override void OnClick()
        {
            if (m_Hook == null)
                return;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "xml�ļ�|*.xml";
            dlg.ShowDialog();
            string xmlpath = dlg.FileName;
            if (xmlpath.Equals(""))
                return;
            SysCommon.CProgress vProgress = new SysCommon.CProgress("������");
            vProgress.EnableCancel = false;
            vProgress.ShowDescription = true;
            vProgress.FakeProgress = true;
            vProgress.TopMost = true;
            vProgress.ShowProgress();

            LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
            string strLog = "���ص�ͼ�ĵ�,ԭʼ�ļ�·��:" + xmlpath;
            vProgress.SetProgress("��ʼ���ص�ͼ�ĵ�");
            if (log != null)
            {
                log.Writelog(strLog);
            }

            string strWorkFile = Application.StartupPath + "\\..\\Temp\\CurPrj.xml";
           
            System.IO.File.Copy(xmlpath, strWorkFile, true);
            
            SetControl pSetControl=(SetControl )m_Hook.MainUserControl;

            //���ǰ�μ��ص�ͼ��

            pSetControl.m_SaveXmlFileName = xmlpath;
            pSetControl.LoadDatafromXml(strWorkFile, vProgress);
            vProgress.Close();

        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppGisUpdateRef;
            m_frmhook = hook as Plugin.Application.IAppFormRef;
        }
    }
}
