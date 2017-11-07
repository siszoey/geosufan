//*********************************************************************************
//** �ļ�����FormFlexcell.cs
//** CopyRight (c) 2000-2007 �人������Ϣ���̼������޹�˾���̲�
//** �����ˣ�chulili
//** ��  �ڣ�2011-03
//** �޸��ˣ�
//** ��  �ڣ�
//** ��  ��������ͼ����Ż� 
//**
//** ��  ����1.0
//*********************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


using System.Data.OleDb ;

namespace GeoDataCenterFunLib
{
    public partial class FormFlexcell : DevComponents.DotNetBar.Office2007Form
    {
        
        private string mstrFile;
        private string m_DefaultFile;
        private string[,] marrCellText=new string [100000,150];

        public FormFlexcell()
        {   
            InitializeComponent();
        }
        public AxFlexCell.AxGrid GetGrid()
        {
            return axGrid1;
        }
        //��ȡĳ��Ԫ�������
        private void axGrid1_GetCellText(object sender, AxFlexCell.__Grid_GetCellTextEvent e)
        {
            if ((e.row >= ModFlexcell.m_startRow && e.col >= ModFlexcell.m_startCol) || e.row == ModFlexcell.m_SpecialRow || e.row == ModFlexcell.m_SpecialRow_ex || e.row == ModFlexcell.m_SpecialRow_ex2)
            {
                e.text = marrCellText[e.row, e.col];
                e.changed = true; //ʹ�������
            }
        }
        //����ĳ��Ԫ�������
        private void axGrid1_SetCellText(object sender, AxFlexCell.__Grid_SetCellTextEvent e)
        {
            if(e.row>0 && e.col>0)
            {
                marrCellText[e.row,e.col]=e.text;
                e.cancel=true;
            }

        }
        //��յ�Ԫ������
        public void ClearAll(long rowstart,long colstart,long rowend,long colend)
        {
            long i,j;
            for(i=rowstart;i<=rowend;i++)
            {
                for (j=colstart;j<=colend;j++)
                {
                    marrCellText[i,j]="";
                }

            }

        }

        private void FormFlexcell_Load(object sender, EventArgs e)
        {
            mstrFile = "";
            m_DefaultFile = "";
            axGrid1.Height =this.Height-buttonXtoExcel.Height-60;
            axGrid1.Top=buttonXtoExcel.Top+buttonXtoExcel.Height+15;
            axGrid1.Left=buttonXSave.Left ;
            axGrid1.Width=this.Width-30;

        }
        //���水ť
        private void buttonXSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "FlexCell Document (*.cel)|*.cel";
            dlg.FileName = m_DefaultFile;
            string oldpath;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                oldpath = dlg.FileName;
                if (System.IO.File.Exists(oldpath) == true)
                {
                    if (MessageBox.Show("�ļ�'"+oldpath+"'�Ѿ����ڣ�Ҫ��������","ѯ��",MessageBoxButtons.YesNo)==DialogResult.Yes )
                    {
                        axGrid1.SaveFile(oldpath);
                    }
                }
                axGrid1.SaveFile(oldpath);
                MessageBox.Show("����ɹ���");
            }
            else
            {
            }


        }
        public void SaveFile(string path)
        {
            try 
            { 
                axGrid1.SaveFile(path);
                m_DefaultFile = path;
            }
            catch
            {
            }
        }
        //���Ϊ��ť
        private void buttonXSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "FlexCell Document (*.cel)|*.cel";
            string oldpath;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                oldpath = dlg.FileName;
                if (System.IO.File.Exists(oldpath) == true)
                {
                    if (MessageBox.Show("�ļ�'" + oldpath + "'�Ѿ����ڣ�Ҫ��������", "ѯ��", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        axGrid1.SaveFile(oldpath);
                    }
                }
                axGrid1.SaveFile(oldpath);
                MessageBox.Show("����ɹ���");
            }
            else
            {
            }
        }
        //�������ܣ�ת����Excel��ʽ
        private void buttonXtoExcel_Click(object sender, EventArgs e)
        {
            
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Excel WorkBook (*.xls)|*.xls";
            string oldpath;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                oldpath = dlg.FileName;
                if (System.IO.File.Exists(oldpath) == true)
                {
                    if (MessageBox.Show("�ļ�'" + oldpath + "'�Ѿ����ڣ�Ҫ��������", "ѯ��", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        //ֱ�ӵ���flexcell����ExportToExcel����ת����excel��ʽ�ļ������������excel�ļ�ȫ·��
                        axGrid1.ExportToExcel(oldpath);
                    }
                }
                axGrid1.ExportToExcel(oldpath);
                MessageBox.Show("�����ɹ���");
            }
            else
            {
            }

        }
        //�������ܣ�����Reader�������ñ������
        //�����������ʼ�У���ʼ�У�Reader��ģ��ȫ·�������������壬�ֺ�
        public  bool SetTextFromRS(int startrow,int startcol,OleDbDataReader dr,string TemplateFile,int recordcount,string sFontName,int iFontSize)
        {
            if (dr.FieldCount <= 0)
                return false;
            if (TemplateFile.Equals(""))
                return false;
            //�ȴ�ģ���ļ�
            axGrid1.OpenFile(TemplateFile);
            int rowcount, colcount;
            rowcount = recordcount;
            colcount = dr.FieldCount;
            //���ñ������������
            axGrid1.Rows = startrow + rowcount;
            axGrid1.Cols = startcol + colcount+1;
            //����������ֺ�
            axGrid1.Range(startrow, startcol, axGrid1.Rows - 1, axGrid1.Cols - 1).FontName =sFontName;
            axGrid1.Range(startrow, startcol, axGrid1.Rows - 1, axGrid1.Cols - 1).FontSize = iFontSize;

            m_DefaultFile = TemplateFile;
            
            axGrid1.Refresh();
            //�ر��Զ�ˢ��
            axGrid1.AutoRedraw = false;
            
            int i, j;
            //����Reader�������ñ������
            for (i = startrow ; i <= startrow + rowcount - 1; i++)
            {
                if (dr.Read())
                for (j = startcol; j <= startcol + colcount - 1; j++)
                { 
                    //�˴����õ������
                    marrCellText[i, j] = dr.GetValue(j - startcol).ToString ();
                }
            }
            //�����Զ�ˢ��
            axGrid1.AutoRedraw=true;
            axGrid1.Refresh();
            return true;
        }

        private void FormFlexcell_Resize(object sender, EventArgs e)
        {
            axGrid1.Height = this.Height - buttonXtoExcel.Height - 60;
            axGrid1.Top = buttonXtoExcel.Top + buttonXtoExcel.Height + 15;
            axGrid1.Left = buttonXSave.Left;
            axGrid1.Width = this.Width - 30;
        }
        //�������ܣ�ֱ�Ӵ�һ��flexcell�ļ�
        //���������flexcell�ļ�ȫ·��  �����������
        public void OpenFlexcellFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                ModFlexcell.m_startCol = 100000;
                ModFlexcell.m_startRow = 50;
                ModFlexcell.m_SpecialRow = -1;
                ModFlexcell.m_SpecialRow_ex = -1;
                ModFlexcell.m_SpecialRow_ex2 = -1;
                axGrid1.OpenFile(path);
                axGrid1.Refresh();
            }
        }
    }
}