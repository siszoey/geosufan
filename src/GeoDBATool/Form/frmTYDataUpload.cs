using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;
using SysCommon;
using SysCommon.Gis;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geometry;
using GeoDataCenterFunLib;


namespace GeoDBATool
{
    public partial class frmTYDataUpload : DevComponents.DotNetBar.Office2007Form
    {
        public frmTYDataUpload(IWorkspace pWorkspace)
        {
            InitializeComponent();
            m_TempWorkspace = pWorkspace;
        }
        private bool _Writelog = true;  //added by chulili 2012-09-07 �Ƿ�д��־
        public bool WriteLog
        {
            get
            {
                return _Writelog;
            }
            set
            {
                _Writelog = value;
            }
        }
        //SysGisDataSet ds = new SysGisDataSet();
        OpenFileDialog OpenFile;
        int i = 0;
        int count = 0;
        bool m_success=false;
        bool m_newfile;
        List<string> filename = new List<string>();//�������ȡ������
        string[] array = new string[6];//������������
        string m_strErr;//������Ϣ����
        private IWorkspace pTargetworkspace;
        private IWorkspace m_TempWorkspace = null;

        /// <summary>
        /// ��GDB�������뵽SDE�� xisheng 20110919
        /// </summary>
        private void ImportGDBToSDE(string file, string outfilename, out int featurecount)
        {
            m_success = false;//��ʼ��
            try
            {
                string filepath = file.Substring(0, file.LastIndexOf("---"));
                string filename = file.Substring(file.LastIndexOf("-") + 1);

                //��mdb�ļ����ڵĹ����ռ�
                ESRI.ArcGIS.Geodatabase.IWorkspaceFactory wf = new FileGDBWorkspaceFactory();
                IFeatureWorkspace pFeatureWorkspaceGDB = wf.OpenFromFile(@filepath, 0) as IFeatureWorkspace;
                IWorkspace pWorkspaceGDB = pFeatureWorkspaceGDB as IWorkspace;
                // ����Դ�����ռ�����     
                IDataset sourceWorkspaceDataset = (IDataset)pFeatureWorkspaceGDB;
                IWorkspaceName sourceWorkspaceName = (IWorkspaceName)sourceWorkspaceDataset.FullName;

                //����Դ���ݼ�����        
                //IFeatureClassName sourceFeatureClassName = serverContext.CreateObject("esriGeoDatabase.FeatureClassName") as IFeatureClassName;
                IFeatureClassName sourceFeatureClassName = new FeatureClassNameClass();
                IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureClassName;
                sourceDatasetName.WorkspaceName = sourceWorkspaceName;
                sourceDatasetName.Name = filename;

                //�򿪴��ڵĹ����ռ䣬��Ϊ����Ŀռ�;
                IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pTargetworkspace;

                //����Ŀ�깤���ռ�����    
                IDataset targetWorkspaceDataset = (IDataset)pTargetworkspace;
                IWorkspaceName targetWorkspaceName = (IWorkspaceName)targetWorkspaceDataset.FullName;
                IWorkspace2 pWorkspace2 = pTargetworkspace as IWorkspace2;
                IFeatureDataset tmpfeaturedataset;
                //����Ŀ�����ݼ�����    
                // IFeatureClassName targetFeatureClassName = serverContext.CreateObject("esriGeoDatabase.FeatureClassName") as IFeatureClassName;
                //�ж�Ҫ���Ƿ���ڣ������ڽ�ɾ��Դ�ļ�
                if (pWorkspace2.get_NameExists(esriDatasetType.esriDTFeatureDataset, textBox.Text))
                {
                    tmpfeaturedataset = pFeatureWorkspace.OpenFeatureDataset(textBox.Text);
                    if (text_prj.Text != "")
                    {
                        IGeoDatasetSchemaEdit pgeodataset = tmpfeaturedataset as IGeoDatasetSchemaEdit;
                        if (pgeodataset.CanAlterSpatialReference)
                            pgeodataset.AlterSpatialReference(GetSpatialReferenceformFile(text_prj.Text));
                    }
                }
                else
                {
                   tmpfeaturedataset = CreateFeatureDataset(pTargetworkspace as IFeatureWorkspace, textBox.Text, text_prj.Text);
                     
                }
                if (pWorkspace2.get_NameExists(esriDatasetType.esriDTFeatureClass,outfilename))
                {

                    IFeatureClass tmpfeatureclass;
                    tmpfeatureclass = pFeatureWorkspace.OpenFeatureClass(outfilename);
                    IDataset tempset = tmpfeatureclass as IDataset;
                    tempset.Delete();
                }

                IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();
                IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
                targetDatasetName.WorkspaceName = targetWorkspaceName;
                targetDatasetName.Name = outfilename;
                //Ŀ�����ݼ�
                IFeatureDatasetName outfeaturedatasetname = tmpfeaturedataset.FullName as IFeatureDatasetName;
                

                //�������Ҫ�����Եõ��ֶζ���      
                ESRI.ArcGIS.esriSystem.IName sourceName = (ESRI.ArcGIS.esriSystem.IName)sourceFeatureClassName;
                IFeatureClass sourceFeatureClass = (IFeatureClass)sourceName.Open();//��ԴҪ���� 

                //��֤�ֶ����ƣ���Ϊ�����ڲ�ͬ���͵Ĺ����ռ�֮���������ת��
                //IFieldChecker fieldChecker = serverContext.CreateObject("esriGeoDatabase.FieldChecker") as IFieldChecker;
                IFieldChecker fieldChecker = new FieldCheckerClass();
                IFields sourceFeatureClassFields = sourceFeatureClass.Fields;
                IFields targetFeatureClassFields;
                IEnumFieldError enumFieldError;

                //����Ҫ�������������֤�����ռ�
                fieldChecker.InputWorkspace = pWorkspaceGDB;
                fieldChecker.ValidateWorkspace = pTargetworkspace;
                fieldChecker.Validate(sourceFeatureClassFields, out enumFieldError, out targetFeatureClassFields);

                //������������ֶ��ҵ������ֶ�
                IField geometryField;
                for (int i = 0; i < targetFeatureClassFields.FieldCount; i++)
                {
                    if (targetFeatureClassFields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        geometryField = targetFeatureClassFields.get_Field(i);
                        //�õ������ֶεļ��ζ���                
                        IGeometryDef geometryDef = geometryField.GeometryDef;
                        //���輸�ζ���һ���ռ�����������Ŀ�͸�����Сֵ
                        IGeometryDefEdit targetFCGeoDefEdit = (IGeometryDefEdit)geometryDef;
                        targetFCGeoDefEdit.GridCount_2 = 1;
                        targetFCGeoDefEdit.set_GridSize(0, 0);
                        //����ArcGISΪ���ݼ���ȷ��һ����Ч�ĸ�����С
                        targetFCGeoDefEdit.SpatialReference_2 = geometryField.GeometryDef.SpatialReference;
                        //ת��Ҫ���������е�Ҫ��
                        //IQueryFilter queryFilter = serverContext.CreateObject("esriGeoDatabase.QueryFilter") as IQueryFilter; ;
                        QueryFilter queryFilter = new QueryFilterClass();
                        queryFilter.WhereClause = "";
                        //����Ҫ����               
                        //IFeatureDataConverter fctofc = serverContext.CreateObject("esriGeoDatabase.FeatureDataConverter") as IFeatureDataConverter;

                        IFeatureDataConverter fctofc = new FeatureDataConverterClass();
                        IEnumInvalidObject enumErrors = fctofc.ConvertFeatureClass(sourceFeatureClassName, queryFilter, outfeaturedatasetname, targetFeatureClassName, geometryDef, targetFeatureClassFields, "", 1000, 0);

                    }
                }
                featurecount=sourceFeatureClass.FeatureCount(null);
                m_success = true;
            }
            catch (Exception ee) { m_success = false; m_strErr = ee.Message; featurecount = 0; }
        
        }

        /// <summary>
        /// ��MDB�������뵽SDE�� xisheng 20110919
        /// </summary>
        private void ImportMDBToSDE(string file, string outfilename, out int featurecount)
        {
            m_success = false;//��ʼ��
            try
            {
                string filepath = file.Substring(0, file.LastIndexOf("---"));
                string filename = file.Substring(file.LastIndexOf("-") + 1);

                //��mdb�ļ����ڵĹ����ռ� 
                ESRI.ArcGIS.Geodatabase.IWorkspaceFactory wf = new AccessWorkspaceFactoryClass();
                IFeatureWorkspace pFeatureWorkspaceGDB = wf.OpenFromFile(@filepath, 0) as IFeatureWorkspace;
                IWorkspace pWorkspaceGDB = pFeatureWorkspaceGDB as IWorkspace;
                // ����Դ�����ռ�����     
                IDataset sourceWorkspaceDataset = (IDataset)pFeatureWorkspaceGDB;
                IWorkspaceName sourceWorkspaceName = (IWorkspaceName)sourceWorkspaceDataset.FullName;

                //����Դ���ݼ�����        
                //IFeatureClassName sourceFeatureClassName = serverContext.CreateObject("esriGeoDatabase.FeatureClassName") as IFeatureClassName;
                IFeatureClassName sourceFeatureClassName = new FeatureClassNameClass();
                IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureClassName;
                sourceDatasetName.WorkspaceName = sourceWorkspaceName;
                sourceDatasetName.Name = filename;

                //�򿪴��ڵĹ����ռ䣬��Ϊ����Ŀռ�;
                IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pTargetworkspace;

                //����Ŀ�깤���ռ�����    
                IDataset targetWorkspaceDataset = (IDataset)pTargetworkspace;
                IWorkspaceName targetWorkspaceName = (IWorkspaceName)targetWorkspaceDataset.FullName;
                IWorkspace2 pWorkspace2 = pTargetworkspace as IWorkspace2;
                IFeatureDataset tmpfeaturedataset;
                //����Ŀ�����ݼ�����    
                // IFeatureClassName targetFeatureClassName = serverContext.CreateObject("esriGeoDatabase.FeatureClassName") as IFeatureClassName;
                //�ж�Ҫ���Ƿ���ڣ������ڽ�ɾ��Դ�ļ�
                if (pWorkspace2.get_NameExists(esriDatasetType.esriDTFeatureDataset, textBox.Text))
                {
                    tmpfeaturedataset = pFeatureWorkspace.OpenFeatureDataset(textBox.Text);
                    if (text_prj.Text != "")
                    {
                        IGeoDatasetSchemaEdit pgeodataset = tmpfeaturedataset as IGeoDatasetSchemaEdit;
                        if (pgeodataset.CanAlterSpatialReference)
                            pgeodataset.AlterSpatialReference(GetSpatialReferenceformFile(text_prj.Text));
                    }
                }
                else
                {
                    tmpfeaturedataset = CreateFeatureDataset(pTargetworkspace as IFeatureWorkspace, textBox.Text, text_prj.Text);

                }
                if (pWorkspace2.get_NameExists(esriDatasetType.esriDTFeatureClass, outfilename))
                {

                    IFeatureClass tmpfeatureclass;
                    tmpfeatureclass = pFeatureWorkspace.OpenFeatureClass(outfilename);
                    IDataset tempset = tmpfeatureclass as IDataset;
                    if(tempset.CanDelete())
                    tempset.Delete();
                }

                IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();
                IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
                targetDatasetName.WorkspaceName = targetWorkspaceName;
                targetDatasetName.Name = outfilename;
                //Ŀ�����ݼ�
                IFeatureDatasetName outfeaturedatasetname = tmpfeaturedataset.FullName as IFeatureDatasetName;


                //�������Ҫ�����Եõ��ֶζ���      
                ESRI.ArcGIS.esriSystem.IName sourceName = (ESRI.ArcGIS.esriSystem.IName)sourceFeatureClassName;
                IFeatureClass sourceFeatureClass = (IFeatureClass)sourceName.Open();//��ԴҪ���� 

                //��֤�ֶ����ƣ���Ϊ�����ڲ�ͬ���͵Ĺ����ռ�֮���������ת��
                //IFieldChecker fieldChecker = serverContext.CreateObject("esriGeoDatabase.FieldChecker") as IFieldChecker;
                IFieldChecker fieldChecker = new FieldCheckerClass();
                IFields sourceFeatureClassFields = sourceFeatureClass.Fields;
                IFields targetFeatureClassFields;
                IEnumFieldError enumFieldError;

                //����Ҫ�������������֤�����ռ�
                fieldChecker.InputWorkspace = pWorkspaceGDB;
                fieldChecker.ValidateWorkspace = pTargetworkspace;
                fieldChecker.Validate(sourceFeatureClassFields, out enumFieldError, out targetFeatureClassFields);

                //������������ֶ��ҵ������ֶ�
                IField geometryField;
                for (int i = 0; i < targetFeatureClassFields.FieldCount; i++)
                {
                    if (targetFeatureClassFields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        geometryField = targetFeatureClassFields.get_Field(i);
                        //�õ������ֶεļ��ζ���                
                        IGeometryDef geometryDef = geometryField.GeometryDef;
                        //���輸�ζ���һ���ռ�����������Ŀ�͸�����Сֵ
                        IGeometryDefEdit targetFCGeoDefEdit = (IGeometryDefEdit)geometryDef;
                        targetFCGeoDefEdit.GridCount_2 = 1;
                        targetFCGeoDefEdit.set_GridSize(0, 0);
                        //����ArcGISΪ���ݼ���ȷ��һ����Ч�ĸ�����С
                        targetFCGeoDefEdit.SpatialReference_2 = geometryField.GeometryDef.SpatialReference;
                        //ת��Ҫ���������е�Ҫ��
                        //IQueryFilter queryFilter = serverContext.CreateObject("esriGeoDatabase.QueryFilter") as IQueryFilter; ;
                        QueryFilter queryFilter = new QueryFilterClass();
                        queryFilter.WhereClause = "";
                        //����Ҫ����               
                        //IFeatureDataConverter fctofc = serverContext.CreateObject("esriGeoDatabase.FeatureDataConverter") as IFeatureDataConverter;

                        IFeatureDataConverter fctofc = new FeatureDataConverterClass();
                        IEnumInvalidObject enumErrors = fctofc.ConvertFeatureClass(sourceFeatureClassName, queryFilter, outfeaturedatasetname, targetFeatureClassName, geometryDef, targetFeatureClassFields, "", 1000, 0);

                    }
                }
                featurecount = sourceFeatureClass.FeatureCount(null);
                m_success = true;
            }
            catch (Exception ee) { m_success = false; m_strErr = ee.Message; featurecount = 0; }

        }

        /// <summary>
        ///  ����Ҫ�ؼ� 20110919 xisheng
        /// </summary>
        /// <param name="feaworkspace">ָ�������ռ�</param>
        /// <param name="datasetname">ָ��Ҫ�ؼ�����</param>
        /// <param name="PrjPath">�ռ�ο�</param>
        /// <returns></returns>
        private static IFeatureDataset CreateFeatureDataset(IFeatureWorkspace feaworkspace, string datasetname, string PrjPath)
        {
            try
            {

                string spatialPath = PrjPath;
                ISpatialReferenceFactory pSpaReferenceFac = new SpatialReferenceEnvironmentClass();//�ռ�ο�����
                ISpatialReference pSpatialReference = null;//������ÿռ�ο�
                if (File.Exists(spatialPath))
                {
                    pSpatialReference = pSpaReferenceFac.CreateESRISpatialReferenceFromPRJFile(spatialPath);
                }
                if (pSpatialReference == null)
                {
                    pSpatialReference = new UnknownCoordinateSystemClass();
                }

                //����Ĭ�ϵ�Resolution
                ISpatialReferenceResolution pSpatiaprefRes = pSpatialReference as ISpatialReferenceResolution;
                pSpatiaprefRes.ConstructFromHorizon();//Defines the XY resolution and domain extent of this spatial reference based on the extent of its horizon 
                pSpatiaprefRes.SetDefaultXYResolution();
                pSpatiaprefRes.SetDefaultZResolution();
                pSpatiaprefRes.SetDefaultMResolution();
                //����Ĭ�ϵ�Tolerence
                ISpatialReferenceTolerance pSpatialrefTole = pSpatiaprefRes as ISpatialReferenceTolerance;
                pSpatialrefTole.SetDefaultXYTolerance();
                pSpatialrefTole.SetDefaultZTolerance();
                pSpatialrefTole.SetDefaultMTolerance();

                //�������ݼ�

                IFeatureDataset pFeatureDataset = null;//�������ݼ�����װ��Ҫ����
                pFeatureDataset = feaworkspace.CreateFeatureDataset(datasetname, pSpatialReference);


                return pFeatureDataset;
            }
            catch (Exception e)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(e, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(e, null, DateTime.Now);
                }
                //********************************************************************
                return null;
            }
        }


        /// <summary>
        ///  ��ÿռ�ο� 20110919 xisheng
        /// </summary>
        /// <param name="PrjPath">�ռ�ο�·��</param>
        /// <returns></returns>
        private static ISpatialReference GetSpatialReferenceformFile(string PrjPath)
        {
            try
            {

                string spatialPath = PrjPath;
                ISpatialReferenceFactory pSpaReferenceFac = new SpatialReferenceEnvironmentClass();//�ռ�ο�����
                ISpatialReference pSpatialReference = null;//������ÿռ�ο�
                if (File.Exists(spatialPath))
                {
                    pSpatialReference = pSpaReferenceFac.CreateESRISpatialReferenceFromPRJFile(spatialPath);
                }
                if (pSpatialReference == null)
                {
                    pSpatialReference = new UnknownCoordinateSystemClass();
                }

                //����Ĭ�ϵ�Resolution
                ISpatialReferenceResolution pSpatiaprefRes = pSpatialReference as ISpatialReferenceResolution;
                pSpatiaprefRes.ConstructFromHorizon();//Defines the XY resolution and domain extent of this spatial reference based on the extent of its horizon 
                pSpatiaprefRes.SetDefaultXYResolution();
                pSpatiaprefRes.SetDefaultZResolution();
                pSpatiaprefRes.SetDefaultMResolution();
                //����Ĭ�ϵ�Tolerence
                ISpatialReferenceTolerance pSpatialrefTole = pSpatiaprefRes as ISpatialReferenceTolerance;
                pSpatialrefTole.SetDefaultXYTolerance();
                pSpatialrefTole.SetDefaultZTolerance();
                pSpatialrefTole.SetDefaultMTolerance();

                return pSpatialReference;
            }
            catch (Exception e)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(e, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(e, null, DateTime.Now);
                }
                //********************************************************************
                return null;
            }
        }

        //���mdb/gdb��Ҫ����
        public List<string> Getfeatureclass(IWorkspace pWorkspaceMDB)
        {
            List<string> list = new List<string>();
            IEnumDataset enumDataset = pWorkspaceMDB.get_Datasets(esriDatasetType.esriDTFeatureClass) as IEnumDataset;
            IDataset dataset = enumDataset.Next();
            while (dataset != null)
            {
                if (dataset.Type == esriDatasetType.esriDTFeatureClass)
                {
                    //IFeatureClass pFeatureClass = dataset as IFeatureClass;
                    //IDataset pDataset = pFeatureClass as IDataset;;
                    list.Add(dataset.Name); 
                    dataset = enumDataset.Next();
                }
            }
            enumDataset = pWorkspaceMDB.get_Datasets(esriDatasetType.esriDTFeatureDataset) as IEnumDataset;
            dataset = enumDataset.Next();
            while (dataset != null)
            {
                if (dataset.Type == esriDatasetType.esriDTFeatureDataset)
                {
                    IEnumDataset pEnumDataset = dataset.Subsets;
                    IDataset dataset2 = pEnumDataset.Next();
                    while (dataset2 != null)
                    {
                        list.Add(dataset2.Name);
                        dataset2 = pEnumDataset.Next();
                    }
            
                }
                dataset = enumDataset.Next();
            }
            return list;
        }
     

        //private void textBox_TextChanged(object sender, EventArgs e)
        //{
        //    string name;
        //    foreach (ListViewItem item in listView.Items)
        //    {
        //        string[] str = item.Text.Split('.');
        //        str[1] = str[1].Substring(0, 3);
        //        if (str[1].ToLower() == "shp")
        //        {
        //            name = System.IO.Path.GetFileNameWithoutExtension(item.Text);
        //            item.SubItems[1].Text = textBox.Text + name.ToUpper();
        //        }
        //        else if (str[1].ToLower() == "mdb")
        //        {
        //            name = item.Text.Substring(item.Text.LastIndexOf("-") + 1);
        //            string strName = GetForwadName(textBox.Text.ToUpper(),name);
        //            item.SubItems[1].Text = strName.ToUpper();
        //        }
        //        else if (str[1].ToLower() == "gdb")
        //        {
        //            name = item.Text.Substring(item.Text.LastIndexOf("-") + 1);
        //            string strName = GetForwadName(textBox.Text.ToUpper(), name);
        //            item.SubItems[1].Text = strName.ToUpper();
        //        }
                
                
        //    }
        //}
        /// <summary>
        /// �����ݷ������ַ�������
        /// </summary>
        /// <param name="filename">��������</param>
        public void AnalyseDataToArray(string filename)
        {
            array[3] = filename.Substring(4, filename.LastIndexOf("_") - 4);
            array[1] = filename.Substring(filename.LastIndexOf("_") + 1);
        }

        public string GetDescrib(string str)
        {
            SysGisTable sysTable = new SysGisTable(m_TempWorkspace);
            Exception err = null;
            Dictionary<string, object> dic = sysTable.GetRow("��׼ͼ������", "CODE='" + str + "'", out err);
            string strreturn = "";
            if (dic != null)
            {
                if (dic.ContainsKey("NAME"))
                {
                    strreturn = dic["NAME"].ToString();
                }
            }
            sysTable = null;
            return strreturn;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string file = "";
            if (pTargetworkspace == null)
            {
                MessageBox.Show("��������Ŀ������Դ", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBox.Text=="")
            {
                MessageBox.Show("�����������ݼ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (rbGDB.Checked)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    if (!fbd.SelectedPath.EndsWith("gdb"))
                    {
                        MessageBox.Show("��ѡ��GDB����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    file = fbd.SelectedPath;
                }
                else
                    return;
            }
            else
            {
                OpenFileDialog OpenFile = new OpenFileDialog();
                OpenFile.Title = "ѡ��ESRI�������ݿ�";
                OpenFile.Filter = "ESRI�������ݿ�(*.mdb)|*.mdb";
                if (OpenFile.ShowDialog() == DialogResult.OK)
                {
                    file = OpenFile.FileName;
                }
                else
                    return;
            }
            
                for (int j = 0; j < filename.Count; j++)
                {
                    if (filename[j].Trim().CompareTo(file) == 0)
                    {
                        MessageBox.Show("�ļ��Ѵ������б���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Cursor = Cursors.Default;
                        return;
                    }

                }
            //string name = file.Substring(file.LastIndexOf('\\') + 1, file.Length - file.LastIndexOf('\\') - 1);
            string[] str = file.Split('.');
            if (str[1].ToLower() == "mdb")
            {
                ESRI.ArcGIS.Geodatabase.IWorkspaceFactory wf = new AccessWorkspaceFactory();
                IFeatureWorkspace pFeatureWorkspaceMDB = wf.OpenFromFile(@file, 0) as IFeatureWorkspace;
                IWorkspace pWorkspaceMDB = pFeatureWorkspaceMDB as IWorkspace;
                List<string> list = Getfeatureclass(pWorkspaceMDB);
                for (int ii = 0; ii < list.Count; ii++)
                {
                    listView.Items.Add(file + "---" + list[ii]);
                        filename.Add(file);
                    listView.Items[i].SubItems.Add(list[ii].ToUpper());
                    string strdescri = GetDescrib(list[ii].ToUpper());
                    if (strdescri.Trim() != "")
                        listView.Items[i].SubItems.Add(strdescri);
                    else
                        listView.Items[i].SubItems.Add("��");
                    if (GetExists(list[ii].ToUpper()))
                    {
                        listView.Items[i].SubItems.Add("�Ѵ���(���滻)");
                        listView.Items[i].SubItems[1].Text = GetNotExistNames(listView.Items[i].SubItems[1].Text);
                    }
                    else
                        listView.Items[i].SubItems.Add("������");
                    listView.Items[i].SubItems.Add("�ȴ����");
                    listView.Items[i].Checked = true;
                    i++;
                }
            }
            else if (str[1].ToLower() == "gdb")
            {
                ESRI.ArcGIS.Geodatabase.IWorkspaceFactory wf = new FileGDBWorkspaceFactoryClass();
                IFeatureWorkspace pFeatureWorkspaceGDB = wf.OpenFromFile(@file, 0) as IFeatureWorkspace;
                IWorkspace pWorkspaceGDB = pFeatureWorkspaceGDB as IWorkspace;
                List<string> list = Getfeatureclass(pWorkspaceGDB);
                for (int ii = 0; ii < list.Count; ii++)
                {
                    listView.Items.Add(file + "---" + list[ii]);
                        filename.Add(file);
                    listView.Items[i].SubItems.Add(list[ii].ToUpper());
                    string strdescri = GetDescrib(list[ii].ToUpper());
                    if (strdescri.Trim() != "")
                        listView.Items[i].SubItems.Add(strdescri);
                    else
                        listView.Items[i].SubItems.Add("��");
                    if (GetExists(list[ii].ToUpper()))
                    {
                        listView.Items[i].SubItems.Add("�Ѵ���(���滻)");
                       
                        listView.Items[i].SubItems[1].Text= GetNotExistNames(listView.Items[i].SubItems[1].Text);
                    }
                    else
                        listView.Items[i].SubItems.Add("������");
                    listView.Items[i].SubItems.Add("�ȴ����");
                    listView.Items[i].Checked = true;
                    i++;
                }
            }

            this.Cursor = Cursors.Default;

        }

        /// <summary>
        /// �ж��Ƿ����ͬ����Ҫ���� xisheng 20110920
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool GetExists(string name)
        {
            IWorkspace2 pworkspace = pTargetworkspace as IWorkspace2;
            if (pworkspace.get_NameExists(esriDatasetType.esriDTFeatureClass, name))
                return true;
            else
                return false;
        }

        /// <summary>
        /// ��ǰ׺���ձ�׼����򻯣�����֯
        /// </summary>
        /// <param name="str1">ǰ׺</param>
        /// <param name="str2">ͼ��</param>
        /// <returns></returns>
        public string GetForwadName(string str1, string str2)
        {
            AnalyseDataToArray(str1);
            str1 =str1+"_"+str2;
            return str1;

        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.Items)
            {
                if (item.Checked)
                {
                    listView.Items.Remove(item);
                    filename.Remove(item.Text.Substring(0,item.Text.LastIndexOf("---")));
                    i--;
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            listView.Items.Clear();
            filename.Clear();
            i = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            bool _check=false;
            if (textBox.Text.Trim() == "")
            {
                MessageBox.Show("������һ�����ݼ����ƣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string lastpath="";
            foreach (ListViewItem item in listView.Items)
            {
                m_strErr = "";
                string []str=item.Text.Split('.');
                string[] strfile = item.Text.Split("-".ToCharArray());
                str[1] = str[1].Substring(0, 3);
                int featurecount = 0;
                if(strfile[0]!=lastpath)
                {
                    lastpath=strfile[0];
                    if (this.WriteLog)
                    {
                        Plugin.LogTable.Writelog("ͨ�����ݿ�ʼ���,Դ����·��Ϊ:" + strfile[0]);
                    }
                }
                if (item.Checked&&item.SubItems[4].Text=="�ȴ����")
                {
                    _check = true;
                    item.SubItems[4].Text = "�������";
                    listView.Refresh();
                    //if (str[1].ToLower() == "shp")
                    //    ImportFeatureClassToNewWorkSpace(item.Text, item.SubItems[1].Text);
                   if (str[1].ToLower() == "mdb")
                       ImportMDBToSDE(item.Text, item.SubItems[1].Text, out featurecount);
                    else if (str[1].ToLower() == "gdb")
                        ImportGDBToSDE(item.Text, item.SubItems[1].Text,out featurecount);

                    if (m_success)
                   {
                        item.SubItems[4].Text = "������";
                       item.SubItems.Add(featurecount + "��Ҫ�������");//���Ӽ�¼���
                       if (this.WriteLog)
                       {
                           Plugin.LogTable.Writelog("Դͼ��:" + strfile[3] + ",Ŀ��ͼ��:" + item.SubItems[1].Text + ",��" + featurecount + "��Ҫ�������");
                       }
                   }
                    else
                    {
                        if(m_strErr!="")
                       {
                            item.SubItems[4].Text = m_strErr + ",���ʧ��";
                            if (this.WriteLog)
                            {
                                Plugin.LogTable.Writelog("Դͼ��:" + strfile[3] + "���ʧ��,��ϸ��ϢΪ:" + m_strErr);
                            }
                       }
                        else
                       {
                        item.SubItems[4].Text = "���ʧ��";
                        if (this.WriteLog)
                        {
                            Plugin.LogTable.Writelog("Դͼ��:" + strfile[3] + "���ʧ��");
                        }
                    }
                   }
                    listView.Refresh();
                }
            }
            if (_check ==false)
                MessageBox.Show("��ѡ��Ҫ�����ļ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("���������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog("������");
            }
        }

        
        private void btnServer_Click(object sender, EventArgs e)
        {
            frmXZDBPropertySet frm = new frmXZDBPropertySet();
            frm.GetPropertySetStr = textSource.Text;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                textSource.Text = frm.GetPropertySetStr;
                pTargetworkspace = frm.m_pworkspace;
            }
        }


        private void btnSelect_Click(object sender, EventArgs e)
        {
            frmNameRule frm = new frmNameRule(textBox,m_TempWorkspace);
            frm.ShowDialog();

        }

        //ȫѡ��ť
        private void btnAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <listView.Items.Count; i++)
            {
                listView.Items[i].Checked = true;
            }

        }
        //��ѡ��ť
        private void btnInverse_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView.Items.Count; i++)
            {
                if (listView.Items[i].Checked == false)
                {
                    listView.Items[i].Checked = true;
                    //datagwSource.Rows[i].Selected = true;
                }
                else
                {
                    listView.Items[i].Checked = false;
                    //datagwSource.Rows[i].Selected = false;
                }
            }
        }

        
        //����ͼ��
        private void btnNewLayer_Click(object sender, EventArgs e)
        {
            List<string> Laylist = new List<string>();
            foreach (ListViewItem item in listView.Items)
            {
                if (item.SubItems[2].Text.Trim() != "��")
                    continue;
                string strName=item.SubItems[1].Text;
               if (strName.Length > 15)
                  strName= strName.Substring(15);
              Laylist.Add(strName);
            }
            frmNewLayer frm = new frmNewLayer(Laylist,this.listView);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                foreach (ListViewItem item in listView.Items)
                {
                    string strName = item.SubItems[1].Text;
                    if (strName.Length > 15)
                    {
                        string strforward = strName.Substring(0,15);
                        strName = strName.Substring(15);
                        item.SubItems[1].Text = GetForwadName(strforward, strName);
                    }
                    
                    string strdescri = GetDescrib(strName); 
                    if (strdescri.Trim() != "")
                    {
                        item.SubItems[2].Text = strdescri;
                    }
                    else
                    {
                        item.SubItems[2].Text="��";
                    }
                }
            }
            listView.Refresh();
        }

        //ѡ��ռ�ο�
        private void btn_SelectPRJ_Click(object sender, EventArgs e)
        {
            OpenFileDialog flg = new OpenFileDialog();
            flg.Filter = "�ռ�ο��ļ�|*.prj";
            flg.Title = "ѡ��һ���ռ�ο��ļ�";
            if (flg.ShowDialog() == DialogResult.OK)
            {
                text_prj.Text = flg.FileName;
            }

        }

        //������ݼ������Ƿ����
        private void btn_CheckExist_Click(object sender, EventArgs e)
        {
            if (pTargetworkspace == null)
            {
                MessageBox.Show("������������Դ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBox.Text.Trim() == "")
            {
                MessageBox.Show("���ݼ����Ʋ���Ϊ�գ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            IWorkspace2 pWorkspace2 = pTargetworkspace as IWorkspace2;
            if (pWorkspace2.get_NameExists(esriDatasetType.esriDTFeatureDataset, textBox.Text))
            {
                MessageBox.Show("���ݼ������Ѵ��ڣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                MessageBox.Show("���ݼ��������óɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btn_CheckExist.Enabled = false;
            }
        }

        /// <summary>
        /// ��ò�ͬ���Ƶ����� 20110920 xisheng 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
      private string GetNotExistNames(string name)
      {
          if(pTargetworkspace==null)
              return name;
          IWorkspace2 pw=pTargetworkspace as IWorkspace2;
          name+="_";
          if (pw.get_NameExists(esriDatasetType.esriDTFeatureClass,name))
          {
              name=GetNotExistNames(name);
          }
          return name;
      }

        private void listView_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                int index = listView.SelectedIndices[0];
                if (listView.Items[index].SubItems[3].Text == "�Ѵ���(���滻)")
                {
                    listView.Items[index].SubItems[3].Text = "�Ѵ���(�滻)";
                    string name = listView.Items[index].Text;
                    listView.Items[index].SubItems[1].Text = name.Substring(name.LastIndexOf("-") + 1);
                }
                else if (listView.Items[index].SubItems[3].Text == "�Ѵ���(�滻)")
                {
                    listView.Items[index].SubItems[3].Text = "�Ѵ���(���滻)";
                    string name = listView.Items[index].SubItems[1].Text;
                    listView.Items[index].SubItems[1].Text = GetNotExistNames(name);
                }
            }
        }

        //�����滻
        private void btn_MutiRepace_Click(object sender, EventArgs e)
        {

            foreach (ListViewItem item in listView.Items)
            {
                if (!item.Checked)
                    continue;
                if (item.SubItems[3].Text == "�Ѵ���(���滻)")
                {
                    item.SubItems[3].Text = "�Ѵ���(�滻)";
                    string name = item.Text;
                    item.SubItems[1].Text = name.Substring(name.LastIndexOf("-") + 1);
                }
                else if (item.SubItems[3].Text == "�Ѵ���(�滻)")
                {
                    item.SubItems[3].Text = "�Ѵ���(���滻)";
                    string name = item.SubItems[1].Text;
                    item.SubItems[1].Text = GetNotExistNames(name);
                }
            }

        }

        //���ı������ݱ��˺󣬼���������
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            btn_CheckExist.Enabled = true;
        }

     

    
      
    }
}