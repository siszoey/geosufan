using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GeoDataChecker
{
   public class ControlsPointCoveredByLineEndpointCheck:Plugin.Interface.CommandRefBase
    {
       private Plugin.Application.IAppGISRef _AppHk;

        public ControlsPointCoveredByLineEndpointCheck()
        {
            base._Name = "GeoDataChecker.ControlsPointCoveredByLineEndpointCheck";
            base._Caption = "��λ���߶˵���";
            base._Tooltip = "�����Ƿ�λ���ߵĶ˵���";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "��λ���߶˵���";
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
            Exception eError = null;

            if (_AppHk == null) return;
            if (_AppHk.MapControl == null) return;

            //ִ�е�λ���߶˵���
            FrmMathematicsCheck mFrmMathematicsCheck = new FrmMathematicsCheck(_AppHk, enumErrorType.��λ���߶˵���);
            mFrmMathematicsCheck.ShowDialog();


            //string oriFeaClsName = "GB500_RES_PT";
            //string desFeaClsName="GB500_RES_LN";

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

            //    //ִ�е�λ���߶˵���
            //    DataCheckClass dataCheckCls = new DataCheckClass(_AppHk);
            //    dataCheckCls.OrdinaryTopoCheck(pFeaDataset, oriFeaClsName,desFeaClsName, esriTopologyRuleType.esriTRTPointCoveredByLineEndpoint, out eError);
            //    if (eError != null)
            //    {
            //        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��λ���߶˵���ʧ�ܣ�"+eError.Message);
            //        return;
            //    }
            //}

            //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��λ���߶˵������!");
        }


       public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGISRef;
            if (_AppHk.MapControl == null) return;
        }
    }
}


