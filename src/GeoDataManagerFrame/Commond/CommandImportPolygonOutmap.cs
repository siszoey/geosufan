using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Controls;
using System.Windows.Forms;

using GeoDataCenterFunLib;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;

namespace GeoDataManagerFrame
{
    public class CommandImportPolygonOutmap : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
       public Plugin.Application.IAppFormRef m_frmhook;
       private IScreenDisplay m_pScreenDisplay;
       private IActiveViewEvents_Event m_pActiveViewEvents;
       IActiveView m_pActiveView = null;
       private IPolygon m_Polygon;
       private GeoPageLayout.GeoPageLayout gpl = null;
       private GeoPageLayout.FrmPageLayout fmPageLayout = null;//��ͼ����

        public CommandImportPolygonOutmap()
        {
            base._Name = "GeoDataManagerFrame.CommandImportPolygonOutmap";
            base._Caption = "���뷶Χ��ͼ";
            base._Tooltip = "���뷶Χ��ͼ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "���뷶Χ��ͼ";
           
        }
        //~CommandImportPolygonOutmap()
        //{
        //    gpl = null;
        //}
        public override bool Enabled
        {
            get
            {
                try
                {
                    if (m_Hook.CurrentControl is ISceneControl) return false;
                    if (m_Hook.MapControl.LayerCount == 0)
                    {
                        base._Enabled = false;
                        return false;
                    }

                    base._Enabled = true;
                    return true;
                }
                catch
                {
                    base._Enabled = false;
                    return false;
                }
                return true;
            }
        }
        //��ȥ�ػ�
        private void m_pActiveViewEvents_AfterDraw(IDisplay Display, esriViewDrawPhase phase)
        {
            if (fmPageLayout!=null && !fmPageLayout.IsDisposed)
            {
              drawgeometryXOR(null, m_pScreenDisplay);
            }
           
        }
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
                MessageBox.Show("���Ƶ��뷶Χ����:" + ex.Message, "��ʾ");
                pFillSymbol = null;
            }
        }
        //Ԫ�ط�ʽ
        //private void addGeometryEle(IPolygon pPolygon)
        //{
        //    ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
        //    ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();
        //    IPolygonElement pPolygonEle = new PolygonElementClass();
        //    try
        //    {
        //        //��ɫ����
        //        IRgbColor pRGBColor = new RgbColorClass();
        //        pRGBColor.UseWindowsDithering = false;
        //        ISymbol pSymbol = (ISymbol)pFillSymbol;
        //        pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

        //        pRGBColor.Red = 255;
        //        pRGBColor.Green = 170;
        //        pRGBColor.Blue = 0;
        //        pLineSymbol.Color = pRGBColor;

        //        pLineSymbol.Width = 0.8;
        //        pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
        //        pFillSymbol.Outline = pLineSymbol;

        //        pFillSymbol.Color = pRGBColor;
        //        pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;

        //        IElement pEle = pPolygonEle as IElement;
        //        pEle.Geometry = pPolygon as IGeometry;
        //        pEle.Geometry.SpatialReference = pPolygon.SpatialReference;
        //        (pEle as IFillShapeElement).Symbol = pFillSymbol;
        //        (pEle as IElementProperties).Name="ImpExtentOutMap";
        //        (m_Hook.ArcGisMapControl.Map as IGraphicsContainer).AddElement(pEle, 0);
        //        (m_Hook.ArcGisMapControl.Map as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

        //        IGraphicsContainer pGra = m_Hook.ArcGisMapControl.Map as IGraphicsContainer;
        //        IActiveView pAv = pGra as IActiveView;
        //        pGra.Reset();
        //        IElement pEle2 = pGra.Next();
        //        while (pEle2 != null)
        //        {
        //            if ((pEle2 as IElementProperties).Name == "ImpExtentOutMap")
        //            {
        //                //pGra.DeleteElement(pEle);

        //                //pGra.Reset();
        //            }
        //            pEle2 = pGra.Next();
        //        }
              
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("���Ƶ��뷶Χ����:" + ex.Message, "��ʾ");
        //        pFillSymbol = null;
        //    }
        //}

        #region
        /// <summary>
        /// ���ļ�·�����һ��PolyGon
        /// </summary>
        /// <param name="path">�ļ�ȫ·��</param>
        /// <returns></returns>
        private IPolygon GetPolyGonFromFile(string path)
        {
            IPolygon pGon = null;
            if (path.EndsWith(".mdb"))
            {
                string errmsg = "";
                IWorkspaceFactory pwf = new AccessWorkspaceFactoryClass();
                IWorkspace pworkspace = pwf.OpenFromFile(path, 0);
                IEnumDataset pEnumdataset = pworkspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
                pEnumdataset.Reset();
                IDataset pDataset = pEnumdataset.Next();
                while (pDataset != null)
                {
                    IFeatureClass pFeatureclass = pDataset as IFeatureClass;
                    if (pFeatureclass.ShapeType != esriGeometryType.esriGeometryPolygon && pFeatureclass.ShapeType != esriGeometryType.esriGeometryPolyline)
                    {
                        pDataset = pEnumdataset.Next();
                        continue;
                    }
                    else if (pFeatureclass.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        IFeatureCursor pCursor = pFeatureclass.Search(null, false);
                        IFeature pFeature = pCursor.NextFeature();
                        if (pFeature != null)
                        {
                            pGon = pFeature.Shape as IPolygon;
                            break;
                        }
                        else
                        {
                            pDataset = pEnumdataset.Next();
                            continue;
                        }
                    }
                    else if (pFeatureclass.ShapeType == esriGeometryType.esriGeometryPolyline)
                    {
                        IFeatureCursor pCursor = pFeatureclass.Search(null, false);
                        IFeature pFeature = pCursor.NextFeature();
                        if (pFeature != null)
                        {

                            IPolyline pPolyline = pFeature.Shape as IPolyline;
                            pGon = GetPolygonFormLine(pPolyline);
                            if (pGon.IsClosed == false)
                            {
                                errmsg = "ѡ���Ҫ�ز��ܹ��ɷ�ն���Σ�";
                                pGon = null;
                                pDataset = pEnumdataset.Next();
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                }
                if (pGon == null)
                {
                    IEnumDataset pEnumdataset1 = pworkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                    pEnumdataset1.Reset();
                    pDataset = pEnumdataset1.Next();
                    while (pDataset != null)
                    {
                        IFeatureDataset pFeatureDataset = pDataset as IFeatureDataset;
                        IEnumDataset pEnumDataset2 = pFeatureDataset.Subsets;
                        pEnumDataset2.Reset();
                        IDataset pDataset1 = pEnumDataset2.Next();
                        while (pDataset1 != null)
                        {
                            if (pDataset1 is IFeatureClass)
                            {

                                IFeatureClass pFeatureclass = pDataset1 as IFeatureClass;
                                if (pFeatureclass.ShapeType != esriGeometryType.esriGeometryPolygon && pFeatureclass.ShapeType != esriGeometryType.esriGeometryPolyline)
                                {
                                    pDataset1 = pEnumDataset2.Next();
                                    continue;
                                }
                                else if (pFeatureclass.ShapeType == esriGeometryType.esriGeometryPolygon)
                                {
                                    IFeatureCursor pCursor = pFeatureclass.Search(null, false);
                                    IFeature pFeature = pCursor.NextFeature();
                                    if (pFeature != null)
                                    {
                                        pGon = pFeature.Shape as IPolygon;
                                        break;
                                    }
                                    else
                                    {
                                        pDataset1 = pEnumDataset2.Next();
                                        continue;
                                    }
                                }
                                else if (pFeatureclass.ShapeType == esriGeometryType.esriGeometryPolyline)
                                {
                                    IFeatureCursor pCursor = pFeatureclass.Search(null, false);
                                    IFeature pFeature = pCursor.NextFeature();
                                    if (pFeature != null)
                                    {

                                        IPolyline pPolyline = pFeature.Shape as IPolyline;
                                        pGon = GetPolygonFormLine(pPolyline);
                                        if (pGon.IsClosed == false)
                                        {
                                            errmsg = "ѡ���Ҫ�ز��ܹ��ɷ�ն���Σ�";
                                            pGon = null;
                                            pDataset1 = pEnumDataset2.Next();
                                            continue;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (pGon != null)
                            break;
                        pDataset = pEnumdataset1.Next();
                    }
                }
                if (pGon == null)
                {
                    if (errmsg != "")
                    {
                        MessageBox.Show(errmsg, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("��ѡ��һ��������Ҫ�غ���Ҫ�ص��ļ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return pGon;
                }
            }
            else if (path.EndsWith(".shp"))
            {
                IWorkspaceFactory pwf = new ShapefileWorkspaceFactoryClass();
                string filepath = System.IO.Path.GetDirectoryName(path);
                string filename = path.Substring(path.LastIndexOf("\\") + 1);
                IFeatureWorkspace pFeatureworkspace = (IFeatureWorkspace)pwf.OpenFromFile(filepath, 0);
                IFeatureClass pFeatureclass = pFeatureworkspace.OpenFeatureClass(filename);
                if (pFeatureclass.ShapeType == esriGeometryType.esriGeometryPolygon)
                {
                    IFeatureCursor pCursor = pFeatureclass.Search(null, false);
                    IFeature pFeature = pCursor.NextFeature();
                    if (pFeature != null)
                    {
                        pGon = pFeature.Shape as IPolygon;
                    }
                }
                else if (pFeatureclass.ShapeType == esriGeometryType.esriGeometryPolyline)
                {
                    IFeatureCursor pCursor = pFeatureclass.Search(null, false);
                    IFeature pFeature = pCursor.NextFeature();
                    if (pFeature != null)
                    {

                        IPolyline pPolyline = pFeature.Shape as IPolyline;
                        pGon = GetPolygonFormLine(pPolyline);
                        if (pGon.IsClosed == false)
                        {
                            MessageBox.Show("ѡ�����Ҫ�ز��ܹ��ɷ�ն���Σ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return null;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("��ѡ��һ���������Ҫ���ļ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return null;
                }


            }
            else if (path.EndsWith(".txt"))
            {
                string txtpath = path;
                System.IO.StreamReader smRead = new System.IO.StreamReader(txtpath, System.Text.Encoding.Default); //����·��  
                string line;

                IPointCollection pc = pGon as IPointCollection;
                double x, y;
                while ((line = smRead.ReadLine()) != null)
                {
                    if (line.IndexOf(",") > 0)
                    {
                        try
                        {
                            x = double.Parse(line.Substring(0, line.IndexOf(",")));
                            y = double.Parse(line.Substring(line.IndexOf(",") + 1));
                        }
                        catch
                        {
                            MessageBox.Show("�ı��ļ���ʽ����ȷ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            smRead.Close();
                            return null;
                        }
                        IPoint tmpPoint = new ESRI.ArcGIS.Geometry.Point();
                        tmpPoint.X = x;
                        tmpPoint.Y = y;
                        object ep = System.Reflection.Missing.Value;

                        pc.AddPoint(tmpPoint, ref ep, ref ep);
                    }

                }
                smRead.Close();
                ICurve pCurve = pGon as ICurve;
                if (pCurve.IsClosed == false)
                {
                    MessageBox.Show("��������겻�ܹ��ɷ�ն���Σ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return null;
                }

            }
            else if (path.EndsWith("gdb"))
            {
                string errmsg = "";
                IWorkspaceFactory pwf = new FileGDBWorkspaceFactoryClass();
                IWorkspace pworkspace = pwf.OpenFromFile(path.Substring(0, path.LastIndexOf("\\")), 0);
                IEnumDataset pEnumdataset = pworkspace.get_Datasets(esriDatasetType.esriDTFeatureClass);
                pEnumdataset.Reset();
                IDataset pDataset = pEnumdataset.Next();
                while (pDataset != null)
                {
                    IFeatureClass pFeatureclass = pDataset as IFeatureClass;
                    if (pFeatureclass.ShapeType != esriGeometryType.esriGeometryPolygon && pFeatureclass.ShapeType != esriGeometryType.esriGeometryPolyline)
                    {
                        pDataset = pEnumdataset.Next();
                        continue;
                    }
                    else if (pFeatureclass.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        IFeatureCursor pCursor = pFeatureclass.Search(null, false);
                        IFeature pFeature = pCursor.NextFeature();
                        if (pFeature != null)
                        {
                            pGon = pFeature.Shape as IPolygon;
                            break;
                        }
                        else
                        {
                            pDataset = pEnumdataset.Next();
                            continue;
                        }
                    }
                    else if (pFeatureclass.ShapeType == esriGeometryType.esriGeometryPolyline)
                    {
                        IFeatureCursor pCursor = pFeatureclass.Search(null, false);
                        IFeature pFeature = pCursor.NextFeature();
                        if (pFeature != null)
                        {

                            IPolyline pPolyline = pFeature.Shape as IPolyline;
                            pGon = GetPolygonFormLine(pPolyline);
                            if (pGon.IsClosed == false)
                            {
                                errmsg = "ѡ���Ҫ�ز��ܹ��ɷ�ն���Σ�";
                                pGon = null;
                                pDataset = pEnumdataset.Next();
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                }
                if (pGon == null)
                {
                    IEnumDataset pEnumdataset1 = pworkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                    pEnumdataset1.Reset();
                    pDataset = pEnumdataset1.Next();
                    while (pDataset != null)
                    {
                        IFeatureDataset pFeatureDataset = pDataset as IFeatureDataset;
                        IEnumDataset pEnumDataset2 = pFeatureDataset.Subsets;
                        pEnumDataset2.Reset();
                        IDataset pDataset1 = pEnumDataset2.Next();
                        while (pDataset1 != null)
                        {
                            if (pDataset1 is IFeatureClass)
                            {

                                IFeatureClass pFeatureclass = pDataset1 as IFeatureClass;
                                if (pFeatureclass.ShapeType != esriGeometryType.esriGeometryPolygon && pFeatureclass.ShapeType != esriGeometryType.esriGeometryPolyline)
                                {
                                    pDataset1 = pEnumDataset2.Next();
                                    continue;
                                }
                                else if (pFeatureclass.ShapeType == esriGeometryType.esriGeometryPolygon)
                                {
                                    IFeatureCursor pCursor = pFeatureclass.Search(null, false);
                                    IFeature pFeature = pCursor.NextFeature();
                                    if (pFeature != null)
                                    {
                                        pGon = pFeature.Shape as IPolygon;
                                        break;
                                    }
                                    else
                                    {
                                        pDataset1 = pEnumDataset2.Next();
                                        continue;
                                    }
                                }
                                else if (pFeatureclass.ShapeType == esriGeometryType.esriGeometryPolyline)
                                {
                                    IFeatureCursor pCursor = pFeatureclass.Search(null, false);
                                    IFeature pFeature = pCursor.NextFeature();
                                    if (pFeature != null)
                                    {

                                        IPolyline pPolyline = pFeature.Shape as IPolyline;
                                        pGon = GetPolygonFormLine(pPolyline);
                                        if (pGon.IsClosed == false)
                                        {
                                            errmsg = "ѡ���Ҫ�ز��ܹ��ɷ�ն���Σ�";
                                            pGon = null;
                                            pDataset1 = pEnumDataset2.Next();
                                            continue;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (pGon != null)
                            break;
                        pDataset = pEnumdataset1.Next();
                    }
                }
                if (pGon == null)
                {
                    if (errmsg != "")
                    {
                        MessageBox.Show(errmsg, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("��ѡ��һ��������Ҫ�غ���Ҫ�ص��ļ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return pGon;
                }
            }
            return pGon;
        }
        #endregion

        //cast the polyline object to the polygon xisheng 20110926 
        private IPolygon GetPolygonFormLine(IPolyline pPolyline)
        {
            ISegmentCollection pRing;
            IGeometryCollection pPolygon = new PolygonClass();
            IGeometryCollection pPolylineC = pPolyline as IGeometryCollection;
            object o = Type.Missing;
            for (int i = 0; i < pPolylineC.GeometryCount; i++)
            {
                pRing = new RingClass();
                pRing.AddSegmentCollection(pPolylineC.get_Geometry(i) as ISegmentCollection);
                pPolygon.AddGeometry(pRing as IGeometry, ref o, ref o);
            }
            IPolygon polygon = pPolygon as IPolygon;
            return polygon;
        }

        public override void OnClick()
        {
            if (m_Hook == null)
                return;
            //LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);

            //if (log != null)
            //{
            //    log.Writelog("���뷶Χ��ͼ");
            //}
            if (m_Hook.ArcGisMapControl.Map.LayerCount == 0)
            {
                MessageBox.Show("��ǰû�е������ݣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            OpenFileDialog dlg = new OpenFileDialog();
            //dlg.Filter = "�������ݿ�(*.mdb)|*.mdb|shp����|*.shp|�ı��ļ�|*.txt";
            dlg.Filter = "shp����|*.shp|�������ݿ�(*.mdb)|*.mdb|�ļ����ݿ�(*.gdb)|gdb";
            if (dlg.ShowDialog() == DialogResult.Cancel)
                return;
            IPolygon pGon = new PolygonClass();
            pGon = GetPolyGonFromFile(dlg.FileName);
            if (pGon == null) return;
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog("���뷶Χ��ͼ,��Χ�ļ�·��Ϊ:" + dlg.FileName);
            }
            ESRI.ArcGIS.Carto.IMap pMap = m_Hook.MapControl.ActiveView.FocusMap;
            pGon.SpatialReference = pMap.SpatialReference;//�븳��һ�µĿռ�ο�
            ITopologicalOperator pTopo = pGon as ITopologicalOperator;
            if (pTopo != null) pTopo.Simplify();
            drawgeometryXOR(pGon, m_pScreenDisplay);

            //addGeometryEle(pGon);
            //SysCommon.CProgress pgss = new SysCommon.CProgress("���ڼ�����ͼ���棬���Ժ�...");
            //pgss.EnableCancel = false;
            //pgss.ShowDescription = false;
            //pgss.FakeProgress = true;
            //pgss.TopMost = true;
            //pgss.ShowProgress();
            //Application.DoEvents();
            fmPageLayout = new GeoPageLayout.FrmPageLayout(pMap, pGon as IGeometry);
            fmPageLayout.WriteLog = this.WriteLog;//ygc 2012-9-12 �Ƿ�д��־
            //pgss.Close();
            fmPageLayout.FormClosed += new FormClosedEventHandler(fmPageLayout_FormClosed);
            fmPageLayout.Show();
            
            Application.DoEvents();
            //gpl = new GeoPageLayout.GeoPageLayout(pMap, pGon as ESRI.ArcGIS.Geometry.IGeometry);
            //gpl.typePageLayout = 2;
            //gpl.MapOut();
            //gpl = null;

            //m_Hook.MapControl.ActiveView.Refresh();
            

        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppGisUpdateRef;
            m_frmhook = hook as Plugin.Application.IAppFormRef;

            m_pActiveView = m_Hook.MapControl.Map as IActiveView;

            m_pActiveViewEvents = m_pActiveView as IActiveViewEvents_Event;
            m_pScreenDisplay = m_pActiveView.ScreenDisplay;
            try
            {
                m_pActiveViewEvents.AfterDraw += new IActiveViewEvents_AfterDrawEventHandler(m_pActiveViewEvents_AfterDraw);
             
            }
            catch
            {
            }
        }
        private void fmPageLayout_FormClosed(object sender,FormClosedEventArgs e)
        {
            // else
            //{
            //    try
            //    {
            //        m_pActiveViewEvents.AfterDraw -= new IActiveViewEvents_AfterDrawEventHandler(m_pActiveViewEvents_AfterDraw);
            //    }
            //    catch
            //    {
            //    }
            //}
            
            //IGraphicsContainer pGra = m_Hook.ArcGisMapControl.Map as IGraphicsContainer;
            //IActiveView pAv = pGra as IActiveView;
            //pGra.Reset();
            //IElement pEle = pGra.Next();
            //while (pEle != null)
            //{
            //    if ((pEle as IElementProperties).Name == "ImpExtentOutMap")
            //    {
            //        pGra.DeleteElement(pEle);
                   
            //        pGra.Reset();
            //    }
            //    pEle = pGra.Next();
            //}
            m_Polygon = null;
            m_pActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
        }
    }
}
