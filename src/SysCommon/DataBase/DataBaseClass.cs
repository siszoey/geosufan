using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Windows.Forms;
using System.IO;

namespace SysCommon.DataBase
{
    public class SysDataBase : IDataBase
    {
        private enumDBConType _DBConType;
        public enumDBConType DBConType
        {
            get { return _DBConType; }
            set { _DBConType = value; }
        }
        private enumDBType _DBType;
        public enumDBType DBType
        {
            get { return _DBType; }
            set { _DBType = value; }
        }

        public DbConnection _DbConn = null;
        public DbConnection DbConn
        {
            get { return _DbConn; }
            set { _DbConn = value; }
        }

        private DbCommand _DbCom = null;

        private DbTransaction _DBTransaction = null;
        public DbTransaction DBTransaction
        {
            get { return _DBTransaction; }
            set { _DBTransaction = value; }
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="strConnectionString">�����ַ���</param>
        /// <param name="DBConType">���ݿ����ӷ�ʽ</param>
        /// <param name="DBType">���ݿ�����</param>
        /// <param name="eError"></param>
        public void SetDbConnection(string strConnectionString, enumDBConType DBConType, enumDBType DBType, out Exception eError)
        {
            eError = null;

            if (strConnectionString == String.Empty) return;
            _DBConType = DBConType;
            _DBType = DBType;

            try
            {
                switch (_DBConType)
                {
                    case enumDBConType.ODBC:
                        _DbConn = new OdbcConnection(strConnectionString);
                        break;
                    case enumDBConType.OLEDB:
                        _DbConn = new OleDbConnection(strConnectionString);
                        break;
                    case enumDBConType.ORACLE:
                        _DbConn = new OracleConnection(strConnectionString);
                        break;
                    case enumDBConType.SQL:
                        _DbConn = new SqlConnection(strConnectionString);
                        break;
                    default:
                        return;
                }

                if (_DbConn == null) return;
                _DbConn.Open();
            }
            catch (DbException eX)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //******************************************
                eError = eX;
                return;
            }
        }

        /// <summary>
        /// �ر�����
        /// </summary>
        public void CloseDbConnection()
        {
            if (_DbConn == null) return;
            if (_DbConn.State != ConnectionState.Open) return;
            _DbConn.Close();
            _DbConn.Dispose();
        }

        /// <summary>
        /// ��������
        /// </summary>
        public void StartTransaction()
        {
            if (_DbConn == null) return;
            if (_DbConn.State != ConnectionState.Open) return;
            _DBTransaction = _DbConn.BeginTransaction();
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="bSave">�Ƿ��ύ����</param>
        public void EndTransaction(bool bSave)
        {
            if (_DBTransaction == null) return;
            if (bSave == true)
            {
                _DBTransaction.Commit();
            }
            else
            {
                _DBTransaction.Rollback();
            }
        }

        /// <summary>
        /// �����ݿ��д�����
        /// </summary>
        /// <param name="strTableName">����</param>
        /// <param name="dic">�ֶ���,�ֶ�������������</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public bool CreateTable(string strTableName, Dictionary<string, string> dic, out Exception eError)
        {
            eError = null;
            if (_DbConn == null) return false;
            if (_DbConn.State != ConnectionState.Open) return false;

            string sqlCreate = @"creat table " + strTableName + "(";
            foreach (KeyValuePair<string, string> keyvalue in dic)
            {
                sqlCreate = sqlCreate + keyvalue.Key + " " + keyvalue.Value.ToString() + ",";
            }
            sqlCreate = sqlCreate.Remove(sqlCreate.Length - 1);
            sqlCreate += ")";

            try
            {
                switch (_DBConType)
                {
                    case enumDBConType.ODBC:
                        _DbCom = new OdbcCommand(sqlCreate, (OdbcConnection)_DbConn);
                        break;
                    case enumDBConType.OLEDB:
                        _DbCom = new OleDbCommand(sqlCreate, (OleDbConnection)_DbConn);
                        break;
                    case enumDBConType.ORACLE:
                        _DbCom = new OracleCommand(sqlCreate, (OracleConnection)_DbConn);
                        break;
                    case enumDBConType.SQL:
                        _DbCom = new SqlCommand(sqlCreate, (SqlConnection)_DbConn);
                        break;
                    default:
                        return false;
                }

                if (_DbCom == null) return false;
                _DbCom.Transaction = _DBTransaction;
                _DbCom.ExecuteNonQuery();
                _DbCom = null;
                return true;
            }
            catch (Exception ex)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //******************************************
                eError = ex;
                return false;
            }
        }

        /// <summary>
        /// ִ��SQL����½����޸ġ�ɾ��
        /// ��������
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="eError"></param>
        public bool UpdateTable(string strSQL, out Exception eError)
        {
            eError = null;
            if (_DbConn == null) return false;
            if (_DbConn.State != ConnectionState.Open)
            {
                return false;
            }
            try
            {
                switch (_DBConType)
                {
                    case enumDBConType.ODBC:
                        _DbCom = new OdbcCommand(strSQL, (OdbcConnection)_DbConn);
                        break;
                    case enumDBConType.OLEDB:
                        _DbCom = new OleDbCommand(strSQL, (OleDbConnection)_DbConn);
                        break;
                    case enumDBConType.ORACLE:
                        _DbCom = new OracleCommand(strSQL, (OracleConnection)_DbConn);
                        break;
                    case enumDBConType.SQL:
                        _DbCom = new SqlCommand(strSQL, (SqlConnection)_DbConn);
                        break;
                    default:
                        return false;
                }

                if (_DbCom == null) return false;
                _DbCom.Transaction = _DBTransaction;
                _DbCom.ExecuteNonQuery();

                _DbCom = null;
                return true;

            }
            catch (Exception ex)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //******************************************
                eError = ex;
                return false;
            }
        }

        /// <summary>
        /// ��ȡ���ݿ������б���
        /// </summary>
        /// <returns></returns>
        public System.Collections.ArrayList GetTablesName()
        {
            if (_DbConn == null) return null;
            if (_DbConn.State != ConnectionState.Open) return null;

            string strKey = string.Empty;
            switch (_DBType)
            {
                case enumDBType.ACCESS:
                    strKey = "TABLE";
                    break;
                case enumDBType.ORACLE:
                    strKey = "BASE TABLE";
                    break;
                case enumDBType.SQLSERVER:
                    strKey = "TABLE";
                    break;
                default:
                    return null;
            }

            try
            {
                ArrayList pArrayList = new ArrayList();
                DataTable pDataTable = _DbConn.GetSchema("Tables");
                for (int i = 0; i < pDataTable.Rows.Count; i++)
                {
                    if (pDataTable.Rows[i].ItemArray[3].ToString() == strKey)
                    {
                        pArrayList.Add(pDataTable.Rows[i].ItemArray[2].ToString());
                    }
                }

                return pArrayList;
            }
            catch (Exception eX)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //******************************************
                return null;
            }
        }

        /// <summary>
        /// ��ȡ���ݿ������л��������Ϣ
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable GetTableSchema()
        {
            if (_DbConn == null) return null;
            if (_DbConn.State != ConnectionState.Open) return null;

            string strKey = string.Empty;
            switch (_DBType)
            {
                case enumDBType.ACCESS:
                    strKey = "TABLE";
                    break;
                case enumDBType.ORACLE:
                    strKey = "BASE TABLE";
                    break;
                case enumDBType.SQLSERVER:
                    strKey = "TABLE";
                    break;
                default:
                    return null;
            }

            try
            {
                DataTable pDataTable = _DbConn.GetSchema("Tables",
                    new string[] { null, null, null, strKey });
                return pDataTable;
            }
            catch (Exception eX)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //******************************************
                return null;

            }
        }

        /// <summary>
        /// ��ȡ���ݿ���������ͼ����Ϣ
        /// </summary>
        /// <returns></returns>
        public System.Data.DataTable GetViewSchema()
        {
            if (_DbConn == null) return null;
            if (_DbConn.State != ConnectionState.Open) return null;

            try
            {
                DataTable pDataTable = _DbConn.GetSchema("Tables",
                    new string[] { null, null, null, "VIEW" });
                return pDataTable;
            }
            catch (Exception eX)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //******************************************
                return null;
            }
        }
    }

    public class SysTable : SysDataBase, ITable
    {
        private DbDataAdapter _DbDataAdapter = null;

        public SysTable()
        {
        }
        public SysTable(SysDataBase pSysDataBase)
        {
            DBConType = pSysDataBase.DBConType;
            DBType = pSysDataBase.DBType;
            DbConn = pSysDataBase.DbConn;
            DBTransaction = pSysDataBase.DBTransaction;
        }

        /// <summary>
        /// ��ȡ���ݿ������еı�
        /// </summary>
        /// <returns></returns>
        public System.Data.DataSet GetAllTables()
        {
            if (DbConn == null) return null;
            if (DbConn.State != ConnectionState.Open) return null;

            try
            {
                DataTable pTableSchema = GetTableSchema();
                if (pTableSchema == null) { return null; }
                DataSet pDataSet = new DataSet();
                foreach (DataRow row in pTableSchema.Rows)
                {
                    Exception Err = null;
                    DataTable pDataTable = GetTable(row["TABLE_NAME"].ToString(), out Err);
                    if (pDataTable == null) { continue; }
                    pDataSet.Tables.Add(pDataTable);
                }
                return pDataSet;
            }
            catch (Exception eX)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eX);
                //******************************************
                return null;
            }
        }

        /// <summary>
        /// ��ȡ��
        /// </summary>
        /// <param name="tablename">����</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public DataTable GetTable(string tablename, out Exception eError)
        {
            eError = null;

            DataTable pDataTable = new DataTable(tablename);
            try
            {
                switch (DBConType)
                {
                    case enumDBConType.ODBC:
                        _DbDataAdapter = new OdbcDataAdapter(@"select * from " + tablename, (OdbcConnection)DbConn);
                        break;
                    case enumDBConType.OLEDB:
                        _DbDataAdapter = new OleDbDataAdapter(@"select * from " + tablename, (OleDbConnection)DbConn);
                        break;
                    case enumDBConType.ORACLE:
                        _DbDataAdapter = new OracleDataAdapter(@"select * from " + tablename, (OracleConnection)DbConn);
                        break;
                    case enumDBConType.SQL:
                        _DbDataAdapter = new SqlDataAdapter(@"select * from " + tablename, (SqlConnection)DbConn);
                        break;
                    default:
                        return null;
                }

                if (_DbDataAdapter == null) return null;
                _DbDataAdapter.SelectCommand.Transaction = DBTransaction;
                try
                {
                    _DbDataAdapter.Fill(pDataTable);
                }
                catch (Exception eX)
                {
                    //******************************************
                    //guozheng added System Exception log
                    if (SysCommon.Log.Module.SysLog == null)
                        SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    SysCommon.Log.Module.SysLog.Write(eX);
                    //******************************************
                    eError = new Exception("��ȡ���ݳ���!");
                    return null;
                }
                _DbDataAdapter = null;
                return pDataTable;
            }
            catch (Exception ex)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //******************************************
                eError = ex;
                return null;
            }
        }

        /// <summary>
        /// ��ȡ��
        /// </summary>
        /// <param name="tablename">����</param>
        /// <param name="condition">����</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public DataTable GetTable(string tablename, string condition, out Exception eError)
        {
            eError = null;

            DataTable pDataTable = new DataTable(tablename);
            try
            {
                switch (DBConType)
                {
                    case enumDBConType.ODBC:
                        _DbDataAdapter = new OdbcDataAdapter(@"select * from " + tablename + " where " + condition, (OdbcConnection)DbConn);
                        break;
                    case enumDBConType.OLEDB:
                        _DbDataAdapter = new OleDbDataAdapter(@"select * from " + tablename + " where " + condition, (OleDbConnection)DbConn);
                        break;
                    case enumDBConType.ORACLE:
                        _DbDataAdapter = new OracleDataAdapter(@"select * from " + tablename + " where " + condition, (OracleConnection)DbConn);
                        break;
                    case enumDBConType.SQL:
                        _DbDataAdapter = new SqlDataAdapter(@"select * from " + tablename + " where " + condition, (SqlConnection)DbConn);
                        break;
                    default:
                        return null;
                }

                if (_DbDataAdapter == null) return null;
                _DbDataAdapter.SelectCommand.Transaction = DBTransaction;
                _DbDataAdapter.Fill(pDataTable);
                _DbDataAdapter = null;
                return pDataTable;
            }
            catch (Exception ex)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //******************************************
                eError = ex;
                return null;
            }
        }

        /// <summary>
        /// ��ȡ��
        /// </summary>
        /// <param name="strSQL">SQL��ѯ���</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public DataTable GetSQLTable(string strSQL, out Exception eError)
        {
            eError = null;

            try
            {
                DataTable pDataTable = new DataTable();
                switch (DBConType)
                {
                    case enumDBConType.ODBC:
                        _DbDataAdapter = new OdbcDataAdapter(strSQL, (OdbcConnection)DbConn);
                        break;
                    case enumDBConType.OLEDB:
                        _DbDataAdapter = new OleDbDataAdapter(strSQL, (OleDbConnection)DbConn);
                        break;
                    case enumDBConType.ORACLE:
                        _DbDataAdapter = new OracleDataAdapter(strSQL, (OracleConnection)DbConn);
                        break;
                    case enumDBConType.SQL:
                        _DbDataAdapter = new SqlDataAdapter(strSQL, (SqlConnection)DbConn);
                        break;
                    default:
                        return null;
                }

                if (_DbDataAdapter == null) return null;
                _DbDataAdapter.SelectCommand.Transaction = DBTransaction;
                _DbDataAdapter.Fill(pDataTable);
                _DbDataAdapter = null;
                return pDataTable;
            }
            catch (Exception ex)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //******************************************
                eError = ex;
                return null;
            }
        }

        /// <summary>
        /// ��ȡ�� ��������
        /// </summary>
        /// <param name="strSQL">SQL��ѯ���</param>
        /// <param name="eError"></param>
        /// <returns></returns>
        public DataTable GetSQLTable(string strSQL, out Exception eError, string connection, string DBType)
        {
            eError = null;
            DataSet pDataTable = new DataSet();
            DataAdapter DataAdapter = null;
            try
            {

                switch (DBType)
                {
                    case "ACCESS":
                        OleDbConnection DB_Con = new OleDbConnection(connection);//�������ݿ�
                        OleDbCommand cmd = new OleDbCommand(strSQL, DB_Con);
                        DataAdapter = new OleDbDataAdapter(cmd);
                        break;
                    case "ORACLE":
                        OracleConnection ODB_Con = new OracleConnection(connection);//�������ݿ�
                        OracleCommand O_cmd = new OracleCommand(strSQL, ODB_Con);
                        DataAdapter = new OracleDataAdapter(O_cmd);
                        break;
                    case "SQL":
                        SqlConnection SDB_Con = new SqlConnection(connection);//�������ݿ�
                        SqlCommand S_cmd = new SqlCommand(strSQL, SDB_Con);
                        DataAdapter = new SqlDataAdapter(S_cmd);
                        break;
                }

                if (DataAdapter == null) return null;
                DataAdapter.Fill(pDataTable);

                DataAdapter = null;
                return pDataTable.Tables[0];
            }
            catch (Exception ex)
            {
                //******************************************
                //guozheng added System Exception log
                if (SysCommon.Log.Module.SysLog == null)
                    SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(ex);
                //******************************************
                eError = ex;
                return null;
            }
        }
    }
}
