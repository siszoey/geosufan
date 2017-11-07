using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
//����
namespace GeoDBConfigFrame
{
    public class SysHelpIndex : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
        private Plugin.Application.IAppFormRef m_frmhook;
        public SysHelpIndex()
        {
            base._Name = "GeoDBConfigFrame.SysHelpIndex";
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
            if (!File.Exists(path))
            {
                MessageBox.Show("�����ļ������� ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Help.ShowHelp(m_frmhook.MainForm, path, navigator);
            }
        

        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppPrivilegesRef;
            m_frmhook = hook as Plugin.Application.IAppFormRef;
        }
    }
}
