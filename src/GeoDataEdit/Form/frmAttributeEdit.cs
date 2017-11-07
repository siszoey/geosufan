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
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using GeoDataCenterFunLib;

namespace GeoDataEdit
{
    public enum enumAttributeEditMode
    {
        Top=0,
        Visiable=1,
        Selectable=2,
        CurEdit=3,
        All=4
    }

    public partial class frmAttributeEdit : DevComponents.DotNetBar.Office2007Form
    {
        private DataTable m_dataSourceGrid;        //grid��datasource
        private IMapControlDefault m_pMapControl;                        //����ѯ�ĵ�ͼ����
        private enumAttributeEditMode m_enumAttributeEditMode;     //��ѯ��ʽ
        public enumAttributeEditMode AttributeEditMode//�Ӳ�ѯ�ǵ������ݲ�ʹ��
        {
            get
            {
                return m_enumAttributeEditMode;
            }
        }
        private IFeatureCursor updateFCur = null;

        /// <summary>
        /// ��ʼ��������SQL��ѯ
        /// </summary>
        /// <param name="pMapControl"></param>
        public frmAttributeEdit(IMapControlDefault pMapControl)
        {
            InitializeComponent();
            labelItem.Visible = false;
            comboBoxItem.Visible = false;

            InitializeGrid();      //��ʼ��Grid�ı���  
            m_pMapControl = pMapControl;
        }
        /// <summary>
        /// ������һ���ѯ,�����ѯ,ѡ���ѯ��һ���ѯ
        /// </summary>
        /// <param name="pMapControl">����ѯ�ĵ�ͼ����</param>
        /// <param name="penumAttributeEditMode">��ѯ��ʽ</param>
        public frmAttributeEdit(IMapControlDefault pMapControl, enumAttributeEditMode penumAttributeEditMode)
        {
            InitializeComponent();
            labelItem.Visible = true;
            comboBoxItem.Visible = true;

            InitializeGrid();      //��ʼ��Grid�ı���  
            m_pMapControl = pMapControl;
            m_enumAttributeEditMode = penumAttributeEditMode;
            switch(penumAttributeEditMode)
            {
                case enumAttributeEditMode.Top:
                    comboBoxItem.SelectedIndex = 0;
                    break;
                case enumAttributeEditMode.Visiable:
                    comboBoxItem.SelectedIndex = 1;
                    break;
                case enumAttributeEditMode.Selectable:
                    comboBoxItem.SelectedIndex = 2;
                    break;
                case enumAttributeEditMode.CurEdit:
                    comboBoxItem.SelectedIndex = 3;
                    break;
                case enumAttributeEditMode.All:
                    comboBoxItem.SelectedIndex = 4;
                    break;
            }
        }

        //��ʼ��grid ��ʾ������
        private void InitializeGrid()
        {
            dataGridViewX.Columns.Clear();
            //dataGridViewX.ReadOnly = true;
            dataGridViewX.AutoGenerateColumns = false;
            m_dataSourceGrid = new DataTable();
            m_dataSourceGrid.Columns.Add("Name", typeof(System.String));
            m_dataSourceGrid.Columns["Name"].ReadOnly = true;
            m_dataSourceGrid.Columns.Add("Value", typeof(System.String));
           // m_dataSourceGrid.Columns["Value"].ReadOnly = true;
            AddColToGrid(dataGridViewX, "�ֶ�����", "Name", true, 132, "TextBox");
            AddColToGrid(dataGridViewX, "�ֶ�ֵ", "Value", true, 132, "TextBox");
            dataGridViewX.Columns[dataGridViewX.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewX.DataSource = m_dataSourceGrid;
        }
        private void AddColToGrid(DataGridView axiGrid, string HeaderText, string Key, bool Visible, int Width, string CellType)
        {
            DataGridViewColumn vColumn;
            switch (CellType)
            {
                case "TextBox":
                    vColumn = new DataGridViewTextBoxColumn();
                    break;
                case "Button":
                    vColumn = new DataGridViewButtonColumn();
                    break;
                case "CheckBox":
                    vColumn = new DataGridViewCheckBoxColumn();
                    break;
                case "ComboBox":
                    vColumn = new DataGridViewComboBoxColumn();
                    break;
                case "Image":
                    vColumn = new DataGridViewImageColumn();
                    break;
                case "Link":
                    vColumn = new DataGridViewLinkColumn();
                    break;
                default:
                    return;
            }

            vColumn.Visible = Visible;
            vColumn.Width = Width;
            vColumn.HeaderText = HeaderText;
            vColumn.Name = Key;
            vColumn.DataPropertyName = Key; //"C" + (axiGrid.Columns.Count + 1).ToString();
            axiGrid.Columns.Add(vColumn);
        }

        private void comboBoxItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxItem.SelectedIndex)
            {
                case 0:
                    m_enumAttributeEditMode = enumAttributeEditMode.Top;
                    break;
                case 1:
                    m_enumAttributeEditMode = enumAttributeEditMode.Visiable;
                    break;
                case 2:
                    m_enumAttributeEditMode = enumAttributeEditMode.Selectable;
                    break; 
                case 3:
                    m_enumAttributeEditMode = enumAttributeEditMode.CurEdit;
                    break;
                case 4:
                    m_enumAttributeEditMode = enumAttributeEditMode.All;
                    break;
            }
        }

        /// <summary>
        /// ���ݷ�Χ����������䴰��
        /// </summary>
        /// <param name="pMap">����ѯ�ĵ�ͼ����</param>
        /// <param name="pGeometry">��ѯ��Χ</param>
        public void FillData(IMap pMap,IGeometry pGeometry)
        {
            advTree.Nodes.Clear();
            m_dataSourceGrid.Clear();

            if (pGeometry == null)
            {
                labelItemMemo.Text = "0��Ҫ��";
                return;
            }

            int intCnt = 0;
            bool bFirst = true;

                IFeatureLayer pFeatLay = getEditLayer.isExistLayer(m_pMapControl.Map) as IFeatureLayer;
                if (pFeatLay == null)
                {
                    MessageBox.Show("�����ñ༭ͼ�㣡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
 
                }
                IWorkspaceEdit iWE = (pFeatLay.FeatureClass as IDataset).Workspace as IWorkspaceEdit;
                if (!iWE.IsBeingEdited())
                    iWE.StartEditing(false);
                ISpatialFilter pSpatialFilter=new SpatialFilterClass();
                pSpatialFilter.Geometry=pGeometry;
                pSpatialFilter.GeometryField = "SHAPE";
                pSpatialFilter.SpatialRel=esriSpatialRelEnum.esriSpatialRelIntersects;
                

                //���ͼ��ڵ�
                DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node();
                node.Text = pFeatLay.Name;
                node.Tag = pFeatLay;
                node.Expand();
                advTree.Nodes.Add(node);

                IFeatureCursor pFeatureCursor = pFeatLay.FeatureClass.Update(pSpatialFilter, false);
                updateFCur = pFeatureCursor;
                IFeature pFeat = pFeatureCursor.NextFeature();
                while (pFeat != null)
                {
                    //���Ҫ�ؽڵ�
                    DevComponents.AdvTree.Node featnode = new DevComponents.AdvTree.Node();
                    featnode.Text = pFeat.OID.ToString();
                    featnode.Tag = pFeat;
                    node.Nodes.Add(featnode);

                    if (bFirst == true)
                    {
                        advTree.SelectedNode = featnode;
                        m_dataSourceGrid.Clear();

                        for (int i = 0; i < pFeat.Fields.FieldCount; i++)
                        {
                            if (pFeat.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                            {
                                string strGeometryType = "";
                                if (pFeat.FeatureType == esriFeatureType.esriFTSimple)
                                {
                                    switch (pFeat.Shape.GeometryType)
                                    {
                                        case esriGeometryType.esriGeometryPoint:
                                            strGeometryType = "��";
                                            break;
                                        case esriGeometryType.esriGeometryPolyline:
                                            strGeometryType = "��";
                                            break;
                                        case esriGeometryType.esriGeometryPolygon:
                                            strGeometryType = "�����";
                                            break;
                                    }
                                }
                                else if (pFeat.FeatureType == esriFeatureType.esriFTAnnotation)
                                {
                                    strGeometryType = "ע��";
                                }
                                if (string.IsNullOrEmpty(strGeometryType))
                                {
                                    strGeometryType = pFeat.Shape.GeometryType.ToString();
                                }
                                m_dataSourceGrid.Rows.Add(new object[] { pFeat.Fields.get_Field(i).AliasName , strGeometryType });
                            }
                            else
                            {
                                m_dataSourceGrid.Rows.Add(new object[] { pFeat.Fields.get_Field(i).AliasName , pFeat.get_Value(i) });
                            }
                        }
                        bFirst = false;
                    }

                    pFeat = pFeatureCursor.NextFeature();

                    intCnt++;
                }

                //if (m_enumAttributeEditMode == enumAttributeEditMode.Top)
                //{
                //    progressBarItem.Value = listLays.Count;
                //    break;
                //}
            

            labelItemMemo.Text = intCnt.ToString() + "��Ҫ��";
            //progressBarItem.Visible = false;
            //this.Refresh();

            DefaultSelNde(pFeatLay as IFeatureSelection);
            setSysFdReadonly();
        }
        private void setSysFdReadonly()
        {
            try
            {
                IFeature pfeature = advTree.SelectedNode.Tag as IFeature;
                if (pfeature != null)
                {
                    for (int i = 0; i < dataGridViewX.RowCount; i++)
                    {
                        int fdpos = pfeature.Fields.FindFieldByAliasName(dataGridViewX[0, i].Value.ToString());
                        IField pField = pfeature.Fields.get_Field(fdpos);
                        if (pField.Type == esriFieldType.esriFieldTypeOID || pField.Type == esriFieldType.esriFieldTypeGeometry || pField.Name.ToUpper().Contains("AREA") || pField.Name.ToUpper().Contains("LEN")||pField.Editable==false)
                        {
                            dataGridViewX[1, i].ReadOnly = true;
                        }
                    }
                    
                    
                }
            }
            catch
            {
                
            }

 
        }//����ϵͳ�ֶ�ֻ��yjl

        //Ĭ��ѡ���һ��Ҫ�ؽڵ�  ��������˸
        private void DefaultSelNde(IFeatureSelection iFS)
        {
            Application.DoEvents();
            m_pMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            iFS.Clear();
            m_pMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            for (int i = 0; i < this.advTree.Nodes.Count; i++)
            {
                if (!this.advTree.Nodes[i].HasChildNodes) continue;

                IFeature pFea = this.advTree.Nodes[i].Nodes[0].Tag as IFeature;
                if (pFea != null)
                {
                    m_pMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                    this.advTree.Nodes[i].IsSelectionVisible = true;
                    iFS.Add(pFea);
                    m_pMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                    //ModDBOperator.FlashFeature(pFea.Shape, m_pMapControl.ActiveView,100);
                    break;
                }
            }
        }


   
        private void advTree_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            try
            {
                IFeature pfeature = advTree.SelectedNode.Tag as IFeature;
                if (pfeature != null)
                {
                    m_dataSourceGrid.Clear();

                    for (int i = 0; i < pfeature.Fields.FieldCount; i++)
                    {
                        if (pfeature.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                        {
                            string strGeometryType = "";
                            if (pfeature.FeatureType == esriFeatureType.esriFTSimple)
                            {
                                switch (pfeature.Shape.GeometryType)
                                {
                                    case esriGeometryType.esriGeometryPoint:
                                        strGeometryType = "��";
                                        break;
                                    case esriGeometryType.esriGeometryPolyline:
                                        strGeometryType = "��";
                                        break;
                                    case esriGeometryType.esriGeometryPolygon:
                                        strGeometryType = "�����";
                                        break;
                                }
                            }
                            else if (pfeature.FeatureType == esriFeatureType.esriFTAnnotation)
                            {
                                strGeometryType = "ע��";
                            }
                            if (string.IsNullOrEmpty(strGeometryType))
                            {
                                strGeometryType = pfeature.Shape.GeometryType.ToString();
                            }
                            m_dataSourceGrid.Rows.Add(new object[] { pfeature.Fields.get_Field(i).AliasName , strGeometryType });
                        }
                        else
                        {
                            m_dataSourceGrid.Rows.Add(new object[] { pfeature.Fields.get_Field(i).AliasName , pfeature.get_Value(i) });
                        }
                    }

                    ModDBOperator.FlashFeature(pfeature, m_pMapControl);
                }
            }
            catch
            {

            }
        }

        private void advTree_NodeDoubleClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            try
            {
                IFeature pfeature = advTree.SelectedNode.Tag as IFeature;
                if (pfeature != null)
                {
                    SysCommon.Gis.ModGisPub.ZoomToFeature(m_pMapControl, pfeature);
                    Application.DoEvents();
                    ModDBOperator.FlashFeature(pfeature, m_pMapControl);
                    //m_pMapControl.FlashShape(pfeature.Shape, 2, 500, null);
                }
            }
            catch
            {
            }
        }

        private void dataGridViewX_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                IFeature pfeature = advTree.SelectedNode.Tag as IFeature;
                if (pfeature != null)
                {
                    IWorkspaceEdit iWE = (pfeature.Class as IDataset).Workspace as IWorkspaceEdit;
                    if (!iWE.IsBeingEdited())
                        iWE.StartEditing(false);
                    iWE.StartEditOperation();
                    int fdpos = pfeature.Fields.FindFieldByAliasName(dataGridViewX[e.ColumnIndex-1,e.RowIndex].Value.ToString());
                    pfeature.set_Value(fdpos, dataGridViewX[e.ColumnIndex, e.RowIndex].Value.ToString());
                    pfeature.Store();
                    iWE.StopEditOperation();
                    labelEditStatus.Text = "����ɸ���!";
                    labelEditStatus.Visible = true;
                    this.Refresh();
                    iWE = null;
                }
            }
            catch
            {
                MessageBox.Show("�밴��ȷ���ֶθ�ʽ��д���ԣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

      

        private void dataGridViewX_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            labelEditStatus.Visible = false;
            this.Refresh();
        }

        private void dataGridViewX_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}