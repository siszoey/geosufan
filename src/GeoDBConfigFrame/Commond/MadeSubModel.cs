using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using GeoDataCenterFunLib;
//�༭ר��ű�
namespace GeoDBConfigFrame
{
    public class MadeSubModel : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;

        public MadeSubModel()
        {
            base._Name = "GeoDBConfigFrame.MadeSubModel";
            base._Caption = "�༭ר��ű�";
            base._Tooltip = "�༭ר��ű�";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�༭ר��ű�";
        }

        public override void OnClick()
        {
            //��ȫ��������������в��ҵ���ǰ��Ҫ�л��Ĵ��롣���Ƶ����ݵ�Ԫ����

            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("�༭ר��ű�");
                }

                SubIndexScript dlg = new SubIndexScript();
                dlg.Show();
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
