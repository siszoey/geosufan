using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using GeoDataCenterFunLib;
namespace GeoDataManagerFrame
{
    public partial class FormLandUseReport : DevComponents.DotNetBar.Office2007Form
    {
        public FormLandUseReport()
        {
            InitializeComponent();
            InitDataSource();
        }
        private void InitDataSource()
        {

            string path = Application.StartupPath + "\\..\\OutputResults\\���ܳɹ�\\ɭ����Դ��״����" ;
            DirectoryInfo pathinfo = new DirectoryInfo(path);
            if (Directory.Exists(path))
            {
                foreach (FileInfo Finfo in pathinfo.GetFiles("*.mdb"))
                {
                    string DataSourceName = Finfo.Name.Substring(0, Finfo.Name.IndexOf("."));
                    this.comboBoxExDataSource.Items.Add(DataSourceName);
                }
            }

        }
        private void buttonXOK_Click(object sender, EventArgs e)
        {
            if (this.comboBoxExAreaName.Text.Equals(""))
            {
                MessageBox.Show("��ѡ�������λ��");
                return;
            }
            if (this.comboBoxExDLGrade.Text.Equals(""))
            {
                MessageBox.Show("��ѡ����༶��");
                return;
            }
            if (this.comboBoxExDataSource.Text.Equals(""))
            {
                MessageBox.Show("��ѡ���������Դ��");
                return;
            }
            string DataSourceName = this.comboBoxExDataSource.Text;
            int index1=DataSourceName.IndexOf("(");
            int index2=DataSourceName.IndexOf(")");
            string xzqcode = DataSourceName.Substring(index1+1,index2-index1-1);
            string DataSourcePath =Application.StartupPath + "\\..\\OutputResults\\���ܳɹ�\\ɭ����Դ��״����";
            string AreaName = this.comboBoxExAreaName.Text;
            string dlGrade = this.comboBoxExDLGrade.Text;
            int iDLJB=1;
            switch (dlGrade)
            {
                case "һ��":
                    iDLJB = 1;
                    break;
                case "����":
                    iDLJB = 2;
                    break;
                default:
                    break;
            }
            //��ʼ��������
            SysCommon.CProgress vProgress = new SysCommon.CProgress("������");
            vProgress.EnableCancel = false;
            vProgress.ShowDescription = true;
            vProgress.FakeProgress = true;
            vProgress.TopMost = true;
            vProgress.ShowProgress();
            vProgress.SetProgress("ͳ��ũ��ɭ����Դ��״����������ܱ�");
            ModStatReport.LandUseCurReport(DataSourcePath, DataSourceName + ".mdb", xzqcode, AreaName,2, iDLJB,"",vProgress);
            this.DialogResult = DialogResult.OK;
        }

        private void buttonXQuit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}