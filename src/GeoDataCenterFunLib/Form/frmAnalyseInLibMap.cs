using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;

namespace GeoDataCenterFunLib
{
    public partial class frmAnalyseInLibMap : DevComponents.DotNetBar.Office2007Form
    {
        public frmAnalyseInLibMap()
        {
            InitializeComponent();
        }

        string m_con;//�����ַ�������
        //string m_path;//·��
        List<string> m_list = new List<string>();//�����б�
        public static TreeNode Node;//���ݵ�Ԫ�����صĽڵ�
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //��������Ϊ��
        private void checkBoxArea_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxArea.Checked)
            {
                groupBoxArea.Enabled = true;
                //string str_Exp = "select �������� from ���ݵ�Ԫ��";
                //GeoDataCenterDbFun dbfun = new GeoDataCenterDbFun();
                //List<string> list = dbfun.GetDataReaderFromMdb(m_con, str_Exp);
                //for (int i = 0; i < list.Count; i++)
                //{
                //    comboBoxArea.Items.Add(list[i]);
                //}
                //if (comboBoxArea.Items.Count > 0)
                //    comboBoxArea.SelectedIndex = 0;
            }
            else
            { 
                groupBoxArea.Enabled = false;
                comboBoxArea.Items.Clear();
                comboBoxArea.Text = "";
            }
        }

        //�����Ϊ��
        private void checkBoxYear_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBoxYear.Checked)
            {
                groupBoxYear.Enabled = true;
                string str_Exp = "select ��� from ���ݱ����";
                GeoDataCenterDbFun dbfun = new GeoDataCenterDbFun();
                List<string> list = dbfun.GetDataReaderFromMdb(m_con, str_Exp);
                for (int i = 0; i < list.Count; i++)
                {
                    if (!comboBoxYear.Items.Contains(list[i]))
                    comboBoxYear.Items.Add(list[i]);
                    
                }
                if (comboBoxYear.Items.Count > 0)
                    comboBoxYear.SelectedIndex = 0;
            }
            else
            {
                groupBoxYear.Enabled = false;
                comboBoxYear.Items.Clear();
                comboBoxYear.Text = "";
            }
        }

        //�Ա�����Ϊ��
        private void checkBoxScale_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBoxScale.Checked)
            {
                groupBoxScale.Enabled = true;
                string str_Exp = "select ����,���� from �����ߴ����";
                GeoDataCenterDbFun dbfun = new GeoDataCenterDbFun();
               DataTable dt=dbfun.GetDataTableFromMdb(m_con,str_Exp);
               for (int i = 0; i < dt.Rows.Count;i++)
               {
                   comboBoxScale.Items.Add(dt.Rows[i][0]+"("+dt.Rows[i][1]+")");
               }
                if (comboBoxScale.Items.Count > 0)
                    comboBoxScale.SelectedIndex = 0;
            }

            else
            { 
                groupBoxScale.Enabled = false;
                
                comboBoxScale.Items.Clear();
                comboBoxScale.Text = "";
            }
        }

        //���ڼ���
        private void frmAnalyseInLibMap_Load(object sender, EventArgs e)
        {
            checkBoxDelold.Checked = true;
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            m_con = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strExp = "select ����Դ���� from ��������Դ��";
            List<string> list = db.GetDataReaderFromMdb(m_con, strExp);
            for (int i = 0; i < list.Count; i++)
            {
                comboBoxSource.Items.Add(list[i]);//��������Դ�б��
            }
            if (list.Count > 0)
            {
                comboBoxSource.SelectedIndex = 0;//Ĭ��ѡ���һ��
            }
            //m_path = GetSourcePath(comboBoxSource.Text.Trim());

        }

        //�õ�����Դ��ַ
        private string GetSourcePath(string str)
        {
            try
            {
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                string strExp = "select ���ݿ� from ��������Դ�� where ����Դ����='" + str + "'";
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                string strname = db.GetInfoFromMdbByExp(strCon, strExp);
                return strname;
            }
            catch { return ""; }
        }
        /// <summary>
        /// �õ����ݿ�ռ� Added by xisheng 2011.04.28
        /// </summary>
        /// <param name="str">����Դ����</param>
        /// <returns>�����ռ�</returns>
        private IWorkspace GetWorkspace(string str)
        {
            try
            {
                IWorkspace pws = null;
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                string strExp = "select * from ��������Դ�� where ����Դ����='" + str + "'";
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
                string type = dt.Rows[0]["����Դ����"].ToString();
                if (type.Trim() == "GDB")
                {
                    IWorkspaceFactory pWorkspaceFactory;
                    pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                    pws = pWorkspaceFactory.OpenFromFile(dt.Rows[0]["���ݿ�"].ToString(), 0);
                }
                else if (type.Trim() == "SDE")
                {
                    IWorkspaceFactory pWorkspaceFactory;
                    pWorkspaceFactory = new SdeWorkspaceFactoryClass();

                    //PropertySet
                    IPropertySet pPropertySet;
                    pPropertySet = new PropertySetClass();
                    pPropertySet.SetProperty("Server", dt.Rows[0]["������"].ToString());
                    pPropertySet.SetProperty("Database", dt.Rows[0]["���ݿ�"].ToString());
                    pPropertySet.SetProperty("Instance", "5151");//"port:" + txtService.Text
                    pPropertySet.SetProperty("user", dt.Rows[0]["�û�"].ToString());
                    pPropertySet.SetProperty("password", dt.Rows[0]["����"].ToString());
                    pPropertySet.SetProperty("version", "sde.DEFAULT");
                    pws = pWorkspaceFactory.Open(pPropertySet, 0);

                }
                return pws;
            }
            catch
            {
                return null;
            }
        }
 
        //��ʼ����
        private void btn_Analys_Click(object sender, EventArgs e)
        {
            SysCommon.CProgress vProgress = new SysCommon.CProgress("��ʼ��������");
            vProgress.EnableCancel = false;
            vProgress.ShowDescription = true;
            vProgress.FakeProgress = true;
            vProgress.TopMost = true;
            vProgress.ShowProgress();
            try
            {
                string strArea = "";
                string strArea2 = "";
                List<string> strAreaChild = new List<string>();
                string strYear = "";
                string strScale = "";
                string strScale2 = "";
                string strExp = "";
                bool flat = false;//ѡ�˱���������״̬
                int index = 0;
                int ifinish=0;//��ʾ�����˶���������
                GeoDataCenterDbFun dbfun = new GeoDataCenterDbFun();
                m_list.Clear();

                //ѡ����8��״̬
                #region
                if (comboBoxScale.Text != "" && comboBoxYear.Text != "" && comboBoxArea.Text != "")
                    index = 1;
                else if (comboBoxArea.Text != "" && comboBoxYear.Text != "")
                    index = 2;
                else if (comboBoxArea.Text != "" && comboBoxScale.Text != "")
                    index = 3;
                else if (comboBoxYear.Text != "" && comboBoxScale.Text != "")
                    index = 4;
                else if (comboBoxArea.Text != "")
                    index = 5;
                else if (comboBoxYear.Text != "")
                    index = 6;
                else if (comboBoxScale.Text != "")
                    index = 7;
                else
                    index = 0;
                #endregion

                if (Node != null && comboBoxArea.Text!="")
                {
                    string[] arrr = comboBoxArea.Text.Split('(', ')');
                    strArea2 = arrr[1];
                    //�õ����и���������Ͻ��
                    switch (Convert.ToInt32(Node.Tag))
                    {
                        case 1:
                            strArea = "�������� like '" + strArea2.Substring(0, 3).Trim() + "*' and ";
                            strExp = "select �������� from ���ݵ�Ԫ�� where �������� like '" + strArea2.Substring(0, 3).Trim() + "*'";
                            break;
                        case 2:
                            strArea = "�������� like '" + strArea2.Substring(0, 4).Trim() + "*' and";
                            strExp = "select �������� from ���ݵ�Ԫ�� where �������� like '" + strArea2.Substring(0, 4).Trim() + "*'";
                            break;
                        case 3:
                            strArea = "��������='" + strArea2 + "' and ";
                            strExp = "select �������� from ���ݵ�Ԫ�� where �������� = '" + strArea2 + "*'";
                            break;
                    }
                    strAreaChild = dbfun.GetDataReaderFromMdb(m_con, strExp);

                }
                if (comboBoxYear.Text != "")
                {
                    strYear = "���='" + comboBoxYear.Text + "' and ";
                }
                if (comboBoxScale.Text != "")
                {

                    string []arrr = comboBoxScale.Text.Split('(', ')');
                    strScale2 = arrr[1];
                    strScale = "������='" + strScale2 + "' and ";
                    //flat = true;
                }
                //else
                //    flat = false;
                //if (flat)//ѡ�˱����ߣ�����ȥ��and
                //    strExp = "select ID from ���ݱ���� where " + strArea + strYear + strScale + " and ����Դ����='" + GetSourceName(m_path) + "'";
                //else if (comboBoxYear.Text != "" ||comboBoxArea.Text != "")//������û��ѡ�񣬶���Ȼ�����������ѡ�ˣ�Ҫȥ��and
                //{
                    //strExp = "select ID from ���ݱ���� where " + strArea + strYear + "  ����Դ����='" + GetSourceName(m_path) + "'";
                //}
                //else//��û��ѡ��
                //    strExp = "select ID from ���ݱ���� where  ����Դ����='" + GetSourceName(m_path) + "'";

                strExp = "select ID from ���ݱ���� where " + strArea + strYear + strScale + " ����Դ����='" +comboBoxSource.Text.Trim() + "'";  
                List<string> list = new List<string>();
                list = dbfun.GetDataReaderFromMdb(m_con, strExp);
              
               // m_path = GetSourcePath(comboBoxSource.Text);
                for (int i = 0; i < list.Count; i++)
                {
                    strExp = "delete * from ���ݱ���� where ID=" + Convert.ToInt32(list[i]);
                    dbfun.ExcuteSqlFromMdb(m_con, strExp);//�����ݱ����ɾ������ID��������
                }
                m_list = new List<string>();
                IWorkspace pWorkspace = GetWorkspace(comboBoxSource.Text);
                //�������ݿ������ݲ�����m_list�б���
                if (pWorkspace!=null)
                {
                   
                    IEnumDataset enumDataset = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass) as IEnumDataset;
                    IDataset dataset = enumDataset.Next();
                    //����mdb��ÿһ������Ҫ����
                    while (dataset != null)
                    {
                        IFeatureClass pFeatureClass = dataset as IFeatureClass;
                        m_list.Add(pFeatureClass.AliasName);
                        dataset = enumDataset.Next();
                    }
                }
                else
                {
                    vProgress.Close();
                    MessageBox.Show("����Դ�ռ䲻����", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Activate();
                    return;

                }
                //��m_list�ж�ȡ���ݲ�����
                string[] array = new string[6];
                bool boolarea = false;

                for (int i = 0; i < m_list.Count; i++)
                {
                    flat = false;
                    if (m_list[i].Contains("."))
                        m_list[i] = m_list[i].Substring(m_list[i].LastIndexOf(".")+1);
                    strExp = "select �ֶ����� from ͼ�����������";
                    string strname =dbfun.GetInfoFromMdbByExp(m_con, strExp);
                    string[] arrName = strname.Split('+');//�����ֶ�����
                    for (int ii = 0; ii < arrName.Length; ii++)
                    {
                        switch (arrName[ii])
                        {
                            case "ҵ��������":
                                array[0] = m_list[i].Substring(0, 2);//ҵ��������
                                m_list[i] = m_list[i].Remove(0, 2);
                                break;
                            case "���":
                                array[1] = m_list[i].Substring(0, 4);//���
                                m_list[i] = m_list[i].Remove(0, 4);
                                break;
                            case "ҵ��С�����":
                                array[2] = m_list[i].Substring(0, 2);//ҵ��С�����
                                m_list[i] = m_list[i].Remove(0, 2);
                                break;
                            case "��������":
                                array[3] = m_list[i].Substring(0, 6);//��������
                                m_list[i] = m_list[i].Remove(0, 6);
                                break;
                            case "������":
                                array[4] = m_list[i].Substring(0, 1);//������
                                m_list[i] = m_list[i].Remove(0, 1);
                                break;
                        }
                    }
                    array[5] = m_list[i];//ͼ�����
                    for (int j = 0; j < strAreaChild.Count; j++)//�ж��Ƿ��������������Ͻ��
                    {
                        if (strAreaChild[j] == array[3])
                        { 
                            boolarea = true; 
                            break; 
                        }
                    }
                    //�ж���������Ƿ����
                    #region
                    switch (index)
                    {
                        case 0:
                            flat = true;
                            break;
                        case 1:
                            if (array[1] == comboBoxYear.Text && array[4] == strScale2 && boolarea)
                                flat = true;
                            break;
                        case 2:
                            if (boolarea && array[1] == comboBoxYear.Text)
                                flat = true;
                            break;
                        case 3:
                            if (boolarea && array[4] == strScale2)
                                flat = true;
                            break;
                        case 4:
                            if (array[1] == comboBoxYear.Text && array[4] == strScale2)
                                flat = true;
                            break;
                        case 5:
                            if (boolarea)
                                flat = true;
                            break;
                        case 6:
                            if (array[1] == comboBoxYear.Text)
                                flat = true;
                            break;
                        case 7:
                            if (array[4] == strScale2)
                                flat = true;
                            break;
                    }
                    #endregion

                    string sourecename = comboBoxSource.Text.Trim();
                    if (flat)
                    {
                        ifinish++;
                        
                        strExp = string.Format("insert into ���ݱ����(ҵ��������,���,ҵ��С�����,��������,������,ͼ�����,����Դ����) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                        array[0], array[1], array[2], array[3], array[4], array[5],sourecename);
                       
                        string strdata = GetLayerName(array[0], array[1], array[2], array[3], array[4]) + array[5];//��֯����
                        string logpath = Application.StartupPath + "\\..\\Log\\DataManagerLog.txt";
                        LogFile log = new LogFile(null,logpath);
                        string strLog = "��ʼ��������Դ" +sourecename+"��"+ strdata+"����";
                        if (log != null)
                        {
                            log.Writelog(strLog);
                        }
                        vProgress.SetProgress(strLog);
                        dbfun.ExcuteSqlFromMdb(m_con, strExp); //�������ݱ����
                        dbfun.UpdateMdbInfoTable(array[0], array[1], array[2], array[3], array[4]);//���µ�ͼ�����Ϣ��
                    }
                  
                }
                ifinish = list.Count >= ifinish ? list.Count : ifinish;
                vProgress.Close();
                this.Activate();
                if (ifinish == 0)
                    MessageBox.Show("û�з�������������!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("�������!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.Close();
            }
            //���±��
            //strExp = "alter table ���ݱ���� alter column ID counter(1,1)";
            // dbfun.ExcuteSqlFromMdb(m_con,strExp);
            catch (System.Exception ex)
            {
                vProgress.Close();
                MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


        }

        /// <summary>
        /// ��֯ǰ׺��added by xs 20110415
        /// </summary>
        /// <param name="str1">ҵ�����</param>
        /// <param name="str2">���</param>
        /// <param name="str3">ҵ��С��</param>
        /// <param name="str4">��������</param>
        /// <param name="str5">������</param>
        /// <returns></returns>
        public string GetLayerName(string str1, string str2, string str3, string str4, string str5)
        {
            string layername = "";
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select �ֶ����� from ͼ�����������";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strname = db.GetInfoFromMdbByExp(strCon, strExp);
            string[] arrName = strname.Split('+');//�����ֶ�����
            for (int i = 0; i < arrName.Length; i++)
            {
                switch (arrName[i])
                {
                    case "ҵ��������":
                        layername += str1;
                        break;
                    case "���":
                        layername += str2;
                        break;
                    case "ҵ��С�����":
                        layername += str3;
                        break;
                    case "��������":
                        layername += str4;
                        break;
                    case "������":
                        layername += str5;
                        break;
                }
            }
            return layername;
        }

        //�õ�����Դ����
        private string GetSourceName(string str)
        {
            try
            {
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                string strExp = "select ����Դ���� from ��������Դ�� where ���ݿ�='" + str + "'";
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                string strname = db.GetInfoFromMdbByExp(strCon, strExp);
                return strname;
            }
            catch
            {
                return str;
            }
        }
        //�����������
        private void comboBoxArea_Click(object sender, EventArgs e)
        {
            GeoDataCenterDbFun dbfun = new GeoDataCenterDbFun();
            
            frmDataUnitTree frm = new frmDataUnitTree();//��ʼ�����ݵ�Ԫ������
            frm.Location = new Point(this.Location.X + 147, this.Location.Y + 80);
            frm.flag=0;
            frm.ShowDialog();
            if (Node != null)//���ص�Node����NULL
            {
                if (Convert.ToInt32(Node.Tag) != 0)
                {
                    
                    string strExp = "select �������� from ���ݵ�Ԫ�� where ��������='" + Node.Text + "' and ���ݵ�Ԫ����='"+Node.Tag+"'";
                    string code = dbfun.GetInfoFromMdbByExp(m_con, strExp);
                    comboBoxArea.Text = Node.Text + "(" + code + ")";//Ϊ���ݵ�Ԫbox��ʾ����
                }
            }
            
           
        }
    }
}