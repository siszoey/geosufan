using System;
using System.Collections.Generic;
using System.Text;

namespace GeoDataManagerFrame
{
   public class MapTdlyztgh : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
       public MapTdlyztgh()
        {
            base._Name = "GeoDataManagerFrame.MapTdlyztgh";
            base._Caption = "������������滮ͼ";
            base._Tooltip = "������������滮ͼ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "������������滮ͼ";
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
        }
    }
}
