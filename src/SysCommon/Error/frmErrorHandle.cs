using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SysCommon.Error
{
    public partial class frmErrorHandle : DevComponents.DotNetBar.Office2007Form
    {
        public frmErrorHandle(string strCaption, string strDes)
        {
            InitializeComponent();
            this.Text = strCaption;
            this.labelX.Text = strDes;
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