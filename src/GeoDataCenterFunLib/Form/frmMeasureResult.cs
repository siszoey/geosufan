using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;

namespace GeoDataCenterFunLib
{
    public partial class frmMeasureResult : DevComponents.DotNetBar.Office2007Form
    {
        //�ֶ�
        private esriUnits m_Units=esriUnits.esriMeters  ;        //��ǰ��ֵ�ĵ�λ
        private bool m_bShowSum;������   //��־�Ƿ���ʾ�ܺ�

        private double m_SegLength;������  // ��εĳ���
        public double dblSegLength
        {
            set
            {
                if (value < 0)
                    m_SegLength = System.Math.Abs(value);
                else
                    m_SegLength = value;
            }
            get
            {
                return m_SegLength;
            }
        }

        private double m_Length;���������� //�߶��ܳ���
        public double dblLength
        {
            set
            {
                if (value < 0)
                    m_Length = System.Math.Abs(value);
                else
                    m_Length = value;
            }
            get
            {
                return m_Length;
            }
        }

        private double m_Area;            //����ε����
        public double dblArea
        {
            set
            {
                if (value < 0)
                    m_Area = System.Math.Abs(value);
                else
                    m_Area = value;
            }
            get
            {
                return m_Area;
            }
        }
        private double m_SumLength;       //�ܳ���
        public double dblSumLength
        {
            set
            {
                if (value < 0)
                    m_SumLength = System.Math.Abs(value);
                else
                    m_SumLength = value;
            }
            get
            {
                return m_SumLength;
            }
        }
        private double m_SumArea;         //�����
        public double dblSumArea
        {
            set
            {
                if (value < 0)
                    m_SumArea = System.Math.Abs(value);
                else
                    m_SumArea = value;
            }
            get
            {
                return m_SumArea;
            }
        }
        public   short   m_CurMeasureType;  //��־��ǰ����Ķ�������:0Ϊ�ߣ�1Ϊ��
        public bool m_bSnapToFeature;  //��־�Ƿ�׽Ҫ�ؽڵ�
        public bool m_bIsFeatureMeasure;  //��־�Ƿ��Ҫ�ؽ��в���

        //���캯��
        public frmMeasureResult()
        {
            InitializeComponent();
            m_CurMeasureType = 0;
            m_bIsFeatureMeasure = false;
            toolMeasureArea.Checked = false;
            toolMeasureFeature.Checked = false;
            toolMeasureLine.Checked = true; 

            m_bShowSum = false;
            m_bSnapToFeature = false;
            toolShowSum.Checked = false;
            toolSnapToFeature.Checked = false;
        }
        //��õ�λ
        private string GetUnitsName(esriUnits nUnit)
        {
            switch ((int)nUnit)
            {
                case 1:
                    return "Ӣ��";
                case 3:
                    return "Ӣ��";
                case 7:
                    return "����";
                case 8:
                    return "����";
                case 9:
                    return "��";
                case 10:
                    return "����";
                case 12:
                    return "����";
                default:
                    return "δ֪";
            }
        }

        //��ʾ�����������
        public  void ShowResult(double segLen,double len,double  SumLen,double Area,double SumArea,bool IsLine)
        {
            string sResult = "";
            string sUnit = "";
            sUnit = this.GetUnitsName(m_Units);

            if (IsLine==true )
            {
                sResult = "�߶β��������\n��ǰ���γ��ȣ�" + segLen.ToString("0.000") + sUnit
                    + "\n��ǰ�߶γ��ȣ�" + len.ToString("0.000") + sUnit;
                if (this.m_bShowSum == true)
                    sResult += "\n\n�ܳ��ȣ�" + SumLen.ToString("0.000") + sUnit;
            }
            else 
            {
                sResult = "����β��������\n��ǰ���γ��ȣ�" + segLen.ToString("0.000") + sUnit
                            + "\n��ǰ������ܳ���" + len.ToString("0.000") +sUnit
                            + "\n��ǰ����������" + Area.ToString("0.000") + "ƽ��" + sUnit;
                if (this.m_bShowSum == true)
                    sResult += "\n\n���ܳ���" + SumLen.ToString("0.000") + sUnit
                                + "\n�������" + SumArea.ToString("0.000") + "ƽ��" + sUnit;
            }
            this.lblResult.Text = sResult;  

        }

        public void ShowResult(bool IsLine)
        {
            string sResult = "";
            string sUnit = "";
            sUnit = this.GetUnitsName(m_Units);

            if (IsLine==true )
            {
                sResult = "�߶β��������\n��ǰ���γ��ȣ�" + this.dblSegLength.ToString("0.000") + sUnit
                    + "\n��ǰ�߶γ��ȣ�" + this.dblLength.ToString("0.000") + sUnit;
                if (this.m_bShowSum == true)
                    sResult += "\n\n�ܳ��ȣ�" + this.dblSumLength.ToString("0.000") + sUnit;
            }
            else 
            {
                sResult = "����β��������\n��ǰ���γ��ȣ�" + this.dblSegLength.ToString("0.000") + sUnit
                            + "\n��ǰ������ܳ���" + this.dblLength.ToString("0.000") + sUnit
                            + "\n��ǰ����������" + this.dblArea.ToString("0.000") + "ƽ��" + sUnit;
                if (this.m_bShowSum == true)
                    sResult += "\n\n���ܳ���" + this.dblSumLength.ToString("0.000") + sUnit
                                + "\n�������" + this.dblSumArea.ToString("0.000") + "ƽ��" + sUnit;
            }
            this.lblResult.Text = sResult;
        }


        private void toolMeasureLine_Click(object sender, EventArgs e)
        {
            this.m_bIsFeatureMeasure = false;
            this.m_CurMeasureType = 0;
            toolMeasureLine.Checked = true;
            toolMeasureArea.Checked = false;
            toolMeasureFeature.Checked = false;
        }

        private void toolMeasureArea_Click(object sender, EventArgs e)
        {
            this.m_bIsFeatureMeasure = false;
            this.m_CurMeasureType = 1;
            toolMeasureLine.Checked = false;
            toolMeasureArea.Checked = true ;
            toolMeasureFeature.Checked = false;  
        }

        private void toolMeasureFeature_Click(object sender, EventArgs e)
        {
            this.m_bIsFeatureMeasure = true;
            toolMeasureLine.Checked = false;
            toolMeasureArea.Checked = false ;
            toolMeasureFeature.Checked = true;  
        }

        private void toolSnapToFeature_Click(object sender, EventArgs e)
        {
            if (this.m_bSnapToFeature == false)            
                this.m_bSnapToFeature = true; 
            else
                this.m_bSnapToFeature = false;
            toolSnapToFeature.Checked = m_bSnapToFeature;
        }

        private void toolShowSum_Click(object sender, EventArgs e)
        {
            if (this.m_bShowSum == false)
                this.m_bShowSum = true;
            else
                this.m_bShowSum = false;
            toolShowSum.Checked = m_bShowSum; 
        }

        private void toolClear_Click(object sender, EventArgs e)
        {
            this .lblResult.Text =""; 
            this.m_SumArea = 0;
            this.m_SumLength = 0;
        }

        //���ò��������ť�Ƿ����
        public void IsCanMeasureArea(bool bCan)
        {
            toolMeasureArea.Enabled = bCan;
        }

        #region ��λ����

        private void toolKM_Click(object sender, EventArgs e)
        {
            this.m_Units = esriUnits.esriKilometers;
            toolKM.Checked = true;
            toolM.Checked = false;
            toolCM.Checked = false;
            toolMM.Checked = false;
        }

        private void toolM_Click(object sender, EventArgs e)
        {
            this.m_Units = esriUnits.esriMeters;
            toolKM.Checked = false;
            toolM.Checked = true;
            toolCM.Checked = false;
            toolMM.Checked = false;
        }        

        private void toolCM_Click(object sender, EventArgs e)
        {
            this.m_Units = esriUnits.esriCentimeters;
            toolKM.Checked = false;
            toolM.Checked = false;
            toolCM.Checked = true;
            toolMM.Checked = false;
        }
        

        private void toolMM_Click(object sender, EventArgs e)
        {
            this.m_Units = esriUnits.esriMillimeters;
            toolKM.Checked = false;
            toolM.Checked = false;
            toolCM.Checked = false;
            toolMM.Checked = true;
        }

        #endregion
    }
}