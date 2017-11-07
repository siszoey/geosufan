using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using GeoDataCenterFunLib;
//��Ӽ�¼
namespace GeoDBConfigFrame
{
    public class AddDataRcd : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
     //   private Plugin.Application.IAppFormRef m_frmhook;
        public AddDataRcd()
        {
            base._Name = "GeoDBConfigFrame.AddDataRcd";
            base._Caption = "��Ӽ�¼";
            base._Tooltip = "��Ӽ�¼";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "��Ӽ�¼";
        }
        //��Ӽ�¼�˵���Ӧ
        public override void OnClick()
        {


            if (m_Hook.GridCtrl  == null)
                return;
            FaceControl pfacecontrol = (FaceControl)m_Hook.MainUserControl;
            DataGridView pGridControl = m_Hook.GridCtrl;
            if (pfacecontrol.getEditable() == false)
                return;
            string connstr, Tablename;
            //��ȡ���ݿ����Ӵ��ͱ���
            connstr = pfacecontrol.m_connstr;
            Tablename = pfacecontrol.m_TableName;
            FaceControl pFaceControl =( FaceControl )(m_Hook.MainUserControl);
            if (pGridControl.DataSource == null)
                return;
            //��ʼ����Ӽ�¼�Ի���
            TableForm myTableForm = new TableForm(pGridControl, connstr, Tablename);
            myTableForm.InitForm("ADD");
           DialogResult result= myTableForm.ShowDialog();
           if (result == DialogResult.OK)//changed by xisheng 2011.06.16
            { //��¼��Ӻ��ٴγ�ʼ��dataview�ؼ�
                pfacecontrol.InitDataInfoList(Tablename,true);
            }
            else
            {
                pfacecontrol.InitDataInfoList(Tablename,false);
            }
           ////ZQ   20111017   add  ��ʱ���������ֵ�
           switch (Tablename)
           {
               case "���Զ��ձ�":
                   SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");
                   break;
               case"��׼ͼ������":
                   SysCommon.ModField.InitLayerNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicLayerName);
                   break;
               //default:
               //    ///ZQ 20111020 add ����������ʾ
               //    MessageBox.Show("��ӵļ�¼ֻ��Ӧ��ϵͳ�����Ժ������Ч��","��ʾ��");
               //    break;
           }
           /////
           if (this.WriteLog)
           {
               Plugin.LogTable.Writelog(Caption);//xisheng 2011.07.09 ������־
           }
            //OleDbCommandBuilder builder = new OleDbCommandBuilder(pFaceControl.m_Adapter );
            ////�������ύ�����ݿ����
            //try
            //{
            //    pFaceControl.m_Adapter.Update(pFaceControl.m_dataTable);
            //    //�ύ�����»�ȡ����
            //    pFaceControl.m_dataTable.Clear();
            //    pFaceControl.m_Adapter.Fill(pFaceControl.m_dataTable);
            //    pGridControl.DataSource = null;
            //    pGridControl.DataSource = pFaceControl.m_dataTable;
            //    //gridControl.Update();
            //}
            //catch (System.Exception m)
            //{
            //    Console.WriteLine(m.Message);
            //}
            //pGridControl = null;
            //pFaceControl = null;
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
         //   m_frmhook = hook as Plugin.Application.IAppFormRef;
        }
    }
}
