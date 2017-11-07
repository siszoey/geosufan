using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Data;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;

namespace GeoDBATool
{
    /// <summary>
    /// ����DGML�ĵ��������ύ��ԭʼ��  ���Ƿ����
    /// </summary>
    public class clsSubmitByDGML
    {
        string[] v_DGMLFiles = null;//DGML�ĵ�·��������
        //FID��������Ϣ
        SysCommon.enumDBConType v_dbConType;
        SysCommon.enumDBType v_dbType;
        string v_FIDConStr = "";
        //Ŀ���������Ϣ
        string v_objDBType = "";
        string v_Sever = "";
        string v_Instance = "";
        string v_DataBase = "";
        string v_User = "";
        string v_Password = "";
        string v_Version = "";
        //��ʷ������
        string v_histoDBType = "";
        string v_histoSever = "";
        string v_histoInstance = "";
        string v_histoDataBase = "";
        string v_histoUser = "";
        string v_histoPassword = "";
        string v_histoVersion = "";
        //SysCommon.Gis.SysGisDataSet v_Gisdt;//Ŀ���������
        //SysCommon.DataBase.SysTable v_Table;//FID��¼���������

        Plugin.Application.IAppGISRef v_AppGIS;//������Ӧ��APP
        Plugin.Application.IAppFormRef v_AppForm;//������APP
        //��ǰ�������ݵ��߳�
        private Thread _CurrentThread;
        public Thread CurrentThread
        {
            get
            {
                return _CurrentThread;
            }
            set
            {
                _CurrentThread = value;
            }
        }

        private bool m_Res;
        public bool Res
        {
            get
            {
                return m_Res;
            }
        }


        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="fileNames">DGML·���ļ�������</param>
        /// /// <param name="pAppGIS"></param>
        public clsSubmitByDGML(string[] fileNames, string pFIDConstr, SysCommon.enumDBConType pConType, SysCommon.enumDBType pDbType, string pObjType, string pServer, string pInstance, string pDatabse, string pUser, string pPassword, string pVersion, string pHistoType, string pHistoServer, string pHistoInstance, string pHistoDB, string pHistoUser, string pHistoPassword, string pHistoVersion, Plugin.Application.IAppGISRef pAppGIS)
        {
            //v_Gisdt = pGisDataset;
            //v_Table = pSysTable;
            v_FIDConStr = pFIDConstr;
            v_dbConType = pConType;
            v_dbType = pDbType;

            v_objDBType = pObjType;
            v_Sever = pServer;
            v_Instance = pInstance;
            v_DataBase = pDatabse;
            v_User = pUser;
            v_Password = pPassword;
            v_Version = pVersion;

            v_histoDBType = pHistoType;
            v_histoSever = pHistoServer;
            v_histoInstance = pHistoInstance;
            v_histoDataBase = pHistoDB;
            v_histoUser = pHistoUser;
            v_histoPassword = pHistoPassword;
            v_histoVersion = pHistoVersion;

            v_DGMLFiles = fileNames;
            v_AppGIS = pAppGIS;
            v_AppForm = pAppGIS as Plugin.Application.IAppFormRef;
        }

        public void SubmitThread()
        {
            bool isConnect = true;//��ʶ��ʷ���Ƿ��ܹ�������
            DateTime pDate = DateTime.Now;
            Exception eError = null;
            m_Res = true;
            //������ͼ
            if (v_AppGIS == null)
            {
                m_Res = false;
                return;
            }
            if (v_AppGIS.DataTree == null)
            {
                m_Res = false;
                return;
            }
            //����������ͼ
            v_AppForm.MainForm.Invoke(new InitTree(IntialTree), new object[] { v_AppGIS.DataTree });
            //�������ڵ���ɫ
            v_AppForm.MainForm.Invoke(new SetSelectNodeColor(setNodeColor), new object[] { v_AppGIS.DataTree });
            v_AppGIS.DataTree.Tag = false;
            //��ʾ������
            v_AppForm.MainForm.Invoke(new ShowProgress(ShowProgressBar), new object[] { true });

            #region ���ÿ�������
            //����Ŀ�������
            SysCommon.Gis.SysGisDataSet v_Gisdt = new SysCommon.Gis.SysGisDataSet();
            if (v_objDBType == "SDE")
            {
                v_Gisdt.SetWorkspace(v_Sever, v_Instance, v_DataBase, v_User, v_Password, v_Version, out eError);
            }
            else if (v_objDBType == "PDB")
            {
                v_Gisdt.SetWorkspace(v_DataBase, SysCommon.enumWSType.PDB, out eError);
            }
            else if (v_objDBType == "GDB")
            {
                v_Gisdt.SetWorkspace(v_DataBase, SysCommon.enumWSType.GDB, out eError);
            }
            if (eError != null)
            {
                v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "Ŀ�����ݿ����ӳ���" });
                m_Res = false;
                //��ֹ����
                v_AppGIS.CurrentThread = null;
                if (_CurrentThread.ThreadState != ThreadState.Stopped)
                {
                    _CurrentThread.Abort();
                }
                return;
            }

            //������ʷ������
            SysCommon.Gis.SysGisDataSet v_HistoGisdt = new SysCommon.Gis.SysGisDataSet();
            if (v_histoDBType == "SDE")
            {
                v_HistoGisdt.SetWorkspace(v_histoSever, v_histoInstance, v_histoDataBase, v_histoUser, v_histoPassword, v_histoVersion, out eError);
            }
            else if (v_histoDBType == "PDB")
            {
                v_HistoGisdt.SetWorkspace(v_histoDataBase, SysCommon.enumWSType.PDB, out eError);
            }
            else if (v_histoDBType == "GDB")
            {
                v_HistoGisdt.SetWorkspace(v_histoDataBase, SysCommon.enumWSType.GDB, out eError);
            }
            if (eError != null)
            {
                //v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "��ʷ�����ݿ����ӳ���" });
                isConnect = false;
                //m_Res = false;
                ////��ֹ����
                //v_AppGIS.CurrentThread = null;
                //_CurrentThread.Abort();
                //return;
            }
            //����FID��¼������
            SysCommon.DataBase.SysTable v_Table = new SysCommon.DataBase.SysTable();
            v_Table.SetDbConnection(v_FIDConStr, v_dbConType, v_dbType, out eError);
            if (eError != null)
            {
                v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "����FID��¼�����ݿ����" });
                m_Res = false;
                // ��ֹ����
                v_AppGIS.CurrentThread = null;
                if (_CurrentThread.ThreadState != ThreadState.Stopped)
                {
                    _CurrentThread.Abort();
                }
                return;
            }
            #endregion
            try
            {
                //��������ļ�
                foreach (string fileName in v_DGMLFiles)
                {
                    //������ͼ���ڵ�(���ļ�����Ϊ�����)
                    DevComponents.AdvTree.Node pNode = new DevComponents.AdvTree.Node();
                    string tempFile = fileName.Substring(fileName.LastIndexOf('\\') + 1);
                    pNode = (DevComponents.AdvTree.Node)v_AppForm.MainForm.Invoke(new CreateTreeNode(CreateAdvTreeNode), new object[] { v_AppGIS.DataTree.Nodes, tempFile, tempFile, v_AppGIS.DataTree.ImageList.Images[5], true });//�ļ����ڵ�
                    //��ʾ״̬��
                    v_AppForm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { "�ļ�" + tempFile + "�е����ݿ�ʼ���" });

                    //��¼ɾ��Ҫ�ص� GOFID
                    StringBuilder FIDsb = new StringBuilder();

                    FIDsb = GetFeaClsByDGML(fileName, "ɾ��", out eError);
                    if (FIDsb == null)
                    {
                        v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", eError.Message });
                        m_Res = false;
                        //// ��ֹ����
                        //v_AppGIS.CurrentThread = null;
                        //if (_CurrentThread.ThreadState != ThreadState.Stopped)
                        //{
                        //    _CurrentThread.Suspend();
                        //    _CurrentThread.Abort();
                        //}
                        break;
                    }

                    //������������༭
                    v_Table.StartTransaction();
                    v_Gisdt.StartWorkspaceEdit(false, out eError);
                    if (eError != null)
                    {
                        v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "�����༭ʧ�ܣ�" });
                        m_Res = false;
                        //�ر����� ��ֹ����
                        v_Table.CloseDbConnection();
                        //���ؽ�����
                        v_AppForm.MainForm.Invoke(new ShowProgress(ShowProgressBar), new object[] { false });
                        //��״̬����Ϣ��Ϊ��
                        v_AppForm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { "" });
                        //v_AppGIS.CurrentThread = null;
                        //if (_CurrentThread.ThreadState != ThreadState.Stopped)
                        //{
                        //    _CurrentThread.Suspend();
                        //    _CurrentThread.Abort();
                        //}
                        break;
                    }
                    //�������ʷ�⣬��Ҫ������ʷ������������ʷ��
                    if (isConnect == true)
                    {
                        v_HistoGisdt.StartWorkspaceEdit(false, out eError);
                        if (eError != null)
                        {
                            v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "�����༭ʧ�ܣ�" });
                            m_Res = false;
                            //�ر����� ��ֹ����
                            v_Table.CloseDbConnection();
                            //���ؽ�����
                            v_AppForm.MainForm.Invoke(new ShowProgress(ShowProgressBar), new object[] { false });
                            //��״̬����Ϣ��Ϊ��
                            v_AppForm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { "" });
                            //v_AppGIS.CurrentThread = null;
                            //if (_CurrentThread.ThreadState != ThreadState.Stopped)
                            //{
                            //    _CurrentThread.Suspend();
                            //    _CurrentThread.Abort();
                            //}
                            break;
                        }
                    }
                    #region ����ʷ��"�޸ĺ�ɾ��"��������Ϊ��ʷ
                    if (isConnect == true)
                    {
                        if (!UpdateHistoData(fileName, v_histoDBType, v_HistoGisdt, v_Table, pDate, out eError))
                        {
                            m_Res = false;
                            //�ع�����,�ر�����
                            v_Table.EndTransaction(false);
                            v_Table.CloseDbConnection();
                            v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", eError.Message });
                            //�����༭
                            v_Gisdt.EndWorkspaceEdit(false, out eError);
                            if (isConnect)
                            {
                                v_HistoGisdt.EndWorkspaceEdit(false, out eError);
                            }
                            if (eError != null)
                            {
                                v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "�����༭����" });
                            }
                            //���ؽ�����
                            v_AppForm.MainForm.Invoke(new ShowProgress(ShowProgressBar), new object[] { false });
                            //��״̬����Ϣ��Ϊ��
                            v_AppForm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { "" });

                            //v_AppGIS.CurrentThread = null;
                            //if (_CurrentThread.ThreadState != ThreadState.Stopped)
                            //{
                            //    _CurrentThread.Suspend();
                            //    _CurrentThread.Abort();
                            //}
                            break;
                        }
                    }
                    #endregion

                    # region ɾ�����ƿ����� ɾ�� �����ݣ�ͬʱɾ��FID��Ӧ�ļ�¼
                    if (!UpdateData(FIDsb, v_Gisdt, v_Table, out eError))
                    {
                        m_Res = false;
                        //�ع�����,�ر�����
                        v_Table.EndTransaction(false);
                        v_Table.CloseDbConnection();
                        v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", eError.Message });
                        //�����༭
                        v_Gisdt.EndWorkspaceEdit(false, out eError);
                        if (isConnect)
                        {
                            v_HistoGisdt.EndWorkspaceEdit(false, out eError);
                        }
                        if (eError != null)
                        {
                            v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "�����༭����" });
                        }
                        //���ؽ�����
                        v_AppForm.MainForm.Invoke(new ShowProgress(ShowProgressBar), new object[] { false });
                        //��״̬����Ϣ��Ϊ��
                        v_AppForm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { "" });
                        //v_AppGIS.CurrentThread = null;
                        //if (_CurrentThread.ThreadState != ThreadState.Stopped)
                        //{
                        //    _CurrentThread.Suspend();
                        //    _CurrentThread.Abort();
                        //}
                        break;
                    }
                    #endregion

                    #region �� �½����޸� �����ݵ���Ŀ�����ݿ����ʷ���У����޸�FID��¼��
                    if (!ImportData(fileName, pNode, v_Gisdt, v_HistoGisdt, v_Table, pDate, isConnect, out eError))
                    {
                        m_Res = false;
                        //�ع�����,�ر�����
                        v_Table.EndTransaction(false);
                        v_Table.CloseDbConnection();
                        v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", eError.Message });
                        //�����༭
                        v_Gisdt.EndWorkspaceEdit(false, out eError);
                        if (isConnect)
                        {
                            v_HistoGisdt.EndWorkspaceEdit(false, out eError);
                        }
                        if (eError != null)
                        {
                            v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "�����༭����" });
                        }
                        //���ؽ�����
                        v_AppForm.MainForm.Invoke(new ShowProgress(ShowProgressBar), new object[] { false });
                        //��״̬����Ϣ��Ϊ��
                        v_AppForm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { "" });
                        //// ��ֹ����
                        //v_AppGIS.CurrentThread = null;
                        //if (_CurrentThread.ThreadState != ThreadState.Stopped)
                        //{
                        //    _CurrentThread.Suspend();
                        //    _CurrentThread.Abort();
                        //}
                        break;
                    }
                    #endregion

                    //������������༭
                    v_Table.EndTransaction(true);
                    v_Gisdt.EndWorkspaceEdit(true, out eError);
                    if (isConnect)
                    {
                        v_HistoGisdt.EndWorkspaceEdit(true, out eError);
                    }
                    if (eError != null)
                    {
                        m_Res = false;
                        v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "�����༭ʧ�ܣ�" });

                        //�ر����� ��ֹ����
                        v_Table.CloseDbConnection();
                        //���ؽ�����
                        v_AppForm.MainForm.Invoke(new ShowProgress(ShowProgressBar), new object[] { false });
                        //��״̬����Ϣ��Ϊ��
                        v_AppForm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { "" });
                        //v_AppGIS.CurrentThread = null;
                        //if (_CurrentThread.ThreadState != ThreadState.Stopped)
                        //{
                        //    _CurrentThread.Suspend();
                        //    _CurrentThread.Abort();
                        //}
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                //*******************************************************************
                //Exception Log
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(ex, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(ex, null, DateTime.Now);
                }
                //********************************************************************
                m_Res = false;
                v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", ex.Message });
                ////�ر����� ��ֹ����
                //v_Table.CloseDbConnection();
                ////���ؽ�����
                //v_AppForm.MainForm.Invoke(new ShowProgress(ShowProgressBar), new object[] { false });
                ////��״̬����Ϣ��Ϊ��
                //v_AppForm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { "" });
                ////ֹͣ�߳�
                //v_AppGIS.CurrentThread = null;
                //if (_CurrentThread.ThreadState != ThreadState.Stopped)
                //{
                //    _CurrentThread.Suspend();
                //    _CurrentThread.Abort();
                //}
            }

            //�ر�����
            v_Table.CloseDbConnection();
            //���ؽ�����
            v_AppForm.MainForm.Invoke(new ShowProgress(ShowProgressBar), new object[] { false });
            //��״̬����Ϣ��Ϊ��
            v_AppForm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { "" });

            if (m_Res == true)
            {
                v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "��ʾ", "�����ɹ�!" });
            }
            //ֹͣ�߳�
            v_AppGIS.CurrentThread = null;
            if (_CurrentThread.ThreadState != ThreadState.Stopped)
            {
                _CurrentThread.Abort();
            }
        }

        #region ����

        /// <summary>
        /// ��DGML������״̬��ȡҪ����Ϣ
        /// </summary>
        /// <param name="pfileName">DGML�ĵ�·��</param>
        /// <param name="state">״̬���������޸ĺ�ɾ����</param>
        /// <returns></returns>
        private StringBuilder GetFeaClsByDGML(string pfileName, string state, out Exception eError)
        {
            try
            {
                eError = null;
                StringBuilder FIDsb = new StringBuilder();
                if (pfileName == "")
                {
                    eError = new Exception("�ļ���Ϊ��!");
                    return null;
                }

                ////�ñ���������¼ԭʼ��ͼ�����ƺ�OID
                //Dictionary<string, StringBuilder> FeaClsDic = new Dictionary<string, StringBuilder>();

                //����DGML�ĵ�
                XmlDocument DGMLDoc = new XmlDocument();
                DGMLDoc.Load(pfileName);

                XmlNodeList RecordList = DGMLDoc.SelectNodes(".//DGML//Data//Record");
                if (RecordList == null)
                {
                    eError = new Exception("xml�ĵ��ṹ����ȷ��������ĵ��Ƿ�Ϊ���µ������ĵ�!");
                    return null;
                }
                foreach (XmlNode recordNode in RecordList)
                {
                    XmlNode sNode = recordNode.SelectSingleNode(".//STATE");//״̬�ڵ�
                    if (sNode == null)
                    {
                        eError = new Exception("xml�ĵ��ṹ����ȷ��������ĵ��Ƿ�Ϊ���µ������ĵ�!");
                        return null;
                    }
                    XmlNode fNode = recordNode.SelectSingleNode(".//GOFID");//GOFID�ڵ�
                    if (fNode == null)
                    {
                        eError = new Exception("xml�ĵ��ṹ����ȷ��������ĵ��Ƿ�Ϊ���µ������ĵ�!");
                        return null;
                    }
                    string pState = sNode.InnerText.Trim();//״̬���½����޸ġ�ɾ��
                    if (pState == state)
                    {
                        if (fNode.InnerText.Trim() == "")
                        {
                            eError = new Exception("��xml�ļ��ж�ȡ�޸Ļ�ɾ�������ݳ���");
                            return null;
                        }
                        string pFID = fNode.InnerText.Trim();//GOFID
                        if (FIDsb.Length != 0)
                        {
                            FIDsb.Append(',');
                        }
                        FIDsb.Append(pFID);//��¼GOFID
                    }
                }
                return FIDsb;
            }
            catch (Exception eex)
            {
                //*******************************************************************
                //Exception Log
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(eex, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(eex, null, DateTime.Now);
                }
                //********************************************************************
                eError = eex;
                return null;
            }
        }
        /// <summary>
        /// ��¼ͼ�����Ͷ�Ӧ��OID�ַ���
        /// </summary>
        /// <param name="v_Table">FID��¼������</param>
        /// <param name="FIDinfo">GOFID�ֶ���Ϣ</param>
        /// <returns></returns>
        private Dictionary<string, StringBuilder> GetDicFeaOIDInfo(SysCommon.DataBase.SysTable v_Table, StringBuilder FIDinfo, out Exception eError)
        {
            eError = null;
            Exception ex = null;
            //��¼ͼ������OIDֵ��
            Dictionary<string, StringBuilder> FCInfo = new Dictionary<string, StringBuilder>();
            if (FIDinfo.Length == 0) return FCInfo;
            DataTable tempTable = v_Table.GetSQLTable("select * from FID��¼�� where GOFID in (" + FIDinfo.ToString() + ")", out ex);
            if (ex != null)
            {
                eError = new Exception("������������FID��¼�����");
                //v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", eError.Message });
                return null;
            }

            for (int i = 0; i < tempTable.Rows.Count; i++)
            {
                string PName = tempTable.Rows[i]["FCNAME"].ToString().Trim();
                string pOID = tempTable.Rows[i]["OID"].ToString().Trim();
                if (!FCInfo.ContainsKey(PName))
                {
                    StringBuilder sb = new StringBuilder();
                    if (sb.Length != 0)
                    {
                        sb.Append(',');
                    }
                    sb.Append(pOID);
                    FCInfo.Add(PName, sb);
                }
                else
                {
                    if (FCInfo[PName].Length != 0)
                    {
                        FCInfo[PName].Append(',');
                    }
                    FCInfo[PName].Append(pOID);
                }
            }
            return FCInfo;
        }
        /// <summary>
        /// ������ʷ���������ʷ����
        /// </summary>
        /// <param name="pfileName">DGML�ĵ�</param>
        /// <param name="v_HistoDBType">��ʷ����������</param>
        /// <param name="v_HistoGIsdt">��ʷ������</param>
        /// <param name="v_Table">FID��¼��</param>
        /// <param name="invalidDate">ʧЧ����</param>
        /// <returns></returns>
        private bool UpdateHistoData(string pfileName, string v_HistoDBType, SysCommon.Gis.SysGisDataSet v_HistoGIsdt, SysCommon.DataBase.SysTable v_Table, DateTime invalidDate, out Exception eError)
        {
            eError = null;
            Exception ex = null;
            StringBuilder updateFID = new StringBuilder();//�޸ĵ�FID����
            StringBuilder deleteFID = new StringBuilder();//�޸ĵ�FID����
            Dictionary<string, StringBuilder> FeaClsUpdateInfo = new Dictionary<string, StringBuilder>();//��¼"�޸�"Ҫ��ͼ������OID
            Dictionary<string, StringBuilder> FeaClsDeleteInfo = new Dictionary<string, StringBuilder>();//��¼"ɾ��"Ҫ��ͼ������OID
            updateFID = GetFeaClsByDGML(pfileName, "�޸�", out ex);
            if (updateFID == null)
            {
                eError = ex;
                return false;
            }
            deleteFID = GetFeaClsByDGML(pfileName, "ɾ��", out ex);
            if (deleteFID == null)
            {
                eError = ex;
                return false;
            }

            if (updateFID.Length == 0 && deleteFID.Length == 0) return true;


            FeaClsUpdateInfo = GetDicFeaOIDInfo(v_Table, updateFID, out ex);
            if (ex != null)
            {
                eError = ex;
            }
            FeaClsDeleteInfo = GetDicFeaOIDInfo(v_Table, deleteFID, out ex);
            if (ex != null)
            {
                eError = ex;
            }
            string[] strArr = deleteFID.ToString().Split(new char[] { ',' });
            int t = 0;
            foreach (KeyValuePair<string, StringBuilder> feaInfo in FeaClsDeleteInfo)
            {
                string[] aa = feaInfo.Value.ToString().Split(new char[] { ',' });
                t += aa.Length;
            }

            if (t != strArr.Length)
            {
                //��DGML�ĵ��е�����ɾ����Ҫ�ؼ�¼��FID��¼���еļ�¼��һ�£�˵����DGML�ĵ����ܲ������µ��ĵ�
                eError = new Exception("��DGML�ĵ��е����������ƿ����ݲ�һ�£����飡");
                return false;
            }

            #region ���޸ĵ�Ҫ�ؽ�����Ӧ���޸�
            foreach (KeyValuePair<string, StringBuilder> updateItem in FeaClsUpdateInfo)
            {
                string pFCName = updateItem.Key + "_GOH";//ͼ����
                IFeatureClass pFeaCls = v_HistoGIsdt.GetFeatureClass(pFCName, out ex);
                if (ex != null)
                {
                    eError = new Exception("���ͼ��" + pFCName + "ʧ�ܣ�");
                    //v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "���ͼ��" + pFCName + "ʧ�ܣ�" });
                    return false;
                }
                int dateIndex = -1;//"ToDate"����ֵ
                //int stateIndex = -1;//"State"����ֵ
                dateIndex = pFeaCls.Fields.FindField("ToDate");
                //stateIndex = pFeaCls.Fields.FindField("State");
                if (dateIndex == -1)
                {
                    eError = new Exception("����ʷ�����Ҳ����ֶ�ToDate��");
                    return false;
                }
                //if (stateIndex == -1) return false;
                //���ù�������
                IQueryFilter pFilter = new QueryFilterClass();
                pFilter.WhereClause = "ToDate='" + DateTime.MaxValue.ToString("u") + "' and SourceOID in (" + updateItem.Value + ")";
                IFeatureCursor pFeaCursor = pFeaCls.Update(pFilter, false);
                if (pFeaCursor == null)
                {
                    //eError = new Exception("IFeatureCursor����Ϊ�գ�");
                    return false;
                }
                IFeature pFeature = pFeaCursor.NextFeature();
                while (pFeature != null)
                {
                    pFeature.set_Value(dateIndex, invalidDate.ToString("u") as object);
                    pFeaCursor.UpdateFeature(pFeature);
                    pFeaCursor.Flush();
                    pFeature = pFeaCursor.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaCursor);
                //if (v_HistoDBType == "SDE")
                //{
                //    pFilter.WhereClause = "ToDate=TO_DATE('" + DateTime.MaxValue+ "','YYYY-MM-DD HH24:MI:SS') and SourceOID in (" + updateItem.Value + ")";
                //}
                //else if (v_HistoDBType == "GDB")
                //{
                //    pFilter.WhereClause = "ToDate=date '"+DateTime.MaxValue+"' and SourceOID in (" + updateItem.Value + ")";
                //}
                //else if (v_HistoDBType == "PDB")
                //{
                //    pFilter.WhereClause = "ToDate=#"+DateTime.MaxValue+"# and SourceOID in (" + updateItem.Value + ")";
                //}
                //pFilter.SubFields = "ToDate,State";
                //ITable pTable = pFeaCls as ITable;
                //IRowBuffer pRowBuf = pTable.CreateRowBuffer();
                //pRowBuf.set_Value(dateIndex, invalidDate.ToString("u") as object);
                //pRowBuf.set_Value(stateIndex, 2);
                //�޸�Ҫ��
                //pTable.UpdateSearchedRows(pFilter, pRowBuf);
                //����Ҫ����ע�ǣ����������������ķ�����
                //......
            }
            #endregion

            # region ��ɾ����Ҫ�ؽ�����Ӧ���޸�
            foreach (KeyValuePair<string, StringBuilder> deleteItem in FeaClsDeleteInfo)
            {
                string pFCName = deleteItem.Key + "_GOH";//ͼ����
                IFeatureClass pFeaCls = v_HistoGIsdt.GetFeatureClass(pFCName, out ex);
                if (ex != null)
                {
                    eError = new Exception("���ͼ��" + pFCName + "ʧ�ܣ�");
                    //v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "���ͼ��" + pFCName + "ʧ�ܣ�" });
                    return false;
                }
                int dateIndex = -1;//"ToDate"����ֵ
                //int stateIndex = -1;//"State"����ֵ
                dateIndex = pFeaCls.Fields.FindField("ToDate");
                //stateIndex = pFeaCls.Fields.FindField("State");
                if (dateIndex == -1)
                {
                    eError = new Exception("����ʷ�����Ҳ����ֶ�ToDate��");
                    return false;
                }
                //if (stateIndex == -1) return false;
                //���ù�������
                IQueryFilter pDelFilter = new QueryFilterClass();
                pDelFilter.WhereClause = "ToDate='" + DateTime.MaxValue.ToString("u") + "' and SourceOID in (" + deleteItem.Value + ")";
                IFeatureCursor pDelFeaCursor = pFeaCls.Update(pDelFilter, false);
                if (pDelFeaCursor == null) return false;
                IFeature pDelFeature = pDelFeaCursor.NextFeature();
                while (pDelFeature != null)
                {
                    pDelFeature.set_Value(dateIndex, invalidDate.ToString("u") as object);
                    pDelFeaCursor.UpdateFeature(pDelFeature);
                    pDelFeaCursor.Flush();
                    pDelFeature = pDelFeaCursor.NextFeature();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pDelFeaCursor);
                //if (v_HistoDBType == "SDE")
                //{
                //    pFilter.WhereClause = "ToDate=TO_DATE('"+DateTime.MaxValue+"','YYYY-MM-DD HH24:MI:SS') and SourceOID in (" + deleteItem.Value + ")";
                //}
                //else if (v_HistoDBType == "GDB")
                //{
                //    pFilter.WhereClause = "ToDate=date '"+DateTime.MaxValue+"' and SourceOID in (" + deleteItem.Value + ")";
                //}
                //else if (v_HistoDBType == "PDB")
                //{
                //    pFilter.WhereClause = "ToDate=#"+DateTime.MaxValue+"# and SourceOID in (" + deleteItem.Value + ")";
                //}
                //pFilter.SubFields = "ToDate,State";
                //ITable pTable = pFeaCls as ITable;
                //IRowBuffer pRowBuf = pTable.CreateRowBuffer();
                //pRowBuf.set_Value(dateIndex, invalidDate);
                //pRowBuf.set_Value(stateIndex, 3);
                //�޸�Ҫ��
                //pTable.UpdateSearchedRows(pFilter, pRowBuf);
                //����Ҫ����ע�ǣ����������������ķ�����
                //......
            }
            # endregion
            return true;
        }
        /// <summary>
        /// ɾ��ԭʼ��������±仯(�޸ĺ�ɾ��)�����ݣ�ɾ��FID��¼�������Ӧ�ļ�¼
        /// </summary>
        /// <param name="FCInfo">���±仯���ݵ�ͼ������OID���޸ĺ�ɾ�������ݣ�</param>
        /// <param name="FIDinfo">FID��¼��������±仯�����ݶ�Ӧ�ļ�¼(�޸ĺ�ɾ��������)</param>
        /// <param name="pGisDataset">ԭʼ������</param>
        /// <param name="pSysTable">FID��¼������</param>
        /// <returns></returns>
        private bool UpdateData(StringBuilder FIDinfo, SysCommon.Gis.SysGisDataSet v_Gisdt, SysCommon.DataBase.SysTable v_Table, out Exception eError)
        {
            eError = null;
            Exception ex = null;
            if (FIDinfo == null) return false;
            if (FIDinfo.Length == 0) return true;
            Dictionary<string, StringBuilder> FCInfo = new Dictionary<string, StringBuilder>();
            FCInfo = GetDicFeaOIDInfo(v_Table, FIDinfo, out ex);
            if (FCInfo == null)
            {
                eError = ex;
                return false;
            }
            if (FCInfo.Count == 0) return true;


            string[] strArr = FIDinfo.ToString().Split(new char[] { ',' });
            int t = 0;
            foreach (KeyValuePair<string, StringBuilder> feaInfo in FCInfo)
            {
                string[] aa = feaInfo.Value.ToString().Split(new char[] { ',' });
                t += aa.Length;
            }

            if (t != strArr.Length)
            {
                //��DGML�ĵ��е�����ɾ����Ҫ�ؼ�¼��FID��¼���еļ�¼��һ�£�˵����DGML�ĵ����ܲ������µ��ĵ�
                eError = new Exception("��DGML�ĵ��е����������ƿ����ݲ�һ�£����飡");
                return false;
            }

            //ɾ������
            foreach (KeyValuePair<string, StringBuilder> FcItem in FCInfo)
            {
                string conndiStr = "OBJECTID in (" + FcItem.Value.ToString() + ")";
                v_Gisdt.DeleteRows(FcItem.Key, conndiStr, out ex);
                if (ex != null)
                {
                    eError = new Exception("���ƿ��С�ɾ���������ݸ���ʧ�ܣ�");
                    //v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "ɾ������ʧ�ܣ�" });
                    return false;
                }
            }
            //ɾ����־��¼�������޸ĺ�ɾ���ļ�¼
            v_Table.UpdateTable("delete from FID��¼�� where GOFID in (" + FIDinfo.ToString() + ")", out ex);
            if (ex != null)
            {
                eError = new Exception("ɾ��FID��¼���¼ʧ�ܣ�");
                //v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "ɾ��FID��¼���¼ʧ�ܣ�" });
                return false;
            }
            return true;
        }
        /// <summary>
        /// �������ƿ����޸�Ҫ�ض�Ӧ��OBJECTID
        /// </summary>
        /// <param name="pFID">�޸�Ҫ�ض�Ӧ��GOFID</param>
        /// <param name="v_Table">FID��¼������</param>
        /// <returns></returns>
        private int GetUserDBOID(int pFID, SysCommon.DataBase.SysTable v_Table)
        {
            Exception eError = null;
            int pOid = -1;
            DataTable dt = v_Table.GetSQLTable("select * from FID��¼�� where GOFID=" + pFID, out eError);
            if (eError != null)
            {
                return pOid;
            }
            if (dt.Rows.Count != 1) return pOid;
            pOid = int.Parse(dt.Rows[0]["OID"].ToString().Trim());
            return pOid;
        }

        /// <summary>
        /// �� �½����޸ĵ�Ҫ�� ���뵽Ŀ������ʷ����
        /// </summary>
        /// <param name="pFileName">DGML�ĵ�</param>
        /// <param name="pNode">������ͼ����㣨�ļ�����</param>
        /// <param name="v_Gisdt">Ŀ�������</param>
        /// <param name="v_histoGisdt">��ʷ������</param>
        /// <param name="v_Table">FID��¼������</param>
        /// <returns></returns>
        private bool ImportData(string pFileName, DevComponents.AdvTree.Node pNode, SysCommon.Gis.SysGisDataSet v_Gisdt, SysCommon.Gis.SysGisDataSet v_histoGisdt, SysCommon.DataBase.SysTable v_Table, DateTime pDate, bool isConne, out Exception eError)
        {
            try
            {
                eError = null;
                Exception ex = null;
                //��¼"�½����޸�"�����ݵ�ͼ���������º��Feature��ԭʼ��OID
                Dictionary<string, Dictionary<XmlNode, int>> pFeaInfo = new Dictionary<string, Dictionary<XmlNode, int>>();
                //����DGML�ĵ�
                XmlDocument DGMLDoc = new XmlDocument();
                DGMLDoc.Load(pFileName);

                #region ������Ҫ�أ��½����޸ĵ�Ҫ�أ���Ӧ��ͼ������Ҫ�ؽڵ㱣������
                XmlNodeList RecordList = DGMLDoc.SelectNodes(".//DGML//Data//Record");
                //if (RecordList == null)
                //{
                //    eError = new Exception("xml�ĵ��ṹ����ȷ��������ĵ��Ƿ�Ϊ���µ������ĵ�!");
                //}
                foreach (XmlNode recordNode in RecordList)
                {
                    string pState = recordNode.SelectSingleNode(".//STATE").InnerText.Trim();//����״̬
                    string FCName = recordNode.SelectSingleNode(".//OLDFCNAME").InnerText.Trim();//ͼ����
                    if (FCName == "")
                    {
                        eError = new Exception("�½����޸ĵ�Ҫ��ͼ����Ϊ�գ�");
                        //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�½����޸ĵ�Ҫ��ͼ����Ϊ�գ�");
                        return false;
                    }
                    XmlNode newFeature = recordNode.SelectSingleNode(".//NEWFEATURE");
                    int pOID = -1;
                    #region ����XML�ĵ���ȡ���½����޸ġ���Ҫ����Ϣ
                    if (pState == "�޸�")
                    {
                        if (recordNode.SelectSingleNode(".//GOFID").InnerText.Trim() == "")
                        {
                            eError = new Exception("��" + FCName + "�У��޸�Ҫ�ص�GOFIDΪ�գ�");
                            //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��" + FCName + "�У��޸�Ҫ�ص�GOFIDΪ�գ�");
                            return false;
                        }
                        int pFid = int.Parse(recordNode.SelectSingleNode(".//GOFID").InnerText.Trim());//ͼ����
                        if (pFid == -1)
                        {
                            eError = new Exception("��" + FCName + "�У��޸�Ҫ�ص�GOFIDΪ-1��");
                            //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��" + FCName + "�У��޸�Ҫ�ص�GOFIDΪ-1��");
                            return false;
                        }
                        pOID = GetUserDBOID(pFid, v_Table);
                        if (pOID == -1)
                        {
                            eError = new Exception("��ȡ���ƿ��޸�Ҫ��ԴOIDʧ��!");
                            //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ƿ��޸�Ҫ��ԴOIDʧ��!");
                            return false;
                        }

                        if (!pFeaInfo.ContainsKey(FCName))
                        {
                            Dictionary<XmlNode, int> tempDic = new Dictionary<XmlNode, int>();
                            tempDic.Add(newFeature, pOID);
                            pFeaInfo.Add(FCName, tempDic);
                        }
                        else
                        {
                            if (!pFeaInfo[FCName].ContainsKey(newFeature))
                            {
                                pFeaInfo[FCName].Add(newFeature, pOID);
                            }
                        }
                    }
                    if (pState == "�½�")
                    {
                        if (!pFeaInfo.ContainsKey(FCName))
                        {
                            Dictionary<XmlNode, int> tempDic = new Dictionary<XmlNode, int>();
                            tempDic.Add(newFeature, pOID);
                            pFeaInfo.Add(FCName, tempDic);
                        }
                        else
                        {
                            if (!pFeaInfo[FCName].ContainsKey(newFeature))
                            {
                                pFeaInfo[FCName].Add(newFeature, pOID);
                            }
                        }
                    }
                    #endregion
                }
                if (pFeaInfo.Count == 0) return true;
                #endregion
                ////���Ŀ���Workspace
                //IFeatureWorkspace pFeaWS = v_Gisdt.WorkSpace as IFeatureWorkspace;
                //if (pFeaWS == null) return false;
                //
                //���µ�Ҫ�ص��뵽��Ӧ��ͼ����
                foreach (KeyValuePair<string, Dictionary<XmlNode, int>> item in pFeaInfo)
                {

                    //�����ͼ ͼ�����ӽڵ�
                    DevComponents.AdvTree.Node FcNode = new DevComponents.AdvTree.Node();
                    FcNode = (DevComponents.AdvTree.Node)v_AppForm.MainForm.Invoke(new CreateTreeNode(CreateAdvTreeNode), new object[] { pNode.Nodes, item.Key, item.Key, v_AppGIS.DataTree.ImageList.Images[6], true });//ͼ�����ӽڵ�
                    //�����ͼ�ڵ���
                    v_AppForm.MainForm.Invoke(new CreateTreeCell(CreateAdvTreeCell), new object[] { FcNode, "����", null });
                    v_AppForm.MainForm.Invoke(new CreateTreeCell(CreateAdvTreeCell), new object[] { FcNode, "", null });

                    //����״̬����ֵ
                    v_AppForm.MainForm.Invoke(new ShowTips(ShowStatusTips), new object[] { "ͼ��" + item.Key + "���ݿ�ʼ���" });
                    //���ó�ʼ������
                    int tempValue = 0;
                    v_AppForm.MainForm.Invoke(new ChangeProgress(ChangeProgressBar), new object[] { v_AppForm.ProgressBar, 0, item.Value.Count, tempValue });

                    try
                    {
                        //���Ŀ���ͼ��
                        IFeatureClass pFeaCLs = v_Gisdt.GetFeatureClass(item.Key.Trim(), out ex);
                        if (ex != null)
                        {
                            eError = new Exception("�Ҳ���ͼ����Ϊ" + item.Key.Trim() + "��ͼ��,���飡");
                            //v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����!", "�Ҳ���ͼ����Ϊ" + item.Key.Trim() + "��ͼ��,���飡" });
                            //�ı���ͼ����״̬
                            v_AppForm.MainForm.Invoke(new ChangeSelectNode(ChangeTreeSelectNode), new object[] { FcNode, "ʧ��", "", false });
                            return false;
                        }
                        IFeatureClass pHistoFeaCLs = null;
                        if (isConne == true)
                        {
                            //�����ʷ��ͼ��
                            pHistoFeaCLs = v_histoGisdt.GetFeatureClass(item.Key.Trim() + "_GOH", out ex);
                            if (ex != null)
                            {
                                eError = new Exception("�Ҳ���ͼ����Ϊ" + item.Key.Trim() + "_goh ��ͼ��,���飡");
                                //v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����!", "�Ҳ���ͼ����Ϊ" + item.Key.Trim() + "_goh ��ͼ��,���飡" });
                                //�ı���ͼ����״̬
                                v_AppForm.MainForm.Invoke(new ChangeSelectNode(ChangeTreeSelectNode), new object[] { FcNode, "ʧ��", "", false });
                                return false;
                            }
                        }
                        # region �����е��½���Ҫ�ص���Ŀ����У��޸ĵ�Ҫ�ؽ����޸�(���ƿ����ʷ��)
                        //�����½���Ҫ��,�����µ�Ҫ�ص��뵽Ŀ�����
                        //����һ��FeatureBuffer��
                        IFeatureBuffer pFeatureBuffer = pFeaCLs.CreateFeatureBuffer();
                        IFeatureCursor pFeaCusor = null;
                        pFeaCusor = pFeaCLs.Insert(true);
                        //����Ҫ���ֵ䣬������Ҫ��
                        foreach (KeyValuePair<XmlNode, int> FeaStateItem in item.Value)
                        {
                            #region ��"�޸�"��Ҫ������ʷ
                            IFeature pHistoFeature = null;
                            int stateIndex = -1;//"״̬"�ֶ�����ֵ
                            int sourceIDIndex = -1;//"ԴOID"����ֵ
                            if (isConne == true)
                            {
                                //���������ʷ���Ҫ�أ��½����޸ĵ����ײ�����ʷ�⣩
                                pHistoFeature = pHistoFeaCLs.CreateFeature();
                                //������ʷ�����������ӵ��ֶ�
                                int fromdateIndex = -1;//"��Ч����" �ֶ�����ֵ 
                                int todateIndex = -1;//"ʧЧ����"�ֶ�����ֵ
                                fromdateIndex = pHistoFeature.Fields.FindField("FromDate");
                                todateIndex = pHistoFeature.Fields.FindField("ToDate");
                                stateIndex = pHistoFeature.Fields.FindField("State");
                                sourceIDIndex = pHistoFeature.Fields.FindField("SourceOID");
                                if (fromdateIndex == -1 || todateIndex == -1 || stateIndex == -1 || sourceIDIndex == -1)
                                {
                                    eError = new Exception("�Ҳ�����ʷ������Ӧ���ֶΣ�");
                                    //v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "��ʾ!", "�Ҳ�����ʷ������Ӧ���ֶΣ�" });
                                    //�ı���ͼ����״̬
                                    v_AppForm.MainForm.Invoke(new ChangeSelectNode(ChangeTreeSelectNode), new object[] { FcNode, "ʧ��", "", false });
                                }
                                //����ʷ�����������ӵ��ֶθ�ֵ
                                pHistoFeature.set_Value(fromdateIndex, pDate.ToString("u") as object);
                                pHistoFeature.set_Value(todateIndex, DateTime.MaxValue.ToString("u") as object);
                            }
                            #endregion

                            //�����޸ĵ�Ҫ�أ����������ƿ�����ҵ���Ӧ��Ҫ�أ�Ȼ���޸��ֶε�ֵ
                            IFeatureCursor pUpdateCurosr = null; //���޸ġ���Ҫ���α�
                            IFeature pUpdateFeature = null;//"�޸�"��Ҫ��

                            XmlNode FeatureNode = FeaStateItem.Key;//Ҫ�ؽڵ�
                            int ppOID = FeaStateItem.Value;//�ɵ�Ҫ�ص�OID
                            if (ppOID != -1)
                            {
                                //�޸ĵ�Ҫ��
                                IQueryFilter ppFilter = new QueryFilterClass();
                                ppFilter.WhereClause = "OBJECTID=" + ppOID;
                                pUpdateCurosr = pFeaCLs.Update(ppFilter, false);
                                if (pUpdateCurosr == null) return false;
                                pUpdateFeature = pUpdateCurosr.NextFeature();
                                if (pUpdateFeature == null)
                                {
                                    eError = new Exception("�����ƿ����Ҳ���OBJECTIDΪ" + ppOID + "�޸ĵ�Ҫ�أ�");
                                    //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ƿ����Ҳ���OBJECTIDΪ"+ppOID+"�޸ĵ�Ҫ�أ�");
                                    return false;
                                }
                            }
                            XmlNodeList valueNodeList = FeatureNode.SelectNodes(".//Value");
                            //��Ҫ�ظ�ֵ������value�ڵ㣬���Ҫ��ÿ���ֶε�ֵ
                            foreach (XmlNode valueNode in valueNodeList)
                            {
                                string fielName = ""; //�ֶ�����
                                string fieldVaue = "";//�ֶ�ֵ
                                int fieldIndex = -1; //�ֶ�����
                                fielName = valueNode.SelectSingleNode(".//FieldName").InnerText.Trim();
                                if (fielName == "GOFID") continue;
                                fieldVaue = valueNode.SelectSingleNode(".//FieldValue").InnerText.Trim();
                                if (fieldVaue == "") continue;
                                fieldIndex = pFeaCLs.Fields.FindField(fielName);
                                if (fieldIndex == -1)
                                {
                                    eError = new Exception("�Ҳ����ֶ���Ϊ'" + fielName.Trim() + "'���ֶ�,���飡");
                                    //v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����!", "�Ҳ����ֶ���Ϊ'" + fielName.Trim() + "'���ֶ�,���飡" });
                                    //�ı���ͼ����״̬
                                    v_AppForm.MainForm.Invoke(new ChangeSelectNode(ChangeTreeSelectNode), new object[] { FcNode, "ʧ��", "", false });
                                    return false;
                                }
                                IField pField = pFeaCLs.Fields.get_Field(fieldIndex);
                                if (pField.Editable == false) continue;
                                //����ͨ�ֶθ�ֵ
                                #region ���½�����Ҫ����⣬���޸ġ���Ҫ�ضԿ��е����ݽ����޸�
                                if (pField.Type != esriFieldType.esriFieldTypeBlob && pField.Type != esriFieldType.esriFieldTypeGeometry)
                                {
                                    if (ppOID == -1)
                                    {
                                        //�½���Ҫ��
                                        pFeatureBuffer.set_Value(fieldIndex, fieldVaue as object);
                                    }
                                    else
                                    {
                                        //�޸ĵ�Ҫ��
                                        pUpdateFeature.set_Value(fieldIndex, fieldVaue as object);
                                    }
                                    if (isConne == true)
                                    {
                                        //������ʷ����˵���޸ĺ��½���Ҫ�ض�Ҫ���뵽��ʷ����
                                        pHistoFeature.set_Value(fieldIndex, fieldVaue as object);
                                    }
                                }
                                else
                                {
                                    # region �����ֶξ���������ֵ
                                    //��xml�ַ���ת��Ϊ�ֽ�
                                    byte[] xmlByte = Convert.FromBase64String(fieldVaue);
                                    //������¼��������ֶ�ֵ
                                    object fieldShape = null;

                                    //���ݲ�ͬ�������fieldShape���г�ʼ��
                                    if (pFeaCLs.FeatureType == esriFeatureType.esriFTAnnotation)
                                    {
                                        //ע��
                                        if (pField.Type == esriFieldType.esriFieldTypeGeometry)
                                        {
                                            fieldShape = new PolygonElementClass();//�����ֶ�
                                        }
                                        if (pField.Type == esriFieldType.esriFieldTypeBlob)
                                        {
                                            fieldShape = new TextElementClass();//BLOB�ֶ�
                                        }
                                    }
                                    else
                                    {
                                        //��ͨҪ����
                                        if (pFeaCLs.ShapeType == esriGeometryType.esriGeometryPoint)
                                        {
                                            fieldShape = new PointClass();
                                        }
                                        else if (pFeaCLs.ShapeType == esriGeometryType.esriGeometryPolyline)
                                        {
                                            fieldShape = new PolylineClass();
                                        }
                                        else if (pFeaCLs.ShapeType == esriGeometryType.esriGeometryPolygon)
                                        {
                                            fieldShape = new PolygonClass();
                                        }
                                    }

                                    //���ֶν��н�������ֵ
                                    if (XmlDeSerializer(xmlByte, fieldShape) == true)
                                    {
                                        if (pField.Type == esriFieldType.esriFieldTypeGeometry)
                                        {
                                            if (ppOID == -1)
                                            {
                                                //�½���Ҫ��
                                                pFeatureBuffer.Shape = fieldShape as IGeometry;
                                                //pFeatureBuffer.set_Value(fieldIndex, fieldShape);
                                            }
                                            else
                                            {
                                                //�޸ĵ�Ҫ��
                                                pUpdateFeature.Shape = fieldShape as IGeometry;
                                            }
                                            if (isConne == true)
                                            {
                                                //����ʷ���Geometry�ֶθ�ֵ
                                                pHistoFeature.Shape = fieldShape as IGeometry;
                                            }
                                        }
                                        else if (pField.Type == esriFieldType.esriFieldTypeBlob)
                                        {
                                            if (ppOID == -1)
                                            {
                                                //�½���Ҫ��
                                                IAnnotationFeature pAnnoFeature = pFeatureBuffer as IAnnotationFeature;
                                                pAnnoFeature.Annotation = fieldShape as IElement;
                                            }
                                            else
                                            {
                                                //�޸ĵ�Ҫ��
                                                IAnnotationFeature pUpdateAnnoFeature = pUpdateFeature as IAnnotationFeature;
                                                pUpdateAnnoFeature.Annotation = fieldShape as IElement;
                                            }
                                            if (isConne == true)
                                            {
                                                //����ʷ����ע�ǵ������ֶθ�ֵ
                                                IAnnotationFeature pHistoAnnoFeature = pHistoFeature as IAnnotationFeature;
                                                pHistoAnnoFeature.Annotation = fieldShape as IElement;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        eError = new Exception("�����ֶ�ֵ����");
                                        //v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "�����ֶ�ֵ����" });
                                        //�ı���ͼ����״̬
                                        v_AppForm.MainForm.Invoke(new ChangeSelectNode(ChangeTreeSelectNode), new object[] { FcNode, "ʧ��", "", false });
                                        return false;
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            //�洢Ҫ�أ����޸�FID��¼��
                            if (ppOID == -1)
                            {
                                //�½���Ҫ��
                                int newOID = (int)pFeaCusor.InsertFeature(pFeatureBuffer);
                                //���½���Ҫ����Ϣд��FID��¼��
                                if (UpdateFIDTable(item.Key, newOID, v_Table, out ex) == false)
                                {
                                    eError = ex;
                                    //v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����!", "�޸�FID��¼��ʧ�ܣ�" });
                                    //�ı���ͼ����״̬
                                    v_AppForm.MainForm.Invoke(new ChangeSelectNode(ChangeTreeSelectNode), new object[] { FcNode, "ʧ��", "", false });
                                    return false;
                                }
                                if (isConne == true)
                                {
                                    //����ʷ�����������ӵ��ֶθ�ֵ���½���Ҫ�ص�״ֵ̬��
                                    pHistoFeature.set_Value(stateIndex, 1 as object);
                                    pHistoFeature.set_Value(sourceIDIndex, newOID as object);
                                }
                            }
                            else
                            {
                                //�޸�Ŀ�������Ҫ��
                                pUpdateCurosr.UpdateFeature(pUpdateFeature);
                                pUpdateCurosr.Flush();

                                if (isConne == true)
                                {
                                    //����ʷ�����������ӵ��ֶθ�ֵ���޸ĵ�Ҫ�أ�
                                    pHistoFeature.set_Value(stateIndex, 2 as object);
                                    pHistoFeature.set_Value(sourceIDIndex, ppOID as object);
                                }
                            }
                            if (isConne == true)
                            {
                                //�洢��ʷ���е�Ҫ��
                                pHistoFeature.Store();
                            }
                            //�ͷ��޸��α�
                            if (pUpdateCurosr != null)
                            {
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(pUpdateCurosr);
                            }
                            tempValue += 1;//��������ֵ��1
                            v_AppForm.MainForm.Invoke(new ChangeProgress(ChangeProgressBar), new object[] { v_AppForm.ProgressBar, -1, -1, tempValue });
                        }
                        if (pFeaCusor != null)
                        {
                            pFeaCusor.Flush();
                        }
                        //�ͷŲ����α�
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaCusor);
                        #endregion


                        //�ı���ͼ����״̬
                        v_AppForm.MainForm.Invoke(new ChangeSelectNode(ChangeTreeSelectNode), new object[] { FcNode, "���", item.Value.Count + "��Ҫ��������", false });
                    }
                    catch (Exception eex)
                    {
                        //*******************************************************************
                        //Exception Log
                        if (ModData.SysLog != null)
                        {
                            ModData.SysLog.Write(eex, null, DateTime.Now);
                        }
                        else
                        {
                            ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                            ModData.SysLog.Write(eex, null, DateTime.Now);
                        }
                        //********************************************************************
                        eError = eex;
                        //v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����!", eex.Message });
                        //�ı���ͼ����״̬
                        v_AppForm.MainForm.Invoke(new ChangeSelectNode(ChangeTreeSelectNode), new object[] { FcNode, "ʧ��", "", false });
                        return false;
                    }
                }
                return true;
            }
            catch (Exception eeex)
            {
                //*******************************************************************
                //Exception Log
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(eeex, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(eeex, null, DateTime.Now);
                }
                //********************************************************************
                eError = eeex;
                return false;
            }

        }

        /// <summary>
        /// ����DGML�ύ���޸�FID��¼��
        /// </summary>
        /// <param name="FCName">ͼ����</param>
        /// <param name="newOID">���º��OID</param>
        /// <param name="pSysTable">FID��¼������</param>
        /// <returns></returns>
        private bool UpdateFIDTable(string FCName, int newOID, SysCommon.DataBase.SysTable v_Table, out Exception eError)
        {
            eError = null;
            Exception ex = null;
            string str = "insert into FID��¼��(FCNAME,OID) values('" + FCName + "'," + newOID.ToString() + ")";
            v_Table.UpdateTable(str, out ex);
            if (ex != null)
            {
                eError = new Exception("�޸�FID��¼��ʧ�ܣ�");
                //v_AppForm.MainForm.Invoke(new ShowForm(ShowErrForm), new object[] { "����", "����FID��¼�����" });
                return false;
            }
            return true;
        }

        /// <summary>
        /// ��xmlByte����Ϊobj
        /// </summary>
        /// <param name="xmlByte"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool XmlDeSerializer(byte[] xmlByte, object obj)
        {
            try
            {
                //�ж��ַ����Ƿ�Ϊ��
                if (xmlByte != null)
                {
                    ESRI.ArcGIS.esriSystem.IPersistStream pStream = obj as ESRI.ArcGIS.esriSystem.IPersistStream;

                    ESRI.ArcGIS.esriSystem.IXMLStream xmlStream = new ESRI.ArcGIS.esriSystem.XMLStreamClass();

                    xmlStream.LoadFromBytes(ref xmlByte);
                    pStream.Load(xmlStream as ESRI.ArcGIS.esriSystem.IStream);

                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                //*******************************************************************
                //Exception Log
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(e, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(e, null, DateTime.Now);
                }
                //********************************************************************
                return false;
            }
        }
        #endregion

        #region ������ͼ��غ���
        //����������ͼ
        private delegate void InitTree(DevComponents.AdvTree.AdvTree aTree);
        private void IntialTree(DevComponents.AdvTree.AdvTree aTree)
        {
            DevComponents.AdvTree.ColumnHeader aColumnHeader;
            aColumnHeader = new DevComponents.AdvTree.ColumnHeader();
            aColumnHeader.Name = "FCName";
            aColumnHeader.Text = "ͼ����";
            aColumnHeader.Width.Relative = 50;
            aTree.Columns.Add(aColumnHeader);

            aColumnHeader = new DevComponents.AdvTree.ColumnHeader();
            aColumnHeader.Name = "NodeState";
            aColumnHeader.Text = "״̬";
            aColumnHeader.Width.Relative = 25;
            aTree.Columns.Add(aColumnHeader);

            aColumnHeader = new DevComponents.AdvTree.ColumnHeader();
            aColumnHeader.Name = "NodeRes";
            aColumnHeader.Text = "���";
            aColumnHeader.Width.Relative = 25;
            aTree.Columns.Add(aColumnHeader);
        }
        //����ѡ�����ڵ����ɫ
        private delegate void SetSelectNodeColor(DevComponents.AdvTree.AdvTree aTree);
        private void setNodeColor(DevComponents.AdvTree.AdvTree aTree)
        {
            DevComponents.DotNetBar.ElementStyle elementStyle = new DevComponents.DotNetBar.ElementStyle();
            elementStyle.BackColor = Color.FromArgb(255, 244, 213);
            elementStyle.BackColor2 = Color.FromArgb(255, 216, 105);
            elementStyle.BackColorGradientAngle = 90;
            elementStyle.Border = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderBottomWidth = 1;
            elementStyle.BorderColor = Color.DarkGray;
            elementStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderLeftWidth = 1;
            elementStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderRightWidth = 1;
            elementStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderTopWidth = 1;
            elementStyle.BorderWidth = 1;
            elementStyle.CornerDiameter = 4;
            elementStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            aTree.NodeStyleSelected = elementStyle;
            aTree.DragDropEnabled = false;
        }

        //������ͼ�ڵ�
        private delegate DevComponents.AdvTree.Node CreateTreeNode(DevComponents.AdvTree.NodeCollection nodeCol, string strText, string strName, Image pImage, bool bExpand);
        private DevComponents.AdvTree.Node CreateAdvTreeNode(DevComponents.AdvTree.NodeCollection nodeCol, string strText, string strName, Image pImage, bool bExpand)
        {

            DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node();
            node.Text = strText;
            node.Image = pImage;
            if (strName != null)
            {
                node.Name = strName;
            }

            if (bExpand == true)
            {
                node.Expand();
            }

            nodeCol.Add(node);
            return node;
        }

        //�����ͼ�ڵ���
        private delegate DevComponents.AdvTree.Cell CreateTreeCell(DevComponents.AdvTree.Node aNode, string strText, Image pImage);
        private DevComponents.AdvTree.Cell CreateAdvTreeCell(DevComponents.AdvTree.Node aNode, string strText, Image pImage)
        {
            DevComponents.AdvTree.Cell aCell = new DevComponents.AdvTree.Cell(strText);
            aCell.Images.Image = pImage;
            aNode.Cells.Add(aCell);

            return aCell;
        }

        //Ϊ���ݴ�����ͼ�ڵ���Ӵ�����״̬
        private delegate void ChangeSelectNode(DevComponents.AdvTree.Node aNode, string strMemo, string strRes, bool bClear);
        private void ChangeTreeSelectNode(DevComponents.AdvTree.Node aNode, string strMemo, string strRes, bool bClear)
        {
            if (aNode == null)
            {
                v_AppGIS.DataTree.SelectedNode = null;
                v_AppGIS.DataTree.Refresh();
                return;
            }

            v_AppGIS.DataTree.SelectedNode = aNode;
            if (bClear)
            {
                v_AppGIS.DataTree.SelectedNode.Nodes.Clear();
            }
            v_AppGIS.DataTree.SelectedNode.Cells[1].Text = strMemo;
            v_AppGIS.DataTree.SelectedNode.Cells[2].Text = strRes;
            v_AppGIS.DataTree.Refresh();
        }
        #endregion

        #region ��������ʾ
        //���ƽ�������ʾ
        private delegate void ShowProgress(bool bVisible);
        private void ShowProgressBar(bool bVisible)
        {
            if (bVisible == true)
            {
                v_AppForm.ProgressBar.Visible = true;
            }
            else
            {
                v_AppForm.ProgressBar.Visible = false;
            }
        }
        //�޸Ľ�����
        private delegate void ChangeProgress(DevComponents.DotNetBar.ProgressBarItem pProgressBar, int min, int max, int value);
        private void ChangeProgressBar(DevComponents.DotNetBar.ProgressBarItem pProgressBar, int min, int max, int value)
        {
            if (min != -1)
            {
                pProgressBar.Minimum = min;
            }
            if (max != -1)
            {
                pProgressBar.Maximum = max;
            }
            pProgressBar.Value = value;
            pProgressBar.Refresh();
        }


        //�ı�״̬����ʾ����
        private delegate void ShowTips(string strText);
        private void ShowStatusTips(string strText)
        {
            v_AppForm.OperatorTips = strText;
        }
        #endregion

        #region ��ʾ�Ի���
        private delegate void ShowForm(string strCaption, string strText);
        private void ShowErrForm(string strCaption, string strText)
        {
            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle(strCaption, strText);
        }
        private delegate bool ShowInfoForm();
        private bool ShowInfoDial(string StrOK, string StrCancel, string StrDesc)
        {
            bool isHisto = true;
            SysCommon.Error.frmInformation InfoDial = new SysCommon.Error.frmInformation(StrOK, StrCancel, StrDesc);
            if (InfoDial.ShowDialog() == DialogResult.OK)
            {
                isHisto = true;
            }
            else
            {
                isHisto = false;
            }
            return isHisto;
        }
        #endregion
    }
}
