using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GeoDataCenterFunLib
{
    public partial class frmExplain : DevComponents.DotNetBar.Office2007Form
    {
        public frmExplain(int ii)
        {
            InitializeComponent();
            index = ii;
        }
        private int index=0;
        private void frmExplain_Load(object sender, EventArgs e)
        {
            //if (index == 0)
            //    labelName.Text = "���ֳ���֮�м�����������Ϣ���ݿ����ϵͳ";
            //else
                //labelName.Text = "���ֳ���֮�м�����������Ϣ���ݿ����ϵͳ";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabelWWW_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("iexplore.exe");  
        }

        private void labelName_Click(object sender, EventArgs e)
        {

        }

        private void warningBox1_OptionsClick(object sender, EventArgs e)
        {

        }
    }
}