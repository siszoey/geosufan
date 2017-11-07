using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace GeoDataEdit
{
    public class DeleteFeature : Plugin.Interface.ToolRefBase
    {

        private Plugin.Application.IAppArcGISRef _AppHk;


        public DeleteFeature()
        {
            base._Name = "GeoDataEdit.DeletePolygon";
            base._Caption = "ɾ��ͼԪ";
            base._Tooltip = "ɾ��ͼԪ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "ɾ��ͼԪ";
            //base._Image = "";
            //base._Category = "";
        }





        public override bool Checked
        {
            get
            {
                if (_AppHk.CurrentTool != this.Name) return false;
                return true;
            }
        }

        public override bool Enabled
        {
            get
            {
                try
                {
                    if (_AppHk.MapControl.Map.LayerCount == 0)
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
            if ( _AppHk == null) return;
            if (_AppHk.MapControl == null) return;
            IFeatureLayer tmpFeatureLayer = getEditLayer.isExistLayer(_AppHk.MapControl.Map) as IFeatureLayer;
            if (tmpFeatureLayer == null)
            {
                MessageBox.Show("�����ñ༭ͼ�㣡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            IWorkspaceEdit iWE = (tmpFeatureLayer.FeatureClass as IDataset).Workspace as IWorkspaceEdit;
            if (!iWE.IsBeingEdited())
                iWE.StartEditing(false);
            ISelectionSet iSS = (tmpFeatureLayer as IFeatureSelection).SelectionSet;
            ICursor iCursor;
            iSS.Search(null, false, out iCursor);
            IFeatureCursor iFC = iCursor as IFeatureCursor;
            IFeature tmpFeature = iFC.NextFeature();
            if (tmpFeature == null)
            {
                MessageBox.Show("��ѡ��༭ͼ����Ҫɾ����ͼԪ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            } 
            
            if (MessageBox.Show("�Ƿ�ɾ��ѡ���ͼԪ��", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                return;
            iWE.StartEditOperation();
            while (tmpFeature != null)
            {
                tmpFeature.Delete();
                tmpFeature = iFC.NextFeature();

            }
            iWE.StopEditOperation();
            Plugin.LogTable.Writelog(Caption);
            //iWE.StopEditing(true);
            iWE = null;
            _AppHk.MapControl.ActiveView.Refresh();
        

            
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppArcGISRef;
            if (_AppHk.MapControl == null) return;

            Plugin.Application.IAppFormRef pAppForm = hook as Plugin.Application.IAppFormRef;
        


        }

        private IFeatureLayer layerCurSeleted()
        {
            IMap pMap = _AppHk.MapControl.Map;
            int iLayerCount = pMap.LayerCount;
            IFeatureLayer pFeatureLayer;
            IFeatureLayer pCurSeLayer = null;

            int countSelectable = 0;
            for (int n = 0; n < iLayerCount; n++)
            {
                if (countSelectable >= 2) break;
                ILayer layer = pMap.get_Layer(n);
                if (layer is IGroupLayer)
                {
                    ICompositeLayer Comlayer = layer as ICompositeLayer;//��һ��������Ĳ����ת����һ����ϲ㣬ʹ�����Ա���
                    for (int c = 0; c < Comlayer.Count; c++)
                    {
                        pFeatureLayer = Comlayer.get_Layer(c) as IFeatureLayer;
                        if (pFeatureLayer != null && pFeatureLayer.Selectable)
                        {
                            countSelectable++;
                            pCurSeLayer = pFeatureLayer;

                        }

                    }
                }
            }//for
            if (countSelectable != 1)
                return null;
            else
                return pCurSeLayer;




        }

    }
}
