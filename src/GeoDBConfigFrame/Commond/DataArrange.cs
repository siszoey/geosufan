using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;

//ʸ����������
namespace GeoDBConfigFrame
{
    public class DataArrange : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
        public DataArrange()
        {
            base._Name = "GeoDBConfigFrame.DataArrange";
            base._Caption = "ʸ����������";
            base._Tooltip = "ʸ����������";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "ʸ����������";
        }

        public override void OnClick()
        {

            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("ʸ����������");

                }
            }
            frmDataReduction frm = new frmDataReduction();
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
