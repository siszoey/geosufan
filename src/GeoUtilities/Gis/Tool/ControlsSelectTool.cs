using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace GeoUtilities
{
    public class ControlsSelectTool : Plugin.Interface.ToolRefBase
    {
        private Plugin.Application.IAppArcGISRef _AppHk;

        private ITool _tool = null;
        private ICommand _cmd = null;

        public ControlsSelectTool()
        {
            base._Name = "GeoUtilities.ControlsSelectTool";
            base._Caption = "ѡ��Ԫ��";
            base._Tooltip = "ѡ��Ԫ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Deactivate = false;
            base._Message = "�������������ѡ��Ԫ��";
            //base._Cursor = (int)esriControlsMousePointer.esriPointerDefault;
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
                    if (_AppHk.MapControl.LayerCount == 0)
                    {
                        base._Enabled = false;
                        return false;
                    }

                    base._Enabled = false;
                    return false;
                }
                catch
                {
                    base._Enabled = false;
                    return false;
                }
            }
        }

        public override bool Checked
        {
            get
            {
                if (_AppHk == null) return false;
                if (_AppHk.CurrentTool != this.Name) return false;
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
            if (_tool == null || _cmd == null || _AppHk == null) return;
            if (_AppHk.MapControl == null) return;
            _AppHk.MapControl.CurrentTool = _tool;
            _AppHk.CurrentTool = this.Name;
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog("ʹ�ö�ά����:" + Caption);//xisheng 2011.07.08 ������־
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppArcGISRef;
            if (_AppHk.MapControl == null) return;

            _tool = new ControlsSelectToolClass();
            _cmd = _tool as ICommand;
            _cmd.OnCreate(_AppHk.MapControl);
        }
    }
}
