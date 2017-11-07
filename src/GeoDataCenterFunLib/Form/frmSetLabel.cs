using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using stdole;


namespace GeoDataCenterFunLib
{
    public partial class frmSetLabel : DevComponents.DotNetBar.Office2007Form
    {
        public frmSetLabel()
        {
            InitializeComponent();
        }

        private IGeoFeatureLayer pGeoFeatLayer = null;
        public IGeoFeatureLayer GeoFeatLayer
        {
            set { pGeoFeatLayer = value; }
        }
        private IMapControlDefault pMapControl = null;
        public IMapControlDefault MapControl
        {
            set { pMapControl = value; }
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            FontDialog pFontDialog = new FontDialog();
            pFontDialog.ShowHelp = true;
            if (pFontDialog.ShowDialog() == DialogResult.OK)
            {
                LabelText.Font = pFontDialog.Font;
            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            ColorDialog pColorDialog = new ColorDialog();
            pColorDialog.AllowFullOpen = true;
            pColorDialog.AnyColor = true;
            if (pColorDialog.ShowDialog() == DialogResult.OK)
            {
                LabelText.ForeColor = pColorDialog.Color;
            }

        }
        private void AddLabel(string StrDisplayField)
        {
            pGeoFeatLayer.DisplayAnnotation = false;
            pMapControl.ActiveView.Refresh();

            pGeoFeatLayer.DisplayField = StrDisplayField;

            IAnnotateLayerPropertiesCollection pAnnoProps = null;
            pAnnoProps = pGeoFeatLayer.AnnotationProperties;

            ILineLabelPosition pPosition = null;
            pPosition = new LineLabelPositionClass();
            pPosition.Parallel = true;
            pPosition.Above = true;

            ILineLabelPlacementPriorities pPlacement = new LineLabelPlacementPrioritiesClass();
            IBasicOverposterLayerProperties4 pBasic = new BasicOverposterLayerPropertiesClass();
            pBasic.FeatureType = esriBasicOverposterFeatureType.esriOverposterPolyline;
            pBasic.LineLabelPlacementPriorities = pPlacement;
            pBasic.LineLabelPosition = pPosition;
            pBasic.BufferRatio = 0;
            pBasic.FeatureWeight = esriBasicOverposterWeight.esriHighWeight;
            pBasic.NumLabelsOption = esriBasicNumLabelsOption.esriOneLabelPerPart;
            //pBasic.PlaceOnlyInsidePolygon = true;//���ڵ����ڲ���ʾ��ע  deleted by chulili s20111018 �����ϲ�û���������ã���仰Ӧע�͵����������Ǵ���

            ILabelEngineLayerProperties pLabelEngine = null;
            pLabelEngine = new LabelEngineLayerPropertiesClass();
            pLabelEngine.BasicOverposterLayerProperties = pBasic as IBasicOverposterLayerProperties;
            pLabelEngine.Expression = "[" + StrDisplayField + "]";
            ITextSymbol pTextSymbol = null;
            pTextSymbol = pLabelEngine.Symbol;
            System.Drawing.Font pFont = null;
            pFont = LabelText.Font;
            IFontDisp pFontDisp = ESRI.ArcGIS.ADF.Converter.ToStdFont(pFont);
            pTextSymbol.Font = pFontDisp;

            IRgbColor pColor = new RgbColorClass();
            pColor.Red = Convert.ToInt32(LabelText.ForeColor.R);
            pColor.Green = Convert.ToInt32(LabelText.ForeColor.G);
            pColor.Blue = Convert.ToInt32(LabelText.ForeColor.B);
            pTextSymbol.Color = pColor as IColor;
            pLabelEngine.Symbol = pTextSymbol;

            IAnnotateLayerProperties pAnnoLayerProps = null;
            pAnnoLayerProps = pLabelEngine as IAnnotateLayerProperties;
            pAnnoLayerProps.LabelWhichFeatures = esriLabelWhichFeatures.esriAllFeatures;
            pAnnoProps.Clear();

            pAnnoProps.Add(pAnnoLayerProps);
            pGeoFeatLayer.DisplayAnnotation = true;
            pMapControl.ActiveView.Refresh();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string DisplayField = "";
            DisplayField = CmbFields.Text.Trim();
            if (DisplayField == "")
            {
                DisplayField = "OBJECTID";
            }
            AddLabel(DisplayField);
            this.Close();
        }

        private void btnConcel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmSetLabel_Load(object sender, EventArgs e)
        {
            //��ȡ�ֶ�
            IFields pFields = null;
            string FieldName = "";
            if (pGeoFeatLayer != null)
            {
                IDisplayRelationshipClass pDisResCls = pGeoFeatLayer as IDisplayRelationshipClass;
                if (pDisResCls.RelationshipClass != null)
                {
                    //��������������ѯ����ͼ����ֶ����Ʒ����˱仯��ͨ�������������͡�ԭ�ֶ����ơ���������γ��µ��ֶ�����
                    //���ص�CmbFields��
                    string pLayerName = pGeoFeatLayer.Name;//���ͼ����
                    pFields = pGeoFeatLayer.FeatureClass.Fields;
                    for (int i = 0; i < pFields.FieldCount; i++)
                    {
                        //����ͼ����ֶΣ�����ͼ�������͡��ֶ������������
                        FieldName = pFields.get_Field(i).Name;
                        if (FieldName.ToLower() == "shape") continue;//����Ǽ����ֶΣ�����˵�
                        if (FieldName.ToLower() == "objectid") continue;//�����ID����
                        FieldName = pLayerName + "." + FieldName;
                        CmbFields.Items.Add(FieldName);
                    }
                }
                else
                {
                    //û�н��������ѯ����ֱ�ӽ��ֶε����Ƽ��ص�CmbFields��
                    pFields = pGeoFeatLayer.FeatureClass.Fields;
                    for (int i = 0; i < pFields.FieldCount; i++)
                    {
                        FieldName = pFields.get_Field(i).Name;
                        if (FieldName.ToLower() == "shape") continue;
                        if (FieldName.ToLower() == "objectid")  
                            continue;//�����ID����
                        CmbFields.Items.Add(FieldName);
                    }
                }
                CmbFields.SelectedIndex = 0;
            }
            //��ȡ��������
            System.Drawing.Text.InstalledFontCollection FontsCol = new System.Drawing.Text.InstalledFontCollection();
            foreach (System.Drawing.FontFamily family in FontsCol.Families)
            {
                CmbFontName.Items.Add(family.Name);
            }
            //��ȡ�����С
            for (int i = 1; i < 30; i++)
            {
                CmbFontSize.Items.Add(i);
            }
        }

        private FontStyle newFont;
        private FontFamily newFamily = new FontFamily("����");
        private float newSize = 8;

        private void FontColorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            LabelText.ForeColor = FontColorPicker.SelectedColor;
        }

        private void CmbFontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            newFamily = new FontFamily(CmbFontName.SelectedItem.ToString());
            setFont();
        }

        private void CmbFontSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            newSize = (float)Convert.ToDouble(CmbFontSize.SelectedItem.ToString());
            setFont();
        }

        private void btnBold_Click(object sender, EventArgs e)
        {
            if (LabelText.Font.Bold == true)
            {
                btnBold.Checked = false;
            }
            else
            {
                btnBold.Checked = true;
            }
            newFont = newFont ^ FontStyle.Bold;
            setFont();
        }

        private void btnItalic_Click(object sender, EventArgs e)
        {
            if (LabelText.Font.Italic == true)
            {
                btnItalic.Checked = false;
            }
            else
            {
                btnItalic.Checked = true;
            }
            newFont = newFont ^ FontStyle.Italic;
            setFont();
        }

        private void btnUnderLine_Click(object sender, EventArgs e)
        {
            if (LabelText.Font.Underline == true)
            {
                btnUnderLine.Checked = false;
            }
            else
            {
                btnUnderLine.Checked = true;
            }
            newFont = newFont ^ FontStyle.Underline;
            setFont();
        }
        private void setFont()
        {
            this.LabelText.Font = new System.Drawing.Font(newFamily, newSize, newFont);
        }

    }
}