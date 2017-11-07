using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Carto;

namespace GeoUtilities
{
    public class ControlsZoomToLayer : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppArcGISRef _AppHk;
        public ControlsZoomToLayer()
        {
            base._Name = "GeoUtilities.ControlsZoomToLayer";
            base._Caption = "���ŵ�ͼ��";
            base._Tooltip = "���ŵ�ͼ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "���ŵ�ͼ��";
        }

        public override bool Enabled
        {
            get
            {
                if (_AppHk.MapControl == null || _AppHk.TOCControl == null) return false;
                if (_AppHk.MapControl.LayerCount == 0) return false;
                ILayer mLayer = _AppHk.MapControl.CustomProperty as ILayer;
                if (mLayer == null) return false;
                if (mLayer is IDynamicLayer) return false;
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
            try
            {
                ILayer pLayer = null;
                pLayer = (ILayer)_AppHk.MapControl.CustomProperty;
                if (pLayer == null) return;
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog(Caption);//xisheng 2011.07.08 ������־
                }
                IActiveView pActiveView = _AppHk.MapControl.Map as IActiveView;
                pActiveView.Extent = pLayer.AreaOfInterest;

                //�������ŵ�ͼ�� ������������ xisheng 20111117********************************************
                //if (pLayer.MinimumScale > 0)
                //    _AppHk.MapControl.Map.MapScale = pLayer.MinimumScale;//yjl20111014 add zoomtovisible 
                //�������ŵ�ͼ�� ������������ xisheng 20111117*****************************************end

                pActiveView.Refresh();
            }
            catch (Exception eError)
            {
                if (SysCommon.Log.Module.SysLog == null) SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppArcGISRef;
        }
    }
}
