using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using GeoDataCenterFunLib;
//������ݷ���
namespace GeoDBConfigFrame
{
    public class AnalyseInLibMap : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;

        public AnalyseInLibMap()
        {
            base._Name = "GeoDBConfigFrame.AnalyseInLibMap";
            base._Caption = "������ݷ���";
            base._Tooltip = "������ݷ���";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "������ݷ���";
        }

        public override void OnClick()
        {
            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("���ͼ�����ݷ���");

                }
            }
            //��ȫ��������������в��ҵ���ǰ��Ҫ�л��Ĵ��롣���Ƶ����ݵ�Ԫ����
            frmAnalyseInLibMap frm = new frmAnalyseInLibMap();
            frm.Show();
           
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppPrivilegesRef; 
        }
    }
}
