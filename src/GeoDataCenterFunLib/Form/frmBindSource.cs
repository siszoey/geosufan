using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GeoDataCenterFunLib
{
    public partial class frmBindSource : DevComponents.DotNetBar.Office2007Form
    {
        public frmBindSource(int ii)
        {
            InitializeComponent();
            index = ii;
        }

        private int index=0;//�ж����ĵ��⻹��Ӱ���
        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string cellValue;
            if (e.RowIndex >= 0)
            {
                try
                {
                    if (dataGridView.Columns[e.ColumnIndex].GetType() == typeof(DataGridViewButtonColumn))
                    {
                        cellValue = dataGridView.Rows[e.RowIndex].Cells["Column5"].Value.ToString();
                        if (index == 0)
                        {
                           
                            frmSelectRaster frm = new frmSelectRaster();
                            frm.value = cellValue;
                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                               if(frm.value.Trim()!="")
                                dataGridView.Rows[e.RowIndex].Cells["Column5"].Value = frm.value;
                            }
                        }
                        else
                        {
                            frmSelectDoc frm = new frmSelectDoc();
                            frm.value = cellValue;
                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                                if (frm.value.Trim() != "")
                                dataGridView.Rows[e.RowIndex].Cells["Column5"].Value = frm.value;
                            }
                        }
                       
                    }
                }
                catch (Exception ex)
                {
                    //ErrorMessage(ex.Message);
                    return;
                }
            }

        }

        //�����б�
        private void frmBindRasterSource_Load(object sender, EventArgs e)
        {

            if (index == 0)
            {
                this.Text = "Ӱ���ҽ�";
                this.dataGridView.Columns["Column5"].HeaderText = "Ӱ���";
                LoadGirdViewRatster();
            }
            else
            {
                this.Text = "�ĵ���ҽ�";
                this.dataGridView.Columns["Column5"].HeaderText = "�ĵ���";
                LoadGirdViewDoc();
            }
           
        }
        private void LoadGirdViewRatster()
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select * from ��ͼ�����Ϣ��";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
            DataGridViewButtonCell cell = new DataGridViewButtonCell();
            cell.Value ="...";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strExp = "select ���� from  ��׼ר����Ϣ�� where ר������='" + dt.Rows[i]["ר������"].ToString() + "'";
                string name = db.GetInfoFromMdbByExp(strCon, strExp);
                dataGridView.Rows.Add(new object[] { dt.Rows[i]["ר������"].ToString(),name, dt.Rows[i]["���"].ToString(), dt.Rows[i]["��������"].ToString(), dt.Rows[i]["������"].ToString(), dt.Rows[i]["Ӱ���"].ToString(),cell});
            }

        }

        private void LoadGirdViewDoc()
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select * from ��ͼ�����Ϣ��";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
            DataGridViewButtonCell cell = new DataGridViewButtonCell();
            cell.Value = "...";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strExp = "select ���� from  ��׼ר����Ϣ�� where ר������='" + dt.Rows[i]["ר������"].ToString() + "'";
                string name = db.GetInfoFromMdbByExp(strCon, strExp);
                dataGridView.Rows.Add(new object[] { dt.Rows[i]["ר������"].ToString(), name, dt.Rows[i]["���"].ToString(), dt.Rows[i]["��������"].ToString(), dt.Rows[i]["������"].ToString(), dt.Rows[i]["�ĵ���"].ToString(), cell });
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                string strtype = "";
                string stryear = "";
                string strareaname = "";
                string strscale = "";
                string sourepath = "";

                for (int i = 0; i < dataGridView.Rows.Count; i++)
                {
                    sourepath = dataGridView.Rows[i].Cells["Column5"].Value.ToString();//����Դ
                    if (sourepath == "") continue;
                    strtype = dataGridView.Rows[i].Cells["Column1"].Value.ToString();//ר������
                    stryear = dataGridView.Rows[i].Cells["Column2"].Value.ToString();//���
                    strareaname = dataGridView.Rows[i].Cells["Column4"].Value.ToString();//������Ԫ
                    strscale = dataGridView.Rows[i].Cells["Column7"].Value.ToString();//������

                    if (index == 0)
                    {

                        GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                        string mypath = dIndex.GetDbInfo();
                        string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                        string strExp = string.Format("update ��ͼ�����Ϣ�� set Ӱ���='{0}' where ר������='{1}' and ���='{2}' and ��������='{3}' and ������='{4}' ",
                            sourepath, strtype, stryear, strareaname, strscale);
                        GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                        db.ExcuteSqlFromMdb(strCon, strExp);
                    }
                    else
                    {
                        GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                        string mypath = dIndex.GetDbInfo();
                        string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                        string strExp = string.Format("update ��ͼ�����Ϣ�� set �ĵ���='{0}' where ר������='{1}' and ���='{2}' and ��������='{3}' and ������='{4}' ",
                            sourepath, strtype, stryear, strareaname, strscale);
                        GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                        db.ExcuteSqlFromMdb(strCon, strExp);
                    }
                }
                MessageBox.Show("��Ϣ�ѱ��棡", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ee)
            {
                MessageBox.Show(ee.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}