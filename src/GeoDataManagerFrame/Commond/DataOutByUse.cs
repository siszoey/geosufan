using System;
using System.Collections.Generic;
using System.Text;

//�û��Զ��巢��
namespace GeoDataManagerFrame
{
    public class DataOutByUse : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
        private Plugin.Application.IAppFormRef m_frmhook;
        public DataOutByUse()
        {
            base._Name = "GeoDataManagerFrame.DataOutByUse";
            base._Caption = "�û��Զ��巢��";
            base._Tooltip = "�û��Զ��巢��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�û��Զ��巢��";
        }

        public override void OnClick()
        {
            if (m_Hook == null)
                return;


        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppGisUpdateRef;
            m_frmhook = hook as Plugin.Application.IAppFormRef;
        }
    }
}
