using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;

namespace GeoDataManagerFrame
{
    public partial class frmMapProperty :  DevComponents.DotNetBar.Office2007Form
    {
        public frmMapProperty()
        {
            InitializeComponent();
        }
        TreeNode thisNode;
        private void frmMapProperty_Load(object sender, EventArgs e)
        {

            //��ʾͼ������
            labNewname.Text = thisNode.Parent.Parent.Text + thisNode.Parent.Text + thisNode.Text;

            //��ȡר������
            string strSubType = thisNode.Tag.ToString();//ר�����ʹ���
            labNewType.Text = thisNode.Parent.Text;

            //�����������ݿ��ַ���
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  
            //��ȡ���
            string strYear = thisNode.Text.Substring(0, 4);
            labNewYear.Text = strYear;

            //��ȡ�����߼������
            GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
            int iStartPos = thisNode.Text.IndexOf("��");
            int iEndPos = thisNode.Text.IndexOf("��");
            int iLength = iEndPos - iStartPos - 1;
            string strScaleName = thisNode.Text.Substring(iStartPos + 1, iLength);
            labNewScale.Text = strScaleName;
            
            string strExp = "select ���� from �����ߴ���� where ���� ='" + strScaleName + "'";
            string strScaleCode = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //��ȡ��������
            labNewDivision.Text = thisNode.Parent.Parent.Text;

            //�ӵ�ͼ�����Ϣ���л�ȡ�����������Ϣ��ͼ����ɣ�
            strExp = "select ͼ����� from ��ͼ�����Ϣ�� where �������� ='" + thisNode.Parent.Parent.Text + "'" + "And " + " ���='" +
              strYear + "'" + "And " + " ������='" + strScaleCode + "'" + "And " + " ר������='" + strSubType + "'";
            string strLayerGroup = dDbFun.GetInfoFromMdbByExp(strCon, strExp);
            string[] array = strLayerGroup.Split("/".ToCharArray());
            for (int i = 0; i < array.Length; i++)
            {
                strExp = "select ���� from ��׼ͼ����Ϣ�� where ����='" + array[i] + "'";
                string strBussinessName = dDbFun.GetInfoFromMdbByExp(strCon, strExp);
                listLayer.Items.Add(strBussinessName);
            }




        }

        public TreeNode ThisNode
        {
            get { return thisNode; }
            set { thisNode = value; }
        }
    }
}