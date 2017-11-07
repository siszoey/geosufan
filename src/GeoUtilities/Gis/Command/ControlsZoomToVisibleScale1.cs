using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Carto;

namespace GeoUtilities
{
    public class ControlsZoomToVisibleScale1 : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppArcGISRef _AppHk;
        public ControlsZoomToVisibleScale1()
        {
            base._Name = "GeoUtilities.ControlsZoomToVisibleScale1";
            base._Caption = "���ŵ�ͼ��";
            base._Tooltip = "���ŵ�ͼ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "���ŵ�ͼ��";
        }
        public override bool Visible
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = _AppHk as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    if (pAppFormRef.LayerAdvTree != null)
                    {
                        DevComponents.AdvTree.AdvTree pTree = pAppFormRef.LayerAdvTree as DevComponents.AdvTree.AdvTree;
                        if (pTree.SelectedNode != null)
                        {
                            DevComponents.AdvTree.Node pNode = pTree.SelectedNode;
                            if (pNode != null)
                            {
                                if (pNode.Tag != null)
                                {
                                    string strtag = pNode.Tag.ToString();
                                    if (strtag.ToLower().Contains("layer"))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }
        }
        public override bool Enabled
        {
            get
            {
                if (_AppHk.MapControl == null || _AppHk.TOCControl == null) return false;
                if (_AppHk.MapControl.LayerCount == 0) return false;
                ILayer mLayer = _AppHk.MapControl.CustomProperty as ILayer;
                if (mLayer == null) return false;
                bool bVisible = false;
                try
                {
                    bVisible = SysCommon.ModuleMap.GetScaleVisibleOfLayer(_AppHk.MapControl.Map.MapScale, mLayer);
                }
                catch
                { }
                if (bVisible)
                {
                    return false;
                }
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
                double OldScale = _AppHk.MapControl.Map.MapScale;
                pActiveView.Extent = pLayer.AreaOfInterest;
                //added by chulili 20111117 ����ǰ��ͼ�����߾����ĸ�����������������ŵ��ĸ�������
                double dMaxScale = pLayer.MaximumScale;
                double dMinScale = pLayer.MinimumScale;
                if (dMaxScale > 0 && dMinScale > 0) //�������ߺ���С�����߶�������
                {
                    if (Math.Abs(dMaxScale - OldScale) > Math.Abs(dMinScale - OldScale))    //�����ĸ�����������������ŵ��ĸ�
                    {
                        _AppHk.MapControl.Map.MapScale = dMinScale;
                        _AppHk.MapControl.Map.MapScale = dMinScale; //added by chulili 20111130 ����һ����ƫ��������鼸��û��ƫ��
                    }
                    else
                    {
                        _AppHk.MapControl.Map.MapScale = dMaxScale;
                        _AppHk.MapControl.Map.MapScale = dMaxScale;
                    }
                }
                else
                {
                    if (dMaxScale <= 0) //δ������������
                    {
                        if (dMinScale > 0)
                        {
                            _AppHk.MapControl.Map.MapScale = dMinScale;
                            _AppHk.MapControl.Map.MapScale = dMinScale;
                        }
                    }
                    else if (dMinScale <= 0)    //δ������С������
                    {
                        _AppHk.MapControl.Map.MapScale = dMaxScale;
                        _AppHk.MapControl.Map.MapScale = dMaxScale;
                    }
                }
                //end added by chulili
                //if (pLayer.MinimumScale > 0)
                //    _AppHk.MapControl.Map.MapScale = pLayer.MinimumScale;//yjl20111014 add zoomtovisible 
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
