using System;
using System.Collections.Generic;
using System.Text;

namespace SysCommon.Authorize
{
    public class User
    {
        /// <summary>
        /// ��Ч��  wgf 20111102
        /// </summary>
        private string U_UserDate;
        public string UserDate
        {
            get { return U_UserDate; }
            set { U_UserDate = value; }
        }
        /// <summary>
        /// �û�������Ϣ ygc 20130319
        /// </summary>
        private string U_UserDepartment;
        public string UserDepartment
        {
            get { return U_UserDepartment; }
            set { U_UserDepartment = value; }
        }

        /// <summary>
        /// ���
        /// </summary>
        private int U_ID;
        public int ID
        {
            get { return U_ID; }
            set { U_ID = value; }
        }

        /// <summary>
        /// �û���
        /// </summary>
        private string U_NAME;
        public string Name
        {
            get { return U_NAME; }
            set { U_NAME = value; }
        }

        /// <summary>
        /// ����
        /// </summary>
        private string U_PWD;
        public string Password
        {
            get { return U_PWD; }
            set { U_PWD = value; }
        }

        /// <summary>
        /// �û���ɫ
        /// </summary>
        private string U_ROLE;
        public string Role
        {
            get { return U_ROLE; }
            set { U_ROLE = value; }
        }

        /// <summary>
        /// �û���ɫ����
        /// </summary>
        private int U_RoleTypeID;
        public int RoleTypeID
        {
            get
            {
                return U_RoleTypeID;
            }
            set
            {
                U_RoleTypeID = value;
            }
        }

        /// <summary>
        /// �Ա�
        /// </summary>
        private string U_SEX;
        public string Sex
        {
            get { return U_SEX; }
            set { U_SEX = value; }
        }

        /// <summary>
        /// ְ��
        /// </summary>
        private string U_JOB;
        public string Position
        {
            get { return U_JOB; }
            set { U_JOB = value; }
        }
        /// <summary>
        /// ְ��
        /// </summary>
        private double U_ExportArea;
        public double  ExportArea
        {
            get { return U_ExportArea; }
            set { U_ExportArea = value; }
        }
        /// <summary>
        /// ��ע
        /// </summary>
        private string U_REMARK;
        public string Remark
        {
            get { return U_REMARK; }
            set { U_REMARK = value; }
        }

        private string _loginInfo;
        public string LoginInfo
        {
            get
            {
                return _loginInfo;
            }
            set
            {
                _loginInfo = value;
            }
        }


        //20110518  add
        /// <summary>
        /// ���
        /// </summary>
        private string _idStr;
        public string IDStr
        {
            get { return _idStr; }
            set { _idStr = value; }
        }

        /// <summary>
        /// �û����ƣ���ƣ�
        /// </summary>
        //private string _name;
        //public string NameStr
        //{
        //    get { return _name; }
        //    set { _name = value; }
        //}

        /// <summary>
        /// �û����ƣ���ʵ����
        /// </summary>
        private string _trueName;
        public string TrueName
        {
            get { return _trueName; }
            set { _trueName = value; }
        }

        /// <summary>
        /// ����
        /// </summary>
        //private string _password;
        //public string Password
        //{
        //    get { return _password; }
        //    set { _password = value; }
        //}

        /// <summary>
        /// �Ա�
        /// </summary>
        private int _sex;
        public int SexInt
        {
            get { return _sex; }
            set { _sex = value; }
        }

        /// <summary>
        /// ְ��
        /// </summary>
        //private string _position;
        //public string Position
        //{
        //    get { return _position; }
        //    set { _position = value; }
        //}

        /// <summary>
        /// ��ע
        /// </summary>
        //private string _remark;
        //public string Remark
        //{
        //    get { return _remark; }
        //    set { _remark = value; }
        //}
        //
    }
}
