using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;
using GeoDataCenterFunLib;
using SysCommon;

namespace GeoDataCenterFrame
{
    public partial class frmDataUpload : DevComponents.DotNetBar.Office2007Form
    {
        public frmDataUpload()
        {
            InitializeComponent();
        }
        OpenFileDialog OpenFile;
        int i = 0;

        private void ImportFeatureClassToNewWorkSpace(string filename,string outfilename)
        {

            //try
            //{
                string ImportShapeFileName = filename;
                if (ImportShapeFileName == "") { return; }
                string ImportFileShortName = System.IO.Path.GetFileNameWithoutExtension(ImportShapeFileName);
                string ExportFileShortName = System.IO.Path.GetFileNameWithoutExtension(outfilename);
                string ImportFilePath = System.IO.Path.GetDirectoryName(ImportShapeFileName);
                IWorkspaceFactory Pwf = new FileGDBWorkspaceFactoryClass();
                IWorkspace pWorkspace = Pwf.OpenFromFile(@comboBox1.Text, 0);
                
                //IWorkspaceFactory Pwf = new AccessWorkspaceFactoryClass();
                //IWorkspace pWorkspace = Pwf.OpenFromFile(@"E:\test\x.mdb", 0);

                IWorkspaceName pInWorkspaceName;
                IFeatureDatasetName pOutFeatureDSName;
                IFeatureClassName pInFeatureClassName;
                IDatasetName pInDatasetName;
                IFeatureClassName pOutFeatureClassName;
                IDatasetName pOutDatasetName;
                long iCounter;
                IFields pOutFields, pInFields;
                IFieldChecker pFieldChecker;
                IField pGeoField;
                IGeometryDef pOutGeometryDef;
                IGeometryDefEdit pOutGeometryDefEdit;
                IName pName;
                IFeatureClass pInFeatureClass;
                IFeatureDataConverter pShpToClsConverter;
                IEnumFieldError pEnumFieldError = null;

                //�õ�һ������SHP�ļ��Ĺ����ռ䣬
                pInWorkspaceName = new WorkspaceNameClass();
                pInWorkspaceName.PathName = ImportFilePath;
                pInWorkspaceName.WorkspaceFactoryProgID = "esriCore.ShapefileWorkspaceFactory.1";
                //����һ���µ�Ҫ�������ƣ�Ŀ����Ϊ������PNAME�ӿڵ�OPEN������SHP�ļ�
                pInFeatureClassName = new FeatureClassNameClass();
                pInDatasetName = (IDatasetName)pInFeatureClassName;
                pInDatasetName.Name = ImportFileShortName;
                pInDatasetName.WorkspaceName = pInWorkspaceName;
                //��һ��SHP�ļ�����Ҫ��ȡ�����ֶμ���
                pName = (IName)pInFeatureClassName;
                pInFeatureClass = (IFeatureClass)pName.Open();
                //ͨ��FIELDCHECKER����ֶεĺϷ��ԣ�Ϊ����Ҫ�������ֶμ���
                pInFields = pInFeatureClass.Fields;
                pFieldChecker = new FieldChecker();
                pFieldChecker.Validate(pInFields, out pEnumFieldError, out pOutFields);
                //ͨ��ѭ�����Ҽ����ֶ�
                pGeoField = null;
                for (iCounter = 0; iCounter < pOutFields.FieldCount; iCounter++)
                {
                    if (pOutFields.get_Field((int)iCounter).Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        pGeoField = pOutFields.get_Field((int)iCounter);
                        break;
                    }
                }
                //�õ������ֶεļ��ζ���
                pOutGeometryDef = pGeoField.GeometryDef;
                //���ü����ֶεĿռ�ο�������
                pOutGeometryDefEdit = (IGeometryDefEdit)pOutGeometryDef;
                pOutGeometryDefEdit.GridCount_2 = 1;
                pOutGeometryDefEdit.set_GridSize(0, 1500000);

                //����һ���µ�Ҫ����������Ϊ���õĲ���
                pOutFeatureClassName = new FeatureClassNameClass();
                pOutDatasetName = (IDatasetName)pOutFeatureClassName;
                pOutDatasetName.Name = ExportFileShortName;
                //����һ���µ����ݼ�������Ϊ���õĲ���
                pOutFeatureDSName = (IFeatureDatasetName)new FeatureDatasetName();
                //���������ֵ��NULL��˵��Ҫ��������Ҫ����
                //����һ�������ڵ�Ҫ�ؼ���pFDN��ͨ������IFeatureClassName�͹����ռ�������������ConvertFeatureClass��������ʹ�øñ�����Ϊ������
                IFeatureDatasetName pFDN = new FeatureDatasetNameClass();
                IDatasetName pDN = (IDatasetName)pFDN;
                IDataset pDS = (IDataset)pWorkspace;
                pDN.WorkspaceName = (IWorkspaceName)pDS.FullName;
                pOutFeatureClassName.FeatureDatasetName = (IDatasetName)pFDN;
                //��pOutFeatureDSName����ΪNull��������Ϊ������ConvertFeatureClass��������ΪIFeatureClassName�����Ѿ��͹����ռ�����ˣ����ɵ�
                //Ҫ�����ڹ����ռ�ĸ�Ŀ¼�£�������Ҫ����
                pOutFeatureDSName = null;


                //��ʼ����
                pShpToClsConverter = new FeatureDataConverterClass();
                pShpToClsConverter.ConvertFeatureClass(pInFeatureClassName, null, pOutFeatureDSName, pOutFeatureClassName, null, pOutFields, "", 1000, 0);
                MessageBox.Show("����ɹ�", "ϵͳ��ʾ");
            //}
            //catch
            //{
            //    MessageBox.Show("����ʧ��", "ϵͳ��ʾ");
            //}
        }

        private void checkboxPf_CheckedChanged(object sender, EventArgs e)
        {
            textBox.Enabled = true;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.SubItems[1].Text = textBox.Text + item.Text;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFile = new OpenFileDialog();
            OpenFile.Filter = "SHP�ļ�|*.shp";
            //��SHP�ļ�
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                string name = OpenFile.FileName.Substring(OpenFile.FileName.LastIndexOf('\\')+1,OpenFile.FileName.Length-OpenFile.FileName.LastIndexOf('\\')-1);
                  listView.Items.Add(name);
                  if (textBox.Text != "")
                      listView.Items[name].SubItems.Add(textBox.Text + name + "");
                  else
                      listView.Items[i].SubItems.Add(name + "");
                  i++;

            }
            
             
            
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                if (item.Checked)
                {
                    listView.Items.Remove(item);
                    i--;
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            listView.Items.Clear();
            i = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            bool _check=false;
            foreach (ListViewItem item in listView.Items)
            {
                if (item.Checked)
                {
                    _check = true;
                    MessageBox.Show("���ڵ���" + item.Text);
                    ImportFeatureClassToNewWorkSpace(OpenFile.FileName,item.SubItems[1].Text);
                }
            }
            if (_check ==false)
                MessageBox.Show("��ѡ��Ҫ�ϴ����ļ���");
        }
    }
}