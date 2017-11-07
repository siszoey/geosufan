using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using System.Windows.Forms;

using ESRI.ArcGIS.DataSourcesGDB;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using System.Drawing;
using DevComponents.AdvTree;
using stdole;

namespace GeoPageLayout
{
    /// <summary>
    /// ���ߣ�yjl
    /// ���ڣ�2011.05.24
    /// ˵������ͼ��ͼ����
    /// </summary>
    public class GeoPageLayout
    {

        private IMap cMap = null;

        //private XmlDocument cXmlDoc = null;
        //IWorkspace srcWs = null;
        string cDir = "";//��ǰʵ��ͼ�γɹ�Ŀ¼
        public int typePageLayout = -1;//��ͼ����0��׼1ҵ��2��̬��Χ3�Ӻ�ͼ����4�����������ַ�5Ͻ��ͼ6����Ͻ��ͼ
        public List<string> listview = new List<string>();
        public bool isNeed = false;
        public string ztlx = "";
        public string stcxzq = "";
        public int stcSclae = 10000;
        private IGeometry pGeometry = null;//��ͼ��״
        private string StrMapNo = "";
        private IPoint pPoint = null;//Ϊ�˴�����ߵ�����
        private int type_ZT = 0;//20110914 yjl add ר������
        private string pXZQMC = null;
        private string pZTMC = null;//ר������
        private Node pXZQ = null;//����Ͻ��ͼ���������ڵ�
        private bool _Writelog = true;  //added by chulili 2012-09-07 �Ƿ�д��־
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
        //�չ��캯��
        public GeoPageLayout()
        { }
        //��׼��ͼ���캯��,δ֪ͼ����
        public GeoPageLayout(IMap pMap)
        {
            cMap = pMap;

        }
        //��׼��ͼ���캯��,��֪ͼ����,֧�ִ��ո��ͼ���źͲ�����
        public GeoPageLayout(IMap pMap, string strMapNo, int iScale, IPoint inPoint, int typeZT)
        {
            cMap = pMap;
            StrMapNo = strMapNo;
            stcSclae = iScale;
            pPoint = inPoint;
            type_ZT = typeZT;//ר������
        }
        //��̬��Χ��ͼ���캯��
        public GeoPageLayout(IMap pMap, IGeometry fGeometry)
        {
            cMap = pMap;
            pGeometry = fGeometry;
        }

        //����������������캯��
        public GeoPageLayout(IMap pMap, int inScale, string inZTMC, Node inXZQ)
        {
            cMap = pMap;
            //pGeometry = xzqGeometry;
            stcSclae = inScale;
            pXZQ = inXZQ;
            pZTMC = inZTMC;
        }
        //��������������ַ����캯��
        public GeoPageLayout(IMap pMap, IGeometry xzqGeometry, int inScale, string xzqName, int typeZT)
        {
            cMap = pMap;
            pGeometry = xzqGeometry;
            stcSclae = inScale;
            pXZQMC = xzqName;
            type_ZT = typeZT;
        }


        //��ͼ������
        public void MapOut()
        {
            if (typePageLayout == 0)//��׼�ַ� pagelayout
            {
                SysCommon.CProgress pgss = new SysCommon.CProgress("���ڼ�����ͼ���棬���Ժ�...");
                pgss.EnableCancel = false;
                pgss.ShowDescription = false;
                pgss.FakeProgress = true;
                pgss.TopMost = true;
                pgss.ShowProgress();
                Application.DoEvents();
                FrmPageLayout pPl = new FrmPageLayout(cMap);//��ͼ������
                pPl.WriteLog = WriteLog;//ygc 2012-9-12 �Ƿ�д��־
                pPl.typeZHT = 0;
                pPl.Show();
                pgss.Close();
                Application.DoEvents();
            }
            //if (typePageLayout == 1)//only for open a saved mxd with a data mdb
            //{
            //    FrmPageLayout pPl = new FrmPageLayout(cDir + "\\" + ztlx + ".mxd");
            //    pPl.ShowDialog();

            //}
            if (typePageLayout == 2)//��̬��Χ pagelayout
            {
                SysCommon.CProgress pgss = new SysCommon.CProgress("���ڼ�����ͼ���棬���Ժ�...");
                pgss.EnableCancel = false;
                pgss.ShowDescription = false;
                pgss.FakeProgress = true;
                pgss.TopMost = true;
                pgss.ShowProgress();
                Application.DoEvents();
                pgss.Close();
                Application.DoEvents();
            }
            if (typePageLayout == 3)//�Ӻ�ͼ��������֪ͼ���ŵı�׼�ַ�
            {
                try
                {
                    Application.DoEvents();
                    SysCommon.CProgress pgss = new SysCommon.CProgress("���ڼ�����ͼ���棬���Ժ�...");
                    pgss.EnableCancel = false;
                    pgss.ShowDescription = false;
                    pgss.FakeProgress = true;
                    pgss.TopMost = true;
                    pgss.ShowProgress();
                    Application.DoEvents();

                    FrmPageLayout pPl = new FrmPageLayout(cMap, StrMapNo, stcSclae, pgss, pPoint, type_ZT);
                    pPl.WriteLog = WriteLog;//ygc 2012-9-12 �Ƿ�д��־
                    pPl.typeZHT = 3;
                    pPl.Show();

                    Application.DoEvents();
                }
                catch
                {
                }
            }
            if (typePageLayout == 4)//�����ַ�ͼ
            {

                BatPageLayoutTDLYFFT(cMap, pXZQMC, stcSclae, type_ZT);

            }
            if (typePageLayout == 5)//����Ͻ��ͼ
            {

                pageLayoutTDLYXQT(cMap, pGeometry, stcSclae);
            }
            if (typePageLayout == 6)//����Ͻ��ͼ
            {

                batPageLayoutTDLYXQT(cMap, pZTMC, stcSclae, pXZQ);
            }

        }//fn_MapOut

        #region ��Χ��ͼ
        private void pageLayoutExtent(IMap pMap, IGeometry pExtent)
        {
            try
            {
                IPageLayout pPageLayout = new PageLayoutClass();
                IActiveView pActiveView = pPageLayout as IActiveView;
                IGraphicsContainer pGra = pPageLayout as IGraphicsContainer;
                IMapLayers pMapLayers = pActiveView.FocusMap as IMapLayers;
                for (int i = 0; i < pMap.LayerCount; i++)
                {
                    pMapLayers.InsertLayer(pMap.get_Layer(i), false, pMapLayers.LayerCount);
                }
                IActiveView mapAV = pMapLayers as IActiveView;
                mapAV.Extent = pExtent.Envelope;
                IMap pgMap = pMapLayers as IMap;
                //pgMap.ClipGeometry = pExtent;
                //pgMap.ClipBorder = createBorder(pExtent, pActiveView);

                //arcgis10�����˽ӿ�������ClipGeometry
                IMapClipOptions pMapClip = pMapLayers as IMapClipOptions;
                pMapClip.ClipType = esriMapClipType.esriMapClipShape;
                pMapClip.ClipGeometry = pExtent;
                pMapClip.ClipBorder = createBorder(pExtent, pActiveView);

                IMapFrame pMapFrame = (IMapFrame)pGra.FindFrame(pActiveView.FocusMap);
                IElement pMapEle = pMapFrame as IElement;
                (pMapEle as IElementProperties).Name = "��ͼ";
                IPage pPage = pPageLayout.Page;
                pPage.IsPrintableAreaVisible = false;
                pMapEle.Geometry = pPage.PrintableBounds;
                FrmPageLayout fmPL = new FrmPageLayout(pPageLayout);
                fmPL.WriteLog = WriteLog;//ygc 2012-9-12 �Ƿ�д��־
                fmPL.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion


        #region �����ַ�ɭ����Դ��״ͼ
        //�ɱ����߻�þ�γ�ȷ���
        private void getDeltaJW(int inScale, ref double diffX, ref double diffY)
        {
            switch (inScale)
            {
                case 500000:
                    diffX = 3 * 3600;
                    diffY = 2 * 3600;
                    break;
                case 250000:
                    diffX = 3 * 3600 / 2;
                    diffY = 1 * 3600;
                    break;
                case 100000:
                    diffX = 1 * 3600 / 2;
                    diffY = 1 * 3600 / 3;
                    break;
                case 50000:
                    diffX = 1 * 3600 / 4;
                    diffY = 1 * 3600 / 6;
                    break;
                case 25000:
                    diffX = 1 * 3600 / 8;
                    diffY = 1 * 3600 / 12;
                    break;
                case 10000:
                    diffX = 1 * 3600 / 16;
                    diffY = 1 * 3600 / 24;
                    break;
                case 5000:
                    diffX = 1 * 3600 / 32;
                    diffY = 1 * 3600 / 48;
                    break;
            }

        }
        //�ɼ�����������ͼ��������
        private List<string> getTFHLst(WKSPoint[] inJD, int inScale, IGeometry inXZQ)
        {

            ISpatialReference pSR = inXZQ.SpatialReference;
            IGeographicCoordinateSystem pGCS = new GeographicCoordinateSystemClass();
            pGCS = (pSR as IProjectedCoordinateSystem).GeographicCoordinateSystem;
            Dictionary<string, IGeometry> res = new Dictionary<string, IGeometry>();
            double difX = 0, difY = 0;
            getDeltaJW(inScale, ref difX, ref difY);
            double minX = Math.Floor(inJD[0].X * 3600 / difX) * difX;//ͼ���ŵļ�ֵ
            double minY = Math.Floor(inJD[0].Y * 3600 / difY) * difY;
            double maxX = Math.Floor(inJD[1].X * 3600 / difX) * difX;
            double maxY = Math.Floor(inJD[1].Y * 3600 / difY) * difY;
            for (double i = minX; i <= maxX; i += difX)
            {
                for (double j = minY; j <= maxY; j += difY)
                {
                    IEnvelope pEnv = new EnvelopeClass();
                    pEnv.PutCoords(i / 3600, j / 3600, (i + difX) / 3600, (j + difY) / 3600);
                    pEnv.SpatialReference = pGCS;
                    string mapNo = "";
                    long lScale = inScale;
                    GeoDrawSheetMap.basPageLayout.GetNewCodeFromCoordinate(ref mapNo, (long)i, (long)(j + 10), lScale);
                    res.Add(mapNo, pEnv as IGeometry);

                }
            }//for i
            //����������������,�Ƚ����������㵽�������

            inXZQ.Project(pGCS);
            inXZQ.SpatialReference = pGCS;
            IRelationalOperator pRO = inXZQ as IRelationalOperator;
            List<string> tfh = new List<string>();
            (cMap as IGraphicsContainer).DeleteAllElements();
            foreach (KeyValuePair<string, IGeometry> kvp in res)
            {
                if (pRO.Overlaps(kvp.Value))
                {
                    //drawPolygonElement(kvp.Value, cMap as IGraphicsContainer);
                    tfh.Add(kvp.Key);
                }
                else if (pRO.Contains(kvp.Value))
                {
                    //drawPolygonElement(kvp.Value, cMap as IGraphicsContainer);
                    tfh.Add(kvp.Key);

                }
            }
            return tfh;
        }
        //�ɷ�Χ���ɼ�������
        private WKSPoint[] getPts(IGeometry inXZQ)
        {
            WKSPoint[] res = new WKSPoint[2];
            ISpatialReference pSpatialRefrence = inXZQ.SpatialReference;
            IEnvelope xzqEnv = inXZQ.Envelope;
            if (pSpatialRefrence is IProjectedCoordinateSystem)
            {
                IProjectedCoordinateSystem pPCS = pSpatialRefrence as IProjectedCoordinateSystem;
                WKSPoint pPointMin = new WKSPoint();
                pPointMin.X = xzqEnv.XMin;
                pPointMin.Y = xzqEnv.YMin;
                pPCS.Inverse(1, ref pPointMin);
                res[0] = pPointMin;
                WKSPoint pPointMax = new WKSPoint();
                pPointMax.X = xzqEnv.XMax;
                pPointMax.Y = xzqEnv.YMax;
                pPCS.Inverse(1, ref pPointMax);
                res[1] = pPointMax;
                return res;
            }
            else if (pSpatialRefrence is IGeographicCoordinateSystem)
            {
                WKSPoint pPointMin = new WKSPoint();
                pPointMin.X = xzqEnv.XMin;
                pPointMin.Y = xzqEnv.YMin;
                res[0] = pPointMin;
                WKSPoint pPointMax = new WKSPoint();
                pPointMax.X = xzqEnv.XMax;
                pPointMax.Y = xzqEnv.YMax;
                res[1] = pPointMax;
                return res;

            }
            else
                return null;

        }
        //�������������ɭ����Դ��״�ַ�ͼ
        private void BatPageLayoutTDLYFFT(IMap pMap, string inXZQMC, int iScale, int inTypeZT)
        {
            FormProgress fmPgs = null;
            SysCommon.CProgress pgss = null;
            try
            {
                cDir = createDir(inXZQMC + pMap.Name + "�����ַ�ͼ");
                if (cDir == "")
                    return;
                IGeometry xzq = pGeometry;
                WKSPoint[] wksP = getPts(xzq);
                if (wksP == null)
                    return;
                pgss = new SysCommon.CProgress("���ڼ���" + inXZQMC + "��Χ��ͼ����...");
                pgss.EnableCancel = false;
                pgss.ShowDescription = false;
                pgss.FakeProgress = true;
                pgss.TopMost = true;
                pgss.ShowProgress();
                Application.DoEvents();
                List<string> TFHs = getTFHLst(wksP, stcSclae, xzq);
                pgss.Close();
                Application.DoEvents();
                if (TFHs.Count == 0)
                    return;
                List<string> strMapNos = TFHs;
                Dictionary<string, string> pgTextElements = null;
                bool hasLegend = false;
                if (inTypeZT == 0)
                {
                    FrmSheetMapSet frmSMS = new FrmSheetMapSet(iScale, "", strMapNos[0]);
                    frmSMS.WriteLog = WriteLog;//ygc 2012-9-12 �Ƿ�д��־
                    if (frmSMS.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    pgTextElements = frmSMS.MapTextElements;
                    hasLegend = frmSMS.HasLegend;
                }
                else if (inTypeZT == 1)
                {
                    FrmSheetMapSet_ZT frmSMS = new FrmSheetMapSet_ZT(iScale, "", strMapNos[0]);
                    frmSMS.WriteLog = WriteLog;//ygc 2012-9-12 �Ƿ�д��־
                    if (frmSMS.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    pgTextElements = frmSMS.MapTextElements;
                    hasLegend = frmSMS.HasLegend;

                }
                fmPgs = new FormProgress();
                ProgressBar pgsBar = fmPgs.progressBar1;
                pgsBar.Minimum = 1;
                pgsBar.Maximum = strMapNos.Count;
                pgsBar.Step = 1;
                fmPgs.TopLevel = true;
                fmPgs.Text = "�����������" + inXZQMC + "ɭ����Դ��״�ַ�ͼ";
                fmPgs.Show();

                foreach (string strMapNo in strMapNos)
                {

                    fmPgs.lblOut.Text = "�ܹ�" + strMapNos.Count + "�������������" + pgsBar.Value + "����ͼ����Ϊ��" + strMapNo;
                    pgTextElements["G50G005005"] = strMapNo;
                    IObjectCopy pOC = new ObjectCopyClass();
                    IMap newMap = pOC.Copy(pMap) as IMap;
                    procLyrScale(newMap);//yjlȡ����������ʾ����
                    IMaps newMaps = new Maps();
                    newMaps.Add(newMap);
                    IPageLayoutControl pPLC = new PageLayoutControlClass();
                    pPLC.PageLayout.ReplaceMaps(newMaps);
                    //axPageLayoutControl1.PageLayout = cMD.PageLayout;
                    if (iScale > 2000)//С������
                    {
                        GeoPageLayoutFn.QuietPageLayoutTDLYFFT(pPLC, pgTextElements, inTypeZT);
                    }
                    IMapFrame pMapFrame = (IMapFrame)pPLC.GraphicsContainer.FindFrame(pPLC.ActiveView.FocusMap);
                    IElement pMapEle = pMapFrame as IElement;
                    (pMapEle as IElementProperties).Name = "��ͼ";
                    double x = pMapEle.Geometry.Envelope.XMax;//��ͼ��ܵ����ϵ�����
                    double y = pMapEle.Geometry.Envelope.YMax;
                    double xmin = pMapEle.Geometry.Envelope.XMin;
                    //���·�Χͼ��
                    GeoPageLayoutFn.updateMapSymbol(pPLC.ActiveView.FocusMap, pPLC.ActiveView.FocusMap.ClipGeometry);
                    if (type_ZT == 1 && hasLegend)
                    {
                        AddLegend(pPLC.PageLayout, pPLC.ActiveView.FocusMap, x + 1, y, 4, 1);

                    }
                    IMapDocument cMD = new MapDocumentClass();
                    string savePath = cDir + @"\" + strMapNo + ".mxd";
                    cMD.New(savePath);
                    cMD.Open(savePath, "");
                    cMD.ReplaceContents(pPLC.PageLayout as IMxdContents);
                    cMD.Save(true, false);
                    cMD.Close();
                    if (fmPgs.IsDisposed)
                    {
                        if (this.WriteLog)
                        {
                            Plugin.LogTable.Writelog("�û�ȡ������ɭ����Դ��״�ַ�ͼ������������ͼ�����ڣ�" + cDir);
                        }
                        break;
                    }

                    pgsBar.PerformStep();
                    Application.DoEvents();

                }//foreach
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("����ɭ����Դ��״�ַ�ͼ�����ɣ�����·����" + cDir);
                }
                //MessageBox.Show("����ɭ����Դ��״�ַ�ͼ�����ɣ�����·����" + cDir, "��ʾ", MessageBoxButtons.OK,
                //         MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {

                //MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (pgss != null)
                    pgss.Close();
                if (fmPgs != null && !fmPgs.IsDisposed)
                    fmPgs.Dispose();


            }
        }
        #endregion

        #region ��״Ͻ��ͼ����������
        //���ɭ����Դ��״Ͻ��ͼ-����
        private void pageLayoutTDLYXQT(IMap inMap, IGeometry inGeometry, int inScale)
        {
            if (inMap == null)
                return;
            FrmTDLYMapSet_XQT tdlyXQT = new FrmTDLYMapSet_XQT(inScale, "");
            tdlyXQT.WriteLog = WriteLog;//ygc 2012-9-12 �Ƿ�д��־
            tdlyXQT.XZQname = pXZQMC;//����������
            if (tdlyXQT.ShowDialog() != DialogResult.OK)
                return;
            bool hasLegend = tdlyXQT.HasLegend;
            Dictionary<string, string> pTextEles = tdlyXQT.MapTextElements;
            IPageLayout tlPageLayout = getTemplateGra(Application.StartupPath + "\\..\\Template\\TDLYXQTTemplate.mxd");
            if (tlPageLayout == null)
                return;
            IGraphicsContainer tlGra = tlPageLayout as IGraphicsContainer;
            IPage tlPage = tlPageLayout.Page;
            double pageW = 0, pageH = 0;
            tlPage.QuerySize(out pageW, out pageH);//ģ��ֽ�Ŵ�С
            tlGra.Reset();
            IElement tlEle = tlGra.Next();
            IElement tlRC = null;
            while (tlEle != null)
            {
                IElementProperties pEP = tlEle as IElementProperties;
                if (pEP.Name == "RC")//���
                    tlRC = tlEle;
                tlEle = tlGra.Next();
            }
            IEnvelope tlRCEnv = tlRC.Geometry.Envelope as IEnvelope;
            if (tlRCEnv == null)
                return;
            //RC��ֽ�ŵľ�����������
            double[] rc2page = new double[4];
            rc2page[0] = tlRCEnv.XMin;
            rc2page[1] = tlRCEnv.YMin;
            rc2page[2] = pageW - tlRCEnv.XMax;
            rc2page[3] = pageH - tlRCEnv.YMax;
            IElement tlMF = tlGra.FindFrame((tlPageLayout as IActiveView).FocusMap) as IElement;
            IEnvelope tlMFEnv = tlMF.Geometry.Envelope as IEnvelope;
            if (tlMFEnv == null)
                return;
            //��ͼ��ܺ�ֽ�ŵľ���
            double[] mf2page = new double[4];
            mf2page[0] = tlMFEnv.XMin;
            mf2page[1] = tlMFEnv.YMin;
            mf2page[2] = pageW - tlMFEnv.XMax;
            mf2page[3] = pageH - tlMFEnv.YMax;

               //������
            SysCommon.CProgress pgss = new SysCommon.CProgress("���ڼ�����ͼ���棬���Ժ�...");
            pgss.EnableCancel = false;
            pgss.ShowDescription = false;
            pgss.FakeProgress = true;
            pgss.TopMost = true;
            pgss.ShowProgress();
            Application.DoEvents();
            try
            {
                IObjectCopy pOC = new ObjectCopyClass();
                IPageLayout workPageLayout = pOC.Copy(tlPageLayout) as IPageLayout;
                //IPageLayoutControl pPLC = new PageLayoutControlClass();
                //pPLC.PageLayout = workPageLayout;
                //������������Χ2����
                IEnvelope workEnv = new EnvelopeClass();
                IEnvelope pGeoEnv = inGeometry.Envelope;
                double xmin = pGeoEnv.XMin - 0.02 * inScale,
                    ymin = pGeoEnv.YMin - 0.02 * inScale,
                    xmax = pGeoEnv.XMax + 0.02 * inScale,
                    ymax = pGeoEnv.YMax + 0.02 * inScale;
                workEnv.PutCoords(xmin, ymin, xmax, ymax);
                workEnv.SpatialReference = inGeometry.SpatialReference;
                //���ݷ�Χ�ͱ����߼�ģ��߾࣬����ֽ�Ŵ�С
                IPage workPage = workPageLayout.Page;
                workPage.StretchGraphicsWithPage = false;
                double workPW = workEnv.Width * 100 / inScale + mf2page[0] + mf2page[2],
                    workPH = workEnv.Height * 100 / inScale + mf2page[1] + mf2page[3];
                workPage.PutCustomSize(workPW, workPH);//����ֽ�Ŵ�С��λ����
                IActiveView workActiveView = workPageLayout as IActiveView;
                pOC = new ObjectCopyClass();
                IMap newMap = pOC.Copy(inMap) as IMap;
                procLyrScale(newMap);//yjlȡ����������ʾ����
                IMap workMap = workActiveView.FocusMap;
                workMap.Name = pTextEles["����"];//
                workMap.SpatialReference = inMap.SpatialReference;
                IMapLayers pMapLayers = workMap as IMapLayers;
                for (int i = 0; i < newMap.LayerCount; i++)
                {
                    pMapLayers.InsertLayer(newMap.get_Layer(i), false, pMapLayers.LayerCount);
                }
                IActiveView workMapAV = workMap as IActiveView;

                workMapAV.Extent = workEnv;
                //workMap.ClipGeometry = pExtent;
                workMap.MapScale = inScale;
                
               
                IGraphicsContainer workGra = workPageLayout as IGraphicsContainer;
                IMapFrame pMapFrame = (IMapFrame)workGra.FindFrame(workActiveView.FocusMap);
                pMapFrame.ExtentType = esriExtentTypeEnum.esriExtentScale;
                pMapFrame.MapBounds = workEnv;
                IElement pMapEle = pMapFrame as IElement;
                IEnvelope pEnv = new EnvelopeClass();
                pEnv.PutCoords(mf2page[0], mf2page[1], workPW - mf2page[2], workPH - mf2page[3]);
                pMapEle.Geometry = pEnv;

                //���߲���
                IObjectCopy cpGeo = new ObjectCopyClass();
                IGeometry xzqPg = cpGeo.Copy(inGeometry) as IGeometry;

                IPointCollection pPC = xzqPg as IPointCollection;
                //xzqBound.SpatialReference = null;

                ITransform2D pT2D = xzqPg as ITransform2D;
                pT2D.Scale(pPC.get_Point(0), (double)100 / inScale, (double)100 / inScale);
                //ֽ�������ȥ��ͼ����õ�ƽ����
                double movX = (pEnv.XMin + (pPC.get_Point(0).X - workMapAV.Extent.XMin) * 100 / inScale) - pPC.get_Point(0).X,
                   movY = (pEnv.YMin + (pPC.get_Point(0).Y - workMapAV.Extent.YMin) * 100 / inScale) - pPC.get_Point(0).Y;
                pT2D.Move(movX, movY);
                xzqPg.SpatialReference = pMapEle.Geometry.SpatialReference;
                ////�ڸ�����������Ԫ��
                //ITopologicalOperator pTO1 = pMapEle.Geometry as ITopologicalOperator;
                //IGeometry pSurround = pTO1.Difference(xzqPg);
                //GeoPageLayoutFn.drawPolygonElement(pSurround, workGra, getRGB(255, 255, 255), false, getRGB(0, 0, 0), 1.5);
                //����
                ITopologicalOperator pTO = xzqPg as ITopologicalOperator;
                IPolyline xzqBound = pTO.Boundary as IPolyline;
                GeoPageLayoutFn.drawPolylineElement(xzqBound, workPageLayout as IGraphicsContainer, getRGB(0, 0, 255), 2);
          
                //���·�Χͼ��
                GeoPageLayoutFn.updateMapSymbol(workMap, workActiveView.Extent);
                
                //���ͼ��
                if (hasLegend)
                {
                    AddLegend(workPageLayout, workMap, workPW - mf2page[2] + 1, workPH - mf2page[3], 4, 1);
                    IMapSurroundFrame pLegend = getMapSurroundFrame(workGra, "ͼ��");
                    IObjectCopy pLC = new ObjectCopyClass();
                    ILegendFormat pLF = pLC.Copy((pLegend.MapSurround as ILegend).Format) as ILegendFormat;
                    pLC = new ObjectCopyClass();
                    ILegendClassFormat pLCF = pLC.Copy((pLegend.MapSurround as ILegend).get_Item(0).LegendClassFormat) as ILegendClassFormat;
                    delUnnecLegends(pLegend);//ȥ������ͼ��
                    (pLegend.MapSurround as ILegend).Format = pLF;
                    resizeLegend(workActiveView, pLCF, pLegend, workPW - mf2page[2] + 1, workPH - mf2page[3], 4);
                }
                IElement workRC = null;//���
                workGra.Reset();
                tlEle = workGra.Next();
                while (tlEle != null)
                {
                    IElementProperties pEP = tlEle as IElementProperties;
                    if (pEP.Name == "RC")//���
                        workRC = tlEle;
                    tlEle = workGra.Next();
                }
                IEnvelope pEnv2 = new EnvelopeClass();
                pEnv2.PutCoords(rc2page[0], rc2page[1], workPW - rc2page[2], workPH - rc2page[3]);
                workRC.Geometry = pEnv2;
                double[] offXY = new double[2];//0Xλ��2Yλ��
                offXY[0] = pEnv2.XMax - tlRCEnv.XMax;
                offXY[1] = pEnv2.YMax - tlRCEnv.YMax;
                moveElements(workGra, offXY);
                updateTextEle(workGra, pTextEles);
                CreateMeasuredGrid(workPageLayout);
                addCornerCoor(workGra, inGeometry);
                FrmPageLayout fmPL = new FrmPageLayout(workPageLayout);
                fmPL.WriteLog = WriteLog;//ygc 2012-9-12 �Ƿ�д��־
                fmPL.Show();
            }
            catch { }
            finally 
            {
                pgss.Close();
            }


        }
        //���ɭ����Դ��״Ͻ��ͼ-����
        private void batPageLayoutTDLYXQT(IMap inMap, string inZTMC, int inScale, Node inXZQnode)
        {
            FormProgress fmPgs = null;
            try
            {
                cDir = createDir(inXZQnode.Text + inZTMC + "����Ͻ��ͼ");
                if (cDir == "")
                    return;
                if (inMap == null)
                    return;
                if (!inXZQnode.HasChildNodes)
                    return;
                FrmTDLYMapSet_XQT tdlyXQT = new FrmTDLYMapSet_XQT(inScale, "");
                tdlyXQT.WriteLog = WriteLog;//ygc 2012-9-12�Ƿ�д��־
                if (tdlyXQT.ShowDialog() != DialogResult.OK)
                    return;
                bool hasLegend = tdlyXQT.HasLegend;
                Dictionary<string, string> pTextEles = tdlyXQT.MapTextElements;
                string title = pTextEles["����"];
                IPageLayout tlPageLayout = getTemplateGra(Application.StartupPath + "\\..\\Template\\TDLYXQTTemplate.mxd");
                if (tlPageLayout == null)
                    return;
                IGraphicsContainer tlGra = tlPageLayout as IGraphicsContainer;
                IPage tlPage = tlPageLayout.Page;
                double pageW = 0, pageH = 0;
                tlPage.QuerySize(out pageW, out pageH);//ģ��ֽ�Ŵ�С
                tlGra.Reset();
                IElement tlEle = tlGra.Next();
                IElement tlRC = null;
                while (tlEle != null)
                {
                    IElementProperties pEP = tlEle as IElementProperties;
                    if (pEP.Name == "RC")//���
                        tlRC = tlEle;
                    tlEle = tlGra.Next();
                }
                IEnvelope tlRCEnv = tlRC.Geometry.Envelope as IEnvelope;
                if (tlRCEnv == null)
                    return;
                //RC��ֽ�ŵľ�����������
                double[] rc2page = new double[4];
                rc2page[0] = tlRCEnv.XMin;
                rc2page[1] = tlRCEnv.YMin;
                rc2page[2] = pageW - tlRCEnv.XMax;
                rc2page[3] = pageH - tlRCEnv.YMax;
                IElement tlMF = tlGra.FindFrame((tlPageLayout as IActiveView).FocusMap) as IElement;
                IEnvelope tlMFEnv = tlMF.Geometry.Envelope as IEnvelope;
                if (tlMFEnv == null)
                    return;
                //��ͼ��ܺ�ֽ�ŵľ���
                double[] mf2page = new double[4];
                mf2page[0] = tlMFEnv.XMin;
                mf2page[1] = tlMFEnv.YMin;
                mf2page[2] = pageW - tlMFEnv.XMax;
                mf2page[3] = pageH - tlMFEnv.YMax;

                fmPgs = new FormProgress();
                ProgressBar pgsBar = fmPgs.progressBar1;
                pgsBar.Minimum = 1;
                pgsBar.Maximum = inXZQnode.Nodes.Count;
                pgsBar.Step = 1;
                fmPgs.TopLevel = true;
                fmPgs.Text = "�����������" + inXZQnode.Text + "ɭ����Դ��״Ͻ��ͼ";
                fmPgs.Show();
                foreach (Node advNode in inXZQnode.Nodes)
                {
                    fmPgs.lblOut.Text = "�ܹ�" + pgsBar.Maximum + "�������������������" + pgsBar.Value + "��������������Ϊ��" + advNode.Text;
                    IGeometry inGeometry = ModGetData.getExtentByXZQ(advNode);
                    IObjectCopy pOC = new ObjectCopyClass();
                    IPageLayout workPageLayout = pOC.Copy(tlPageLayout) as IPageLayout;
                    //IPageLayoutControl pPLC = new PageLayoutControlClass();
                    //pPLC.PageLayout = workPageLayout;
                    //������������Χ2����
                    IEnvelope workEnv = new EnvelopeClass();
                    IEnvelope pGeoEnv = inGeometry.Envelope;
                    double xmin = pGeoEnv.XMin - 0.02 * inScale,
                        ymin = pGeoEnv.YMin - 0.02 * inScale,
                        xmax = pGeoEnv.XMax + 0.02 * inScale,
                        ymax = pGeoEnv.YMax + 0.02 * inScale;
                    workEnv.PutCoords(xmin, ymin, xmax, ymax);
                    workEnv.SpatialReference = inGeometry.SpatialReference;
                    //���ݷ�Χ�ͱ����߼�ģ��߾࣬����ֽ�Ŵ�С
                    IPage workPage = workPageLayout.Page;
                    workPage.StretchGraphicsWithPage = false;
                    double workPW = workEnv.Width * 100 / inScale + mf2page[0] + mf2page[2],
                        workPH = workEnv.Height * 100 / inScale + mf2page[1] + mf2page[3];
                    workPage.PutCustomSize(workPW, workPH);//����ֽ�Ŵ�С��λ����
                    IActiveView workActiveView = workPageLayout as IActiveView;
                    IMap newMap = null;
                    bool isSpecial = ModGetData.IsMapSpecial();

                    if (isSpecial)//������ض�ר��
                    {
                        newMap = new MapClass();
                        ModGetData.AddMapOfByXZQ(newMap, "TDLY", inZTMC, inMap, advNode.Text);
                        if (newMap.LayerCount == 0)//���ǿյ�ͼ��ǰ��
                        {
                            if (this.WriteLog)
                            {
                                Plugin.LogTable.Writelog(advNode.Text + "û���ҵ�ͼ�㣡");
                            }
                            continue;
                        }
                        ModuleMap.LayersComposeEx(newMap);//ͼ������
                    }
                    else
                    {
                        IObjectCopy pOC1 = new ObjectCopyClass();
                        newMap = pOC1.Copy(inMap) as IMap;//���Ƶ�ͼ
                    }
                    if (newMap.LayerCount == 0)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "δ�ҵ�ͼ�㡣");
                        return;

                    }

                    string xzqdmFD = "";
                    //�������ߺ���Ⱦͼ��
                    IFeatureClass xzqFC = ModGetData.getFCByXZQ(advNode, ref xzqdmFD);
                    if (xzqFC != null && xzqdmFD != null)
                    {
                        ILayer hachureLyr = GeoPageLayoutFn.createHachureLyr(xzqFC, xzqdmFD, advNode.Name);
                        if (hachureLyr != null)
                        {
                            IMapLayers pMapLayers1 = newMap as IMapLayers;
                            IGroupLayer pGroupLayer = newMap.get_Layer(0) as IGroupLayer;
                            if (pGroupLayer != null)
                            {
                                pMapLayers1.InsertLayerInGroup(pGroupLayer, hachureLyr, false, 0);
                            }

                        }
                    }
                    pTextEles["����"] = advNode.Text + title;
                    //fmPgs.lblOut.Text = "�����������" + advNode.Text;
                    procLyrScale(newMap);//yjlȡ����������ʾ����
                    IMap workMap = workActiveView.FocusMap;
                    workMap.Name = "ɭ����Դ��״Ͻ��ͼ";
                    workMap.SpatialReference = inMap.SpatialReference;
                    IMapLayers pMapLayers = workMap as IMapLayers;
                    for (int i = 0; i < newMap.LayerCount; i++)
                    {
                        pMapLayers.InsertLayer(newMap.get_Layer(i), false, pMapLayers.LayerCount);
                    }
                    IActiveView workMapAV = workMap as IActiveView;

                    workMapAV.Extent = workEnv;
                    //workMap.ClipGeometry = pExtent;
                    workMap.MapScale = inScale;


                    IGraphicsContainer workGra = workPageLayout as IGraphicsContainer;
                    IMapFrame pMapFrame = (IMapFrame)workGra.FindFrame(workActiveView.FocusMap);
                    pMapFrame.ExtentType = esriExtentTypeEnum.esriExtentBounds;
                    pMapFrame.MapBounds = workEnv;
                    IElement pMapEle = pMapFrame as IElement;
                    IEnvelope pEnv = new EnvelopeClass();
                    pEnv.PutCoords(mf2page[0], mf2page[1], workPW - mf2page[2], workPH - mf2page[3]);
                    pMapEle.Geometry = pEnv;

                    //���߲���
                    IObjectCopy cpGeo = new ObjectCopyClass();
                    IGeometry xzqPg = cpGeo.Copy(inGeometry) as IGeometry;

                    IPointCollection pPC = xzqPg as IPointCollection;
                    //xzqBound.SpatialReference = null;

                    ITransform2D pT2D = xzqPg as ITransform2D;
                    pT2D.Scale(pPC.get_Point(0), (double)100 / inScale, (double)100 / inScale);
                    //ֽ�������ȥ��ͼ����õ�ƽ����
                    double movX = (pEnv.XMin + (pPC.get_Point(0).X - workMapAV.Extent.XMin) * 100 / inScale) - pPC.get_Point(0).X,
                       movY = (pEnv.YMin + (pPC.get_Point(0).Y - workMapAV.Extent.YMin) * 100 / inScale) - pPC.get_Point(0).Y;
                    pT2D.Move(movX, movY);
                    xzqPg.SpatialReference = pMapEle.Geometry.SpatialReference;
                    ////�ڸ�����������Ԫ��
                    //ITopologicalOperator pTO1 = pMapEle.Geometry as ITopologicalOperator;
                    //IGeometry pSurround = pTO1.Difference(xzqPg);
                    //GeoPageLayoutFn.drawPolygonElement(pSurround, workGra, getRGB(255, 255, 255), false, getRGB(0, 0, 0), 1.5);
                    //����
                    ITopologicalOperator pTO = xzqPg as ITopologicalOperator;
                    IPolyline xzqBound = pTO.Boundary as IPolyline;
                    GeoPageLayoutFn.drawPolylineElement(xzqBound, workPageLayout as IGraphicsContainer, getRGB(0, 0, 255), 2);

                    //���·�Χͼ��
                    GeoPageLayoutFn.updateMapSymbol(workMap, workActiveView.Extent);
                    
                    //���ͼ��
                    if (hasLegend)
                    {
                        AddLegend(workPageLayout, workMap, workPW - mf2page[2] + 1, workPH - mf2page[3], 4, 1);
                        IMapSurroundFrame pLegend = getMapSurroundFrame(workGra, "ͼ��");
                        IObjectCopy pLC = new ObjectCopyClass();
                        ILegendFormat pLF = pLC.Copy((pLegend.MapSurround as ILegend).Format) as ILegendFormat;
                        pLC = new ObjectCopyClass();
                        ILegendClassFormat pLCF = pLC.Copy((pLegend.MapSurround as ILegend).get_Item(0).LegendClassFormat) as ILegendClassFormat;
                        delUnnecLegends(pLegend);//ȥ������ͼ��
                        (pLegend.MapSurround as ILegend).Format = pLF;
                        resizeLegend(workActiveView, pLCF, pLegend, workPW - mf2page[2] + 1, workPH - mf2page[3], 4);
                    }
                    IElement workRC = null;//���
                    workGra.Reset();
                    tlEle = workGra.Next();
                    while (tlEle != null)
                    {
                        IElementProperties pEP = tlEle as IElementProperties;
                        if (pEP.Name == "RC")//���
                            workRC = tlEle;
                        tlEle = workGra.Next();
                    }
                    IEnvelope pEnv2 = new EnvelopeClass();
                    pEnv2.PutCoords(rc2page[0], rc2page[1], workPW - rc2page[2], workPH - rc2page[3]);
                    workRC.Geometry = pEnv2;
                    double[] offXY = new double[2];//0Xλ��2Yλ��
                    offXY[0] = pEnv2.XMax - tlRCEnv.XMax;
                    offXY[1] = pEnv2.YMax - tlRCEnv.YMax;
                    moveElements(workGra, offXY);
                    updateTextEle(workGra, pTextEles);
                    CreateMeasuredGrid(workPageLayout);
                    addCornerCoor(workGra, inGeometry);
                    workPageLayout.ZoomToWhole();


                    IMapDocument cMD = new MapDocumentClass();
                    string savePath = cDir + @"\" + advNode.Text + ".mxd";
                    cMD.New(savePath);
                    cMD.Open(savePath, "");
                    cMD.ReplaceContents(workPageLayout as IMxdContents);
                    cMD.Save(true, false);
                    cMD.Close();
                    cMD = null;
                    if (fmPgs.IsDisposed)
                    {
                        if (this.WriteLog)
                        {
                            Plugin.LogTable.Writelog("�û�ȡ������ɭ����Դ��״Ͻ��ͼ������������ͼ�����ڣ�" + cDir);
                        }
                        break;
                    }

                    pgsBar.PerformStep();
                    Application.DoEvents();
                }//foreach
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("����ɭ����Դ��״Ͻ��ͼ�����ɣ�����·����" + cDir);
                }
                //MessageBox.Show("����ɭ����Դ��״Ͻ��ͼ�����ɣ�����·����" + cDir, "��ʾ", MessageBoxButtons.OK,
                         //MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {

                //MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (fmPgs != null && !fmPgs.IsDisposed)
                    fmPgs.Dispose();
            }
        }
        //���µ�ͼ��ע
        private void updateTextEle(IGraphicsContainer inGra, Dictionary<string, string> inTexts)
        {
            foreach (KeyValuePair<string, string> kvp in inTexts)
            {
                IGraphicsContainer pgGC = inGra;
                pgGC.Reset();
                IElement pgEle = pgGC.Next();
                while (pgEle != null)
                {
                    if (pgEle is ITextElement)
                    {
                        ITextElement pTE=pgEle as ITextElement;
                        if ((pgEle as IElementProperties).Name == kvp.Key || (pgEle as ITextElement).Text == kvp.Key)
                        {
                            if (kvp.Value == "")
                            {
                                inGra.DeleteElement(pgEle);
                                inGra.Reset();
                            }
                            else
                            {
                                pTE.Text = kvp.Value;
                            }
                        }
                        if ((pgEle as IElementProperties).Name == "����")
                        {
                            if(inTexts.ContainsKey("����"))
                            {
                                if (inTexts["����"] == "")
                                {
                                    inGra.DeleteElement(pgEle);
                                    inGra.Reset();
                                }
                                else
                                    pTE.Text = inTexts["����"];
                            }
                        }

                    }
                    pgEle = pgGC.Next();
                }
            }
        }
        //��ȡģ�����ͼ����
        private IPageLayout getTemplateGra(string inPath)
        {
            IPageLayout res = null;
            if (!File.Exists(inPath))
                return null;//1
            else
            {
                IMapDocument pMD = new MapDocumentClass();
                pMD.Open(inPath, "");
                res = pMD.PageLayout;
                pMD.Close();
                return res;//2
            }
        }
        //����border
        private IBorder createBorder(IGeometry inGeometry, IActiveView inAV)
        {
            ISymbolBorder pSymbolBorder = new SymbolBorderClass();
            pSymbolBorder.GetGeometry(inAV.ScreenDisplay as IDisplay, inGeometry.Envelope);
            ILineSymbol pLSymbol = new SimpleLineSymbolClass();
            pLSymbol.Color = getRGB(0, 0, 0);
            pLSymbol.Width = 1;
            pSymbolBorder.LineSymbol = pLSymbol;


            return pSymbolBorder as IBorder;
        }
        //�ƶ��ܱ�Ԫ��
        private void moveElements(IGraphicsContainer inGra, double[] inOff)
        {
            inGra.Reset();
            IElement pEle = inGra.Next();
            while (pEle != null)
            {
                IElementProperties pEP = pEle as IElementProperties;
                if (pEP.Name.Contains("����"))
                    GeoPageLayoutFn.MoveElement(pEle, inOff[0], 0);
                else if (pEP.Name.Contains("����"))
                    GeoPageLayoutFn.MoveElement(pEle, 0, inOff[1]);
                else if (pEP.Name.Contains("����") || pEle is IPictureElement)
                    GeoPageLayoutFn.MoveElement(pEle, inOff[0], inOff[1]);
                else if (pEP.Name == "�滮����")
                    GeoPageLayoutFn.MoveElement(pEle, 0, inOff[1]);
                else if (pEP.Name == "����" || pEP.Name == "����")
                    GeoPageLayoutFn.MoveElement(pEle, inOff[0] / 2, inOff[1]);
                else if (pEP.Name == "������")
                    GeoPageLayoutFn.MoveElement(pEle, inOff[0] / 2, 0);
                inGra.UpdateElement(pEle);
                pEle = inGra.Next();
            }

        }
        //ʮ���ƾ�γ��ת�ַ���
        private void degreesTostring(double x, ref string deg, ref string min, ref string sec)
        {
            int degree = (int)Math.Floor(x);
            int minute = (int)Math.Floor((x - degree) * 60);
            int second = (int)Math.Floor(((x - degree) * 60 - minute) * 60);

            deg = degree + "��";
            min = minute + "��";
            sec = second + "��";

        }
        //�����ǵ�����
        private void addCornerCoor(IGraphicsContainer inGra, IGeometry inGeometry)
        {

            WKSPoint[] pPts = getPts(inGeometry);
            if (pPts == null)
                return;
            inGra.Reset();
            IElement plele = inGra.Next();
            while (plele != null)
            {
                if (plele is ITextElement)
                {
                    ITextElement pTE = plele as ITextElement;
                    switch (pTE.Text)
                    {
                        case "���½Ǿ���":
                            {
                                string d = "", m = "", s = "";
                                degreesTostring(pPts[0].X, ref d, ref m, ref s);
                                pTE.Text = d + m + s;
                                break;
                            }
                        case "���½�\r\nγ��":
                            {
                                string d = "", m = "", s = "";
                                degreesTostring(pPts[0].Y, ref d, ref m, ref s);
                                pTE.Text = d + "\r\n" + m + s;
                                break;
                            }
                        case "���½Ǿ���":
                            {
                                string d = "", m = "", s = "";
                                degreesTostring(pPts[1].X, ref d, ref m, ref s);
                                pTE.Text = d + m + s;
                                break;
                            }
                        case "���½�\r\nγ��":
                            {
                                string d = "", m = "", s = "";
                                degreesTostring(pPts[0].Y, ref d, ref m, ref s);
                                pTE.Text = d + "\r\n" + m + s;
                                break;
                            }
                        case "���ϽǾ���":
                            {
                                string d = "", m = "", s = "";
                                degreesTostring(pPts[0].X, ref d, ref m, ref s);
                                pTE.Text = d + m + s;
                                break;
                            }
                        case "���Ͻ�\r\nγ��":
                            {
                                string d = "", m = "", s = "";
                                degreesTostring(pPts[1].Y, ref d, ref m, ref s);
                                pTE.Text = d + "\r\n" + m + s;
                                break;
                            }
                        case "���ϽǾ���":
                            {
                                string d = "", m = "", s = "";
                                degreesTostring(pPts[1].X, ref d, ref m, ref s);
                                pTE.Text = d + m + s;
                                break;
                            }
                        case "���Ͻ�\r\nγ��":
                            {
                                string d = "", m = "", s = "";
                                degreesTostring(pPts[1].Y, ref d, ref m, ref s);
                                pTE.Text = d + "\r\n" + m + s;
                                break;
                            }


                    }
                }
                plele = inGra.Next();

            }


        }
        #endregion

   

        #region ������Χ�ַ����mxdʸ��
        public void pageLayoutExtentBatFFT(IMap pMap, Dictionary<string, List<int>> inDic, IFeatureClass inExtentFC, string inPath)
        {
            if (inDic.Count == 0)
                return;
            if (pMap.LayerCount == 0)
                return;
            if (inExtentFC == null)
                return;
            IFeatureClass jhtb = null;
            try
            {
                jhtb = ModGetData.GetFeatureClassByNodeKey(ModGetData.AttrValue("JHTB1W", "NodeKey"));
            }
            catch(Exception ex)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���������ļ�δ�ҵ�1��Ӻ�ͼ��㣡");
            }
            if (jhtb == null)
                return;
            IObjectCopy pOC = new ObjectCopyClass();
            IMap newMap = pOC.Copy(pMap) as IMap;
            IMapLayers newMapLyrs = newMap as IMapLayers;
            for (int i = 0; i < newMap.LayerCount; i++)
            {
                ILayer pLyr = newMap.get_Layer(i);
                if (pLyr.Name.Contains("Ӱ��"))
                {
                    newMap.DeleteLayer(pLyr);
                    newMapLyrs.InsertLayer(pLyr, false, newMapLyrs.LayerCount);
                }
            }
            IEnumerator<string> enm = inDic.Keys.GetEnumerator();
            enm.Reset();
            string key = "";
            if(enm.MoveNext())
                key=enm.Current;
            FrmSheetMapSetExtent_ZT tdlyXQT = new FrmSheetMapSetExtent_ZT(10000, "", key);
            tdlyXQT.WriteLog = WriteLog;//ygc 2012-9-12 �Ƿ�д��־
            tdlyXQT.Jhtb = jhtb;
            IList<string> fdLst = new List<string>();
            for (int fdx = 0; fdx < inExtentFC.Fields.FieldCount; fdx++)
            {
                if (inExtentFC.Fields.get_Field(fdx).Type != esriFieldType.esriFieldTypeGeometry)
                {
                    fdLst.Add(inExtentFC.Fields.get_Field(fdx).Name);
                }

            }
            tdlyXQT.LstFields = fdLst;//�ֶμ���

            List<ILayer> lyrs = new List<ILayer>();
            IList<ILayer> pgLyrs = new List<ILayer>();//��㼯��

            for (int i = 0; i < newMap.LayerCount; i++)
            {
                ILayer pLyr = newMap.get_Layer(i);
                addLayer(pLyr, ref lyrs);
            }
            foreach (ILayer lyr in lyrs)
            {
                if (lyr is IFeatureLayer)
                {
                    IFeatureLayer pFL = lyr as IFeatureLayer;
                    if (pFL.FeatureClass == null)
                        continue;
                    if (pFL.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                        pgLyrs.Add(lyr);
                }
            }
            tdlyXQT.LstPolygonLyrs = pgLyrs;

            tdlyXQT.ExtentFeature = inExtentFC.Search(null, false).NextFeature();

            if (tdlyXQT.ShowDialog() != DialogResult.OK)//�û����浯��
                return;
            if (tdlyXQT.LstResPolygonLyrs != null)
                pgLyrs = tdlyXQT.LstResPolygonLyrs;//��㼯�ϣ�Ϊ�����û�δ����
            //����������͸��
            if (pgLyrs != null)
            {
                foreach (ILayer lyr in pgLyrs)
                {
                    setNoSymbol(lyr);
                }
            }
            FormProgress fmPgs = null;
            try
            {
                string fcName = (inExtentFC as IDataset).Name;


                ILayer extenLayer = makeLayer(inExtentFC);
                extenLayer.Name = "��������";
                IMapLayers pMapLayers = newMap as IMapLayers;
                pMapLayers.InsertLayer(extenLayer, false, 0);
                fmPgs = new FormProgress();
                ProgressBar pgsBar = fmPgs.progressBar1;
                pgsBar.Minimum = 1;
                pgsBar.Maximum = inDic.Count + 1;
                pgsBar.Step = 1;
                fmPgs.TopLevel = true;
                fmPgs.Text = "�����������" + fcName + "�ļ������ķ�Χ����ͼ";
                fmPgs.Show();
                //int i = 1;
                string OID = inExtentFC.OIDFieldName;
                foreach (KeyValuePair<string, List<int>> kvp in inDic)
                {
                    fmPgs.lblOut.Text = "�ܹ�" + inDic.Count.ToString() + "���ĵ������������" + pgsBar.Value + "��";
                    string lyrDef = OID + " IN(";
                    tdlyXQT.MapTextElements["��ͼ����"] = kvp.Key;
                    if (tdlyXQT.MapTextElements["�¾�ͼ����"] == "old")
                    {
                        string whr = "MAPNUMBER='", tfh = kvp.Key;
                        if (tfh.Contains(" "))
                            whr += tfh + "'";
                        else
                            whr += tfh.Insert(3, " ").Insert(5, " ") + "'";
                        tdlyXQT.MapTextElements["ͼ����"] = getValueOfFeature(whr, "MAPNUMBER_OLD", tdlyXQT.Jhtb);
                    }
                    else
                        tdlyXQT.MapTextElements["ͼ����"] = kvp.Key;
                    foreach (int oid in kvp.Value)
                    {
                        lyrDef += oid.ToString() + ",";


                    }
                    lyrDef = lyrDef.Substring(0, lyrDef.Length - 1);
                    lyrDef += ")";
                    IFeatureLayerDefinition pFLD = extenLayer as IFeatureLayerDefinition;
                    pFLD.DefinitionExpression = lyrDef;
                    pageLayoutExtentZTFFTforBat(newMap, tdlyXQT, inPath, kvp.Value, inExtentFC);
                    //i++;
                    if (fmPgs.IsDisposed)
                    {
                        if (this.WriteLog)
                        {
                            Plugin.LogTable.Writelog("�û�ȡ������" + fcName + "�ļ������ķ�Χ����ͼ������������ͼ�����ڣ�" + inPath);
                        }
                        break;
                    }

                    pgsBar.PerformStep();
                    Application.DoEvents();
                }
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("������Χͼ�����ɣ�����·����" + inPath);
                }
                //MessageBox.Show("������Χͼ�����ɣ�����·����" + cDir, "��ʾ", MessageBoxButtons.OK,
                //         MessageBoxIcon.Information);
            }
            catch { }
            finally
            {
                if (fmPgs != null && !fmPgs.IsDisposed)
                    fmPgs.Dispose();
            }
        }
        //������Χ����������
        private void pageLayoutExtentZTFFTforBat(IMap inMap, FrmSheetMapSetExtent_ZT inFmSet, string inPath
            , List<int> oids, IFeatureClass inFC)
        {
            FrmSheetMapSetExtent_ZT tdlyXQT = inFmSet;
            IList<string> lblFds = tdlyXQT.LstResFields;//��ע���ϣ�Ϊ�����û�δ����
            IList<ILayer> pgLyrs = null;
            if (tdlyXQT.LstResPolygonLyrs != null)
                pgLyrs = tdlyXQT.LstResPolygonLyrs;
            else
                pgLyrs = tdlyXQT.LstPolygonLyrs;
            string subHeadFd = tdlyXQT.ResSubHeadFields;//�������ֶ�
            bool hasSubHead = tdlyXQT.HasSubHead;//�Ƿ���ʾ������
            bool hasLegend = tdlyXQT.HasLegend;
            Dictionary<string, string> pTextEles = tdlyXQT.MapTextElements;
            int inScale = Convert.ToInt32(pTextEles["������"].Split(':')[1]);
            string subHead = "";
            if (hasSubHead)
            {
                string whr = "", tfh = pTextEles["ͼ����"];
                if (pTextEles["�¾�ͼ����"] == "new")
                {
                    whr = "MAPNUMBER='";
                    if (tfh.Contains(" "))
                        whr += tfh + "'";
                    else
                        whr += tfh.Insert(3, " ").Insert(5, " ") + "'";
                }
                else
                    whr = "MAPNUMBER_OLD='" + tfh + "'";
               

                subHead = getValueOfFeature(whr, "MAPNAME", tdlyXQT.Jhtb);
                pTextEles["����"] = subHead + "\r\n" + pTextEles["ͼ����"];
            }
            else
                pTextEles["����"] = pTextEles["ͼ����"];
            IPageLayout workPageLayout = new PageLayoutClass();
            IPageLayoutControl pPLC = new PageLayoutControlClass();
            pPLC.PageLayout = workPageLayout;
            
            IActiveView workActiveView = workPageLayout as IActiveView;
            //pOC = new ObjectCopyClass();
            //IMap newMap = pOC.Copy(inMap) as IMap;
            IMap newMap = inMap;//��ͼ���ã����ǿ�����������
            procLyrScale(newMap);//yjlȡ����������ʾ����
            IMap workMap = workActiveView.FocusMap;
            workMap.Name = pTextEles["����"];
            workMap.SpatialReference = inMap.SpatialReference;
            IMapLayers pMapLayers = workMap as IMapLayers;
            for (int i = 0; i < newMap.LayerCount; i++)
            {
                pMapLayers.InsertLayer(newMap.get_Layer(i), false, pMapLayers.LayerCount);
            }
            //���ɷַ�ͼ��ʽ
            pageLayoutTDLYFFT(pPLC, pTextEles, 1);
            IGeometry inGeometry=workMap.ClipGeometry;
            IGraphicsContainer workGra = workPageLayout as IGraphicsContainer;
            IMapFrame pMapFrame = (IMapFrame)workGra.FindFrame(workActiveView.FocusMap);
            IElement pMapEle = pMapFrame as IElement;
            IEnvelope pEnv = pMapEle.Geometry.Envelope;
            //���·�Χͼ��--��������Ⱦ����
            GeoPageLayoutFn.updateMapSymbol(inMap, inGeometry);
            double x = pMapEle.Geometry.Envelope.XMax;//��ͼ��ܵ����ϵ�����
            double y = pMapEle.Geometry.Envelope.YMax;
            //���ͼ��
           
            if (hasLegend)
            {
                AddLegend(pPLC.PageLayout, pPLC.ActiveView.FocusMap, x + 1, y, 6, 1);
                IMapSurroundFrame pLegend = getMapSurroundFrame(workGra, "ͼ��");
                IObjectCopy pLC = new ObjectCopyClass();
                ILegendFormat pLF = pLC.Copy((pLegend.MapSurround as ILegend).Format) as ILegendFormat;
                pLC = new ObjectCopyClass();
                ILegendClassFormat pLCF = pLC.Copy((pLegend.MapSurround as ILegend).get_Item(0).LegendClassFormat) as ILegendClassFormat;
                if (pgLyrs != null)
                {
                    if (pgLyrs.Count != 0)
                        delUnnecLegends(pLegend, pgLyrs);//ȥ��͸��ͼ��
                }
                sortLegend(pLegend);
                (pLegend as IElement).Activate(workActiveView.ScreenDisplay);//����ͼ��
                delUnnecLegendItemInfo(pLegend);
                (pLegend.MapSurround as ILegend).Format = pLF;

                resizeLegend(workPageLayout as IActiveView,pLCF, pLegend, x + 1, y, 6);

            }
            //else
            //   workGra.DeleteElement(pLegend as IElement);
           
           
            bool hasBL = false;
            if (tdlyXQT.HasBootLine && inFC.ShapeType == esriGeometryType.esriGeometryPolygon)//�������й�
                hasBL = true;
            foreach (int oid in oids)
            {
                IFeature pFeature = inFC.GetFeature(oid);
                IEnvelope pEnvFea = pFeature.ShapeCopy.Envelope;

                //IPoint fromPoint = new PointClass();
                //fromPoint.X = pEnv.XMin + ((pEnvFea.XMax+pEnvFea.XMin)/2 - inGeometry.Envelope.XMin) * 100 / inScale;
                //fromPoint.Y = pEnv.YMin + ((pEnvFea.YMax + pEnvFea.YMin) / 2 - inGeometry.Envelope.YMin) * 100 / inScale;




                IPoint p = new PointClass();
                p.X = pEnv.XMin + (pFeature.ShapeCopy.Envelope.XMax - inGeometry.Envelope.XMin) * 100 / inScale;
                p.Y = pEnv.YMin + (pFeature.ShapeCopy.Envelope.YMin - inGeometry.Envelope.YMin) * 100 / inScale;

                IPoint fromPoint = null;
                if (hasBL)
                {
                    IArea pArea = pFeature.ShapeCopy as IArea;
                    fromPoint = pArea.LabelPoint;
                    fromPoint.X = pEnv.XMin + (fromPoint.X - inGeometry.Envelope.XMin) * 100 / inScale;
                    fromPoint.Y = pEnv.YMin + (fromPoint.Y - inGeometry.Envelope.YMin) * 100 / inScale;
                    IPolyline pLine = new PolylineClass();//����
                    pLine.FromPoint = fromPoint;
                    pLine.ToPoint = p;
                    GeoPageLayoutFn.drawPolylineElement(pLine, workGra, getRGB(255, 0, 0), 1);
                }

                string strEle = "";
                //���ñ�ע
                if (lblFds != null)
                {
                    foreach (string fd in lblFds)
                    {
                        int fdx = inFC.FindField(fd);
                        strEle += pFeature.get_Value(fdx).ToString() + "\r\n";
                    }
                    strEle = strEle.Substring(0, strEle.Length - 2);
                    GeoPageLayoutFn.drawTextElement(p, workGra, strEle);
                }
                //���ø�����
               
            }//foreach oid
           
            //if (!tdlyXQT.HasNorthArrow)
            //    delNorthArrow(workGra);
            updateTextEle(workGra, pTextEles);
            IMapDocument cMD = new MapDocumentClass();
            string savePath = inPath + "\\";
            if (pTextEles["����"] != "")
                savePath += pTextEles["����"].Replace("\r\n","") + ".mxd";
            else if (pTextEles["����"] != "")
                savePath += pTextEles["����"] + ".mxd";
            else
                savePath += "noname.mxd";
            cMD.New(savePath);
            cMD.Open(savePath, "");
            workPageLayout.ZoomToWhole();
            //FrmPageLayout fmPl = new FrmPageLayout(workPageLayout);
            //fmPl.Show();
            cMD.ReplaceContents(workPageLayout as IMxdContents);
            cMD.Save(true, false);
            cMD.Close();
        }
        //���ɷַ���ʽ
        private  void pageLayoutTDLYFFT(IPageLayoutControl axPageLayoutControl1, Dictionary<string, string> pgTextElements, int typeZT)
        {
            ISpatialReference pSpatialRefrence = axPageLayoutControl1.ActiveView.FocusMap.SpatialReference;
            GeoDrawSheetMap.clsDrawSheetMap pDrawSheetMap = new GeoDrawSheetMap.clsDrawSheetMap();
            pDrawSheetMap.vPageLayoutControl = axPageLayoutControl1 as IPageLayoutControl;
            pDrawSheetMap.vScale = Convert.ToUInt32(pgTextElements["������"].Substring(2));
            pDrawSheetMap.m_intPntCount = 3;
            pDrawSheetMap.type_ZT = typeZT;//ר������
            pDrawSheetMap.m_pPrjCoor = axPageLayoutControl1.ActiveView.FocusMap.SpatialReference;
            string[] astrMapNo = pgTextElements["��ͼ����"].Split(' ');//��ͼ���Ŵ��ո� ��ȥ�ո�
            string realMapNo = "";
            foreach (string str in astrMapNo)
            {
                realMapNo += str;
            }
            pDrawSheetMap.m_strSheetNo = realMapNo;//ͼ����

            //����ͼ���Ż��ͶӰ�ļ�
            if (axPageLayoutControl1.ActiveView.FocusMap.SpatialReference is IProjectedCoordinateSystem)
            {
                pDrawSheetMap.m_pPrjCoor = axPageLayoutControl1.ActiveView.FocusMap.SpatialReference;
            }
            else
            {
                ISpatialReference pSpa = GetSpatialByMapNO(pDrawSheetMap.m_strSheetNo);
                if (pSpa == null)
                {
                    MessageBox.Show("�����õ�ͼ��ͶӰ���꣡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                pDrawSheetMap.m_pPrjCoor = pSpa;
                axPageLayoutControl1.ActiveView.FocusMap.SpatialReference = pSpa;
            }

            pDrawSheetMap.DrawSheetMap();

            foreach (KeyValuePair<string, string> kvp in pgTextElements)
            {
                IGraphicsContainer pgGC = axPageLayoutControl1.GraphicsContainer;
                pgGC.Reset();
                IElement pgEle = pgGC.Next();
                while (pgEle != null)
                {
                    if (pgEle is ITextElement)
                    {
                        if((pgEle as ITextElement).Text == kvp.Key||(pgEle as IElementProperties).Name==kvp.Key)
                        {
                            if(kvp.Value!="")
                               (pgEle as ITextElement).Text = kvp.Value;
                            else
                            {
                                pgGC.DeleteElement(pgEle);
                                pgGC.Reset();
                            }

                        }


                    }
                    pgEle = pgGC.Next();
                }
            }

            IGraphicsContainerSelect pGCS = axPageLayoutControl1.PageLayout as IGraphicsContainerSelect;
            if (pGCS.ElementSelectionCount != 0)
            {
                pGCS.UnselectAllElements();

            }
        }

        // ����ͼ���Ż��ͶӰ�ļ�   
        private ISpatialReference GetSpatialByMapNO(string strMapNO)
        {
            //double dblX = 0;
            //double dblY = 0;

            // GeoDrawSheetMap.basPageLayout.GetCoordinateFromNewCode(strMapNO, ref dblX, ref dblY);

            //dblX = dblX / 3600;
            //dblY = dblY / 3600;
            string strtfh = "";
            try
            {
                strtfh = strMapNO.Substring(1, 2);
                int tfh = Convert.ToInt32(strtfh);
                tfh = tfh * 6 - 180 - 3;
                strtfh = "CGCS 2000 GK CM " + tfh.ToString() + "E.prj";
            }
            catch
            {

            }
            string strPrjFileName = Application.StartupPath + "\\..\\Prj\\CGCS2000\\" + strtfh;
            if (!System.IO.File.Exists(strPrjFileName)) return null;

            ISpatialReferenceFactory pSpaFac = new SpatialReferenceEnvironmentClass();
            return pSpaFac.CreateESRISpatialReferenceFromPRJFile(strPrjFileName);

        }
        //����ĳ�ֶλ�ȡ��һ�ֶε�ֵ
        private string getValueOfFeature(string whereclause, string desField,IFeatureClass inFC)
        {
            string res = "";
            IQueryFilter pQF = new QueryFilterClass();
            pQF.WhereClause = whereclause;
            ITable pTable = inFC as ITable;
            ICursor pCursor = pTable.Search(pQF, false);
            IRow pRow = pCursor.NextRow();
            int fdx = pTable.FindField(desField);
            if (fdx != -1 && pRow != null)
            {
                res = pRow.get_Value(fdx).ToString();

            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
            pCursor = null;
            pRow = null;
            return res;

        }
        #endregion

        #region ������Χ���mxdʸ��
        public void pageLayoutExtentBat(IMap pMap, Dictionary<IGeometry, List<int>> inDic,IFeatureClass inExtentFC,string inPath)
        {
            if (inDic.Count == 0)
                return;
            if (pMap.LayerCount == 0)
                return;
            if (inExtentFC == null)
                return;
            IObjectCopy pOC = new ObjectCopyClass();
            IMap newMap = pOC.Copy(pMap) as IMap;
            IMapLayers newMapLyrs = newMap as IMapLayers;
            for (int i = 0; i < newMap.LayerCount; i++)
            {
                ILayer pLyr = newMap.get_Layer(i);
                if (pLyr.Name.Contains("Ӱ��"))
                {
                    newMap.DeleteLayer(pLyr);
                    newMapLyrs.InsertLayer(pLyr, false, newMapLyrs.LayerCount);
                }
            }
            FrmExtentZTMapSetBat tdlyXQT = new FrmExtentZTMapSetBat();
            tdlyXQT.WriteLog = WriteLog; //ygc 2012-9-12 �Ƿ�д��־
            IList<string> fdLst = new List<string>();
            for (int fdx = 0; fdx < inExtentFC.Fields.FieldCount; fdx++)
            {
                if (inExtentFC.Fields.get_Field(fdx).Type != esriFieldType.esriFieldTypeGeometry)
                {
                    fdLst.Add(inExtentFC.Fields.get_Field(fdx).Name);
                }
 
            }
            tdlyXQT.LstFields = fdLst;//�ֶμ���
          
            List<ILayer> lyrs = new List<ILayer>();
            IList<ILayer> pgLyrs=new List<ILayer>();//��㼯��

            for (int i = 0; i < newMap.LayerCount; i++)
            {
                ILayer pLyr = newMap.get_Layer(i);
                addLayer(pLyr, ref lyrs);
            }
            foreach (ILayer lyr in lyrs)
            {
                if(lyr is IFeatureLayer)
                {
                    IFeatureLayer pFL=lyr as IFeatureLayer;
                    if (pFL.FeatureClass == null)
                        continue;
                    if(pFL.FeatureClass.ShapeType==esriGeometryType.esriGeometryPolygon)
                        pgLyrs.Add(lyr);
                }
            }
            tdlyXQT.LstPolygonLyrs=pgLyrs;
            
            tdlyXQT.ExtentFeature = inExtentFC.Search(null,false).NextFeature();

            if (tdlyXQT.ShowDialog() != DialogResult.OK)//�û����浯��
                return;
            if (tdlyXQT.LstResPolygonLyrs != null)
            {
                pgLyrs = tdlyXQT.LstResPolygonLyrs;//��㼯�ϣ�Ϊ�����û�δ����
                //����������͸��
                if (pgLyrs != null)
                {
                    foreach (ILayer lyr in pgLyrs)
                    {
                        setNoSymbol(lyr);
                    }
                }
            }
            FormProgress fmPgs = null;
            try
            {
                string fcName = (inExtentFC as IDataset).Name;
                
               
                ILayer extenLayer = makeLayer(inExtentFC);
                extenLayer.Name = "��������";
                IMapLayers pMapLayers = newMap as IMapLayers;
                pMapLayers.InsertLayer(extenLayer, false, 0);
                fmPgs = new FormProgress();
                ProgressBar pgsBar = fmPgs.progressBar1;
                pgsBar.Minimum = 1;
                pgsBar.Maximum = inDic.Count+1;
                pgsBar.Step = 1;
                fmPgs.TopLevel = true;
                fmPgs.Text = "�����������" + fcName + "�ļ������ķ�Χ����ͼ";
                fmPgs.Show();
                //int i = 1;
                string OID = inExtentFC.OIDFieldName;
                foreach (KeyValuePair<IGeometry, List<int>> kvp in inDic)
                {
                    fmPgs.lblOut.Text = "�ܹ�" + inDic.Count.ToString() + "���ĵ������������" + pgsBar.Value + "��";
                    string lyrDef = OID+" IN(";
                    foreach (int oid in kvp.Value)
                    {
                        lyrDef += oid.ToString()+",";


                    }
                    lyrDef = lyrDef.Substring(0, lyrDef.Length - 1);
                    lyrDef += ")";
                    IFeatureLayerDefinition pFLD = extenLayer as IFeatureLayerDefinition;
                    pFLD.DefinitionExpression = lyrDef;
                    pageLayoutExtentZTforBat(newMap, kvp.Key, tdlyXQT,inPath,kvp.Value,inExtentFC);
                    //i++;
                    if (fmPgs.IsDisposed)
                    {
                        if (this.WriteLog)
                        {
                            Plugin.LogTable.Writelog("�û�ȡ������" + fcName + "�ļ������ķ�Χ����ͼ������������ͼ�����ڣ�" + inPath);
                        }
                        break;
                    }

                    pgsBar.PerformStep();
                    Application.DoEvents();
                }
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("������Χͼ�����ɣ�����·����" + inPath);
                }
                //MessageBox.Show("������Χͼ�����ɣ�����·����" + cDir, "��ʾ", MessageBoxButtons.OK,
                //         MessageBoxIcon.Information);
            }
            catch { }
            finally
            {
                if (fmPgs != null && !fmPgs.IsDisposed)
                    fmPgs.Dispose();
            }
        }
        //ʶ��ֽ��
        private esriPageFormID getPageID(short portrait, int w)
        {
            esriPageFormID PFI = esriPageFormID.esriPageFormCUSTOM;
            if (portrait==1)
            {
                switch (w)
                {
                    case 841:
                        PFI = esriPageFormID.esriPageFormA0;
                        break;
                    case 594:
                        PFI = esriPageFormID.esriPageFormA1;
                        break;
                    case 420:
                        PFI = esriPageFormID.esriPageFormA2;
                        break;
                    case 297:
                        PFI = esriPageFormID.esriPageFormA3;
                        break;
                    case 210:
                        PFI = esriPageFormID.esriPageFormA4;
                        break;
                    case 148:
                        PFI = esriPageFormID.esriPageFormA5;
                        break;

                }

            }
            else
            {
                switch (w)
                {
                    case 1189:
                        PFI = esriPageFormID.esriPageFormA0;
                        break;
                    case 841:
                        PFI = esriPageFormID.esriPageFormA1;
                        break;
                    case 594:
                        PFI = esriPageFormID.esriPageFormA2;
                        break;
                    case 420:
                        PFI = esriPageFormID.esriPageFormA3;
                        break;
                    case 297:
                        PFI = esriPageFormID.esriPageFormA4;
                        break;
                    case 210:
                        PFI = esriPageFormID.esriPageFormA5;
                        break;

                }
 
            }
            return PFI;
        }
        //���췶Χ����ͼ��
        private ILayer makeLayer(IFeatureClass inFC)
        {
            IFeatureLayer pLayer = new FeatureLayerClass();
            pLayer.FeatureClass = inFC;
            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();
            //��ɫ����
            IRgbColor pRGBColor = new RgbColorClass();
            pRGBColor.UseWindowsDithering = false;
            ISymbol pSymbol = (ISymbol)pFillSymbol;
            //pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

            pRGBColor.Red = 255;
            pRGBColor.Green = 0;
            pRGBColor.Blue = 0;
            pLineSymbol.Color = pRGBColor;

            pLineSymbol.Width = 2;
            //pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            pFillSymbol.Outline = pLineSymbol;
            pRGBColor.Transparency = 0;
            pFillSymbol.Color = pRGBColor;
            ISimpleRenderer pSimpleRenderer=new SimpleRendererClass();
            if (inFC.ShapeType == esriGeometryType.esriGeometryPolyline)
                pSimpleRenderer.Symbol = pLineSymbol as ISymbol;
            else if (inFC.ShapeType == esriGeometryType.esriGeometryPolygon)
                pSimpleRenderer.Symbol = pFillSymbol as ISymbol;
            IGeoFeatureLayer pGeoLyr = pLayer as IGeoFeatureLayer;
            pGeoLyr.Renderer = pSimpleRenderer as IFeatureRenderer;
            return pLayer as ILayer;

        }
        //������Χ����������
        private void pageLayoutExtentZTforBat(IMap inMap, IGeometry inGeometry,FrmExtentZTMapSetBat inFmSet,string inPath
            ,List<int> oids,IFeatureClass inFC)
        {
            FrmExtentZTMapSetBat tdlyXQT=inFmSet;
            IList<string> lblFds = tdlyXQT.LstResFields;//��ע���ϣ�Ϊ�����û�δ����
            IList<ILayer> pgLyrs=null;
            if (tdlyXQT.LstResPolygonLyrs != null)
                pgLyrs = tdlyXQT.LstResPolygonLyrs;
            else
                pgLyrs = tdlyXQT.LstPolygonLyrs;
            string subHeadFd = tdlyXQT.ResSubHeadFields;//�������ֶ�
            bool hasSubHead = tdlyXQT.HasSubHead;//�Ƿ���ʾ������
            bool hasLegend = tdlyXQT.HasLegend;
            Dictionary<string, string> pTextEles = tdlyXQT.MapTextElements;
            int inScale = Convert.ToInt32(pTextEles["������"].Split(':')[1]);
            IPageLayout tlPageLayout = getTemplateGra(Application.StartupPath + "\\..\\Template\\BatExtentTemplate.mxd");
            if (tlPageLayout == null)
                return;
            IGraphicsContainer tlGra = tlPageLayout as IGraphicsContainer;
            IPage tlPage = tlPageLayout.Page;
            double pageW = 0, pageH = 0;
            tlPage.QuerySize(out pageW, out pageH);//ģ��ֽ�Ŵ�С
            tlGra.Reset();
            IElement tlEle = tlGra.Next();
            IElement tlRC = null;
            while (tlEle != null)
            {
                IElementProperties pEP = tlEle as IElementProperties;
                if (pEP.Name == "RC")//���
                    tlRC = tlEle;
                tlEle = tlGra.Next();
            }
            IEnvelope tlRCEnv = tlRC.Geometry.Envelope as IEnvelope;
            if (tlRCEnv == null)
                return;
            //RC��ֽ�ŵľ�����������
            double[] rc2page = new double[4];
            rc2page[0] = tlRCEnv.XMin;
            rc2page[1] = tlRCEnv.YMin;
            rc2page[2] = pageW - tlRCEnv.XMax;
            rc2page[3] = pageH - tlRCEnv.YMax;
            IElement tlMF = tlGra.FindFrame((tlPageLayout as IActiveView).FocusMap) as IElement;
            IEnvelope tlMFEnv = tlMF.Geometry.Envelope as IEnvelope;
            if (tlMFEnv == null)
                return;
            //��ͼ��ܺ�ֽ�ŵľ���
            double[] mf2page = new double[4];
            mf2page[0] = tlMFEnv.XMin;
            mf2page[1] = tlMFEnv.YMin;
            mf2page[2] = pageW - tlMFEnv.XMax;
            mf2page[3] = pageH - tlMFEnv.YMax;
            IObjectCopy pOC = new ObjectCopyClass();
            IPageLayout workPageLayout = pOC.Copy(tlPageLayout) as IPageLayout;
            //IPageLayoutControl pPLC = new PageLayoutControlClass();
            //pPLC.PageLayout = workPageLayout;
            IPage workPage = workPageLayout.Page;
            IEnvelope workEnv = inGeometry.Envelope;
            workPage.StretchGraphicsWithPage = false;
            double workPW = workEnv.Width * 100 / inScale + mf2page[0] + mf2page[2],
                workPH = workEnv.Height * 100 / inScale + mf2page[1] + mf2page[3];
            esriPageFormID pfi = getPageID(workPage.Orientation, (int)(workPW * 10));
            if (pfi != esriPageFormID.esriPageFormCUSTOM)
                workPage.FormID = pfi;
            else
                workPage.PutCustomSize(workPW, workPH);//����ֽ�Ŵ�С��λ����
            IActiveView workActiveView = workPageLayout as IActiveView;
            //pOC = new ObjectCopyClass();
            //IMap newMap = pOC.Copy(inMap) as IMap;
            IMap newMap = inMap;//��ͼ���ã����ǿ�����������
            procLyrScale(newMap);//yjlȡ����������ʾ����
            IMap workMap = workActiveView.FocusMap;
            workMap.Name = pTextEles["����"];
            workMap.SpatialReference = inMap.SpatialReference;
            IMapLayers pMapLayers = workMap as IMapLayers;
            for (int i = 0; i < newMap.LayerCount; i++)
            {
                pMapLayers.InsertLayer(newMap.get_Layer(i), false, pMapLayers.LayerCount);
            }
            IActiveView workMapAV = workMap as IActiveView;
            workMapAV.Extent = inGeometry.Envelope;
            workMap.ClipGeometry = inGeometry.Envelope;
            workMap.MapScale = inScale;
            //ITopologicalOperator pTO = inGeometry as ITopologicalOperator;
            //IPolyline xzqBound = pTO.Boundary as IPolyline;
            //GeoPageLayoutFn.drawPolylineElement(xzqBound, workMap as IGraphicsContainer);
            IGraphicsContainer workGra = workPageLayout as IGraphicsContainer;
            IMapFrame pMapFrame = (IMapFrame)workGra.FindFrame(workActiveView.FocusMap);
            pMapFrame.ExtentType = esriExtentTypeEnum.esriExtentBounds;
            pMapFrame.MapBounds = inGeometry.Envelope;
            IElement pMapEle = pMapFrame as IElement;
            IEnvelope pEnv = new EnvelopeClass();
            pEnv.PutCoords(mf2page[0], mf2page[1], workPW - mf2page[2], workPH - mf2page[3]);
            pMapEle.Geometry = pEnv;
            //���·�Χͼ��--��������Ⱦ����
            GeoPageLayoutFn.updateMapSymbol(inMap, inGeometry);
            IMapSurroundFrame pLegend = getMapSurroundFrame(workGra, "ͼ��");
            //���ͼ��
            if (hasLegend)
            {
                //AddLegend2(workPageLayout, workMap, workPW - mf2page[2] + 0.5, workPH - mf2page[3], 3, 1,pgLyrs);
                if (pgLyrs != null)
                {
                    if (pgLyrs.Count != 0)
                        delUnnecLegends(pLegend, pgLyrs);//ȥ��͸��ͼ��
                }
                //delUnnecLegendItemInfo(pLegend);
                sortLegend(pLegend);
                (pLegend as IElement).Activate(workActiveView.ScreenDisplay);//����ͼ��

            }
            else
                workGra.DeleteElement(pLegend as IElement);
            IElement workRC = null;//���
            workGra.Reset();
            tlEle = workGra.Next();
            while (tlEle != null)
            {
                IElementProperties pEP = tlEle as IElementProperties;
                if (pEP.Name == "RC")//���
                    workRC = tlEle;
                tlEle = workGra.Next();
            }
            IEnvelope pEnv2 = new EnvelopeClass();
            pEnv2.PutCoords(rc2page[0], rc2page[1], workPW - rc2page[2], workPH - rc2page[3]);
            workRC.Geometry = pEnv2;
            double[] offXY = new double[2];//0Xλ��2Yλ��
            offXY[0] = pEnv2.XMax - tlRCEnv.XMax;
            offXY[1] = pEnv2.YMax - tlRCEnv.YMax;
            string subHead = "";
            bool hasBL=false;
            if (tdlyXQT.HasBootLine && inFC.ShapeType == esriGeometryType.esriGeometryPolygon)//�������й�
                hasBL = true;
            foreach (int oid in oids)
            {
                IFeature pFeature = inFC.GetFeature(oid);
                IEnvelope pEnvFea = pFeature.ShapeCopy.Envelope;
                
                //IPoint fromPoint = new PointClass();
                //fromPoint.X = pEnv.XMin + ((pEnvFea.XMax+pEnvFea.XMin)/2 - inGeometry.Envelope.XMin) * 100 / inScale;
                //fromPoint.Y = pEnv.YMin + ((pEnvFea.YMax + pEnvFea.YMin) / 2 - inGeometry.Envelope.YMin) * 100 / inScale;


              

                IPoint p = new PointClass();
                p.X = pEnv.XMin + (pFeature.ShapeCopy.Envelope.XMax - inGeometry.Envelope.XMin) * 100 / inScale;
                p.Y = pEnv.YMin + (pFeature.ShapeCopy.Envelope.YMin - inGeometry.Envelope.YMin) * 100 / inScale;

                IPoint fromPoint = null;
                if (hasBL)
                {
                    IArea pArea = pFeature.ShapeCopy as IArea;
                    fromPoint = pArea.LabelPoint;
                    fromPoint.X = pEnv.XMin + (fromPoint.X - inGeometry.Envelope.XMin) * 100 / inScale;
                    fromPoint.Y = pEnv.YMin + (fromPoint.Y - inGeometry.Envelope.YMin) * 100 / inScale;
                    IPolyline pLine = new PolylineClass();//����
                    pLine.FromPoint = fromPoint;
                    pLine.ToPoint = p;
                    GeoPageLayoutFn.drawPolylineElement(pLine, workGra, getRGB(255, 0, 0), 1);
                }
               
                string strEle = "";
                //���ñ�ע
                if (lblFds != null)
                {
                    foreach (string fd in lblFds)
                    {
                        int fdx = inFC.FindField(fd);
                        strEle += pFeature.get_Value(fdx).ToString() + "\r\n";
                    }
                    strEle = strEle.Substring(0, strEle.Length - 2);
                    GeoPageLayoutFn.drawTextElement(p, workGra, strEle);
                }
                //���ø�����
                if (hasSubHead)
                {
                    int idx = inFC.FindField(subHeadFd);
                    subHead += pFeature.get_Value(idx).ToString() + ",";
                }
            }//foreach oid
            if (subHead != "")
            {
                subHead = subHead.Substring(0, subHead.Length - 1);
                pTextEles["����"] = subHead;
            }
            moveElements(workGra, offXY);
            updateTextEle(workGra, pTextEles);
            if (!tdlyXQT.HasNorthArrow)
                delNorthArrow(workGra);
            IMapDocument cMD = new MapDocumentClass();
            string savePath = inPath+"\\";
            if (hasSubHead && pTextEles["����"] != "")
                savePath += pTextEles["����"] + ".mxd";
            else if (pTextEles["����"] != "")
                savePath += pTextEles["����"] + ".mxd";
            else
                savePath += "noname.mxd";
            cMD.New(savePath);
            cMD.Open(savePath, "");
            workPageLayout.ZoomToWhole();
            //FrmPageLayout fmPl = new FrmPageLayout(workPageLayout);
            //fmPl.Show();
            cMD.ReplaceContents(workPageLayout as IMxdContents);
            cMD.Save(true, false);
            cMD.Close();
        }
        //����������͸��
        private void setNoSymbol(ILayer inLayer)
        {
            IGeoFeatureLayer pGFL = inLayer as IGeoFeatureLayer;
            if (pGFL == null)
                return;
            IFeatureLayer pFL = pGFL as IFeatureLayer;
            IFeatureClass inFC = pFL.FeatureClass;
            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();
            //��ɫ����
            IRgbColor pRGBColor = new RgbColorClass();
            pRGBColor.UseWindowsDithering = false;
            ISymbol pSymbol = (ISymbol)pFillSymbol;
            //pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

            pRGBColor.Red = 255;
            pRGBColor.Green = 255;
            pRGBColor.Blue = 255;
            pRGBColor.Transparency = 0;
            pLineSymbol.Color = pRGBColor;
            
            pLineSymbol.Width = 4;
            //pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            pFillSymbol.Outline = pLineSymbol;
            
            pFillSymbol.Color = pRGBColor;
            ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();
            if (inFC.ShapeType == esriGeometryType.esriGeometryPolyline)
                pSimpleRenderer.Symbol = pLineSymbol as ISymbol;
            else if (inFC.ShapeType == esriGeometryType.esriGeometryPolygon)
                pSimpleRenderer.Symbol = pFillSymbol as ISymbol;
            pGFL.Renderer = pSimpleRenderer as IFeatureRenderer;
        }
        #endregion

        #region ������Χ���mxdդ��
        public void pageLayoutExtentRasterBat(IMap pMap, Dictionary<IGeometry, List<int>> inDic, IFeatureClass inExtentFC, string inPath)
        {
            if (inDic.Count == 0)
                return;
            if (pMap.LayerCount == 0)
                return;
            if (inExtentFC == null)
                return;
            IObjectCopy pOC = new ObjectCopyClass();
            IMap newMap = pOC.Copy(pMap) as IMap;
            FrmExtentZTRasterMapSetBat tdlyXQT = new FrmExtentZTRasterMapSetBat();
            tdlyXQT.WriteLog = WriteLog;//ygc 2012-9-12  �Ƿ�д��־
            tdlyXQT.SourceMap = newMap;
            IList<string> fdLst = new List<string>();
            for (int fdx = 0; fdx < inExtentFC.Fields.FieldCount; fdx++)
            {
                if (inExtentFC.Fields.get_Field(fdx).Type != esriFieldType.esriFieldTypeGeometry)
                    fdLst.Add(inExtentFC.Fields.get_Field(fdx).Name);

            }
            tdlyXQT.LstFields = fdLst;//�ֶμ���
          
           
            if (tdlyXQT.ShowDialog() != DialogResult.OK)//�û����浯��
                return;
            if (tdlyXQT.DesMap.LayerCount == 0)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("δ�ҵ�դ��ͼ�㣡","");
            }
            newMap = tdlyXQT.DesMap;
            FormProgress fmPgs = null;
            try
            {
                string fcName = (inExtentFC as IDataset).Name;


                ILayer extenLayer = makeLayer(inExtentFC);
                extenLayer.Name = "��������";
                IMapLayers pMapLayers = newMap as IMapLayers;
                pMapLayers.InsertLayer(extenLayer, false, 0);
                fmPgs = new FormProgress();
                ProgressBar pgsBar = fmPgs.progressBar1;
                pgsBar.Minimum = 1;
                pgsBar.Maximum = inDic.Count + 1;
                pgsBar.Step = 1;
                fmPgs.TopLevel = true;
                fmPgs.Text = "�����������" + fcName + "�ļ������ķ�Χ����ͼ";
                fmPgs.Show();
                int i = 1;
                string OID = inExtentFC.OIDFieldName;
                foreach (KeyValuePair<IGeometry, List<int>> kvp in inDic)
                {
                    fmPgs.lblOut.Text = "�ܹ�" + inDic.Count.ToString() + "���ĵ������������" + pgsBar.Value + "��";
                    string lyrDef = OID + " IN(";
                    foreach (int oid in kvp.Value)
                    {
                        lyrDef += oid.ToString() + ",";


                    }
                    lyrDef = lyrDef.Substring(0, lyrDef.Length - 1);
                    lyrDef += ")";
                    IFeatureLayerDefinition pFLD = extenLayer as IFeatureLayerDefinition;
                    pFLD.DefinitionExpression = lyrDef;
                    //string path = inPath + "\\" + i.ToString() + ".mxd";
                    pageLayoutExtentRasterZTforBat(newMap, kvp.Key, tdlyXQT, inPath, kvp.Value, inExtentFC);
                    i++;
                    if (fmPgs.IsDisposed)
                    {
                        if (this.WriteLog)
                        {
                            Plugin.LogTable.Writelog("�û�ȡ������" + fcName + "�ļ������ķ�Χ����ͼ������������ͼ�����ڣ�" + inPath);
                        }
                        break;
                    }

                    pgsBar.PerformStep();
                    Application.DoEvents();
                }
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("������Χͼ�����ɣ�����·����" + inPath);
                }
                //MessageBox.Show("������Χͼ�����ɣ�����·����" + cDir, "��ʾ", MessageBoxButtons.OK,
                //         MessageBoxIcon.Information);
            }
            catch { }
            finally
            {
                if (fmPgs != null && !fmPgs.IsDisposed)
                    fmPgs.Dispose();
            }
        }
       
        //������Χ����������
        private void pageLayoutExtentRasterZTforBat(IMap inMap, IGeometry inGeometry, FrmExtentZTRasterMapSetBat inFmSet, string inPath
            , List<int> oids, IFeatureClass inFC)
        {
            FrmExtentZTRasterMapSetBat tdlyXQT = inFmSet;
            IList<string> lblFds = tdlyXQT.LstResFields;//��ע���ϣ�Ϊ�����û�δ����
            
            //int inScale = Convert.ToInt32(pTextEles["������"].Split(':')[1]);
            int inScale = 10000;
            IPageLayout tlPageLayout = getTemplateGra(Application.StartupPath + "\\..\\Template\\BatExtentRasterTemplate.mxd");
            if (tlPageLayout == null)
                return;
           
            IObjectCopy pOC = new ObjectCopyClass();
            IPageLayout workPageLayout = pOC.Copy(tlPageLayout) as IPageLayout;
            //IPageLayoutControl pPLC = new PageLayoutControlClass();
            //pPLC.PageLayout = workPageLayout;
            IPage workPage = workPageLayout.Page;
            IEnvelope workEnv = inGeometry.Envelope;
            double workPW = workEnv.Width * 100 / inScale,
               workPH = workEnv.Height * 100 / inScale;
            workPage.StretchGraphicsWithPage = false;
            esriPageFormID pfi = getPageID(workPage.Orientation, (int)(workPW * 10));
            if (pfi != esriPageFormID.esriPageFormCUSTOM)
                workPage.FormID = pfi;
            else
                workPage.PutCustomSize(workPW, workPH);//����ֽ�Ŵ�С��λ����
            IGraphicsContainer workGra = workPageLayout as IGraphicsContainer;
            IActiveView workActiveView = workPageLayout as IActiveView;
            //pOC = new ObjectCopyClass();
            //IMap newMap = pOC.Copy(inMap) as IMap;
            IMap newMap = inMap;//��ͼ���ã����ǿ�����������
            procLyrScale(newMap);//yjlȡ����������ʾ����
            IMap workMap = workActiveView.FocusMap;
            workMap.SpatialReference = inMap.SpatialReference;
            IMapLayers pMapLayers = workMap as IMapLayers;
            for (int i = 0; i < newMap.LayerCount; i++)
            {
                pMapLayers.InsertLayer(newMap.get_Layer(i), false, pMapLayers.LayerCount);
            }
            IActiveView workMapAV = workMap as IActiveView;
            workMapAV.Extent = inGeometry.Envelope;
            workMap.ClipGeometry = inGeometry.Envelope;
            workMap.MapScale = inScale;

            IMapFrame pMapFrame = (IMapFrame)workGra.FindFrame(workActiveView.FocusMap);
            pMapFrame.ExtentType = esriExtentTypeEnum.esriExtentBounds;
            pMapFrame.MapBounds = inGeometry.Envelope;
            IElement pMapEle = pMapFrame as IElement;
            IEnvelope pEnv = new EnvelopeClass();
            pEnv.PutCoords(0, 0, workPW, workPH);
            pMapEle.Geometry = pEnv;

            bool hasBL = false;
            if (tdlyXQT.HasBootLine && inFC.ShapeType == esriGeometryType.esriGeometryPolygon)//�������й�
                hasBL = true;
            string mxdname = "";
            int idx = inFC.FindField("DKBH");
            foreach (int oid in oids)
            {
                IFeature pFeature = inFC.GetFeature(oid);
                IPoint p = new PointClass();
                p.X = (pFeature.ShapeCopy.Envelope.XMax - inGeometry.Envelope.XMin) * 100 / inScale;
                p.Y = (pFeature.ShapeCopy.Envelope.YMin - inGeometry.Envelope.YMin) * 100 / inScale;
                IPoint fromPoint = null;
                if (hasBL)
                {
                    IArea pArea = pFeature.ShapeCopy as IArea;
                    fromPoint = pArea.LabelPoint;
                    fromPoint.X = pEnv.XMin + (fromPoint.X - inGeometry.Envelope.XMin) * 100 / inScale;
                    fromPoint.Y = pEnv.YMin + (fromPoint.Y - inGeometry.Envelope.YMin) * 100 / inScale;
                    IPolyline pLine = new PolylineClass();//����
                    pLine.FromPoint = fromPoint;
                    pLine.ToPoint = p;
                    GeoPageLayoutFn.drawPolylineElement(pLine, workGra, getRGB(255, 0, 0), 1);
                }

                string strEle = "";
                //���ñ�ע
                if (lblFds != null)
                {
                    foreach (string fd in lblFds)
                    {
                        int fdx = inFC.FindField(fd);
                        strEle += pFeature.get_Value(fdx).ToString() + "\r\n";
                    }
                    strEle = strEle.Substring(0, strEle.Length - 2);
                    GeoPageLayoutFn.drawTextElement(p, workGra, strEle);
                }
                
                if (idx != -1)
                    mxdname += pFeature.get_Value(idx).ToString() + ",";
                else
                    mxdname += oid + ",";
              
            }//foreach oid
            if (mxdname != "")
                mxdname = mxdname.Substring(0, mxdname.Length - 1);
            IMapDocument cMD = new MapDocumentClass();
            string savePath = inPath +"\\";
            if (mxdname != "")
                savePath += mxdname + ".mxd"; 

            cMD.New(savePath);
            cMD.Open(savePath, "");
            workPageLayout.ZoomToWhole();
            //FrmPageLayout fmPl = new FrmPageLayout(workPageLayout);
            //fmPl.Show();
            cMD.ReplaceContents(workPageLayout as IMxdContents);
            cMD.Save(true, false);
            cMD.Close();
        }

        #endregion

        #region Ͻ���ܹ�ͼ
      
        public void pageLayoutZTGHTXQT(IMap inMap, IGeometry inGeometry,string xzqName)
        {
            if (inMap == null)
                return;
            FrmTDLYGHMapSet tdlyXQT = new FrmTDLYGHMapSet();
            tdlyXQT.WriteLog = WriteLog;//ygc 2012-9-12 �Ƿ�д��־
            tdlyXQT.XZQname = xzqName;
            if (tdlyXQT.ShowDialog() != DialogResult.OK)
                return;
            bool hasLegend = tdlyXQT.HasLegend;
            Dictionary<string, string> pTextEles = tdlyXQT.MapTextElements;
            int inScale = Convert.ToInt32(pTextEles["������"].Split(':')[1]);
            IPageLayout tlPageLayout = getTemplateGra(Application.StartupPath + "\\..\\Template\\TDLYGHTTemplate.mxd");
            if (tlPageLayout == null)
                return;
            IGraphicsContainer tlGra = tlPageLayout as IGraphicsContainer;
            IPage tlPage = tlPageLayout.Page;
            double pageW = 0, pageH = 0;
            tlPage.QuerySize(out pageW, out pageH);//ģ��ֽ�Ŵ�С
            tlGra.Reset();
            IElement tlEle = tlGra.Next();
            IElement tlRC = null;
            while (tlEle != null)
            {
                IElementProperties pEP = tlEle as IElementProperties;
                if (pEP.Name == "RC")//���
                    tlRC = tlEle;
                tlEle = tlGra.Next();
            }
            IEnvelope tlRCEnv = tlRC.Geometry.Envelope as IEnvelope;
            if (tlRCEnv == null)
                return;
            //RC��ֽ�ŵľ�����������
            double[] rc2page = new double[4];
            rc2page[0] = tlRCEnv.XMin;
            rc2page[1] = tlRCEnv.YMin;
            rc2page[2] = pageW - tlRCEnv.XMax;
            rc2page[3] = pageH - tlRCEnv.YMax;
            IElement tlMF = tlGra.FindFrame((tlPageLayout as IActiveView).FocusMap) as IElement;
            IEnvelope tlMFEnv = tlMF.Geometry.Envelope as IEnvelope;
            if (tlMFEnv == null)
                return;
            //��ͼ��ܺ�ֽ�ŵľ���
            double[] mf2page = new double[4];
            mf2page[0] = tlMFEnv.XMin;
            mf2page[1] = tlMFEnv.YMin;
            mf2page[2] = pageW - tlMFEnv.XMax;
            mf2page[3] = pageH - tlMFEnv.YMax;
            //������
            SysCommon.CProgress pgss = new SysCommon.CProgress("���ڼ�����ͼ���棬���Ժ�...");
            pgss.EnableCancel = false;
            pgss.ShowDescription = false;
            pgss.FakeProgress = true;
            pgss.TopMost = true;
            pgss.ShowProgress();
            Application.DoEvents();
            try
            {
                IObjectCopy pOC = new ObjectCopyClass();
                IPageLayout workPageLayout = pOC.Copy(tlPageLayout) as IPageLayout;
                //IPageLayoutControl pPLC = new PageLayoutControlClass();
                //pPLC.PageLayout = workPageLayout;

                //������������Χ2����
                IEnvelope workEnv = new EnvelopeClass();
                IEnvelope pGeoEnv = inGeometry.Envelope;
                double xmin = pGeoEnv.XMin - 0.02 * inScale,
                    ymin = pGeoEnv.YMin - 0.02 * inScale,
                    xmax = pGeoEnv.XMax + 0.02 * inScale,
                    ymax = pGeoEnv.YMax + 0.02 * inScale;
                workEnv.PutCoords(xmin, ymin, xmax, ymax);
                workEnv.SpatialReference = inGeometry.SpatialReference;
                //���ݷ�Χ�ͱ����߼�ģ��߾࣬����ֽ�Ŵ�С
                IPage workPage = workPageLayout.Page;
                workPage.StretchGraphicsWithPage = false;
                double workPW = workEnv.Width * 100 / inScale + mf2page[0] + mf2page[2],
                    workPH = workEnv.Height * 100 / inScale + mf2page[1] + mf2page[3];
                workPage.PutCustomSize(workPW, workPH);//����ֽ�Ŵ�С��λ����
                IActiveView workActiveView = workPageLayout as IActiveView;
                pOC = new ObjectCopyClass();
                IMap newMap = pOC.Copy(inMap) as IMap;
                procLyrScale(newMap);//yjlȡ����������ʾ����
                IMap workMap = workActiveView.FocusMap;
                workMap.Name = pTextEles["����"];
                workMap.SpatialReference = inMap.SpatialReference;
                IMapLayers pMapLayers = workMap as IMapLayers;
                for (int i = 0; i < newMap.LayerCount; i++)
                {
                    pMapLayers.InsertLayer(newMap.get_Layer(i), false, pMapLayers.LayerCount);
                }
                IActiveView workMapAV = workMap as IActiveView;
                workMapAV.Extent = workEnv;
                //workMap.ClipGeometry = pExtent;
                workMap.MapScale = inScale;
                IGraphicsContainer workGra = workPageLayout as IGraphicsContainer;
                IMapFrame pMapFrame = (IMapFrame)workGra.FindFrame(workActiveView.FocusMap);
                pMapFrame.ExtentType = esriExtentTypeEnum.esriExtentBounds;
                pMapFrame.MapBounds = workEnv;
                IElement pMapEle = pMapFrame as IElement;
                IEnvelope pEnv = new EnvelopeClass();
                pEnv.PutCoords(mf2page[0], mf2page[1], workPW - mf2page[2], workPH - mf2page[3]);
                pMapEle.Geometry = pEnv;

                //���߲���
                IObjectCopy cpGeo = new ObjectCopyClass();
                IGeometry xzqPg = cpGeo.Copy(inGeometry) as IGeometry;

                IPointCollection pPC = xzqPg as IPointCollection;
                //xzqBound.SpatialReference = null;

                ITransform2D pT2D = xzqPg as ITransform2D;
                pT2D.Scale(pPC.get_Point(0), (double)100 / inScale, (double)100 / inScale);
                //ֽ�������ȥ��ͼ����õ�ƽ����
                double movX = (pEnv.XMin + (pPC.get_Point(0).X - workMapAV.Extent.XMin) * 100 / inScale) - pPC.get_Point(0).X,
                   movY = (pEnv.YMin + (pPC.get_Point(0).Y - workMapAV.Extent.YMin) * 100 / inScale) - pPC.get_Point(0).Y;
                pT2D.Move(movX, movY);
                xzqPg.SpatialReference = pMapEle.Geometry.SpatialReference;
                ////�ڸ�����������Ԫ��
                //ITopologicalOperator pTO1 = pMapEle.Geometry as ITopologicalOperator;
                //IGeometry pSurround = pTO1.Difference(xzqPg);
                //GeoPageLayoutFn.drawPolygonElement(pSurround, workGra, getRGB(255, 255, 255), false, getRGB(0, 0, 0), 1.5);
                //����
                ITopologicalOperator pTO = xzqPg as ITopologicalOperator;
                IPolyline xzqBound = pTO.Boundary as IPolyline;
                GeoPageLayoutFn.drawPolylineElement(xzqBound, workPageLayout as IGraphicsContainer, getRGB(0, 0, 255), 2);

                //���·�Χͼ��
                GeoPageLayoutFn.updateMapSymbol(workMap, workActiveView.Extent);

                //���ͼ��
                if (hasLegend)
                {
                    GeoPageLayoutFn.AddLegendZT(workPageLayout, workMap, workPW - mf2page[2], mf2page[1], 8, 2);
                    IMapSurroundFrame pLegend = getMapSurroundFrame(workGra, "ͼ��");
                    IObjectCopy pLC = new ObjectCopyClass();
                    ILegendFormat pLF = pLC.Copy((pLegend.MapSurround as ILegend).Format) as ILegendFormat;
                    pLC = new ObjectCopyClass();
                    ILegendClassFormat pLCF = pLC.Copy((pLegend.MapSurround as ILegend).get_Item(0).LegendClassFormat) as ILegendClassFormat;
                    delUnnecLegends(pLegend);//ȥ������ͼ��
                    (pLegend.MapSurround as ILegend).Format = pLF;
                     resizeLegendGH(workActiveView, pLCF, pLegend,workPW - mf2page[2], mf2page[1], 8,2);

                }
                IElement workRC = null;//���
                workGra.Reset();
                tlEle = workGra.Next();
                while (tlEle != null)
                {
                    IElementProperties pEP = tlEle as IElementProperties;
                    if (pEP.Name == "RC")//���
                        workRC = tlEle;
                    tlEle = workGra.Next();
                }
                IEnvelope pEnv2 = new EnvelopeClass();
                pEnv2.PutCoords(rc2page[0], rc2page[1], workPW - rc2page[2], workPH - rc2page[3]);
                workRC.Geometry = pEnv2;
                double[] offXY = new double[2];//0Xλ��2Yλ��
                offXY[0] = pEnv2.XMax - tlRCEnv.XMax;
                offXY[1] = pEnv2.YMax - tlRCEnv.YMax;
                moveElements(workGra, offXY);
                updateTextEle(workGra, pTextEles);
                CreateMeasuredGrid(workPageLayout);
                addCornerCoor(workGra, inGeometry);
                FrmPageLayout fmPL = new FrmPageLayout(workPageLayout);
                fmPL.WriteLog = WriteLog;//ygc 2012-9-12 �Ƿ�д��־
                fmPL.Show();
            }
            catch { }
            finally
            {
                pgss.Close();
            }
        }
        #endregion

        private IRgbColor getRGB(int r, int g, int b)
        {
            IRgbColor pColor;
            pColor = new RgbColorClass();
            pColor.Red = r;
            pColor.Green = g;
            pColor.Blue = b;
            return pColor;
        }
        //����ͼ����ɾ������ͼ����Ĭ����ͼ��Ϊһ��ר��
        private void delUnnecLegends(IMapSurroundFrame inMSF)
        {
            ILegend2 pLegend = inMSF.MapSurround as ILegend2;
            if (pLegend == null)
                return;
            IMap pMap = pLegend.Map;
            if (pMap.LayerCount < 1)
                return;
            List<ILayer> lyrs = new List<ILayer>();
            for (int i = 1; i < pMap.LayerCount; i++)
            {
                ILayer pLyr = pMap.get_Layer(i);
                addLayer(pLyr, ref lyrs);
            }
            for (int i = 0; i < pLegend.ItemCount; i++)
            {
                ILegendItem pLegendItem = pLegend.get_Item(i);
                ILayer tmp = pLegendItem.Layer;
                if (tmp.Name == "����")
                {
                    pLegend.RemoveItem(i);
                    break;
                }
            }
            foreach (ILayer lyr in lyrs)
            {
                for (int i = 0; i < pLegend.ItemCount; i++)
                {
                    ILegendItem pLegendItem = pLegend.get_Item(i);
                    ILayer tmp = pLegendItem.Layer;
            
                    IFeatureLayer pFeatureLayer = tmp as IFeatureLayer;
                    if (pFeatureLayer != null)
                    {
                        IFeatureClass pFC = pFeatureLayer.FeatureClass;
                        if (pFC != null)
                        {
                            IDataset pDataset = pFC as IDataset;
                            if (pDataset.Name.Contains("XZQH") || pDataset.Name.Contains("PDT"))
                            {
                                pLegend.RemoveItem(i);//ȥ�����������¶�ͼ
                                break;

                            }
                        }
                    }
                    if (lyr.Equals(tmp))
                    {
                        pLegend.RemoveItem(i);
                        break;
                    }
                    
                   
                }

            }


        }
        //����ͼ����ɾ��͸��ͼ��
        private void delUnnecLegends(IMapSurroundFrame inMSF, IList<ILayer> lyrs)
        {
            ILegend2 pLegend = inMSF.MapSurround as ILegend2;
          
            if (pLegend == null)
                return;
          
            foreach (ILayer lyr in lyrs)
            {
                for (int i = 0; i < pLegend.ItemCount; i++)
                {
                    ILegendItem pLegendItem = pLegend.get_Item(i);
                    ILayer tmp = pLegendItem.Layer;

                    if (lyr.Equals(tmp))
                    {
                        pLegend.RemoveItem(i);
                        break;
                    }
                }
            }
        }
        //����ͼ����ȥ��ͼ�����ͼ�����ͱ�ͷ
        private void delUnnecLegendItemInfo(IMapSurroundFrame inMSF)
        {
            ILegend2 pLegend = inMSF.MapSurround as ILegend2;
           
            if (pLegend == null)
                return;

      
            for (int i = 0; i < pLegend.ItemCount; i++)
            {
                ILegendItem pLegendItem = pLegend.get_Item(i);
                pLegendItem.ShowDescriptions = false;
                pLegendItem.ShowHeading = false;
                pLegendItem.ShowLayerName = false;
               
            }
            
        }
        //����ͼ����ͼ�����ʱ仯������ͼ���Ĵ�С
        private void resizeLegend(IActiveView inAV,ILegendClassFormat inLCF, IMapSurroundFrame inMSF, double posX, double posY, double legW)
        {
            ILegend2 pLegend = inMSF.MapSurround as ILegend2;
            pLegend.Refresh();
            if (pLegend == null)
                return;
            pLegend.Format.DefaultPatchHeight = 12;
            pLegend.Format.DefaultPatchWidth = 24;
            pLegend.Format.VerticalPatchGap = 10;
            for (int i = 0; i < pLegend.ItemCount; i++)
            {
                ILegendItem pLegendItem = pLegend.get_Item(i);
                pLegendItem.LegendClassFormat.LabelSymbol=inLCF.LabelSymbol;
            }

            IElement element = inMSF as IElement;
            //Get aspect ratio
            ESRI.ArcGIS.Carto.IQuerySize querySize = inMSF.MapSurround as ESRI.ArcGIS.Carto.IQuerySize; // Dynamic Cast
            System.Double w = 0;
            System.Double h = 0;
            querySize.QuerySize(ref w, ref h);
            System.Double aspectRatio = w / h;

            ESRI.ArcGIS.Geometry.IEnvelope envelope = new ESRI.ArcGIS.Geometry.EnvelopeClass();
            envelope.PutCoords(posX, (posY - legW / aspectRatio), (posX + legW), posY);
            element.Geometry = envelope;
            IElementProperties3 pep = element as IElementProperties3;
            pep.AnchorPoint = esriAnchorPointEnum.esriTopLeftCorner;
            element.Activate(inAV.ScreenDisplay);
            inAV.Refresh();

        }
        //����ͼ����ͼ�����ʱ仯������ͼ���Ĵ�С--�滮Ͻ��ͼ
        private void resizeLegendGH(IActiveView inAV, ILegendClassFormat inLCF, IMapSurroundFrame inMSF, double posX, double posY, double legW,int col)
        {
            ILegend2 pLegend = inMSF.MapSurround as ILegend2;
            pLegend.Refresh();
            if (pLegend == null)
                return;
            pLegend.Format.DefaultPatchHeight = 12;
            pLegend.Format.DefaultPatchWidth = 24;
            pLegend.Format.VerticalPatchGap = 10;
            pLegend.Format.TitleSymbol.HorizontalAlignment = esriTextHorizontalAlignment.esriTHACenter;
            for (int i = 0; i < pLegend.ItemCount; i++)
            {
                ILegendItem pLegendItem = pLegend.get_Item(i);
                pLegendItem.LegendClassFormat.LabelSymbol = inLCF.LabelSymbol;
            }
            pLegend.AdjustColumns(col);
            IElement element = inMSF as IElement;
            //Get aspect ratio
            ESRI.ArcGIS.Carto.IQuerySize querySize = inMSF.MapSurround as ESRI.ArcGIS.Carto.IQuerySize; // Dynamic Cast
            System.Double w = 0;
            System.Double h = 0;
            querySize.QuerySize(ref w, ref h);
            System.Double aspectRatio = w / h;

            ESRI.ArcGIS.Geometry.IEnvelope envelope = new ESRI.ArcGIS.Geometry.EnvelopeClass();
            envelope.PutCoords(posX - legW, posY, posX, (posY + legW / aspectRatio));
            element.Geometry = envelope;
            IElementProperties3 pep = element as IElementProperties3;
            pep.AnchorPoint = esriAnchorPointEnum.esriBottomRightCorner;
            element.Activate(inAV.ScreenDisplay);
            inAV.Refresh();

        }
        //����ͼ������ͼ�����������ߵ�,ͬʱɾ��ͼ�����ͼ�����ͱ�ͷ
        private void sortLegend(IMapSurroundFrame inMSF)
        {
            ILegend2 pLegend = inMSF.MapSurround as ILegend2;

            if (pLegend == null)
                return;
            //��������Ƶ�����
            for (int i = 0; i < pLegend.ItemCount; i++)
            {
                ILegendItem pLegendItem = pLegend.get_Item(i);
                pLegendItem.ShowDescriptions = false;
                pLegendItem.ShowHeading = false;
                pLegendItem.ShowLayerName = false;

                ILayer tmp = pLegendItem.Layer;
                if (!tmp.Valid)
                {
                    pLegend.RemoveItem(i);
                }
                IFeatureLayer pFeatureLayer = tmp as IFeatureLayer;
                if (pFeatureLayer != null)
                {

                    if (pFeatureLayer.FeatureClass != null)
                    {
                        if ((tmp as IFeatureLayer).FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                        {
                            pLegend.RemoveItem(i);
                            pLegend.InsertItem(0, pLegendItem);
                        }
                    }
                }
            }
            //�����Ƶ�����
            for (int i = pLegend.ItemCount - 1; i >= 0; i--)
            {
                ILegendItem pLegendItem = pLegend.get_Item(i);
                ILayer tmp = pLegendItem.Layer;
                IFeatureLayer pFeatureLayer = tmp as IFeatureLayer;
                if (pFeatureLayer != null)
                {
                    if (pFeatureLayer.FeatureClass != null)
                    {
                        if ((tmp as IFeatureLayer).FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                        {
                            pLegend.RemoveItem(i);
                            pLegend.InsertItem(pLegend.ItemCount, pLegendItem);//remove����itemcountʱʱ�仯��cautions
                        }
                    }
                }

            }
        }
        //Ѱ��ͼ������
        private IMapSurroundFrame getMapSurroundFrame(IGraphicsContainer inGra,string inName)
        {
            IMapSurroundFrame res = null;
            IGraphicsContainer pgGC = inGra;
            pgGC.Reset();
            IElement pgEle = pgGC.Next();
            while (pgEle != null)
            {
                if (pgEle is IMapSurroundFrame)
                {
                    switch(inName)
                    {
                        case "ͼ��":
                            {
                                IMapSurroundFrame pMSF = pgEle as IMapSurroundFrame;
                                if (pMSF.MapSurround is ILegend)
                                    res = pMSF;
                                break;
                            }
                    }
                }
                pgEle = pgGC.Next();
            }
            return res;
 
        }
        //ɾ��ģ���ϵ�ָ����
        private void delNorthArrow(IGraphicsContainer inGra)
        {
            IGraphicsContainer pgGC = inGra;
            pgGC.Reset();
            IElement pgEle = pgGC.Next();
            while (pgEle != null)
            {
                if (pgEle is IMapSurroundFrame)
                {
                    IMapSurroundFrame pMSF = pgEle as IMapSurroundFrame;
                    if (pMSF.MapSurround is INorthArrow)
                    {
                        pgGC.DeleteElement(pgEle);
                        break;
                    }
                }
                pgEle = pgGC.Next();
            }
        }
        //�õ���ͼ���µ�ͼ�㼯�ϣ��ݹ�
        private void addLayer(ILayer inGPLayer, ref List<ILayer> lyrLst)
        {
            if (inGPLayer is IGroupLayer)
            {
                ICompositeLayer pCL = inGPLayer as ICompositeLayer;
                for (int i = 0; i < pCL.Count; i++)
                {
                    ILayer tmp = pCL.get_Layer(i);
                    if (tmp is IGroupLayer)
                        addLayer(tmp, ref lyrLst);
                    else
                        lyrLst.Add(tmp);
                }
            }
            else
                lyrLst.Add(inGPLayer);
        }
        //����ͼ�ν��Ŀ¼"\\..\\OutputResults\\ͼ�γɹ�\\"
        private string createDir(string inPostfix)
        {
            string datefilename = DateTime.Now.ToString().Replace(':', '_');
            string dstfilename = Application.StartupPath + "\\..\\OutputResults\\ͼ�γɹ�\\" + inPostfix;
            if (Directory.Exists(dstfilename))
            {
                if (MessageBox.Show("�Ѿ���������������������Ƿ񸲸ǣ�", "��ʾ", MessageBoxButtons.YesNo,
                         MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Directory.Delete(dstfilename, true);

                }
                else
                    return "";
            }

            Directory.CreateDirectory(dstfilename);
            string Dir = dstfilename;
            return Dir;

        }
        //ȥ��ͼ��ı�������ʾ����
        private void procLyrScale(IMap inMap)
        {
            for (int i = 0; i < inMap.LayerCount; i++)
            {

                ILayer pLayer = inMap.get_Layer(i);
                if (pLayer is IGroupLayer)
                {
                    ICompositeLayer pCLayer = pLayer as ICompositeLayer;
                    for (int j = 0; j < pCLayer.Count; j++)
                    {
                        ILayer pFLayer = pCLayer.get_Layer(j);
                        pFLayer.MaximumScale = 0;
                        pFLayer.MinimumScale = 0;

                    }
                }
                else//����grouplayer
                {
                    pLayer.MaximumScale = 0;
                    pLayer.MinimumScale = 0;
                }
            }
        }
        //���ͼ��
        public void AddLegend(ESRI.ArcGIS.Carto.IPageLayout pageLayout, ESRI.ArcGIS.Carto.IMap map, System.Double posX, System.Double posY, System.Double legW, int legendCol)
        {

            if (pageLayout == null || map == null)
            {
                return;
            }
            ESRI.ArcGIS.Carto.IGraphicsContainer graphicsContainer = pageLayout as ESRI.ArcGIS.Carto.IGraphicsContainer; // Dynamic Cast
            ESRI.ArcGIS.Carto.IMapFrame mapFrame = graphicsContainer.FindFrame(map) as ESRI.ArcGIS.Carto.IMapFrame; // Dynamic Cast
            ESRI.ArcGIS.esriSystem.IUID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = "esriCarto.Legend";
            ESRI.ArcGIS.Carto.IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame((ESRI.ArcGIS.esriSystem.UID)uid, null); // Explicit Cast


            ILegend2 pLegend = mapSurroundFrame.MapSurround as ILegend2;
            //��������Ƶ�����
            for (int i = 0; i < pLegend.ItemCount; i++)
            {
                ILegendItem pLegendItem = pLegend.get_Item(i);
                pLegendItem.ShowDescriptions = false;
                pLegendItem.ShowHeading = false;
                pLegendItem.ShowLayerName = false;

                ILayer tmp = pLegendItem.Layer;
                if (!tmp.Valid)
                {
                    pLegend.RemoveItem(i);
                }
                IFeatureLayer pFeatureLayer = tmp as IFeatureLayer;
                if (pFeatureLayer != null)
                {

                    if (pFeatureLayer.FeatureClass != null)
                    {
                        if ((tmp as IFeatureLayer).FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                        {
                            pLegend.RemoveItem(i);
                            pLegend.InsertItem(0, pLegendItem);
                        }
                    }
                }
            }
            //�����Ƶ�����
            for (int i = pLegend.ItemCount - 1; i >= 0; i--)
            {
                ILegendItem pLegendItem = pLegend.get_Item(i);
                ILayer tmp = pLegendItem.Layer;
                IFeatureLayer pFeatureLayer = tmp as IFeatureLayer;
                if (pFeatureLayer != null)
                {
                    if (pFeatureLayer.FeatureClass != null)
                    {
                        if ((tmp as IFeatureLayer).FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                        {
                            pLegend.RemoveItem(i);
                            pLegend.InsertItem(pLegend.ItemCount, pLegendItem);//remove����itemcountʱʱ�仯��cautions
                        }
                    }
                }

            }


            pLegend.Title = "ͼ ��";
            pLegend.AdjustColumns(legendCol);//yjl20110812
            ESRI.ArcGIS.Carto.IElement element = mapSurroundFrame as ESRI.ArcGIS.Carto.IElement; // Dynamic Cast
            
            //Get aspect ratio
            ESRI.ArcGIS.Carto.IQuerySize querySize = mapSurroundFrame.MapSurround as ESRI.ArcGIS.Carto.IQuerySize; // Dynamic Cast
            System.Double w = 0;
            System.Double h = 0;
            querySize.QuerySize(ref w, ref h);
            System.Double aspectRatio = w / h;

            ESRI.ArcGIS.Geometry.IEnvelope envelope = new ESRI.ArcGIS.Geometry.EnvelopeClass();
            envelope.PutCoords(posX, (posY - legW / aspectRatio), (posX + legW), posY);
            element.Geometry = envelope;
            IElementProperties3 pep = element as IElementProperties3;
            pep.Name = "ͼ��";
            pep.AnchorPoint = esriAnchorPointEnum.esriTopLeftCorner;
            element.Activate((pageLayout as IActiveView).ScreenDisplay);//�ؼ�����
            graphicsContainer.AddElement(element, 0);
            (graphicsContainer as IActiveView).Refresh();
        }
        //���ͼ��--������Χʸ��
        public void AddLegend2(ESRI.ArcGIS.Carto.IPageLayout pageLayout, ESRI.ArcGIS.Carto.IMap map, System.Double posX, System.Double posY, System.Double legW, int legendCol,IList<ILayer> lyrs)
        {

            if (pageLayout == null || map == null)
            {
                return;
            }
            ESRI.ArcGIS.Carto.IGraphicsContainer graphicsContainer = pageLayout as ESRI.ArcGIS.Carto.IGraphicsContainer; // Dynamic Cast
            ESRI.ArcGIS.Carto.IMapFrame mapFrame = graphicsContainer.FindFrame(map) as ESRI.ArcGIS.Carto.IMapFrame; // Dynamic Cast
            ESRI.ArcGIS.esriSystem.IUID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = "esriCarto.Legend";
            ESRI.ArcGIS.Carto.IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame((ESRI.ArcGIS.esriSystem.UID)uid, null); // Explicit Cast


            ILegend2 pLegend = mapSurroundFrame.MapSurround as ILegend2;
            ILegendFormat pLegendFormat = pLegend.Format;
            ITextSymbol titleSymbol = pLegendFormat.TitleSymbol;
            //ȥ������Ҫ�����ͼ���ͼ��
            for (int i = 0; i < pLegend.ItemCount; i++)
            {
                ILegendItem pLegendItem = pLegend.get_Item(i);
                ILayer tmp = pLegendItem.Layer;
                foreach (ILayer lyr in lyrs)
                {
                    if(lyr.Equals(tmp))
                        pLegend.RemoveItem(i);

                }
                if (!tmp.Valid)
                {
                    pLegend.RemoveItem(i);
                }
            }
            //��������Ƶ�����
            for (int i = 0; i < pLegend.ItemCount; i++)
            {
                ILegendItem pLegendItem = pLegend.get_Item(i);
                pLegendItem.ShowDescriptions = false;
                pLegendItem.ShowHeading = false;
                pLegendItem.ShowLayerName = false;
                ILayer tmp = pLegendItem.Layer;
                IFeatureLayer pFeatureLayer = tmp as IFeatureLayer;
                if (pFeatureLayer.FeatureClass != null)
                {
                    if ((tmp as IFeatureLayer).FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        pLegend.RemoveItem(i);
                        pLegend.InsertItem(0, pLegendItem);
                    }
                }
            }
            //�����Ƶ�����
            for (int i = pLegend.ItemCount - 1; i >= 0; i--)
            {
                ILegendItem pLegendItem = pLegend.get_Item(i);
                ILayer tmp = pLegendItem.Layer;
                IFeatureLayer pFeatureLayer = tmp as IFeatureLayer;
                if (pFeatureLayer.FeatureClass != null)
                {
                    if ((tmp as IFeatureLayer).FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                    {
                        pLegend.RemoveItem(i);
                        pLegend.InsertItem(pLegend.ItemCount, pLegendItem);//remove����itemcountʱʱ�仯��cautions
                    }
                }

            }


            pLegend.Title = "ͼ ��";
            pLegend.AdjustColumns(legendCol);//yjl20110812
            ESRI.ArcGIS.Carto.IElement element = mapSurroundFrame as ESRI.ArcGIS.Carto.IElement; // Dynamic Cast

            //Get aspect ratio
            ESRI.ArcGIS.Carto.IQuerySize querySize = mapSurroundFrame.MapSurround as ESRI.ArcGIS.Carto.IQuerySize; // Dynamic Cast
            System.Double w = 0;
            System.Double h = 0;
            querySize.QuerySize(ref w, ref h);
            System.Double aspectRatio = w / h;

            ESRI.ArcGIS.Geometry.IEnvelope envelope = new ESRI.ArcGIS.Geometry.EnvelopeClass();
            envelope.PutCoords(posX, (posY - legW / aspectRatio), (posX + legW), posY);
            element.Geometry = envelope;
            IElementProperties3 pep = element as IElementProperties3;
            pep.Name = "ͼ��";
            pep.AnchorPoint = esriAnchorPointEnum.esriTopLeftCorner;
            element.Activate((pageLayout as IActiveView).ScreenDisplay);//�ؼ�����
            graphicsContainer.AddElement(element, 0);
            (graphicsContainer as IActiveView).Refresh();
        }

        //���ɵ�ͼ������
        private void CreateMeasuredGrid(IPageLayout inPageLayout)
        {
            //PageLayout
            IActiveView pActiveView;
            IMap pMap;
            pActiveView = inPageLayout as IActiveView;
            pMap = pActiveView.FocusMap;
            IGraphicsContainer pGraCner = inPageLayout as IGraphicsContainer;
            IFrameProperties frameProperties = (IFrameProperties)pGraCner.FindFrame(pActiveView.FocusMap);
            //IStyleGallery pStyleGallery = new ServerStyleGalleryClass();
            //IStyleGalleryStorage pStyleGalleryStorage;
            //IEnumStyleGalleryItem pEnumStyleGalleryItem = new EnumServerStyleGalleryItemClass();
            //pStyleGalleryStorage = pStyleGallery as IStyleGalleryStorage;
            //pStyleGalleryStorage.AddFile(Application.StartupPath + @"\..\sTyles\pagelayout.ServerStyle");

            //pEnumStyleGalleryItem = pStyleGallery.get_Items("Borders", Application.StartupPath + @"\..\sTyles\pagelayout.ServerStyle", "");
            //pEnumStyleGalleryItem.Reset();
            //IStyleGalleryItem pEnumItem;
            //pEnumItem = pEnumStyleGalleryItem.Next();
            //while (pEnumItem != null)
            //{
            //    if (pEnumItem.Name == "ͼ��")
            //    {
            //        frameProperties.Border = (IBorder)pEnumItem.Item;
            //        frameProperties.Border.Gap = 17;
            //        break;
            //    }
            //    pEnumItem = pEnumStyleGalleryItem.Next();
            //}

            //System.Runtime.InteropServices.Marshal.ReleaseComObject(pEnumStyleGalleryItem);
            //���mapGrid����ʽ��ͼ


            IMapGrids myMapGrids = (IMapGrids)pGraCner.FindFrame(pActiveView.FocusMap);
            myMapGrids.AddMapGrid(GeoPageLayoutFn.creategrid(true, inPageLayout));
            myMapGrids.AddMapGrid(GeoPageLayoutFn.creategrid(false, inPageLayout));
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);


        }
    }

    /// <summary>
    /// ���ߣ�yjl
    /// ���ڣ�20110909
    /// ˵������ͼ��ͼ���ú�����fromVB��net
    /// </summary>
    public static class GeoPageLayoutFn
    {
        
        //�ƶ���ͼģ���Ԫ��
        public static void MoveElement(IElement inElement, double inOffX, double inOffY)
        {
            //if (inElement.Geometry.GeometryType == esriGeometryType.esriGeometryPoint)
            //{
            //    IPoint pPoint = inElement.Geometry as IPoint;
            //    pPoint.X += inOffX;
            //    pPoint.Y += inOffY;
            //    inElement.Geometry = pPoint;
            //}
            //else if (inElement.Geometry.GeometryType == esriGeometryType.esriGeometryLine)
            //{
            //    ILine pLine = inElement.Geometry as ILine;

            //    pLine.FromPoint.X += inOffX;
            //    pLine.FromPoint.Y += inOffY;
            //    pLine.ToPoint.X += inOffX;
            //    pLine.ToPoint.Y += inOffY;
            //}
            //else if (inElement.Geometry.GeometryType == esriGeometryType.esriGeometryPolyline)
            //{
                //IPointCollection pPC = inElement.Geometry as IPointCollection;
                //for (int i = 0; i < pPC.PointCount; i++)
                //{
                //    IPoint p = pPC.get_Point(i);
                //    p.X += inOffX;
                //    p.Y += inOffY;
                //    pPC.UpdatePoint(i, p);
                //}
                ITransform2D pT2D = inElement.Geometry as ITransform2D;
                pT2D.Move(inOffX, inOffY);
                inElement.Geometry = pT2D as IGeometry;
                
            //}
            //else
            //{
            //    IEnvelope pEnv = inElement.Geometry.Envelope;
            //    pEnv.Offset(inOffX, inOffY);
            //    inElement.Geometry = pEnv;
            //}
        }
        //����ͼģ���ȡģ��Ԫ��
        public static void OpenTemplateElement(IPageLayoutControl inPLC)
        {
            IMapDocument pMD = new MapDocumentClass();
            pMD.Open(Application.StartupPath + @"\..\Template\TDLY10000.mxd", "");
            //����ɾ��ԭ�е�Ԫ��
            IGraphicsContainer pOriGC = inPLC.PageLayout as IGraphicsContainer;
            pOriGC.Reset();
            IElement pEle = pOriGC.Next();
            while (pEle != null)
            {
                if (!(pEle is IMapFrame))
                {
                    pOriGC.DeleteElement(pEle);
                    pOriGC.Reset();//ɾ��֮��Ҫ���ã�����ɾ������

                }
                pEle = pOriGC.Next();
            }//while
            //�����ͼģ���Ԫ��
            IGraphicsContainer pMbGC = pMD.PageLayout as IGraphicsContainer;
            pMbGC.Reset();
            pEle = pMbGC.Next();
            while (pEle != null)
            {
                if (pEle is ITextElement)
                {
                    pOriGC.AddElement(pEle, 0);

                }
                pEle = pMbGC.Next();
            }//while
        }
        //��̨����ɭ����Դ��״��׼�ַ�ͼ--����ͼ�ؼ����в���
        public static void QuietPageLayoutTDLYFFT(IPageLayoutControl axPageLayoutControl1, Dictionary<string, string> pgTextElements, int typeZT)
        {
            ISpatialReference pSpatialRefrence = axPageLayoutControl1.ActiveView.FocusMap.SpatialReference;
            GeoDrawSheetMap.clsDrawSheetMap pDrawSheetMap = new GeoDrawSheetMap.clsDrawSheetMap();
            pDrawSheetMap.vPageLayoutControl = axPageLayoutControl1 as IPageLayoutControl;
            pDrawSheetMap.vScale = Convert.ToUInt32(pgTextElements["������"].Substring(2));
            pDrawSheetMap.m_intPntCount = 3;
            pDrawSheetMap.type_ZT = typeZT;//ר������
            pDrawSheetMap.m_pPrjCoor = axPageLayoutControl1.ActiveView.FocusMap.SpatialReference;
            string[] astrMapNo = pgTextElements["G50G005005"].Split(' ');//��ͼ���Ŵ��ո� ��ȥ�ո�
            string realMapNo = "";
            foreach (string str in astrMapNo)
            {
                realMapNo += str;
            }
            pDrawSheetMap.m_strSheetNo = realMapNo;//ͼ����

            //����ͼ���Ż��ͶӰ�ļ�
            if (axPageLayoutControl1.ActiveView.FocusMap.SpatialReference is IProjectedCoordinateSystem)
            {
                pDrawSheetMap.m_pPrjCoor = axPageLayoutControl1.ActiveView.FocusMap.SpatialReference;
            }
            else
            {
                ISpatialReference pSpa = GetSpatialByMapNO(pDrawSheetMap.m_strSheetNo);
                if (pSpa == null)
                {
                    MessageBox.Show("�����õ�ͼ��ͶӰ���꣡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                pDrawSheetMap.m_pPrjCoor = pSpa;
                axPageLayoutControl1.ActiveView.FocusMap.SpatialReference = pSpa;
            }

            pDrawSheetMap.DrawSheetMap();

            foreach (KeyValuePair<string, string> kvp in pgTextElements)
            {
                IGraphicsContainer pgGC = axPageLayoutControl1.GraphicsContainer;
                pgGC.Reset();
                IElement pgEle = pgGC.Next();
                while (pgEle != null)
                {
                    if (pgEle is ITextElement && (pgEle as ITextElement).Text == kvp.Key)
                    {
                        (pgEle as ITextElement).Text = kvp.Value;

                    }
                    pgEle = pgGC.Next();
                }
            }

            IGraphicsContainerSelect pGCS = axPageLayoutControl1.PageLayout as IGraphicsContainerSelect;
            if (pGCS.ElementSelectionCount != 0)
            {
                pGCS.UnselectAllElements();

            }
        }

        /// <summary>
        /// ����ͼ���Ż��ͶӰ�ļ�
        /// </summary>
        /// <param name="strMapNO"></param>
        /// <returns></returns>
        private static ISpatialReference GetSpatialByMapNO(string strMapNO)
        {
            //double dblX = 0;
            //double dblY = 0;

            // GeoDrawSheetMap.basPageLayout.GetCoordinateFromNewCode(strMapNO, ref dblX, ref dblY);

            //dblX = dblX / 3600;
            //dblY = dblY / 3600;
            string strtfh = "";
            try
            {
                strtfh = strMapNO.Substring(1, 2);
                int tfh = Convert.ToInt32(strtfh);
                tfh = tfh * 6 - 180 - 3;
                strtfh = "CGCS 2000 GK CM " + tfh.ToString() + "E.prj";
            }
            catch
            {

            }
            string strPrjFileName = Application.StartupPath + "\\..\\Prj\\CGCS2000\\" + strtfh;
            if (!System.IO.File.Exists(strPrjFileName)) return null;

            ISpatialReferenceFactory pSpaFac = new SpatialReferenceEnvironmentClass();
            return pSpaFac.CreateESRISpatialReferenceFromPRJFile(strPrjFileName);

        }
        //�������ע��
        public static void drawTextElement(IPoint p, IGraphicsContainer pGra,string strEle)
        {
            ITextElement pTextElement = new TextElementClass();
            ITextSymbol pTextSymbol = new TextSymbolClass();
            stdole.StdFont mySedFont = new stdole.StdFontClass();
            mySedFont.Name = "����";
            mySedFont.Size = 10;
            mySedFont.Bold = true;
            
            IRgbColor color = new RgbColorClass();
            color.Blue = 0;
            color.Green = 0;
            color.Red = 255;
            pTextSymbol.Font = mySedFont as IFontDisp;
            pTextSymbol.Color = color;
            pTextSymbol.HorizontalAlignment = esriTextHorizontalAlignment.esriTHALeft;
            pTextElement.Text = strEle;
            pTextElement.Symbol = pTextSymbol;
            (pTextElement as IElement).Geometry = (IGeometry)p;
            //(pTextElement as IElementProperties3).AnchorPoint = esriAnchorPointEnum.esriBottomMidPoint;
            //(pTextElement as IElementProperties3).AutoTransform = true;
            //(pTextElement as IElementProperties3).Name = "1";

            pGra.AddElement(pTextElement as IElement, 0);
           
        }

        //��mapcontrol�ϻ������
        public static void drawPolygonElement(IGeometry pPolygon, IGraphicsContainer pGra)
        {
            if (pPolygon == null)
                return;
            //pPolygon.Project((pGra as IActiveView).FocusMap.SpatialReference);
            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();
            IFillShapeElement pPolygonElement = null;
            if (pPolygon.GeometryType == esriGeometryType.esriGeometryEnvelope)
                pPolygonElement = new RectangleElementClass();
            else if (pPolygon.GeometryType == esriGeometryType.esriGeometryPolygon)
                pPolygonElement = new PolygonElementClass();
            try
            {
                //��ɫ����
                IRgbColor pRGBColor = new RgbColorClass();
                pRGBColor.UseWindowsDithering = false;
                ISymbol pSymbol = (ISymbol)pFillSymbol;
                //pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

                pRGBColor.Red = 0;
                pRGBColor.Green = 0;
                pRGBColor.Blue = 255;
                pLineSymbol.Color = pRGBColor;

                pLineSymbol.Width = 2;
                //pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                pFillSymbol.Outline = pLineSymbol;
                pRGBColor.Transparency = 0;
                pFillSymbol.Color = pRGBColor;
                //pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;
                (pPolygonElement as IElement).Geometry = pPolygon;
                pPolygonElement.Symbol = pFillSymbol;
                pGra.AddElement(pPolygonElement as IElement, 0);



            }
            catch (Exception ex)
            {
                //MessageBox.Show("���Ʒ�Χ����:" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pFillSymbol = null;
            }
        }
        //������mapcontrol�ϻ�����Σ�����Ų���
        public static void drawPolygonElement(IGeometry pPolygon, IGraphicsContainer pGra, IRgbColor fillClr, bool transparency, IRgbColor inRgbColor, double inWidth)
        {
            if (pPolygon == null)
                return;
            //pPolygon.Project((pGra as IActiveView).FocusMap.SpatialReference);
            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();
            IFillShapeElement pPolygonElement = null;
            if (pPolygon.GeometryType == esriGeometryType.esriGeometryEnvelope)
                pPolygonElement = new RectangleElementClass();
            else if (pPolygon.GeometryType == esriGeometryType.esriGeometryPolygon)
                pPolygonElement = new PolygonElementClass();
            try
            {
                //��ɫ����
                fillClr.UseWindowsDithering = false;
                inRgbColor.UseWindowsDithering = false;
                ISymbol pSymbol = (ISymbol)pFillSymbol;
                //pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                pLineSymbol.Color = inRgbColor;

                pLineSymbol.Width = inWidth;
                //pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                pFillSymbol.Outline = pLineSymbol;
                if(transparency)
                    fillClr.Transparency = 0;
                pFillSymbol.Color = fillClr;
                //pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;
                (pPolygonElement as IElement).Geometry = pPolygon;
                pPolygonElement.Symbol = pFillSymbol;
                pGra.AddElement(pPolygonElement as IElement, 0);



            }
            catch (Exception ex)
            {
                //MessageBox.Show("���Ʒ�Χ����:" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pFillSymbol = null;
            }
        }
        //��mapcontrol�ϻ���
        public static void drawPolylineElement(IGeometry pPolyline, IGraphicsContainer pGra)
        {
            if (pPolyline == null)
                return;
            //pPolyline.Project((pGra as IMap).SpatialReference);
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();
            ILineElement pLineEle = new LineElementClass();
            try
            {
                //��ɫ����
                IRgbColor pRGBColor = new RgbColorClass();
                pRGBColor.UseWindowsDithering = false;
                pRGBColor.Red = 0;
                pRGBColor.Green = 0;
                pRGBColor.Blue = 255;
                pLineSymbol.Color = pRGBColor;

                pLineSymbol.Width = 2;
                pLineEle.Symbol = pLineSymbol as ILineSymbol;
                (pLineEle as IElement).Geometry = pPolyline;
                pGra.AddElement(pLineEle as IElement, 0);



            }
            catch (Exception ex)
            {
                //MessageBox.Show("���Ʒ�Χ����:" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pLineSymbol = null;
            }
        }
        //��mapcontrol�ϻ���������Ҫ��ɫ�Ϳ�Ȳ���
        public static void drawPolylineElement(IGeometry pPolyline, IGraphicsContainer pGra,IRgbColor inRgbColor,double inWidth)
        {
            if (pPolyline == null)
                return;
            //pPolyline.Project((pGra as IMap).SpatialReference);
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();
            ILineElement pLineEle = new LineElementClass();
            try
            {
                pLineSymbol.Color = inRgbColor;

                pLineSymbol.Width = inWidth;
                pLineEle.Symbol = pLineSymbol as ILineSymbol;
                (pLineEle as IElement).Geometry = pPolyline;
                pGra.AddElement(pLineEle as IElement, 0);



            }
            catch (Exception ex)
            {
                //MessageBox.Show("���Ʒ�Χ����:" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pLineSymbol = null;
            }
        }
        //�ɾ�γ�ȵõ�ͼ����---δ������
        public static bool GetNewCodeFromCoordinate(out string tfh, double x, double y, long vScale)
        {
            tfh = "";
            string b;
            int l;
            int r;
            int c; string th; int YInt; double XPlus;
            double YPlus;
            YInt = Convert.ToInt32(y / 4 / 3600) + 1;
            b = Convert.ToString(YInt + 64);
            l = Convert.ToInt32(x / 6 / 3600) + 31; switch (vScale)
            {
                case 500000:
                    th = "B";
                    XPlus = 3 * 3600;
                    YPlus = 2 * 3600;
                    break;
                case 250000:
                    th = "C";
                    XPlus = 3 / 2 * 3600;
                    YPlus = 1 * 3600;
                    break;
                case 100000:
                    th = "D";
                    XPlus = 1 / 2 * 3600;
                    YPlus = 1 / 3 * 3600;
                    break;
                case 50000:
                    th = "E";
                    XPlus = 1 / 4 * 3600;
                    YPlus = 1 / 6 * 3600;
                    break;
                case 25000:
                    th = "F";
                    XPlus = 1 / 8 * 3600;
                    YPlus = 1 / 12 * 3600;
                    break;
                case 10000:
                    th = "G";
                    XPlus = 1 / 16 * 3600;
                    YPlus = 1 / 24 * 3600;
                    break;
                case 5000:
                    th = "H";
                    XPlus = 1 / 32 * 3600;
                    YPlus = 1 / 48 * 3600;
                    break;
                default:
                    return false;
            }
            r = Convert.ToInt32(4 / (YPlus / 3600) - Convert.ToInt32(((y / 3600) - Convert.ToInt32((y / 3600) / 4) * 4) / (YPlus / 3600)));
            c = Convert.ToInt32(((x / 3600) - Convert.ToInt32((x / 3600) / 6) * 6) / (XPlus / 3600)) + 1;
            tfh = b + l + th + String.Format("00#", r) + String.Format("00#", c);
            return true;
        }
        public static IMapGrid creategrid(bool is_zuoyou, IPageLayout inPageLayout)
        {
            IActiveView pActiveView = inPageLayout as IActiveView;
            IMapGrid myMapGrid;
            IMeasuredGrid myMeasuredGrid = new MeasuredGridClass();
            IProjectedGrid pProjectedGrid = (IProjectedGrid)myMeasuredGrid;
            pProjectedGrid.SpatialReference = pActiveView.FocusMap.SpatialReference;
            myMeasuredGrid.FixedOrigin = true;
            myMeasuredGrid.Units = esriUnits.esriKilometers;
            myMeasuredGrid.XIntervalSize = 1;
            myMeasuredGrid.XOrigin = 0;
            myMeasuredGrid.YIntervalSize = 1;
            myMeasuredGrid.YOrigin = 0;

            myMapGrid = (IMapGrid)myMeasuredGrid;


            IRgbColor rgbColor = new RgbColor();
            rgbColor.Red = 0;
            rgbColor.Green = 0;
            rgbColor.Blue = 0;
            IColor color = rgbColor as IColor;

            //�����ߵķ�����ʽ
            ICartographicLineSymbol pLineSymbol;
            pLineSymbol = new CartographicLineSymbolClass();
            pLineSymbol.Cap = esriLineCapStyle.esriLCSButt;
            pLineSymbol.Width = 0;
            myMapGrid.LineSymbol = pLineSymbol;
            //�̶��߷�����ʽ
            pLineSymbol = new CartographicLineSymbolClass();
            pLineSymbol.Cap = esriLineCapStyle.esriLCSSquare;
            pLineSymbol.Join = esriLineJoinStyle.esriLJSBevel;
            pLineSymbol.Width = 0.2;
            pLineSymbol.Color = color;
            myMapGrid.TickMarkSymbol = null;
            myMapGrid.TickLineSymbol = pLineSymbol;
            myMapGrid.TickLength = 17;
            myMapGrid.SetTickVisibility(false, false, false, false);


            ISimpleMapGridBorder simpleMapGridBorder = new SimpleMapGridBorderClass();
            ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            simpleLineSymbol.Color = color;
            simpleLineSymbol.Width = 0.4;
            simpleMapGridBorder.LineSymbol = simpleLineSymbol as ILineSymbol;
            myMapGrid.Border = simpleMapGridBorder as IMapGridBorder;

            myMapGrid.Border = (IMapGridBorder)simpleMapGridBorder;

            //������ǩ��������������
            IMixedFontGridLabel mGrdLab = new MixedFontGridLabelClass();
            IFormattedGridLabel myMapFormattedGridLabel = mGrdLab as IFormattedGridLabel;
            IGridLabel myGridLabel = (IGridLabel)myMapFormattedGridLabel;

            if (is_zuoyou)
            {
                mGrdLab.NumGroupedDigits = 1;
            }
            else
            {
                mGrdLab.NumGroupedDigits = 2;
            }
            IRgbColor glColor = new RgbColorClass();
            glColor.Red = 0;
            glColor.Green = 0;
            glColor.Blue = 0;
            mGrdLab.SecondaryColor = glColor;
            stdole.StdFont mySedFont = new stdole.StdFontClass();
            mySedFont.Name = "����";
            mySedFont.Size = 7;
            mGrdLab.SecondaryFont = (stdole.IFontDisp)mySedFont;
            stdole.StdFont myFont = new stdole.StdFontClass();
            myFont.Name = "����";
            myFont.Size = 10;
            myFont.Bold = true;
            myGridLabel.Font = (stdole.IFontDisp)myFont;

            IColor myColor = new RgbColorClass();
            myColor.RGB = Convert.ToInt32(0);
            myGridLabel.LabelOffset = 0;
            myGridLabel.Color = myColor;


            myGridLabel.set_LabelAlignment(esriGridAxisEnum.esriGridAxisLeft, true);
            myGridLabel.set_LabelAlignment(esriGridAxisEnum.esriGridAxisTop, true);
            myGridLabel.set_LabelAlignment(esriGridAxisEnum.esriGridAxisRight, true);
            myGridLabel.set_LabelAlignment(esriGridAxisEnum.esriGridAxisBottom, true);
            INumericFormat myNumericFormat = new NumericFormatClass();
            myNumericFormat.AlignmentOption = esriNumericAlignmentEnum.esriAlignLeft;
            if (!is_zuoyou)
            {
                myNumericFormat.AlignmentOption = esriNumericAlignmentEnum.esriAlignRight;
                myNumericFormat.AlignmentWidth = 4;
            }
            myNumericFormat.RoundingOption = esriRoundingOptionEnum.esriRoundNumberOfDecimals;
            myNumericFormat.RoundingValue = 0;
            myNumericFormat.ShowPlusSign = false;
            myNumericFormat.UseSeparator = false;
            myMapFormattedGridLabel.Format = (INumberFormat)myNumericFormat;
            myMapGrid.LabelFormat = myGridLabel;
            if (is_zuoyou)
            {
                myMapGrid.SetLabelVisibility(true, false, true, false);
            }
            else
            {
                myMapGrid.SetLabelVisibility(false, true, false, true);
            }

            return myMapGrid;
        }
        //����ͼ���Ψһֵ��Ⱦ��ֻ������Χ�ڵķ���
        public static void updateLayerSymbol(ILayer inLayer, IGeometry inGeometry)
        {
            if (inLayer is IGroupLayer)//�������ͼ�㣬�ݹ�
            {
                ICompositeLayer pCL = inLayer as ICompositeLayer;
                for (int i = 0; i < pCL.Count; i++)
                {
                    updateLayerSymbol(pCL.get_Layer(i), inGeometry);

                }
            }
            else
            {
                IFeatureLayer pFeaLyr = inLayer as IFeatureLayer;
                if (pFeaLyr == null)
                    return;
                IGeoFeatureLayer pGeoFeaLayer = pFeaLyr as IGeoFeatureLayer;
                IFeatureRenderer pFeaRenderer = pGeoFeaLayer.Renderer;
                IUniqueValueRenderer pUVRenderer = pFeaRenderer as IUniqueValueRenderer;
                if (pUVRenderer == null)
                    return;
                if (pUVRenderer.FieldCount != 1)
                    return;
                IFeatureClass pFeaCls = pGeoFeaLayer.FeatureClass;
                if (pFeaCls == null)
                    return;
                ISpatialFilter pSpaFlr = new SpatialFilterClass();
                pSpaFlr.Geometry = inGeometry;
                pSpaFlr.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                ICursor pCursor = pFeaCls.Search(pSpaFlr, false) as ICursor;
                IFeatureRenderer pNewFeaRenderer = new UniqueValueRendererClass();
                IUniqueValueRenderer pNewUVRenderer = pNewFeaRenderer as IUniqueValueRenderer;
                IDataStatistics pDataStatistics = new DataStatisticsClass();
                pDataStatistics.Cursor = pCursor;
                pDataStatistics.Field = pUVRenderer.get_Field(0);
                pNewUVRenderer.FieldCount = 1;
                pNewUVRenderer.set_Field(0, pUVRenderer.get_Field(0));
                System.Collections.IEnumerator pEnumerator = pDataStatistics.UniqueValues;
                while (pEnumerator.MoveNext())
                {
                    object obj = pEnumerator.Current;
                    string vle = obj.ToString();
                    for (int i = 0; i < pUVRenderer.ValueCount; i++)
                    {
                        string orivle = pUVRenderer.get_Value(i);
                        if (orivle.Contains(vle))
                        {
                            pNewUVRenderer.AddValue(orivle, pUVRenderer.get_Heading(orivle), pUVRenderer.get_Symbol(orivle));
                            pNewUVRenderer.set_Label(orivle, pUVRenderer.get_Label(orivle));
                        }
                    }
                    pNewUVRenderer.UseDefaultSymbol = false;//����ʾ����ֵ
                    pGeoFeaLayer.Renderer = pNewFeaRenderer;
                }
            }//else

        }
        //���ĵ�ͼ��ͼ���Ψһֵ��Ⱦ��ֻ������Χ�ڵķ���
        public static void updateMapSymbol(IMap pMap, IGeometry inGeometry)
        {
            for (int i = 0; i < pMap.LayerCount; i++)
            {
                updateLayerSymbol(pMap.get_Layer(i), inGeometry);

            }
 
        }
        //���ͼ��
        public static void AddLegendZT(ESRI.ArcGIS.Carto.IPageLayout pageLayout, ESRI.ArcGIS.Carto.IMap map, System.Double posX, System.Double posY, System.Double legW, int legendCol)
        {

            if (pageLayout == null || map == null)
            {
                return;
            }
            ESRI.ArcGIS.Carto.IGraphicsContainer graphicsContainer = pageLayout as ESRI.ArcGIS.Carto.IGraphicsContainer; // Dynamic Cast
            ESRI.ArcGIS.Carto.IMapFrame mapFrame = graphicsContainer.FindFrame(map) as ESRI.ArcGIS.Carto.IMapFrame; // Dynamic Cast
            ESRI.ArcGIS.esriSystem.IUID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = "esriCarto.Legend";
            ESRI.ArcGIS.Carto.IMapSurroundFrame mapSurroundFrame = mapFrame.CreateSurroundFrame((ESRI.ArcGIS.esriSystem.UID)uid, null); // Explicit Cast


            ILegend2 pLegend = mapSurroundFrame.MapSurround as ILegend2;
            //��������Ƶ�����
            for (int i = 0; i < pLegend.ItemCount; i++)
            {
                ILegendItem pLegendItem = pLegend.get_Item(i);
                pLegendItem.ShowDescriptions = false;
                pLegendItem.ShowHeading = false;
                pLegendItem.ShowLayerName = false;
                ILayer tmp = pLegendItem.Layer;
                if (tmp.Valid)
                {
                    if ((tmp as IFeatureLayer).FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                    {
                        pLegend.RemoveItem(i);
                        pLegend.InsertItem(0, pLegendItem);
                    }

                }
                else
                    pLegend.RemoveItem(i);


            }
            //�����Ƶ�����
            for (int i = pLegend.ItemCount - 1; i >= 0; i--)
            {
                ILegendItem pLegendItem = pLegend.get_Item(i);
                ILayer tmp = pLegendItem.Layer;
                if (tmp.Valid)
                {
                    if ((tmp as IFeatureLayer).FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                    {
                        pLegend.RemoveItem(i);
                        pLegend.InsertItem(pLegend.ItemCount, pLegendItem);//remove����itemcountʱʱ�仯��cautions
                    }
                }
                else
                    pLegend.RemoveItem(i);
            }


            pLegend.Title = "ͼ ��";
            pLegend.AdjustColumns(legendCol);//yjl20110812
            pLegend.Refresh();
            //Get aspect ratio
            ESRI.ArcGIS.Carto.IQuerySize querySize = mapSurroundFrame.MapSurround as ESRI.ArcGIS.Carto.IQuerySize; // Dynamic Cast
            System.Double w = 0;
            System.Double h = 0;
            querySize.QuerySize(ref w, ref h);
            System.Double aspectRatio = w / h;

            ESRI.ArcGIS.Geometry.IEnvelope envelope = new ESRI.ArcGIS.Geometry.EnvelopeClass();
            envelope.PutCoords(posX - legW, posY, posX, posY + (legW / aspectRatio));
            mapSurroundFrame.Border = createBorder(envelope, pageLayout as IActiveView);//�߿�diff
            ESRI.ArcGIS.Carto.IElement element = mapSurroundFrame as ESRI.ArcGIS.Carto.IElement; // Dynamic Cast
            element.Geometry = envelope;
            IElementProperties3 pep = element as IElementProperties3;
            pep.AnchorPoint = esriAnchorPointEnum.esriBottomRightCorner;
            pep.Name = "ͼ��";
            element.Activate((pageLayout as IActiveView).ScreenDisplay);//�ؼ�����
            graphicsContainer.AddElement(element, 0);
           
            
        }
        //����border
        private static IBorder createBorder(IGeometry inGeometry, IActiveView inAV)
        {
            ISymbolBorder pSymbolBorder = new SymbolBorderClass();
            pSymbolBorder.GetGeometry(inAV.ScreenDisplay as IDisplay, inGeometry.Envelope);
            ILineSymbol pLSymbol = new SimpleLineSymbolClass();
            pLSymbol.Color = getRGB(0, 0, 0);
            pLSymbol.Width = 1;
            pSymbolBorder.LineSymbol = pLSymbol;


            return pSymbolBorder as IBorder;
        }
        //������ɫ
        private static IRgbColor getRGB(int r, int g, int b)
        {
            IRgbColor pColor;
            pColor = new RgbColorClass();
            pColor.Red = r;
            pColor.Green = g;
            pColor.Blue = b;
            return pColor;
        }
        //���ģ����ͼ����
        //��ȡģ�����ͼ����
        public static  IPageLayout GetTemplateGra(string inPath)
        {
            IPageLayout res = null;
            if (!File.Exists(inPath))
                return null;//1
            else
            {
                IMapDocument pMD = new MapDocumentClass();
                pMD.Open(inPath, "");
                res = pMD.PageLayout;
                pMD.Close();
                return res;//2
            }
        }
        //��������ͼ��
         public static ILayer createHachureLyr(IFeatureClass inFC, string inFd, string inCode)
        {
            IFeatureLayer pFeaLyr = new FeatureLayerClass();
            pFeaLyr.FeatureClass = inFC;
            IFeatureRenderer pNewFeaRenderer = new UniqueValueRendererClass();
            IUniqueValueRenderer pNewUVRenderer = pNewFeaRenderer as IUniqueValueRenderer;
            pNewUVRenderer.FieldCount = 1;
            pNewUVRenderer.set_Field(0, inFd);
            ISymbol pSym = createFillSymbol(getRGB(0,0,0), true, getRGB(0, 0, 255), 2);
            pNewUVRenderer.AddValue(inCode, "", pSym);
            pNewUVRenderer.set_Label(inCode, "����");
            ISymbol pDftSym = createFillSymbol(getRGB(255, 255, 255), false, getRGB(255, 255, 255), 0);
            pNewUVRenderer.DefaultSymbol = pDftSym;
            pNewUVRenderer.DefaultLabel = "";
            pNewUVRenderer.UseDefaultSymbol = true;
            IGeoFeatureLayer pGFL = pFeaLyr as IGeoFeatureLayer;
            pGFL.Renderer = pNewFeaRenderer;
            pFeaLyr.Name = "����";
            return pFeaLyr as ILayer;
            
        }
        //�����������
         private static ISymbol createFillSymbol(IRgbColor fillClr, bool transparency, IRgbColor inRgbColor, double inWidth)
         {
             ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
             ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();
             ISymbol pSymbol = (ISymbol)pFillSymbol;
             try
             {
                 //��ɫ����
                 fillClr.UseWindowsDithering = false;
                 inRgbColor.UseWindowsDithering = false;
                 
                 //pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                 pLineSymbol.Color = inRgbColor;

                 pLineSymbol.Width = inWidth;
                 //pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                 pFillSymbol.Outline = pLineSymbol;
                 if (transparency)
                     fillClr.NullColor = transparency;
                 pFillSymbol.Color = fillClr;
                 //pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;




             }
             catch (Exception ex)
             {
                 //MessageBox.Show("���Ʒ�Χ����:" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 pFillSymbol = null;
             }
             return pSymbol;
 
         }


    }
}
