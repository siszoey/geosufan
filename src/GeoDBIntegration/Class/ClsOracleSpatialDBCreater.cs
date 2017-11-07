using System;
using System.Collections.Generic;
using System.Text;
using SCHEMEMANAGERCLASSESLib;
using System.Data.OracleClient;
using System.IO;
using System.Data;
using System.Windows.Forms;

namespace GeoDBIntegration
{
    class ClsOracleSpatialDBCreater
    {
        /*
         * guozheng added 2010-5-...
         * ����ʵ��OracleSpatial�ռ��Ŀ��崴��
         * ��Ҫ��ȡOracle �ķ��������û����������Լ�Geoone�������÷����ļ�
         * 
         */
        private string _Server = string.Empty;///////Oracle������
        public string Server
        {
            get { return this._Server; }
            set { this._Server = value; }
        }
        private string _User = string.Empty;////////Oracle�û���
        public string Usr
        {
            get { return this._User; }
            set { this._User = value; }
        }
        private string _Password = string.Empty;//////Oracle�û�����
        public string Password
        {
            get { return this._Password; }
            set { this._Password = value; }
        }

        private ClsDatabase m_DataBaseOper = null;////////���ݿ��������

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="ServerName">Oracle������</param>
        /// <param name="UserID">Oracle�û���</param>
        /// <param name="Passw">Oracle�û�����</param>
        public ClsOracleSpatialDBCreater(string ServerName, string UserID, string Passw)
        {
            this._Password = Passw;
            this._Server = ServerName;
            this._User = UserID;
            OracleConnectionStringBuilder constrbuild = new OracleConnectionStringBuilder();
            constrbuild.DataSource = ServerName;
            constrbuild.UserID = UserID;
            constrbuild.Password = Passw;
            /////////////ʵ�������ݿ��������
            m_DataBaseOper = new ClsDatabase(constrbuild.ConnectionString);
        }
        /// <summary>
        /// �������彨��
        /// </summary>
        /// <param name="DbTemplatePath">�������÷���ģ��·��</param>
        /// <param name="IsHisDb">�Ƿ�Ϊ��ʷ����</param>
        /// <param name="ex"></param>
        public void CreateDataBase(string DbTemplatePath, bool IsHisDb, out Exception ex)
        {
            FrmProcessBar ProcFrm = new FrmProcessBar();
            ProcFrm.Show();
            ex = null;
            //***********************************************
            //guozheng added ��������ͽ����SQL�ṩ�û�ѡ��
            Dictionary<string, string> TableDic = new Dictionary<string, string>();
            //***********************************************
            ISchemeProject m_pProject = null;
            int m_DBScale = -1;/////��������Ϣ
            ProcFrm.SetFrmProcessBarText("���ڼ������÷���");
            #region �������÷���
            List<string> ltablename = new List<string>();
            try
            {
                m_pProject = new SchemeProjectClass();     //����ʵ��
                int index = DbTemplatePath.LastIndexOf('.');
                string lastName = DbTemplatePath.Substring(index + 1);
                if (lastName == "mdb")
                {
                    m_pProject.Load(DbTemplatePath, e_FileType.GO_SCHEMEFILETYPE_MDB);    //����schema�ļ�
                }
                else if (lastName == "gosch")
                {
                    m_pProject.Load(DbTemplatePath, e_FileType.GO_SCHEMEFILETYPE_GOSCH);    //����schema�ļ�
                }
                else
                {
                    ex = new Exception("���ݿ����÷����ļ���ʽ���淶������");
                    ProcFrm.Close();
                    return;
                }

                ///������سɹ����ȡ�����߷���true�����򷵻�false
                if (m_pProject != null)
                {
                    string DBScale = m_pProject.get_MetaDataValue("Scale") as string;   //��ȡ��������Ϣ���ܹ����е�Ĭ�ϱ����ߣ�
                    string[] DBScaleArayy = DBScale.Split(':');
                    m_DBScale = Convert.ToInt32(DBScaleArayy[1]);

                }
                else
                {
                    ex = new Exception("�������ݿ����÷����ļ���" + DbTemplatePath + "ʧ�ܣ�");
                    ProcFrm.Close();
                    return;
                }
            }
            catch
            {
                ex = new Exception("�������ݿ����÷����ļ���" + DbTemplatePath + "ʧ�ܣ�");
                ProcFrm.Close();
                return;
            }
            #endregion
            List<string> DataSpace = new List<string>();
            string sDataBaseName = string.Empty;
            string sNow = DateTime.Now.ToLongDateString();
            #region ��ȡ�ֶ���Ϣ
            try
            {
                IChildItemList pProjects = m_pProject as IChildItemList;
                //��ȡ���Կ⼯����Ϣ
                ISchemeItem pDBList = pProjects.get_ItemByName("ATTRDB");
                IChildItemList pDBLists = pDBList as IChildItemList;
                //�������Կ⼯��
                long DBNum = pDBLists.GetCount();
                for (int i = 0; i < DBNum; i++)
                {
                    int m_DSScale = 0;    //��������Ϣ
                    #region ��ȡ������
                    //ȡ�����Կ���Ϣ
                    ISchemeItem pDB = pDBLists.get_ItemByIndex(i);
                    ///��ȡ���ݼ��ı�������Ϣ�������ȡʧ����ȡĬ�ϱ�������Ϣ
                    IAttribute pa = pDB.AttributeList.get_AttributeByName("Scale") as IAttribute;
                    if (pa == null)
                    {
                        m_DSScale = m_DBScale;
                    }
                    else
                    {
                        string[] DBScaleArayy = pa.Value.ToString().Split(':');
                        m_DSScale = Convert.ToInt32(DBScaleArayy[1]);
                    }
                    #endregion
                    IChildItemList pDBs = pDB as IChildItemList;
                    string pDatasetName = pDB.Name;
                    DataSpace.Add(pDatasetName);
                    sDataBaseName = pDatasetName;
                    //////////////////////////////////////������/////////////////////
                    //���������༯��
                    //�������Ա�
                    long TabNum = pDBs.GetCount();
                    ProcFrm.SetFrmProcessBarMax(TabNum);
                    for (int j = 0; j < TabNum; j++)
                    {
                        //��ȡ���Ա���Ϣ
                        ISchemeItem pTable = pDBs.get_ItemByIndex(j);  //��ȡ���Ա����
                        ProcFrm.SetFrmProcessBarValue(j);
                        ProcFrm.SetFrmProcessBarText("���ڻ�ȡ���Ա�");

                        string pFeatureClassName = pTable.Name;     //Ҫ��������
                        string pFeatureClassType = pTable.Value as string;   //Ҫ��������
                        string sTableName = pFeatureClassName;
                        string sTableType = pFeatureClassType;
                        //��õ����������
                        string sField = string.Empty;
                        string sViewField = string.Empty;
                        sField += ModuleData.s_KeyFieldName + " NUMBER PRIMARY KEY,";
                        sViewField += ModuleData.s_KeyFieldName + ",";
                        ///////�����ֶ�
                        sField = sField + ModuleData.s_GeometryFieldName + "  " + "MDSYS.SDO_GEOMETRY,";

                        if (pFeatureClassType == "ANNO")///////ע�ǲ㲻�账��
                            continue;

                        //�����ֶ�
                        IAttributeList pAttrs = pTable.AttributeList;
                        long FNum = pAttrs.GetCount();
                        int lfldcnt = pAttrs.GetCount();
                        int n = 0;
                        for (n = 0; n < lfldcnt; n++)
                        {
                            IAttribute pAttr = pAttrs.get_AttributeByIndex(n);
                            //��ȡ��չ������Ϣ
                            IAttributeDes pAttrDes = pAttr.Description;
                            //���±������������ֶε�����
                            string fieldName = pAttr.Name;      //��¼�ֶ�����
                            string fieldType = pAttr.Type.ToString();   //��¼�ֶ�����
                            int fieldLen = Convert.ToInt32(pAttrDes.InputWidth);     //��¼�ֶγ���
                            bool isNullable = pAttrDes.AllowNull;   //��¼�ֶ��Ƿ������ֵ                
                            if (fieldLen <= 0)
                                fieldLen = 30;
                            int precision = Convert.ToInt32(pAttrDes.PrecisionEx);        //����
                            bool required = bool.Parse(pAttrDes.Necessary.ToString());
                            ////////////////��¼�ֶ����ڴ�����///////////////////                            
                            string sFildType = string.Empty;
                            switch (fieldType)
                            {
                                case "GO_VALUETYPE_STRING":
                                    sFildType = "VARCHAR2(" + fieldLen.ToString() + ")";
                                    break;
                                case "GO_VALUETYPE_LONG":
                                    sFildType = "NUMBER";
                                    break;
                                case "GO_VALUETYPE_DATE":
                                    sFildType = "DATE";
                                    break;
                                case "GO_VALUETYPE_FLOAT":
                                    sFildType = "FLOAT";
                                    break;
                                case "GO_VALUETYPE_DOUBLE":
                                    sFildType = "NUMBER";
                                    break;
                                case "GO_VALUETYPE_BYTE":
                                    sFildType = "BLOB";
                                    break;
                                case "GO_VALUETYPE_BOOL":
                                    sFildType = "CHAR";
                                    break;
                                default:
                                    continue;
                                    break;
                            }

                            if (!string.IsNullOrEmpty(sFildType))
                            {
                                if (fieldType == "GO_VALUETYPE_BOOL")
                                {
                                    sFildType += " CHECK (" + fieldName + " IN('N','Y'))";
                                }
                                else
                                {
                                    //************************************
                                    //guozheng 2010-12-8 added ���ӷǿ��ж�
                                    if (!isNullable)
                                        sFildType += " NOT NULL";
                                    //************************************
                                }
                                sField = sField + " " + fieldName + "  " + sFildType + ",";
                                sViewField += fieldName + ",";
                            }
                            else
                            {
                                continue;
                            }
                        }

                        string sMaxvalue = DateTime.MaxValue.ToLongDateString();
                        ///////////////////////////////������/////////////sTableName,sField
                        ProcFrm.SetFrmProcessBarText("������֯���ֶ�");
                        if (IsHisDb)/////������ʷ�������Ӻ�׺
                        {
                            sTableName = sTableName.Trim() + "_GOH";///////���Ӻ�׺
                            ///////�����ֶ�
                            sField = sField + "FromDate" + "  " + "VARCHAR2(30)" + " " + "DEFAULT('" + sNow + "'),";
                            sField = sField + "ToDate" + "  " + "VARCHAR2(30)" + " " + "DEFAULT('" + sMaxvalue + "'),";
                            sField = sField + "SourceOID" + "  " + "NUMBER" + ",";
                            sField = sField + "State" + "  " + "NUMBER" + ",";
                            sField = sField + "VERSION" + "  " + "NUMBER" + " " + "DEFAULT(0) NOT NULL,";

                        }
                        //////��������SQL���
                        string CreateSQL = string.Empty;
                        if (sTableType != "ANNO")/////����ע�ǲ�
                        {
                            sField = sField.Substring(0, sField.LastIndexOf(","));
                            CreateSQL = "CREATE TABLE " + this._User.Trim() + "." + sTableName.Trim() + " " + "(" + sField + ")";
                        }
                        else////////ע�ǲ�
                        {
                            continue;
                        }
                        //////////
                        ProcFrm.SetFrmProcessBarText("��¼��" + sTableName);
                        //////////
                        if (!IsHisDb)
                        {
                            if (!TableDic.ContainsKey(sTableName.Trim()))
                            {
                                TableDic.Add(sTableName.Trim(), CreateSQL);
                            }
                        }
                        if (IsHisDb)
                        {
                            int index = sTableName.IndexOf("_GOH");
                            if (index < 0) { ex = new Exception("��ʷ�����Ʋ��淶"); return; }
                            string userTableName = sTableName.Substring(0, index);
                            string strsql = "SELECT COUNT(*) FROM " + userTableName;
                            CreateTable(strsql, out ex);////�ж��û����Ƿ����
                            if (ex != null)////�û������ڣ�����Ҫ������ʷ��
                            {
                                continue;
                            }
                            InitialHisTable(sTableName, CreateSQL, sViewField, out ex);
                            //if (!TableDic.ContainsKey(sTableName.Trim()))
                            //{
                            //    TableDic.Add(sTableName.Trim(), CreateSQL);
                            //}
                        }
                        ////////////////////////////////////////////////////////////////                                               
                    }
                }
                ProcFrm.Close();
                if (!IsHisDb)
                {
                    ProcFrm.SetFrmProcessBarText("���ڴ���Զ����־��");
                    /////////����Զ����־��//////
                    string LogSQL = "CREATE TABLE GO_DATABASE_UPDATELOG(OID NUMBER,STATE NUMBER,LAYERNAME VARCHAR2(30),LASTUPDATE DATE,VERSION NUMBER DEFAULT(0) NOT NULL,XMIN NUMBER,XMAX NUMBER,YMIN NUMBER,YMAX NUMBER)";
                    ltablename.Add("GO_DATABASE_UPDATELOG");
                    /////////Զ�̸�����־
                    TableDic.Add("Զ����־��:GO_DATABASE_UPDATELOG", LogSQL);
                    /////////���ݿ�汾��
                    LogSQL = "CREATE TABLE go_database_version(VERSION NUMBER DEFAULT(0) NOT NULL,USERNAME VARCHAR2(30),VERSIONTIME DATE,DES VARCHAR2(30))";
                    ltablename.Add("go_database_version");
                    TableDic.Add("���ݿ�汾��:go_database_version", LogSQL);
                    if (TableDic.Count > 0)
                    {
                        frmChooseTable fChooseTables = new frmChooseTable(TableDic, this._Server, this._User, this._Password);
                        if (DialogResult.OK == fChooseTables.ShowDialog())
                        {
                            Dictionary<string, string> getCreatedTables = fChooseTables.CreatedTable;
                        }
                        else
                        {
                            ex = new Exception("ȡ������");
                            ProcFrm.Close();
                            return;
                        }

                    }

                }
                ProcFrm.Close();
            }
            catch (Exception dd)
            {
                //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", dd.Message);
                ex = dd;
                ProcFrm.Close();
            }

            #endregion
            System.Runtime.InteropServices.Marshal.ReleaseComObject(m_pProject);

        }
        /// <summary>
        /// ִ��ָ����SQL��䣬�����ڽ���ɾ�������û���
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="ex"></param>
        public void CreateTable(string SQL, out Exception ex)
        {
            ex = null;
            if (this.m_DataBaseOper == null)
            {
                ex = new Exception("Oracle������Ϣδ��ʼ����");
                return;
            }
            try
            {
                this.m_DataBaseOper.ExecuteSql(SQL);
                return;
            }
            catch (Exception eRror)
            {
                ex = new Exception("����ʧ�ܣ�ԭ��" + eRror.Message);
                return;
            }
        }
        /// <summary>
        /// ��ʼ����ʷ�����������������Ȼ���û���������Ӧ���е����ݼ���ʱ������Ƶ���ʷ���У�SOURCIDΪԭҪ��OBJECTID��
        /// </summary>
        /// <param name="sTableName">��ʷ����</param>
        /// <param name="SQL">������ʷ���Sql���</param>
        /// <param name="HisFileds">�û����SHAPE�ֶ���������ֶΣ����ڴ�����ͼ</param>
        /// <param name="ex"></param>
        private void InitialHisTable(string sTableName, string SQL, string HisFileds, out Exception ex)
        {
            ex = null;

            if (this.m_DataBaseOper == null)
            {
                ex = new Exception("Oracle������Ϣδ��ʼ����");
                return;
            }
            int index = sTableName.IndexOf("_GOH");
            if (index < 0) { ex = new Exception("��ʷ�����Ʋ��淶"); return; }
            string userTableName = sTableName.Substring(0, index);
            HisFileds = HisFileds.Remove(HisFileds.LastIndexOf(","), 1);/////���ڴ�����ͼ
            #region �����û�����ͼ
            string CreateView = "CREATE OR REPLACE VIEW " + userTableName + "_VIEW(" + HisFileds + ") AS SELECT " + HisFileds + " FROM " + userTableName;
            m_DataBaseOper.ExecuteSql(CreateView);
            #endregion
            string sNow = DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString();
            string strsql = "SELECT COUNT(*) FROM " + sTableName;
            CreateTable(strsql, out ex);////�жϣ���ʷ���Ƿ����
            if (ex != null)////��ʱ������
            {
                CreateTable(SQL, out ex);/////������ʷ��
                if (ex != null) return;
            }
            ex = null;
            //////////���û����е����ݵ��뵽��ʷ����////////////

            strsql = "SELECT * FROM " + userTableName + "_VIEW";
            DataTable gettable = new DataTable();
            gettable = m_DataBaseOper.GetSQLTable(strsql, out ex);
            if (ex != null) return;
            if (gettable != null)
            {
                FrmProcessBar Procbar = new FrmProcessBar();
                Procbar.SetChild();
                Procbar.SetFrmProcessBarMax((long)gettable.Rows.Count);
                for (int i = 0; i < gettable.Rows.Count; i++)
                {
                    Procbar.SetFrmProcessBarValue((long)i);
                    Procbar.Show();
                    Procbar.SetFrmProcessBarText("����ת����" + userTableName + ":��������:" + sTableName);
                    System.Windows.Forms.Application.DoEvents();
                    int iColCount = gettable.Columns.Count;

                    string sFields = string.Empty;
                    string sValue = string.Empty;
                    DataRow getrow = gettable.Rows[i];
                    long rec = 0;
                    //////��ȡ��ʷ���м�¼��Ŀ
                    #region ��ȡ��¼��
                    string getrec = "SELECT COUNT(*) FROM " + sTableName;
                    DataTable rectable = m_DataBaseOper.GetSQLTable(getrec, out ex);
                    if (ex != null) return;
                    if (rectable != null)
                    {
                        try
                        {
                            rec = Convert.ToInt64(rectable.Rows[0][0].ToString()) + 1;
                        }
                        catch
                        {
                            rec = 1;
                        }
                    }
                    #endregion
                    long OID = -1;
                    OID = Convert.ToInt64(getrow[ModuleData.s_KeyFieldName].ToString());
                    //////��ȡ�ֶκ�ֵ
                    #region ��ȡ�ֶκ�ֵ������ͼ����ֶγ��⣩
                    for (int j = 0; j < iColCount; j++)
                    {
                        string sFieldName = gettable.Columns[j].ColumnName;
                        Type gettype = gettable.Columns[j].DataType;
                        string value = string.Empty;
                        if (sFieldName == ModuleData.s_KeyFieldName || sFieldName == ModuleData.s_GeometryFieldName) continue;
                        sFields += sFieldName + ",";
                        if (getrow[j] == null || string.IsNullOrEmpty(getrow[j].ToString()))
                            value = "NULL";
                        else
                        {
                            if (gettype.FullName == "System.String" || gettype.FullName == "System.Char" || gettype.FullName == "System.DateTime")
                            {
                                value = "'" + getrow[j].ToString() + "'";
                            }
                            else
                            {
                                value = getrow[j].ToString();
                            }
                        }
                        sValue += value + ",";

                    }
                    #endregion
                    sFields += ModuleData.s_KeyFieldName + ",";
                    sValue += rec.ToString() + ",";
                    sFields += "SourceOID" + ",";
                    sValue += OID.ToString() + ",";
                    sFields += "FromDate" + ",";
                    sValue += "'" + sNow + "',";
                    sFields += ModuleData.s_GeometryFieldName;
                    sValue += ModuleData.s_GetGeometry + "(" + OID.ToString() + ")";
                    ///////////
                    List<string> SQllist = new List<string>();
                    //   SQllist.Add("declare begin "+ModData.s_SaveShapePack.Trim()+"."+ModData.s_ShapePackSet+"MDSYS.SDO_GEOMETRY(SELECT "+ModData.s_GeometryFieldName+" FROM "+userTableName+" WHERE "+ModData.s_KeyFieldName+"="+OID.ToString()+")");
                    SQllist.Add("CREATE OR REPLACE FUNCTION " + ModuleData.s_GetGeometry + "(id IN NUMBER) RETURN MDSYS.SDO_GEOMETRY AS resgeo MDSYS.SDO_GEOMETRY;BEGIN SELECT " + ModuleData.s_GeometryFieldName + " INTO resgeo FROM " + userTableName + " WHERE " + ModuleData.s_KeyFieldName + "=id;RETURN resgeo;END;");
                    SQllist.Add("INSERT INTO " + sTableName + "(" + sFields + ")" + " VALUES(" + sValue + ")");
                    foreach (string sql in SQllist)
                    {
                        try
                        {
                            m_DataBaseOper.ExecuteSql(sql);

                        }
                        catch (Exception error)
                        {
                            ex = error;

                            continue;
                        }

                    }
                    rec += 1;
                }
                Procbar.Close();
            }

        }
    }
}
