using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using GeoDataCenterFunLib;
//�������ݵ�Ԫ����ͼ
namespace GeoDBConfigFrame
{
    public class UpLoadDataUnitIndexMap : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;

        public UpLoadDataUnitIndexMap()
        {
            base._Name = "GeoDBConfigFrame.UpLoadDataUnitIndexMap";
            base._Caption = "�������ݵ�Ԫ����ͼ";
            base._Tooltip = "�������ݵ�Ԫ����ͼ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�������ݵ�Ԫ����ͼ";
        }

        public override void OnClick()
        {
            //��ȫ��������������в��ҵ���ǰ��Ҫ�л��Ĵ��롣���Ƶ����ݵ�Ԫ����
            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("�������ݵ�Ԫ����ͼ");

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
