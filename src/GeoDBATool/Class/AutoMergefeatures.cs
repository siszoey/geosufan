using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using System.Collections;

namespace GeoDBATool
{
    /// <summary>
    /// �ں��ύ�¼�ί��
    /// </summary>
    /// <param name="FeatureClassName">�����ںϵ�Ҫ��������</param>
    /// <param name="Ŀ��Ҫ��ID">�ںϺ��Ŀ��Ҫ��OID</param>
    /// <param name="SourOID">���ںϵ�Ҫ��OID����OID����ʧ��</param>
    public delegate void AutoMergefeaturesCommitEventHander(string FeatureClassName, int DesOID,int SourOID);

    /// <summary>
    /// �ںϴ����¼�ί��
    /// </summary>
    /// <param name="ErrorString">������Ϣ�ı�</param>
    public delegate void AutoMergefeaturesReturnErr(string ErrorString);

    /// <summary>
    /// �Զ��ں��ࣨĿǰֻ���GDB���ݣ�
    /// </summary>
    public class AutoMergefeatures
    {

        private IFeatureWorkspace _DesWorkspace=null;
        private FeatureSearcheProperties[] _SearchInfo=null;
        /// <summary>
        /// �Ƿ��������־��
        /// </summary>
        private bool _TreatUpdateLog = false;

        /// <summary>
        /// ��ʼ���ںϲ���
        /// </summary>
        /// <param name="SearchInfo">Ҫ��������Ϣ��ͼ�㣬�ݲ�ֶεȵȣ�</param>
        /// <param name="DesWorkspace">��Ҫ�����ںϵ�Ŀ�����</param>
        public AutoMergefeatures(FeatureSearcheProperties[] SearchInfo,IFeatureWorkspace DesWorkspace)
        {
            ///����ں�Ŀ�겻Ϊ��
            if (DesWorkspace != null && SearchInfo != null)
            {
                this._DesWorkspace=DesWorkspace;
                this._SearchInfo=SearchInfo;
            }
        }

        public AutoMergefeatures(FeatureSearcheProperties[] SearchInfo, IFeatureWorkspace DesWorkspace,bool TreatUpdateLog)
        {
            ///����ں�Ŀ�겻Ϊ��
            if (DesWorkspace != null && SearchInfo != null)
            {
                this._DesWorkspace = DesWorkspace;
                this._SearchInfo = SearchInfo;
                this._TreatUpdateLog = TreatUpdateLog;
            }
        }

        /// <summary>
        /// ִ���ںϲ���
        /// </summary>
        /// <param name="_TreatUpdateLog">�Ƿ���и�����־�����������</param>
        public void ExcuteMerge(bool _TreatUpdateLog)
        {
            if (!_TreatUpdateLog)  ///������������־�����������еĴ���
            {
                ///�����Դ��Ч��������ںϲ���
                if (this._DesWorkspace != null && this._SearchInfo != null)
                {
                    try
                    {
                        foreach (FeatureSearcheProperties FeatureSearcheProperty in this._SearchInfo)
                        {
                            ///�����Ҫ������Ҫ����
                            string pFeatureClassName = FeatureSearcheProperty.FeatureClass;
                            IFeatureClass pFeatureClass = this._DesWorkspace.OpenFeatureClass(pFeatureClassName);

                            if (pFeatureClass != null)
                            {
                                IFields pFields = FeatureSearcheProperty.CompareFields;
                                if (FeatureSearcheProperty.SpatialRange != null)
                                {
                                    ///��ȡ�����ںϵ�Ҫ���α�
                                    ///
                                    IFeatureCursor pFeatureCursor = GetFeatureCursor4Merge(pFeatureClass, FeatureSearcheProperty.SpatialRange, FeatureSearcheProperty.SearchBuffer);
                                    Dictionary<int, List<int>> TouchedGroup = GetTouchedGroup(pFeatureClass,pFeatureCursor, FeatureSearcheProperty.CompareFields);

                                    //�ͷ�cursor
                                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
                                }
                                else
                                {
                                    this.ErrOcur("�ںϷ�Χ��ȡʧ��!");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //*******************************************************************
                        //�쳣��־
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

                        this.ErrOcur(e.Message);
                        
                    }
                }
            }
            else  ///����������־���е���ؼ�¼
            {
                ///�����Դ��Ч��������ںϲ���
                if (this._DesWorkspace != null && this._SearchInfo != null)
                {

                }
            }
        }

        /// <summary>
        /// �����α��е�Ҫ�ػ�ȡ�ռ��ϵΪTouch���ֶ���ͬ��Ҫ�����һ��
        /// </summary>
        /// <param name="pFeatureCursor">Ҫ���α�</param>
        /// <param name="iFields">�ֶ���</param>
        ///  <param name="iFields">Ŀ��Ҫ����</param>
        /// <returns>������Ҫ�ںϵ�Ҫ����ϣ�keyΪ�ںϺ�Ҫ��oid��valueΪ���ں�Ҫ��oid</returns>
        private Dictionary<int, List<int>> GetTouchedGroup(IFeatureClass DesFLC , IFeatureCursor pFeatureCursor, IFields CompareFields)
        {

            Dictionary<int, List<int>> pTouchedgroup = new Dictionary<int, List<int>>();
            IFeature pSourceFeature = pFeatureCursor.NextFeature();

            while (pSourceFeature != null)
            {
                ISpatialFilter pSpatialFilter = new SpatialFilterClass();
                try
                {
                    ///�ռ��ϵ����
                    ///
                    pSpatialFilter.GeometryField = "SHAPE";
                    pSpatialFilter.Geometry = pSourceFeature.Shape;
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelTouches;

                    ///������������
                    ///
                    string pWhereClause = "";

                    if (CompareFields != null) ///���ֶαȶ�����
                    {
                        ///�����ֶ��γɲ�ѯ���
                        for (int i = 0; i < CompareFields.FieldCount; i++)
                        {
                            IField pField = CompareFields.get_Field(i);

                            if (pWhereClause == "")///����ǵ�һ���ֶ�
                            {
                                switch (pField.Type)
                                {
                                    case esriFieldType.esriFieldTypeString:

                                        pWhereClause = pField.Name + " = '" + pSourceFeature.get_Value(pSourceFeature.Fields.FindField(pField.Name)) + "'";
                                        break;
                                    default:
                                        pWhereClause = pField.Name + " = " + pSourceFeature.get_Value(pSourceFeature.Fields.FindField(pField.Name));
                                        break;
                                }

                            }
                            else///������ǵ�һ���ֶ�
                            {
                                switch (pField.Type)
                                {
                                    case esriFieldType.esriFieldTypeString:

                                        pWhereClause = pWhereClause + " and " + pField.Name + " = '" + pSourceFeature.get_Value(pSourceFeature.Fields.FindField(pField.Name)) + "'";
                                        break;
                                    default:
                                        pWhereClause = pWhereClause + " and " + pField.Name + " = " + pSourceFeature.get_Value(pSourceFeature.Fields.FindField(pField.Name));
                                        break;
                                }
                            }
                        }
                    }

                    ///���pTouchedgroup����
                    ///
                    IFeatureCursor pMatchedFeatureCursor = DesFLC.Search(pSpatialFilter, false);
                    if (pMatchedFeatureCursor!=null)
                    {
                        IFeature pMatchedFeature = pMatchedFeatureCursor.NextFeature();
                        List<int> MatchedFeatures = new List<int>();

                        while (pMatchedFeature != null)
                        {
                            if (!pTouchedgroup.ContainsKey(pSourceFeature.OID))
                            {
                                MatchedFeatures.Add(pMatchedFeature.OID);
                            }
                            else
                            {
                                break;
                            }
                            pMatchedFeature = pMatchedFeatureCursor.NextFeature();
                        }

                        if (!pTouchedgroup.ContainsKey(pSourceFeature.OID))
                        {
                            pTouchedgroup.Add(pSourceFeature.OID, MatchedFeatures);
                        }
                    }

                    //�ͷ�cursor
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pMatchedFeatureCursor);

                    pTouchedgroup = Regroup(pTouchedgroup);
                }
                catch (Exception e)
                {

                    //*******************************************************************
                    //Exception Log
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

                    this.ErrOcur(e.Message);
                    return null;
                    
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pSpatialFilter);
                }

                pSourceFeature = pFeatureCursor.NextFeature();
            }

            return pTouchedgroup;
        }

        /// <summary>
        /// ����ƥ���Ҫ���б��������ںϲ���
        /// </summary>
        /// <param name="pTouchedgroup">ԭʼҪ���б�</param>
        /// <returns></returns>
        private Dictionary<int, List<int>> Regroup(Dictionary<int, List<int>> pTouchedgroup)
        {
            Dictionary<int, List<int>> AfterRegroup = new Dictionary<int, List<int>>();
            

            foreach (KeyValuePair<int,List<int>> Item in pTouchedgroup)
            {
                List<int> pList = Item.Value as List<int>;
                List<int> pHash = new List<int>();

                //if (!pHash.Contains(Item.Key))
                //{
                //    pHash.Add(Item.Key);
                //}

                Recursive(pTouchedgroup, pHash, pList);

                if (pHash.Contains(Item.Key))
                {
                    pHash.Remove(Item.Key);
                }

                AfterRegroup.Add(Item.Key, pHash);
            }
            return AfterRegroup;
        }

        /// <summary>
        /// �ݹ��ȡ�б��е�ֵ
        /// </summary>
        /// <param name="pTouchedgroup"></param>
        /// <param name="pHash"></param>
        /// <param name="pList"></param>
        private void Recursive(Dictionary<int, List<int>> pTouchedgroup, List<int> pHash, List<int> pList)
        {
            foreach (int var in pList)
            {
                ///���Item��ֵ�����ڼ��ϵ�Keys��
                if (pTouchedgroup.ContainsKey(var))
                {
                    Recursive(pTouchedgroup, pHash, pTouchedgroup[var]);
                    pTouchedgroup.Remove(var);
                }

                if (!pHash.Contains(var))
                {
                    pHash.Add(var);
                }

            }
        }


        /// <summary>
        /// ��ȡ�����ںϵ�Ҫ���α�
        /// </summary>
        /// <param name="pFeatureClass">�����ںϵ�Ҫ����</param>
        /// <param name="pRangePolygon">�ں������ķ�Χ</param>
        ///  <param name="pSearchBuffer">�ں���������뾶</param>
        /// <returns></returns>
        private IFeatureCursor GetFeatureCursor4Merge(IFeatureClass pFeatureClass, IGeometry pRangePolygon, double pSearchBuffer)
        {

            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            try
            {
                ITopologicalOperator pTopologicalOperator = pRangePolygon as ITopologicalOperator;
                IGeometry Boundery = pTopologicalOperator.Boundary;
                pTopologicalOperator = Boundery as ITopologicalOperator;

                IGeometry Buffered = pTopologicalOperator.Buffer(pSearchBuffer);

                pSpatialFilter.GeometryField = "SHAPE";
                pSpatialFilter.Geometry = Buffered;
                pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                IFeatureCursor pFeatureCursor = pFeatureClass.Search(pSpatialFilter, false);
                if (pFeatureCursor != null)
                {
                    return pFeatureCursor;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                //*******************************************************************
                //Exception Log
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
                
                this.ErrOcur(e.Message);
                return null;
            }
            finally
            {
                
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pSpatialFilter);
            }
           
        }

        /// <summary>
        /// �����ں�Ҫ���¼�
        /// </summary>
        //public event AutoMergefeaturesCommitEventHander CommitMerge;

        public event AutoMergefeaturesReturnErr ErrOcur;

        /// <summary>
        /// �ͷ���Դ
        /// </summary>
        public void Dispose()
        {
            this._DesWorkspace = null;
            this._SearchInfo = null;
        }

        /// <summary>
        /// Ҫ���ں� ���Ƿ����
        /// </summary>
        /// <param name="pFeatureClass">��Ҫ�ںϵ�ͼ��</param>
        /// <param name="newOID">�ںϺ󱣴��Ҫ��OID</param>
        /// <param name="oldOIDLst">��Ҫ�ںϵ�Ҫ��OID</param>
        private  void MergeFeatures(IFeatureClass pFeatureClass, int newOID, List<int> oldOIDLst)
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

    /// <summary>
    /// ����Ҫ����������
    /// </summary>
    public class FeatureSearcheProperties 
    {

        private IGeometry _SpatialRange = null;
        private string _FeatureClass = null;
        private IFields _CompareFields = null;
        private double  _SearchBuffer = 0;

        /// <summary>
        /// ��ʼ�����Ա
        /// </summary>
        public FeatureSearcheProperties()
        { 

        }

        /// <summary>
        /// ��ʼ�����Ա(������)
        /// </summary>
        public FeatureSearcheProperties(IGeometry SpatialRange, string FeatureClass, IFields CompareFields)
        {
            this._SpatialRange = SpatialRange;
            this._FeatureClass = FeatureClass;
            this._CompareFields = CompareFields;
                
        }
        /// <summary>
        /// ��ȡ�ں������ı߽緶Χ
        /// </summary>
        public IGeometry SpatialRange
        {
            get
            {
                return this._SpatialRange;
            }
            set
            {
                this._SpatialRange = value;
            }
        }

        /// <summary>
        /// ��ȡ��Ҫ�����ںϵ�Ҫ����
        /// </summary>
        public string FeatureClass
        {
            get
            {
                return this._FeatureClass;
            }
            set
            {
                this._FeatureClass = value;
            }
        }

        /// <summary>
        /// �����ںϱȽϵ������ֶ�
        /// </summary>
        public IFields CompareFields
        {
            get
            {
                return this._CompareFields;
            }
            set
            {
                this._CompareFields = value;
            }
        }

        /// <summary>
        /// �����ݲ�
        /// </summary>
        public double  SearchBuffer
        {
            get
            {
                return this._SearchBuffer;
            }
            set
            {
                this._SearchBuffer = value;
            }
        }
    }


}
