using System;
using System.Collections.Generic;
using System.Text;
using SysCommon.Authorize;
using SysCommon.Error;

namespace GeoUserManager
{
    public class ControlsDelUser : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;                //����Ĵ������
        private User m_AppUser;                                             //��ǰ��½���û�

        public ControlsDelUser()
        {
            base._Name = "GeoUserManager.ControlsDelUser";
            base._Caption = "ɾ���û�";
            base._Tooltip = "ɾ���û�";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "ɾ���û���Ϣ";
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
                        if (pRole.UCtag == "User" && m_Hook.UserTree.SelectedNode != null)
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
                if (m_AppUser == null) return;
                if (user.Name.ToLower() == "admin")
                {
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", "����Ա����ɾ����");
                    return;
                }
                if (user.IDStr  == m_AppUser.IDStr )
                {
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", "�û�����ɾ���Լ���");
                    return;
                }
                if (ErrorHandle.ShowFrmInformation("ȷ��", "ȡ��", "ȷ��ɾ����"))
                {
                    //ɾ����ȡ��Χ
                    ModuleOperator.DeleteData("USER_EXPORT", "userid", user.IDStr, ref ModData.gisDb, out eError);
                    if (ModuleOperator.DeleteData("user_info", "userid", user.IDStr, ref ModData.gisDb, out eError))
                    {
                        if (this.WriteLog)
                        {
                            Plugin.LogTable.Writelog(Caption);//xisheng 2011.07.09 ������־
                        }
                        m_Hook.UserTree.SelectedNode.Remove();
                    }
                    else
                    {
                        if (eError != null)
                        {
                            ErrorHandle.ShowInform("��ʾ", eError.Message);
                            return;
                        }
                    }
                }
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Plugin.Application.IAppPrivilegesRef;
            m_AppUser = (m_Hook as Plugin.Application.IAppFormRef).ConnUser;
        }

    }
}
