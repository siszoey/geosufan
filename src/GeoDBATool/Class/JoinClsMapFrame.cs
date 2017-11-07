using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;


using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

using System.Xml;
namespace GeoDBATool
{
    /*
     * ClsMapFrame(��׼ͼ��)��ʵ����IMapframe ���ӱ�ͼ�����ӿ�
     * ��ȡ��Դ�ӱ�Ҫ�ػ�������ͼ���ڣ���Ŀ��Ҫ�ػ�������ͼ���⣩��һ��׼ͼ�������±߽�Ļ������ֱ���ͼ����Χ�󽻡����ķ�Χ
     * ���õ�ǰ�ӱ�ͼ����ʹ��GetOriFrame����ͨ��ͼ�����ֶ�����ֵ�������ã�Ҳ��ͨ��OriMapFrame����ֱ�Ӹ�ֵ
     * Getborderline����ȡ�ӱ߽߱磩�ķ���ʵ�ֵ��ǻ�ȡ��׼ͼ������߽硢�±߽磨�������ڲ�����Χ�ӱߣ� 
     * 
    */
    class ClsMapFrame : IMapframe
    {
        public ClsMapFrame()////���캯��
        {
            this._mapframefea = null;
            this._OriMapFrame = null;
        }
        private IFeatureClass _mapframefea;
        public IFeatureClass MapFrameFea
        {
            get { return _mapframefea; }
            set { this._mapframefea = value; }
        }
        private IFeature _OriMapFrame;
        public IFeature OriMapFrame
        {
            get { return this._OriMapFrame; }
            set { this._OriMapFrame = value; }
        }
        public IFeature GetOriFrame(string OriFrameNoValue, string OriFrameNoField)
        {
            IFeature Fea = this.GetFrame(OriFrameNoValue, OriFrameNoField);
            _OriMapFrame = Fea;
            return Fea;
        }
        public void GetBufferArea(double dBufferRadius, out IGeometry OriBufferArea, out IGeometry DesBufferArea)
        {
            OriBufferArea = null;
            DesBufferArea = null;
            if (null == this._OriMapFrame)
                return;
            ////��ȡԴͼ������߽硢�±߽�Ļ�������
            try
            {
                IGeometry LBufferArea = GetBufferGeometryByMapFrame(_OriMapFrame.Shape, dBufferRadius, 1);
                IGeometry DBufferArea = GetBufferGeometryByMapFrame(_OriMapFrame.Shape, dBufferRadius, 4);
                ITopologicalOperator topo = LBufferArea as ITopologicalOperator;
                IGeometry UnionArea = topo.Union(DBufferArea);
                ////��ȡԴͼ����Ŀ��ͼ���Ľӱ߻�����
                topo = UnionArea as ITopologicalOperator;
                OriBufferArea = topo.Intersect((IGeometry)_OriMapFrame.Shape, esriGeometryDimension.esriGeometry2Dimension);
                DesBufferArea = topo.Difference((IGeometry)_OriMapFrame.Shape);
            }
            catch (Exception Error)
            {
                throw Error;
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(Error, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(Error, null, DateTime.Now);
                }
                //********************************************************************
            }
            //OriBufferArea = UnionArea;
            //DesBufferArea = UnionArea;

        }
        private IFeature GetFrame(string OriFrameNoValue, string OriFrameNoField)
        {
            if (null == this._mapframefea)
                return null;
            else
            {
                try
                {
                    IFeatureCursor FeatureCursor = _mapframefea.Search(null, false);
                    IFeature fea = FeatureCursor.NextFeature();
                    while (null != fea)
                    {
                        int index = -1;
                        index = fea.Fields.FindField(OriFrameNoField);
                        if (index > 0)
                        {
                            string MapFrameno = fea.get_Value(index).ToString();
                            if (OriFrameNoValue == MapFrameno)
                                return fea;
                        }
                        fea = FeatureCursor.NextFeature();
                    }

                    //�ͷ�cursor
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(FeatureCursor);
                }
                catch (Exception e)
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
                    return null;
                }
            }
            return null;
        }
        private IGeometry GetBufferGeometryByMapFrame(IGeometry pGeometry, double distance, int state)
        {
            IEnvelope pEnvelope = pGeometry.Envelope;
            IPolyline polyline = new PolylineClass();

            switch (state)
            {
                case 1:      //���
                    polyline.FromPoint = pEnvelope.UpperLeft;
                    polyline.ToPoint = pEnvelope.LowerLeft;
                    break;
                case 3:     //�ұ�
                    polyline.FromPoint = pEnvelope.UpperRight;
                    polyline.ToPoint = pEnvelope.LowerRight;
                    break;
                case 2:     //�ϱ�
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
        public List<IPolyline> Getborderline()
        {
            if (null == this._OriMapFrame)
                return null;
            List<IPolyline> ResList = new List<IPolyline>();
            IEnvelope pEnvelope = this._OriMapFrame.Shape.Envelope;
            IPolyline Lpolyline = new PolylineClass();
            IPolyline Dpolyline = new PolylineClass();
            ////���
            Lpolyline.FromPoint = pEnvelope.UpperLeft;
            Lpolyline.ToPoint = pEnvelope.LowerLeft;
            ResList.Add(Lpolyline);
            ////�±�
            Dpolyline.FromPoint = pEnvelope.LowerLeft;
            Dpolyline.ToPoint = pEnvelope.LowerRight;
            ResList.Add(Dpolyline);
            return ResList;
        }
    }
    /*
     * ClsTaskFrame(�Ǳ�׼ͼ��)��ʵ����IMapframe ���ӱ�ͼ�����ӿ�
     * ��ȡ��Դ�ӱ�Ҫ�ػ�������ͼ���ڣ���Ŀ��Ҫ�ػ�������ͼ���⣩��ͼ���߽�Ļ������ֱ���ͼ����Χ�󽻡����ķ�Χ
     * ���õ�ǰ�ӱ�ͼ����ʹ��GetOriFrame����ͨ��ͼ�����ֶ�����ֵ�������ã�Ҳ��ͨ��OriMapFrame����ֱ�Ӹ�ֵ
     * Getborderline����ȡ�ӱ߽߱磩�ķ���ʵ�ֵĻ�ȡͼ�������б߽磨ͬʱ�����ڹ���Χ�Ͳ�����Χ�ӱߣ�����ʹ�������Χ�ӱ�ʱЧ�ʽϵͣ�
     */
    class ClsTaskFrame : IMapframe
    {
        public ClsTaskFrame()////���캯��
        {
            this._mapframefea = null;
            this._OriMapFrame = null;
        }
        private IFeatureClass _mapframefea;
        public IFeatureClass MapFrameFea
        {
            get { return _mapframefea; }
            set { this._mapframefea = value; }
        }
        private IFeature _OriMapFrame;
        public IFeature OriMapFrame
        {
            get { return this._OriMapFrame; }
            set { this._OriMapFrame = value; }
        }
        public IFeature GetOriFrame(string OriFrameNoValue, string OriFrameNoField)
        {
            IFeature Fea = this.GetFrame(OriFrameNoValue, OriFrameNoField);
            _OriMapFrame = Fea;
            return Fea;
        }
        public void GetBufferArea(double dBufferRadius, out IGeometry OriBufferArea, out IGeometry DesBufferArea)
        {
            OriBufferArea = null;
            DesBufferArea = null;
            if (null == this._OriMapFrame)
                return;
            IGeometry BufferArea = this.GetBufferGeometryByMapFrame(dBufferRadius);
            if (null == BufferArea)
                return;
            ////��ȡԴͼ����Ŀ��ͼ���Ľӱ߻�����
            try
            {
                ITopologicalOperator topo = BufferArea as ITopologicalOperator;
                OriBufferArea = topo.Intersect(_OriMapFrame.Shape, esriGeometryDimension.esriGeometry2Dimension);
                DesBufferArea = topo.Difference(_OriMapFrame.Shape);
            }
            catch (Exception Error)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(Error, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(Error, null, DateTime.Now);
                }
                //********************************************************************
                throw Error;
            }
        }
        private IFeature GetFrame(string OriFrameNoValue, string OriFrameNoField)
        {
            if (null == this._mapframefea)
                return null;
            else
            {
                try
                {
                    IFeatureCursor FeatureCursor = _mapframefea.Search(null, false);
                    IFeature fea = FeatureCursor.NextFeature();
                    while (null != fea)
                    {
                        int index = -1;
                        index = fea.Fields.FindField(OriFrameNoField);
                        if (index > 0)
                        {
                            string MapFrameno = fea.get_Value(index).ToString();
                            if (OriFrameNoValue == MapFrameno)
                                return fea;
                        }
                        fea = FeatureCursor.NextFeature();
                    }

                    //�ͷ�cursor
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(FeatureCursor);
                }
                catch (Exception e)
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
                    return null;
                }
            }
            return null;
        }
        private IGeometry GetBufferGeometryByMapFrame(double distance)
        {
            IGeometry geo = null;
            List<IPolyline> BorList = this.Getborderline();
            if (null == BorList)
                return null;
            List<IGeometry> AreaList = new List<IGeometry>();
            for (int i = 0; i < BorList.Count; i++)
            {
                IPolyline line = new PolylineClass();
                line.FromPoint = BorList[i].FromPoint;
                line.ToPoint = BorList[i].ToPoint;
                line.SpatialReference = BorList[i].SpatialReference;
                ITopologicalOperator topo = line as ITopologicalOperator;
                IGeometry buffer = topo.Buffer(distance);
                AreaList.Add(buffer);
            }
            if (AreaList != null)
            {
                for (int i = 0; i < AreaList.Count; i++)
                {
                    IGeometry getGeo = AreaList[i];
                    if (geo == null)
                        geo = getGeo;
                    else
                    {
                        ITopologicalOperator UniTop = geo as ITopologicalOperator;
                        geo = UniTop.Union(getGeo);
                    }
                }
            }
            return geo;

        }
        public List<IPolyline> Getborderline()
        {
            if (null == this._OriMapFrame)
                return null;
            List<IPolyline> ResList = new List<IPolyline>();
            if (this._OriMapFrame.Shape.GeometryType != esriGeometryType.esriGeometryPolygon)
                return null;
            ////////��ȡ�����������б߽�
            IPolygon MapFramePolygon = this._OriMapFrame.ShapeCopy as IPolygon;
            IPointCollection PoCol = new Polyline();
            PoCol.AddPointCollection(MapFramePolygon as IPointCollection);
            ISegmentCollection newSeCol = PoCol as ISegmentCollection;
            int count = newSeCol.SegmentCount;
            ISegment Seg = null;
            ILine getLine = null;
            for (int j = 0; j < count; j++)
            {
                Seg = newSeCol.get_Segment(j);
                if (Seg.GeometryType == esriGeometryType.esriGeometryLine)
                {
                    getLine = Seg as ILine;
                    getLine.SpatialReference = _OriMapFrame.Shape.SpatialReference;
                    IPolyline GetPolyline = new PolylineClass();
                    GetPolyline.FromPoint = getLine.FromPoint;
                    GetPolyline.ToPoint = getLine.ToPoint;
                    GetPolyline.SpatialReference = getLine.SpatialReference;
                    ResList.Add(GetPolyline);
                }
            }
            return ResList;
        }
    }
    /*
     * ClsDestinatDataset��ʵ����IDestinatDataset���ӱ�Ŀ��㼯���ӿ�
     * GetFeaturesByGeometry����ȡ���ӱ�Ҫ����Ϣ������ʵ����ͨ����������ȡ���ӱ�Ҫ����Ϣ�ķ�����
     *   GetFeaturesByGeometry����ȡ���ӱ�Ҫ����Ϣ��������ά��һ����¼���������Ľӱ�Ҫ�ؼ�¼����
     *   Ϊ��ֹ�����ظ��ӱ��������ʹ��ѭ�����軺������ȡ�ӱ�Ҫ��ʱ������Ӧ����ѭ���ⲿ
     *   Dictionary��string key �ǽӱ�ͼ������long�ǽӱ�Ҫ��oid
     */

    class ClsDestinatDataset : IDestinatDataset
    {
        public ClsDestinatDataset()////���캯��
        {
            this._IsStandardMapFrame = false;
            this._JoinFeatureClass = null;
            this._IsGeometrySimplify = false;
            this._IsRemoveRedundantPnt = false;
            this._Angle_to = 1;
            this.m_lremve = new List<long>();
            this.m_lsimplify = new List<long>();
        }
        public ClsDestinatDataset(bool IsStandard)////���캯��
        {
            this._IsStandardMapFrame = IsStandard;
            this._JoinFeatureClass = null;
            this._IsGeometrySimplify = false;
            this._IsRemoveRedundantPnt = false;
            this._Angle_to = 1;
            this.m_lremve = new List<long>();
            this.m_lsimplify = new List<long>();
        }
        private bool _IsRemoveRedundantPnt;/////�Ƿ�ɾ��������϶����
        public bool IsRemoveRedundantPnt
        {
            get { return this._IsRemoveRedundantPnt; }
            set { this._IsRemoveRedundantPnt = value; }
        }
        private bool _IsGeometrySimplify;//////�Ƿ����Ҫ�صļ򵥻�����Ծ��ж��Geometry��Ҫ�أ�
        public bool IsGeometrySimplify
        {
            get { return this._IsGeometrySimplify; }
            set { this._IsGeometrySimplify = value; }
        }
        private double _Angle_to;/////�Ƕ��ݲ�
        public double Angle_to
        {
            get { return this._Angle_to; }
            set { this._Angle_to = value; }
        }

        private bool _IsStandardMapFrame;
        public bool IsStandardMapFrame
        {
            get { return this._IsStandardMapFrame; }
            set { this._IsStandardMapFrame = value; }
        }////�Ƿ�Ϊ��׼ͼ����

        private List<long> m_lremve;
        private List<long> m_lsimplify;///////ά��һ���б��¼ɾ������㡢�򵥻�Ҫ�ص�oid�����ظ�����

        private Dictionary<string, List<long>> JoinedFeaOIDDic = new Dictionary<string, List<long>>();////ά��һ���ѽӱߵ�Ҫ�ؼ�¼��ϣ���ֹ�ظ��ӱ�
        private List<IFeatureClass> _JoinFeatureClass;
        public List<IFeatureClass> JoinFeatureClass
        {
            get { return this._JoinFeatureClass; }
            set { this._JoinFeatureClass = value; }
        }
        public IFeatureClass TargetFeatureClass(string FeatureClassName)////������ȡͼ��
        {
            if (null == this._JoinFeatureClass)
                return null;
            foreach (IFeatureClass Feas in this._JoinFeatureClass)
            {
                if ((Feas as IDataset).Name == FeatureClassName)
                    return Feas;
            }
            return null;
        }
        public Dictionary<string, List<long>> GetFeaturesByGeometry(IGeometry BufferArea, bool isOriArea)
        {
            Dictionary<string, List<long>> ResDic = new Dictionary<string, List<long>>();
            try
            {
                if (null != this._JoinFeatureClass)
                {
                    //////���ݲ�����
                    ClsDataOperationer DataOper = new ClsDataOperationer();
                    DataOper.Angle_to = this._Angle_to;
                    for (int i = 0; i < this._JoinFeatureClass.Count; i++)
                    {
                        IFeatureClass ipFtClass = _JoinFeatureClass[i];
                        DataOper.OpeFeaClss = ipFtClass;
                        ISpatialFilter SpFilter = new SpatialFilterClass();
                        SpFilter.Geometry = BufferArea;
                        IFeatureClass FromFeaCls = ipFtClass;
                        string FeaName = (FromFeaCls as IDataset).Name;
                        List<IFeature> GetFea = new List<IFeature>();

                        if (ipFtClass.ShapeType == esriGeometryType.esriGeometryPolyline)//////����Ҫ�ػ�ȡ
                        {
                            #region ��Ҫ�ش�Խ��ȡ
                            SpFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                            IFeatureCursor GetFeaCur = FromFeaCls.Search(SpFilter, false);
                            if (null != GetFeaCur)
                            {
                                IFeature Fea = null;
                                if (this._IsGeometrySimplify)//////Ҫ�ؼ򵥻�
                                {
                                    Fea = GetFeaCur.NextFeature();
                                    while (null != Fea)
                                    {
                                        if (!this.m_lsimplify.Contains((long)Fea.OID))
                                        {
                                            this.m_lsimplify.Add((long)Fea.OID);
                                            DataOper.GeometrySimplify((long)Fea.OID);
                                            Fea = GetFeaCur.NextFeature();
                                        }
                                        Fea = GetFeaCur.NextFeature();
                                    }
                                    System.Runtime.InteropServices.Marshal.ReleaseComObject(GetFeaCur);
                                    GetFeaCur = FromFeaCls.Search(SpFilter, false);
                                }

                                Fea = GetFeaCur.NextFeature();
                                while (null != Fea)
                                {
                                    GetFea.Add(Fea);
                                    Fea = GetFeaCur.NextFeature();
                                }

                            }
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(GetFeaCur);/////�ͷ��α������ѭ���ᱨ��
                            #endregion

                        }
                        else if (ipFtClass.ShapeType == esriGeometryType.esriGeometryPolygon)//////�����Ҫ�ػ�ȡ
                        {
                            #region ����ν�����ȡ
                            SpFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelOverlaps;
                            IFeatureCursor GetFeaCur = FromFeaCls.Search(SpFilter, false);
                            if (null != GetFeaCur)
                            {
                                IFeature Fea = null;
                                if (this._IsGeometrySimplify)//////Ҫ�ؼ򵥻�
                                {
                                    Fea = GetFeaCur.NextFeature();
                                    while (null != Fea)
                                    {
                                        if (!this.m_lsimplify.Contains((long)Fea.OID))
                                        {
                                            this.m_lsimplify.Add((long)Fea.OID);
                                            DataOper.GeometrySimplify((long)Fea.OID);
                                            Fea = GetFeaCur.NextFeature();
                                        }
                                        Fea = GetFeaCur.NextFeature();
                                    }
                                    System.Runtime.InteropServices.Marshal.ReleaseComObject(GetFeaCur);
                                    GetFeaCur = FromFeaCls.Search(SpFilter, false);
                                }

                                Fea = GetFeaCur.NextFeature();
                                while (null != Fea)
                                {

                                    if (this._IsRemoveRedundantPnt)//////ȥ��������϶����
                                    {
                                        if (!this.m_lremve.Contains((long)Fea.OID))
                                        {
                                            DataOper.RemoveRedundantPntFromPolygon((long)Fea.OID);
                                            this.m_lremve.Add((long)Fea.OID);
                                        }
                                    }

                                    GetFea.Add(Fea);
                                    Fea = GetFeaCur.NextFeature();
                                }
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(GetFeaCur);
                            }
                            #endregion
                        }
                        SpFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;//////����Ҫ��������Ҫ��(����)��ȡ
                        IFeatureCursor GetFeaCur2 = FromFeaCls.Search(SpFilter, false);
                        if (null != GetFeaCur2)
                        {
                            #region ��Ҫ�ء���Ҫ�ذ�����ȡ
                            IFeature Fea = null;
                            if (this._IsGeometrySimplify)//////Ҫ�ؼ򵥻�
                            {
                                Fea = GetFeaCur2.NextFeature();
                                while (null != Fea)
                                {
                                    if (!this.m_lsimplify.Contains((long)Fea.OID))
                                    {
                                        this.m_lsimplify.Add((long)Fea.OID);
                                        DataOper.GeometrySimplify((long)Fea.OID);
                                        Fea = GetFeaCur2.NextFeature();

                                    }
                                    Fea = GetFeaCur2.NextFeature();
                                }
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(GetFeaCur2);
                                GetFeaCur2 = FromFeaCls.Search(SpFilter, false);
                            }

                            Fea = GetFeaCur2.NextFeature();
                            while (null != Fea)
                            {
                                if (Fea.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                                {
                                    if (this._IsRemoveRedundantPnt)//////ȥ��������϶����
                                    {
                                        if (!this.m_lremve.Contains((long)Fea.OID))
                                        {
                                            DataOper.RemoveRedundantPntFromPolygon((long)Fea.OID);
                                            this.m_lremve.Add((long)Fea.OID);
                                        }
                                    }
                                }
                                GetFea.Add(Fea);
                                Fea = GetFeaCur2.NextFeature();
                            }
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(GetFeaCur2);/////�ͷ��α������ѭ���ᱨ��
                            #endregion
                        }

                        if (null != GetFea)
                        {

                            for (int n = 0; n < GetFea.Count; n++)
                            {

                                IFeature Fea = GetFea[n];
                                long OID = (long)Fea.OID;
                                /////////////////////�жϴ�Ҫ���Ƿ��Ѿ���������¼�����(ֻ�ڷǱ�׼ͼ���ӱ�ʱ�ж�)
                                if (this._IsStandardMapFrame == false)
                                {
                                    if (isOriArea == false)
                                    {
                                        if (JoinedFeaOIDDic.ContainsKey(FeaName))
                                        {
                                            if (JoinedFeaOIDDic[FeaName].Contains(OID))//////��Ҫ���Ѿ����ڼ�¼
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                }
                                /////////////////////
                                if (ResDic.ContainsKey(FeaName))
                                {
                                    ResDic[FeaName].Add(OID);
                                }
                                else
                                {
                                    List<long> OidList = new List<long>();
                                    OidList.Add(OID);
                                    ResDic.Add(FeaName, OidList);
                                }
                                //////////////////��¼����������Ҫ�ؼ�¼
                                if (this._IsStandardMapFrame == false)
                                {
                                    if (isOriArea == true)
                                    {
                                        if (JoinedFeaOIDDic.ContainsKey(FeaName))
                                        {
                                            JoinedFeaOIDDic[FeaName].Add(OID);
                                        }
                                        else
                                        {
                                            List<long> OidList = new List<long>();
                                            OidList.Add(OID);
                                            JoinedFeaOIDDic.Add(FeaName, OidList);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return ResDic;
            }
            catch (Exception eError)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog == null)
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                ModData.SysLog.Write(eError, null, DateTime.Now);
                //********************************************************************
                return null;
            }
        }
    }
    /*
     * ClsCheckOperationer��ʵ����ICheckOperation���ӱ߼��ӿڣ�
     * 
     */
    class ClsCheckOperationer : ICheckOperation
    {
        private bool _CreatLog;
        public bool CreatLog
        {
            get { return this._CreatLog; }
            set { this._CreatLog = value; }
        }
        public ClsCheckOperationer()////���캯��
        {
            this._Angel_Tolerrance = 1;
            this._borderline = null;
            this._DesBufferArea = null;
            this._DestinatFeaCls = null;
            this._Dis_Tolerance = 3;
            this._Length_Tolerrance = 1;
            this._OriBufferArea = null;
            this._OriFeaturesOID = null;
            this._Search_Tolerrance = 3;
            this._DesFeaturesOID = null;
            this._CreatLog = false;
            this.FieldsControlList = null;
        }
        private IFeatureClass _DestinatFeaCls;////���ӱ�ͼ��
        public IFeatureClass DestinatFeaCls
        {
            get { return this._DestinatFeaCls; }
            set { this._DestinatFeaCls = value; }
        }

        private List<IPolyline> _borderline;/////�ӱ߽߱�
        public List<IPolyline> borderline
        {
            get { return this._borderline; }
            set { this._borderline = value; }
        }

        private List<long> _OriFeaturesOID;/////���ӱ�ԴҪ��OID
        public List<long> OriFeaturesOID
        {
            get { return this._OriFeaturesOID; }
            set { this._OriFeaturesOID = value; }
        }

        private List<long> _DesFeaturesOID;/////���ӱ�Ŀ��Ҫ��OID
        public List<long> DesFeaturesOID
        {
            get { return this._DesFeaturesOID; }
            set { this._DesFeaturesOID = value; }
        }

        private List<string> _FieldsControlList;///////���������ֶ��б�
        public List<string> FieldsControlList
        {
            get { return this._FieldsControlList; }
            set { this._FieldsControlList = value; }
        }

        private double _Dis_Tolerance;/////�����ݲ�
        public double Dis_Tolerance
        {
            get { return this._Dis_Tolerance; }
            set { this._Dis_Tolerance = value; }
        }
        private double _Search_Tolerrance;/////�����ݲ�
        public double Search_Tolerrance
        {
            get { return this._Search_Tolerrance; }
            set { this._Search_Tolerrance = value; }
        }
        private double _Angel_Tolerrance;/////�Ƕ��ݲ�
        public double Angel_Tolerrance
        {
            get { return this._Angel_Tolerrance; }
            set { this._Angel_Tolerrance = value; }
        }
        private double _Length_Tolerrance;
        public double Length_Tolerrance ////�����ݲ�
        {
            get { return this._Length_Tolerrance; }
            set { this._Length_Tolerrance = value; }
        }
        private IGeometry _OriBufferArea;////Դͼ���ӱ߻�������
        public IGeometry OriBufferArea
        {
            get { return this._OriBufferArea; }
            set { this._OriBufferArea = value; }
        }
        private IGeometry _DesBufferArea;/////Ŀ��ͼ���ӱ߻�������
        public IGeometry DesBufferArea
        {
            get { return this._DesBufferArea; }
            set { this._DesBufferArea = value; }
        }
        public esriGeometryType GetDatasetGeometryType()
        {
            if (null == this._DestinatFeaCls)
                return esriGeometryType.esriGeometryAny;
            return this._DestinatFeaCls.ShapeType;
        }
        public DataTable GetPolylineDesFeatureOIDByOriFeature()////��ȡ���ʹ��ӱ�Ŀ��Ҫ�ؼ���Ҫ��ԴҪ�ؽӱߵ�Ҫ��
        {
                Exception ex = null;
                if (this._OriFeaturesOID == null)
                    return null;
                IJoinLOG JoinLog = new ClsJoinLog();

                DataTable ResTable = new DataTable();
                DataColumn dc1 = new DataColumn("���ݼ�", Type.GetType("System.String"));
                DataColumn dc2 = new DataColumn("Ҫ������", Type.GetType("System.String"));
                DataColumn dc3 = new DataColumn("ԴҪ��ID", Type.GetType("System.Int64"));
                dc3.DefaultValue = -1;
                DataColumn dc4 = new DataColumn("OriPtn", Type.GetType("System.String"));
                DataColumn dc5 = new DataColumn("Ŀ��Ҫ��ID", Type.GetType("System.Int64"));
                dc5.DefaultValue = -1;
                DataColumn dc6 = new DataColumn("DesPtn", Type.GetType("System.String"));
                DataColumn dc7 = new DataColumn("�ӱ�״̬", Type.GetType("System.String"));
                ResTable.Columns.Add(dc1);
                ResTable.Columns.Add(dc2);
                ResTable.Columns.Add(dc3);
                ResTable.Columns.Add(dc4);
                ResTable.Columns.Add(dc5);
                ResTable.Columns.Add(dc6);
                ResTable.Columns.Add(dc7);
                FrmProcessBar ProcBar = new FrmProcessBar((long)this._OriFeaturesOID.Count);
                ProcBar.SetChild();
                ProcBar.Show();
                for (int i = 0; i < this._OriFeaturesOID.Count; i++)
                {
                    long OriOID = this._OriFeaturesOID[i];
                    ////������
                    ProcBar.SetFrmProcessBarValue(i);
                    ProcBar.SetFrmProcessBarText("���ڴ�����Ҫ�أ�" + OriOID);
                    System.Windows.Forms.Application.DoEvents();
                    IFeatureClass Feacls = this._DestinatFeaCls;
                    IFeature Fea = Feacls.GetFeature((int)OriOID);
                    IPolyline OriPolyline = Fea.ShapeCopy as IPolyline;

                    IPoint FrPoint = OriPolyline.FromPoint;
                    IPoint ToPOint = OriPolyline.ToPoint;
                    DataRow newRow = ResTable.NewRow();
                    newRow["���ݼ�"] = Feacls.AliasName;
                    newRow["Ҫ������"] = "Polyline";
                    newRow["ԴҪ��ID"] = OriOID;
                    newRow["OriPtn"] = "ToPoint";


                    DataRow newRow2 = ResTable.NewRow();
                    newRow2["���ݼ�"] = Feacls.AliasName; ;
                    newRow2["Ҫ������"] = "Polyline";
                    newRow2["ԴҪ��ID"] = OriOID;
                    newRow2["OriPtn"] = "FromPoint";
                    bool rec = false;
                    bool judge = false;
                    DataRow AddRowT = null;
                    DataRow AddRowF = null;
                    ProcBar.SetFrmProcessBarText("���ڴ�����Ҫ�أ�" + OriOID + "���ӱ�Ŀ��Ҫ������");
                    System.Windows.Forms.Application.DoEvents();
                    judge = JudgePolygonIsContainPT(ToPOint, this._OriBufferArea);

                    if (judge)
                    {
                        AddRowT = GetPointJionDes(ToPOint, newRow, out rec);//////�յ�����
                        if (rec)
                        {
                            ResTable.Rows.Add(AddRowT);
                            if (this._CreatLog)
                            {
                                JoinLog.OnDataJoin_OnCheck(AddRowT, ToPOint.X, ToPOint.Y, out ex);
                            }
                        }
                    }
                    else if (JudgePolygonIsContainPT(ToPOint, this._DesBufferArea))
                    {
                        AddRowT = GetPointJionDes(ToPOint, newRow, out rec);//////�յ�����
                        if (rec)
                        {
                            ResTable.Rows.Add(AddRowT);
                            if (this._CreatLog)
                            {
                                JoinLog.OnDataJoin_OnCheck(AddRowT, ToPOint.X, ToPOint.Y, out ex);
                            }
                        }
                    }
                    judge = JudgePolygonIsContainPT(FrPoint, this._OriBufferArea);
                    if (judge)
                    {
                        AddRowF = GetPointJionDes(FrPoint, newRow2, out rec);//////�������
                        if (rec)
                        {
                            ResTable.Rows.Add(AddRowF);
                            if (this._CreatLog)
                            {
                                JoinLog.OnDataJoin_OnCheck(AddRowF, FrPoint.X, FrPoint.Y, out ex);
                            }
                        }
                    }
                    else if (JudgePolygonIsContainPT(FrPoint, this._DesBufferArea))
                    {
                        AddRowF = GetPointJionDes(FrPoint, newRow2, out rec);//////�������
                        if (rec)
                        {
                            ResTable.Rows.Add(AddRowF);
                            if (this._CreatLog)
                            {
                                JoinLog.OnDataJoin_OnCheck(AddRowF, FrPoint.X, FrPoint.Y, out ex);
                            }
                        }
                    }

                }
                ProcBar.Close();
                JoinLog.onDataJoin_Terminate(0, out ex);
                return ResTable;
            

 
        }
       
        public DataTable GetPolygonDesFeatureOIDByOriFeature()////��ȡ����δ��ӱ�Ŀ��Ҫ�ؼ���Ҫ��ԴҪ�ؽӱߵ�Ҫ��
        {
            DataTable ResTable = new DataTable();
            IJoinLOG JoinLog = new ClsJoinLog();
            DataColumn dc1 = new DataColumn("���ݼ�", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("Ҫ������", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("ԴҪ��ID", Type.GetType("System.Int64"));
            dc3.DefaultValue = -1;
            DataColumn dc4 = new DataColumn("OriLineIndex", Type.GetType("System.Int64"));
            dc4.DefaultValue = -1;
            DataColumn dc5 = new DataColumn("Ŀ��Ҫ��ID", Type.GetType("System.Int64"));
            dc5.DefaultValue = -1;
            DataColumn dc6 = new DataColumn("DesLineIndex", Type.GetType("System.Int64"));
            dc6.DefaultValue = -1;
            DataColumn dc7 = new DataColumn("�ӱ�״̬", Type.GetType("System.String"));
            ResTable.Columns.Add(dc1);
            ResTable.Columns.Add(dc2);
            ResTable.Columns.Add(dc3);
            ResTable.Columns.Add(dc4);
            ResTable.Columns.Add(dc5);
            ResTable.Columns.Add(dc6);
            ResTable.Columns.Add(dc7);
            //////////////////////////////////////////////////////////
            if (this._OriFeaturesOID == null)
                return null;
            FrmProcessBar ProcBar = new FrmProcessBar((long)this._OriFeaturesOID.Count);
            ProcBar.SetChild();
            ProcBar.Show();
            for (int i = 0; i < this._OriFeaturesOID.Count; i++)
            {
                /////////////////////////��ȡ���ӱ�ԴҪ�ؼ��ӱߵıߵ�Index
                /////������
                long OriOID = this._OriFeaturesOID[i];
                ProcBar.SetFrmProcessBarValue(i);
                ProcBar.SetFrmProcessBarText("���ڴ�������Ҫ�أ�" + OriOID);
                System.Windows.Forms.Application.DoEvents();

                IFeatureClass Feacls = this._DestinatFeaCls;
                IFeature Fea = Feacls.GetFeature((int)OriOID);
                IPolygon OriPolygon = Fea.Shape as IPolygon;
                if (null == OriPolygon)
                    continue;
                List<long> Index = new List<long>();
                List<ILine> lineList = GetPolygonJoinLine(Fea, this._OriBufferArea, out Index);
                ///////////////////////��ȡ���ӱ�Ŀ��Ҫ�ؼ��ӱߵıߵ�Index
                List<long> oid = new List<long>();
                List<long> oidindex = new List<long>();
                if (null != lineList)
                {
                    for (int j = 0; j < lineList.Count; j++)
                    {
                        ILine getline = lineList[j];
                        long OIDIndex = -1;
                        long lOID = GetDesOIDByLine(getline, out OIDIndex);
                        oid.Add(lOID);
                        oidindex.Add(OIDIndex);
                    }
                }
                if (null != oid && null != lineList)
                {
                    if (lineList.Count == oid.Count)
                    {
                        for (int index = 0; index < oid.Count; index++)
                        {
                            if (OriOID == oid[index])
                                continue;
                            if (oid[index] == -1)
                                continue;
                            /////������
                            ProcBar.SetFrmProcessBarText("���ڴ�������Ҫ�أ�" + OriOID + ",�ӱ�Ŀ��Ҫ�أ�" + oid[index]);
                            System.Windows.Forms.Application.DoEvents();
                            DataRow newRow = ResTable.NewRow();
                            newRow["���ݼ�"] = this._DestinatFeaCls.AliasName;
                            newRow["Ҫ������"] = "Polygon";
                            newRow["ԴҪ��ID"] = OriOID;
                            newRow["OriLineIndex"] = Index[index];
                            newRow["Ŀ��Ҫ��ID"] = oid[index];
                            newRow["DesLineIndex"] = oidindex[index];
                            ////////////////�����ж�/////////////////////
                            string sFieldState = string.Empty;
                            if (this.FieldsControlList != null)
                            {
                                if (judgeTwoFeatureField(OriOID, oid[index]))
                                    sFieldState = "������һ��";
                                else
                                    sFieldState = "�����Բ�һ��";
                            }
                            ///////////////////////////////////////////////
                            IFeature DesFeature = Feacls.GetFeature((int)oid[index]);
                            IRelationalOperator RelOper = Fea.ShapeCopy as IRelationalOperator;
                            if (RelOper.Touches(DesFeature.ShapeCopy))
                            {
                                newRow["�ӱ�״̬"] = "�ѽӱ�" + sFieldState;
                            }
                            else
                            {
                                newRow["�ӱ�״̬"] = "δ�ӱ�" + sFieldState;
                            }

                            ResTable.Rows.Add(newRow);
                            if (this._CreatLog)
                            {
                                Exception ex = null;
                                JoinLog.OnDataJoin_OnJoin(newRow, OriPolygon.ToPoint.X, OriPolygon.ToPoint.Y, out ex);
                            }
                        }
                    }
                }
            }
            ProcBar.Close();
            return ResTable;
        }
        public static bool JudgePolygonIsContainPT(IPoint po, IGeometry Area)////�жϵ��Ƿ��ڶ������
        {
            IRelationalOperator RelOp = Area as IRelationalOperator;
            bool Sate = RelOp.Contains(po);
            if (Sate == false)
            {
                RelOp = Area as IRelationalOperator;
                Sate = RelOp.Touches(po);
            }
            return Sate;

        }
        public static bool JudgePolygonIsContainLine(ILine line, IGeometry Area)////�ж����Ƿ��ڶ������
        {
            IRelationalOperator RelOp = Area as IRelationalOperator;
            IPointCollection pPntColLine = null;
            IGeometry pGeometry = null;
            pPntColLine = new Polyline();
            object missing = Type.Missing;
            pPntColLine.AddPoint(line.FromPoint, ref missing, ref missing);
            pPntColLine.AddPoint(line.ToPoint, ref missing, ref missing);
            pGeometry = pPntColLine as IGeometry;
            bool Sate = RelOp.Contains(pGeometry);
            bool touch = false;
            touch = RelOp.Touches(pGeometry);
            if (Sate || touch)
                return true;
            else
                return false;
        }
        private IFeatureCursor GetFeaturesByGeometry(IGeometry Area, esriSpatialRelEnum Rel)/////ͨ��Geometry���ռ��ϵ��ȡ��Ҫ�ؼ�
        {
            if (this._DestinatFeaCls == null)
                return null;
            ISpatialFilter SpFilter = new SpatialFilterClass();
            SpFilter.Geometry = Area;
            SpFilter.SpatialRel = Rel;
            //IFeatureClass FromFeaCls = this._DestinatFeaCls;
            IFeatureCursor GetFeaCur = this._DestinatFeaCls.Search(SpFilter, false);
            return GetFeaCur;
        }
        private DataRow GetPointJionDes(IPoint point, DataRow in_Row, out bool rec)////��ȡ���ӱߵ��Ŀ��OID
        {
            rec = false;
            if (null == in_Row)
                return null;
            long OriOid = -1;
            try
            {
                OriOid = Convert.ToInt64(in_Row["ԴҪ��ID"].ToString());
            }
            catch
            {
                rec = false;
                return null;
            }
            ITopologicalOperator Topoper = point as ITopologicalOperator;
            IGeometry Pointbuffer = Topoper.Buffer(this._Search_Tolerrance);
            List<IFeature> FeaList = new List<IFeature>();
            IFeatureCursor DesFeaCursor = this.GetFeaturesByGeometry(Pointbuffer, esriSpatialRelEnum.esriSpatialRelIntersects);
            if (null != DesFeaCursor)
            {
                IFeature Fea = DesFeaCursor.NextFeature();
                while (null != Fea)
                {
                    FeaList.Add(Fea);
                    Fea = DesFeaCursor.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(DesFeaCursor);/////�ͷ��α������ѭ�������
            }
            if (FeaList.Count == 0)
                return null;
            string TopointDesPt = string.Empty;
            string FrmpointDesPt = string.Empty;
            double MinDis = this._Dis_Tolerance;
            string DesPt = string.Empty;
            string joinpt = string.Empty;
            IPolyline DesPolyline = null;
            IPolyline ResPolyline = null;
            long ResFeatureOID = -1;
            #region ����FeaList��ȡ����ӱ�����Ҫ�ص�OID

            for (int i = 0; i < FeaList.Count; i++)
            {
                IFeature DesFea = FeaList[i];
                if (OriOid == (long)DesFea.OID)
                    continue;

                if (this._DesFeaturesOID.Contains((long)DesFea.OID) && DesFea.Shape.GeometryType == esriGeometryType.esriGeometryPolyline)
                {
                    DesPolyline = DesFea.Shape as IPolyline;
                    IPoint DesFrpoint = DesPolyline.FromPoint;
                    IPoint DesTopoint = DesPolyline.ToPoint;
                    ////����������feature��������㡢�յ�����ӱ�Դpolyline�˵�ľ��룬ȡ������С�ģ�
                    // IRelationalOperator RelOper = point as IRelationalOperator;
                    if (JudgePolygonIsContainPT(DesFrpoint, Pointbuffer))
                    {
                        #region �ж�DesFromPoint
                        double distance = ClsCheckOperationer.CalculateDistance(point, DesFrpoint);
                        if (distance < MinDis)
                        {
                            MinDis = distance;
                            joinpt = "FromPoint";
                            //ResPoint = DesFrpoint;
                            ResFeatureOID = (long)DesFea.OID;
                            ResPolyline = DesPolyline;
                            //in_Row["Ŀ��Ҫ��ID"] = DesFea.OID; ;
                            //in_Row["DesPtn"] = "FromPoint";
                            //if (RelOper.Equals(DesFrpoint))
                            //{
                            //    in_Row["�ӱ�״̬"] = "�ѽӱ�";
                            //}
                            //else
                            //{
                            //    in_Row["�ӱ�״̬"] = "δ�ӱ�";
                            //}
                            rec = true;
                        }
                        #endregion
                    }

                    if (JudgePolygonIsContainPT(DesTopoint, Pointbuffer))
                    {
                        #region �ж�DesToPoint
                        double distance = ClsCheckOperationer.CalculateDistance(point, DesTopoint);
                        if (distance < MinDis)
                        {
                            MinDis = distance;
                            joinpt = "ToPoint";
                            //ResPoint = DesTopoint;
                            ResFeatureOID = (long)DesFea.OID;
                            ResPolyline = DesPolyline;
                            //in_Row["Ŀ��Ҫ��ID"] = DesFea.OID;
                            //in_Row["DesPtn"] = "ToPoint";
                            //if (RelOper.Equals(DesTopoint))
                            //{
                            //    in_Row["�ӱ�״̬"] = "�ѽӱ�";
                            //}
                            //else
                            //{
                            //    in_Row["�ӱ�״̬"] = "δ�ӱ�";
                            //}
                            rec = true;
                        }

                        #endregion
                    }
                }
                else
                {
                    continue;
                }
            }

            #endregion
            /////////////////////�ж��������///////////////////////////
            string sFieldState = string.Empty;
            if (this.FieldsControlList != null)
            {

                if (judgeTwoFeatureField(OriOid, ResFeatureOID))
                    sFieldState = "������һ��";
                else
                    sFieldState = "�����Բ�һ��";
            }
            ///////////////////////////////////////////////////////////

            if (string.IsNullOrEmpty(joinpt))
            {
                rec = false;
                return null;
            }
            IRelationalOperator RelOper = point as IRelationalOperator;
            if (joinpt == "FromPoint")
            {
                in_Row["Ŀ��Ҫ��ID"] = ResFeatureOID;
                in_Row["DesPtn"] = "FromPoint";
                if (RelOper.Equals(ResPolyline.FromPoint))
                {
                    in_Row["�ӱ�״̬"] = "�ѽӱ�" + sFieldState;
                }
                else
                {
                    in_Row["�ӱ�״̬"] = "δ�ӱ�" + sFieldState;
                }
                rec = true;
            }
            else if (joinpt == "ToPoint")
            {
                in_Row["Ŀ��Ҫ��ID"] = ResFeatureOID;
                in_Row["DesPtn"] = "ToPoint";
                if (RelOper.Equals(ResPolyline.ToPoint))
                {
                    in_Row["�ӱ�״̬"] = "�ѽӱ�" + sFieldState;
                }
                else
                {
                    in_Row["�ӱ�״̬"] = "δ�ӱ�" + sFieldState;
                }
                rec = true;
            }


            DesFeaCursor = null;
            return in_Row;

        }
        private List<ILine> GetPolygonJoinLine(IFeature Pol, IGeometry JoinArea, out List<long> Index)//// ��ȡ���ӱ߶�������Ҫ����ӱߵı�
        {
            Index = new List<long>();
            IGeometryCollection GeoCol = Pol.ShapeCopy as IGeometryCollection;
            if (GeoCol.GeometryCount != 1)
                return null;
            List<ILine> linelist = new List<ILine>();
            IPointCollection PoCol = new Polyline();
            IPolygon newpolygon = Pol.ShapeCopy as IPolygon;
            PoCol.AddPointCollection(newpolygon as IPointCollection);
            ISegmentCollection newSeCol = PoCol as ISegmentCollection;
            int count = newSeCol.SegmentCount;

            ISegment Seg = null;
            ILine getLine = null;
            for (int j = 0; j < count; j++)
            {
                Seg = newSeCol.get_Segment(j);
                if (Seg.GeometryType == esriGeometryType.esriGeometryLine)
                {
                    getLine = Seg as ILine;
                    getLine.SpatialReference = Pol.Shape.SpatialReference;
                    if (JudgePolygonIsContainLine(getLine, JoinArea))
                    {
                        bool ISParallel = false;
                        if (this._borderline != null)//////�������˽ӱ߽߱���ʹ�ýӱ߽߱�ƽ��������ɸѡ��
                        {
                            double minDistanxe = this._Dis_Tolerance;
                            ILine newline = null;
                            for (int i = 0; i < this._borderline.Count; i++)
                            {
                                IPolyline BorPolLine = this._borderline[i];

                                double distance = -1;
                                double curve = -1;
                                bool Isright = true;
                                BorPolLine.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, getLine.ToPoint, false, null, ref curve, ref distance, ref Isright);
                                if (distance < minDistanxe)
                                {
                                    BorPolLine.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, getLine.FromPoint, false, null, ref curve, ref distance, ref Isright);
                                    if (distance < minDistanxe)
                                    {
                                        minDistanxe = distance;
                                        newline = new LineClass();
                                        newline.ToPoint = BorPolLine.ToPoint;
                                        newline.FromPoint = BorPolLine.FromPoint;
                                        newline.SpatialReference = BorPolLine.SpatialReference;

                                        ISParallel = JudgeParallel(newline, getLine, this.Angel_Tolerrance);
                                        if (ISParallel)
                                        {

                                            linelist.Add(getLine);
                                            Index.Add(j);
                                        }
                                    }
                                }

                            }
                            //ISParallel = JudgeParallel(newline, getLine, this.Angel_Tolerrance);
                            //if (ISParallel)
                            //{

                            //    linelist.Add(getLine);
                            //    Index.Add(j);
                            //}
                        }
                        else
                        {
                            linelist.Add(getLine);
                            Index.Add(j);
                        }
                    }
                }

            }
            return linelist;
        }
        public static bool JudgeParallel(ILine Oline, ILine Dline, double AngeleTo)////���ݽǶ��ݲ��ж��������Ƿ�ƽ��
        {
            double dOriAngle;
            double dDesAngle;
            double PI = 3.1415927;
            if (null == Oline || null == Dline)
                return false;
            dOriAngle = Oline.Angle * 180 / PI;
            dDesAngle = Dline.Angle * 180 / PI;

            ///���ڲ������ߵķ���,����ֻ��������x��ļнǵľ���ֵ
            if (dOriAngle < 0)
                dOriAngle = 180 + dOriAngle;
            if (dDesAngle < 0)
                dDesAngle = 180 + dDesAngle;
            if (Math.Abs(dOriAngle - dDesAngle) <= AngeleTo || Math.Abs(dOriAngle - dDesAngle) >= (180 - AngeleTo))
                return true;
            else
                return false;
        }
        private long GetDesOIDByLine(ILine joinline, out long jionIndex)////ͨ������ε�һ���߻�ȡ���ӱ�Ŀ������OID
        {
            jionIndex = -1;
            if (this._DesFeaturesOID == null)
                return -1;
            if (joinline == null)
                return -1;

            IPolyline ppolyline = new PolylineClass();
            ppolyline.ToPoint = joinline.ToPoint;
            ppolyline.FromPoint = joinline.FromPoint;
            ppolyline.SpatialReference = joinline.SpatialReference;
            ITopologicalOperator Topoper = ppolyline as ITopologicalOperator;
            IGeometry Linebuffer = Topoper.Buffer(this._Search_Tolerrance);
            List<IFeature> Fealist = new List<IFeature>();
            IFeatureCursor DesFeaCursor = this.GetFeaturesByGeometry(Linebuffer, esriSpatialRelEnum.esriSpatialRelIntersects);
            if (DesFeaCursor != null)
            {
                IFeature getfea = DesFeaCursor.NextFeature();
                while (null != getfea)
                {
                    Fealist.Add(getfea);
                    getfea = DesFeaCursor.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(DesFeaCursor);
            }
            DesFeaCursor = null;
            if (Fealist.Count == 0)
                return -1;
            long OID = -1;
            double distance = this._Dis_Tolerance;
            double pdistance = this._Dis_Tolerance;
            for (int j = 0; j < Fealist.Count; j++)
            {
                IFeature DesFea = Fealist[j];
                if (!this._DesFeaturesOID.Contains((long)DesFea.OID))
                    continue;

                List<long> Index = new List<long>();
                List<ILine> DesLine = this.GetPolygonJoinLine(DesFea, this._DesBufferArea, out Index);
                if (null != DesLine)
                {

                    for (int i = 0; i < DesLine.Count; i++)
                    {
                        ILine line = DesLine[i];
                        ///////////��Ҫ���жϣ������������δ��ӱߵ������������˵㹲�㣬�������˵㲻��������Ϊ�ӱ�
                        if (!judgeTwoLineIsVerOrOnline(joinline, line))
                            continue;
                        double curve = -1;
                        bool Isright = true;
                        IPolyline newpolyline = new PolylineClass();
                        newpolyline.ToPoint = line.ToPoint;
                        newpolyline.FromPoint = line.FromPoint;
                        newpolyline.SpatialReference = line.SpatialReference;
                        newpolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, joinline.ToPoint, false, null, ref curve, ref distance, ref Isright);
                        double tem = -1;
                        newpolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, joinline.FromPoint, false, null, ref curve, ref tem, ref Isright);
                        if (tem < distance)
                            distance = tem;
                        if (distance < this._Dis_Tolerance)
                        {
                            if (!this.JudgeTwoLineIsinLength_to(joinline, line)) //////��������߲��ڳ����ݲΧ�ڼ���������һ����
                                continue;


                            tem = CalculateDistance(joinline.ToPoint, line.ToPoint);//////�ҳ������������
                            if (tem < pdistance)
                            {
                                pdistance = tem;
                                OID = (long)DesFea.OID;
                                jionIndex = Index[i];
                            }
                            tem = CalculateDistance(joinline.ToPoint, line.FromPoint);
                            if (tem < pdistance)
                            {
                                pdistance = tem;
                                OID = (long)DesFea.OID;
                                jionIndex = Index[i];
                            }
                            tem = CalculateDistance(joinline.FromPoint, line.ToPoint);
                            if (tem < pdistance)
                            {
                                pdistance = tem;
                                OID = (long)DesFea.OID;
                                jionIndex = Index[i];
                            }
                            tem = CalculateDistance(joinline.FromPoint, line.FromPoint);
                            if (tem < pdistance)
                            {
                                pdistance = tem;
                                OID = (long)DesFea.OID;
                                jionIndex = Index[i];
                            }
                        }
                    }
                }

            }
            return OID;
        }
        public static double CalculateDistance(IPoint FromPoint, IPoint ToPoint)//////�����������
        {
            if (null == ToPoint || null == FromPoint)
                return -1;
            double x1 = FromPoint.X;
            double y1 = FromPoint.Y;
            double x2 = ToPoint.X;
            double y2 = ToPoint.Y;
            double _x = x2 - x1;
            double _y = y2 - y1;
            double Distance = Math.Sqrt(Math.Pow(_x, 2) + Math.Pow(_y, 2));
            return Distance;
        }
        private bool JudgeTwoLineIsinLength_to(ILine line1, ILine line2)
        {
            double length = Math.Abs(line1.Length - line2.Length);
            if (length < this._Length_Tolerrance)
                return true;
            else
                return false;
        }
        private bool judgeTwoLineIsVerOrOnline(ILine line1, ILine line2)
        {
            IPoint Topoint1 = line1.ToPoint;
            IPoint Frompoint1 = line1.FromPoint;
            IPoint Topoint2 = line2.ToPoint;
            IPoint Frompoint2 = line2.FromPoint;
            IRelationalOperator SpaRel = Topoint1 as IRelationalOperator;
            if (SpaRel.Equals(Topoint2))
            {
                SpaRel = Frompoint1 as IRelationalOperator;
                if (!SpaRel.Equals(Frompoint2))
                {
                    double dis = CalculateDistance(Frompoint1, Frompoint2);
                    if (dis > this.Search_Tolerrance)
                        return false;
                    else
                        return true;
                }
                else
                {
                    return true;
                }
            }
            else if (SpaRel.Equals(Frompoint2))
            {
                SpaRel = Frompoint1 as IRelationalOperator;
                if (!SpaRel.Equals(Topoint2))
                {
                    double dis = CalculateDistance(Frompoint1, Topoint2);
                    if (dis > this.Search_Tolerrance)
                        return false;
                    else
                        return true;
                }
                else
                {
                    return true;
                }
            }
            //////////////////////////////////////////////
            SpaRel = Frompoint1 as IRelationalOperator;
            if (SpaRel.Equals(Topoint2))
            {
                SpaRel = Topoint1 as IRelationalOperator;
                if (!SpaRel.Equals(Frompoint2))
                {
                    double dis = CalculateDistance(Topoint1, Frompoint2);
                    if (dis > this.Search_Tolerrance)
                        return false;
                    else
                        return true;
                }
                else
                {
                    return true;
                }
            }
            else if (SpaRel.Equals(Frompoint2))
            {
                SpaRel = Topoint1 as IRelationalOperator;
                if (!SpaRel.Equals(Topoint2))
                {
                    double dis = CalculateDistance(Topoint1, Topoint2);
                    if (dis > this.Search_Tolerrance)
                        return false;
                    else
                        return true;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }

        private bool judgeTwoFeatureField(long OriOId, long DesOid)/////�ж�����Ҫ�ص������ֶ��Ƿ�ƥ��
        {
            if (this.FieldsControlList == null) return true;
            if (this._DestinatFeaCls == null) return false;
            try
            {
                IFeature OriFeature = this._DestinatFeaCls.GetFeature((int)OriOId);
                IFeature DesFeature = this._DestinatFeaCls.GetFeature((int)DesOid);
                for (int i = 0; i < this.FieldsControlList.Count; i++)
                {
                    string FieldName = this.FieldsControlList[i];
                    int index1 = OriFeature.Fields.FindField(FieldName);
                    if (index1 < 0) return false;
                    int index2 = DesFeature.Fields.FindField(FieldName);
                    if (index2 < 0) return false;
                    if (OriFeature.get_Value(index1) == null && DesFeature.get_Value(index2) == null) continue;
                    if (OriFeature.get_Value(index1) == null && DesFeature.get_Value(index2) != null) return false;
                    if (OriFeature.get_Value(index1) != null && DesFeature.get_Value(index2) == null) return false;
                    if (OriFeature.get_Value(index1).ToString().Trim() != DesFeature.get_Value(index2).ToString().Trim()) return false;
                }
                return true;
            }
            catch (Exception e)
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
                return false;
            }
        }
    }
    class ClsMergeOperationer : IMergeOperation
    {
        private bool _CreatLog;
        public bool CreatLog
        {
            get { return this._CreatLog; }
            set { this._CreatLog = value; }
        }
        public ClsMergeOperationer()/////���캯��
        {
            this._JoinFeaClss = null;
            this._SetDesValueToOri = false;
            this._CreatLog = false;
        }

        private List<IFeatureClass> _JoinFeaClss;
        public List<IFeatureClass> JoinFeaClss
        {
            get { return this._JoinFeaClss; }
            set { this._JoinFeaClss = value; }
        }
        private bool _SetDesValueToOri;
        public bool SetDesValueToOri
        {
            get { return this._SetDesValueToOri; }
            set { this._SetDesValueToOri = value; }
        }
        public bool MergePolyline(string DataSetName, long OriOID, long DesOID)
        {
            IFeatureClass FeaCls = GetFeatureClassByName(DataSetName);
            if (null == FeaCls)
                return false;
            IFeature OriFea = null;
            IFeature DesFea = null;
            try
            {
                OriFea = FeaCls.GetFeature((int)OriOID);
                DesFea = FeaCls.GetFeature((int)DesOID);
            }
            catch
            {
                return false;
            }
            if (null == OriFea || null == DesFea)
                return false;
            if (OriFea.Shape.GeometryType != esriGeometryType.esriGeometryPolyline || DesFea.Shape.GeometryType != esriGeometryType.esriGeometryPolyline)
                return false;
            UnionTwoFeat(DataSetName, OriFea, DesFea, this._SetDesValueToOri);
            return true;
        }
        public bool MergePolygon(string DataSetName, long OriOID, long DesOID)
        {
            IFeatureClass FeaCls = GetFeatureClassByName(DataSetName);
            if (null == FeaCls)
                return false;
            IFeature OriFea = null;
            IFeature DesFea = null;
            try
            {
                OriFea = FeaCls.GetFeature((int)OriOID);
                DesFea = FeaCls.GetFeature((int)DesOID);
            }
            catch (Exception e)
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
                return false;
            }
            if (null == OriFea || null == DesFea)
                return false;
            if (OriFea.Shape.GeometryType != esriGeometryType.esriGeometryPolygon || DesFea.Shape.GeometryType != esriGeometryType.esriGeometryPolygon)
                return false;
            UnionTwoFeat(DataSetName, OriFea, DesFea, this._SetDesValueToOri);
            return true;
        }
        private bool UnionTwoFeat(string DataSetName, IFeature OriFea, IFeature DesFea, bool SetDesValueToOri)
        {
            string UniLogstr = string.Empty;        
            try
            {
                IWorkspaceEdit workspaceEdit = ((IFeatureClass)OriFea.Class as IDataset).Workspace as IWorkspaceEdit;
                workspaceEdit.StartEditing(true);
                
                IRelationalOperator RelOper = OriFea.ShapeCopy as IRelationalOperator;
                if (!RelOper.Touches(DesFea.ShapeCopy))
                    return false;
                ITopologicalOperator pTopoOperator = OriFea.ShapeCopy as ITopologicalOperator;
                IGeometry pGeometry = pTopoOperator.Union(DesFea.ShapeCopy);
                pTopoOperator = pGeometry as ITopologicalOperator;
                pTopoOperator.Simplify();
                OriFea.Shape = pGeometry;

                IFields pFields = OriFea.Fields;  
                if (SetDesValueToOri)////��Ŀ��Ҫ�ص����Ը���ԴҪ��(OID,shape�ֶγ���)
                {
                    SetFieldsValue(ref OriFea, ref DesFea);
                }
                else/////////////////////��Ŀ��Ҫ�ص�����ֵ�ӵ�ԴҪ��(OID,shape�ֶγ���),�ַ����ֶ��ö��ŷָ������������
                {
                    AddFieldsValue(ref OriFea, ref DesFea);
                }
                workspaceEdit.StartEditOperation();
                OriFea.Store();
                DesFea.Delete();
                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);
                UniLogstr = "���ں�";
            }
            catch (Exception e)
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
                //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("������ʾ", "Ҫ�أ�" + OriFea.OID+"�޷����棬\n��ȷ�������ļ��Ƿ�ռ�á�");
                UniLogstr = "δ�ں�";
            }

            ///////��־
            if (this._CreatLog)
            {
                DataTable Table = new DataTable();
                DataColumn dc1 = new DataColumn("���ݼ�", Type.GetType("System.String"));
                DataColumn dc2 = new DataColumn("Ҫ������", Type.GetType("System.String"));
                DataColumn dc3 = new DataColumn("ԴҪ��ID", Type.GetType("System.Int64"));
                dc3.DefaultValue = -1;
                DataColumn dc4 = new DataColumn("Ŀ��Ҫ��ID", Type.GetType("System.Int64"));
                dc4.DefaultValue = -1;
                DataColumn dc5 = new DataColumn("������", Type.GetType("System.String"));

                Table.Columns.Add(dc1);
                Table.Columns.Add(dc2);
                Table.Columns.Add(dc3);
                Table.Columns.Add(dc4);
                Table.Columns.Add(dc5);
                double x = -1;
                double y = -2;
                ///////////////////////////////////////////////////////////////////////////
                DataRow LogRow = Table.NewRow();
                LogRow["���ݼ�"] = DataSetName;
                if (OriFea.Shape.GeometryType == esriGeometryType.esriGeometryPolyline)
                {
                    LogRow["Ҫ������"] = "Polyline";
                    x = (OriFea.ShapeCopy as IPolyline).FromPoint.X;
                    y = (OriFea.ShapeCopy as IPolyline).FromPoint.Y;
                }
                else
                {
                    LogRow["Ҫ������"] = "Polygone";
                    x = (OriFea.ShapeCopy as IPolygon).ToPoint.X;
                    y = (OriFea.ShapeCopy as IPolygon).ToPoint.Y;
                }
                LogRow["ԴҪ��ID"] = OriFea.OID;
                LogRow["Ŀ��Ҫ��ID"] = DesFea.OID;
                LogRow["������"] = UniLogstr;
                Exception ex = null;
                IJoinLOG log = new ClsJoinLog();
                log.OnDataJoin_OnMerge(LogRow, x, y, out ex);

            }

            return true;
        }
        private bool SetFieldsValue(ref IFeature pOriFeat, ref IFeature pDesFeat)/////��pDesFeat���ֶθ���pOriFeat
        {

            int IdesFiledIndex = -1;
            string sFieldName = string.Empty;

            for (int i = 0; i < pOriFeat.Fields.FieldCount; i++)
            {
                if (pOriFeat.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeOID || pOriFeat.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                    continue;
                if (pOriFeat.Fields.get_Field(i).Editable)
                {
                    sFieldName = pOriFeat.Fields.get_Field(i).Name;
                    IdesFiledIndex = pDesFeat.Fields.FindField(sFieldName);
                    if (IdesFiledIndex > -1)
                    {
                        if (pDesFeat.get_Value(IdesFiledIndex) != null)
                        {
                            pOriFeat.set_Value(i, pDesFeat.get_Value((int)IdesFiledIndex));

                        }
                        else
                        {
                            if (pDesFeat.Fields.get_Field(IdesFiledIndex).IsNullable)
                            {
                                pOriFeat.set_Value(i, null);
                            }
                            else
                            {
                                if (pDesFeat.Fields.get_Field(IdesFiledIndex).Type == esriFieldType.esriFieldTypeString)
                                {
                                    pOriFeat.set_Value(i, string.Empty);
                                }
                                else if (pDesFeat.Fields.get_Field(IdesFiledIndex).Type == esriFieldType.esriFieldTypeDouble || pDesFeat.Fields.get_Field(IdesFiledIndex).Type == esriFieldType.esriFieldTypeInteger || pDesFeat.Fields.get_Field(IdesFiledIndex).Type == esriFieldType.esriFieldTypeSingle)
                                {
                                    pOriFeat.set_Value(i, 0);
                                }
                            }

                        }
                    }
                }
            }
            return true;
        }
        private bool AddFieldsValue(ref IFeature pOriFeat, ref IFeature pDesFeat)
        {
            int IdesFiledIndex = -1;
            string sFieldName = string.Empty;

            for (int i = 0; i < pOriFeat.Fields.FieldCount; i++)
            {
                if (pOriFeat.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeOID || pOriFeat.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                    continue;
                if (pOriFeat.Fields.get_Field(i).Editable)
                {
                    sFieldName = pOriFeat.Fields.get_Field(i).Name;
                    IdesFiledIndex = pDesFeat.Fields.FindField(sFieldName);
                    if (IdesFiledIndex > -1)
                    {
                        if (!string.IsNullOrEmpty(pDesFeat.get_Value(IdesFiledIndex).ToString()))
                        {
                            if (pOriFeat.get_Value(i) == null)
                            {
                                if (pOriFeat.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeString)
                                {
                                    pOriFeat.set_Value(i, pDesFeat.get_Value(IdesFiledIndex).ToString());
                                }
                                else
                                {
                                    pOriFeat.set_Value(i, pDesFeat.get_Value(IdesFiledIndex));
                                }
                            }
                            else
                            {
                                if (pOriFeat.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeString)
                                {
                                    if (pOriFeat.get_Value(i).ToString() != pDesFeat.get_Value(IdesFiledIndex).ToString())
                                        pOriFeat.set_Value(i, pOriFeat.get_Value(i).ToString() + "," + pDesFeat.get_Value(IdesFiledIndex).ToString());
                                }
                                else if (pOriFeat.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeDouble)
                                {
                                    try
                                    {
                                        pOriFeat.set_Value(i, Convert.ToDouble(pOriFeat.get_Value(i).ToString()) + Convert.ToDouble(pDesFeat.get_Value(IdesFiledIndex).ToString()));
                                    }
                                    catch
                                    {
                                        pOriFeat.set_Value(i, Convert.ToDouble(pDesFeat.get_Value(IdesFiledIndex).ToString()));
                                    }
                                }
                                else if (pOriFeat.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeSingle)
                                {
                                    try
                                    {
                                        pOriFeat.set_Value(i, Convert.ToSingle(pOriFeat.get_Value(i).ToString()) + Convert.ToSingle(pDesFeat.get_Value(IdesFiledIndex).ToString()));
                                    }
                                    catch
                                    {
                                        pOriFeat.set_Value(i, Convert.ToSingle(pDesFeat.get_Value(IdesFiledIndex).ToString()));
                                    }
                                }
                                else if (pOriFeat.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeInteger)
                                {
                                    try
                                    {
                                        pOriFeat.set_Value(i, Convert.ToInt32(pOriFeat.get_Value(i).ToString()) + Convert.ToInt32(pDesFeat.get_Value(IdesFiledIndex).ToString()));
                                    }
                                    catch
                                    {
                                        pOriFeat.set_Value(i, Convert.ToInt32(pDesFeat.get_Value(IdesFiledIndex).ToString()));
                                    }
                                }
                                else if (pOriFeat.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeSmallInteger)
                                {
                                    try
                                    {
                                        pOriFeat.set_Value(i, Convert.ToInt16(pOriFeat.get_Value(i).ToString()) + Convert.ToInt16(pDesFeat.get_Value(IdesFiledIndex).ToString()));
                                    }
                                    catch
                                    {
                                        pOriFeat.set_Value(i, Convert.ToInt16(pDesFeat.get_Value(IdesFiledIndex).ToString()));
                                    }
                                }
                            }

                        }
                        else
                        {

                        }
                    }
                }
            }
            return true;
        }
        private IFeatureClass GetFeatureClassByName(string FeatureClassName)
        {
            if (this._JoinFeaClss == null)
                return null;
            for (int i = 0; i < this._JoinFeaClss.Count; i++)
            {
                IFeatureClass GetFeaClss = this._JoinFeaClss[i];
                if ((GetFeaClss as IDataset).Name == FeatureClassName)
                    return GetFeaClss;
            }
            return null;
        }
    }
    class ClsJoinOperationer : IJoinOperation
    {
        public ClsJoinOperationer()
        {
            this._CreatLog = false;
            this._JoinFeaClss = null;
        }
        private List<IPolyline> _borderline;/////�ӱ߽߱�
        public List<IPolyline> borderline
        {
            get { return this._borderline; }
            set { this._borderline = value; }
        }
        private bool _CreatLog;
        public bool CreatLog
        {
            get { return this._CreatLog; }
            set { this._CreatLog = value; }
        }
        private List<IFeatureClass> _JoinFeaClss;
        public List<IFeatureClass> JoinFeaClss
        {
            get { return this._JoinFeaClss; }
            set { this._JoinFeaClss = value; }
        }
        public DataTable MovePolylinePnt(DataTable OIDInfo)/////�ƶ����ϵĵ���нӱ�
        {

            DataTable ResTable = new DataTable();
            DataColumn dc1 = new DataColumn("���ݼ�", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("Ҫ������", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("ԴҪ��ID", Type.GetType("System.Int64"));
            dc3.DefaultValue = -1;
            DataColumn dc4 = new DataColumn("Ŀ��Ҫ��ID", Type.GetType("System.Int64"));
            dc4.DefaultValue = -1;
            DataColumn dc5 = new DataColumn("������", Type.GetType("System.String"));

            ResTable.Columns.Add(dc1);
            ResTable.Columns.Add(dc2);
            ResTable.Columns.Add(dc3);
            ResTable.Columns.Add(dc4);
            ResTable.Columns.Add(dc5);
            ///////////////////////////////////////////////////////////////////////////

            if (OIDInfo == null)
                return null;

            for (int i = 0; i < OIDInfo.Rows.Count; i++)
            {
                string DatasetName = OIDInfo.Rows[i]["���ݼ�"].ToString();
                IDataset Destinataset = GetFeatureClassByName(DatasetName) as IDataset;
                if (null == Destinataset)
                    return null;

                long OriOID = -1;
                long DesOID = -1;
                string OriPtn = string.Empty;
                string DesPtn = string.Empty;
                string JoinState = string.Empty;
                try
                {
                    OriOID = Convert.ToInt64(OIDInfo.Rows[i]["ԴҪ��ID"]);
                    DesOID = Convert.ToInt64(OIDInfo.Rows[i]["Ŀ��Ҫ��ID"]);
                    OriPtn = OIDInfo.Rows[i]["OriPtn"].ToString();
                    DesPtn = OIDInfo.Rows[i]["DesPtn"].ToString();
                    JoinState = OIDInfo.Rows[i]["�ӱ�״̬"].ToString();
                }
                catch
                {
                    continue;
                }
                ////////��ȡ��λ����
                double x = -1;
                double y = -1;
                if (this._CreatLog)
                {
                    IPolyline getPolyline = ((IFeatureClass)Destinataset).GetFeature((int)OriOID).Shape as IPolyline;
                    if (OriPtn == "ToPoint")
                    {
                        x = getPolyline.ToPoint.X;
                        y = getPolyline.ToPoint.Y;
                    }
                    else
                    {
                        x = getPolyline.FromPoint.X;
                        y = getPolyline.FromPoint.Y;
                    }
                }
                if (JoinState.Substring(0, 3) == "�ѽӱ�")
                {
                    DataRow newRow = ResTable.NewRow();
                    newRow["���ݼ�"] = DatasetName;
                    newRow["Ҫ������"] = "Polyline";
                    newRow["ԴҪ��ID"] = OriOID;
                    newRow["Ŀ��Ҫ��ID"] = DesOID;
                    newRow["������"] = "�ѽӱ�";
                    ResTable.Rows.Add(newRow);
                    if (this._CreatLog)
                    {
                        Exception ex = null;
                        IJoinLOG joinLog = new ClsJoinLog();
                        joinLog.OnDataJoin_OnJoin(newRow, x, y, out ex);
                    }
                }
                if (OriOID == -1)
                    continue;
                bool state = false;
                if (JoinState.Substring(0, 3) == "δ�ӱ�")
                {
                    state = PolylineDoJoin(Destinataset, OriOID, OriPtn, DesOID, DesPtn);

                    DataRow newRow = ResTable.NewRow();
                    newRow["���ݼ�"] = DatasetName;
                    newRow["Ҫ������"] = "Polyline";
                    newRow["ԴҪ��ID"] = OriOID;
                    newRow["Ŀ��Ҫ��ID"] = DesOID;
                    if (state)
                        newRow["������"] = "�ѽӱ�";
                    else
                        newRow["������"] = "δ�ӱ�";
                    ResTable.Rows.Add(newRow);
                }
            }
            return ResTable;
        }
        public DataTable MovePolygonPnt(DataTable OIDInfo)//////�ƶ�������ϵĵ���нӱ�
        {
            DataTable ResTable = new DataTable();
            DataColumn dc1 = new DataColumn("���ݼ�", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("Ҫ������", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("ԴҪ��ID", Type.GetType("System.Int64"));
            dc3.DefaultValue = -1;
            DataColumn dc4 = new DataColumn("Ŀ��Ҫ��ID", Type.GetType("System.Int64"));
            dc4.DefaultValue = -1;
            DataColumn dc5 = new DataColumn("������", Type.GetType("System.String"));

            ResTable.Columns.Add(dc1);
            ResTable.Columns.Add(dc2);
            ResTable.Columns.Add(dc3);
            ResTable.Columns.Add(dc4);
            ResTable.Columns.Add(dc5);
            ///////////////////////////////////////////////////////////////////////////
            if (null == OIDInfo)
                return null;
            for (int i = 0; i < OIDInfo.Rows.Count; i++)
            {
                DataRow getrow = OIDInfo.Rows[i];
                string FeaClsname = getrow["���ݼ�"].ToString();
                string FeaType = getrow["Ҫ������"].ToString();
                long OriOID = -1;
                long OriLineIndex = -1;
                long DesOID = -1;
                long DesLineIndex = -1;
                string JoinState = string.Empty;
                try
                {
                    OriOID = Convert.ToInt64(getrow["ԴҪ��ID"].ToString());
                    OriLineIndex = Convert.ToInt64(getrow["OriLineIndex"].ToString());
                    DesOID = Convert.ToInt64(getrow["Ŀ��Ҫ��ID"].ToString());
                    DesLineIndex = Convert.ToInt64(getrow["DesLineIndex"].ToString());
                    JoinState = getrow["�ӱ�״̬"].ToString();
                }
                catch (Exception e)
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
                    continue;
                }
                ///////��ȡ��λ����
                double x = -1;
                double y = -1;
                if (this._CreatLog)
                {
                    IPolygon GetPolygon = GetFeatureClassByName(FeaClsname).GetFeature((int)OriOID).ShapeCopy as IPolygon;
                    x = GetPolygon.ToPoint.X;
                    y = GetPolygon.ToPoint.Y;
                }
                if (JoinState == "�ѽӱ�")
                {
                    DataRow addrow = ResTable.NewRow();
                    addrow["���ݼ�"] = FeaClsname;
                    addrow["Ҫ������"] = "Polygon";
                    addrow["ԴҪ��ID"] = OriOID;
                    addrow["Ŀ��Ҫ��ID"] = DesOID;
                    addrow["������"] = "�ѽӱ�";
                    ResTable.Rows.Add(addrow);
                    if (this._CreatLog)
                    {
                        Exception ex = null;
                        IJoinLOG joinLog = new ClsJoinLog();
                        joinLog.OnDataJoin_OnJoin(addrow, x, y, out ex);
                    }
                }
                else
                {
                    if (OriOID == -1 || DesOID == -1)
                        continue;
                    bool bRes = PolygonDoJoin(FeaClsname, OriOID, OriLineIndex, DesOID, DesLineIndex);
                    DataRow addrow = ResTable.NewRow();
                    addrow["���ݼ�"] = FeaClsname;
                    addrow["Ҫ������"] = "Polygon";
                    addrow["ԴҪ��ID"] = OriOID;
                    addrow["Ŀ��Ҫ��ID"] = DesOID;
                    if (bRes)
                        addrow["������"] = "�ѽӱ�";
                    else
                        addrow["������"] = "δ�ӱ�";
                    ResTable.Rows.Add(addrow);
                }
            }
            return ResTable;

        }
        private bool PolygonDoJoin(string DataSetName, long OriFeaOID, long OriLineIndex, long DesFeaOID, long DesLineIndex)
        {

            IFeatureClass FeaCls = GetFeatureClassByName(DataSetName);

            IFeature OriFeature = null;
            IFeature DesFeature = null;
            OriFeature = FeaCls.GetFeature((int)OriFeaOID);
            DesFeature = FeaCls.GetFeature((int)DesFeaOID);
            if (null == OriFeature || null == DesFeature)
                return false;

            IPointCollection OriPoCol = OriFeature.Shape as IPointCollection;
            IPointCollection DesPoCol = DesFeature.Shape as IPointCollection;
            IPolygon Oripolygon = OriFeature.ShapeCopy as IPolygon;
            IPolygon Despolygon = DesFeature.ShapeCopy as IPolygon;

            ISegmentCollection OriSeCol = OriPoCol as ISegmentCollection;
            ISegmentCollection DesSeCol = DesPoCol as ISegmentCollection;
            ISegment Seg = null;
            ILine OriLine = null;
            ILine DesLine = null;
            Seg = OriSeCol.get_Segment((int)OriLineIndex);
            OriLine = Seg as ILine;
            Seg = DesSeCol.get_Segment((int)DesLineIndex);
            DesLine = Seg as ILine;
            if (null == OriLine || null == DesLine)
                return false;
            IPoint OriTopoint = OriLine.ToPoint;
            IPoint DesTopoint = DesLine.ToPoint;
            IPoint DesFrompoint = DesLine.FromPoint;
            //////�жϽӱ����
            double disto = -1;
            double disfrom = -1;
            disto = this.CalculateDistance(OriTopoint, DesTopoint);
            disfrom = this.CalculateDistance(OriTopoint, DesFrompoint);
            IPoint newpoint = null; ;
            ////////////////////////////��ʼ�ӱߣ��ӱ߿���ʵ�ֶ����㷨����������ѡ���е�,(guozheng 2011-2-22 �޸�Ϊ ѡ����ӱ߽߱�Ľ���)
            //double x = -1;
            //double y = -1;
            try
            {
                IWorkspaceEdit workspaceEdit = (FeaCls as IDataset).Workspace as IWorkspaceEdit;
                workspaceEdit.StartEditing(true);
                workspaceEdit.StartEditOperation();
                if (disto >= disfrom)/////Ori��ToPoint��Des��FromPoint
                {
                    IPoint GetPoint = GetInsertPoint(OriLine.ToPoint, DesLine.FromPoint);
                    if (GetPoint == null) return false;
                    //x = (OriLine.ToPoint.X + DesLine.FromPoint.X) / 2;
                    //y = (OriLine.ToPoint.Y + DesLine.FromPoint.Y) / 2;
                    newpoint = new PointClass();
                    //newpoint.PutCoords(x, y);
                    newpoint.PutCoords(GetPoint.X, GetPoint.Y);
                    ReplaceOnePntOfPolygon(OriPoCol, OriLineIndex + 1, newpoint, OriSeCol.SegmentCount);
                    ReplaceOnePntOfPolygon(DesPoCol, DesLineIndex, newpoint, DesSeCol.SegmentCount);
                    ///////////////
                    //x = (OriLine.FromPoint.X + DesLine.ToPoint.X) / 2;
                    //y = (OriLine.FromPoint.Y + DesLine.ToPoint.Y) / 2;
                    newpoint = new PointClass();
                    GetPoint = GetInsertPoint(OriLine.FromPoint, DesLine.ToPoint);
                    if (GetPoint == null) return false;
                    newpoint.PutCoords(GetPoint.X, GetPoint.Y);
                    ReplaceOnePntOfPolygon(OriPoCol, OriLineIndex, newpoint, OriSeCol.SegmentCount);
                    ReplaceOnePntOfPolygon(DesPoCol, DesLineIndex + 1, newpoint, DesSeCol.SegmentCount);
                    OriFeature.Shape = OriPoCol as IPolygon;
                    DesFeature.Shape = DesPoCol as IPolygon;
                    OriFeature.Store();
                    DesFeature.Store();
                }
                else/////Ori��ToPoint��Des��ToPoint
                {
                    //x = (OriLine.ToPoint.X + DesLine.ToPoint.X) / 2;
                    //y = (OriLine.ToPoint.Y + DesLine.ToPoint.Y) / 2;
                    IPoint GetPoint = GetInsertPoint(OriLine.ToPoint, DesLine.ToPoint);
                    if (GetPoint == null) return false;
                    newpoint = new PointClass();
                    newpoint.PutCoords(GetPoint.X, GetPoint.Y);
                    ReplaceOnePntOfPolygon(OriPoCol, OriLineIndex + 1, newpoint, OriSeCol.SegmentCount);
                    ReplaceOnePntOfPolygon(DesPoCol, DesLineIndex + 1, newpoint, DesSeCol.SegmentCount);
                    ///////////////
                    //x = (OriLine.FromPoint.X + DesLine.FromPoint.X) / 2;
                    //y = (OriLine.FromPoint.Y + DesLine.FromPoint.Y) / 2;
                    GetPoint = GetInsertPoint(OriLine.FromPoint, DesLine.FromPoint);
                    if (GetPoint == null) return false;
                    newpoint = new PointClass();
                    newpoint.PutCoords(GetPoint.X, GetPoint.Y);
                    ReplaceOnePntOfPolygon(OriPoCol, OriLineIndex, newpoint, OriSeCol.SegmentCount);
                    ReplaceOnePntOfPolygon(DesPoCol, DesLineIndex, newpoint, DesSeCol.SegmentCount);
                    OriFeature.Shape = OriPoCol as IPolygon;
                    DesFeature.Shape = DesPoCol as IPolygon;
                    OriFeature.Store();
                    DesFeature.Store();

                }
                workspaceEdit.StopEditOperation();
                workspaceEdit.StopEditing(true);
                return true;
            }
            catch (Exception eError)
            {
                if (ModData.SysLog == null) ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                ModData.SysLog.Write(eError, null, DateTime.Now);
                return false;
            }
        }
        private bool PolylineDoJoin(IDataset Dataset, long OriFeaOID, string OriPT, long DesFeaOID, string DesPT)////ִ���ߵĽӱ�
        {

            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)Dataset.Workspace;
            try////////////////////////////////////////////////////�ӱ��㷨����ʵ�ֶ���(���ն�������NewPoint�µĵ�λ)������ȡ�е�
            {
                IFeatureClass FeaCls = Dataset as IFeatureClass;
                IFeature OriFea = FeaCls.GetFeature((int)OriFeaOID);
                IFeature DesFea = FeaCls.GetFeature((int)DesFeaOID);
                IPolyline OriPolyline = OriFea.Shape as IPolyline;
                IPolyline DesPolyline = DesFea.Shape as IPolyline;
                IPoint OriPoint = null;
                IPoint DesPoint = null;
                IPoint NewPoint = new PointClass();
                if ("FromPoint" == OriPT)
                {
                    OriPoint = OriPolyline.FromPoint;
                }
                else if ("ToPoint" == OriPT)
                {
                    OriPoint = OriPolyline.ToPoint;
                }

                if ("FromPoint" == DesPT)
                {
                    DesPoint = DesPolyline.FromPoint;
                }
                else if ("ToPoint" == DesPT)
                {
                    DesPoint = DesPolyline.ToPoint;
                }

                double x = (OriPoint.X + DesPoint.X) / 2;
                double y = (OriPoint.Y + DesPoint.Y) / 2;
                NewPoint.X = x;
                NewPoint.Y = y;
                /////////////��ʼ�ӱ�    ///////����ʵ�ֵ���ȡ�е��㷨     
                try
                {
                    workspaceEdit.StartEditing(true);
                    workspaceEdit.StartEditOperation();
                    //IFeatureWorkspace Fw = (IFeatureWorkspace)this._Destinataset.Workspace;
                    //IFeatureClass DesFeaCls = Fw.OpenFeatureClass(Dataset.Name);
                    //IFeature Orif = DesFeaCls.GetFeature((int)OriFeaOID);
                    //IFeature Desf = DesFeaCls.GetFeature((int)DesFeaOID);

                    if ("FromPoint" == OriPT)
                    {
                        OriPolyline.FromPoint = NewPoint;
                    }
                    else if ("ToPoint" == OriPT)
                    {
                        OriPolyline.ToPoint = NewPoint;
                    }

                    if ("FromPoint" == DesPT)
                    {
                        DesPolyline.FromPoint = NewPoint;
                    }
                    else if ("ToPoint" == DesPT)
                    {
                        DesPolyline.ToPoint = NewPoint;
                    }
                    OriFea.Shape = OriPolyline;
                    DesFea.Shape = DesPolyline;
                    OriFea.Store();
                    DesFea.Store();
                    workspaceEdit.StopEditOperation();
                    workspaceEdit.StopEditing(true);
                }
                catch(Exception eError)
                {
                    if (ModData.SysLog == null) ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(eError, null, DateTime.Now);
                    return false;
                }

                return true;
            }
            catch (Exception e)
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
                if (workspaceEdit.IsBeingEdited())
                    workspaceEdit.StopEditing(false);
                return false;
            }
        }
        private double CalculateDistance(IPoint FromPoint, IPoint ToPoint)//////�����������
        {
            if (null == ToPoint || null == FromPoint)
                return -1;
            double x1 = FromPoint.X;
            double y1 = FromPoint.Y;
            double x2 = ToPoint.X;
            double y2 = ToPoint.Y;
            double _x = x2 - x1;
            double _y = y2 - y1;
            double Distance = Math.Sqrt(Math.Pow(_x, 2) + Math.Pow(_y, 2));
            return Distance;
        }
        private bool ReplaceOnePntOfPolygon(IPointCollection pPntCol, long lIndex, IPoint pPnt, long lSegmentCount)////�޸Ķ�����еĵ���нӱ� 
        {
            pPntCol.ReplacePoints((int)lIndex, 1, 1, ref pPnt);
            //����ı���ǵ�һ����,����Ҫ�޸����һ���������
            if (lIndex == 0)
                pPntCol.ReplacePoints((int)lSegmentCount, 1, 1, ref pPnt);

            //����ı�������һ����,����Ҫ�޸ĵ�һ���������
            if (lIndex == lSegmentCount)
                pPntCol.ReplacePoints(0, 1, 1, ref pPnt);
            return true;
        }
        private IFeatureClass GetFeatureClassByName(string FeatureClassName)
        {
            if (this._JoinFeaClss == null)
                return null;
            for (int i = 0; i < this._JoinFeaClss.Count; i++)
            {
                IFeatureClass GetFeaClss = this._JoinFeaClss[i];
                if ((GetFeaClss as IDataset).Name == FeatureClassName)
                    return GetFeaClss;
            }
            return null;
        }
        //*************************************************
        //���Դ���   ͨ�������ӱߵ���ӱ߽߱��󽻣����ؽ�����Ϊ�ӱߵ�
        private IPoint GetInsertPoint(IPoint in_FromPt, IPoint in_ToPt)
        {
            if (null == in_FromPt || null == in_ToPt) return null;
            if (this._borderline == null) return null;
            /////////ȡ����/////////////////////
            IPolyline pLine = new PolylineClass();
            pLine.ToPoint = in_ToPt;
            pLine.FromPoint = in_FromPt;
            foreach (IPolyline GetBorline in this._borderline)
            {
                if (GetBorline == null) continue;
                try
                {
                    pLine.SpatialReference = GetBorline.SpatialReference;
                    ITopologicalOperator pTopologicalOperator1 = pLine as ITopologicalOperator;
                    pTopologicalOperator1.Simplify();
                    IRelationalOperator pRelationalOperator = GetBorline as IRelationalOperator;

                    ITopologicalOperator pTopologicalOperator = GetBorline as ITopologicalOperator;
                    pTopologicalOperator.Simplify();
                    IGeometry GetPoints = pTopologicalOperator.Intersect(pLine, esriGeometryDimension.esriGeometry0Dimension) as IGeometry;
                    if (GetPoints != null)
                    {
                        if (!GetPoints.IsEmpty)
                        {
                            IGeometryCollection GeoCollection = GetPoints as IGeometryCollection;
                            IPoint ResPoint = null;
                            IGeometry Shape = GeoCollection.get_Geometry(0);
                            ResPoint = Shape as IPoint;
                            return ResPoint;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
            return null;
        }
        //*************************************************
    }
    class ClsDataOperationer/////���ݴ����ࣨ���ж���ζ�����ɾ����Ҫ��Geometry�򵥻�����
    {
        public ClsDataOperationer()
        {
            this._Angle_to = 1;
            this._OpeFeaClss = null;
        }
        private double _Angle_to;
        public double Angle_to
        {
            get { return this._Angle_to; }
            set { this._Angle_to = value; }
        }
        private IFeatureClass _OpeFeaClss;
        public IFeatureClass OpeFeaClss
        {
            get { return this._OpeFeaClss; }
            set { this._OpeFeaClss = value; }
        }
        public void RemoveRedundantPntFromPolygon(long OID)
        {
            if (null == this._OpeFeaClss)
                return;

            IFeatureClass FeaCls = this._OpeFeaClss;
            IFeature pFeat = null;
            IPointCollection pOriPntCol = null;
            IPointCollection pNewPntCol = null;
            IZAware pZAware = null;
            bool IsPolygon = false;

            /////����Ǹ��������,��������,��Ϊ����ʱ�����
            IGeometryCollection pGeometryCol = null;
            if (FeaCls.ShapeType == esriGeometryType.esriGeometryPolygon)
                IsPolygon = true;
            else
                IsPolygon = false;

            pFeat = FeaCls.GetFeature((int)OID);
            pGeometryCol = pFeat.ShapeCopy as IGeometryCollection;
            #region ��ʼ����
            if (pGeometryCol.GeometryCount == 1)
            {
                pZAware = pFeat.Shape as IZAware;
                if (pFeat.Shape.GeometryType == esriGeometryType.esriGeometryPolyline)
                {
                    pNewPntCol = new Polyline();
                    pOriPntCol = pFeat.ShapeCopy as IPointCollection;
                }
                else if (pFeat.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                {
                    pNewPntCol = new Polygon();
                    pOriPntCol = new Polyline();
                    object missing = Type.Missing;
                    pOriPntCol.AddPointCollection(pFeat.ShapeCopy as IPointCollection);
                }
                if (pZAware.ZAware)
                {
                    pZAware = pNewPntCol as IZAware;
                    pZAware.ZAware = true;
                }
                pNewPntCol = RemoveRedundantPnt(pOriPntCol, IsPolygon);

                if (pNewPntCol != null)
                {
                    try
                    {
                        IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)(this._OpeFeaClss as IDataset).Workspace;
                        workspaceEdit.StartEditing(true);
                        workspaceEdit.EnableUndoRedo();
                        IPolygon newpolygon = pNewPntCol as IPolygon;
                        pFeat.Shape = newpolygon;
                        pFeat.Store();
                        workspaceEdit.StopEditing(true);
                    }
                    catch (Exception eError)
                    {
                        if (null == ModData.SysLog) ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                        ModData.SysLog.Write(eError);
                    }
                }


            }
            #endregion
        }
        public void GeometrySimplify(long OID)
        {
            if (null == this._OpeFeaClss)
                return;
            IFeatureClass ipFtClass = this._OpeFeaClss;
            IFeature Fea = ipFtClass.GetFeature((int)OID);
            IGeometryCollection GeoCol = Fea.Shape as IGeometryCollection;
            if (GeoCol.GeometryCount > 1)
            {
                ITopologicalOperator Topo = Fea.Shape as ITopologicalOperator;
                Topo.Simplify();
            }
            ipFtClass = null;
            Fea = null;
            GeoCol = null;
        }
        private IPointCollection RemoveRedundantPnt(IPointCollection pOriPntCol, bool IsPolygon)
        {
            IPointCollection pNewPntCol = null;
            if (IsPolygon)
                pNewPntCol = new Polygon();
            else
                pNewPntCol = new Polyline();
            long i = -1;
            ILine pFirstLine = null;
            ILine pSecontLine = null;
            long lSegmentCount = -1;
            ISegmentCollection pSegmentCol = null;
            ISegment pSegment = null;
            bool bIsParallel = false;

            if (pOriPntCol == null)
                return null;


            pSegmentCol = pOriPntCol as ISegmentCollection;
            lSegmentCount = pSegmentCol.SegmentCount;
            object missing = Type.Missing;
            if (lSegmentCount > 1)
            {
                for (i = 0; i <= lSegmentCount - 2; i++)
                {
                    pSegment = pSegmentCol.get_Segment((int)i);
                    if (pSegment.GeometryType == esriGeometryType.esriGeometryLine)
                        pFirstLine = pSegment as ILine;
                    else
                        return null;
                    pSegment = pSegmentCol.get_Segment((int)i + 1);
                    if (pSegment.GeometryType == esriGeometryType.esriGeometryLine)
                        pSecontLine = pSegment as ILine;
                    else
                        return null;

                    bIsParallel = ClsCheckOperationer.JudgeParallel(pFirstLine, pSecontLine, this._Angle_to);

                    if (bIsParallel == false)
                    {
                        if (i == 0)
                        {
                            pNewPntCol.AddPoint(pOriPntCol.get_Point((int)i), ref missing, ref missing);
                            pNewPntCol.AddPoint(pOriPntCol.get_Point((int)i + 1), ref missing, ref missing);
                        }
                        else
                        {
                            pNewPntCol.AddPoint(pOriPntCol.get_Point((int)i + 1), ref missing, ref missing);
                        }

                    }
                    else
                    {
                        if (i == 0)
                            pNewPntCol.AddPoint(pOriPntCol.get_Point((int)i), ref missing, ref missing);


                    }
                }
                //������һ����

                pNewPntCol.AddPoint(pOriPntCol.get_Point((int)i + 1), ref missing, ref missing);

                //����Ƕ����,����Ҫ�жϵ�һ���ߺ����һ�����Ƿ�ƽ��,
                //���ƽ�����Ƴ���һ������,ͬʱ�޸����һ������
                if (IsPolygon && pNewPntCol.PointCount > 4)
                {
                    //����Ҫ�ж����һ���ߺ͵�һ�����Ƿ�ƽ��,���ƽ��,��Ҫ�Ƴ���һ����
                    pSegment = pSegmentCol.get_Segment(0);
                    if (pSegment.GeometryType == esriGeometryType.esriGeometryLine)
                        pFirstLine = pSegment as ILine;

                    pSegment = pSegmentCol.get_Segment((int)lSegmentCount - 1);
                    if (pSegment.GeometryType == esriGeometryType.esriGeometryLine)
                        pSecontLine = pSegment as ILine;

                    bIsParallel = ClsCheckOperationer.JudgeParallel(pFirstLine, pSecontLine, this._Angle_to);
                    if (bIsParallel)
                    {
                        pNewPntCol.RemovePoints(0, 1);
                        IPoint newpoint = pNewPntCol.get_Point(0);
                        pNewPntCol.ReplacePoints(pNewPntCol.PointCount - 1, 1, 1, ref newpoint);
                        //pNewPntCol.SetPoints(0,ref newpoint);
                    }
                }

            }
            pSegmentCol = null;
            return pNewPntCol;
        }
    }

    class ClsJoinLog : IJoinLOG
    {

        private string _Logpath;
        public string LogPath
        {
            get { return this._Logpath; }
            set { this._Logpath = value; }
        }
        /// <summary>
        /// ��ʼ����־�ļ�
        /// </summary>
        /// <param name="ex"></param>
        public void InitialLog(out Exception ex)
        {
            ex = null;
            if (string.IsNullOrEmpty(ModData.v_JoinSettingXML))
            {
                ex = new Exception("��ȡ�ӱ߲��������ļ�ʧ��!");
                return;
            }
            if (!File.Exists(ModData.v_JoinSettingXML))
            {
                ex = new Exception("��ȡ�ӱ߲��������ļ���ʧ�����飺" + ModData.v_JoinSettingXML);
                return;
            }
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(ModData.v_JoinSettingXML);
            if (XmlDoc == null)
            {
                ex = new Exception("��ȡ�ӱ߲��������ļ�ʧ��!");
                return;
            }
            XmlElement ele = XmlDoc.SelectSingleNode(".//�ӱ�����") as XmlElement;
            ////// ��ʼ������
            #region  ��ʼ������
            string sDisto = ele.GetAttribute("�����ݲ�");
            string sAngelto = ele.GetAttribute("�Ƕ��ݲ�");
            string slegthto = ele.GetAttribute("�����ݲ�");
            string sSearchto = ele.GetAttribute("�����ݲ�");
            string sjointype = ele.GetAttribute("�ӱ�����");
            string sIsDelPnt = ele.GetAttribute("ɾ������ζ����");
            string sIsSimplify = ele.GetAttribute("�򵥻�Ҫ��");

            ele = XmlDoc.SelectSingleNode(".//��־����") as XmlElement;
            string Logpath = ele.GetAttribute("��־·��");
            if (string.IsNullOrEmpty(Logpath))
            {
                ex = new Exception("��־·����ȡʧ�ܣ�");
                return;
            }
            if (File.Exists(Logpath))
            {
                File.Delete(Logpath);
            }
            try
            {
                File.Copy(ModData.v_JoinLOGXML, Logpath);
            }
            catch (Exception e)
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
                ex = new Exception("��ʼ���ӱ���־ʧ�ܣ���ȷ����־ģ���ļ��Ƿ���ڡ�");
                return;
            }
            XmlDoc.Load(Logpath);
            ele = XmlDoc.SelectSingleNode(".//������/����") as XmlElement;
            ele.SetAttribute("�����ݲ�", sDisto);
            ele.SetAttribute("�Ƕ��ݲ�", sAngelto);
            ele.SetAttribute("�����ݲ�", slegthto);
            ele.SetAttribute("�����ݲ�", sSearchto);
            ele.SetAttribute("�ӱ�����", sjointype);
            ele.SetAttribute("�Ƿ�ɾ������ζ����", sIsDelPnt);
            ele.SetAttribute("�Ƿ�Ҫ�ؼ򵥻�", sIsSimplify);
            try
            {
                XmlDoc.Save(Logpath);
            }
            catch (Exception e)
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
                ex = new Exception("�ӱ߲���д��ӱ���־ʧ�ܣ���ȷ��־�ļ��Ƿ���ڻ�Ϊֻ��״̬");
                return;
            }
            #endregion

        }

        public void onDataJoin_JoinDataSet(int state, List<IFeatureClass> DataSetList, out Exception ex)
        {
            ex = null;
            if (DataSetList == null)
            {
                ex = new Exception("û�нӱ�ͼ��");
                return;
            }
            if (string.IsNullOrEmpty(this._Logpath))
            {
                ex = new Exception("��־·��δ��ʼ����");
                return;
            }
            XmlDocument Doc = new XmlDocument();
            Doc.Load(_Logpath);
            if (Doc == null)
            {
                ex = new Exception("��־����ʧ��!");
                return;
            }
            XmlElement getele = null;
            switch (state)
            {
                case 0://///������
                    getele = Doc.SelectSingleNode(".//������/���ͼ��") as XmlElement;
                    break;
                case 1://///�ӱ߲���
                    getele = Doc.SelectSingleNode(".//�ӱ߲���/�ӱ�ͼ��") as XmlElement;
                    break;
                case 2:////�ںϲ���
                    getele = Doc.SelectSingleNode(".//�ںϲ���/�ں�ͼ��") as XmlElement;
                    break;
            }
            if (null == getele)
            {
                ex = new Exception("�����ڵ��ȡʧ��");
                return;
            }
            for (int i = 0; i < DataSetList.Count; i++)
            {
                string sDataSetName = (DataSetList[i] as IDataset).Name;
                string sPath = (DataSetList[i] as IDataset).Workspace.ConnectionProperties.ToString();
                XmlNode addNode = Doc.CreateNode(XmlNodeType.Element, string.Empty, string.Empty);

                XmlAttribute addAttr = null;
                ///////
                addAttr = Doc.CreateAttribute("�ӱ�ͼ��");
                addAttr.Value = sDataSetName;
                addNode.Attributes.SetNamedItem(addAttr);
                ///////
                addAttr = Doc.CreateAttribute("����·��");
                addAttr.Value = sPath;
                addNode.Attributes.SetNamedItem(addAttr);
                getele.AppendChild(addNode);
            }
            try
            {
                Doc.Save(_Logpath);
            }
            catch (Exception e)
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
                ex = new Exception("�ӱ߼����־д��ʧ�ܣ�");
                return;
            }

        }

        public void onDataJoin_Start(int state, out Exception ex)
        {
            ex = null;
            XmlDocument Doc = new XmlDocument();
            if (string.IsNullOrEmpty(this._Logpath))
            {
                //ex = new Exception("��־·��δ��ʼ����");
                //return;

                Doc.Load(ModData.v_JoinSettingXML);
                if (null == Doc)
                {
                    ex = new Exception("��ȡ�ӱ߲��������ļ�ʧ��");
                    return;
                }
                XmlElement ele = Doc.SelectSingleNode(".//��־����") as XmlElement;
                this._Logpath = ele.GetAttribute("��־·��");
                if (string.IsNullOrEmpty(this._Logpath))
                {
                    ex = new Exception("��־·��δ��ʼ����");
                    return;
                }
            }
            ////////��־�ļ����������ʼ����һ��־�ļ�
            if (!File.Exists(_Logpath))
            {
                IJoinLOG JoinLog = new ClsJoinLog();
                JoinLog.InitialLog(out ex);
                if (ex != null)
                {
                    return;
                }
            }
            Doc.Load(_Logpath);
            if (Doc == null)
            {
                ex = new Exception("��־����ʧ��!");
                return;
            }
            XmlElement getele = null;
            switch (state)
            {
                case 0://///������
                    getele = Doc.SelectSingleNode(".//������") as XmlElement;
                    break;
                case 1://///�ӱ߲���
                    getele = Doc.SelectSingleNode(".//�ӱ߲���") as XmlElement;
                    break;
                case 2:////�ںϲ���
                    getele = Doc.SelectSingleNode(".//�ںϲ���") as XmlElement;
                    break;
            }
            if (null == getele)
            {
                ex = new Exception("�����ڵ��ȡʧ��");
                return;
            }
            getele.SetAttribute("��ʼʱ��", DateTime.Now.ToLongTimeString());
            try
            {
                Doc.Save(_Logpath);
            }
            catch (Exception e)
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
                ex = new Exception("�ӱ߼����־д��ʧ�ܣ�");
                return;
            }
        }

        public void onDataJoin_Terminate(int state, out Exception ex)
        {
            ex = null;
            XmlDocument Doc = new XmlDocument();
            if (string.IsNullOrEmpty(this._Logpath))
            {
                //ex = new Exception("��־·��δ��ʼ����");
                //return;

                Doc.Load(ModData.v_JoinSettingXML);
                if (null == Doc)
                {
                    ex = new Exception("��ȡ�ӱ߲��������ļ�ʧ��");
                    return;
                }
                XmlElement ele = Doc.SelectSingleNode(".//��־����") as XmlElement;
                this._Logpath = ele.GetAttribute("��־·��");
                if (string.IsNullOrEmpty(this._Logpath))
                {
                    ex = new Exception("��־·��δ��ʼ����");
                    return;
                }
            }

            Doc.Load(_Logpath);
            if (Doc == null)
            {
                ex = new Exception("��־����ʧ��!");
                return;
            }
            XmlElement getele = null;
            switch (state)
            {
                case 0://///������
                    getele = Doc.SelectSingleNode(".//������") as XmlElement;
                    break;
                case 1://///�ӱ߲���
                    getele = Doc.SelectSingleNode(".//�ӱ߲���") as XmlElement;
                    break;
                case 2:////�ںϲ���
                    getele = Doc.SelectSingleNode(".//�ںϲ���") as XmlElement;
                    break;
            }
            if (null == getele)
            {
                ex = new Exception("�����ڵ��ȡʧ��");
                return;
            }
            getele.SetAttribute("����ʱ��", DateTime.Now.ToLongTimeString());
            try
            {
                Doc.Save(_Logpath);
            }
            catch (Exception e)
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
                ex = new Exception("�ӱ߼����־д��ʧ�ܣ�");
                return;
            }


        }

        public void OnDataJoin_OnCheck(DataRow in_DataRow, double x, double y, out Exception ex)
        {
            ex = null;
            XmlDocument Doc = new XmlDocument();
            if (string.IsNullOrEmpty(this._Logpath))
            {
                //ex = new Exception("��־·��δ��ʼ����");
                //return;

                Doc.Load(ModData.v_JoinSettingXML);
                if (null == Doc)
                {
                    ex = new Exception("��ȡ�ӱ߲��������ļ�ʧ��");
                    return;
                }
                XmlElement ele = Doc.SelectSingleNode(".//��־����") as XmlElement;
                this._Logpath = ele.GetAttribute("��־·��");
                if (string.IsNullOrEmpty(this._Logpath))
                {
                    ex = new Exception("��־·��δ��ʼ����");
                    return;
                }
            }

            Doc.Load(_Logpath);
            if (Doc == null)
            {
                ex = new Exception("��־����ʧ��!");
                return;
            }
            string DataSetName = string.Empty;
            string Geometrytype = string.Empty;
            string Result = string.Empty;
            long OriOID = -1;
            long DesOId = -1;
            try
            {
                DataSetName = in_DataRow["���ݼ�"].ToString().Trim();
                Geometrytype = in_DataRow["Ҫ������"].ToString().Trim();
                Result = in_DataRow["�ӱ�״̬"].ToString().Trim();
                OriOID = Convert.ToInt64(in_DataRow["ԴҪ��ID"].ToString());
                DesOId = Convert.ToInt64(in_DataRow["Ŀ��Ҫ��ID"].ToString());
            }
            catch (Exception e)
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
                ex = new Exception("��ȡ�ӱ���Ϣʧ��");
                return;
            }

            XmlNode resultNode = null;
            if (Geometrytype == "Polyline")
                resultNode = Doc.SelectSingleNode("//������/�����/��Ҫ��");
            else if (Geometrytype == "Polygon")
                resultNode = Doc.SelectSingleNode("//������/�����/��Ҫ��");


            XmlNode addNode = Doc.CreateNode(XmlNodeType.Element, "Ҫ��", null);

            XmlAttribute addAttr = null;
            ///////
            addAttr = Doc.CreateAttribute("�ӱ�ͼ��");
            addAttr.Value = DataSetName;
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("Ҫ������");
            addAttr.Value = Geometrytype;
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("ԴҪ��OID");
            addAttr.Value = OriOID.ToString();
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("Ŀ��Ҫ��OID");
            addAttr.Value = DesOId.ToString();
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("��λ����X");
            addAttr.Value = x.ToString();
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("��λ����Y");
            addAttr.Value = y.ToString();
            addNode.Attributes.SetNamedItem(addAttr);


            addAttr = Doc.CreateAttribute("�����");
            addAttr.Value = Result;
            addNode.Attributes.SetNamedItem(addAttr);
            ///////
            resultNode.AppendChild(addNode);
            try
            {
                Doc.Save(_Logpath);
            }
            catch (Exception e)
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
                ex = new Exception("�ӱ߼����־д��ʧ�ܣ�");
                return;
            }
        }

        public void OnDataJoin_OnJoin(DataRow in_DataRow, double x, double y, out Exception ex)
        {
            ex = null;
            XmlDocument Doc = new XmlDocument();
            if (string.IsNullOrEmpty(this._Logpath))
            {
                //ex = new Exception("��־·��δ��ʼ����");
                //return;

                Doc.Load(ModData.v_JoinSettingXML);
                if (null == Doc)
                {
                    ex = new Exception("��ȡ�ӱ߲��������ļ�ʧ��");
                    return;
                }
                XmlElement ele = Doc.SelectSingleNode(".//��־����") as XmlElement;
                this._Logpath = ele.GetAttribute("��־·��");
                if (string.IsNullOrEmpty(this._Logpath))
                {
                    ex = new Exception("��־·��δ��ʼ����");
                    return;
                }
            }
            // XmlDocument Doc = new XmlDocument();
            Doc.Load(_Logpath);
            if (Doc == null)
            {
                ex = new Exception("��־����ʧ��!");
                return;
            }
            string DataSetName = string.Empty;
            string Geometrytype = string.Empty;
            string Result = string.Empty;
            long OriOID = -1;
            long DesOId = -1;
            try
            {
                DataSetName = in_DataRow["���ݼ�"].ToString().Trim();
                Geometrytype = in_DataRow["Ҫ������"].ToString().Trim();
                Result = in_DataRow["������"].ToString().Trim();
                OriOID = Convert.ToInt64(in_DataRow["ԴҪ��ID"].ToString());
                DesOId = Convert.ToInt64(in_DataRow["Ŀ��Ҫ��ID"].ToString());
            }
            catch (Exception e)
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
                ex = new Exception("��ȡ�ӱ���Ϣʧ��");
                return;
            }

            XmlNode resultNode = null;
            if (Geometrytype == "Polyline")
                resultNode = Doc.SelectSingleNode("//�ӱ߲���/�ӱ߽��/��Ҫ��");
            else if (Geometrytype == "Polygon")
                resultNode = Doc.SelectSingleNode("//�ӱ߲���/�ӱ߽��/��Ҫ��");
            XmlNode addNode = Doc.CreateNode(XmlNodeType.Element, "Ҫ��", null);

            XmlAttribute addAttr = null;
            ///////
            addAttr = Doc.CreateAttribute("�ӱ�ͼ��");
            addAttr.Value = DataSetName;
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("Ҫ������");
            addAttr.Value = Geometrytype;
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("ԴҪ��OID");
            addAttr.Value = OriOID.ToString();
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("Ŀ��Ҫ��OID");
            addAttr.Value = DesOId.ToString();
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("��λ����X");
            addAttr.Value = x.ToString();
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("��λ����Y");
            addAttr.Value = y.ToString();
            addNode.Attributes.SetNamedItem(addAttr);


            addAttr = Doc.CreateAttribute("�ӱ߽��");
            addAttr.Value = Result;
            addNode.Attributes.SetNamedItem(addAttr);
            ///////
            resultNode.AppendChild(addNode);
            try
            {
                Doc.Save(_Logpath);
            }
            catch (Exception e)
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
                ex = new Exception("�ӱ߼����־д��ʧ�ܣ�");
                return;
            }
        }

        public void OnDataJoin_OnMerge(DataRow in_DataRow, double x, double y, out Exception ex)
        {
            ex = null;
            XmlDocument Doc = new XmlDocument();
            if (string.IsNullOrEmpty(this._Logpath))
            {
                //ex = new Exception("��־·��δ��ʼ����");
                //return;

                Doc.Load(ModData.v_JoinSettingXML);
                if (null == Doc)
                {
                    ex = new Exception("��ȡ�ӱ߲��������ļ�ʧ��");
                    return;
                }
                XmlElement ele = Doc.SelectSingleNode(".//��־����") as XmlElement;
                this._Logpath = ele.GetAttribute("��־·��");
                if (string.IsNullOrEmpty(this._Logpath))
                {
                    ex = new Exception("��־·��δ��ʼ����");
                    return;
                }
            }
            // XmlDocument Doc = new XmlDocument();
            Doc.Load(_Logpath);
            if (Doc == null)
            {
                ex = new Exception("��־����ʧ��!");
                return;
            }
            string DataSetName = string.Empty;
            string Geometrytype = string.Empty;
            string Result = string.Empty;
            long OriOID = -1;
            long DesOId = -1;
            try
            {
                DataSetName = in_DataRow["���ݼ�"].ToString().Trim();
                Geometrytype = in_DataRow["Ҫ������"].ToString().Trim();
                Result = in_DataRow["������"].ToString().Trim();
                OriOID = Convert.ToInt64(in_DataRow["ԴҪ��ID"].ToString());
                DesOId = Convert.ToInt64(in_DataRow["Ŀ��Ҫ��ID"].ToString());
            }
            catch (Exception e)
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
                ex = new Exception("��ȡ�ӱ���Ϣʧ��");
                return;
            }

            XmlNode resultNode = null;
            if (Geometrytype == "Polyline")
                resultNode = Doc.SelectSingleNode("//�ںϲ���/�ںϽ��/��Ҫ��");
            else if (Geometrytype == "Polygone")
                resultNode = Doc.SelectSingleNode("//�ںϲ���/�ںϽ��/��Ҫ��");
            XmlNode addNode = Doc.CreateNode(XmlNodeType.Element, "Ҫ��", null);

            XmlAttribute addAttr = null;
            ///////
            addAttr = Doc.CreateAttribute("�ں�ͼ��");
            addAttr.Value = DataSetName;
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("Ҫ������");
            addAttr.Value = Geometrytype;
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("ԴҪ��OID");
            addAttr.Value = OriOID.ToString();
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("Ŀ��Ҫ��OID");
            addAttr.Value = DesOId.ToString();
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("��λ����X");
            addAttr.Value = x.ToString();
            addNode.Attributes.SetNamedItem(addAttr);

            addAttr = Doc.CreateAttribute("��λ����Y");
            addAttr.Value = y.ToString();
            addNode.Attributes.SetNamedItem(addAttr);


            addAttr = Doc.CreateAttribute("�ںϽ��");
            addAttr.Value = Result;
            addNode.Attributes.SetNamedItem(addAttr);
            ///////
            if (resultNode != null)
            {
                resultNode.AppendChild(addNode);
                try
                {
                    Doc.Save(_Logpath);
                }
                catch (Exception e)
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
                    ex = new Exception("�ӱ߼����־д��ʧ�ܣ�");
                    return;
                }
            }
        }
    }
}
