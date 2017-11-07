using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using GeoDataCenterFunLib;
//���������߼�����Դ
namespace GeoDBConfigFrame
{
    public class SetLogicDataSource : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;

        public SetLogicDataSource()
        {
            base._Name = "GeoDBConfigFrame.SetLogicDataSource";
            base._Caption = "���������߼�����Դ";
            base._Tooltip = "���������߼�����Դ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "���������߼�����Դ";
        }

        public override void OnClick()
        {
            //��ȫ��������������в��ҵ���ǰ��Ҫ�л��Ĵ��롣���Ƶ����ݵ�Ԫ����

            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("���������߼�����Դ");

                }
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppPrivilegesRef; 
        }
    }
}
