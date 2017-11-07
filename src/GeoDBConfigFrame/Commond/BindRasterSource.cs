using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;

namespace GeoDBConfigFrame
{
    class BindRasterSource : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
    
     public BindRasterSource()
        {
            base._Name = "GeoDBConfigFrame.BindRasterSource";
            base._Caption = "դ�����ݹҽ�";
            base._Tooltip = "դ�����ݹҽ�";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "դ�����ݹҽ�";
        }

        public override void OnClick()
        {

            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("Ӱ���ҽ�");

                }
            }
            frmBindSource frm = new frmBindSource(0);
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
