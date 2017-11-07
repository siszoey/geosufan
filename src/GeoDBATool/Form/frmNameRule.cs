using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using GeoDataCenterFunLib;
using ESRI.ArcGIS.Geodatabase;
using SysCommon.Gis;

namespace GeoDBATool
{
    public partial class frmNameRule :DevComponents.DotNetBar.Office2007Form
    {

        private TextBox tb;
        public frmNameRule(TextBox textbox,IWorkspace pworkspace)
        {
            InitializeComponent();
            tb = textbox;
            m_TempWorkspace = pworkspace;
            mSystable = new SysGisTable(pworkspace);
        }
        private IWorkspace m_TempWorkspace = null;
        string mypath = ModData.v_DbInfopath;
        private string prename;//��ȡ������ǰ׺�ı�������
        private SysGisTable mSystable ;
        private string[] array = new string[6];

        private void frmNameRule_Load(object sender, EventArgs e)
        {
            prename = tb.Text;
            LoadCombox();

            if (prename != "")
            {
                AnalyseDataToArray(prename);
                ChangeComboxText();
            }
        }
        /// <summary>
        /// ����textbox�����б��text
        /// </summary>
        private void ChangeComboxText()
        {
             Exception err=null;
            Dictionary<string, object> dic = mSystable.GetRow("�������ֵ��", "CODE='" + array[3] + "'", out err);
            if (dic != null)
            {
                comboBoxArea.SelectedItem = dic["NAME"] + "(" + dic["CODE"] + ")";
            }
            comboBoxYear.Text = array[1];
        }

        /// <summary>
        /// �����б��
        /// </summary>
        private void LoadCombox()
        {  
            Exception err=null;
            //������������
            List<Dictionary<string,object>> listdic= mSystable.GetRows("�������ֵ��", "XZJB=3", out err);
            if (listdic != null)
            {
                foreach (Dictionary<string, object> dic in listdic)
                {
                    string str = dic["NAME"] + "(" + dic["CODE"] + ")";
                    if (!comboBoxArea.Items.Contains(str))
                        comboBoxArea.Items.Add(str);
                }
            }
            if (comboBoxArea.Items.Count > 0)
                comboBoxArea.SelectedIndex = 0;
            listdic = mSystable.GetRows("��ȴ����", "", out err);
            if (listdic != null)
            {
                foreach (Dictionary<string, object> dic in listdic)
                {
                    string stryear = dic["CODE"].ToString();
                    if (!comboBoxYear.Items.Contains(stryear))
                        comboBoxYear.Items.Add(stryear);
                }
            }
            if (comboBoxYear.Items.Count > 0)
                comboBoxYear.SelectedIndex = 0;
        }

      

        /// <summary>
        /// ͨ������(����)�����ô���
        /// </summary>
        /// <param name="strname">����(����)</param>
        private string GetCode(string strname,out bool right)
        {
            try
            {
                string[] arr = strname.Split('(', ')');
                right = true;
                return arr[1];
            }
            catch
            {
                MessageBox.Show("���������룩��ʽ����ȷ!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                right = false;
                return strname;
            }
        }


        /// <summary>
        /// �����ݷ������ַ�������
        /// </summary>
        /// <param name="filename">��������</param>
        public void AnalyseDataToArray(string filename)
        {
            try
            {
                array[3] = filename.Substring(4, filename.LastIndexOf("_") - 4);
                array[1] = filename.Substring(filename.LastIndexOf("_") + 1);
            }
            catch { }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            bool right=true;
            prename = "";//���ԭ������
            // array[0] = GetCode(comboBoxBus.Text);//ҵ����������DZ
            array[1] = comboBoxYear.Text;//�����2009
            // array[2] = GetCode(comboBoxType.Text);//ר�������01
            array[3] = GetCode(comboBoxArea.Text,out right);//������420683
            if (!right)
                return;
            //array[4] = GetCode(comboBoxScale.Text);//��������G

           Regex regex=new Regex(@"[1-9]\d{3}");
           if (!regex.IsMatch(array[1]))
           {
               MessageBox.Show("��Ȳ��淶!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
               return;
           }
            regex=new Regex(@"[1-9]\d*");
            if (!regex.IsMatch(array[3]))
            {
                MessageBox.Show("�������벻�淶!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            prename = "LYZY" + array[3] + "_" + array[1];

            //����ǰ׺
            tb.Text = prename;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmNameRule_FormClosed(object sender, FormClosedEventArgs e)
        {
            mSystable = null;
        }

       
    }
}