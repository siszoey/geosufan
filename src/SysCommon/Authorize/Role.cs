using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SysCommon.Authorize
{
    public class Role
    {
        /// <summary>
        /// ���
        /// </summary>
        /// 

        
        private string _TypeID;
      
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        //20110518 add
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
        /// ������
        /// </summary>
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// ��ɫ��Ӧ����Ŀ��ID
        /// </summary>
        private string _ProjectID;
        public string PROJECTID
        {
            get { return _ProjectID; }
            set { _ProjectID = value; }
        }

        /// <summary>
        /// ������ID
        /// </summary>
        public string TYPEID
        {
            get { return _TypeID; }
            set { _TypeID = value; }
        }

        /// <summary>
        /// ���������
        /// </summary>
        private string _type;
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        /// <summary>
        /// Ȩ��
        /// </summary>
        private XmlDocument _privilege;
        public XmlDocument Privilege
        {
            get { return _privilege; }
            set { _privilege = value; }
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
        /// <summary>
        /// ��ɫ����ID��0:����Ա��1����ͨ�û���
        /// </summary>
        private int _RoleTypeID = -1;
        public int RoleTypeID
        {
            get { return this._RoleTypeID; }
            set { this._RoleTypeID = value; }
        }

     
        /// <summary>
        /// ������
        /// </summary>
        //private string _name;
        //public string Name
        //{
        //    get { return _name; }
        //    set { _name = value; }
        //}

        /// <summary>
        /// Ȩ��
        /// </summary>
        //private XmlDocument _privilege;
        //public XmlDocument Privilege
        //{
        //    get { return _privilege; }
        //    set { _privilege = value; }
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
        //end
    }
}
