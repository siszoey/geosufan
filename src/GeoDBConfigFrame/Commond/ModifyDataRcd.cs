using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using GeoDataCenterFunLib;

//�޸ļ�¼
namespace GeoDBConfigFrame
{
    public class ModifyDataRcd : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
        public ModifyDataRcd()
        {
            base._Name = "GeoDBConfigFrame.ModifyDataRcd";
            base._Caption = "�޸ļ�¼";
            base._Tooltip = "�޸ļ�¼";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�޸ļ�¼";
        }
        //��Ӽ�¼�˵���Ӧ
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
            FaceControl pFaceControl = (FaceControl)(m_Hook.MainUserControl);
            if (pGridControl.DataSource == null)
                return;
            //���δѡ�м�¼������
            if (pGridControl.SelectedRows.Count == 0)
            {
                MessageBox.Show("δѡ�м�¼��");
                return;
            }
            //���ѡ�ж��м�¼������
            if (pGridControl.SelectedRows.Count > 1)
            {
                MessageBox.Show("�����޸�һ�У�");
                return;
            }
            int idx = pGridControl.SelectedRows[0].Index;//yjl20111103 add
            //��ʼ���޸ļ�¼�Ի���
            TableForm myTableForm = new TableForm(pGridControl,connstr,Tablename);
            myTableForm.InitForm("MODIFY");
            myTableForm.ShowDialog();
            //��¼�޸ĺ��ٴγ�ʼ��dataview�ؼ�
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
                //    MessageBox.Show("�޸ĵļ�¼ֻ��Ӧ��ϵͳ�����Ժ������Ч��", "��ʾ��");
                //    break;
            }
            
            
            //yjl20111103 add 
            if (pGridControl.Rows.Count > idx)
            {
                //ȡ��Ĭ�ϵĵ�1��ѡ��
                pGridControl.Rows[0].Selected = false;
                //����ѡ����
                pGridControl.Rows[idx].Selected = true;
                //���õ�ǰ��
                pGridControl.CurrentCell = pGridControl.Rows[idx].Cells[0];
            }
            /////
            if (this.WriteLog)
            {
                Plugin.LogTable.Writelog(Caption);//xisheng 2011.07.09 ������־
            }
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
