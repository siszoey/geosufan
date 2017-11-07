using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Controls;
using SysCommon.Gis;

namespace GeoDBATool
{
    public partial class FrmBatchUpdate : DevComponents.DotNetBar.Office2007Form
    {
        Plugin.Application.IAppGISRef v_AppGIS;
        EnumOperateType v_OpeType;
        private System.Windows.Forms.Timer _timer;
        private IGeometry m_Geometry = null;

        IFeatureLayer m_FeaLayer = null;
        public FrmBatchUpdate(Plugin.Application.IAppGISRef pAppGIS)
        {
            InitializeComponent();
            v_AppGIS = pAppGIS;

        }
        private void FrmBatchUpdate_Load(object sender, EventArgs e)
        {
            comboBoxUptDataType.Items.Add("ESRI�������ݿ�(*.mdb)");
            comboBoxUptDataType.Items.Add("ESRI�ļ����ݿ�(*.gdb)");

            comboBoxUptRangeType.Items.Add("ShapeFile(*.shp)");

            comboBoxUptRangeType.SelectedIndex = 0;
            comboBoxUptDataType.SelectedIndex = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Exception eError = null;
            
            //�ж�Դ�����Ƿ�����
            if (this.textBoxUptRange.Text == "")
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ø��·�Χ��������");
                return;
            }
            if (this.textBoxUptData.Text == "")
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ø�����������");
                return;
            }
            
            ClsBatchUpdate pClsBatchUpdate = new ClsBatchUpdate();
            //��������ͼ�л�ȡ���ƿ����ʷ��ڵ�
            DevComponents.AdvTree.Node pCurNode = pClsBatchUpdate.GetNodeOfProjectTree(v_AppGIS.ProjectTree, "DB", "���ƿ�");
            DevComponents.AdvTree.Node pHisNode = pClsBatchUpdate.GetNodeOfProjectTree(v_AppGIS.ProjectTree, "DB", "��ʷ��");
            if (pCurNode == null || pHisNode==null)
            {
                return;
            }

            //��ȡ���ƿ�����
            XmlElement elementTemp = (pCurNode.Tag as XmlElement).SelectSingleNode(".//������Ϣ") as XmlElement;
            IWorkspace pCurWorkSpace = ModDBOperator.GetDBInfoByXMLNode(elementTemp, "") as IWorkspace;
            if (pCurWorkSpace == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�������ƿ�ʧ��!");
                return;
            }

            //��ȡ��ʷ������
            elementTemp = (pHisNode.Tag as XmlElement).SelectSingleNode(".//������Ϣ") as XmlElement;            
            IWorkspace pHisWorkSpace = ModDBOperator.GetDBInfoByXMLNode(elementTemp, "") as IWorkspace;
            if (pHisWorkSpace == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������ʷ��ʧ��!");
                return;
            }

            //��ȡ�������ݿ�����
            SysCommon.Gis.SysGisDataSet pUptSysGisDT = new SysCommon.Gis.SysGisDataSet();//���¿�����
            SysCommon.enumWSType pUptType = SysCommon.enumWSType.PDB;
            if (this.textBoxUptData.Tag == "PDB")
            {
                pUptType = SysCommon.enumWSType.PDB;
            }
            else if(this.textBoxUptData.Tag =="GDB")
            {
                pUptType = SysCommon.enumWSType.GDB;
            }
            Exception ERR0 = null;
            pUptSysGisDT.SetWorkspace(this.textBoxUptData.Text, pUptType, out ERR0);
            IWorkspace pUptWorkSpace = pUptSysGisDT.WorkSpace;
            //��ȡ���·�Χ
            IGeometry pUptGeometry = null;
            pUptGeometry = SysCommon.ModPublicFun.GetPolyGonFromFile(this.textBoxUptRange.Text);
            this.Hide();
            FrmProcessBar frmbar = new FrmProcessBar();
            frmbar.Show();
            if (pUptGeometry != null)
            {
                pClsBatchUpdate.DoBatchUpdate(pCurWorkSpace, pHisWorkSpace, pUptWorkSpace, pUptGeometry, pCurNode, pHisNode,frmbar);
            }
            frmbar.Close();
            this.Close();
        }

        

        private void buttonUptRange_Click(object sender, EventArgs e)
        {
            if (comboBoxUptRangeType.Tag == "PDB")
            {
                OpenFileDialog OpenFile = new OpenFileDialog();
                OpenFile.Title = "ѡ����·�Χ����";
                OpenFile.Filter = "MDB����(*.mdb)|*.mdb";
                if (OpenFile.ShowDialog() == DialogResult.OK)
                {
                    this.textBoxUptRange.Text = OpenFile.FileName;
                }
            }
            else if(comboBoxUptRangeType.Tag=="GDB")
            {
                FolderBrowserDialog pFolderBrowser = new FolderBrowserDialog();
                
                if (pFolderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (!pFolderBrowser.SelectedPath.EndsWith(".gdb"))
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��GDB��ʽ�ļ�");
                        return;
                    }
                    this.textBoxUptRange.Text = pFolderBrowser.SelectedPath;
                }
            }
            else if(comboBoxUptRangeType.Tag=="SHP")
            {
                OpenFileDialog OpenFile = new OpenFileDialog();
                OpenFile.Title = "ѡ����·�Χ����";
                OpenFile.Filter = "SHP����(*.shp)|*.shp";
                if (OpenFile.ShowDialog() == DialogResult.OK)
                {
                    this.textBoxUptRange.Text = OpenFile.FileName;
                }
            }
        }


        private void btnUptData_Click(object sender, EventArgs e)
        {
            if (comboBoxUptDataType.Tag == "PDB")
            {
                OpenFileDialog OpenFile = new OpenFileDialog();
                OpenFile.Title = "ѡ���������";
                OpenFile.Filter = "MDB����(*.mdb)|*.mdb";
                if (OpenFile.ShowDialog() == DialogResult.OK)
                {
                    this.textBoxUptData.Text = OpenFile.FileName;
                }
            }
            else if (comboBoxUptDataType.Tag == "GDB")
            {
                FolderBrowserDialog pFolderBrowser = new FolderBrowserDialog();

                if (pFolderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (!pFolderBrowser.SelectedPath.EndsWith(".gdb"))
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��GDB��ʽ�ļ�");
                        return;
                    }
                    this.textBoxUptData.Text = pFolderBrowser.SelectedPath;
                }
            }
            //else if (comboBoxUptRangeType.Tag == "SHP")
            //{
            //    OpenFileDialog OpenFile = new OpenFileDialog();
            //    OpenFile.Title = "ѡ���������";
            //    OpenFile.Filter = "SHP����(*.shp)|*.shp";
            //    if (OpenFile.ShowDialog() == DialogResult.OK)
            //    {
            //        this.textBoxUptData.Text = OpenFile.FileName;
            //    }
            //}
        }

        private void comboBoxUptRangeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxUptRangeType.Text == "ESRI�������ݿ�(*.mdb)")
            {
                comboBoxUptRangeType.Tag = "PDB";
            }
            else if (comboBoxUptRangeType.Text == "ESRI�ļ����ݿ�(*.gdb)")
            {
                comboBoxUptRangeType.Tag = "GDB";
            }
            else if (comboBoxUptRangeType.Text == "ShapeFile(*.shp)")
            {
                comboBoxUptRangeType.Tag = "SHP";
            }
            else if (comboBoxUptRangeType.Text == "ArcSDE(For Oracle)")
            {
                comboBoxUptRangeType.Tag = "SDE";
            }
        }

        private void comboBoxUptDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxUptDataType.Text == "ESRI�������ݿ�(*.mdb)")
            {
                comboBoxUptDataType.Tag = "PDB";
            }
            else if (comboBoxUptDataType.Text == "ESRI�ļ����ݿ�(*.gdb)")
            {
                comboBoxUptDataType.Tag = "GDB";
            }
            else if (comboBoxUptDataType.Text == "ShapeFile(*.shp)")
            {
                comboBoxUptDataType.Tag = "SHP";
            }
            else if (comboBoxUptDataType.Text == "ArcSDE(For Oracle)")
            {
                comboBoxUptDataType.Tag = "SDE";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}