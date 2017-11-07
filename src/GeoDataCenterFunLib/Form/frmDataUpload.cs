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

//*********************************************************************************
//** �ļ�����frmDataUpload.cs
//** CopyRight (c) �人������Ϣ���̼������޹�˾�����������
//** �����ˣ�ϯʤ
//** ��  �ڣ�20011-03-07
//** �޸��ˣ�
//** ��  �ڣ�
//** ��  ����
//**
//** ��  ����1.0
//*********************************************************************************

namespace GeoDataCenterFunLib
{
    public partial class frmDataUpload : DevComponents.DotNetBar.Office2007Form
    {
        public frmDataUpload()
        {
            InitializeComponent();
            
        }

        private void frmDataUpload_Load(object sender, EventArgs e)
        {
           
            string strExp = "select ����Դ���� from ��������Դ��";
            string mypath = m_dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            List<string> list = db.GetDataReaderFromMdb(strCon, strExp);
            for (int ii = 0; ii < list.Count; ii++)
            {
                comboBoxSource.Items.Add(list[ii]);//��������Դ�б��
            }
            if (list.Count > 0)
            {
                comboBoxSource.SelectedIndex = 0;//Ĭ��ѡ���һ��
            }
            checkBoxNew.Checked = true;//����Դ�ļ���checkboxĬ��Ϊ��
            checkboxLegal.Checked = true;//����ļ����Ϸ���
        }

        GetDataTreeInitIndex m_dIndex = new GetDataTreeInitIndex();//ȡ��·������
        frmDataReduction fdr=new frmDataReduction();//ɾ��ʱ��������������ɾ����ķ���
        //SysGisDataSet ds = new SysGisDataSet();
        OpenFileDialog OpenFile;
        int i = 0;
        bool m_success=false;
        bool m_newfile;
        string[] array = new string[6];//������������
        string m_strErr;//������Ϣ����

        //SHP������⵽GDB���ݿ�ķ���
        private void ImportFeatureClassToNewWorkSpace(string filename,string outfilename)
        {
            //try
            //{   
                m_success = false;//��ʼ��
                string ImportShapeFileName = filename;
                  string ExportFileShortName = outfilename;
                if (ImportShapeFileName == "") { return; }
                string ImportFileShortName = System.IO.Path.GetFileNameWithoutExtension(ImportShapeFileName);
                string ImportFilePath = System.IO.Path.GetDirectoryName(ImportShapeFileName);
                
                //�򿪴��ڵĹ����ռ䣬��Ϊ����Ŀռ�
                IWorkspaceFactory Pwf = new FileGDBWorkspaceFactoryClass();
                //IWorkspace pWorkspace = Pwf.OpenFromFile(GetSourcePath(comboBoxSource.Text), 0);
               // IWorkspace2 pWorkspace2 =(IWorkspace2)(Pwf.OpenFromFile(GetSourcePath(comboBoxSource.Text), 0));
                IWorkspace pWorkspace=GetWorkspace(comboBoxSource.Text);
                if (pWorkspace == null)
                {
                    m_strErr = "����Դδ�ҵ�";
                    m_success = false;
                    return;
                }
                string username = GetSourceUser(comboBoxSource.Text);
                if (username.Trim() != "")
                    ExportFileShortName = username + "." + ExportFileShortName;
                IWorkspace2 pWorkspace2 = pWorkspace as IWorkspace2;
                //�ж�Ҫ���Ƿ���ڣ������ڽ�ɾ��Դ�ļ�
                if (pWorkspace2.get_NameExists(esriDatasetType.esriDTFeatureClass,ExportFileShortName))
                {
                    if (m_newfile == true)
                    {
                        IFeatureClass tmpfeatureclass;
                        IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspace;
                        tmpfeatureclass = pFeatureWorkspace.OpenFeatureClass(ExportFileShortName);
                        IDataset set = tmpfeatureclass as IDataset;
                        fdr.DeleteSql(ExportFileShortName);
                        set.CanDelete();
                        set.Delete();
                    }
                    else
                    {
                        //MessageBox.Show("������ͬ�ļ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        m_strErr = "������ͬ�ļ���";
                        m_success = false;
                        return;
                    }

                }  
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
                if (InsertIntoDatabase(ExportFileShortName))
                {
                    pShpToClsConverter = new FeatureDataConverterClass();
                    pShpToClsConverter.ConvertFeatureClass(pInFeatureClassName, null, pOutFeatureDSName, pOutFeatureClassName, pOutGeometryDef, pOutFields, "", 1000, 0);
                    //MessageBox.Show("����ɹ�", "��ʾ");
                    m_success = true;
                }
                  
            //}
            //catch
            //{
            //    m_success = false;
            //}
        }

       
        //���mdb��Ҫ����
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
        //��mdb��Ҫ����ת�����Ƶ�GDB���ݿ���
        private void ImportMDBToGDB(string file,string outfilename)
        {

            m_success = false;//��ʼ��
            try
            {
                string filepath = file.Substring(0, file.LastIndexOf("---"));
                string filename = file.Substring(file.LastIndexOf("-") + 1);

                //��mdb�ļ����ڵĹ����ռ�
                ESRI.ArcGIS.Geodatabase.IWorkspaceFactory wf = new AccessWorkspaceFactory();
                IFeatureWorkspace pFeatureWorkspaceMDB = wf.OpenFromFile(@filepath, 0) as IFeatureWorkspace;
                IWorkspace pWorkspaceMDB = pFeatureWorkspaceMDB as IWorkspace;
                // ����Դ�����ռ�����     
                IDataset sourceWorkspaceDataset = (IDataset)pWorkspaceMDB;
                IWorkspaceName sourceWorkspaceName = (IWorkspaceName)sourceWorkspaceDataset.FullName;

                //����Դ���ݼ�����        
                //IFeatureClassName sourceFeatureClassName = serverContext.CreateObject("esriGeoDatabase.FeatureClassName") as IFeatureClassName;
                IFeatureClassName sourceFeatureClassName = new FeatureClassNameClass();
                IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureClassName;
                sourceDatasetName.WorkspaceName = sourceWorkspaceName;
                sourceDatasetName.Name = filename;

                //�򿪴��ڵĹ����ռ䣬��Ϊ����Ŀռ�
                IWorkspaceFactory Pwf = new FileGDBWorkspaceFactoryClass();
                //IWorkspace pWorkspaceGDB = Pwf.OpenFromFile(GetSourcePath(comboBoxSource.Text), 0);
                //IWorkspace2 pWorkspace2 = (IWorkspace2)(Pwf.OpenFromFile(GetSourcePath(comboBoxSource.Text), 0));
                IWorkspace pWorkspaceGDB =GetWorkspace(comboBoxSource.Text);
                if (pWorkspaceGDB == null)
                {
                    m_strErr = "����Դδ�ҵ�";
                    m_success = false;
                    return;
                }
                string username = GetSourceUser(comboBoxSource.Text);
                if (username.Trim() != "")
                    outfilename = username + "." + outfilename;
                IWorkspace2 pWorkspace2 = pWorkspaceGDB as IWorkspace2;
                IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceGDB;

                //����Ŀ�깤���ռ�����    
                IDataset targetWorkspaceDataset = (IDataset)pWorkspaceGDB;
                IWorkspaceName targetWorkspaceName = (IWorkspaceName)targetWorkspaceDataset.FullName;

                //����Ŀ�����ݼ�����    
                // IFeatureClassName targetFeatureClassName = serverContext.CreateObject("esriGeoDatabase.FeatureClassName") as IFeatureClassName;
                //�ж�Ҫ���Ƿ���ڣ������ڽ�ɾ��Դ�ļ�
                if (pWorkspace2.get_NameExists(esriDatasetType.esriDTFeatureClass, outfilename))
                {
                    if (m_newfile == true)
                    {
                        IFeatureClass tmpfeatureclass;
                        tmpfeatureclass = pFeatureWorkspace.OpenFeatureClass(outfilename);
                        IDataset set = tmpfeatureclass as IDataset;
                        fdr.DeleteSql(filename);
                        set.CanDelete();
                        set.Delete();
                    }
                    else
                    {
                        m_strErr = "������ͬ�ļ���";
                        m_success = false;
                        return;
                    }
                }
                IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();
                IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
                targetDatasetName.WorkspaceName = targetWorkspaceName;
                targetDatasetName.Name = outfilename;

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
                fieldChecker.InputWorkspace = pWorkspaceMDB;
                fieldChecker.ValidateWorkspace = pWorkspaceGDB;
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
                        if (InsertIntoDatabase(outfilename))//�������ݿ�
                        {
                            IFeatureDataConverter fctofc = new FeatureDataConverterClass();
                            IEnumInvalidObject enumErrors = fctofc.ConvertFeatureClass(sourceFeatureClassName, queryFilter, null, targetFeatureClassName, geometryDef, targetFeatureClassFields, "", 1000, 0);
                        }
                    }

                }
                m_success = true;
            }
            catch { m_success = false; m_strErr = ""; }
        }

          
        private void checkboxPf_CheckedChanged(object sender, EventArgs e)
        {
            if (checkboxPf.Checked == true)
            {
                textBox.Enabled = true;
                btnSelect.Enabled = true;
            }
            else
            {
                textBox.Text = "";
                textBox.Enabled = false;
                btnSelect.Enabled = false;
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            string name;string strName="";
            foreach (ListViewItem item in listView.Items)
            {
                string[] str = item.Text.Split('.');
                str[1] = str[1].Substring(0, 3);
                if (str[1].ToLower() == "shp")
                {
                    name = System.IO.Path.GetFileNameWithoutExtension(item.Text);
                    item.SubItems[1].Text = textBox.Text + name.ToUpper();
                }
                else if (str[1].ToLower() == "mdb")
                {
                    name = item.Text.Substring(item.Text.LastIndexOf("-") + 1);
                    if (textBox.Text.Trim() != "")
                        strName = GetForwadName(textBox.Text.ToUpper(), name);
                    else
                        strName = name;
                    item.SubItems[1].Text = strName.ToUpper();
                }
                
                
            }
        }
        /// <summary>
        /// �����ݷ������ַ�������
        /// </summary>
        /// <param name="filename">��������</param>
        public void AnalyseDataToArray(string filename)
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select �ֶ����� from ͼ�����������";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strname = db.GetInfoFromMdbByExp(strCon, strExp);
            string[] arrName = strname.Split('+');//�����ֶ�����
            for (int i = 0; i < arrName.Length; i++)
            {
                switch (arrName[i])
                {
                    case "ҵ��������":
                        array[0] = filename.Substring(0, 2);//ҵ��������
                        filename = filename.Remove(0, 2);
                        break;
                    case "���":
                        array[1] = filename.Substring(0, 4);//���
                        filename = filename.Remove(0, 4);
                        break;
                    case "ҵ��С�����":
                        array[2] = filename.Substring(0, 2);//ҵ��С�����
                        filename = filename.Remove(0, 2);
                        break;
                    case "��������":
                        array[3] = filename.Substring(0, 6);//��������
                        filename = filename.Remove(0, 6);
                        break;
                    case "������":
                        array[4] = filename.Substring(0, 1);//������
                        filename = filename.Remove(0, 1);
                        break;
                }
            }
        }

        public string GetDescrib(string str)
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select ���� from ��׼ͼ����Ϣ�� where ����='"+str+"'";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strreturn=db.GetInfoFromMdbByExp(strCon, strExp);
            return strreturn;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //if (!Directory.Exists(@GetSourcePath(comboBoxSource.Text)))
            //{
            //    MessageBox.Show("����Դ·��������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
            OpenFile = new OpenFileDialog();
            OpenFile.Filter = "SHP����|*.shp|MDB����|*.mdb;";
            OpenFile.Multiselect = true;  
             
            //��SHP�ļ�
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;

                foreach (string file in OpenFile.FileNames)
                {
                    for (int j = 0; j < i; j++)
                    {
                        string strExist = listView.Items[j].Text.Trim();
                        if (strExist.Contains("---"))
                        {
                            strExist = strExist.Substring(0, strExist.LastIndexOf("---"));
                        }
                        if (strExist.CompareTo(file) == 0)
                        {
                            MessageBox.Show("�ļ��Ѵ������б���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Cursor = Cursors.Default;
                            return;
                        }

                    }
                    //string name = file.Substring(file.LastIndexOf('\\') + 1, file.Length - file.LastIndexOf('\\') - 1);
                    string[] str = file.Split('.');
                    if (str[1].ToLower() == "shp")
                    {
                        listView.Items.Add(file);
                        string name1 = System.IO.Path.GetFileNameWithoutExtension(file).ToUpper();
                        if (textBox.Text != "")
                        {
                            string strName = GetForwadName(textBox.Text.ToUpper(), name1);
                            listView.Items[i].SubItems.Add(strName);
                        }
                        else
                            listView.Items[i].SubItems.Add(name1);
                        if (name1.Length > 15)
                            name1=name1.Substring(15);
                        string strdescri = GetDescrib(name1.ToUpper());
                        if (strdescri.Trim() != "")
                        {
                            listView.Items[i].SubItems.Add(strdescri);
                        }
                        else
                        {
                            listView.Items[i].SubItems.Add("��Ҫ����");
                        }
                        listView.Items[i].SubItems.Add("�ȴ����");
                         listView.Items[i].Checked = true;
                        i++;
                    }
                    else if (str[1].ToLower() == "mdb")
                    {
                        ESRI.ArcGIS.Geodatabase.IWorkspaceFactory wf = new AccessWorkspaceFactory();
                        IFeatureWorkspace pFeatureWorkspaceMDB = wf.OpenFromFile(@file, 0) as IFeatureWorkspace;
                        IWorkspace pWorkspaceMDB = pFeatureWorkspaceMDB as IWorkspace;
                        List<string> list = Getfeatureclass(pWorkspaceMDB);
                        for (int ii = 0; ii < list.Count; ii++)
                        {
                            listView.Items.Add(file+"---"+list[ii]);
                            if (textBox.Text != "")
                            {
                                string strName = GetForwadName(textBox.Text.ToUpper(), list[ii].ToUpper());
                                listView.Items[i].SubItems.Add(strName);

                            }
                            else
                                listView.Items[i].SubItems.Add(list[ii].ToUpper());
                            if (list[ii].Length> 15)
                                list[ii]=list[ii].Substring(15);
                            string strdescri = GetDescrib(list[ii].ToUpper());
                            if (strdescri.Trim() != "")
                                listView.Items[i].SubItems.Add(strdescri);
                            else
                                listView.Items[i].SubItems.Add("��Ҫ����");    
                            listView.Items[i].SubItems.Add("�ȴ����");
                            listView.Items[i].Checked = true;
                            i++;
                        }
                    }
                }
                this.Cursor = Cursors.Default;
            }
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
            str1 = "";
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select ҵ��������,ҵ��С����� from ��׼ͼ����Ϣ�� where ����='" + str2 + "'";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
            if (dt.Rows.Count > 0)
            {
                array[0] = dt.Rows[0]["ҵ��������"].ToString();
                array[2] = dt.Rows[0]["ҵ��С�����"].ToString();
            }
            strExp = "select �ֶ����� from ͼ�����������";
            string strname = db.GetInfoFromMdbByExp(strCon, strExp);
            string[] arrName = strname.Split('+');//�����ֶ�����
            for (int i = 0; i < arrName.Length; i++)
            {

                switch (arrName[i])
                {
                    case "ҵ��������":
                        str1 += array[0];
                        break;
                    case "���":
                        str1 += array[1];
                        break;
                    case "ҵ��С�����":
                        str1 += array[2];
                        break;
                    case "��������":
                        str1 += array[3];
                        break;
                    case "������":
                        str1 += array[4];
                        break;
                }
            }
            str1 += str2;
            return str1;

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
                m_strErr = "";
                string []str=item.Text.Split('.');
                str[1] = str[1].Substring(0, 3);
                if (item.Checked&&item.SubItems[3].Text=="�ȴ����")
                { 
                    _check = true;
                    if (checkboxLegal.Checked)
                    {
                        if (item.SubItems[1].Text.Trim()!=""&&!CheckNames(item.SubItems[1].Text))
                        {
                            item.SubItems[3].Text = "����������,���ʧ��";
                            continue;
                        }
                    }
                   
                    item.SubItems[3].Text = "�������";
                    listView.Refresh();
                    if (str[1].ToLower() == "shp")
                        ImportFeatureClassToNewWorkSpace(item.Text, item.SubItems[1].Text);
                    else if (str[1].ToLower() == "mdb")
                        ImportMDBToGDB(item.Text, item.SubItems[1].Text);

                    if (m_success)
                        item.SubItems[3].Text = "������";
                    else
                    {
                        if(m_strErr!="")
                            item.SubItems[3].Text = m_strErr + ",���ʧ��";
                        else
                        item.SubItems[3].Text = "���ʧ��";
                    }
                    listView.Refresh();
                }
            }
            if (_check ==false)
                MessageBox.Show("��ѡ��Ҫ�����ļ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("���������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void checkBoxNew_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxNew.Checked == true)
                m_newfile = true;
            else
                m_newfile = false;
        }
        /// <summary>
        /// ������ƺϷ���
        /// </summary>
        /// <param name="name">��Ҫ��������</param>
        /// <returns></returns>
        public bool CheckNames(string name)
        {
            if (name.Length < 15 && name.Length > 20)
                return false;
            try
            {
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                string strExp = "select �������� from ͼ�����������";
                string strRegex = db.GetInfoFromMdbByExp(strCon, strExp);//������ʽ
                strExp = "select �ֶ����� from ͼ�����������";
                string strname = db.GetInfoFromMdbByExp(strCon, strExp);
                string[] arrRegex = strRegex.Split('(', ')');//����������ʽ
                string[] arrName = strname.Split('+');//�����ֶ�����
                Regex regex;
                for (int i = 0; i < arrName.Length; i++)
                {
                    regex = new Regex(arrRegex[2 * i + 1]);
                    switch (arrName[i])
                    {
                        case "ҵ��������":
                            if (!regex.IsMatch(name.Substring(0, 2)))//ƥ��ҵ��������
                            {
                                return false;
                            }
                            else name = name.Remove(0, 2);
                            break;
                        case "���":
                            if (!regex.IsMatch(name.Substring(0, 4)))//ƥ�����
                            {
                                return false;
                            }
                            else name = name.Remove(0, 4);
                            break;
                        case "ҵ��С�����":
                            if (!regex.IsMatch(name.Substring(0, 2)))//ƥ��ҵ��С�����
                            {
                                return false;
                            }
                            else name = name.Remove(0, 2);
                            break;
                        case "��������":
                            if (!regex.IsMatch(name.Substring(0, 6)))//ƥ����������
                            {
                                return false;
                            }
                            else name = name.Remove(0, 6);
                            break;
                        case "������":
                            if (!regex.IsMatch(name.Substring(0, 1)))//ƥ���б�����
                            {
                                return false;
                            }
                            else name = name.Remove(0, 1);
                            break;
                    }

                }
                return true;
            }
            catch
            {
                return false;
            }

        }
        private void btnServer_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FolderBrowser = new FolderBrowserDialog();
            if (FolderBrowser.ShowDialog() == DialogResult.OK)
            {
                if (!FolderBrowser.SelectedPath.Contains(".gdb"))
                {
                    MessageBox.Show("��ѡ��File Geodatabase���ݿ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                comboBoxSource.Text = FolderBrowser.SelectedPath;
            }
        }

        //д�����ݱ����͵�ͼ�����Ϣ��ķ���
        public bool InsertIntoDatabase(string filename)
        {
            bool success;
            try
            {
                if (filename.Contains("."))
                    filename = filename.Substring(filename.LastIndexOf(".") + 1);//���SDE
                if (filename.Length > 16)
                {
                    GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                    string mypath = dIndex.GetDbInfo();
                    string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                     string strExp = "select �ֶ����� from ͼ�����������";
                    GeoDataCenterDbFun db=new GeoDataCenterDbFun();
                    string strname = db.GetInfoFromMdbByExp(strCon, strExp);
                     string[] arrName = strname.Split('+');//�����ֶ�����
                     for (int i = 0; i < arrName.Length; i++)
                     {
                         switch (arrName[i])
                         {
                             case "ҵ��������":
                                 array[0] = filename.Substring(0, 2);//ҵ��������
                                 filename = filename.Remove(0, 2);
                                 break;
                             case "���":
                                 array[1] = filename.Substring(0, 4);//���
                                 filename = filename.Remove(0, 4);
                                 break;
                             case "ҵ��С�����":
                                 array[2] = filename.Substring(0, 2);//ר��
                                 filename = filename.Remove(0, 2);
                                 break;
                             case "��������":
                                 array[3]= filename.Substring(0, 6);//��������
                                 filename = filename.Remove(0, 6);
                                 break;
                             case "������":
                                 array[4]= filename.Substring(0, 1);//������
                                 filename = filename.Remove(0, 1);
                                 break;
                         }
                     }
                    array[5] = filename;//ͼ�����
                    string sourcename = comboBoxSource.Text.Trim();
                    strExp = string.Format("select count(*) from ���ݱ���� where ҵ��������='{0}' and ���='{1}' and ҵ��С�����='{2}'and ��������='{3}' and ������='{4}' and ͼ�����='{5}' and ����Դ����='{6}'",
                    array[0], array[1], array[2], array[3], array[4], array[5],sourcename);
                    GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
                    int count = dDbFun.GetCountFromMdb(strCon, strExp);
                    if (count!= 1)
                    {
                        strExp = string.Format("insert into ���ݱ����(ҵ��������,���,ҵ��С�����,��������,������,ͼ�����,����Դ����) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                            array[0], array[1], array[2], array[3], array[4], array[5],sourcename);
                        dDbFun.ExcuteSqlFromMdb(strCon, strExp); //�������ݱ����
                        dDbFun.UpdateMdbInfoTable(array[0], array[1], array[2], array[3], array[4]);//���µ�ͼ�����Ϣ��
                    }
                    success = true;
                }
                else
                {
                    m_strErr = "����������д�����ݱ�ʧ��";
                    success = false;
                }
            }
            catch(System.Exception e)
            {
                MessageBox.Show(e.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                success = false;
            }
            return success;
        }
        //�õ�����Դ��ַ
        private string GetSourceUser(string str)
        {
            try
            {
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                string strExp = "select �û� from ��������Դ�� where ����Դ����='" + str + "'";
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                string strname = db.GetInfoFromMdbByExp(strCon, strExp);
                return strname.ToUpper();
            }
            catch { return ""; }
        }
       /// <summary>
        /// �õ����ݿ�ռ� Added by xisheng 2011.04.28
       /// </summary>
       /// <param name="str">����Դ����</param>
       /// <returns>�����ռ�</returns>
        private IWorkspace GetWorkspace(string str)
        {
            try
            {
                IWorkspace pws = null;
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                string strExp = "select * from ��������Դ�� where ����Դ����='" + str + "'";
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
                string type = dt.Rows[0]["����Դ����"].ToString();
               if (type.Trim() == "GDB")
               {
                   IWorkspaceFactory pWorkspaceFactory;
                   pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                   pws = pWorkspaceFactory.OpenFromFile(dt.Rows[0]["���ݿ�"].ToString(), 0);
               }
               else if (type.Trim() == "SDE")
               {
                   IWorkspaceFactory pWorkspaceFactory;
                   pWorkspaceFactory = new SdeWorkspaceFactoryClass();

                   //PropertySet
                   IPropertySet pPropertySet;
                   pPropertySet = new PropertySetClass();
                   pPropertySet.SetProperty("Server", dt.Rows[0]["������"].ToString());
                   pPropertySet.SetProperty("Database", dt.Rows[0]["���ݿ�"].ToString());
                   pPropertySet.SetProperty("Instance","5151");//"port:" + txtService.Text
                   pPropertySet.SetProperty("user", dt.Rows[0]["�û�"].ToString());
                   pPropertySet.SetProperty("password", dt.Rows[0]["����"].ToString());
                   pPropertySet.SetProperty("version", "sde.DEFAULT");
                   pws = pWorkspaceFactory.Open(pPropertySet, 0);
                  
               }
               return pws;
            }
            catch
            {
                return null; 
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            frmNameRule frm = new frmNameRule(textBox);
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

        //ȡ������������
        private void checkboxLegal_CheckedChanged(object sender, EventArgs e)
        {
            if (checkboxLegal.Checked == false)
            {
                foreach (ListViewItem item in listView.Items)
                {
                    if (item.SubItems[3].Text.Trim() == "����������,���ʧ��")
                    {
                        item.SubItems[3].Text = "�ȴ����";
                    }
                }

            }

        }

        //����ͼ��
        private void btnNewLayer_Click(object sender, EventArgs e)
        {
            List<string> Laylist = new List<string>();
            foreach (ListViewItem item in listView.Items)
            {
                if (item.SubItems[2].Text.Trim() != "��Ҫ����")
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
                        item.SubItems[2].Text="��Ҫ����";
                    }
                }
            }
            listView.Refresh();
        }
      
    }
}