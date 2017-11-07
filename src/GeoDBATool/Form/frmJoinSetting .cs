using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace GeoDBATool
{
    public partial class frmJoinSetting : DevComponents.DotNetBar.Office2007Form
    {


        public frmJoinSetting()
        {
            InitializeComponent();
            this.com_jointype.Items.Add("��׼ͼ��");
            this.com_jointype.Items.Add("�Ǳ�׼ͼ��");
            this.com_MergeAtrSet.Items.Add("��ӵ�ԴҪ��");
            this.com_MergeAtrSet.Items.Add("����ԴҪ��");
        }

        private void frmConSet_Load(object sender, EventArgs e)
        {
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(ModData.v_JoinSettingXML);
            if (null == XmlDoc)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ�ӱ߲��������ļ�ʧ�ܣ�");
                return;
            }
            XmlElement ele = XmlDoc.SelectSingleNode(".//�ӱ�����") as XmlElement;
            string sDisTo = ele.GetAttribute("�����ݲ�");
            string sSeacherTo = ele.GetAttribute("�����ݲ�");
            string sAngleTo = ele.GetAttribute("�Ƕ��ݲ�");
            string sLengthTo = ele.GetAttribute("�����ݲ�");
            string sjoinType = ele.GetAttribute("�ӱ�����");
            string sIsRemovePnt = ele.GetAttribute("ɾ������ζ����").Trim();
            string sIsSimplify = ele.GetAttribute("�򵥻�Ҫ��").Trim();

            ele = XmlDoc.SelectSingleNode(".//�ں�����") as XmlElement;
            string sMergeAtrSet = ele.GetAttribute("���Ը���").Trim();
            ele = XmlDoc.SelectSingleNode(".//��־����") as XmlElement;
            string sLogPath = ele.GetAttribute("��־·��").Trim();
            double dDisTo = -1;
            double dSeacherTo = -1;
            double dAngleTo = -1;
            double dLengthTo = -1;
            try
            {
                dDisTo = Convert.ToDouble(sDisTo);
                dSeacherTo = Convert.ToDouble(sSeacherTo);
                dAngleTo = Convert.ToDouble(sAngleTo);
                dLengthTo = Convert.ToDouble(sLengthTo);
            }
            catch (Exception er)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(er, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(er, null, DateTime.Now);
                }
                //********************************************************************
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ӱ߲��������ļ��в�������ȷ��");
                return;
            }
            this.con_AngleTo.Value = dAngleTo;
            this.con_DisTo.Value = dDisTo;
            this.con_LengthTo.Value = dLengthTo;
            this.con_SearchTo.Value = dSeacherTo;
            if (sjoinType == "��׼ͼ��")
            {
                this.com_jointype.SelectedIndex = 0;
            }
            else if (sjoinType == "�Ǳ�׼ͼ��")
            {
                this.com_jointype.SelectedIndex = 1;
            }
            if (sIsRemovePnt == "Y")
                this.check_RemovePoPnt.Checked = true;
            else
                this.check_RemovePoPnt.Checked = false;

            if (sIsSimplify == "Y")
                this.check_Simplify.Checked = true;
            else
                this.check_Simplify.Checked = false;
            if (sMergeAtrSet == "Y")
                this.com_MergeAtrSet.SelectedIndex = 1;
            else
                this.com_MergeAtrSet.SelectedIndex = 0;
            if (!string.IsNullOrEmpty(sLogPath))
            {
                this.logcheck.Checked = true;
                this.label_LogPath.Visible = true;
                this.label_LogPath.Text = sLogPath;
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.com_jointype.Text))
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��һ���ӱ����ͣ�");
                return;
            }
            if (string.IsNullOrEmpty(this.com_MergeAtrSet.Text))
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��Ҫ���ں��������ݴ������ͣ�");
                return;
            }
            double dDisTo = this.con_DisTo.Value; ;
            double dSeacherTo = this.con_SearchTo.Value;
            double dAngleTo = this.con_AngleTo.Value;
            double dLengthTo = this.con_LengthTo.Value;
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(ModData.v_JoinSettingXML);
            if (null == XmlDoc)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ�ӱ߲��������ļ�ʧ�ܣ�");
                return;
            }
            XmlElement ele = XmlDoc.SelectSingleNode(".//�ӱ�����") as XmlElement;
            ele.SetAttribute("�����ݲ�", dDisTo.ToString());
            ele.SetAttribute("�����ݲ�", dSeacherTo.ToString());
            ele.SetAttribute("�Ƕ��ݲ�", dAngleTo.ToString());
            ele.SetAttribute("�����ݲ�", dLengthTo.ToString());
            ele.SetAttribute("�ӱ�����", this.com_jointype.Text.Trim());
            if (this.check_RemovePoPnt.Checked == true)
                ele.SetAttribute("ɾ������ζ����", "Y");
            else
                ele.SetAttribute("ɾ������ζ����", "N");

            if (this.check_Simplify.Checked == true)
                ele.SetAttribute("�򵥻�Ҫ��", "Y");
            else
                ele.SetAttribute("�򵥻�Ҫ��", "N");

            XmlElement ele2 = XmlDoc.SelectSingleNode(".//�ں�����") as XmlElement;
            if (this.com_MergeAtrSet.Text.Trim() == "��ӵ�ԴҪ��")
            {
                ele2.SetAttribute("���Ը���", "N");
            }
            else
            {
                ele2.SetAttribute("���Ը���", "Y");
            }
            XmlElement ele3 = XmlDoc.SelectSingleNode(".//��־����") as XmlElement;
            if (this.logcheck.Checked == true)
            {
                if (!string.IsNullOrEmpty(this.label_LogPath.Text))
                {
                    ele3.SetAttribute("��־·��", this.label_LogPath.Text.Trim());
                }
            }
            else
            {
                ele3.SetAttribute("��־·��", string.Empty);
            }

            try
            {
                XmlDoc.Save(ModData.v_JoinSettingXML);
            }
            catch (Exception er)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(er, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(er, null, DateTime.Now);
                }
                //********************************************************************
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�ӱ������ļ�����д��ʧ�ܣ�\n ��ȷ���ļ��Ƿ�ֻ����\n" + ModData.v_JoinSettingXML);
                return;
            }
            if (this.logcheck.Checked == true)
            {
                IJoinLOG JoinLog = new ClsJoinLog();
                Exception ex = null;
                JoinLog.InitialLog(out ex);
                if (null != ex)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
                    return;
                }
            }
            this.Close();
        }

        private void logcheck_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void logcheck_Click(object sender, EventArgs e)
        {
            if (logcheck.Checked == false)
            {
                SaveFileDialog saveDia = new SaveFileDialog();
                saveDia.Filter = "xml ��־�ļ� (*.xml)|*.xml";
                if (saveDia.ShowDialog() != DialogResult.No || saveDia.ShowDialog() != DialogResult.Cancel)
                {
                    this.label_LogPath.Text = saveDia.FileName;
                    this.label_LogPath.Visible = true;
                }
            }
            else
            {
                this.label_LogPath.Visible = false;
                this.label_LogPath.Text = string.Empty;
            }
        }

    }
}