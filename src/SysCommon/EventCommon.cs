using System;
using System.Collections.Generic;
using System.Text;

namespace Fan.Common
{
    #region ϵͳ��½ʱ��Ϣ��ʾ ί���¼�����
    //����ί��
    public delegate void SysLogInfoChangedHandle(object sender, SysLogInfoChangedEvent e);
    //����InfoChangedEvent��Ϣ
    public class SysLogInfoChangedEvent : EventArgs
    {
        //  �ı���Ϣ�ַ���
        private string _information;
        public string Information
        {
            get
            {
                return _information;
            }
        }

        //  Ĭ�Ϲ��캯��
        public SysLogInfoChangedEvent()
        {
            _information = string.Empty;
        }

        //  ʵ�ʹ��캯��
        public SysLogInfoChangedEvent(string information)
        {
            _information = information;
        }
    }

    #endregion
}
