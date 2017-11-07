using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.IO;

using ESRI.ArcGIS.Carto;

namespace GeoDataChecker
{
    /// <summary>
    /// �����׼�����
    /// </summary>
    public class ControlsCodeStandardizeCheck: Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGISRef  _AppHk;
        private DataTable _ResultTable;
        private clsCodeCheckProccess _CodeErrorPro;
       
        public ControlsCodeStandardizeCheck()
        {
            base._Name = "GeoDataChecker.ControlsCodeStandardizeCheck";
            base._Caption = "�����׼�����";
            base._Tooltip = "�����ص�ͼ����Ҫ�ش����Ƿ�������ݱ�׼����";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�����׼�����";
        }

        /// <summary>
        /// ͼ���д�������ʱ����״̬Ϊ����ʱ�ſ���
        /// </summary>
        public override bool Enabled
        {
            get
            {
                try
                {

                    if (_AppHk == null) return false;
                    if (_AppHk.MapControl == null) return false;
                    if (_AppHk.MapControl.LayerCount == 0)
                    {
                        base._Enabled = false;
                        return false;
                    }
                    else
                    {
                        base._Enabled = true;
                        return true;
                        
                    }
                }
                catch
                {
                    base._Enabled = false;
                    return false;
                }
            }
        }
        public override string Message
        {
            get
            {
                Plugin.Application.IAppFormRef pAppFormRef = _AppHk as Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Plugin.Application.IAppFormRef pAppFormRef = _AppHk as Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        } 
              
        public override void OnClick()
        {
            Exception eError = null;
            #region ��ʼ�������б�,���󶨵�DataGrid����
            //_ResultTable = new DataTable();
            //_ResultTable.Columns.Add("Ҫ��������", typeof(string));
            //_ResultTable.Columns.Add("OBJECTID", typeof(string));
            //_ResultTable.Columns.Add("������", typeof(string));
            //_ResultTable.Columns.Add("��������", typeof(string));
            //_ResultTable.Columns.Add("���ʱ��", typeof(string));

            //_AppHk.DataCheckGrid.DataSource = null;
            //_AppHk.DataCheckGrid.DataSource = _ResultTable;
            //_AppHk.DataCheckGrid.Visible = true;
            //_AppHk.DataCheckGrid.ReadOnly = true;
            //_AppHk.DataCheckGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //for (int j = 0; j < _AppHk.DataCheckGrid.Columns.Count; j++)
            //{
            //    _AppHk.DataCheckGrid.Columns[j].Width = (_AppHk.DataCheckGrid.Width - 15) / _AppHk.DataCheckGrid.Columns.Count;
            //}
            //_AppHk.DataCheckGrid.RowHeadersWidth = 20;
            #endregion

            #region ���Ҫ����IFeatureLayer�ļ���
            //��Mapcontrol�����е�ͼ������������
            List<IFeatureLayer> LstFeaLayer = new List<IFeatureLayer>();
            for (int i = 0; i < _AppHk.MapControl.LayerCount; i++)
            {
                ILayer player = _AppHk.MapControl.get_Layer(i);
                if (player is IGroupLayer)
                {
                    if (player.Name == "��Χ")
                    {
                        continue;
                    }
                    ICompositeLayer pComLayer = player as ICompositeLayer;
                    for (int j = 0; j < pComLayer.Count; j++)
                    {
                        ILayer mLayer = pComLayer.get_Layer(j);
                        IFeatureLayer mfeatureLayer = mLayer as IFeatureLayer;
                        if (mfeatureLayer == null) return;
                        if (!LstFeaLayer.Contains(mfeatureLayer))
                        {
                            LstFeaLayer.Add(mfeatureLayer);
                        }

                    }
                }
                else
                {
                    IFeatureLayer pfeatureLayer = player as IFeatureLayer;
                    if (pfeatureLayer == null) return;
                    if (!LstFeaLayer.Contains(pfeatureLayer))
                    {
                        LstFeaLayer.Add(pfeatureLayer);
                    }

                }
            }
            #endregion

            string path = TopologyCheckClass.GeoDataCheckParaPath;// Application.StartupPath + "\\..\\Res\\checker\\GeoCheckPara.mdb";
            Plugin.Application.IAppFormRef pAppForm = _AppHk as Plugin.Application.IAppFormRef;

            #region ������־����
            //������־������Ϣ
            string logPath = TopologyCheckClass.GeoLogPath + "Log" + System.DateTime.Today.Year.ToString() + System.DateTime.Today.Month.ToString() + System.DateTime.Today.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString() + ".xls"; ;
            if (File.Exists(logPath))
            {
                if (SysCommon.Error.ErrorHandle.ShowFrmInformation("��", "��", "��־�ļ�\n'" + logPath + "'\n�Ѿ�����,�Ƿ��滻?"))
                {
                    File.Delete(logPath);
                }
                else
                {
                    return;
                }
            }
            //������־��ϢEXCEL��ʽ
            SysCommon.DataBase.SysDataBase pSysLog = new SysCommon.DataBase.SysDataBase();
            pSysLog.SetDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + logPath + "; Extended Properties=Excel 8.0;", SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out eError);
            if (eError != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��־��Ϣ������ʧ�ܣ�");
                return;
            }
            string strCreateTableSQL = @" CREATE TABLE ";
            strCreateTableSQL += @" ������־ ";
            strCreateTableSQL += @" ( ";
            strCreateTableSQL += @" ��鹦���� VARCHAR, ";
            strCreateTableSQL += @" �������� VARCHAR, ";
            strCreateTableSQL += @" �������� VARCHAR, ";
            strCreateTableSQL += @" ����ͼ��1 VARCHAR, ";
            strCreateTableSQL += @" ����OID1 VARCHAR, ";
            strCreateTableSQL += @" ����ͼ��2 VARCHAR, ";
            strCreateTableSQL += @" ����OID2 VARCHAR, ";
            strCreateTableSQL += @" ��λ��X VARCHAR , ";
            strCreateTableSQL += @" ��λ��Y VARCHAR , ";
            strCreateTableSQL += @" ���ʱ�� VARCHAR ,";
            strCreateTableSQL += @" �����ļ�·�� VARCHAR ";
            strCreateTableSQL += @" ) ";

            pSysLog.UpdateTable(strCreateTableSQL, out eError);
            if (eError != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ͷ����");
                pSysLog.CloseDbConnection();
                return;
            }

            //DataCheckClass dataCheckCls = new DataCheckClass(_AppHk);
            //����־��������Ϣ�ͱ�����������
            //dataCheckCls.ErrDbCon = pSysLog.DbConn;
            //dataCheckCls.ErrTableName = "������־";
            #endregion
           
            //_CodeErrorPro = new clsCodeCheckProccess();
            ClsCodeCheck CodeErrerCheck = new ClsCodeCheck(_AppHk, path, LstFeaLayer);
            CodeErrerCheck.ErrDbCon = pSysLog.DbConn;
            CodeErrerCheck.ErrTableName = "������־";

            //CodeErrerCheck.FindErr += new GOGISErrorChecker.EventHandle(CodeErrerCheck_FindErr);
            //CodeErrerCheck.ProgressStep += new GOGISErrorChecker.ProgressHandle(CodeErrerCheck_ProgressStep);

            pAppForm.ProgressBar.Visible = true;
            CodeErrerCheck.ExcuteCheck();
            pAppForm.ProgressBar.Visible = false;


            pSysLog.CloseDbConnection();
            //_CodeErrorPro.Dispose();
        }

       private  void CodeErrerCheck_ProgressStep(object sender, int CurStep, int MaxValue)
        {
            Plugin.Application.IAppFormRef pAppForm = sender as Plugin.Application.IAppFormRef;
            pAppForm.ProgressBar.Maximum = MaxValue;
            pAppForm.ProgressBar.Minimum = 0;
            pAppForm.ProgressBar.Value = CurStep;
        }

        /// <summary>
        /// ���ִ����Ĵ�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ErrorArg"></param>
        private void CodeErrerCheck_FindErr(object sender, ErrorEventArgs ErrorArg)
        {
            //����������Ϊ��
            if (ErrorArg !=null)
            {
                for (int i = 0; i < ErrorArg.OIDs.Length; i++)
                {
                    DataRow newRow = _ResultTable.NewRow();
                    newRow["Ҫ��������"] = ErrorArg.FeatureClassName;
                    newRow["OBJECTID"] = ErrorArg.OIDs[i];
                    newRow["������"] = ErrorArg.ErrorName;
                    newRow["��������"] = ErrorArg.ErrDescription;
                    newRow["���ʱ��"] = ErrorArg.CheckTime;
                    _ResultTable.Rows.Add(newRow);
                }
                //�û������ϱ��ֳ�������Ϣ
                Plugin.Application.IAppGISRef mHook = (Plugin.Application.IAppGISRef)sender;    //�������б�
                if (mHook == null) return;
                mHook.DataCheckGrid.Update();

                //���ô�������ĺ���,����鵽�Ĵ�����Ϣ��д����־
                if (_ResultTable.Rows.Count > 0)
                {
                    _CodeErrorPro.LogErr(sender, ErrorArg);
                }
               
            }

        }

       


        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGISRef;
            if (_AppHk.MapControl == null) return;
        }
    }
}