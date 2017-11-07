using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;

namespace GeoUtilities
{
    /// <summary>
    /// �Ҽ��˵��۵�ͼ��   chenyafei
    /// </summary>
    public class ControlsCollapse:Plugin.Interface.CommandRefBase
    {
     private Plugin.Application.IAppArcGISRef myHook;
        public ControlsCollapse()
        {

            base._Name = "GeoUtilities.ControlsCollapse";
            base._Caption = "�۵�ͼ��";
            base._Tooltip = "�۵�ͼ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�۵�����ͼ��";
        }
        public override bool Enabled
        {
            get
            {
                if (myHook.MapControl == null || myHook.TOCControl == null) return false;
                if (myHook.MapControl.LayerCount == 0) return false;
                ILayer nLayer = myHook.MapControl.CustomProperty as ILayer;
                if(nLayer is IFeatureLayer)
                {
                    IFeatureLayer tLayer = nLayer as IFeatureLayer;
                    if (tLayer.FeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public override void OnClick()
        {
            int playerCount = myHook.MapControl.Map.LayerCount;//���MapControl��ͼ�������
            IFeatureLayer pFeatureLayer = new FeatureLayerClass();

            ILayer mLayer = myHook.MapControl.CustomProperty  as ILayer ;
            if (mLayer is IGroupLayer)
            {
                IGroupLayer pGLayer = mLayer as IGroupLayer;
                CollapseGroupLayer(pGLayer);
            }
            else if (mLayer is IFeatureLayer)
            {
                pFeatureLayer = mLayer as IFeatureLayer;
                CollapseLayer(pFeatureLayer);
            }
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(base._Tooltip);//xisheng 2011.07.08 ������־
            }
            myHook.TOCControl.Update();
        }
        private void CollapseGroupLayer(IGroupLayer pGroupLauer)
        {
            ICompositeLayer pComLayer = pGroupLauer as ICompositeLayer;
            for (int j = 0; j < pComLayer.Count; j++)
            {
                ILayer mmLayer = pComLayer.get_Layer(j);
                IFeatureLayer pFeatureLayer = mmLayer as IFeatureLayer;
                CollapseLayer(pFeatureLayer);
            }
        }
        private void CollapseLayer(IFeatureLayer pFeatureLayer)
        {
            ILegendInfo pLegendInfo = null;
            ILegendGroup pLegendGroup = new LegendGroupClass();
            if (pFeatureLayer.FeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
            {
                #region ���ͼ����ע�ǲ�
                //IAnnotationLayer pAnnoLayer = pFeatureLayer as IAnnotationLayer;
                //IGeoFeatureLayer pGeoFeaLayer = pFeatureLayer as IGeoFeatureLayer;
                //IFeatureRenderer pRender = pGeoFeaLayer.Renderer;
                //ISymbol pSym = (pRender as IUniqueValueRenderer).DefaultSymbol;
                //ISimpleTextSymbol pTextSym = pSym as ISimpleTextSymbol;
                //ICompositeLayer pComLayer = pAnnoLayer as ICompositeLayer;
                //IGroupLayer pgLayer = new GroupLayerClass();
                //pgLayer.Name = pFeatureLayer.Name;
                //for (int j = 0; j < pComLayer.Count; j++)
                //{
                //    ILayer ppLayer = pComLayer.get_Layer(j);
                //    pgLayer.Add(ppLayer);
                //}
                //myHook.MapControl.Map.DeleteLayer(pFeatureLayer as ILayer);
                //myHook.MapControl.Map.AddLayer(pgLayer);
                //myHook.MapControl.ActiveView.Refresh();
                //pgLayer.Expanded = false;
                //myHook.TOCControl.Update();
                #endregion
            }
            else
            {
                //���ͼ��ʱ��ͨ��ͼ��
                pLegendInfo = pFeatureLayer as ILegendInfo;
                if (pLegendInfo == null) return;
                pLegendGroup = pLegendInfo.get_LegendGroup(0);
                pLegendGroup.Visible = false;
            }
 
        }
        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = myHook as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = myHook as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }
        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            myHook = hook as Plugin.Application.IAppArcGISRef;
            if (myHook.MapControl == null) return;
        }
    }
}
