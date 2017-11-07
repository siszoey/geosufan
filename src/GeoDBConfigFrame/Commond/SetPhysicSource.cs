using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using GeoDataCenterFunLib;

namespace GeoDBConfigFrame
{
   public class SetPhysicSource : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
        public SetPhysicSource()
        {
            base._Name = "GeoDBConfigFrame.SetPhysicSource";
            base._Caption = "����ͼ������Դ";
            base._Tooltip = "����ͼ������Դ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "����ͼ������Դ";
        }

        public override void OnClick()
        {
            //��ȫ��������������в��ҵ���ǰ��Ҫ�л��Ĵ��롣���Ƶ����ݵ�Ԫ����

            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("������������Դ");

                }
                SetPhysicsDataSourceForm frm=new SetPhysicsDataSourceForm();
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
