using System;
using System.Collections.Generic;
using System.Text;
using SysCommon.Authorize;
using SysCommon.Error;
using System.Windows.Forms;

namespace GeoDBIntegration
{
    /// <summary>
    /// chenyafei  20110311 add content�� ɾ���û�����������Ա�߱�ɾ���û����ʸ�
    /// </summary>
    public class ControlsDelUser : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppDBIntegraRef m_Hook;                //����Ĵ������
        private User m_AppUser;                                             //��ǰ��½���û�

        public ControlsDelUser()
        {
            base._Name = "GeoDBIntegration.ControlsDelUser";
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
                //����ͨ�û���¼���ð�ť������
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
            //ɾ���û�
            DelGroup pDelGroup = new DelGroup(false);
            if (pDelGroup.BeSuccedd)
            {
                pDelGroup.ShowDialog();
            }
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(Caption);//xisheng 2011.07.09 ������־
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Plugin.Application.IAppDBIntegraRef;
            m_AppUser = (m_Hook as Plugin.Application.IAppFormRef).ConnUser;
        }

    }
}
