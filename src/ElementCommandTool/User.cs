using System;
using System.Collections.Generic;
using System.Text;

namespace SysCommon.Authorize
{
    public class User
    {
        /// <summary>
        /// ���
        /// </summary>
        private string _id;
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// �û����ƣ���ƣ�
        /// </summary>
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

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
        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// �Ա�
        /// </summary>
        private int _sex;
        public int Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }

        /// <summary>
        /// ְ��
        /// </summary>
        private string _position;
        public string Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// ��ע
        /// </summary>
        private string _remark;
        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }
    }
}
