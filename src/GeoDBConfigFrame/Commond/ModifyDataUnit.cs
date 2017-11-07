using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
//using ModifyDataUnitControl;
using GeoDataCenterFunLib;

//�޸����ݵ�Ԫ
namespace GeoDBConfigFrame
{
    public class ModifyDataUnit : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;

        public ModifyDataUnit()
        {
            base._Name = "GeoDBConfigFrame.ModifyDataUnit";
            base._Caption = "�޸����ݵ�Ԫ";
            base._Tooltip = "�޸����ݵ�Ԫ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�޸����ݵ�Ԫ";
        }

        public override void OnClick()
        {
            //��ȫ��������������в��ҵ���ǰ��Ҫ�л��Ĵ��롣���Ƶ����ݵ�Ԫ����
            ModifyDataUnitControl dlg = new ModifyDataUnitControl();
            dlg.Show();
            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("�޸����ݵ�Ԫ");

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
