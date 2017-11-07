using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;
//�������ݿ�  �Ͽ����ݿ�
namespace GeoDBConfigFrame
{
    public class DataBaseBreak : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
        public DataBaseBreak()
        {
            base._Name = "GeoDBConfigFrame.DataBaseBreak";
            base._Caption = "�Ͽ����ݿ�";
            base._Tooltip = "�Ͽ����ݿ�";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�Ͽ����ݿ�";
        }

        public override void OnClick()
        {

            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("�Ͽ����ݿ�");

                }
            }
            //�Ͽ�����

            //���listview����
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppPrivilegesRef;
        }
    }
}
