using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;
using Fan.DataBase;
using Fan.DataBase.Module;

namespace Fan.Common
{
    public class Role
    {
        public Role(string strRoleID, IDBOperate iDbOperate)
        {
            m_RoleID = strRoleID;
            m_DbOperate = iDbOperate;
        }
        #region Class Attribute 
        private string m_RoleID = string.Empty;
        public string RoleID
        {
            get { return m_RoleID; }
        }
        private string m_RoleName = string.Empty;
        public string RoleName
        {
            get { return m_RoleName; }
        }
        public DataTable RoleFunDt
        {
            get {
                DataTable Dt = default(DataTable);
                if (!string.IsNullOrEmpty(m_RoleID))
                {
                    Dt = m_DbOperate.GetTable(TableName.VRoleFunction, string.Format("{0}='{1}'", ColumnName.RoleID, m_RoleID));
                }
                return Dt;
            }
        }
        private IDBOperate m_DbOperate = default(IDBOperate);
        #endregion

        #region Class Function
        /// <summary>
        /// ���½�ɫ����Ȩ�ޣ���ɾ������
        /// </summary>
        /// <param name="listFunc"></param>
        /// <returns></returns>
        public string UpdateRoleFunction(List<string> listFunc)
        {
            if (listFunc.Count <= 0) return string.Format("�����б���Ϊ��");
            if (!m_DbOperate.DeleteRow(TableName.TRoleFunction, string.Format("{0}='{1}'", ColumnName.RoleID, m_RoleID)))
            {
                return string.Format("�޸�Ȩ��ʧ�ܣ�ɾ��Ȩ��ʧ��");
            }
            foreach (string func in listFunc)
            {
                if (m_DbOperate.AddRow(TableName.TRoleFunction, new List<string> { ColumnName.RoleID,ColumnName.FID },m_RoleID,func)) continue;
                else return string.Format("�޸�Ȩ��ʧ��");
            }
            return string.Empty;
        }
        /// <summary>
        /// ��ӽ�ɫ
        /// </summary>
        /// <param name="dicRoleInfo">��ɫ��Ϣ</param>
        /// <param name="listFunc">Ȩ��ID�б�</param>
        /// <returns></returns>
        public string AddRole(Dictionary<string,string>dicRoleInfo,List<string> listFunc)
        {
            if (dicRoleInfo.Count <= 0 || listFunc.Count <= 0)
            {
                return string.Format("��ӽ�ɫʧ�ܣ�������ɫ��Ϣ����ȫ");
            }
            foreach (string key in dicRoleInfo.Keys)
            {
                switch (key)
                {
                    case ColumnName.RoleID:
                        m_RoleID = dicRoleInfo[key];
                        break;
                    case ColumnName.RoleName:
                        m_RoleName = dicRoleInfo[key];
                        break;
                }
            }
            DataTable pDt = m_DbOperate.GetTable(TableName.TRole, string.Format("{0}='{1}'", ColumnName.RoleID,m_RoleID));
            if (pDt != null && pDt.Rows.Count > 0)
            {
                return string.Format("��ӽ�ɫʧ�ܣ���ɫID�Դ���");
            }
            if (!m_DbOperate.AddRow(TableName.TRole, new List<string> { ColumnName.RoleID, ColumnName.RoleName },
                m_RoleID, m_RoleName))
            {
                return string.Format("��ӽ�ɫʧ�ܣ�д������ʧ��");
            }
            return UpdateRoleFunction(listFunc);
        }
        /// <summary>
        /// ɾ����ɫ
        /// </summary>
        /// <returns></returns>
        public string DeleteRole()
        {
            if (string.IsNullOrEmpty(m_RoleID)) return string.Format("ɾ����ɫʧ�ܣ���ɫID����Ϊ��");
            if (!m_DbOperate.DeleteRow(TableName.TRoleFunction, string.Format("{0}='{1}'", ColumnName.RoleID, m_RoleID)))
            {
                return string.Format("ɾ����ɫʧ�ܣ�ɾ����ɫȨ��ʧ��");
            }
            if (!m_DbOperate.DeleteRow(TableName.TRole, string.Format("{0}='{1}'", ColumnName.RoleID, m_RoleID)))
            {
                return string.Format("ɾ����ɫʧ��");
            }
            return string.Empty;
        }
        /// <summary>
        /// ���½�ɫ��Ϣ
        /// </summary>
        /// <param name="UpdateRole">��ɫ��Ϣ</param>
        /// <returns></returns>
        public string UpdateRole(Dictionary<string,string> UpdateRole)
        {
            if (UpdateRole == null || UpdateRole.Count <= 0)
            {
                return string.Format("���½�ɫ��Ϣʧ��");
            }
            foreach (string key in UpdateRole.Keys)
            {
                switch (key)
                {
                    case ColumnName.RoleName:
                        m_RoleName = UpdateRole[key];
                        break;
                    case ColumnName.RoleID:
                        m_RoleID = UpdateRole[key];
                        break;
                }
            }
            if (!m_DbOperate.UpdateTable(TableName.TRole, string.Format("{0}='{1}'", ColumnName.RoleID, m_RoleID),
                string.Format("{0}='{1}'", ColumnName.RoleName, m_RoleName)))
            {
                return string.Format("���½�ɫ��Ϣʧ�ܣ��������ݿ�ʧ��");
            }
            return string.Empty;
        }
        #endregion
    }
}
