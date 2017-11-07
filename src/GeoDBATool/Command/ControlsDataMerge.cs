using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace GeoDBATool
{
    /// <summary>
    /// ���Ƿ����
    /// </summary>
   public class ControlsDataMerge: Plugin.Interface.CommandRefBase
    {

        private Plugin.Application.IAppGISRef m_Hook;
       private IFeatureLayer m_MergeLayer = null;

       public ControlsDataMerge()
        {
            base._Name = "GeoDBATool.ControlsDataMerge";
            base._Caption = "Ҫ���ں�";
            base._Tooltip = "����ϵ�Ҫ���ں�";
            base._Visible = true;
            base._Enabled = true;
            base._Message = "����ϵ�Ҫ���ں�";

        }

        public override bool Enabled
        {
            get
            {
                if (m_Hook == null) return false;
                if (m_Hook.CurrentThread != null) return false;
                try
                {
                    //�ж��Ƿ������ͼ��
                    if (m_Hook.MapControl.LayerCount == 0)
                    {
                        base._Enabled = false;
                        return false;
                    }
                    //�ж��Ƿ�ѡ���������������ϵ�Ҫ��Ҫ��
                    if (m_Hook.MapControl.Map.SelectionCount < 2)
                    {
                        base._Enabled = false;
                        return false;
                    }

                    //��ȡ��ѡ��Ҫ�ص����ݼ����ж��Ƿ�༭�Ѿ���
                    bool isEditing = GetDatasetEditState();
                    if (!isEditing)
                    {
                        base._Enabled = false;
                        return false;
                    }

                    //��������������������Ϊ����
                    base._Enabled = true;
                    return true;
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

                    base._Enabled = false;
                    return false;
                }
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
            if (m_MergeLayer != null)
            {
                List<int> OIDLst = new List<int>();
                IFeatureSelection pFeatSel = m_MergeLayer as IFeatureSelection;
                ISelectionSet pSelectionSet = pFeatSel.SelectionSet;

                //���ѡ���Ҫ�ض���һ��������Կ�ʼ�ں�
                if (pSelectionSet.Count > 1)
                {
                    int pOID=-1;
                    IEnumIDs pEnumIDs=pSelectionSet.IDs;
                    pEnumIDs.Reset();
                    pOID=pEnumIDs.Next();
                    while (pOID != -1)
                    {
                        OIDLst.Add(pOID);
                        pOID = pEnumIDs.Next();
                    }
                    frmDataMerge pfrmDataMerge = new frmDataMerge(m_MergeLayer.FeatureClass, OIDLst, m_Hook);
                    pfrmDataMerge.ShowDialog();
                }
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            m_Hook = hook as Plugin.Application.IAppGISRef;
        }
       //�ж�Ҫ���Ƿ���ͬһͼ���Լ�ͼ��ı༭״̬
        private bool GetDatasetEditState()
       {

           int pSameLyr = 0;  //��¼Ҫ���Ƿ�Ϊͬ��
           ILayer pLayer = null;
           //�ж�ѡ���Ҫ���Ƿ���ͬһ��ͼ��
           for (int i = 0; i <m_Hook.MapControl.Map.LayerCount; i++)
           {
               IFeatureLayer pFeatLyr = null;
               IFeatureSelection pFeatSel = null;
               pLayer = m_Hook.MapControl.Map.get_Layer(i);
               if (pLayer is IGroupLayer)
               {
                   if (pLayer.Name == "ʾ��ͼ")
                   {
                       continue;
                   }
                   ICompositeLayer pComLayer = pLayer as ICompositeLayer;
                   for (int j = 0; j < pComLayer.Count; j++)
                   {
                       ILayer mLayer = pComLayer.get_Layer(j);
                       pFeatLyr = mLayer as IFeatureLayer;
                       if (pFeatLyr != null)
                       {
                           pFeatSel = pFeatLyr as IFeatureSelection;
                           if (pFeatSel.SelectionSet.Count > 0)
                           {
                               pSameLyr = pSameLyr + 1;
                               m_MergeLayer = pFeatLyr;//��ֻ��һ��ͼ�㱻ѡ��ʱ��pFeatLyr����Ҫ�����ںϵ�Ŀ��ͼ��
                           }
                       }
                   }
               }

               pFeatLyr = pLayer as IFeatureLayer;
               if (pFeatLyr != null)
               {
                   pFeatSel = pFeatLyr as IFeatureSelection;
                   if (pFeatSel.SelectionSet.Count > 0)
                   {
                       pSameLyr = pSameLyr + 1;
                       m_MergeLayer = pFeatLyr;//��ֻ��һ��ͼ�㱻ѡ��ʱ��pFeatLyr����Ҫ�����ںϵ�Ŀ��ͼ��
                   }
               }
           }
           //���ѡ���Ҫ�����ڵĲ�������1����Ҫ�ز���ͬһ����
           if (pSameLyr != 1)
           {
               return false;
           }
           return true;
       }
    }
}

