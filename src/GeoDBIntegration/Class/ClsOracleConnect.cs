using System;
using System.Data;
using System.Data.OracleClient;
using System.Web.UI.WebControls;
namespace GeoDBIntegration
{
    /// <summary>
    /// ���ݿ�ͨ�ò�����
    /// </summary>
    public class ClsDatabase
    {
        protected OracleConnection con;//���Ӷ���


        public ClsDatabase(string constr)
        {
            con = new OracleConnection(constr);
        }
        #region �����ݿ�����
        /// <summary>
        /// �����ݿ�����
        /// </summary>
        public void Open()
        {
            //�����ݿ�����
            if (con.State == ConnectionState.Closed)
            {
                try
                {
                    //�����ݿ�����
                    con.Open();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        #endregion
        #region �ر����ݿ�����
        /// <summary>
        /// �ر����ݿ�����
        /// </summary>
        public void Close()
        {
            //�ж����ӵ�״̬�Ƿ��Ѿ���
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
        }
        #endregion
        #region ִ�в�ѯ��䣬����OracleDataReader ( ע�⣺���ø÷�����һ��Ҫ��OracleDataReader����Close )
        /// <summary>
        /// ִ�в�ѯ��䣬����OracleDataReader ( ע�⣺���ø÷�����һ��Ҫ��OracleDataReader����Close )
        /// </summary>
        /// <param name="sql">��ѯ���</param>
        /// <returns>OracleDataReader</returns>   
        public OracleDataReader ExecuteReader(string sql)
        {
            OracleDataReader myReader;
            Open();
            OracleCommand cmd = new OracleCommand(sql, con);
            myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return myReader;
        }
        #endregion
        #region ִ�д�������SQL���
        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="sql">SQL���</param>
        /// <returns>Ӱ��ļ�¼��</returns>   
        public int ExecuteSql(string sql, params OracleParameter[] cmdParms)
        {
            OracleCommand cmd = new OracleCommand();
            {
                try
                {
                    //PrepareCommand(cmd, con, null, sql, cmdParms);
                    //int rows = cmd.ExecuteNonQuery();
                    //cmd.Parameters.Clear();
                  //  return rows;
                    return 0;
                }
                catch (System.Data.OracleClient.OracleException e)
                {
                    throw e;
                }
            }
        }
        #endregion
        #region ִ�в���������SQL���
        /// <summary>
        /// ִ�в���������SQL���
        /// </summary>
        /// <param name="sql">SQL���</param>     
        public void ExecuteSql(string sql)
        {
            OracleCommand cmd = new OracleCommand(sql, con);
            try
            {
                Open();
             //   OracleString oras=null;
             //   cmd.ExecuteOracleNonQuery(out oras);
                cmd.ExecuteNonQuery();
                Close();
            }
            catch (System.Data.OracleClient.OracleException e)
            {
                Close();
                throw e;
            }
        }
        #endregion
        #region ִ��SQL��䣬�������ݵ�DataSet��
        /// <summary>
        /// ִ��SQL��䣬�������ݵ�DataSet��
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <returns>����DataSet</returns>
        public DataSet GetDataSet(string sql)
        {
            DataSet ds = new DataSet();
            try
            {
                Open();//����������
                OracleDataAdapter adapter = new OracleDataAdapter(sql, con);
                adapter.Fill(ds);
            }
            catch//(Exception ex)
            {
            }
            finally
            {
                Close();//�ر����ݿ�����
            }
            return ds;
        }
        #endregion
        #region ִ��SQL��䣬�������ݵ��Զ���DataSet��
        /// <summary>
        /// ִ��SQL��䣬�������ݵ�DataSet��
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <param name="DataSetName">�Զ��巵�ص�DataSet����</param>
        /// <returns>����DataSet</returns>
        public DataSet GetDataSet(string sql, string DataSetName)
        {
            DataSet ds = new DataSet();
            Open();//����������
            OracleDataAdapter adapter = new OracleDataAdapter(sql, con);
            adapter.Fill(ds, DataSetName);
            Close();//�ر����ݿ�����
            return ds;
        }
        #endregion
        #region ִ��Sql���,���ش���ҳ���ܵ��Զ���dataset
        /// <summary>
        /// ִ��Sql���,���ش���ҳ���ܵ��Զ���dataset
        /// </summary>
        /// <param name="sql">Sql���</param>
        /// <param name="PageSize">ÿҳ��ʾ��¼��</param>
        /// <param name="CurrPageIndex">��ǰҳ</param>
        /// <param name="DataSetName">����dataset����</param>
        /// <returns>����DataSet</returns>
        public DataSet GetDataSet(string sql, int PageSize, int CurrPageIndex, string DataSetName)
        {
            DataSet ds = new DataSet();
            Open();//����������
            OracleDataAdapter adapter = new OracleDataAdapter(sql, con);
            adapter.Fill(ds, PageSize * (CurrPageIndex - 1), PageSize, DataSetName);
            Close();//�ر����ݿ�����
            return ds;
        }
        #endregion
        #region ִ��SQL��䣬���ؼ�¼����
        /// <summary>
        /// ִ��SQL��䣬���ؼ�¼����
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <returns>���ؼ�¼������</returns>
        public int GetRecordCount(string sql)
        {
            int recordCount = 0;
            Open();//����������
            OracleCommand command = new OracleCommand(sql, con);
            OracleDataReader dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                recordCount++;
            }
            dataReader.Close();
            Close();//�ر����ݿ�����
            return recordCount;
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public DataTable GetSQLTable(string SQL, out Exception ex)
        {
            ex = null;
            try
            {
                Open();
                OracleDataAdapter adapter = new OracleDataAdapter(SQL, con);
                DataTable getTable=new DataTable();
                adapter.Fill(getTable);
                Close();
                return getTable;
            }
            catch(Exception error)
            {
                ex = error;
                return null;
            }
            finally
            {
                Close();
            }
        }
        #region ͳ��ĳ���¼����
        /// <summary>
        /// ͳ��ĳ���¼����
        /// </summary>
        /// <param name="KeyField">����/������</param>
        /// <param name="TableName">���ݿ�.�û���.����</param>
        /// <param name="Condition">��ѯ����</param>
        /// <returns>���ؼ�¼����</returns> 
        public int GetRecordCount(string keyField, string tableName, string condition)
        {
            int RecordCount = 0;
            string sql = "select count(" + keyField + ") as count from " + tableName + " " + condition;
            DataSet ds = GetDataSet(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            ds.Clear();
            ds.Dispose();
            return RecordCount;
        }
        /// <summary>
        /// ͳ��ĳ���¼����
        /// </summary>
        /// <param name="Field">���ظ����ֶ�</param>
        /// <param name="tableName">���ݿ�.�û���.����</param>
        /// <param name="condition">��ѯ����</param>
        /// <param name="flag">�ֶ��Ƿ�����</param>
        /// <returns>���ؼ�¼����</returns> 
        public int GetRecordCount(string Field, string tableName, string condition, bool flag)
        {
            int RecordCount = 0;
            if (flag)
            {
                RecordCount = GetRecordCount(Field, tableName, condition);
            }
            else
            {
                string sql = "select count(distinct(" + Field + ")) as count from " + tableName + " " + condition;
                DataSet ds = GetDataSet(sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    RecordCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                }
                ds.Clear();
                ds.Dispose();
            }
            return RecordCount;
        }
        #endregion
        #region ͳ��ĳ���ҳ����
        /// <summary>
        /// ͳ��ĳ���ҳ����
        /// </summary>
        /// <param name="keyField">����/������</param>
        /// <param name="tableName">����</param>
        /// <param name="condition">��ѯ����</param>
        /// <param name="pageSize">ҳ��</param>
        /// <param name="RecordCount">��¼����</param>
        /// <returns>���ط�ҳ����</returns> 
        public int GetPageCount(string keyField, string tableName, string condition, int pageSize, int RecordCount)
        {
            int PageCount = 0;
            PageCount = (RecordCount % pageSize) > 0 ? (RecordCount / pageSize) + 1 : RecordCount / pageSize;
            if (PageCount < 1) PageCount = 1;
            return PageCount;
        }
        /// <summary>
        /// ͳ��ĳ���ҳ����
        /// </summary>
        /// <param name="keyField">����/������</param>
        /// <param name="tableName">����</param>
        /// <param name="condition">��ѯ����</param>
        /// <param name="pageSize">ҳ��</param>
        /// <returns>����ҳ������</returns> 
        public int GetPageCount(string keyField, string tableName, string condition, int pageSize, ref int RecordCount)
        {
            RecordCount = GetRecordCount(keyField, tableName, condition);
            return GetPageCount(keyField, tableName, condition, pageSize, RecordCount);
        }
        /// <summary>
        /// ͳ��ĳ���ҳ����
        /// </summary>
        /// <param name="Field">���ظ����ֶ�</param>
        /// <param name="tableName">����</param>
        /// <param name="condition">��ѯ����</param>
        /// <param name="pageSize">ҳ��</param>
        /// <param name="flag">�Ƿ�����</param>
        /// <returns>����ҳҳ����</returns> 
        public int GetPageCount(string Field, string tableName, string condition, ref int RecordCount, int pageSize, bool flag)
        {
            RecordCount = GetRecordCount(Field, tableName, condition, flag);
            return GetPageCount(Field, tableName, condition, pageSize, ref RecordCount);
        }
        #endregion
        #region Sql��ҳ����
        /// <summary>
        /// �����ҳ��ѯSQL���
        /// </summary>
        /// <param name="KeyField">����</param>
        /// <param name="FieldStr">������Ҫ��ѯ���ֶ�(field1,field2...)</param>
        /// <param name="TableName">����.ӵ����.����</param>
        /// <param name="where">��ѯ����1(where ...)</param>
        /// <param name="order">��������2(order by ...)</param>
        /// <param name="CurrentPage">��ǰҳ��</param>
        /// <param name="PageSize">ҳ��</param>
        /// <returns>SQL���</returns> 
        public string JoinPageSQL(string KeyField, string FieldStr, string TableName, string Where, string Order, int CurrentPage, int PageSize)
        {
            string sql = null;
            if (CurrentPage == 1)
            {
                sql = "select  " + CurrentPage * PageSize + " " + FieldStr + " from " + TableName + " " + Where + " " + Order + " ";
            }
            else
            {
                sql = "select * from (";
                sql += "select  " + CurrentPage * PageSize + " " + FieldStr + " from " + TableName + " " + Where + " " + Order + ") a ";
                sql += "where " + KeyField + " not in (";
                sql += "select  " + (CurrentPage - 1) * PageSize + " " + KeyField + " from " + TableName + " " + Where + " " + Order + ")";
            }
            return sql;
        }
        /// <summary>
        /// �����ҳ��ѯSQL���
        /// </summary>
        /// <param name="Field">�ֶ���(������)</param>
        /// <param name="TableName">����.ӵ����.����</param>
        /// <param name="where">��ѯ����1(where ...)</param>
        /// <param name="order">��������2(order by ...)</param>
        /// <param name="CurrentPage">��ǰҳ��</param>
        /// <param name="PageSize">ҳ��</param>
        /// <returns>SQL���</returns> 
        public string JoinPageSQL(string Field, string TableName, string Where, string Order, int CurrentPage, int PageSize)
        {
            string sql = null;
            if (CurrentPage == 1)
            {
                sql = "select rownum " + CurrentPage * PageSize + " " + Field + " from " + TableName + " " + Where + " " + Order + " group by " + Field;
            }
            else
            {
                sql = "select * from (";
                sql += "select rownum " + CurrentPage * PageSize + " " + Field + " from " + TableName + " " + Where + " " + Order + " group by " + Field + " ) a ";
                sql += "where " + Field + " not in (";
                sql += "select rownum " + (CurrentPage - 1) * PageSize + " " + Field + " from " + TableName + " " + Where + " " + Order + " group by " + Field + ")";
            }
            return sql;
        }
        #endregion
    }
}
