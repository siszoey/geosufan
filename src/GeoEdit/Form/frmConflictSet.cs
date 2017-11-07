using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GeoEdit
{
    public partial class frmConflictSet : DevComponents.DotNetBar.Office2007Form
    {
        bool beChildWin = true;   //�Ƿ�ʹ�õ�ǰ�汾
        bool beMerge = true;      //�Ƿ��ں�

        public bool BECHILDWIM
        {
            get
            {
                return beChildWin;
            }
            set
            {
                beChildWin = value;
            }
        }

        public bool BEMERGE
        {
            get
            {
                return beMerge;
            }
            set
            {
                beMerge = value;
            }
        }
        public frmConflictSet()
        {
            InitializeComponent();
            this.rbCurVersion.Checked = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbCurVersion.Checked)
            {
                beChildWin = true;
            }
            else
            {
                beChildWin = false;
            }
            if (chbMerge.Checked)
            {
                beMerge = true;
            }
            else
            {
                beMerge = false;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}