using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SysCommon.Error;

namespace GeoDBIntegration
{
    /// <summary>
    /// chenyafei  20110311 add  content������û�����������Ա�߱�����û���Ȩ��
    /// </summary>
    public class ControlsAddUser : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppDBIntegraRef m_Hook;
        public ControlsAddUser()
        {
            base._Name = "GeoDBIntegration.ControlsAddUser";
            base._Caption = "����û�";
            base._Tooltip = "����û�";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "����û���Ϣ";
        }

        public override bool Enabled
        {
            get 
            {
                //��ͨ�û����߱�����û���Ȩ��
                if (ModuleData.m_User == null) return false;
                if (ModuleData.m_User.RoleTypeID == EnumRoleType.��ͨ�û�.GetHashCode() ) return false;
                return true;
            }
        }

        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = m_Hook as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

        public override void OnClick()
        {
            //����û�
            AddUser frmUser = new AddUser(null);
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(Caption);//xisheng 2011.07.09 ������־
            }
            if (frmUser.BeSuccedd)
            {
                frmUser.ShowDialog();
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Plugin.Application.IAppDBIntegraRef;
        }

    }
}
