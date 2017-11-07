using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Office.Interop.Word;

//*********************************************************************************
//** �ļ�����frmDetailLog.cs
//** CopyRight (c) �人������Ϣ���̼������޹�˾�����������
//** �����ˣ�ϯʤ
//** ��  �ڣ�20011-03-11
//** �޸��ˣ�yjl
//** ��  �ڣ�
//** ��  ����
//**
//** ��  ����1.0
//*********************************************************************************
namespace GeoDataCenterFunLib
{  
    public partial class frmDetailLog : DevComponents.DotNetBar.Office2007Form
    {
        public string m_strLogFilePath;
        public frmDetailLog(string strFilePath)
        {
            InitializeComponent();
            m_strLogFilePath = strFilePath;
        }
        LogFile log;
        private void btnSearch_Click(object sender, EventArgs e)
        {
            //log = new LogFile(null, m_strLogFilePath);
            string whereclause="";
            whereclause+=(dateTimePickerStart.Text =="")?"":"logTime > '"+Convert.ToDateTime(dateTimePickerStart.Text).ToString("yyyy-MM-dd HH:mm:ss")+"' AND "; 
            whereclause+=(dateTimePickerEnd.Text =="")?"":"logTime < '"+Convert.ToDateTime(dateTimePickerEnd.Text+" 23:59:59").ToString("yyyy-MM-dd HH:mm:ss")+"' AND "; 
            whereclause+=(cBoxUser.Text =="")?"":"logUser = '"+cBoxUser.Text+"' AND "; 
            whereclause+=(cBoxAccessIP.Text=="")?"":"logIP = '"+cBoxAccessIP.Text+"'";
            if(whereclause.EndsWith(" AND "))
                whereclause=whereclause.Substring(0,whereclause.Length-5);
            listView.Items.Clear();
            List<string[]> list = LogTable.SeachLog(whereclause);
           
            if (list.Count == 0)
            {
                MessageBox.Show("û�з�����������־", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            for(int i=0;i<list.Count;i++)
            {
                string[] strRow = list[i];
                listView.Items.Add(strRow[0]);
                listView.Items[i].SubItems.Add(strRow[1]);
                listView.Items[i].SubItems.Add(strRow[2]);
                listView.Items[i].SubItems.Add(strRow[3]);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            //log = new LogFile(null, m_strLogFilePath);
            //log.ClearLog(this.listView);
            if (listView.Items.Count == 0)
                return;
            if( MessageBox.Show("��ȷ��Ҫ����б���������־�𣿣���պ󲻿ɻָ���", "��ʾ", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information)!=DialogResult.OK)
                return;
            LogTable.ClearLog(listView);
            btnSearch_Click(sender,e);
        }

        private void frmDetailLog_Load(object sender, EventArgs e)
        {
            ////log = new LogFile(null, m_strLogFilePath);
            listView.Items.Clear();
            dateTimePickerStart.Value = DateTime.Today;
            dateTimePickerEnd.Value = DateTime.Today;
            btnSearch_Click(sender, e);
            //List<string> list = log.SeachLog();
            //for (int i = 0; i < list.Count; i++)
            //{
            //    string[] array = list[i].Split('/');
            //    listView.Items.Add(array[0]);
            //    listView.Items[i].SubItems.Add(array[1]);
            //}
            List<string> distinctUser = LogTable.SeachLog2("logUser");
            cBoxUser.Items.Clear();
            foreach (string user in distinctUser)
            {
                cBoxUser.Items.Add(user);
            }
            List<string> distinctIP = LogTable.SeachLog2("logIP");
            cBoxAccessIP.Items.Clear();
            foreach (string IP in distinctIP)
            {
                cBoxAccessIP.Items.Add(IP);
            }
           
        }

        //����
        private void btn_Export_Click(object sender, EventArgs e)
        {
            //log = new LogFile(null, m_strLogFilePath);
            //FolderBrowserDialog dg=new FolderBrowserDialog();
            //if (dg.ShowDialog() == DialogResult.OK)
            //{
            //    log.ExportLog(dg.SelectedPath + "\\log.txt");
            //    Stream pStream=File.op
            //}
            LogTable.ExportLog("��־��¼",listView);
        }
        //ɾ��ѡ����־
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                return;
            if (MessageBox.Show("��ȷ��Ҫɾ���б�����ѡ�����־�𣿣�ɾ���󲻿ɻָ���", "��ʾ", MessageBoxButtons.OKCancel,
                 MessageBoxIcon.Information) != DialogResult.OK)
                return;
            LogTable.DeleteSelectedLog(listView);
            btnSearch_Click(sender,e);
        }

      
    }
}