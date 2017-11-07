using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GeoDBATool
{
    /// <summary>
    /// ���Ƿ����
    /// </summary>
    public class ControlsUpdateZoomToNew : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGISRef m_Hook;
        private ILayer m_CurLayer = null;//ѡ�е�ͼ��
        public ControlsUpdateZoomToNew()
        {
            base._Name = "GeoDBATool.ControlsUpdateZoomToNew";
            base._Caption = "��λ�����º������";
            base._Tooltip = "��λ�����º������";
            base._Visible = true;
            base._Enabled = true;
            base._Message = "��λ�����º������";

        }

        public override bool Enabled
        {
            get
            {
                if (m_Hook == null) return false;
                bool bExitLay = false;
                if (m_Hook.MapControl == null || m_Hook.UpdateGrid == null) return false;
                if (m_Hook.UpdateGrid.SelectedRows.Count == 0 || m_Hook.UpdateGrid.SelectedRows.Count > 1) return false;
                if (m_Hook.UpdateGrid.SelectedRows.Count == 1)
                {
                    string orgLayerName = m_Hook.UpdateGrid.SelectedRows[0].Cells["������ͼ����"].FormattedValue.ToString().Trim();
                    if (orgLayerName == "") return false;
                    string pState = m_Hook.UpdateGrid.SelectedRows[0].Cells["����״̬"].FormattedValue.ToString().Trim();
                    if (pState == "ɾ��") return false;//��Ϊɾ����Ҫ�أ���ð�ť������
                    for (int i = 0; i < m_Hook.MapControl.Map.LayerCount; i++)
                    {
                        ILayer pLayer = m_Hook.MapControl.Map.get_Layer(i);
                        if (pLayer is IGroupLayer)
                        {
                            IGroupLayer tLayer = pLayer as IGroupLayer;
                            if (tLayer.Name == "NewData")
                            {
                                ICompositeLayer comLayer = tLayer as ICompositeLayer;
                                for (int j = 0; j < comLayer.Count; j++)
                                {
                                    ILayer tt = comLayer.get_Layer(j);
                                    if (tt.Name == orgLayerName)
                                    {
                                        bExitLay = true;
                                        m_CurLayer = tt;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                return bExitLay;
            }
        }

        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

        public override void OnClick()
        {
            try
            {
                IFeatureLayer pFeatLay = m_CurLayer as IFeatureLayer;
                IFeatureClass pFeatCls = pFeatLay.FeatureClass;

                int pAOID = Convert.ToInt32(m_Hook.UpdateGrid.SelectedRows[0].Cells["������OID"].FormattedValue.ToString());
                IQueryFilter pFilter = new QueryFilterClass();
                pFilter.WhereClause = "AOID=" + pAOID;
                IFeatureCursor pFeatureCusor = pFeatCls.Search(pFilter, false);
                if (pFeatureCusor == null) return;
                IFeature pFeature = pFeatureCusor.NextFeature();
                //�ͷ�cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCusor);

                if (pFeature == null) return;
                ISpatialReference pSpatialRef = null;
                IGeoDataset pGeoDt = pFeatCls as IGeoDataset;
                if (pGeoDt != null)
                {
                    pSpatialRef = pGeoDt.SpatialReference;
                }

                m_Hook.MapControl.Map.ClearSelection();
                m_Hook.MapControl.Map.SelectFeature(m_CurLayer, pFeature);
                SysCommon.Gis.ModGisPub.ZoomToFeature(m_Hook.MapControl, pFeature, pSpatialRef);
            }
            catch(Exception e)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(e, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(e, null, DateTime.Now);
                }
                //********************************************************************
                return;
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Plugin.Application.IAppGISRef;
        }
    }
}