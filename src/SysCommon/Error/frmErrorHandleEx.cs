using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SysCommon.Error
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
            if (null == SysCommon.Log.Module.SysLog)
                SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
            SysCommon.Log.Module.SysLog.Write(1, Pra);
            //********************************************
        }

        private void buttonX_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}