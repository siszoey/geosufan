using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GeoDataChecker
{
    //�����ֶ�д�����㷨ʵ��
   public class ControlsLineIntersectCheck:Plugin.Interface.CommandRefBase
    {

       private Plugin.Application.IAppGISRef _AppHk;

        public ControlsLineIntersectCheck()
        {
            base._Name = "GeoDataChecker.ControlsLineIntersectCheck";
            base._Caption = "ͬ�����ཻ���";
            base._Tooltip = "�����ͬһ��Ҫ�����У�ͬһ��֮��Ĺ�ϵ���������Ƿ��ཻ";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "ͬ�����ཻ���";
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

            //ִ��ͬ�����ཻ���
            FrmMathematicsCheck mFrmMathematicsCheck = new FrmMathematicsCheck(_AppHk, enumErrorType.ͬ�����ཻ���);
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

            //    //ִ��ͬ�����ཻ���
            //    DataCheckClass dataCheckCls = new DataCheckClass(_AppHk);
            //    dataCheckCls.OrdinaryTopoCheck( pFeaDataset, esriGeometryType.esriGeometryPolyline, esriTopologyRuleType.esriTRTLineNoIntersection, out eError);
            //    if (eError != null)
            //    {
            //        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "ͬ�����ཻ���ʧ�ܣ�"+eError.Message );
            //        return;
            //    }
            //}

            //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "ͬ�����ཻ������!");
        }


       public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGISRef;
            if (_AppHk.MapControl == null) return;
        }
    }
}
