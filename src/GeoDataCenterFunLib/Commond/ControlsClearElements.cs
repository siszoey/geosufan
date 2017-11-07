using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace GeoDataCenterFunLib
{
    public class ControlsClearElements : Plugin.Interface.CommandRefBase
    {

        private Plugin.Application.IAppArcGISRef _AppHk;

        public ControlsClearElements()
        {
            base._Name = "GeoDataCenterFunLib.ControlsClearElements";
            base._Caption = "���Ԫ��";
            base._Tooltip = "���Ԫ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "���Ԫ��";
            //base._Image = "";
            //base._Category = "";
        }
        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = _AppHk as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }
        public override bool Enabled
        {
            get
            {
                try
                {
                    if (_AppHk.MapControl.Map.LayerCount == 0)
                    {
                        base._Enabled = false;
                        return false;
                    }

                    base._Enabled = true;
                    return true;
                }
                catch
                {
                    base._Enabled = false;
                    return false;
                }
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = _AppHk as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

        public override void OnClick()
        {
            if (_AppHk.MapControl == null) return;
            _AppHk.MapControl.ActiveView.GraphicsContainer.DeleteAllElements();
            _AppHk.MapControl.ActiveView.Refresh();
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppArcGISRef;
            if (_AppHk.MapControl == null)
                return;
        }
    }
}
