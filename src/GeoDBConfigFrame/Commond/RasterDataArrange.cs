using System;
using System.Collections.Generic;
using System.Text;
using GeoDataCenterFunLib;

//դ����������
namespace GeoDBConfigFrame
{
    public class RasterDataArrange : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
        public RasterDataArrange()
        {
            base._Name = "GeoDBConfigFrame.RasterDataArrange";
            base._Caption = "դ����������";
            base._Tooltip = "դ����������";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "դ����������";
        }

        public override void OnClick()
        {

            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("դ����������");

                }
            }
            frmRasterDataReduction frm = new frmRasterDataReduction();
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
