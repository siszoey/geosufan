using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace GeoProperties.UserControls
{
    public partial class uctrSource : UserControl
    {
        ILayer m_pLayer;

        public uctrSource(ILayer pLayer)
        {
            m_pLayer = pLayer;

            try
            {
                InitializeComponent();
                GetLayerExtent();
                GetDataSource();
            }
            catch
            {
                
            }
        }

        private void GetLayerExtent()
        {
            IEnvelope pEnvelop = (m_pLayer as IGeoDataset).Extent;
            lblWest.Text = pEnvelop.XMin.ToString();
            lblEast.Text = pEnvelop.XMax.ToString();
            lblNorth.Text = pEnvelop.YMax.ToString();
            lblSouth.Text = pEnvelop.YMin.ToString();
        }
        private void GetDataSource()
        {
            IFeatureLayer pFeatureLayer = (IFeatureLayer)m_pLayer;
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            IDataset pFeatureDataset = pFeatureClass as IDataset;
            ISpatialReference pSpatialRef;
            pSpatialRef= (pFeatureClass as IGeoDataset).SpatialReference;
            
            //�ж����ת����ͶӰ�����Ƿ�ɹ� xisheng 20111122
            IProjectedCoordinateSystem pProjectedCoordinateSystem = ((pFeatureClass as IGeoDataset).SpatialReference) as IProjectedCoordinateSystem;
            IGeographicCoordinateSystem pGeographicCoordinateSystem = null ;
            if (pProjectedCoordinateSystem == null)
            {
                pGeographicCoordinateSystem = ((pFeatureClass as IGeoDataset).SpatialReference) as IGeographicCoordinateSystem;
            }
            else
            {
                pGeographicCoordinateSystem = pProjectedCoordinateSystem.GeographicCoordinateSystem;
            }
            //**************************************************end
            string strDataType = pFeatureLayer.DataSourceType;
            string strLocation = pFeatureDataset.Workspace.PathName;
            string strFeatureDSName = pFeatureDataset.Name;
            string strFCName = pFeatureClass.AliasName;
            string strFeatureType = pFeatureClass.FeatureType.ToString().Substring(6);
            string strGeometryType = pFeatureClass.ShapeType.ToString().Substring(12);

            if (pProjectedCoordinateSystem != null)
            {
                string strPCSName = pProjectedCoordinateSystem.Name;
                string strProjection = pProjectedCoordinateSystem.Projection.Name;
                string strFalseEasting = pProjectedCoordinateSystem.FalseEasting.ToString();
                string strFalseNorthing = pProjectedCoordinateSystem.FalseNorthing.ToString();
                string strCentralMeridian = pProjectedCoordinateSystem.get_CentralMeridian(true).ToString();
                string strScaleFactor = pProjectedCoordinateSystem.ScaleFactor.ToString();
                string strLinearUnit = pProjectedCoordinateSystem.CoordinateUnit.Name;

                string strGCSName = pGeographicCoordinateSystem.Name;
                string strDatum = pGeographicCoordinateSystem.Datum.Name;
                string strPrimeMeridian = pGeographicCoordinateSystem.PrimeMeridian.Name;
                string strAngularUnit = pGeographicCoordinateSystem.CoordinateUnit.Name;
                txtDataSource.Text =
                    "Data Type:\t\t\t" + strDataType +
                    "\r\nλ��:\t\t\t" + strLocation +
                    "\r\nҪ�ؼ�:\t\t" + strFeatureDSName +
                    "\r\nҪ����:\t\t\t" + strFCName +
                    "\r\nҪ������:\t\t\t" + strFeatureType +
                    "\r\n��������:\t\t\t" + strGeometryType +
                    "\r\n\r\nͶӰ����ϵ��Ϣ:\t" + strPCSName +
                    "\r\nͶӰ:\t\t\t" + strProjection +
                    "\r\nFalse_Easting:\t\t\t" + strFalseEasting +
                    "\r\nFalse_Northing:\t\t\t" + strFalseNorthing +
                    "\r\nCentral_Meridian:\t\t" + strCentralMeridian +
                    "\r\nScale_Factor:\t\t\t" + strScaleFactor +
                    "\r\nLinear Unit:\t\t\t" + strLinearUnit +
                    "\r\n\r\nGeographic Coordinate System:\t" + strGCSName +
                    "\r\nDatum:\t\t\t\t" + strDatum +
                    "\r\nPrime Meridian:\t\t\t" + strPrimeMeridian +
                    "\r\nAngular Unit:\t\t\t" + strAngularUnit + "\r\n";
            }
            else
            {
                string strGCSName = pGeographicCoordinateSystem.Name;
                string strDatum = pGeographicCoordinateSystem.Datum.Name;
                string strPrimeMeridian = pGeographicCoordinateSystem.PrimeMeridian.Name;
                string strAngularUnit = pGeographicCoordinateSystem.CoordinateUnit.Name;
                txtDataSource.Text =
                    "Data Type:\t\t\t" + strDataType +
                    "\r\nLocation:\t\t\t" + strLocation +
                    "\r\nFeature Dataset:\t\t" + strFeatureDSName +
                    "\r\nFeature Class:\t\t\t" + strFCName +
                    "\r\nFeature Type:\t\t\t" + strFeatureType +
                    "\r\nGeometry Type:\t\t\t" + strGeometryType +
                    "\r\n\r\nGeographic Coordinate System:\t" + strGCSName +
                    "\r\nDatum:\t\t\t\t" + strDatum +
                    "\r\nPrime Meridian:\t\t\t" + strPrimeMeridian +
                    "\r\nAngular Unit:\t\t\t" + strAngularUnit + "\r\n";
            }
        }

        private string MapUnitChinese(esriUnits mapUnit)
        {
            string mapUnitChinese = "";
            switch (mapUnit)
            {
                case esriUnits.esriCentimeters:
                    mapUnitChinese = "����";
                    break;
                case esriUnits.esriDecimeters:
                    mapUnitChinese = "����";
                    break;
                case esriUnits.esriKilometers:
                    mapUnitChinese = "ǧ��";
                    break;
                case esriUnits.esriMeters:
                    mapUnitChinese = "��";
                    break;
                case esriUnits.esriMillimeters:
                    mapUnitChinese = "����";
                    break;
                case esriUnits.esriInches:
                    mapUnitChinese = "Ӣ��";
                    break;
                case esriUnits.esriUnknownUnits:
                    mapUnitChinese = "δ֪��λ";
                    break;
                default:
                    mapUnitChinese = "";
                    break;
            }
            return mapUnitChinese;
        }

        private void btnSetDataSource_Click(object sender, EventArgs e)
        {

        }
    }
}