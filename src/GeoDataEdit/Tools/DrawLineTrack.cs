using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SystemUI;

namespace GeoDataEdit
{
    internal class DrawLineTrack
    {

        private DrawTypeConstant m_nDrawType;                                         //��ǰ��������
        //1Ϊ��ͨ�㣬2��ͨ�ߣ�3Ϊ��ͨ�棬4Ϊƽ���ߣ�5��ӽڵ㣬6ɾ���ڵ㣬7�ƶ��ڵ㣬8ɾ��Ҫ��,9�ƶ�Ҫ��,
        //10����֪�ߵ�ƽ����,11�ı��ߵķ���,12����������,13��������,14Ҫ��ת��,15Ҫ�������޸�,16�ϲ���,
        //17�ϲ���,18�ָ���,19�ָ���,20��ע��,21�޸�ע��,22ѡ��
        //23��Բ�ǣ�24��ֱ�ǣ�25�����ߣ�26�޼��ߣ�27����28ͶӰ,29�ֽ���,30�����
        public IPointCollection m_pDrawedPoints;     //�Ѿ����ĵ㼯
        public IGeometryCollection m_pGeometryBag;   //���·�����߻��İ�

        private IMap m_pCurMap;                          //��ǰ��ͼ
        private IFeatureLayer m_pCurFeatureLayer;        //��ǰ�༭��
        private esriGeometryType m_nShapeType;        //��ǰͼ�㼸������

        private IOperationStack m_pOperationStack;   //����ջ
        private IPoint m_pLastMouseStopedPoint;        //����ϴ�ͣ����λ��

        private ClsEditorMain clsEditorMain;

        public DrawLineTrack ()
        {
            this.m_pDrawedPoints = null;
            this.m_pGeometryBag = null;
            this.m_pLastMouseStopedPoint = new PointClass ();
        }

        ~DrawLineTrack ()
        {
            if ( this.m_pDrawedPoints != null )
                this.m_pDrawedPoints = null;

            if ( this.m_pGeometryBag != null )
                this.m_pGeometryBag = null;

            this.m_pLastMouseStopedPoint = null;
        }

        public void ReSet ()
        {
            this.m_pDrawedPoints = null;
            this.m_pGeometryBag = null;

            if ( this.m_pLastMouseStopedPoint == null )
                this.m_pLastMouseStopedPoint = new PointClass ();
            else
                this.m_pLastMouseStopedPoint.SetEmpty ();

        }

        public DrawTypeConstant GetCurDrawType ()
        {
            return this.m_nDrawType;
        }

        public Boolean InitTrack ( DrawTypeConstant nDrawType , ref IMap pCurMap , ClsEditorMain _clsEditorMain )
        {

            if ( _clsEditorMain == null ) return false;
            clsEditorMain = _clsEditorMain;

            m_pCurFeatureLayer = clsEditorMain.EditFeatureLayer;
            //if ( m_pCurFeatureLayer == null ) return false;
            //if ( m_pCurFeatureLayer.FeatureClass == null ) return false;

            //m_pOperationStack = clsEditorMain.OperationStack;
            //if ( m_pOperationStack == null ) return false;

            this.m_pCurMap = pCurMap;
            if ( m_pCurMap == null ) return false;
            if ( m_pCurMap.LayerCount < 1 ) return false;


            this.m_nDrawType = nDrawType;
            this.m_nShapeType = esriGeometryType.esriGeometryPolygon;

            return true;
        }

        //��鵱ǰͼ���Ƿ��ʺϵ�ǰ�ı༭����
        public bool CheckCanDraw ()
        {
            if ( m_pCurMap == null )
            {
                System.Windows.Forms.MessageBox.Show ( "���ȼ��ص�ͼ��" , "ϵͳ��ʾ" );
                return false;
            }

            if ( m_pCurMap.LayerCount < 1 )
            {
                System.Windows.Forms.MessageBox.Show ( "���ȼ���ͼ�㣡" , "ϵͳ��ʾ" );
                return false;
            }

            if ( clsEditorMain.EditWorkspace == null )
            {
                System.Windows.Forms.MessageBox.Show ( "�༭���̲���Ϊ�գ�" , "ϵͳ��ʾ" );
                return false;
            }


            if ( !( m_pCurFeatureLayer.FeatureClass as IWorkspaceEdit ).IsBeingEdited () )
            {
                System.Windows.Forms.MessageBox.Show ( "���������༭��" , "ϵͳ��ʾ" );
                return false;
            }

            if ( clsEditorMain.EditFeatureLayer == null )
            {
                System.Windows.Forms.MessageBox.Show ( "û�����õ�ǰ�༭�Ĳ㣡" , "ϵͳ��ʾ" );
                return false;
            }

            if ( clsEditorMain.EditFeatureLayer.Visible == false )
            {
                System.Windows.Forms.MessageBox.Show ( "��ǰ�㲻�ɼ������ܱ༭��" , "ϵͳ��ʾ" );
                return false;
            }

            if ( !( clsEditorMain.EditFeatureLayer is IGeoFeatureLayer ) )
            {
                System.Windows.Forms.MessageBox.Show ( "��ǰ�㲻�ܽ���Ҫ�ر༭��" , "ϵͳ��ʾ" );
                return false;
            }



            IFeatureClass pFeatureClass = m_pCurFeatureLayer.FeatureClass;

            if ( pFeatureClass == null )
            {
                System.Windows.Forms.MessageBox.Show ( "��ǰ��û�й�����Ҫ���࣡" , "ϵͳ��ʾ" );
                return false;
            }

            esriGeometryType nGeometryType = pFeatureClass.ShapeType;

            switch ( m_nDrawType )
            {
                case DrawTypeConstant.CommonPoint:
                    if ( nGeometryType != esriGeometryType.esriGeometryPoint )
                    {
                        System.Windows.Forms.MessageBox.Show ( "��ǰ�༭�Ĳ㲻�ǵ�Ҫ�ز㣡" , "ϵͳ��ʾ" );
                        return false;
                    }
                    break;

                case DrawTypeConstant.CommonLine:
                case DrawTypeConstant.ParallelLine:
                case DrawTypeConstant.AnchorParallelLine:
                case DrawTypeConstant.ChangeLineDirection:
                case DrawTypeConstant.VerticalLineFromPoint:
                case DrawTypeConstant.UnionLine:
                case DrawTypeConstant.SplitLine:
                case DrawTypeConstant.InRoundAngle:
                    if ( nGeometryType != esriGeometryType.esriGeometryPolyline )
                    {
                        System.Windows.Forms.MessageBox.Show ( "��ǰ�༭�Ĳ㲻����Ҫ�ز㣡" , "ϵͳ��ʾ" );
                        return false;
                    }
                    break;

                case DrawTypeConstant.CommonPolygon:
                case DrawTypeConstant.UnionPolygon:
                case DrawTypeConstant.SplitPolygon:
                    if ( nGeometryType != esriGeometryType.esriGeometryPolygon )
                    {
                        System.Windows.Forms.MessageBox.Show ( "��ǰ�༭�Ĳ㲻����Ҫ�ز㣡" , "ϵͳ��ʾ" );
                        return false;
                    }
                    break;

                case DrawTypeConstant.AddVertex:
                case DrawTypeConstant.DeleteVertex:
                case DrawTypeConstant.ScaleZoom:
                    if ( nGeometryType != esriGeometryType.esriGeometryPolyline && nGeometryType != esriGeometryType.esriGeometryPolygon )
                    {
                        System.Windows.Forms.MessageBox.Show ( "��ǰ�༭�Ĳ�ֻ������Ҫ�ز����Ҫ�ز㣡" , "ϵͳ��ʾ" );
                        return false;
                    }
                    break;

                default:
                    break;
            }
            return true;

        }

        public bool InitTrack ( DrawTypeConstant nDrawType , IMap pCurMap , ClsEditorMain _clsEditorMain )
        {

            if ( _clsEditorMain == null ) return false;
            clsEditorMain = _clsEditorMain;

            m_pOperationStack = clsEditorMain.OperationStack;
            if ( m_pOperationStack == null ) return false;

            this.m_pCurMap = pCurMap;
            if ( m_pCurMap == null ) return false;

            this.m_nDrawType = nDrawType;
            this.m_nShapeType = esriGeometryType.esriGeometryPolyline;

            //this.m_pDrawedPoints = null;

            return true;
        }

        //�õ��Ի���������������ɵ�·�����ϵĵ㣩
        public int GetPointCount ()
        {
            if ( this.m_pDrawedPoints == null )
                return 0;
            else
                return this.m_pDrawedPoints.PointCount;

        }

        //����·����
        public int GetPartCount ()
        {
            if ( this.m_pGeometryBag == null )
                return 0;
            else
                return this.m_pGeometryBag.GeometryCount;

        }


        public IPoint AddPoint ( double x , double y , bool blOnMap )
        {
            IActiveView pActiveView = this.m_pCurMap as IActiveView;

            IPoint pClickPoint;

            object pBObj = Type.Missing;
            object pAObj = Type.Missing;

            if ( blOnMap == true )
            {
                pClickPoint = new PointClass ();
                pClickPoint.X = x;
                pClickPoint.Y = y;
            }
            else
            {
                IPoint pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint ( ( int )x , ( int )y );
                this.clsEditorMain.SnapPoint.SnapExcute ( pPoint );
                pClickPoint = this.clsEditorMain.SnapPoint.SnapResultPoint;
            }


            if ( this.m_nDrawType == DrawTypeConstant.AddVertex ) return pClickPoint;
            if ( this.m_nShapeType == esriGeometryType.esriGeometryPoint ) return pClickPoint;

            int nPointCount;
            if ( this.m_pDrawedPoints == null )
                nPointCount = 0;
            else
                nPointCount = this.m_pDrawedPoints.PointCount;

            if ( nPointCount == 0 )
                this.m_pDrawedPoints = new PolylineClass ();

            this.m_pDrawedPoints.AddPoint ( pClickPoint , ref pBObj , ref pAObj );

            DrawWhileDown ( ref pActiveView );

            return pClickPoint;
        }

        //
        private void DrawWhileDown ( ref IActiveView pActiveView )
        {
            int nPointCount;

            if ( this.m_pDrawedPoints == null )
                nPointCount = -1;
            else
                nPointCount = this.m_pDrawedPoints.PointCount - 1;

            if ( nPointCount < 0 ) return;

            ISymbol pFeedbackSymbol = MakeFeedbackSymbol ( ref this.m_pCurMap ) as ISymbol;
            ILineSymbol pLineSymbol = pFeedbackSymbol as ILineSymbol;
            ISymbol pPFeedbackSymbol = MakePolygonFeedbackSymbol ( ref this.m_pCurMap , ref  pLineSymbol ) as ISymbol;
            ISymbol pFeatureSymbol = MakeFeatureSymbol ( ref this.m_pCurMap ) as ISymbol;
            ISimpleMarkerSymbol pPTSymbol = MakeMarkerSymbol ( ref this.m_pCurMap , 0 , 0 , 128 , false ) as ISimpleMarkerSymbol;
            ISimpleMarkerSymbol pTSSymbol = MakeMarkerSymbol ( ref this.m_pCurMap , 255 , 0 , 0 , false ) as ISimpleMarkerSymbol;

            IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;

            if ( ( this.m_nDrawType == DrawTypeConstant.CommonLine ) || ( this.m_nDrawType == DrawTypeConstant.CommonPolygon ) )
            {
                pScreenDisplay.StartDrawing ( pScreenDisplay.hDC , -1 /*esriScreenCache.esriNoScreenCache*/);

                IPoint pLastPoint;
                pLastPoint = this.m_pDrawedPoints.get_Point ( nPointCount );

                ISymbol pSymbol = pTSSymbol as ISymbol;
                //���µ�β�ڵ�
                pScreenDisplay.SetSymbol ( pSymbol );
                pScreenDisplay.DrawPoint ( pLastPoint );

                if ( nPointCount >= 1 )
                {
                    IPoint pLLastPoint;
                    pLLastPoint = this.m_pDrawedPoints.get_Point ( nPointCount - 1 );
                    //�ϴε�β�ڵ��Ϊ��ͨ�ڵ�
                    pSymbol = pPTSymbol as ISymbol;
                    pScreenDisplay.SetSymbol ( pSymbol );
                    pScreenDisplay.DrawPoint ( pLLastPoint );

                    IPolyline pPolyline;
                    pPolyline = MakePolylineFromTowPoint ( pLLastPoint , pLastPoint );
                    //����µ�Ҫ����
                    pSymbol = pFeatureSymbol as ISymbol;
                    pScreenDisplay.SetSymbol ( pSymbol );
                    pScreenDisplay.DrawPolyline ( pPolyline );

                    if ( m_pLastMouseStopedPoint.IsEmpty == false )
                    {
                        pPolyline = MakePolylineFromTowPoint ( pLLastPoint , m_pLastMouseStopedPoint );
                        //�����ϴεķ�����
                        pSymbol = pFeedbackSymbol as ISymbol;
                        pScreenDisplay.SetSymbol ( pSymbol );
                        pScreenDisplay.DrawPolyline ( pPolyline );
                        //���µķ�����
                        pPolyline = MakePolylineFromTowPoint ( pLastPoint , m_pLastMouseStopedPoint );
                        pScreenDisplay.DrawPolyline ( pPolyline );
                    }
                }
                pScreenDisplay.FinishDrawing ();
            }

        }

        //
        public string GetLastPointXYStr ()
        {
            int nPointCount;
            double fX = 0;
            double fY;
            string sX;
            string sY;

            if ( this.m_pDrawedPoints != null )
            {
                nPointCount = this.m_pDrawedPoints.PointCount;

                if ( nPointCount > 0 )
                {
                    try
                    {
                        IPointCollection pPointColl = this.m_pDrawedPoints as IPointCollection;
                        IGeometry pGeometry = m_pDrawedPoints as IGeometry;
                        fX = pPointColl.get_Point ( nPointCount - 1 ).X;
                    }
                    catch ( Exception ex )
                    { }

                    sX = fX.ToString ( ".###" );

                    fY = this.m_pDrawedPoints.get_Point ( nPointCount - 1 ).Y;
                    sY = fY.ToString ( ".###" );

                    return sX + "," + sY;

                }
            }

            if ( this.m_pGeometryBag != null )
            {
                int nGeometryCount;
                nGeometryCount = this.m_pGeometryBag.GeometryCount;

                if ( nGeometryCount > 0 )
                {
                    IPointCollection pPointCollection = this.m_pGeometryBag.get_Geometry ( nGeometryCount - 1 ) as IPointCollection;

                    nPointCount = pPointCollection.PointCount;

                    if ( this.m_nDrawType == DrawTypeConstant.CommonPolygon )
                        nPointCount = nPointCount - 2;
                    else
                        nPointCount = nPointCount - 1;

                    fX = pPointCollection.get_Point ( nPointCount ).X;
                    sX = fX.ToString ( ".###" );

                    fY = pPointCollection.get_Point ( nPointCount ).Y;
                    sY = fY.ToString ( ".###" );

                    return sX + "," + sY;
                }
            }
            return "";
        }

        //�õ��ѻ���״
        public IGeometry GetDrawGeometry ( double fLen )
        {

            int nPointCount;
            nPointCount = GetPointCount ();

            int nPart;
            nPart = GetPartCount ();

            if ( ( nPointCount == 0 ) && ( nPart == 0 ) ) return null;

            nPointCount = nPointCount - 1;

            IPointCollection pPointCollection;

            IPoint pPoint;

            int i;

            object pBObj = Type.Missing;
            object pAObj = Type.Missing;

            if ( this.m_nDrawType == DrawTypeConstant.CommonLine )
            {
                if ( nPart < 1 )
                    this.m_pGeometryBag = new PolylineClass ();


                if ( nPointCount >= 1 )
                {
                    IPath pPath = new PathClass ();

                    pPointCollection = pPath as IPointCollection;

                    for ( i = 0 ; i <= nPointCount ; i++ )
                    {
                        pPoint = this.m_pDrawedPoints.get_Point ( i );
                        pPointCollection.AddPoint ( pPoint , ref pBObj , ref pAObj );
                    }

                    this.m_pGeometryBag.AddGeometry ( pPath , ref pBObj , ref pAObj );
                    pPath = null;
                }
                return this.m_pGeometryBag as IGeometry;
            }

            if ( this.m_nDrawType == DrawTypeConstant.CommonPolygon )
            {
                if ( nPart < 1 )
                    this.m_pGeometryBag = new PolygonClass ();

                if ( nPointCount >= 2 )
                {
                    IRing pRing = new RingClass ();

                    pPointCollection = pRing as IPointCollection;

                    for ( i = 0 ; i <= nPointCount ; i++ )
                    {
                        pPoint = this.m_pDrawedPoints.get_Point ( i );
                        pPointCollection.AddPoint ( pPoint , ref pBObj , ref pAObj );
                    }

                    pPoint = this.m_pDrawedPoints.get_Point ( 0 );
                    pPointCollection.AddPoint ( pPoint , ref pBObj , ref pAObj );

                    this.m_pGeometryBag.AddGeometry ( pRing , ref pBObj , ref pAObj );
                    pRing = null;
                }
                return this.m_pGeometryBag as IGeometry;
            }

            if ( this.m_pGeometryBag != null )
                this.m_pGeometryBag = null;


            if ( ( this.m_nDrawType == DrawTypeConstant.ParallelLine ) && ( nPointCount >= 1 ) )
            {
                IPolyline pPolyline = GetParallelPolyline2 ( this.m_pDrawedPoints , null , fLen , this.m_pCurMap );

                this.m_pGeometryBag = new PolylineClass ();

                IGeometryCollection pGeometryCollection = pPolyline as IGeometryCollection;

                if ( pGeometryCollection == null )
                    nPart = -1;
                else
                    nPart = pGeometryCollection.GeometryCount - 1;

                IGeometry pGeometry;

                for ( i = 0 ; i <= nPart ; i++ )
                {

                    pGeometry = pGeometryCollection.get_Geometry ( i );
                    this.m_pGeometryBag.AddGeometry ( pGeometry , ref pBObj , ref pAObj );
                }

                pGeometryCollection = this.m_pDrawedPoints as IGeometryCollection;

                if ( pGeometryCollection == null )
                    nPart = -1;
                else
                    nPart = pGeometryCollection.GeometryCount - 1;

                for ( i = 0 ; i <= nPart ; i++ )
                {
                    pGeometry = pGeometryCollection.get_Geometry ( i );
                    this.m_pGeometryBag.AddGeometry ( pGeometry , ref pBObj , ref pAObj );
                }

                return m_pGeometryBag as IGeometry;

            }

            this.m_pDrawedPoints = null;

            return null;
        }

        //
        public IPolyline MakePolylineFromTowPoint ( IPoint sTartPoint , IPoint pEndPoint )
        {
            ILine pLine;
            pLine = new LineClass ();
            pLine.FromPoint = sTartPoint;
            pLine.ToPoint = pEndPoint;

            object pBObj = Type.Missing;
            object pAObj = Type.Missing;

            IPolyline pPolyline = new PolylineClass ();

            ISegmentCollection pSegmentCollection = pPolyline as ISegmentCollection;
            pSegmentCollection.AddSegment ( pLine as ISegment , ref  pBObj , ref pAObj );

            pLine = null;

            return pPolyline;
        }


        public bool MakeClose ()
        {
            if ( this.m_pDrawedPoints == null ) return false;

            int nPointCount = this.m_pDrawedPoints.PointCount - 1;
            if ( nPointCount < 0 ) return false;

            IPoint pPoint;
            object pBObj = Type.Missing;
            object pAObj = Type.Missing;

            pPoint = this.m_pDrawedPoints.get_Point ( 0 );
            this.m_pDrawedPoints.AddPoint ( pPoint , ref pBObj , ref pAObj );
            return true;
        }

        //����do��undo
        public void SetDo ( bool blDo )
        {
            clsEditorMain.BDo = blDo;
            //this.m_pSnapPointClass.blDo = blDo;
        }
        //�ж�do��undo
        public bool GetDo ()
        {
            //return this.m_pSnapPointClass.blDo;
            return clsEditorMain.BDo;
        }

        //�õ�����ջ
        public IOperationStack GetOperationStack ()
        {
            return this.m_pOperationStack;
        }

        //����
        public void GiveUpDraw ()
        {
            if ( this.m_pDrawedPoints != null )
                this.m_pDrawedPoints = null;

            this.m_pLastMouseStopedPoint.SetEmpty ();

            if ( this.m_pGeometryBag != null )
                this.m_pGeometryBag = null;

            ESRI.ArcGIS.Carto.IActiveView pActiveView;
            pActiveView = this.m_pCurMap as ESRI.ArcGIS.Carto.IActiveView;
            pActiveView.Refresh ();
        }

        //���ȸ��ģ���������ʱ��
        public bool LengthChange ( double fNewLength )
        {
            int nPointCount = this.m_pDrawedPoints.PointCount - 1;

            IPoint pfPoint = this.m_pDrawedPoints.get_Point ( nPointCount - 1 );

            IPoint pOldTPoint = this.m_pDrawedPoints.get_Point ( nPointCount );

            ICurve pCurve = new LineClass () as ICurve;
            pCurve.FromPoint = pfPoint;
            pCurve.ToPoint = pOldTPoint;

            IPoint pNewTPoint = new PointClass ();

            IConstructPoint pConstructPoint = pNewTPoint as IConstructPoint;
            pConstructPoint.ConstructAlong ( pCurve , esriSegmentExtension.esriExtendTangentAtTo , fNewLength , false );

            this.m_pDrawedPoints.RemovePoints ( nPointCount , 1 );

            object pBObj = Type.Missing;
            object pAObj = Type.Missing;

            this.m_pDrawedPoints.AddPoint ( pNewTPoint , ref pBObj , ref pAObj );

            ( m_pCurMap as IActiveView ).Refresh ();

            pCurve = null;
            pNewTPoint = null;
            return true;
        }

        //��·���򻷿�ʼ
        public bool NewPartBegin ()
        {
            if ( this.m_pDrawedPoints == null ) return false;

            int nPointCount = this.m_pDrawedPoints.PointCount - 1;
            if ( nPointCount < 0 ) return false;

            IPoint pPoint;
            object pBObj = Type.Missing;
            object pAObj = Type.Missing;
            IPointCollection pPointCollection;

            //��·��
            if ( m_nDrawType == DrawTypeConstant.CommonLine )
            {
                if ( this.m_pGeometryBag == null )
                    this.m_pGeometryBag = new PolylineClass ();


                IPath pPath = new PathClass ();

                pPointCollection = pPath as IPointCollection;


                for ( int i = 0 ; i <= nPointCount ; i++ )
                {
                    pPoint = this.m_pDrawedPoints.get_Point ( i );
                    pPointCollection.AddPoint ( pPoint , ref pBObj , ref pAObj );
                }

                this.m_pGeometryBag.AddGeometry ( pPath , ref pBObj , ref pAObj );
                pPath = null;
            }
            //�໷
            if ( this.m_nDrawType == DrawTypeConstant.CommonPolygon )
            {
                if ( this.m_pGeometryBag == null )
                    this.m_pGeometryBag = new PolygonClass ();

                IRing pRing = new RingClass ();

                pPointCollection = pRing as IPointCollection;

                for ( int i = 0 ; i <= nPointCount ; i++ )
                {
                    pPoint = this.m_pDrawedPoints.get_Point ( i );
                    pPointCollection.AddPoint ( pPoint , ref pBObj , ref pAObj );
                }
                pPoint = this.m_pDrawedPoints.get_Point ( 0 );
                pPointCollection.AddPoint ( pPoint , ref pBObj , ref pAObj );

                this.m_pGeometryBag.AddGeometry ( pRing , ref pBObj , ref pAObj );
                pRing = null;
            }

            this.m_pDrawedPoints = null;
            this.m_pLastMouseStopedPoint.SetEmpty ();

            ( m_pCurMap as IActiveView ).Refresh ();

            return true;
        }


        //�����
        public bool GoBackaPoint ()
        {
            int nPointCount;
            if ( this.m_pDrawedPoints == null )
                nPointCount = -1;
            else
                nPointCount = this.m_pDrawedPoints.PointCount - 1;


            bool blCanBack = false;

            int nGeometryCount;
            IPoint pPoint;

            object pBObj = Type.Missing;
            object pAObj = Type.Missing;

            IPointCollection pPoints;
            int nCount;


            if ( nPointCount >= 0 )
            {
                this.m_pDrawedPoints.RemovePoints ( nPointCount , 1 );
                blCanBack = true;

                if ( nPointCount == 1 )
                    this.m_pLastMouseStopedPoint.SetEmpty ();  //���ϴ�ͣ��λ�����

            }
            else
            {
                if ( this.m_nDrawType == DrawTypeConstant.CommonLine )
                {
                    if ( this.m_pGeometryBag == null )
                        nGeometryCount = -1;
                    else
                        nGeometryCount = this.m_pGeometryBag.GeometryCount - 1;

                    if ( nGeometryCount >= 0 )
                    {
                        if ( m_pDrawedPoints != null )
                            m_pDrawedPoints = null;

                        m_pDrawedPoints = new PolylineClass ();

                        IPath pPath = this.m_pGeometryBag.get_Geometry ( nGeometryCount ) as IPath;

                        pPoints = pPath as IPointCollection;

                        nCount = pPoints.PointCount - 1;

                        nCount = nCount - 1;
                        for ( int i = 0 ; i <= nCount ; i++ )
                        {
                            pPoint = pPoints.get_Point ( i );
                            this.m_pDrawedPoints.AddPoint ( pPoint , ref pBObj , ref pAObj );
                        }

                        this.m_pGeometryBag.RemoveGeometries ( nGeometryCount , 1 );
                        blCanBack = true;
                    }
                }

                if ( this.m_nDrawType == DrawTypeConstant.CommonPolygon )
                {
                    if ( this.m_pGeometryBag == null )
                        nGeometryCount = -1;
                    else
                        nGeometryCount = this.m_pGeometryBag.GeometryCount - 1;

                    if ( nGeometryCount >= 0 )
                    {
                        if ( this.m_pDrawedPoints != null )
                            this.m_pDrawedPoints = null;

                        this.m_pDrawedPoints = new PolylineClass ();

                        IRing pRing = this.m_pGeometryBag.get_Geometry ( nGeometryCount ) as IRing;

                        pPoints = pRing as IPointCollection;

                        nCount = pPoints.PointCount - 1;
                        nCount = nCount - 2;

                        for ( int i = 0 ; i <= nCount ; i++ )
                        {
                            pPoint = pPoints.get_Point ( i );
                            this.m_pDrawedPoints.AddPoint ( pPoint , ref pBObj , ref pAObj );
                        }

                        this.m_pGeometryBag.RemoveGeometries ( nGeometryCount , 1 );
                        blCanBack = true;
                    }
                }
            }

            //ʹ��ǰ��׽��Ч

            ( m_pCurMap as IActiveView ).Refresh ();

            return blCanBack;
        }


        //�ػ�
        public bool ReDraw ( bool blDrawTrack )
        {
            IActiveView pActiveView = this.m_pCurMap as IActiveView;


            this.m_pLastMouseStopedPoint.SetEmpty (); ;

            IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
            pScreenDisplay.StartDrawing ( pScreenDisplay.hDC , -1 /*esriScreenCache.esriNoScreenCache*/);

            IPolyline pPolyline;

            if ( ( this.m_nDrawType == DrawTypeConstant.CommonLine ) || ( this.m_nDrawType == DrawTypeConstant.CommonPolygon ) )
            {
                //������ɵ�·����
                if ( this.m_pGeometryBag != null )
                {
                    int nGeometryCount;
                    nGeometryCount = this.m_pGeometryBag.GeometryCount - 1;
                    if ( nGeometryCount >= 0 )
                    {
                        if ( this.m_nShapeType == esriGeometryType.esriGeometryPolygon )
                        {
                            for ( int i = 0 ; i <= nGeometryCount ; i++ )
                            {
                                IRing pRing = this.m_pGeometryBag.get_Geometry ( i ) as IRing;

                                pPolyline = MakePolyLine ( pRing );
                                DrawPoints ( ref pScreenDisplay , ref pPolyline , ref this.m_pCurMap );
                            }
                        }
                        else
                        {
                            pPolyline = this.m_pGeometryBag as IPolyline;
                            DrawPoints ( ref pScreenDisplay , ref pPolyline , ref this.m_pCurMap );
                        }
                    }
                }
                //��δ��ɵ�·����
                if ( m_pDrawedPoints != null )
                {
                    int nPointCount;
                    nPointCount = m_pDrawedPoints.PointCount - 1;

                    if ( nPointCount >= 0 )
                    {
                        pPolyline = m_pDrawedPoints as IPolyline;
                        DrawPoints ( ref pScreenDisplay , ref pPolyline , ref this.m_pCurMap );

                        IPoint pPoint = this.m_pDrawedPoints.get_Point ( nPointCount );

                        if ( ( blDrawTrack == true ) && ( this.m_pLastMouseStopedPoint.IsEmpty == false ) )
                            DrawFeedback ( ref pScreenDisplay , pPoint , m_pLastMouseStopedPoint , ref this.m_pCurMap );
                        else
                        {
                            if ( this.m_pLastMouseStopedPoint.IsEmpty == false )
                                this.m_pLastMouseStopedPoint.SetEmpty ();
                        }
                    }
                }
            }

            pScreenDisplay.FinishDrawing ();
            return true;
        }

        //�ɻ����ɶ����
        private IPolyline MakePolyLine ( IRing pRing )
        {
            IPolyline pPolyline = new PolylineClass ();

            IPointCollection pPointCollection = pPolyline as IPointCollection;

            IPointCollection pPoints = pRing as IPointCollection;

            int nCount = pPoints.PointCount - 1;

            object obj = Type.Missing;

            for ( int i = 0 ; i <= nCount ; i++ )
                pPointCollection.AddPoint ( pPoints.get_Point ( i ) , ref obj , ref obj );

            return pPolyline;
        }

        //���ڵ�
        private void DrawPoints ( ref IScreenDisplay pScreenDisplay , ref IPolyline pPolyline , ref IMap pCurMap )
        {

            ISymbol pFeatureSymbol = MakeFeatureSymbol ( ref pCurMap ) as ISymbol;
            ISimpleMarkerSymbol pPTSymbol = MakeMarkerSymbol ( ref pCurMap , 0 , 0 , 128 , false ) as ISimpleMarkerSymbol;
            ISimpleMarkerSymbol pTSSymbol = MakeMarkerSymbol ( ref pCurMap , 255 , 0 , 0 , false ) as ISimpleMarkerSymbol;

            ISymbol pSymbol = pFeatureSymbol;

            IPointCollection pPointCollection = pPolyline as IPointCollection;

            pScreenDisplay.SetSymbol ( pSymbol );
            pScreenDisplay.DrawPolyline ( pPointCollection as IPolyline );

            int nPointCount;
            if ( pPointCollection == null )
                nPointCount = -1;
            else
                nPointCount = pPointCollection.PointCount - 1;

            if ( nPointCount < 0 ) return;

            for ( int i = 0 ; i <= nPointCount ; i++ )
            {
                IPoint pPoint = pPointCollection.get_Point ( i );

                if ( i == 0 )
                {
                    pScreenDisplay.SetSymbol ( pPTSymbol as ESRI.ArcGIS.Display.ISymbol );
                    pScreenDisplay.DrawPoint ( pPoint );
                }
                else if ( i == nPointCount )
                {
                    pScreenDisplay.SetSymbol ( pTSSymbol as ESRI.ArcGIS.Display.ISymbol );
                    pScreenDisplay.DrawPoint ( pPoint );
                }
                else
                    pScreenDisplay.DrawPoint ( pPoint );

            }
        }

        //���廭������
        private void DrawFeedback ( ref IScreenDisplay pScreenDisplay , IPoint sTartPoint , IPoint pEndPoint , ref IMap pCurMap )
        {
            ISymbol pFeedbackSymbol = MakeFeedbackSymbol ( ref pCurMap ) as ISymbol;

            IPolyline pPolyline = MakePolylineFromTowPoint ( sTartPoint , pEndPoint );

            ISimpleLineSymbol pSimpleLineSymbol = pFeedbackSymbol as ISimpleLineSymbol;

            pScreenDisplay.SetSymbol ( pSimpleLineSymbol as ISymbol );
            pScreenDisplay.DrawPolyline ( pPolyline );

            pPolyline = null;
        }

        //���廭������
        private void DrawFeedbackLine ( ref IScreenDisplay pScreenDisplay , IPolyline pPolyline , ref IMap pCurMap )
        {
            ISymbol pFeedbackSymbol = MakeFeedbackSymbol ( ref pCurMap ) as ISymbol;

            ISimpleLineSymbol pSimpleLineSymbol = pFeedbackSymbol as ISimpleLineSymbol;

            pScreenDisplay.SetSymbol ( pSimpleLineSymbol as ISymbol );
            pScreenDisplay.DrawPolyline ( pPolyline );
        }

        public IPoint MouseMove ( int x , int y )
        {
            IActiveView pActiveView = m_pCurMap as IActiveView;

            IPoint pMouseStopedPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint ( x , y );

            clsEditorMain.SnapPoint.SnapExcute ( pMouseStopedPoint );

            pMouseStopedPoint = clsEditorMain.SnapPoint.SnapResultPoint;

            DrawWhileMove ( ref pActiveView , pMouseStopedPoint , ref this.m_pCurMap );

            return pMouseStopedPoint;
        }

        //���·�����
        private bool DrawWhileMove ( ref IActiveView pActiveView , IPoint pMouseStopedPoint , ref IMap pCurMap )
        {
            int nPointCount;
            if ( this.m_pDrawedPoints == null )
                nPointCount = -1;
            else
                nPointCount = this.m_pDrawedPoints.PointCount - 1;

            if ( nPointCount < 0 ) return false;

            IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;

            if ( ( this.m_nDrawType == DrawTypeConstant.CommonLine ) || ( this.m_nDrawType == DrawTypeConstant.CommonPolygon ) )
            {
                IPoint pPoint = this.m_pDrawedPoints.get_Point ( nPointCount );

                pScreenDisplay.StartDrawing ( pScreenDisplay.hDC , -1 /*esriScreenCache.esriNoScreenCache*/);

                if ( this.m_pLastMouseStopedPoint.IsEmpty == false )
                    DrawFeedback ( ref pScreenDisplay , pPoint , m_pLastMouseStopedPoint , ref pCurMap );


                DrawFeedback ( ref pScreenDisplay , pPoint , pMouseStopedPoint , ref pCurMap );
                this.m_pLastMouseStopedPoint.PutCoords ( pMouseStopedPoint.X , pMouseStopedPoint.Y );


                pScreenDisplay.FinishDrawing ();
                return true;
            }

            return true;

        }



        //���ɷ����ߵķ���
        public static ISimpleLineSymbol MakeFeedbackSymbol ( ref IMap pMap )
        {
            IRgbColor pRgbColor = ModForEdit.CreatRgbColor ( 255 , 0 , 0 , false );
            pRgbColor.Transparency = 255;

            ISimpleLineSymbol pSimpleLineSymbol = new SimpleLineSymbolClass ();
            pSimpleLineSymbol.Color = pRgbColor;
            pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;

            ISymbol pSymbol = pSimpleLineSymbol as ISymbol;
            //�������ʽ���ƣ�������ǰ���ķ���
            pSymbol.ROP2 = esriRasterOpCode.esriROPXOrPen;
            //����Ķ��д����ǲ�����Ҫ�޸�,���ŷŴ�Ĳ�ͬ
            pSimpleLineSymbol.Width = ModForEdit.ScaleChange ( ref pMap , 1 );
            return pSimpleLineSymbol;

        }

        //
        public static ISymbol MakePolygonFeedbackSymbol ( ref IMap pMap , ref ILineSymbol pLineSymbol )
        {
            IRgbColor pFillColor = ModForEdit.CreatRgbColor ( 0 , 0 , 0 , true );

            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass ();

            pFillSymbol.Color = pFillColor;
            pFillSymbol.Outline = pLineSymbol;

            return pFillSymbol as ISymbol;

        }

        //����Ҫ���ߵķ���
        private static ISimpleLineSymbol MakeFeatureSymbol ( ref IMap pMap )
        {
            IRgbColor pRgbColor = ModForEdit.CreatRgbColor ( 0 , 128 , 0 , false );

            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass ();
            pLineSymbol.Color = pRgbColor;
            pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            pLineSymbol.Width = ModForEdit.ScaleChange ( ref pMap , 1 );

            return pLineSymbol;
        }

        //�ڵ����
        private static ISymbol MakeMarkerSymbol ( ref IMap pMap , int nR , int nG , int nB , bool blTrans )
        {
            IRgbColor pRgbColor = ModForEdit.CreatRgbColor ( nR , nG , nB , blTrans );

            ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass ();

            pMarkerSymbol.Size = ModForEdit.ScaleChange ( ref pMap , 4 );
            pMarkerSymbol.Color = pRgbColor;
            pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSSquare;

            return pMarkerSymbol as ISymbol;
        }

        //�ڵ����
        private static ISymbol MakeCanRasteMarkerSymbol ( ref IMap pMap , int nR , int nG , int nB , bool blTrans )
        {
            IRgbColor pRgbColor = ModForEdit.CreatRgbColor ( nR , nG , nB , blTrans );

            ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass ();

            pMarkerSymbol.Size = ModForEdit.ScaleChange ( ref pMap , 4 );
            pMarkerSymbol.Color = pRgbColor;
            pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSSquare;

            ISymbol pSymbol = pMarkerSymbol as ISymbol;
            pSymbol.ROP2 = esriRasterOpCode.esriROPXOrPen;

            return pSymbol;
        }
        //
        public bool DrawMoveGeometrys ( ref IGeometryCollection pGeometrys , ref IPoint pfPoint , ref IPoint ptPoint , ref IPoint pLastPoint )
        {
            if ( pGeometrys == null || pfPoint == null || ptPoint == null ) return false;


            int nCount = pGeometrys.GeometryCount;
            if ( nCount < 1 ) return false;
            nCount = nCount - 1;

            IActiveView pActiveView = this.m_pCurMap as IActiveView;

            IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
            pScreenDisplay.StartDrawing ( pScreenDisplay.hDC , -1 );

            double dx = ptPoint.X - pfPoint.X;
            double dy = ptPoint.Y - pfPoint.Y;
            double fLastX = 0;
            double fLastY = 0;

            if ( pLastPoint != null && !pLastPoint.IsEmpty )
            {
                fLastX = pLastPoint.X - pfPoint.X;
                fLastY = pLastPoint.Y - pfPoint.Y;
            }

            IPoint pPoint = null;
            IPolyline pPolyline = null;
            IPolygon pPolygon = null;
            ITransform2D pTransform2D = null;

            ISymbol pFeedbackSymbol = MakeFeedbackSymbol ( ref this.m_pCurMap ) as ISymbol;
            ILineSymbol pLineSymbol = pFeedbackSymbol as ILineSymbol;

            ISymbol pPFeedbackSymbol = MakePolygonFeedbackSymbol ( ref this.m_pCurMap , ref  pLineSymbol ) as ISymbol;

            ISymbol pPTSymbol = MakeCanRasteMarkerSymbol ( ref this.m_pCurMap , 255 , 0 , 0 , false );

            for ( int i = 0 ; i <= nCount ; i++ )
            {
                IGeometry pGeometry = pGeometrys.get_Geometry ( i );

                if ( !pGeometry.IsEmpty && pGeometry.GeometryType == esriGeometryType.esriGeometryPoint )
                {
                    pScreenDisplay.SetSymbol ( pPTSymbol );
                    pTransform2D = pGeometry as ITransform2D;

                    if ( !( fLastX == 0 && fLastY == 0 ) )
                    {
                        pTransform2D.Move ( fLastX , fLastY );

                        pPoint = pGeometry as IPoint;
                        pScreenDisplay.DrawPoint ( pPoint );

                        pTransform2D.Move ( -fLastX , -fLastY );
                    }

                    if ( !( dx == 0 && dy == 0 ) )
                    {
                        pTransform2D.Move ( dx , dy );

                        pPoint = pGeometry as IPoint;
                        pScreenDisplay.DrawPoint ( pPoint );

                        pTransform2D.Move ( -dx , -dy );
                    }
                }
                else if ( !pGeometry.IsEmpty && pGeometry.GeometryType == esriGeometryType.esriGeometryPolyline )
                {
                    pScreenDisplay.SetSymbol ( pFeedbackSymbol );
                    pTransform2D = pGeometry as ITransform2D;

                    if ( !( fLastX == 0 && fLastY == 0 ) )
                    {
                        pTransform2D.Move ( fLastX , fLastY );

                        pPolyline = pGeometry as IPolyline;
                        pScreenDisplay.DrawPolyline ( pPolyline );

                        pTransform2D.Move ( -fLastX , -fLastY );
                    }

                    if ( !( dx == 0 && dy == 0 ) )
                    {
                        pTransform2D.Move ( dx , dy );

                        pPolyline = pGeometry as IPolyline;
                        pScreenDisplay.DrawPolyline ( pPolyline );

                        pTransform2D.Move ( -dx , -dy );
                    }
                }
                else if ( !pGeometry.IsEmpty && pGeometry.GeometryType == esriGeometryType.esriGeometryPolygon )
                {
                    pScreenDisplay.SetSymbol ( pPFeedbackSymbol );
                    pTransform2D = pGeometry as ITransform2D;

                    if ( !( fLastX == 0 && fLastY == 0 ) )
                    {
                        pTransform2D.Move ( fLastX , fLastY );

                        pPolygon = pGeometry as IPolygon;
                        pScreenDisplay.DrawPolygon ( pPolygon );

                        pTransform2D.Move ( -fLastX , -fLastY );
                    }

                    if ( !( dx == 0 && dy == 0 ) )
                    {
                        pTransform2D.Move ( dx , dy );

                        pPolygon = pGeometry as IPolygon;
                        pScreenDisplay.DrawPolygon ( pPolygon );

                        pTransform2D.Move ( -dx , -dy );
                    }
                }

            }

            pScreenDisplay.SetSymbol ( pFeedbackSymbol );

            if ( pLastPoint != null && !pLastPoint.IsEmpty )
            {
                pPolyline = MakePolylineFromTowPoint ( pfPoint , pLastPoint );
                pScreenDisplay.DrawPolyline ( pPolyline );
            }

            pPolyline = MakePolylineFromTowPoint ( pfPoint , ptPoint );
            pScreenDisplay.DrawPolyline ( pPolyline );

            pScreenDisplay.FinishDrawing ();
            return true;
        }

        public bool DrawRotateGeometrys ( ref  IGeometryCollection pGeometrys , ref IPoint pFromPoint , double fLastAngle ,
            double fAngle , ref IPoint ptPoint , ref IPoint pLastPoint )
        {

            if ( pGeometrys == null || pFromPoint == null || pFromPoint.IsEmpty ) return false;

            int nCount = pGeometrys.GeometryCount;
            if ( nCount < 1 ) return false;
            nCount = nCount - 1;

            IActiveView pActiveView = this.m_pCurMap as IActiveView;

            IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
            pScreenDisplay.StartDrawing ( pScreenDisplay.hDC , -1 );

            IPoint pPoint = null;
            IPolyline pPolyline = null;
            IPolygon pPolygon = null;
            ITransform2D pTransform2D = null;

            ISymbol pFeedbackSymbol = MakeFeedbackSymbol ( ref this.m_pCurMap ) as ISymbol;
            ILineSymbol pLineSymbol = pFeedbackSymbol as ILineSymbol;

            ISymbol pPFeedbackSymbol = MakePolygonFeedbackSymbol ( ref this.m_pCurMap , ref  pLineSymbol ) as ISymbol;

            ISymbol pPTSymbol = MakeCanRasteMarkerSymbol ( ref this.m_pCurMap , 255 , 0 , 0 , false );

            for ( int i = 0 ; i <= nCount ; i++ )
            {
                IGeometry pGeometry = pGeometrys.get_Geometry ( i );

                if ( pGeometry.IsEmpty == false && pGeometry.GeometryType == esriGeometryType.esriGeometryPoint )
                {
                    pScreenDisplay.SetSymbol ( pPTSymbol );

                    pTransform2D = pGeometry as ITransform2D;

                    if ( fLastAngle != 0 )
                    {
                        pTransform2D.Rotate ( pFromPoint , fLastAngle );

                        pPoint = pGeometry as IPoint;
                        pScreenDisplay.DrawPoint ( pPoint );

                        pTransform2D.Rotate ( pFromPoint , -fLastAngle );
                    }

                    if ( fAngle != 0 )
                    {
                        pTransform2D.Rotate ( pFromPoint , fAngle );

                        pPoint = pGeometry as IPoint;
                        pScreenDisplay.DrawPoint ( pPoint );

                        pTransform2D.Rotate ( pFromPoint , -fAngle );
                    }
                }
                else if ( pGeometry.IsEmpty == false && pGeometry.GeometryType == esriGeometryType.esriGeometryPolyline )
                {
                    pScreenDisplay.SetSymbol ( pFeedbackSymbol );
                    pTransform2D = pGeometry as ITransform2D;

                    if ( fLastAngle != 0 )
                    {
                        pTransform2D.Rotate ( pFromPoint , fLastAngle );

                        pPolyline = pGeometry as IPolyline;
                        pScreenDisplay.DrawPolyline ( pPolyline );

                        pTransform2D.Rotate ( pFromPoint , -fLastAngle );
                    }

                    if ( fAngle != 0 )
                    {
                        pTransform2D.Rotate ( pFromPoint , fAngle );

                        pPolyline = pGeometry as IPolyline;
                        pScreenDisplay.DrawPolyline ( pPolyline );

                        pTransform2D.Rotate ( pFromPoint , -fAngle );
                    }
                }
                else if ( pGeometry.IsEmpty == false && pGeometry.GeometryType == esriGeometryType.esriGeometryPolygon )
                {
                    pScreenDisplay.SetSymbol ( pPFeedbackSymbol );
                    pTransform2D = pGeometry as ITransform2D;

                    if ( fLastAngle != 0 )
                    {
                        pTransform2D.Rotate ( pFromPoint , fLastAngle );

                        pPolygon = pGeometry as IPolygon;
                        pScreenDisplay.DrawPolygon ( pPolygon );

                        pTransform2D.Rotate ( pFromPoint , -fLastAngle );
                    }

                    if ( fAngle != 0 )
                    {
                        pTransform2D.Rotate ( pFromPoint , fAngle );

                        pPolygon = pGeometry as IPolygon;
                        pScreenDisplay.DrawPolygon ( pPolygon );

                        pTransform2D.Rotate ( pFromPoint , -fAngle );
                    }

                }
            }

            pScreenDisplay.SetSymbol ( pFeedbackSymbol );

            if ( pLastPoint != null && !pLastPoint.IsEmpty )
            {
                pPolyline = MakePolylineFromTowPoint ( pFromPoint , pLastPoint );
                pScreenDisplay.DrawPolyline ( pPolyline );
            }

            pPolyline = MakePolylineFromTowPoint ( pFromPoint , ptPoint );
            pScreenDisplay.DrawPolyline ( pPolyline );

            pScreenDisplay.FinishDrawing ();

            return true;
        }


        //
        public bool DrawMirror ( ref IMap pMap , ref IPoint pFromPoint , ref IPoint pPoint , ref IPoint pLastPoint )
        {
            if ( pMap == null || pFromPoint == null || pPoint == null ) return false;

            IEnumFeature pEnumFeature = pMap.FeatureSelection as IEnumFeature;
            if ( pEnumFeature == null ) return false;
            pEnumFeature.Reset ();

            IFeature pFeature = pEnumFeature.Next ();
            if ( pFeature == null ) return false;

            IActiveView pActiveView = pMap as IActiveView;


            IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
            pScreenDisplay.StartDrawing ( pScreenDisplay.hDC , -1 );

            IPoint pTempPoint = null;
            IPolyline pPolyline = null;
            IPolygon pPolygon = null;

            ISymbol pFeedbackSymbol = MakeFeedbackSymbol ( ref pMap ) as ISymbol;
            ILineSymbol pLineSymbol = pFeedbackSymbol as ILineSymbol;

            ISymbol pPFeedbackSymbol = MakePolygonFeedbackSymbol ( ref pMap , ref  pLineSymbol ) as ISymbol;

            ISymbol pPTSymbol = MakeCanRasteMarkerSymbol ( ref pMap , 255 , 0 , 0 , false );

            if ( !pLastPoint.IsEmpty )
                DrawFeedback ( ref pScreenDisplay , pFromPoint , pLastPoint , ref pMap );
            DrawFeedback ( ref pScreenDisplay , pFromPoint , pPoint , ref pMap );

            while ( pFeature != null )
            {
                IGeometry pGeometry = pFeature.ShapeCopy;
                if ( !( pGeometry == null || pGeometry.IsEmpty ) )
                {
                    ESRI.ArcGIS.esriSystem.IClone pClone = pGeometry as ESRI.ArcGIS.esriSystem.IClone;
                    IGeometry pTempGeometry = pClone.Clone () as IGeometry;

                    if ( !pLastPoint.IsEmpty )
                    {
                        IGeometry pLastMirrorGeo = GetMirrorGeometry ( pGeometry , pFromPoint , pLastPoint );
                        if ( pLastMirrorGeo != null )
                        {
                            if ( pGeometry.GeometryType == esriGeometryType.esriGeometryPolygon )
                            {
                                pScreenDisplay.SetSymbol ( pPFeedbackSymbol );

                                pPolygon = pLastMirrorGeo as IPolygon;
                                pScreenDisplay.DrawPolygon ( pPolygon );
                            }
                            else if ( pGeometry.GeometryType == esriGeometryType.esriGeometryPolyline )
                            {
                                pScreenDisplay.SetSymbol ( pFeedbackSymbol );

                                pPolyline = pLastMirrorGeo as IPolyline;
                                pScreenDisplay.DrawPolyline ( pPolyline );
                            }
                            else if ( pGeometry.GeometryType == esriGeometryType.esriGeometryPoint )
                            {
                                pScreenDisplay.SetSymbol ( pPTSymbol );

                                pTempPoint = pLastMirrorGeo as IPoint;
                                pScreenDisplay.DrawPoint ( pTempPoint );
                            }
                        }
                    }

                    IGeometry pMirrorGeo = GetMirrorGeometry ( pTempGeometry , pFromPoint , pPoint );
                    if ( pMirrorGeo != null )
                    {
                        if ( pGeometry.GeometryType == esriGeometryType.esriGeometryPolygon )
                        {
                            pScreenDisplay.SetSymbol ( pPFeedbackSymbol );

                            pPolygon = pMirrorGeo as IPolygon;
                            pScreenDisplay.DrawPolygon ( pPolygon );
                        }
                        else if ( pGeometry.GeometryType == esriGeometryType.esriGeometryPolyline )
                        {
                            pScreenDisplay.SetSymbol ( pFeedbackSymbol );

                            pPolyline = pMirrorGeo as IPolyline;
                            pScreenDisplay.DrawPolyline ( pPolyline );
                        }
                        else if ( pGeometry.GeometryType == esriGeometryType.esriGeometryPoint )
                        {
                            pScreenDisplay.SetSymbol ( pPTSymbol );

                            pTempPoint = pMirrorGeo as IPoint;
                            pScreenDisplay.DrawPoint ( pTempPoint );
                        }
                    }
                }
                pFeature = pEnumFeature.Next ();
            }
            pScreenDisplay.FinishDrawing ();

            return true;
        }

        public IGeometry GetMirrorGeometry ( IGeometry pGeometry , IPoint pFromPoint , IPoint pToPoint )
        {
            if ( pGeometry == null || pFromPoint == null || pToPoint == null || pFromPoint.IsEmpty || pToPoint.IsEmpty )
                return null;

            if ( Math.Abs ( pFromPoint.X - pToPoint.X ) < 0.01 && Math.Abs ( pFromPoint.Y - pToPoint.Y ) < 0.01 ) return null;

            ILine pLine = new LineClass ();
            pLine.FromPoint = pFromPoint;
            pLine.ToPoint = pToPoint;

            IAffineTransformation2D pATransform2D = new AffineTransformation2DClass ();
            pATransform2D.DefineReflection ( pLine );

            ITransform2D pTransform2D = pGeometry as ITransform2D;
            pTransform2D.Transform ( esriTransformDirection.esriTransformForward , pATransform2D );

            pLine = null;

            return pTransform2D as IGeometry;
        }

        /*
        public void DrawParrelline(double fLen, IPoint pMousePoint)
        {
            Boolean blDrawTrack = true;

            int nPointCount = 0;
            if (this.m_pDrawedPoints == null)
                nPointCount = -1;
            else
                nPointCount = this.m_pDrawedPoints.PointCount - 1;

            if (nPointCount < 0) return;
            if (pMousePoint == null && nPointCount < 1) return;

            ESRI.ArcGIS.Carto.IActiveView pActiveView = this.m_pCurMap as ESRI.ArcGIS.Carto.IActiveView;

            ESRI.ArcGIS.Display.IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
            pScreenDisplay.StartDrawing(pScreenDisplay.hDC, -1);// esriNoScreenCache

            IPolyline pPolyline = null;

            IPolyline pTempPolyline = null;

            if (pMousePoint == null)
            {
                //�����(������������)
                pPolyline = GetParallelPolyline2(this.m_pDrawedPoints, null, fLen, this.m_pCurMap);

                pTempPolyline = this.m_pDrawedPoints as IPolyline;
                DrawPoints(ref pScreenDisplay, ref  pTempPolyline, ref this.m_pCurMap);
                DrawPoints(ref pScreenDisplay, ref  pPolyline, ref this.m_pCurMap);
            }
            else
            {
                //����ƶ���������һ���㣩
                IPoint pPoint = this.m_pDrawedPoints.get_Point(nPointCount);
                DrawFeedback(ref pScreenDisplay, pPoint, pMousePoint, ref this.m_pCurMap);

                if (nPointCount > 0)
                {
                    pTempPolyline = this.m_pDrawedPoints as IPolyline;
                    DrawPoints(ref pScreenDisplay, ref pTempPolyline, ref this.m_pCurMap);

                }

                pPolyline = GetParallelPolyline2(this.m_pDrawedPoints, pMousePoint, fLen, this.m_pCurMap);
                if (!pPolyline.IsEmpty)
                {
                    IPointCollection pPointCollection = pPolyline as IPointCollection;

                    int nCount = pPointCollection.PointCount;

                    IPoint pPoint1 = pPointCollection.get_Point(nCount - 2);

                    IPoint pPoint2 = pPointCollection.get_Point(nCount - 1);

                    DrawFeedback(ref pScreenDisplay, pPoint1, pPoint2, ref this.m_pCurMap);

                    if (nCount > 2)
                    {
                        pPointCollection.RemovePoints(nCount - 1, 1);
                        pPolyline = pPointCollection as IPolyline;
                        DrawPoints(ref pScreenDisplay, ref pPolyline, ref this.m_pCurMap);
                    }
                }
            }
            pScreenDisplay.FinishDrawing();
        }*/

        //�����ߵĵ㼯�Լ���ǰ�������㣬���ظ��������ƽ����
        private IPolyline GetParallelPolyline2 ( IPointCollection pPointCol , IPoint pCurMonsePoint , double dDistance , IMap pRelMap )
        {
            if ( pPointCol == null || pRelMap == null || pPointCol.PointCount < 1 || dDistance == 0 ) return null;

            IPointCollection pPoints = null;
            IPolyline pPolyline = null;

            object pBObj = Type.Missing;
            object pAObj = Type.Missing;

            if ( pPointCol.PointCount == 1 )
            {
                if ( pCurMonsePoint != null )
                {
                    IPoint pFromPoint = null;
                    IPoint pToPoint = null;

                    pPoints = new PolylineClass () as IPointCollection;
                    pFromPoint = GetPdicularPoint ( pPointCol.get_Point ( 0 ) , pCurMonsePoint , dDistance , pRelMap );

                    if ( pFromPoint != null )
                    {
                        pPoints.AddPoint ( pFromPoint , ref pBObj , ref pAObj );
                        pToPoint = GetCrossPoint ( pPointCol.get_Point ( 0 ) , pFromPoint , pCurMonsePoint );
                        if ( pToPoint != null )
                            pPoints.AddPoint ( pToPoint , ref pBObj , ref pAObj );

                    }

                    if ( pPoints.PointCount > 1 )
                    {
                        pPolyline = pPoints as IPolyline;
                        return pPolyline;
                    }
                }
            }

            pPoints = new PolylineClass () as IPointCollection;

            pPoints.AddPointCollection ( pPointCol );

            if ( pCurMonsePoint != null )
                pPoints.AddPoint ( pCurMonsePoint , ref pBObj , ref pAObj );

            pPolyline = pPoints as IPolyline;

            IConstructCurve pCtructCurcve = new PolylineClass () as IConstructCurve;

            IPolycurve pPolycurve = pPolyline as IPolycurve;

            pCtructCurcve.ConstructOffset ( pPolycurve , dDistance , ref pBObj , ref pAObj );

            return pCtructCurcve as IPolyline;
        }

        //����һƽ���ı��������㣬����һ����(���������ͬһ������ϵ��)
        private IPoint GetCrossPoint ( IPoint pOriginPoint , IPoint pFirstPoint , IPoint pSecondPoint )
        {
            if ( pOriginPoint == null || pFirstPoint == null || pSecondPoint == null ) return null;

            IPoint pPoint = new PointClass ();
            pPoint.X = pFirstPoint.X + pSecondPoint.X - pOriginPoint.X;
            pPoint.Y = pFirstPoint.Y + pSecondPoint.Y - pOriginPoint.Y;
            return pPoint;
        }

        //����һ�����ϵĵ�һ���˵㣬�������Ҵ����ϸ�������(��ͼ�������)����һ�������
        private IPoint GetPdicularPoint ( IPoint pFirstPointOnLine , IPoint pSecondPointOnLine , double dDistance , IMap pRelMap )
        {

            double dNewDistance = 0;
            IPoint pFirstPoint = null;
            IPoint pSecondPoint = null;

            if ( pFirstPointOnLine == null || pSecondPointOnLine == null || pRelMap == null ) return null;

            dNewDistance = MapLengthToDeviceLenth ( dDistance , pRelMap );
            if ( dNewDistance == 0 ) return null;

            pFirstPoint = MapPointToDevicePoint ( pFirstPointOnLine , pRelMap );
            pSecondPoint = MapPointToDevicePoint ( pSecondPointOnLine , pRelMap );

            if ( CompareTwoPoints ( pFirstPoint , pSecondPoint ) ) return null;

            IPoint pPoint = new PointClass ();

            if ( pFirstPoint.X == pSecondPoint.X )
            {
                if ( pFirstPoint.Y < pSecondPoint.Y )
                {
                    pPoint.X = pFirstPoint.X - dNewDistance;
                    pPoint.Y = pFirstPoint.Y;
                }
                else
                {
                    pPoint.X = pFirstPoint.X + dNewDistance;
                    pPoint.Y = pFirstPoint.Y;
                }
                return DevicePointToMapPoint ( pPoint , pRelMap );
            }

            if ( pFirstPoint.Y == pSecondPoint.Y )
            {
                if ( pFirstPoint.X < pSecondPoint.X )
                {
                    pPoint.X = pFirstPoint.X;
                    pPoint.Y = pFirstPoint.Y + dNewDistance;
                }
                else
                {
                    pPoint.X = pFirstPoint.X;
                    pPoint.Y = pFirstPoint.Y - dNewDistance;
                }
                return DevicePointToMapPoint ( pPoint , pRelMap );
            }

            double dKey = ( pSecondPoint.Y - pFirstPoint.Y ) / ( pSecondPoint.X - pFirstPoint.X );
            double dAngle = Math.Atan ( dKey ) + 1.571;  //1.571����90�ȵĻ��ȴ�С
            pPoint.X = dNewDistance * Math.Cos ( dAngle ) + pFirstPoint.X;
            pPoint.Y = dNewDistance * Math.Sin ( dAngle ) + pFirstPoint.Y;

            return DevicePointToMapPoint ( pPoint , pRelMap );
        }

        //�Ƚ���������Ƿ�Ϊͬһ����
        //todo:���㷨������
        private bool CompareTwoPoints ( IPoint pPointOne , IPoint pPointTwo )
        {
            if ( pPointOne == null || pPointTwo == null ) return false;

            if ( pPointOne.X == pPointTwo.X && pPointOne.Y == pPointTwo.Y )
                return true;
            else
                return false;

        }

        //��ͼ����ת��Ϊ�豸����
        private double MapLengthToDeviceLenth ( double dMapLenth , IMap pRelMap )
        {
            if ( pRelMap == null ) return 0;

            IActiveView pActView = pRelMap as IActiveView;

            return pActView.ScreenDisplay.DisplayTransformation.ToPoints ( dMapLenth );
        }

        //�豸�����ת��Ϊ��ͼ�����
        private IPoint DevicePointToMapPoint ( IPoint pDevicePoint , IMap pRelMap )
        {
            if ( pDevicePoint == null || pRelMap == null ) return null;

            IActiveView pActView = pRelMap as IActiveView;
            IPoint pPoint = pActView.ScreenDisplay.DisplayTransformation.ToMapPoint ( ( int )pDevicePoint.X , ( int )pDevicePoint.Y );

            return pPoint;
        }

        //��ͼ�����ת��Ϊ�豸�����
        private IPoint MapPointToDevicePoint ( IPoint pMapPoint , IMap pRelMap )
        {
            if ( pMapPoint == null || pRelMap == null ) return null;

            IActiveView pActView = pRelMap as IActiveView;
            IPoint pPoint = new PointClass ();

            int dx = 0;
            int dy = 0;
            pActView.ScreenDisplay.DisplayTransformation.FromMapPoint ( pMapPoint , out  dx , out  dy );

            pPoint.X = dx;
            pPoint.Y = dy;

            return pPoint;
        }

        public void DrawParalMove ( double fLen , ref IPoint pMousePoint , ref IPoint pLastMousePoint , ref IPolyline pLastPolyline )
        {
            int nPointCount = 0;
            if ( this.m_pDrawedPoints == null )
                nPointCount = -1;
            else
                nPointCount = this.m_pDrawedPoints.PointCount - 1;

            if ( nPointCount < 0 ) return; //����ƶ���������һ���㣩
            if ( pMousePoint == null || pMousePoint.IsEmpty ) return;

            IActiveView pActiveView = this.m_pCurMap as IActiveView;

            IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
            pScreenDisplay.StartDrawing ( pScreenDisplay.hDC , -1 );// esriNoScreenCache

            IPolyline pPolyline = null;

            IPolyline pTempPolyline = null;

            if ( !( pLastPolyline == null || pLastPolyline.IsEmpty ) )
            {
                pTempPolyline = GetPartPolyLine ( true , pLastPolyline , nPointCount );
                if ( pTempPolyline != null )
                    DrawFeedbackLine ( ref pScreenDisplay , pTempPolyline , ref this.m_pCurMap );//�����ϴε�ƽ����
            }

            IPoint pPoint = this.m_pDrawedPoints.get_Point ( nPointCount );

            if ( pLastMousePoint.IsEmpty == false )
                DrawFeedback ( ref pScreenDisplay , pPoint , pLastMousePoint , ref this.m_pCurMap ); //�����ϴεķ�����

            DrawFeedback ( ref pScreenDisplay , pPoint , pMousePoint , ref this.m_pCurMap );         //�汾�η�����

            pPolyline = GetParallelPolyline2 ( this.m_pDrawedPoints , pMousePoint , fLen , this.m_pCurMap );
            if ( !pPolyline.IsEmpty )
            {
                pTempPolyline = GetPartPolyLine ( true , pPolyline , nPointCount );
                if ( pTempPolyline != null )
                    DrawFeedbackLine ( ref pScreenDisplay , pTempPolyline , ref this.m_pCurMap );
                pLastPolyline = pPolyline;       //�汾�ε�ƽ����

            }
            pScreenDisplay.FinishDrawing ();
        }

        public void DrawParalDown ( double fLen , ref IPolyline pLastPolyline )
        {
            int nPointCount = 0;
            if ( this.m_pDrawedPoints == null )
                nPointCount = -1;
            else
                nPointCount = this.m_pDrawedPoints.PointCount - 1;

            if ( nPointCount < 1 ) return; //����ƶ��������ж����㣩

            IActiveView pActiveView = this.m_pCurMap as IActiveView;

            IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
            pScreenDisplay.StartDrawing ( pScreenDisplay.hDC , -1 );// esriNoScreenCache

            IPolyline pPolyline = null;

            IPolyline pTempPolyline = null;

            if ( !( pLastPolyline == null || pLastPolyline.IsEmpty ) )
            {
                pTempPolyline = GetPartPolyLine ( true , pLastPolyline , nPointCount );
                if ( pTempPolyline != null )
                    DrawFeedbackLine ( ref pScreenDisplay , pTempPolyline , ref this.m_pCurMap );
            }

            pPolyline = GetParallelPolyline2 ( this.m_pDrawedPoints , null , fLen , this.m_pCurMap );
            if ( !pPolyline.IsEmpty )
            {
                pTempPolyline = GetPartPolyLine ( true , pPolyline , nPointCount );
                if ( pTempPolyline != null )
                    DrawFeedbackLine ( ref pScreenDisplay , pTempPolyline , ref this.m_pCurMap );
                pLastPolyline = pPolyline;

                pTempPolyline = GetPartPolyLine ( false , pLastPolyline , nPointCount );
                if ( pTempPolyline != null && nPointCount > 1 )
                {
                    pTempPolyline = GetLastSegment ( pTempPolyline );
                    DrawPoints ( ref pScreenDisplay , ref pTempPolyline , ref this.m_pCurMap );
                }
            }

            if ( nPointCount > 0 )
            {
                pTempPolyline = MakePolylineFromTowPoint ( this.m_pDrawedPoints.get_Point ( nPointCount - 1 ) , this.m_pDrawedPoints.get_Point ( nPointCount ) );
                DrawPoints ( ref pScreenDisplay , ref pTempPolyline , ref this.m_pCurMap );          //���Ѿ�ȷ������
            }

            pScreenDisplay.FinishDrawing ();
        }


        private IPolyline GetPartPolyLine ( bool blEnd , IPolyline pPolyline , int nPointCount )
        {
            if ( pPolyline == null || pPolyline.IsEmpty ) return null;
            ISegmentCollection pSegmentCollection = pPolyline as ISegmentCollection;

            int nCount = pSegmentCollection.SegmentCount;
            if ( nCount < 1 ) return null;

            IPolyline pNewPolyline = new PolylineClass ();
            ISegmentCollection pSegments = pNewPolyline as ISegmentCollection;


            object pBobj = Type.Missing;
            object pAobj = Type.Missing;

            ISegment pSegment = null;

            if ( blEnd )
            {
                if ( nCount - 2 > -1 )
                {
                    pSegment = pSegmentCollection.get_Segment ( nCount - 2 );
                    pSegments.AddSegment ( pSegment , ref pBobj , ref pAobj );
                }

                pSegment = pSegmentCollection.get_Segment ( nCount - 1 );
                pSegments.AddSegment ( pSegment , ref pBobj , ref pAobj );
            }
            else
            {
                if ( nCount < 2 ) return null;

                int i = 0;
                for ( i = 0 ; i < nPointCount - 1 ; i++ )
                {
                    pSegment = pSegmentCollection.get_Segment ( i );
                    pSegments.AddSegment ( pSegment , ref pBobj , ref pAobj );
                }
            }
            return pNewPolyline;
        }


        private IPolyline GetLastSegment ( IPolyline pPolyline )
        {
            if ( pPolyline == null || pPolyline.IsEmpty ) return null;
            ISegmentCollection pSegmentCollection = pPolyline as ISegmentCollection;

            int nCount = pSegmentCollection.SegmentCount;
            if ( nCount < 1 ) return null;

            IPolyline pNewPolyline = new PolylineClass ();
            ISegmentCollection pSegments = pNewPolyline as ISegmentCollection;


            object pBobj = Type.Missing;
            object pAobj = Type.Missing;

            ISegment pSegment = null;
            pSegment = pSegmentCollection.get_Segment ( nCount - 1 );
            pSegments.AddSegment ( pSegment , ref pBobj , ref pAobj );
            return pNewPolyline;
        }


        public void ReDrawParal ( double fLen )
        {
            int nPointCount = 0;
            if ( this.m_pDrawedPoints == null )
                nPointCount = -1;
            else
                nPointCount = this.m_pDrawedPoints.PointCount - 1;

            if ( nPointCount < 1 ) return; //����ƶ��������ж����㣩


            IActiveView pActiveView = this.m_pCurMap as IActiveView;

            IScreenDisplay pScreenDisplay = pActiveView.ScreenDisplay;
            pScreenDisplay.StartDrawing ( pScreenDisplay.hDC , -1 );// esriNoScreenCache

            IPolyline pPolyline = this.m_pDrawedPoints as IPolyline;
            DrawPoints ( ref pScreenDisplay , ref pPolyline , ref this.m_pCurMap );

            IPolyline pTempPolyline = null;

            pPolyline = GetParallelPolyline2 ( this.m_pDrawedPoints , null , fLen , this.m_pCurMap );
            if ( !pPolyline.IsEmpty )
            {
                pTempPolyline = GetPartPolyLine ( false , pPolyline , nPointCount );
                if ( pTempPolyline != null && nPointCount > 1 )
                    DrawPoints ( ref pScreenDisplay , ref pTempPolyline , ref this.m_pCurMap );
            }

            pScreenDisplay.FinishDrawing ();
        }

    }
}
