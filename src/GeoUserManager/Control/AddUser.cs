using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SysCommon.Gis;
using SysCommon.Error;
using SysCommon.Authorize;

namespace GeoUserManager
{
    public partial class AddUser : DevComponents.DotNetBar.Office2007Form
    {
        User _user = null;                      //�û�����
        bool isUpdate = false;                  //�жϵ�ǰ�Ƿ�Ϊ����

        public User user
        {
            get { return _user; }
            set { _user = value; }
        }

        public AddUser()
        {
            InitializeComponent();
        }

        public AddUser(User user)
        {
            InitializeComponent();
            _user = user;
            isUpdate = true;
        }
        private List<string> m_listExtentCode = new List<string>();
        private List<string> m_listExtentName = new List<string>();

        private void AddUser_Load(object sender, EventArgs e)
        {
            InitilComboBox();
            InitilCbDepartment();
            if (isUpdate)
            {
                if (_user.Name.ToLower() == "admin")
                {
                    this.txtUser.Enabled = false;
                }
                this.btnAddUser.Text = "����";
                this.Text = "�޸��û���Ϣ";
                this.txtUser.Text = _user.Name;
                this.txtTrueName.Text = _user.TrueName;
                this.txtPassword.Text = "";// _user.Password;
                this.comboSex.SelectedIndex = _user.SexInt;
                this.txtPosition.Text = _user.Position;
                this.txtComment.Text = _user.Remark;
                this.dateTimePicker.Text = user.UserDate; //wgf 20111102

                //�������� ygc20130319
                SysGisTable ksTable = new SysGisTable(Plugin.ModuleCommon.TmpWorkSpace);
                Exception ex = null;
                Dictionary<string, object> newdic = new Dictionary<string, object>();
                newdic = ksTable.GetRow("USER_DEPARTMENT", "DEPARTMENTID='" + _user.UserDepartment  + "'", out ex);
                if (newdic.Count > 0)
                {
                    this.cbDepartment .Text = newdic["DEPARTMENTNAME"].ToString();
                }
                List<Dictionary<string, object>> listDic = new List<Dictionary<string, object>>();
                listDic = ksTable.GetRows("USER_EXPORT", "USERID='" + _user.IDStr + "'", out ex);
                string tempStr = "";
                for (int i = 0; i < listDic.Count; i++)
                {
                    newdic =new Dictionary<string,object> ();
                    newdic =listDic[i];
                    m_listExtentCode.Add(newdic["EXPORTEXTENT"].ToString ());
                    tempStr += newdic["XIANNAME"].ToString();
                }
                txtExportArea.Text = tempStr;
                    //end
                    if (_user.ExportArea >= 0)
                    {
                        this.txtExportArea.Text = _user.ExportArea.ToString();
                    }
            }
            else
            {
                this.btnAddUser.Text = "���";
            }
            this.txtUser.Focus();
        }

        private void InitilComboBox()
        {
            ComboBoxItem item = new ComboBoxItem("��", 0);
            this.comboSex.Items.Add(item);
            item = new ComboBoxItem("Ů", 1);
            this.comboSex.Items.Add(item);
        }
        //ygc 20130319 ��ʼ������������
        private void InitilCbDepartment()
        {
            List<Dictionary<string, object>> listDepartment = new List<Dictionary<string, object>>();
            Exception ex = null;
            SysGisTable sysTable = new SysGisTable(Plugin.ModuleCommon.TmpWorkSpace);
            listDepartment = sysTable.GetRows("USER_DEPARTMENT", "", out ex);
            if (listDepartment !=null|| listDepartment.Count != 0)
            {
                
                for (int i = 0; i < listDepartment.Count; i++)
                {
                    Dictionary<string, object> newdic = new Dictionary<string, object>();
                    newdic = listDepartment[i];
                    cbDepartment.Items.Add(newdic["DEPARTMENTNAME"]).ToString ();
                    
                }
        
            }
            if (cbDepartment.Items.Count != 0)
            {
                cbDepartment.SelectedIndex = 0;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            Exception exError = null;
            errorProvider.Clear();
            try
            {
                //��֤
                if (string.IsNullOrEmpty(this.txtUser.Text))
                {
                    errorProvider.SetError(txtUser, "�û�����Ʋ���Ϊ�գ�");
                    return;
                }
                else if (string.IsNullOrEmpty(this.txtTrueName.Text))
                {
                    errorProvider.SetError(txtTrueName, "�û���ʵ������Ϊ�գ�");
                    return;
                }
                else if (string.IsNullOrEmpty(this.txtPassword.Text))
                {
                    if (!isUpdate)
                    {
                        errorProvider.SetError(txtPassword, "���벻��Ϊ�գ�");
                        return;
                    }
                }
                else if (string.IsNullOrEmpty(this.comboSex.Text))
                {
                    errorProvider.SetError(comboSex, "�Ա���Ϊ�գ�");
                    return;
                }
                else if (string.IsNullOrEmpty(this.txtPosition.Text))
                {
                   // errorProvider.SetError(txtPosition, "ְ�Ʋ���Ϊ�գ�");
                   // return;
                }
 //* / //20111102 wgf
                ModData.gisDb.StartTransaction(out exError);
                SysGisTable sysTable = new SysGisTable(ModData.gisDb);
                Dictionary<string, object> dicData = new Dictionary<string, object>();
                dicData.Add("userid", this.txtUser.Text.Trim() + DateTime.Now.ToString("yyyyMMddHHmmss"));
                dicData.Add("name", this.txtUser.Text.Trim());
                //����Ǹ����û� �����û������Ϊ�� �򲻸�������
                if (!(isUpdate && this.txtPassword.Text.Trim()==""))
                {
                    dicData.Add("upwd", SysCommon.Authorize.AuthorizeClass.ComputerSecurity(this.txtPassword.Text.Trim()));
                }
                    
                dicData.Add("usex", (this.comboSex.SelectedItem as ComboBoxItem).Value);
                dicData.Add("uposition", this.txtPosition.Text.Trim());
                dicData.Add("uremark", this.txtComment.Text.Trim());
                dicData.Add("TRUTHNAME", this.txtTrueName.Text);

                //�������� ygc20130319
                SysGisTable ksTable = new SysGisTable(Plugin.ModuleCommon.TmpWorkSpace);
                Exception ex = null;
                Dictionary<string, object> newdic = new Dictionary<string, object>();
                newdic = ksTable.GetRow("USER_DEPARTMENT", "DEPARTMENTNAME='" + this.cbDepartment.Text + "'", out ex);
                if (newdic.Count > 0)
                {
                    dicData.Add("USERDEPARTMENT", newdic["DEPARTMENTID"].ToString());//ygc 20130319 ���ӿ���
                }
                //end
                //wgf ��������  20111102
                if (checkBoxDate.Checked == true)
                {
                   // dateTimePicker.get
                    string strDate = dateTimePicker.Value.Year.ToString() + dateTimePicker.Value.Month.ToString() + dateTimePicker.Value.Day.ToString();
                    dicData.Add("ENDDATE", dateTimePicker.Text);
                }
                //end
                //ygc 20130420 �޸���ȡ���
               // if (txtExportArea.Text == "")
               // {
                    dicData.Add("EXPORTAREA", "-1");
              //  }
              //  else
              //  {
               //     dicData.Add("EXPORTAREA", txtExportArea.Text);
              //  }
                //�ж��Ǹ��»������
                if (isUpdate)
                {
                    //dicData.Remove("name");changed by xisheng 2011.06.30 �����޸�����
                    dicData.Remove("userid");
                    if (!sysTable.UpdateRow("user_info", "userid='" + _user.IDStr+"'", dicData, out exError))
                    {
                        ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ʧ�ܣ�" + exError.Message);
                        return;
                    }
                }
                else
                {
                    if (!sysTable.ExistData("user_info", "NAME='" + txtUser.Text.Trim() + "'"))
                    {
                        if (!sysTable.NewRow("user_info", dicData, out exError))
                        {
                            ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ʧ�ܣ�" + exError.Message);
                            return;
                        }
                    }
                    else
                    {
                        ErrorHandle.ShowFrmErrorHandle("��ʾ", "�Ѿ����ڴ��û���ƣ����ʧ�ܣ�");
                        return;
                    }
                }
                //ygc 20130420 �����ȡ��Χ����
                if (m_listExtentCode != null)
                {
                    //��ɾ��ԭ�е�Ȩ��
                    try
                    {
                        if (isUpdate)
                        {
                            sysTable.DeleteRows("USER_EXPORT", "USERID='" + user.IDStr + "'", out exError);
                        }
                    }
                    catch { }
                    for (int i = 0; i < m_listExtentCode.Count; i++)
                    {
                        newdic = new Dictionary<string, object>();
                        if (isUpdate)
                        {
                            newdic.Add("USERID", user.IDStr);
                        }
                        else
                        {
                            newdic.Add("USERID", dicData["userid"].ToString());
                        }
                        newdic.Add("EXPORTEXTENT", m_listExtentCode[i]);
                        newdic.Add("XIANNAME",m_listExtentName [i]);
                        sysTable.NewRow("USER_EXPORT", newdic, out exError);
                    }
                }
                ModData.gisDb.EndTransaction(true, out exError);
                this.DialogResult=DialogResult.OK;
            }
            catch (Exception ex)
            {
                exError = ex;
                ModData.gisDb.EndTransaction(false, out exError);
                if (exError != null)
                {
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", exError.Message);
                }
                else
                {
                    MessageBox.Show(ex.ToString (),"��ʾ",MessageBoxButtons.OK ,MessageBoxIcon.Error);
                }
            }
        }

        private void txtExportArea_KeyPress(object sender, KeyPressEventArgs e)
        {
            string strnum = "0123456789.";

            if (!char.IsControl(e.KeyChar) && (!strnum.Contains(e.KeyChar.ToString())))
            {
                e.Handled = true;
            }
        }

        //״̬�ı��ʱ��
        private void checkBoxDate_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDate.Checked == false)
            {
                dateTimePicker.Enabled = false;
            }
            else if (checkBoxDate.Checked == true)
            {
                dateTimePicker.Enabled = true;
            }
        }

        private void btnGetExtent_Click(object sender, EventArgs e)
        {
            SelectExportExtent newfrm = new SelectExportExtent(m_listExtentCode);
            //newfrm.m_ListXian = m_listExtentCode;
            if (newfrm.ShowDialog() != DialogResult.OK) return;
            m_listExtentCode = newfrm.m_ListXian;
            m_listExtentName = newfrm.m_ListName;
            if (newfrm.m_ListXian != null)
            {
                string tempStr = "";
                for (int i = 0; i < newfrm.m_ListName.Count; i++)
                {
                    tempStr += newfrm.m_ListName[i] + ";";
                }
                txtExportArea.Text = tempStr;
            }
        }
    }
}

 