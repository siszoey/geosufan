using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GeoUserManager
{
    public partial class frmEditPassword : DevComponents.DotNetBar.Office2007Form
    {
        public frmEditPassword()
        {
            InitializeComponent();
        }

        //pl
        Fan.Common.Gis.SysGisDataSet m_pGisDb;
        public Fan.Common.Gis.SysGisDataSet GisDB
        {
            set { m_pGisDb = value; }
        }
        //
        string m_intUserID;
        public string UserID
        {
            set { m_intUserID = value; }
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            if (m_pGisDb == null) return;

            Exception Err;
            if (this.txtOldSec.Text.Trim() == "" || this.txtNewSec.Text.Trim()=="" || this.txtNewSec2.Text=="")
            {
                Fan.Common.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ","�¾����붼����Ϊ�ա�");
                return;
            }
            if (this.txtNewSec2.Text.Trim() != this.txtNewSec.Text.Trim())
            {
                Fan.Common.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��������������벻һ�¡�");
                return;
            }
            
            //��ʼ�޸�����
            Fan.Common.Gis.SysGisTable vGisTb = new Fan.Common.Gis.SysGisTable(m_pGisDb);

            string strOldPass = vGisTb.GetFieldValue("USER_INFO", "UPWD", "USERID='" + m_intUserID.ToString()+"'", out Err).ToString();
            if (Fan.Common.Authorize.AuthorizeClass.ComputerSecurity(this.txtOldSec.Text.Trim()) != strOldPass)
            {
                Fan.Common.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ľ����벻��ȷ��");
                return;
            }

            Dictionary<string, object> dicvalue = new Dictionary<string, object>();
            dicvalue.Add("UPWD", Fan.Common.Authorize.AuthorizeClass.ComputerSecurity(this.txtNewSec.Text.Trim()));
            if (vGisTb.UpdateRow("USER_INFO", "USERID='" + m_intUserID.ToString()+"'", dicvalue, out Err))
            {
                Fan.Common.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�޸�����ɹ���");
                this.Close();
            }
            else
            {
                Fan.Common.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "ϵͳ������ȷ���޸����롣");
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}