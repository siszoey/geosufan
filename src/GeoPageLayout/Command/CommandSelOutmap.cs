using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using System.Collections.Generic;
using System.Windows.Forms;
using ESRI.ArcGIS.Display;
using SysCommon.Error;

namespace GeoPageLayout
{
    /// <summary>
    /// Summary description for CommandSelOutmap.
    /// </summary>
    [Guid("7569408E-569E-404D-8BF9-B41F3E8C4967")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("GeoPageLayout.CommandSelOutmap")]
    public sealed class CommandSelOutmap : BaseCommand
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
            ControlsCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private bool _Writelog = true;  //added by chulili 2012-09-10 ɽ��֧�֡��Ƿ�д��־������
        public bool WriteLog
        {
            get
            {
                return _Writelog;
            }
            set
            {
                _Writelog = value;
            }
        }

        private IHookHelper m_hookHelper;
        //Ϊ���Ƶ���ķ�Χ
        private IScreenDisplay m_pScreenDisplay;
        private IActiveViewEvents_Event m_pActiveViewEvents;
        private IActiveView m_pActiveView = null;
        private IPolygon m_Polygon;
        //Ϊ���Ƶ���ķ�Χ
        FrmPageLayout frm = null;
        public CommandSelOutmap()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = ""; //localizable text
            base.m_caption = "";  //localizable text
            base.m_message = "";  //localizable text 
            base.m_toolTip = "";  //localizable text 
            base.m_name = "GeoPageLayout_CommandSelOutmap";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;
            IMapControl2 pMapCtl = m_hookHelper.Hook as IMapControl2;
            pMapCtl.CurrentTool = null;
            m_pActiveView = m_hookHelper.FocusMap as IActiveView;

            m_pActiveViewEvents = m_pActiveView as IActiveViewEvents_Event;
            m_pScreenDisplay = m_pActiveView.ScreenDisplay;
            try
            {
                m_pActiveViewEvents.AfterDraw += new IActiveViewEvents_AfterDrawEventHandler(m_pActiveViewEvents_AfterDraw);

            }
            catch
            {
            }
            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            if (m_hookHelper.Hook == null) return;
            //IFeatureLayer tmpFeatureLayer = layerCurSeleted();
            //if (tmpFeatureLayer == null)
            //{
            //    MessageBox.Show("���ڵ�ͼĿ¼���õ�ǰѡ��ͼ�㣡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            try
            {
                //if (m_hookHelper.FocusMap.SelectionCount > 100)
                //int ooo = m_hookHelper.FocusMap.SelectionCount;
                List<IGeometry> vTemp = GetDataGeometry(m_hookHelper.FocusMap);
                if (vTemp == null) return;
                SysCommon.CProgress pgss = new SysCommon.CProgress("���ڼ�����ͼ���棬���Ժ�...");
                pgss.EnableCancel = false;
                pgss.ShowDescription = false;
                pgss.FakeProgress = true;
                pgss.TopMost = true;
                pgss.ShowProgress();
                
                //ITopologicalOperator pTO = GetUnion(vTemp) as ITopologicalOperator;

                ESRI.ArcGIS.Geometry.IGeometry pGeometry = GetUnion(vTemp);

                m_hookHelper.FocusMap.ClearSelection();

                //m_hookHelper.ActiveView.Extent=pGeometry.Envelope;
                m_hookHelper.ActiveView.Refresh();
                Application.DoEvents();
                drawgeometryXOR(pGeometry as IPolygon, m_pScreenDisplay);

                frm = new FrmPageLayout(m_hookHelper.FocusMap, pGeometry);
                frm.WriteLog = WriteLog;//ygc 2012-9-12 �Ƿ�д��־
                frm.FormClosed += new FormClosedEventHandler(frm_FormClosed);
                frm.typeZHT = 2;
                frm.Show();
                pgss.Close();
                Application.DoEvents();
            }
            catch (Exception exError)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", exError.Message);
            }
        }
        //����ر�ʱ ˢ��ǰ��
        private void frm_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_Polygon = null;
            m_pActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
        }
        #endregion


        private List<ESRI.ArcGIS.Geometry.IGeometry> GetDataGeometry(IMap pMap)
        {
            List<IGeometry> lstGeometrys = new List<IGeometry>();

            //IGeometry pTempGeo = null;
            if (pMap.SelectionCount < 1)
            {
                System.Windows.Forms.MessageBox.Show("��ʹ�õ�ͼ�������еġ�ѡ��Ҫ�ء������ڵ�ǰ��ͼ��ѡ��Ҫ�أ�Ȼ�󷽿�ʹ�øù��ܡ�","ϵͳ��ʾ",MessageBoxButtons.OK ,MessageBoxIcon.Information );
                return null;
            }
            if (pMap.SelectionCount > 20)
            {
                System.Windows.Forms.MessageBox.Show("ѡ���Ҫ�ع��࣡", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
            IActiveView pAv = pMap as IActiveView;
            ISelection pSelection = pMap.FeatureSelection;
            if (pSelection == null) return null;

            IEnumFeature pEnumFea = pSelection as IEnumFeature;
            IFeature pFea = pEnumFea.Next();
            while (pFea != null)
            {
                if (pFea.ShapeCopy != null)
                {
                    if (pFea.Shape.GeometryType != esriGeometryType.esriGeometryPolygon)
                    {
                        pFea = pEnumFea.Next();//whileѭ����continue��ʱ��ǧ��ǵ� ���ƣ�
                        continue;
                    }
                    lstGeometrys.Add(pFea.ShapeCopy);
                }
                pFea = pEnumFea.Next();
            }

            return lstGeometrys;
        }

        /// <summary>
        /// union������Ҫ��
        /// </summary>
        /// <param name="lstGeometry">��Ҫ��������Ҫ�ؼ���</param>
        /// <returns>����union���ͼ��</returns>
        public static IGeometry GetUnion(List<IGeometry> lstGeometry)
        {
            IGeometryBag pGeoBag = new GeometryBagClass();
            IGeometryCollection pGeoCol = pGeoBag as IGeometryCollection;
           
            if (lstGeometry.Count < 1) return null;
            if (lstGeometry[0].SpatialReference != null)
            {
                pGeoBag.SpatialReference = lstGeometry[0].SpatialReference;
            }

            object obj = System.Type.Missing;
            for (int i = 0; i < lstGeometry.Count; i++)
            {
                IGeometry pTempGeo = lstGeometry[i];
                pGeoCol.AddGeometry(pTempGeo, ref obj, ref obj);
            }
            ISpatialIndex pSI = pGeoBag as ISpatialIndex;
            pSI.AllowIndexing = true;
            pSI.Invalidate();

            ITopologicalOperator pTopo = new PolygonClass();
            pTopo.ConstructUnion(pGeoBag as IEnumGeometry);
            IGeometry pGeo = pTopo as IGeometry;

            return pGeo;
        }
        private IFeatureLayer layerCurSeleted()
        {

            int iLayerCount = m_hookHelper.FocusMap.LayerCount;
            IFeatureLayer pFeatureLayer;
            IFeatureLayer pCurSeLayer = null;
            IMap pMap = m_hookHelper.FocusMap;
            int countSelectable = 0;
            for (int n = 0; n < iLayerCount; n++)
            {
                if (countSelectable >= 2) break;
                ILayer layer = pMap.get_Layer(n);
                if (layer is IGroupLayer)
                {
                    ICompositeLayer Comlayer = layer as ICompositeLayer;//��һ��������Ĳ����ת����һ����ϲ㣬ʹ�����Ա���
                    for (int c = 0; c < Comlayer.Count; c++)
                    {
                        pFeatureLayer = Comlayer.get_Layer(c) as IFeatureLayer;
                        if (pFeatureLayer != null && pFeatureLayer.Selectable)
                        {
                            countSelectable++;
                            pCurSeLayer = pFeatureLayer;

                        }

                    }
                }
            }//for
            if (countSelectable != 1)
                return null;
            else
                return pCurSeLayer;




        }

        //��ȥ�ػ�
        private void m_pActiveViewEvents_AfterDraw(IDisplay Display, esriViewDrawPhase phase)
        {
            if (frm != null && !frm.IsDisposed)
            {
                drawgeometryXOR(null, m_pScreenDisplay);
            }

        }
        //���Ƶ���ķ�Χ
        private void drawgeometryXOR(IPolygon pPolygon, IScreenDisplay pScreenDisplay)
        {
            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();

            try
            {
                //��ɫ����
                IRgbColor pRGBColor = new RgbColorClass();
                pRGBColor.UseWindowsDithering = false;
                ISymbol pSymbol = (ISymbol)pFillSymbol;
                pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

                pRGBColor.Red = 255;
                pRGBColor.Green = 170;
                pRGBColor.Blue = 0;
                pLineSymbol.Color = pRGBColor;

                pLineSymbol.Width = 0.8;
                pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                pFillSymbol.Outline = pLineSymbol;

                pFillSymbol.Color = pRGBColor;
                pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;

                pScreenDisplay.StartDrawing(m_pScreenDisplay.hDC, -1);  //esriScreenCache.esriNoScreenCache -1
                pScreenDisplay.SetSymbol(pSymbol);

                //�������ѻ����Ķ����
                if (pPolygon != null)
                {
                    pScreenDisplay.DrawPolygon(pPolygon);
                    m_Polygon = pPolygon;
                }
                //�����ѻ����Ķ����
                else
                {
                    if (m_Polygon != null)
                    {
                        pScreenDisplay.DrawPolygon(m_Polygon);
                    }
                }

                pScreenDisplay.FinishDrawing();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pFillSymbol = null;
            }
        }
    }
}
