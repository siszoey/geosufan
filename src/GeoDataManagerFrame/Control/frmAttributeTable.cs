using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

namespace GeoDataManagerFrame
{
    /// <summary>
    /// ���ߣ�ϯʤ
    /// ���ڣ�2011.03.02
    /// ˵����ͼ�����Ա�ʵ�ִ���
    /// </summary>
    public partial class frmAttributeTable : DevComponents.DotNetBar.Office2007Form
    {
        public frmAttributeTable(ESRI.ArcGIS.Controls.AxMapControl axMapControl)
        {
            InitializeComponent();
            m_axMapControl=axMapControl;
        }
        public IMap m_map;
        private ESRI.ArcGIS.Controls.AxMapControl m_axMapControl;
        public DataTable attributeTable;
        static IFeatureLayer m_pFeatLyr;
        static ILayer m_player;
        int n;//��ʼ���ص�����
        DataTable pDataTable;
        ITable pTable;
        string shapeType;//ͼ������
        
        /// <summary>
        /// ����ͼ���ֶδ���һ��ֻ���ֶεĿ�DataTable
        /// </summary>
        /// <param name="pLayer"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static DataTable CreateDataTableByLayer(ILayer pLayer, string tableName)
        {
            //����һ��DataTable��
            DataTable pDataTable = new DataTable(tableName);
            //ȡ��ITable�ӿ�
            ITable pTable = pLayer as ITable;
            IField pField = null;
            DataColumn pDataColumn;
            //����ÿ���ֶε����Խ���DataColumn����
            for (int i = 0; i < pTable.Fields.FieldCount; i++)
            {
                pField = pTable.Fields.get_Field(i);
                //�½�һ��DataColumn������������
                pDataColumn = new DataColumn(pField.Name);
                if (pField.Name == pTable.OIDFieldName)
                {
                    pDataColumn.Unique = true;//�ֶ�ֵ�Ƿ�Ψһ
                }
                //�ֶ�ֵ�Ƿ�����Ϊ��
                pDataColumn.AllowDBNull = pField.IsNullable;
                //�ֶα���
                pDataColumn.Caption = pField.AliasName;
                //�ֶ���������
                pDataColumn.DataType = System.Type.GetType(ParseFieldType(pField.Type));
                //�ֶ�Ĭ��ֵ
                pDataColumn.DefaultValue = pField.DefaultValue;
                //���ֶ�ΪString�����������ֶγ���
                if (pField.VarType == 8)
                {
                    pDataColumn.MaxLength = pField.Length;
                }
                //�ֶ���ӵ�����
                pDataTable.Columns.Add(pDataColumn);
                pField = null;
                pDataColumn = null;
            }
            return pDataTable;
        }


        /// <summary>
        /// ��GeoDatabase�ֶ�����ת����.Net��Ӧ����������
        /// </summary>
        /// <param name="fieldType">�ֶ�����</param>
        /// <returns></returns>
        public static string ParseFieldType(esriFieldType fieldType)
        {
            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeBlob:
                    return "System.String";
                case esriFieldType.esriFieldTypeDate:
                    return "System.DateTime";
                case esriFieldType.esriFieldTypeDouble:
                    return "System.Double";
                case esriFieldType.esriFieldTypeGeometry:
                    return "System.String";
                case esriFieldType.esriFieldTypeGlobalID:
                    return "System.String";
                case esriFieldType.esriFieldTypeGUID:
                    return "System.String";
                case esriFieldType.esriFieldTypeInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeOID:
                    return "System.String";
                case esriFieldType.esriFieldTypeRaster:
                    return "System.String";
                case esriFieldType.esriFieldTypeSingle:
                    return "System.Single";
                case esriFieldType.esriFieldTypeSmallInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeString:
                    return "System.String";
                default:
                    return "System.String";
            }
        }
        /// <summary> 
        /// ���DataTable�е�����
        /// </summary>
        /// <param name="pLayer"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public  DataTable CreateDataTable(ILayer pLayer, string tableName)
        {
            //������DataTable
            pDataTable = CreateDataTableByLayer(pLayer, tableName);
            //ȡ��ͼ������
           shapeType = getShapeType(pLayer);
            //����DataTable���ж���
            DataRow pDataRow = null;
            //��ILayer��ѯ��ITable
            pTable = pLayer as ITable;
            //ȡ��ITable�е�����Ϣ
            ICursor pCursor = pTable.Search(null, false);
            IRow pRow = pCursor.NextRow();
            n = 0;
            while (pRow != null)
            {
                //�½�DataTable���ж���
                pDataRow = pDataTable.NewRow();
                for (int i = 0; i < pRow.Fields.FieldCount; i++)
                {
                    //����ֶ�����ΪesriFieldTypeGeometry�������ͼ�����������ֶ�ֵ
                    if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        pDataRow[i] = shapeType;
                    }
                    //��ͼ������ΪAnotationʱ��Ҫ�����л���esriFieldTypeBlob���͵����ݣ�
                    //��洢���Ǳ�ע���ݣ��������轫��Ӧ���ֶ�ֵ����ΪElement
                    else if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeBlob)
                    {
                        pDataRow[i] = "Element";
                    }
                    else
                    {
                        pDataRow[i] = pRow.get_Value(i);
                    }
                }
                //���DataRow��DataTable
                pDataTable.Rows.Add(pDataRow);
                pDataRow = null;
                n++;
                //Ϊ��֤Ч�ʣ�һ��ֻװ���������¼
                if (n ==100)
                {
                    pRow = null;
                }
                else
                {
                    pRow = pCursor.NextRow();
                }
            }
            return pDataTable;
        }

        /// <summary>
        /// ���ͼ���Shape����
        /// </summary>
        /// <param name="pLayer">ͼ��</param>
        /// <returns></returns>
        public static string getShapeType(ILayer pLayer)
        {
            IFeatureLayer pFeatLyr = (IFeatureLayer)pLayer;
            m_pFeatLyr = pFeatLyr;//ȫ�ֱ���
            m_player = pLayer;
            switch (pFeatLyr.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    return "Point";
                case esriGeometryType.esriGeometryPolyline:
                    return "Polyline";
                case esriGeometryType.esriGeometryPolygon:
                    return "Polygon";
                default:
                    return "";
            }
        }

        /// <summary> 
        /// ��DataTable��DataGridView
        /// </summary>
        /// <param name="player"></param>
        public void CreateAttributeTable(ILayer player)
        {
            string tableName;
            if (player == null) return;
            tableName = getValidFeatureClassName(player.Name);
            this.Text = "���Ա�[" + tableName + "] ";
            attributeTable = CreateDataTable(player, tableName);
            this.dataGridView.DataSource = attributeTable;
            
        }

        /// <summary>
        /// �滻���ݱ����еĵ�
        /// </summary>
        /// <param name="FCname"></param>
        /// <returns></returns>
        public static string getValidFeatureClassName(string FCname)
        {
            int dot = FCname.IndexOf(".");
            if (dot != -1)
            {
                return FCname.Replace(".", "_");
            }
            return FCname;
        }

        private void frmAttributeTable_Load(object sender, EventArgs e)
        {
            textBoxIndex.Text ="1";
            dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];
            panel1.Height = 30;
            dataGridView.Height = this.Height - 65;
            timer.Enabled = true;


        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textBoxIndex.Text = e.RowIndex + 1 + "";
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            textBoxIndex.Text = "1";
            dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];
        }

        private void btnprevious_Click(object sender, EventArgs e)
        {   
            int index=dataGridView.CurrentRow.Index;
            if (index > 0)
            {
                textBoxIndex.Text = index.ToString();
                dataGridView.CurrentCell = dataGridView.Rows[index - 1].Cells[0];
            }
        }

        private void btnnext_Click(object sender, EventArgs e)
        {
            int index = dataGridView.CurrentRow.Index;
            if (index < dataGridView.Rows.Count - 2)
            {
                textBoxIndex.Text = index + 2 + "";
                dataGridView.CurrentCell = dataGridView.Rows[index + 1].Cells[0];
            }
        }

        private void btnlast_Click(object sender, EventArgs e)
        {
            textBoxIndex.Text = dataGridView.Rows.Count - 1 + "";
            dataGridView.CurrentCell = dataGridView.Rows[dataGridView.Rows.Count-2].Cells[0];
        }

        //�����ؼ�����
        private void frmAttributeTable_ResizeEnd(object sender, EventArgs e)
        {
            panel1.Height = 30;
            dataGridView.Height = this.Height - 65;
        }

        private void frmAttributeTable_SizeChanged(object sender, EventArgs e)
        {
            panel1.Height = 30;
            dataGridView.Height = this.Height - 65;
        }

        //ʵ��Ҫ����ͼ��������˫����ʵ��
        private void dataGridView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        { 
           IFeature feature = m_pFeatLyr.FeatureClass.GetFeature(e.RowIndex+1);//�õ���ǰѡ��Ҫ��
           IMap map = m_axMapControl.Map;//�õ���ǰ��ͼ
           //map.ClearLayers();
           map.ClearSelection();//����ϴθ�����ʾ��Ҫ��
           map.SelectFeature(m_player, feature);//ѡ��ǰѡ�е�Ҫ�ز�������ʾ
           m_axMapControl.Extent = feature.Shape.Envelope;//�Ŵ�Ҫ�ز�����
           //IFeatureSelection pFeatureSel =new FeatureLayerClass();
           //pFeatureSel = (IFeatureSelection)m_pFeatLyr;
            //pFeatureSel.SelectFeatures
           m_axMapControl.ActiveView.Refresh();//ˢ�µ�ͼ
        }

        /// <summary>
        /// �򿪴�����������GridView
        /// </summary>
        private void LoadDataTable()
        {
            try
            {
                IQueryFilter pqueryFilter = new QueryFilterClass();
                pqueryFilter.SubFields = "*";
                pqueryFilter.WhereClause = "OBJECTID>" + n;
                ICursor pCursor = pTable.Search(pqueryFilter, false);
                //����DataTable���ж���
                DataRow pDataRow = null;
                //ȡ��ITable�е�����Ϣ
                IRow pRow = pCursor.NextRow();
                if (pRow == null)
                {
                    this.timer.Enabled = false;
                    this.Text += "��¼����" + attributeTable.Rows.Count;
                }
                int m = 0;
                while (pRow != null)
                {
                    //�½�DataTable���ж���
                    pDataRow = pDataTable.NewRow();
                    for (int i = 0; i < pRow.Fields.FieldCount; i++)
                    {
                        //����ֶ�����ΪesriFieldTypeGeometry�������ͼ�����������ֶ�ֵ
                        if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                        {
                            pDataRow[i] = shapeType;
                        }
                        //��ͼ������ΪAnotationʱ��Ҫ�����л���esriFieldTypeBlob���͵����ݣ�
                        //��洢���Ǳ�ע���ݣ��������轫��Ӧ���ֶ�ֵ����ΪElement
                        else if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeBlob)
                        {
                            pDataRow[i] = "Element";
                        }
                        else
                        {
                            pDataRow[i] = pRow.get_Value(i);
                        }
                    }
                    //���DataRow��DataTable
                    pDataTable.Rows.Add(pDataRow);
                    pDataRow = null;
                    m++;
                    n++;
                    //Ϊ��֤Ч�ʣ�һ��ֻװ���������¼
                    if (m == 100)
                    {
                        pRow = null;
                    }
                    else
                    {
                        pRow = pCursor.NextRow();
                    }
                }
            }
            catch
            { }



        }
        private void timer_Tick(object sender, EventArgs e)
        {
            //this.Cursor = Cursors.WaitCursor;
            LoadDataTable();
            //this.Cursor = Cursors.Default;
           
        } 


    }
}