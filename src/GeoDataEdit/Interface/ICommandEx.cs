using System;
using System.Collections.Generic;
using System.Text;



namespace GeoDataEdit
{
    public interface ICommandEx
    {
        bool GetInputMsg ( string sType , string strInfo );

        //���ڵõ��Ҽ��˵���Keyֵ
        void GetMenuKey ( string sKey );

        //����Tool��ı���
        void ResetMoveEvent ();
    }
}
