using System;
using System.Collections.Generic;
using System.Text;
using Fan.DataBase;
using System.Data;
using Fan.DataBase.Module;

namespace SysCommon
{
    public class User
    {
        #region Construction 
        /// <summary>
        /// �û���ȡ
        /// </summary>
        /// <param name="usercodeStr"></param>
        /// <param name="userPasswordStr"></param>
        /// <param name="iDBOperate"></param>
        public User(string usercodeStr,string userPasswordStr,IDBOperate iDBOperate)
        {
            m_UserCode = usercodeStr;
            m_UserPassword = userPasswordStr;
            m_dbOperate = iDBOperate;
        }
        /// <summary>
        /// �û�����
        /// </summary>
        /// <param name="iDBOperate"></param>
        public User(IDBOperate iDBOperate)
        {
            m_dbOperate = iDBOperate;
        }
        #endregion
        #region Class Attribute 
        private string m_UserCode = string.Empty;
        public string UserCode
        {
            get { return m_UserCode; }
        }
        private string m_UserPassword = string.Empty;
        public string UserPassword
        {
            get { return m_UserPassword; }
        }
        private Role m_UserRole = default(Role);
        public Role UserRole
        {
            get { return m_UserRole; }
        }
        private string m_UserName = string.Empty;
        public string UserName
        {
            get { return m_UserName; }
        }
        private IDBOperate m_dbOperate = default(IDBOperate);
        #endregion
        #region  Class Function
        /// <summary>
        /// ����Ƿ���Խ��е�¼��ֻ�ǽ����û���������ƥ��
        /// </summary>
        /// <returns></returns>
        public string CheckLogin()
        {
            DataTable dtUser = m_dbOperate.GetTable(TableName.VUserInfo, string.Format("{0}='{1}'", ColumnName.UserCode, m_UserCode));
            if (dtUser == null || dtUser.Rows.Count <= 0)
            {
                return string.Format("δ֪�ʺ�");
            }
            DataRow pRow = dtUser.Rows[0];
            string UserPass = Encryption.Decrypt(pRow[ColumnName.UserPassword].ToString());
            if (m_UserPassword != UserPass)
            {
                return string.Format("�������!");
            }
            m_UserRole = new Role(pRow[ColumnName.RoleID].ToString(), m_dbOperate);
            m_UserName = pRow[ColumnName.UserName].ToString();
            return string.Empty;
        }
        /// <summary>
        /// ����û�
        /// </summary>
        /// <param name="dicUser">�û���Ϣ</param>
        /// <returns></returns>
        public string AddUser(Dictionary<string,string> dicUser)
        {
            foreach (string key in dicUser.Keys)
            {
                switch (key)
                {
                    case ColumnName.UserCode:
                        m_UserCode = dicUser[key];
                        break;
                    case ColumnName.UserName:
                        m_UserName = dicUser[key];
                        break;
                    case ColumnName.UserPassword:
                        m_UserPassword = Encryption.Encrypt(dicUser[key]);
                        break;
                    case ColumnName.RoleID:
                        m_UserRole = new Role(dicUser[key], m_dbOperate);
                        break;
                }
            }
            if (string.IsNullOrEmpty(m_UserCode))
            {
                return  string.Format("�޷�����û�:�û�ID����Ϊ��");
            }
            DataTable pTable = m_dbOperate.GetTable(TableName.TUser, string.Format("{0}='{1}'", ColumnName.UserCode, m_UserCode));
            if (pTable.Rows.Count > 0)
            {
                return string.Format("�޷�����û�:��ǰ�û�ID�Ѿ�����");
            }
            if (!m_dbOperate.AddRow(TableName.TUser,new List<string> { ColumnName.UserCode, ColumnName.UserName, ColumnName.UserPassword},
                string.Format("'{0}'",m_UserCode),
                 string.Format("'{0}'", m_UserName),
                 string.Format("'{0}'", m_UserPassword)))
            {
                return string.Format("����û�ʧ��:���ѯ��Ӧ��־��Ϣ");
            }
            else
            {
                if (!m_dbOperate.AddRow(TableName.TUserRole, new List<string> { ColumnName.UserCode, ColumnName.RoleID },
                    string.Format("'{0}'", m_UserCode),
                   string.Format("'{0}'", m_UserRole.RoleID)))
                {
                    return string.Format("���û���Ȩʧ��:���ѯ��Ӧ��־��Ϣ");
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// �����û�
        /// </summary>
        /// <param name="dicUser">�û���Ϣ</param>
        /// <returns></returns>
        public string UpdateUser(Dictionary<string,string> dicUser)
        {
            foreach (string key in dicUser.Keys)
            {
                switch (key)
                {
                    case ColumnName.UserCode:
                        m_UserCode = dicUser[key];
                        break;
                    case ColumnName.UserName:
                        m_UserName = dicUser[key];
                        break;
                    case ColumnName.UserPassword:
                        m_UserPassword = Encryption.Encrypt(dicUser[key]);
                        break;
                    case ColumnName.RoleID:
                        m_UserRole = new Role(dicUser[key], m_dbOperate);
                        break;
                }
            }
            if (string.IsNullOrEmpty(m_UserCode))
            {
                return string.Format("�޷��޸��û�:�û�ID����Ϊ��");
            }
            DataTable pTable = m_dbOperate.GetTable(TableName.TUser, string.Format("{0}='{1}'", ColumnName.UserCode, m_UserCode));
            if (pTable.Rows.Count <= 0)
            {
                return string.Format("�޷��޸��û�:��ǰ�û�ID������");
            }
            if (!m_dbOperate.UpdateTable(TableName.TUser,
                string.Format("{0}='{1}'",ColumnName.UserCode,m_UserCode),
                string.Format("{0}='{1}'", ColumnName.UserName, m_UserName),
                string.Format("{0}='{1}'", ColumnName.UserPassword, m_UserPassword)
               ))
            {
                return string.Format("�޸��û�ʧ��:���ѯ��Ӧ��־��Ϣ");
            }
            else
            {
                if (!m_dbOperate.UpdateTable(TableName.TUserRole,
                    string.Format("{0}='{1}'", ColumnName.UserCode, m_UserCode),
                    string.Format("{0}='{1}'", ColumnName.RoleID, m_UserRole.RoleID)))
                {
                    return string.Format("�޸��û���Ȩʧ��:���ѯ��Ӧ��־��Ϣ");
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// ɾ���û�
        /// </summary>
        /// <returns></returns>
        public string DeleteUser()
        {
            if (string.IsNullOrEmpty(m_UserCode))
            {
                return string.Format("ɾ���û�ʧ��:�û�IDΪ��");
            }
            if(!m_dbOperate.DeleteRow(TableName.TUserRole,string.Format("{0}='{1}'",ColumnName.UserCode,m_UserCode)))
            {
                return string.Format("ɾ���û�Ȩ��ʧ��:���ѯ��Ӧ��־��Ϣ");
            }
            if (!m_dbOperate.DeleteRow(TableName.TUser, string.Format("{0}='{1}'", ColumnName.UserCode, m_UserCode)))
            {
                return string.Format("ɾ���û�ʧ��:���ѯ��Ӧ��־��Ϣ");
            }
            return string.Empty;
        }
        #endregion
    }
}
