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
    public class ControlsNewRectangleTool : Plugin.Interface.ToolRefBase
    {

        private Plugin.Application.IAppArcGISRef _AppHk;

        private ITool _tool = null;
        private ICommand _cmd = null;

        public ControlsNewRectangleTool()
        {
            base._Name = "GeoUtilities.ControlsNewRectangleTool";
            base._Caption = "����";
            base._Tooltip = "����";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "����";
            //base._Image = "";
            //base._Category = "";
        }

        public override bool Enabled
        {
            get
            {
                if (_AppHk.PageLayoutControl == null || _AppHk.TOCControl == null) return false;
                if (!(_AppHk.CurrentControl is IPageLayoutControl2)) return false;
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
            if (_cmd == null || _AppHk == null) return;
            if (_AppHk.PageLayoutControl == null) return;
            _cmd.OnClick();
            _AppHk.PageLayoutControl.CurrentTool = _tool;
            _AppHk.CurrentTool = this.Name;
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppArcGISRef;
            if (_AppHk.PageLayoutControl == null) return;

            string progID = base._Name;
            int index = _AppHk.ToolbarControls.Find(progID);
            if (index != -1)
            {
                IToolbarItem toolItem = _AppHk.ToolbarControls.GetItem(index);
                _cmd = toolItem.Command;
                _tool = (ITool)_cmd;                
            }
        }
    }
}
