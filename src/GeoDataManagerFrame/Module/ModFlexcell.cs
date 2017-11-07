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

using System.Data.OleDb;
namespace GeoDataManagerFrame
{
    class ModFlexcell
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
            if (ModTableFun.isExist(conn, TableName) == false)
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
                if (ModTableFun.isFieldExist(conn,TableName,OrderByField)==false)
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
        //��������
        public static FormFlexcell SendDataToFlexcell(string connstr, string caption, string TableName, string Fieldstr, string OrderByField, string TemplateFile, int startrow, int startcol)
        {
            return SendDataToFlexcell(connstr,caption,TableName,Fieldstr,OrderByField, TemplateFile,startrow,startcol,"����",9);
        }

    }
}
