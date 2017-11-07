using System;
using System.Collections.Generic;
using System.Text;
using GeoDataCenterFunLib;
//����
namespace GeoDBConfigFrame
{
    public class SysExplain : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
        public SysExplain()
        {
            base._Name = "GeoDBConfigFrame.SysExplain";
            base._Caption = "����";
            base._Tooltip = "����";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "����";
        }

        public override void OnClick()
        {
            if (m_Hook == null)
                return;
            frmExplain frm = new frmExplain(1);
            frm.ShowDialog();

        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppPrivilegesRef;
        }
    }
}
