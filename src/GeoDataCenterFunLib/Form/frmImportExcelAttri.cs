using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Carto;


namespace GeoDataCenterFunLib
{
    public partial class frmImportExcelAttri : DevComponents.DotNetBar.Office2007Form
    {
        private IFeatureClass m_pCurFeaCls = null;  //Ŀ��ͼ���Ҫ����
        private Dictionary<string, int> _DicFieds = new Dictionary<string, int>();

        public frmImportExcelAttri()
        {
            InitializeComponent();
        }
        private void InitListFields()
        {
            if (m_pCurFeaCls != null)
            {
                if (SysCommon.ModField._DicFieldName.Keys.Count == 0)
                {
                    SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");
                }

                ListFields.Items.Clear();
                _DicFieds.Clear();
                IFields pFields = m_pCurFeaCls.Fields;
                for (int i = 0; i < pFields.FieldCount; i++)
                {
                    IField pField = pFields.get_Field(i);
                    switch (pField.Type)
                    {
                        case esriFieldType.esriFieldTypeBlob:
                            continue;
                            break;
                        case esriFieldType.esriFieldTypeGeometry:
                            continue;
                            break;
                        case esriFieldType.esriFieldTypeGlobalID:
                            continue;
                            break;
                        case esriFieldType.esriFieldTypeGUID:
                            continue;
                            break;
                        case esriFieldType.esriFieldTypeOID:
                            continue;
                            break;
                        case esriFieldType.esriFieldTypeRaster:
                            continue;
                            break;
                        case esriFieldType.esriFieldTypeXML:
                            continue;
                            break;
                        default:
                            break;
                                                
                    }
                    if(pField.Name.ToUpper().StartsWith("SHAPE"))
                    {
                        continue;
                    }
                    string strChineseName = SysCommon.ModField.GetChineseNameOfField(pField.Name);
                    ListFields.Items.Add(strChineseName);
                    cmbKeyField.Items.Add(strChineseName);
                    _DicFieds.Add(strChineseName, i);
                    
                }
                for (int j = 0; j < cmbKeyField.Items.Count; j++)
                {
                    if (cmbKeyField.Items[j].ToString().ToUpper() == "С��ID")
                    {
                        cmbKeyField.SelectedIndex = j;
                    }
                }
            }
        }
        private void txtBoxLayer_Click(object sender, EventArgs e)
        {
            m_pCurFeaCls = null;
            SysCommon.SelectLayerByTree frm = new SysCommon.SelectLayerByTree(Plugin.ModuleCommon.TmpWorkSpace, Plugin.ModuleCommon.ListUserdataPriID);
            frm._LayerTreePath = System.Windows.Forms.Application.StartupPath + "\\..\\res\\xml\\��ʱͼ����.xml";
            if (frm.ShowDialog() == DialogResult.OK)
            {
                if (frm.m_NodeKey.Trim() != "")
                {
                    m_pCurFeaCls = SysCommon.ModSysSetting.GetFeatureClassByNodeKey(Plugin.ModuleCommon.TmpWorkSpace, frm._LayerTreePath, frm.m_NodeKey);

                }

                if (m_pCurFeaCls != null)
                {
                    txtBoxLayer.Text = frm.m_NodeText;
                    InitListFields();
                }
            }
            System.IO.File.Delete(frm._LayerTreePath);
            frm = null;
        }

        private void btnOpenExcel_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog sOpenFileD = new System.Windows.Forms.OpenFileDialog();
            sOpenFileD.CheckFileExists = true;
            sOpenFileD.CheckPathExists = true;
            sOpenFileD.Multiselect = false;
            sOpenFileD.Title = "ѡ������Դ";
            sOpenFileD.Filter = "Excel 97-2003 ������ (*.xls)|*.xls|Excel ������(*.xlsx)|*.xlsx";
        
            if (sOpenFileD.ShowDialog() == DialogResult.OK)
            {
                txtBoxExcelPath.Text = sOpenFileD.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ListFields.Items.Count; i++)
            {
                ListFields.SetItemChecked(i, true);
            }
        }

        private void chkBoxSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBoxSelectAll.Checked)
            {
                for (int i = 0; i < ListFields.Items.Count; i++)
                {
                    ListFields.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < ListFields.Items.Count; i++)
                {
                    ListFields.SetItemChecked(i, false);
                }
            }
        }

        private void frmImportExcelAttri_Load(object sender, EventArgs e)
        {
            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtBoxLayer.Text == "")
            {
                MessageBox.Show("��ѡ��Ŀ��ͼ��!");
                return;
            }
            if (txtBoxExcelPath.Text=="")
            {
                MessageBox.Show("��ѡ�������ҵ�����!");
                return;
            }
            if (cmbKeyField.Text == "")
            {
                MessageBox.Show("��ѡ��ƥ��Ĺؼ��ֶ�!");
                return;
            }
            if (m_pCurFeaCls != null)
            {
                List<string> ListFieldNames=new List<string>();
                for (int i = 0; i < ListFields.Items.Count; i++)
                {
                    if (ListFields.GetItemChecked(i))
                    {
                        string strChineseName = ListFields.Items[i].ToString();
                        int intFieldIndex = -1;
                        if (_DicFieds.ContainsKey(strChineseName) && strChineseName!=cmbKeyField.Text)
                        {                            
                            intFieldIndex = _DicFieds[strChineseName];
                            string strFieldName = m_pCurFeaCls.Fields.get_Field(intFieldIndex).Name;
                            ListFieldNames.Add(strFieldName);
                        }
                        
                    }
                }
                string strKeyChineseName = cmbKeyField.Text;
                string strKeyFieldName = "";
                if (_DicFieds.ContainsKey(strKeyChineseName))
                {
                    int indexKeyField =_DicFieds[strKeyChineseName];
                    strKeyFieldName = m_pCurFeaCls.Fields.get_Field(indexKeyField).Name ;
                }
                if (ListFieldNames.Count == 0)
                {
                    MessageBox.Show("��ѡ���������!");
                    return;
                }
                if (strKeyFieldName == "")
                {
                    MessageBox.Show("�Ҳ���ƥ��Ĺؼ��ֶ�!");
                    return;
                }
                if (strKeyFieldName != "")
                {
                    DoImport(m_pCurFeaCls, txtBoxExcelPath.Text, ListFieldNames, strKeyFieldName);
                }
            }
        }
        private void DoImport(IFeatureClass pFeatureClass,string strExcelPath,List<string> ListFieldNames,string strKeyFieldName)
        {
            int startrow = 1;   //�����ӵڼ��п�ʼ
            ExcelHelper elh = new ExcelHelper(strExcelPath);
            Microsoft.Office.Interop.Excel.Worksheet sheet = elh.excelApp.Worksheets[1] as Microsoft.Office.Interop.Excel.Worksheet;

            Exception er = null;//������Ϣ
            progressBarX1.Visible = true;
            progressBarX1.Maximum = sheet.UsedRange.Rows.Count + 1 - startrow;
            progressBarX1.Minimum = 1;
            progressBarX1.Value = 1;
            progressBarX1.Step = 1;
            sheet.Columns.EntireColumn.AutoFit();   //�Զ������п�  added by chulili 2012-11-29 
            ////�����㣬�����뵽��ͼ��
            //this.Invoke(new delegateWriteLog(WriteLog), "ѭ�����е��У�ִ�д���Ҫ��");
            IFeatureCursor pCursor = null;
            IQueryFilter pQueryFilter = null;
            IFeature pFeature = null;
            int i = 1;
            
            Plugin.LogTable.WriteLocalLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  ��ʼ����Ҫ��+++++++++++++++++++++++++++++++++++++++++++");

            //ѭ��������������ȡ�ؼ�����
            lblTips.Text = "���ڲ��ҹؼ����С�����";
            int Key1Index = 0; string strKey1 = ""; IField pKeyField = null;
            //int Key2Index = 0; string strKey2 = "";
            int iNullCnt = 0;   //����Ϊ�յ��ۼӸ���
            Application.DoEvents();
            for (i = 1; i <= sheet.UsedRange.Columns.Count; i++)
            {
                string strExcelName = ((Microsoft.Office.Interop.Excel.Range)sheet.UsedRange.Cells[startrow, i]).Text.ToString();//����
                if (strExcelName == "")
                {
                    iNullCnt = iNullCnt + 1;
                    if (iNullCnt > 10)  //����Ϊ�յģ���������10����������ѭ��
                    {
                        break;
                    }
                    continue;
                }
                else
                {
                    iNullCnt = 0;
                }
                if (strKeyFieldName != "" && strKeyFieldName.ToUpper() == strExcelName.ToUpper())
                {
                    Key1Index = i;
                    if (pFeatureClass.FindField(strKeyFieldName) == -1)//���Ҫ�����в�����
                        Key1Index = 0;
                }
                if (Key1Index != 0) break;
            }
            if (Key1Index == 0)
            {
                lblTips.Text = "Excel�ļ��л���ͼ����δ�ҵ��ؼ�����";
                Application.DoEvents();
                return;
            }
            int iNullRowCnt = 0;
            for (i = startrow + 1; i <= sheet.UsedRange.Rows.Count; i++)
            {
                er = null;

                lblTips.Text = "���ڸ��µ�" + (i - startrow).ToString() + "��Ҫ��";
                Application.DoEvents();
                string sql = "";
                if (Key1Index != 0)
                {
                    strKey1 = ((Microsoft.Office.Interop.Excel.Range)sheet.UsedRange.Cells[i, Key1Index]).Text.ToString();//�ؼ���1��ֵ
                    if (strKey1 == "")
                    {
                        iNullRowCnt = iNullRowCnt + 1;
                        if (iNullRowCnt > 10)
                        {
                            break;
                        }
                        continue;
                    }
                    else
                    {
                        iNullRowCnt = 0;
                    }
                    pKeyField = pFeatureClass.Fields.get_Field(pFeatureClass.FindField(strKeyFieldName));
                    if (pKeyField.Type == esriFieldType.esriFieldTypeString)
                    {
                        sql = strKeyFieldName + "='" + strKey1 + "' ";
                    }
                    else
                    {
                        sql = strKeyFieldName + "=" + strKey1;
                    }
                }
                pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = sql;
                pCursor = pFeatureClass.Search(pQueryFilter, false);
                pFeature = pCursor.NextFeature ();
                if (pFeature == null)
                {
                    Plugin.LogTable.WriteLocalLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ��" + (i - startrow).ToString() + "��Ҫ��δ�ҵ�");
                    progressBarX1.PerformStep();
                    continue;
                }
                iNullCnt = 0;
                for (int j = 1; j <= sheet.UsedRange.Columns.Count; j++)
                {
                    string strExcelName = ((Microsoft.Office.Interop.Excel.Range)sheet.UsedRange.Cells[startrow, j]).Text.ToString();//����
                    if (strExcelName == "")
                    {
                        iNullCnt = iNullCnt + 1;
                        if (iNullCnt > 10)  //����Ϊ�յģ���������10����������ѭ��
                        {
                            break;
                        }
                        continue;
                    }
                    else
                    {
                        iNullCnt = 0;
                    }
                    strExcelName = strExcelName.Replace("\n", "");
                    strExcelName = strExcelName.Replace(" ", "");
                    if (!ListFieldNames.Contains(strExcelName) && !ListFieldNames.Contains(strExcelName.ToUpper())) continue;
                    string strName = strExcelName;
                    object objtext = ((Microsoft.Office.Interop.Excel.Range)sheet.UsedRange.Cells[i, j]).Text;//��Ӧ�еĸ��е�ֵ
                    string strtext = objtext.ToString();
                    strtext = strtext.Replace("\0", "");
                    
                    if (strName == "") continue;

                    if (strtext != "")
                    {
                        objtext = strtext as object;
                    }
                    else
                    {
                        objtext = null;
                    }


                    try
                    {
                        if (objtext != null)
                        {
                            pFeature.set_Value(pFeatureClass.Fields.FindField(strName), objtext);
                        }
                        else
                        {
                            pFeature.set_Value(pFeatureClass.Fields.FindField(strName), DBNull.Value as object);
                        }

                    }
                    catch (Exception err)
                    {
                        er = err;
                        Plugin.LogTable.WriteLocalLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  ������" + (i - startrow) + "��Ҫ��ʧ��,�к�:" + i + ",EXCEL����:" + strExcelName + ",���ݿ�������:" + strName + "����ֵΪ:" + objtext + ",�쳣��Ϣ��" + err.Message);
                        j = sheet.UsedRange.Columns.Count;
                        break;
                    }

                }
                if (er == null)
                {
                    pFeature.Store();

                }
                progressBarX1.PerformStep();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
                pCursor = null;
            }

            sheet = null;            
            this.progressBarX1.Visible = false;
            elh.Close();
            lblTips.Text = "�������!";
            Plugin.LogTable.WriteLocalLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  ����Ҫ�����+++++++++++++++++++++++++++++++++++++++++++");
            Plugin.LogTable.LocalLogClose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}