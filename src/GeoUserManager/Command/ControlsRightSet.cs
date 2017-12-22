using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using Fan.Common.Authorize;
using Fan.Common.Error;

namespace GeoUserManager
{
    public class ControlsRightSet : Fan.Plugin.Interface.CommandRefBase
    {
        private Fan.Plugin.Application.IAppPrivilegesRef m_Hook;
        public ControlsRightSet()
        {
            base._Name = "GeoUserManager.ControlsRightSet";
            base._Caption = "Ȩ������";
            base._Tooltip = "Ȩ������";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "Ȩ������";
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
                        if (pRole.UCtag == "Role")
                        {
                            if (m_Hook.RoleTree.SelectedNode != null)
                            {
                                if (m_Hook.PrivilegeTree.Nodes.Count > 0)
                                {
                                    return true;
                                }
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
                Fan.Plugin.Application.IAppPrivilegesRef pAppFormRef = m_Hook as Fan.Plugin.Application.IAppPrivilegesRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Fan.Plugin.Application.IAppPrivilegesRef pAppFormRef = m_Hook as Fan.Plugin.Application.IAppPrivilegesRef;
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
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "ѡ��Ȩ���ļ�";
                ofd.Filter = "Xml�ļ�|*xml";
                ofd.RestoreDirectory = false;
                if (ofd.ShowDialog()==DialogResult.OK)
                {
                    XmlDocument doc = new XmlDocument();
                    string fileName = ofd.FileName;
                    doc.Load(fileName);
                    //���û������Ȩ��
                    if (ModuleOperator.AddPrivilege(role, doc, ref ModData.gisDb, out eError))
                    {
                        //��Ȩ����ʾ��Ȩ������
                        ModuleOperator.DisplayInLstView(doc, m_Hook.PrivilegeTree);
                    }
                    else
                    {
                        if (eError != null)
                        {
                            ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                            return;
                        }
                    }
                }
            }
        }

        public override void OnCreate(Fan.Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            m_Hook = hook as Fan.Plugin.Application.IAppPrivilegesRef;
        }

    }
}
