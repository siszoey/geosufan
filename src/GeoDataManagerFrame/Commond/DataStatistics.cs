using System;
using System.Collections.Generic;
using System.Text;

//����ͳ��
namespace GeoDataCenterFrame
{
    public class DataStatistics : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
        private Plugin.Application.IAppFormRef m_frmhook;
        public DataStatistics()
        {
            base._Name = "GeoDataCenterFrame.DataStatistics";
            base._Caption = "����ͳ��";
            base._Tooltip = "����ͳ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "����ͳ��";
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
