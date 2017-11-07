using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using System.Runtime.InteropServices;

namespace GeoDataEdit
{
    /// <summary>
    /// ��׽�������ö��ֵ
    /// </summary>
    public enum PointType { CenterPoint,PortPoint,MidPoint,VertexPoint,BoundryPoint,IntersectPoint};

    public class SnapPointClass
    {

        #region ����

        #region ����ӿڱ���

        #region �Ƿ�����׽  IsOpenSnap
        private bool b_IsOpenSnap;
        public bool IsOpenSnap
        {
            get { return b_IsOpenSnap; }
            set { this.b_IsOpenSnap = value; }
        }
        #endregion

        #region �Ƿ�׽�˵� IsSnapPortPoint
        private bool b_IsSnapPortPoint;
        public bool IsSnapPortPoint
        {
            get { return this.b_IsSnapPortPoint; }
            set { this.b_IsSnapPortPoint = value; }
        }
        #endregion

        #region �Ƿ�׽�е� IsSnapMidPoint
        private bool b_IsSnapMidPoint;
        public bool IsSnapMidPoint
        {
            get { return this.b_IsSnapMidPoint; }
            set { this.b_IsSnapMidPoint = value; }
        }
        /***************************************************************/
        #endregion

        #region �Ƿ�׽���� IsSnapIntersectPoint
        private bool b_IsSnapIntersectPoint;
        public bool IsSnapIntersectPoint
        {
            get { return this.b_IsSnapIntersectPoint; }
            set { this.b_IsSnapIntersectPoint = value; }
        }
        #endregion

        #region �Ƿ�׽��� IsSnapNodePoint
        private bool b_IsSnapNodePoint;
        public bool IsSnapNodePoint
        {
            get { return this.b_IsSnapNodePoint; }
            set { this.b_IsSnapNodePoint = value; }
        }
        #endregion

        #region �Ƿ�׽���ϵ� IsSnapBoundryPoint
        private bool b_IsSnapBoundryPoint;
        public bool IsSnapBoundryPoint
        {
            get { return this.b_IsSnapBoundryPoint; }
            set { this.b_IsSnapBoundryPoint = value; }
        }
        #endregion

        #region �Ƿ�׽���ĵ� IsSnapCenterPoint
        private bool b_IsSnapCenterPoint;
        public bool IsSnapCenterPoint
        {
            get { return this.b_IsSnapCenterPoint; }
            set { this.b_IsSnapCenterPoint = value; }
        }
        #endregion

        #region ��׽�뾶(��ͼ) SnapMapRadius
        private double d_SnapMapRadius;
        public double SnapMapRadius
        {
            get { return this.d_SnapMapRadius; }
        }
        #endregion

        #region ����뾶(��ͼ) CacheMapRadius
        private double d_CacheMapRadius;
        public double CacheMapRadius
        {
            get { return this.d_CacheMapRadius; }
        }
        #endregion

        #region ��׽�뾶(��Ļ) SnapPixelRadius
        private double d_SnapPixelRadius;
        public double SnapPixelRadius
        {
            get { return d_SnapPixelRadius; }
            set
            {
                this.d_SnapPixelRadius = value;
                if ( m_CurrentMap != null )
                    d_SnapMapRadius = ConvertPixelDistanceToMapDistance ( this.d_SnapPixelRadius );
            }
        }
        #endregion

        #region ����뾶(��Ļ) CachePixelRadius
        private double d_CachePixelRadius;
        public double CachePixelRadius
        {
            get { return d_CachePixelRadius; }
            set
            {
                this.d_CachePixelRadius = value;
                if ( m_CurrentMap != null )
                    d_CacheMapRadius = ConvertPixelDistanceToMapDistance ( this.d_CachePixelRadius );
            }
        }
        #endregion

        #region ��׽����� SnapResultPoint
        private IPoint m_SnapResultPoint;
        public IPoint SnapResultPoint
        {
            get { return this.m_SnapResultPoint; }
        }
        #endregion

        #region ��׽�ο��� SnapRefrencePoint
        private IPoint m_SnapRefrencePoint;
        private IPoint SnapRefrencePoint
        {
            get { return this.m_SnapRefrencePoint; }
            set { this.m_SnapRefrencePoint = value; }
        }
        #endregion

        #region ��ǰMap  CurrentMap
        private IMap m_CurrentMap;
        #endregion

        #endregion

        #region �����
        private IFeatureCache2 m_FeatureCache;
        public IPoint m_LastSnapPoint;
        private PointType m_PointType;
        private ISymbol m_LastSnapSymbol;
        private double m_IgnoreDistance;
        private IMap m_pNewMap;//�������Cache
        private double m_pMapScale;//���ڵ�ͼ�����߱仯ʱ
        #endregion

        #endregion

        /*************************���캯��********************************/
        public SnapPointClass ()
        {
            DefaultSetting ();
            Init ();
        }

        /// <summary>
        /// �û�����
        /// </summary>        
        public void CustomSetting ( bool _IsSnapPortPoint , bool _IsSnapMidPoint , bool _IsSnapNodePoint ,
            bool _IsSnapIntersectPoint , bool _IsSnapPointPoint , bool _IsSnapBoundryPoint,bool _IsSnapCenterPoint,
            double _SnapPixelRadius , double _CachePixelRadius )
        {
            IsOpenSnap = true;
            IsSnapPortPoint = _IsSnapPortPoint;
            IsSnapMidPoint = _IsSnapMidPoint;
            IsSnapIntersectPoint = _IsSnapIntersectPoint;
            IsSnapBoundryPoint = _IsSnapBoundryPoint;
            IsSnapCenterPoint = _IsSnapCenterPoint;
            IsSnapNodePoint = _IsSnapNodePoint;

            SnapPixelRadius = _SnapPixelRadius;
            CachePixelRadius = _CachePixelRadius;
        }

        /// <summary>
        /// Ĭ������
        /// </summary>
        private void DefaultSetting ()
        {
            IsOpenSnap = false;

            IsSnapPortPoint = true;
            IsSnapNodePoint = false;
            IsSnapMidPoint = false;
            IsSnapIntersectPoint = false;
            IsSnapCenterPoint = false;
            IsSnapBoundryPoint = false;
            //b_IsHasSnaped = false;
            //IsReFillCache = true;

            SnapPixelRadius = 5;
            CachePixelRadius = 300;

            m_IgnoreDistance = 0;
        }

        /// <summary>
        /// �����ʼ��
        /// </summary>
        private void Init ()
        {
            SnapRefrencePoint = null;
            m_SnapResultPoint = null;


            m_LastSnapPoint = null;
            m_FeatureCache = new FeatureCacheClass ();
            m_LastSnapSymbol = null;
            m_PointType = PointType.VertexPoint;
        }

        /// <summary>
        /// ����ӿڣ�ִ�в�׽��Ĳ���
        /// ִ�иò���֮ǰ����Ҫ��CurrentMap������ֵ
        /// ����CurrentMap�в��LayerDescriptionֵ����ΪXML��ʽ
        /// ���򽫸���XML�е�IsSnap����ȷ���Ƿ�Ըò���в�׽����
        /// </summary>
        public bool SnapExcute ( IPoint _SnapPoint  )
        {           
            //�����׽δ��������ô��׽�㼴Ϊ�ο���
            //b_IsHasSnaped = false;
            if ( !IsOpenSnap )
            {
                m_SnapResultPoint = _SnapPoint;                
                return false;
            }

            m_PointType = PointType.VertexPoint;
            /*******��ʱ��׽����***********/

            //���ñ�����
            if ( m_pMapScale != m_CurrentMap.MapScale )
            {
                m_pMapScale = m_CurrentMap.MapScale;
                d_SnapMapRadius = ConvertPixelDistanceToMapDistance ( this.d_SnapPixelRadius );
                d_CacheMapRadius = ConvertPixelDistanceToMapDistance ( this.d_CachePixelRadius );
            }

            SnapRefrencePoint = _SnapPoint;//��ǰ���㣬���ο���
            m_SnapResultPoint = null;
            if ( !isCanExcute () ) return false;


            //������仺����
            if ( m_FeatureCache == null )
                m_FeatureCache = new FeatureCacheClass ();

            if ( !m_FeatureCache.Contains ( _SnapPoint ) )
            {
                if ( !ReFillCache () ) return false;
            }
            

            HitFeature ();

            /*********************�����㷨**********************************/
            if ( m_LastSnapPoint != null && m_LastSnapSymbol != null )
            {
                DrawPoint ( m_LastSnapPoint , m_LastSnapSymbol as ISymbol );
            }
            if ( SnapResultPoint != null && SnapResultPoint.IsEmpty == false )
            {
                DrawPoint ( SnapResultPoint , SetSnapSymbol () as ISymbol );
            }           
            m_LastSnapPoint = SnapResultPoint;
            m_LastSnapSymbol = SetSnapSymbol () as ISymbol;

            //���û�в�׽���㣬��ô�Ѳ�׽�ο��㷵��
            if ( SnapResultPoint == null || SnapResultPoint.IsEmpty )
                m_SnapResultPoint = SnapRefrencePoint;

            return true;

        }

        /// <summary>
        /// �Ӵ����Map�����еõ�����׽ͼ����б�
        /// LayerDescription
        /// </summary>
        /// <returns></returns>
        public bool InitMap ( IMap pMap )
        {
            //��LayerDescription��ȡ������Ϣ����ʼ��ͼ���б�
            if ( pMap == null ) return false;
            if ( m_pNewMap == null ) m_pNewMap = new MapClass ();
            m_pNewMap.ClearLayers ();
            m_CurrentMap = pMap;
            //XmlDocument xmlDoc = new XmlDocument ();
            //XmlNode pSnapNode;

            for ( int i = 0 ; i < pMap.LayerCount ; i++ )
            {
                if ( pMap.get_Layer ( i ) is IFeatureLayer )
                {
                    try
                    {
                        if ((pMap.get_Layer(i) as IFeatureLayer).FeatureClass == null) continue;
                        if ((pMap.get_Layer(i) as IFeatureLayer).FeatureClass.FeatureType != esriFeatureType.esriFTSimple) continue;
                       
                        //xmlDoc.LoadXml((pMap.get_Layer(i) as ILayerGeneralProperties).LayerDescription);
                        //pSnapNode = xmlDoc.SelectSingleNode("//*[@IsSnap='true']");
                        //if (pSnapNode != null)
                        //{
                            //�ò�ɼ�������IGeoFeatureLayer����
                        if (pMap.get_Layer(i).Visible && pMap.get_Layer(i) is IGeoFeatureLayer)
                        {
                            IObjectCopy pOC = new ObjectCopyClass();
                            m_pNewMap.AddLayer(pOC.Copy(pMap.get_Layer(i)) as ILayer);
                        }
                        //}
                    }
                    catch
                    { 
                    
                    }

                }
            }

            m_pMapScale = pMap.MapScale;
            d_SnapMapRadius = ConvertPixelDistanceToMapDistance ( d_SnapPixelRadius );
            d_CacheMapRadius = ConvertPixelDistanceToMapDistance ( d_CachePixelRadius );

            return true;
        }

        /// <summary>
        /// ��֤��ǰ��Ϣ�Ƿ����ִ�в�׽��Ĳ���
        /// </summary>
        /// <returns></returns>
        private bool isCanExcute ()
        {
            if ( m_CurrentMap == null ) return false;//δ���õ�ǰ��ͼ
            if ( SnapRefrencePoint == null ) return false;//δ���ò�׽�ο���
            return true;
        }

        /// <summary>
        /// ������仺����
        /// </summary>
        public bool ReFillCache ()
        {
            if ( SnapRefrencePoint == null ) return false;
            if ( CacheMapRadius == 0 ) return false;
            m_FeatureCache = new FeatureCacheClass ();
            m_FeatureCache.Initialize ( SnapRefrencePoint , CacheMapRadius );            
            
            if ( m_pNewMap == null ) return false;
            UID pUID = new UIDClass ();
            pUID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";
            if ( m_pNewMap.LayerCount > 0 )
            {
                IEnumLayer pEnumLayer = m_pNewMap.get_Layers ( pUID , true );
                m_FeatureCache.AddLayers ( pEnumLayer , GetPointBufferEnvelope (SnapRefrencePoint, CacheMapRadius ) );
                return true;
            }
            return false;
        }

        /// <summary>
        /// �õ���pPoint��Ϊ����
        /// radiusΪ�뾶��Envelope
        /// </summary>
        private IEnvelope GetPointBufferEnvelope (IPoint pPoint, double radius )
        {
            IEnvelope pEnvelope = new EnvelopeClass ();
            pEnvelope.XMax = pPoint.X + radius;
            pEnvelope.XMin = pPoint.X - radius;
            pEnvelope.YMin = pPoint.Y - radius;
            pEnvelope.YMax = pPoint.Y + radius;
            return pEnvelope;
        }

        //�õ�pPointColl�о���pPoint����ĵ�
        private IPoint GetClosedPoint ( IPointCollection pPointColl , IPoint pPoint )
        {
            if ( pPointColl.PointCount <= 0 ) return null;

            IPoint pTempPoint = pPointColl.get_Point ( 0 );
            for ( int i = 0 ; i < pPointColl.PointCount ; i++ )
            {
                if ( GetDistance ( pTempPoint , pPoint ) > GetDistance ( pPointColl.get_Point ( i ) , pPoint ) )
                    pTempPoint = pPointColl.get_Point ( i );
            }
            if ( GetDistance ( pTempPoint , pPoint ) < SnapMapRadius * SnapMapRadius )
                return pTempPoint;
            else
                return null;
        }

        /// <summary>
        /// ������(��Ļ)����ת����Ϊ��ͼ�ϵľ���
        /// </summary>
        private double ConvertPixelDistanceToMapDistance ( double PixelDistance )
        {
            tagPOINT tagP = new tagPOINT ();
            WKSPoint wksP = new WKSPoint ();

            tagP.x = ( int )PixelDistance;
            tagP.y = ( int )PixelDistance;
            ( m_CurrentMap as IActiveView ).ScreenDisplay.DisplayTransformation.TransformCoords ( ref wksP , ref tagP , 1 , 6 );
            return wksP.X;
        }

        private double ScaleChange ( double vVal )
        {
            if ( m_CurrentMap.MapScale == 0 || m_CurrentMap.ReferenceScale == 0 )
                return vVal;
            else
                return vVal * m_CurrentMap.MapScale / m_CurrentMap.ReferenceScale;
        }

        /// <summary>
        /// ������׽��
        /// </summary>
        /// <param name="pAV"></param>
        /// <param name="pPoint"></param>
        private void DrawPoint ( IPoint pPoint , ISymbol symbol )
        {
            IActiveView pAV = m_CurrentMap as IActiveView;
            pAV.ScreenDisplay.StartDrawing ( pAV.ScreenDisplay.hDC , -1 );
            pAV.ScreenDisplay.SetSymbol ( symbol );
            pAV.ScreenDisplay.DrawPoint ( pPoint );
            pAV.ScreenDisplay.FinishDrawing ();
        }

        /// <summary>
        /// �õ������Symbol
        /// </summary>
        /// <returns></returns>
        private ISimpleMarkerSymbol SetSnapSymbol ()
        {
            IRgbColor pRGB = new RgbColorClass ();
            pRGB.Blue = 0;
            pRGB.Green = 0;
            pRGB.Red = 255;
            pRGB.Transparency = 0;

            ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass ();
            ISymbol pSymbol = pMarkerSymbol as ISymbol;
            pSymbol.ROP2 = esriRasterOpCode.esriROPXOrPen;
            pMarkerSymbol.Color = pRGB;

            switch ( m_PointType )
            {
                case PointType.BoundryPoint:
                    pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCross;
                    break;
                case PointType.CenterPoint:
                    pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSDiamond;
                    break;
                case PointType.IntersectPoint:
                    pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSX;
                    break;
                case PointType.MidPoint:
                    pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSSquare;
                    break;
                case PointType.PortPoint:
                    pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                    break;
                case PointType.VertexPoint:
                    pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSX;
                    break;
                default:
                    pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSSquare;
                    break;
            }
            pRGB.Transparency =255;
            pRGB.Blue = 0;
            pRGB.Green =255;
            pRGB.Red = 0;
            pMarkerSymbol.Outline = true;
            pMarkerSymbol.OutlineColor = pRGB;

            pMarkerSymbol.OutlineSize = ScaleChange (1);
            pMarkerSymbol.Size = ScaleChange ( 6);

            return pMarkerSymbol;
        }

        private bool HitFeature ()
        {
            int segmentIndex = 0;
            double hitDist = 0;
            int part = 0;
            bool rightSide = false;

            IPointCollection pPointColl_Intersect = new PolygonClass ();

            IProximityOperator pProximityOp = SnapRefrencePoint as IProximityOperator;

            //���벶׽�ο�������ĵ㣨�������⣩
            IPoint pClosedPoint = new PointClass ();

            //��ò�׽�ο��㻺�����
            IEnvelope pEnvelope = GetPointBufferEnvelope ( SnapRefrencePoint,SnapMapRadius );

            //�ж�pClosedPoint�Ƿ��Ѿ�������ֵ
            bool isSet = false;

            //���ڼ��㽻����б�
            List<IFeature> list_AllFeatures = new List<IFeature> ();

            for ( int i = 0 ; i < m_FeatureCache.Count ; i++ )
            {
                IFeature pFeature = m_FeatureCache.get_Feature ( i );

                //�ж�IFeature������
                if ( pFeature.Shape.GeometryType != esriGeometryType.esriGeometryPoint &&
                    pFeature.Shape.GeometryType != esriGeometryType.esriGeometryPolyline &&
                    pFeature.Shape.GeometryType != esriGeometryType.esriGeometryPolygon )
                    continue;

                //�ж�IFeature�Ƿ��벶׽��Ļ������ཻ
                if ( pFeature.Shape.Envelope.XMax < pEnvelope.XMin || pFeature.Shape.Envelope.XMin > pEnvelope.XMax ||
                    pFeature.Shape.Envelope.YMax < pEnvelope.YMin || pFeature.Shape.Envelope.YMin > pEnvelope.YMax )
                    continue;

                //�ж�IFeature�Ͳ�׽�ο���ľ���,�˴��Ƿ�Ӧ�ô��ڲ�׽�뾶�������ǲ�׽�뾶��ƽ��
                if ( pProximityOp.ReturnDistance ( pFeature.Shape ) > SnapMapRadius /** SnapMapRadius */) continue;

                IHitTest pHitTest = m_FeatureCache.get_Feature ( i ).Shape as IHitTest;

                IPoint[] pResPoint = new IPoint[5];
                for ( int j = 0 ; j < 5 ; j++ )
                {
                    pResPoint[j] = new PointClass ();
                }

                #region HitTest��׽

                //��׽�˵�
                if ( IsSnapPortPoint )
                    pHitTest.HitTest ( SnapRefrencePoint , SnapMapRadius , esriGeometryHitPartType.esriGeometryPartEndpoint , pResPoint[0] , ref hitDist , ref part , ref segmentIndex , ref rightSide );
                else
                    pResPoint[0] = null;

                //��׽�е�
                if ( IsSnapMidPoint )
                    pHitTest.HitTest ( SnapRefrencePoint , SnapMapRadius , esriGeometryHitPartType.esriGeometryPartMidpoint , pResPoint[1] , ref hitDist , ref part , ref  segmentIndex , ref  rightSide );
                else
                    pResPoint[1] = null;

                //��׽�ڵ�
                if ( IsSnapNodePoint )
                    pHitTest.HitTest ( SnapRefrencePoint , SnapMapRadius , esriGeometryHitPartType.esriGeometryPartVertex , pResPoint[2] , ref  hitDist , ref part , ref segmentIndex , ref rightSide );
                else
                    pResPoint[2] = null;

                //��׽���ϵ�
                if ( IsSnapBoundryPoint )
                    pHitTest.HitTest ( SnapRefrencePoint , SnapMapRadius , esriGeometryHitPartType.esriGeometryPartBoundary , pResPoint[3] , ref  hitDist , ref part , ref segmentIndex , ref rightSide );
                else
                    pResPoint[3] = null;

                //��׽���ĵ�
                if ( IsSnapCenterPoint )
                    pHitTest.HitTest ( SnapRefrencePoint , SnapMapRadius , esriGeometryHitPartType.esriGeometryPartCentroid , pResPoint[4] , ref  hitDist , ref part , ref segmentIndex , ref rightSide );
                else
                    pResPoint[4] = null;

                #endregion

                //��׽����
                if ( IsSnapIntersectPoint )
                {
                    list_AllFeatures.Add ( pFeature );
                }

                #region ��׽�˵�
                if ( pResPoint[0] != null && pResPoint[0].IsEmpty == false )
                {
                    if ( isSet )
                    {
                        if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) > GetDistance ( SnapRefrencePoint , pResPoint[0] ) )
                        {
                            pClosedPoint = pResPoint[0];

                            //��������Χ��
                            //if ( pClosedPoint != null && pClosedPoint.IsEmpty == false )
                            //{
                            //    if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) < m_IgnoreDistance )
                            //    {
                            //        m_SnapResultPoint = pClosedPoint;
                            //        m_PointType = PointType.PortPoint;
                            //        return true;
                            //    }
                            //}
                            m_PointType = PointType.PortPoint;
                        }
                    }
                    else
                    {
                        pClosedPoint = pResPoint[0];

                        //��������Χ��
                        //if ( pClosedPoint != null && pClosedPoint.IsEmpty == false )
                        //{
                        //    if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) < m_IgnoreDistance )
                        //    {
                        //        m_SnapResultPoint = pClosedPoint;
                        //        m_PointType = PointType.PortPoint;
                        //        return true;
                        //    }
                        //}
                        isSet = true;
                        m_PointType = PointType.PortPoint;
                    }
                }
                #endregion

                #region ��׽�е�
                if ( pResPoint[1] != null && pResPoint[1].IsEmpty == false )
                {
                    if ( isSet )
                    {
                        if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) > GetDistance ( SnapRefrencePoint , pResPoint[1] ) )
                        {
                            pClosedPoint = pResPoint[1];

                            //��������Χ��                           
                            //if ( pClosedPoint != null && pClosedPoint.IsEmpty == false )
                            //{
                            //    if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) < m_IgnoreDistance )
                            //    {
                            //        m_SnapResultPoint = pClosedPoint;
                            //        m_PointType = PointType.MidPoint;
                            //        return true;
                            //    }
                            //}
                            m_PointType = PointType.MidPoint;
                        }
                    }
                    else
                    {
                        pClosedPoint = pResPoint[1];

                        //��������Χ��
                        //if ( pClosedPoint != null && pClosedPoint.IsEmpty == false )
                        //{
                        //    if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) < m_IgnoreDistance )
                        //    {
                        //        m_SnapResultPoint = pClosedPoint;
                        //        m_PointType = PointType.MidPoint;
                        //        return true;
                        //    }
                        //}
                        isSet = true;
                        m_PointType = PointType.MidPoint;
                    }
                }
                #endregion

                #region ��׽�ڵ�
                if ( pResPoint[2] != null && pResPoint[2].IsEmpty == false )
                {
                    if ( isSet )
                    {
                        if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) > GetDistance ( SnapRefrencePoint , pResPoint[2] ) )                        
                        {
                            pClosedPoint = pResPoint[2];
                            //��������Χ��
                            //if ( pClosedPoint != null && pClosedPoint.IsEmpty == false )
                            //{
                            //    if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) < m_IgnoreDistance )
                            //    {
                            //        m_SnapResultPoint = pClosedPoint;
                            //        m_PointType = PointType.VertexPoint;
                            //        return true;
                            //    }
                            //}
                            m_PointType = PointType.VertexPoint;
                        }
                    }
                    else
                    {
                        pClosedPoint = pResPoint[2];
                        //��������Χ��
                        //if ( pClosedPoint != null && pClosedPoint.IsEmpty == false )
                        //{
                        //    if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) < m_IgnoreDistance )
                        //    {
                        //        m_SnapResultPoint = pClosedPoint;
                        //        m_PointType = PointType.VertexPoint;
                        //        return true;
                        //    }
                        //}
                        isSet = true;
                        m_PointType = PointType.VertexPoint;
                    }
                }
                #endregion

                #region ��׽���ϵ�
                if ( pResPoint[3] != null && pResPoint[3].IsEmpty == false )
                {
                    if ( isSet )
                    {
                        if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) > GetDistance ( SnapRefrencePoint , pResPoint[3] ) )
                        {
                            pClosedPoint = pResPoint[3];
                            //��������Χ��
                            //if ( pClosedPoint != null && pClosedPoint.IsEmpty == false )
                            //{
                            //    if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) < m_IgnoreDistance )
                            //    {
                            //        m_SnapResultPoint = pClosedPoint;
                            //        m_PointType = PointType.BoundryPoint;
                            //        return true;
                            //    }
                            //}
                            m_PointType = PointType.BoundryPoint;
                        }
                    }
                    else
                    {
                        pClosedPoint = pResPoint[3];
                        //��������Χ��
                        //if ( pClosedPoint != null && pClosedPoint.IsEmpty == false )
                        //{
                        //    if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) < m_IgnoreDistance )
                        //    {
                        //        m_SnapResultPoint = pClosedPoint;
                        //        m_PointType = PointType.BoundryPoint;
                        //        return true;
                        //    }
                        //}
                        isSet = true;
                        m_PointType = PointType.BoundryPoint;
                    }
                }
                #endregion

                #region ��׽���ĵ�
                if ( pResPoint[4] != null && pResPoint[4].IsEmpty == false )
                {
                    if ( isSet )
                    {
                        if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) > GetDistance ( SnapRefrencePoint , pResPoint[4] ) )
                        {
                            pClosedPoint = pResPoint[4];
                            //��������Χ��
                            //if ( pClosedPoint != null && pClosedPoint.IsEmpty == false )
                            //{
                            //    if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) < m_IgnoreDistance )
                            //    {
                            //        m_SnapResultPoint = pClosedPoint;
                            //        m_PointType = PointType.CenterPoint;
                            //        return true;
                            //    }
                            //}
                            m_PointType = PointType.CenterPoint;
                        }
                    }
                    else
                    {
                        pClosedPoint = pResPoint[4];
                        //��������Χ��
                        //if ( pClosedPoint != null && pClosedPoint.IsEmpty == false )
                        //{
                        //    if ( GetDistance ( pClosedPoint , SnapRefrencePoint ) < m_IgnoreDistance )
                        //    {
                        //        m_SnapResultPoint = pClosedPoint;
                        //        m_PointType = PointType.CenterPoint;
                        //        return true;
                        //    }
                        //}
                        isSet = true;
                        m_PointType = PointType.CenterPoint;
                    }
                }
                #endregion

            }

            //���㽻�㼯��
            if ( IsSnapIntersectPoint )
            {
                while ( list_AllFeatures.Count > 0 )
                {
                    pPointColl_Intersect.AddPointCollection ( GetAllIntersect ( list_AllFeatures[0] , list_AllFeatures ) );
                    list_AllFeatures.RemoveAt ( 0 );
                }
            }

            //�õ�����Ľ���
            IPoint pIntersectPoint = GetClosedPoint ( pPointColl_Intersect , SnapRefrencePoint );

            //�Ƚ�����Ľ�����������͵�������㵽�ο���ľ���
            if ( pIntersectPoint != null && pIntersectPoint.IsEmpty == false )
            {
                if ( pClosedPoint != null && pClosedPoint.IsEmpty == false )
                {
                    if ( GetDistance ( pIntersectPoint , SnapRefrencePoint ) < GetDistance ( pClosedPoint , SnapRefrencePoint ) )
                    {
                        m_SnapResultPoint = pIntersectPoint;
                        //b_IsHasSnaped = true;
                        m_PointType = PointType.IntersectPoint;
                    }
                    else
                    {
                        m_SnapResultPoint = pClosedPoint;
                        //b_IsHasSnaped = true;
                    }
                }
                else
                {
                    m_SnapResultPoint = null;
                    //b_IsHasSnaped = false;
                }
            }
            else if ( pClosedPoint != null && pClosedPoint.IsEmpty == false )
            {
                m_SnapResultPoint = pClosedPoint;
                //b_IsHasSnaped = true;
            }
            else
            {
                m_SnapResultPoint = null;
                //b_IsHasSnaped = false;
            }

            return true;
        }

        //�������������ƽ�� 
        private double GetDistance ( IPoint point1 , IPoint point2 )
        {
            return ( ( point1.X - point2.X ) * ( point1.X - point2.X ) + 
                ( point1.Y - point2.Y ) * ( point1.Y - point2.Y ) );
        }

        #region ��ý�����㷨 GetIntersectPointCollection
        /// <summary>
        /// �õ�Feature�� FeatureList���еĽ���
        /// </summary>
        /// <param name="vNewFeature"></param>
        /// <param name="vFeatureCol"></param>
        /// <returns></returns>
        private IPointCollection GetAllIntersect ( IFeature OneOfFeature , List<IFeature> list_AllFeatures )
        {
            IPolyline tempLine = new PolylineClass ();
            IPointCollection pPointColl = tempLine as IPointCollection;
            pPointColl.AddPointCollection ( OneOfFeature.Shape as IPointCollection );

            IPointCollection vItersectCol = new MultipointClass ();


            IMultipoint vIntersectPnt = new MultipointClass ();

            IFeature vFeature;
            for ( int i = 0 ; i < list_AllFeatures.Count ; i++ )
            {
                vFeature = list_AllFeatures[i];
                if ( vFeature != OneOfFeature )
                {
                    vIntersectPnt = GetIntersection ( vFeature.Shape , tempLine as IPolyline ) as IMultipoint;
                    if ( vIntersectPnt != null )
                        vItersectCol.AddPointCollection ( vIntersectPnt as IPointCollection );
                }
            }

            return vItersectCol;

        }

        /// <summary>
        /// �õ�Geometry��Polyline�Ľ���
            /// </summary>
        /// <param name="vIntersect"></param>
        /// <param name="vOther"></param>
        /// <returns></returns>
        private IGeometry GetIntersection ( IGeometry vIntersect , IPolyline vOther )
        {
            //�ж��������Ƿ��н���
            IEnvelope pEnvIntersect = vIntersect.Envelope;
            IEnvelope pEnvOther = vOther.Envelope;

            if ( pEnvIntersect.XMax < pEnvOther.XMin || pEnvIntersect.XMin > pEnvOther.XMax ||
                pEnvIntersect.YMax < pEnvOther.YMin || pEnvIntersect.YMin > pEnvOther.YMax )
                return null;

            if ( vIntersect.SpatialReference != null && !vIntersect.SpatialReference.Equals ( vOther.SpatialReference ) )
            {
                vOther.Project ( vIntersect.SpatialReference );
            }

            ITopologicalOperator vTopoOp = vIntersect as ITopologicalOperator;
            vTopoOp.Simplify ();
            IGeometry vGeomResult = vTopoOp.Intersect ( vOther , esriGeometryDimension.esriGeometry0Dimension );
            if ( vGeomResult == null ) return null;

            if ( vGeomResult is IPointCollection )
            {
                if ( !( ( vGeomResult as IPointCollection ).PointCount >= 1 ) )
                    return null;
            }

            if ( !( vGeomResult.GeometryType == esriGeometryType.esriGeometryMultipoint ) )
            {
                return null;
            }

            return vGeomResult;
        }

        #endregion

    }
}
