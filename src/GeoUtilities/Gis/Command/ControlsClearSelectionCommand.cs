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
    public class ControlsClearSelectionCommand : Plugin.Interface.CommandRefBase
    {

        private Plugin.Application.IAppArcGISRef _AppHk;

        private ICommand _cmd = null;

        public ControlsClearSelectionCommand()
        {
            base._Name = "GeoUtilities.ControlsClearSelectionCommand";
            base._Caption = "���ѡ��";
            base._Tooltip = "���ѡ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "���ѡ��";
            //base._Image = "";
            //base._Category = "";
        }

        public override bool Enabled
        {
            get
            {
                try
                {
                    if (_AppHk.CurrentControl is ISceneControl) return false;  //Ϊ��ֻ��Ч��2ά�ؼ�
                    if (_AppHk.MapControl.Map.SelectionCount == 0)
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
            if (_cmd == null || _AppHk == null) return;
            if (_AppHk.MapControl == null) return;
            _cmd.OnClick();
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(base._Tooltip);//xisheng 2011.07.08 ������־
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppArcGISRef;
            if (_AppHk.MapControl == null) return;

            _cmd = new ControlsClearSelectionCommandClass();
            _cmd.OnCreate(_AppHk.MapControl);
        }
    }
}
