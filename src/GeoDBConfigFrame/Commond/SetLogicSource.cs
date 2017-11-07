using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using GeoDataCenterFunLib;

namespace GeoDBConfigFrame
{
   public class SetLogicSource : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
        public SetLogicSource()
        {
            base._Name = "GeoDBConfigFrame.SetLogicSource";
            base._Caption = "�����߼�����Դ";
            base._Tooltip = "�����߼�����Դ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�����߼�����Դ";
        }

        public override void OnClick()
        {
            //��ȫ��������������в��ҵ���ǰ��Ҫ�л��Ĵ��롣���Ƶ����ݵ�Ԫ����

            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("�����߼�����Դ");

                }
                SetLogicDataSourceForm frm=new SetLogicDataSourceForm();
                frm.Show();
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
