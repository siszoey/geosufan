using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Data;

namespace FileDBTool
{
    public class clsProject
    {
        private string _ProName;////////������
        private string _ftpIP;//////////����ip
        private string _ftpUSER;///////�����û�id
        private string _ftpPassWord;////�����û�����
        private FTP_Class ftp;/////////
        #region ���캯��
        /// <summary>
        /// ���캯������ʼ����������ip,�û�id������
        /// </summary>
        /// <param name="ProName">������</param>
        /// <param name="ftpIP">ip</param>
        /// <param name="ftpUser">�û�id</param>
        /// <param name="PassWord">����</param>
        public clsProject(string ProName,string ftpIP,string ftpUser,string PassWord)
        {
            this._ProName = ProName;
            this._ftpIP = ftpIP;
            this._ftpUSER = ftpUser;
            this._ftpPassWord = PassWord;
            ftp = new FTP_Class(_ftpIP, _ftpUSER, _ftpPassWord);
            ftp.FtpUpDown(_ftpIP, _ftpUSER, _ftpPassWord);          
        }
        #endregion
        #region ��������
        /// <summary>
        /// �������̣��ɹ�����true,ʧ�ܷ���false
        /// </summary>
        /// <returns></returns>
        public bool Create()
        {
            if (null == _ProName || "" == _ProName)
                return false;
            try
            {               
                XmlDocument Doc = new XmlDocument();
                Doc.Load(ModData.v_ProjectInfoXML);
                XmlNodeList Nodelist = Doc.SelectSingleNode("��ĿĿ¼").ChildNodes;
                if (null == Nodelist)
                    return false;
                StringBuilder result = new StringBuilder();
                String[] Floders;
                foreach (XmlNode Node1 in Nodelist)
                {
                    result.Append(Node1.InnerText);
                    result.Append("\n");
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                Floders = result.ToString().Split('\n');
                string err;
                ftp.MakeDir(_ProName,out  err);
                if (null == Floders)
                    return false;
                foreach (string FloderName in Floders)
                {
                    string errs="";
                    ftp.MakeDir(_ProName +"/"+ FloderName.Trim(),out errs);
                    if("Succeed"!=err)
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (null != ftp)
                    ftp = null;
            }
        }
        #endregion
        #region ɾ��һ������
        /// <summary>
        /// ɾ��һ������
        /// </summary>
        /// <returns></returns>
        public bool DeleteProject(DevComponents.AdvTree.Node ProjectNode, string ConStr,long Projiectid��,out string err)
        {
            Exception ex = null;
            err = "";
            if (string.IsNullOrEmpty(this._ProName))
            {
                err = "û��ָ����Ŀ���ƣ�";
                return false;
            }
            if (string.IsNullOrEmpty(ConStr))
            {
                err = "û��ָ��Ԫ��Ϣ��������Ϣ��";
                return false;
            }
            int value = 0;
            FrmProcessBar DelBar = new FrmProcessBar(10);
            DelBar.Show();
            DelBar.SetFrmProcessBarText("��������Ԫ��Ϣ��");
            Application.DoEvents();
            SysCommon.DataBase.SysTable pSysDB = new SysCommon.DataBase.SysTable();    //���Կ�������
            pSysDB.SetDbConnection(ConStr, SysCommon.enumDBConType.ORACLE, SysCommon.enumDBType.ORACLE, out ex);
            if (ex != null)
            {
                err = "Ԫ��Ϣ������ʧ�ܣ����ӵ�ַΪ��" + ConStr;
                pSysDB.CloseDbConnection();
                DelBar.Dispose();
                DelBar.Close();
                return false;
            }    
            long ProjectID = Projiectid;
            #region ɾ����Ŀ�����в�Ʒ
            string Sql = "SELECT ID,��Ʒ����,�洢λ�� FROM ProductMDTable WHERE ��ĿID=" + ProjectID;
            DataTable GetTable = pSysDB.GetSQLTable(Sql,out ex);
            if (null != ex)
            {
                err = "��ȡ��Ŀ�Ĳ�Ʒ��Ϣʧ�ܣ�";
                pSysDB.CloseDbConnection();
                DelBar.Dispose();
                DelBar.Close();
                return false;
            }
            if (null != GetTable)
            {
                for (int i = 0; i < GetTable.Rows.Count; i++)
                {
                   
                    long ProducetId = -1;
                    string ProductName = string.Empty;
                    string ProductPath = string.Empty;
                    ProducetId = long.Parse(GetTable.Rows[i][0].ToString());
                    ProductName = GetTable.Rows[i][1].ToString().Trim();
                    ProductPath = GetTable.Rows[i][2].ToString().Trim();
                    DelBar.SetFrmProcessBarValue(value);
                    DelBar.SetFrmProcessBarText(" ����ɾ����Ʒ��" + ProductName);
                    value += 1;
                    if (value == 10)
                        value = 0;
                    Application.DoEvents();
                    DevComponents.AdvTree.Node ProductNode= ModDBOperator.GetTreeNode(ProjectNode,ProductPath, ProductName , EnumTreeNodeType.PRODUCT.ToString(), ProducetId, out ex);
                    if (null!=ex)
                    {
                        err = "��ȡ��Ŀ�Ĳ�Ʒ:��" + ProductName + "�����ڵ�ʧ�ܣ�";
                        pSysDB.CloseDbConnection();
                        DelBar.Dispose();
                        DelBar.Close();
                        return false;
                    }
                    if (null != ProductNode)
                        if (!ModDBOperator.DelProduct(ProductNode, out ex))
                        {
                            err = "ɾ����Ŀ�Ĳ�Ʒ:��" + ProductName + "��ʧ�ܣ�";
                            pSysDB.CloseDbConnection();
                            DelBar.Dispose();
                            DelBar.Close();
                            return false;
                        }
                }
            }
            #endregion
            #region ɾ����Ŀ���ļ���
            DelBar.SetFrmProcessBarText("����ɾ��������ĿĿ¼");
            Application.DoEvents();
            if (!ModDBOperator.DelDirtory(this._ftpIP, this._ftpUSER, this._ftpPassWord, this._ProName, out ex))
            {
                err = "������Ŀ��ĿĿ¼ɾ��ʧ�ܣ�" ;
                pSysDB.CloseDbConnection();
                DelBar.Dispose();
                DelBar.Close();
                return false;
            }
            #endregion
            DelBar.Dispose();
            DelBar.Close();
            pSysDB.CloseDbConnection();
            return true;

        }
        #endregion
    }
}
