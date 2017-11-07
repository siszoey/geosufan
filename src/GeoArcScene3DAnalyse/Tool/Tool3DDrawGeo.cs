using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Carto;
using System.Windows.Forms;
//������ά����    ����   20110628
namespace GeoArcScene3DAnalyse
{
    public delegate void myEventHandler(bool BeginDraw);//ί���¼�
    /// <summary>
    /// Summary description for Tool3DDrawGeo.
    /// </summary>
    [Guid("5817a215-7bba-447c-acdb-2a8297d2f7ed")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("GeoArcScene3DAnalyse.Tool.Tool3DDrawGeo")]
    public sealed class Tool3DDrawGeo : BaseTool
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            SxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            SxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion
        //public event myEventHandler BeginDrawed;//�����¼�
        public event myEventHandler EndDtrawd;//��������¼�
        private ISceneHookHelper m_sceneHookHelper = null;
        //���ƶ���㼯��
        private ESRI.ArcGIS.Geometry.IPointCollection m_pPointColl;
        //�������ɵļ��϶���
        public IGeometry m_Geometry;
        private const string sPolyOutlineName = "_POLYOUTLINE_";
        //���ƶ�������� Ŀǰֻ���ṩ�㡢�ߡ���Ļ���
        ESRI.ArcGIS.Geometry.esriGeometryType m_DrawType;
        
        public ESRI.ArcGIS.Geometry.esriGeometryType GeometryType
        {
            set
            {
                m_DrawType = value;
            }
        }
        //������άҪ�����ڵĻ�׼��
        ISurface m_psurface = null;
        public ISurface pSurface
        {
            set
            {
                m_psurface = value;
            }
        }

       

        public Tool3DDrawGeo()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = ""; //localizable text 
            base.m_caption = "";  //localizable text 
            base.m_message = "������ά����";  //localizable text
            base.m_toolTip = "3D Developer Samples";  //localizable text
            base.m_name = "";   //unique id, non-localizable (e.g. "MyCategory_MyTool")
            try
            {
                //
                // TODO: change resource name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            try
            {
                m_sceneHookHelper = new SceneHookHelperClass();
                m_sceneHookHelper.Hook = hook;
                if (m_sceneHookHelper.ActiveViewer == null)
                {
                    m_sceneHookHelper = null;
                }
            }
            catch
            {
                m_sceneHookHelper = null;
            }

            if (m_sceneHookHelper == null)
                base.m_enabled = false;
            else
                base.m_enabled = true;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add Tool3DDrawGeo.OnClick implementation
        }
        /// <summary>
        /// ��ȡ�����ͼ�ϵĵ�
        /// </summary>
        /// <param name="Button"></param>
        /// <param name="Shift"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            try
            {
                ESRI.ArcGIS.Geometry.IPoint pMapPoint= new ESRI.ArcGIS.Geometry.PointClass();;
                ISceneGraph pSceneGraph = m_sceneHookHelper.SceneGraph;
                object pOwner;
                object pObject;
                object before = Type.Missing;
                object after = Type.Missing;
                object StepSize =Type.Missing;
                IDisplay3D pDisplay;
                pSceneGraph.Locate(pSceneGraph.ActiveViewer, X, Y, esriScenePickMode.esriScenePickGeography, true, out pMapPoint, out  pOwner, out pObject);//��ȡ�������λ�ò�ת��Ϊ��������
                if (pMapPoint == null)
                {
                    return;
                }
                pMapPoint.Z = pMapPoint.Z / m_sceneHookHelper.Scene.ExaggerationFactor;
                pMapPoint.Z = m_psurface.GetElevation(pMapPoint);
                pMapPoint.SpatialReference = pSceneGraph.Scene.SpatialReference;
                pDisplay = m_sceneHookHelper.SceneGraph as IDisplay3D;
                pDisplay.FlashLocation(pMapPoint);//��˸��ʾ�������λ��
                IGeometry pGeom = null;
                Cls3DMarkDraw.DeleteAllElementsWithName(m_sceneHookHelper.Scene, sPolyOutlineName);
                //���ݻ��ƶ������͵Ĳ�ͬ���岻ͬ������
                switch(m_DrawType.ToString())
                {
                    case "esriGeometryPoint":
                        m_Geometry = pMapPoint;
                        break;
                    case "esriGeometryLine":
                        if(m_pPointColl ==null)
                        {
                            m_pPointColl = new PolylineClass();
                            pGeom = new PolylineClass();
                        }
                        m_pPointColl.AddPoint(pMapPoint, ref before, ref after);
                        break;
                    case "esriGeometryPolygon":
                        if (m_pPointColl == null)
                        {
                            m_pPointColl = new PolygonClass();
                            pGeom = new PolygonClass();
                        }
                        m_pPointColl.AddPoint(pMapPoint, ref before, ref after);
                        break;
                }

                //BeginDrawed(true);
              
                IGroupElement pGroup =null;
                if (m_pPointColl.PointCount == 1)
                {
                    //��Ϊһ����ʱ���Ƶ�
                    Cls3DMarkDraw.AddSimpleGraphic(pMapPoint, Cls3DMarkDraw.getRGB(71, 61, 255), 4, sPolyOutlineName, m_sceneHookHelper.Scene, pGroup);

                }
                else if (m_DrawType.ToString() == "esriGeometryLine")
                {
                    
                    pGeom = m_pPointColl as IGeometry;
                    pGeom.SpatialReference = pMapPoint.SpatialReference;
                    m_psurface.InterpolateShape(pGeom, out  pGeom, ref StepSize);
                    Cls3DMarkDraw.AddSimpleGraphic(pGeom, Cls3DMarkDraw.getRGB(71, 61, 255), 4, sPolyOutlineName, m_sceneHookHelper.Scene, pGroup);
                    m_pPointColl = pGeom as IPointCollection;
                }
                else
                {
                    ITopologicalOperator pTopo = m_pPointColl as ITopologicalOperator;
                    pGeom = pTopo.Boundary;
                    pGeom.SpatialReference = pMapPoint.SpatialReference;
                    m_psurface.InterpolateShape(pGeom, out  pGeom, ref StepSize);
                    Cls3DMarkDraw.AddSimpleGraphic(pGeom, Cls3DMarkDraw.getRGB(71, 61, 255), 4, sPolyOutlineName, m_sceneHookHelper.Scene, pGroup);
                }

                m_sceneHookHelper.SceneGraph.RefreshViewers();

            }
            catch
            {
                return;
            }
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add Tool3DDrawGeo.OnMouseMove implementation
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add Tool3DDrawGeo.OnMouseUp implementation
        }
        /// <summary>
        /// ˫����ʾ���ƶ������
        /// </summary>
        public override void OnDblClick()
        {
            ITopologicalOperator pTopoOp;
            if(m_pPointColl !=null)
            {
                switch (m_DrawType.ToString())
                {
                  
                    case "esriGeometryLine":
                        IPointCollection pPolyLine = new PolylineClass();
                        pPolyLine.AddPointCollection(m_pPointColl);
                        pTopoOp = pPolyLine as ITopologicalOperator;
                        pTopoOp.Simplify();
                        m_Geometry = pPolyLine as IGeometry;
                        m_Geometry.SpatialReference = m_sceneHookHelper.Scene.SpatialReference;
                        Cls3DMarkDraw.DeleteAllElementsWithName(m_sceneHookHelper.Scene, sPolyOutlineName);
                        EndDtrawd(true);//���������¼�
                        break;
                    case "esriGeometryPolygon":
                      if(m_pPointColl.PointCount<3)
                      {
                          return;
                      }
                      IPointCollection pPolygon = new PolygonClass();
                      pPolygon.AddPointCollection(m_pPointColl);
                      pTopoOp = pPolygon as ITopologicalOperator;
                      pTopoOp.Simplify();
                      m_Geometry = pPolygon as IGeometry;
                      m_Geometry.SpatialReference = m_sceneHookHelper.Scene.SpatialReference;
                      Cls3DMarkDraw.DeleteAllElementsWithName(m_sceneHookHelper.Scene, sPolyOutlineName);
                      EndDtrawd(true);//���������¼�
                        break;
                }
                m_pPointColl = null;

            }
            //Cls3DMarkDraw.DeleteAllElementsWithName(m_sceneHookHelper.Scene, sPolyOutlineName);
            m_sceneHookHelper.SceneGraph.RefreshViewers();
        }
        #endregion
    }
}
