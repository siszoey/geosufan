using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace GeoUtilities
{
    public class ControlsMapZoomInFixedCommand:Plugin.Interface.CommandRefBase
    {

        private Plugin.Application.IAppArcGISRef _AppHk;

        private ICommand _cmd = null;

        public ControlsMapZoomInFixedCommand()
        {
            base._Name = "GeoUtilities.ControlsMapZoomInFixedCommand";
            base._Caption = "���ķŴ�";
            base._Tooltip = "���ķŴ�";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "���ķŴ�";
            //base._Image = "";
            //base._Category = "";
        }
        public override bool Enabled
        {
            get
            {
                if (_AppHk == null) return false;
               
                if (_AppHk.CurrentControl is ISceneControl) return false;  //Ϊ��ֻ��Ч��2ά�ؼ�
                return true;
            }
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
            if ( _cmd == null || _AppHk == null) return;
            if (_AppHk.MapControl == null) return;
            _cmd.OnClick();
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog("ʹ�ö�ά����:" + Message);//xisheng 2011.07.08 ������־
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppArcGISRef;
            if (_AppHk.MapControl == null) return;

            _cmd = new ControlsMapZoomInFixedCommandClass();
            _cmd.OnCreate(_AppHk.MapControl);
        }
    }
}
