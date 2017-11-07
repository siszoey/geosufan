using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OleDb;

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

//*********************************************************************************
//** �ļ�����frmDataReduction.cs
//** CopyRight (c) �人������Ϣ���̼������޹�˾�����������
//** �����ˣ�ϯʤ
//** ��  �ڣ�20011-03-10
//** �޸��ˣ�
//** ��  �ڣ�
//** ��  ����
//**
//** ��  ����1.0
//*********************************************************************************
namespace GeoDataCenterFunLib
{
    public partial class frmDataReduction : DevComponents.DotNetBar.Office2007Form
    {
        public frmDataReduction()
        {
            InitializeComponent();
        }
        IWorkspace pWorkspace;
        IWorkspace2 pWorkspace2;
        string m_startstr;//�б��б༭ǰ����
        string m_endstr;//�༭������
        string[] array = new string[6];//�������ݳ�������ʽ
        bool m_first;//�Ƿ��һ�μ����б��
        int [] m_state={0,0,0,0,0};//4��ѡ���б��ѡ��״̬
        public static TreeNode Node;//������̬������ô����ݵ�Ԫ���Ľڵ�

        private void frmDataReduction_Load(object sender, EventArgs e)
        {
            m_first = true;
            //��ʼ��������
            SysCommon.CProgress vProgress = new SysCommon.CProgress("���ڼ�������");
            vProgress.EnableCancel = false;
            vProgress.ShowDescription = true;
            vProgress.FakeProgress = true;
            vProgress.TopMost = true;
            vProgress.ShowProgress();
            LoadGridView(vProgress);
            vProgress.Close();
            this.Activate();
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            //listBoxDetail.Items.Clear();
            datagwSource.Rows.Clear();
        }
        //�޸�
        private void datagwSource_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1||e.ColumnIndex!=1)
                return;
            //IWorkspaceFactory Pwf = new FileGDBWorkspaceFactoryClass();
            //pWorkspace = (IWorkspace)(Pwf.OpenFromFile(GetSourcePath(comboBoxSource.Text), 0));
            pWorkspace = GetWorkspace(comboBoxSource.Text);
            if (pWorkspace != null)
            {
                pWorkspace2 = (IWorkspace2)pWorkspace;
                m_endstr = datagwSource.Rows[e.RowIndex].Cells[1].Value.ToString();
                if (pWorkspace2.get_NameExists(esriDatasetType.esriDTFeatureClass, m_endstr))
                {
                    MessageBox.Show("���������Ѵ���,���޸�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    datagwSource.Rows[e.RowIndex].Cells[1].Value = m_startstr;
                    return;
                }
                if (pWorkspace2.get_NameExists(esriDatasetType.esriDTFeatureClass, m_startstr))
                {
                    IFeatureClass tmpfeatureclass;
                    IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspace;
                    tmpfeatureclass = pFeatureWorkspace.OpenFeatureClass(m_startstr);
                    IDataset set = tmpfeatureclass as IDataset;
                    set.Rename(m_endstr);
                    EditSql(m_startstr, m_endstr);

                    //���Ĵ��� ʵʱ����ͼ������
                    GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                    string mypath = dIndex.GetDbInfo();
                    string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                    string player = m_endstr.Substring(15);//ͼ�����
                    string strExp = "select ���� from ��׼ͼ����Ϣ�� where ����='" + player + "'";
                    GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                    string playername = db.GetInfoFromMdbByExp(strCon, strExp);
                    if (playername != "")
                        datagwSource.Rows[e.RowIndex].Cells[2].Value = playername;

                    //listBoxDetail.Items.Add("��" + m_startstr);
                    //listBoxDetail.Items.Add("��Ϊ" + m_endstr);
                    //listBoxDetail.Items.Add(" ");
                    MessageBox.Show("�޸����ݳɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void datagwSource_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                
                if(e.ColumnIndex==1)
                m_startstr = datagwSource.Rows[e.RowIndex].Cells[1].Value.ToString();
            }
            catch { }
        }

        private void ɾ��ѡ����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool flag = false;
            foreach (DataGridViewRow row in datagwSource.Rows)
            {
                if ((bool)row.Cells[0].EditedFormattedValue == true)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                MessageBox.Show("û��ѡ���У��޷�ɾ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;             
            }
            DialogResult result=MessageBox.Show("�Ƿ�ȷ��ɾ��!","��ʾ",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if(result==DialogResult.Yes)
            {
                foreach (DataGridViewRow row in datagwSource.Rows)
                {
                    if ((bool)row.Cells[0].EditedFormattedValue == true)
                    {
                        //IWorkspaceFactory Pwf = new FileGDBWorkspaceFactoryClass();
                        //pWorkspace = (IWorkspace)(Pwf.OpenFromFile(GetSourcePath(comboBoxSource.Text), 0));
                        pWorkspace = GetWorkspace(comboBoxSource.Text);
                        if (pWorkspace != null)
                        {
                            pWorkspace2 = (IWorkspace2)pWorkspace;
                            if (pWorkspace2.get_NameExists(esriDatasetType.esriDTFeatureClass, row.Cells[1].Value.ToString().Trim()))
                            {
                                IFeatureClass tmpfeatureclass;
                                IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspace;
                                tmpfeatureclass = pFeatureWorkspace.OpenFeatureClass(row.Cells[1].Value.ToString().Trim());
                                IDataset set = tmpfeatureclass as IDataset;
                                set.CanDelete();
                                set.Delete();
                                
                                //listBoxDetail.Items.Add("ɾ����" + row.Cells[1].Value + "����");
                                //listBoxDetail.Items.Add(" ");
                                //listBoxDetail.Refresh();
                            }
                        }
                        DeleteSql(row.Cells[1].Value.ToString());
                      
                    }
                }
                datagwSource.Rows.Clear();
                ChangeGridView();//���¼�������
                    MessageBox.Show("ɾ�����ݳɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
        }

        private void btn_Detail_Click(object sender, EventArgs e)
        {
            //if (this.panelDetail.Visible)
            //{
            //    this.panelDetail.Visible = false;
            //    btn_Detail.Text = "��ϸ��Ϣ";
            //    btn_Clear.Visible = false;
            //    this.Width = 384;
            //}
            //else
            //{
            //    this.panelDetail.Visible = true;
            //    btn_Detail.Text = "����";
            //    btn_Clear.Visible = true;
            //    this.Width = 580;
            //}
        }

        private void �޸�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (datagwSource.SelectedRows.Count == 1)
            {
                this.datagwSource.CurrentCell = this.datagwSource.SelectedRows[0].Cells[1];//��ȡ��ǰ��Ԫ��
                this.datagwSource.BeginEdit(true);//����Ԫ����Ϊ�༭״̬
            }
            else
            {
                MessageBox.Show("��ѡ��һ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }


        /// <summary>
        /// �����ݷ������ַ�������
        /// </summary>
        /// <param name="filename">��������</param>
        public void AnalyseDataToArray(string filename)
        {
            if (filename.Contains("."))//���SDE �û���.ͼ������ʽ��
                filename = filename.Substring(filename.LastIndexOf(".")+1);
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
            array[5] = filename;//ͼ�����
        }

        //ɾ�����ݿ�������
        public void DeleteSql(string data)
        {
            try
            {
                AnalyseDataToArray(data);
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                string strExp = string.Format("delete from ���ݱ���� where ҵ��������='{0}' and ���='{1}' and ҵ��С�����='{2}'and ��������='{3}' and ������='{4}' and ͼ�����='{5}' and ����Դ����='{6}'",
                    array[0], array[1], array[2], array[3], array[4], array[5],comboBoxSource.Text.Trim());
                GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
                dDbFun.ExcuteSqlFromMdb(strCon, strExp);
               
                //�����ݱ������������Ϣ��
                dDbFun.UpdateMdbInfoTable(array[0], array[1], array[2], array[3], array[4]);
                
                //�Ӵ�����������Ϣ��
                   #region
                //strExp = "select ͼ����� from ��ͼ�����Ϣ�� where ҵ��������='" + array[0] + "' And �������� ='" + array[3] + "' And ���='" +
                //    array[1] + "'And  ������='" + array[4] + "'And ҵ��С�����='" + array[2] + "'";
                //string layers = dDbFun.GetInfoFromMdbByExp(strCon, strExp);
                //if (!layers.Contains('/'.ToString()))
                //{
                //    if (layers.Trim() != array[5])
                //        return;
                //    else
                //        layers = "";
                //}
                //else
                //{
                //    string[] layer = layers.Split('/');
                //    for (int i = 0; i < layer.Length; i++)
                //    {
                //        if (layer[i].Trim() == array[5])
                //        {
                //            if (i == 0)
                //            {
                //                layers = layers.Substring(array[5].Length + 1);
                //            }
                //            else
                //                layers = layers.Replace('/' + layer[i], "");
                //        }
                //    }
                //}
                //strExp = "update ��ͼ�����Ϣ�� set ͼ�����='" + layers + "' where ҵ��������='" + array[0] + "' And �������� ='" + array[3] + "' And ���='" +
                //    array[1] + "'And  ������='" + array[4] + "'And ҵ��С�����='" + array[2] + "'";
                //dDbFun.ExcuteSqlFromMdb(strCon, strExp);
                #endregion
            }
            catch(System.Exception e)
            {
              //  MessageBox.Show(e.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //�޸����ݿ�������
        public void EditSql(string data1, string data2)
        {
            try
            {
                AnalyseDataToArray(data1);
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                string strExp = string.Format("select ID from ���ݱ���� where ҵ��������='{0}' and ���='{1}' and ҵ��С�����='{2}'and ��������='{3}' and ������='{4}' and ͼ�����='{5}' and ����Դ����='6'",
                    array[0], array[1], array[2], array[3], array[4], array[5],comboBoxSource.Text.Trim());
                GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
                int id1 = dDbFun.GetIDFromMdb(strCon, strExp);
                AnalyseDataToArray(data2);
                if (id1 != 0)
                    strExp = string.Format("update ���ݱ���� set ҵ��������='{0}',���='{1}',ҵ��С�����='{2}',��������='{3}',������='{4}',ͼ�����='{5}' where ID={6}",
                        array[0], array[1], array[2], array[3], array[4], array[5], id1);
                dDbFun.ExcuteSqlFromMdb(strCon, strExp);//�������ݱ����
                dDbFun.UpdateMdbInfoTable(array[0], array[1], array[2], array[3], array[4]);//���µ�ͼ�����Ϣ��
              
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void btn_Edit_Click(object sender, EventArgs e)
        {
            this.�޸�ToolStripMenuItem_Click(sender, e);
        }

        private void btn_Del_Click(object sender, EventArgs e)
        {
            this.ɾ��ѡ����ToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// ��һ��Ҫ�����һ�������ռ�ת�Ƶ�����һ�������ռ�
        /// ע��Ŀ�깤���ռ䲻���и�Ҫ���࣬���������  
        /// </summary>
        /// <param name="sourceWorkspace">Դ�����ռ�</param>
        /// <param name="targetWorkspace">Ŀ�깤���ռ�</param>
        /// <param name="nameOfSourceFeatureClass">ԴҪ������</param>
        /// <param name="nameOfTargetFeatureClass">Ŀ��Ҫ������</param>
        public void IFeatureDataConverter_ConvertFeatureClass(IWorkspace sourceWorkspace, IWorkspace targetWorkspace, string nameOfSourceFeatureClass, string nameOfTargetFeatureClass)
        {
            //create source workspace name   
            IDataset sourceWorkspaceDataset = (IDataset)sourceWorkspace;
            IWorkspaceName sourceWorkspaceName = (IWorkspaceName)sourceWorkspaceDataset.FullName;
            //create source dataset name   
            IFeatureClassName sourceFeatureClassName = new FeatureClassNameClass();
            IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureClassName;
            sourceDatasetName.WorkspaceName = sourceWorkspaceName;
            sourceDatasetName.Name = nameOfSourceFeatureClass;
            //create target workspace name   
            IDataset targetWorkspaceDataset = (IDataset)targetWorkspace;
            IWorkspaceName targetWorkspaceName = (IWorkspaceName)targetWorkspaceDataset.FullName;
            //create target dataset name   
            IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();
            IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
            targetDatasetName.WorkspaceName = targetWorkspaceName;
            targetDatasetName.Name = nameOfTargetFeatureClass;
            //Open input Featureclass to get field definitions.   
            ESRI.ArcGIS.esriSystem.IName sourceName = (ESRI.ArcGIS.esriSystem.IName)sourceFeatureClassName;
            IFeatureClass sourceFeatureClass = (IFeatureClass)sourceName.Open();
            //Validate the field names because you are converting between different workspace types.   
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IFields targetFeatureClassFields;
            IFields sourceFeatureClassFields = sourceFeatureClass.Fields;
            IEnumFieldError enumFieldError;
            // Most importantly set the input and validate workspaces!     
            fieldChecker.InputWorkspace = sourceWorkspace;
            fieldChecker.ValidateWorkspace = targetWorkspace;
            fieldChecker.Validate(sourceFeatureClassFields, out enumFieldError, out targetFeatureClassFields);
            // Loop through the output fields to find the geomerty field   
            IField geometryField;
            for (int i = 0; i < targetFeatureClassFields.FieldCount; i++)
            {
                if (targetFeatureClassFields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                {
                    geometryField = targetFeatureClassFields.get_Field(i);
                    // Get the geometry field's geometry defenition            
                    IGeometryDef geometryDef = geometryField.GeometryDef;
                    //Give the geometry definition a spatial index grid count and grid size        
                    IGeometryDefEdit targetFCGeoDefEdit = (IGeometryDefEdit)geometryDef;
                    targetFCGeoDefEdit.GridCount_2 = 1;
                    targetFCGeoDefEdit.set_GridSize(0, 0);
                    //Allow ArcGIS to determine a valid grid size for the data loaded      
                    targetFCGeoDefEdit.SpatialReference_2 = geometryField.GeometryDef.SpatialReference;
                    // we want to convert all of the features   
                    IQueryFilter queryFilter = new QueryFilterClass();
                    queryFilter.WhereClause = "";
                    // Load the feature class     
                    IFeatureDataConverter fctofc = new FeatureDataConverterClass();
                    IEnumInvalidObject enumErrors = fctofc.ConvertFeatureClass(sourceFeatureClassName, queryFilter, null, targetFeatureClassName, geometryDef, targetFeatureClassFields, "", 1000, 0);
                    break;
                }
            }
        }
        //����ΪMDB�ļ�
        private void btn_ExportNDB_Click(object sender, EventArgs e)
        {
            pWorkspace = GetWorkspace(comboBoxSource.Text);
            if (pWorkspace==null)
            {
                MessageBox.Show("����Դ�ռ䲻����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool flag = false;
            //��ȡģ��·��
            string sourcefilename = Application.StartupPath + "\\..\\Template\\DATATEMPLATE.mdb";
            foreach (DataGridViewRow row in datagwSource.Rows)
            {
                if ((bool)row.Cells[0].EditedFormattedValue == true)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                MessageBox.Show("û��ѡ����!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SysCommon.CProgress vProgress = new SysCommon.CProgress("�����������ݣ����Ժ�");
            try
            {
                if (File.Exists(sourcefilename))//ԭģ�����
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.Filter = "MDB����|*.mdb";
                    dlg.OverwritePrompt =false;
                    dlg.Title = "���浽MDB";


                    DialogResult result = MessageBox.Show("�����Ƿ�ȥ��ǰ׺��", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {

                        //��ʼ��������
                        
                        vProgress.EnableCancel = false;
                        vProgress.ShowDescription = false;
                        vProgress.FakeProgress = true;
                        vProgress.TopMost = true;
                        vProgress.ShowProgress();
                        Application.DoEvents();
                        //IWorkspaceFactory pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                        //pWorkspace = (IWorkspace)(pWorkspaceFactory.OpenFromFile(GetSourcePath(comboBoxSource.Text), 0));
                        //�������mdb,�滻�ļ�������ģ�嵽ָ��·��
                        //�������mdb�����滻����׷�ӵ�����ļ�
                        File.Copy(sourcefilename, dlg.FileName, true);
                        string cellvalue = "";
                        IWorkspaceFactory Pwf = new AccessWorkspaceFactoryClass();
                        IWorkspace pws = (IWorkspace)(Pwf.OpenFromFile(dlg.FileName, 0));
                        IWorkspace2 pws2 = (IWorkspace2)pws;
                        foreach (DataGridViewRow row in datagwSource.Rows)
                        {
                            if ((bool)row.Cells[0].EditedFormattedValue == true)
                            {
                                
                                cellvalue = row.Cells[1].Value.ToString().Trim();
                                if (cellvalue.Contains("."))
                                {
                                    cellvalue = cellvalue.Substring(cellvalue.LastIndexOf(".") + 1);
                                }
                                if (result == DialogResult.Yes) cellvalue = cellvalue.Substring(15);//ȥ��ǰ׺
                                pws2 = (IWorkspace2)pws;
                                if (pws2.get_NameExists(esriDatasetType.esriDTFeatureClass, cellvalue))
                                {
                                    IFeatureClass tmpfeatureclass;
                                    IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pws;
                                    tmpfeatureclass = pFeatureWorkspace.OpenFeatureClass(cellvalue);
                                    IDataset set = tmpfeatureclass as IDataset;
                                    set.CanDelete();
                                    set.Delete();
                                    flag = true;
                                }
                                IFeatureDataConverter_ConvertFeatureClass(pWorkspace, pws, row.Cells[1].Value.ToString().Trim(), cellvalue);
                            }
                        }
                        vProgress.Close();
                        MessageBox.Show("���سɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Activate();
                    }
                }
            }
            catch (Exception ex)
            {
                vProgress.Close();
                MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Activate();
            }

        }

        //�����б��
        private void LoadCombox()
        {
           GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
           string mypath = dIndex.GetDbInfo();
           string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
           string str_Exp = "select ��� from ���ݱ����";
           GeoDataCenterDbFun dbfun = new GeoDataCenterDbFun();
           List<string> list = dbfun.GetDataReaderFromMdb(strCon, str_Exp);
           comboBoxYear.Items.Add("�������");
           for (int i = 0; i < list.Count; i++)
           {
               if (!comboBoxYear.Items.Contains(list[i]))
                   comboBoxYear.Items.Add(list[i]);

           }
           if (comboBoxYear.Items.Count > 0)
               comboBoxYear.SelectedIndex = 0;
           //str_Exp = "select ��������,�������� from ���ݵ�Ԫ��";
           //DataTable dt = dbfun.GetDataTableFromMdb(strCon, str_Exp);
           //comboBoxArea.Items.Add("����������");
           //for (int i = 0; i < dt.Rows.Count; i++)
           //{
           //    comboBoxArea.Items.Add(dt.Rows[i]["��������"]+"("+dt.Rows[i]["��������"]+")");
           //}
           //if (comboBoxArea.Items.Count > 0)
           //    comboBoxArea.SelectedIndex = 0;
           str_Exp = "select ����,���� from �����ߴ����";
           DataTable dt = dbfun.GetDataTableFromMdb(strCon, str_Exp);
           comboBoxScale.Items.Add("���б�����");
           for (int i = 0; i < dt.Rows.Count; i++)
           {
               comboBoxScale.Items.Add(dt.Rows[i]["����"]+"("+dt.Rows[i]["����"]+")");
           }
           if (comboBoxScale.Items.Count > 0)
               comboBoxScale.SelectedIndex = 0;
           str_Exp = "select ����,���� from ҵ���������";
           dt= dbfun.GetDataTableFromMdb(strCon, str_Exp);
           comboBoxBig.Items.Add("���д���ҵ��");
           for (int i = 0; i <dt.Rows.Count; i++)
           {
               comboBoxBig.Items.Add(dt.Rows[i]["����"]+"("+dt.Rows[i]["����"]+")");
           }
           if (comboBoxBig.Items.Count > 0)
               comboBoxBig.SelectedIndex = 0;
           //str_Exp = "select ����,ҵ��С����� from ҵ��С����Ϣ��";
           //dt = dbfun.GetDataTableFromMdb(strCon, str_Exp);
           comboBoxSub.Items.Add("����С��ҵ��");
           //for (int i = 0; i < dt.Rows.Count; i++)
           //{
           //    comboBoxSub.Items.Add(dt.Rows[i]["����"] + "(" + dt.Rows[i]["ҵ��С�����"] + ")");
           //}
           if (comboBoxSub.Items.Count > 0)
               comboBoxSub.SelectedIndex = 0;

        }
        private void comboBoxYear_Click(object sender, EventArgs e)
        {
            if (m_first)
            {
                LoadCombox();
                m_first = false;
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       void LoadGridView()
       {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string strExp = "select ����Դ���� from ��������Դ��";
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            List<string> list = db.GetDataReaderFromMdb(strCon, strExp);
            comboBoxSource.Items.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                comboBoxSource.Items.Add(list[i]);//��������Դ�б��
            }
            if (list.Count > 0)
            {
                comboBoxSource.SelectedIndex = 0;//Ĭ��ѡ���һ��
            }
            string player = "";
                string sourename =comboBoxSource.Text.Trim();
                strExp = "select * from ���ݱ���� where ����Դ����='" + sourename + "'";
                DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    strExp = "select �ֶ����� from ͼ�����������";
                    string strname = db.GetInfoFromMdbByExp(strCon, strExp);
                    string[] arrName = strname.Split('+');//�����ֶ�����
                    string layername = "";
                    for (int j = 0; j < arrName.Length; j++)
                    {
                        switch (arrName[j])
                        {
                            case "ҵ��������":
                                layername += dt.Rows[i]["ҵ��������"].ToString();//ҵ��������
                                break;
                            case "���":
                                layername += dt.Rows[i]["���"].ToString();//���
                                break;
                            case "ҵ��С�����":
                                layername += dt.Rows[i]["ҵ��С�����"].ToString();//ҵ��С�����
                                break;
                            case "��������":
                                layername += dt.Rows[i]["��������"].ToString();//��������
                                break;
                            case "������":
                                layername += dt.Rows[i]["������"].ToString();//������
                                break;
                        }
                    }
                    layername += dt.Rows[i]["ͼ�����"].ToString();
                    strExp = "select ���� from ��׼ͼ����Ϣ�� where ����='" + dt.Rows[i]["ͼ�����"].ToString() + "'";
                    string playername = db.GetInfoFromMdbByExp(strCon, strExp);
                    if (playername != "")
                        player = playername;
                    string username = GetSourceUser(comboBoxSource.Text).Trim().ToUpper(); ;
                    if (username != "")
                        layername = username + "." + layername;
                    datagwSource.Rows.Add(new object[] { true, layername, player });
                }
                //IWorkspaceFactory Pwf = new FileGDBWorkspaceFactoryClass();
                //pWorkspace = (IWorkspace)(Pwf.OpenFromFile(comboBoxSource.Text, 0));
                //IEnumDataset enumDataset = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass) as IEnumDataset;
                //IDataset dataset = enumDataset.Next();
                ////����mdb��ÿһ������Ҫ����
                //while (dataset != null)
                //{

                //    IFeatureClass pFeatureClass = dataset as IFeatureClass;
                //    player = pFeatureClass.AliasName.Substring(15);
                //strExp = "select ���� from ��׼ͼ����Ϣ�� where ����='" + player + "'";
                //string playername = db.GetInfoFromMdbByExp(strCon, strExp);
                //if (playername != "")
                //    player = playername;
                //datagwSource.Rows.Add(new object[] { true, pFeatureClass.AliasName, player });
                //dataset = enumDataset.Next();
                //}
            
       }

        //��ʾȫ������
        private void LoadGridView(CProgress vprocess)//���ؼ������ݺ���
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string strExp = "select ����Դ���� from ��������Դ��";
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            List<string> list = db.GetDataReaderFromMdb(strCon, strExp);
            for (int i = 0; i < list.Count; i++)
            {
                comboBoxSource.Items.Add(list[i]);//��������Դ�б��
            }
            if (list.Count > 0)
            {
                comboBoxSource.SelectedIndex = 0;//Ĭ��ѡ���һ��
            }
            string player = "";
                string sourename=comboBoxSource.Text.Trim();
                strExp = "select * from ���ݱ���� where ����Դ����='" + sourename + "'";
                DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    strExp = "select �ֶ����� from ͼ�����������";
                    string strname = db.GetInfoFromMdbByExp(strCon, strExp);
                    string[] arrName = strname.Split('+');//�����ֶ�����
                    string layername="";
                    player = "";
                    for (int j = 0; j < arrName.Length; j++)
                    {
                        switch (arrName[j])
                        {
                            case "ҵ��������":
                                layername += dt.Rows[i]["ҵ��������"].ToString();//ҵ��������
                                break;
                            case "���":
                                layername += dt.Rows[i]["���"].ToString();//���
                                break;
                            case "ҵ��С�����":
                                layername += dt.Rows[i]["ҵ��С�����"].ToString();//ҵ��С�����
                                break;
                            case "��������":
                                layername += dt.Rows[i]["��������"].ToString();//��������
                                break;
                            case "������":
                                layername += dt.Rows[i]["������"].ToString();//������
                                break;
                        }
                    }
                    layername += dt.Rows[i]["ͼ�����"].ToString();
                    strExp = "select ���� from ��׼ͼ����Ϣ�� where ����='" + dt.Rows[i]["ͼ�����"].ToString() + "'";
                    string playername = db.GetInfoFromMdbByExp(strCon, strExp);
                    if (playername != "")
                    {
                        player = playername;
                    }
                    else
                    {
                        player = dt.Rows[i]["ͼ�����"].ToString();
                    }
                    string username=GetSourceUser(comboBoxSource.Text).Trim().ToUpper();;
                    if (username != "")
                        layername = username +"."+layername;
                    datagwSource.Rows.Add(new object[] { true, layername, player });
                }
                    //IWorkspaceFactory Pwf = new FileGDBWorkspaceFactoryClass();
                    //pWorkspace = (IWorkspace)(Pwf.OpenFromFile(comboBoxSource.Text, 0));
                    //IEnumDataset enumDataset = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass) as IEnumDataset;
                    //IDataset dataset = enumDataset.Next();
                    ////����mdb��ÿһ������Ҫ����
                    //while (dataset != null)
                    //{

                    //    IFeatureClass pFeatureClass = dataset as IFeatureClass;
                    //    player = pFeatureClass.AliasName.Substring(15);
                    //strExp = "select ���� from ��׼ͼ����Ϣ�� where ����='" + player + "'";
                    //string playername = db.GetInfoFromMdbByExp(strCon, strExp);
                    //if (playername != "")
                    //    player = playername;
                    //datagwSource.Rows.Add(new object[] { true, pFeatureClass.AliasName, player });
                    //dataset = enumDataset.Next();
                //}
            
           
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
                    pPropertySet.SetProperty("Instance", "5151");//"port:" + txtService.Text
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

       //�õ����ݿ��û�
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
                return strname;
            }
            catch { return ""; }
        }
 

        //���ѡ��״̬
        private void comboBoxYear_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (!m_first)
            {
                m_state[0] = comboBoxYear.SelectedIndex;
                ChangeGridView();
            }

        }
        //��������ֵ�����任
        private void comboBoxArea_TextChanged(object sender, EventArgs e)
        {
                ChangeGridView();
            
        }

        //������ѡ��״̬
        private void comboBoxScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_first)
            {
                m_state[2] = comboBoxScale.SelectedIndex;
                ChangeGridView();
            }
        }

        //ҵ�����ѡ��״̬
        private void comboBoxBig_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (!m_first)
            {
                m_state[3] = comboBoxBig.SelectedIndex;
                ChangeComboxSub();
                
                ChangeGridView();
            }
        }
        //ҵ��С��ѡ��״̬
        private void comboBoxSub_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (!m_first)
            {
                m_state[4] = comboBoxSub.SelectedIndex;

                ChangeGridView();
            }
        }
        //����Դѡ��
        private void comboBoxSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_first)
            {
                ChangeGridView();
            }
        }

        private void ChangeComboxSub()
        {
            comboBoxSub.Items.Clear();
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string strExp = "";
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            comboBoxSub.Items.Add("����С��ҵ��");
            if (m_state[3] != 0)
            {
                string strall = comboBoxBig.Items[m_state[3]].ToString();
                string[] BigClass = strall.Split('(', ')');
                strExp = "select ����,ҵ��С����� from ҵ��С������ where ҵ��������='" + BigClass[1] + "'";
                DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    comboBoxSub.Items.Add(dt.Rows[i]["����"] + "(" + dt.Rows[i]["ҵ��С�����"] + ")");
                }
            }
            if (comboBoxSub.Items.Count > 0)
                comboBoxSub.SelectedIndex = 0;

        }


        /// <summary>
        /// �����б��ѡ��״̬��̬��ʾGirdView
        /// </summary>
        private void ChangeGridView()
        {
            this.Cursor = Cursors.WaitCursor;
            string strall = "";
            datagwSource.Rows.Clear();
            bool state = true;
            string player = "";
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            //path = GetSourcePath(comboBoxSource.Text);
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string str_Exp = "";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            //if (Directory.Exists(@path))
            //{
                //IWorkspaceFactory Pwf = new FileGDBWorkspaceFactoryClass();
            pWorkspace = GetWorkspace(comboBoxSource.Text);
            if (pWorkspace == null)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show("����Դ�ռ䲻����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
                IEnumDataset enumDataset = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass) as IEnumDataset;
                IDataset dataset = enumDataset.Next();
                //����mdb��ÿһ������Ҫ����
                while (dataset != null)
                {
                    state = true;
                    //IFeatureClass pFeatureClass = dataset as IFeatureClass;
                    //player = pFeatureClass.AliasName;
                    player = dataset.Name;

                    if(pWorkspace.Type==esriWorkspaceType.esriRemoteDatabaseWorkspace)//���SDEȥ���û���ǰ׺
                    { 
                        Int32 userlenth=pWorkspace.ConnectionProperties.GetProperty("USER").ToString().Length;
                        player = player.Substring(userlenth + 1);
                    }

                    AnalyseDataToArray(player);//��������
                    //MessageBox.Show(array[1] + "," + comboBoxYear.Items[m_state[0]].ToString());
                    if (m_state[0] != 0 && array[1] != (comboBoxYear.Items[m_state[0]].ToString()))
                    {
                        state = false; dataset = enumDataset.Next(); continue;
                    }

                    if (Node != null && Convert.ToInt32(Node.Tag) != 0 && comboBoxArea.Text.Trim() != "����������")
                    {
                        int tag = Convert.ToInt32(Node.Tag);
                        strall = comboBoxArea.Text.ToString();
                        string[] area = strall.Split('(', ')');
                        switch (tag)
                        {
                            case 1:
                                if (array[3].Substring(0, 3) != area[1].Substring(0, 3))//ʡ���ڵ�ȡǰ��λ
                                {
                                    state = false;
                                    dataset = enumDataset.Next();
                                    continue;
                                }
                                break;
                            case 2:
                                if (array[3].Substring(0, 4) != area[1].Substring(0, 4))//�м��ڵ�ȡǰ��λ
                                {
                                    state = false;
                                    dataset = enumDataset.Next();
                                    continue;
                                }
                                break;
                            case 3:
                                if (array[3] != area[1])
                                {
                                    state = false;
                                    dataset = enumDataset.Next();
                                    continue;
                                }
                                break;
                        }

                        //if (!array[3].Contains(area[1]))
                        //{
                        //    state = false; dataset = enumDataset.Next(); continue;
                        //}
                    }
                    if (m_state[2] != 0)
                    {
                        strall = comboBoxScale.Items[m_state[2]].ToString();
                        string[] scale = strall.Split('(', ')');
                        //str_Exp = "select ���� from �����ߴ���� where ����='" + comboBoxScale.Items[m_state[2]].ToString() + "'";
                        //string scale = db.GetInfoFromMdbByExp(strCon, str_Exp);
                        if (array[4] != scale[1])
                        {
                            state = false; dataset = enumDataset.Next(); continue;
                        }
                    }
                    if (m_state[3] != 0)//ҵ�����
                    {
                        strall = comboBoxBig.Items[m_state[3]].ToString();
                        string[] type = strall.Split('(', ')');
                        //str_Exp = "select ר������ from ��׼ר����Ϣ�� where ����='" + comboBoxType.Items[m_state[3]].ToString() + "'";
                        //string type= db.GetInfoFromMdbByExp(strCon, str_Exp);
                        if (array[0] != type[1])
                        {
                            state = false; dataset = enumDataset.Next(); continue;
                        }

                    }
                    if (m_state[4] != 0)//ҵ��С��
                    {
                        strall = comboBoxSub.Items[m_state[4]].ToString();
                        string[] type = strall.Split('(', ')');
                        //str_Exp = "select ר������ from ��׼ר����Ϣ�� where ����='" + comboBoxType.Items[m_state[3]].ToString() + "'";
                        //string type= db.GetInfoFromMdbByExp(strCon, str_Exp);
                        if (array[2] != type[1])
                        {
                            state = false; dataset = enumDataset.Next(); continue;
                        }

                    }
                    str_Exp = "select ���� from ��׼ͼ����Ϣ�� where ����='" + array[5] + "'";
                    string playername = db.GetInfoFromMdbByExp(strCon, str_Exp);
                    if (playername != "")
                        array[5] = playername;
                    if (state)
                    {
                        datagwSource.Rows.Add(new object[] { true,player, array[5] });
                    }
                    dataset = enumDataset.Next();

                } this.Cursor = Cursors.Default;
            //}
            //else
            //{
            //    this.Cursor = Cursors.Default;
            //    MessageBox.Show("����Դ·��������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
        }

        //ȫѡ��ť
        private void btnAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < datagwSource.Rows.Count; i++)
            {
                this.datagwSource.Rows[i].Cells[0].Value = true;
            }

        }
        //��ѡ��ť
        private void btnInverse_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < datagwSource.Rows.Count; i++)
            {
                if ((bool)datagwSource.Rows[i].Cells[0].EditedFormattedValue == false)
                {
                    this.datagwSource.Rows[i].Cells[0].Value = true;
                    //datagwSource.Rows[i].Selected = true;
                }
                else
                {
                    this.datagwSource.Rows[i].Cells[0].Value = false;
                    //datagwSource.Rows[i].Selected = false;
                }
            }
        }
        //����������
        private void comboBoxArea_Click(object sender, EventArgs e)
        {
            GeoDataCenterDbFun dbfun = new GeoDataCenterDbFun();
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            frmDataUnitTree frm = new frmDataUnitTree();//��ʼ�����ݵ�Ԫ������
            frm.Location = new Point(this.Location.X +42, this.Location.Y + 140);
            frm.flag = 1;
            frm.ShowDialog();
            if (Node != null)//���ص�Node����NULL
            {
                if (Convert.ToInt32(Node.Tag) != 0)
                {

                    string strExp = "select �������� from ���ݵ�Ԫ�� where ��������='" + Node.Text + "'  and ���ݵ�Ԫ����='" + Node.Tag + "'";
                    string code = dbfun.GetInfoFromMdbByExp(strCon, strExp);
                    comboBoxArea.Text = Node.Text + "(" + code + ")";//Ϊ���ݵ�Ԫbox��ʾ����
                }
                else
                {
                    comboBoxArea.Text = Node.Text; //Ϊ���ݵ�Ԫbox��ʾ����
                }
            }

        }

       
    }
}