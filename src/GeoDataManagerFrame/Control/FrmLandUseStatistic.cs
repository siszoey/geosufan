using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using SysCommon.Gis;
using ESRI.ArcGIS.Geodatabase;
using System.Xml;
namespace GeoDataManagerFrame
{
    public partial class FrmLandUseStatistic : DevComponents.DotNetBar.Office2007Form
    {
        Dictionary<string, string> DicXZQ = new Dictionary<string, string>();
        public string _XZQcode = "";
        public string _XZQmc = "";
        public string _Year = "";
        public string _AreaUnit = "";
        private string _LayerTreeXmlPath = "";
        public int _FractionNum = 2;
        public string _ResultPath = "";
        public FrmLandUseStatistic()
        {
            InitializeComponent();
        }
        public FrmLandUseStatistic(string XmlPath)
        {
            InitializeComponent();
            _LayerTreeXmlPath = XmlPath;
        }

        private void buttonXOK_Click(object sender, EventArgs e)
        {
            if (cmbXZQ.Text.Equals(""))
            {
                MessageBox.Show("��ѡ��ͳ������");
                return;
            }
            if(cmbAreaUnit.Text.Equals(""))
            {
                MessageBox.Show("��ѡ�������λ��");
                return;
            }
            if (cmbYear.Text.Equals(""))
            {
                MessageBox.Show("��ѡ��ͳ����ȣ�");
                return;
            }
            ModStatReport._Statistic_XZQ = cmbXZQ.Text;
            ModStatReport._Statistic_AreaUnit = cmbAreaUnit.Text;
            ModStatReport._Statistic_Year = cmbYear.Text;
            ModStatReport._Statistic_Fractionnum = txtFractionNum.Text;
            ModStatReport._ResultPath_LandUse = textBoxResultPath.Text;
            _AreaUnit = cmbAreaUnit.Text;
            _XZQmc = cmbXZQ.Text;
            _XZQcode = DicXZQ[_XZQmc];

            _Year = cmbYear.Text;
            _FractionNum = int.Parse(txtFractionNum.Text);
            //if (textBoxResultPath.Text.Equals("Ĭ��·��"))
            //{
            //    _ResultPath = Application.StartupPath + @"\..\OutputResults\ͳ�Ƴɹ�\LandUseStatistic";
            //}
            //else
            //{
                _ResultPath = textBoxResultPath.Text;
            //}
            this.DialogResult = DialogResult.OK;
        }

        private void buttonXQuit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void FrmImpLandUseReport_Load(object sender, EventArgs e)
        {
            textBoxResultPath.Text = ModStatReport._ResultPath_LandUse;
            txtFractionNum.Text = ModStatReport._Statistic_Fractionnum;
            //ͳ�Ƶ������λ ��ʼ��
            cmbAreaUnit.Items.Add("ƽ����");
            cmbAreaUnit.Items.Add("Ķ");
            cmbAreaUnit.Items.Add("����");
            cmbAreaUnit.SelectedIndex = 0;
            if (ModStatReport._Statistic_AreaUnit != "")
            {
                for (int i = 0; i < cmbAreaUnit.Items.Count; i++)
                {
                    if (cmbAreaUnit.Items[i].ToString() == ModStatReport._Statistic_AreaUnit)
                    {
                        cmbAreaUnit.SelectedIndex = i;
                        break;
                    }
                }
            }
            //ͳ�������ʼ��
//            140202--��ͬ����
//140203--��ͬ����
//140211--��ͬ���Ͻ���
//140212--��ͬ��������
//140221--������
//140222--������
//140223--������
//140224--������
//140225--��Դ��
//140226--������
//140227--��ͬ��


            cmbXZQ.Items.Add("��ͬ����"); DicXZQ.Add("��ͬ����", "140202");
            cmbXZQ.Items.Add("��ͬ����"); DicXZQ.Add("��ͬ����", "140203");
            cmbXZQ.Items.Add("��ͬ���Ͻ���"); DicXZQ.Add("��ͬ���Ͻ���", "140211");
            cmbXZQ.Items.Add("��ͬ��������"); DicXZQ.Add("��ͬ��������", "140212");
            cmbXZQ.Items.Add("������"); DicXZQ.Add("������", "140221");
            cmbXZQ.Items.Add("������"); DicXZQ.Add("������", "140222");
            cmbXZQ.Items.Add("������"); DicXZQ.Add("������", "140223");
            cmbXZQ.Items.Add("������"); DicXZQ.Add("������", "140224");
            cmbXZQ.Items.Add("��Դ��"); DicXZQ.Add("��Դ��", "140225");
            cmbXZQ.Items.Add("������"); DicXZQ.Add("������", "140226");
            cmbXZQ.Items.Add("��ͬ��"); DicXZQ.Add("��ͬ��", "140227");
            cmbXZQ.SelectedIndex = 1;
            if (ModStatReport._Statistic_XZQ != "")
            {
                for (int i = 0; i < cmbXZQ.Items.Count; i++)
                {
                    if (cmbXZQ.Items[i].ToString() == ModStatReport._Statistic_XZQ)
                    {
                        cmbXZQ.SelectedIndex = i;
                        break;
                    }
                }
            }
            cmbYear.Items.Add("2009");
            cmbYear.Items.Add("2010");
            cmbYear.Items.Add("2011");
            cmbYear.Items.Add("2012");
            //DicXZQ.Add("�ع���", "4402");
            //DicXZQ.Add("�ʻ���", "440224");
            //DicXZQ.Add("������", "440282");
            //DicXZQ.Add("�佭��", "440203");
            //DicXZQ.Add("ʼ����", "440222");
            //DicXZQ.Add("�·���", "440233");
            //DicXZQ.Add("䥽���", "440204");
            //DicXZQ.Add("��Դ��", "440229");
            //DicXZQ.Add("�ֲ���", "440281");
            //DicXZQ.Add("������", "440205");
            //DicXZQ.Add("��Դ����������", "440232");

            //DicXZQ.Add("4402","�ع���");
            //DicXZQ.Add("440224","�ʻ���");
            //DicXZQ.Add("440282","������");
            //DicXZQ.Add("440203","�佭��");
            //DicXZQ.Add("440222","ʼ����");
            //DicXZQ.Add("440233","�·���");
            //DicXZQ.Add("440204","䥽���");
            //DicXZQ.Add("440229","��Դ��");
            //DicXZQ.Add("440281","�ֲ���");
            //DicXZQ.Add("440205","������");
            //DicXZQ.Add("440232","��Դ����������");
            //if (File.Exists(_LayerTreeXmlPath))
            //{
            //    XmlDocument pXmlDoc = new XmlDocument();
            //    pXmlDoc.Load(_LayerTreeXmlPath);
            //    XmlNodeList pDIRList = pXmlDoc.SelectNodes("//DIR [@DIRType='TDLY']");
            //    if (pDIRList != null)
            //    {
            //        foreach (XmlNode pDIRNode in pDIRList)
            //        {
            //            XmlElement pDIREle = pDIRNode as XmlElement;
            //            if (pDIREle != null)
            //            {
            //                if (pDIREle.HasAttribute("Year"))
            //                {
            //                    string strYear=pDIREle.GetAttribute("Year");
            //                    try
            //                    {
            //                        int iYear = int.Parse(strYear);
            //                        if (iYear > 2008)
            //                        {
            //                            if (!cmbYear.Items.Contains(strYear))
            //                            {
            //                                cmbYear.Items.Add(strYear);
            //                            }
            //                        }
            //                    }
            //                    catch(Exception err)
            //                    {}
            //                }
            //                //if (pDIREle.HasAttribute("XZQCode"))
            //                //{
            //                //    string strXZQcode = pDIREle.GetAttribute("XZQCode");
            //                //    string strXZQName = DicXZQ[strXZQcode];
            //                //    if (!cmbXZQ.Items.Contains(strXZQName))
            //                //    {
            //                //        cmbXZQ.Items.Add(strXZQName);
            //                //    }
            //                //}
            //            }
            //        }
            //    }
            //    pXmlDoc = null;

            //}
                if (cmbYear.Items.Count > 0)
                {
                    cmbYear.SelectedIndex = 0;
                }
                if (ModStatReport._Statistic_Year != "")
                {
                    for (int i = 0; i < cmbYear.Items.Count; i++)
                    {
                        if (cmbYear.Items[i].ToString() == ModStatReport._Statistic_Year)
                        {
                            cmbYear.SelectedIndex = i;
                            break;
                        }
                    }
                }
        }

        private void txtFractionNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            string strnum = "0123456789";
            if (!char.IsControl(e.KeyChar) && (!strnum.Contains(e.KeyChar.ToString())))
            {
                e.Handled = true;
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog pFolderBrowser = new FolderBrowserDialog();
            if (pFolderBrowser.ShowDialog() == DialogResult.OK)
            {
                textBoxResultPath.Text = pFolderBrowser.SelectedPath;
                pFolderBrowser = null;
            }
        }
    }
}