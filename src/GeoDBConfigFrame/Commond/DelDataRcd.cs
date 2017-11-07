using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using GeoDataCenterFunLib;

using SysCommon.Error;
using ESRI.ArcGIS.Geodatabase;
//ɾ����¼
namespace GeoDBConfigFrame
{
    public class DelDataRcd : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppPrivilegesRef m_Hook;
        public DelDataRcd()
        {
            base._Name = "GeoDBConfigFrame.DelDataRcd";
            base._Caption = "ɾ����¼";
            base._Tooltip = "ɾ����¼";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "ɾ����¼";
        }
        //ɾ����¼�˵���Ӧ
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
            //���δѡ�м�¼...
            if (pGridControl.SelectedRows.Count == 0)
            { 
                DevComponents.DotNetBar.MessageBoxEx.Show("δѡ�м�¼��");
                return;
            }
            int k = pGridControl.SelectedRows.Count;
            //ɾ������ʱ��Ҫѯ��һ��
            if (pGridControl.SelectedRows.Count > 0)
            {
                if (DevComponents.DotNetBar.MessageBoxEx.Show("ȷ��Ҫɾ��ѡ�еļ�¼��", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            //OleDbConnection mycon = new OleDbConnection(connstr);   //����OleDbConnection����ʵ�����������ݿ�
            //string strExp = "";
            //OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            //mycon.Open();
            //int i = 0, j = 0;
            if (GeoDataCenterFunLib.LogTable.m_sysTable == null)
                return;
            Exception err;
            ITable pTable =LogTable.m_sysTable.OpenTable(Tablename, out err);
            if (pTable == null)
            {
                //MessageBox.Show(err.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ErrorHandle.ShowFrmErrorHandle("��ʾ", err.Message);
                return;//����־�����ڣ�����null
            }
            if (pGridControl.SelectedRows.Count > 0)
            {
                string objectID = pTable.OIDFieldName;
                string strExp = objectID+" IN (";
                for (int h = 0; h < pGridControl.SelectedRows.Count; h++)
                {   
                    //����ɾ����¼�����
                    
                    for (int i = 0; i < pGridControl.ColumnCount; i++)
                    {
                        if (pGridControl.Columns[i].Name.ToUpper().Equals("ID"))
                        {
                            
                            strExp += pGridControl.SelectedRows[h].Cells[i].Value.ToString()+",";
                        }
                    }
                }
                strExp = strExp.Substring(0, strExp.Length - 1);
                strExp += ")";
                //ִ��ɾ����¼������
                IWorkspace pWorkspace = LogTable.m_gisDb.WorkSpace;
                ITransactions pTransactions = (ITransactions)pWorkspace;
                try
                {

                    if (!pTransactions.InTransaction) pTransactions.StartTransaction();
                }
                catch (Exception eX)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eX.Message);
                    return;
                }
                Exception exError;
                if (!LogTable.m_sysTable.DeleteRows(Tablename, strExp, out exError))
                {
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", "ɾ����¼ʧ�ܣ�" + exError.Message);
                    return;
                }
              
                try
                {
                    if (pTransactions.InTransaction) pTransactions.CommitTransaction();
                }
                catch (Exception eX)
                {
                }
            }
        
           
            //�ٴγ�ʼ��datagridview�ؼ�
            pfacecontrol.InitDataInfoList(Tablename );
            
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
