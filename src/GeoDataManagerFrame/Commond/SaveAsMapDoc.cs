using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;

//����ͼ�ĵ�
namespace GeoDataManagerFrame
{
    public class SaveAsMapDoc : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef m_Hook;
        private Plugin.Application.IAppFormRef m_frmhook;
        public SaveAsMapDoc()
        {
            base._Name = "GeoDataManagerFrame.SaveAsMapDoc";
            base._Caption = "����ͼ�ĵ�";
            base._Tooltip = "����ͼ�ĵ�";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "����ͼ�ĵ�";
        }

        public override void OnClick()
        {
            if (m_Hook == null)
                return;
             
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "xml�ļ�|*.xml";
            dlg.ShowDialog();
            string xmlpath = dlg.FileName;
            if (xmlpath.Equals ("") )
                return;

            LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
            string strLog = "����ͼ�ĵ�,Ŀ���ļ�·��:" + xmlpath;
            if (log != null)
            {
                log.Writelog(strLog);
            }

            string strWorkFile = Application.StartupPath + "\\..\\Temp\\CurPrj.xml";
            System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
            xmldoc.Load(strWorkFile);
            xmldoc.Save(xmlpath);

            /*       Exception eError;
                   AddGroup frmGroup = new AddGroup();
                   if (frmGroup.ShowDialog() == DialogResult.OK)
                   {
                       ModuleOperator.DisplayRoleTree("", m_Hook.RoleTree, ref ModData.gisDb, out eError);
                       if (eError != null)
                       {
                           ErrorHandle.ShowFrmError("��ʾ", eError.Message);
                           return;
                       }
                   }
             */
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null)
                return;
            m_Hook = hook as Plugin.Application.IAppGisUpdateRef;
            m_frmhook = hook as Plugin.Application.IAppFormRef;
        }
    }
}
