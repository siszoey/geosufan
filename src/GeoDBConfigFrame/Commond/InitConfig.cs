using System;
using System.Collections.Generic;
using System.Text;
using GeoDataCenterFunLib;
using System.Data.OleDb;
using System.Xml;
using System.IO;
using System.Windows.Forms;
//�༭��������
namespace GeoDBConfigFrame
{
    public class InitConfig : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
        public InitConfig()
        {
            base._Name = "GeoDBConfigFrame.InitConfig";
            base._Caption = "��ʼ������";
            base._Tooltip = "��ʼ������";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "��ʼ������";
        }
        public override void OnClick()
        {
            if (MessageBox.Show("ȷ��Ҫ��ʼ�������𣿱���������ղ���ҵ���", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string xmlpath = dIndex.m_strInitXmlPath;
            XmlDocument xmldoc = new XmlDocument();
            XmlNode xmlNode=null;
            if (xmldoc != null)
            {
                if (File.Exists(xmlpath))
                {
                    xmldoc.Load(xmlpath);
                    string strSearch = "//InitConfig";
                    xmlNode = xmldoc.SelectSingleNode(strSearch);
                    if (xmlNode == null) return;

                }
                else
                    return;
            }
            else
                return;

            
            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
                if (log != null)
                {
                    log.Writelog("��ʼ������");

                }
            }
            if (m_Hook.GridCtrl == null)
                return;
            //��ȡ���ݿ����Ӵ��ͱ���
            string dbpath = dIndex.GetDbInfo();
            string connstr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbpath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���

            OleDbConnection mycon = new OleDbConnection(connstr);   //����OleDbConnection����ʵ�����������ݿ�
            mycon.Open();
            OleDbCommand aCommand =mycon.CreateCommand();
            XmlNodeList xmlNdList = xmlNode.ChildNodes;
            foreach (XmlNode xmlChild in xmlNdList)
            {
                XmlElement xmlElent = (XmlElement)xmlChild;
                string strTblName = xmlElent.GetAttribute("ItemName");
                if (isExist(mycon, strTblName) == true)
                {
                    //ִ��ɾ�����
                    aCommand.CommandText = "delete from " + strTblName;
                    aCommand.ExecuteNonQuery();
                }
            }                       
            mycon.Close();
            MessageBox.Show("��ʼ��������ϡ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information );
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
        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppPrivilegesRef;
        }
    }
}
