using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace GeoProperties.UserControls
{
    public partial class uctrDefinitionQuery : UserControl
    {
        private ILayer m_pLayer;
        private IFeatureLayer m_pCurrentLayer;
        private frmQuery frmQBDialog;
        private IFeatureLayerDefinition m_pFeatureLayerDefinition;
        string LayerID = "";
        public uctrDefinitionQuery(ILayer pLayer,string Layerid)
        {
            try
            {
                LayerID = Layerid;
                m_pLayer = pLayer;
                m_pCurrentLayer = (IFeatureLayer)m_pLayer;
                m_pFeatureLayerDefinition = (IFeatureLayerDefinition)m_pCurrentLayer;

                InitializeComponent();
            }
            catch { }
        }

        private void btnQueryBuilder_Click(object sender, EventArgs e)
        {
            frmQBDialog = new frmQuery(m_pCurrentLayer, txtDefinitionQuery.Text);
            frmQBDialog.ShowDialog();
            if (frmQBDialog.DialogResult == DialogResult.OK)
            {
                txtDefinitionQuery.Text = frmQBDialog.m_sWhereClause;
            }
        }

        public void GetQueryResult()
        {
            //��ǰͼ��Ϊ�գ��˳�
            if (m_pCurrentLayer == null) return;
            m_pFeatureLayerDefinition.DefinitionExpression = txtDefinitionQuery.Text.Trim();
        }

        private void uctrDefinitionQuery_Load(object sender, EventArgs e)
        {
            string layerDefinExp = m_pFeatureLayerDefinition.DefinitionExpression;
            txtDefinitionQuery.Text = layerDefinExp;
        }

        private void btnOpenShow_Click(object sender, EventArgs e)
        {
            if (m_pLayer == null || LayerID == "")
            {
                MessageBox.Show("����ѡ��鿴��ͼ�㣡", "��ʾ");
                return;
            }
            FrmOpenSQLCondition newfrm = new FrmOpenSQLCondition(m_pCurrentLayer.FeatureClass.FeatureDataset.Workspace);
            newfrm.m_TableName = "DEFINITIONMAPSHOW";
            newfrm.m_LayerId = LayerID;
            newfrm.m_FrmText = "�򿪷���";
            newfrm.m_DvCaption = "��������";
            newfrm.TopMost = true;
            if (DialogResult.OK != newfrm.ShowDialog()) return;
            txtDefinitionQuery.Text = newfrm.m_Condition;
        }

        private void btnSaveShow_Click(object sender, EventArgs e)
        {
            if (m_pLayer == null || LayerID == "")
            {
                MessageBox.Show("����ѡ��鿴��ͼ�㣡", "��ʾ");
                return;
            }
            if (txtDefinitionQuery.Text == "")
            {
                MessageBox.Show("δѡ��鿴������", "��ʾ");
                return;
            }
            FrmSaveSQLSolution newFrm = new FrmSaveSQLSolution(m_pCurrentLayer.FeatureClass.FeatureDataset.Workspace);
            newFrm.m_TableName = "DEFINITIONMAPSHOW";
            newFrm.m_LayerID = LayerID;
            newFrm.m_FrmText = "���淽��";
            newFrm.m_Condition = txtDefinitionQuery.Text.Trim();
            newFrm.TopMost = true;
            newFrm.ShowDialog();
        }

    }
}
