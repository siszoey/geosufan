using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

namespace GeoDBATool
{
   public class clsMergeFeatures
    {
       /// <summary>
       /// Ҫ���ں� ���Ƿ����
       /// </summary>
       /// <param name="pFeatureClass">��Ҫ�ںϵ�ͼ��</param>
       /// <param name="newOID">�ںϺ󱣴��Ҫ��OID</param>
       /// <param name="oldOIDLst">��Ҫ�ںϵ�Ҫ��OID</param>
       public void MergeFeatures(IFeatureClass pFeatureClass, int newOID, List<int> oldOIDLst)
       {
           IGeometry tempGeo = null;
           for (int i = 0; i < oldOIDLst.Count; i++)
           {
               int oldOID = oldOIDLst[i];
               IFeature pFeature = pFeatureClass.GetFeature(oldOID);
               if (tempGeo != null)
               {
                   ITopologicalOperator pTop = tempGeo as ITopologicalOperator;
                   tempGeo = pTop.Union(pFeature.Shape);
                   //�ںϺ�ͼ�μ򵥻�
                   pTop = tempGeo as ITopologicalOperator;
                   pTop.Simplify();
               }
               else
               {
                   tempGeo = pFeature.Shape;
               }
           }

           IFeature newFea = pFeatureClass.GetFeature(newOID);
           //���ںϺ��ͼ�θ�ֵ���µ�Ҫ��
           newFea.Shape = tempGeo;
          
           //�������ɵ�Ҫ�ش洢
           newFea.Store();

           //�ںϺ�ɾ�����ںϵ�Ҫ��
           for (int j = 0; j < oldOIDLst.Count; j++)
           {
               if (oldOIDLst[j] != newOID)
               {
                   IFeature delFeature = pFeatureClass.GetFeature(oldOIDLst[j]);
                   delFeature.Delete();
               }
           }
       }
   }
}
