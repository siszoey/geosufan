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
    public partial class AddGroup : DevComponents.DotNetBar.Office2007Form
    {
        Role _role = null;                      //�û�����
        bool isUpdate = false;                  //�жϵ�ǰ�Ƿ�Ϊ����

        public AddGroup()
        {
            InitializeComponent();
        }

        public AddGroup(Role role)
        {
            InitializeComponent();
            _role = role;
            isUpdate = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnAddRole_Click(object sender, EventArgs e)
        {
            Exception exError = null;
            
            try
            {
                if (string.IsNullOrEmpty(this.txtRole.Text))
                {
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ɫ������Ϊ�գ�");
                    return;
                }
                string typeid = "";
                SysGisTable sysTable = new SysGisTable(ModData.gisDb);
                if (this.comboBoxRoleType.Text.Equals(""))
                {
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ɫ���Ͳ���Ϊ�գ�");
                    return;
                }
                else 
                {
                    typeid = sysTable.GetFieldValue("ROLETYPE", "TYPEID", "ROLETYPE='" + this.comboBoxRoleType.Text + "'", out exError).ToString();             
                }
                ModData.gisDb.StartTransaction(out exError);
                
                Dictionary<string, object> dicData = new Dictionary<string, object>();
                dicData.Add("roleid", this.txtRole.Text.Trim() + DateTime.Now.ToString("yyyymmddhhss"));
                dicData.Add("name", this.txtRole.Text.Trim());
                dicData.Add("remark", this.txtComment.Text.Trim());

                string projectid = "";
                if (!this.comboBoxProjectgroup.Text.Equals(""))
                {
                    object objProject = sysTable.GetFieldValue("PROJECTGROUP", "PROJECTID", "PROJECTNAME='" + this.comboBoxProjectgroup.Text + "'", out exError);
                    if (objProject == null)
                    {
                        projectid = System.Guid.NewGuid().ToString();
                        Dictionary<string, object> dicProjectGroup = new Dictionary<string, object>();
                        dicProjectGroup.Add("PROJECTNAME", this.comboBoxProjectgroup.Text);
                        dicProjectGroup.Add("PROJECTID", projectid);
                        sysTable.NewRow("PROJECTGROUP", dicProjectGroup, out exError);

                    }
                    else
                    {
                        projectid = objProject.ToString();
                    }
                }
                dicData.Add("TYPEID", typeid);
                dicData.Add("PROJECTID",projectid );
                if (isUpdate)
                {
                    if(!this.txtRole.Text.Trim().Equals(_role.Name))
                    {
                        if (sysTable.ExistData("role", "name='" + this.txtRole.Text.Trim() + "'"))
                        {
                            ErrorHandle.ShowFrmErrorHandle("��ʾ", "�Ѵ�����ͬ�Ľ�ɫ����");
                            return;
                        }
                    }
                    dicData.Remove("roleid");
                    if (!sysTable.UpdateRow("role", "roleid='" + _role.IDStr + "'", dicData, out exError))
                    {
                        if (exError != null)
                        {
                            ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ʧ�ܣ�" + exError.Message);
                        }
                        else
                        {
                            ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ʧ�ܣ�");
                        }
                        return;
                    }
                }
                else
                {
                    if (sysTable.ExistData("role", "name='" + this.txtRole.Text.Trim() + "'"))
                    {
                        ErrorHandle.ShowFrmErrorHandle("��ʾ", "�Ѵ�����ͬ�Ľ�ɫ����");
                        return;
                    }
                    if (!sysTable.NewRow("role", dicData, out exError))
                    {
                        if (exError != null)
                        {
                            ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ʧ�ܣ�" + exError.Message);
                        }
                        else
                        {
                            ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ʧ�ܣ�");
                        }
                        return;
                    }
                }
                ModData.gisDb.EndTransaction(true, out exError);
                this.DialogResult = DialogResult.OK;
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
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
                }
            }
        }

        private void AddGroup_Load(object sender, EventArgs e)
        {
            Exception exError;
            SysGisTable sysTable = new SysGisTable(ModData.gisDb);
            List<object> typelist = sysTable.GetFieldValues("ROLETYPE", "ROLETYPE", "", out exError);
            List<object> projectlist = sysTable.GetFieldValues("PROJECTGROUP", "PROJECTNAME", "", out exError);
            foreach (object typeitem in typelist)
            {
                this.comboBoxRoleType.Items.Add(typeitem.ToString());
            }
            foreach (object projectitem in projectlist)
            {
                this.comboBoxProjectgroup.Items.Add(projectitem.ToString());
            }
            if (isUpdate)
            {
                btnAddRole.Text = "����";
                this.txtRole.Text = _role.Name;
                this.txtComment.Text = _role.Remark;


                if (!_role.TYPEID.Equals(""))
                {
                    string typeName = sysTable.GetFieldValue("ROLETYPE", "ROLETYPE", "TYPEID='" + _role.TYPEID + "'", out exError).ToString();
                    this.comboBoxRoleType.Text = typeName;
                }
                if (!_role.PROJECTID.Equals(""))
                {
                    string projectName = sysTable.GetFieldValue("PROJECTGROUP", "PROJECTNAME", "PROJECTID='" + _role.PROJECTID + "'", out exError).ToString();
                    this.comboBoxProjectgroup.Text = projectName;
                }
            }
            else
            {
                btnAddRole.Text = "���";
            }
            this.txtRole.Focus();
        }

    }
}