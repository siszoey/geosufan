using System;
using System.Collections.Generic;
using System.Text;

//��ר�ⷢ��
namespace GeoDataManagerFrame
{
    public class DataOutBySub : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
        private Plugin.Application.IAppFormRef m_frmhook;
        public DataOutBySub()
        {
            base._Name = "GeoDataManagerFrame.DataOutBySub";
            base._Caption = "��ר�ⷢ��";
            base._Tooltip = "��ר�ⷢ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "��ר�ⷢ��";
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
