using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Data;

using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace GeoUtilities
{
    public sealed class ControlsMapMeasureToolDefClass : BaseTool
    {
        //����ֶ�
        private IHookHelper m_hookHelper;
        private IMap m_pMap;
        private IScreenDisplay m_pScreenDisplay;
        private IActiveView m_pActiveView;

        private INewLineFeedback m_pNewLineFeedback;
        private INewPolygonFeedback m_pPlygonFeadback;

        IPointCollection m_PntCol;         //��ʱ��¼δ����ǰ���ĵ���

        IPoint m_pFromPoint;
        IPoint m_pToPoint;
        IPointCollection m_pPlygon;       //�����������ʱ�����
        ISymbol m_pSnapPntSymbol;        //��¼�����Ʋ�׽�ĵ�ķ��ţ�������֤ÿ�β�׽ʱ�ڵ��ػ�
        IPoint m_pSnapPoint;              //��¼�ϴβ�׽���Ľڵ�

        private frmMeasureResult m_frmMeasureResult;
		private double dlength;//��¼ÿ����ʱ���� xisheng 20110801
        //private double darea;//��¼ÿ����ʱ���� xisheng 20110808

        private INewLineFeedback m_meaAangleLineFb = null; //yjl20110816 add�����Ƕ�
        private IPoint m_meaFromPoint = null;//yjl20110816 add 

        //��ķ���
        public ControlsMapMeasureToolDefClass()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "GeoCommon"; //localizable text 
            base.m_caption = "MeasureTool";  //localizable text 
            base.m_message = "���㹤��";  //localizable text
            base.m_toolTip = "���㹤��";  //localizable text
            base.m_name = base.m_category + "_" + base.m_caption;   //unique id, non-localizable (e.g. "MyCategory_MyTool")
            try
            {
                //
                // TODO: change resource name if necessary
                //
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), "Resources.MeasureTool.cur");
            }
            catch
            {
                //System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            // TODO:  Add MeasureTool.OnCreate implementation
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;
            if (m_frmMeasureResult == null)
            {
                m_frmMeasureResult = new frmMeasureResult(m_hookHelper.FocusMap.MapUnits, this);
                m_frmMeasureResult.FormClosed += new FormClosedEventHandler(FrmMeasureResult_FormClosed);
            }

            m_PntCol = new PolylineClass();
        }
        //added by chulili 20121022 �Թر��¼��Ĵ���
        private void FrmMeasureResult_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_frmMeasureResult = null;
        }
        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add MeasureTool.OnClick implementation
            //�������壬������ʾ������
            m_pMap = m_hookHelper.FocusMap;
            m_pActiveView = m_hookHelper.ActiveView;
            m_pScreenDisplay = m_pActiveView.ScreenDisplay;

            if (m_frmMeasureResult == null)
            {
                m_frmMeasureResult = new frmMeasureResult(m_hookHelper.FocusMap.MapUnits,this);
                m_frmMeasureResult.Show();
                m_frmMeasureResult.FormClosed += new FormClosedEventHandler(FrmMeasureResult_FormClosed);
            }
            else if (this.m_frmMeasureResult.Visible == false)
            {
                m_frmMeasureResult.Visible = true;
                m_frmMeasureResult.TopMost = true;
				m_frmMeasureResult.Focus();
                m_frmMeasureResult.Show();
            }

            //���Ʋ��������ť�Ƿ���ã�ֻ�е�ǰ�ռ�ο���ͶӰ����ϵ�ſɲ����������
            ISpatialReference pSpatialRef = m_pMap.SpatialReference;
			m_frmMeasureResult.IsCanMeasureArea(true);
            
            //if (pSpatialRef is IProjectedCoordinateSystem)
            //    m_frmMeasureResult.IsCanMeasureArea(true);
            //else
            //    m_frmMeasureResult.IsCanMeasureArea(false);

            m_pActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, m_pActiveView.Extent);

            if (m_PntCol.PointCount!=0)
            {
                if (this.m_frmMeasureResult.m_CurMeasureType == 0)
                {
                    m_frmMeasureResult.dblSegLength = 0;
                    m_frmMeasureResult.dblLength = 0;
                    IPoint pPoint = m_PntCol.get_Point(0);
                    for (int i = 1; i < m_PntCol.PointCount; i++)
                    {
                        double deltaX;
                        double deltaY;
                        deltaX = m_PntCol.get_Point(i).X - pPoint.X;
                        deltaY = m_PntCol.get_Point(i).Y - pPoint.Y;
                        m_frmMeasureResult.dblLength+=System.Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
                        
                        //�ж��ǲ���Ҫ�����ܺ� xisheng 20111118
                        if (m_frmMeasureResult.m_bShowSum)
                        {
                            this.m_frmMeasureResult.dblSumLength += m_frmMeasureResult.dblLength;
                        }
                        //***********************************end

                        pPoint = m_PntCol.get_Point(i);
                    }
                }
                else if (this.m_frmMeasureResult.m_CurMeasureType == 1)
                {
                    m_frmMeasureResult.dblSegLength = 0;
                    m_frmMeasureResult.dblLength = 0;
                    m_frmMeasureResult.dblArea = 0;
                    IPoint pPoint = m_PntCol.get_Point(0);
                    for (int i = 1; i < m_PntCol.PointCount; i++)
                    {
                        double deltaX;
                        double deltaY;
                        deltaX = m_PntCol.get_Point(i).X - pPoint.X;
                        deltaY = m_PntCol.get_Point(i).Y - pPoint.Y;
                        m_frmMeasureResult.dblLength += System.Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
                        //�ж��ǲ���Ҫ�����ܺ� xisheng 20111118
                        if (m_frmMeasureResult.m_bShowSum)
                        {
                            this.m_frmMeasureResult.dblSumLength += m_frmMeasureResult.dblLength;
                        }
                        //***********************************end

                        pPoint = m_PntCol.get_Point(i);
                    }

                    //��¼��ʱ�����
                    m_pPlygon = new ESRI.ArcGIS.Geometry.PolygonClass();
                    for (int i = 0; i < m_PntCol.PointCount; i++)
                    {
                        m_pFromPoint = m_PntCol.get_Point(i);
                        m_pPlygon.AddPoints(1, ref m_pFromPoint);
                    }

                    //IArea parea = m_pPlygon as IArea;
                    //this.m_frmMeasureResult.dblArea = parea.Area;
                    //this.m_frmMeasureResult.dblSumArea += parea.Area;
                }
            }
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            if (m_frmMeasureResult != null)
            {
                if (m_pNewLineFeedback == null && m_pPlygonFeadback == null && m_frmMeasureResult.m_bIsFeatureMeasure == true) return;

            }
            // TODO:  Add MeasureTool.OnMouseDown implementation
            if (Button != 1) return; 

            //���������������
            if (this.m_frmMeasureResult == null)
            {
                m_frmMeasureResult = new frmMeasureResult(m_hookHelper.FocusMap.MapUnits,this);
                m_frmMeasureResult.Show();
                m_frmMeasureResult.FormClosed += new FormClosedEventHandler(FrmMeasureResult_FormClosed);
            }
            else if (this.m_frmMeasureResult.Visible == false)
            {
				//20110801 xisheng 
                m_frmMeasureResult.Visible = true;
                //m_frmMeasureResult = new frmMeasureResult(m_hookHelper.FocusMap.MapUnits, this);
				m_frmMeasureResult.Focus();
                m_frmMeasureResult.Show();
            }
            //����Ļ����ת������������
            m_pFromPoint = m_pScreenDisplay.DisplayTransformation.ToMapPoint((int)X, (int)Y);
           
            //��ʼ�����û����Ƶ�ͼ��                           
            if (m_frmMeasureResult.m_bSnapToFeature == true)   // ��׽Ҫ�ؽڵ�
            {
                IPoint pNearPnt = SnapPnt(m_pFromPoint, m_pMap);

                //�����ϴλ��ƵĽڵ�
                if (m_pSnapPoint != null)
                    if (m_pSnapPoint.IsEmpty == false)
                    {
                        m_pScreenDisplay.StartDrawing(m_pScreenDisplay.hDC, (short)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache);
                        m_pScreenDisplay.SetSymbol(m_pSnapPntSymbol);
                        m_pScreenDisplay.DrawPoint(m_pSnapPoint as IGeometry);
                        m_pScreenDisplay.FinishDrawing();
                        m_pSnapPoint.SetEmpty();
                        m_pSnapPoint = null;
                    }

                if (pNearPnt != null)
                    if (pNearPnt.IsEmpty == false)
                    {
                        //�ڵ�ͼ�ϻ����׽�Ľڵ�                        
                        m_pScreenDisplay.StartDrawing(m_pScreenDisplay.hDC, (short)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache);
                        m_pScreenDisplay.SetSymbol(m_pSnapPntSymbol = CreateSimMarkerSymbol());
                        m_pScreenDisplay.DrawPoint(pNearPnt as IGeometry);
                        m_pScreenDisplay.FinishDrawing();

                        m_pFromPoint.PutCoords(pNearPnt.X, pNearPnt.Y);
                        m_pSnapPoint = new PointClass();
                        m_pSnapPoint.PutCoords(pNearPnt.X, pNearPnt.Y);
                        pNearPnt.SetEmpty();
                        pNearPnt = null;
                    }
            }
            if (this.m_frmMeasureResult.m_CurMeasureType == 2 && this.m_meaAangleLineFb == null&&
                m_pNewLineFeedback==null&&m_pPlygonFeadback==null)//yjl20110816 add �����Ƕ�
            {

                m_meaAangleLineFb = new NewLineFeedbackClass();
                m_meaAangleLineFb.Display = m_pScreenDisplay;
                m_meaAangleLineFb.Start(m_pFromPoint);
                m_meaFromPoint = m_pFromPoint;
                   
             
            }

            if (this.m_frmMeasureResult.m_CurMeasureType == 0 && this.m_pPlygonFeadback == null && m_meaAangleLineFb == null)//yjl20110801 modify ��ֹ������;�ı�����
            {
                if (this.m_pNewLineFeedback == null)
                {
                    m_pNewLineFeedback = new NewLineFeedbackClass();
                    m_pNewLineFeedback.Display = m_pScreenDisplay;
                    m_frmMeasureResult.dblSegLength = 0;
                    m_frmMeasureResult.dblLength = 0;
					dlength = 0;
                    m_pNewLineFeedback.Start(m_pFromPoint);
                }
                else             // ���û�û��˫��ǰ��������
                {
                    m_pNewLineFeedback.AddPoint(m_pFromPoint);
                }
            }
            else if (this.m_frmMeasureResult.m_CurMeasureType == 1  && this.m_pNewLineFeedback == null&&m_meaAangleLineFb==null)//yjl20110816 modify ��ֹ������;�ı�����
            {
                if (this.m_pPlygonFeadback == null)
                {
                    m_pPlygonFeadback = new NewPolygonFeedbackClass();
                    m_pPlygonFeadback.Display = m_pScreenDisplay;
                    m_frmMeasureResult.dblSegLength = 0;
                    m_frmMeasureResult.dblLength = 0;
                    m_frmMeasureResult.dblArea = 0;
					dlength = 0;//xisheng 20110801
                    //darea = 0;
                    m_pPlygonFeadback.Start(m_pFromPoint);
                }
                else             // ���û�û��˫��ǰ��������
                    m_pPlygonFeadback.AddPoint(m_pFromPoint);

                //��¼��ʱ�����
                if (this.m_pPlygon == null)
                {
                    m_pPlygon = new ESRI.ArcGIS.Geometry.PolygonClass();
                }
                m_pPlygon.AddPoints(1, ref m_pFromPoint);
            }
            else if (this.m_pNewLineFeedback != null)//yjl20110801 add ��ֹ������;�ı�����
            {
                m_pNewLineFeedback.AddPoint(m_pFromPoint);

            }
            else if (this.m_pPlygonFeadback != null)//yjl20110801 add ��ֹ������;�ı�����
            {
                m_pPlygonFeadback.AddPoint(m_pFromPoint);
                //��¼��ʱ�����
                if (this.m_pPlygon == null)
                {
                    m_pPlygon = new ESRI.ArcGIS.Geometry.PolygonClass();
                }
                m_pPlygon.AddPoints(1, ref m_pFromPoint);

            }
            //m_frmMeasureResult.dblLength += m_frmMeasureResult.dblSegLength;
            //m_frmMeasureResult.dblSumLength += m_frmMeasureResult.dblSegLength;

            m_PntCol.AddPoints(1, ref m_pFromPoint);
        }

        //���û�ѡ�е�Ҫ�ؽ��в���
        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            if (m_frmMeasureResult == null)
            {
                return;
            }
            if (Button != 1) return;
        	if (m_pNewLineFeedback != null || m_pPlygonFeadback != null)
            {
               m_frmMeasureResult.dblLength += dlength;//20110801

               ////�ж��ǲ���Ҫ�����ܺ� xisheng 20111118
               //if (m_frmMeasureResult.m_bShowSum)
               //{
                   m_frmMeasureResult.dblSumLength += dlength;//20110801
               //}
                //**********************************end
               //m_frmMeasureResult.dblSumArea += darea;
                return;
            }

            

			double dblTempLength = 0;
            double dblTempArea = 0;
            //���������������
            if (this.m_frmMeasureResult == null)
            {
                m_frmMeasureResult = new frmMeasureResult(m_hookHelper.FocusMap.MapUnits,this);
                m_frmMeasureResult.Show();
                m_frmMeasureResult.FormClosed += new FormClosedEventHandler(FrmMeasureResult_FormClosed);
            }
            else if (this.m_frmMeasureResult.Visible == false)
            {
                m_frmMeasureResult = null;
                m_frmMeasureResult = new frmMeasureResult(m_hookHelper.FocusMap.MapUnits,this);
                m_frmMeasureResult.Show();
                m_frmMeasureResult.FormClosed += new FormClosedEventHandler(FrmMeasureResult_FormClosed);
            }

            if (this.m_frmMeasureResult.m_bIsFeatureMeasure == true && m_pNewLineFeedback == null && m_pPlygonFeadback == null)
            {
                //����Ļ����ת������������
                m_pFromPoint = m_pScreenDisplay.DisplayTransformation.ToMapPoint((int)X, (int)Y);
                //����ѡ��Ҫ�صļ�����״

                m_frmMeasureResult.dblLength = 0;
                m_frmMeasureResult.dblSegLength = 0;
                m_frmMeasureResult.dblArea = 0;
                IGeometry pNearGeo = FindNearFeatByPnt(m_pFromPoint, m_pMap, true);

                if (pNearGeo is IPolygon)
                {
                    IArea parea = pNearGeo as IArea;
                    GetAreaAndLength(pNearGeo, ref dblTempLength, ref dblTempArea, m_pMap.SpatialReference);

                    this.m_frmMeasureResult.dblArea = dblTempArea;
                    this.m_frmMeasureResult.dblSumArea += dblTempArea;

                    ISegmentCollection pSegCollection = pNearGeo as ISegmentCollection;

                    //for (int i = 0; i < pSegCollection.SegmentCount; i++)
                    //{
                    //    m_frmMeasureResult.dblLength += pSegCollection.get_Segment(i).Length;
                    //}
                    m_frmMeasureResult.dblLength += dblTempLength;

                    //�ж��ǲ���Ҫ�����ܺ� xisheng 20111118
                    if (m_frmMeasureResult.m_bShowSum)
                    {
                        this.m_frmMeasureResult.dblSumLength += dblTempLength;
                    }
                    //***********************************end
                    //�ڴ�����������
                    m_frmMeasureResult.ShowResult(2,true);//yjl20110816 modify �����˽ǶȲ�������
                }
                else if (pNearGeo is IPolyline)
                {
                    IPolyline pPlyline = pNearGeo as IPolyline;
                    GetAreaAndLength(pNearGeo, ref dblTempLength, ref dblTempArea, m_pMap.SpatialReference);

                    m_frmMeasureResult.dblLength = dblTempLength;
                    //�ж��ǲ���Ҫ�����ܺ� xisheng 20111118
                    if (m_frmMeasureResult.m_bShowSum)
                    {
                        this.m_frmMeasureResult.dblSumLength += m_frmMeasureResult.dblLength;
                    }
                    //***********************************end

                    //�ڴ�����������
                    m_frmMeasureResult.ShowResult(1,true);

                }
            }
        }

        //ͶӰ ���ͶӰ���
        private void GetAreaAndLength(IGeometry pGeo, ref double dblLength, ref double dblArea,ISpatialReference pOldSpatial)
        {
            dblLength = 0;
            dblArea = 0;

            IClone pClone = pGeo as IClone;
            IGeometry pNewGeo=pClone.Clone() as IGeometry;

            //��ͶӰ�任
            if (!(pOldSpatial is IProjectedCoordinateSystem))
            {
                pNewGeo.SpatialReference = pOldSpatial;
                //�������ϵ
                ISpatialReference pNewSpatial = SysCommon.Gis.ModGisPub.GetSpatialByX((pNewGeo.Envelope.XMin + pNewGeo.Envelope.XMax) / 2);
                if (pNewSpatial != null) pNewGeo.Project(pNewSpatial);

            }

            //����ֵ
            if (pNewGeo is IPolygon)
            {
                IPolygon pPolygon = pNewGeo as IPolygon;
                IArea parea = pNewGeo as IArea;

                dblLength = pPolygon.Length;
                dblArea = parea.Area;
            }
            else if (pNewGeo is IPolyline)
            {
                IPolyline pPolyline = pNewGeo as IPolyline;
                dblLength = pPolyline.Length;
            }
        }
        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            if (m_frmMeasureResult == null)
            {
                return;
            }
            // TODO:  Add MeasureTool.OnMouseMove implementation
            //�ƶ� Feedback ����ǰ���λ��     
            //if (m_pNewLineFeedback == null && m_pPlygonFeadback == null) return;

            m_pToPoint = m_pScreenDisplay.DisplayTransformation.ToMapPoint((int)X, (int)Y);

            if (m_frmMeasureResult.m_bSnapToFeature == true)   // ��׽Ҫ�ؽڵ�
            {
                IPoint pNearPnt = SnapPnt(m_pToPoint, m_pMap);

                //�����ϴλ��ƵĽڵ�
                if (m_pSnapPoint != null)
                    if (m_pSnapPoint.IsEmpty == false)
                    {
                        m_pScreenDisplay.StartDrawing(m_pScreenDisplay.hDC, (short)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache);
                        m_pScreenDisplay.SetSymbol(m_pSnapPntSymbol);
                        m_pScreenDisplay.DrawPoint(m_pSnapPoint as IGeometry);
                        m_pScreenDisplay.FinishDrawing();
                        m_pSnapPoint.SetEmpty();
                        m_pSnapPoint = null;
                    }

                if (pNearPnt != null)
                    if (pNearPnt.IsEmpty == false)
                    {
                        //�ڵ�ͼ�ϻ����׽�Ľڵ�                        
                        m_pScreenDisplay.StartDrawing(m_pScreenDisplay.hDC, (short)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache);
                        m_pScreenDisplay.SetSymbol(m_pSnapPntSymbol = CreateSimMarkerSymbol());
                        m_pScreenDisplay.DrawPoint(pNearPnt as IGeometry);
                        m_pScreenDisplay.FinishDrawing();

                        m_pToPoint.PutCoords(pNearPnt.X, pNearPnt.Y);

                        m_pSnapPoint = new PointClass();
                        m_pSnapPoint.PutCoords(pNearPnt.X, pNearPnt.Y);

                        pNearPnt.SetEmpty();
                        pNearPnt = null;
                    }
            }

            bool isMeasureLine = true;

            double dblTempLength=0;
            double dblTempArea=0;
            if (m_pNewLineFeedback != null)// && this.m_frmMeasureResult.m_CurMeasureType == 0)//yjl20110801 modify
            {
                m_pNewLineFeedback.MoveTo(m_pToPoint);
               
            }
            if (m_meaAangleLineFb != null)//yjl20110816 add �����Ƕ�
           {
               m_meaAangleLineFb.MoveTo(m_pToPoint);
               ILine meaAngle = new LineClass();
               meaAngle.FromPoint = m_meaFromPoint;
               meaAngle.ToPoint = m_pToPoint;
               double pi = 4 * Math.Atan(1);
               double ang = 180 * meaAngle.Angle / pi;
               m_frmMeasureResult.ang = ang;
               m_frmMeasureResult.ShowResult();


           }
            else if (m_pPlygonFeadback != null)// && this.m_frmMeasureResult.m_CurMeasureType == 1)//yjl20110801 modify
            {
                m_pPlygonFeadback.MoveTo(m_pToPoint);
                isMeasureLine = false;

                //������ʱ���
                m_pPlygon.AddPoints(1, ref m_pToPoint);

                //���ͶӰ������
                GetAreaAndLength(m_pPlygon as IGeometry,ref dblTempLength,ref dblTempArea,m_pMap.SpatialReference);
                m_frmMeasureResult.dblArea = dblTempArea;
                //darea = m_frmMeasureResult.dblArea;
                m_pPlygon.RemovePoints(m_pPlygon.PointCount - 1, 1);
            }


            //���㵱ǰ�߶εĳ���
            double deltaX;
            double deltaY;
            if (m_pToPoint != null && (m_pNewLineFeedback != null || m_pPlygonFeadback != null))
            {
                deltaX = m_pToPoint.X - m_pFromPoint.X;
                deltaY = m_pToPoint.Y - m_pFromPoint.Y;
                IPolyline pPolylineSeg = new PolylineClass();
                IPointCollection pPntCol = pPolylineSeg as IPointCollection;
                object obj = System.Type.Missing;
                pPntCol.AddPoint(m_pToPoint, ref obj,ref obj);
                pPntCol.AddPoint(m_pFromPoint,ref obj,ref obj);

                GetAreaAndLength(pPolylineSeg, ref dblTempLength, ref dblTempArea, m_pMap.SpatialReference);

                m_frmMeasureResult.dblSegLength = dblTempLength;
                dlength = m_frmMeasureResult.dblLength + dblTempLength;//��ֵ���߶γ��� xisheng 20110801
                
                //������
                m_frmMeasureResult.ShowResult(m_frmMeasureResult.dblSegLength, m_frmMeasureResult.dblLength + m_frmMeasureResult.dblSegLength, m_frmMeasureResult.dblSumLength + m_frmMeasureResult.dblSegLength,
                    m_frmMeasureResult.dblArea, m_frmMeasureResult.dblSumArea + m_frmMeasureResult.dblArea, isMeasureLine);
            }
        }

        public override void OnDblClick()
        {
            if (m_frmMeasureResult == null)
            {
                return;
            }
            //��ȡ��ǰ������״,�����Ȼ������ֵ��ӵ��ܺ���
            //   ˫��ֹͣfeedback
            if (m_pNewLineFeedback == null && m_pPlygonFeadback == null&&m_meaAangleLineFb==null) return;//yjl20110816 modify
			m_frmMeasureResult.Focus();

            //bool isMeasureLine = true;
            int meaType = 1;//yjl20110816 modify 1��2��3�Ƕ�
            double dblTempLength = 0;
            double dblTempArea = 0;
            if (m_pNewLineFeedback != null)// && this.m_frmMeasureResult.m_CurMeasureType == 0)//yjl20110801 modify
            {
                IPolyline pResultLine = m_pNewLineFeedback.Stop();
                m_pNewLineFeedback = null;
                this.m_pFromPoint = null;
                this.m_pToPoint = null;
            }
            else if (m_meaAangleLineFb != null)//yjl20110816 add �ǶȲ���
            {
                m_meaAangleLineFb.Stop();
                m_meaAangleLineFb = null;
                this.m_pFromPoint = null;
                this.m_pToPoint = null;
                meaType = 3;
 
            }
            else if (m_pPlygonFeadback != null)// && this.m_frmMeasureResult.m_CurMeasureType == 1)//yjl20110801 modify
            {
                IPolygon pPolygon = m_pPlygonFeadback.Stop();
                // ������õ������ܹ�����ʱ�������ж�       ZQ   20110713  modity
                if (pPolygon != null)
                {
                    //IArea pArea = pPolygon as IArea;
                    //���ͶӰ������
                    GetAreaAndLength(pPolygon as IGeometry, ref dblTempLength, ref dblTempArea, m_pMap.SpatialReference);
                   
                    //xisheng 20111118 �ж��Ƿ���ʾ�ܺ�*************************************************************
                    if (m_frmMeasureResult.m_bShowSum)
                    {
                        m_frmMeasureResult.dblArea = dblTempArea;
                        m_frmMeasureResult.dblSumArea += m_frmMeasureResult.dblArea;  //�����Ƶ���������ӵ��ܺ���
                    }
                    //xisheng 20111118 �ж��Ƿ���ʾ�ܺ�*********************************************************end
                }
                else
                {
                    m_frmMeasureResult.dblArea = 0;
                    m_frmMeasureResult.dblSumArea += 0;
                }
                // end
                //isMeasureLine = false;
                meaType = 2;
                m_pPlygonFeadback = null;
                m_pPlygon = null;
                this.m_pFromPoint = null;
                this.m_pToPoint = null;
            }

            m_PntCol = new PolylineClass();
            //m_frmMeasureResult.dblLength = 0;
            //m_frmMeasureResult.dblSegLength = 0;
            //m_frmMeasureResult.dblArea = 0;
            //�ڴ�����������
            m_frmMeasureResult.ShowResult(meaType,true);
        }

        public override void Refresh(int hDC)
        {
            if (m_pNewLineFeedback != null)
            {
                m_pNewLineFeedback.Refresh(hDC);
            }

            if (m_pPlygonFeadback != null)
            {
                m_pPlygonFeadback.Refresh(hDC);
            }
        }

        //���߲�����ʱ�ͷŴ���ȱ���
        public override bool Deactivate()
        {
            //this.m_frmMeasureResult.Close();
            //this.m_frmMeasureResult = null;
            return true;
        }
        #endregion

        //��ѯ��ͼ��ĳ�������ͼ��Ҫ��
        private IGeometry FindNearFeatByPnt(IPoint pPnt, IMap pMap, bool bNoPnt)
        {
            if (pPnt == null || pMap == null) return null;

            //���㻺��
            ITopologicalOperator pBuffPlygon = pPnt as ITopologicalOperator;
            IGeometry pEnv = pBuffPlygon.Buffer(10);
            if (pEnv == null) return null;

            ILayer pLyr;
            IFeatureLayer2 pFeatLyr;
            //ISpatialFilter pSpaFilter;
            //IFeatureCursor pFeatCursor;
            IFeature pFeature = null;
            //int sGeoType;

            for (int i = 0; i < pMap.LayerCount; i++)
            {
                pLyr = pMap.get_Layer(i);
                if (pLyr.Visible == false)
                    continue;
                //�жϵ�Ϊͼ��ʱ����ȡͼ���е�ͼ��      ZQ   201107113     add
                if (pLyr is IGroupLayer)
                {
                    ICompositeLayer comlayer = pLyr as ICompositeLayer;
                     for (int j = 0; j < comlayer.Count; j++)
                     {
                         ILayer pTmpLayer = comlayer.get_Layer(j);
                         if(pTmpLayer.Visible ==false)
                             continue;
                         pFeatLyr = pTmpLayer as IFeatureLayer2;
                         if (pFeatLyr == null)
                              continue;
                         pFeature = getBufferFeature(pFeatLyr, pEnv, bNoPnt);
                         if (pFeature == null)
                             continue;
                         else
                             break;
                     }
                }
               //end
                else
                {
                    pFeatLyr = pLyr as IFeatureLayer2;
                    if (pFeatLyr == null)
                        continue;
                    pFeature = getBufferFeature(pFeatLyr, pEnv, bNoPnt);
                }
                if (pFeature == null)
                    continue;
                else
                    break;
               
            }

            if (pFeature == null)
                return null;
            else
                return pFeature.ShapeCopy;
        }
        /// <summary>
        ///��ȡ ��ͼ��ĳ�������ͼ��Ҫ�� ZQ    20110713    modity
        /// </summary>
        /// <param name="pFeatureLayer2"></param>
        /// <param name="pEnv"></param>
        /// <param name="bNoPnt"></param>
        /// <returns></returns>
        private IFeature getBufferFeature(IFeatureLayer2 pFeatureLayer2, IGeometry pEnv, bool bNoPnt)
        {
            ISpatialFilter pSpaFilter;
            IFeatureCursor pFeatCursor;
            IFeature pFeature = null;
            int sGeoType;

            pSpaFilter = new SpatialFilterClass();
            sGeoType = (int)pFeatureLayer2.ShapeType;
            switch (sGeoType)
            {
                case (int)esriGeometryType.esriGeometryPoint:
                    if (bNoPnt == true)
                        return pFeature =null;
                    pSpaFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    break;
                case (int)esriGeometryType.esriGeometryPolygon:
                    if (bNoPnt == true)
                        pSpaFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    else
                        pSpaFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelOverlaps;
                    break;
                case (int)esriGeometryType.esriGeometryPolyline:
                    pSpaFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
                default:
                    return pFeature =null;
            }

            pSpaFilter.Geometry = pEnv;
            pSpaFilter.GeometryField = pFeatureLayer2.FeatureClass.ShapeFieldName;
            pFeatCursor = pFeatureLayer2.Search(pSpaFilter, false);
            if (pFeatCursor == null)
                return pFeature = null; 
            pFeature = pFeatCursor.NextFeature();
            return pFeature;


        }
        //�жϻ���Ҫ�صĸ���yjl629
        public int CacheCount
        {
            get
            {
                IFeatureCache2 pFeatureCache;      //���嵱ǰ�༭��Χ��Ҫ��

                IPoint pPnt = null;

                IPoint pPoint = new PointClass();
                pPoint.X = (m_pActiveView.Extent.XMax + m_pActiveView.Extent.XMin) / 2;
                pPoint.Y = (m_pActiveView.Extent.YMax + m_pActiveView.Extent.YMin) / 2;

                double fDis = (m_pActiveView.Extent.XMax - m_pActiveView.Extent.XMin) / 2;

                IEnvelope pClipEnvelope = new EnvelopeClass();
                pClipEnvelope.XMax = pPoint.X + 3 * fDis;
                pClipEnvelope.YMax = pPoint.Y + 3 * fDis;
                pClipEnvelope.XMax = pPoint.X - 3 * fDis;
                pClipEnvelope.YMax = pPoint.Y - 3 * fDis;

                pFeatureCache = new FeatureCacheClass();
                pFeatureCache.Initialize(pPoint, 3 * fDis);
                IMap PTempMap = new MapClass();
                ILayer pLyr;
                for (int i = 0; i < m_pMap.LayerCount; i++)
                {
                    pLyr = m_pMap.get_Layer(i);
                    if (pLyr.Visible == true)
                        PTempMap.AddLayer(pLyr);
                }

                ESRI.ArcGIS.esriSystem.UID pUID = new ESRI.ArcGIS.esriSystem.UID();
                pUID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}" as object; //UID for IGeoFeatureLayer
                //'���ͼ����Ϊ�������Ӳ����Ļ�����Զ�������
                IEnumLayer pEnumLayer;
                if (PTempMap.LayerCount > 0)
                {
                    pEnumLayer = PTempMap.get_Layers(pUID, true);
                    pFeatureCache.AddLayers(pEnumLayer, pClipEnvelope);
                }
                return pFeatureCache.Count;
            }
        }
        //��׽�ڵ�
        private IPoint SnapPnt(IPoint pSnapPnt, IMap pMap)
        {
            double fTolerance = ConvertPixelsToMapUnits(m_pActiveView, 7);

            IFeature pFeature;
            IFeatureCache2 pFeatureCache;      //���嵱ǰ�༭��Χ��Ҫ��
            IGeometry pFeatShape;
            IPoint pPnt = null;

            IPoint pPoint = new PointClass();
            pPoint.X = (m_pActiveView.Extent.XMax + m_pActiveView.Extent.XMin) / 2;
            pPoint.Y = (m_pActiveView.Extent.YMax + m_pActiveView.Extent.YMin) / 2;

            double fDis = (m_pActiveView.Extent.XMax - m_pActiveView.Extent.XMin) / 2;

            IEnvelope pClipEnvelope = new EnvelopeClass();
            pClipEnvelope.XMax = pPoint.X +  fDis;
            pClipEnvelope.YMax = pPoint.Y +  fDis;
            pClipEnvelope.XMax = pPoint.X -  fDis;
            pClipEnvelope.YMax = pPoint.Y -  fDis;

            pFeatureCache = new FeatureCacheClass();
            pFeatureCache.Initialize(pPoint,  fDis);

            IMap PTempMap = new MapClass();
            ILayer pLyr;
            for (int i = 0; i < m_pMap.LayerCount; i++)
            {
                pLyr = m_pMap.get_Layer(i);
                if (pLyr.Visible == true)
                    PTempMap.AddLayer(pLyr);
            }

            ESRI.ArcGIS.esriSystem.UID pUID = new ESRI.ArcGIS.esriSystem.UID();
            pUID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}" as object; //UID for IGeoFeatureLayer
            //'���ͼ����Ϊ�������Ӳ����Ļ�����Զ�������
            IEnumLayer pEnumLayer;
            if (PTempMap.LayerCount > 0)
            {
                pEnumLayer = PTempMap.get_Layers(pUID, true);
                pFeatureCache.AddLayers(pEnumLayer, pClipEnvelope);
            }

            for (int i = 0; i < pFeatureCache.Count; i++)
            {
                pFeature = pFeatureCache.get_Feature(i);
                if (pFeature == null)
                    continue;
                pFeatShape = pFeature.Shape;

                if (pFeatShape == null)
                    continue;
                if (pFeatShape.IsEmpty == true)
                    continue;

                pPnt = GetSnapPoint(m_pActiveView, pFeatShape, pSnapPnt, fTolerance);
                if (pPnt != null && pPnt.IsEmpty == false)
                    break;
            }
            return pPnt;
        }

        //�ڵ㲶׽ʱ���ڵ��ռ�
        private IPoint GetSnapPoint(IActiveView pActiveView, IGeometry pFeatureShape, IPoint pMousePoint, double fTolerance)
        {
            IProximityOperator pProximityOperator = pMousePoint as IProximityOperator;
            int nSegmentIndex = 0;
            double fHitDist = 0;
            int nPart = 0;

            IPoint[] pResPoint = new IPoint[2];

            for (int i = 0; i < 2; i++)
            {
                pResPoint[i] = new PointClass();
            }

            IHitTest pHitTest = pFeatureShape as IHitTest;
            bool bRigthSize = false;
            pHitTest.HitTest(pMousePoint, fTolerance, esriGeometryHitPartType.esriGeometryPartVertex, pResPoint[0], ref  fHitDist, ref  nPart, ref  nSegmentIndex, ref  bRigthSize);
            pHitTest.HitTest(pMousePoint, fTolerance, esriGeometryHitPartType.esriGeometryPartEndpoint, pResPoint[1], ref  fHitDist, ref  nPart, ref  nSegmentIndex, ref bRigthSize);


            IPoint pClosedPoint = new PointClass();

            if (pResPoint[0] != null)
            {
                if (pResPoint[0].IsEmpty == false)
                {
                    pClosedPoint.X = pResPoint[0].X;
                    pClosedPoint.Y = pResPoint[0].Y;

                }
                pResPoint[0] = null;
            }

            if (pResPoint[1] != null)
            {
                if (pResPoint[1].IsEmpty == false)
                {
                    if (pProximityOperator.ReturnDistance(pClosedPoint) > pProximityOperator.ReturnDistance(pResPoint[1]))
                    {
                        pClosedPoint.X = pResPoint[1].X;
                        pClosedPoint.Y = pResPoint[1].Y;
                    }
                }
                pResPoint[1] = null;
            }

            return pClosedPoint;
        }

        public double ConvertPixelsToMapUnits(IActiveView pActiveView, double nPixelUnits)
        {
            tagRECT pDeviceRECT;
            pDeviceRECT = pActiveView.ScreenDisplay.DisplayTransformation.get_DeviceFrame();

            int nPixelExtent = pDeviceRECT.right - pDeviceRECT.left;

            double nRealWorldDisplayExtent;
            nRealWorldDisplayExtent = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;

            double nSizeOfOnePixel = nRealWorldDisplayExtent / nPixelExtent;
            return nPixelUnits * nSizeOfOnePixel;
        }


        //����һ����״�������ڻ��Ʋ�׽���Ľڵ�
        private ISymbol CreateSimMarkerSymbol()
        {
            ISimpleMarkerSymbol pMarSymbol = new SimpleMarkerSymbolClass();
            IRgbColor pRGB = new RgbColorClass();
            pRGB.NullColor = true;
            pMarSymbol.Color = pRGB as IColor;
            pMarSymbol.Style = ESRI.ArcGIS.Display.esriSimpleMarkerStyle.esriSMSSquare;

            ISymbol pSymbol = pMarSymbol as ISymbol;
            //�������ʽ���ƣ�������ǰ���ķ���
            pSymbol.ROP2 = ESRI.ArcGIS.Display.esriRasterOpCode.esriROPXOrPen;

            pRGB = null;
            pRGB = new RgbColorClass();
            pRGB.Red = 0; pRGB.Green = 255; pRGB.Blue = 255;
            pRGB.Transparency = 255;
            pMarSymbol.Outline = true;
            pMarSymbol.OutlineColor = pRGB as IColor;
            return pSymbol;
        }
    }

    public class ControlsSelFeature : BaseTool
    {
        private IHookHelper m_hookHelper;
        private IMapControlDefault m_MapControl;
        //����ѡ���ͼ��
        private List<ILayer> layerList = new List<ILayer>();
        public List<ILayer> LayerList
        {
            get
            {
                return this.layerList;
            }
            set
            {
                this.layerList = value;
            }
        }

        //��ķ���
        public ControlsSelFeature()
        {
            base.m_category = "GeoCommon";
            base.m_caption = "SelFeature";
            base.m_message = "ѡ��Χ";
            base.m_toolTip = "ѡ��Χ";
            base.m_name = base.m_category + "_" + base.m_caption;
            try
            {

                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), "Resources.Select.cur");
            }
            catch
            {

            }
        }

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;
            m_MapControl = hook as IMapControlDefault;
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            //֧�ֿ�ݼ�
            m_MapControl.KeyIntercept = 1;  //esriKeyInterceptArrowKeys
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            if (Button != 1) return;

            //���õ�ѡ���ݲ�
            ISelectionEnvironment pSelectEnv = new SelectionEnvironmentClass();
            double Length = ConvertPixelsToMapUnits(m_hookHelper.ActiveView, pSelectEnv.SearchTolerance);

            IPoint pPoint = m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
            IGeometry pGeometry = pPoint as IGeometry;
            ITopologicalOperator pTopo = pGeometry as ITopologicalOperator;
            IGeometry pBuffer = pTopo.Buffer(Length);

            //�����ܱ���ཻ����ᱻѡȡ
            pGeometry = m_MapControl.TrackRectangle() as IGeometry;
            bool bjustone = true;
            if (pGeometry != null)
            {
                if (pGeometry.IsEmpty)
                {
                    pGeometry = pBuffer;
                }
                else
                {
                    bjustone = false;
                }
            }
            else
            {
                pGeometry = pBuffer;
            }

            UID pUID = new UIDClass();
            pUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";   //UID for IFeatureLayer
            IEnumLayer pEnumLayer = m_MapControl.Map.get_Layers(pUID, true);
            pEnumLayer.Reset();
            ILayer pLayer = pEnumLayer.Next();
            while (pLayer != null)
            {
                if (pLayer.Visible == false)
                {
                    pLayer = pEnumLayer.Next();
                    continue;
                }
                IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                if (pFeatureLayer.Selectable == false)
                {
                    pLayer = pEnumLayer.Next();
                    continue;
                }
                for (int j = 0; j < layerList.Count; j++)
                {
                    if (pLayer==layerList[j])
                    {
                        //�����ͼ����Ҫѡ���ͼ�㣬��ѡ�и�ͼ��
                        GetSelctionSet(pFeatureLayer, pGeometry, bjustone, Shift);
                        break;
                    }
                }
                pLayer = pEnumLayer.Next();
            }

            //����Mapѡ�����仯�¼�
            ISelectionEvents pSelectionEvents = m_hookHelper.FocusMap as ISelectionEvents;
            pSelectionEvents.SelectionChanged();
            //ˢ��
            m_hookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, m_hookHelper.ActiveView.Extent);
        }
        //������ֵת��Ϊ��ͼ��λ
        private double ConvertPixelsToMapUnits(IActiveView pActiveView, int pixelUnits)
        {
            tagRECT deviceRECT = pActiveView.ScreenDisplay.DisplayTransformation.get_DeviceFrame();
            int pixelExtent = deviceRECT.right - deviceRECT.left;
            double realWorldDisplayExtent = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;
            double sizeOfOnePixel = realWorldDisplayExtent / pixelExtent;
            return pixelUnits * sizeOfOnePixel;
        }
        private void GetSelctionSet(IFeatureLayer pFeatureLayer, IGeometry pGeometry, bool bjustone, int Shift)
        {
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            if (Shift == 2) Shift = 0;//�������ctrl����������0
            switch (Shift)
            {
                case 1:   //����ѡ������
                    GetSelctionSet(pFeatureLayer, pGeometry, pFeatureClass, esriSelectionResultEnum.esriSelectionResultAdd, bjustone);
                    break;
                case 4:   //����ѡ������
                    GetSelctionSet(pFeatureLayer, pGeometry, pFeatureClass, esriSelectionResultEnum.esriSelectionResultSubtract, bjustone);
                    break;
                case 2:
                    GetSelctionSet(pFeatureLayer, pGeometry, pFeatureClass, esriSelectionResultEnum.esriSelectionResultXOR, bjustone);
                    break;
                default:   //�½�ѡ������
                    GetSelctionSet(pFeatureLayer, pGeometry, pFeatureClass, esriSelectionResultEnum.esriSelectionResultNew, bjustone);
                    break;
            }
        }
        //ѡ��ΧҪ��
        private void GetSelctionSet(IFeatureLayer pFeatureLayer, IGeometry pGeometry, IFeatureClass pFeatureClass, esriSelectionResultEnum pselecttype, bool bjustone)
        {
            ISpatialFilter pSpatialFilter = new SpatialFilterClass();
            pSpatialFilter.Geometry = pGeometry;
            pSpatialFilter.GeometryField = pFeatureClass.ShapeFieldName;

            switch (pFeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                    break;
                default:
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    break;
            }

            IFeatureSelection pFeaSelection = pFeatureLayer as IFeatureSelection;
            pFeaSelection.SelectFeatures(pSpatialFilter as IQueryFilter, pselecttype, bjustone);
            pFeaSelection.SelectionChanged();
        }

    }

    /// <summary>
    /// �Զ����ͼ�� DynamicLayerBase serves as a BaseClass for DynamicLayers.
    /// </summary>
    /// <remarks>In order to implemet a DynamicLayer using this base-class, the user has to
    /// inherit this base class and override method DrawDynamicLayer().</remarks>
    public abstract class DynamicLayerBase : Control, ILayer, IDynamicLayer, IGeoDataset, IPersistVariant, ILayerGeneralProperties, ILayerExtensions, IDisposable
    {
        #region Class members
        /// <summary>
        /// Keep the layer's extent. Returned by the ILayer::Extent property
        /// </summary>
        /// <remarks>The extent should be spatial-referenced to the DateFrame's spatial reference.
        /// </remarks>
        protected IEnvelope m_extent = null;

        /// <summary>
        /// Store the layer's underlying data spatial reference. Returned by IGeoDataset::SpatialReference.
        /// </summary>
        /// <remarks>This spatial reference should not be reprojected. In your inheriting 
        /// class you will need to have another parameter that will keep the DataFrame's spatial reference
        /// that would use to reproject the geometries and the extent of the layer.</remarks>
        protected ISpatialReference m_spatialRef = null;

        /// <summary>
        /// Layer's name. Returned by ILayer::Name property
        /// </summary>
        protected string m_sName;

        /// <summary>
        /// Flag which determines whether the layers is visible. Returned by ILayer::Visible
        /// </summary>
        /// <remarks>You should use this member in your inherited class in the Draw method.</remarks>
        protected bool m_visible;

        /// <summary>
        /// determines whether the layers is cached
        /// </summary>
        protected bool m_IsCached;

        /// <summary>
        /// Flag thich determine whether the layer is valid (connected to its data source, has valid information etc.).
        /// Returned by ILAyer::Valid.
        /// </summary>
        /// <remarks>You can use this flag to determine for example whether the layer can be available or not.</remarks>
        protected bool m_bValid = true;

        /// <summary>
        /// Keep the maximum scale value at which the layer will display
        /// </summary>
        protected double m_MaximumScale;

        /// <summary>
        /// Keep the minimum scale value at which the layer will display
        /// </summary>
        protected double m_MinimumScale;

        /// <summary>
        /// determines whether the layers is supposed to show its MapTips
        /// </summary>
        protected bool m_ShowTips;

        /// <summary>
        /// the layer dirty flag which determine whether its display list need to be recreate
        /// </summary>
        protected bool m_bIsImmediateDirty = false;

        /// <summary>
        /// the layer dirty flag which determine whether its display list need to be recreate
        /// </summary>
        protected bool m_bIsCompiledDirty = false;

        /// <summary>
        /// The rate in which the DynamicDisplay recompile its display lists
        /// </summary>
        protected int m_nRecompileRate = -1;

        /// <summary>
        /// The layer's UID
        /// </summary>
        protected UID m_uid;

        /// <summary>
        /// An arraylist to store the layer's extensions.
        /// </summary>
        protected ArrayList m_extensions = null;
        #endregion

        #region class constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DynamicLayerBase()
        {
            m_sName = string.Empty;
            m_visible = true;
            m_IsCached = false;
            m_MaximumScale = 0;
            m_MinimumScale = 0;

            m_extent = new EnvelopeClass();
            m_extent.SetEmpty();


            m_uid = new UIDClass();

            m_extensions = new ArrayList();

            //make sure tha the control got created and that it has a valid handle
            this.CreateHandle();
            this.CreateControl();
        }
        #endregion

        #region IGeoDataset Members
        virtual public IEnvelope Extent
        {
            get
            {
                return m_extent;
            }
        }

        /// <summary>
        /// The spatial reference of the underlying data.
        /// </summary>
        /// <remarks>The property must return the underlying data spatial reference and
        /// must not reporoject it into the layer's spatial reference </remarks>

        virtual public ISpatialReference SpatialReference
        {
            get
            {
                return m_spatialRef;
            }
        }

        #endregion

        #region IPersistVariant Members

        /// <summary>
        /// The ID of the object.
        /// </summary>
        virtual public UID ID
        {
            get
            {
                // TODO:  Add clsCustomLayer.ID getter implementation
                return null;
            }
        }

        /// <summary>
        /// Loads the object properties from the stream.
        /// </summary>
        /// <param name="Stream"></param>
        /// <remarks>The Load method must read the data from the stream in the same order the data was 
        /// written to the stream in the Save method. 
        /// Streams are sequential; you mut ensure that your data is saved and loaded in the correct order, 
        /// so that the correct data is written to the correct member.
        /// </remarks>
        virtual public void Load(IVariantStream Stream)
        {
            m_extensions = (ArrayList)Stream.Read();
        }

        /// <summary>
        /// Saves the object properties to the stream.
        /// </summary>
        /// <param name="Stream"></param>
        virtual public void Save(IVariantStream Stream)
        {
            Stream.Write(m_extensions);
        }

        #endregion

        #region ILayer Members

        #region Properties

        /// <summary>
        /// Indicates if the layer shows map tips.
        /// </summary>
        /// <remarks>Indicates whether or not map tips are shown for the layer. 
        /// If set to True, then map tips will be shown for the layer. 
        /// You can determine the text that will be shown via TipText. 
        ///</remarks>
        virtual public bool ShowTips
        {
            get
            {
                return m_ShowTips;
            }
            set
            {
                m_ShowTips = value;
            }
        }

        /// <summary>
        /// The default area of interest for the layer. Returns the spatial-referenced extent of the layer.
        /// </summary>
        virtual public IEnvelope AreaOfInterest
        {
            get
            {
                return m_extent;
            }
        }

        /// <summary>
        /// Indicates if the layer is currently visible.
        /// </summary>
        virtual new public bool Visible
        {
            get
            {
                return m_visible;
            }
            set
            {
                m_visible = value;
            }
        }


        /// <summary>
        /// Indicates if the layer needs its own display cache.
        /// </summary>
        /// <remarks>This property indicates whether or not the layer requires its own display cache. 
        /// If this property is True, then the Map will use a separate display cache for the layer so 
        /// that it can be refreshed indpendently of other layers.</remarks>
        virtual public bool Cached
        {
            get
            {
                return m_IsCached;
            }
            set
            {
                m_IsCached = value;
            }
        }

        /// <summary>
        /// Minimum scale (representative fraction) at which the layer will display.
        /// </summary>
        /// <remarks>Specifies the minimum scale at which the layer will be displayed. 
        /// This means that if you zoom out beyond this scale, the layer will not display. 
        /// For example, specify 1000 to have the layer not display when zoomed out beyond 1:1000.</remarks>
        virtual public double MinimumScale
        {
            get
            {
                return m_MinimumScale;
            }
            set
            {
                m_MinimumScale = value;
            }
        }

        /// <summary>
        /// Indicates if the layer is currently valid.
        /// </summary>
        /// <remarks>The valid property indicates if the layer is currently valid.
        /// Layers that reference feature classes are valid when they hold a reference to a valid feature class.
        /// The property does not however validate the integrity of the feature classes reference to the database.
        /// Therefore, in rare situations if a datasource is removed after a layer is initialized, 
        /// the layer will report itself as valid but query attempts to the data source will error due to the lack 
        /// of underlying data.</remarks>
        virtual public bool Valid
        {
            get
            {
                return m_bValid;
            }
        }

        /// <summary>
        /// The Layer name.
        /// </summary>
        virtual new public string Name
        {
            get
            {
                return m_sName;
            }
            set
            {
                m_sName = value;
            }
        }

        /// <summary>
        /// Maximum scale (representative fraction) at which the layer will display.
        /// </summary>
        /// <remarks>Specifies the maximum scale at which the layer will be displayed. 
        /// This means that if you zoom in beyond this scale, the layer will not display. 
        /// For example, specify 500 to have the layer not display when zoomed in beyond 1:500.</remarks>
        virtual public double MaximumScale
        {
            get
            {
                return m_MaximumScale;
            }
            set
            {
                m_MaximumScale = value;
            }
        }

        /// <summary>
        /// Supported draw phases.
        /// </summary>
        /// <remarks>Indicates the draw phases supported by the layer (esriDPGeography, esriDPAnnotation, 
        /// esriDPSelection, or any combination of the three). 
        /// The supported draw phases are defined by esriDrawPhase. 
        /// When multiple draw phases are supported, the sum of the constants is used. 
        /// For example, if SupportedDrawPhases = 3 then the layer supports drawing in the geography and annotation phases.</remarks>
        public int SupportedDrawPhases
        {
            get
            {
                return (int)esriDrawPhase.esriDPGeography;
            }
        }

        /// <summary>
        /// Spatial reference for the layer.
        /// </summary>
        ///<remarks>This property is only used for map display, setting this property does not 
        ///change the spatial reference of the layer's underlying data. 
        ///The ArcGIS framework uses this property to pass the spatial reference from the map 
        ///to the layer in order to support on-the-fly projection.</remarks> 
        ISpatialReference ESRI.ArcGIS.Carto.ILayer.SpatialReference
        {
            set
            {
                m_spatialRef = value;
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Map tip text at the specified location. 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Tolerance"></param>
        /// <returns>The text string that gets displayed as a map tip if ShowTips = true.</returns>
        virtual public string get_TipText(double X, double Y, double Tolerance)
        {
            return null;
        }

        /// <summary>
        /// Draws the layer to the specified display for the given draw phase.
        /// </summary>
        /// <param name="drawPhase"></param>
        /// <param name="Display"></param>
        /// <param name="trackCancel"></param>
        /// <remarks>This method draws the layer to the Display for the specified DrawPhase. 
        /// Use the TrackCancel object to allow the drawing of the layer to be interrupted by the user.
        /// In order to implement you inheriting class, you must override this method</remarks>
        public virtual void Draw(esriDrawPhase drawPhase, IDisplay Display, ITrackCancel trackCancel)
        {
            return;
        }
        #endregion
        #endregion

        #region IDynamicLayer Members

        /// <summary>
        /// The dynamic layer draw method
        /// </summary>
        /// <param name="DynamicDrawPhase">the current drawphase of the drawing</param>
        /// <param name="Display">the ActiveView's display</param>
        /// <param name="DynamicDisplay">the ActiveView's dynamic display</param>
        /// <remarks>This method is set as abstract, which means that the inherited class must override it</remarks>
        public abstract void DrawDynamicLayer(esriDynamicDrawPhase DynamicDrawPhase, IDisplay Display, IDynamicDisplay DynamicDisplay);

        /// <summary>
        /// get the dirty flag of the layer
        /// </summary>
        /// <param name="DynamicDrawPhase"></param>
        /// <returns></returns>
        public virtual bool get_DynamicLayerDirty(esriDynamicDrawPhase DynamicDrawPhase)
        {
            switch (DynamicDrawPhase)
            {
                case esriDynamicDrawPhase.esriDDPCompiled:
                    return m_bIsCompiledDirty;
                case esriDynamicDrawPhase.esriDDPImmediate:
                    return m_bIsImmediateDirty;
            }

            return false;
        }

        /// <summary>
        /// set the dirty flag of the layer
        /// </summary>
        /// <param name="DynamicDrawPhase"></param>
        /// <param name="Dirty"></param>
        public virtual void set_DynamicLayerDirty(esriDynamicDrawPhase DynamicDrawPhase, bool Dirty)
        {
            switch (DynamicDrawPhase)
            {
                case esriDynamicDrawPhase.esriDDPCompiled:
                    m_bIsCompiledDirty = Dirty;
                    break;
                case esriDynamicDrawPhase.esriDDPImmediate:
                    m_bIsImmediateDirty = Dirty;
                    break;
            }
        }

        /// <summary>
        /// This is the rate in which the DynamicDisplay builds the Display-Lists of the 'compiled'
        /// DrawPhase
        /// </summary>
        public virtual int DynamicRecompileRate
        {
            get { return m_nRecompileRate; }
        }

        #endregion

        #region ILayerGeneralProperties Members

        /// <summary>
        /// Last maximum scale setting used by layer.
        /// </summary>
        virtual public double LastMaximumScale
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Last minimum scale setting used by layer.
        /// </summary>
        virtual public double LastMinimumScale
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Description for the layer.
        /// </summary>
        virtual public string LayerDescription
        {
            get
            {
                return null;
            }
            set
            {

            }
        }
        #endregion

        #region ILayerExtensions Members

        /// <summary>
        /// Removes the specified extension.
        /// </summary>
        /// <param name="Index"></param>
        public virtual void RemoveExtension(int Index)
        {
            if (Index < 0 || Index > m_extensions.Count - 1)
                return;

            m_extensions.RemoveAt(Index);
        }

        /// <summary>
        /// Number of extensions.
        /// </summary>
        public virtual int ExtensionCount
        {
            get
            {
                return m_extensions.Count;
            }
        }

        /// <summary>
        /// Adds a new extension.
        /// </summary>
        /// <param name="ext"></param>
        public virtual void AddExtension(object ext)
        {
            if (null == ext)
                return;

            m_extensions.Add(ext);
        }

        /// <summary>
        /// The extension at the specified index. 
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public virtual object get_Extension(int Index)
        {
            if (Index < 0 || Index > m_extensions.Count - 1)
                return null;

            return m_extensions[Index];
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose the layer
        /// </summary>
        public new void Dispose()
        {
            m_extent = null;
            m_spatialRef = null;

            base.Dispose();
        }

        #endregion

    }
}
