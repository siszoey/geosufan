using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Controls;

namespace GeoUtilities
{
    /// <summary>
    /// �Ҽ��˵���չͼ�� chenyafei
    /// </summary>
    public class ControlsExpandAllLayer1 : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppFormRef myHook;
        public ControlsExpandAllLayer1()
        {

            base._Name = "GeoUtilities.ControlsExpandAllLayer1";
            base._Caption = "��չ����ͼ��";
            base._Tooltip = "��չ����ͼ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "��չ����ͼ��";
        }
        public override bool Enabled
        {
            get
            {
                if (myHook.MapControl == null || myHook.TOCControl == null) return false;
                if (myHook.MapControl.LayerCount == 0) return false;
                return true;
            }
        }
        public override void OnClick()
        {
            int playerCount = myHook.MapControl.Map.LayerCount;//���MapControl��ͼ�������
            IFeatureLayer pFeatureLayer = new FeatureLayerClass();
            for (int i = 0; i < playerCount; i++)
            {
                ILayer player = myHook.MapControl.Map.get_Layer(i);
                if (player is IGroupLayer)
                {
                    //���ʱGroupLayer������������Expended��������չͼ��
                    IGroupLayer pGroupLayer = player as IGroupLayer;
                    ExpandGroupLayer(pGroupLayer);
                    pGroupLayer.Expanded = true;
                }

                else if (player is IFeatureLayer)
                {
                    //�������ͨ��ͼ�㣬����pLegendGroup.Visible����չͼ��
                    pFeatureLayer = player as IFeatureLayer;
                    ExpandLayer(pFeatureLayer);
                }
            }
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(Message);//xisheng 2011.07.08 ������־
            }
            myHook.TOCControl.Update();
        }
        private void ExpandGroupLayer(IGroupLayer pGroupLayer)
        {
            ICompositeLayer pComLayer = pGroupLayer as ICompositeLayer;
            for (int i = 0; i < pComLayer.Count; i++)
            {
                IFeatureLayer pFlayer = pComLayer.get_Layer(i) as IFeatureLayer;
                ExpandLayer(pFlayer);
            }
        }
        private void ExpandLayer(IFeatureLayer pFeatureLayer)
        {
            ILegendInfo pLegendInfo = null;
            ILegendGroup pLegendGroup = new LegendGroupClass();
            pLegendInfo = pFeatureLayer as ILegendInfo;
            if (pLegendInfo == null) return;
            pLegendGroup = pLegendInfo.get_LegendGroup(0);
            pLegendGroup.Visible = true;
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
            myHook = hook as Plugin.Application.IAppFormRef;
            if (myHook.MapControl == null) return;
        }
    }
}