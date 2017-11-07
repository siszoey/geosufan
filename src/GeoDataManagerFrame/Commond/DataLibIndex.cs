using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using GeoDataCenterFunLib;
using SysCommon.Gis;
using SysCommon;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

//���ݲֿ�Ŀ¼
namespace GeoDataManagerFrame
{
    public class DataLibIndex : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
        private Plugin.Application.IAppFormRef m_frmhook;
        private enumWSType wsType;
        public DataLibIndex()
        {
            base._Name = "GeoDataManagerFrame.DataLibIndex";
            base._Caption = "����Ŀ¼";
            base._Tooltip = "��������Ŀ¼";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "����Ŀ¼";
        }

        public override void OnClick()
        {
            if (m_Hook != null)
            {
                LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);

                if (log != null)
                {
                    log.Writelog("��������Ŀ¼");
                }


                //�������ӵ����ݿ���Ϣ ��ʼ�����ݵ�Ԫ��
                InitDataUnitTree();

                //��������
                if (m_Hook.gisDataSet == null)
                {
                    m_Hook.gisDataSet = new SysGisDataSet();
                }
                if (m_Hook.gisDataSet.WorkSpace == null)
                {
                    GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                    string mypath = dIndex.GetDbValue("dbServerPath");
                    string dbType = dIndex.GetDbValue("dbType");
                    string dataBase = dIndex.GetDbValue("dbServerName");
                    string user = dIndex.GetDbValue("dbUser");
                    string password = dIndex.GetDbValue("dbPassword");
                    string service = dIndex.GetDbValue("dbService");
                    string version = dIndex.GetDbValue("dbVersion");

                    Exception eError = null;
                    if (dbType.Equals("SDE"))
                    {
                        wsType = enumWSType.SDE;
                    }
                    else if (dbType.Equals("PDB"))
                    {
                        wsType = enumWSType.PDB;
                    }
                    else if (dbType.Equals("GDB"))
                    {
                        wsType = enumWSType.GDB;
                    }
                    IWorkspace pWorkspace = GetWorkspace(mypath, service, dataBase, user, password, version, wsType, out eError);
                }


                //��������ת����ͼ�ĵ�����
                   // m_Hook.IndextabControl.SelectedTab = m_Hook.IndextabControl.Tabs["PageIndex"];
            }

        }

        public void InitDataUnitTree()
        {
            //�� ���ݵ�Ԫ�� �л�ȡ��Ϣ
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strDispLevel = dIndex.GetXmlElementValue("UnitTree", "tIsDisp");//�Ƿ���м���ʼ�������ݵ�Ԫ��
            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            OleDbConnection mycon = new OleDbConnection(constr);   //����OleDbConnection����ʵ�����������ݿ�
            string strExp = "";
            strExp = "select * from " + "���ݵ�Ԫ��";
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            try
            {
                mycon.Open();

                //����datareader   ���������ӵ���     
                OleDbDataReader aReader = aCommand.ExecuteReader();
                m_Hook.DataUnitTree.Nodes.Clear();
                TreeNode tparent;
                tparent = new TreeNode();
                tparent.Text = "���ݵ�Ԫ";
                tparent.Tag = 0;

                TreeNode tRoot;
                tRoot = new TreeNode();
                tRoot = tparent;
                TreeNode tNewNode;
                TreeNode tNewNodeClild;
                TreeNode tNewLeafNode;
                m_Hook.DataUnitTree.Nodes.Add(tparent);
                while (aReader.Read())
                {
                    //�����������������1���Ǹ��ڵ�
                    //�˴�Ĭ�϶��Ѿ����������������ά���������ֵ�ά��������ʵ��
                    if (aReader["���ݵ�Ԫ����"].ToString().Equals("1")) //ʡ���ڵ�
                    {
                        if (strDispLevel.Equals("0"))
                        {
                            tNewNode = new TreeNode();
                            tNewNode.Text = aReader["��������"].ToString();
                            tNewNode.Name = aReader["��������"].ToString();
                            tparent.Nodes.Add(tNewNode);
                            tparent.Expand();
                            tparent = tNewNode;
                            tNewNode.Tag = 1;
                            tNewNode.ImageIndex = 17;
                        }
                 
                    }
                    else if (aReader["���ݵ�Ԫ����"].ToString().Equals("2")) //�м��ڵ�
                    {
                        tNewNodeClild = new TreeNode();
                        tNewNodeClild.Text = aReader["��������"].ToString();
                        tNewNodeClild.Name = aReader["��������"].ToString();
                        tparent.Nodes.Add(tNewNodeClild);
                        tparent.Expand();
                        tRoot = tNewNodeClild;
                        tNewNodeClild.Tag = 2;
                        tNewNodeClild.ImageIndex =17;

                    }
                    else if (aReader["���ݵ�Ԫ����"].ToString().Equals("3"))//�ؼ��ڵ�
                    {
                        tNewLeafNode = new TreeNode();
                        tNewLeafNode.Text = aReader["��������"].ToString();
                        tNewLeafNode.Name = aReader["��������"].ToString();
                        tRoot.Nodes.Add(tNewLeafNode);
                        tRoot.Expand();
                        tNewLeafNode.Tag = 3;
                        tNewLeafNode.ImageIndex = 17;
                        
                        string strSql = "";
                        string strScaleName = "";
                        string strSubTypeName = "";
                        string strLeafName = "";
                        string strYear = "";
                        string strSubType = "";
                        string strXzqName = "";
                        string strScale = "";

                        //����ר��ڵ�
                        CreateSubTreeItem(tNewLeafNode, constr);

                        //����Ҷ�ӽڵ�
                        TreeNode tLeafItem;
                        TreeNode tFindParentItem;
                        GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
                        strExp = "select * from ��ͼ�����Ϣ�� where �������� = '" + tNewLeafNode.Name + "'";
                        OleDbCommand aCommand2 = new OleDbCommand(strExp, mycon);
                        OleDbDataReader aReader2 = aCommand2.ExecuteReader();
                        while (aReader2.Read())
                        {
                            strYear = aReader2["���"].ToString();
                            strSubType = aReader2["ר������"].ToString();
                            strXzqName = aReader2["��������"].ToString();
                            strScale = aReader2["������"].ToString();

                            //��ȡ������
                            strSql = "select ���� from �����ߴ���� where ���� ='" + strScale + "'";
                            strScaleName = dDbFun.GetInfoFromMdbByExp(constr, strSql);

                            //��ȡר������
                            strSql = "select ���� from ��׼ר����Ϣ�� where ר������ ='" + strSubType + "'";
                            strSubTypeName = dDbFun.GetInfoFromMdbByExp(constr, strSql);


                            //��֯���ڵ� 
                            strLeafName = strYear + "�꡾" + strScaleName + "��";

                            //��ȡ�ϼ��ڵ�
                            tFindParentItem = FindNode(tNewLeafNode, strSubTypeName);

                            tFindParentItem.ImageIndex = 9;
                            tFindParentItem.SelectedImageIndex = 10;
                            //�˴�Ĭ�� ��ͼ�����Ϣ�������м�¼�Ѿ����� ���ݵ�Ԫ��ר����������
                            tLeafItem = new TreeNode();
                            tLeafItem.Text = strLeafName;
                            tLeafItem.Tag = strSubType;  //��ʱ��ר�����ͼ�¼��tag��
                            tFindParentItem.Nodes.Add(tLeafItem);
                            //tFindParentItem.ExpandAll();
                            tLeafItem.ImageIndex = 7;
                            tLeafItem.SelectedImageIndex = 8;
                        }
                        aReader2.Close();
                    }
                    else
                    {
                        tNewNodeClild = new TreeNode();
                        tNewNodeClild.Text = aReader["��������"].ToString();
                        tNewNodeClild.Name = aReader["��������"].ToString();
                        tparent.Nodes.Add(tNewNodeClild);
                        tparent.ExpandAll();
                        tNewNodeClild.Tag = 1;
                    }
                }

                //�ر�reader����     
                aReader.Close();
                

                //�ر�����,�����Ҫ     
                mycon.Close();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //���ݽڵ����ƻ�ȡ�ڵ�
        private TreeNode FindNode(TreeNode tnParent, string strValue)
        {
            if (tnParent == null)
                return null;
            if (tnParent.Text == strValue)
                return tnParent;
            TreeNode tnRet = null;
            foreach (TreeNode tn in tnParent.Nodes)
            {
                tnRet = FindNode(tn, strValue);
                if (tnRet != null)
                    break;
            }
            return tnRet;
        } 

        /// <summary>
        /// ����ר��ڵ�
        /// </summary>
        /// <param name="tRootItem">�ؼ��������ڵ�</param>
        /// <param name="strCon">�������</param>
        public void CreateSubTreeItem(TreeNode tRootItem, string strCon)
        {
            string strXzqCode = tRootItem.Name;
            GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
            string strExp = "select distinct ר������ from ��ͼ�����Ϣ�� where �������� = '" + strXzqCode + "'";
            OleDbConnection mycon = new OleDbConnection(strCon);   //����OleDbConnection����ʵ�����������ݿ�
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            try
            {
                mycon.Open();
                OleDbDataReader aReader = aCommand.ExecuteReader();
                string strSql = "";
                string strSubType = "";
                string strSubTypeName = "";
                TreeNode tSubItem;

                while (aReader.Read())
                {
                    strSubType = aReader["ר������"].ToString();

                    //��ȡר������
                    strSql = "select ���� from ��׼ר����Ϣ�� where ר������ ='" + strSubType + "'";
                    strSubTypeName = dDbFun.GetInfoFromMdbByExp(strCon, strSql);


                    tSubItem = new TreeNode();
                    tSubItem.Text = strSubTypeName;
                    tRootItem.Nodes.Add(tSubItem);
                    //tRootItem.ExpandAll();
                }

                //�ر�reader����     
                aReader.Close();

                //�ر�����     
                mycon.Close();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// �õ������ռ�
        /// </summary>
        /// <param name="eError"></param>
        /// <returns></returns>
        public IWorkspace GetWorkspace(string server, string service, string dataBase, string user, string password, string version, enumWSType wsType, out Exception eError)
        {
            eError = null;
            bool result = false;

            if (m_Hook.gisDataSet == null)
            {
                m_Hook.gisDataSet = new SysGisDataSet();
            }
            try
            {
                switch (wsType)
                {
                    case enumWSType.SDE:
                        result = m_Hook.gisDataSet.SetWorkspace(server, service, dataBase, user, password, version, out eError);
                        break;
                    case enumWSType.PDB:
                    case enumWSType.GDB:
                        result = m_Hook.gisDataSet.SetWorkspace(server, wsType, out eError);
                        break;
                    default:
                        break;
                }
                if (result)
                {
                    return m_Hook.gisDataSet.WorkSpace;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                eError = ex;
                return null;
            }
        }


        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppGisUpdateRef;
            m_frmhook = hook as Plugin.Application.IAppFormRef;
        }
    }
}
