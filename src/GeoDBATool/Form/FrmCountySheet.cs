using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using stdole;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

namespace GeoDBATool
{
    public partial class FrmCountySheet : DevComponents.DotNetBar.Office2007Form
    {
        //������
        //private string v_Scale = "";
        //ѡ����ܷ�Χ
        //private IGeometry m_CountyGeometry = null;
        //public IGeometry CountyGEOMETRY
        //{
        //    get 
        //    {
        //        return m_CountyGeometry;
        //    }
        //    set 
        //    {
        //        m_CountyGeometry=value;
        //    }
        //}

        //ѡ��ķ�Χ��Ҫ����Ϣ����
        private Dictionary<string, IGeometry> m_rangeFeaDic = null;
        public Dictionary<string, IGeometry> RANGEFEADIC
        {
            get 
            {
                return m_rangeFeaDic;
            }
            set 
            {
                m_rangeFeaDic = value;
            }
        }

        public FrmCountySheet(string pScale,string textStr)
        {
            InitializeComponent();
            axToolbarControl1.SetBuddyControl(countySheetControl.Object);

            Exception eError = null;

            this.Text = textStr;
            //v_Scale = pScale;
            //���ͼ��
            if (this.Text == "��������Χѡ��")
            {
                AddRangeLayer(ModData.countyPath, pScale, "NAME",out eError);
            }
            else if (this.Text == "ͼ��ѡ��")
            {
                AddRangeLayer(ModData.MapPath, pScale, "MAP_NEWNO", out eError);
            }
            if(eError!=null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                return;
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            Exception eError = null;
            if (countySheetControl.Map.LayerCount == 0) return;
            IFeatureLayer pFeaLayer = countySheetControl.Map.get_Layer(0) as IFeatureLayer;
            IFeatureSelection pFeaSel = pFeaLayer as IFeatureSelection;
            if (pFeaSel.SelectionSet == null || pFeaSel.SelectionSet.Count == 0)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ͼ��ѡ��Χ��");
                return;
            }
            if (pFeaSel.SelectionSet != null && pFeaSel.SelectionSet.Count != 0)
            {
                //m_CountyGeometry = GetFeaGeometry(pFeaSel, pFeaLayer,"NAME", out m_rangeFeaDic);
                if (this.Text == "��������Χѡ��")
                {
                    m_rangeFeaDic = GetFeaGeometry(pFeaSel, pFeaLayer, "NAME", out eError);

                }
                else if (this.Text == "ͼ��ѡ��")
                {
                    m_rangeFeaDic = GetFeaGeometry(pFeaSel, pFeaLayer, "MAP_NEWNO", out eError);
                }
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                    return;
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        //���ѡ���ͼ��ķ�Χ
        private Dictionary<string,IGeometry> GetFeaGeometry(IFeatureSelection pFeatureSel, IFeatureLayer pMapLayer,string fieldName, out Exception eError)
        {
            eError = null;
            Dictionary<string, IGeometry> feaDic = new Dictionary<string, IGeometry>();
            IEnumIDs pEnumIDs = pFeatureSel.SelectionSet.IDs;
            int id = pEnumIDs.Next();
            //IGeometry pGeo = null;
            //int rangNOIndex = pMapLayer.FeatureClass.Fields.FindField("RANGE_NO");  //��Χ������
            int rangeNameIndex = pMapLayer.FeatureClass.Fields.FindField(fieldName);   //��Χ��������
            if (rangeNameIndex == -1) //||rangNOIndex==-1
            {
                eError=new Exception("��Χ�Ż�Χ�����ֶβ����ڣ�");
                return null;
            }
            while (id != -1)
            {
                IFeature pFeat = pMapLayer.FeatureClass.GetFeature(id);
                //string rangeNO = pFeat.get_Value(rangNOIndex).ToString();   //��Χ��
                string rangeName = pFeat.get_Value(rangeNameIndex).ToString();//��Χ����
                if(!feaDic.ContainsKey(rangeName))
                {
                    feaDic.Add(rangeName, pFeat.Shape);
                }
                //if (pGeo == null)
                //{
                //    pGeo = pFeat.Shape;
                //}
                //else
                //{
                //    ITopologicalOperator pTop = pGeo as ITopologicalOperator;
                //    pGeo = pTop.Union(pFeat.Shape);
                //}
                id = pEnumIDs.Next();
            }
            return feaDic;
        }

        //���ͼ��
        private void AddRangeLayer(string wsPath, string pScale, string fieldName, out Exception eError)
        {
            eError = null;
            try
            {
                IWorkspaceFactory pWorkSpaceFactory = new AccessWorkspaceFactoryClass();
                IWorkspace pWorkSpace = null;
                IFeatureClass pFeatureClass = null;
                if (ModData.countyPath != "")
                {
                    pWorkSpace = pWorkSpaceFactory.OpenFromFile(wsPath, 0);
                    IFeatureWorkspace pFeatureWorkSpace = pWorkSpace as IFeatureWorkspace;
                    string rangeLayerName = GetMapFrameName(wsPath, pScale, out eError);
                    if (rangeLayerName == "" || eError != null)
                    {
                        return;
                    }
                    pFeatureClass = pFeatureWorkSpace.OpenFeatureClass(rangeLayerName);
                }
                if (pFeatureClass != null)
                {
                    IDataset pDataSet = pFeatureClass as IDataset;
                    IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                    pFeatureLayer.FeatureClass = pFeatureClass;
                    ILayer pLayer = pFeatureLayer as ILayer;
                    pLayer.Name = pDataSet.Name;
                    countySheetControl.AddLayer(pLayer);

                    //ͼ����ʾ��ע
                    SetLableToGeoFeatureLayer(pFeatureLayer as IGeoFeatureLayer, fieldName, Convert.ToInt32(pScale), countySheetControl.ReferenceScale);
                }
            }
            catch (Exception ex)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(ex, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(ex, null, DateTime.Now);
                }
                //********************************************************************

                eError = ex;
                return;
            }
        }

        /// <summary>
        /// �������Ա�ע
        /// </summary>
        /// <param name="pGeoFeatureLayer">ͼ��</param>
        /// <param name="vLabelField">�����ֶ�</param>
        /// <param name="vMapFrameScale">ͼ�������</param>
        /// <param name="vMapRefrenceScale">�ο�������</param>
        private void SetLableToGeoFeatureLayer(IGeoFeatureLayer pGeoFeatureLayer, string vLabelField, int vMapFrameScale, double vMapRefrenceScale)
        {
            IAnnotateLayerPropertiesCollection pAnnoLayerProperCol = pGeoFeatureLayer.AnnotationProperties;
            IAnnotateLayerProperties pAnnoLayerProper;
            IElementCollection placedElements;
            IElementCollection unplacedElements;
            //�õ���ǰ��ĵ�ǰ��ע����
            pAnnoLayerProperCol.QueryItem(0, out pAnnoLayerProper, out placedElements, out unplacedElements);
            ILabelEngineLayerProperties pLabelEngineLayerProper = (ILabelEngineLayerProperties)pAnnoLayerProper;
            IBasicOverposterLayerProperties4 pBasicOverposterLayerProper = (IBasicOverposterLayerProperties4)pLabelEngineLayerProper.BasicOverposterLayerProperties;
            //��ע������
            ITextSymbol pTextSymbol = pLabelEngineLayerProper.Symbol;
            IRgbColor pRGBColor = new RgbColorClass();
            pRGBColor.Red = 0;
            pRGBColor.Blue = 255;
            pRGBColor.Green = 0;
            pTextSymbol.Color = pRGBColor;
            stdole.StdFont pStdFont = new stdole.StdFontClass();
            IFontDisp pFont = (IFontDisp)pStdFont;
            pFont.Name = "����";
            if (vMapRefrenceScale != 0)
            {
                double size = (vMapFrameScale / 3) * vMapFrameScale / vMapRefrenceScale;
                pFont.Size = (decimal)size;
            }
            pTextSymbol.Font = pFont;
            //��ע����
            pLabelEngineLayerProper.Expression = "[" + vLabelField + "]";
            pBasicOverposterLayerProper.NumLabelsOption = esriBasicNumLabelsOption.esriOneLabelPerPart;
            //��ע�ķ�����Ϣ
            pBasicOverposterLayerProper.PolygonPlacementMethod = esriOverposterPolygonPlacementMethod.esriAlwaysHorizontal;
            //��ע���뼸��ͼ�εĴ�С��ϵ
            pBasicOverposterLayerProper.PlaceOnlyInsidePolygon = false;
            //������ע
            pGeoFeatureLayer.DisplayAnnotation = true;
        }

        /// <summary>
        /// �ж��Ƿ��������Ӧ�����ߵ�ͼ��
        /// </summary>
        /// <param name="MapPath">ͼ��·��</param>
        /// <param name="StrScale">������</param>
        /// <returns></returns>
        private string GetMapFrameName(string MapPath, string StrScale, out Exception eError)
        {
            eError = null;
            IWorkspaceFactory pWorkSpaceFactory = new AccessWorkspaceFactoryClass();
            IWorkspace pWorkSpace = null;
            IEnumDataset pEnumDataSet = null;
            IDataset pDataSet = null;
            IFeatureDataset pFeatureDataSet = null;
            IEnumDataset pEnumFC = null;
            IDataset pFC = null;
            string LayerName = "";
            string MapFrameName = "";
            if (MapPath != "")
            {
                pWorkSpace = pWorkSpaceFactory.OpenFromFile(MapPath, 0);
                pEnumDataSet = pWorkSpace.get_Datasets(esriDatasetType.esriDTFeatureClass);
                pEnumDataSet.Reset();
                pDataSet = pEnumDataSet.Next();
                while (pDataSet != null)
                {
                    if (pDataSet is IFeatureClass)
                    {
                        LayerName = pDataSet.Name;
                        if (LayerName.Contains(StrScale))
                        {
                            MapFrameName = LayerName;
                        }
                    }
                    else if (pDataSet is IFeatureDataset)
                    {
                        pFeatureDataSet = pDataSet as IFeatureDataset;
                        pEnumFC = pFeatureDataSet.Subsets;
                        pEnumFC.Reset();
                        pFC = pEnumFC.Next();
                        while (pFC != null)
                        {
                            LayerName = pFC.Name;
                            if (LayerName.Contains(StrScale))
                            {
                                MapFrameName = LayerName;
                            }
                        }
                    }
                    else
                    {
                        MapFrameName = "";
                    }
                    pDataSet = pEnumDataSet.Next();
                }
                if (MapFrameName == "")
                {
                    eError = new Exception("δ�ҵ�������Ϊ'" + StrScale + "'�ķ�Χ���ݣ����飡");
                    return "";
                }
            }
            return MapFrameName;
        }
    }
}