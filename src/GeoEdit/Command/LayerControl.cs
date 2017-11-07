using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;

namespace GeoEdit
{
    public class LayerControl : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGISRef myHook;
        private DevComponents.DotNetBar.ComboBoxItem _ComboBoxLayer;

        private IMap _map;               //һ����map��Ϊȫ�ֱ�������IActiveViewEvents_Event����¼��޷���Ӧ

        public LayerControl()
        {
            base._Name = "GeoEdit.LayerControl";
            base._Caption = "ͼ�����";
            base._Tooltip = "ͼ�����";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "ͼ�����";
        }

        public override bool Enabled
        {
            get
            {
                if (myHook == null) return false;
                if (_ComboBoxLayer == null)
                {
                    //��ȡͼ�����������
                    foreach (KeyValuePair<DevComponents.DotNetBar.BaseItem, string> kvCmd in Plugin.ModuleCommon.DicBaseItems)
                    {
                        if (kvCmd.Value == "GeoEdit.LayerControl")
                        {
                            DevComponents.DotNetBar.ComboBoxItem aComboBoxItem = kvCmd.Key as DevComponents.DotNetBar.ComboBoxItem;
                            if (aComboBoxItem != null)
                            {
                                aComboBoxItem.ComboWidth = 150;
                                _ComboBoxLayer = aComboBoxItem;
                                _ComboBoxLayer.SelectedIndexChanged += new EventHandler(ComboBoxLayer_SelectedIndexChanged);
                                break;
                            }
                        }
                    }
                }
                       
                if (myHook.MapControl == null) return false;
                if (MoData.v_CurWorkspaceEdit == null) return false;
                
                return true;
            }
        }

        public override void OnClick()
        {

        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            myHook = hook as Plugin.Application.IAppGISRef;

            _map = myHook.ArcGisMapControl.ActiveView.FocusMap;
            ((IActiveViewEvents_Event)_map).ItemAdded += new IActiveViewEvents_ItemAddedEventHandler(LayerControl_ItemAdded);
            ((IActiveViewEvents_Event)_map).ItemDeleted += new IActiveViewEvents_ItemDeletedEventHandler(LayerControl_ItemDeleted); 
        }

        private void ComboBoxLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            //List��ͼ��ѭ����ComboBoxLayer��ͼ��ѭ��һ��(�ڼ���ʱ����)
            List<ILayer> plistLay = _ComboBoxLayer.Tag as List<ILayer>;
            if (plistLay == null) return;
            MoData.v_CurLayer = plistLay[_ComboBoxLayer.SelectedIndex];
        }

        private void LayerControl_ItemAdded(object Item)
        {
            if (_ComboBoxLayer == null) return;
            GetLayers();
        }

        private void LayerControl_ItemDeleted(object Item)
        {
            ILayer pLay = Item as ILayer;
            IGroupLayer pGroupLayer = pLay as IGroupLayer;
            if (pGroupLayer != null)
            {
                EndEdit(pGroupLayer);
                return;
            }

            IFeatureLayer pFeatureLayer = pLay as IFeatureLayer;
            if (pFeatureLayer == null) return;
            EndEdit(pFeatureLayer);

            if (_ComboBoxLayer == null) return;
            GetLayers();
        }
        private void EndEdit(IGroupLayer pGroupLayer)
        {
            ICompositeLayer pCompositeLayer = pGroupLayer as ICompositeLayer;
            for (int i = 0; i < pCompositeLayer.Count; i++)
            {
                IGroupLayer pIGroupLayerTemp = pCompositeLayer.get_Layer(i) as IGroupLayer;
                if (pIGroupLayerTemp != null)
                {
                    EndEdit(pIGroupLayerTemp);
                    continue;
                }
                IFeatureLayer pFeatureLayer = pCompositeLayer.get_Layer(i) as IFeatureLayer;
                if (pFeatureLayer == null) continue;
                EndEdit(pFeatureLayer);
            }
        }
        private void EndEdit(IFeatureLayer pFeatureLayer)
        {
            IFeatureClass pFeatClass = pFeatureLayer.FeatureClass;
            IDataset pDataset = pFeatClass as IDataset;
            IWorkspaceEdit pWSEdit = pDataset.Workspace as IWorkspaceEdit;
            if (pWSEdit.IsBeingEdited())
            {
                //�жϸù����ռ�����map���Ƿ���ͼ��
                bool bHasLay = false;

                if (myHook.MapControl.Map.LayerCount == 0)
                {
                    bHasLay = false;
                }
                else
                {
                    UID pUID = new UIDClass();
                    pUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";   //UID for IFeatureLayer
                    IEnumLayer pEnumLayer = myHook.MapControl.Map.get_Layers(pUID, true);
                    pEnumLayer.Reset();
                    ILayer pLayer = pEnumLayer.Next();
                    while (pLayer != null)
                    {
                        IFeatureLayer pTempFeatureLayer = pLayer as IFeatureLayer;
                        IDataset pTempDataset = pTempFeatureLayer.FeatureClass as IDataset;
                        if (pTempDataset.Workspace == pDataset.Workspace)
                        {
                            bHasLay = true;
                            break;
                        }

                        pLayer = pEnumLayer.Next();
                    }
                }

                //�����̴߳���ʱ����������Ӧ�ų�
                if (bHasLay == false && myHook.CurrentThread == null)
                {
                    bool bHasEdits = false;
                    bool bSave = false;
                    pWSEdit.HasEdits(ref bHasEdits);
                    if (bHasEdits == true)
                    {
                        bSave = SysCommon.Error.ErrorHandle.ShowFrmInformation("��", "��", "ͼ���ѽ��й��༭���Ƿ���Ҫ���棿");
                    }

                    pWSEdit.StopEditing(bSave);
                    MoData.v_CurWorkspaceEdit = null;
                    //������־��¼����޸�
                    if (MoData.v_LogTable != null)
                    {
                        MoData.v_LogTable.EndTransaction(bSave);
                        MoData.v_LogTable.CloseDbConnection();
                        MoData.v_LogTable = null;
                    }

                    myHook.CurrentTool = "";
                    myHook.MapControl.CurrentTool = null;
                    myHook.MapControl.Map.ClearSelection();
                    myHook.MapControl.ActiveView.Refresh();
                }
            }
        }

        public void GetLayers()
        {
            _ComboBoxLayer.Items.Clear();
            List<ILayer> plistLay = ModPublic.LoadAllEditLyr(myHook.MapControl.Map);
            if (plistLay.Count != 0)
            {
                _ComboBoxLayer.Tag = plistLay;
                for (int i = 0; i < plistLay.Count; i++)
                {
                    _ComboBoxLayer.Items.Add(plistLay[i].Name);
                }
                _ComboBoxLayer.SelectedIndex = 0;
            }
        }
    }
}
