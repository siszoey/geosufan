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
    /// chenyafei  20110314  add content:��ӽ�ɫ 
    /// </summary>
    public partial class AddGroup : DevComponents.DotNetBar.Office2007Form
    {
        bool m_BeUpdate = false;                //���(true �޸�)
        bool m_beSucced = true;
        public bool beSucceed
        {
            get { return m_beSucced; }
            set { m_beSucced = value; }
        }
        
        public AddGroup(bool pBeUpdate)
        {
            InitializeComponent();
            m_BeUpdate = pBeUpdate;
            //��ʼ����ɫ�б��
            intialForm();
        }

        /// <summary>
        /// ��ʼ����ɫ�����б��
        /// </summary>
        private void intialForm()
        {
            SysCommon.DataBase.SysTable pSysDB=null;
            Exception outError = null;
            //�������ݿ�
            ModDBOperate.ConnectDB(out pSysDB, out outError);
            if (outError != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", outError.Message);
                m_beSucced = false;
                return;
            }
            #region ��ʼ����ɫ�����б�
            string selStr = "";//��ʼ����ɫ�����б���ַ���
            selStr = "select * from roletypeinfo WHERE ROLETYPEID<> 3";   //��ѯ��ɫ���ͱ�,3Ϊϵͳ����Ա
            DataTable pTable = pSysDB.GetSQLTable(selStr, out outError);
            if (outError != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ��ɫ������Ϣ��ʧ��,ԭ��" + outError.Message);
                pSysDB.CloseDbConnection();
                m_beSucced = false;
                return;
            }

            //��������¼������ɫ������Ϣ���ص�ComboBox��
            for (int j = 0; j < pTable.Rows.Count; j++)
            {
                string roleID = "";     //��ɫID
                string roleName = "";   //��ɫ��������
                roleID = pTable.Rows[j][0].ToString().Trim();
                roleName = pTable.Rows[j][2].ToString().Trim();
                ComboBoxItem pItem = new ComboBoxItem(roleName, roleID);
                cmbGroupType.Items.Add(pItem);
            }
            if (cmbGroupType.Items.Count > 0)
            {
                cmbGroupType.SelectedIndex = 0;
            }
            #endregion

            if (m_BeUpdate)
            {
                #region �޸ģ���ʼ����ɫ���б��
                string str = "select * from rolebaseinfo where roletypeid<>3";  //��ѯ��ɫ���ͱ� 3Ϊϵͳ����Ա�������޸�
                DataTable RoleTable = pSysDB.GetSQLTable(str, out outError);
                if (outError != null || RoleTable == null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ��ɫ������Ϣ��ʧ�ܣ�");
                    pSysDB.CloseDbConnection();
                    m_beSucced = false;
                    return;
                }
                if (RoleTable.Rows.Count == 0)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��δ��ӹ���ɫ������");
                    pSysDB.CloseDbConnection();
                    m_beSucced = false;
                    return;
                }
                for (int i = 0; i < RoleTable.Rows.Count; i++)
                {
                    int pRoleID = -1;   //��ɫID
                    string pRoleName = ""; //��ɫ����
                    int pRoleTypeID = -1;   //��ɫ����ID
                    pRoleID = Convert.ToInt32(RoleTable.Rows[i][0].ToString().Trim());
                    pRoleName = RoleTable.Rows[i][1].ToString().Trim();
                    pRoleTypeID = Convert.ToInt32(RoleTable.Rows[i][2].ToString().Trim());
                    ComboBoxItem pItem = new ComboBoxItem(pRoleName, pRoleID);
                    cmbRoleName.Items.Add(pItem);
                }
                if (cmbRoleName.Items.Count > 0)
                {
                    cmbRoleName.SelectedIndex = 0;
                }
                #endregion
            }
            //else
            //{
               
            //}
            pSysDB.CloseDbConnection();
        }

      

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnAddRole_Click(object sender, EventArgs e)
        {
            Exception pError = null;

            try
            {
                #region �Խ�����п���
                if (txtComment.Text.Length >= 200)
                {
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ע����̫����");
                    return;
                }
                if (string.IsNullOrEmpty(this.txtRole.Text.Trim()))
                {
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", "�û���������Ϊ�գ�");
                    return;
                }
                if (txtRole.Text.IndexOf(" ") != -1)
                {
                    errorProvider1.SetError(txtRole, "���������пո�");
                    return;
                }

                #endregion

                SysCommon.DataBase.SysTable pSysDB = null;
                //����ϵͳά����
               ModDBOperate.ConnectDB(out pSysDB, out pError);
                if (pError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", pError.Message);
                    return;
                }
                int pRoleID = -1;       //��ɫID
                int pRoleTypeID = -1;     //��ɫ����ID
                pRoleTypeID = Convert.ToInt32((cmbGroupType.SelectedItem as ComboBoxItem).Value.ToString());
                if(pRoleTypeID == -1)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ��ɫ����IDʧ�ܣ�");
                    pSysDB.CloseDbConnection();
                    return;
                }

                //�ж��Ƿ����ͬ���Ľ�ɫ
                string tempStr = "select * from rolebaseinfo where ROLENAME='" + txtRole.Text.Trim() + "'";
                DataTable mTable = pSysDB.GetSQLTable(tempStr, out pError);
                if (pError != null || mTable == null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ��ɫ������Ϣ��ʧ��");
                    pSysDB.CloseDbConnection();
                    return;
                }
               

                if (m_BeUpdate)
                {
                    //�ж��Ƿ����ͬ����ɫ
                    if (mTable.Rows.Count > 1)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ͬ���Ľ�ɫ��");
                        pSysDB.CloseDbConnection();
                        return;
                    }
                    #region �Խ�����п���,��ɫ���Ʊ���
                    if (string.IsNullOrEmpty(cmbRoleName.Text.Trim()))
                    {
                        ErrorHandle.ShowFrmErrorHandle("��ʾ", "�û���������Ϊ�գ�");
                        return;
                    }
                    if (cmbRoleName.Text.Trim().IndexOf(" ") != -1)
                    {
                        errorProvider1.SetError(txtRole, "���������пո�");
                        return;
                    }
                   
                    #endregion

                    //�ж��Ƿ����ͬ���Ľ�ɫ
                    //if (cmbRoleName.Items.Contains(txtRole.Text.Trim()))
                    //{
                    //    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ͬ���Ľ�ɫ��");
                    //    pSysDB.CloseDbConnection();
                    //    return;
                    //}

                    #region �޸Ľ�ɫ��Ϣ�����½�ɫ������Ϣ��

                    pRoleID = Convert.ToInt32((cmbRoleName.SelectedItem as ComboBoxItem).Value.ToString().Trim()); 
                    string updateStr = "update rolebaseinfo set ROLENAME='"+txtRole.Text.Trim()+"', ROLETYPEID="+pRoleTypeID+", REMARK='"+txtComment.Text.Trim()+"' where ROLEID="+pRoleID;
                    pSysDB.UpdateTable(updateStr, out pError);
                    if (pError != null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�޸Ľ�ɫ������Ϣ��ʧ�ܣ�");
                        pSysDB.CloseDbConnection();
                        return;
                    }
                    pSysDB.CloseDbConnection();

                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�޸Ľ�ɫ��Ϣ�ɹ���");
                    #endregion
                }
                else
                {
                    //�ж��Ƿ����ͬ����ɫ
                    if (mTable.Rows.Count > 0)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ͬ���Ľ�ɫ��");
                        pSysDB.CloseDbConnection();
                        return;
                    }
                    #region ��ӽ�ɫ��Ϣ,����ɫ����������¼
                  
                    //��ѯ��ɫID�����ֵ
                    string selStr = "select Max(ROLEID) from rolebaseinfo";
                    DataTable tempTable = pSysDB.GetSQLTable(selStr, out pError);
                    if (pError != null || tempTable == null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ��ɫ������Ϣ��ʧ��");
                        pSysDB.CloseDbConnection();
                        return;
                    }
                    if (tempTable.Rows.Count == 0)
                    {
                        pRoleID = 1;
                    }
                    else
                    {
                        if (tempTable.Rows[0][0].ToString().Trim() == "")
                        {
                            //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ��ɫ����IDʧ�ܣ�");
                            //pSysDB.CloseDbConnection();
                            //return;
                            pRoleID = 1;
                        }
                        else
                        {
                            pRoleID = Convert.ToInt32(tempTable.Rows[0][0].ToString()) + 1;
                        }
                    }
                    if (pRoleID == -1)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ��ɫIDʧ�ܣ�");
                        return;
                    }
                    string str = "insert into rolebaseinfo(ROLEID,ROLENAME,ROLETYPEID,REMARK) values (";
                    str += pRoleID + ",'" + txtRole.Text.Trim() + "'," + pRoleTypeID + ",'" + txtComment.Text.Trim() + "')";
                    //�������ݿ�
                    pSysDB.UpdateTable(str, out pError);
                    if (pError != null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���½�ɫ������Ϣ��ʧ�ܣ�");
                        pSysDB.CloseDbConnection();
                        return;
                    }
                    pSysDB.CloseDbConnection();

                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ӽ�ɫ�ɹ���");
                    #endregion
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
               
            }
        }

        private void AddGroup_Load(object sender, EventArgs e)
        {
            if (m_BeUpdate)
            {
                this.Text = "�޸Ľ�ɫ";
                btnAddRole.Text = "�޸Ľ�ɫ";
                this.cmbRoleName.Enabled = true;
                //this.txtComment.Text = _role.Remark;
            }
            else
            {
                this.Text = "��ӽ�ɫ";
                btnAddRole.Text = "��ӽ�ɫ";
                this.cmbRoleName.Enabled = false;
                this.txtRole.Focus();
            }
            
        }

        private void txtRole_TextChanged(object sender, EventArgs e)
        {
            if (txtRole.Text.IndexOf(" ") != -1)
            {
                errorProvider1.SetError(txtRole, "���������пո�");
                return;
            }
            else
            {
                errorProvider1.Clear();
            }
        }

        private void cmbRoleName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //���ݽ�ɫ���Ƴ�ʼ����ɫ����

            txtRole.Text = cmbRoleName.Text.Trim();
            SysCommon.DataBase.SysTable pSysDB = null;
            Exception outError = null;
            //�������ݿ�
            ModDBOperate.ConnectDB(out pSysDB, out outError);
            if (outError != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", outError.Message);
                return;
            }

            string selStr = "";//��ʼ����ɫ�����б���ַ���
            selStr = "select * from roletypeinfo inner join rolebaseinfo on rolebaseinfo.ROLETYPEID=roletypeinfo.ROLETYPEID and rolebaseinfo.ROLEID=" + Convert.ToInt32((cmbRoleName.SelectedItem as ComboBoxItem).Value.ToString());   //��ѯ��ɫ���ͱ�
            DataTable pTable = pSysDB.GetSQLTable(selStr, out outError);
            if (outError != null||pTable==null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ��ɫ���ͱ�ʧ��,ԭ��" + outError.Message);
                pSysDB.CloseDbConnection();
                return;
            }
            if (pTable.Rows.Count == 0)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ��ɫ���ͱ�ʧ��,ԭ��" + outError.Message);
                pSysDB.CloseDbConnection();
                return;
            }
            //��������¼������ɫ������Ϣ���ص�ComboBox��
            //for (int j = 0; j < pTable.Rows.Count; j++)
            //{
                string roleTypeID = "";     //��ɫ����ID
                string roleTypeName = "";   //��ɫ����
                roleTypeID = pTable.Rows[0]["ROLETYPEID"].ToString().Trim();
                roleTypeName = pTable.Rows[0]["ROLEREMARK"].ToString().Trim();
                //ComboBoxItem pItem = new ComboBoxItem(roleTypeName, roleTypeID);
                //cmbGroupType.SelectedItem = pItem;
                cmbGroupType.Text = roleTypeName;
                //cmbGroupType.Items.Add(pItem);
            //}
            //if (cmbGroupType.Items.Count > 0)
            //{
            //    cmbGroupType.SelectedIndex = 0;
            //}

            //��ʼ����ע�б��
            selStr = "select * from rolebaseinfo where ROLEID=" + Convert.ToInt32((cmbRoleName.SelectedItem as ComboBoxItem).Value.ToString().Trim());
            pTable = pSysDB.GetSQLTable(selStr, out outError);
            if (outError != null || pTable==null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ��ɫ���ͱ�ʧ��,ԭ��" + outError.Message);
                pSysDB.CloseDbConnection();
                return;
            }
            if (pTable.Rows.Count == 0)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���û������ڣ�");
                pSysDB.CloseDbConnection();
                return;
            }
            txtComment.Text = pTable.Rows[0][3].ToString().Trim();

            pSysDB.CloseDbConnection();
        }
    }
}