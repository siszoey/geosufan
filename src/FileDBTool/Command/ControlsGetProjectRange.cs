using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
namespace FileDBTool
{
    class ControlsGetProjectRange:Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppFileRef m_Hook;
        public ControlsGetProjectRange()
        {
            base._Name = "FileDBTool.ControlsGetProjectRange";
            base._Caption = "��ȡ��ͼ";
            base._Tooltip = "��ȡ��Ŀ�ĵ�ͼ��Χ";
            base._Visible = true;
            base._Enabled = true;
            base._Message = "��ȡ��Ŀ�ĵ�ͼ��Χ";

        }

        public override bool Enabled
        {
            get
            {
                //if (m_Hook == null) return false;
                //if (m_Hook.CurrentThread != null) return false;
                //if (m_Hook.ProjectTree.SelectedNode == null) return false;
                //if (m_Hook.ProjectTree.SelectedNode.DataKey == null) return false;
                //if (m_Hook.ProjectTree.SelectedNode.DataKey.ToString() != EnumTreeNodeType.PROJECT.ToString())
                //    return false;
                return false;
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
          ///////ִ�л�ȡ��ͼ����/////////////   
            DevComponents.AdvTree.Node mDBNode = m_Hook.ProjectTree.SelectedNode;
            DevComponents.AdvTree.Node ProjectNode = m_Hook.ProjectTree.SelectedNode;
            string ipStr = "";
            string ConnStr = "";
            long ProjectId = -1;
            Exception ex=null;
            ///////��ȡ������Ŀ�ڵ�
            while (EnumTreeNodeType.PROJECT.ToString()!= ProjectNode.DataKey.ToString())
            {
                if (ProjectNode.Parent == null)
                    return;
                ProjectNode = ProjectNode.Parent;
            }
            /////��ȡ���ӽڵ�
            while (mDBNode.Parent != null)
            {
                mDBNode = mDBNode.Parent;
            }
            if (mDBNode.Name == "�ļ�����")
            {
                System.Xml.XmlElement dbElem = mDBNode.Tag as System.Xml.XmlElement;
                if (dbElem == null) return;
                ipStr = dbElem.GetAttribute("MetaDBConn");
                ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + ipStr + ";Persist Security Info=True";//Ԫ���������ַ���
                try
                {
                    string pId = ProjectNode.Tag.ToString();
                    ProjectId = Convert.ToInt64(pId);
                }
                catch
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("������ʾ","��ȡ������ĿIDʧ��!");
                    return;
                }
                /////////////////���е�ͼ�Ļ�ȡ/////////////////////
               // ModDBOperator.AddProjectMapRange(ProjectId, ipStr, out ex);
                if (null != ex)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("������ʾ0", "��ȡ��ͼʧ�ܣ�");
                }
                //////////////////////////////////////////////////////////////////////////////

            }


        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            m_Hook = hook as Plugin.Application.IAppFileRef;
            if (m_Hook == null) return;
        }
       
    }
}
