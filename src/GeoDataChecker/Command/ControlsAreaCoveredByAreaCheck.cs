using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GeoDataChecker
{
    public class ControlsAreaCoveredByAreaCheck:Plugin.Interface.CommandRefBase
    {
       private Plugin.Application.IAppGISRef _AppHk;

        public ControlsAreaCoveredByAreaCheck()
        {
            base._Name = "GeoDataChecker.ControlsAreaCoveredByAreaCheck";
            base._Caption = "�溬����";
            base._Tooltip = "һ��Ҫ�����еĶ����Ҫ���Ƿ���һ��Ҫ�����ж����Ҫ�ظ���";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "�溬����";
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

            if (_AppHk == null) return;
            if (_AppHk.MapControl == null) return;

            //ִ���溬����
            FrmMathematicsCheck mFrmMathematicsCheck = new FrmMathematicsCheck(_AppHk, enumErrorType.�溬����);
            mFrmMathematicsCheck.ShowDialog();


            //string oriFeaClsName = "A2_JJ_PY";
            //string desFeaClsName = "A1_JJ_PY";

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

            //    //ִ���溬����
            //    DataCheckClass dataCheckCls = new DataCheckClass(_AppHk);
            //    dataCheckCls.OrdinaryTopoCheck(pFeaDataset, oriFeaClsName, desFeaClsName, esriTopologyRuleType.esriTRTAreaCoveredByArea, out eError);
            //    if (eError != null)
            //    {
            //        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�溬����ʧ�ܣ�"+eError.Message);
            //        return;
            //    }
            //}

            //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�溬�������!");
        }


       public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGISRef;
            if (_AppHk.MapControl == null) return;
        }
    }
}

