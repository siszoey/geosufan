using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;
using System.Data.Common;
using ESRI.ArcGIS.Geodatabase ;
using ESRI.ArcGIS.Geometry;
using System.Windows.Forms;

using System.Collections;
using ESRI.ArcGIS.Carto;

/// <summary>
/// ���Ƿ����
/// </summary>
namespace GeoDataChecker
{
   //���˴�����
    public static class TopologyCheckClass
    {

        //public static string DataCheckPath = @"C:\Documents and Settings\chenyafei\����\���ݼ���������.mdb";
        //public static string DataCheckPath = @"C:\Documents and Settings\chenyafei\����\�����˼���������.mdb";
        public static string GeoDataCheckParaPath = Application.StartupPath + @"\..\Res\checker\GIS���ݼ�����ñ�.mdb";
        public static string GeoLogPath = Application.StartupPath + @"\..\Log\";
      
        /// <summary>
        /// Ϊ���ݼ���������
        /// </summary>
        /// <param name="pFeatureDataset"></param>
        /// <param name="outError"></param>
        /// <returns></returns>
        public static ITopology CreateTopo(IFeatureDataset pFeatureDataset, out Exception outError)
        {
            outError = null;

            ITopology pTopo = null;
            try
            {
                ITopologyContainer2 pTopoContainer = pFeatureDataset as ITopologyContainer2;

                try
                {
                    ITopology tempTopo = pTopoContainer.get_TopologyByName(pFeatureDataset.Name);
                    if (tempTopo != null)
                    {
                        RemoveTopo(pFeatureDataset, out outError);
                    }
                }
                catch
                { }
                pTopo = pTopoContainer.CreateTopology(pFeatureDataset.Name,
                                                          pTopoContainer.DefaultClusterTolerance, -1, "");
            }
            catch (Exception ex)
            {
                outError = ex;
            }
            return pTopo;
        }

        /// <summary>
        /// �����������Ҫ���� 
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="pFeaDataset"></param>
        /// <param name="outError"></param>
        public static void AddFeaClasstoTopo(ITopology pTopo,IFeatureDataset pFeaDataset,out Exception outError)
         {
             outError = null;
             try
             {
                 bool b = false;
                 ITopologyRuleContainer pTopoRulrContainer = pTopo as ITopologyRuleContainer;
                 List<IDataset> LstDataSet = GetAllFeaCls(pFeaDataset);
                 foreach (IDataset pDt in LstDataSet)
                 {
                     IFeatureClass pFeaCls = pDt as IFeatureClass;
                     if (pFeaCls == null) continue;
                     if (pFeaCls.FeatureType != esriFeatureType.esriFTSimple) continue;
                     pTopo.AddClass(pFeaCls as IClass, 5, 1, 1, false);
                     b = true;
                 }
                 if (b == false)
                 {
                     outError = new Exception("Ҫ���м���Ҫ���಻���ڣ�");
                     return;
                 }
             }
             catch (Exception ex)
             {
                 outError = ex;
             }
         }

        /// <summary>
        /// ����������ӹ���
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="pFeaClsNameList"></param>
        /// <param name="pTopoRuleType"></param>
        /// <param name="outError"></param>
        public static void AddFeaRuletoTopo(ITopology pTopo, List<string> pFeaClsNameList,esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError = null;

            try
            {
                ITopologyRuleContainer pRuleContainer = pTopo as ITopologyRuleContainer;
                IFeatureClassContainer pFeaClsContainer = pTopo as IFeatureClassContainer;
                IEnumFeatureClass pEnumFeaCls = pFeaClsContainer.Classes;
                pEnumFeaCls.Reset();
                IFeatureClass pFeaCls = pEnumFeaCls.Next();
                while (pFeaCls != null)
                {
                    string pFeaName =(pFeaCls as IDataset).Name.Trim();
                    if (pFeaName.Contains("."))
                    {
                        pFeaName = pFeaName.Substring(pFeaName.IndexOf('.') + 1);
                    }
                    if (pFeaClsNameList.Contains(pFeaName))
                    {
                   
                        ITopologyRule pTopoRule = new TopologyRuleClass();
                        pTopoRule.TopologyRuleType = pTopoRuleType;
                        pTopoRule.Name = pTopoRule.ToString();
                        pTopoRule.OriginClassID = pFeaCls.FeatureClassID;
                        pTopoRule.AllOriginSubtypes = true;
                        pRuleContainer.AddRule(pTopoRule);
                    }
                    pFeaCls = pEnumFeaCls.Next();
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// ��Ҫ������ӵ�������
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="pGeometryType"></param>
        /// <param name="outError"></param>
        private static void AddClasstoTopology(ITopology pTopo,IFeatureDataset pFeatureDataset,esriGeometryType pGeometryType,out Exception outError)
        {
            outError = null;
            try
            {
                bool b = false;
                ITopologyRuleContainer pTopoRulrContainer = pTopo as ITopologyRuleContainer;
                List<IDataset> LstDataSet = GetAllFeaCls(pFeatureDataset);
                foreach (IDataset pDt in LstDataSet)
                {
                    IFeatureClass pFeaCls = pDt as IFeatureClass;
                    if (pFeaCls == null) continue;
                    if (pFeaCls.FeatureType != esriFeatureType.esriFTSimple) continue;
                    if (pGeometryType != esriGeometryType.esriGeometryNull)
                    {
                        if (pFeaCls.ShapeType != pGeometryType) continue;
                    }
                    pTopo.AddClass(pFeaCls as IClass, 5, 1, 1, false);
                    b = true;
                }
                if (b == false)
                {
                    outError = new Exception("Ҫ���м���Ҫ���಻���ڣ�");
                    return;
                }
            }
            catch(Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// ΪҪ���ഴ�����˹�����ӵ�������
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="pGeometryTyoe"></param>
        /// <param name="pTopoRuleType"></param>
        /// <param name="outError"></param>
        public static void AddRuletoTopology(ITopology pTopo, esriGeometryType pGeometryType, esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError=null;

            try
            {
                ITopologyRuleContainer pRuleContainer = pTopo as ITopologyRuleContainer;
                IFeatureClassContainer pFeaClsContainer = pTopo as IFeatureClassContainer;
                IEnumFeatureClass pEnumFeaCls = pFeaClsContainer.Classes;
                pEnumFeaCls.Reset();
                IFeatureClass pFeaCls = pEnumFeaCls.Next();
                while (pFeaCls != null)
                {
                    if (pFeaCls.ShapeType == pGeometryType)
                    {
                        ITopologyRule pTopoRule = new TopologyRuleClass();
                        pTopoRule.TopologyRuleType = pTopoRuleType;
                        pTopoRule.Name = (pFeaCls as IDataset).Name;
                        pTopoRule.OriginClassID = pFeaCls.FeatureClassID;
                        pTopoRule.AllOriginSubtypes = true;
                        pRuleContainer.AddRule(pTopoRule);
                    }
                    pFeaCls = pEnumFeaCls.Next();
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// ��Ҫ������ӵ������в��������˹���
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="pGeometryType"></param>
        /// <param name="pTopoRuleType"></param>
        /// <param name="outError"></param>
        public static void AddRuleandClasstoTopology(ITopology pTopo, IFeatureDataset pFeatureDataset, esriGeometryType pGeometryType, esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError = null;
            AddClasstoTopology(pTopo, pFeatureDataset, pGeometryType, out outError);
            if (outError != null) return;

            AddRuletoTopology(pTopo, pTopoRuleType, out outError);
            if (outError != null) return;
        }




        /// <summary>
        /// ��Ҫ������ӵ������У��������飬����Դ��Ŀ���
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="feaClsNameDic"></param>
        /// <param name="outError"></param>
        private static void AddClasstoTopology(ITopology pTopo, IFeatureDataset pFeatureDataset, string oriFeaClsName,string desFeaClsName, out Exception outError)
        {
            outError = null;

            try
            {
                ITopologyRuleContainer pTopoRulrContainer = pTopo as ITopologyRuleContainer;
                List<IDataset> LstDataSet = GetAllFeaCls(pFeatureDataset);
                bool b = false;
                foreach (IDataset pDt in LstDataSet)
                {
                    IFeatureClass pFeaCls = pDt as IFeatureClass;
                    if (pFeaCls == null) continue;
                    if (pFeaCls.FeatureType != esriFeatureType.esriFTSimple) continue;
                    string pFeaName = pDt.Name;
                    if(pFeaName.Contains("."))
                    {
                        pFeaName = pFeaName.Substring(pFeaName.IndexOf('.') + 1);
                    }
                    if (pFeaName != oriFeaClsName && pFeaName != desFeaClsName) continue;
                    pTopo.AddClass(pFeaCls as IClass, 5, 1, 1, false);
                    b = true;
                }
                if (b == false)
                {
                    outError = new Exception("Ҫ���м���Ҫ���಻���ڣ�");
                    return;
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// ��Ҫ������ӵ������У��������飬����Դ��Ŀ���(�б�)
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="feaClsNameDic"></param>
        /// <param name="outError"></param>
        private static void AddDicClasstoTopology(ITopology pTopo, IFeatureDataset pFeatureDataset, List<string> feaclsNameDic, out Exception outError)
        {
            outError = null;
            List<string> feaClsNameLst = new List<string>();
            for (int i = 0; i < feaclsNameDic.Count; i++)
            {
                string feaclsDic = feaclsNameDic[i];
                string[] tStrArr = new string[2];
                tStrArr = feaclsDic.Split(new char[] { ';' });
                for (int j = 0; j < tStrArr.Length; j++)
                    if (!feaClsNameLst.Contains(tStrArr[j]))
                    {
                        feaClsNameLst.Add(tStrArr[j]);
                    }
            }

            try
            {
                ITopologyRuleContainer pTopoRulrContainer = pTopo as ITopologyRuleContainer;
                List<IDataset> LstDataSet = GetAllFeaCls(pFeatureDataset);
                bool b = false;
                foreach (IDataset pDt in LstDataSet)
                {
                    IFeatureClass pFeaCls = pDt as IFeatureClass;
                    if (pFeaCls == null) continue;
                    if (pFeaCls.FeatureType != esriFeatureType.esriFTSimple) continue;
                    string pFeaName = pDt.Name;
                    if (pFeaName.Contains("."))
                    {
                        pFeaName = pFeaName.Substring(pFeaName.IndexOf('.') + 1);
                    }
                    if (feaClsNameLst.Contains(pFeaName))
                    {
                        pTopo.AddClass(pFeaCls as IClass, 5, 1, 1, false);
                        b = true;
                    }
                }
                if (b == false)
                {
                    outError = new Exception("Ҫ���м���Ҫ���಻���ڣ�");
                    return;
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// ΪҪ����������˹����������飬����Դ��Ŀ���
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="feaclsNameDic"></param>
        /// <param name="pTopoRuleType"></param>
        /// <param name="outError"></param>
        private static void AddRuletoTopology(ITopology pTopo, string oriFeaClsName,string desFeaClsName, esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError = null;
           
            try
            {
                ITopologyRuleContainer pRuleContainer = pTopo as ITopologyRuleContainer;
                IFeatureClassContainer pFeaClsContainer = pTopo as IFeatureClassContainer;
                IEnumFeatureClass pEnumFeaCls = pFeaClsContainer.Classes;
                pEnumFeaCls.Reset();
                IFeatureClass pFeaCls = pEnumFeaCls.Next();

                //�������˹���
                ITopologyRule pTopoRule = new TopologyRuleClass();
                pTopoRule.TopologyRuleType = pTopoRuleType;
                while (pFeaCls != null)
                {
                    string pFeaClsName = (pFeaCls as IDataset).Name;
                    if(pFeaClsName.Contains("."))
                    {
                        pFeaClsName = pFeaClsName.Substring(pFeaClsName.IndexOf('.') + 1);
                    }
                    if (pFeaClsName == oriFeaClsName)
                    {
                        pTopoRule.Name = (pFeaCls as IDataset).Name;
                        pTopoRule.OriginClassID = pFeaCls.FeatureClassID;
                        pTopoRule.AllOriginSubtypes = true;
                    }
                    if (pFeaClsName == desFeaClsName)
                    {
                        pTopoRule.DestinationClassID = pFeaCls.FeatureClassID;
                        pTopoRule.AllDestinationSubtypes = true;
                    }
                    pFeaCls = pEnumFeaCls.Next();
                }
                pRuleContainer.AddRule(pTopoRule);
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// ΪҪ����������˹����������飬����Դ��Ŀ���(�б�)
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="feaclsNameDic"></param>
        /// <param name="pTopoRuleType"></param>
        /// <param name="outError"></param>
        public static void AddDicRuletoTopology(ITopology pTopo, List<string> feaclsNameDic, esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError = null;

            try
            {
                //�������˹���
                ITopologyRuleContainer pRuleContainer = pTopo as ITopologyRuleContainer;
               
                for (int i = 0; i < feaclsNameDic.Count; i++)
                {
                    string feaclsDic = feaclsNameDic[i];
                    string[] tStrArr = new string[2];
                    tStrArr = feaclsDic.Split(new char[] { ';' });
                    string oriFeaClsName = tStrArr[0];
                    string desFeaClsName = tStrArr[1];
                    
                    IFeatureClassContainer pFeaClsContainer = pTopo as IFeatureClassContainer;
                    IEnumFeatureClass pEnumFeaCls = pFeaClsContainer.Classes;
                    pEnumFeaCls.Reset();
                    IFeatureClass pFeaCls = pEnumFeaCls.Next();
                    ITopologyRule pTopoRule = new TopologyRuleClass();
                    pTopoRule.TopologyRuleType = pTopoRuleType;
                    while (pFeaCls != null)
                    {
                        string pFeaClsName = (pFeaCls as IDataset).Name;
                        if (pFeaClsName.Contains("."))
                        {
                            pFeaClsName = pFeaClsName.Substring(pFeaClsName.IndexOf('.') + 1);
                        }
                        if (pFeaClsName == oriFeaClsName)
                        {
                            pTopoRule.Name = (pFeaCls as IDataset).Name;
                            pTopoRule.OriginClassID = pFeaCls.FeatureClassID;
                            pTopoRule.AllOriginSubtypes = true;
                        }
                        if (pFeaClsName == desFeaClsName)
                        {
                            pTopoRule.DestinationClassID = pFeaCls.FeatureClassID;
                            pTopoRule.AllDestinationSubtypes = true;
                        }
                        pFeaCls = pEnumFeaCls.Next();
                    }
                    pRuleContainer.AddRule(pTopoRule);
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// ��Ҫ������ӵ������У���ΪҪ����������˹����������飬����Դ��Ŀ���
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="feaclsNameDic"></param>
        /// <param name="pTopoRuleType"></param>
        /// <param name="outError"></param>
        public static void AddRuleandClasstoTopology(ITopology pTopo, IFeatureDataset pFeatureDataset, string oriFeaClsName, string desFeaClsName, esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError = null;
            AddClasstoTopology(pTopo, pFeatureDataset,oriFeaClsName,desFeaClsName, out outError);
            if (outError != null) return;

            AddRuletoTopology(pTopo, oriFeaClsName,desFeaClsName, pTopoRuleType, out outError);
            if (outError != null) return;
        }

        /// <summary>
        /// ��Ҫ������ӵ������У���ΪҪ����������˹����������飬����Դ��Ŀ���(�б�)
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="feaclsNameDic"></param>
        /// <param name="pTopoRuleType"></param>
        /// <param name="outError"></param>
        public static void AddDicRuleandClasstoTopology(ITopology pTopo, IFeatureDataset pFeatureDataset,List<string> feaClsNameDic, esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError = null;
            AddDicClasstoTopology(pTopo, pFeatureDataset,feaClsNameDic, out outError);
            if (outError != null) return;

            AddDicRuletoTopology(pTopo, feaClsNameDic, pTopoRuleType, out outError);
            if (outError != null) return;
        }


        /// <summary>
        /// ��Ҫ������ӵ������У������������Ҫ������
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="feaClsNameLst"></param>
        /// <param name="outError"></param>
        private static void AddClasstoTopology(ITopology pTopo, IFeatureDataset pFeatureDataset, string pFeaClsName, out Exception outError)
        {
            outError = null;

            try
            {
                ITopologyRuleContainer pTopoRulrContainer = pTopo as ITopologyRuleContainer;
                List<IDataset> LstDataSet = GetAllFeaCls(pFeatureDataset);
                bool b = false;
                foreach (IDataset pDt in LstDataSet)
                {
                    IFeatureClass pFeaCls = pDt as IFeatureClass;
                    if (pFeaCls == null) continue;
                    if (pFeaCls.FeatureType != esriFeatureType.esriFTSimple) continue;
                    string pFeaName = pDt.Name.Trim();
                    if (pFeaName.Contains("."))
                    {
                        pFeaName = pFeaName.Substring(pFeaName.IndexOf('.') + 1);
                    }
                    if (pFeaName == pFeaClsName)
                    {
                        pTopo.AddClass(pFeaCls as IClass, 5, 1, 1, false);
                        b = true;
                        break;
                    }
                }
                if (b == false)
                {
                    outError = new Exception("Ҫ���м���Ҫ���಻���ڣ�");
                    return;
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// ��Ҫ������ӵ������У������������Ҫ������(�б�)
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="feaClsNameLst"></param>
        /// <param name="outError"></param>
        private static void AddClasstoTopology(ITopology pTopo, IFeatureDataset pFeatureDataset, List<string> pFeaClsNameList, out Exception outError)
        {
            outError = null;

            try
            {
                ITopologyRuleContainer pTopoRulrContainer = pTopo as ITopologyRuleContainer;
                List<IDataset> LstDataSet = GetAllFeaCls(pFeatureDataset);
                bool b = false;
                foreach (IDataset pDt in LstDataSet)
                {
                    IFeatureClass pFeaCls = pDt as IFeatureClass;
                    if (pFeaCls == null) continue;
                    if (pFeaCls.FeatureType != esriFeatureType.esriFTSimple) continue;
                    string pFeaName = pDt.Name.Trim();
                    if (pFeaName.Contains("."))
                    {
                        pFeaName = pFeaName.Substring(pFeaName.IndexOf('.') + 1);
                    }
                    if (pFeaClsNameList.Contains(pFeaName))
                    {
                        pTopo.AddClass(pFeaCls as IClass, 5, 1, 1, false);
                        b = true;
                    }
                }
                if (b == false)
                {
                    outError = new Exception("Ҫ���м���Ҫ���಻���ڣ�");
                    return;
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }


        /// <summary>
        /// ΪҪ����������˹��������������Ҫ������
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="pTopoRuleType"></param>
        /// <param name="outError"></param>
        public static void AddRuletoTopology(ITopology pTopo, esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError = null;

            try
            {
                ITopologyRuleContainer pRuleContainer = pTopo as ITopologyRuleContainer;
                IFeatureClassContainer pFeaClsContainer = pTopo as IFeatureClassContainer;
                IEnumFeatureClass pEnumFeaCls = pFeaClsContainer.Classes;
                pEnumFeaCls.Reset();
                IFeatureClass pFeaCls = pEnumFeaCls.Next();

                //�������˹���
                while (pFeaCls != null)
                {
                    ITopologyRule pTopoRule = new TopologyRuleClass();
                    pTopoRule.TopologyRuleType = pTopoRuleType;
                    pTopoRule.Name = (pFeaCls as IDataset).Name;
                    pTopoRule.OriginClassID = pFeaCls.FeatureClassID;
                    pTopoRule.AllOriginSubtypes = true;
                    pRuleContainer.AddRule(pTopoRule);
                    pFeaCls = pEnumFeaCls.Next();
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// ��Ҫ������ӵ������У���ΪҪ����������˹��������������Ҫ������
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="feaClsNameLst"></param>
        /// <param name="pTopoRuleType"></param>
        /// <param name="outError"></param>
        public static void AddRuleandClasstoTopology(ITopology pTopo, IFeatureDataset pFeatureDataset, string pFeaClsName, esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError = null;
            AddClasstoTopology(pTopo, pFeatureDataset,pFeaClsName, out outError);
            if (outError != null) return;

            AddRuletoTopology(pTopo, pTopoRuleType, out outError);
            if (outError != null) return;
        }

        /// <summary>
        /// ��Ҫ�����б���ӵ������У���ΪҪ����������˹��������������Ҫ������ 
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="feaClsNameLst"></param>
        /// <param name="pTopoRuleType"></param>
        /// <param name="outError"></param>
        public static void AddRuleandClasstoTopology(ITopology pTopo, IFeatureDataset pFeatureDataset, List<string> pFeaClsNameList, esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError = null;
            AddClasstoTopology(pTopo, pFeatureDataset, pFeaClsNameList, out outError);
            if (outError != null) return;

            AddRuletoTopology(pTopo, pTopoRuleType, out outError);
            if (outError != null) return;
        }

        
        //��֤����
        public static void ValidateTopology(ITopology topology, IEnvelope envelope, out Exception errEx)
        {
            errEx = null;
            try
            {
                ISegmentCollection pLocation = new PolygonClass();
                pLocation.SetRectangle(envelope);
                IPolygon pPoly = topology.get_DirtyArea(pLocation as IPolygon);
                IEnvelope pAreaValidated = topology.ValidateTopology(pPoly.Envelope);
            }
            catch (Exception err)
            {
                errEx = err;
            }
        }

        /// <summary>
        /// �Ƴ����˹���
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="outError"></param>
       private static void RemoveTopoRule(ITopology pTopo, out Exception outError)
        {
            outError = null;
            try
            {
                ITopologyRuleContainer pTopoRuleCon = pTopo as ITopologyRuleContainer;
                IEnumRule pEnumRule = pTopoRuleCon.Rules;
                pEnumRule.Reset();
                IRule pRule = pEnumRule.Next();
                while (pRule != null && pRule is ITopologyRule)
                {
                    pTopoRuleCon.DeleteRule(pRule as ITopologyRule);
                    pRule = pEnumRule.Next();
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// �Ƴ�������
        /// </summary>
        /// <param name="pTopo"></param>
        /// <param name="outError"></param>
        private static void RemoveTopoClass(ITopology pTopo, out Exception outError)
        {
            outError = null;
            try
            {
                IFeatureClassContainer pFeaClsCon = pTopo as IFeatureClassContainer;
                for (int j = pFeaClsCon.ClassCount - 1; j >= 0; j--)
                {
                    pTopo.RemoveClass(pFeaClsCon.get_Class(j) as IClass );
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// �Ƴ�����
        /// </summary>
        /// <param name="pFeaDataet"></param>
        /// <param name="pTopoName"></param>
        /// <param name="outError"></param>
        public static void RemoveTopo(IFeatureDataset pFeaDataet,out Exception outError)
        {
            outError = null;
            try
            {
                ITopologyContainer pTopoCon = pFeaDataet as ITopologyContainer;
                ITopology pTopo = pTopoCon.get_TopologyByName(pFeaDataet.Name);

                //ɾ�������������е����˹���
                RemoveTopoRule(pTopo, out outError);
                if (outError != null) return;

                //ɾ�������������ҵ�����ͼ��
                RemoveTopoClass(pTopo,out outError);
                if (outError != null) return;

                //ɾ������
                (pTopo as IDataset).Delete();
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// ������ݼ������е�Ҫ����
        /// </summary>
        /// <param name="pFeatureDataset"></param>
        /// <returns></returns>
        private  static List<IDataset> GetAllFeaCls(IFeatureDataset pFeatureDataset)
        {
            List<IDataset> LstDT = new List<IDataset>();

            IEnumDataset pEnumDt = pFeatureDataset.Subsets;
            pEnumDt.Reset();
            IDataset pDataset = pEnumDt.Next();
            while (pDataset != null)
            {
                LstDT.Add(pDataset);
                pDataset = pEnumDt.Next();
            }
            return LstDT;
        }

        //��ȡ����Ҫ�صĶ�λ��(��ʱΪҪ�ص�һ����)
        public static IPoint GetPointofFeature(IFeature feature)
        {
            IPointCollection pntCol = feature.Shape as IPointCollection;
            if (pntCol == null) return null;
            return pntCol.get_Point(0);
        }


        /// <summary>
        /// ���Ҳ���ֵ���
        /// </summary>
        /// <param name="pSysTable"></param>
        /// <param name="checkParaID">����ID��Ψһ��ʶ�������</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public static DataTable GetParaValueTable(SysCommon.DataBase.SysTable pSysTable, int checkParaID, out Exception eError)
        {
            eError = null;
            DataTable mTable = null;

            string selStr = "select * from GeoCheckPara where ����ID=" + checkParaID;
            DataTable pTable = pSysTable.GetSQLTable(selStr, out eError);
            if (eError != null)
            {
                eError = new Exception("��ѯ�����󣬱���Ϊ��GeoCheckPara������IDΪ:" + checkParaID);
                return null;
            }

            if (pTable == null || pTable.Rows.Count == 0)
            {
                eError = new Exception("�Ҳ�����¼������IDΪ:" + checkParaID);
                return null;
            }
            string ParaType = pTable.Rows[0]["��������"].ToString().Trim();            //��������
            if (ParaType == "GeoCheckParaValue")
            {
                int ParaValue = int.Parse(pTable.Rows[0]["����ֵ"].ToString().Trim());   //����ֵ��������ʶ�������
                string str = "select * from GeoCheckParaValue where �������=" + ParaValue;
                mTable = pSysTable.GetSQLTable(str, out eError);
                if (eError != null)
                {
                    eError = new Exception("��ѯ�����󣬱���Ϊ��GeoCheckParaValue���������Ϊ:" + ParaValue);
                    return null;
                }
            }
            return mTable;
        }

        /// <summary>
        /// ���Ҳ���ֵ���
        /// </summary>
        /// <param name="pFeaDataset"></param>
        /// <param name="pSysTable"></param>
        /// <param name="checkParaID">����ID��Ψһ��ʶ�������</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public static DataTable GetParaValueTable(IFeatureDataset pFeaDataset, SysCommon.DataBase.SysTable pSysTable, int checkParaID, out Exception eError)
        {
            eError = null;
            DataTable mTable = null;

            string selStr = "select * from GeoCheckPara where ����ID=" + checkParaID;
            DataTable pTable = pSysTable.GetSQLTable(selStr, out eError);
            if (eError != null)
            {
                eError = new Exception("��ѯ�����󣬱���Ϊ��GeoCheckPara������IDΪ:" + checkParaID);
                return null;
            }

            if (pTable == null || pTable.Rows.Count == 0)
            {
                eError = new Exception("�Ҳ�����¼������IDΪ:" + checkParaID);
                return null;
            }
            string ParaType = pTable.Rows[0]["��������"].ToString().Trim();            //��������
            if (ParaType == "GeoCheckParaValue")
            {
                int ParaValue = int.Parse(pTable.Rows[0]["����ֵ"].ToString().Trim());   //����ֵ��������ʶ�������
                string feaDTName = pFeaDataset.Name;                                   //���ݼ����� 
                if(feaDTName.Contains("."))
                {
                    feaDTName = feaDTName.Substring(feaDTName.IndexOf('.') + 1);
                }
                string str = "select * from GeoCheckParaValue where �������=" + ParaValue + " and ���ݼ�='" + feaDTName + "'";
                mTable = pSysTable.GetSQLTable(str, out eError);
                if (eError != null)
                {
                    eError = new Exception("��ѯ�����󣬱���Ϊ��GeoCheckParaValue���������Ϊ:" + ParaValue);
                    return null;
                }
            }
            return mTable;
        }

        /// <summary>
        /// ���ҷ�������ֶ�����
        /// </summary>
        /// <param name="pSysTable"></param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public static string GetCodeName(SysCommon.DataBase.SysTable pSysTable, out Exception eError)
        {
            eError = null;
            string codeName = "";    //��������ֶ�����
            string selStr = "select * from GeoCheckPara where ����ID=1";
            DataTable pTable = pSysTable.GetSQLTable(selStr, out eError);
            if (eError != null)
            {
                eError = new Exception("���ҷ�������ֶ�����ʧ�ܣ�");
            }

            if (pTable == null || pTable.Rows.Count == 0)
            {
                eError = new Exception("�Ҳ�����������ֶ����Ƽ�¼!");
            }
            codeName = pTable.Rows[0]["����ֵ"].ToString().Trim();
            if (codeName == "")
            {
                eError = new Exception("���ñ���δ���÷�������ֶ��������飡");
            }
            return codeName;
        }


        /// <summary>
        /// ���ݲ���ID��ò���ֵ
        /// </summary>
        /// <param name="pSysTable"></param>
        /// <param name="checkParaID"></param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public static string GetParaValue(SysCommon.DataBase.SysTable pSysTable, int checkParaID, out Exception eError)
        {
            eError = null;
            string paraValue = "";

            string selStr = "select * from GeoCheckPara where ����ID=" + checkParaID;
            DataTable pTable = pSysTable.GetSQLTable(selStr, out eError);
            if (eError != null)
            {
                eError = new Exception("��ѯ�����󣬱���Ϊ��GeoCheckPara������IDΪ:" + checkParaID);
                return "";
            }

            if (pTable == null || pTable.Rows.Count == 0)
            {
                eError = new Exception("�Ҳ�����¼������IDΪ:" + checkParaID);
                return "";
            }
            paraValue = pTable.Rows[0]["����ֵ"].ToString().Trim();            //��������
            return paraValue;
        }

       
    }

    //���ݼ����
    public class DataCheckClass
    {
        private DbConnection _ErrDbCon = null;                       //������־������
        public DbConnection ErrDbCon
        {
            get
            {
                return _ErrDbCon;
            }
            set
            {
                _ErrDbCon = value;
            }
        }
        private string _ErrTableName = "";                            //������־����
        public string ErrTableName
        {
            get
            {
                return _ErrTableName;
            }
            set
            {
                _ErrTableName = value;
            }
        }


        public event DataErrTreatHandle DataErrTreat;

        private DataTable _ResultTable =new DataTable();
        private Plugin.Application.IAppGISRef hook = null;

        private void CreateTable()
        {
            _ResultTable.Columns.Add("��鹦����", typeof(string));
            _ResultTable.Columns.Add("��������", typeof(string));
            _ResultTable.Columns.Add("��������", typeof(string));
            _ResultTable.Columns.Add("����ͼ��1", typeof(string));
            _ResultTable.Columns.Add("Ҫ��OID1", typeof(string));
            _ResultTable.Columns.Add("����ͼ��2", typeof(string));
            _ResultTable.Columns.Add("Ҫ��OID2", typeof(string));
            _ResultTable.Columns.Add("���ʱ��", typeof(string));
            _ResultTable.Columns.Add("��λ��X", typeof(string));
            _ResultTable.Columns.Add("��λ��Y", typeof(string));
            _ResultTable.Columns.Add("�����ļ���", typeof(string));
        }

        //���캯��
        public DataCheckClass(Plugin.Application.IAppGISRef phook)
        {
            CreateTable();
            hook = phook;
            if (hook.DataCheckGrid.RowCount > 0)
            {
                hook.DataCheckGrid.DataSource = null;
            }
            hook.DataCheckGrid.DataSource = _ResultTable;
            hook.DataCheckGrid.SelectionMode=DataGridViewSelectionMode.FullRowSelect;
            DataErrTreat += new DataErrTreatHandle(GeoDataChecker_DataErrTreat);
        }

        #region ���˼��
        //һ������˼�飬���ͬ����
        public void OrdinaryTopoCheck(IFeatureDataset pFeaDatset, esriGeometryType pGeometryType, esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError = null;
            Exception exError = null;
            //��������
            ITopology pTopo = TopologyCheckClass.CreateTopo(pFeaDatset, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }

            //��Ҫ������ӵ������в��������˹���
            TopologyCheckClass.AddRuleandClasstoTopology(pTopo, pFeaDatset, pGeometryType, pTopoRuleType, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                //outError = exError;
                return;
            }

            //��֤����
            TopologyCheckClass.ValidateTopology(pTopo, (pFeaDatset as IGeoDataset).Extent, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }

            //��ô����б�
            GetErrorList(pFeaDatset, pTopo,pTopoRuleType,"","", out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }
            TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
            if (outError != null) return;
        }
 
        //һ������˼�飬�������飬����Դ��Ŀ���
        public void OrdinaryTopoCheck(IFeatureDataset pFeaDatset, string oriFeaClsName, string desFeaClsName, esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError = null;
            Exception exError = null;
            //��������
            ITopology pTopo = TopologyCheckClass.CreateTopo(pFeaDatset, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }

            //��Ҫ������ӵ������в��������˹���
            TopologyCheckClass.AddRuleandClasstoTopology(pTopo, pFeaDatset, oriFeaClsName,desFeaClsName, pTopoRuleType, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                //outError = exError;
                return;
            }

            //��֤����
            TopologyCheckClass.ValidateTopology(pTopo, (pFeaDatset as IGeoDataset).Extent, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }

            //��ô����б�
            GetErrorList(pFeaDatset, pTopo, pTopoRuleType, oriFeaClsName, desFeaClsName, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }

            TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
            if (outError != null) return;
        }

        //һ������˼�飬�������飬����Դ��Ŀ���(�б�)
        public void OrdinaryDicTopoCheck(IFeatureDataset pFeaDatset, List<string> feaClsNameDic, esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError = null;
            Exception exError = null;
            //��������
            ITopology pTopo = TopologyCheckClass.CreateTopo(pFeaDatset, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }

            //��Ҫ������ӵ������в��������˹���
            TopologyCheckClass.AddDicRuleandClasstoTopology(pTopo, pFeaDatset, feaClsNameDic, pTopoRuleType, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                //outError = exError;
                return;
            }

            //��֤����
            TopologyCheckClass.ValidateTopology(pTopo, (pFeaDatset as IGeoDataset).Extent, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }

            //��ô����б�
            GetErrorList(pFeaDatset, pTopo, pTopoRuleType, "", "", out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }

            TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
            if (outError != null) return;
        }


        //һ������˼�飬��������ļ��,����Դ
        public void OrdinaryTopoCheck(IFeatureDataset pFeaDatset, string pFeaClsName, esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError = null;
            Exception exError = null;
            //��������
            ITopology pTopo = TopologyCheckClass.CreateTopo(pFeaDatset, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }

            //��Ҫ������ӵ������в��������˹���
            TopologyCheckClass.AddRuleandClasstoTopology(pTopo, pFeaDatset,pFeaClsName, pTopoRuleType, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                //outError = exError;
                return;
            }

            //��֤����
            TopologyCheckClass.ValidateTopology(pTopo, (pFeaDatset as IGeoDataset).Extent, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }

            //��ô����б�
            GetErrorList(pFeaDatset, pTopo, pTopoRuleType,pFeaClsName,"", out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }

            TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
            if (outError != null) return;
        }

        //һ������˼�飬��������ļ��,����Դ,�����б�������
        public void OrdinaryTopoCheck(IFeatureDataset pFeaDatset, List<string> pFeaClsNameList, esriTopologyRuleType pTopoRuleType, out Exception outError)
        {
            outError = null;
            Exception exError = null;
            //��������
            ITopology pTopo = TopologyCheckClass.CreateTopo(pFeaDatset, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }

            //��Ҫ������ӵ������в��������˹���
            TopologyCheckClass.AddRuleandClasstoTopology(pTopo, pFeaDatset, pFeaClsNameList, pTopoRuleType, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                //outError = exError;
                return;
            }

            //��֤����
            TopologyCheckClass.ValidateTopology(pTopo, (pFeaDatset as IGeoDataset).Extent, out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }

            //��ô����б�
            GetErrorList(pFeaDatset, pTopo, pTopoRuleType, "", "", out exError);
            if (exError != null)
            {
                TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                outError = exError;
                return;
            }

            TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
            if (outError != null) return;
        }


        /// <summary>
        /// �ߴ�����
        /// </summary>
        /// <param name="pFeaDatset"></param>
        /// <param name="oriFeaClsName">��Ҫ��������</param>
        /// <param name="desFeaClsName">��Ҫ��������</param>
        /// <param name="outError"></param>
        public void CrossTopoCheck(IFeatureDataset pFeaDatset, string oriFeaClsName, string desFeaClsName, out Exception outError)
        {
            outError = null;
            try
            {
                //ErrorEventArgs pErrorArgs = new ErrorEventArgs();

                //��Ҫ�������Ҫ����
                IFeatureClass mLineFeaCls = (pFeaDatset.Workspace as IFeatureWorkspace).OpenFeatureClass(oriFeaClsName);
                IFeatureClass mAreaFeaCls = (pFeaDatset.Workspace as IFeatureWorkspace).OpenFeatureClass(desFeaClsName);
                IFeatureCursor mAreaCursor = mAreaFeaCls.Search(null, false);
                if (mAreaCursor == null) return;
                IFeature mAreaFea = mAreaCursor.NextFeature();
                //������Ҫ��
                while (mAreaFea != null)
                {
                    //�����ߴ����Ҫ��
                    ISpatialFilter mFilter = new SpatialFilterClass();
                    mFilter.Geometry = mAreaFea.Shape;
                    mFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                    IFeatureCursor pCursor = mLineFeaCls.Search(mFilter, false);
                    if (pCursor == null) return;
                    IFeature mLineErrFeature = pCursor.NextFeature();
                    //�����������Ҫ��
                    while (mLineErrFeature != null)
                    {
                        //���������
                        IPoint pPoint = TopologyCheckClass.GetPointofFeature(mLineErrFeature);
                        double pMapx = pPoint.X;
                        double pMapy = pPoint.Y;

                        List<object> ErrorLst = new List<object>();
                        ErrorLst.Add("�������˼��");//����������
                        ErrorLst.Add("�ߴ�����");//��������
                        ErrorLst.Add((pFeaDatset as IDataset).Workspace.PathName);  //�����ļ���
                        ErrorLst.Add(enumErrorType.�ߴ�����.GetHashCode());//����id;
                        ErrorLst.Add("�ȸ��߲��ܴ�����");//��������
                        ErrorLst.Add(pMapx);    //...
                        ErrorLst.Add(pMapy);    //...
                        ErrorLst.Add(oriFeaClsName);
                        ErrorLst.Add(mLineErrFeature.OID);
                        ErrorLst.Add(desFeaClsName);
                        ErrorLst.Add(mAreaFea.OID);
                        ErrorLst.Add(false);
                        ErrorLst.Add(System.DateTime.Now.ToString());

                        //���ݴ�����־
                        IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                        DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                        DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
                        mLineErrFeature = pCursor.NextFeature();
                    }

                    //�ͷ�cursor
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);

                    mAreaFea = mAreaCursor.NextFeature();
                }

                //�ͷ�cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(mAreaCursor);
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// �����ҵ���
        /// </summary>
        /// <param name="pFeaDatset"></param>
        /// <param name="oriFeaClsName">Ҫ������Ҫ��������</param>
        /// <param name="oriWhereStr">��Ҫ�ع�������</param>
        /// <param name="desFeaClsName">Ŀ��Ҫ��������</param>
        /// <param name="desWhereStr">Ŀ��Ҫ�ع�������</param>
        /// <param name="tolenrence">�㻺���ݲ�</param>
        /// <param name="outError"></param>
        public void LineDangleCheck(IFeatureDataset pFeaDatset, string oriFeaClsName, string oriWhereStr, string desFeaClsName, string desWhereStr, double tolenrence, out Exception outError)
        {
            outError = null;
            try
            {
                //ErrorEventArgs pErrorArgs = new ErrorEventArgs();

                //����Ҫ�������ƻ����Ҫ���������Ҫ����
                IFeatureClass pLineFeCls = (pFeaDatset.Workspace as IFeatureWorkspace).OpenFeatureClass(oriFeaClsName);
                IFeatureClass pDesFeaCls = (pFeaDatset.Workspace as IFeatureWorkspace).OpenFeatureClass(desFeaClsName);
                
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = oriWhereStr;
                IFeatureCursor pOriCursor = pLineFeCls.Search(pQueryFilter, false);
                if (pOriCursor == null) return;
                IFeature lineFea = pOriCursor.NextFeature();
                while (lineFea != null)
                {
                    string eerStr = "��ȡ��Ҫ�ض˵������Ҫ��OIDΪ��";
                    //�Է�����������Ҫ�ؽ��������ҵ���

                    //������Ҫ�ص������˵�
                    IPointCollection mPointCol = new PolylineClass();
                    mPointCol.AddPointCollection(lineFea.ShapeCopy as IPointCollection);
                    //��Ҫ�������˵�
                    IPoint firstPoint = new PointClass();
                    IPoint lastPoint = new PointClass();
                    mPointCol.QueryPoint(0, firstPoint);
                    if (firstPoint == null)
                    {
                        eerStr += lineFea.OID + ",";
                        outError = new Exception(eerStr.Substring(0, eerStr.Length - 1));
                        lineFea = pOriCursor.NextFeature();
                        continue;
                    }
                    mPointCol.QueryPoint(mPointCol.PointCount - 1, lastPoint);
                    if (lastPoint == null)
                    {
                        eerStr += lineFea.OID + ",";
                        outError = new Exception(eerStr.Substring(0, eerStr.Length - 1));
                        lineFea = pOriCursor.NextFeature();
                        continue;
                    }

                    //���ݸ������ݲ�ֱ����Ҫ�ص������˵���л���
                    //�������巶Χ
                    IGeometry firstGeo = null;
                    IGeometry lastGeo = null;
                    ITopologicalOperator pTopoOper = firstPoint as ITopologicalOperator;
                    firstGeo = pTopoOper.Buffer(tolenrence);
                    pTopoOper = lastPoint as ITopologicalOperator;
                    lastGeo = pTopoOper.Buffer(tolenrence);
                    if (firstGeo == null || lastGeo == null)
                    {
                        lineFea = pOriCursor.NextFeature();
                        continue;
                    }

                    # region ���ݻ��巶Χ��Ŀ��Ҫ�����н��в��ң����Ҳ���Ҫ�أ������Ҫ�ش������ҵ����
                    //�ȸ��ݵ�һ���˵�Ļ��巶Χ���в���
                    ISpatialFilter spatialFilter = new SpatialFilterClass();
                    spatialFilter.WhereClause = desWhereStr;
                    spatialFilter.Geometry = firstGeo;
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelRelation;
                    spatialFilter.SpatialRelDescription = "***TT****";
                    IFeatureCursor desFeaCursor = pDesFeaCls.Search(spatialFilter, false);
                    if (desFeaCursor == null) return;
                    IFeature desFea = desFeaCursor.NextFeature();
                    if (desFea == null)
                    {
                        //û���ҵ������׶˵��ཻ��Ҫ�أ���Ҫ�ش������ҵ����

                        //����������ʾ����
                        GetErrorList(pFeaDatset , oriFeaClsName, lineFea);
                    }
                    else
                    {
                        //����β�˵�Ļ��巶Χ���в���
                        spatialFilter = new SpatialFilterClass();
                        spatialFilter.WhereClause = desWhereStr;
                        spatialFilter.Geometry = lastGeo;
                        spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelRelation;
                        spatialFilter.SpatialRelDescription = "***TT****";
                        IFeatureCursor desFeaCursor2 = pDesFeaCls.Search(spatialFilter, false);
                        if (desFeaCursor2 == null) return;
                        IFeature desFea2 = desFeaCursor2.NextFeature();
                        if (desFea2 == null)
                        {
                            //û���ҵ������׶˵��ཻ��Ҫ�أ���Ҫ�ش������ҵ����

                            //����������ʾ����
                            GetErrorList(pFeaDatset, oriFeaClsName, lineFea);
                        }

                        //�ͷ�cursor
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(desFeaCursor2);
                    }

                    //�ͷ�cursor
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(desFeaCursor);
                    #endregion
                    lineFea = pOriCursor.NextFeature();
                }

                //�ͷ�cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pOriCursor);
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }
        
        /// <summary>
        /// ��ô����б�һ������˼��
        /// </summary>
        /// <param name="pFeaDatset"></param>
        /// <param name="pTopo"></param>
        /// <param name="outError"></param>
        private void GetErrorList(IFeatureDataset pFeaDatset, ITopology pTopo, esriTopologyRuleType pTopoRuleType, string pOrgFeaClsName, string desFeaClsName, out Exception outError)
        {
            outError = null;
            try
            {
                //�������˹����ȡ����Ҫ��
                IErrorFeatureContainer pErrorFeaCon = pTopo as IErrorFeatureContainer;
                ITopologyRuleContainer pTopoRuleCon = pTopo as ITopologyRuleContainer;
                IEnumRule pEnumRule = pTopoRuleCon.Rules;
                pEnumRule.Reset();
                ITopologyRule pTopoRule = pEnumRule.Next() as ITopologyRule;
                //ErrorEventArgs pErrorArgs = new ErrorEventArgs();
                while (pTopoRule != null)
                {
                    IEnumTopologyErrorFeature pEnumErrorFea = null;
                    try
                    {
                        pEnumErrorFea = pErrorFeaCon.get_ErrorFeatures(
                            (pFeaDatset as IGeoDataset).SpatialReference, pTopoRule, (pFeaDatset as IGeoDataset).Extent, true, false);
                    }
                    catch (Exception ex)
                    {
                        TopologyCheckClass.RemoveTopo(pFeaDatset, out outError);
                        outError = ex;
                        return;
                    }
                    if (pEnumErrorFea == null)
                    {
                        pTopoRule = pEnumRule.Next() as ITopologyRule;
                        continue;
                    }
                    ITopologyErrorFeature pErrorFea = pEnumErrorFea.Next();
                    while (pErrorFea != null)
                    {
                        //����������������
                        double pMapx = 0.0;
                        double pMapy = 0.0;
                        int oriFeaClsID = pErrorFea.OriginClassID;
                        int desFeaClsID = pErrorFea.DestinationClassID;
                        //string desFeaClsName = "";
                        int desID = pErrorFea.DestinationOID;
                        int oriID = pErrorFea.OriginOID;

                        # region ͬ�����ཻ���ų���ͬ�����ص������
                        if (pTopoRuleType == esriTopologyRuleType.esriTRTLineNoIntersection)
                        {
                            if (pErrorFea.ShapeType == esriGeometryType.esriGeometryPolyline)
                            {
                                //ͬ�����ص�����Ϊ�������˴���
                                pTopoRuleCon.PromoteToRuleException(pErrorFea);
                                pErrorFea = pEnumErrorFea.Next();
                                continue;
                            }
                        }
                        #endregion

                        if (desFeaClsID > 0)
                        {
                            //Ŀ��Ҫ��������
                            desFeaClsName = ((pTopo as IFeatureClassContainer).get_ClassByID(pErrorFea.DestinationClassID) as IDataset).Name;
                        }
                        if (desID <= 0)
                        {
                            //Ŀ��Ҫ��OID
                            desID = -1;
                        }

                        IFeatureClass oriFeaCls = null;
                        //if (oriFeaClsID <= 0)
                        //{
                        //    outError = new Exception("���˼������ʾ����ȷ���Ҳ���ԴҪ����ID��");
                        //    return;
                        //    //pErrorFea = pEnumErrorFea.Next();
                        //    //continue;
                        //}
                        if (oriFeaClsID > 0)
                        {
                            oriFeaCls = (pTopo as IFeatureClassContainer).get_ClassByID(oriFeaClsID);
                        }
                        else
                        {
                            if (pTopoRuleType == esriTopologyRuleType.esriTRTAreaAreaCoverEachOther)
                            {
                            }
                            else
                            {
                                if (oriFeaCls == null)
                                {
                                    outError = new Exception("��ȡԴҪ������Ϣ����");
                                    return;
                                }
                            }
                        }
                        
                        
                        if (oriID > 0)
                        {
                            IPoint pPoint = null;
                            IFeature oriFeature = oriFeaCls.GetFeature(oriID);
                            if (oriFeature.Shape.GeometryType != esriGeometryType.esriGeometryPoint)
                            {
                                pPoint = TopologyCheckClass.GetPointofFeature(oriFeature);
                            }
                            else
                            {
                                //˵����Ҫ���ǵ�Ҫ��
                                pPoint = oriFeature.Shape as IPoint;
                            }
                            pMapx = pPoint.X;
                            pMapy = pPoint.Y;
                        }
                        else
                        {
                            if (pTopoRuleType == esriTopologyRuleType.esriTRTAreaNoGaps)
                            {
                                # region ���϶����oriIDΪ0��ͨ������Ĵ���������϶����ԴҪ��
                                IFeature oriFea = null;

                                //�����㼯��
                                IPointCollection mPointCol = new PolygonClass();
                                IFeature pFeature = pErrorFea as IFeature;
                                mPointCol.AddPointCollection(pFeature.ShapeCopy as IPointCollection);

                                //�պϵ㼯�ϳ���״
                                IPolygon mPolygon = mPointCol as IPolygon;
                                mPolygon.Close();

                                //���ݱպϵ���״���в���
                                mPointCol = mPolygon as IPointCollection;
                                if (mPointCol is IArea)
                                {
                                    //���㼯������״�����д���
                                    ITopologicalOperator2 mTopoOper = mPointCol as ITopologicalOperator2;
                                    mTopoOper.IsKnownSimple_2 = false;
                                    mTopoOper.Simplify();
                                    mPointCol = mTopoOper as IPointCollection;
                                    //������˴������Ҫ�أ���Ϊ���⴦��
                                    ISpatialFilter pFilter = new SpatialFilterClass();
                                    pFilter.Geometry = mPointCol as IGeometry;
                                    pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                                    IFeatureCursor pFeaCursor = oriFeaCls.Search(pFilter, false);
                                    if (pFeaCursor == null) return;
                                    oriFea = pFeaCursor.NextFeature();
                                    if (oriFea != null)
                                    {
                                        pTopoRuleCon.PromoteToRuleException(pErrorFea);
                                        pErrorFea = pEnumErrorFea.Next();
                                        continue;
                                    }
                                    //�����Ϊ0.����Ϊ���⴦��
                                    IArea mArea = mPointCol as IArea;
                                    if (mArea.Area == 0)
                                    {
                                        pTopoRuleCon.PromoteToRuleException(pErrorFea);
                                        pErrorFea = pEnumErrorFea.Next();
                                        continue;
                                    }
                                    else
                                    {
                                        //���ԴҪ��
                                        pFilter = new SpatialFilterClass();
                                        pFilter.Geometry = mTopoOper as IGeometry;
                                        pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;//����
                                        pFeaCursor = oriFeaCls.Search(pFilter, false);
                                        if (pFeaCursor == null) return;
                                        oriFea = pFeaCursor.NextFeature();
                                        if (oriFea == null)
                                        {
                                            pErrorFea = pEnumErrorFea.Next();
                                            continue;
                                        }
                                        oriID = oriFea.OID;
                                        IPoint mPoint = TopologyCheckClass.GetPointofFeature(oriFea);
                                        pMapx = mPoint.X;
                                        pMapy = mPoint.Y;
                                    }

                                    //�ͷ�cursor
                                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaCursor);
                                }
                                else
                                {
                                    pErrorFea = pEnumErrorFea.Next();
                                    continue;
                                }
                                #endregion
                            }
                            if (desID >0 && desFeaClsID >0)
                            {
                                IFeatureClass desFeaCls = (pTopo as IFeatureClassContainer).get_ClassByID(desFeaClsID);
                                IPoint pPoint = null;
                                IFeature desFeature = desFeaCls.GetFeature(desID);
                                if (desFeature.Shape.GeometryType != esriGeometryType.esriGeometryPoint)
                                {
                                    pPoint = TopologyCheckClass.GetPointofFeature(desFeature);
                                }
                                else
                                {
                                    //˵����Ҫ���ǵ�Ҫ��
                                    pPoint = desFeature.Shape as IPoint;
                                }
                                pMapx = pPoint.X;
                                pMapy = pPoint.Y;
                            }
                        }

                        # region ����������Ϣ���ౣ��
                        int errID = 0;              //����ID��
                        string errDes = "";         //��������
                        switch (pTopoRuleType)     
                        {
                            case esriTopologyRuleType.esriTRTLineNoSelfIntersect:
                                //�����ཻ
                                errID = enumErrorType.�����ཻ���.GetHashCode();
                                errDes = "�߲��ܺ��Լ��ཻ";
                                break;
                            case esriTopologyRuleType.esriTRTLineNoSelfOverlap:
                                //�����ص�
                                errID = enumErrorType.�����ص����.GetHashCode();
                                errDes = "�߲��ܺ��Լ��ص�";
                                break;
                            case esriTopologyRuleType.esriTRTLineNoPseudos:
                                //��α�ڵ�
                                errID = enumErrorType.�ߴ���α�ڵ�.GetHashCode();
                                errDes = "������α�ڵ㣬���߶εĶ˵㲻�ܽ����������˵�ĽӴ��㣨������β�Ӵ��ĳ��⣩";
                                break;
                            case esriTopologyRuleType.esriTRTAreaNoOverlap:
                                //ͬ�����ص�
                                errID = enumErrorType.ͬ�����ص����.GetHashCode();
                                errDes = "ͬһ��Ҫ�����еĶ����Ҫ�ز��ܻ����ص�";
                                break;
                            case esriTopologyRuleType.esriTRTLineNoDangles:
                                //�ߴ������ҵ�
                                errID = enumErrorType.�ߴ������ҵ�.GetHashCode();
                                errDes = "��������Ҫ��������㣬��ÿһ���߶εĶ˵㶼���ܹ���";
                                break;
                            case esriTopologyRuleType.esriTRTLineNoMultipart:
                                //���߼��
                                errID = enumErrorType.���߼��.GetHashCode();
                                errDes = "�߱���Ϊ���ߣ������໥�Ӵ����ص�";
                                break;
                            case esriTopologyRuleType.esriTRTAreaNoOverlapArea:
                                //������ص�
                                errID = enumErrorType.������ص����.GetHashCode();
                                errDes = "ԴҪ�����еĶ���β�����Ŀ��Ҫ�����еĶ�����ص�";
                                break;
                            case esriTopologyRuleType.esriTRTAreaNoGaps:
                                //���϶
                                errID = enumErrorType.���϶���.GetHashCode();
                                errDes = "����β����з�϶";
                                break;
                            case esriTopologyRuleType.esriTRTAreaCoveredByArea:
                                //�溬��
                                errID = enumErrorType.�溬����.GetHashCode();
                                errDes = "ԴҪ�����еĶ���α��뱻Ŀ��Ҫ�����еĶ����������";
                                break;
                            case esriTopologyRuleType.esriTRTPointCoveredByAreaBoundary:
                                //�����
                                errID = enumErrorType.�������.GetHashCode();
                                errDes = "ԴҪ�����еĵ������Ŀ��Ҫ����Ķ���α߽���";
                                break;
                            case esriTopologyRuleType.esriTRTPointCoveredByLine:
                                //�����
                                errID = enumErrorType.����߼��.GetHashCode();
                                errDes = "ԴҪ�����еĵ������Ŀ��Ҫ�������֮��";
                                break;
                            case esriTopologyRuleType.esriTRTPointCoveredByLineEndpoint:
                                //��λ���߶˵���
                                errID = enumErrorType.��λ���߶˵���.GetHashCode();
                                errDes = "ԴҪ�����еĵ����λ��Ŀ��Ҫ�����е��ߵĶ˵���";
                                break;
                            case esriTopologyRuleType.esriTRTPointProperlyInsideArea:
                                //��λ�����ڼ��
                                errID = enumErrorType.��λ�����ڼ��.GetHashCode();
                                errDes = "ԴҪ�����еĵ����λ��Ŀ��Ҫ����Ķ������";
                                break;
                            case esriTopologyRuleType.esriTRTLineNoOverlap:
                                //ͬ�����ص�
                                errID = enumErrorType.ͬ�����ص����.GetHashCode();
                                errDes = "ͬһҪ�����У�������Ҫ�ز����ص�";
                                break;
                            case esriTopologyRuleType.esriTRTLineNoIntersection:
                                //ͬ�����ཻ
                                errID = enumErrorType.ͬ�����ཻ���.GetHashCode();
                                errDes = "ͬһҪ�����У�������Ҫ�ز����ཻ";
                                break;
                            case esriTopologyRuleType.esriTRTLineCoveredByAreaBoundary:
                                //����߽��غϼ��
                                errID = enumErrorType.����߽��غϼ��.GetHashCode();
                                errDes = "ԴҪ�����е��߱��뱻Ŀ��Ҫ�����еĶ���εı߽縲��";
                                break;
                            case esriTopologyRuleType.esriTRTLineNoOverlapLine:
                                //������ص����
                                errID = enumErrorType.������ص����.GetHashCode();
                                errDes = "ԴҪ�����е�����Ŀ��Ҫ�����е��߲����ص�";
                                break;
                            case esriTopologyRuleType.esriTRTAreaBoundaryCoveredByAreaBoundary:
                                //��߽���߽��غϼ��
                                errID = enumErrorType.��߽���߽��غϼ��.GetHashCode();
                                errDes = "ԴҪ�����еĶ���α߽���뱻Ŀ��Ҫ�����еĶ���α߽縲��";
                                break;
                            case esriTopologyRuleType.esriTRTAreaAreaCoverEachOther:
                                //�����໥���Ǽ��
                                errID = enumErrorType.�����໥���Ǽ��.GetHashCode();
                                errDes = "ԴҪ������Ŀ��Ҫ�����еĶ���α����໥����";
                                break;
                            case esriTopologyRuleType.esriTRTAreaContainPoint:
                                //�溬����
                                errID = enumErrorType.�溬����.GetHashCode();
                                errDes = "ԴҪ�����еĶ���α��������";
                                break;
                            case esriTopologyRuleType.esriTRTAreaBoundaryCoveredByLine:
                                //��߽����غϼ��
                                errID = enumErrorType.��߽����غϼ��.GetHashCode();
                                errDes = "ԴҪ�����ж���εı߽�������Ҫ���غ�";
                                break;
                            case esriTopologyRuleType.esriTRTLineCoveredByLineClass:
                                //�����غϼ��
                                errID = enumErrorType.�����غϼ��.GetHashCode();
                                errDes = "ԴҪ�����е���Ҫ�ر��뱻Ŀ��Ҫ�����е���Ҫ�ظ���";
                                break;
                            case esriTopologyRuleType.esriTRTLineEndpointCoveredByPoint:
                                //�߶˵㱻�㸲�Ǽ��
                                errID = enumErrorType.�߶˵㱻�㸲�Ǽ��.GetHashCode();
                                errDes = "ԴҪ�������ߵĶ˵���뱻Ŀ��Ҫ�����еĵ㸲��";
                                break;
                            default:
                                break;
                        }
                        #endregion

                        //�������������
                        List<object> ErrorLst = new List<object>();
                        ErrorLst.Add("һ�����˼��");//����������
                        ErrorLst.Add("һ�����˼��");//��������
                        ErrorLst.Add((pFeaDatset as IDataset).Workspace.PathName);  //�����ļ���
                        ErrorLst.Add(errID );//����id;
                        ErrorLst.Add(errDes);//��������
                        ErrorLst.Add(pMapx);    //...
                        ErrorLst.Add(pMapy);    //...

                        
                        if(oriFeaCls!=null)
                        {
                            ErrorLst.Add((oriFeaCls as IDataset).Name);
                            ErrorLst.Add(oriID);
                        }
                        else 
                        {
                            ErrorLst.Add(pOrgFeaClsName);
                            ErrorLst.Add(-1);
                        }
                       
                        ErrorLst.Add(desFeaClsName);
                        ErrorLst.Add(desID);
                        ErrorLst.Add(false);
                        ErrorLst.Add(System.DateTime.Now.ToString());

                        //���ݴ�����־
                        IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                        DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                        DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
                        pErrorFea = pEnumErrorFea.Next();
                    }

                    pTopoRule = pEnumRule.Next() as ITopologyRule;
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// ��ô����б� ,�����ҵ���
        /// </summary>
        /// <param name="pFeaDatset"></param>
        /// <param name="oriFeaclsName"></param>
        /// <param name="oriFeature"></param>
        /// <param name="desFeaclsName"></param>
        /// <param name="desFeature"></param>
        private void GetErrorList(IFeatureDataset pFeaDatset, string oriFeaclsName, IFeature oriFeature)
        {
            //���������
            IPoint pPoint = TopologyCheckClass.GetPointofFeature(oriFeature);
            double pMapx = pPoint.X;
            double pMapy = pPoint.Y;

            List<object> ErrorLst = new List<object>();
            ErrorLst.Add("�����˼��");//����������
            ErrorLst.Add("�����ҵ���");//��������
            ErrorLst.Add((pFeaDatset as IDataset).Workspace.PathName);  //�����ļ���
            ErrorLst.Add(enumErrorType.�ߴ������ҵ�.GetHashCode());//����id;
            ErrorLst.Add("��������Ҫ��������㣬��ÿһ���߶εĶ˵㶼���ܹ���");//��������
            ErrorLst.Add(pMapx);    //...
            ErrorLst.Add(pMapy);    //...
            ErrorLst.Add(oriFeaclsName);
            ErrorLst.Add(oriFeature.OID );
            ErrorLst.Add("");
            ErrorLst.Add(-1);
            ErrorLst.Add(false);
            ErrorLst.Add(System.DateTime.Now.ToString());

            //���ݴ�����־
            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
            DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
        }

        #endregion


        /// <summary>
        /// ��ѧ������ȷ�Լ��
        /// </summary>
        /// <param name="hook"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="coorStr">�ռ�ο��ַ���</param>
        /// <param name="outError"></param>
        public void MathematicsCheck(IFeatureDataset pFeatureDataset, string coorStr, out Exception outError)
        {
            outError = null;

            //������ݼ��Ŀռ�ο�
            IGeoDataset pGeoDataset = pFeatureDataset as IGeoDataset;
            ISpatialReference pSpatialRef = pGeoDataset.SpatialReference;

            string strSpatialDes = "";//�ռ�ο��ַ���

            #region ���� �ռ�ο��ַ���
            int byteRead = -1;
            try
            {
                IESRISpatialReferenceGEN pSpatialRefGEN = null;
                IGeographicCoordinateSystem pGeoCoor = new GeographicCoordinateSystemClass();
                IProjectedCoordinateSystem pProCoor = new ProjectedCoordinateSystemClass();
                IUnknownCoordinateSystem pUnknownCoor = new UnknownCoordinateSystemClass();
                pGeoCoor = pSpatialRef as IGeographicCoordinateSystem;
                pProCoor = pSpatialRef as IProjectedCoordinateSystem;
                pUnknownCoor = pSpatialRef as IUnknownCoordinateSystem;

                if (pGeoCoor != null)
                {
                    pSpatialRefGEN = pGeoCoor as IESRISpatialReferenceGEN;
                }
                if (pProCoor != null)
                {
                    pSpatialRefGEN = pProCoor as IESRISpatialReferenceGEN;
                }
                if (pSpatialRefGEN != null)
                {
                    pSpatialRefGEN.ExportToESRISpatialReference(out strSpatialDes, out byteRead);
                    if (strSpatialDes == "")
                    {
                        outError = new Exception("�����ռ�ο�����");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                outError = ex;
                return;
            }
            #endregion

            //���׼�Ŀռ�ο��ַ����Աȼ��
            if (strSpatialDes != coorStr)
            {
                //�������������
                List<object> ErrorLst = new List<object>();
                ErrorLst.Add("��ѧ������ȷ�Լ��");//����������
                ErrorLst.Add("��ѧ������ȷ�Լ��");//��������
                ErrorLst.Add((pFeatureDataset as IDataset).Workspace.PathName);  //�����ļ���
                ErrorLst.Add(enumErrorType.��ѧ������ȷ�Լ��.GetHashCode());//����id;
                ErrorLst.Add("���ݼ�" + pFeatureDataset.Name + "�Ŀռ�ο����׼�Ŀռ�ο���һ��");//��������
                ErrorLst.Add(0);    //...
                ErrorLst.Add(0);    //...
                ErrorLst.Add("");
                ErrorLst.Add(-1);
                ErrorLst.Add("");
                ErrorLst.Add(-1);
                ErrorLst.Add(false);
                ErrorLst.Add(System.DateTime.Now.ToString());

                //���ݴ�����־
                IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
            }
        }

        /// <summary>
        /// ��ѧ������ȷ�Լ��
        /// </summary>
        /// <param name="pFeatureDataset"></param>
        /// <param name="standardRef"></param>
        /// <param name="outError">��׼�Ŀռ�ο�</param>
        public void MathematicsCheck(IFeatureDataset pFeatureDataset,ISpatialReference standardRef, out Exception outError)
        {
            outError = null;

            //��׼�ռ�ο��ַ���
            string sptialStandStr = "";
            int bytesWrote1;
            IESRISpatialReferenceGEN pSpatialStandGEN = standardRef as IESRISpatialReferenceGEN;
            pSpatialStandGEN.ExportToESRISpatialReference(out sptialStandStr, out bytesWrote1);

            //������ݼ��Ŀռ�ο�
            IGeoDataset pGeoDataset = pFeatureDataset as IGeoDataset;
            ISpatialReference pSpatialRef = pGeoDataset.SpatialReference;
            IESRISpatialReferenceGEN pSpatialGEN = pSpatialRef as IESRISpatialReferenceGEN;
            string spatialStr = "";
            int bytesWrote;
            pSpatialGEN.ExportToESRISpatialReference(out spatialStr, out bytesWrote);
           
            //���׼�Ŀռ�ο��Աȼ��
            //if (pSpatialRef!=standardRef)
            if (sptialStandStr != spatialStr)
            {
                //�������������
                List<object> ErrorLst = new List<object>();
                ErrorLst.Add("��ѧ������ȷ�Լ��");//����������
                ErrorLst.Add("��ѧ������ȷ�Լ��");//��������
                ErrorLst.Add((pFeatureDataset as IDataset).Workspace.PathName);  //�����ļ���
                ErrorLst.Add(enumErrorType.��ѧ������ȷ�Լ��.GetHashCode());//����id;
                ErrorLst.Add("���ݼ�" + pFeatureDataset.Name + "�Ŀռ�ο����׼�Ŀռ�ο���һ��");//��������
                ErrorLst.Add(0);    //...
                ErrorLst.Add(0);    //...
                ErrorLst.Add("");
                ErrorLst.Add(-1);
                ErrorLst.Add("");
                ErrorLst.Add(-1);
                ErrorLst.Add(false);
                ErrorLst.Add(System.DateTime.Now.ToString());

                //���ݴ�����־
                IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
            }
        }

        /// <summary>
        /// ��ѧ������ȷ�Լ��
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <param name="standardRef"></param>
        /// <param name="outError"></param>
        public void MathematicsCheck(IFeatureClass pFeatureClass, ISpatialReference standardRef, out Exception outError)
        {
            outError = null;
            
            //��׼�ռ�ο��ַ���
            string sptialStandStr = "";
            int bytesWrote1;
            IESRISpatialReferenceGEN pSpatialStandGEN = standardRef as IESRISpatialReferenceGEN;
            pSpatialStandGEN.ExportToESRISpatialReference(out sptialStandStr, out bytesWrote1);

            //������ݼ��Ŀռ�ο�
            IGeoDataset pGeoDataset = pFeatureClass as IGeoDataset;
            ISpatialReference pSpatialRef = pGeoDataset.SpatialReference;
            IESRISpatialReferenceGEN pSpatialGEN = pSpatialRef as IESRISpatialReferenceGEN;
            string spatialStr = "";
            int bytesWrote;
            pSpatialGEN.ExportToESRISpatialReference(out spatialStr, out bytesWrote);
            


            //���׼�Ŀռ�ο��Աȼ��
            //if (pSpatialRef != standardRef)
            if (sptialStandStr != spatialStr)
            {
                //�������������
                List<object> ErrorLst = new List<object>();
                ErrorLst.Add("��ѧ������ȷ�Լ��");//����������
                ErrorLst.Add("��ѧ������ȷ�Լ��");//��������
                ErrorLst.Add((pFeatureClass as IDataset).Workspace.PathName);  //�����ļ���
                ErrorLst.Add(enumErrorType.��ѧ������ȷ�Լ��.GetHashCode());//����id;
                ErrorLst.Add("���ݼ�" + (pFeatureClass as IDataset).Name + "�Ŀռ�ο����׼�Ŀռ�ο���һ��");//��������
                ErrorLst.Add(0);    //...
                ErrorLst.Add(0);    //...
                ErrorLst.Add("");
                ErrorLst.Add(-1);
                ErrorLst.Add("");
                ErrorLst.Add(-1);
                ErrorLst.Add(false);
                ErrorLst.Add(System.DateTime.Now.ToString());

                //���ݴ�����־
                IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
            }
        }


        #region Ҫ�����Լ��

        /// <summary>
        /// ��ֵ���
        /// </summary>
        /// <param name="hook"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="FeaClsNameDic">ͼ���������Էǿյ��ֶ�����</param>
        /// <param name="outError"></param>
        public void IsNullableCheck( IFeatureDataset pFeatureDataset, Dictionary<string, List<string>> FeaClsNameDic, out Exception outError)
        {
            outError = null;
            try
            {
                foreach (KeyValuePair<string, List<string>> item in FeaClsNameDic)
                {
                    IFeatureClass pFeaCls = (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(item.Key.Trim());
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        string fieldName = item.Value[i].Trim();
                        int index = pFeaCls.Fields.FindField(fieldName);
                        if (index == -1)
                        {
                            outError = new Exception("�Ҳ����ֶ���Ϊ" + fieldName + "���ֶ�");
                            return;
                        }
                        if (pFeaCls.Fields.get_Field(index).IsNullable == true)
                        {
                            //�ֶ�����Ϊ�գ����׼��һ�£��������������
                            List<object> ErrorLst = new List<object>();
                            ErrorLst.Add("Ҫ�����Լ��");//����������
                            ErrorLst.Add("��ֵ���");//��������
                            ErrorLst.Add((pFeatureDataset as IDataset).Workspace.PathName);  //�����ļ���
                            ErrorLst.Add(enumErrorType.��ֵ���.GetHashCode());//����id;
                            ErrorLst.Add("ͼ��" + item.Key + "���ֶ�" + fieldName + "����ֵ������Ϊ��");//��������
                            ErrorLst.Add(0);    //...
                            ErrorLst.Add(0);    //...
                            ErrorLst.Add(item.Key);
                            ErrorLst.Add(-1);
                            ErrorLst.Add("");
                            ErrorLst.Add(-1);
                            ErrorLst.Add(false);
                            ErrorLst.Add(System.DateTime.Now.ToString());

                            //���ݴ�����־
                            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                            DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
                        }
                        else
                        {
                            IFeatureCursor pFeaCusor = pFeaCls.Search(null, false);
                            if(pFeaCusor==null) return;
                            IFeature pFeature = pFeaCusor.NextFeature();
                            if (pFeature == null) continue;
                            while (pFeature != null)
                            {
                                object fieldValue = pFeature.get_Value(index);
                                if (fieldValue == null || fieldValue.ToString() == "")
                                {
                                    //�ֶ�ֵ����Ϊ�գ����������������
                                    double pMapx = 0.0;
                                    double pMapy = 0.0;
                                    IPoint pPoint = new PointClass();
                                    if (pFeaCls.ShapeType != esriGeometryType.esriGeometryPoint)
                                    {
                                        pPoint = TopologyCheckClass.GetPointofFeature(pFeature);
                                    }
                                    else
                                    {
                                        pPoint = pFeature.Shape as IPoint;
                                    }
                                    pMapx = pPoint.X;
                                    pMapy = pPoint.Y;
                                    List<object> ErrorLst = new List<object>();
                                    ErrorLst.Add("Ҫ�����Լ��");//����������
                                    ErrorLst.Add("��ֵ���");//��������
                                    ErrorLst.Add((pFeatureDataset as IDataset).Workspace.PathName);  //�����ļ���
                                    ErrorLst.Add(enumErrorType.��ֵ���.GetHashCode());//����id;
                                    ErrorLst.Add("ͼ��" + item.Key + "���ֶ�" + fieldName + "��ֵ����Ϊ��");//��������
                                    ErrorLst.Add(pMapx);    //...
                                    ErrorLst.Add(pMapy);    //...
                                    ErrorLst.Add(item.Key);
                                    ErrorLst.Add(pFeature.OID);
                                    ErrorLst.Add("");
                                    ErrorLst.Add(-1);
                                    ErrorLst.Add(false);
                                    ErrorLst.Add(System.DateTime.Now.ToString());

                                    //���ݴ�����־
                                    IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                                    DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                                    DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
                                }
                                pFeature = pFeaCusor.NextFeature();
                            }

                            //�ͷ�cursor
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaCusor);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// ��ֵ���
        /// </summary>
        /// <param name="pFeatureClassLst"></param>
        /// <param name="FeaClsNameDic"></param>
        /// <param name="outError"></param>
        public void IsNullableCheck(List<IFeatureClass> pFeatureClassLst, Dictionary<string, List<string>> FeaClsNameDic, out Exception outError)
        {
            outError = null;
            try
            {

                if (hook.DataTree == null) return;
                hook.DataTree.Nodes.Clear();
                //����������ͼ
                IntialTree(hook.DataTree);
                //�������ڵ���ɫ
                setNodeColor(hook.DataTree);
                hook.DataTree.Tag = false;

                foreach (KeyValuePair<string, List<string>> item in FeaClsNameDic)
                {
                    IFeatureClass pFeaCls = null;
                    foreach(IFeatureClass pFeaClsItem in pFeatureClassLst)
                    {
                        string pFeaClsNm= (pFeaClsItem as IDataset).Name;
                        if(pFeaClsNm.Contains("."))
                        {
                            pFeaClsNm = pFeaClsNm.Substring(pFeaClsNm.IndexOf('.') + 1);
                        }
                        if (pFeaClsNm == item.Key)
                        {
                            pFeaCls = pFeaClsItem;
                            break;
                        }
                    }
                    if(pFeaCls==null) continue;
                    //IFeatureClass pFeaCls = (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(item.Key.Trim());


                    //������ͼ�ڵ�(��ͼ������Ϊ�����)
                    DevComponents.AdvTree.Node pNode = new DevComponents.AdvTree.Node();
                    pNode = (DevComponents.AdvTree.Node)CreateAdvTreeNode(hook.DataTree.Nodes, item.Key, item.Key, hook.DataTree.ImageList.Images[6], true);//ͼ�����ڵ�
                    //��ʾ������
                    ShowProgressBar(true);

                    int tempValue = 0;
                    ChangeProgressBar1((hook as Plugin.Application.IAppFormRef).ProgressBar, 0, item.Value.Count, tempValue);

                   
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        string fieldName = item.Value[i].Trim();
                        int index = pFeaCls.Fields.FindField(fieldName);
                        if (index == -1)
                        {
                            //outError = new Exception("�Ҳ����ֶ���Ϊ" + fieldName + "���ֶ�");
                            //return;
                            continue;
                        }
                        if (pFeaCls.Fields.get_Field(index).IsNullable == true)
                        {
                            //�ֶ�����Ϊ�գ����׼��һ�£��������������
                            List<object> ErrorLst = new List<object>();
                            ErrorLst.Add("Ҫ�����Լ��");//����������
                            ErrorLst.Add("��ֵ���");//��������
                            ErrorLst.Add("");  //�����ļ���
                            ErrorLst.Add(enumErrorType.��ֵ���.GetHashCode());//����id;
                            ErrorLst.Add("ͼ��" + item.Key + "���ֶ�" + fieldName + "����ֵ������Ϊ��");//��������
                            ErrorLst.Add(0);    //...
                            ErrorLst.Add(0);    //...
                            ErrorLst.Add(item.Key);
                            ErrorLst.Add(-1);
                            ErrorLst.Add("");
                            ErrorLst.Add(-1);
                            ErrorLst.Add(false);
                            ErrorLst.Add(System.DateTime.Now.ToString());

                            //���ݴ�����־
                            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                            DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
                        }
                        else
                        {
                            IFeatureCursor pFeaCusor = pFeaCls.Search(null, false);
                            if (pFeaCusor == null) return;
                            IFeature pFeature = pFeaCusor.NextFeature();
                            if (pFeature == null) continue;
                            while (pFeature != null)
                            {
                                object fieldValue = pFeature.get_Value(index);
                                if (fieldValue == null || fieldValue.ToString() == "")
                                {
                                    //�ֶ�ֵ����Ϊ�գ����������������
                                    double pMapx = 0.0;
                                    double pMapy = 0.0;
                                    IPoint pPoint = new PointClass();
                                    if (pFeaCls.ShapeType != esriGeometryType.esriGeometryPoint)
                                    {
                                        pPoint = TopologyCheckClass.GetPointofFeature(pFeature);
                                    }
                                    else
                                    {
                                        pPoint = pFeature.Shape as IPoint;
                                    }
                                    pMapx = pPoint.X;
                                    pMapy = pPoint.Y;
                                    List<object> ErrorLst = new List<object>();
                                    ErrorLst.Add("Ҫ�����Լ��");//����������
                                    ErrorLst.Add("��ֵ���");//��������
                                    ErrorLst.Add("");  //�����ļ���
                                    ErrorLst.Add(enumErrorType.��ֵ���.GetHashCode());//����id;
                                    ErrorLst.Add("ͼ��" + item.Key + "���ֶ�" + fieldName + "��ֵ����Ϊ��");//��������
                                    ErrorLst.Add(pMapx);    //...
                                    ErrorLst.Add(pMapy);    //...
                                    ErrorLst.Add(item.Key);
                                    ErrorLst.Add(pFeature.OID);
                                    ErrorLst.Add("");
                                    ErrorLst.Add(-1);
                                    ErrorLst.Add(false);
                                    ErrorLst.Add(System.DateTime.Now.ToString());

                                    //���ݴ�����־
                                    IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                                    DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                                    DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
                                }
                                pFeature = pFeaCusor.NextFeature();
                            }

                            //�ͷ�cursor
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaCusor);
                        }

                        tempValue += 1;//��������ֵ��1
                        ChangeProgressBar1((hook as Plugin.Application.IAppFormRef).ProgressBar, -1, -1, tempValue);
                    }
                    //�ı���ͼ����״̬
                    ChangeTreeSelectNode(pNode, "���ͼ��" + item.Key + "�Ŀ�ֵ��飡", false);
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

       
        /// <summary>
        /// �߳����߼��Լ��
        /// </summary>
        /// <param name="hook"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="fieldName">��������ֶ���</param>
        /// <param name="feaCodeDic">Ҫ����ͷ�������ֵ��</param>
        /// <param name="minLen">����С����</param>
        /// <param name="maxLen">����󳤶�</param>
        /// <param name="outError"></param>
        public void LineLengthLogicCheck(IFeatureDataset pFeatureDataset,string fieldName, Dictionary<string, string> feaCodeDic, string  minLen, string maxLen, out Exception outError)
        {
            outError = null;
            try
            {
                //����ͼ��
                foreach (KeyValuePair<string, string> item in feaCodeDic)
                {
                    string feaClsName = item.Key;
                    IFeatureClass feaCls = (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(feaClsName);
                    if (feaCls.ShapeType != esriGeometryType.esriGeometryPolyline) continue;
                    int index = feaCls.Fields.FindField("SHAPE_Length");
                    if (index == -1)
                    {
                        index = feaCls.Fields.FindField("SHAPE_Leng");//shape�ļ�
                        if(index==-1)
                        {
                            outError = new Exception("�ֶ�SHAPE_Leng�����ڣ����飡");
                            return;
                        }
                    }

                    string code = item.Value;
                    string filterStr = "";
                    if (code != "")
                    {
                        //���������������
                        filterStr = fieldName + " in (" + code + ")";
                    }
                    IQueryFilter pFilter = new QueryFilterClass();
                    pFilter.WhereClause = filterStr;
                    IFeatureCursor pCursor = feaCls.Search(pFilter, false);
                    if (pCursor == null) return;
                    IFeature pFeature = pCursor.NextFeature();
                    if (pFeature == null) continue;
                    while (pFeature != null)
                    {
                        # region ִ�м��
                        double lineLen = Convert.ToDouble(pFeature.get_Value(index).ToString());
                        double dminLen = 0;
                        double dmaxLen = 999999999999;
                        if (minLen != "")
                        {
                            dminLen = Convert.ToDouble(minLen);
                        }
                        if (maxLen != "")
                        {
                            dmaxLen =Convert.ToDouble(maxLen);
                        }
                        if (lineLen < dminLen)
                        {
                            //�߳��Ȳ���ָ���ĳ��ȷ�Χ�ڣ�����������ʾ����
                            IPoint pPoint = TopologyCheckClass.GetPointofFeature(pFeature);
                            double pMapx = pPoint.X;
                            double pMapy = pPoint.Y;

                            List<object> ErrorLst = new List<object>();
                            ErrorLst.Add("Ҫ�����Լ��");//����������
                            ErrorLst.Add("�߳����߼��Լ��");//��������
                            ErrorLst.Add((pFeatureDataset as IDataset).Workspace.PathName);  //�����ļ���
                            ErrorLst.Add(enumErrorType.�߳����߼��Լ��.GetHashCode());//����id;
                            ErrorLst.Add("�߳��Ȳ��ڸ����ķ�Χ��");//��������
                            ErrorLst.Add(pMapx);    //...
                            ErrorLst.Add(pMapy);    //...
                            ErrorLst.Add(feaClsName);
                            ErrorLst.Add(pFeature.OID);
                            ErrorLst.Add("");
                            ErrorLst.Add(-1);
                            ErrorLst.Add(false);
                            ErrorLst.Add(System.DateTime.Now.ToString());

                            //���ݴ�����־
                            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                            DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);

                            pFeature = pCursor.NextFeature();
                            continue;
                        }
                        else if (lineLen > dmaxLen)
                        {
                            //�߳��Ȳ���ָ���ĳ��ȷ�Χ�ڣ�����������ʾ����
                            IPoint pPoint = TopologyCheckClass.GetPointofFeature(pFeature);
                            double pMapx = pPoint.X;
                            double pMapy = pPoint.Y;

                            List<object> ErrorLst = new List<object>();
                            ErrorLst.Add("Ҫ�����Լ��");//����������
                            ErrorLst.Add("�߳����߼��Լ��");//��������
                            ErrorLst.Add((pFeatureDataset as IDataset).Workspace.PathName);  //�����ļ���
                            ErrorLst.Add(enumErrorType.�߳����߼��Լ��.GetHashCode());//����id;
                            ErrorLst.Add("�߳��Ȳ��ڸ����ķ�Χ��");//��������
                            ErrorLst.Add(pMapx);    //...
                            ErrorLst.Add(pMapy);    //...
                            ErrorLst.Add(feaClsName);
                            ErrorLst.Add(pFeature.OID);
                            ErrorLst.Add("");
                            ErrorLst.Add(-1);
                            ErrorLst.Add(false);
                            ErrorLst.Add(System.DateTime.Now.ToString());

                            //���ݴ�����־
                            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                            DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
                        }
                        #endregion
                        pFeature = pCursor.NextFeature();
                    }

                    //�ͷ�cursor
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// �߳����߼��Լ��
        /// </summary>
        /// <param name="hook"></param>
        /// <param name="feaClsLst"></param>
        /// <param name="fieldName">��������ֶ���</param>
        /// <param name="feaCodeDic">Ҫ����ͷ�������ֵ��</param>
        /// <param name="minLen">����С����</param>
        /// <param name="maxLen">����󳤶�</param>
        /// <param name="outError"></param>
        public void LineLengthLogicCheck(List<IFeatureClass> feaClsLst, string fieldName, Dictionary<string, string> feaCodeDic, string minLen, string maxLen, out Exception outError)
        {
            outError = null;
            try
            {
                if (hook.DataTree == null) return;
                hook.DataTree.Nodes.Clear();
                //����������ͼ
                IntialTree(hook.DataTree);
                //�������ڵ���ɫ
                setNodeColor(hook.DataTree);
                hook.DataTree.Tag = false;

                //����ͼ��
                foreach (KeyValuePair<string, string> item in feaCodeDic)
                {
                    string feaClsName = item.Key;
                    IFeatureClass feaCls = null;// (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(feaClsName);
                    foreach(IFeatureClass mFeaCls in feaClsLst)
                    {
                        string tempFeaClsNm = (mFeaCls as IDataset).Name;
                        if(tempFeaClsNm.Contains("."))
                        {
                            tempFeaClsNm = tempFeaClsNm.Substring(tempFeaClsNm.IndexOf('.') + 1);
                        }
                        if(tempFeaClsNm==feaClsName)
                        {
                            feaCls = mFeaCls;
                            break;
                        }
                    }
                    if (feaCls == null) continue;

                    //������ͼ�ڵ�(��ͼ������Ϊ�����)
                    DevComponents.AdvTree.Node pNode = new DevComponents.AdvTree.Node();
                    pNode = (DevComponents.AdvTree.Node)CreateAdvTreeNode(hook.DataTree.Nodes, item.Key, item.Key, hook.DataTree.ImageList.Images[6], true);//ͼ�����ڵ�
                   

                    if (feaCls.ShapeType != esriGeometryType.esriGeometryPolyline) continue;
                    int index = feaCls.Fields.FindField("SHAPE_Length");
                    if (index == -1)
                    {
                        index = feaCls.Fields.FindField("SHAPE_Leng");
                        if (index == -1)
                        {
                            index = feaCls.Fields.FindField("SHAPE.LEN");
                            if (index == -1)
                            {
                                index = feaCls.Fields.FindField("shape.len");
                                outError = new Exception("�ֶ�SHAPE_Leng�����ڣ����飡");
                                return;
                            }
                        }
                    }

                    string code = item.Value;
                    string filterStr = "";
                    if (code != "")
                    {
                        //���������������
                        filterStr = fieldName + " in (" + code + ")";
                    }
                    IQueryFilter pFilter = new QueryFilterClass();
                    pFilter.WhereClause = filterStr;
                    IFeatureCursor pCursor = feaCls.Search(pFilter, false);
                    if (pCursor == null) return;
                    IFeature pFeature = pCursor.NextFeature();
                    if (pFeature == null) continue;
                    //��ʾ������
                    ShowProgressBar(true);

                    int tempValue = 0;
                    ChangeProgressBar1((hook as Plugin.Application.IAppFormRef).ProgressBar, 0, feaCls.FeatureCount(null), tempValue);

                    while (pFeature != null)
                    {
                        # region ִ�м��
                        double lineLen = Convert.ToDouble(pFeature.get_Value(index).ToString());
                        double dminLen = 0;
                        double dmaxLen = 999999999999;
                        if (minLen != "")
                        {
                            dminLen = Convert.ToDouble(minLen);
                        }
                        if (maxLen != "")
                        {
                            dmaxLen = Convert.ToDouble(maxLen);
                        }
                        if (lineLen < dminLen)
                        {
                            //�߳��Ȳ���ָ���ĳ��ȷ�Χ�ڣ�����������ʾ����
                            IPoint pPoint = TopologyCheckClass.GetPointofFeature(pFeature);
                            double pMapx = pPoint.X;
                            double pMapy = pPoint.Y;

                            List<object> ErrorLst = new List<object>();
                            ErrorLst.Add("Ҫ�����Լ��");//����������
                            ErrorLst.Add("�߳����߼��Լ��");//��������
                            ErrorLst.Add("");  //�����ļ���
                            ErrorLst.Add(enumErrorType.�߳����߼��Լ��.GetHashCode());//����id;
                            ErrorLst.Add("�߳��Ȳ��ڸ����ķ�Χ��");//��������
                            ErrorLst.Add(pMapx);    //...
                            ErrorLst.Add(pMapy);    //...
                            ErrorLst.Add(feaClsName);
                            ErrorLst.Add(pFeature.OID);
                            ErrorLst.Add("");
                            ErrorLst.Add(-1);
                            ErrorLst.Add(false);
                            ErrorLst.Add(System.DateTime.Now.ToString());

                            //���ݴ�����־
                            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                            DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);

                            pFeature = pCursor.NextFeature();
                            tempValue += 1;//��������ֵ��1
                            ChangeProgressBar1((hook as Plugin.Application.IAppFormRef).ProgressBar, -1, -1, tempValue);
                            continue;
                        }
                        else if (lineLen > dmaxLen)
                        {
                            //�߳��Ȳ���ָ���ĳ��ȷ�Χ�ڣ�����������ʾ����
                            IPoint pPoint = TopologyCheckClass.GetPointofFeature(pFeature);
                            double pMapx = pPoint.X;
                            double pMapy = pPoint.Y;

                            List<object> ErrorLst = new List<object>();
                            ErrorLst.Add("Ҫ�����Լ��");//����������
                            ErrorLst.Add("�߳����߼��Լ��");//��������
                            ErrorLst.Add("");  //�����ļ���
                            ErrorLst.Add(enumErrorType.�߳����߼��Լ��.GetHashCode());//����id;
                            ErrorLst.Add("�߳��Ȳ��ڸ����ķ�Χ��");//��������
                            ErrorLst.Add(pMapx);    //...
                            ErrorLst.Add(pMapy);    //...
                            ErrorLst.Add(feaClsName);
                            ErrorLst.Add(pFeature.OID);
                            ErrorLst.Add("");
                            ErrorLst.Add(-1);
                            ErrorLst.Add(false);
                            ErrorLst.Add(System.DateTime.Now.ToString());

                            //���ݴ�����־
                            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                            DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
                        }
                        #endregion
                        pFeature = pCursor.NextFeature();

                        tempValue += 1;//��������ֵ��1
                        ChangeProgressBar1((hook as Plugin.Application.IAppFormRef).ProgressBar, -1, -1, tempValue);
                    }
                    //�ͷ�cursor
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);

                    //�ı���ͼ����״̬
                    ChangeTreeSelectNode(pNode, "���ͼ��" + item.Key + "�߳����߼���飡", false);
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }


        /// <summary>
        /// ������߼��Լ��
        /// </summary>
        /// <param name="hook"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="fieldName">��������ֶ�</param>
        /// <param name="feaCodeDic">Ҫ����ͷ�������ֵ��</param>
        /// <param name="minArea">��С���</param>
        /// <param name="maxArea">������</param>
        /// <param name="outError"></param>
        public void AreaLogicCheck( IFeatureDataset pFeatureDataset, string fieldName, Dictionary<string, string> feaCodeDic, string minArea, string maxArea, out Exception outError)
        {
            outError = null;
            try
            {
                //����ͼ��
                foreach (KeyValuePair<string, string> item in feaCodeDic)
                {
                    string feaClsName = item.Key;
                    IFeatureClass feaCls = (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(feaClsName);
                    if (feaCls.ShapeType != esriGeometryType.esriGeometryPolygon) continue;
                    int index = feaCls.Fields.FindField("SHAPE_Area");
                    if (index == -1)
                    {
                        index = feaCls.Fields.FindField("SHAPE.AREA");
                        if (index == -1)
                        {
                            outError = new Exception("�ֶ�SHAPE.AREA�����ڣ����飡");
                            return;
                        }
                    }

                    string code = item.Value;
                    string filterStr = "";
                    if (code != "")
                    {
                        //���������������
                        filterStr = fieldName + " in (" + code + ")";
                    }
                    IQueryFilter pFilter = new QueryFilterClass();
                    pFilter.WhereClause = filterStr;
                    IFeatureCursor pCursor = feaCls.Search(pFilter, false);
                    if (pCursor == null) return;
                    IFeature pFeature = pCursor.NextFeature();
                    if (pFeature == null) continue;
                    while (pFeature != null)
                    {
                        #region ִ�м��
                        double Area = Convert.ToDouble(pFeature.get_Value(index).ToString());
                        double dminArea = 0;                   //��С���Ĭ��ֵ
                        double dmaxArea = 999999999999;        //������Ĭ��ֵ
                        if (minArea != "")
                        {
                            dminArea = Convert.ToDouble(minArea);
                        }
                        if (maxArea != "")
                        {
                            dmaxArea = Convert.ToDouble(maxArea);
                        }
                        if (Area < dminArea)
                        {
                            //���������ָ���������Χ�ڣ�����������ʾ����
                            IPoint pPoint = TopologyCheckClass.GetPointofFeature(pFeature);
                            double pMapx = pPoint.X;
                            double pMapy = pPoint.Y;

                            List<object> ErrorLst = new List<object>();
                            ErrorLst.Add("Ҫ�����Լ��");//����������
                            ErrorLst.Add("������߼��Լ��");//��������
                            ErrorLst.Add((pFeatureDataset as IDataset).Workspace.PathName);  //�����ļ���
                            ErrorLst.Add(enumErrorType.������߼��Լ��.GetHashCode());//����id;
                            ErrorLst.Add("��������ڸ����ķ�Χ��");//��������
                            ErrorLst.Add(pMapx);    //...
                            ErrorLst.Add(pMapy);    //...
                            ErrorLst.Add(feaClsName);
                            ErrorLst.Add(pFeature.OID);
                            ErrorLst.Add("");
                            ErrorLst.Add(-1);
                            ErrorLst.Add(false);
                            ErrorLst.Add(System.DateTime.Now.ToString());

                            //���ݴ�����־
                            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                            DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);

                            pFeature = pCursor.NextFeature();
                            continue;
                        }
                        else if (Area > dmaxArea)
                        {
                            //���������ָ���������Χ�ڣ�����������ʾ����
                            IPoint pPoint = TopologyCheckClass.GetPointofFeature(pFeature);
                            double pMapx = pPoint.X;
                            double pMapy = pPoint.Y;

                            List<object> ErrorLst = new List<object>();
                            ErrorLst.Add("Ҫ�����Լ��");//����������
                            ErrorLst.Add("������߼��Լ��");//��������
                            ErrorLst.Add((pFeatureDataset as IDataset).Workspace.PathName);  //�����ļ���
                            ErrorLst.Add(enumErrorType.������߼��Լ��.GetHashCode());//����id;
                            ErrorLst.Add("��������ڸ����ķ�Χ��");//��������
                            ErrorLst.Add(pMapx);    //...
                            ErrorLst.Add(pMapy);    //...
                            ErrorLst.Add(feaClsName);
                            ErrorLst.Add(pFeature.OID);
                            ErrorLst.Add("");
                            ErrorLst.Add(-1);
                            ErrorLst.Add(false);
                            ErrorLst.Add(System.DateTime.Now.ToString());

                            //���ݴ�����־
                            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                            DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
                        }

                        pFeature = pCursor.NextFeature();
                        #endregion
                    }

                    //�ͷ�cursor
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);

                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }


        /// <summary>
        /// ������߼��Լ��
        /// </summary>
        /// <param name="hook"></param>
        /// <param name="feaClsLst"></param>
        /// <param name="fieldName">��������ֶ�</param>
        /// <param name="feaCodeDic">Ҫ����ͷ�������ֵ��</param>
        /// <param name="minArea">��С���</param>
        /// <param name="maxArea">������</param>
        /// <param name="outError"></param>
        public void AreaLogicCheck(List<IFeatureClass> feaClsLst, string fieldName, Dictionary<string, string> feaCodeDic, string minArea, string maxArea, out Exception outError)
        {
            outError = null;
            try
            {
                if (hook.DataTree == null) return;
                hook.DataTree.Nodes.Clear();
                //����������ͼ
                IntialTree(hook.DataTree);
                //�������ڵ���ɫ
                setNodeColor(hook.DataTree);
                hook.DataTree.Tag = false;


                //����ͼ��
                foreach (KeyValuePair<string, string> item in feaCodeDic)
                {
                    string feaClsName = item.Key;
                    IFeatureClass feaCls=null;// = (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(feaClsName);
                    foreach (IFeatureClass mFeaCls in feaClsLst)
                    {
                        string tempName = (mFeaCls as IDataset).Name;
                        if(tempName.Contains("."))
                        {
                            tempName = tempName.Substring(tempName.IndexOf('.') + 1);
                        }
                        if (tempName == feaClsName)
                        {
                            feaCls = mFeaCls;
                            break;
                        }
                    }
                    if (feaCls == null) continue;

                    if (feaCls.ShapeType != esriGeometryType.esriGeometryPolygon) continue;
                    int index = feaCls.Fields.FindField("SHAPE_Area");
                    if (index == -1)
                    {
                        index = feaCls.Fields.FindField("SHAPE.AREA");
                        if (index == -1)
                        {
                            outError = new Exception("�ֶ�SHAPE.AREA�����ڣ����飡");
                            return;
                        }
                    }

                    string code = item.Value;
                    string filterStr = "";
                    if (code != "")
                    {
                        //���������������
                        filterStr = fieldName + " in (" + code + ")";
                    }
                    IQueryFilter pFilter = new QueryFilterClass();
                    pFilter.WhereClause = filterStr;
                    IFeatureCursor pCursor = feaCls.Search(pFilter, false);
                    if (pCursor == null) return;
                    IFeature pFeature = pCursor.NextFeature();
                    if (pFeature == null) continue;

                    //��ʾ������
                    ShowProgressBar(true);

                    int tempValue = 0;
                    ChangeProgressBar1((hook as Plugin.Application.IAppFormRef).ProgressBar, 0, feaCls.FeatureCount(null), tempValue);


                    while (pFeature != null)
                    {
                        #region ִ�м��
                        double Area = Convert.ToDouble(pFeature.get_Value(index).ToString());
                        double dminArea = 0;                   //��С���Ĭ��ֵ
                        double dmaxArea = 999999999999;        //������Ĭ��ֵ
                        if (minArea != "")
                        {
                            dminArea = Convert.ToDouble(minArea);
                        }
                        if (maxArea != "")
                        {
                            dmaxArea = Convert.ToDouble(maxArea);
                        }
                        if (Area < dminArea)
                        {
                            //���������ָ���������Χ�ڣ�����������ʾ����
                            IPoint pPoint = TopologyCheckClass.GetPointofFeature(pFeature);
                            double pMapx = pPoint.X;
                            double pMapy = pPoint.Y;

                            List<object> ErrorLst = new List<object>();
                            ErrorLst.Add("Ҫ�����Լ��");//����������
                            ErrorLst.Add("������߼��Լ��");//��������
                            ErrorLst.Add("");  //�����ļ���
                            ErrorLst.Add(enumErrorType.������߼��Լ��.GetHashCode());//����id;
                            ErrorLst.Add("��������ڸ����ķ�Χ��");//��������
                            ErrorLst.Add(pMapx);    //...
                            ErrorLst.Add(pMapy);    //...
                            ErrorLst.Add(feaClsName);
                            ErrorLst.Add(pFeature.OID);
                            ErrorLst.Add("");
                            ErrorLst.Add(-1);
                            ErrorLst.Add(false);
                            ErrorLst.Add(System.DateTime.Now.ToString());

                            //���ݴ�����־
                            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                            DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);

                            pFeature = pCursor.NextFeature();

                            tempValue += 1;//��������ֵ��1
                            ChangeProgressBar1((hook as Plugin.Application.IAppFormRef).ProgressBar, -1, -1, tempValue);
                            continue;
                        }
                        else if (Area > dmaxArea)
                        {
                            //���������ָ���������Χ�ڣ�����������ʾ����
                            IPoint pPoint = TopologyCheckClass.GetPointofFeature(pFeature);
                            double pMapx = pPoint.X;
                            double pMapy = pPoint.Y;

                            List<object> ErrorLst = new List<object>();
                            ErrorLst.Add("Ҫ�����Լ��");//����������
                            ErrorLst.Add("������߼��Լ��");//��������
                            ErrorLst.Add("");  //�����ļ���
                            ErrorLst.Add(enumErrorType.������߼��Լ��.GetHashCode());//����id;
                            ErrorLst.Add("��������ڸ����ķ�Χ��");//��������
                            ErrorLst.Add(pMapx);    //...
                            ErrorLst.Add(pMapy);    //...
                            ErrorLst.Add(feaClsName);
                            ErrorLst.Add(pFeature.OID);
                            ErrorLst.Add("");
                            ErrorLst.Add(-1);
                            ErrorLst.Add(false);
                            ErrorLst.Add(System.DateTime.Now.ToString());

                            //���ݴ�����־
                            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                            DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
                        }

                        pFeature = pCursor.NextFeature();
                        #endregion

                        tempValue += 1;//��������ֵ��1
                        ChangeProgressBar1((hook as Plugin.Application.IAppFormRef).ProgressBar, -1, -1, tempValue);
                    }

                    //�ͷ�cursor
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
                }
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }



        /// <summary>
        /// �ȸ��߸߳�ֵ��飬��������
        /// </summary>
        /// <param name="pFeatureDataset">���ݼ�</param>
        /// <param name="feaClsName">ͼ����</param>
        /// <param name="contourFiledName">�߳��ֶ���</param>
        /// <param name="elevMin">�߳���Сֵ</param>
        /// <param name="elevMax">�߳����ֵ</param>
        /// <param name="intevalValue">�̼߳��ֵ</param>
        /// <param name="outError"></param>
        public void contourIntevalCheck(IFeatureDataset pFeatureDataset, string feaClsName, string contourFiledName,string elevMin,string elevMax, double intevalValue, out Exception outError)
        {
            outError = null;
            try
            {
                IFeatureClass pFeaCls = (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(feaClsName);
                if (pFeaCls.ShapeType != esriGeometryType.esriGeometryPolyline)
                {
                    outError = new Exception("��ͼ�㲻����ͼ�㣬���顣ͼ����Ϊ:" + feaClsName);
                    return;
                }

                //�߳��ֶε�����ֵ
                int index = pFeaCls.Fields.FindField(contourFiledName);
                if (index == -1)
                {
                    outError = new Exception("�߳��ֶ���" + contourFiledName + "�����ڣ�");
                    return;
                }
                //���Ҫ�صĸ߳�ֵ�Ƿ����ƶ��ĸ̷߳�Χ��
                ElevationValueCheck(pFeaCls, index, pFeatureDataset, feaClsName, elevMin, elevMax, out outError);
                if (outError != null) return;

                //���ȸ��ߵĸ̼߳���Ƿ�һ��
                //������ѯ������
                //IQueryFilter pFilter = new QueryFilterClass();
                //pFilter.WhereClause = "";
                //IQueryFilterDefinition pFilterDef = pFilter as IQueryFilterDefinition;
                //pFilterDef.PostfixClause = "ORDER BY " + contourFiledName;
                //IFeatureCursor pCursor = pFeaCls.Search(pFilter, true);
                ITableSort pTableSort = new TableSortClass();
                pTableSort.Fields = contourFiledName;
                pTableSort.set_Ascending(contourFiledName, true);
                pTableSort.set_CaseSensitive(contourFiledName, true);
                pTableSort.QueryFilter = null;
                pTableSort.Table = pFeaCls as ITable;

                //��������
                pTableSort.Sort(null);
                ICursor pCursor = pTableSort.Rows;
                if (pCursor == null) return;
                IFeatureCursor pFeaCursor = pCursor as IFeatureCursor;
                IFeature pFeature = pFeaCursor.NextFeature();
                double contValue = -1;
                IFeature oriFeature = null;
                while (pFeature != null)
                {
                    if (contValue == -1)
                    {
                        contValue = Convert.ToDouble(pFeature.get_Value(index).ToString());
                        oriFeature = pFeature;
                    }
                    else
                    {
                        double tempValue = contValue;
                        IFeature tempFeature = oriFeature;
                        contValue = Convert.ToDouble(pFeature.get_Value(index).ToString());
                        oriFeature = pFeature;
                        if (contValue - tempValue != intevalValue)
                        {
                            double errInterval = contValue - tempValue;
                            //�ȸ��߸߳�ֵ��಻�ȣ����������������
                            IPoint pPoint = TopologyCheckClass.GetPointofFeature(tempFeature);
                            double pMapx = pPoint.X;
                            double pMapy = pPoint.Y;

                            List<object> ErrorLst = new List<object>();
                            ErrorLst.Add("Ҫ�����Լ��");//����������
                            ErrorLst.Add("�ȸ��߸̼߳����");//��������
                            ErrorLst.Add((pFeatureDataset as IDataset).Workspace.PathName);  //�����ļ���
                            ErrorLst.Add(enumErrorType.�ȸ��߸߳�ֵ���.GetHashCode());//����id;
                            ErrorLst.Add("ͼ��" + feaClsName + "�̼߳��ֵì��,���ֵΪ:"+errInterval.ToString());//��������
                            ErrorLst.Add(pMapx);    //...
                            ErrorLst.Add(pMapy);    //...
                            ErrorLst.Add(feaClsName);
                            ErrorLst.Add(tempFeature.OID);
                            ErrorLst.Add(feaClsName);
                            ErrorLst.Add(pFeature.OID);
                            ErrorLst.Add(false);
                            ErrorLst.Add(System.DateTime.Now.ToString());

                            //���ݴ�����־
                            IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                            DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                            DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
                        }
                    }
                    pFeature = pFeaCursor.NextFeature();
                }

                //�ͷ�cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaCursor);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// ���߲�ȸ���
        /// </summary>
        /// <param name="pFeatureCls">�ȸ���Ҫ����</param>
        /// <param name="contourFieldName">�߳��ֶ���</param>
        /// <param name="pPolyLine">����</param>
        /// <param name="intervalValue">�̼߳��</param>
        /// <param name="orient">�ȸ��߷��򣺵������ݼ�</param>
        /// <param name="eError"></param>
        public void LineIntervalCheck(IFeatureClass pFeatureCls, string contourFieldName, IPolyline pPolyLine, double intervalValue, string orient, out Exception eError)
        {
            eError = null;
            try
            {
                //�����pPolyLine�������յ�
                IPointCollection pointCol = pPolyLine as IPointCollection;
                IPoint firstPoint = pointCol.get_Point(0);     //���
                IPoint lastPoint = pointCol.get_Point(pointCol.PointCount - 1);      //�յ�
                double firstX = firstPoint.X;                    //���X����
                double firstY = firstPoint.Y;                    //���Y����
                double lastX = lastPoint.X;                      //�յ�X����
                double lastY = lastPoint.Y;                      //�յ�Y����

                #region ����Ҫ���࣬������pPolyLine�ཻ�ĵ��Ҫ�ر�������
                Dictionary<double, IFeature> feaDic = new Dictionary<double, IFeature>();
                List<double> pointXLst = new List<double>();         //�����ཻ���X����
                IFeatureCursor pCusor = pFeatureCls.Search(null, false);
                if (pCusor == null) return;
                IFeature pFeature = pCusor.NextFeature();
                while (pFeature != null)
                {
                    IRelationalOperator pRelOper = pPolyLine as IRelationalOperator;
                    if (!pRelOper.Disjoint(pFeature.Shape))
                    {
                        //�ཻ
                        ITopologicalOperator pTopoOper = pFeature.Shape as ITopologicalOperator;
                        IGeometry pGeo = pTopoOper.Intersect(pPolyLine as IGeometry, esriGeometryDimension.esriGeometry0Dimension);
                        IPoint pTemp = null;      //������ȸ���ͼ��Ҫ�صĽ���
                        if(pGeo.GeometryType==esriGeometryType.esriGeometryMultipoint)
                        {
                            //��㣬ȡ����һ����
                            IPointCollection mPpointCol = pGeo as IPointCollection;
                            if (mPpointCol != null)
                            {
                                pTemp = mPpointCol.get_Point(0);
                            }
                        }
                        else if (pGeo.GeometryType == esriGeometryType.esriGeometryPoint)
                        {
                            //����
                            pTemp = pGeo as IPoint;
                        }
                        if (pTemp == null) return;
                        double tempX = pTemp.X;
                        if (!feaDic.ContainsKey(tempX))
                        {
                            feaDic.Add(tempX, pFeature);
                        }
                        if (!pointXLst.Contains(tempX))
                        {
                            pointXLst.Add(tempX);
                        }
                    }
                    pFeature = pCusor.NextFeature();
                }

                //�ͷ�cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCusor);

                #endregion
                if (feaDic.Count == 0)
                {
                    eError = new Exception("������ȸ��߲����ཻ��");
                    return;
                }

                //���Ҫ����ĸ߳��ֶ�����
                int index = pFeatureCls.Fields.FindField(contourFieldName);
                if (index == -1)
                {
                    eError = new Exception("�Ҳ����߳��ֶΣ��ֶ���Ϊ��" + contourFieldName);
                    return;
                }

                #region ������㵽�յ�ķ������洢�ཻ��
                //������㵽�յ�ķ��򽫸���Ҫ�صĵ������������,����������
                if (firstX < lastX)
                {
                    //���С����
                    //���ཻ���X������д�С���������
                    SortList(pointXLst);
                }
                else if (firstX > lastX)
                {
                    //��Ӵ�С
                    //���ཻ���X������дӴ�С������
                    SortList2(pointXLst);
                }
                else
                {
                    if (firstY < lastY)
                    {
                        //���С����
                        //���ཻ���Y������д�С���������
                        SortList(pointXLst);
                    }
                    else if (firstY > lastY)
                    {
                        //��Ӵ�С
                        //���ཻ���Y������дӴ�С������
                        SortList2(pointXLst);
                    }
                    else
                    {
                        eError = new Exception("�뻮ֱ�߼���Ƿ���ڵȸ��߸߳�ֵ�쳣��");
                        return;
                    }
                }

                #endregion

                //�Ը߳�ֵ���м��
                //double tempValue = 0.0;
                double firstValue=0.0;
                //IFeature tempFea = null;
                for (int i = 0; i < pointXLst.Count; i++)
                {
                    double pointX = pointXLst[i];
                    IFeature mFea = feaDic[pointX];
                    //Ҫ�صĸ߳�ֵ 
                    string pValueStr = mFea.get_Value(index).ToString().Trim();
                    if (pValueStr == "")
                    {
                        eError = new Exception("�߳�ֵΪ�գ�");
                        return;
                    }
                    double pValue = Convert.ToDouble(pValueStr);
                    if (i == 0)
                    {
                        //��һ����ĸ߳�
                        firstValue = pValue;
                        continue;
                    }
                    //if (tempValue != 0.0 || tempFea != null)
                    //{
                    if (orient == "����")
                    {
                        if (pValue != firstValue + intervalValue * i)
                        {
                            //����,tempFea,mFea
                            GetErrorList(pFeatureCls, mFea, null, null, out eError);
                            if (eError != null)
                            {
                                return;
                            }
                        }
                    }
                    else if (orient == "�ݼ�")
                    {
                        if (pValue != firstValue - intervalValue * i)
                        {
                            //����,tempFea,mFea
                            GetErrorList(pFeatureCls, mFea, null, null, out eError);
                            if (eError != null)
                            {
                                return;
                            }
                        }
                    }
                    //}
                    //tempValue = pValue;
                    //tempFea = mFea;
                }
            }
            catch (System.Exception ex)
            {
                eError = ex;
            }
        }

        //���б���д�С��������
        private void SortList(List<double> pointLst)
        {
            for(int i=0;i<pointLst.Count-1 ;i++)
            {
                for(int j=i+1;j<pointLst.Count;j++)
                {
                    if(pointLst[i]>pointLst[j])
                    {
                        //ChangeValue(pointLst[i], pointLst[j]);
                        double tempV = pointLst[i];
                        pointLst[i] = pointLst[j];
                        pointLst[j] = tempV;
                    }
                }
            }
        }
        //���б���дӴ�С����
        private void SortList2(List<double> pointLst)
        {
            for (int i = 0; i < pointLst.Count - 1; i++)
            {
                for (int j =i+ 1; j < pointLst.Count; j++)
                {
                    if (pointLst[i] < pointLst[j])
                    {
                        //ChangeValue(pointLst[i],pointLst[j]);
                        double tempV = pointLst[i];
                        pointLst[i] = pointLst[j];
                        pointLst[j]=tempV;
                    }
                }
            }
        }
        //��������
       private void ChangeValue(double v1,double v2)
       {
           double temp = 0.0;
           temp = v1;
           v1=v2;
           v2=temp;
       }
        //�ȸ��߸߳�ֵ�������б�
        private void GetErrorList(IFeatureClass pOriFeatureCls, IFeature pOriFeature, IFeatureClass pDesFeatureCls, IFeature pDesFeature, out Exception eError)
        {
            eError = null;
            try
            {
                int eErrorID = enumErrorType.�ȸ��߸߳�ֵ���.GetHashCode();
                string eErrorDes = "�ȸ��߸߳�ֵ����ȷ";
                double pMapx = 0.0;
                double pMapy = 0.0;
                IPoint pPoint = null;
                //�߳�ֵ���ڸ����ĸ̷߳�Χ��,����������������
                pPoint = TopologyCheckClass.GetPointofFeature(pOriFeature);

                if(pPoint!=null)
                {
                    pMapx = pPoint.X;
                    pMapy = pPoint.Y;
                }

                List<object> ErrorLst = new List<object>();
                ErrorLst.Add("Ҫ�����Լ��");//����������
                ErrorLst.Add("�ȸ��߸߳�ֵ���");//��������
                ErrorLst.Add("");  //�����ļ���
                ErrorLst.Add(eErrorID);//����id;
                ErrorLst.Add(eErrorDes);//��������
                ErrorLst.Add(pMapx);    //...
                ErrorLst.Add(pMapy);    //...
                ErrorLst.Add((pOriFeatureCls as IDataset).Name);
                ErrorLst.Add(pOriFeature.OID);
                if (pDesFeatureCls != null)
                {
                    ErrorLst.Add((pDesFeatureCls as IDataset).Name);
                }
                else
                {
                    ErrorLst.Add("");
                }
                if (pDesFeature != null)
                {
                    ErrorLst.Add(pDesFeature.OID);
                }
                else
                {
                    ErrorLst.Add(-1);
                }
               
                ErrorLst.Add(false);
                ErrorLst.Add(System.DateTime.Now.ToString());

                //���ݴ�����־
                IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
            }
            catch (Exception ex)
            {
                eError = ex;
            }
        }

        /// <summary>
        /// �߳�ֵ���
        /// </summary>
        /// <param name="pFeatureDataset"></param>
        /// <param name="feaClsName"></param>
        /// <param name="elevMin">��С�߳�</param>
        /// <param name="elevMax">���߳�</param>
        /// <param name="outError"></param>
        public void CoutourValueCheck(IFeatureDataset pFeatureDataset, string feaClsName,string contourFiledName, string elevMin, string elevMax, out Exception outError)
        {
            outError = null;
            try
            {
                IFeatureClass pFeaCls = (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(feaClsName);
                //if (pFeaCls.ShapeType != esriGeometryType.esriGeometryPolyline)
                //{
                //    outError = new Exception("��ͼ�㲻����ͼ�㣬���顣ͼ����Ϊ:" + feaClsName);
                //    return;
                //}

                //�߳��ֶε�����ֵ
                int index = pFeaCls.Fields.FindField(contourFiledName);
                if (index == -1)
                {
                    outError = new Exception("�߳��ֶ���" + contourFiledName + "�����ڣ�");
                    return;
                }
                //���Ҫ�صĸ߳�ֵ�Ƿ���ָ���ĸ̷߳�Χ��
                ElevationValueCheck(pFeaCls, index, pFeatureDataset, feaClsName, elevMin, elevMax, out outError);
                if (outError != null) return;
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// �߳�ֵ���
        /// </summary>
        /// <param name="pFeatureDataset"></param>
        /// <param name="feaClsName"></param>
        /// <param name="elevMin">��С�߳�</param>
        /// <param name="elevMax">���߳�</param>
        /// <param name="outError"></param>
        public void CoutourValueCheck(List<IFeatureClass> feaClsLst, string feaClsName, string contourFiledName, string elevMin, string elevMax, out Exception outError)
        {
            outError = null;
            try
            {
                IFeatureClass pFeaCls = null;// = (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(feaClsName);
                foreach (IFeatureClass mFeaCls in feaClsLst)
                {
                    string tempName = (mFeaCls as IDataset).Name;
                    if(tempName.Contains("."))
                    {
                        tempName = tempName.Substring(tempName.IndexOf('.') + 1);
                    }
                    if (tempName == feaClsName)
                    {
                        pFeaCls = mFeaCls;
                        break;
                    }
                }
                if (pFeaCls == null) return;
                //if (pFeaCls.ShapeType != esriGeometryType.esriGeometryPolyline)
                //{
                //    outError = new Exception("��ͼ�㲻����ͼ�㣬���顣ͼ����Ϊ:" + feaClsName);
                //    return;
                //}

                //�߳��ֶε�����ֵ
                int index = pFeaCls.Fields.FindField(contourFiledName);
                if (index == -1)
                {
                    outError = new Exception("�߳��ֶ���" + contourFiledName + "�����ڣ�");
                    return;
                }
                //���Ҫ�صĸ߳�ֵ�Ƿ���ָ���ĸ̷߳�Χ��
                ElevationValueCheck(pFeaCls, index, null, feaClsName, elevMin, elevMax, out outError);
                if (outError != null) return;
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// �쳣�߳�ֵ���
        /// </summary>
        /// <param name="pFeaCls">ͼ��</param>
        /// <param name="index">�߳��ֶ�����</param>
        /// <param name="pFeatureDataset">���ݼ�</param>
        /// <param name="feaClsName">ͼ����</param>
        /// <param name="elevMin">��С�߳�ֵ</param>
        /// <param name="elevMax">���߳�ֵ</param>
        /// <param name="outError"></param>
        private void ElevationValueCheck(IFeatureClass pFeaCls, int index, IFeatureDataset pFeatureDataset, string feaClsName, string elevMin, string elevMax, out Exception outError)
        {
            outError = null;
            try
            {
                IFeatureCursor pCursor = pFeaCls.Search(null, false);
                if (pCursor == null) return;
                IFeature pFeature = pCursor.NextFeature();
                if (pFeature == null) return;

                //��ʾ������
                ShowProgressBar(true);

                int tempValue = 0;
                ChangeProgressBar1((hook as Plugin.Application.IAppFormRef).ProgressBar, 0, pFeaCls.FeatureCount(null), tempValue);

                while (pFeature != null)
                {
                    double fieldValue = Convert.ToDouble(pFeature.get_Value(index).ToString());
                    double dminElev = 0;                   //��С�߳�ֵ
                    double dmaxElev = 999999999999;        //���߳�ֵ
                    if (elevMin != "")
                    {
                        dminElev = Convert.ToDouble(elevMin);
                    }
                    if (elevMax != "")
                    {
                        dmaxElev = Convert.ToDouble(elevMax);
                    }
                    //���ȼ�����и߳�ֵ�Ƿ����ƶ��ķ�Χ��
                    if (fieldValue < dminElev)
                    {
                        ElevCheckErrShow(pFeatureDataset, feaClsName, pFeature, out outError);
                        if (outError != null)
                        {
                            pFeature = pCursor.NextFeature();
                            continue;
                        }
                        pFeature = pCursor.NextFeature();
                        continue;
                    }
                    else if (fieldValue > dmaxElev)
                    {
                        ElevCheckErrShow(pFeatureDataset, feaClsName, pFeature, out outError);
                        if (outError != null)
                        {
                            pFeature = pCursor.NextFeature();
                            continue;
                        }
                    }
                    pFeature = pCursor.NextFeature();

                    tempValue += 1;//��������ֵ��1
                    ChangeProgressBar1((hook as Plugin.Application.IAppFormRef).ProgressBar, -1, -1, tempValue);
                }

                //�ͷ�cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// �ȸ��߸̼߳���������Ϣ
        /// </summary>
        /// <param name="pFeatureDataset"></param>
        /// <param name="feaClsname"></param>
        /// <param name="pFeature"></param>
        /// <param name="eError"></param>
        private void ElevCheckErrShow(IFeatureDataset pFeatureDataset, string feaClsname, IFeature pFeature,out Exception eError)
        {
            eError = null;
            try
            {
                //�߳�ֵ���ڸ����ĸ̷߳�Χ��,����������������
                IPoint pPoint = null;
                double pMapx = 0.0;
                double pMapy = 0.0;
                if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
                {
                    pPoint = pFeature.Shape as IPoint;
                }
                else
                {
                    pPoint = TopologyCheckClass.GetPointofFeature(pFeature);
                }
                if (pPoint != null)
                {
                    pMapx = pPoint.X;
                    pMapy = pPoint.Y;
                }

                List<object> ErrorLst = new List<object>();
                ErrorLst.Add("Ҫ�����Լ��");//����������
                ErrorLst.Add("�쳣�߳�ֵ���");//��������
                if (pFeatureDataset == null)
                {
                    ErrorLst.Add("");  //�����ļ���
                }
                else
                {
                    ErrorLst.Add((pFeatureDataset as IDataset).Workspace.PathName);  //�����ļ���
                }

                ErrorLst.Add(enumErrorType.�߳�ֵ���.GetHashCode());//����id;
                ErrorLst.Add("ͼ��" + feaClsname + "�߳�ֵ���ڸ����ķ�Χ��");//��������
                ErrorLst.Add(pMapx);    //...
                ErrorLst.Add(pMapy);    //...
                ErrorLst.Add(feaClsname);
                ErrorLst.Add(pFeature.OID);
                ErrorLst.Add("");
                ErrorLst.Add(-1);
                ErrorLst.Add(false);
                ErrorLst.Add(System.DateTime.Now.ToString());

                //���ݴ�����־
                IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
            }
            catch (Exception ex)
            {
                eError = ex;
            }
        }


        /// <summary>
        /// ע�Ǹ߳�ֵһ���Լ��
        /// </summary>
        /// <param name="hook"></param>
        /// <param name="pFeatureDataset"></param>
        /// <param name="codeName">��������ֶ���</param>
        /// <param name="oriFeaClsName">ԴҪ��������</param>
        /// <param name="oriCodeValue">ԴҪ����������ֵ</param>
        /// <param name="oriElevFieldName">Դ�߳�ֵ�ֶ���</param>
        /// <param name="desFeaClsName">Ŀ��Ҫ��������</param>
        /// <param name="desCodeValue">Ŀ��Ҫ����������ֵ</param>
        /// <param name="labelFieldName">Ŀ��Ҫ����߳�ֵ</param>
        /// <param name="radius">�����뾶</param>
        /// <param name="precision">���ȿ���</param>
        /// <param name="outError"></param>
        public void ElevAccordanceCheck(IFeatureDataset pFeatureDataset, string codeName, string oriFeaClsName, string oriCodeValue, string oriElevFieldName, string desFeaClsName, string desCodeValue, string labelFieldName, double radius, long precision, out Exception outError)
        {
            outError = null;
            try
            {
                //ԴҪ����
                IFeatureClass pOriFeaCls = (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(oriFeaClsName);
                //ԴҪ����߳��ֶ�����ֵ
                int oriEleIndex = pOriFeaCls.Fields.FindField(oriElevFieldName);
                if (oriEleIndex == -1)
                {
                    outError = new Exception("Ҫ����" + oriFeaClsName + "�ֶ�" + oriElevFieldName + "�����ڣ�");
                    return;
                }

                //Ŀ��Ҫ����
                IFeatureClass pDesFeaCls = (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(desFeaClsName);
                //Ŀ��Ҫ����߳��ֶ�����ֵ
                int desElevIndex = pDesFeaCls.Fields.FindField(labelFieldName);
                if (desElevIndex == -1)
                {
                    outError = new Exception("Ҫ����" + desFeaClsName + "�ֶ�" + labelFieldName + "�����ڣ�");
                    return;
                }

                //����ԴҪ�����з��Ϸ����������������Ҫ��
                string whereStr = "";
                if (oriCodeValue != "")
                {
                    whereStr = codeName + " ='" + oriCodeValue + "'";
                }
                IQueryFilter pFilter = new QueryFilterClass();
                pFilter.WhereClause = whereStr;
                IFeatureCursor pCursor = pOriFeaCls.Search(pFilter, false);
                if (pCursor == null) return;
                IFeature pOriFea = pCursor.NextFeature();

                //����ԴҪ�أ����бȽ�
                while (pOriFea != null)
                {
                    #region ���м��
                    string oriElevValue = pOriFea.get_Value(oriEleIndex).ToString();
                    //����ԭ�߳�ֵ�����������ĸ߳�ֵ���ڱȽ�
                    if (oriElevValue.Contains("."))
                    {
                        //ԭ�߳�ֵ����С����
                        int oriDotIndex = oriElevValue.IndexOf('.');
                        if (precision == 0)
                        {
                            oriElevValue = oriElevValue.Substring(0, oriDotIndex);
                        }
                        else if (oriElevValue.Substring(oriDotIndex + 1).Length > precision && precision > 0)
                        {
                            //ԭ�߳�ֵ��С����λ�����ھ��ȿ���
                            int oriLen = oriDotIndex + 1 +Convert.ToInt32(precision);
                            oriElevValue = oriElevValue.Substring(0, oriLen);
                        }
                    }

                    IFeature desFeature = GetNearestFeature(pDesFeaCls, codeName, desCodeValue, pOriFea, radius, out outError);
                    if (outError != null) return;
                    if (desFeature == null)
                    {
                        pOriFea = pCursor.NextFeature();
                        continue;
                    }
                    string desElevValue = desFeature.get_Value(desElevIndex).ToString();
                    if (desElevValue.Contains("."))
                    {
                        //Ŀ��߳�ֵ����С����
                        int desDotIndex = desElevValue.IndexOf('.');
                        if (precision == 0)
                        {
                            desElevValue = desElevValue.Substring(0, desDotIndex);
                        }
                        else if (desElevValue.Substring(desDotIndex + 1).Length > precision && precision > 0)
                        {
                            //Ŀ��߳�ֵ��С����λ�����ھ���
                            int desLen = desDotIndex + 1 +Convert.ToInt32(precision);
                            desElevValue = desElevValue.Substring(0, desLen);
                        }
                    }

                    //���ݾ��Ƚ��бȽϣ�������ķ�Χ�ڲ���ͬ�������
                    if (Convert.ToDouble(oriElevValue) !=Convert.ToDouble(desElevValue))
                    {
                        //˵���㣬��������Ӧע�ǵĸ߳�ֵ��һ�¡�����������ʾ����
                        double pMapx = 0.0;
                        double pMapy = 0.0;
                        IPoint pPoint = new PointClass();
                        if (pOriFeaCls.ShapeType != esriGeometryType.esriGeometryPoint)
                        {
                            pPoint = TopologyCheckClass.GetPointofFeature(pOriFea);
                        }
                        else  
                        {
                            //��Ҫ����
                            pPoint = pOriFea.Shape as IPoint;
                        }
                        pMapx = pPoint.X;
                        pMapy = pPoint.Y;

                        List<object> ErrorLst = new List<object>();
                        ErrorLst.Add("Ҫ�����Լ��");//����������
                        ErrorLst.Add("����ע�Ǹ߳�ֵһ���Լ��");//��������
                        ErrorLst.Add((pFeatureDataset as IDataset).Workspace.PathName);  //�����ļ���
                        ErrorLst.Add(0);//����id;
                        ErrorLst.Add("ͼ��" + oriFeaClsName + "��ͼ��" + desFeaClsName + "�߳�ֵ��һ��");//��������
                        ErrorLst.Add(pMapx);    //...
                        ErrorLst.Add(pMapy);    //...
                        ErrorLst.Add(oriFeaClsName);
                        ErrorLst.Add(pOriFea.OID);
                        ErrorLst.Add(desFeaClsName);
                        ErrorLst.Add(desFeature.OID);
                        ErrorLst.Add(false);
                        ErrorLst.Add(System.DateTime.Now.ToString());

                        //���ݴ�����־
                        IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                        DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                        DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);

                    }
                    pOriFea = pCursor.NextFeature();
                    #endregion
                }

                //�ͷ�cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }

        /// <summary>
        /// ע�Ǹ߳�ֵһ���Լ��
        /// </summary>
        /// <param name="hook"></param>
        /// <param name="feaClsLst"></param>
        /// <param name="codeName">��������ֶ���</param>
        /// <param name="oriFeaClsName">ԴҪ��������</param>
        /// <param name="oriCodeValue">ԴҪ����������ֵ</param>
        /// <param name="oriElevFieldName">Դ�߳�ֵ�ֶ���</param>
        /// <param name="desFeaClsName">Ŀ��Ҫ��������</param>
        /// <param name="desCodeValue">Ŀ��Ҫ����������ֵ</param>
        /// <param name="labelFieldName">Ŀ��Ҫ����߳�ֵ</param>
        /// <param name="radius">�����뾶</param>
        /// <param name="precision">���ȿ���</param>
        /// <param name="outError"></param>
        public void ElevAccordanceCheck(List<IFeatureClass> feaClsLst, string codeName, string oriFeaClsName, string oriCodeValue, string oriElevFieldName, string desFeaClsName, string desCodeValue, string labelFieldName, double radius, long precision, out Exception outError)
        {
            outError = null;
            try
            {
                //ԴҪ����
                IFeatureClass pOriFeaCls = null;// (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(oriFeaClsName);
                foreach (IFeatureClass mFeaCls in feaClsLst)
                {
                    string tempNm = (mFeaCls as IDataset).Name;
                    if(tempNm.Contains("."))
                    {
                        tempNm = tempNm.Substring(tempNm.IndexOf('.') + 1);
                    }
                    if (tempNm == oriFeaClsName)
                    {
                        
                        pOriFeaCls = mFeaCls;
                        break;
                    }
                }
                if (pOriFeaCls == null) return;
                //ԴҪ����߳��ֶ�����ֵ
                int oriEleIndex = pOriFeaCls.Fields.FindField(oriElevFieldName);
                if (oriEleIndex == -1)
                {
                    outError = new Exception("Ҫ����" + oriFeaClsName + "�ֶ�" + oriElevFieldName + "�����ڣ�");
                    return;
                }

                //Ŀ��Ҫ����
                IFeatureClass pDesFeaCls = null;// (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(desFeaClsName);
                foreach (IFeatureClass mFeaCls in feaClsLst)
                {
                    string tempNm = (mFeaCls as IDataset).Name;
                    if (tempNm.Contains("."))
                    {
                        tempNm = tempNm.Substring(tempNm.IndexOf('.') + 1);
                    }
                    if (tempNm== desFeaClsName)
                    {
                        pDesFeaCls = mFeaCls;
                        break;
                    }
                }
                if (pDesFeaCls == null) return;
                //Ŀ��Ҫ����߳��ֶ�����ֵ
                int desElevIndex = pDesFeaCls.Fields.FindField(labelFieldName);
                if (desElevIndex == -1)
                {
                    outError = new Exception("Ҫ����" + desFeaClsName + "�ֶ�" + labelFieldName + "�����ڣ�");
                    return;
                }

                //����ԴҪ�����з��Ϸ����������������Ҫ��
                string whereStr = "";
                if (oriCodeValue != "")
                {
                    whereStr = codeName + " ='" + oriCodeValue + "'";
                }
                IQueryFilter pFilter = new QueryFilterClass();
                pFilter.WhereClause = whereStr;
                IFeatureCursor pCursor = pOriFeaCls.Search(pFilter, false);
                if (pCursor == null) return;
                IFeature pOriFea = pCursor.NextFeature();

                //��ʾ������
                ShowProgressBar(true);

                int tempValue = 0;
                ChangeProgressBar1((hook as Plugin.Application.IAppFormRef).ProgressBar, 0, pOriFeaCls.FeatureCount(null), tempValue);

                //����ԴҪ�أ����бȽ�
                while (pOriFea != null)
                {
                    #region ���м��
                    string oriElevValue = pOriFea.get_Value(oriEleIndex).ToString();
                    //����ԭ�߳�ֵ�����������ĸ߳�ֵ���ڱȽ�
                    if (oriElevValue.Contains("."))
                    {
                        //ԭ�߳�ֵ����С����
                        int oriDotIndex = oriElevValue.IndexOf('.');
                        if (precision == 0)
                        {
                            oriElevValue = oriElevValue.Substring(0, oriDotIndex);
                        }
                        else if (oriElevValue.Substring(oriDotIndex + 1).Length > precision && precision > 0)
                        {
                            //ԭ�߳�ֵ��С����λ�����ھ��ȿ���
                            int oriLen = oriDotIndex + 1 + Convert.ToInt32(precision);
                            oriElevValue = oriElevValue.Substring(0, oriLen);
                        }
                    }

                    IFeature desFeature = GetNearestFeature(pDesFeaCls, codeName, desCodeValue, pOriFea, radius, out outError);
                    if (outError != null) return;
                    if (desFeature == null)
                    {
                        pOriFea = pCursor.NextFeature();
                        continue;
                    }
                    string desElevValue = desFeature.get_Value(desElevIndex).ToString();
                    if (desElevValue.Contains("."))
                    {
                        //Ŀ��߳�ֵ����С����
                        int desDotIndex = desElevValue.IndexOf('.');
                        if (precision == 0)
                        {
                            desElevValue = desElevValue.Substring(0, desDotIndex);
                        }
                        else if (desElevValue.Substring(desDotIndex + 1).Length > precision && precision > 0)
                        {
                            //Ŀ��߳�ֵ��С����λ�����ھ���
                            int desLen = desDotIndex + 1 + Convert.ToInt32(precision);
                            desElevValue = desElevValue.Substring(0, desLen);
                        }
                    }

                    //���ݾ��Ƚ��бȽϣ�������ķ�Χ�ڲ���ͬ�������
                    if (Convert.ToDouble(oriElevValue) != Convert.ToDouble(desElevValue))
                    {
                        //˵���㣬��������Ӧע�ǵĸ߳�ֵ��һ�¡�����������ʾ����
                        double pMapx = 0.0;
                        double pMapy = 0.0;
                        IPoint pPoint = new PointClass();
                        if (pOriFeaCls.ShapeType != esriGeometryType.esriGeometryPoint)
                        {
                            pPoint = TopologyCheckClass.GetPointofFeature(pOriFea);
                        }
                        else
                        {
                            //��Ҫ����
                            pPoint = pOriFea.Shape as IPoint;
                        }
                        pMapx = pPoint.X;
                        pMapy = pPoint.Y;

                        List<object> ErrorLst = new List<object>();
                        ErrorLst.Add("Ҫ�����Լ��");//����������
                        ErrorLst.Add("����ע�Ǹ߳�ֵһ���Լ��");//��������
                        ErrorLst.Add("");  //�����ļ���
                        ErrorLst.Add(0);//����id;
                        ErrorLst.Add("ͼ��" + oriFeaClsName + "��ͼ��" + desFeaClsName + "�߳�ֵ��һ��");//��������
                        ErrorLst.Add(pMapx);    //...
                        ErrorLst.Add(pMapy);    //...
                        ErrorLst.Add(oriFeaClsName);
                        ErrorLst.Add(pOriFea.OID);
                        ErrorLst.Add(desFeaClsName);
                        ErrorLst.Add(desFeature.OID);
                        ErrorLst.Add(false);
                        ErrorLst.Add(System.DateTime.Now.ToString());

                        //���ݴ�����־
                        IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                        DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                        DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);

                    }
                    pOriFea = pCursor.NextFeature();
                    #endregion


                    tempValue += 1;//��������ֵ��1
                    ChangeProgressBar1((hook as Plugin.Application.IAppFormRef).ProgressBar, -1, -1, tempValue);
                }

                //�ͷ�cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
            }
            catch (Exception ex)
            {
                outError = ex;
            }
        }


        /// <summary>
        /// �ȸ��ߵ���ì��һ���Լ��
        /// </summary>
        /// <param name="pFeatureDataset"></param>
        /// <param name="lineFeaClsName">�ȸ���Ҫ��������</param>
        /// <param name="lineFieldName">�ȸ��߸߳��ֶ���</param>
        /// <param name="pointFeaClsName">�̵߳�Ҫ��������</param>
        /// <param name="pointFieldName">�̵߳��ֶ���</param>
        /// <param name="radiu">�̵߳������뾶</param>
        /// <param name="intervalValue">�ȸ��߼��ֵ</param>
        /// <param name="eError"></param>
        public void PointLineElevCheck(IFeatureDataset pFeatureDataset, string lineFeaClsName, string lineFieldName, string pointFeaClsName, string pointFieldName,double intervalValue, out Exception eError)
        {
            eError = null;
            try
            {
                //�ȸ���Ҫ����
                IFeatureClass lineFeaCls = (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(lineFeaClsName);
                //�ȸ��߸߳��ֶ�����ֵ
                int lineIndex = lineFeaCls.Fields.FindField(lineFieldName);
                if (lineIndex == -1)
                {
                    eError = new Exception("�ȸ���ͼ��ĸ߳��ֶβ�����,�ֶ���Ϊ��" + lineFieldName);
                    return;
                }
                //�̵߳�Ҫ����
                IFeatureClass pointFeaCls = (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(pointFeaClsName );
                int pointIndex = pointFeaCls.Fields.FindField(pointFieldName);
                if (lineIndex == -1)
                {
                    eError = new Exception("�̵߳�ͼ��ĸ߳��ֶβ�����,�ֶ���Ϊ��" + pointFieldName);
                    return;
                }

                //�����̵߳�Ҫ��
                IFeatureCursor pCusor = pointFeaCls.Search(null, false);
                if (pCusor == null) return;
                IFeature pointFeature = pCusor.NextFeature();
                while (pointFeature != null)
                {
                    //�̵߳�Ҫ�صĸ߳�ֵ
                    double pointElevValue =Convert.ToDouble(pointFeature.get_Value(pointIndex).ToString());
                    //���Ҹ̵߳����ڵ������߳���Ҫ��

                    //��̵߳�����ĵȸ���Ҫ���Լ���̾���
                    Dictionary<double, IFeature> nearestFeaDic = GetShortestDis(lineFeaCls, pointFeature, out eError);
                    if (eError != null || nearestFeaDic == null)
                    {
                        eError = new Exception("��������Χ�ڵ�δ�ҵ�Ҫ��!");
                        return;
                    }
                    double pShortestDis = -1;
                    IFeature nearestFea = null;
                    foreach (KeyValuePair<double, IFeature> item in nearestFeaDic)
                    {
                        pShortestDis = item.Key;
                        nearestFea = item.Value;
                        break;
                    }
                    if(eError!=null||pShortestDis==-1) return;
                    //��õȸ�������̵߳�����ĵ�
                    IPoint nearestPoint = new PointClass();//�ȸ����ϵ������
                    IProximityOperator mProxiOpe = nearestFea.Shape as IProximityOperator;
                    if (mProxiOpe == null) return;
                    nearestPoint = mProxiOpe.ReturnNearestPoint(pointFeature.Shape as IPoint, esriSegmentExtension.esriNoExtension);
                    //���̵߳�͵ȸ����ϵĵ������߶β����������ӳ�

                    PointLineAccordanceCheck2(pFeatureDataset, lineFeaCls, pointFeaCls, pointFeature, lineFieldName,lineIndex, pointIndex,nearestFea,pShortestDis, intervalValue, out eError);
                    //PointLineAccordanceCheck(pFeatureDataset, lineFeaCls, pointFeaCls, pointFeature, lineIndex, pointIndex, pShortestDis, nearestFea, intervalValue, out eError);
                    if (eError != null) return;
                    pointFeature = pCusor.NextFeature();
                }

                //�ͷ�cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCusor);
            }
            catch (Exception ex)
            {
                eError = ex;
            }
        }

        /// <summary>
        /// �ȸ��ߵ���ì��һ���Լ��
        /// </summary>
        /// <param name="feaClsLst"></param>
        /// <param name="lineFeaClsName">�ȸ���Ҫ��������</param>
        /// <param name="lineFieldName">�ȸ��߸߳��ֶ���</param>
        /// <param name="pointFeaClsName">�̵߳�Ҫ��������</param>
        /// <param name="pointFieldName">�̵߳��ֶ���</param>
        /// <param name="radiu">�̵߳������뾶</param>
        /// <param name="intervalValue">�ȸ��߼��ֵ</param>
        /// <param name="eError"></param>
        public void PointLineElevCheck(List<IFeatureClass> feaClsLst,string lineFeaClsName, string lineFieldName, string pointFeaClsName, string pointFieldName, double intervalValue, out Exception eError)
        {
            eError = null;
            try
            {
                //�ȸ���Ҫ����
                IFeatureClass lineFeaCls = null;// (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(lineFeaClsName);
                foreach (IFeatureClass mFeaCls in feaClsLst)
                {
                    string pName = (mFeaCls as IDataset).Name;
                    if(pName.Contains("."))
                    {
                        pName = pName.Substring(pName.IndexOf('.') + 1);
                    }
                    if (pName == lineFeaClsName)
                    {
                        lineFeaCls = mFeaCls;
                        break;
                    }
                }
                if (lineFeaCls == null) return;
                //�ȸ��߸߳��ֶ�����ֵ
                int lineIndex = lineFeaCls.Fields.FindField(lineFieldName);
                if (lineIndex == -1)
                {
                    eError = new Exception("�ȸ���ͼ��ĸ߳��ֶβ�����,�ֶ���Ϊ��" + lineFieldName);
                    return;
                }
                //�̵߳�Ҫ����
                IFeatureClass pointFeaCls = null;// (pFeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass(pointFeaClsName);
                foreach (IFeatureClass mFeaCls in feaClsLst)
                {
                    string pName = (mFeaCls as IDataset).Name;
                    if (pName.Contains("."))
                    {
                        pName = pName.Substring(pName.IndexOf('.') + 1);
                    }
                    if (pName == pointFeaClsName)
                    {
                        pointFeaCls = mFeaCls;
                        break;
                    }
                }
                if (pointFeaCls == null) return;
                int pointIndex = pointFeaCls.Fields.FindField(pointFieldName);
                if (lineIndex == -1)
                {
                    eError = new Exception("�̵߳�ͼ��ĸ߳��ֶβ�����,�ֶ���Ϊ��" + pointFieldName);
                    return;
                }

                //�����̵߳�Ҫ��
                IFeatureCursor pCusor = pointFeaCls.Search(null, false);
                if (pCusor == null) return;
                IFeature pointFeature = pCusor.NextFeature();

                //��ʾ������
                ShowProgressBar(true);

                int tempValue = 0;
                ChangeProgressBar1((hook as Plugin.Application.IAppFormRef).ProgressBar, 0, pointFeaCls.FeatureCount(null), tempValue);

                while (pointFeature != null)
                {
                    //�̵߳�Ҫ�صĸ߳�ֵ
                    double pointElevValue = Convert.ToDouble(pointFeature.get_Value(pointIndex).ToString());
                    //���Ҹ̵߳����ڵ������߳���Ҫ��

                    //��̵߳�����ĵȸ���Ҫ���Լ���̾���
                    Dictionary<double, IFeature> nearestFeaDic = GetShortestDis(lineFeaCls, pointFeature, out eError);
                    if (eError != null || nearestFeaDic == null)
                    {
                        eError = new Exception("��������Χ�ڵ�δ�ҵ�Ҫ��!");
                        return;
                    }
                    double pShortestDis = -1;
                    IFeature nearestFea = null;
                    foreach (KeyValuePair<double, IFeature> item in nearestFeaDic)
                    {
                        pShortestDis = item.Key;
                        nearestFea = item.Value;
                        break;
                    }
                    if (eError != null || pShortestDis == -1) return;
                    //��õȸ�������̵߳�����ĵ�
                    IPoint nearestPoint = new PointClass();//�ȸ����ϵ������
                    IProximityOperator mProxiOpe = nearestFea.Shape as IProximityOperator;
                    if (mProxiOpe == null) return;
                    nearestPoint = mProxiOpe.ReturnNearestPoint(pointFeature.Shape as IPoint, esriSegmentExtension.esriNoExtension);
                    //���̵߳�͵ȸ����ϵĵ������߶β����������ӳ�

                    PointLineAccordanceCheck2(null, lineFeaCls, pointFeaCls, pointFeature, lineFieldName, lineIndex, pointIndex, nearestFea, pShortestDis, intervalValue, out eError);
                    //PointLineAccordanceCheck(pFeatureDataset, lineFeaCls, pointFeaCls, pointFeature, lineIndex, pointIndex, pShortestDis, nearestFea, intervalValue, out eError);
                    if (eError != null) return;
                    pointFeature = pCusor.NextFeature();

                    tempValue += 1;//��������ֵ��1
                    ChangeProgressBar1((hook as Plugin.Application.IAppFormRef).ProgressBar, -1, -1, tempValue);
                }

                //�ͷ�cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCusor);
            }
            catch (Exception ex)
            {
                eError = ex;
            }
        }


        /// <summary>
        /// ��Ҫ�����ϲ��Ҿ��������Ҫ��
        /// </summary>
        /// <param name="desFeaCls">Ŀ��Ҫ����</param>
        /// <param name="codeName">��������ֶ���</param>
        /// <param name="codeValue">�������ֵ</param>
        /// <param name="oriFeature">ԴҪ��</param>
        /// <param name="radiu">����뾶</param>
        /// <param name="outError"></param>
        /// <returns></returns>
        private IFeature GetNearestFeature(IFeatureClass desFeaCls, string codeName, string codeValue, IFeature oriFeature, double radiu, out Exception outError)
        {
            outError = null;
            IFeature returnFeature = null;
            double pDistance = -1;
            try
            {
                //����ԴҪ�صĻ���뾶�õ����巶Χ
                ITopologicalOperator pTopoOper = oriFeature.Shape as ITopologicalOperator;
                IGeometry pGeo = pTopoOper.Buffer(radiu);

                //����Ŀ��Ҫ�����и����������Ļ��巶Χ�ڵ�Ҫ��
                string str = "";
                if (codeValue != "")
                {
                    str = codeName + " = '" + codeValue + "'";
                }
                ISpatialFilter pFilter = new SpatialFilterClass();
                pFilter.WhereClause = str;
                pFilter.GeometryField = "SHAPE";
                pFilter.Geometry = pGeo;
                pFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;//.esriSpatialRelWithin;
                IFeatureCursor pCursor = desFeaCls.Search(pFilter, false);
                if (pCursor == null) return null;
                IFeature pFeature = pCursor.NextFeature();

                //����Ҫ�أ����Ҿ��������Ҫ��
                while (pFeature != null)
                {
                    IProximityOperator pProxiOper = oriFeature.Shape as IProximityOperator;
                    double tempDis = pProxiOper.ReturnDistance(pFeature.Shape);
                    if (pDistance == -1 || pDistance > tempDis)
                    {
                        pDistance = tempDis;
                        returnFeature = pFeature;
                    }
                    pFeature = pCursor.NextFeature();
                }

                //�ͷ�cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
            }
            catch (Exception ex)
            {
                outError = ex;
            }
            return returnFeature;
        }

        /// <summary>
        /// ��Ҫ����Ŀ��Ҫ�ص���̾���
        /// </summary>
        /// <param name="desFeaCls"></param>
        /// <param name="pointFeature">��Ҫ��</param>
        /// <param name="radiu">��Ҫ�������뾶</param>
        /// <param name="outError"></param>
        /// <returns></returns>
        private Dictionary<double, IFeature> GetShortestDis(IFeatureClass desFeaCls, IFeature pointFeature, out Exception outError)
        {
            outError = null;
            Dictionary<double, IFeature> ShortestFeature = new Dictionary<double, IFeature>();
            double pShortestDistance = -1;
            IFeature pFeature = null;
            try
            {
                //List<IFeature> tempFeaLst = new List<IFeature>();
                //tempFeaLst = GetFeatureByDis(desFeaCls, pointFeature,esriSpatialRelEnum.esriSpatialRelIntersects, radiu);
                IFeatureCursor pCusor = desFeaCls.Search(null, false);
                if (pCusor == null) return null ;
                IFeature mFea = pCusor.NextFeature();
                //����Ҫ�أ����Ҿ��������Ҫ��,
                while(mFea!=null)
                {
                    IProximityOperator pProxiOper = pointFeature.Shape as IProximityOperator;
                    double tempDis = pProxiOper.ReturnDistance(mFea.Shape);
                    if (pShortestDistance == -1 || pShortestDistance > tempDis)
                    {
                        pShortestDistance = tempDis;
                        pFeature = mFea;
                    }
                    mFea = pCusor.NextFeature();
                }
                if (pShortestDistance == -1||pFeature==null)
                {
                    outError = new Exception("δ�ҵ��κ�Ҫ��!");
                    return null;
                }
                ShortestFeature.Add(pShortestDistance, pFeature);
            }
            catch (Exception ex )
            {
                outError = ex;
            }
            return ShortestFeature;
        }

        private void PointLineAccordanceCheck(IFeatureDataset pFeaDataset, IFeatureClass desFeaCls, IFeatureClass oriFeaCls, IFeature pointFeature, int lineIndex, int pointIndex, double pShortestDistance,IFeature nearestFea, double intervalElev, out Exception eError)
        {
            eError = null;
            try
            {
                //�������ʹν�����Ҫ��
                IFeature pLineFeature1 = null;
                IFeature pLineFeature2 = null;
                //������Ҫ�صĸ߳�ֵ
                string pLineElev1 = "";
                string pLineElev2 = "";
                //��Ҫ�صĸ߳�ֵ
                string pPointElev = "";
                //����ID
                int eErrorID = enumErrorType.�ȸ��ߵ���ì�ܼ��.GetHashCode();
                //��������
                string eErrorDes = "";

                //��Ҫ�صĸ߳�ֵ
                pPointElev = pointFeature.get_Value(pointIndex).ToString().Trim();
                if (pPointElev == "")
                {
                    eError = new Exception("�̵߳�ĸ߳�ֵΪ�գ�OIDΪ��" + pointFeature.OID);
                    return;
                }
                if (pShortestDistance == 0)
                {
                    #region �̵߳�λ�ڵȸ�����
                    //�̵߳�λ�ڵȸ�����
                    
                    pLineElev1 =nearestFea.get_Value(lineIndex).ToString().Trim();
                    if (pLineElev1 == "")
                    {
                        eError = new Exception("�ȸ��ߵĸ߳�ֵΪ�գ�OIDΪ��" + nearestFea.OID);
                        return;
                    }
                    //�Ƚϵ�Ҫ�غ���Ҫ�صĸ߳�ֵ��������ȣ�����Ϊ����ȷ
                    if (Convert.ToDouble(pLineElev1) != Convert.ToDouble(pPointElev))
                    {
                        //����������������
                        eErrorDes = "�̵߳���߳��ߵĸ߳�ֵì��";
                        GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, nearestFea, eErrorID, eErrorDes, out eError);
                        if (eError != null) return;
                    }
                    #endregion 
                }
                else if (pShortestDistance >= intervalElev)
                {
                    #region  �̵߳�λ�ڵȸ��ߵ��������������
                    //���ݸþ�����в���
                    List<IFeature> lstFea = GetFeatureByDis(desFeaCls, pointFeature, esriSpatialRelEnum.esriSpatialRelTouches, pShortestDistance);
                    if (lstFea==null|| lstFea.Count == 0) return;
                    //��������һ����
                    pLineFeature1 = lstFea[0];
                    pLineElev1 = pLineFeature1.get_Value(lineIndex).ToString().Trim();
                    if (pLineElev1 == "")
                    {
                        eError = new Exception("�ȸ��ߵĸ߳�ֵΪ�գ�OIDΪ��" + pLineFeature1.OID);
                        return;
                    }
                    lstFea = GetFeatureByDis(desFeaCls, pointFeature, esriSpatialRelEnum.esriSpatialRelTouches, (intervalElev+pShortestDistance));
                    if (lstFea == null || lstFea.Count == 0) return;
                    //���ν���һ����
                    pLineFeature2 = lstFea[0];
                    pLineElev2 = pLineFeature2.get_Value(lineIndex).ToString().Trim();
                    if (pLineElev2 == "")
                    {
                        eError = new Exception("�ȸ��ߵĸ߳�ֵΪ�գ�OIDΪ��" + pLineFeature2.OID);
                        return;
                    }
                    eErrorDes = "�̵߳�������ĵȸ��߸߳�ֵì��";
                    if ((Convert.ToDouble(pLineElev1) < Convert.ToDouble(pLineElev2)) && (Convert.ToDouble(pPointElev) > Convert.ToDouble(pLineElev1)))
                    {
                        //�̵߳����ĸ߳�ֵ�����������
                        GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, pLineFeature1, eErrorID, eErrorDes, out eError);
                    }
                    if ((Convert.ToDouble(pLineElev1) > Convert.ToDouble(pLineElev2)) && (Convert.ToDouble(pPointElev) < Convert.ToDouble(pLineElev1)))
                    {
                        //�̵߳����ĸ߳�ֵ�����������
                        GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, pLineFeature1, eErrorID, eErrorDes, out eError);
                    }
                    #endregion
                }
                else if(pShortestDistance<intervalElev)
                {
                    #region ��������������̵߳�λ�������ȸ���֮�䡢�̵߳�λ�ڵȸ����������������
                    List<IFeature> lstFeature2 = new List<IFeature>();
                    //���ݾ�����в���
                    lstFeature2 = GetFeatureByDis(desFeaCls, pointFeature, esriSpatialRelEnum.esriSpatialRelIntersects, (intervalElev - pShortestDistance));
                    if (lstFeature2 == null || lstFeature2.Count == 0) return;
                    if (lstFeature2.Count == 1)
                    {
                        //�̵߳�λ�ڵȸ��ߵ��������������
                        //��̵߳������Ҫ��
                        pLineFeature1 = lstFeature2[0];
                        pLineElev1 = pLineFeature1.get_Value(lineIndex).ToString().Trim();
                        if (pLineElev1 == "")
                        {
                            eError = new Exception("�ȸ��ߵĸ߳�ֵΪ�գ�OIDΪ��" + pLineFeature1.OID);
                            return;
                        }
                        List<IFeature> mFeaLst = GetFeatureByDis(desFeaCls, pointFeature, esriSpatialRelEnum.esriSpatialRelTouches, (pShortestDistance+intervalElev));
                        if (mFeaLst == null || mFeaLst.Count == 0) return;
                        //��̵߳�ν���Ҫ��
                        pLineFeature2 = mFeaLst[0];
                        pLineElev2 = pLineFeature2.get_Value(lineIndex).ToString().Trim();
                        if (pLineElev2 == "")
                        {
                            eError = new Exception("�ȸ��ߵĸ߳�ֵΪ�գ�OIDΪ��" + pLineFeature2.OID);
                            return;
                        }
                        eErrorDes = "�̵߳�������ĵȸ��߸߳�ֵì��";
                        if ((Convert.ToDouble(pLineElev1) < Convert.ToDouble(pLineElev2)) && (Convert.ToDouble(pLineElev1) < Convert.ToDouble(pPointElev)))
                        {
                            //�̵߳���ȸ��߸߳�ֵì�ܣ����������
                            GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, pLineFeature1, eErrorID, eErrorDes,out eError);
                        }
                        if ((Convert.ToDouble(pLineElev1) > Convert.ToDouble(pLineElev2)) && (Convert.ToDouble(pLineElev1) > Convert.ToDouble(pPointElev)))
                        {
                            //�̵߳���ȸ��߸߳�ֵì�ܣ����������
                            GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, pLineFeature1, eErrorID, eErrorDes, out eError);
                        }
                    }
                    else if (lstFeature2.Count == 2)
                    {
                        //�̵߳�λ�ڵȸ���֮��
                        //��̵߳��������Ҫ��
                        List<IFeature> pFeaLst1 = GetFeatureByDis(desFeaCls, pointFeature, esriSpatialRelEnum.esriSpatialRelTouches, pShortestDistance);
                        if (pFeaLst1 == null || pFeaLst1.Count == 0) return;
                        pLineFeature1 = pFeaLst1[0];
                        pLineElev1 = pLineFeature1.get_Value(lineIndex).ToString().Trim();
                        if (pLineElev1 == "")
                        {
                            eError = new Exception("�ȸ��ߵĸ߳�ֵΪ�գ�OIDΪ��" + pLineFeature1.OID);
                            return;
                        }
                        //��̵߳�ν�����Ҫ��
                        List<IFeature> pFeaLst2 = GetFeatureByDis(desFeaCls, pointFeature, esriSpatialRelEnum.esriSpatialRelTouches, (intervalElev - pShortestDistance));
                        if (pFeaLst2 == null || pFeaLst2.Count == 0) return;
                        pLineFeature2 = pFeaLst2[0];
                        pLineElev2 = pLineFeature2.get_Value(lineIndex).ToString().Trim();
                        if (pLineElev2 == "")
                        {
                            eError = new Exception("�ȸ��ߵĸ߳�ֵΪ�գ�OIDΪ��" + pLineFeature2.OID);
                            return;
                        }
                        eErrorDes = "�̵߳����ڽ��������ȸ��߸߳�ֵì��";
                        if (Convert.ToDouble(pLineElev1) > Convert.ToDouble(pLineElev2))
                        {
                            if((Convert.ToDouble(pPointElev)>=Convert.ToDouble(pLineElev1))||(Convert.ToDouble(pPointElev)<=Convert.ToDouble(pLineElev2)))
                            {
                                GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, pLineFeature1, eErrorID, eErrorDes, out eError);
                                if (eError != null) return;
                            }
                        }
                        if (Convert.ToDouble(pLineElev1) < Convert.ToDouble(pLineElev2))
                        {
                            if ((Convert.ToDouble(pPointElev) <= Convert.ToDouble(pLineElev1)) || (Convert.ToDouble(pPointElev) >= Convert.ToDouble(pLineElev2)))
                            {
                                GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, pLineFeature1, eErrorID, eErrorDes, out eError);
                                if (eError != null) return;
                            }
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                eError = ex;
                return;
            }
        }

        /// <summary>
        /// �ȸ��߸̵߳����ì�ܼ��,����Ҫ����
        /// </summary>
        /// <param name="pFeaDataset"></param>
        /// <param name="desFeaCls">�ȸ���Ҫ����</param>
        /// <param name="oriFeaCls">�̵߳�Ҫ����</param>
        /// <param name="pointFeature">�̵߳�Ҫ��</param>
        /// <param name="lineFieldName">�ȸ��߸߳��ֶ���</param>
        /// <param name="lineIndex">�ȸ��߸߳��ֶ�����</param>
        /// <param name="pointIndex">�̵߳�߳��ֶ���</param>
        /// <param name="nearestFea">��̵߳�Ҫ������ĵȸ���Ҫ��</param>
        /// <param name="intervalElev">�ȸ��߼��ֵ</param>
        /// <param name="eError"></param>
        private void PointLineAccordanceCheck2(IFeatureDataset pFeaDataset, IFeatureClass desFeaCls, IFeatureClass oriFeaCls, IFeature pointFeature,string lineFieldName,int lineIndex, int pointIndex, IFeature nearestFea,double pShortestDis, double intervalElev, out Exception eError)
        {
            eError = null;

            try
            {
                double pValue = 0.0;                                       //��Ҫ�صĸ߳�ֵ
                double fValue = 0.0;                                       //�����Ҫ�ظ߳�ֵ
                double lValue = 0.0;                                        //�����Ҫ�����ڵĵ�һ��Ҫ��
                double sValue = 0.0;                                       //�����Ҫ�����ڵĵڶ���Ҫ��
                int eErrorID = enumErrorType.�ȸ��ߵ���ì�ܼ��.GetHashCode(); //����ID
                string eErrorDes = "�ȸ�����̵߳�߳�ֵ����ì�ܣ�";       //��������

                //��Ҫ�صĸ߳�ֵ
                string pValueStr = pointFeature.get_Value(pointIndex).ToString().Trim();
                if (pValueStr == "")
                {
                    eError = new Exception("�̵߳�ĸ߳�ֵΪ�գ�OIDΪ��" + pointFeature.OID);
                    return;
                }
                pValue = Convert.ToDouble(pValueStr);
                //�����Ҫ�ظ߳�ֵ
                string fValueStr = nearestFea.get_Value(lineIndex).ToString().Trim();
                if (fValueStr == "")
                {
                    eError = new Exception("��Ҫ�صĸ߳�ֵΪ�ա�OIDΪ��" + nearestFea.OID);
                    return;
                }
                fValue = Convert.ToDouble(fValueStr);

                if (pShortestDis == 0)
                {
                    //˵����������
                    if (pValue != fValue)
                    {
                        //���߸߳�ֵì��
                        GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, nearestFea, eErrorID, eErrorDes, out eError);
                        if (eError != null) return;
                    }
                }
                else
                {
                    if (fValue < intervalElev)
                    {
                        #region ˵������С�߳�,˵����Ҫ������������������Ҫ��
                        //�����Ҫ�����ڵĵ�һ��Ҫ�صĸ߳�ֵ
                        lValue = fValue + intervalElev;
                        string whereStr = lineFieldName + "=" + lValue;
                        List<IFeature> lstFefa = GetFeatureByStr(desFeaCls, whereStr, out eError);
                        if (eError != null || lstFefa == null || lstFefa.Count == 0)
                        {
                            return;
                        }
                        //bool isIntersact = false;
                        
                        //for (int k = 0; k < lstFefa.Count; k++)
                        //{
                        if (lstFefa.Count == 1)
                        {
                            IFeature secondFea = lstFefa[0];
                            //����secondFea����̵߳�����ĵ�
                            IProximityOperator pProxiOper = secondFea.Shape as IProximityOperator;
                            IPoint p = new PointClass();
                            p = pProxiOper.ReturnNearestPoint(pointFeature.Shape as IPoint, esriSegmentExtension.esriNoExtension);
                            //    if (IsIntersect(pointFeature.Shape as IPoint, p, nearestFea))
                            //    {
                            //        isIntersact = true;
                            //        secondFea = lstFefa[k];
                            //        break;
                            //    }
                            //}

                            #region �����Ҫ�����ڵĵ�һ��Ҫ���ϵ���̵߳������������Ҫ�ص����˹�ϵ��������������ཻ�����ཻ��
                            if (IsIntersect(pointFeature.Shape as IPoint, p, nearestFea))
                            {
                                //������������nearestFea�ཻ��˵���̵߳�����Ҫ�ص����������������
                                //����fValue<lValue
                                if (pValue > fValue)
                                {
                                    //���߸߳�ֵì��
                                    GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, nearestFea, eErrorID, eErrorDes, out eError);
                                    if (eError != null) return;
                                }
                            }
                            else
                            {
                                //������������nearestFea���ཻ���̵߳���������Ҫ��֮��
                                if (pValue < fValue)
                                {
                                    //���߸߳�ֵì��
                                    GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, secondFea, eErrorID, eErrorDes, out eError);
                                    if (eError != null) return;
                                }
                                if (pValue > lValue)
                                {
                                    //���߸߳�ֵì��
                                    GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, nearestFea, eErrorID, eErrorDes, out eError);
                                    if (eError != null) return;
                                }
                            }
                            #endregion
                        }

                        #endregion
                    }
                    else if (fValue >= intervalElev)
                    {
                        #region ��Ϊ�������������������������������Ҫ�ء��������м���Ҫ��
                        //��nearestFea���ڵ�����Ҫ�صĸ߳�ֵ
                        lValue = fValue + intervalElev;
                        sValue = fValue - intervalElev;
                        //�����Ҫ��Ҫ�ص�����Ҫ��
                        IFeature secondFea = null;
                        IFeature thirdFea = null;

                        string whereStr = lineFieldName + "=" + lValue;
                        
                        List<IFeature> lstFea2=GetFeatureByStr(desFeaCls, whereStr, out eError);
                        if (eError != null||lstFea2==null)
                        {
                            return;
                        }
                        whereStr = lineFieldName + "=" + sValue;
                        List<IFeature> lstFea3=GetFeatureByStr(desFeaCls, whereStr, out eError);
                        if (eError != null || lstFea3 == null)
                        {
                            return;
                        }
                      
                        if (lstFea2.Count==0&&lstFea3.Count==0)
                        {
                            //ֻ��һ���ȸ���
                            eError = new Exception("ֻ��һ���ȸ��ߣ����ܽ��м�飡");
                            return;
                        }
                        else if (lstFea2.Count==1 && lstFea3.Count==1)
                        {
                            secondFea = lstFea2[0];
                            thirdFea = lstFea3[0];
                            #region nearestFea������������������Ҫ��
                            IPoint p = new PointClass();
                            IProximityOperator pProxiOper = secondFea.Shape as IProximityOperator;
                            p = pProxiOper.ReturnNearestPoint(pointFeature.Shape as IPoint, esriSegmentExtension.esriNoExtension);
                            if (IsIntersect(pointFeature.Shape as IPoint, p, nearestFea))
                            {
                                //�ཻ��sValue<fValue<lValue ,pValueӦλ��sValue��fValue֮��
                                if (pValue < sValue)
                                {
                                    //���߸߳�ֵì��
                                    GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, thirdFea, eErrorID, eErrorDes, out eError);
                                    if (eError != null) return;
                                }
                                if (pValue > fValue)
                                {
                                    //���߸߳�ֵì��
                                    GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, nearestFea, eErrorID, eErrorDes, out eError);
                                    if (eError != null) return;
                                }
                            }
                            else
                            {
                                //���ཻ��sValue<fValue<lValue ,pValueӦλ��fValue��lValue֮��
                                if (pValue < fValue)
                                {
                                    //���߸߳�ֵì��
                                    GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, nearestFea, eErrorID, eErrorDes, out eError);
                                    if (eError != null) return;
                                }
                                if (pValue > lValue)
                                {
                                    //���߸߳�ֵì��
                                    GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, secondFea, eErrorID, eErrorDes, out eError);
                                    if (eError != null) return;
                                }
                            }
                            #endregion
                        }
                        else if(lstFea2.Count==1 || lstFea3.Count==1)
                        {
                            if (lstFea2.Count == 1&&lstFea3.Count==0)
                            {
                                secondFea = lstFea2[0];
                            }
                            else if (lstFea3.Count == 1 && lstFea2.Count == 0)
                            {
                                thirdFea = lstFea3[0];
                            }
                            if (secondFea == null && thirdFea == null) return;

                            #region  nearestFea����������������Ҫ��
                            IPoint p = new PointClass();
                            if (secondFea != null)
                            {
                                IProximityOperator pProxiOper = secondFea.Shape as IProximityOperator;
                                p = pProxiOper.ReturnNearestPoint(pointFeature.Shape as IPoint, esriSegmentExtension.esriNoExtension);
                            }
                            if (thirdFea != null)
                            {
                                IProximityOperator pProxiOper = thirdFea.Shape as IProximityOperator;
                                p = pProxiOper.ReturnNearestPoint(pointFeature.Shape as IPoint, esriSegmentExtension.esriNoExtension);
                            }
                            if (IsIntersect(pointFeature.Shape as IPoint, p, nearestFea))
                            {
                                //�ཻ���̵߳��ڵȸ��ߵ��������������
                                if (secondFea != null)
                                {
                                    //����fValue<lValue
                                    if (pValue > fValue)
                                    {
                                        //���߸߳�ֵì��
                                        GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, nearestFea, eErrorID, eErrorDes, out eError);
                                        if (eError != null) return;
                                    }
                                }
                                if (thirdFea != null)
                                {
                                    //����fValue>sValue
                                    if (pValue < fValue)
                                    {
                                        //���߸߳�ֵì��
                                        GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, nearestFea, eErrorID, eErrorDes, out eError);
                                        if (eError != null) return;
                                    }
                                }
                            }
                            else
                            {
                                //���ཻ���̵߳��ڵȸ��ߵ���ߵ������ȸ���֮��
                                if (secondFea != null)
                                {
                                    //����fValue<lValue
                                    if (pValue < fValue)
                                    {
                                        //���߸߳�ֵì��
                                        GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, nearestFea, eErrorID, eErrorDes, out eError);
                                        if (eError != null) return;
                                    }
                                    if (pValue > lValue)
                                    {
                                        //���߸߳�ֵì��
                                        GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, secondFea, eErrorID, eErrorDes, out eError);
                                        if (eError != null) return;
                                    }
                                }
                                if (thirdFea != null)
                                {
                                    //����fValue>sValue
                                    if (pValue > fValue)
                                    {
                                        //���߸߳�ֵì��
                                        GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, nearestFea, eErrorID, eErrorDes, out eError);
                                        if (eError != null) return;
                                    }
                                    if (pValue < sValue)
                                    {
                                        //���߸߳�ֵì��
                                        GetErrorList(pFeaDataset, oriFeaCls, pointFeature, desFeaCls, thirdFea, eErrorID, eErrorDes, out eError);
                                        if (eError != null) return;
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                eError = ex;
            }
        }
        /// <summary>
        /// ����֮��������Ƿ������ཻ
        /// </summary>
        /// <param name="mFPoint"></param>
        /// <param name="mLPoint"></param>
        /// <param name="lineFeature"></param>
        /// <returns>true:�ཻ;false:���ཻ</returns>
        private bool IsIntersect(IPoint mFPoint,IPoint mLPoint,IFeature lineFeature)
        {
            //������Ҫ��
            //IPolyline mPLine = new PolylineClass();
            //mPLine.FromPoint = mFPoint;
            //mPLine.ToPoint = mLPoint;
            //�����㼯
            //IPointCollection mPointCol = new PolylineClass();
            //object obj = System.Reflection.Missing.Value;
            //mPointCol.AddPoint(mFPoint, ref obj, ref obj);
            //mPointCol.AddPoint(mLPoint, ref obj, ref obj);
            //mPLine = mPointCol as IPolyline;

            ILine pLine=new LineClass();
            pLine.PutCoords(mFPoint,mLPoint);
            ISegmentCollection pSegCol=new PolylineClass();
            object obj=Type.Missing;
            pSegCol.AddSegment(pLine as ISegment,ref obj,ref obj);
            IRelationalOperator pRelOpera=pSegCol as IRelationalOperator;
            if (pRelOpera.Disjoint(lineFeature.Shape))
            {
                //���ཻ
                return false;
            }
            else
            {
                //�ཻ
                return true;
            }
        }

        /// <summary>
        /// ����ָ�����������Ҫ��
        /// </summary>
        /// <param name="desFeaCls"></param>
        /// <param name="whereStr"></param>
        /// <param name="eError"></param>
        /// <returns></returns>
        private List<IFeature> GetFeatureByStr(IFeatureClass desFeaCls,string whereStr,out Exception eError)
        {
            eError=null;
            List<IFeature> LstFeature = new List<IFeature>();
            try
            {
                IQueryFilter pFilter = new QueryFilterClass();
                pFilter.WhereClause = whereStr;

                IFeatureCursor pCusor = desFeaCls.Search(pFilter, false);
                if (pCusor == null) return null;
                IFeature pFea = pCusor.NextFeature();
                while (pFea != null)
                {
                    LstFeature.Add(pFea);
                    pFea = pCusor.NextFeature();
                }

                //�ͷ�cursor
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCusor);
                return LstFeature;
            }
            catch (Exception ex)
            {
                eError = ex;
                return null;
            }
        }

        /// <summary>
        /// ����Ҫ�صĻ���������Ҫ��
        /// </summary>
        /// <param name="desFeaCls"></param>
        /// <param name="pointFeature"></param>
        /// <param name="dis"></param>
        /// <returns></returns>
        private List<IFeature> GetFeatureByDis(IFeatureClass desFeaCls, IFeature pointFeature,esriSpatialRelEnum spatialRelEnum, double dis)
        {
            List<IFeature> LstFeature = new List<IFeature>();
            //����ԴҪ�صĻ���뾶�õ����巶Χ
            ITopologicalOperator pTopoOper = pointFeature.Shape as ITopologicalOperator;
            IGeometry pGeo = pTopoOper.Buffer(dis);

            //���һ��巶Χ�ڵ�Ҫ��
            ISpatialFilter pFilter = new SpatialFilterClass();
            pFilter.GeometryField = "SHAPE";
            pFilter.Geometry = pGeo;
            pFilter.SpatialRel = spatialRelEnum;// esriSpatialRelEnum.esriSpatialRelIntersects;//.esriSpatialRelWithin;
            IFeatureCursor pCursor = desFeaCls.Search(pFilter, false);
            if (pCursor == null) return null;
            IFeature pFeature = pCursor.NextFeature();
            while (pFeature != null)
            { 
                LstFeature.Add(pFeature);
                pFeature=pCursor.NextFeature();
            }
            return LstFeature;
        }

        /// <summary>
        /// ��ô����б��ȸ��ߵ���ì��
        /// </summary>
        /// <param name="pFeatureDataset"></param>
        /// <param name="pFeatureCls"></param>
        /// <param name="feaClsname"></param>
        /// <param name="pFeature"></param>
        /// <param name="eErrorID"></param>
        /// <param name="eError"></param>
        private void GetErrorList(IFeatureDataset pFeatureDataset,IFeatureClass pOriFeatureCls,IFeature pOriFeature,IFeatureClass pDesFeatureCls, IFeature pDesFeature, int eErrorID,string eErrorDes, out Exception eError)
        {
            eError = null;
            try
            {
                double pMapx = 0.0;
                double pMapy = 0.0;
                IPoint pPoint = null;
                //�߳�ֵ���ڸ����ĸ̷߳�Χ��,����������������
                if (pOriFeatureCls.ShapeType != esriGeometryType.esriGeometryPoint)
                {
                    pPoint = TopologyCheckClass.GetPointofFeature(pOriFeature);
                }
                else
                {
                    pPoint = pOriFeature.Shape as IPoint;
                }
                pMapx = pPoint.X;
                pMapy = pPoint.Y;

                List<object> ErrorLst = new List<object>();
                ErrorLst.Add("Ҫ�����Լ��");//����������
                ErrorLst.Add("�ȸ��ߵ���ì�ܼ��");//��������
                if (pFeatureDataset == null)
                {
                    ErrorLst.Add("");  //�����ļ���
                }
                else
                {
                    ErrorLst.Add((pFeatureDataset as IDataset).Workspace.PathName);  //�����ļ���
                }
                
                ErrorLst.Add(eErrorID );//����id;
                ErrorLst.Add(eErrorDes);//��������
                ErrorLst.Add(pMapx);    //...
                ErrorLst.Add(pMapy);    //...
                ErrorLst.Add((pOriFeatureCls as IDataset).Name);
                ErrorLst.Add(pOriFeature.OID);
                ErrorLst.Add((pDesFeatureCls as IDataset).Name);
                ErrorLst.Add(pDesFeature.OID);
                ErrorLst.Add(false);
                ErrorLst.Add(System.DateTime.Now.ToString());

                //���ݴ�����־
                IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
            }
            catch (Exception ex)
            {
                eError = ex;
            }
        }


        #endregion

        # region �ӱ߼��
        double SearchValue = 0;//���������ֵ
        double AreaValue = 0;//��Χ�����ֵ
        public double SEARCHValue
        {
            get 
            {
                return SearchValue;
            }
            set 
            {
                SearchValue=value;
            }
        }
        public double AREAValue
        {
            get 
            {
                return AreaValue;
            }
            set 
            {
                AreaValue=value;
            }
        }
        private Plugin.Application.IAppGISRef _AppHk;
        ArrayList FilterList = new ArrayList();//���������ظ����ֵ���
      
        /// <summary>
        /// ��ճ������
        /// </summary>
        /// <param name="AppHk"></param>
        private delegate void ClearCheck(Plugin.Application.IAppGISRef AppHk);
        private void Clear(Plugin.Application.IAppGISRef AppHk)
        {
            if (AppHk.DataCheckGrid.Rows.Count > 0)
            {
                AppHk.DataCheckGrid.DataSource = null;
            }
            Plugin.Application.IAppFormRef pAppFormRef = AppHk as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
            AppHk.DataTree.Nodes.Clear();

        }

        /// <summary>
        /// ˫���¼������ʵ��ί�еķ��� ����������֯�����ݰ���б��� �������ί��
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="AppHk"></param>
        private delegate void DeleteBindCilck(DataTable tb, Plugin.Application.IAppGISRef AppHk);
        /// <summary>
        /// ˫���¼������ʵ��ί�еķ��� ����������֯�����ݰ���б���
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="AppHk"></param>
        private void BindClick(DataTable tb, Plugin.Application.IAppGISRef AppHk)
        {
            AppHk.DataCheckGrid.DataSource = tb;
            AppHk.DataCheckGrid.Columns[0].Width = AppHk.DataCheckGrid.Width/2-5;
            AppHk.DataCheckGrid.Columns[1].Width = AppHk.DataCheckGrid.Width / 2 - 5;
            AppHk.DataCheckGrid.Columns[2].Visible = false;//���ص�����
            if (AppHk.DataCheckGrid.DataSource != null)
            {
                AppHk.DataCheckGrid.MouseDoubleClick += new MouseEventHandler(Overridfunction.DataCheckGridDoubleClick);//����˫���¼�
            }
        }

        /// <summary>
        /// ��ʼ����
        /// </summary>
        public bool Initialize_Tree(SysCommon.Gis.SysGisDataSet pRangeGisDB, string LayerName, string FiledName, DevComponents.AdvTree.AdvTree pDataTree, DevComponents.AdvTree.AdvTree Trees, Plugin.Application.IAppGISRef AppHk,out Exception err)
        {
            err = null;

            ArrayList StrParlist = new ArrayList();//������Žڵ�����������ѡ��Ĳ������ֶ���
            bool CheckTree = false;//ȷ���Ƿ�ѡ�����
            pDataTree.Nodes.Clear();//������еĽڵ�
            //��ȡҪ�ӱ߼�������ͼ�� �����ý���������ѡ��Ĳ�ȡ
            List<ILayer> listCheckLays = new List<ILayer>();
            if (Trees.Nodes.Count == 0) return false;
            foreach (DevComponents.AdvTree.Node tempnode in Trees.Nodes)
            {
                if (tempnode.Checked)
                {
                    CheckTree = true;
                    StrParlist.Add(tempnode.Name.Trim());//����Ӧ�Ĳ��ֶ����Լ��뵽�����б���
                    listCheckLays.Add(tempnode.Tag as ILayer);//�����ý���������ѡ��Ҫ���нӱ߼��Ĳ���뵽��̬���鵱��
                }
            }
            if (!CheckTree)
            {
                SetCheckState.Message(AppHk as Plugin.Application.IAppFormRef, "��ʾ", "��ѡ��ͼ����м�飡");
                return false;
            }
            bool ReState = BindTree(pRangeGisDB,LayerName, FiledName, pDataTree, listCheckLays, StrParlist, AppHk,out err);
            if (!ReState)
            {
                return false;
            }
            
            return CheckTree;//����״̬��ȷ���Ƿ�ѡ���Ҫ���ӱߵĲ�
        }
        /// <summary>
        /// �ڽ��нӱ߼��ʱ������ѡ������ͼ��Ϊ���ջ���������Ϊ���գ�Ȼ����ʾ����������
        /// </summary>
        /// <param name="LayerName"></param>
        /// <param name="FiledName"></param>
        /// <param name="pDataTree"></param>
        private bool BindTree(SysCommon.Gis.SysGisDataSet pRangeGisDB, string LayerName, string FiledName, DevComponents.AdvTree.AdvTree pDataTree, List<ILayer> listCheckLays, ArrayList StrParlist, Plugin.Application.IAppGISRef AppHk,out Exception err)
        {
            bool State = true;
            err = null;
            try
            {
                //��ȡͼ��
                IFeatureClass pMapFeaCls=(pRangeGisDB.WorkSpace as IFeatureWorkspace).OpenFeatureClass(LayerName);
                 IFeatureLayer pFeatMapLay=new FeatureLayerClass();
                pFeatMapLay.FeatureClass=pMapFeaCls;
                if (pFeatMapLay == null) return false;
                //����ͼ����ϱ�
                if (pMapFeaCls.Fields.FindField(FiledName) == -1) return false;
                IFeatureCursor pFeatCur = pMapFeaCls.Search(null, false);
                IFeature pFeature = pFeatCur.NextFeature();
                string tempStr = "�ӱ߷�Χ";
                //if (LayerName.IndexOf("ͼ��") != -1) tempStr = "ͼ����";
                while (pFeature != null)
                {
                    string TableName = pFeature.get_Value(pFeature.Fields.FindField(FiledName)).ToString();//ͼ����ϱ���
                    DevComponents.AdvTree.Node aNode = new DevComponents.AdvTree.Node();
                    aNode.Text = tempStr + TableName;
                    aNode.Name = TableName;
                    aNode.Tag = pFeature;//��ͼ������ڸ��ڵ�TAG��
                    aNode.Image = pDataTree.ImageList.Images[4];//���ýڵ�����ʾ��ͼƬ

                    //�����е��ߺ������Ϊ�ӽڵ���֯��ʽ�磺ͼ��Ϊ���ڵ㣬�ߺ����Ϊ�ӽڵ�
                    for (int n = 0; n < listCheckLays.Count; n++)
                    {
                        DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node();
                        node.Text = ((listCheckLays[n] as IFeatureLayer).FeatureClass as IDataset).Name;//listCheckLays[n].Name;//��������Ϊ�ӽڵ�
                        node.Name = TableName + "@" + ((listCheckLays[n] as IFeatureLayer).FeatureClass as IDataset).Name;// listCheckLays[n].Name;
                        node.Tag = listCheckLays[n];//����Ӧ�Ĳ�ŵ���Ӧ���Բ���Ϊ�ӽڵ�Ľڵ�TAG�ϣ��Ա�����ʹ��
                        node.Image = pDataTree.ImageList.Images[8];//�����ӽڵ��ͼƬ
                        node.DataKey = StrParlist[n];//����ֵ
                        aNode.Nodes.Add(node);

                    }

                    aNode.CollapseAll();
                    pDataTree.Nodes.Add(aNode);//����֯�õĽڵ��������
                    pFeature = pFeatCur.NextFeature();
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatCur);
                return State;
            }
            catch (System.Exception ex)
            {
                err = ex;
                return false;
            }
           
        }
        /// <summary>
        /// �ӱ߼�������� , int pur
        /// </summary>
        /// <param name="pAppHk"></param>
        public void DoJoinCheck(object Para,int pur)
        {
            //Application.DoEvents();
            Plugin.Application.IAppGISRef pAppHk = Para as Plugin.Application.IAppGISRef;
            char[] splite = new char[] { ' ' };//�ָ�Ĳ���
            Plugin.Application.IAppFormRef pAppFrm = pAppHk as Plugin.Application.IAppFormRef;
            if (pAppHk.DataTree.Nodes.Count == 0) return;

            //�����ӱ߼�����б�
            //DataTable table = new DataTable();
            //table.Columns.Add("Դ����", typeof(string));
            //table.Columns.Add("Ŀ������", typeof(string));
            //table.Columns.Add("��", typeof(object));

            pAppFrm.MainForm.Invoke(new ShowProgress(ShowProgressBar), new object[] { pAppFrm, true });

            int intAllCnt = pAppHk.DataTree.Nodes.Count;
            int intCnt = 0;
            foreach (DevComponents.AdvTree.Node aNode in pAppHk.DataTree.Nodes)
            {
                intCnt++;
                //ѡ�и�ͼ���ڵ�
                pAppFrm.MainForm.Invoke(new ChangeSelectNode(ChangeTreeSelectNode), new object[] { pAppHk.DataTree, aNode });
                string strMemo = "����" + aNode.Text + "(" + intCnt.ToString() + "/" + intAllCnt.ToString() + ")�ӱ߼��...";
                pAppFrm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { pAppFrm, strMemo });

                IFeature pFeature = aNode.Tag as IFeature;
                if (pFeature == null) continue;
                double distance = SearchValue;
                //������������Ϊ����ȡ�߽���л���
                //���Ƚ���������
                try
                {
                    if (pur == 1)
                    {
                        #region ����������Ϊ�ӱ߲���
                        
                        ITopologicalOperator topo = pFeature.Shape as ITopologicalOperator;
                        IGeometry geo = topo.Boundary;//�õ����ı߽�
                        topo.Simplify();

                        IGeometry NewGeo = null;
                        ITopologicalOperator NewTopo = null;
                        NewTopo = geo as ITopologicalOperator;
                        NewGeo = NewTopo.Buffer(distance);//�õ��߽���ٴεĽ��л���
                        NewTopo = NewGeo as ITopologicalOperator;
                        NewTopo.Simplify();

                        ////���巶Χ����������
                        NewGeo = NewTopo.Intersect(pFeature.Shape, esriGeometryDimension.esriGeometry2Dimension);
                        NewTopo = NewGeo as ITopologicalOperator;
                        NewTopo.Simplify(); //��߲�ѯЧ�� 

                        int intLayCnt = 0;
                        int intLays = aNode.Nodes.Count;
                        foreach (DevComponents.AdvTree.Node nodeChild in aNode.Nodes)
                        {
                            intLayCnt++;
                            //ѡ�и�ͼ���ڵ�
                            pAppFrm.MainForm.Invoke(new ChangeSelectNode(ChangeTreeSelectNode), new object[] { pAppHk.DataTree, nodeChild });
                            strMemo = "����" + aNode.Text + "(" + intCnt.ToString() + "/" + intAllCnt.ToString() + ")�е�" + nodeChild.Text + "(" + intLayCnt.ToString() + "/" + intLays.ToString() + ")�ӱ߼��...";
                            pAppFrm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { pAppFrm, strMemo });

                            ILayer pLay = nodeChild.Tag as ILayer;
                            if (pLay == null) continue;

                            //����ͼ���ȡ��Χ�ڵ�Ҫ��

                            List<IFeature> listLeftFeatures = GetFeaturesByGeometry(pLay, NewGeo);

                            if (listLeftFeatures == null) continue;
                            int allFeatsCnt = listLeftFeatures.Count;
                            if (allFeatsCnt == 0) continue;

                            pAppFrm.MainForm.Invoke(new ChangeProgress(ChangeProgressBar), new object[] { pAppFrm.ProgressBar, 0, allFeatsCnt, 0 });
                            pAppFrm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { pAppFrm, strMemo });

                            //ȡ�ڵ�������ŵ�ͼ����������������
                            List<string> tempAttribute = new List<string>();//�γ���������
                            string[] Attribute = nodeChild.DataKey.ToString().Split(splite);
                            for (int A = 0; A < Attribute.Length; A++)
                            {
                                tempAttribute.Add(Attribute[A]);
                            }
                            //������Ϊ���ս��нӱ߼��
                            DoJoinCheckByEdge(pFeature, NewGeo, pLay, listLeftFeatures, pAppFrm, AreaValue * 2, tempAttribute);
                            
                        }
                        
                        #endregion
                    }
                    else
                    {
                        //��ȡͼ����߻��巶Χ
                        #region ��ͼ����Ϊ����
                        IGeometry pLeftGeometry = GetBufferGeometryByMapFrame(pFeature.Shape, distance, 1);
                        //���巶Χ��ͼ����
                        ITopologicalOperator pTop = pLeftGeometry as ITopologicalOperator;
                        pLeftGeometry = pTop.Intersect(pFeature.Shape, esriGeometryDimension.esriGeometry2Dimension);
                        pTop = pLeftGeometry as ITopologicalOperator;
                        pTop.Simplify();  //��߲�ѯЧ��

                        //��ȡͼ���±߻��巶Χ
                        IGeometry pDownGeometry = GetBufferGeometryByMapFrame(pFeature.Shape, distance, 4);
                        //���巶Χ��ͼ����
                        pTop = pDownGeometry as ITopologicalOperator;
                        pDownGeometry = pTop.Intersect(pFeature.Shape, esriGeometryDimension.esriGeometry2Dimension);
                        pTop = pDownGeometry as ITopologicalOperator;
                        pTop.Simplify(); //��߲�ѯЧ�� 



                        int intLayCnt = 0;
                        int intLays = aNode.Nodes.Count;
                        foreach (DevComponents.AdvTree.Node nodeChild in aNode.Nodes)
                        {
                            intLayCnt++;
                            //ѡ�и�ͼ���ڵ�
                            pAppFrm.MainForm.Invoke(new ChangeSelectNode(ChangeTreeSelectNode), new object[] { pAppHk.DataTree, nodeChild });
                            strMemo = aNode.Text + "(" + intCnt.ToString() + "/" + intAllCnt.ToString() + ")�е�" + nodeChild.Text + "(" + intLayCnt.ToString() + "/" + intLays.ToString() + ")�ӱ߼��...";
                            pAppFrm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { pAppFrm, strMemo });

                            ILayer pLay = nodeChild.Tag as ILayer;
                            if (pLay == null) continue;

                            //����ͼ���ȡ��߷�Χ�ڵ�Ҫ��

                            List<IFeature> listLeftFeatures = GetFeaturesByGeometry(pLay, pLeftGeometry);
                            //����ͼ���ȡ�±߷�Χ�ڵ�Ҫ��
                            List<IFeature> listDownFeatures = GetFeaturesByGeometry(pLay, pDownGeometry);

                            if (listLeftFeatures == null && listDownFeatures == null) continue;
                            int allFeatsCnt = listLeftFeatures.Count + listDownFeatures.Count;
                            if (allFeatsCnt == 0) continue;

                            pAppFrm.MainForm.Invoke(new ChangeProgress(ChangeProgressBar), new object[] { pAppFrm.ProgressBar, 0, allFeatsCnt, 0 });
                            pAppFrm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { pAppFrm, strMemo });

                            //ȡ�ڵ�������ŵ�ͼ����������������
                            List<string> tempAttribute = new List<string>();//�γ���������
                            string[] Attribute = nodeChild.DataKey.ToString().Split(splite);
                            for (int A = 0; A < Attribute.Length; A++)
                            {
                                tempAttribute.Add(Attribute[A]);
                            }
                            //��ͼ����߽ӱ�ͼ����
                            DoJoinCheckByEdge(pFeature, pLeftGeometry, pLay, listLeftFeatures,  pAppFrm, distance * 2, tempAttribute);
                            //��ͼ���±߽ӱ�ͼ����
                            DoJoinCheckByEdge(pFeature, pDownGeometry, pLay, listDownFeatures, pAppFrm, distance * 2, tempAttribute);
                            
                        }

                        #endregion
                    }

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeature);
                }
                catch (Exception ex)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("�ݲ��������������", ex.Message);
                    FilterList.Clear();
                    pAppFrm.MainForm.Invoke(new ClearCheck(Clear), new object[] { pAppFrm });
                    pAppFrm.MainForm.Invoke(new ShowProgress(ShowProgressBar), new object[] { pAppFrm, false });
                    return;
                }
            }
            FilterList.Clear();
            pAppFrm.MainForm.Invoke(new ShowProgress(ShowProgressBar), new object[] { pAppFrm, false });

            //���ӱ߼�����б����ݰ������ؼ���
            //pAppFrm.MainForm.Invoke(new DeleteBindCilck(BindClick), new object[] { table, pAppHk });

            pAppHk.CurrentThread = null;
            //ѡ�м������
            pAppFrm.MainForm.Invoke(new SelectDataCheckGrid(SelectCheckGrid), pAppHk);
            pAppFrm.OperatorTips = "�����ɣ�";
            pAppFrm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "��ʾ", "������!" });
        }

        /// <summary>
        ///  ��ͼ��ĳ�����ϵĽӱ߼��
        /// </summary>
        /// <param name="pFeature">ͼ��Ҫ��</param>
        /// <param name="pEdgeGeometry">ͼ���߻�����ͼ���󽻷�Χ</param>
        /// <param name="listFeatures">�ӱ߼��ͼ��</param>
        /// <param name="listFeatures">�ӱ߼��ͼ��Ҫ��</param>
        /// <param name="table">�ӱ߼�����б�</param>
        /// <param name="pAppFrm">��Ӧ�ó������Ի�ȡ��������ʾ����</param>
        /// <param name="distance">�㻺��뾶</param>
        private void DoJoinCheckByEdge(IFeature pFeature, IGeometry pEdgeGeometry, ILayer pCheckLay, List<IFeature> listFeatures,  Plugin.Application.IAppFormRef pAppFrm, double distance, List<string> tempAttribute)
        {
            int intcnt = pAppFrm.ProgressBar.Value;
            foreach (IFeature pFeat in listFeatures)
            {
                intcnt++;
                pAppFrm.MainForm.Invoke(new ChangeProgress(ChangeProgressBar), new object[] { pAppFrm.ProgressBar, -1, -1, intcnt });

                //��ȡҪ��λ�ڷ�Χ�ڵĽڵ㣨�����ڷ�Χ�߽��ϣ�
                List<IPoint> listPoints = GetFeatPointsBufferByGeometry(pFeat, pEdgeGeometry);
                if (listPoints == null) continue;


                foreach (IPoint pTempPnt in listPoints)
                {
                    IGeometry pTempGeo = null;
                    try
                    {
                        //�Ե�Ϊ���ĵõ����巶Χ,����ͼ�����
                        ITopologicalOperator pTop = pTempPnt as ITopologicalOperator;
                        pTempGeo = pTop.Buffer(distance);
                        pTop = pTempGeo as ITopologicalOperator;
                        pTempGeo = pTop.Difference(pFeature.Shape);
                        pTop = pTempGeo as ITopologicalOperator;
                        pTop.Simplify(); //��߲�ѯЧ��
                    }
                    catch (Exception ex)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("�ݲ��������������", ex.Message);
                        return;
                    }

                    //��ȡ�ӱ�������������
                    string strCon = GetJoinCheckCon(pFeat, tempAttribute);
                    //��ȡͬ�㷶Χ�ڵ���ͬ����������Ҫ��
                    List<IFeature> pObjFeats = GetFeaturesByGeometry(pCheckLay, pTempGeo, strCon);
                    if (pObjFeats == null)
                    {
                        //System.Windows.Forms.MessageBox.Show("");
                    }

                    if (pObjFeats == null) continue;

                    IFeature pObjFeature = pObjFeats[0];
                    //������Ҫ�ؽӱߵ�Ҫ�ؿ���Ϊ����Ӧȡ����õ������Ҫ��
                    if (pObjFeats.Count > 1)
                    {
                        //...........................
                        int cntFeat = 0;
                        double minDistance = 0;
                        foreach (IFeature pTempFeat in pObjFeats)
                        {
                            cntFeat++;
                            IProximityOperator pProximityOperator = pTempPnt as IProximityOperator;
                            double pTemp = pProximityOperator.ReturnDistance(pTempFeat.Shape);
                            if (cntFeat == 1)
                            {
                                minDistance = pTemp;
                                pObjFeature = pTempFeat;
                            }
                            else if (minDistance > pTemp)
                            {
                                minDistance = pTemp;
                                pObjFeature = pTempFeat;
                            }
                        }
                    }

                    //�����Ҫ���ڽڵ�pTempPnt���Ƿ�ӱ���
                    if (IsJoinedOn(pFeat, pTempPnt, pObjFeature) == false)
                    {
                        //Ϊδ�ӱ�Ҫ�����
                        //.............................
                        //string temp = pCheckLay.Name + "��" + pFeat.OID.ToString() + pObjFeature.OID.ToString();
                        //string temp_1 = pCheckLay.Name + "��" + pObjFeature.OID.ToString() + pFeat.OID.ToString();
                        //DataRow row = table.NewRow();
                        //row[0] = pCheckLay.Name + "��" + pFeat.OID.ToString();
                        //row[1] = pObjFeature.OID.ToString();
                        //row[2] = pTempPnt;
                        //if (!FilterList.Contains(temp) && !FilterList.Contains(temp_1))
                        //{
                        //FilterList.Add(temp);
                        //table.Rows.Add(row);
                        //}
                        string mFeaClsName = (((pCheckLay as IFeatureLayer).FeatureClass) as IDataset).Name;
                        #region ����������������
                        //���������
                        double pMapx = pTempPnt.X;
                        double pMapy = pTempPnt.Y;

                        List<object> ErrorLst = new List<object>();
                        ErrorLst.Add("�ӱ߼��");//����������
                        ErrorLst.Add("�ӱ߼��");//��������
                        ErrorLst.Add("");  //�����ļ���
                        ErrorLst.Add(enumErrorType.�ӱ߼��.GetHashCode());//����id;
                        ErrorLst.Add("�ӱ߼�����");//��������
                        ErrorLst.Add(pMapx);    //...
                        ErrorLst.Add(pMapy);    //...
                        ErrorLst.Add(mFeaClsName);
                        ErrorLst.Add(pFeat.OID);
                        ErrorLst.Add(mFeaClsName);
                        ErrorLst.Add(pObjFeature.OID);
                        ErrorLst.Add(false);
                        ErrorLst.Add(System.DateTime.Now.ToString());

                        //���ݴ�����־
                        IDataErrInfo dataErrInfo = new DataErrInfo(ErrorLst);
                        DataErrTreatEvent dataErrTreatEvent = new DataErrTreatEvent(dataErrInfo);
                        DataErrTreat(hook.DataCheckGrid as object, dataErrTreatEvent);
                        #endregion

                        System.Runtime.InteropServices.Marshal.ReleaseComObject(pObjFeature);
                    }
                }
                
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeat);
            }
        }

        #region ���������ؼ���Ӧʵ��


        //������ʾ�Ի���
        private delegate void ShowForm(string strCaption, string strText);
        private void ShowErrForm(string strCaption, string strText)
        {
            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle(strCaption, strText);
        }

        //ѡ����ͼ�ڵ�
        private delegate void ChangeSelectNode(DevComponents.AdvTree.AdvTree aTree, DevComponents.AdvTree.Node aNode);
        private void ChangeTreeSelectNode(DevComponents.AdvTree.AdvTree aTree, DevComponents.AdvTree.Node aNode)
        {

            aTree.SelectedNode = aNode;
            if (aNode.PrevNode != null)
            {
                aNode.PrevNode.CollapseAll();
            }
            aNode.Expand();
            aTree.Refresh();
        }

        //ѡ�м�����б�
        private delegate void SelectDataCheckGrid(Plugin.Application.IAppGISRef pApp);
        private void SelectCheckGrid(Plugin.Application.IAppGISRef pApp)
        {
            DevComponents.DotNetBar.PanelDockContainer PanelTip = pApp.DataCheckGrid.Parent as DevComponents.DotNetBar.PanelDockContainer;
            if (PanelTip != null)
            {
                PanelTip.DockContainerItem.Selected = true;
            }
        }

        //�ı������
        private delegate void ChangeProgress(DevComponents.DotNetBar.ProgressBarItem pProgressBar, int min, int max, int value);
        private void ChangeProgressBar(DevComponents.DotNetBar.ProgressBarItem pProgressBar, int min, int max, int value)
        {
            if (min != -1)
            {
                pProgressBar.Minimum = min;
            }
            if (max != -1)
            {
                pProgressBar.Maximum = max;
            }
            pProgressBar.Value = value;
            pProgressBar.Refresh();
        }

        //���ƽ�������ʾ
        private delegate void ShowProgress(Plugin.Application.IAppFormRef pAppFrm, bool bVisible);
        private void ShowProgressBar(Plugin.Application.IAppFormRef pAppFrm, bool bVisible)
        {
            if (bVisible == true)
            {
                pAppFrm.ProgressBar.Visible = true;
            }
            else
            {
                pAppFrm.ProgressBar.Visible = false;
            }
        }

        //�ı�״̬����ʾ����
        private delegate void ShowTips(Plugin.Application.IAppFormRef pAppFrm, string strText);
        private void ShowStatusTips(Plugin.Application.IAppFormRef pAppFrm, string strText)
        {
            pAppFrm.OperatorTips = strText;
        }
        #endregion

        #region  �ӱ߼�鹫������


        /// <summary>
        /// ��mapcontrol�ϻ�ȡͼ����ϱ�
        /// </summary>
        /// <param name="pMap"></param>
        /// <param name="strName"></param>
        /// <returns></returns>
        public ILayer GetMapFrameLayer(IMap pMap, string strName, string C_name)
        {

            for (int i = 0; i < pMap.LayerCount; i++)
            {
                ILayer pLayer = pMap.get_Layer(i);
                IGroupLayer pGroupLayer = pLayer as IGroupLayer;
                if (pGroupLayer == null) continue;
                if (pGroupLayer.Name != strName) continue;
                ICompositeLayer pCompositeLayer = pGroupLayer as ICompositeLayer;
                if (pCompositeLayer.Count == 0) return null;
                ILayer layer = null;
                for (int n = 0; n < pCompositeLayer.Count; n++)
                {
                    layer = pCompositeLayer.get_Layer(n);
                    if (layer.Name == C_name) break;

                }
                return layer;
            }
            return null;
        }

        /// <summary>
        /// ��ȡͼ����ĳ�߻����Χ
        /// </summary>
        /// <param name="pGeometry">һ��ͼ��ͼ��</param>
        /// <param name="distance">����뾶</param>
        /// <param name="state">����1-��2-�ң�3-�ϣ�4-��</param>
        /// <returns></returns>
        public IGeometry GetBufferGeometryByMapFrame(IGeometry pGeometry, double distance, int state)
        {
            IEnvelope pEnvelope = pGeometry.Envelope;
            IPolyline polyline = new PolylineClass();

            switch (state)
            {
                case 1:      //���
                    polyline.FromPoint = pEnvelope.UpperLeft;
                    polyline.ToPoint = pEnvelope.LowerLeft;
                    break;
                case 2:     //�ұ�
                    polyline.FromPoint = pEnvelope.UpperRight;
                    polyline.ToPoint = pEnvelope.LowerRight;
                    break;
                case 3:     //�ϱ�
                    polyline.FromPoint = pEnvelope.UpperLeft;
                    polyline.ToPoint = pEnvelope.UpperRight;
                    break;
                case 4:     //�±�
                    polyline.FromPoint = pEnvelope.LowerLeft;
                    polyline.ToPoint = pEnvelope.LowerRight;
                    break;
                default:
                    return null;
            }

            ITopologicalOperator topo = polyline as ITopologicalOperator;
            IGeometry geo = topo.Buffer(distance);
            topo = geo as ITopologicalOperator;
            topo.Simplify();
            return geo;

        }

        /// <summary>
        /// ��ȡͼ��λ�ڷ�Χ�ڵ�Ҫ��(�����ཻ�������Ӵ�)
        /// </summary>
        /// <param name="pLay">ͼ��</param>
        /// <param name="pGeometry">��Χ����</param>
        /// <returns></returns>
        public List<IFeature> GetFeaturesByGeometry(ILayer pLay, IGeometry pGeometry)
        {
            IFeatureLayer pFeatLay = pLay as IFeatureLayer;
            if (pFeatLay == null) return null;

            IFeatureClass pFeatCls = pFeatLay.FeatureClass;

            //��Χ����,��ȥ����ϵΪ�Ӵ���Ҫ�� 
            //double xdbl = pGeometry.Envelope.XMax - pGeometry.Envelope.XMin;
            //double ydbl = pGeometry.Envelope.YMax - pGeometry.Envelope.YMin;
            //double distance = xdbl;
            //if (xdbl > ydbl)
            //{
            //    distance = ydbl;
            //}
            //ITopologicalOperator pTop = pGeometry as ITopologicalOperator;
            ////IGeometry pTempGeo = pTop.Buffer(0 - distance / 10);
            //IGeometry pTempGeo = pTop.Buffer(-0.5);
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.Geometry = pGeometry;
            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            pSpatialFilter.GeometryField = "SHAPE";

            List<IFeature> listFeats = new List<IFeature>();
            IFeatureCursor pFeatCur = pFeatCls.Search(pSpatialFilter, false);
            IFeature pFeat = pFeatCur.NextFeature();
            while (pFeat != null)
            {
                listFeats.Add(pFeat);
                pFeat = pFeatCur.NextFeature();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatCur);
            return listFeats;
        }

        /// <summary>
        /// ��ȡͼ�㷶Χ�ڵ���ͬ����������Ҫ��(�����ཻ�������Ӵ�)
        /// </summary>
        /// <param name="listLays">ͼ��</param>
        /// <param name="pGeometry">��Χ����</param>
        /// <param name="pGeometry">��������</param>
        /// <returns></returns>
        public List<IFeature> GetFeaturesByGeometry(ILayer pLay, IGeometry pGeometry, string strCon)
        {
            try
            {
                IFeatureLayer pFeatLay = pLay as IFeatureLayer;
                if (pFeatLay == null) return null;
                IFeatureClass pFeatCls = pFeatLay.FeatureClass;

                //��Χ����,��ȥ����ϵΪ�Ӵ���Ҫ�� 
                double xdbl = pGeometry.Envelope.XMax - pGeometry.Envelope.XMin;
                double ydbl = pGeometry.Envelope.YMax - pGeometry.Envelope.YMin;
                double distance = xdbl;
                if (xdbl > ydbl)
                {
                    distance = ydbl;
                }
                ITopologicalOperator pTop = pGeometry as ITopologicalOperator;
                IGeometry pTempGeo = pTop.Buffer(0 - distance / 10);

                ISpatialFilter pSpatialFilter = new SpatialFilterClass();
                pSpatialFilter.Geometry = pTempGeo;
                pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                pSpatialFilter.GeometryField = "SHAPE";

                IQueryFilter pQueryFilter = pSpatialFilter as IQueryFilter;
                pQueryFilter.WhereClause = strCon;

                List<IFeature> listValue = new List<IFeature>();
                IFeatureCursor pFeatCur = pFeatCls.Search(pSpatialFilter, false);
                IFeature pFeat = pFeatCur.NextFeature();
                while (pFeat != null)
                {
                    listValue.Add(pFeat);
                    pFeat = pFeatCur.NextFeature();
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatCur);

                if (listValue.Count == 0) return null;
                return listValue;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// ��ȡҪ��λ�ڷ�Χ�ڵĽڵ㣨�����ڷ�Χ�߽��ϣ�
        /// </summary>
        /// <param name="pFeat">Ҫ��</param>
        /// <param name="pGeometry">��Χ</param>
        /// <returns>����Ҫ���ϵĽڵ㣨λ�ڷ�Χ�ڻ��ڷ�Χ�߽��ϣ�</returns>
        public List<IPoint> GetFeatPointsBufferByGeometry(IFeature pFeat, IGeometry pGeometry)
        {
            List<IPoint> listPoints = new List<IPoint>();
            IPointCollection pPointCollection = pFeat.Shape as IPointCollection;
            //��Χ����,�Ի�ȡ��ϵΪ�ڷ�Χ�߽��ϵĵ�
            double xdbl = pGeometry.Envelope.XMax - pGeometry.Envelope.XMin;
            double ydbl = pGeometry.Envelope.YMax - pGeometry.Envelope.YMin;
            double distance = xdbl;
            if (xdbl > ydbl)
            {
                distance = ydbl;
            }
            ITopologicalOperator pTop = pGeometry as ITopologicalOperator;
            //IGeometry pTempGeo = pTop.Buffer(distance / 100);
            IGeometry pTempGeo = pTop.Buffer(1);//��ʼ����һ��Ĭ��ֵ1
            IRelationalOperator pRelation = pTempGeo as IRelationalOperator;
            if (pPointCollection == null || pRelation == null) return null;

            int cnt = pPointCollection.PointCount;
            //���������Ӧȥ��β��
            if (pFeat.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
            {
                cnt = pPointCollection.PointCount - 1;
            }

            for (int i = 0; i < cnt; i++)
            {
                IGeometry geo = pPointCollection.get_Point(i) as IGeometry;
                if (pRelation.Contains(geo))
                {
                    listPoints.Add(pPointCollection.get_Point(i));
                }
            }

            if (listPoints.Count == 0) return null;
            return listPoints;
        }

        /// <summary>
        /// ��ȡ�ӱ���������
        /// </summary>
        /// <param name="pFeat">�ӱ߲���Ҫ��</param>
        /// <param name="listFields">�ӱ߲����ֶ�</param>
        /// <returns></returns>
        public string GetJoinCheckCon(IFeature pFeat, List<string> listFields)
        {
            if (pFeat == null || listFields == null) return "";
            if (listFields.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pFeat.Fields.FieldCount; i++)
            {
                IField pField = pFeat.Fields.get_Field(i);
                if (!listFields.Contains(pField.Name)) continue;

                if (sb.Length != 0)
                {
                    sb.Append(" and ");
                }

                switch (pField.Type)
                {
                    case esriFieldType.esriFieldTypeDate:
                        sb.Append(pField.Name + "=#" + pFeat.get_Value(i).ToString() + "#");
                        break;
                    case esriFieldType.esriFieldTypeString:
                        sb.Append(pField.Name + "='" + pFeat.get_Value(i).ToString() + "'");
                        break;
                    default:
                        sb.Append(pField.Name + "=" + pFeat.get_Value(i).ToString());
                        break;
                }
            }

            return sb.ToString();
        }


        /// <summary>
        /// �����Ҫ���ڽڵ�pPnt���Ƿ�ӱ���
        /// </summary>
        /// <param name="pPnt"> ԴҪ��</param>
        /// <param name="pPnt"> ԴҪ���Ͻڵ�</param>
        /// <param name="pFeature">Ŀ��Ҫ��</param>
        /// <returns></returns>
        private bool IsJoinedOn(IFeature pOrgFeature, IPoint pPnt, IFeature pObjFeature)
        {
            //Ŀ��Ҫ�ؾ���ԴҪ�ؽڵ�����ĵ��������Ҫ�ؽڵ�����һ��
            IProximityOperator pProximity = pObjFeature.Shape as IProximityOperator;
            IPoint pNearestPnt = pProximity.ReturnNearestPoint(pPnt, esriSegmentExtension.esriNoExtension);
            if (pPnt.X == pNearestPnt.X && pPnt.Y == pNearestPnt.Y)
            {
                return true;
            }
            else
            {
                //���������Ӧ�ж�Ŀ��Ҫ�ؾ���ԴҪ�ؽڵ�����ĵ��Ƿ���ԴҪ����
                if (pOrgFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                {
                    pProximity = pOrgFeature.Shape as IProximityOperator;
                    IPoint pNearestPntTemp = pProximity.ReturnNearestPoint(pNearestPnt, esriSegmentExtension.esriNoExtension);
                    if (pNearestPntTemp.X == pNearestPnt.X && pNearestPntTemp.Y == pNearestPnt.Y) return true;
                }

                return false;
            }
        }

        #endregion

        #endregion

        //��������
        public void GeoDataChecker_DataErrTreat(object sender, DataErrTreatEvent e)
        {
            //�û������ϱ��ֳ�������Ϣ
            DevComponents.DotNetBar.Controls.DataGridViewX pDataGrid = sender as DevComponents.DotNetBar.Controls.DataGridViewX;
            if (_ResultTable == null) return;
            DataRow newRow = _ResultTable.NewRow();
            newRow["��鹦����"] = e.ErrInfo.FunctionName;
            newRow["��������"] = GeoDataChecker.GetErrorTypeString(Enum.Parse(typeof(enumErrorType), e.ErrInfo.ErrID.ToString()).ToString());
            newRow["��������"] = e.ErrInfo.ErrDescription;
            newRow["����ͼ��1"] = e.ErrInfo.OriginClassName;
            newRow["Ҫ��OID1"] = e.ErrInfo.OriginFeatOID;
            newRow["����ͼ��2"] = e.ErrInfo.DestinationClassName;
            newRow["Ҫ��OID2"] = e.ErrInfo.DestinationFeatOID;
            newRow["���ʱ��"] = e.ErrInfo.OperatorTime;
            newRow["��λ��X"] = e.ErrInfo.MapX;
            newRow["��λ��Y"] = e.ErrInfo.MapY;
            newRow["�����ļ���"] = e.ErrInfo.DataFileName;
            _ResultTable.Rows.Add(newRow);

            pDataGrid.Update();

            //��������excle

            InsertRowToExcel(e);
        }

        //�����ݼ��������Excel�� 
        private void InsertRowToExcel(DataErrTreatEvent e)
        {
            if (_ErrDbCon != null && _ErrTableName != "")
            {
                SysCommon.DataBase.SysTable sysTable = new SysCommon.DataBase.SysTable();
                sysTable.DbConn = _ErrDbCon;
                sysTable.DBConType = SysCommon.enumDBConType.OLEDB;
                sysTable.DBType = SysCommon.enumDBType.ACCESS;
                string sql = "insert into " +_ErrTableName +
                    "(�����ļ�·��,��鹦����,��������,��������,����ͼ��1,����OID1,����ͼ��2,����OID2,��λ��X,��λ��Y,���ʱ��) values(" +
                    "'" + e.ErrInfo.DataFileName + "','" + e.ErrInfo.FunctionName + "','" + GeoDataChecker.GetErrorTypeString(Enum.Parse(typeof(enumErrorType), e.ErrInfo.ErrID.ToString()).ToString()) + "','" + e.ErrInfo.ErrDescription + "','" + e.ErrInfo.OriginClassName + "','" + e.ErrInfo.OriginFeatOID.ToString() + "','" +
                    e.ErrInfo.DestinationClassName + "','" + e.ErrInfo.DestinationFeatOID.ToString() + "'," + e.ErrInfo.MapX + "," + e.ErrInfo.MapY + ",'" + e.ErrInfo.OperatorTime + "')";

                Exception errEx = null;
                sysTable.UpdateTable(sql, out errEx);
                if (errEx != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "д��Excel�ļ�����");
                    return;
                }
            }
        }



        #region ������ͼ��غ���
        //����������ͼ
        public void IntialTree(DevComponents.AdvTree.AdvTree aTree)
        {
            DevComponents.AdvTree.ColumnHeader aColumnHeader;
            aColumnHeader = new DevComponents.AdvTree.ColumnHeader();
            aColumnHeader.Name = "FCName";
            aColumnHeader.Text = "ͼ����";
            aColumnHeader.Width.Relative = 50;
            aTree.Columns.Add(aColumnHeader);

            aColumnHeader = new DevComponents.AdvTree.ColumnHeader();
            aColumnHeader.Name = "NodeRes";
            aColumnHeader.Text = "���";
            aColumnHeader.Width.Relative = 45;
            aTree.Columns.Add(aColumnHeader);
        }
        //����ѡ�����ڵ���ɫ
       public void setNodeColor(DevComponents.AdvTree.AdvTree aTree)
        {
            DevComponents.DotNetBar.ElementStyle elementStyle = new DevComponents.DotNetBar.ElementStyle();
            elementStyle.BackColor = Color.FromArgb(255, 244, 213);
            elementStyle.BackColor2 = Color.FromArgb(255, 216, 105);
            elementStyle.BackColorGradientAngle = 90;
            elementStyle.Border = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderBottomWidth = 1;
            elementStyle.BorderColor = Color.DarkGray;
            elementStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderLeftWidth = 1;
            elementStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderRightWidth = 1;
            elementStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderTopWidth = 1;
            elementStyle.BorderWidth = 1;
            elementStyle.CornerDiameter = 4;
            elementStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            aTree.NodeStyleSelected = elementStyle;
            aTree.DragDropEnabled = false;
        }
        //������ͼ�ڵ�
        public  DevComponents.AdvTree.Node CreateAdvTreeNode(DevComponents.AdvTree.NodeCollection nodeCol, string strText, string strName, Image pImage, bool bExpand)
        {

            DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node();
            node.Text = strText;
            node.Image = pImage;
            if (strName != null)
            {
                node.Name = strName;
            }

            if (bExpand == true)
            {
                node.Expand();
            }
            //�����ͼ�нڵ�
            DevComponents.AdvTree.Cell aCell = new DevComponents.AdvTree.Cell();
            aCell.Images.Image = null;
            node.Cells.Add(aCell);
            nodeCol.Add(node);
            return node;
        }

        //�����ͼ�ڵ���
        public DevComponents.AdvTree.Cell CreateAdvTreeCell(DevComponents.AdvTree.Node aNode, string strText, Image pImage)
        {
            DevComponents.AdvTree.Cell aCell = new DevComponents.AdvTree.Cell(strText);
            aCell.Images.Image = pImage;
            aNode.Cells.Add(aCell);

            return aCell;
        }

        //Ϊ���ݴ�����ͼ�ڵ���Ӵ�����״̬
        public void ChangeTreeSelectNode(DevComponents.AdvTree.Node aNode, string strRes, bool bClear)
        {
            if (aNode == null)
            {
                hook.DataTree.SelectedNode = null;
                hook.DataTree.Refresh();
                return;
            }

            hook.DataTree.SelectedNode = aNode;
            if (bClear)
            {
                hook.DataTree.SelectedNode.Nodes.Clear();
            }
            hook.DataTree.SelectedNode.Cells[1].Text = strRes;
            hook.DataTree.Refresh();
        }
        #endregion

        #region ��������ʾ
        //���ƽ�������ʾ
        public void ShowProgressBar(bool bVisible)
        {
            if (bVisible == true)
            {
                (hook as Plugin.Application.IAppFormRef).ProgressBar.Visible = true;
            }
            else
            {
                (hook as Plugin.Application.IAppFormRef).ProgressBar.Visible = false;
            }
        }
        //�޸Ľ�����
        private void ChangeProgressBar1(DevComponents.DotNetBar.ProgressBarItem pProgressBar, int min, int max, int value)
        {
            if (min != -1)
            {
                pProgressBar.Minimum = min;
            }
            if (max != -1)
            {
                pProgressBar.Maximum = max;
            }
            pProgressBar.Value = value;
            pProgressBar.Refresh();
        }


        //�ı�״̬����ʾ����
        private void ShowStatusTips(string strText)
        {
            (hook as Plugin.Application.IAppFormRef).OperatorTips = strText;
        }
        #endregion
    }
}
