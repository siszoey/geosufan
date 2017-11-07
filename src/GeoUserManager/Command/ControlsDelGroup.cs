using System;
using System.Collections.Generic;
using System.Text;
using SysCommon.Authorize;
using SysCommon.Error;

namespace GeoUserManager
{
    public class ControlsDelGroup : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
        public ControlsDelGroup()
        {
            base._Name = "GeoUserManager.ControlsDelGroup";
            base._Caption = "ɾ����ɫ";
            base._Tooltip = "ɾ����ɫ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "ɾ����ɫ";
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
                        if (pRole.UCtag == "Role" && m_Hook.RoleTree.SelectedNode != null && m_Hook.RoleTree.SelectedNode.Level != 0)
                        {
                            return true;
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
            if (m_Hook.RoleTree.SelectedNode != null)
            {
                Exception eError;
                Role role = m_Hook.RoleTree.SelectedNode.Tag as Role;
                if (role == null) return;
                if (role.Name == "��������Ա")
                {
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", "����Ա����ɾ����");
                    return;
                }
                if (ErrorHandle.ShowFrmInformation("ȷ��", "ȡ��", "ȷ��ɾ����"))
                {
                    if (ModuleOperator.DeleteData("role", "roleid", role.IDStr, ref ModData.gisDb, out eError))
                    {
                        if (this.WriteLog)
                        {
                            Plugin.LogTable.Writelog(Caption);//xisheng 2011.07.09 ������־
                        }
                        m_Hook.RoleTree.SelectedNode.Remove();
                    }
                    else
                    {
                        if (eError != null)
                        {
                            ErrorHandle.ShowInform("��ʾ", eError.Message);
                        }
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
