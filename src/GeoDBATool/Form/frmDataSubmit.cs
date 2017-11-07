using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using System.IO;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;


namespace GeoDBATool
{
    /// <summary>
    /// ���Ƿ����
    /// </summary>
    public partial class frmDataSubmit : DevComponents.DotNetBar.Office2007Form
    {
        Plugin.Application.IAppGISRef v_AppGIS;
        EnumOperateType v_OpeType;
        private clsDataThread m_DataThread;
        private System.Windows.Forms.Timer _timer;
        private IGeometry m_Geometry = null;

        public frmDataSubmit(EnumOperateType pType, Plugin.Application.IAppGISRef pAppGIS)
        {
            InitializeComponent();
            v_OpeType = pType;
            v_AppGIS = pAppGIS;
            InitForm();
        }
        private void InitForm()
        {
            object[] TagDBType = new object[] { "ESRI�ļ����ݿ�(*.gdb)", "ArcSDE(For Oracle)", "ESRI�������ݿ�(*.mdb)" };//"GDB", "SDE", "PDB"
            comboBoxOrgType.Items.AddRange(TagDBType);//Դ����
            comboBoxOrgType.SelectedIndex = 0;
            comboBoxOrgType.Enabled = false;
            comBoxType.Items.AddRange(TagDBType);//Ŀ������
            comBoxType.SelectedIndex = 0;
            cmbHistoType.Items.AddRange(TagDBType);//��ʷ��
            cmbHistoType.SelectedIndex = 0;

            groupPanel3.Visible = false;

            TagDBType = new object[] { "ORACLE", "ACCESS", "SQL" };
            cmbFIDType.Items.AddRange(TagDBType);//FID��¼��
            cmbFIDType.SelectedIndex = 0;
            #region ��xml�ж�ȡ��Ϣ���ұ����ڽ�����
            XmlNode ProNode = v_AppGIS.ProjectTree.SelectedNode.Tag as XmlNode;
            //ͼ����������Ϣ
            XmlElement workDBElem = ProNode.SelectSingleNode(".//����//ͼ��������//������Ϣ") as XmlElement;
            //cyf 20110628 modify:����ͳһ
            if (workDBElem.GetAttribute("����").Trim() == "PDB")
            {
                comboBoxOrgType.Text = "ESRI�������ݿ�(*.mdb)";
            }
            else if (workDBElem.GetAttribute("����").Trim() == "GDB")
            {
                comboBoxOrgType.Text = "ESRI�ļ����ݿ�(*.gdb)";
            }
            else if (workDBElem.GetAttribute("����").Trim() == "SDE")
            {
                comboBoxOrgType.Text = "ArcSDE(For Oracle)";
            }
            //comboBoxOrgType.Text = workDBElem.GetAttribute("����").ToString().Trim();
            //end
            textBoxX1.Text = workDBElem.GetAttribute("���ݿ�").ToString().Trim();
            //���ƿ���Ϣ
            XmlElement userDBElem = ProNode.SelectSingleNode(".//����//���ƿ�//������Ϣ") as XmlElement;
            //cyf 20110628 modify:����ͳһ
            if (userDBElem.GetAttribute("����").Trim() == "PDB")
            {
                comBoxType.Text = "ESRI�������ݿ�(*.mdb)";
            }
            else if (userDBElem.GetAttribute("����").Trim() == "GDB")
            {
                comBoxType.Text = "ESRI�ļ����ݿ�(*.gdb)";
            }
            else if (userDBElem.GetAttribute("����").Trim() == "SDE")
            {
                comBoxType.Text = "ArcSDE(For Oracle)";
            }
            //comBoxType.Text = userDBElem.GetAttribute("����").Trim();
            //end
            txtServer.Text = userDBElem.GetAttribute("������").Trim();
            txtInstance.Text = userDBElem.GetAttribute("������").Trim();
            txtDB.Text = userDBElem.GetAttribute("���ݿ�").Trim();
            txtUser.Text = userDBElem.GetAttribute("�û�").Trim();
            txtPassword.Text = userDBElem.GetAttribute("����").Trim();
            txtVersion.Text = userDBElem.GetAttribute("�汾").Trim();
            //��ʷ����Ϣ
            XmlElement historyDBElem = ProNode.SelectSingleNode(".//����//��ʷ��//������Ϣ") as XmlElement;
            //cyf 20110628 modify:����ͳһ
            if (historyDBElem.GetAttribute("����").Trim() == "PDB")
            {
                cmbHistoType.Text = "ESRI�������ݿ�(*.mdb)";
            }
            else if (historyDBElem.GetAttribute("����").Trim() == "GDB")
            {
                cmbHistoType.Text = "ESRI�ļ����ݿ�(*.gdb)";
            }
            else if (historyDBElem.GetAttribute("����").Trim() == "SDE")
            {
                cmbHistoType.Text = "ArcSDE(For Oracle)";
            }
            //cmbHistoType.Text = historyDBElem.GetAttribute("����").Trim();
            //end
            txtHistoServer.Text = historyDBElem.GetAttribute("������").Trim();
            txtHistoInstance.Text = historyDBElem.GetAttribute("������").Trim();
            txtHistoDB.Text = historyDBElem.GetAttribute("���ݿ�").Trim();
            txtHistoUser.Text = historyDBElem.GetAttribute("�û�").Trim();
            txtHistoPassword.Text = historyDBElem.GetAttribute("����").Trim();
            txtHistoVersion.Text = historyDBElem.GetAttribute("�汾").Trim();
            //FID��¼��������Ϣ
            //XmlElement FIDDBElem = ProNode.SelectSingleNode(".//����//FID��¼��//������Ϣ") as XmlElement;
            //cmbFIDType.Text = FIDDBElem.GetAttribute("����").Trim();
            //txtFIDServer.Text = FIDDBElem.GetAttribute("������").Trim();
            //txtFIDInstance.Text = FIDDBElem.GetAttribute("���ݿ�").Trim();
            //txtFIDUser.Text = FIDDBElem.GetAttribute("�û�").Trim();
            //txtFIDPassword.Text = FIDDBElem.GetAttribute("����").Trim();
            #endregion
        }

        private void comBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //cyf 20110628 
            if (comBoxType.Text == "ESRI�������ݿ�(*.mdb)")
            {
                comBoxType.Tag = "PDB";
            }
            else if (comBoxType.Text == "ESRI�ļ����ݿ�(*.gdb)")
            {
                comBoxType.Tag = "GDB";
            }
            else if (comBoxType.Text == "ArcSDE(For Oracle)")
            {
                comBoxType.Tag = "SDE";
            }
            //end
            if (comBoxType.Text != "ArcSDE(For Oracle)")
            {
                btnDB.Visible = true;
                txtDB.Size = new Size(txtServer.Size.Width - btnDB.Size.Width, txtDB.Size.Height);
                txtServer.Enabled = false;
                txtInstance.Enabled = false;
                txtUser.Enabled = false;
                txtPassword.Enabled = false;
                txtVersion.Enabled = false;
            }
            else
            {
                btnDB.Visible = false;
                txtDB.Size = new Size(txtServer.Size.Width, txtDB.Size.Height);
                txtServer.Enabled = true;
                txtInstance.Enabled = true;
                txtUser.Enabled = true;
                txtPassword.Enabled = true;
                txtVersion.Enabled = true;

            }
        }

        private void cmbFIDType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFIDType.Text == "ACCESS")
            {
                btnAccess.Visible = true;
                txtFIDInstance.Size = new Size(txtFIDUser.Size.Width - btnAccess.Size.Width, txtFIDUser.Size.Height);
                txtFIDServer.Enabled = false;
                txtFIDUser.Enabled = false;
                txtFIDPassword.Enabled = false;
            }
            else if (cmbFIDType.Text == "ORACLE")
            {
                btnAccess.Visible = false;
                txtFIDInstance.Size = new Size(txtFIDUser.Size.Width, txtFIDUser.Size.Height);
                txtFIDServer.Enabled = false ;
                txtFIDUser.Enabled = true;
                txtFIDPassword.Enabled = true;
            }
            else
            {
                btnAccess.Visible = false;
                txtFIDInstance.Size = new Size(txtFIDUser.Size.Width, txtFIDUser.Size.Height);
                txtFIDServer.Enabled = true;
                txtFIDUser.Enabled = true;
                txtFIDPassword.Enabled = true;
            }
        }

        private void btnXML_Click(object sender, EventArgs e)
        {
            //OpenFileDialog OpenFile = new OpenFileDialog();
            //OpenFile.CheckFileExists = true;
            //OpenFile.CheckPathExists = true;
            //OpenFile.Title = "ѡ��ӳ���ļ�";
            //OpenFile.Filter = "ӳ���ļ�(*.xml)|*.xml";
            //if (OpenFile.ShowDialog() == DialogResult.OK)
            //{
            //    txtXML.Text = OpenFile.FileName;
            //}
        }

        private void btnDB_Click(object sender, EventArgs e)
        {
            switch (comBoxType.Text)
            {
                case "ArcSDE(For Oracle)":

                    break;

                case "ESRI�ļ����ݿ�(*.gdb)":
                    FolderBrowserDialog pFolderBrowser = new FolderBrowserDialog();
                    if (pFolderBrowser.ShowDialog() == DialogResult.OK)
                    {
                        if (!pFolderBrowser.SelectedPath.EndsWith(".gdb"))
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��GDB");
                            return;
                        }
                        txtDB.Text = pFolderBrowser.SelectedPath;
                    }
                    break;

                case "ESRI�������ݿ�(*.mdb)":
                    OpenFileDialog OpenFile = new OpenFileDialog();
                    OpenFile.Title = "ѡ��ESRI�������ݿ�";
                    OpenFile.Filter = "ESRI�������ݿ�(*.mdb)|*.mdb";
                    if (OpenFile.ShowDialog() == DialogResult.OK)
                    {
                        txtDB.Text = OpenFile.FileName;
                    }
                    break;

                default:
                    break;
            }
        }

        private void btnAccess_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Title = "ѡ��MDB����";
            OpenFile.Filter = "MDB����(*.mdb)|*.mdb";
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
               txtFIDInstance.Text = OpenFile.FileName;
            }
        }

        private void btnOrg_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog pFolderBrowser = new FolderBrowserDialog();
            if (pFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                if (!pFolderBrowser.SelectedPath.EndsWith(".gdb"))
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��GDB��ʽ�ļ�");
                    return;
                }
                this.textBoxX1.Text = pFolderBrowser.SelectedPath;
            }
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Exception eError = null;
            SysCommon.Gis.SysGisDataSet pHistoSysGisDT = new SysCommon.Gis.SysGisDataSet();//��ʷ������
            //�ж�Դ�����Ƿ�����
            if (comboBoxOrgType.Text == "ESRI�ļ����ݿ�(*.gdb)")
            {
                if (this.textBoxX1.Text == "")
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������Դ��������");
                    return;
                }
            }

            else if (comBoxType.Text == "ArcSDE(For Oracle)")
            {
                if (txtServer.Text==""||txtInstance.Text==""||txtDB.Text==""||txtVersion.Text==""|| txtUser.Text == "" || txtPassword.Text == "")
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������Ŀ����������!");
                    return;
                }
            }
            else
            {
                if (txtDB.Text == "")
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������Ŀ����������!");
                    return;
                }
            }

            string fileXml = "";
            if (v_OpeType == EnumOperateType.Submit)
            {
                //ͼ�������ύ
                fileXml = ModData.DBTufuInputXml;
            }
            if (!File.Exists(fileXml))
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���չ�ϵ�ļ�������!\n"+fileXml);
                return;
            }

            //�ж���ʷ���Ƿ����ӳɹ�
            bool isConSucc = true;
            if (cmbHistoType.Text == "ArcSDE(For Oracle)")
            {
                if (txtHistoServer.Text.Trim() == "" || txtHistoUser.Text.Trim() == "" || txtHistoPassword.Text.Trim() == "")
                {
                    isConSucc = false;
                }
                else
                {
                    pHistoSysGisDT.SetWorkspace(txtHistoServer.Text.Trim(), txtHistoInstance.Text.Trim(), txtHistoDB.Text.Trim(), txtHistoUser.Text.Trim(), txtHistoPassword.Text.Trim(), txtHistoVersion.Text.Trim(), out eError);
                    if (eError != null)
                    {
                        isConSucc = false;
                    }
                }
            }
            else if (cmbHistoType.Text.Trim() == "ESRI�ļ����ݿ�(*.gdb)")
            {
                if (txtHistoDB.Text.Trim() == "")
                {
                    isConSucc = false;
                }
                else
                {
                    pHistoSysGisDT.SetWorkspace(txtHistoDB.Text.Trim(), SysCommon.enumWSType.GDB, out eError);
                    if (eError != null)
                    {
                        isConSucc = false;
                    }
                }

            }
            else
            {
                if (txtHistoDB.Text.Trim() == "")
                {
                    isConSucc = false;
                }
                else
                {
                    pHistoSysGisDT.SetWorkspace(txtHistoDB.Text.Trim(), SysCommon.enumWSType.PDB, out eError);
                    if (eError != null)
                    {
                        isConSucc = false;
                    }
                }
            }
            if (!isConSucc)
            {
                //����ʷ�����Ӳ��ɹ�
                SysCommon.Error.frmInformation InfoDial = new SysCommon.Error.frmInformation("��", "��", "�Ƿ�������Ϊ��ʷ��");
                if (InfoDial.ShowDialog() == DialogResult.OK)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ʷ������ʧ��,������ѡ����ʷ�⣡");
                    return;
                }
            }
#region FID��¼���Ƿ����ӳɹ�
            //SysCommon.DataBase.SysDataBase pSysDB = new SysCommon.DataBase.SysDataBase();
            //string connectionStr = "";
            //switch (cmbFIDType.Text.ToUpper())
            //{
            //    case "ORACLE":
            //        connectionStr = "Data Source=" + txtFIDInstance.Text + ";Persist Security Info=True;User ID=" + txtFIDUser.Text + ";Password=" + txtFIDPassword.Text + ";Unicode=True";
            //        pSysDB.SetDbConnection(connectionStr, SysCommon.enumDBConType.ORACLE, SysCommon.enumDBType.ORACLE, out eError);
            //        break;
            //    case "ACCESS":
            //        connectionStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +txtFIDInstance.Text + ";Mode=Share Deny None;Persist Security Info=False";
            //        pSysDB.SetDbConnection(connectionStr, SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out eError);
            //        break;
            //    case "SQL":
            //        connectionStr = "Data Source=" + txtFIDInstance.Text + ";Initial Catalog=" + txtFIDServer.Text + ";User ID=" + txtFIDUser.Text + ";Password=" + txtFIDPassword.Text;
            //        pSysDB.SetDbConnection(connectionStr, SysCommon.enumDBConType.SQL, SysCommon.enumDBType.SQLSERVER, out eError);
            //        break;
            //}
            //if (eError != null)
            //{
            //    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "FID��¼�������ʧ��!");
            //    return;
            //}
#endregion

            //���ͼ���ܷ�Χ
            XmlNode ProNode =v_AppGIS.ProjectTree.SelectedNode.Tag as XmlNode;
            XmlElement workDBElem = ProNode.SelectSingleNode(".//����//ͼ��������//��Χ��Ϣ") as XmlElement;
            string rangeStr = workDBElem.GetAttribute("��Χ").Trim();
            byte[] xmlByte = Convert.FromBase64String(rangeStr);
            object pGeo = new PolygonClass();
            if (XmlDeSerializer(xmlByte, pGeo) == false)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���η�Χ��������");
                return;
            }
            m_Geometry = pGeo as IGeometry;
            if (m_Geometry == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("����", "ͼ����ΧΪ�գ�");
                return;
            }

            string pDateTimeStr = DateTime.Now.ToString("u");

            Plugin.Application.IAppFormRef pAppFormRef =v_AppGIS as Plugin.Application.IAppFormRef;
            pAppFormRef.OperatorTips = "��ȡ������Ϣ...";

            //���ݴ��������γ����ݲ���xml����
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<������ֲ></������ֲ>");
            XmlNode sourceNode = ModDBOperator.SelectNode(xmlDoc.DocumentElement as XmlNode, "Դ��������");

            XmlElement dataElement = xmlDoc.CreateElement("Դ����");
            dataElement.SetAttribute("����", comboBoxOrgType.Tag.ToString().Trim());
            if (comboBoxOrgType.Text.Trim() == "ESRI�ļ����ݿ�(*.gdb)")
            {
                dataElement.SetAttribute("������", "");
                dataElement.SetAttribute("������", "");
                dataElement.SetAttribute("���ݿ�", this.textBoxX1.Text);
                dataElement.SetAttribute("�û�", "");
                dataElement.SetAttribute("����", "");
                dataElement.SetAttribute("�汾", "");
            } 
            sourceNode.AppendChild((XmlNode)dataElement);

            XmlElement objElementTemp = ModDBOperator.SelectNode(xmlDoc.DocumentElement as XmlNode, "Ŀ����������") as XmlElement;
            objElementTemp.SetAttribute("����", comBoxType.Tag.ToString().Trim());
            objElementTemp.SetAttribute("������", txtServer.Text.Trim());
            objElementTemp.SetAttribute("������", txtInstance.Text.Trim());
            objElementTemp.SetAttribute("���ݿ�", txtDB.Text.Trim());
            objElementTemp.SetAttribute("�û�", txtUser.Text.Trim());
            objElementTemp.SetAttribute("����", txtPassword.Text.Trim());
            objElementTemp.SetAttribute("�汾", txtVersion.Text.Trim());

            XmlElement RuleElement = ModDBOperator.SelectNode(xmlDoc.DocumentElement as XmlNode, "����") as XmlElement;
            RuleElement.SetAttribute("·��", fileXml);
           
            pAppFormRef.OperatorTips = "��ʼ�����ݴ�����ͼ...";
            //��ʼ�����ݴ�����ͼ
            InitialDBTree newInitialDBTree = new InitialDBTree();
            newInitialDBTree.OnCreateDataTree(v_AppGIS.DataTree, xmlDoc);
            if ((bool)v_AppGIS.DataTree.Tag == false) return;

            //����������ֲ
            this.Hide();
            pAppFormRef.OperatorTips = "����ͼ�������ύ...";
            esriSpatialRelEnum RelEnum = esriSpatialRelEnum.esriSpatialRelIntersects;
			if (m_DataThread == null)
            {
            m_DataThread = new clsDataThread(v_AppGIS, m_Geometry, RelEnum, true, null, null, v_OpeType,pHistoSysGisDT,pDateTimeStr);
			}
            Thread aThread = new Thread(new ThreadStart(m_DataThread.DoBatchWorks));
            m_DataThread.CurrentThread = aThread;
			m_DataThread.UserName = txtHistoUser.Text;//��ʷ���û���  xisheng 2011.07.15
            m_DataThread.UserNameNow = txtUser.Text;//��ʵ���û���  xisheng 2011.07.15
            v_AppGIS.CurrentThread = aThread;
            aThread.Start();

            //���ü�ʱ��ˢ��mapcontrol
			if (_timer == null)
            {
            _timer = new System.Windows.Forms.Timer();
			}
            _timer.Interval = 800;
            _timer.Enabled = true;
            _timer.Tick += new EventHandler(Timer_Tick);
        }

        //���ü�ʱ��ˢ��mapcontrol
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (m_DataThread.CurrentThread.ThreadState == ThreadState.Stopped)
            {
                if (m_DataThread.Res == true)
                {
                    #region �������ϵ���Ϣд��XML���汣������
                    //XmlDocument ProXMlDoc = new XmlDocument();
                    //ProXMlDoc.Load(ModData.v_projectXML);
                    XmlNode ProjectNode = v_AppGIS.ProjectTree.SelectedNode.Tag as XmlNode;// ProXMlDoc.SelectSingleNode(".//����[@����='" + v_AppGIS.ProjectTree.SelectedNode.Name + "']");
                    //�������ϵ�ͼ����������Ϣд�뵽XML��
                    XmlElement workDBElem = ProjectNode.SelectSingleNode(".//����//ͼ��������//������Ϣ") as XmlElement;
                    workDBElem.SetAttribute("����", comboBoxOrgType.Tag.ToString().Trim());  //cyf 20110628
                    workDBElem.SetAttribute("���ݿ�", textBoxX1.Text.Trim());
                    //�������ϵ����ƿ���Ϣд�뵽XML��
                    XmlElement userDBElem = ProjectNode.SelectSingleNode(".//����//���ƿ�//������Ϣ") as XmlElement;
                    userDBElem.SetAttribute("����", comBoxType.Tag.ToString().Trim()); //cyf 20110628 
                    userDBElem.SetAttribute("������", txtServer.Text.Trim());
                    userDBElem.SetAttribute("������", txtInstance.Text.ToString().Trim());
                    userDBElem.SetAttribute("���ݿ�", txtDB.Text.Trim());
                    userDBElem.SetAttribute("�û�", txtUser.Text.Trim());
                    userDBElem.SetAttribute("����", txtPassword.Text.Trim());
                    userDBElem.SetAttribute("�汾", txtVersion.Text.Trim());
                    //�������ϵ���ʷ����Ϣд�뵽XML��
                    XmlElement historyDBElem = ProjectNode.SelectSingleNode(".//����//��ʷ��//������Ϣ") as XmlElement;
                    historyDBElem.SetAttribute("����", cmbHistoType.Tag.ToString().Trim());//cyf 20110628
                    historyDBElem.SetAttribute("������", txtHistoServer.Text.Trim());
                    historyDBElem.SetAttribute("������", txtHistoInstance.Text.ToString().Trim());
                    historyDBElem.SetAttribute("���ݿ�", txtHistoDB.Text.Trim());
                    historyDBElem.SetAttribute("�û�", txtHistoUser.Text.Trim());
                    historyDBElem.SetAttribute("����", txtHistoPassword.Text.Trim());
                    historyDBElem.SetAttribute("�汾", txtHistoVersion.Text.Trim());
                    //cyf 2011028
                    //�������ϵ�FID��¼�������Ϣд�뵽XML�� 
                    //XmlElement FIDDBElem = ProjectNode.SelectSingleNode(".//����//FID��¼��//������Ϣ") as XmlElement;
                    //FIDDBElem.SetAttribute("����", cmbFIDType.Text.ToString().Trim());
                    //FIDDBElem.SetAttribute("������", txtFIDServer.Text.ToString().Trim());
                    //FIDDBElem.SetAttribute("���ݿ�", txtFIDInstance.Text.Trim());
                    //FIDDBElem.SetAttribute("�û�", txtFIDUser.Text.Trim());
                    //FIDDBElem.SetAttribute("����", txtFIDPassword.Text.Trim());
                    //��ӳ�������Ϣд��XML��
                    //XmlElement ruleElem = ProjectNode.SelectSingleNode(".//����//���ݲ�������//����[@����='ͼ�����ݸ������']") as XmlElement;
                    //ruleElem.SetAttribute("·��", ModData.DBTufuSubmitXml);
                    //ProjectNode.OwnerDocument.Save(ModData.v_projectXML);
                    //end
                    #endregion
                  
                    this.Close();
                }
                else
                {
                    this.Show();
                }

                m_DataThread = null;
                _timer.Enabled = false;
            }
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
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// �������ַ����õ���ΧPolygon
        /// </summary>
        /// <param name="strCoor">�����ַ���,��ʽΪX@Y,�Զ��ŷָ�</param>
        /// <returns></returns>
        public static IPolygon GetPolygonByCol(string strCoor)
        {
            try
            {
                object after = Type.Missing;
                object before = Type.Missing;
                IPolygon polygon = new PolygonClass();
                IPointCollection pPointCol = (IPointCollection)polygon;
                string[] strTemp = strCoor.Split(',');
                for (int index = 0; index < strTemp.Length; index++)
                {
                    string CoorLine = strTemp[index];
                    string[] coors = CoorLine.Split('@');

                    double X = Convert.ToDouble(coors[0]);
                    double Y = Convert.ToDouble(coors[1]);

                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(X, Y);
                    pPointCol.AddPoint(pPoint, ref before, ref after);
                }

                polygon = (IPolygon)pPointCol;
                polygon.Close();

                ITopologicalOperator pTopo = (ITopologicalOperator)polygon;
                pTopo.Simplify();

                return polygon;
            }
            catch(Exception e)
            {
                //*******************************************************************
                //guozheng added
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

                return null;
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            //OpenFileDialog OpenFile = new OpenFileDialog();
            //OpenFile.CheckFileExists = true;
            //OpenFile.CheckPathExists = true;
            //OpenFile.Title = "ѡ��ͼ�η�Χ����txt";
            //OpenFile.Filter = "ͼ�η�Χ�����ı�(*.txt)|*.txt";
            //if (OpenFile.ShowDialog() == DialogResult.OK)
            //{
            //    this.textBoxX2.Text = OpenFile.FileName;
            //    StringBuilder sb = new StringBuilder();
            //    try
            //    {
            //        StreamReader sr = new StreamReader(OpenFile.FileName);
            //        while (sr.Peek() >= 0)
            //        {
            //            string[] strTemp = sr.ReadLine().Split(',');
            //            if (sb.Length != 0)
            //            {
            //                sb.Append(",");
            //            }
            //            sb.Append(strTemp[0] + "@" + strTemp[1]);
            //        }
            //    }
            //    catch
            //    {
            //        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "ͼ�η�Χ����txt��ʽ����ȷ!\n�ı�ÿ��Ϊ����������','�ָ�");
            //        return;
            //    }

            //    if (sb.Length == 0) return;
            //    m_Geometry = ModDBOperator.GetPolygonByCol(sb.ToString()) as IGeometry;
            //}
        }

        private void cmbHistoType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //cyf 20110628 
            if (cmbHistoType.Text == "ESRI�������ݿ�(*.mdb)")
            {
                cmbHistoType.Tag = "PDB";
            }
            else if (cmbHistoType.Text == "ESRI�ļ����ݿ�(*.gdb)")
            {
                cmbHistoType.Tag = "GDB";
            }
            else if (cmbHistoType.Text == "ArcSDE(For Oracle)")
            {
                cmbHistoType.Tag = "SDE";
            }
            //end
            if (cmbHistoType.Text != "ArcSDE(For Oracle)")
            {
               btnHistoDB.Visible = true;
               txtHistoDB.Size = new Size(txtHistoServer.Size.Width -btnHistoDB.Size.Width,txtHistoDB.Size.Height);
               txtHistoServer.Enabled = false;
               txtHistoInstance.Enabled = false;
               txtHistoUser.Enabled = false;
               txtHistoPassword.Enabled = false;
               txtHistoVersion.Enabled = false;
            }
            else
            {
                btnHistoDB.Visible = false;
                txtHistoDB.Size = new Size(txtHistoServer.Size.Width, txtHistoDB.Size.Height);
                txtHistoServer.Enabled = true;
                txtHistoInstance.Enabled = true;
                txtHistoUser.Enabled = true;
                txtHistoPassword.Enabled = true;
                txtHistoVersion.Enabled = true;

            }
        }

        private void btnHistoDB_Click(object sender, EventArgs e)
        {
            switch (cmbHistoType.Text)
            {
                case "ArcSDE(For Oracle)":

                    break;

                case "ESRI�ļ����ݿ�(*.gdb)":
                    FolderBrowserDialog pFolderBrowser = new FolderBrowserDialog();
                    if (pFolderBrowser.ShowDialog() == DialogResult.OK)
                    {
                        if (!pFolderBrowser.SelectedPath.EndsWith(".gdb"))
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��GDB");
                            return;
                        }
                       txtHistoDB.Text = pFolderBrowser.SelectedPath;
                    }
                    break;

                case "ESRI�������ݿ�(*.mdb)":
                    OpenFileDialog OpenFile = new OpenFileDialog();
                    OpenFile.Title = "ѡ��ESRI�������ݿ�";
                    OpenFile.Filter = "ESRI�������ݿ�(*.mdb)|*.mdb";
                    if (OpenFile.ShowDialog() == DialogResult.OK)
                    {
                       txtHistoDB.Text = OpenFile.FileName;
                    }
                    break;

                default:
                    break;
            }
        }

        private void comboBoxOrgType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //cyf 20110628 
            if (comboBoxOrgType.Text == "ESRI�������ݿ�(*.mdb)")
            {
                comboBoxOrgType.Tag = "PDB";
            }
            else if (comboBoxOrgType.Text == "ESRI�ļ����ݿ�(*.gdb)")
            {
                comboBoxOrgType.Tag = "GDB";
            }
            else if (comboBoxOrgType.Text == "ArcSDE(For Oracle)")
            {
                comboBoxOrgType.Tag = "SDE";
            }
            //end
        }
    }
}