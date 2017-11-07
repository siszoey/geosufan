using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using GeoDataCenterFunLib;
//ȫ��ɾ��
namespace GeoDBConfigFrame
{
    public class DelAllDataRcd : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
        public DelAllDataRcd()
        {
            base._Name = "GeoDBConfigFrame.DelAllDataRcd";
            base._Caption = "ȫ��ɾ��";
            base._Tooltip = "ȫ��ɾ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "ȫ��ɾ��";
        }
        //ȫ��ɾ���˵���Ӧ
        public override void OnClick()
        {
            
            if (m_Hook.GridCtrl == null)
                return;
            FaceControl pfacecontrol = (FaceControl)m_Hook.MainUserControl;
            DataGridView pGridControl = m_Hook.GridCtrl;
            if (pfacecontrol.getEditable() == false)
                return;
            string connstr, Tablename;
            //��ȡ���ݿ����Ӵ��ͱ���
            connstr = pfacecontrol.m_connstr;
            Tablename = pfacecontrol.m_TableName;
            if (Tablename.Contains("."))
                Tablename = Tablename.Split('.')[1];//����SDE��
            FaceControl pFaceControl = (FaceControl)(m_Hook.MainUserControl);
            if (pGridControl.DataSource == null)
                return;
            //ɾ������ʱҪѯ��һ��
            if (DevComponents.DotNetBar.MessageBoxEx.Show("ȷ��Ҫȫ��ɾ����ɾ���󲻿ɻָ���", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            try
            {
                if (GeoDataCenterFunLib.LogTable.m_sysTable == null)
                    return;
                Exception ex = null;
                LogTable.m_sysTable.DeleteRows(Tablename, "", out ex);//ɾ��
                if (ex != null)
                    throw new Exception("ɾ��ȫ������ʧ�ܣ�", ex);
            }
            catch (Exception pEx)
            {
                DevComponents.DotNetBar.MessageBoxEx.Show(pEx.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            pfacecontrol.InitDataInfoList(Tablename);
            ////ZQ   20111017   add  ��ʱ���������ֵ�
            switch (Tablename)
            {
                case "���Զ��ձ�":
                    SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");
                    break;
                case "��׼ͼ������":
                    SysCommon.ModField.InitLayerNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicLayerName);
                    break;
                //default:
                //    ///ZQ 20111020 add ����������ʾ
                //    MessageBox.Show("ɾ���ļ�¼ֻ��Ӧ��ϵͳ�����Ժ������Ч��", "��ʾ��");
                //    break;
            }
            /////
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(Caption);//xisheng 2011.07.09 ������־
            }
            //OleDbConnection mycon = new OleDbConnection(connstr);   //����OleDbConnection����ʵ�����������ݿ�
            ////����ɾ�����ݵ����
            //string strExp = "delete from " + Tablename ;
            //OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            //mycon.Open();
            ////ִ��ɾ�����
            //aCommand.ExecuteNonQuery();
            //mycon.Close();
            ////�ٴγ�ʼ��datagridview�ؼ�
            //pfacecontrol.InitDataInfoList(Tablename );

            //if (m_Hook != null)
            //{
            //    LogFile log = new LogFile(m_Hook.tipRichBox, m_Hook.strLogFilePath);
            //    if (log != null)
            //    {
            //        log.Writelog("����ȫ��ɾ��");

            //    }
            //}

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
            m_Hook = hook as Plugin.Application.IAppPrivilegesRef;
        }
    }
}
