using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GeoDBATool
{
    /// <summary>
    /// ���Ƿ����
    /// </summary>
     public class ControlsInputMapFramData: Plugin.Interface.CommandRefBase
    {

        private Plugin.Application.IAppGISRef m_Hook;

         public ControlsInputMapFramData()
        {
            base._Name = "GeoDBATool.ControlsInputMapFramData";
            base._Caption = "ͼ���������";
            base._Tooltip = "����ͼ�����������ͼ������������";
            base._Visible = true;
            base._Enabled = true;
            base._Message = "����ͼ�����������ͼ������������";

        }

        public override bool Enabled
        {
            get
            {
                if (m_Hook == null) return false;
                if (m_Hook.CurrentThread != null) return false;
                if (m_Hook.ProjectTree.SelectedNode == null) return false;
                if (m_Hook.ProjectTree.SelectedNode.DataKeyString != "project") return false;
                if (m_Hook.ProjectTree.SelectedNode.Tag == null) return false;
                XmlNode ProNode = m_Hook.ProjectTree.SelectedNode.Tag as XmlNode;
                if (ProNode == null) return false;
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
            frmDataTransplant newFrm = new frmDataTransplant("ͼ���������",EnumOperateType.Input,m_Hook, m_Hook.ProjectTree.SelectedNode.Tag as XmlElement);
            newFrm.ShowDialog();
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            m_Hook = hook as Plugin.Application.IAppGISRef;
        }
    }
}
