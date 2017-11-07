using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace GeoProperties.UserControls
{
    public partial class uctrFields : UserControl
    {
        ILayer m_pLayer; //��ȡ��Layer
        IFeatureLayer m_pCurrentLayer; //��ǰͼ��
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="pLayer"></param>
        public uctrFields(ILayer pLayer)
        {

            try
            {//ʵ����
                m_pLayer = pLayer;
                m_pCurrentLayer = (IFeatureLayer)m_pLayer;

                InitializeComponent();
                InitLayerFields();
            }
            catch
            {
             
            }
        }
        /// <summary>
        /// ��ʼ���ֶ�����
        /// </summary>
        private void InitLayerFields()
        {

            IFields pFields;
            ListViewItem fieldItem;
            pFields = m_pCurrentLayer.FeatureClass.Fields; //��ȡ��ǰͼ��������ֶ�
            ILayerFields pLayerFields =(ILayerFields) m_pCurrentLayer;
            //��������ȡÿ���ֶ�
            for (int i = 0; i < pFields.FieldCount; i++)
            {
                //�����ֶ���������Item
                fieldItem = new ListViewItem();
                IField pField = pFields.get_Field(i);
                IFieldInfo pFieldInfo = pLayerFields.get_FieldInfo(i);
                fieldItem.Name = pField.Name;
                fieldItem.Text = pField.Name;
                fieldItem.Tag = pField;
                fieldItem.Checked = pFieldInfo.Visible;
                //����Item��SubItems
                string[] subItems = new string[5];
                subItems[0] = pField.AliasName;
                subItems[1] = GetFieldType(pField.Type);
                subItems[2] = pField.Length.ToString();
                subItems[3] = pField.Precision.ToString();
                subItems[4] = pField.Scale.ToString();
                fieldItem.SubItems.AddRange(subItems);
                //���Item��ListView
                lstFieldsView.Items.Add(fieldItem);
                if (fieldItem.Checked)
                {
                    cboFields.Items.Add(fieldItem.Name);
                }
            }
            if (cboFields.Items.Count > 0)
            {
                cboFields.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// ȫѡ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            //�������е�Item��������Checked����Ϊtrue
            for (int i = 0; i < lstFieldsView.Items.Count; i++)
            {
                lstFieldsView.Items[i].Checked = true;
            }
        }
        /// <summary>
        /// ȫ��ѡ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearAll_Click(object sender, EventArgs e)
        {
            //�������е�Item��������Checked����Ϊfalse
            for (int i = 0; i < lstFieldsView.Items.Count; i++)
            {
                lstFieldsView.Items[i].Checked = false;
            }
        }
        /// <summary>
        /// ��ȡField�����ͣ������涨������ʾ
        /// </summary>
        /// <param name="fieldTpye"></param>
        /// <returns></returns>
        private string GetFieldType(esriFieldType fieldTpye)
        {
            string type = null;
            switch (fieldTpye)
            {
                case esriFieldType.esriFieldTypeBlob:
                    type = "Blob";
                    break;
                case esriFieldType.esriFieldTypeDate:
                    type = "Data";
                    break;
                case esriFieldType.esriFieldTypeDouble:
                    type = "Double";
                    break;
                case esriFieldType.esriFieldTypeGeometry:
                    type = GetGeometryType();
                    break;
                case esriFieldType.esriFieldTypeGlobalID:
                    type = "GlobalID";
                    break;
                case esriFieldType.esriFieldTypeGUID:
                    type = "GUID";
                    break;
                case esriFieldType.esriFieldTypeInteger:
                    type = "Integer";
                    break;
                case esriFieldType.esriFieldTypeOID:
                    type = "ObjectID";
                    break;
                case esriFieldType.esriFieldTypeRaster:
                    type = "Raster";
                    break;
                case esriFieldType.esriFieldTypeSingle:
                    type = "Single";
                    break;
                case esriFieldType.esriFieldTypeSmallInteger:
                    type = "SmallInteger";
                    break;
                case esriFieldType.esriFieldTypeString:
                    type = "Text";
                    break;
                case esriFieldType.esriFieldTypeXML:
                    type = "XML";
                    break;
            }
            return type;
        }
        /// <summary>
        /// ����FeatureClass��ShapeType��������ʾ����
        /// </summary>
        /// <returns></returns>
        private string GetGeometryType()
        {
            string type = null;
            switch (m_pCurrentLayer.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    type = "Point";
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    type = "Polyline";
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    type = "Polygon";
                    break;
                case esriGeometryType.esriGeometryLine:
                    type = "Line";
                    break;
                case esriGeometryType.esriGeometryMultipoint:
                    type = "Multipoint";
                    break;
                default:
                    type = "Unknow";
                    break;
            }
            return type;
        }
        /// <summary>
        /// �����ֶθ���
        /// </summary>
        public void SaveFieldChange()
        {
            ILayerFields pLayerFields = (ILayerFields)m_pCurrentLayer;
            if (cboFields == null) return;
            cboFields.Items.Clear();
            cboFields.Text = null;
            //�������е�Item����������Checked���������ֶε���ʾ���
            for (int i = 0; i < lstFieldsView.Items.Count; i++)
            {
                IField pField;
                pField = lstFieldsView.Items[i].Tag as IField;
                IFieldInfo pFieldInfo = pLayerFields.get_FieldInfo(i);
                ListViewItem fieldItem=lstFieldsView.Items[i];
                pFieldInfo.Visible = fieldItem.Checked;
                if (fieldItem.Checked)
                {
                    cboFields.Items.Add(fieldItem.Name);
                }
            }
            if (cboFields.Items.Count > 0)
            {
                cboFields.SelectedIndex = 0;
            }
            
        }
    }
}
