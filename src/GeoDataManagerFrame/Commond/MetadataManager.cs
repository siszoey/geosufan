using System;
using System.Collections.Generic;
using System.Text;

namespace GeoDataManagerFrame
{
  public class MetadataManager : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
       public MetadataManager()
        {
            base._Name = "GeoDataManagerFrame.MetadataManager";
            base._Caption = "Ԫ���ݹ���";
            base._Tooltip = "Ԫ���ݹ���";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "Ԫ���ݹ���";
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
