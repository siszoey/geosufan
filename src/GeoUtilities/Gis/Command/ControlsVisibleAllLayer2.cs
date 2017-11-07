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
    public class ControlsVisibleAllLayer2 : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppArcGISRef myHook;
        public ControlsVisibleAllLayer2()
        {

            base._Name = "GeoUtilities.ControlsVisibleAllLayer2";
            base._Caption = "��ʾ����ͼ��";
            base._Tooltip = "��ʾ����ͼ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "��ʾ����ͼ��";
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
            bool changed = false;
            for (int i = 0; i < playerCount; i++)
            {
                ILayer player = myHook.MapControl.Map.get_Layer(i);
                if (player is IGroupLayer)
                {
                    //�����GroupLayer
                    IGroupLayer pGroupLayer = player as IGroupLayer;
                    if (!pGroupLayer.Visible )
                    {
                        pGroupLayer.Visible = true;
                        changed = true;
                    }
                    bool bRes=VisibleGroupLayer(pGroupLayer);
                    if (bRes)
                    {
                        changed = true;
                    }
                }
                else if (player is IFeatureLayer || player is IRasterLayer || player is IRasterCatalog)
                {
                    if (!player.Visible)
                    {
                        player.Visible = true;
                        changed = true;
                    }
                }
            }
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(Message);//xisheng 2011.07.08 ������־
            }
            if (changed)    //�Ƿ������ͼ��Ŀɼ��ԣ�δ������ˢ��
            {
                myHook.MapControl.ActiveView.Refresh();
            }
        }
        //ʹͼ����������ͼ��ɼ�
        private bool VisibleGroupLayer(IGroupLayer pGroupLayer)
        {
            bool changed = false;
            ICompositeLayer pComLayer = pGroupLayer as ICompositeLayer;
            if (pComLayer != null)
            {
                for (int i = 0; i < pComLayer.Count; i++)
                {
                    ILayer player = pComLayer.get_Layer(i);
                    if (!player.Visible)
                    {
                        player.Visible = true;
                        changed = true;
                    }
                    if (player is IGroupLayer)
                    {
                        bool bRes=VisibleGroupLayer(player as IGroupLayer);
                        if (bRes)
                        {
                            changed = true;
                        }
                    }
                }
            }
            return changed;
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