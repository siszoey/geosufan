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
using System.Xml;
using System.IO;

namespace GeoDBIntegration
{
    /// <summary>
    /// chenyafei  20100311 add  content :����û�����
    /// </summary>
    public partial class AddUser : DevComponents.DotNetBar.Office2007Form
    {
        User _user = null;                      //�û���
        bool m_BeSuccedd = true;               //��־���ؽ����Ƿ�ɹ�
        public bool BeSuccedd
        {
            get
            {
                return m_BeSuccedd;
            }
            set
            {
                m_BeSuccedd = value;
            }
        }

        string m_UserName = "";                //����ǰ���û������������бȽ��Ƿ����仯
        string m_Password = "";                //����ǰ�����룬�����Ƚ��Ƿ����仯
        string m_OldRoleName = "";             //����ǰ�Ľ�ɫ���� ,�������бȽ�
        string m_Sex = "";                     //����ǰ���Ա�
        string m_Position = "";                //����ǰ��ְ��
        string m_Remark = "";                  //����ǰ�ı�ע

        //bool m_isUpdate = false;                  //�жϵ�ǰ�Ƿ�Ϊ����

        //public User user
        //{
        //    get { return _user; }
        //    set { _user = value; }
        //}

        public AddUser(User user)
        {
            InitializeComponent();
            _user = user;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="beUpdate"></param>
        //public AddUser(bool beUpdate)
        //{
        //    InitializeComponent();
        //    //m_isUpdate = beUpdate;

        //}

        /// <summary>
        /// chenyafei 20110311 add:��ʼ������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddUser_Load(object sender, EventArgs e)
        {
            //��ʼ������---------�����б��
            InitilComboBox();
           
            if (_user!=null)
            {
                //�޸��û�����
                this.btnAddUser.Text = "�����û�";
                this.Text = "�޸��û���Ϣ";
                this.txtTrueName.Text = _user.Name;
                this.txtPassword.Text = _user.Password;
                this.cmbRole.Text = _user.Role;
                this.comboSex.Text = _user.Sex;
                this.txtPosition.Text = _user.Position;
                this.txtComment.Text = _user.Remark;

                //����ɵ��û���Ϣ�������Ƚϻ�����Ϣ�ͺͽ�ɫ�Ƿ����仯
                m_UserName = _user.Name;
                m_Password = _user.Password;
                m_OldRoleName = _user.Role; //����ɵĽ�ɫ��
                m_Sex = _user.Sex;
                m_Position = _user.Position;
                m_Remark = _user.Remark;
                //cyf 20110602 modify
                ///////////////////////////////Ȩ���жϣ���ͨ�û����ܸı��Լ��Ľ�ɫ��ϵͳ����Ա���ܸı��ɫ/////////
                //if (_user.RoleTypeID == EnumRoleType.ϵͳ����Ա.GetHashCode() || _user.RoleTypeID == EnumRoleType.��ͨ�û�.GetHashCode())
                //{
                //    this.cmbRole.Enabled = false;
                //}
                //end
            }
            else
            {
                //����û�����
                this.btnAddUser.Text = "����û�";
                this.Text="����û�";
            }
            this.txtTrueName.Focus();
            
        }

        /// <summary>
        /// chenyafei 20110311 add content:��ʼ������
        /// </summary>
        private void InitilComboBox()
        {
            Exception outError = null;
            #region ��ʼ���Ա��б��
            ComboBoxItem item = new ComboBoxItem("��", 0);
            this.comboSex.Items.Add(item);
            item = new ComboBoxItem("Ů", 1);
            this.comboSex.Items.Add(item);
            this.comboSex.SelectedIndex = 0;
            #endregion

            #region ��ʼ����ɫ�б��
            //����ϵͳά����
            SysCommon.DataBase.SysTable pSysDB = null;
            ModDBOperate.ConnectDB(out pSysDB, out outError);
            if (outError != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", outError.Message);
                m_BeSuccedd = false;
                return;
            }
            //��ѯ��ɫ������Ϣ��
            string selStr = string.Empty;
            if (this._user == null)
                selStr = "select * from rolebaseinfo WHERE rolebaseinfo.ROLETYPEID<>3";   //��ѯ��ɫ�ַ���,3Ϊϵͳ����Ա��ɫ��ϵͳ����Ա��ɫ�������
            else
            {
                //cyf 20110602 delete
                //if (this._user.RoleTypeID==EnumRoleType.ϵͳ����Ա.GetHashCode())
                //    selStr = "select * from rolebaseinfo";
                //else
                //    selStr = "select * from rolebaseinfo WHERE rolebaseinfo.ROLETYPEID<>3";
                //end
            }
            DataTable pTable = pSysDB.GetSQLTable(selStr, out outError);
            if (outError != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ��ɫ������Ϣ��ʧ��,ԭ��" + outError.Message);
                m_BeSuccedd = false;
                pSysDB.CloseDbConnection();
                return;
            }
            pSysDB.CloseDbConnection();
            //��������¼������ɫ��Ϣ���ص�ComboBox��
            for (int i = 0; i < pTable.Rows.Count; i++)
            {
                string roleID = "";     //��ɫID
                string roleName = "";   //��ɫ����
                roleID = pTable.Rows[i][0].ToString().Trim();
                roleName = pTable.Rows[i][1].ToString().Trim();
                ComboBoxItem pItem = new ComboBoxItem(roleName, roleID);
                cmbRole.Items.Add(pItem);
            }
            if (cmbRole.Items.Count > 0)
            {
                cmbRole.SelectedIndex = 0;
            }
            #endregion
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
      
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            Exception exError = null;

            try
            {
                #region ���н������
                if (txtComment.Text.Length > 200)
                {
                    ErrorHandle.ShowInform("��ʾ", "��ע����̫����");
                    txtComment.Focus();
                    return;
                }
                if (txtTrueName.Text.IndexOf(" ") != -1)
                {
                    errorProvider.SetError(txtTrueName, "�û��������пո�");
                    return;
                }
                //��֤
                if (string.IsNullOrEmpty(this.txtTrueName.Text))
                {
                    errorProvider.SetError(txtTrueName, "�û���ʵ������Ϊ�գ�");
                    return;
                }
                else if (string.IsNullOrEmpty(this.txtPassword.Text))
                {
                    errorProvider.SetError(txtPassword, "���벻��Ϊ�գ�");
                    return;
                }
                else if (this.comboSex.Text == "")
                {
                    errorProvider.SetError(comboSex, "��ѡ���û���ɫ��");
                    return;
                }
                else if (this.comboSex.Text == "��ѡ��")
                {
                    errorProvider.SetError(comboSex, "��ѡ���Ա�");
                    return;
                }
                else if (string.IsNullOrEmpty(this.txtPosition.Text))
                {
                    errorProvider.SetError(txtPosition, "��������Ϊ�գ�");
                    return;
                }
                if (txtTrueName.Text == "Admin")
                {
                    ErrorHandle.ShowInform("��ʾ", "���û��Ѵ���,���������룡");
                    txtTrueName.Focus();
                    return;
                }
                #endregion

                //l����ϵͳά����
                SysCommon.DataBase.SysTable pSysDB = null;
                ModDBOperate.ConnectDB(out pSysDB, out exError);
                if (exError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", exError.Message);
                    return;
                }

                #region ����Ҫ�ж��û��������ظ�

                //�û��������볬������Աͬ��
                string[] arr = ModuleData.v_AppConnStr.Split(';');
                if (arr.Length != 3)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ַ�������ȷ��");
                    return;
                }

                if (txtTrueName.Text.Trim() == arr[1].Substring(arr[1].IndexOf('=') + 1))
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ͬ�����û���");
                    pSysDB.CloseDbConnection();
                    return;
                }

                //���������ݿ��е��û�ͬ��
                string str = "select * from userbaseinfo where USERNAME='" + txtTrueName.Text.Trim() + "'";
                DataTable pUserTable = pSysDB.GetSQLTable(str, out exError);
                if (exError != null || pUserTable == null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ�û�������Ϣ��ʧ�ܣ�");
                    pSysDB.CloseDbConnection();
                    return;
                }

                #endregion

                //�ж��Ǹ��»������
                if (_user != null)
                {
                    // ���޸��û��� �ж��Ƿ����ͬ���û�
                    if (pUserTable.Rows.Count > 1)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ͬ�����û���");
                        pSysDB.CloseDbConnection();
                        return;
                    }

                    #region �û��޸�,1����Ҫ�޸��û�������Ϣ��2����Ҫ�޸��û���ɫ��ϵ��,3�������û���¼��Ϣ
                    pSysDB.StartTransaction();
                    //�����û���Ϣ

                    #region �����û���Ϣ��
                    //���û��Ļ�����Ϣ�����˱����������û�������Ϣ��
                    int pUserID = _user.ID;   //�û�ID
                    int pRoleTypeID = _user.RoleTypeID;
                    string updateStr = "";
                    if (m_UserName != txtTrueName.Text.Trim() || m_Password != txtPassword.Text.Trim() || comboSex.Text.Trim() != m_Sex || txtPosition.Text.Trim() != m_Position || txtComment.Text.Trim() != m_Remark)
                    {
                        //ֻҪ������һ�������Ϣ�����˱仯���͸�injiben��Ϣ��
                        updateStr = "update userbaseinfo set UserName='" + txtTrueName.Text.Trim() + "', Password='" + txtPassword.Text.Trim() + "', Sex='" + comboSex.Text.Trim() + "', Job='" + txtPosition.Text.Trim() + "', Remark='" + txtComment.Text.Trim() + "' where UserID=" + pUserID;
                        pSysDB.UpdateTable(updateStr, out exError);
                        if (exError != null)
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����û���Ϣ��ʧ�ܣ�");
                            pSysDB.EndTransaction(false);
                            pSysDB.CloseDbConnection();
                            return;
                        }
                    }
                    #endregion
                    if (cmbRole.Text.Trim() != m_OldRoleName)
                    {
                        //˵���û���Ӧ�Ľ�ɫҲ�����˸���
                        #region �����û���ɫ��ϵ��
                        if ((cmbRole.SelectedItem as ComboBoxItem).Value == null || (cmbRole.SelectedItem as ComboBoxItem).Value.ToString().Trim() == "")
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ��ɫIDʧ�ܣ�");
                            pSysDB.EndTransaction(false);
                            pSysDB.CloseDbConnection();
                            return;
                        }
                        int pRoleID = -1; //��ɫID  Convert.ToInt32((cmbRole.SelectedItem as ComboBoxItem).Value.ToString().Trim()); 
                        string mStr = "select * from rolebaseinfo where ROLENAME='" + cmbRole.Text.Trim() + "'";
                        DataTable tTable = pSysDB.GetSQLTable(mStr, out exError);
                        if (exError != null || tTable == null)
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ��ɫ��Ϣ��ʧ�ܣ�");
                            pSysDB.EndTransaction(false);
                            pSysDB.CloseDbConnection();
                            return;
                        }
                        if (tTable.Rows.Count == 0)
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ��ɫIDʧ�ܣ�");
                            pSysDB.EndTransaction(false);
                            pSysDB.CloseDbConnection();
                            return;
                        }
                        pRoleID = Convert.ToInt32(tTable.Rows[0][0].ToString().Trim());
                        //���Ȳ�ѯ�û���ɫ��ϵ�����Ƿ���������¼
                        //******************************************************************
                        //guozheng added 2011-3-24
                        string sSQL = "SELECT ROLEID FROM rolebaseinfo WHERE ROLETYPEID=" + _user.RoleTypeID;
                        DataTable GetTable = pSysDB.GetSQLTable(sSQL, out exError);
                        if (exError != null || GetTable == null)
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ�û���ɫ��ϵ��ʧ�ܣ�");
                            pSysDB.EndTransaction(false);
                            pSysDB.CloseDbConnection();
                            return;
                        }
                        if (GetTable.Rows.Count < 0)
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "û���ҵ����û���Ӧ�Ľ�ɫ��Ϣ"); return;
                        }
                        //******************************************************************
                        string tempStr = "select * from userrolerelationinfo where ROLEID=" + GetTable.Rows[0][0].ToString() + " and USERID=" + pUserID;
                        DataTable temTable = pSysDB.GetSQLTable(tempStr, out exError);
                        if (exError != null || temTable == null)
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ�û���ɫ��ϵ��ʧ�ܣ�");
                            pSysDB.EndTransaction(false);
                            pSysDB.CloseDbConnection();
                            return;
                        }
                        if (temTable.Rows.Count == 0)
                        {
                            //�������ڣ�������ϵ��¼
                            updateStr = "insert into userrolerelationinfo(USERID,ROLEID) values (" + pUserID + "," + pRoleID + ")";
                        }
                        else
                        {
                            //�����ڣ�����¹�ϵ��¼
                            updateStr = "update userrolerelationinfo set ROLEID=" + pRoleID + " where USERID=" + pUserID;
                        }

                        pSysDB.UpdateTable(updateStr, out exError);
                        if (exError != null)
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����û���ɫ��ϵ��ʧ�ܣ�");
                            pSysDB.EndTransaction(false);
                            pSysDB.CloseDbConnection();
                            return;
                        }
                        #endregion

                        //��ý�ɫ����
                        tempStr = "select * from rolebaseinfo where ROLEID=" + pRoleID;
                        temTable = pSysDB.GetSQLTable(tempStr, out exError);
                        if (exError != null || temTable == null)
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ��ɫ������Ϣ��ʧ�ܣ�");
                            pSysDB.EndTransaction(false);
                            pSysDB.CloseDbConnection();
                            return;
                        }
                        if (temTable.Rows.Count == 0)
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ڸý�ɫ");
                            pSysDB.EndTransaction(false);
                            pSysDB.CloseDbConnection();
                            return;
                        }
                        pRoleTypeID = Convert.ToInt32(temTable.Rows[0][2].ToString().Trim());
                    }
                   
                    pSysDB.EndTransaction(true);
                    pSysDB.CloseDbConnection();
                    #region �����û���¼��Ϣ
                    ModuleData.m_User = new User();
                    ModuleData.m_User.ID = pUserID;
                    ModuleData.m_User.Name = txtTrueName.Text.Trim();
                    ModuleData.m_User.Password = txtPassword.Text.Trim();
                    ModuleData.m_User.Sex = comboSex.Text.Trim();
                    ModuleData.m_User.Position = txtPosition.Text.Trim();
                    ModuleData.m_User.Remark = txtComment.Text.Trim();
                    ModuleData.m_User.Role = cmbRole.Text.Trim();
                    ModuleData.m_User.RoleTypeID = pRoleTypeID;
                    #endregion
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�޸��û���Ϣ�ɹ���");
                    #endregion
                }
                else
                {
                    //�������û����ж��Ƿ����ͬ���û�
                    if (pUserTable.Rows.Count > 0)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ͬ�����û���");
                        pSysDB.CloseDbConnection();
                        return;
                    }
                    //����û���Ϣ
                    #region ����û���1����Ҫ����û�������Ϣ��2����Ҫ����û���ɫ��ϵ��
                    pSysDB.StartTransaction();

                    #region ����û�ID
                    int pUserID = -1;
                    string seleStr = "select Max(UserID) from userbaseinfo";
                    DataTable tempTable = pSysDB.GetSQLTable(seleStr, out exError);
                    if (exError != null || tempTable == null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ�û���Ϣ��ʧ�ܣ�");
                        pSysDB.CloseDbConnection();
                        return;
                    }
                    if (tempTable.Rows.Count == 0)
                    {
                        pUserID = 1;
                    }
                    else
                    {
                        if (tempTable.Rows[0][0].ToString().Trim() == "")
                        {
                            pUserID = 1;
                        }
                        else
                        {
                            pUserID = Convert.ToInt32(tempTable.Rows[0][0].ToString().Trim()) + 1;
                        }
                    }
                    if (pUserID == -1)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ�û�IDʧ�ܣ�");
                        return;
                    }
                    #endregion

                    #region �����û�������Ϣ��
                    string insertStr = "insert into userbaseinfo(UserID,UserName,Password,Sex,Job,Remark) values(" + pUserID + ",'" + txtTrueName.Text.Trim() + "','" + txtPassword.Text.Trim() + "','" + comboSex.Text.Trim() + "','" + txtPosition.Text.Trim() + "','" + txtComment.Text.Trim() + "')";

                    pSysDB.UpdateTable(insertStr, out exError);
                    if (exError != null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����û�����Ϣʧ�ܣ�");
                        pSysDB.EndTransaction(false);
                        pSysDB.CloseDbConnection();
                        return;
                    }
                    #endregion
                    #region �����û���ɫ��

                    //��ý�ɫID
                    int pRoleID = -1;   //��ɫID
                    if ((cmbRole.SelectedItem as ComboBoxItem).Value == null || (cmbRole.SelectedItem as ComboBoxItem).Value.ToString().Trim() == "")
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ��ɫIDʧ�ܣ�");
                        pSysDB.EndTransaction(false);
                        pSysDB.CloseDbConnection();
                        return;
                    }
                    pRoleID = Convert.ToInt32((cmbRole.SelectedItem as ComboBoxItem).Value.ToString().Trim());

                    //�������ݵ���ɫ��ϵ������
                    string inseStr = "insert into userrolerelationinfo(UserID,RoleID) values(" + pUserID + "," + pRoleID + ")";
                    pSysDB.UpdateTable(inseStr, out exError);
                    if (exError != null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����û���ɫ��ϵ��ʧ�ܣ�");
                        pSysDB.EndTransaction(false);
                        pSysDB.CloseDbConnection();
                        return;
                    }
                    #endregion
                    pSysDB.EndTransaction(true);
                    pSysDB.CloseDbConnection();

                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ ", "����û���Ϣ�ɹ���");

                    #endregion
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                exError = ex;
            }
        }
    }
}