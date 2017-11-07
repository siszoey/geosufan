using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;
using System.Xml;
using System.Data.OleDb;

namespace GeoDBConfigFrame
{
    public partial class AddLayerForm : DevComponents.DotNetBar.Office2007Form
    {
        public string m_strSubName;
        public string m_strLayerDescri;
        public string m_strLayerName;
        public string m_strCurSubName;
        public XmlDocument m_xmldoc;
        public string m_strScale;//������
        public string m_strTransp;//͸����

        /// <summary>
        /// ���ͼ�㹹�캯��
        /// </summary>
        /// <param name="xmldoc">xml�ĵ�</param>
        /// <param name="strCurName">��ڵ��ı�</param>
        public AddLayerForm(XmlDocument xmldoc,string strCurName)
        {
            InitializeComponent();

            m_xmldoc = xmldoc;
            m_strCurSubName = strCurName;
            textBoxGroupType.Visible = false;

            //��ʼ�� ������ڵ�
            InitGroupText();
            //��ʼ��ͼ������
            InitComBoxName();  

            //��ʼ�� ������
            InitScaleItem();
            comboBoxScale.SelectedIndex = 0;
        }

        /// <summary>
        /// �༭ͼ�㹹�캯��
        /// </summary>
        /// <param name="xmldoc">xml�ĵ�</param>
        /// <param name="node">Ҷ�ӽڵ�</param>
        public AddLayerForm(XmlDocument xmldoc, TreeNode node)
        {

            InitializeComponent();
            m_xmldoc = xmldoc;
            m_strCurSubName = node.Parent.Text;
            textBoxGroupType.Text = m_strCurSubName;
            textBoxGroupType.ReadOnly = true;

            ////��ʼ�� ������ڵ�
            //InitGroupText();
            InitComBoxName();
            groupType.Visible = false;

            //��ʼ�� ������
            InitScaleItem();
            ComboBoxName.Text = node.Name;
            textBoxDescri.Text = node.Text;
            string strSearch = "//Layer[@sItemName='"+node.Name+"']";
            XmlNode xmlNode = m_xmldoc.SelectSingleNode(strSearch);
            if (xmlNode != null)
            {
                if ((xmlNode as XmlElement).GetAttribute("sDispScale") != "")
                {
                    comboBoxScale.Text = (xmlNode as XmlElement).GetAttribute("sDispScale");
                }
                else
                {
                    comboBoxScale.SelectedIndex = 0;
                }
                if ((xmlNode as XmlElement).GetAttribute("sDiaphaneity") != "")
                {
                    numericTranspar.Value = Convert.ToDecimal((xmlNode as XmlElement).GetAttribute("sDiaphaneity"));
                }
            } 
         }

        private void InitComBoxName()
        {
           ComboBoxName.Items.Clear();
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string path = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select ���� from ��׼ͼ����Ϣ��";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            List<string> list = db.GetDataReaderFromMdb(strCon, strExp);
            for (int i = 0; i < list.Count; i++)
            {
                ComboBoxName.Items.Add(list[i]);
            }
            if (list.Count > 0)
            {
                ComboBoxName.SelectedIndex = 0;
            }
        }
        public void InitGroupText()
        {
            try
            {
                //��ȡ��ǰ���ڵ������������
                if (m_xmldoc != null)
                {
                    int iCurIndex = 0;
                    int iIndex = 0;
                    string strSubName;
                    string strSearchRoot = "//SubGroup";
                    XmlNodeList nodeList = m_xmldoc.SelectNodes(strSearchRoot);
                    if (nodeList != null && nodeList.Count > 0)
                    {
                        foreach (XmlNode node in nodeList)
                        {
                            strSubName = (node as XmlElement).GetAttribute("sItemName");
                            iIndex = groupType.Items.Add(strSubName);
                            if (strSubName.Equals(m_strCurSubName))
                            {
                                iCurIndex = iIndex;
                            }
                        }
                    }
                    groupType.SelectedIndex = iCurIndex;

                }
            }
            catch
            { }
        }

        public void InitScaleItem()
        {
            object[] scaleItems = new object[14];
            scaleItems[0] = "";
            scaleItems[1] = "1:500";
            scaleItems[2] = "1:1000";
            scaleItems[3] = "1:10000";
            scaleItems[4] = "1:50000";
            scaleItems[5] = "1:100000";
            scaleItems[6] = "1:250000";
            scaleItems[7] = "1:500000";
            scaleItems[8] = "1:750000";
            scaleItems[9] = "1:1000000";
            scaleItems[10] = "1:2500000";
            scaleItems[11] = "1:5000000";
            scaleItems[12] = "1:7500000";
            scaleItems[13] = "1:10000000";
            comboBoxScale.Items.AddRange(scaleItems);
            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_strSubName = groupType.Text.Trim();
            m_strLayerName = ComboBoxName.Text.Trim();
            m_strLayerDescri = textBoxDescri.Text.Trim();
            m_strScale = comboBoxScale.Text;
            m_strTransp =numericTranspar.Value.ToString();
            if (m_strLayerName.Equals(""))
            {
                MessageBox.Show("������ͼ������!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (m_strLayerDescri.Equals(""))
            {
                MessageBox.Show("������ͼ������!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
         
            this.DialogResult = DialogResult.OK;
            this.Hide();
            this.Dispose(true);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
            this.Dispose(true);
        }

        //��������ʱ��Ӧ
        private void trackBar_Scroll(object sender, EventArgs e)
        {
            numericTranspar.Value =Convert.ToDecimal(trackBar.Value);
        }

   
        private void ComboBoxName_TextChanged(object sender, EventArgs e)
        {
            textBoxDescri.Text = ComboBoxName.Text;
        }
    }
}