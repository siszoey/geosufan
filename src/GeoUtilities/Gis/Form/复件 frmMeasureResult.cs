using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using SysCommon.Error;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;

namespace GeoUtilities
{
    public partial class frmMeasureResult : DevComponents.DotNetBar.Office2007Form
    {
        //�ֶ�
        private esriUnits m_Units=esriUnits.esriMeters ;        //��ǰ��ֵ�ĵ�λ
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
                 if (m_Units == esriUnits.esriKilometers)
                     m_Area = m_Area /1000000;
              
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
        private double m_SumArea=0;         //�����
        public double dblSumArea
        {
            set
            {
                //if (m_Units == esriUnits.esriKilometers)
                //    value = value / 1000000;
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
        public   short   m_CurMeasureType;  //��־��ǰ����Ķ�������:0Ϊ�ߣ�1Ϊ��;2Ϊ�Ƕ�
        public bool m_bSnapToFeature;  //��־�Ƿ�׽Ҫ�ؽڵ�
        public bool m_bIsFeatureMeasure;  //��־�Ƿ��Ҫ�ؽ��в���

        /////guozhen 2011-5-8 added ʵ�ʵ�ͼ����
        private esriUnits m_Displayunit = esriUnits.esriUnknownUnits;
        ControlsMapMeasureToolDefClass pTool;

        public double ang = 0;//yjl20110816 add �ǶȲ����ĽǶ�

        //���캯��
        public frmMeasureResult( esriUnits in_DisplayUnit,ControlsMapMeasureToolDefClass inTool)
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
            toolM.Checked = true;//ѡ��M
            toolKM.Checked = false;
           // toolM.ReadOnly = true;
            //toolSquareM.ReadOnly = true;
            /////
            this.m_Displayunit = in_DisplayUnit;
            if (in_DisplayUnit == esriUnits.esriUnknownUnits)//��ͼ��λ
            {
                m_Displayunit = esriUnits.esriMeters;
            }
            pTool = inTool;
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
                    return "ǧ��";
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
            ////////////////////�ж����굥λ�Ƿ�һ��////////////////////////
            double dSum = SumLen;
            double bCurrentlen = segLen;
            double dlen = len;
            if (this.m_Displayunit != esriUnits.esriUnknownUnits)
            {
                if (this.m_Displayunit != this.m_Units)
                {
                    dSum = UnitConvert(dSum);
                    bCurrentlen = UnitConvert(bCurrentlen);
                    dlen = UnitConvert(dlen);
                    //Area = UnitConvert(Area);
                    //SumArea = UnitConvert(SumArea);
                }
            }
            ////////////////////////////////////////////////////////////////
            if (IsLine==true )
            {
                sResult = "�߶β��������\n��ǰ���γ��ȣ�" + bCurrentlen + sUnit
                    + "\n��ǰ�߶γ��ȣ�" + dlen + sUnit;
                if (this.m_bShowSum == true)
                    sResult += "\n\n�ܳ��ȣ�" + dSum + sUnit;
            }
            else 
            {
                sResult = "����β��������\n��ǰ���γ��ȣ�" + bCurrentlen + sUnit
                            + "\n��ǰ������ܳ���" + dlen + sUnit
                            + "\n��ǰ����������" + Area  + "ƽ��" + sUnit;
                if (this.m_bShowSum == true)
                    sResult += "\n\n���ܳ���" + dSum + sUnit
                                + "\n�������" + SumArea + "ƽ��" + sUnit;
            }
            this.lblResult.Text = sResult;  

        }
        //��ʾ�����������yjl20110816 add
        public void ShowResult()
        {

            this.lblResult.Text = "��X����нǣ�" + ang.ToString("f2") + "��";

        }
        //yjl20110816 modify.1��2��3�Ƕ�
        public void ShowResult(int meaType)
        {
            string sResult = "";
            string sUnit = "";
            sUnit = this.GetUnitsName(m_Units);
            ////////////////////�ж����굥λ�Ƿ�һ��////////////////////////
            double dSum = this.dblSumLength;
            double bCurrentlen = this.dblSegLength;
            double dlen = this.dblLength;
            if (this.m_Displayunit != esriUnits.esriUnknownUnits)
            {
                if (this.m_Displayunit != this.m_Units)
                {
                    dSum = UnitConvert(dSum);
                    bCurrentlen = UnitConvert(bCurrentlen);
                    dlen = UnitConvert(dlen);

                    //��������仯 xisheng 20111117**************
                    if (dblSumArea > 0)
                    {
                        if (m_Units == esriUnits.esriKilometers)
                        {
                            dblSumArea = dblSumArea / 1000000;
                            m_Area = m_Area / 1000000;
                        }
                        else if (m_Units == esriUnits.esriMeters)
                        {
                            dblSumArea = dblSumArea * 1000000;
                            m_Area = m_Area * 1000000;
                        }
                    }
                    //��������仯 xisheng 20111117***********end
                }
            }
            ////////////////////////////////////////////////////////////////

            if (meaType == 1)
            {
                sResult = "�߶β��������\n��ǰ���γ��ȣ�" + bCurrentlen + sUnit
                    + "\n��ǰ�߶γ��ȣ�" + dlen + sUnit;
                if (this.m_bShowSum == true)
                    sResult += "\n\n�ܳ��ȣ�" + dSum + sUnit;
            }
            else if (meaType == 2)
            {
                sResult = "����β��������\n��ǰ���γ��ȣ�" + bCurrentlen + sUnit
                            + "\n��ǰ������ܳ���" + dlen + sUnit
                            + "\n��ǰ����������" + this.m_Area + "ƽ��" + sUnit;
                if (this.m_bShowSum == true)
                    sResult += "\n\n���ܳ���" + dSum + sUnit
                                + "\n�������" + this.dblSumArea + "ƽ��" + sUnit;
            }
            else if (meaType==3)
            {
                sResult = "��X����нǣ�" + ang.ToString("f2") + "��";
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
            toolMeasureAngle.Checked = false;//yjl20110816
        }

        private void toolMeasureFeature_Click(object sender, EventArgs e)
        {
            this.m_bIsFeatureMeasure = true;
            toolMeasureLine.Checked = false;
            toolMeasureArea.Checked = false ;
            toolMeasureFeature.Checked = true;
            toolMeasureAngle.Checked = false;//yjl20110816
        }

        private void toolSnapToFeature_Click(object sender, EventArgs e)
        {
            if (this.m_bSnapToFeature == false)
            {
                if (pTool.CacheCount > 2000)
                {
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", "Ҫ�ػ��������������ޡ�����С������ͼ�ķ�Χ�򽫲���Ҫ��ͼ��رա�");
                    return;
                }
                this.m_bSnapToFeature = true;
            }
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
			this.dblSegLength = 0;
            this.dblLength = 0;
        }

        //���ò��������ť�Ƿ����
        public void IsCanMeasureArea(bool bCan)
        {
            toolMeasureArea.Enabled = bCan;
        }

        #region ��λ����


        private void toolM_Click(object sender, EventArgs e)
        {
            //������λʱ��ǰ������ű仯 xisheng 20111117************
            if (m_Units == esriUnits.esriKilometers)
            {
                this.m_Units = esriUnits.esriMeters;
                if (toolMeasureLine.Checked) ShowResult(1);
                else if (toolMeasureArea.Checked) ShowResult(2);
            }
            //������λʱ��ǰ������ű仯 xisheng 20111117********end
            toolKM.Checked = false; toolM.Checked = true;
        }

        

        //private void toolCM_Click(object sender, EventArgs e)
        //{
        //    this.m_Units = esriUnits.esriCentimeters; 
        //}
        

        //private void toolMM_Click(object sender, EventArgs e)
        //{
        //    this.m_Units = esriUnits.esriMillimeters;
        //}

        #endregion

        /// <summary>
        /// guozheng 2011-5-8 added ���������굥λת��Ϊʵ��Ҫ��ĵ�λ
        /// </summary>
        /// <param name="in_Len">������Ҫת������ֵ</param>
        /// <returns>�����ת�������ֵ���ڲ��쳣����-1����¼�쳣��־</returns>
        private double UnitConvert(double in_Len)
        {
            IUnitConverter pUnitConvert = new UnitConverterClass();
            try
            {
                double Res = pUnitConvert.ConvertUnits(in_Len, this.m_Displayunit, this.m_Units);
                return Res;
            }
            catch (Exception eError)
            {
                if (SysCommon.Log.Module.SysLog == null) SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
                return -1;
            }
        }

        private void toolKM_Click_1(object sender, EventArgs e)
        {
            //������λʱ��ǰ������ű仯 xisheng 20111117************
            if (m_Units == esriUnits.esriMeters)
            {
                this.m_Units = esriUnits.esriKilometers;

                if (toolMeasureLine.Checked) ShowResult(1);
                else if (toolMeasureArea.Checked) ShowResult(2);
    
            }
            //������λʱ��ǰ������ű仯 xisheng 20111117*********end
            toolKM.Checked = true; toolM.Checked = false;
        }

        private void frmMeasureResult_Load(object sender, EventArgs e)
        {
           
        }
        //yjl20110816 add
        private void toolMeasureAngle_Click(object sender, EventArgs e)
        {
            this.m_bIsFeatureMeasure = false;
            this.m_CurMeasureType = 2;
            toolMeasureLine.Checked = false;
            toolMeasureArea.Checked = false;
            toolMeasureFeature.Checked = false;
            toolMeasureAngle.Checked = true;
        }
    }
}