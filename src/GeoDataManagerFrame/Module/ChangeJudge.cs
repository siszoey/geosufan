//*********************************************************************************
//** �ļ�����ChangeJudge.cs
//** CopyRight (c) 2000-2007 �人������Ϣ���̼������޹�˾���̲�
//** �����ˣ�chulili
//** ��  �ڣ�2011-03-21
//** �޸��ˣ�
//** ��  �ڣ�
//** ��  ��������ͼ�����й���
//**
//** ��  ����1.0
//*********************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SysCommon.Gis;
using System.Xml;
using System.IO;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using System.Data.OleDb;

using GeoDataCenterFunLib;

namespace GeoDataManagerFrame
{
    class ChangeJudge
    {
        public const string g_DLBM = "DLBM"; //�������
        public const string g_DLMC = "DLMC";//��������
        public const string g_XZQHDM = "XZQHDM";//������������
        public const string g_TBBH = "TBBH";//ͼ�߱��
        public const string g_MJ = "MJ";//���----����Ϊ�仯ͼ������һ������(���)
        public const string g_PC = "PCMC";//����
        public const string g_PCBH = "PCBH";//���α��
        public const string g_SGTJPFWH = "SGTJPFWH";//����ҵ�������ĺ�
        public const string g_SGTTPFWH = "SGTTPZWH";//ʡ��ҵ����׼�ĺ�
        public const string g_GWYPFWH = "GWYPFWH";//����Ժ�����ĺ�
        public const string g_TDYTFQDM = "TDYTFQDM";//ɭ����;��������
        public const string g_TDYTFQBH = "TDYTFQBH";//ɭ����;�������
        public const string g_TBMJ = "TBMJ";//ͼ�����
        public const string g_XZDWMJ = "XZDWMJ";//��״�������
        public const string g_LXDWMJ = "LXDWMJ";//���ǵ������
        public const string g_TBDLMJ = "TBDLMJ";//ͼ�ߵ������
        public const string g_ZB = "ZB";//ռ��
        private static string m_WorkPath = "";
        //added by chulili 
        //�������ܣ�ͼ������������
        //����������仯ͼ��ͼ�� ���α���ͼ�� ɭ����;ͼ�� ����ͼ��ͼ�� ������  ������������

        public static void DoJudgeBySelect(IFeatureLayer pChangeFeaLayer, IFeatureLayer pcbpFeaLayer, IFeatureLayer tdytFeaLayer, IFeatureLayer dltbFeaLayer,SysCommon.CProgress vProgress)
        {
            if (pChangeFeaLayer == null)
                return;
            IFeatureClass pChangeFeaClass=pChangeFeaLayer.FeatureClass;
            IFeatureSelection pSelect = pChangeFeaLayer as IFeatureSelection ;
            //���仯ͼ�߲㲻����ѡ�����˳�
            if (pSelect.SelectionSet.Count < 1)
                return;
            if (pcbpFeaLayer == null)
                return;
            if (tdytFeaLayer==null)
                return;
            if (dltbFeaLayer==null)
                return;
            string workpath = Application.StartupPath + @"\..\OutputResults\�ĵ��ɹ�\" + System.DateTime.Now.ToString("yyyyMMddhhmmss");
            m_WorkPath = workpath;
            string workSpaceName = workpath + "\\" + "JudgeRes.mdb";
            //�жϽ��Ŀ¼�Ƿ���ڣ��������򴴽�
            if (System.IO.Directory.Exists(workpath) == false)
                System.IO.Directory.CreateDirectory(workpath);
            //����һ���µ�mdb���ݿ�,���򿪹����ռ�
            IWorkspace pOutWorkSpace = CreatePDBWorkSpace(workpath, "JudgeRes.mdb");
            //��������ʾ
            if (vProgress!=null) vProgress.SetProgress("�������ɼ��ͼ�߱��������");
            pcbpJudge(pChangeFeaLayer, pcbpFeaLayer, pOutWorkSpace, false ,vProgress );
            //��������ʾ
            if (vProgress != null) vProgress.SetProgress("�������ɼ��ͼ�߹滮�����");
            tdytJudge(pChangeFeaLayer, tdytFeaLayer, pOutWorkSpace, false, vProgress);
            //��������ʾ
            if (vProgress != null) vProgress.SetProgress("�������ɼ��ͼ�ߵ��������");
            dltbJudge(pChangeFeaLayer, dltbFeaLayer, pOutWorkSpace, false, vProgress);

        }
        //ɭ����;����
        private static void tdytJudge(IFeatureLayer pChangeFeaLayer, IFeatureLayer tdytFeaLayer, IWorkspace pOutWorkSpace, bool useSelection,SysCommon.CProgress vProgress)
        {
            ITable pChangeTable = pChangeFeaLayer as ITable;
            ITable tdytTable = tdytFeaLayer as ITable;
            double rol = 0.0001;
            IFeatureClass tdytFeaClass = tdytFeaLayer.FeatureClass;
            //�����������������
            IFeatureClassName pResFeaClassName = new FeatureClassNameClass();
            String fcName = tdytFeaClass.AliasName.Trim().Substring(tdytFeaClass.AliasName.Trim().IndexOf(".") + 1)+"_res";
            IDataset pOutDataset = (IDataset)pOutWorkSpace;
            IDatasetName pOutDatasetName = (IDatasetName)pResFeaClassName;
            pOutDatasetName.WorkspaceName = (IWorkspaceName)pOutDataset.FullName;
            pOutDatasetName.Name = fcName;
            IBasicGeoprocessor pGeoProcessor = new BasicGeoprocessorClass();
            //���÷���
            pGeoProcessor.Intersect(pChangeTable, useSelection, tdytTable, false, rol, pResFeaClassName);

            //�ӵ��ý�����ɱ���
            string connstr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pOutDatasetName.WorkspaceName.PathName;
            OleDbConnection oledbconn = new OleDbConnection(connstr);
            oledbconn.Open();
            ModTableFun.DropTable(oledbconn,"tmprel");
            string sqlstr = "select "+g_XZQHDM+","+g_TBBH+","+g_MJ+" as jctbmj,"+g_TDYTFQBH+","+g_TDYTFQDM+",shape_area as jsmj,shape_area as mj,shape_area as zb into tmprel from " + fcName;
                            //�����������룬��ţ����ͼ��������滮ͼ�߱�ţ��滮�õ���;����
            OleDbCommand oledbcomm = oledbconn.CreateCommand();
            oledbcomm.CommandText = sqlstr;
            oledbcomm.ExecuteNonQuery();

            oledbcomm.CommandText = "update tmprel set zb=mj/jctbmj*100";
            oledbcomm.ExecuteNonQuery();

            ModTableFun.DropTable(oledbconn ,"ɭ����;�ֵ�");
            CopyTdytDictionary(oledbconn);//��ҵ������濽��ɭ����;�ֵ����
            
            //����ɭ����;�ֵ����ɭ����;����
            oledbcomm.CommandText = "alter table tmprel add tdytmc text(30)";
            oledbcomm.ExecuteNonQuery();
            oledbcomm.CommandText = "update tmprel set tdytmc=" + g_TDYTFQDM;
            oledbcomm.ExecuteNonQuery();
            if (ModTableFun.isExist(oledbconn,"ɭ����;�ֵ�"))
            {               
                oledbcomm.CommandText = "update tmprel a,ɭ����;�ֵ� b set a.tdytmc=b.ɭ����;�������� where a." + g_TDYTFQDM + "=b.����";
                oledbcomm.ExecuteNonQuery();
            }
            //����ģ��·��
            string Templatepath = Application.StartupPath + "\\..\\Template\\ɭ����Դ�滮����ģ��.cel";
            //���ɱ���Ի���
            oledbconn.Close();
            FormFlexcell frm;
            ModFlexcell.m_SpecialRow = -1;
            ModFlexcell.m_SpecialRow_ex = -1;
            ModFlexcell.m_SpecialRow_ex2 = -1;
            //�滮ͼ��û��ͼ�߱����ô�죿����ʱʹ��ɭ����;�������
            frm = ModFlexcell.SendDataToFlexcell(connstr, "���ͼ�߹滮�����", "tmprel", g_XZQHDM + "," + g_TBBH + ",jctbmj,TDYTFQBH,tdytmc," + g_MJ + "," + g_ZB, "", Templatepath, 4, 2);
            //��������Ի���

            AxFlexCell.AxGrid pGrid = frm.GetGrid();
            string excelPath = m_WorkPath + "\\���ͼ�߹滮�����.xls";
            pGrid.ExportToExcel(excelPath);

            //frm.SaveFile(m_WorkPath + "\\���ͼ�߹滮�����.cel");
            ModStatReport.OpenExcelFile(excelPath);
            
        }
        //���α�������
        private static void pcbpJudge(IFeatureLayer pChangeFeaLayer, IFeatureLayer pcbpFeaLayer, IWorkspace pOutWorkSpace, bool useSelection,SysCommon.CProgress vProgress)
        {
            ITable pChangeTable = pChangeFeaLayer as ITable;
            ITable pcbpTable = pcbpFeaLayer as ITable;
            double rol = 0.0001;
            IFeatureClass pcbpFeaClass = pcbpFeaLayer.FeatureClass;
            //�����������������
            IFeatureClassName pResFeaClassName = new FeatureClassNameClass();
            String fcName = pcbpFeaClass.AliasName.Trim().Substring(pcbpFeaClass.AliasName.Trim().IndexOf(".") + 1)+"_res";
            IDataset pOutDataset = (IDataset)pOutWorkSpace;
            IDatasetName pOutDatasetName = (IDatasetName)pResFeaClassName;
            pOutDatasetName.WorkspaceName = (IWorkspaceName)pOutDataset.FullName;
            pOutDatasetName.Name = fcName;
            IBasicGeoprocessor pGeoProcessor = new BasicGeoprocessorClass();
            //���÷���
            pGeoProcessor.Intersect(pChangeTable, useSelection, pcbpTable, false, rol, pResFeaClassName);

            //�ӵ��ý�����ɱ���
            string connstr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pOutDatasetName.WorkspaceName.PathName;
            OleDbConnection oledbconn = new OleDbConnection(connstr);
            oledbconn.Open();
            ModTableFun.DropTable(oledbconn, "tmprel");
            string sqlstr = "select "+g_XZQHDM+","+g_TBBH+","+g_MJ+" as jctbmj,"+g_PC  +",shape_area as " +g_MJ+",shape_area as zb,"+g_GWYPFWH+" as bpwh,"+g_SGTTPFWH+","+g_SGTJPFWH+" into tmprel from " + fcName;
                            //�����������룬��ţ����ͼ��������������ƣ����������ռ�ȣ���׼�ĺţ�������׼�ĺŵı�ѡ�ֶΣ�
            OleDbCommand oledbcomm = oledbconn.CreateCommand();
            oledbcomm.CommandText = sqlstr;
            oledbcomm.ExecuteNonQuery();

            oledbcomm.CommandText = "update tmprel set zb="+g_MJ+"/jctbmj*100";
            oledbcomm.ExecuteNonQuery();
            //�����ĺ�Ϊ�գ���ȡʡ��ҵ�������ĺ�
            oledbcomm.CommandText = "update tmprel set bpwh=" + g_SGTTPFWH + " where bpwh is null and " + g_SGTTPFWH + "  is not null";
            oledbcomm.ExecuteNonQuery();
            //�����ĺ�Ϊ�գ���ȡʡ����ҵ�������ĺ�
            oledbcomm.CommandText = "update tmprel set bpwh=" + g_SGTJPFWH + " where bpwh is null and " + g_SGTJPFWH + " is not null";
            oledbcomm.ExecuteNonQuery();
            //����ģ��·��
            string Templatepath = Application.StartupPath + "\\..\\Template\\���α�������ģ��.cel";
            oledbconn.Close();
            //���ɱ���Ի���
            FormFlexcell frm;
            ModFlexcell.m_SpecialRow = -1;
            ModFlexcell.m_SpecialRow_ex = -1;
            ModFlexcell.m_SpecialRow_ex2 = -1;
            frm = ModFlexcell.SendDataToFlexcell(connstr, "���ͼ�߱��������", "tmprel", g_XZQHDM + "," + g_TBBH + ",jctbmj," + g_PC + "," + g_MJ + "," + g_ZB + ",bpwh", "", Templatepath, 4, 2);

            AxFlexCell.AxGrid pGrid = frm.GetGrid();
            string excelPath = m_WorkPath + "\\���ͼ�߱��������.xls";
            pGrid.ExportToExcel(excelPath);
            
            //frm.SaveFile(m_WorkPath + "\\���ͼ�߱��������.cel");
            //��������
            ModStatReport.OpenExcelFile(excelPath);
            
        }
        //ɭ����Դ/����ͼ������
        private static void dltbJudge(IFeatureLayer pChangeFeaLayer, IFeatureLayer dltbFeaLayer, IWorkspace pOutWorkSpace, bool useSelection,SysCommon.CProgress vProgress)
        {
            ITable pChangeTable = pChangeFeaLayer as ITable;
            ITable pcbpTable = dltbFeaLayer as ITable;
            IFeatureClass dltbFeaClass = dltbFeaLayer.FeatureClass;
            double rol = 0.0001;
            IFeatureClassName pResFeaClassName = new FeatureClassNameClass();
            //�����������������
            String fcName = dltbFeaClass.AliasName.Trim().Substring(dltbFeaClass.AliasName.Trim().IndexOf(".") + 1)+"_res";
            IDataset pOutDataset = (IDataset)pOutWorkSpace;
            IDatasetName pOutDatasetName = (IDatasetName)pResFeaClassName;
            pOutDatasetName.WorkspaceName = (IWorkspaceName)pOutDataset.FullName;
            pOutDatasetName.Name = fcName;
            IBasicGeoprocessor pGeoProcessor = new BasicGeoprocessorClass();
            //���÷���
            pGeoProcessor.Intersect(pChangeTable, useSelection, pcbpTable, false, rol, pResFeaClassName);

            //�ӵ��ý�����ɱ���
            string connstr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pOutDatasetName.WorkspaceName.PathName ;
            OleDbConnection oledbconn = new OleDbConnection(connstr);
            oledbconn.Open();
            ModTableFun.DropTable(oledbconn, "tmprel");
            string sqlstr = "select "+g_XZQHDM+","+g_TBBH+","+g_MJ+" as jctbmj,"+g_TBBH+"_1,"+g_DLBM+","+g_TBMJ+","+g_TBDLMJ+",shape_area as jsmj,shape_area as mj,shape_area as zb into tmprel from " + fcName;
                            //�����������룬��ţ������ͼ�߱�ţ�������룬ͼ�������ͼ�ߵ�����������������ռ��
            OleDbCommand oledbcomm = oledbconn.CreateCommand();
            oledbcomm.CommandText = sqlstr;
            oledbcomm.ExecuteNonQuery();
            //���ý������������㷽����  ���=���ý���������*����ͼ�ߵ������/����ͼ�������
            oledbcomm.CommandText = "update tmprel set mj=jsmj*"+g_TBDLMJ+"/"+g_TBMJ+"";
            oledbcomm.ExecuteNonQuery();
            //����ռ��
            oledbcomm.CommandText = "update tmprel set zb=mj/jctbmj*100";
            oledbcomm.ExecuteNonQuery();
            //����ģ��·��
            oledbconn.Close();
            string Templatepath = Application.StartupPath + "\\..\\Template\\ɭ����Դ��״����ģ��.cel";
            //���ɱ���Ի���
            FormFlexcell frm;
            ModFlexcell.m_SpecialRow = -1;
            ModFlexcell.m_SpecialRow_ex = -1;
            ModFlexcell.m_SpecialRow_ex2 = -1;
            frm = ModFlexcell.SendDataToFlexcell(connstr, "���ͼ�ߵ��������", "tmprel", g_XZQHDM + "," + g_TBBH + ",jctbmj," + g_TBBH + "_1,"+g_DLBM+"," + g_MJ + "," + g_ZB, "", Templatepath, 4, 2);

            AxFlexCell.AxGrid pGrid = frm.GetGrid();
            string excelPath = m_WorkPath + "\\���ͼ�ߵ��������.xls";
            pGrid.ExportToExcel(excelPath);
            
            //frm.SaveFile(m_WorkPath + "\\���ͼ�ߵ��������.cel");
            //��������
            ModStatReport.OpenExcelFile(excelPath);
            
        }
        //�������ܣ�����ɭ����;�ֵ�  ������������ݿ�����  ������������
        //ɭ����;�ֵ��������֪
        private static void CopyTdytDictionary(OleDbConnection conn)
        {
            if (conn==null)
                return;
            //if (conn.State!=1)
            //    return;
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            //string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���

            OleDbCommand mycomm = conn.CreateCommand();
            
            mycomm.CommandText = "select * into ɭ����;�ֵ� from [" + mypath + "].ɭ����;�ֵ�";
            try
            {
                mycomm.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                e.Data.Clear();
                return;
            }


        }
        //added by chulili 
        //�������ܣ�����һ���µĵ����� �������������������  �ο���featurelayer ���������ڹ����ռ�  CLSID CLSEXT �����༸������
        //����������������
        //������Դ�����ͬ�´���  
        private static IFeatureClass CreateFeatureClass(string name, IFeatureLayer pfeaturelayer, IWorkspace pWorkspace, UID uidCLSID, UID uidCLSEXT, esriGeometryType GeometryType)
        {
            try
            {
                if (uidCLSID == null)
                {
                    uidCLSID = new UIDClass();
                    switch (pfeaturelayer.FeatureClass.FeatureType)
                    {
                        case (esriFeatureType.esriFTSimple):
                            uidCLSID.Value = "{52353152-891A-11D0-BEC6-00805F7C4268}";
                            break;
                        case (esriFeatureType.esriFTSimpleJunction):
                            GeometryType = esriGeometryType.esriGeometryPoint;
                            uidCLSID.Value = "{CEE8D6B8-55FE-11D1-AE55-0000F80372B4}";
                            break;
                        case (esriFeatureType.esriFTComplexJunction):
                            uidCLSID.Value = "{DF9D71F4-DA32-11D1-AEBA-0000F80372B4}";
                            break;
                        case (esriFeatureType.esriFTSimpleEdge):
                            GeometryType = esriGeometryType.esriGeometryPolyline;
                            uidCLSID.Value = "{E7031C90-55FE-11D1-AE55-0000F80372B4}";
                            break;
                        case (esriFeatureType.esriFTComplexEdge):
                            GeometryType = esriGeometryType.esriGeometryPolyline;
                            uidCLSID.Value = "{A30E8A2A-C50B-11D1-AEA9-0000F80372B4}";
                            break;
                        case (esriFeatureType.esriFTAnnotation):
                            GeometryType = esriGeometryType.esriGeometryPolygon;
                            uidCLSID.Value = "{E3676993-C682-11D2-8A2A-006097AFF44E}";
                            break;
                        case (esriFeatureType.esriFTDimension):
                            GeometryType = esriGeometryType.esriGeometryPolygon;
                            uidCLSID.Value = "{496764FC-E0C9-11D3-80CE-00C04F601565}";
                            break;
                    }
                }

                // ���� uidCLSEXT (if Null)
                if (uidCLSEXT == null)
                {
                    switch (pfeaturelayer.FeatureClass.FeatureType)
                    {
                        case (esriFeatureType.esriFTAnnotation):
                            uidCLSEXT = new UIDClass();
                            uidCLSEXT.Value = "{24429589-D711-11D2-9F41-00C04F6BC6A5}";
                            break;
                        case (esriFeatureType.esriFTDimension):
                            uidCLSEXT = new UIDClass();
                            uidCLSEXT.Value = "{48F935E2-DA66-11D3-80CE-00C04F601565}";
                            break;
                    }
                }

                IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspace;
                IFields pFields = new FieldsClass();
                IFieldsEdit pFieldsEdit = pFields as IFieldsEdit;
                //Ϊ����������ֶ�
                for (int i = 0; i < pfeaturelayer.FeatureClass.Fields.FieldCount; i++)
                {
                    IClone pClone = pfeaturelayer.FeatureClass.Fields.get_Field(i) as IClone;
                    IField pTempField = pClone.Clone() as IField;
                    IFieldEdit pTempFieldEdit = pTempField as IFieldEdit;
                    string strFieldName = pTempField.Name;
                    string[] strFieldNames = strFieldName.Split('.');

                    if (pFieldsEdit.FindField(strFieldNames[strFieldNames.GetLength(0) - 1]) > -1) continue;

                    pTempFieldEdit.Name_2 = strFieldNames[strFieldNames.GetLength(0) - 1];
                    pFieldsEdit.AddField(pTempField);
                }

                string strShapeFieldName = pfeaturelayer.FeatureClass.ShapeFieldName;
                string[] strShapeNames = strShapeFieldName.Split('.');
                strShapeFieldName = strShapeNames[strShapeNames.GetLength(0) - 1];


                IFeatureClass targetFeatureclass = pFeatureWorkspace.CreateFeatureClass("" + name + "", pFields, uidCLSID, uidCLSEXT, pfeaturelayer.FeatureClass.FeatureType, strShapeFieldName, "");

                return targetFeatureclass;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Cannot create a low precision dataset in a high precision database.")
                {
                    MessageBox.Show("���ݱ�����ArcGis9.3�����ݣ��뽫���ݴ����ArcGis9.2�����ݣ�");
                }
            }
            IFeatureClass featureclass = null;
            return featureclass;
        }
        //added by chulili
        //�������ܣ�����PDB�����ռ�  ��������������ռ������ļ���·��  �����ռ����� ���������������ռ�
        //������Դ�����ͬ�´���
        public static IWorkspace CreatePDBWorkSpace(string path,string filename)
        {
            IWorkspaceFactory pWorkspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.AccessWorkspaceFactoryClass();
            if (System.IO.File.Exists(filename))
            {
                if (pWorkspaceFactory.IsWorkspace(filename))
                {
                    IWorkspace pTempWks = pWorkspaceFactory.OpenFromFile(filename, 0);
                    pWorkspaceFactory = null;
                    return pTempWks;
                }
            }

            IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create("" + path + "", "" + filename + "", null, 0);
            IName name = (ESRI.ArcGIS.esriSystem.IName)pWorkspaceName;
            IWorkspace PDB_workspace = (IWorkspace)name.Open();
            pWorkspaceFactory = null;
            return PDB_workspace;

        }
    }
}
