using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace GeoDataEdit
{
    public static class ModForEdit
    {
        //������ɫ
        public static IRgbColor CreatRgbColor ( int nR , int nG , int nB , bool blTransparency )
        {
            ESRI.ArcGIS.Display.IRgbColor pRgbColor=null;
            pRgbColor = new ESRI.ArcGIS.Display.RgbColor ();

            if ( blTransparency == true )
                pRgbColor.Transparency = 0;
            else
            {
                pRgbColor.Red = nR;
                pRgbColor.Green = nG;
                pRgbColor.Blue = nB;
            }
            return pRgbColor;
        }

        //���ɺ��ʵ��С
        public static double ScaleChange ( ref IMap pMap , double fValue )
        {
            if ( pMap == null ) return fValue;
            if ( pMap.MapScale == 0 || pMap.ReferenceScale == 0 )
                return fValue;
            else
                return fValue * pMap.MapScale / pMap.ReferenceScale;
        }

        //ˢ��Ҫ��
        public static void RefreshFeature ( ref IMap pMap , IFeature pData )
        {
            ESRI.ArcGIS.Carto.IActiveView pActiveView;
            pActiveView = pMap as ESRI.ArcGIS.Carto.IActiveView;

            if ( pData == null )
            {
                pActiveView.Refresh ();
                return;
            }

            ESRI.ArcGIS.Geodatabase.IInvalidArea pRefreshArea;
            pRefreshArea = new ESRI.ArcGIS.Carto.InvalidAreaClass ();
            pRefreshArea.Display = pActiveView.ScreenDisplay;
            pRefreshArea.Add ( pData );
            pRefreshArea.Invalidate ( -2 /*ESRI.ArcGIS.Display.esriScreenCache.esriAllScreenCaches*/);

            pRefreshArea = null;
        }

        //���������õ�Tool�����ֽ�ò���
        public static bool GetToolStepInfo ( string sToolShortName , string sCondition , ref string sIputType ,
                                           ref string sInputTips , ref string sKeyWords , ClsEditorMain clsEditorMain )
        {

            ////sUserInputΪ�û������봮��Ҫ���ô��Ƿ��ǺϷ����������ƣ�������򷵻ظ������UID,���򷵻ؿ�
            //string sXPath = "//*[@ShortName='" + sToolShortName.ToUpper () + "']";
            //XmlNode nodeXml = clsEditorMain.docEditCmdCollectionXml.SelectSingleNode ( sXPath );
            //if ( nodeXml == null ) return false;
            //if ( nodeXml.Name != "Tool" && nodeXml.Name != "Command" ) return false;

            //sXPath = ".//*[@conditons='" + sCondition + "']";
            //XmlNode nodeChildXml = nodeXml.SelectSingleNode ( sXPath );
            //if ( nodeChildXml == null ) return false;

            //sIputType = nodeChildXml.Attributes["Type"].Value;
            //sInputTips = nodeChildXml.Attributes["Text"].Value;
            //sKeyWords = nodeChildXml.Attributes["Keyword"].Value;

            return true;

        }

        //
        public static double ConvertPixelsToMapUnits ( IActiveView pActiveView , double nPixelUnits )
        {
            tagRECT pDeviceRECT;
            pDeviceRECT = pActiveView.ScreenDisplay.DisplayTransformation.get_DeviceFrame ();

            int nPixelExtent;
            nPixelExtent = pDeviceRECT.right - pDeviceRECT.left;

            double nRealWorldDisplayExtent;
            nRealWorldDisplayExtent = pActiveView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;

            double nSizeOfOnePixel;
            nSizeOfOnePixel = nRealWorldDisplayExtent / nPixelExtent;
            return nPixelUnits * nSizeOfOnePixel;
        }

        //��UID�õ��������
        public static string GetShortNameFromKey ( string sKey , ClsEditorMain clsEditorMain )
        {
            string sXPath = "//*[@Key='" + sKey + "']";
            XmlNode nodeXml = clsEditorMain.docEditCmdCollectionXml.SelectSingleNode ( sXPath );
            if ( nodeXml == null ) return "";
            return nodeXml.Attributes["ShortName"].Value;
        }

        public static ILayer GetLayerFromName ( string sCodeName , IMap pMap )
        {
            for ( int i = 0 ; i < pMap.LayerCount ; i++ )
            {
                ILayer pLayer = pMap.get_Layer ( i );
                if ( pLayer is IDataLayer )
                {
                    IDataLayer pDataLayer = pLayer as IDataLayer;
                    IDatasetName pDatasetName = pDataLayer.DataSourceName as IDatasetName;
                    if ( pDatasetName.Name.ToLower () == sCodeName.ToLower () )
                    {
                        return pLayer;
                    }
                }

            }
            return null;
        }


        public static IPolygon MakePolygonFromRing ( IRing pRing , bool blMakeAreaZ/*ʹ���Ϊ��*/)
        {
            if ( pRing == null || pRing.IsEmpty || pRing.IsClosed == false ) return null;
            IPolygon pPolygon = new PolygonClass ();
            IGeometryCollection pGeometryCollection = pPolygon as IGeometryCollection;

            object pBObj = Type.Missing;
            object pAObj = Type.Missing;
            IGeometry pGeometry = pRing as IGeometry;
            pGeometryCollection.AddGeometry ( pGeometry , ref pBObj , ref pAObj );

            if ( blMakeAreaZ == false ) return pPolygon;

            IArea pArea = pPolygon as IArea;
            if ( pArea.Area >= 0 ) return pPolygon;

            ESRI.ArcGIS.esriSystem.IClone pClone = pRing as ESRI.ArcGIS.esriSystem.IClone;
            IRing pNewRing = pClone.Clone () as IRing;
            pNewRing.ReverseOrientation ();
            pGeometry = pNewRing as IGeometry;
            pGeometryCollection.AddGeometry ( pGeometry , ref pBObj , ref pAObj );
            return pPolygon;
        }

        //�߹���
        public static void MakePolygonFromPolyline ( IWorkspaceEdit pWorkspaceEdit , IFeatureLayer pPolyLineFeaLayer ,
                                                   IFeatureLayer pPolygonFeaLayer )
        {
            if ( pPolyLineFeaLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPolyline ) return;
            if ( pPolygonFeaLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPolygon ) return;

            pWorkspaceEdit.StartEditOperation ();

            IFeatureSelection pFeaSelection = pPolyLineFeaLayer as IFeatureSelection;
            ISelectionSet pSelectionSet = pFeaSelection.SelectionSet;
            IEnumIDs pEnumIDs = pSelectionSet.IDs;
            int ID = pEnumIDs.Next ();
            while ( ID != -1 )
            {
                IFeature pFeature = pPolyLineFeaLayer.FeatureClass.GetFeature ( ID );
                if ( pFeature == null ) continue;
                IPolyline pPolyLine = pFeature.ShapeCopy as IPolyline;

                //��ѡ�����ת����棬����Ϊ���Feature
                IPolygon pPolygon = PolylineToPolygon ( ref pPolyLine );

                IFeature pPolygonFeature = pPolygonFeaLayer.FeatureClass.CreateFeature ();
                pPolygonFeature.Shape = pPolygon;
                pPolygonFeature.Store ();

                //��������
                for ( int i = 0 ; i < pFeature.Fields.FieldCount ; i++ )
                {
                    string sFieldName = pFeature.Fields.get_Field ( i ).Name;

                    if ( sFieldName == "SHAPE" ) continue;
                    int iIndex = pPolygonFeature.Fields.FindField ( sFieldName );

                    if ( ( iIndex > -1 ) && ( pPolygonFeature.Fields.get_Field ( iIndex ).Editable == true ) )
                    {
                        pPolygonFeature.set_Value ( iIndex , pFeature.get_Value ( i ) );
                    }
                }

                ID = pEnumIDs.Next ();
            }

            pWorkspaceEdit.StopEditOperation ();
        }

        //�߹���
        private static IPolygon PolylineToPolygon ( ref  IPolyline pPolyline )
        {
            try
            {
                //����һ���µ�Polygon geometry.
                IGeometryCollection pPolygonGeometryCol = new PolygonClass ();

                //��¡����Ҫ������Polyline
                IClone pClone = pPolyline as IClone;

                IGeometryCollection pGeoms_Polyline = pClone.Clone () as IGeometryCollection;
                object pBObj = Type.Missing;
                object pAObj = Type.Missing;
                for ( int i = 0 ; i < pGeoms_Polyline.GeometryCount ; i++ )
                {
                    //ͨ��Polyline��ÿ��Path����Ϊһ���µ�Ring,����Ring���ӵ�һ���µ�Polygon
                    ISegmentCollection pSegs_Ring = new RingClass ();
                    pSegs_Ring.AddSegmentCollection ( pGeoms_Polyline.get_Geometry ( i ) as ISegmentCollection );
                    pPolygonGeometryCol.AddGeometry ( pSegs_Ring as IGeometry , ref pBObj , ref pAObj );
                }
                //���ɵ�Polygon��ת��˳����ܲ���ȷ,Ϊȷ����ȷ����SimplifyPreserveFromTo
                IPolygon pNewPolygon = new PolygonClass ();
                pNewPolygon = pPolygonGeometryCol as IPolygon;
                pNewPolygon.SimplifyPreserveFromTo ();
                return pNewPolygon;
            }
            catch
            {
                return null;
            }

        }

        //�湹����
        private static IPolyline PolygonToPolyline ( ref  IPolygon pPolygon )
        {
            try
            {
                //����һ���µ�Polygon geometry.
                IGeometryCollection pPolygonGeometryCol = new PolylineClass ();

                //��¡����Ҫ������Polyline
                IClone pClone = pPolygon as IClone;

                IGeometryCollection pGeoms_Polygon = pClone.Clone () as IGeometryCollection;
                object pBObj = Type.Missing;
                object pAObj = Type.Missing;
                for ( int i = 0 ; i < pGeoms_Polygon.GeometryCount ; i++ )
                {
                    //ͨ��Polyline��ÿ��Path����Ϊһ���µ�Ring,����Ring���ӵ�һ���µ�Polygon
                    ISegmentCollection pSegs_Path = new Path () as ISegmentCollection;
                    pSegs_Path.AddSegmentCollection ( pGeoms_Polygon.get_Geometry ( i ) as ISegmentCollection );
                    pPolygonGeometryCol.AddGeometry ( pSegs_Path as IGeometry , ref pBObj , ref pAObj );
                }
                //���ɵ�Polygon��ת��˳����ܲ���ȷ,Ϊȷ����ȷ����SimplifyPreserveFromTo
                IPolyline pNewPolyline = new PolylineClass ();
                pNewPolyline = pPolygonGeometryCol as IPolyline;

                return pNewPolyline;
            }
            catch
            {
                return null;
            }

        }


        public static bool CutPolygonByLine ( IPolygon pPolygon , IPolyline pCutLine , ref IGeometry pRight , ref  IGeometry pLeft )
        {
            pRight = null;
            pLeft = null;
            if ( pPolygon == null || pPolygon.IsEmpty || pCutLine == null || pCutLine.IsEmpty )
                return false;
            try
            {
                ITopologicalOperator pTopologicalOperator = pPolygon as ITopologicalOperator;
                pTopologicalOperator.Cut ( pCutLine , out pLeft , out pRight );
                return true;
            }
            catch ( Exception e )
            {

                return false;
            }
        }
    }



}
