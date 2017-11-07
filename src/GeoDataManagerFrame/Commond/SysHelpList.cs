using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace GeoDataManagerFrame
{
    public class SysHelpList : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
        private Plugin.Application.IAppFormRef m_frmhook;
        public SysHelpList()
        {
            base._Name = "GeoDataManagerFrame.SysHelpList";
            base._Caption = "Ŀ¼";
            base._Tooltip = "Ŀ¼";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "Ŀ¼";
        }

        public override void OnClick()
        {
            if (m_Hook == null)
                return;
            string path = Application.StartupPath + "\\..\\Help\\����������Ϣ���ݿ����ϵͳ�û��ֲ�.chm";
            HelpNavigator navigator = new HelpNavigator();
            navigator = HelpNavigator.TableOfContents;//Ŀ¼ö��
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
            m_Hook = hook as Plugin.Application.IAppGisUpdateRef;
            m_frmhook = hook as Plugin.Application.IAppFormRef;
        }
    }
}
