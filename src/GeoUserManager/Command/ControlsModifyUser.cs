using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SysCommon.Authorize;
using SysCommon.Error;

namespace GeoUserManager
{
    public class ControlsModifyUser : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;

        public ControlsModifyUser()
        {
            base._Name = "GeoUserManager.ControlsModifyUser";
            base._Caption = "�޸��û�";
            base._Tooltip = "�޸��û�";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�޸��û���Ϣ";
        }

        public override bool Enabled
        {
            get
            {
                if (m_Hook != null)
                {
                    if (m_Hook.MainForm.Controls[0] is UCRole)
                    {
                        UCRole pRole = m_Hook.MainForm.Controls[0] as UCRole;
                        if (pRole.UCtag == "User" && m_Hook.UserTree.SelectedNode != null )
                        {
                            if (ModuleOperator.GroupByName == "")
                            {
                                if (m_Hook.UserTree.SelectedNode.Level != 0)
                                    return true;
                            }
                            else
                            {
                                if (m_Hook.UserTree.SelectedNode.Level == 2)
                                    return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        public override string Message
        {
            get
            {
                Plugin.Application.IAppPrivilegesRef pAppFormRef = m_Hook as Plugin.Application.IAppPrivilegesRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppPrivilegesRef pAppFormRef = m_Hook as Plugin.Application.IAppPrivilegesRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

        public override void OnClick()
        {
            if (m_Hook.UserTree.SelectedNode != null)
            {
                Exception eError;
                User user = m_Hook.UserTree.SelectedNode.Tag as User;
                if (user == null) return;
                AddUser frmUser = new AddUser(user);
                if (frmUser.ShowDialog() == DialogResult.OK)
                {
                    if (this.WriteLog)
                    {
                        Plugin.LogTable.Writelog(Caption);//xisheng 2011.07.09 ������־
                    }
                    ModuleOperator.DisplayUserTree("", m_Hook.UserTree, ref ModData.gisDb,out eError);
                    if (eError != null)
                    {
                        ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                        return;
                    }
                }
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Plugin.Application.IAppPrivilegesRef;
        }

    }
}
