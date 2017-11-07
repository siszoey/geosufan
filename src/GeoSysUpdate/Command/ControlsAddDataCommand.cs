using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace GeoSysUpdate
{
    public class ControlsAddDataCommand : Plugin.Interface.CommandRefBase
    {

        private Plugin.Application.IAppArcGISRef _AppHk;

        private ICommand _cmd = null;

        public ControlsAddDataCommand()
        {
            base._Name = "GeoSysUpdate.ControlsAddDataCommand";
            base._Caption = "��������";
            base._Tooltip = "��������";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "��������";
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
            int LayerCountOld = _AppHk.MapControl.Map.LayerCount;
            _cmd.OnClick();
            int LayerCountNew = _AppHk.MapControl.Map.LayerCount;

            //û��Ҫ�����κ�����_cmd������Զ������¼��ص�ͼ�����ͽ�������
            //��λ�ȡ������ӽ�����ͼ���أ�
            //for (int i = LayerCountOld; i < LayerCountNew; i++)
            //{
            //    GeoLayerTreeLib.LayerManager.ModuleMap.DealOrderOfNewLayer(_AppHk.MapControl as IMapControlDefault, _AppHk.MapControl.Map.get_Layer(i));
            //}
            //GeoLayerTreeLib.LayerManager.ModuleMap.LayersComposeEx(_AppHk.MapControl as IMapControlDefault);
            //LayersCompose(_AppHk.MapControl);
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

            _cmd = new ControlsAddDataCommandClass();
            _cmd.OnCreate(_AppHk.MapControl);
        }

        /// <summary>
        /// ��mapcontrol�ϵ�ͼ���������
        /// </summary>
        /// <param name="vAxMapControl"></param>
        private void LayersCompose(IMapControlDefault pMapcontrol)
        {
            IMap pMap = pMapcontrol.Map;
            int[] iLayerIndex = new int[2] { 0, 0 };
            int[] iFeatureLayerIndex = new int[3] { 0, 0, 0 };

            int iCount = pMapcontrol.LayerCount;
            for (int iIndex = 0; iIndex < iCount; iIndex++)
            {
                IFeatureLayer pFeatureLayer = pMap.get_Layer(iIndex) as IFeatureLayer;

                if (pFeatureLayer == null) return;
                switch (pFeatureLayer.FeatureClass.FeatureType)
                {
                    case esriFeatureType.esriFTDimension:
                        pMap.MoveLayer(pFeatureLayer, iLayerIndex[0]);
                        iLayerIndex[0] = iLayerIndex[0] + 1;
                        break;
                    case esriFeatureType.esriFTAnnotation:

                        pMap.MoveLayer(pFeatureLayer, iLayerIndex[0] + iLayerIndex[1]);
                        iLayerIndex[1] = iLayerIndex[1] + 1;
                        break;
                    case esriFeatureType.esriFTSimple:

                        switch (pFeatureLayer.FeatureClass.ShapeType)
                        {
                            case esriGeometryType.esriGeometryPoint:
                                pMap.MoveLayer(pFeatureLayer, iLayerIndex[0] + iLayerIndex[1] + iFeatureLayerIndex[0]);
                                iFeatureLayerIndex[0] = iFeatureLayerIndex[0] + 1;
                                break;
                            case esriGeometryType.esriGeometryLine:
                            case esriGeometryType.esriGeometryPolyline:
                                pMap.MoveLayer(pFeatureLayer, iLayerIndex[0] + iLayerIndex[1] + iFeatureLayerIndex[0] + iFeatureLayerIndex[1]);
                                iFeatureLayerIndex[1] = iFeatureLayerIndex[1] + 1;
                                break;
                            case esriGeometryType.esriGeometryPolygon:
                                pMap.MoveLayer(pFeatureLayer, iLayerIndex[0] + iLayerIndex[1] + iFeatureLayerIndex[0] + iFeatureLayerIndex[1] + iFeatureLayerIndex[2]);
                                iFeatureLayerIndex[2] = iFeatureLayerIndex[2] + 1;
                                break;
                        }
                        break;
                }
            }
        }
    }
}
