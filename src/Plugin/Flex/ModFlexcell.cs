//*********************************************************************************
//** �ļ�����ModFlexcell.cs
//** CopyRight (c) 2000-2007 �人������Ϣ���̼������޹�˾���̲�
//** �����ˣ�chulili
//** ��  �ڣ�2011-03
//** �޸��ˣ�
//** ��  �ڣ�
//** ��  �������ڱ������
//**
//** ��  ����1.0
//*********************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.Data.OleDb;
namespace Plugin.Flex
{
    public static class ModFlexcell
    {
        public static int m_startCol;       //��ʼ��
        public static int m_startRow;       //��ʼ��
        public static int m_SpecialRow;     //�����У���Ҫ�����޸�ĳһ�е���ʾʱ���õ���ͨ������¶������������
        public static int m_SpecialRow_ex;  //�����У���Ҫ�����޸�ĳһ�е���ʾʱ���õ���ͨ������¶������������
        public static int m_SpecialRow_ex2; //�����У���Ҫ�����޸�ĳһ�е���ʾʱ���õ���ͨ������¶������������
        //added by chulili
        //�������ܣ�������д������У����ݴ��������ݿ��һ�ű��У�
        //������������ݿ����Ӵ�������Ի�����⣬����Դ������ָ��������ֶΣ������ֶΣ��������������У�������ģ��ȫ·������ʼ�У���ʼ�У����壬�ֺ�
        public static FormFlexcell  SendDataToFlexcell(string connstr,string caption,string TableName,string Fieldstr,string OrderByField,string TemplateFile,int startrow,int startcol,string FontName,int FontSize)
        {
            //�������Ӵ��������ݿ�
            OleDbConnection conn = new OleDbConnection(connstr );
            if (conn == null)
                return null;
            conn.Open();
            //�ж�����Դ���Ƿ����
            if (isExist(conn, TableName) == false)
                return null;
            //������ʼ�У���ʼ��
            m_startCol = startcol;
            m_startRow = startrow;
            //��ʼ������Ի���
            FormFlexcell frm = new FormFlexcell();
            //��ȡ����Դ��¼����OleDbDataReader�����һ��ȱ����û��recordcount���Ի򷽷�����Ҫ����ȡ��¼����
            int recordcount=0;
            OleDbDataReader myreader;
            OleDbCommand mycomm = conn.CreateCommand();
            mycomm.CommandText = "select count(*) from " + TableName;
            myreader = mycomm.ExecuteReader();
            if (myreader.Read())
                recordcount = (int)myreader.GetValue(0);
            myreader.Close();
            //�ж��Ƿ����ȫ���ֶ�
            if (Fieldstr.Equals(""))
                Fieldstr="*";
            //�ж������ֶ��Ƿ����
            if (OrderByField.Equals("")==false)
                if (isFieldExist(conn,TableName,OrderByField)==false)
                    OrderByField="";
            string sqlstr;
            //�����ȡ���ݵ�sql���
            if (OrderByField.Equals (""))
            {
                sqlstr="select "+Fieldstr +" from " + TableName ;
            }
            else
            {
                sqlstr="select "+Fieldstr +" from " + TableName +" order by "+OrderByField ;
            }
            mycomm.CommandText=sqlstr;
            myreader=mycomm.ExecuteReader();
            //�򱨱�Ի�����������
            frm.SetTextFromRS(startrow,startcol,myreader,TemplateFile,recordcount,FontName,FontSize);
            if (caption.Equals("") )
                caption=TableName;
            frm.Text = caption;
            myreader.Close();

            conn.Close();
            return frm;
        }
        public static void SendTableToExcel(FormFlexcell pFrm,IWorkspace pWks, string TableName,string condition,string ExcelFileName)
        {
            if (pFrm == null)
                return;
            if (pWks == null)
                return;
            if (TableName.Equals(""))
                return; 
            IFeatureWorkspace pFeaWks=pWks as IFeatureWorkspace;
            ITable pTable=pFeaWks.OpenTable(TableName );
            if(pTable==null)
            {
                return;
            }
            
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = condition;

            AxFlexCell.AxGrid pGrid = pFrm.GetGrid();
            pGrid.Cols = pTable.Fields.FieldCount + 1;
            pGrid.Rows = pTable.RowCount(pQueryFilter) + 2;
            pGrid.AutoRedraw = false;

            ICursor pCursor = pTable.Search(pQueryFilter, true);
            int rowid = 1;
            
            for (int i = 0; i < pTable.Fields.FieldCount; i++)
            {
                pGrid.Cell(rowid, i + 1).Text = pTable.Fields.get_Field(i).AliasName;
            }
            rowid = rowid + 1;
            if (pCursor != null)
            {
                IRow row = pCursor.NextRow();
                while (row != null)
                {
                    for (int i = 0; i < pTable.Fields.FieldCount; i++)
                    {
                        object obj = row.get_Value(i);
                        pGrid.Cell(rowid, i + 1).Text = obj.ToString();
                    }
                    rowid = rowid + 1;
                    row = pCursor.NextRow();
                }
            }
            pGrid.AutoRedraw = true;
            pGrid.ExportToExcel(ExcelFileName );

        }
        //��������
        public static FormFlexcell SendDataToFlexcell(string connstr, string caption, string TableName, string Fieldstr, string OrderByField, string TemplateFile, int startrow, int startcol)
        {
            return SendDataToFlexcell(connstr,caption,TableName,Fieldstr,OrderByField, TemplateFile,startrow,startcol,"����",9);
        }
        //�������ܣ��жϱ��Ƿ����  
        //������������ݿ����� ����  ���������������
        public static bool isExist(OleDbConnection conn, string TableName)
        {
            OleDbCommand comm = conn.CreateCommand();
            comm.CommandText = "select * from " + TableName + " where 1=0";
            OleDbDataReader myreader;
            //���ݴ��󱣻��жϱ��Ƿ����
            try
            {
                myreader = comm.ExecuteReader();
                myreader.Close();
                return true;
            }
            //�������ʾ������
            catch (System.Exception e)
            {
                e.Data.Clear();
                return false;
            }

        }
        //�жϱ����Ƿ����ĳ���ֶΡ�
        //������������ݿ�����  ����  �ֶ���  ���������������

        public static bool isFieldExist(OleDbConnection conn, string TableName, string FieldName)
        {
            OleDbCommand comm = conn.CreateCommand();
            comm.CommandText = "select " + FieldName + " from " + TableName + " where 1=0";
            OleDbDataReader myreader;
            //���ݴ��󱣻��ж�ĳ���ֶ��Ƿ����
            try
            {
                myreader = comm.ExecuteReader();
                myreader.Close();
                return true;
            }
            //�������ʾ����ֶβ�����
            catch (System.Exception e)
            {
                e.Data.Clear();
                return false;
            }
        }

    }
}
