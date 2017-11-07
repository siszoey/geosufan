using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Threading;

namespace GeoDBATool
{
    /// <summary>
    /// ���Ƿ� ���
    /// </summary>
    public partial class frmInputUpdateData : DevComponents.DotNetBar.Office2007Form
    {
        Plugin.Application.IAppGISRef m_Hook;//�����ܴ���
        private System.Windows.Forms.Timer _timer;
        private clsSubmitByDGML pClsSubmitByDGML;
        public frmInputUpdateData(Plugin.Application.IAppGISRef pHook)
        {
            InitializeComponent();

            m_Hook = pHook;

            //��ʼ�����ݿ�����
            cmbType.Items.Clear();
            cmbHistoType.Items.Clear();
            comBoxType.Items.Clear();
            //Ŀ�����ݿ�
            cmbType.Items.AddRange(new object[] { "GDB", "PDB", "SDE" });
            if (cmbType.Items.Count > 0)
            {
                cmbType.SelectedIndex = 0;
            }
            //��ʷ��
            cmbHistoType.Items.AddRange(new object[] { "GDB", "PDB", "SDE" });
            if (cmbHistoType.Items.Count > 0)
            {
                cmbHistoType.SelectedIndex = 0;
            }
            //FID�� 
            comBoxType.Items.AddRange(new object[] { "SQL", "ACCESS", "ORACLE" });
            comBoxType.Text = "ACCESS";

            //��ʼ������������Ϣ
            InitiaDBConn(m_Hook);
        }
        //��XML�ж�ȡ�����Ϣ�ڽ����ϱ��ֳ���
        private void InitiaDBConn(Plugin.Application.IAppGISRef pHook)
        {
            XmlNode ProNode = pHook.ProjectTree.SelectedNode.Tag as XmlNode;
            //��ȡ���ƿ���Ϣ�������ڽ����ϱ��ֳ���
            XmlElement userDBElem = ProNode.SelectSingleNode(".//����//���ƿ�//������Ϣ") as XmlElement;
            cmbType.Text = userDBElem.GetAttribute("����").ToString().Trim();//����
            txtServer.Text = userDBElem.GetAttribute("������").ToString().Trim();
            txtInstance.Text = userDBElem.GetAttribute("������").ToString().Trim();
            txtDB.Text = userDBElem.GetAttribute("���ݿ�").ToString().Trim();
            txtUser.Text = userDBElem.GetAttribute("�û�").ToString().Trim();
            txtPassword.Text = userDBElem.GetAttribute("����").ToString().Trim();
            txtVersion.Text = userDBElem.GetAttribute("�汾").ToString().Trim();
            //��ȡ��ʷ����Ϣ�����ڽ����ϱ��ֳ���
            XmlElement historyDBElem = ProNode.SelectSingleNode(".//����//��ʷ��//������Ϣ") as XmlElement;
            cmbHistoType.Text = historyDBElem.GetAttribute("����").ToString().Trim();
            txtHistoServer.Text = historyDBElem.GetAttribute("������").ToString().Trim();
            txtHistoInstance.Text = historyDBElem.GetAttribute("������").ToString().Trim();
            txtHistoDB.Text = historyDBElem.GetAttribute("���ݿ�").ToString().Trim();
            txtHistoUser.Text = historyDBElem.GetAttribute("�û�").ToString().Trim();
            txtHistoPassword.Text = historyDBElem.GetAttribute("����").ToString().Trim();
            txtHistoVersion.Text = historyDBElem.GetAttribute("�汾").ToString().Trim();
            //��ȡFID��¼����Ϣ�����ڽ����ϱ��ֳ���
            XmlElement FIDDBElem = ProNode.SelectSingleNode(".//����//FID��¼��//������Ϣ") as XmlElement;
            comBoxType.Text = FIDDBElem.GetAttribute("����").ToString().Trim();
            textBoxXServer.Text = FIDDBElem.GetAttribute("������").ToString().Trim();
            textBoxXDB.Text = FIDDBElem.GetAttribute("���ݿ�").ToString().Trim();
            textBoxXUser.Text = FIDDBElem.GetAttribute("�û�").ToString().Trim();
            textBoxXPassword.Text = FIDDBElem.GetAttribute("����").ToString().Trim();
        }

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtServer.Text  = "";
            txtInstance.Text  = "";
            txtDB.Text="";
            txtUser.Text="";
            txtPassword.Text="";
            txtVersion.Text="SDE.DEFAULT";
            
            if (cmbType.Text == "PDB")
            {
                txtServer.Enabled = false;
                txtInstance.Enabled = false;
                txtDB.Enabled = true;
                txtUser.Enabled = false;
                txtPassword.Enabled = false;
                txtVersion.Enabled = false;

                btnDB.Visible = true;
                btnDB.Enabled = true;
            }
            else if (cmbType.Text  == "GDB")
            {
                txtServer.Enabled = false;
                txtInstance.Enabled = false;
                txtDB.Enabled = true;
                txtUser.Enabled = false;
                txtPassword.Enabled = false;
                txtVersion.Enabled = false;

                btnDB.Visible = true;
                btnDB.Enabled = true;
            }
            else if (cmbType.Text  == "SDE")
            {
                txtServer.Enabled = true;
                txtInstance.Enabled = true;
                txtDB.Enabled = true;
                txtUser.Enabled = true;
                txtPassword.Enabled = true;
                txtVersion.Enabled = true;

                btnDB.Visible = false;
                btnDB.Enabled = false;
            }
        }

        private void comBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxXServer.Text = "";
            textBoxXDB.Text = "";
            textBoxXUser.Text = "";
            textBoxXServer.Text = "";

            if (comBoxType.Text == "ACCESS")
            {
                textBoxXServer.Enabled=false;
                textBoxXDB.Enabled = true;
                textBoxXUser.Enabled = false;
                textBoxXPassword.Enabled = false;

                buttonXFile.Visible = true;
                buttonXFile.Enabled = true;
                textBoxXDB.Width = textBoxXUser.Width - buttonXFile.Width;
            }
            else if (comBoxType.Text == "SQL")
            {
                textBoxXServer.Enabled = true;
                textBoxXDB.Enabled = true;
                textBoxXUser.Enabled = true;
                textBoxXPassword.Enabled = true;

                buttonXFile.Visible = false;
                buttonXFile.Enabled = false;
                textBoxXDB.Width = textBoxXUser.Width;
            }
            else if (comBoxType.Text == "ORACLE")
            {
                textBoxXServer.Enabled =false;
                textBoxXDB.Enabled = true;
                textBoxXUser.Enabled = true;
                textBoxXPassword.Enabled = true;

                buttonXFile.Visible = false ;
                buttonXFile.Enabled = false ;
                textBoxXDB.Width = textBoxXUser.Width;
            }
        }

        private void btnDGML_Click(object sender, EventArgs e)
        {
            lVFileNames.Items.Clear();
            OpenFileDialog Ofd = new OpenFileDialog();
            Ofd.Title = "ѡ���ļ�";
            Ofd.Filter = "�ļ�(*.xml)|*.xml";
            Ofd.Multiselect = true;
            if (Ofd.ShowDialog() == DialogResult.OK)
            {
                if (Ofd.FileNames != null && Ofd.FileNames.Length > 0)
                {
                    ListViewItem[] lvItem = new ListViewItem[Ofd.FileNames.Length];
                    for (int i = 0; i < Ofd.FileNames.Length; i++)
                    {
                        lvItem[i] = new ListViewItem();
                        lvItem[i].Text = Ofd.FileNames[i];
                        lvItem[i].ToolTipText = Ofd.FileNames[i];
                    }
                    lVFileNames.Items.AddRange(lvItem);

                    //��XML�ļ��ж�ȡ��Ϣ����ʼ��������Ϣ
                    //XmlDocument DGMLXMl = new XmlDocument();
                    //DGMLXMl.Load(Ofd.FileNames[0]);
                    ////Ŀ������ӽڵ�
                    //XmlElement DBNode = DGMLXMl.SelectSingleNode(".//DGML//MetaInfo//ProjectInfo//OutDataBase") as XmlElement;
                    //cmbType.Text = DBNode.GetAttribute("DBType").Trim();//����
                    //txtServer.Text = DBNode.GetAttribute("Server").Trim();//������
                    //txtInstance.Text = DBNode.GetAttribute("ServiceName").Trim();//ʵ����
                    //txtDB.Text = DBNode.GetAttribute("DataBase").Trim();//���ݿ�
                    //txtUser.Text = DBNode.GetAttribute("User").Trim();//�û�
                    //txtPassword.Text = DBNode.GetAttribute("Password").Trim();//����
                    //txtVersion.Text = DBNode.GetAttribute("Version").Trim();//�汾
                }
            }
        }

        private void btnDB_Click(object sender, EventArgs e)
        {
            if (cmbType.Text == "PDB")
            {
                OpenFileDialog Ofd = new OpenFileDialog();
                Ofd.Title = "ѡ���ļ�";
                Ofd.Filter = "�ļ�(*.mdb)|*.mdb";
                Ofd.Multiselect = false;
                if (Ofd.ShowDialog() == DialogResult.OK)
                {
                    txtDB.Text = Ofd.FileName;
                }
            }
            if (cmbType.Text == "GDB")
            {
                FolderBrowserDialog FolderBrowser = new FolderBrowserDialog();
                if (FolderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (!FolderBrowser.SelectedPath.Contains(".gdb"))
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��GDB�͵����ݿ⣡");
                        return;
                    }
                    txtDB.Text = FolderBrowser.SelectedPath;
                }
            }
        }

        private void buttonXFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog Ofd = new OpenFileDialog();
            Ofd.Title = "ѡ���ļ�";
            Ofd.Filter = "�ļ�(*.mdb)|*.mdb";
            Ofd.Multiselect = false;
            if (Ofd.ShowDialog() == DialogResult.OK)
            {
                textBoxXDB.Text = Ofd.FileName;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
            //DGML�ĵ�����
           string[] fileNames = new string[lVFileNames.Items.Count];
            for (int i = 0; i < lVFileNames.Items.Count; i++)
            {
               
                fileNames[i] = lVFileNames.Items[i].Text;
            }
            //Ŀ���������
            //if (cmbType.Text.Trim() == "SDE")
            //{
            //    pSysDT.SetWorkspace(txtServer.Text.Trim(), txtInstance.Text.Trim(), txtDB.Text.Trim(), txtUser.Text.Trim(), txtPassword.Text.Trim(), txtVersion.Text.Trim(), out eError);
            //}
            //else if (cmbType.Text.Trim() == "PDB")
            //{
            //    pSysDT.SetWorkspace(txtDB.Text.Trim(), SysCommon.enumWSType.PDB, out eError);
            //}
            //else if (cmbType.Text.Trim() == "GDB")
            //{
            //    pSysDT.SetWorkspace(txtDB.Text.Trim(), SysCommon.enumWSType.GDB, out eError);
            //}
            //if (eError != null)
            //{
            //    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "Ŀ�����ݿ����ӳ���");
            //    return;
            //}

            //FID������
            string connectionStr = "";
            SysCommon.enumDBConType dbConType=new SysCommon.enumDBConType();
            SysCommon.enumDBType dbType=new SysCommon.enumDBType();
            if (comBoxType.Text.Trim()== "ACCESS")
            {
                connectionStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + textBoxXDB.Text.Trim() + ";Mode=Share Deny None;Persist Security Info=False";
                dbConType = SysCommon.enumDBConType.OLEDB;
                dbType = SysCommon.enumDBType.ACCESS;
                //pTable.SetDbConnection(connectionStr, SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out eError);
            }
            else if (comBoxType.Text.Trim() == "ORACLE")
            {
                //connectionStr = "data source=" + pServe + ";User Id=" + pUser + ";Password=" + pPassword+";";
                connectionStr = "Data Source=" + textBoxXDB.Text.Trim() + ";Persist Security Info=True;User ID=" + textBoxXUser.Text.Trim() + ";Password=" + textBoxXPassword.Text.Trim() + ";Unicode=True";
                dbConType = SysCommon.enumDBConType.ORACLE;
                dbType = SysCommon.enumDBType.ORACLE;
                //pTable.SetDbConnection(connectionStr, SysCommon.enumDBConType.ORACLE, SysCommon.enumDBType.ORACLE, out eError);
            }
            else if (comBoxType.Text.Trim() == "SQL")
            {
                connectionStr = "Data Source=" + textBoxXDB.Text.Trim() + ";Initial Catalog=" + textBoxXServer.Text.Trim() + ";User ID=" + textBoxXUser.Text.Trim() + ";Password=" + textBoxXPassword.Text.Trim();
                dbConType = SysCommon.enumDBConType.SQL;
                dbType = SysCommon.enumDBType.SQLSERVER;
                //pTable.SetDbConnection(connectionStr, SysCommon.enumDBConType.SQL, SysCommon.enumDBType.SQLSERVER, out eError);
            }

            if (fileNames == null || fileNames.Length == 0) return;

            if (dbConType.GetHashCode().ToString() == "" || dbType.GetHashCode().ToString() == "" || connectionStr == "") return;
            //SysCommon.DataBase.SysTable pTable = frmSubmitData.v_Table;//��ȡFID��������
            //SysCommon.Gis.SysGisDataSet pSysDT = frmSubmitData.v_SysDT;//��ȡĿ�����ݿ�����
            Plugin.Application.IAppFormRef pAppForm = m_Hook as Plugin.Application.IAppFormRef;

            //���������ύ
            pAppForm.OperatorTips = "����DGML���������ύ...";
            pClsSubmitByDGML = new clsSubmitByDGML(fileNames, connectionStr, dbConType, dbType, cmbType.Text.Trim(), txtServer.Text.Trim(), txtInstance.Text.Trim(), txtDB.Text.Trim(), txtUser.Text.Trim(), txtPassword.Text.Trim(), txtVersion.Text.Trim(),cmbHistoType.Text.Trim(),txtHistoServer.Text.Trim(),txtHistoInstance.Text.Trim(),txtHistoDB.Text.Trim(),txtHistoUser.Text.Trim(),txtHistoPassword.Text.Trim(),txtHistoVersion.Text.Trim(), m_Hook);
            //pClsSubmitByDGML.SubmitThread();
            Thread pThread = new Thread(new ThreadStart(pClsSubmitByDGML.SubmitThread));
            pClsSubmitByDGML.CurrentThread = pThread;
            m_Hook.CurrentThread = pThread;
            pThread.Start();


            //���ü�ʱ��ˢ��mapcontrol
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 800;
            _timer.Enabled = true;
            _timer.Tick += new EventHandler(Timer_Tick);

        }

        //���ü�ʱ��ˢ��mapcontrol
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (pClsSubmitByDGML.CurrentThread.ThreadState == ThreadState.Stopped)
            {
                if (pClsSubmitByDGML.Res == true)
                {
                    #region �������ϵ���Ϣд��XML���汣������
                    //XmlDocument ProXMlDoc = new XmlDocument();
                    //ProXMlDoc.Load(ModData.v_projectXML);
                    XmlNode ProjectNode = m_Hook.ProjectTree.SelectedNode.Tag as XmlNode;//ProXMlDoc.SelectSingleNode(".//����[@����='" + m_Hook.ProjectTree.SelectedNode.Name + "']");
                    //�������ϵ����ƿ���Ϣд�뵽XML��
                    XmlElement userDBElem = ProjectNode.SelectSingleNode(".//����//���ƿ�//������Ϣ") as XmlElement;
                    userDBElem.SetAttribute("����", cmbType.Text.ToString().Trim());
                    userDBElem.SetAttribute("������", txtServer.Text.Trim());
                    userDBElem.SetAttribute("������", txtInstance.Text.ToString().Trim());
                    userDBElem.SetAttribute("���ݿ�", txtDB.Text.Trim());
                    userDBElem.SetAttribute("�û�", txtUser.Text.Trim());
                    userDBElem.SetAttribute("����", txtPassword.Text.Trim());
                    userDBElem.SetAttribute("�汾", txtVersion.Text.Trim());
                    //�������ϵ���ʷ����Ϣд�뵽XML��
                    XmlElement historyDBElem = ProjectNode.SelectSingleNode(".//����//��ʷ��//������Ϣ") as XmlElement;
                    historyDBElem.SetAttribute("����", cmbHistoType.Text.ToString().Trim());
                    historyDBElem.SetAttribute("������", txtHistoServer.Text.Trim());
                    historyDBElem.SetAttribute("������", txtHistoInstance.Text.ToString().Trim());
                    historyDBElem.SetAttribute("���ݿ�", txtHistoDB.Text.Trim());
                    historyDBElem.SetAttribute("�û�", txtHistoUser.Text.Trim());
                    historyDBElem.SetAttribute("����", txtHistoPassword.Text.Trim());
                    historyDBElem.SetAttribute("�汾", txtHistoVersion.Text.Trim());
                    //�������ϵ�FID��¼�������Ϣд�뵽XML��
                    XmlElement FIDDBElem = ProjectNode.SelectSingleNode(".//����//FID��¼��//������Ϣ") as XmlElement;
                    FIDDBElem.SetAttribute("����", comBoxType.Text.ToString().Trim());
                    FIDDBElem.SetAttribute("������", textBoxXServer.Text.ToString().Trim());
                    FIDDBElem.SetAttribute("���ݿ�", textBoxXDB.Text.Trim());
                    FIDDBElem.SetAttribute("�û�", textBoxXUser.Text.Trim());
                    FIDDBElem.SetAttribute("����", textBoxXPassword.Text.Trim());

                    ProjectNode.OwnerDocument.Save(ModData.v_projectXML);
                    #endregion
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                    this.Show();
                _timer.Enabled = false;
            }
        }


        private void cmbHistoType_SelectedIndexChanged(object sender, EventArgs e)
        {
           txtHistoServer.Text="";
           txtHistoInstance.Text = "";
           txtHistoDB .Text = "";
           txtHistoUser .Text = "";
           txtHistoPassword .Text = "";
           txtHistoVersion.Text = "SDE.DEFAULT";

           if (cmbHistoType.Text == "PDB" || cmbHistoType.Text == "GDB")
            {
                txtHistoServer.Enabled = false;
                txtHistoInstance.Enabled = false;
                txtHistoDB.Enabled = true;
                txtHistoUser.Enabled = false;
                txtHistoPassword.Enabled = false;
                txtHistoVersion.Enabled = false;

               btnHistoDB .Visible = true;
               btnHistoDB.Enabled = true;
            }
            else if (cmbHistoType.Text == "SDE")
            {
                txtHistoServer.Enabled = true;
                txtHistoInstance.Enabled = true;
                txtHistoDB.Enabled = true;
                txtHistoUser.Enabled = true;
                txtHistoPassword.Enabled = true;
                txtHistoVersion.Enabled = true;

                btnHistoDB.Visible = false;
                btnHistoDB.Enabled = false;
            }
        }

        private void btnHistoDB_Click(object sender, EventArgs e)
        {
            if (cmbHistoType.Text == "PDB")
            {
                OpenFileDialog Ofd = new OpenFileDialog();
                Ofd.Title = "ѡ���ļ�";
                Ofd.Filter = "�ļ�(*.mdb)|*.mdb";
                Ofd.Multiselect = false;
                if (Ofd.ShowDialog() == DialogResult.OK)
                {
                   txtHistoDB.Text = Ofd.FileName;
                }
            }
            if (cmbHistoType.Text == "GDB")
            {
                FolderBrowserDialog FolderBrowser = new FolderBrowserDialog();
                if (FolderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (!FolderBrowser.SelectedPath.Contains(".gdb"))
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��GDB�͵����ݿ⣡");
                        return;
                    }
                    txtHistoDB.Text = FolderBrowser.SelectedPath;
                }
            }
        }
    }
}