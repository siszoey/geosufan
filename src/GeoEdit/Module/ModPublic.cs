using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;

namespace GeoEdit
{
    public static class ModPublic
    {
        #region ��MapControl�ϵ�ͼ�㰴�չ����ռ���д洢
        public static Dictionary<IWorkspace, List<IFeatureLayer>> GetAllLayersFromMap(IMapControlDefault pMapControl)
        {
            if (pMapControl.Map.LayerCount == 0) return null;
            Dictionary<IWorkspace, List<IFeatureLayer>> dicValue = new Dictionary<IWorkspace, List<IFeatureLayer>>();
            List<IFeatureLayer> pListLays;

            UID pUID = new UIDClass();
            try
            {
                //ʸ�����ݴ���
                pUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";   //UID for IFeatureLayer
                IEnumLayer pEnumLayer = pMapControl.Map.get_Layers(pUID, true);
                pEnumLayer.Reset();
                ILayer pLayer = pEnumLayer.Next();
                while (pLayer != null)
                {
                    IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                    IDataset pDataset = pFeatureLayer.FeatureClass as IDataset;
                    if (!dicValue.ContainsKey(pDataset.Workspace))
                    {
                        pListLays = new List<IFeatureLayer>();
                        pListLays.Add(pFeatureLayer);
                        dicValue.Add(pDataset.Workspace, pListLays);
                    }
                    else
                    {
                        dicValue[pDataset.Workspace].Add(pFeatureLayer);
                    }
                    pLayer = pEnumLayer.Next();
                }
            }
            catch (Exception ex)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //******************************************
                return null;
            }


            return dicValue;
        }
        #endregion


        #region ѡ��༭Ҫ��ʱ�õ��ĺ�������
        public static double ConvertPixelsToMapUnits(IActiveView pActiveView, int pixelUnits)
        {
            tagRECT deviceRECT = pActiveView.ScreenDisplay.DisplayTransformation.get_DeviceFrame();
            int pixelExtent = deviceRECT.right - deviceRECT.left;
            double realWorldDisplayExtent = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;
            double sizeOfOnePixel = realWorldDisplayExtent / pixelExtent;
            return pixelUnits * sizeOfOnePixel;
        }


        //ѡ��Ҫ��
        public static void GetSelctionSet(IFeatureLayer pFeatureLayer, IGeometry pGeometry, IFeatureClass pFeatureClass, esriSelectionResultEnum pselecttype, bool bjustone)
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


        /// <summary>
        /// ��Ҫ���������һ������ק�ķ���
        /// </summary>
        /// <param name="geometry"></param>
        public static void DrawEditSymbol(IGeometry geometry, IActiveView pActiveView)
        {
            IColor pColor = SysCommon.Gis.ModGisPub.GetRGBColor(38, 115, 0);
            ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
            pSimpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSSquare;
            pSimpleMarkerSymbol.Color = pColor;
            pSimpleMarkerSymbol.Size = 2.0;

            pActiveView.ScreenDisplay.StartDrawing(pActiveView.ScreenDisplay.hDC, -1);
            pActiveView.ScreenDisplay.SetSymbol(pSimpleMarkerSymbol as ISymbol);
            IPointCollection pointCol = geometry as IPointCollection;
            for (int i = 0; i < pointCol.PointCount; i++)
            {
                pActiveView.ScreenDisplay.DrawPoint(pointCol.get_Point(i));
            }

            pActiveView.ScreenDisplay.FinishDrawing();
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphicSelection, null, pActiveView.Extent);
        }

        //�ж�����Ƿ�����ѡ��ļ�¼����
        public static bool MouseOnSelection(IPoint pPnt, IActiveView pActiveView)
        {
            //���õ�ѡ���ݲ�
            ISelectionEnvironment pSelectEnv = new SelectionEnvironmentClass();
            double Length = ConvertPixelsToMapUnits(pActiveView, pSelectEnv.SearchTolerance);

            UID pUID = new UIDClass();
            pUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";   //UID for IFeatureLayer
            IEnumLayer pEnumLayer = pActiveView.FocusMap.get_Layers(pUID, true);
            pEnumLayer.Reset();
            ILayer pLayer = pEnumLayer.Next();
            while (pLayer != null)
            {
                if (MouseOnSelection(pLayer as IFeatureLayer, pPnt, pActiveView, Length) == true)
                {
                    return true;
                }
                pLayer = pEnumLayer.Next();
            }

            return false;
        }
        private static bool MouseOnSelection(IFeatureLayer pFeatureLay, IPoint pPnt, IActiveView pActiveView, double Length)
        {
            IFeatureSelection pFeatureSelection = pFeatureLay as IFeatureSelection;
            if (pFeatureSelection == null) return false;
            ISelectionSet pSelectionSet = pFeatureSelection.SelectionSet;
            if (pSelectionSet.Count == 0) return false;
            ICursor pCursor = null;
            pSelectionSet.Search(null, false, out pCursor);
            IFeatureCursor pFeatureCursor = pCursor as IFeatureCursor;
            if (pFeatureCursor == null) return false;
            IFeature pFeature = pFeatureCursor.NextFeature();
            while (pFeature != null)
            {
                IGeometry4 pGeom = pFeature.ShapeCopy as IGeometry4;
                //��pGeomһ��ȷ���Ŀռ�ο�
                pGeom.Project(pActiveView.FocusMap.SpatialReference);
                if (pGeom.IsEmpty) continue;

                IProximityOperator pObj = pGeom as IProximityOperator;
                double dblDist = pObj.ReturnDistance(pPnt);
                if (dblDist < Length)
                {
                    return true;
                }

                pFeature = pFeatureCursor.NextFeature();
            }

            return false;
        }

        //�ж�����Ƿ�����ѡ��Ҫ�صĽڵ���
        public static bool MouseOnFeatureVertex(IPoint pPnt, IFeature pFeature, IActiveView pActiveView)
        {
            //���õ�ѡ���ݲ�
            ISelectionEnvironment pSelectEnv = new SelectionEnvironmentClass();
            double Length = ConvertPixelsToMapUnits(pActiveView, pSelectEnv.SearchTolerance);

            IPointCollection pointCol = pFeature.Shape as IPointCollection;
            for (int i = 0; i < pointCol.PointCount; i++)
            {
                IGeometry4 pGeom = pointCol.get_Point(i) as IGeometry4;
                //��pGeomһ��ȷ���Ŀռ�ο�
                pGeom.Project(pActiveView.FocusMap.SpatialReference);
                if (pGeom.IsEmpty) return false;

                IProximityOperator pObj = pGeom as IProximityOperator;
                double dblDist = pObj.ReturnDistance(pPnt);
                if (dblDist < Length)
                {
                    return true;
                }
            }

            return false;
        }

        //��ȡMAP�е����п����༭��ͼ��
        public static List<ILayer> LoadAllEditLyr(IMap pMap)
        {
            List<ILayer> listLay = new List<ILayer>();
            UID pUID = new UIDClass();
            pUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";   //UID for IFeatureLayer
            IEnumLayer pEnumLayer = pMap.get_Layers(pUID, true);
            pEnumLayer.Reset();
            ILayer pLayer = pEnumLayer.Next();
            while (pLayer != null)
            {
                IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                IDataset pDataset = pFeatureLayer.FeatureClass as IDataset;
                if (pDataset == null)
                {
                    pLayer = pEnumLayer.Next();
                    continue;
                }
                if (pDataset.Workspace == null)
                {
                    pLayer = pEnumLayer.Next();
                    continue;
                }
                IWorkspaceEdit pWorkspaceEdit = pDataset.Workspace as IWorkspaceEdit;
                if (pWorkspaceEdit.IsBeingEdited())
                {
                    listLay.Add(pLayer);
                }

                pLayer = pEnumLayer.Next();
            }

            return listLay;
        }
        #endregion

        #region ʵ�ֲ�׽����

        private static IPointCollection m_PointCollection;                           //��׽�㼯��
        private static Dictionary<IPointCollection, string> m_dicPointCollection;     //���ڷ�����Ⱦ
        private static IPoint m_LastSnapPnt;                                         //�洢�ϴβ�׽�ĵ㲢���´β�׽ʱ�ػ���ˢ�½���
        //��׽�ڵ�
        public static IPoint SnapPoint(IPoint pPnt, IActiveView pActiveView)
        {
            if (MoData.v_dicSnapLayers == null) return null;

            //����Ҫλ�ڵ�ǰ����ͼ��Χ֮�ڲŽ��в�׽
            if (pPnt.X < pActiveView.Extent.XMax && pPnt.X > pActiveView.Extent.XMin && pPnt.Y < pActiveView.Extent.YMax && pPnt.Y > pActiveView.Extent.YMin)
            {
                return SnapPnt(pPnt, pActiveView);
            }

            return null;
        }
        public static IPoint SnapPnt(IPoint pPnt, IActiveView pActiveView)
        {
            m_dicPointCollection = new Dictionary<IPointCollection, string>();
            m_PointCollection = new MultipointClass();

            //�����ҵ�Ҫ�ش洢��FeatureCache
            double dCacheradius = ConvertPixelDistanceToMapDistance(pActiveView, MoData.v_CacheRadius);
            List<IFeature> listFeats = GetFeats(pActiveView, pPnt, dCacheradius);

            //ֱ�����õ�ͼ����õ���׽�뾶
            double dSearchDist = ConvertPixelDistanceToMapDistance(pActiveView, MoData.v_SearchDist);
            //���ڼ�����������������ľ���,��ȡ����׽��
            IProximityOperator pProximity = pPnt as IProximityOperator;
            foreach (KeyValuePair<ILayer, ArrayList> keyValue in MoData.v_dicSnapLayers)
            {
                bool bVertexPoint = Convert.ToBoolean(keyValue.Value[0]);       //�ڵ㲶׽
                bool bPortPoint = Convert.ToBoolean(keyValue.Value[1]);         //�˵㲶׽
                bool bIntersectPoint = Convert.ToBoolean(keyValue.Value[2]);    //�ཻ�㲶׽
                bool bMidPoint = Convert.ToBoolean(keyValue.Value[3]);          //�е㲶׽
                bool bNearestPoint = Convert.ToBoolean(keyValue.Value[4]);      //����㲶׽

                IFeatureLayer pFeatLay = keyValue.Key as IFeatureLayer;
                if (pFeatLay == null) continue;
                if (bVertexPoint)
                {
                    GetNodeCollection(listFeats, pFeatLay.FeatureClass, pProximity, dSearchDist);
                }

                if (bPortPoint)
                {
                    GetPortPntCollection(listFeats, pFeatLay.FeatureClass, pProximity, dSearchDist);
                }

                if (bIntersectPoint)
                {
                    GetIntersectPntCollection(listFeats, pFeatLay.FeatureClass, pProximity, dSearchDist);
                }

                if (bMidPoint)
                {
                    GetMidPntCollection(listFeats, pFeatLay.FeatureClass, pProximity, dSearchDist);
                }

                if (bNearestPoint)
                {
                    GetNearestPntCollection(listFeats, pFeatLay.FeatureClass, pPnt, dSearchDist);
                }
            }

            //�ҵ�������������ĵ�
            IPoint pClosedPnt = GetClosedPnt(m_PointCollection, dSearchDist, pProximity);
            if (pClosedPnt == null) return null;

            pPnt.PutCoords(pClosedPnt.X, pClosedPnt.Y);

            //�洢�ϴβ�׽�ĵ㲢���´β�׽ʱ�ػ���ˢ�½���(����MAPCONTROL�ϻử�������׽��)
            if (m_LastSnapPnt != null)
            {

                DrawRectangle(pActiveView, m_LastSnapPnt, m_dicPointCollection);
            }
            DrawRectangle(pActiveView, pPnt, m_dicPointCollection);
            m_LastSnapPnt = pPnt;

            return pClosedPnt;
        }
        //��һ���㼯�л�þ����������ĵ�
        private static IPoint GetClosedPnt(IPointCollection pPointCollection, double dSearchDist, IProximityOperator pProximity)
        {
            IPoint pClosedPnt = null;
            double dClosedDistance = dSearchDist;
            for (int i = 0; i < pPointCollection.PointCount; i++)
            {
                IPoint pPoint = pPointCollection.get_Point(i);
                double dScreenDist = pProximity.ReturnDistance(pPoint);
                if (dScreenDist < dClosedDistance)
                {
                    dClosedDistance = dScreenDist;
                    pClosedPnt = pPoint;
                }
            }

            return pClosedPnt;
        }

        //��MAPCONTROL�ϱ��ֳ���׽��
        private static void DrawRectangle(IActiveView pActiveView, IPoint pPoint, Dictionary<IPointCollection, string> dicPointCollection)
        {
            pActiveView.ScreenDisplay.StartDrawing(pActiveView.ScreenDisplay.hDC, (short)esriScreenCache.esriNoScreenCache);
            esriSimpleMarkerStyle pStyle = esriSimpleMarkerStyle.esriSMSCircle;
            foreach (KeyValuePair<IPointCollection, string> keyValue in dicPointCollection)
            {
                for (int i = 0; i < keyValue.Key.PointCount; i++)
                {
                    if (pPoint.X == keyValue.Key.get_Point(i).X && pPoint.Y == keyValue.Key.get_Point(i).Y)
                    {
                        switch (keyValue.Value)
                        {
                            case "PortPnt":
                                pStyle = esriSimpleMarkerStyle.esriSMSCircle;
                                break;
                            case "MidPnt":
                                pStyle = esriSimpleMarkerStyle.esriSMSDiamond;
                                break;
                            case "Node":
                                pStyle = esriSimpleMarkerStyle.esriSMSSquare;
                                break;
                            case "IntersectPnt":
                                pStyle = esriSimpleMarkerStyle.esriSMSX;
                                break;
                            case "NearestPnt":
                                pStyle = esriSimpleMarkerStyle.esriSMSCircle;
                                break;
                        }
                        break;
                    }
                }
            }

            pActiveView.ScreenDisplay.SetSymbol(SetSnapSymbol(pStyle) as ISymbol);
            pActiveView.ScreenDisplay.DrawPoint(pPoint);
            pActiveView.ScreenDisplay.FinishDrawing();
        }
        //���û���׽ʱ��ķ���
        private static ISimpleMarkerSymbol SetSnapSymbol(esriSimpleMarkerStyle pStyle)
        {
            ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();
            ISymbol pSymbol = pMarkerSymbol as ISymbol;

            IRgbColor pRgbColor = new RgbColorClass();
            pRgbColor.Transparency = 0;
            //�������ʽ���ƣ�������ǰ���ķ���
            pSymbol.ROP2 = esriRasterOpCode.esriROPXOrPen;
            pMarkerSymbol.Color = pRgbColor;
            pMarkerSymbol.Style = pStyle;

            //������������ʽ
            pRgbColor.Red = 255;
            pRgbColor.Blue = 0;
            pRgbColor.Green = 0;
            pRgbColor.Transparency = 255;
            pMarkerSymbol.Outline = true;
            pMarkerSymbol.OutlineColor = pRgbColor;
            pMarkerSymbol.OutlineSize = 1;
            pMarkerSymbol.Size = 4.0;

            return pMarkerSymbol;
        }

        //������(��Ļ)����ת����Ϊ��ͼ�ϵľ���
        public static double ConvertPixelDistanceToMapDistance(IActiveView pActiveView, double pPixelDistance)
        {
            tagPOINT tagPOINT = new tagPOINT();
            tagPOINT.x = Convert.ToInt32(pPixelDistance);
            tagPOINT.y = Convert.ToInt32(pPixelDistance);

            WKSPoint pWKSPoint = new WKSPoint();
            pActiveView.ScreenDisplay.DisplayTransformation.TransformCoords(ref pWKSPoint, ref tagPOINT, 1, 6);

            return pWKSPoint.X;
        }

        //��ȡ���ҵ���Ҫ��
        private static List<IFeature> GetFeats(IActiveView pActiveView, IPoint pPnt, double dRadius)
        {
            List<IFeature> listFeats = new List<IFeature>();

            //���ɻ�����
            IGeometry pGeometry = pPnt as IGeometry;
            ITopologicalOperator pTop = pGeometry as ITopologicalOperator;
            pGeometry = pTop.Buffer(dRadius);

            UID pUID = new UIDClass();
            pUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";   //UID for IFeatureLayer
            IEnumLayer pEnumLayer = pActiveView.FocusMap.get_Layers(pUID, true);
            pEnumLayer.Reset();
            ILayer pLay = pEnumLayer.Next();
            while (pLay != null)
            {
                if (MoData.v_dicSnapLayers.ContainsKey(pLay) && pLay.Visible == true)
                {
                    IFeatureLayer pFeatLay = pLay as IFeatureLayer;
                    ISpatialFilter pSpatialFilter = new SpatialFilterClass();
                    pSpatialFilter.Geometry = pGeometry;
                    pSpatialFilter.GeometryField = "SHAPE";
                    switch (pFeatLay.FeatureClass.ShapeType)
                    {
                        case esriGeometryType.esriGeometryPoint:
                            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                            break;
                        default:
                            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                            break;
                    }
                    IFeatureCursor pFeatCur = pFeatLay.FeatureClass.Search(pSpatialFilter, false);
                    IFeature pFeat = pFeatCur.NextFeature();
                    while (pFeat != null)
                    {
                        listFeats.Add(pFeat);
                        pFeat = pFeatCur.NextFeature();
                    }
                    Marshal.ReleaseComObject(pFeatCur);
                }

                pLay = pEnumLayer.Next();
            }

            return listFeats;
        }

        // �˵㲶׽
        private static void GetPortPntCollection(List<IFeature> listFeats, IFeatureClass pFeatureClass, IProximityOperator pProximity, double dSearchDist)
        {
            IPointCollection pPntColTemp = new MultipointClass();

            for (int i = 0; i < listFeats.Count; i++)
            {
                IFeature pFeature = listFeats[i];
                //�жϸ�Featureͼ��
                if (pFeature.Class.ObjectClassID != pFeatureClass.ObjectClassID) continue;

                if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolyline || pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                {
                    double dScreenSearchDist = pProximity.ReturnDistance(pFeature.Shape);
                    if (dScreenSearchDist < 1.5 * dSearchDist)
                    {
                        IGeometryCollection pGeoCollection = pFeature.Shape as IGeometryCollection;
                        for (int j = 0; j < pGeoCollection.GeometryCount; j++)
                        {
                            IGeometry pGeom = pGeoCollection.get_Geometry(j);
                            IPointCollection pPntCol = pGeom as IPointCollection;
                            m_PointCollection.AddPointCollection(pPntCol);
                            pPntColTemp.AddPointCollection(pPntCol);
                        }
                    }
                }
            }

            m_dicPointCollection.Add(pPntColTemp, "PortPnt");
        }

        //�е㲶׽
        private static void GetMidPntCollection(List<IFeature> listFeats, IFeatureClass pFeatureClass, IProximityOperator pProximity, double dSearchDist)
        {
            IPointCollection pPntColTemp = new MultipointClass();

            for (int i = 0; i < listFeats.Count; i++)
            {
                IFeature pFeature = listFeats[i];
                //�жϸ�Featureͼ��
                if (pFeature.Class.ObjectClassID != pFeatureClass.ObjectClassID) continue;

                if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolyline || pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                {
                    double dScreenSearchDist = pProximity.ReturnDistance(pFeature.Shape);
                    if (dScreenSearchDist < 1.5 * dSearchDist)
                    {
                        IGeometryCollection pGeoCollection = pFeature.Shape as IGeometryCollection;
                        for (int j = 0; j < pGeoCollection.GeometryCount; j++)
                        {
                            IGeometry pGeom = pGeoCollection.get_Geometry(j);
                            ISegmentCollection pSegColl = pGeom as ISegmentCollection;

                            for (int k = 0; k < pSegColl.SegmentCount; k++)
                            {
                                ISegment pSeg = pSegColl.get_Segment(k);
                                IPoint pMidPoint = new PointClass();
                                double x = (pSeg.FromPoint.X + pSeg.ToPoint.X) / 2;
                                double y = (pSeg.FromPoint.Y + pSeg.ToPoint.Y) / 2;
                                pMidPoint.PutCoords(x, y);
                                object befor = Type.Missing;
                                object after = Type.Missing;
                                m_PointCollection.AddPoint(pMidPoint, ref befor, ref after);
                                pPntColTemp.AddPoint(pMidPoint, ref befor, ref after);

                            }
                        }
                    }
                }
            }

            m_dicPointCollection.Add(pPntColTemp, "MidPnt");
        }

        //�ڵ㲶׽
        private static void GetNodeCollection(List<IFeature> listFeats, IFeatureClass pFeatureClass, IProximityOperator pProximity, double dSearchDist)
        {
            IPointCollection pPntColTemp = new MultipointClass();

            for (int i = 0; i < listFeats.Count; i++)
            {
                IFeature pFeature = listFeats[i];
                //�жϸ�Featureͼ��
                if (pFeature.Class.ObjectClassID != pFeatureClass.ObjectClassID) continue;

                double dScreenSearchDist = pProximity.ReturnDistance(pFeature.Shape);
                if (dScreenSearchDist < 1.5 * dSearchDist)
                {
                    if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
                    {
                        IPoint pPoint = pFeature.Shape as IPoint;
                        object befor = Type.Missing;
                        object after = Type.Missing;
                        m_PointCollection.AddPoint(pPoint, ref befor, ref after);
                        pPntColTemp.AddPoint(pPoint, ref befor, ref after);
                    }
                    else
                    {
                        IPointCollection pTempPtcln = pFeature.Shape as IPointCollection;
                        m_PointCollection.AddPointCollection(pTempPtcln);
                        pPntColTemp.AddPointCollection(pTempPtcln);
                    }
                }
            }

            m_dicPointCollection.Add(pPntColTemp, "Node");
        }

        //�ཻ�㲶׽
        private static void GetIntersectPntCollection(List<IFeature> listFeats, IFeatureClass pFeatureClass, IProximityOperator pProximity, double dSearchDist)
        {
            IPointCollection pPntColTemp = new MultipointClass();

            List<IFeature> listFeatsTemp = new List<IFeature>();

            for (int i = 0; i < listFeats.Count; i++)
            {
                IFeature pFeature = listFeats[i];
                if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolyline || pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                {
                    double dScreenSearchDist = pProximity.ReturnDistance(pFeature.Shape);
                    if (dScreenSearchDist < 1.5 * dSearchDist)
                    {
                        listFeatsTemp.Add(pFeature);
                    }
                }
            }

            //�ռ��������ཻ�㣬�ռ��Ľ������ظ������ǲ�Ӱ����
            foreach (IFeature pFeat in listFeatsTemp)
            {
                IPointCollection pPntCol = GetAllIntersect(pFeat, listFeatsTemp);
                m_PointCollection.AddPointCollection(pPntCol);
                pPntColTemp.AddPointCollection(pPntCol);
            }

            m_dicPointCollection.Add(pPntColTemp, "IntersectPnt");
        }
        //�õ����н���
        private static IPointCollection GetAllIntersect(IFeature pFeat, List<IFeature> listFeats)
        {
            IPointCollection pItersectCol = new MultipointClass();

            IPointCollection pPntColTemp = pFeat.Shape as IPointCollection;
            IPolyline pPolyline = pPntColTemp as IPolyline;

            foreach (IFeature pFeatTemp in listFeats)
            {
                IGeometry pGeometry = pFeat.Shape;
                if (!pFeat.Equals(pFeatTemp))
                {
                    IPointCollection pItersectColTemp = GetIntersection(pFeatTemp.Shape, pPolyline);
                    if (pItersectColTemp != null)
                    {
                        pItersectCol.AddPointCollection(pItersectColTemp);
                    }
                }
            }

            return pItersectCol;
        }
        private static IPointCollection GetIntersection(IGeometry pIntersect, IPolyline pPolyline)
        {
            if (pIntersect.SpatialReference.SpatialReferenceImpl != pPolyline.SpatialReference.SpatialReferenceImpl)
            {
                pPolyline.Project(pIntersect.SpatialReference);
            }

            ITopologicalOperator pTopoOp = pIntersect as ITopologicalOperator;
            pTopoOp.Simplify();
            IGeometry pGeomResult = pTopoOp.Intersect(pPolyline, esriGeometryDimension.esriGeometry0Dimension);
            if (pGeomResult == null) return null;
            IPointCollection pPointCollection = pGeomResult as IPointCollection;
            return pPointCollection;
        }

        //����㲶׽
        private static void GetNearestPntCollection(List<IFeature> listFeats, IFeatureClass pFeatureClass, IPoint pPnt, double dSearchDist)
        {
            IPointCollection pPntColTemp = new MultipointClass();

            for (int i = 0; i < listFeats.Count; i++)
            {
                IFeature pFeature = listFeats[i];
                if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolyline || pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                {
                    IProximityOperator pProximity = pFeature.Shape as IProximityOperator;
                    IPoint pNearestPnt = pProximity.ReturnNearestPoint(pPnt, esriSegmentExtension.esriNoExtension);
                    object befor = Type.Missing;
                    object after = Type.Missing;
                    m_PointCollection.AddPoint(pNearestPnt, ref befor, ref after);
                    pPntColTemp.AddPoint(pNearestPnt, ref befor, ref after);
                }
            }

            m_dicPointCollection.Add(pPntColTemp, "NearestPnt");
        }
        #endregion



        #region Ҫ���ں�ʱ����������־��¼����غ���
        /// <summary>
        /// ��mapcontrol�ϻ�ȡͼ����ϱ�,���Ȼ�á���Χ��ͼ����
        /// </summary>
        /// <param name="pMapcontrol"></param>
        /// <param name="strGroupLayerName">GroupLayer����</param>
        /// <param name="strLayerName">GroupLayer�µ�ĳ��ͼ������</param>
        /// <returns></returns>
        public static ILayer GetLayerOfGroupLayer(IMapControlDefault pMapcontrol, string strGroupLayerName, string strLayerName)
        {
            ILayer pLayer = null;
            IGroupLayer pGroupLayer = GetGroupLayer(pMapcontrol, strGroupLayerName);//��÷�Χͼ����
            ICompositeLayer pCompositeLayer = pGroupLayer as ICompositeLayer;
            if (pCompositeLayer.Count == 0) return null;
            for (int i = 0; i < pCompositeLayer.Count; i++)
            {
                ILayer mLayer = pCompositeLayer.get_Layer(i);
                if (mLayer.Name == strLayerName)
                {
                    pLayer = mLayer;
                    break;
                }
            }
            return pLayer;//���ͼ����ϱ�
        }

        /// <summary>
        /// ����ͼ��������ȡ��ͼ��
        /// </summary>
        /// <param name="pMapcontrol"></param>
        /// <param name="strName">ͼ��������</param>
        /// <returns></returns>
        public static IGroupLayer GetGroupLayer(IMapControlDefault pMapcontrol, string strName)
        {
            IGroupLayer pGroupLayer = new GroupLayerClass();
            for (int i = 0; i < pMapcontrol.Map.LayerCount; i++)
            {
                ILayer pLayer = pMapcontrol.Map.get_Layer(i);
                if (pLayer is IGroupLayer)
                {
                    if (pLayer.Name == strName)
                    {
                        pGroupLayer = pLayer as IGroupLayer;
                        break;
                    }
                }
            }
            return pGroupLayer;
        }

        /// <summary>
        /// �������ַ����õ���ΧPolygon
        /// </summary>
        /// <param name="strCoor">�����ַ���,��ʽΪX@Y,�Զ��ŷָ�</param>
        /// <returns></returns>
        public static IPolygon GetPolygonByCol(string strCoor)
        {
            try
            {
                object after = Type.Missing;
                object before = Type.Missing;
                IPolygon polygon = new PolygonClass();
                IPointCollection pPointCol = (IPointCollection)polygon;
                string[] strTemp = strCoor.Split(',');
                for (int index = 0; index < strTemp.Length; index++)
                {
                    string CoorLine = strTemp[index];
                    string[] coors = CoorLine.Split('@');

                    double X = Convert.ToDouble(coors[0]);
                    double Y = Convert.ToDouble(coors[1]);

                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(X, Y);
                    pPointCol.AddPoint(pPoint, ref before, ref after);
                }

                polygon = (IPolygon)pPointCol;
                polygon.Close();

                if (IsValidateGeometry(polygon)) return polygon;
                return null;
            }
            catch (Exception eError)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
                //******************************************
                return null;
            }
        }

        // �ӷ�ΧPolygon�õ���Ӧ�������ַ���
        public static string GetColByPolygon(IPolygon polygon)
        {
            if (polygon == null) return "";
            IPointCollection pPointCol = (IPointCollection)polygon;

            try
            {
                StringBuilder sb = new StringBuilder();
                for (int index = 0; index < pPointCol.PointCount; index++)
                {
                    IPoint pPoint = pPointCol.get_Point(index);

                    string X = Convert.ToString(pPoint.X);
                    string Y = Convert.ToString(pPoint.Y);

                    if (sb.Length != 0)
                    {
                        sb.Append(",");
                    }
                    sb.Append(X + "@" + Y);
                }

                return sb.ToString();
            }
            catch (Exception eError)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
                //******************************************
                return "";
            }
        }

        // ���һ���������Ƿ�Ƿ�
        private static bool IsValidateGeometry(IGeometry pgeometry)
        {
            // ��ȡ��Geometry��ԭʼ����
            IPointCollection pOrgPointCol = (IPointCollection)pgeometry;

            // ��ȡ��Geometry��ԭʼPart��
            IGeometryCollection pOrgGeometryCol = (IGeometryCollection)pgeometry;

            // ��Ŀ����п�¡�Ͷ�Ӧ�Ĵ���
            IClone pClone = (IClone)pgeometry;
            IGeometry pGeometryTemp = (IPolygon)pClone.Clone();
            ITopologicalOperator pTopo = (ITopologicalOperator)pGeometryTemp;
            pTopo.Simplify();

            // �õ��µ�Geometry
            pGeometryTemp = (IPolygon)pTopo;

            // ��ȡ�µ�Geometry�ĵ���
            IPointCollection pObjPointCol = (IPointCollection)pGeometryTemp;

            // ��ȡ�µ�Geometry��Part��
            IGeometryCollection pObjGeometryCol = (IGeometryCollection)pGeometryTemp;

            // ���бȽ�
            if (pOrgPointCol.PointCount != pObjPointCol.PointCount) return false;

            if (pOrgGeometryCol.GeometryCount != pObjGeometryCol.GeometryCount) return false;

            return true;
        }
        #endregion
    }
}
