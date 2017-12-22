using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Fan.Common.Error
{
    public partial class frmErrorHandleEx : BaseForm
    {
        public frmErrorHandleEx(string strCaption, string strDes,List<string> pList)
        {
            InitializeComponent();
            this.Text = strCaption;
            this.labelX.Text = strDes;
            if (pList != null)
            {
                for (int i = 0; i < pList.Count; i++)
                {
                    ListViewItem pItem = new ListViewItem();
                    pItem.Text = pList[i];
                    listErrorInfo.Items.Add(pItem);
                }
            
            }
            //********************************************
            //guozheng added ϵͳ������־
            // 1 �Ǵ�����ʾ��־
            List<string> Pra = new List<string>();
            Pra.Add(strCaption);
            Pra.Add(strDes);
            if (null == Fan.Common.Log.Module.SysLog)
                Fan.Common.Log.Module.SysLog = new Fan.Common.Log.clsWriteSystemFunctionLog();
            Fan.Common.Log.Module.SysLog.Write(1, Pra);
            //********************************************
        }

        private void buttonX_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}