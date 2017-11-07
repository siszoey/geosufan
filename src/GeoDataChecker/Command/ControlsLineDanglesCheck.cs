using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GeoDataChecker
{
    //�����ֶ�д�����㷨���иĽ�������
    public class ControlsLineDanglesCheck:Plugin.Interface.CommandRefBase
    {
       private Plugin.Application.IAppGISRef _AppHk;

        public ControlsLineDanglesCheck()
        {
            base._Name = "GeoDataChecker.ControlsLineDanglesCheck";
            base._Caption = "�����ҵ���";
            base._Tooltip = "�����Ҫ�����Ƿ�������㣬��ÿһ���߶εĶ˵��Ƿ����";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�����ҵ���";
        }

        public override bool Enabled
        {
            get
            {
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
            //Exception eError = null;

            //string oriFeaClsName = "GB500_PIP_LN";
            //string desFeaClsName = "GB500_RES_PY";
            ////string desFeaClsName = "GB500_RES_LN";
            ////string desFeaClsName = "GB500_PIP_LN";

            if (_AppHk == null) return;
            if (_AppHk.MapControl == null) return;

            //ִ�������ҵ���
            FrmMathematicsCheck mFrmMathematicsCheck = new FrmMathematicsCheck(_AppHk, enumErrorType.�ߴ������ҵ�);
            mFrmMathematicsCheck.ShowDialog();

            //SysCommon.Gis.SysGisDataSet pGisDT = new SysCommon.Gis.SysGisDataSet();
            //pGisDT.SetWorkspace(TopologyCheckClass.DataCheckPath, SysCommon.enumWSType.PDB , out eError );
            //if (eError != null)
            //{
            //    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�������ݿ����");
            //    return;
            //}
            //List<string> feaDatasetNameLst = pGisDT.GetAllFeatureDatasetNames();
            //for (int i = 0; i < feaDatasetNameLst.Count; i++)
            //{
            //    IFeatureDataset pFeaDataset = pGisDT.GetFeatureDataset(feaDatasetNameLst[i], out eError);
            //    if (eError != null)
            //    {
            //        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ȡ���ݼ�ʧ��,���ݼ�����Ϊ:"+feaDatasetNameLst[i]);
            //        continue;
            //    }

            //    //ִ�������ҵ���
            //    DataCheckClass dataCheckCls = new DataCheckClass(_AppHk);
            //    if (oriFeaClsName == desFeaClsName)
            //    {
            //        //ͬ�������ҵ���
            //        dataCheckCls.OrdinaryTopoCheck(pFeaDataset, oriFeaClsName, esriTopologyRuleType.esriTRTLineNoDangles, out eError);
            //        if (eError != null)
            //        {
            //            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ҵ���ʧ�ܣ�" + eError.Message);
            //            return;
            //        }
            //    }
            //    else
            //    {
            //        string oriStr = "GISID='1'";//1,2
            //        string desStr = "GISID='31090030'";//38020520,31090030
            //        double tolerence = 0.5;
            //        dataCheckCls.LineDangleCheck(pFeaDataset, oriFeaClsName, oriStr, desFeaClsName, desStr, tolerence, out eError);
            //        if (eError != null)
            //        {
            //            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ҵ���ʧ�ܣ�" + eError.Message);
            //            return;
            //        }
            //    }
            //}

            //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ҵ������!");
        }


       public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGISRef;
            if (_AppHk.MapControl == null) return;
        }
    }
}
