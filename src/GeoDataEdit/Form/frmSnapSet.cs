using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace GeoDataEdit
{
    public partial class frmSnapSet : DevComponents.DotNetBar.Office2007Form
    {

        private bool valitateSnapRadius;
        private bool valitateCacheRadius;

        //��׽���÷��ؽ��
        private bool m_OutRes;
        public bool OutRes
        {
            get
            {
                return m_OutRes;
            }
        }
        
        public frmSnapSet(IMap pMap)
        {
            InitializeComponent();

            //��ʼ��dataGridViewX
            InitialdataGridViewX();

            //����ͼ�㵽listViewExLayers
            if (pMap.LayerCount == 0) return;
            AddLayersToList(pMap, dataGridViewX);
            if (MoData.v_SearchDist != 0)
            {
                txtSnapRadius.Text = MoData.v_SearchDist.ToString();
            }

            if (MoData.v_CacheRadius != 0)
            {
                txtCacheRadius.Text = MoData.v_CacheRadius.ToString();
            }
        }

        //��ʼ��dataGridViewX
        private void InitialdataGridViewX()
        {
            dataGridViewX.DataSource = null;

            DataGridViewTextBoxColumn textBoxColumn = new DataGridViewTextBoxColumn();
            textBoxColumn.Name = "colLayName";
            textBoxColumn.HeaderText = "ͼ��";
            textBoxColumn.DataPropertyName = "colLayName";
            textBoxColumn.Width = dataGridViewX.Width*3/8-18;
            textBoxColumn.ReadOnly = true;
            dataGridViewX.Columns.Add(textBoxColumn);

            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.Name = "colVertexPoint";
            checkBoxColumn.HeaderText = "�ڵ�";
            checkBoxColumn.DataPropertyName = "colVertexPoint";
            checkBoxColumn.Width = dataGridViewX.Width * 1 / 8;
            dataGridViewX.Columns.Add(checkBoxColumn);

            checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.Name = "colPortPoint";
            checkBoxColumn.HeaderText = "�˵�";
            checkBoxColumn.DataPropertyName = "colPortPoint";
            checkBoxColumn.Width = dataGridViewX.Width * 1 / 8;
            dataGridViewX.Columns.Add(checkBoxColumn);

            checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.Name = "colIntersectPoint";
            checkBoxColumn.HeaderText = "�ཻ��";
            checkBoxColumn.DataPropertyName = "colIntersectPoint";
            checkBoxColumn.Width = dataGridViewX.Width * 1 / 8;
            dataGridViewX.Columns.Add(checkBoxColumn);

            checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.Name = "colMidPoint";
            checkBoxColumn.HeaderText = "�е�";
            checkBoxColumn.DataPropertyName = "colMidPoint";
            checkBoxColumn.Width = dataGridViewX.Width * 1 / 8;
            dataGridViewX.Columns.Add(checkBoxColumn);

            checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.Name = "colNearestPoint";
            checkBoxColumn.HeaderText = "�����";
            checkBoxColumn.DataPropertyName = "colNearestPoint";
            checkBoxColumn.Width = dataGridViewX.Width * 1 / 8;
            dataGridViewX.Columns.Add(checkBoxColumn);

            //��ʼ��DataGridView(dgvType)��������
            dataGridViewX.RowTemplate.Height = 18;
            dataGridViewX.RowTemplate.Resizable = DataGridViewTriState.False;
            dataGridViewX.RowHeadersVisible = false;
            dataGridViewX.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        //����ͼ�㵽listViewExLayers
        private void AddLayersToList(IMap pMap, DevComponents.DotNetBar.Controls.DataGridViewX pDataGridViewX)
        {
            UID pUID = new UIDClass();
            pUID.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}";   //UID for IGeoFeatureLayer
            IEnumLayer pEnumLayer = pMap.get_Layers(pUID, true);
            pEnumLayer.Reset();
            ILayer pLayer = pEnumLayer.Next();
            while(pLayer!=null)
            {
                AddLayerToList(pLayer, pDataGridViewX);
                pLayer = pEnumLayer.Next();
            }
        }
        private void AddLayerToList(ILayer pLayer, DevComponents.DotNetBar.Controls.DataGridViewX pDataGridViewX)
        {
            int index = pDataGridViewX.Rows.Add();
            DataGridViewRow aRow = pDataGridViewX.Rows[index];
            aRow.Tag = pLayer;
            aRow.Cells[0].Value = pLayer.Name; //ͼ����
            aRow.Cells[1].Value = false;        //�ڵ㲶׽
            aRow.Cells[2].Value = false;        //�˵㲶׽
            aRow.Cells[3].Value = false;        //�ཻ�㲶׽
            aRow.Cells[4].Value = false;        //�е㲶׽
            aRow.Cells[5].Value = false;      //����㲶׽

            if (MoData.v_dicSnapLayers != null)
            {
                if (MoData.v_dicSnapLayers.ContainsKey(pLayer))
                {
                    object[] values = MoData.v_dicSnapLayers[pLayer].ToArray();
                    aRow.Cells[1].Value = Convert.ToBoolean(values[0]);        //�ڵ㲶׽
                    aRow.Cells[2].Value = Convert.ToBoolean(values[1]);        //�˵㲶׽
                    aRow.Cells[3].Value = Convert.ToBoolean(values[2]);        //�ཻ�㲶׽
                    aRow.Cells[4].Value = Convert.ToBoolean(values[3]);        //�е㲶׽
                    aRow.Cells[5].Value = Convert.ToBoolean(values[4]);        //����㲶׽
                }
            }
        }

        private void buttonXOk_Click(object sender, EventArgs e)
        {
            if (txtSnapRadius.Text == "" || txtCacheRadius.Text == "")
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��׽�뾶�򻺴�뾶����Ϊ�ջ�����!");
                return;
            }

            MoData.v_SearchDist = Convert.ToDouble(txtSnapRadius.Text);
            MoData.v_CacheRadius = Convert.ToDouble(txtCacheRadius.Text);

            if (MoData.v_dicSnapLayers == null)
            {
                MoData.v_dicSnapLayers = new Dictionary<ILayer, ArrayList>();
            }

            foreach (DataGridViewRow aRow in dataGridViewX.Rows)
            {
                bool bVertexPoint = Convert.ToBoolean(aRow.Cells[1].Value);       //�ڵ㲶׽
                bool bPortPoint = Convert.ToBoolean(aRow.Cells[2].Value);         //�˵㲶׽
                bool bIntersectPoint = Convert.ToBoolean(aRow.Cells[3].Value);    //�ཻ�㲶׽
                bool bMidPoint = Convert.ToBoolean(aRow.Cells[4].Value);          //�е㲶׽
                bool bNearestPoint = Convert.ToBoolean(aRow.Cells[5].Value);      //����㲶׽
                ILayer pLay = aRow.Tag as ILayer;
                if (pLay == null) continue;

                ArrayList aArrayList = new ArrayList();
                aArrayList.AddRange(new bool[] { bVertexPoint, bPortPoint, bIntersectPoint, bMidPoint, bNearestPoint });

                if (bVertexPoint || bPortPoint || bIntersectPoint || bMidPoint || bNearestPoint)
                {
                    if (!MoData.v_dicSnapLayers.ContainsKey(pLay))
                    {
                        MoData.v_dicSnapLayers.Add(pLay, aArrayList);
                    }
                }

                if (MoData.v_dicSnapLayers.ContainsKey(pLay))
                {
                    MoData.v_dicSnapLayers[pLay] = aArrayList;
                }
            }
            
            m_OutRes = true;
            this.Close();
        }

        private void buttonXCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSnapRadius_KeyDown(object sender, KeyEventArgs e)
        {
            valitateSnapRadius = false;

            //С�����".",���С�����Ƿ��Ѿ�����
            if (e.KeyValue == 46 && txtSnapRadius.Text.Contains("."))
            {
                valitateSnapRadius = true;
                return;
            }

            //���ּ�(48~57),�˸��8
            if ((e.KeyValue > 57 || e.KeyValue < 48) && e.KeyValue != 8)
            {
                valitateSnapRadius = true;
                return;
            }
        }

        private void txtSnapRadius_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (valitateSnapRadius == true)
            {
                e.Handled = true;
            }
        }

        private void txtCacheRadius_KeyDown(object sender, KeyEventArgs e)
        {
            valitateSnapRadius = false;

            //С�����".",���С�����Ƿ��Ѿ�����
            if (e.KeyValue == 46 && txtSnapRadius.Text.Contains("."))
            {
                valitateCacheRadius = true;
                return;
            }

            //���ּ�(48~57),�˸��8
            if ((e.KeyValue > 57 || e.KeyValue < 48) && e.KeyValue != 8)
            {
                valitateCacheRadius = true;
                return;
            }
        }

        private void txtCacheRadius_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (valitateCacheRadius == true)
            {
                e.Handled = true;
            }
        }

        private void txtSnapRadius_TextChanged(object sender, EventArgs e)
        {
            if (txtSnapRadius.Text == "") return;
            if (Convert.ToDouble(txtSnapRadius.Text) > 1300)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���õ�ֵ����!��ֵ��ò�Ҫ������Ļ�Ŀ��(���ص�λ)!����������!");
            }
        }

        private void txtCacheRadius_TextChanged(object sender, EventArgs e)
        {
            if (txtCacheRadius.Text == "") return;
            if (Convert.ToDouble(txtCacheRadius.Text) > 1300)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���õ�ֵ����!��ֵ��ò�Ҫ������Ļ�Ŀ��(���ص�λ)!����������!");
            }
        }
    }
}