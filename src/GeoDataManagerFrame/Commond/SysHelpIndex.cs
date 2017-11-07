using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
//����
namespace GeoDataManagerFrame
{
    public class SysHelpIndex : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
        private Plugin.Application.IAppFormRef m_frmhook;
        public SysHelpIndex()
        {
            base._Name = "GeoDataManagerFrame.SysHelpIndex";
            base._Caption = "����";
            base._Tooltip = "����";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "����";
        }

        public override void OnClick()
        {
            if (m_Hook == null)
                return;
            string path = Application.StartupPath + "\\..\\Help\\����������Ϣ���ݿ����ϵͳ�û��ֲ�.chm";
            HelpNavigator navigator = new HelpNavigator();
            navigator = HelpNavigator.Index;//����ö��
            Help.ShowHelp(m_frmhook.MainForm, path, navigator);


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
