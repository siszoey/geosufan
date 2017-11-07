using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GeoDataCenterFunLib
{
    //=============================
    //���ߣ�ϯʤ
    //ʱ�䣺2011-2-22
    //���ӽ�������ʼ�������÷���
    //=============================
    public class GeoProgressBar
    {
        //���ý���������Ϊȫ�ֱ������Ȼ�����״̬ʱ����
        ProgressBarStatDlg prgfm;
       //��״̬��ʾ�Ľ���������
        public void ProgressBar_Start(Form fm)
        {
            ProgressBarDlg prgfm = new ProgressBarDlg();
            prgfm.Show(fm);
        }
       
        //��״̬��ʾ�Ľ������ر�
        public void ProgressBar_End(Form fm)
        {
            fm.OwnedForms[0].Close();
            //Form.ActiveForm.Close();
        }

        //��״̬��ʾ�Ľ���������
        public void ProgressBarStat_Start(Form fm)
        {
            prgfm = new ProgressBarStatDlg();
            prgfm.Show(fm);
        }


        //��״̬��ʾ�Ľ������ر�
        public void ProgressBarStat_End(Form fm)
        {
            fm.OwnedForms[0].Close();
            //Form.ActiveForm.Close();
        }

        //���ý���״ֵ̬
        public void SetState(string value)
        {
           
            if (Form.ActiveForm.Equals(prgfm))
                prgfm.Value = value;

            
           
        }
    }

}
