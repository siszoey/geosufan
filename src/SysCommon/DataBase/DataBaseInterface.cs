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

namespace SysCommon.DataBase
{
    public interface IDataBase
    {
        //��������
        void SetDbConnection(string strConnectionString, enumDBConType DBConType, enumDBType DBType, out Exception eError);
        //�ر�����
        void CloseDbConnection();

        //�����ݿ��д�����
        bool CreateTable(string strTableName, Dictionary<string, string> dic, out Exception eError);

        //ִ��SQL����½����޸ġ�ɾ������
        bool UpdateTable(string strSQL, out Exception eError);

        //��ȡ���ݿ������б���
        ArrayList GetTablesName();
        //��ȡ���ݿ������л��������Ϣ
        DataTable GetTableSchema();
        //��ȡ���ݿ���������ͼ����Ϣ
        DataTable GetViewSchema();
    }

    public interface ITable
    {
        //��ȡ���ݿ������еı�
        DataSet GetAllTables();

        //��ȡ��
        DataTable GetTable(string tablename, out Exception eError);
        DataTable GetTable(string tablename, string condition, out Exception eError);
    }
    
    public interface ICommon
    {
    }
}
