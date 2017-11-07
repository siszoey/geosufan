using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;


namespace GeoDataChecker
{
    /// <summary>
    /// �߳����߼��Լ��
    /// </summary>
    public class ControlsLineLengthLogicCheck:Plugin.Interface.CommandRefBase
    {
       private Plugin.Application.IAppGISRef _AppHk;

        public ControlsLineLengthLogicCheck()
        {
            base._Name = "GeoDataChecker.ControlsLineLengthLogicCheck";
            base._Caption = "�߳����߼��Լ��";
            base._Tooltip = "���ָ�������ݵĳ����Ƿ����ָ���߼�ֵ���ɸ����㳤���߼�飩";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�߳����߼��Լ��";
        }

        /// <summary>
        /// ͼ���д�������ʱ����״̬Ϊ����ʱ�ſ���
        /// </summary>
        public override bool Enabled
        {
            get
            {
                if (_AppHk == null) return false;
                if (_AppHk.MapControl == null) return false;
                if (_AppHk.MapControl.LayerCount == 0) return false;
                return true;
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
            //FrmMathematicsCheck mFrmMathematicsCheck = new FrmMathematicsCheck(_AppHk, enumErrorType.�߳����߼���);
            //mFrmMathematicsCheck.ShowDialog();

            Exception eError = null;

            FrmLineLengthCheck pFrmLineLengthCheck = new FrmLineLengthCheck();
            if (pFrmLineLengthCheck.ShowDialog() == DialogResult.OK)
            {
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

                DataCheckClass dataCheckCls = new DataCheckClass(_AppHk);
                //����־��������Ϣ�ͱ�����������
                dataCheckCls.ErrDbCon = pSysLog.DbConn;
                dataCheckCls.ErrTableName = "������־";
                #endregion

                #region ���Ҫ����IFeatureClass�ļ���
                //��Mapcontrol�����е�ͼ������������
                List<IFeatureClass> LstFeaClass = new List<IFeatureClass>();
                for (int i = 0; i < _AppHk.MapControl.LayerCount; i++)
                {
                    ILayer player = _AppHk.MapControl.get_Layer(i);
                    if (player is IGroupLayer)
                    {

                        ICompositeLayer pComLayer = player as ICompositeLayer;
                        for (int j = 0; j < pComLayer.Count; j++)
                        {
                            ILayer mLayer = pComLayer.get_Layer(j);
                            IFeatureLayer mfeatureLayer = mLayer as IFeatureLayer;
                            if (mfeatureLayer == null) continue;
                            IFeatureClass pfeaCls = mfeatureLayer.FeatureClass;
                            if (!LstFeaClass.Contains(pfeaCls))
                            {
                                LstFeaClass.Add(pfeaCls);
                            }
                        }
                    }
                    else
                    {
                        IFeatureLayer pfeatureLayer = player as IFeatureLayer;
                        if (pfeatureLayer == null) continue;
                        IFeatureClass mFeaCls = pfeatureLayer.FeatureClass;
                        if (!LstFeaClass.Contains(mFeaCls))
                        {
                            LstFeaClass.Add(mFeaCls);
                        }
                    }
                }
                #endregion
                try
                {
                    SysCommon.DataBase.SysTable pSysTable = new SysCommon.DataBase.SysTable();
                    string conStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + TopologyCheckClass.GeoDataCheckParaPath;
                    pSysTable.SetDbConnection(conStr, SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out eError);
                    if (eError != null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "GIS���ݼ�����ñ�����ʧ�ܣ�");
                        pSysLog.CloseDbConnection();
                        return;
                    }

                    //��������ֶ���
                    string codeName =TopologyCheckClass.GetCodeName(pSysTable, out eError);
                    if (eError != null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                        pSysLog.CloseDbConnection();
                        pSysTable.CloseDbConnection();
                        return;
                    }

                    DataTable mTable = TopologyCheckClass.GetParaValueTable(pSysTable, 3, out eError);
                    if (eError != null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", eError.Message);
                        pSysLog.CloseDbConnection();
                        pSysTable.CloseDbConnection();
                        return;
                    }
                    if (mTable.Rows.Count == 0)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "δ�����߳��ȼ�����ã�");
                        pSysLog.CloseDbConnection();
                        pSysTable.CloseDbConnection();
                        return;
                    }
                    //ִ���߳����߼��Լ��
                    double pmax = pFrmLineLengthCheck.MaxValue;
                    double pmin = pFrmLineLengthCheck.MinValue;
                    LineLogicCheck(dataCheckCls, LstFeaClass, mTable, codeName, pmin.ToString(), pmax.ToString(), out eError);
                    if (eError != null)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�߳����߼��Լ��ʧ�ܡ�" + eError.Message);
                        pSysLog.CloseDbConnection();
                        pSysTable.CloseDbConnection();
                        return;
                    }
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݼ�����!");

                    pSysLog.CloseDbConnection();
                    pSysTable.CloseDbConnection();
                    //���ؽ�����
                    dataCheckCls.ShowProgressBar(false);
                }
                catch (Exception ex)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
                    pSysLog.CloseDbConnection();
                    return;
                }
            }
        }


        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGISRef;
            if (_AppHk.MapControl == null) return;
        }


        /// <summary>
        /// �߳����߼��Լ��
        /// </summary>
        /// <param name="pDataCheckClass"></param>
        /// <param name="pFeaDataset"></param>
        /// <param name="pTable">�������ñ�</param>
        /// <param name="codeName">��������ֶ���</param>
        /// <param name="lenMin">��С����</param>
        /// <param name="lenMax">��󳤶�</param>
        /// <param name="eError"></param>
        private void LineLogicCheck(DataCheckClass pDataCheckClass, List<IFeatureClass> LstFeaClass, DataTable pTable, string codeName, string lenMin, string lenMax, out Exception eError)
        {
            eError = null;
            Dictionary<string, string> feaClsInfodic = new Dictionary<string, string>();
            for (int i = 0; i < pTable.Rows.Count; i++)
            {
                string pFeaClsName = pTable.Rows[i]["ͼ��"].ToString().Trim();  //ͼ����
                string codeValue = pTable.Rows[i]["�����"].ToString().Trim();  //Ҫ���ķ������ֵ����
                if (pFeaClsName == "")
                {
                    eError = new Exception("ͼ����Ϊ��!");
                    return;
                }
                if (!feaClsInfodic.ContainsKey(pFeaClsName))
                {
                    feaClsInfodic.Add(pFeaClsName, codeValue);
                }
                else
                {
                    if (codeValue != "")
                    {
                        string tempStr = feaClsInfodic[pFeaClsName] + "," + codeValue;
                        feaClsInfodic[pFeaClsName] = tempStr;
                    }
                    else
                    {
                        feaClsInfodic[pFeaClsName] = "";
                    }
                }
            }
            pDataCheckClass.LineLengthLogicCheck(LstFeaClass, codeName, feaClsInfodic, lenMin, lenMax, out eError);
        }
    }
}
