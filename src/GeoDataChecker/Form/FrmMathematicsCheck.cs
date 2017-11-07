using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GeoDataChecker
{
    /// <summary>
    /// ���Ƿ�  ���
    /// </summary>
    public partial class FrmMathematicsCheck : DevComponents.DotNetBar.Office2007Form
    {
        Plugin.Application.IAppGISRef _AppHk;
        enumErrorType _ErrorType;
        SysCommon.Gis.SysGisDataSet pGisDT = null;

        public FrmMathematicsCheck(Plugin.Application.IAppGISRef hook,enumErrorType errorType)
        {
            InitializeComponent();
           
            _AppHk= hook;
            _ErrorType = errorType;

            IntialForm();
        }

        private void IntialForm()
        {
            lstRangeName.Items.Clear();
            lstRangeName.CheckBoxes = true;

            comBoxType.Items.AddRange(new object[] { "PDB", "GDB", "SDE" });
            comBoxType.SelectedIndex = 0;
            switch (_ErrorType)
            {
                case enumErrorType.��ѧ������ȷ�Լ��:
                    txtPrj.Enabled = true;
                    btnPrj.Enabled = true;
                    txtMax.Enabled = false;
                    txtMin.Enabled = false;
                    break;
                case enumErrorType.��ֵ���:
                    txtPrj.Enabled = false ;
                    btnPrj.Enabled = false;
                    txtMax.Enabled = false;
                    txtMin.Enabled = false;
                    break;
                case enumErrorType.�߳����߼��Լ��:
                    txtPrj.Enabled = false;
                    btnPrj.Enabled = false;
                    txtMax.Enabled = true;
                    txtMin.Enabled = true ;
                    break;
                case enumErrorType.������߼��Լ��:
                    txtPrj.Enabled = false;
                    btnPrj.Enabled = false;
                    txtMax.Enabled = true;
                    txtMin.Enabled = true;
                    break;
                case enumErrorType.�߳�ֵ��� :
                    txtPrj.Enabled = false;
                    btnPrj.Enabled = false;
                    txtMax.Enabled = true;
                    txtMin.Enabled = true;
                    break;
                case enumErrorType.�ȸ��߸߳�ֵ���:
                    txtPrj.Enabled = false;
                    btnPrj.Enabled = false;
                    txtMax.Enabled = true ;
                    txtMin.Enabled = true ;
                    break;
                default:
                    txtPrj.Enabled = false;
                    btnPrj.Enabled = false;
                    txtMax.Enabled = false;
                    txtMin.Enabled = false;
                    break;
            }
        }

        private void btnDB_Click(object sender, EventArgs e)
        {
            switch (comBoxType.Text)
            {
                case "SDE":

                    break;

                case "GDB":
                    FolderBrowserDialog pFolderBrowser = new FolderBrowserDialog();
                    if (pFolderBrowser.ShowDialog() == DialogResult.OK)
                    {
                        if (!pFolderBrowser.SelectedPath.EndsWith(".gdb"))
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��GDB");
                            return;
                        }
                        txtDB.Text = pFolderBrowser.SelectedPath;
                    }
                    break;

                case "PDB":
                    OpenFileDialog OpenFile = new OpenFileDialog();
                    OpenFile.Title = "ѡ��PDB����";
                    OpenFile.Filter = "PDB����(*.mdb)|*.mdb";
                    if (OpenFile.ShowDialog() == DialogResult.OK)
                    {
                        txtDB.Text = OpenFile.FileName;
                    }
                    break;

                default:
                    break;
            }
        }

        private void comBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comBoxType.Text != "SDE")
            {
                btnDB.Visible = true;
                txtDB.Size = new Size(txtServer.Size.Width - btnDB.Size.Width, txtDB.Size.Height);
                txtServer.Enabled = false;
                txtInstance.Enabled = false;
                txtUser.Enabled = false;
                txtPassword.Enabled = false;
                txtVersion.Enabled = false;
            }
            else
            {
                btnDB.Visible = false;
                txtDB.Size = new Size(txtServer.Size.Width, txtDB.Size.Height);
                txtServer.Enabled = true;
                txtInstance.Enabled = true;
                txtUser.Enabled = true;
                txtPassword.Enabled = true;
                txtVersion.Enabled = true;

            }
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstRangeName.Items.Count; i++)
            {
                lstRangeName.Items[i].Checked = true;
            }
        }

        private void buttonX3_Click(object sender, EventArgs e)
        {
            lstRangeName.Items.Clear();

            //for (int i = 0; i < lstRangeName.Items.Count; i++)
            //{
            //    lstRangeName.Items[i].Checked = false;
            //}
        }

        private void btnPrj_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.CheckFileExists = true;
            OpenFile.CheckPathExists = true;
            OpenFile.Title = "ѡ��ռ�ο��ļ�";
            OpenFile.Filter = "�ռ�ο��ļ�(*.prj)|*.prj";
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                txtPrj.Text = OpenFile.FileName;
            }
        }

        private void btnCon_Click(object sender, EventArgs e)
        {
            Exception eError = null;

            //�������ݿ�
            pGisDT = new SysCommon.Gis.SysGisDataSet();
            if (comBoxType.Text.Trim() == "SDE")
            {
                pGisDT.SetWorkspace(txtServer.Text,txtInstance.Text ,txtDB.Text ,txtUser.Text ,txtPassword.Text ,txtVersion.Text ,out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݿ�����ʧ�ܣ�");
                    return;
                }
            }
            else if (comBoxType.Text.Trim() == "GDB")
            {
                pGisDT.SetWorkspace(txtDB.Text.Trim(), SysCommon.enumWSType.GDB, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݿ�����ʧ�ܣ�");
                    return;
                }
            }
            else
            {
                pGisDT.SetWorkspace(txtDB.Text.Trim(), SysCommon.enumWSType.PDB, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݿ�����ʧ�ܣ�");
                    return;
                }
            }

            //������ݿ��е����ݼ�
            List<string> feaDatasetNameLst = pGisDT.GetAllFeatureDatasetNames();
            for (int i = 0; i < feaDatasetNameLst.Count; i++)
            {
                ListViewItem aItem=lstRangeName.Items.Add(new ListViewItem(new string[]{feaDatasetNameLst[i]}));
                aItem.ToolTipText = feaDatasetNameLst[i];
            }
            for (int j = 0; j < lstRangeName.Items.Count; j++)
            {
                lstRangeName.Items[j].Checked = true;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Exception eError = null;

           
                if (lstRangeName.CheckedItems.Count == 0)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ�����ݼ���");
                    return;
                }

                if (txtPrj.Enabled == true)
                {
                    //�ж��Ƿ�����ռ�ο��ļ�
                    if (txtPrj.Text == "" || !File.Exists(txtPrj.Text.Trim()))
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���׼�Ŀռ�ο��ļ�!");
                        return;
                    }
                }
                if (txtMin.Enabled == true && txtMax.Enabled == true)
                {
                    //�ж��ı��������Ƿ�����Ƿ��ַ�������Сֵ�Ƿ�С�����ֵ
                    if (txtMin.Text == "" && txtMax.Text == "")
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����д���ֵ����Сֵ�ķ�Χ��");
                        return;
                    } try
                    {
                        if (txtMin.Text != "" && txtMax.Text != "")
                        {
                            if (Convert.ToDouble(txtMin.Text.Trim()) > Convert.ToDouble(txtMax.Text.Trim()))
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��Сֵ���ܴ������ֵ��");
                                return;
                            }
                        }

                    }
                    catch (Exception eex)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "'���ֵ'��'��Сֵ'ӦΪ���֣���������Ч�����֣�");
                        return;
                    }
                }
                try
                {
                //������־������Ϣ
                string logPath = txtLog.Text;
                if (logPath.Trim() == string.Empty)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ����־���·��!");
                    return;
                }
                if (File.Exists(logPath))
                {
                    if (SysCommon.Error.ErrorHandle.ShowFrmInformation("��", "��", "�����ļ�\n'" + logPath + "'\n�Ѿ�����,�Ƿ��滻?"))
                    {
                        File.Delete(logPath);
                    }
                    else 
                    {
                        return;
                    }
                    //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ļ��Ѵ���!\n" );
                }
                //������־��ϢEXCEL��ʽ
                SysCommon.DataBase.SysDataBase pSysLog = new SysCommon.DataBase.SysDataBase();
                pSysLog.SetDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + logPath + "; Extended Properties=Excel 8.0;", SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out eError );
                if (eError!= null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��־��Ϣ������ʧ�ܣ�");
                    return;
                }
                string strCreateTableSQL = @" CREATE TABLE ";
                strCreateTableSQL += @" ������־ ";
                strCreateTableSQL += @" ( ";
                strCreateTableSQL += @" ��鹦���� VARCHAR, ";
                strCreateTableSQL += @" �������� VARCHAR, ";
                strCreateTableSQL += @" �������� VARCHAR, ";
                strCreateTableSQL += @" ����ͼ��1 VARCHAR, ";
                strCreateTableSQL += @" ����OID1 VARCHAR, ";
                strCreateTableSQL += @" ����ͼ��2 VARCHAR, ";
                strCreateTableSQL += @" ����OID2 VARCHAR, ";
                strCreateTableSQL += @" ��λ��X VARCHAR , ";
                strCreateTableSQL += @" ��λ��Y VARCHAR , ";
                strCreateTableSQL += @" ���ʱ�� VARCHAR ,";
                strCreateTableSQL += @" �����ļ�·�� VARCHAR ";
                strCreateTableSQL += @" ) ";

                pSysLog.UpdateTable(strCreateTableSQL, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ͷ����");
                    pSysLog.CloseDbConnection();
                    return;
                }
                //����־��������Ϣ�ͱ�����������
                //TopologyCheckClass.ErrDbCon = pSysLog.DbConn;
                //TopologyCheckClass.ErrTableName = "������־";

                DataCheckClass dataCheckCls = new DataCheckClass(_AppHk);
                //����־��������Ϣ�ͱ�����������
                dataCheckCls.ErrDbCon = pSysLog.DbConn;
                dataCheckCls.ErrTableName = "������־";

                SysCommon.DataBase.SysTable pSysTable = new SysCommon.DataBase.SysTable();
                string conStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + TopologyCheckClass.GeoDataCheckParaPath;
                pSysTable.SetDbConnection(conStr, SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out eError);
                if (eError != null)
                {
                   SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ","GIS���ݼ�����ñ�����ʧ�ܣ�");
                    return;
                }
                //��������ֶ���
                string codeName = GetCodeName(pSysTable, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                    pSysLog.CloseDbConnection();
                    pSysTable.CloseDbConnection();
                    return;
                }

                #region �������ݼ����������ݼ��
                bool b = false;
                for (int i = 0; i < lstRangeName.CheckedItems.Count; i++)
                {
                    IFeatureDataset pFeaDataset = pGisDT.GetFeatureDataset(lstRangeName.CheckedItems[i].Text.Trim(), out eError);
                    if (eError != null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ�ʧ��,���ݼ�����Ϊ:" + lstRangeName.Items[i].Text.Trim());
                        continue;
                    }
                    DataTable mTable = null;
                    switch (_ErrorType)
                    {
                        case enumErrorType.��ѧ������ȷ�Լ��:
                            ISpatialReferenceFactory spatialRefFac = new SpatialReferenceEnvironmentClass();
                            ISpatialReference standardSpatialRef = spatialRefFac.CreateESRISpatialReferenceFromPRJFile(txtPrj.Text.Trim());
                            dataCheckCls.MathematicsCheck(pFeaDataset, standardSpatialRef, out eError);
                            if (eError != null)
                            {
                                //Enum.Parse(typeof(enumErrorType), _ErrorType.GetHashCode().ToString()).ToString()
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѧ��������ȷ���ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.��ֵ���:
                            //��ֵ������IDΪ2
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 2, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            //ִ�п�ֵ���
                            IsNullCheck(dataCheckCls, pFeaDataset, mTable, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ֵ���ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�߳����߼��Լ��:
                            //�߳����߼��Լ�飬����IDΪ3
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 3, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            //ִ���߳����߼��Լ��
                            LineLogicCheck(dataCheckCls, pFeaDataset, mTable, codeName, txtMin.Text.Trim(), txtMax.Text.Trim(), out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�߳����߼��Լ��ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.������߼��Լ��:
                            //������߼��Լ�飬����IDΪ4
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 4, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            //ִ��������߼��Լ��
                            AreaCheck(dataCheckCls, pFeaDataset, mTable, codeName, txtMin.Text.Trim(), txtMax.Text.Trim(), out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������߼��Լ��ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�߳�ֵ���:
                            //�߳�ֵ��飬����IDΪ5
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 5, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            //ִ�и߳�ֵ���
                            ElevValueCheck(dataCheckCls, pFeaDataset, mTable, txtMin.Text.Trim(), txtMax.Text.Trim(), out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�쳣�߳�ֵ���ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�ȸ��߸߳�ֵ���:
                            //�ȸ��߸߳�ֵ���
                            //�ȸ���ͼ����������IDΪ20
                            string pFeaClsName = GetParaValue(pSysTable, 20, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            //�ȸ��߸߳��ֶ���,����ID23
                            string pFieldName = GetParaValue(pSysTable, 23, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }

                            //�ȸ��߸̼߳�����ֵ,����IDΪ21
                            string paraValue = GetParaValue(pSysTable, 21, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            double intevalValue = Convert.ToDouble(paraValue);
                            //ִ�еȸ��߼��
                            dataCheckCls.contourIntevalCheck(pFeaDataset, pFeaClsName, pFieldName, txtMin.Text.Trim(), txtMax.Text.Trim(), intevalValue, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ȸ��߸߳�ֵ���ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.���Ƶ�ע��һ���Լ��:
                            //���Ƶ�ע�Ǽ�飬����IDΪ29
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 29, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            //���Ƶ�ע�Ǽ�������뾶,����IDΪ32
                            string paraValue1 = GetParaValue(pSysTable, 32, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            double serchRadiu1 = Convert.ToDouble(paraValue1);
                            //���Ƶ�ע�Ǽ�龫�ȿ��ƣ�����IDΪ35
                            paraValue1 = GetParaValue(pSysTable, 35, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            long precision1 = Convert.ToInt64(paraValue1);
                            //ִ�п��Ƶ�ע�Ǽ��
                            PointAnnoCheck(dataCheckCls, pFeaDataset, mTable, codeName, serchRadiu1, precision1, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���Ƶ�ע�Ǽ��ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�̵߳�ע��һ���Լ��:
                            //�̵߳�ע�Ǽ�飬����IDΪ30
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 30, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            //�̵߳�ע�Ǽ�������뾶,����IDΪ33
                            string paraValue2 = GetParaValue(pSysTable, 33, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            double serchRadiu2 = Convert.ToDouble(paraValue2);
                            //�̵߳�ע�Ǽ�龫�ȿ��ƣ�����IDΪ36
                            paraValue2 = GetParaValue(pSysTable, 36, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            long precision2 = Convert.ToInt64(paraValue2);
                            //ִ�и̵߳�ע�Ǽ��
                            PointAnnoCheck(dataCheckCls, pFeaDataset, mTable, codeName, serchRadiu2, precision2, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�̵߳�ע�Ǽ��ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�ȸ���ע��һ���Լ��:
                            //�ȸ���ע�Ǽ�飬����IDΪ31
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 31, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            //�ȸ���ע�Ǽ�������뾶,����IDΪ34
                            string paraValue3 = GetParaValue(pSysTable, 34, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            double serchRadiu3 = Convert.ToDouble(paraValue3);
                            //�ȸ���ע�Ǽ�龫�ȿ��ƣ�����IDΪ37
                            paraValue3 = GetParaValue(pSysTable, 37, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            long precision3 = Convert.ToInt64(paraValue3);
                            //ִ�еȸ���ע�Ǽ��
                            PointAnnoCheck(dataCheckCls, pFeaDataset, mTable, codeName, serchRadiu3, precision3, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ȸ���ע�Ǽ��ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�ȸ��ߵ���ì�ܼ��:
                            //�̵߳�ͼ��,����IDΪ19(����Ҫ�Ľ���
                            string pointFeaclsname = GetParaValue(pSysTable, 19, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            //�ȸ���ͼ��,����IDΪ20
                            string lineFeaclsname = GetParaValue(pSysTable, 20, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            //�̵߳�߳��ֶ���,����IDΪ22
                            string pointFieldsname = GetParaValue(pSysTable, 22, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            //�ȸ��߸߳��ֶ���,����IDΪ23
                            string lineFieldname = GetParaValue(pSysTable, 23, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            //�ȸ��߼��ֵ,����IDΪ21
                            string intervalValue = GetParaValue(pSysTable, 21, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            //�̵߳������뾶,����IDΪ38
                            string radiu = GetParaValue(pSysTable, 38, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            //ִ�еȸ��ߵ���ì�ܼ��
                            dataCheckCls.PointLineElevCheck(pFeaDataset, lineFeaclsname, lineFieldname, pointFeaclsname, pointFieldsname, Convert.ToDouble(intervalValue), out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ȸ��ߵ���ì�ܼ��ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�����ཻ���:
                            dataCheckCls.OrdinaryTopoCheck(pFeaDataset, esriGeometryType.esriGeometryPolyline, esriTopologyRuleType.esriTRTLineNoSelfIntersect, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ཻ���ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�����ص����:
                            dataCheckCls.OrdinaryTopoCheck(pFeaDataset, esriGeometryType.esriGeometryPolyline, esriTopologyRuleType.esriTRTLineNoSelfOverlap, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ص����ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�ߴ������ҵ�:
                            //�����ҵ��飬����IDΪ6
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 6, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            //�����ҵ��������ݲ�,����IDΪ38
                            string lineDangleRadiuStr = GetParaValue(pSysTable, 38, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ����ñ�ʧ�ܡ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            double lineDangleRadiu = Convert.ToDouble(lineDangleRadiuStr);
                            LineDangleCheck2(dataCheckCls, pFeaDataset, mTable, codeName, lineDangleRadiu, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ҵ���ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�ߴ���α�ڵ�:
                            dataCheckCls.OrdinaryTopoCheck(pFeaDataset, esriGeometryType.esriGeometryPolyline, esriTopologyRuleType.esriTRTLineNoPseudos, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��α�ڵ���ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.ͬ�����ص����:
                            //ͬ�����ص���飬����IDΪ14
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 14, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            SpecialFeaClsTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTLineNoOverlap, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "ͬ�����ص����ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.ͬ�����ཻ���:
                            //ͬ�����ཻ��飬����IDΪ15
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 15, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            SpecialFeaClsTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTLineNoIntersection, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "ͬ�����ཻ���ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.ͬ�����ص����:
                            dataCheckCls.OrdinaryTopoCheck(pFeaDataset, esriGeometryType.esriGeometryPolygon, esriTopologyRuleType.esriTRTAreaNoOverlap, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "ͬ�����ص����ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.������ص����:
                            //������ص���飬����IDΪ7
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 7, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }

                            AreaTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTAreaNoOverlapArea, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������ص����ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.���϶���:
                            //���϶��飬����IDΪ8
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 8, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            SpecialFeaClsTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTAreaNoGaps, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���϶���ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�溬����:
                            //�溬���飬����IDΪ9
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 9, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            AreaTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTAreaCoveredByArea, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�溬����ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�������:
                            //������飬����IDΪ10
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 10, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            AreaTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTPointCoveredByAreaBoundary, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�������ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.����߼��:
                            //����߼�飬����IDΪ11
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 11, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            AreaTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTPointCoveredByLine, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����߼��ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.��λ���߶˵���:
                            //��λ���߶˵��飬����IDΪ12
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 12, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            AreaTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTPointCoveredByLineEndpoint, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��λ���߶˵���ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.��λ�����ڼ��:
                            //��λ�����ڼ�飬����IDΪ13
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 13, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            AreaTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTPointProperlyInsideArea, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�溬����ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.����߽��غϼ��:
                            //����߽��غϼ�飬����IDΪ16
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 16, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            AreaTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTLineCoveredByAreaBoundary, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����غϼ��ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�ߴ�����:
                            //�ߴ����飬����IDΪ17
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 17, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            LineCrossAreaCheck(dataCheckCls, pFeaDataset, mTable, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ߴ�����ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.������ص����:
                            //������ص���飬����IDΪ40
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 40, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            AreaTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTLineNoOverlapLine, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������ص����ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.��߽���߽��غϼ��:
                            //��߽���߽��غϼ�飬����IDΪ41
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 41, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            AreaTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTAreaBoundaryCoveredByAreaBoundary, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��߽���߽��غϼ��ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�����໥���Ǽ��:
                            //�����໥���Ǽ�飬����IDΪ42
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 42, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            AreaTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTAreaAreaCoverEachOther, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����໥���Ǽ��ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�溬����:
                            //�溬���飬����IDΪ43
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 43, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            AreaTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTAreaContainPoint, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�溬����ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.��߽����غϼ��:
                            //��߽����غϼ�飬����IDΪ44
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 44, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            AreaTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTAreaBoundaryCoveredByLine, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��߽����غϼ��ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�����غϼ��:
                            //�����غϼ�飬����IDΪ45
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 45, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            AreaTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTLineCoveredByLineClass, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����غϼ��ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�߶˵㱻�㸲�Ǽ��:
                            //�߶˵㱻�㸲�Ǽ�飬����IDΪ46
                            mTable = GetParaValueTable(pFeaDataset, pSysTable, 46, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            if (mTable.Rows.Count == 0)
                            {
                                b = true;
                                break;
                            }
                            AreaTopoCheck2(dataCheckCls, pFeaDataset, mTable, esriTopologyRuleType.esriTRTLineEndpointCoveredByPoint, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�߶˵㱻�㸲�Ǽ��ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.���߼��:
                            //���߼��
                            dataCheckCls.OrdinaryTopoCheck(pFeaDataset, esriGeometryType.esriGeometryPolyline, esriTopologyRuleType.esriTRTLineNoMultipart, out eError);
                            if (eError != null)
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���߼��ʧ�ܣ�" + eError.Message);
                                pSysLog.CloseDbConnection();
                                pSysTable.CloseDbConnection();
                                return;
                            }
                            break;
                        case enumErrorType.�ӱ߼��:
                            //���ýӱ߼������
                            //FromJoinCheck = new SetJoinChecks(_AppHk );
                            //FromJoinCheck.Show();
                            break;
                        default:
                            break;
                    }
                    if (b == true)
                    {
                        continue;
                    }

                }
                #endregion

                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݼ�����!");
                pSysLog.CloseDbConnection();
                pSysTable.CloseDbConnection();
                this.Close();
            }
            catch (Exception ex)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
                return;
            }
        }

        #region ���ݼ�麯��

        /// <summary>
        /// ���ҷ�������ֶ�����
        /// </summary>
        /// <param name="pSysTable"></param>
        /// <param name="eError"></param>
        /// <returns></returns>
        private string GetCodeName(SysCommon.DataBase.SysTable pSysTable, out Exception eError)
        {
            eError = null;
            string codeName = "";    //��������ֶ�����
            string selStr = "select * from GeoCheckPara where ����ID=1";
            DataTable pTable = pSysTable.GetSQLTable(selStr, out eError);
            if (eError != null)
            {
                eError = new Exception("���ҷ�������ֶ�����ʧ�ܣ�");
            }

            if (pTable == null || pTable.Rows.Count == 0)
            {
                eError = new Exception("�Ҳ�����������ֶ����Ƽ�¼!");
            }
            codeName = pTable.Rows[0]["����ֵ"].ToString().Trim();
            if (codeName == "")
            {
                eError = new Exception("���ñ���δ���÷�������ֶ��������飡");
            }
            return codeName;
        }

        /// <summary>
        /// ���Ҳ���ֵ���
        /// </summary>
        /// <param name="pFeaDataset"></param>
        /// <param name="pSysTable"></param>
        /// <param name="checkParaID">����ID��Ψһ��ʶ�������</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        private DataTable  GetParaValueTable(IFeatureDataset pFeaDataset, SysCommon.DataBase.SysTable pSysTable,int checkParaID,out Exception eError)
        {
            eError = null;
            DataTable mTable = null;

            string selStr = "select * from GeoCheckPara where ����ID=" + checkParaID;
            DataTable pTable = pSysTable.GetSQLTable(selStr, out eError);
            if (eError != null)
            {
                eError = new Exception("��ѯ�����󣬱���Ϊ��GeoCheckPara������IDΪ:" + checkParaID);
                return null ;
            }

            if (pTable == null || pTable.Rows.Count == 0)
            {
                eError = new Exception("�Ҳ�����¼������IDΪ:" + checkParaID);
                return null ;
            }
            string ParaType = pTable.Rows[0]["��������"].ToString().Trim();            //��������
            if (ParaType == "GeoCheckParaValue")
            {
                int ParaValue=int.Parse(pTable.Rows[0]["����ֵ"].ToString().Trim());   //����ֵ��������ʶ�������
                string feaDTName = pFeaDataset.Name;                                   //���ݼ�����  
                if(feaDTName.Contains("."))
                {
                    feaDTName = feaDTName.Substring(feaDTName.IndexOf('.') + 1);
                }
                string str = "select * from GeoCheckParaValue where �������=" + ParaValue+" and ���ݼ�='"+feaDTName+"'";
                mTable = pSysTable.GetSQLTable(str, out eError);
                if (eError != null)
                {
                    eError = new Exception("��ѯ�����󣬱���Ϊ��GeoCheckParaValue���������Ϊ:" + ParaValue);
                    return null ;
                }
            }
            return mTable;
        }

        /// <summary>
        /// ���ݲ���ID��ò���ֵ
        /// </summary>
        /// <param name="pSysTable"></param>
        /// <param name="checkParaID"></param>
        /// <param name="eError"></param>
        /// <returns></returns>
        private string GetParaValue(SysCommon.DataBase.SysTable pSysTable, int checkParaID, out Exception eError)
        {
            eError = null;
            string paraValue="";

            string selStr = "select * from GeoCheckPara where ����ID=" + checkParaID;
            DataTable pTable = pSysTable.GetSQLTable(selStr, out eError);
            if (eError != null)
            {
                eError = new Exception("��ѯ�����󣬱���Ϊ��GeoCheckPara������IDΪ:" + checkParaID);
                return "";
            }

            if (pTable == null || pTable.Rows.Count == 0)
            {
                eError = new Exception("�Ҳ�����¼������IDΪ:" + checkParaID);
                return "";
            }
            paraValue = pTable.Rows[0]["����ֵ"].ToString().Trim();            //��������
            return paraValue;
        }

        /// <summary>
        /// ��ֵ���
        /// </summary>
        /// <param name="pDataCheckClass"></param>
        /// <param name="pFeaDataset"></param>
        /// <param name="pTable">�������ñ�</param>
        /// <param name="eError"></param>
        private void IsNullCheck(DataCheckClass pDataCheckClass,IFeatureDataset pFeaDataset,DataTable pTable,out Exception eError)
        {
            eError = null;

            //��������ͼ�����ͺ�Ҫ�����ֶ���
            Dictionary<string, List<string>> feaClsInfodic = new Dictionary<string, List<string>>();
            for (int i = 0; i < pTable.Rows.Count; i++)
            {
                string pFeaClsName = pTable.Rows[i]["ͼ��"].ToString().Trim();   //ͼ����
                string fieldName = pTable.Rows[i]["�ֶ���"].ToString().Trim();   //Ҫ���м����ֶ���
                if (pFeaClsName == "")
                {
                    eError = new Exception("ͼ����Ϊ��!");
                    return;
                }
                if (fieldName == "")
                {
                    eError = new Exception("�ֶ���Ϊ��!");
                    return;
                }
                if (!feaClsInfodic.ContainsKey(pFeaClsName))
                {
                    List<string> fieldList = new List<string>();
                    fieldList.Add(fieldName);
                    feaClsInfodic.Add(pFeaClsName, fieldList);
                }
                else
                {
                    if (!feaClsInfodic[pFeaClsName].Contains(fieldName))
                    {
                        feaClsInfodic[pFeaClsName].Add(fieldName);
                    }
                }
            }
            pDataCheckClass.IsNullableCheck(pFeaDataset, feaClsInfodic, out eError);
        }

        /// <summary>
        /// �߳����߼��Լ��
        /// </summary>
        /// <param name="pDataCheckClass"></param>
        /// <param name="pFeaDataset"></param>
        /// <param name="pTable">�������ñ�</param>
        /// <param name="codeName">��������ֶ���</param>
        /// <param name="lenMin">��С����</param>
        /// <param name="lenMax">��󳤶�</param>
        /// <param name="eError"></param>
        private void LineLogicCheck(DataCheckClass pDataCheckClass, IFeatureDataset pFeaDataset, DataTable pTable, string codeName,string lenMin,string lenMax, out Exception eError)
        {
            eError = null;
            Dictionary<string, string> feaClsInfodic = new Dictionary<string, string>();
            for (int i = 0; i < pTable.Rows.Count; i++)
            {
                string pFeaClsName = pTable.Rows[i]["ͼ��"].ToString().Trim();  //ͼ����
                string codeValue = pTable.Rows[i]["�����"].ToString().Trim();  //Ҫ���ķ������ֵ����
                if (pFeaClsName == "")
                {
                    eError = new Exception("ͼ����Ϊ��!");
                    return;
                }
                if (!feaClsInfodic.ContainsKey(pFeaClsName))
                {
                    feaClsInfodic.Add(pFeaClsName, codeValue);
                }
                else
                {
                    if (codeValue != "")
                    {
                        string tempStr = feaClsInfodic[pFeaClsName] + "," + codeValue;
                        feaClsInfodic[pFeaClsName] = tempStr;
                    }
                    else
                    {
                        feaClsInfodic[pFeaClsName] = "";
                    }
                }
            }
            pDataCheckClass.LineLengthLogicCheck(pFeaDataset, codeName, feaClsInfodic, lenMin, lenMax, out eError);
        }

        /// <summary>
        /// ������߼��Լ��
        /// </summary>
        /// <param name="pDataCheckClass"></param>
        /// <param name="pFeaDataset"></param>
        /// <param name="pTable">�������ñ�</param>
        /// <param name="codeName">���������</param>
        /// <param name="AreaMin">�����Сֵ</param>
        /// <param name="AreaMax">������ֵ</param>
        /// <param name="eError"></param>
        private void AreaCheck(DataCheckClass pDataCheckClass, IFeatureDataset pFeaDataset, DataTable pTable, string codeName, string AreaMin, string AreaMax, out Exception eError)
        {
            eError = null;
            Dictionary<string, string> feaClsInfodic = new Dictionary<string, string>();
            for (int i = 0; i < pTable.Rows.Count; i++)
            {
                string pFeaClsName = pTable.Rows[i]["ͼ��"].ToString().Trim();//ͼ����
                string codeValue = pTable.Rows[i]["�����"].ToString().Trim();//Ҫ���ķ������ֵ����
                if (pFeaClsName == "")
                {
                    eError = new Exception("ͼ����Ϊ��!");
                    return;
                }
                if (!feaClsInfodic.ContainsKey(pFeaClsName))
                {
                    feaClsInfodic.Add(pFeaClsName, codeValue);
                }
                else
                {
                    if (codeValue != "")
                    {
                        string tempStr = feaClsInfodic[pFeaClsName] + "," + codeValue;
                        feaClsInfodic[pFeaClsName] = tempStr;
                    }
                    else
                    {
                        feaClsInfodic[pFeaClsName] = "";
                    }
                }
            }
            pDataCheckClass.AreaLogicCheck(pFeaDataset, codeName, feaClsInfodic, AreaMin, AreaMax, out eError);
        }

        /// <summary>
        /// �߳�ֵ���
        /// </summary>
        /// <param name="pDataCheckClass"></param>
        /// <param name="pFeaDataset"></param>
        /// <param name="pTable"></param>
        /// <param name="ElevMin">��С�߳�ֵ</param>
        /// <param name="ElevMax">���߳�ֵ</param>
        /// <param name="eError"></param>
        private void ElevValueCheck(DataCheckClass pDataCheckClass, IFeatureDataset pFeaDataset, DataTable pTable,string ElevMin,string ElevMax, out Exception eError)
        {
            eError = null;
            
            for (int i = 0; i < pTable.Rows.Count; i++)
            {
                string pFeaClsName = pTable.Rows[i]["ͼ��"].ToString().Trim();  //ͼ����
                string fieldName = pTable.Rows[i]["�ֶ���"].ToString().Trim();  //�߳��ֶ���
                if (pFeaClsName == "")
                {
                    eError = new Exception("ͼ����Ϊ��!");
                    return;
                }
                if (fieldName == "")
                {
                    eError = new Exception("�ֶ���Ϊ��!");
                    return;
                }
                pDataCheckClass.CoutourValueCheck(pFeaDataset, pFeaClsName,fieldName, ElevMin, ElevMax, out eError);
                if (eError != null) return;
            }
        }

        /// <summary>
        /// ���Ƶ�߳�ע�Ǽ�顢�̵߳�߳�ע�Ǽ�顢�ȸ��߸߳�ע�Ǽ��
        /// </summary>
        /// <param name="pDataCheckClass"></param>
        /// <param name="pFeaDataset"></param>
        /// <param name="pTable"></param>
        /// <param name="codeName">�����������</param>
        /// <param name="radiu">�����뾶</param>
        /// <param name="precision">���ȿ���</param>
        /// <param name="eError"></param>
        private void PointAnnoCheck(DataCheckClass pDataCheckClass, IFeatureDataset pFeaDataset, DataTable pTable,string codeName, double radiu, long precision, out Exception eError)
        {
             eError = null;

             for (int i = 0; i < pTable.Rows.Count; i++)
             {
                 string FeaClsName = pTable.Rows[i]["ͼ��"].ToString().Trim();  //ͼ����
                 string FieldName = pTable.Rows[i]["�ֶ���"].ToString().Trim();  //�ֶ���
                 string codeValue = pTable.Rows[i]["�����"].ToString().Trim();  //�����
                 if ((FeaClsName == "")||(!FeaClsName.Contains(";")))
                 {
                     eError = new Exception("ͼ����Ϊ�ջ����ò���ȷ!");
                     return;
                 }
                 if ((FieldName == "") || (!FeaClsName.Contains(";"))) 
                 {
                     eError = new Exception("�ֶ���Ϊ��!");
                     return;
                 }
                 string oriCodeValue = "";
                 string desCodeValue = "";

                 string[] feaNameArr = FeaClsName.Split(new char[] { ';' });
                 string oriFeaClsName = feaNameArr[0].Trim();         //ԴҪ��������
                 string desFeaClsName = feaNameArr[1].Trim();         //Ŀ��Ҫ��������
                 string[] fieldNameArr = FieldName.Split(new char[] { ';' });
                 string oriFieldName = fieldNameArr[0].Trim();        //Դ�߳��ֶ���
                 string desFielsName = fieldNameArr[1].Trim();        //Ŀ��߳��ֶ���

                 if (codeValue != "" && codeValue.Contains(";"))
                 {
                     string[] codeValueArr = codeValue.Split(new char[] { ';' });
                     oriCodeValue = codeValueArr[0].Trim();        //ԴҪ������������������
                     desCodeValue = codeValueArr[1].Trim();        //Ŀ��Ҫ������������������
                 }
                 pDataCheckClass.ElevAccordanceCheck(pFeaDataset, codeName, oriFeaClsName, oriCodeValue, oriFieldName, desFeaClsName, desCodeValue, desFielsName, radiu, precision, out eError);
                 if (eError != null) return;
             }
        }

        /// <summary>
        /// �����ҵ���
        /// </summary>
        /// <param name="pDataCheckClass"></param>
        /// <param name="pFeaDataset"></param>
        /// <param name="pTable"></param>
        /// <param name="codeName">�����������</param>
        /// <param name="tolerence">�����ݲ�</param>
        /// <param name="eError"></param>
        public void LineDangleCheck(DataCheckClass pDataCheckClass, IFeatureDataset pFeaDataset, DataTable pTable, string codeName,double tolerence, out Exception eError)
        {
              eError = null;

              for (int i = 0; i < pTable.Rows.Count; i++)
              {
                  string FeaClsName = pTable.Rows[i]["ͼ��"].ToString().Trim();  //ͼ����
                  string codeValue = pTable.Rows[i]["�����"].ToString().Trim();  //�������ֵ
                  if ((FeaClsName == "") || (!FeaClsName.Contains(";")))
                  {
                      eError = new Exception("ͼ����Ϊ�ջ����ò���ȷ!");
                      return;
                  }

                  string[] feaNameArr=FeaClsName.Split(new char[]{';'});
                  string oriFeaClsName = feaNameArr[0].Trim();  //ԴҪ������
                  string desFeaClsName = feaNameArr[1].Trim();  //Ŀ��Ҫ����
                  if (oriFeaClsName == desFeaClsName)
                  {
                      //ͬ�������ҵ���
                      pDataCheckClass.OrdinaryTopoCheck(pFeaDataset,oriFeaClsName, esriTopologyRuleType.esriTRTLineNoDangles, out eError);
                      if (eError != null)
                      {
                          return;
                      }
                  }
                  else
                  {
                      //�������
                      string oriCodeValueStr = "";
                      string desCodeValueStr = "";

                      if (codeValue != "" && codeValue.Contains(";"))
                      {
                          string[] codeValueArr = codeValue.Split(new char[] { ';' });
                          string oriCodeValue = codeValueArr[0].Trim();       
                          string desCodeValue = codeValueArr[1].Trim();        
                          oriCodeValueStr = codeName + "='" + oriCodeValue + "'"; //ԴҪ������������������
                          desCodeValueStr = codeName + "='" + desCodeValue + "'"; //Ŀ��Ҫ������������������
                      }

                      pDataCheckClass.LineDangleCheck(pFeaDataset, oriFeaClsName, oriCodeValueStr, desFeaClsName, desCodeValueStr, tolerence, out eError);
                      if (eError != null)
                      {
                          return;
                      }
                  }
              }
 
        }

        /// <summary>
        /// �����ҵ��� ͨ���㷨(�б�)
        /// </summary>
        /// <param name="pDataCheckClass"></param>
        /// <param name="pFeaDataset"></param>
        /// <param name="pTable"></param>
        /// <param name="codeName">�����������</param>
        /// <param name="tolerence">�����ݲ�</param>
        /// <param name="eError"></param>
        public void LineDangleCheck2(DataCheckClass pDataCheckClass, IFeatureDataset pFeaDataset, DataTable pTable, string codeName, double tolerence, out Exception eError)
        {
            eError = null;
            List<string> feaClsNameList = new List<string>();
            for (int i = 0; i < pTable.Rows.Count; i++)
            {
                string FeaClsName = pTable.Rows[i]["ͼ��"].ToString().Trim();  //ͼ����
                string codeValue = pTable.Rows[i]["�����"].ToString().Trim();  //�������ֵ
                if ((FeaClsName == "") || (!FeaClsName.Contains(";")))
                {
                    eError = new Exception("ͼ����Ϊ�ջ����ò���ȷ!");
                    return;
                }

                string[] feaNameArr = FeaClsName.Split(new char[] { ';' });
                string oriFeaClsName = feaNameArr[0].Trim();  //ԴҪ������
                string desFeaClsName = feaNameArr[1].Trim();  //Ŀ��Ҫ����
                if (oriFeaClsName == desFeaClsName)
                {
                    //ͬ�������ҵ��飬���˼��
                    //pDataCheckClass.OrdinaryTopoCheck(pFeaDataset, oriFeaClsName, esriTopologyRuleType.esriTRTLineNoDangles, out eError);
                    //if (eError != null)
                    //{
                    //    return;
                    //}
                    if(!feaClsNameList.Contains(oriFeaClsName))
                    {
                        feaClsNameList.Add(oriFeaClsName);
                    }
                }
                else
                {
                    //�������
                    string oriCodeValueStr = "";
                    string desCodeValueStr = "";

                    if (codeValue != "" && codeValue.Contains(";"))
                    {
                        string[] codeValueArr = codeValue.Split(new char[] { ';' });
                        string oriCodeValue = codeValueArr[0].Trim();
                        string desCodeValue = codeValueArr[1].Trim();
                        oriCodeValueStr = codeName + "='" + oriCodeValue + "'"; //ԴҪ������������������
                        desCodeValueStr = codeName + "='" + desCodeValue + "'"; //Ŀ��Ҫ������������������
                    }

                    pDataCheckClass.LineDangleCheck(pFeaDataset, oriFeaClsName, oriCodeValueStr, desFeaClsName, desCodeValueStr, tolerence, out eError);
                    if (eError != null)
                    {
                        return;
                    }
                }
            }
            if(feaClsNameList.Count==0)
            {
                return;
            }
            //ͬ�������ҵ���
            pDataCheckClass.OrdinaryTopoCheck(pFeaDataset, feaClsNameList, esriTopologyRuleType.esriTRTLineNoDangles, out eError);
            if (eError != null)
            {
                return;
            }
        }



        /// <summary>
        /// �����˼��,�溬���飬���϶���
        /// </summary>
        /// <param name="pDataCheckClass"></param>
        /// <param name="pFeaDataset"></param>
        /// <param name="pTable"></param>
        /// <param name="topoRule"></param>
        /// <param name="eError"></param>
        public void AreaTopoCheck(DataCheckClass pDataCheckClass, IFeatureDataset pFeaDataset, DataTable pTable, esriTopologyRuleType topoRule, out Exception eError)
        {
            eError = null;

            for (int i = 0; i < pTable.Rows.Count; i++)
            {
                string FeaClsName = pTable.Rows[i]["ͼ��"].ToString().Trim();  //ͼ����
                if ((FeaClsName == "") || (!FeaClsName.Contains(";")))
                {
                    eError = new Exception("ͼ����Ϊ�ջ����ò���ȷ!");
                    return;
                }

                string[] feaNameArr = FeaClsName.Split(new char[] { ';' });
                string oriFeaClsName = feaNameArr[0].Trim();  //ԴҪ������
                string desFeaClsName = feaNameArr[1].Trim();  //Ŀ��Ҫ����
                pDataCheckClass.OrdinaryTopoCheck(pFeaDataset, oriFeaClsName, desFeaClsName, topoRule, out eError);
                if (eError != null) return;
            }
        }

        /// <summary>
        /// �����˼��,�溬����(�б�)
        /// </summary>
        /// <param name="pDataCheckClass"></param>
        /// <param name="pFeaDataset"></param>
        /// <param name="pTable"></param>
        /// <param name="topoRule"></param>
        /// <param name="eError"></param>
        public void AreaTopoCheck2(DataCheckClass pDataCheckClass, IFeatureDataset pFeaDataset, DataTable pTable, esriTopologyRuleType topoRule, out Exception eError)
        {
            eError = null;
            List<string> FeaClsNameDic = new List<string>();
            for (int i = 0; i < pTable.Rows.Count; i++)
            {
                string FeaClsName = pTable.Rows[i]["ͼ��"].ToString().Trim();  //ͼ����
                if ((FeaClsName == "") || (!FeaClsName.Contains(";")))
                {
                    eError = new Exception("ͼ����Ϊ�ջ����ò���ȷ!");
                    return;
                }

                string[] feaNameArr = FeaClsName.Split(new char[] { ';' });
                string oriFeaClsName = feaNameArr[0].Trim();  //ԴҪ������
                string desFeaClsName = feaNameArr[1].Trim();  //Ŀ��Ҫ����
                if(!FeaClsNameDic.Contains(oriFeaClsName+";"+desFeaClsName))
                {
                    FeaClsNameDic.Add(oriFeaClsName + ";" + desFeaClsName);
                }
            }
            if (FeaClsNameDic.Count == 0) return;
            pDataCheckClass.OrdinaryDicTopoCheck(pFeaDataset, FeaClsNameDic, topoRule, out eError);
            if (eError != null) return;
        }

        /// <summary>
        /// ���϶���,ͬ�����ཻ���ȵ�
        /// </summary>
        /// <param name="pDataCheckClass"></param>
        /// <param name="pFeaDataset"></param>
        /// <param name="pTable"></param>
        /// <param name="eError"></param>
        public void SpecialFeaClsTopoCheck(DataCheckClass pDataCheckClass, IFeatureDataset pFeaDataset, DataTable pTable,esriTopologyRuleType pTopoRule, out Exception eError)
        {
            eError = null;

            for (int i = 0; i < pTable.Rows.Count; i++)
            {
                string FeaClsName = pTable.Rows[i]["ͼ��"].ToString().Trim();  //ͼ����
                if (FeaClsName == "")
                {
                    eError = new Exception("ͼ����Ϊ��!");
                    return;
                }
                pDataCheckClass.OrdinaryTopoCheck(pFeaDataset, FeaClsName, pTopoRule, out eError);
                if (eError != null) return;
            }
        }

        public void SpecialFeaClsTopoCheck2(DataCheckClass pDataCheckClass, IFeatureDataset pFeaDataset, DataTable pTable, esriTopologyRuleType pTopoRule, out Exception eError)
        {
            eError = null;
            List<string> lstFeaClsName = new List<string>();
            for (int i = 0; i < pTable.Rows.Count; i++)
            {
                string FeaClsName = pTable.Rows[i]["ͼ��"].ToString().Trim();  //ͼ����
                if (FeaClsName == "")
                {
                    eError = new Exception("ͼ����Ϊ��!");
                    return;
                }
               if(!lstFeaClsName.Contains(FeaClsName))
               {
                   lstFeaClsName.Add(FeaClsName);
               }
            }
            if (lstFeaClsName.Count == 0) return;
            pDataCheckClass.OrdinaryTopoCheck(pFeaDataset, lstFeaClsName, pTopoRule, out eError);
            if (eError != null) return;
        }

        /// <summary>
        /// �ߴ�����
        /// </summary>
        /// <param name="pDataCheckClass"></param>
        /// <param name="pFeaDataset"></param>
        /// <param name="pTable"></param>
        /// <param name="eError"></param>
        public void LineCrossAreaCheck(DataCheckClass pDataCheckClass, IFeatureDataset pFeaDataset, DataTable pTable, out Exception eError)
        {
            eError = null;

            for (int i = 0; i < pTable.Rows.Count; i++)
            {
                string FeaClsName = pTable.Rows[i]["ͼ��"].ToString().Trim();  //ͼ����
                if ((FeaClsName == "") || (!FeaClsName.Contains(";")))
                {
                    eError = new Exception("ͼ����Ϊ�ջ����ò���ȷ!");
                    return;
                }

                string[] feaNameArr = FeaClsName.Split(new char[] { ';' });
                string oriFeaClsName = feaNameArr[0].Trim();  //ԴҪ������
                string desFeaClsName = feaNameArr[1].Trim();  //Ŀ��Ҫ����
                pDataCheckClass.CrossTopoCheck(pFeaDataset, oriFeaClsName, desFeaClsName, out eError);
                if (eError != null) return;
            }
        }

        #endregion

        private void buttonX1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.OverwritePrompt = false;
            saveFile.Title = "����ΪEXCEL��ʽ";
            saveFile.Filter = "EXCEL��ʽ(*.xls)|*.xls";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                txtLog.Text = saveFile.FileName;
            }
        }
    }
}