using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GeoDataChecker
{
    public class ControlsLinePseudosCheck:Plugin.Interface.CommandRefBase
    {
       private Plugin.Application.IAppGISRef _AppHk;

        public ControlsLinePseudosCheck()
        {
            base._Name = "GeoDataChecker.ControlsLinePseudosCheck";
            base._Caption = "��α�ڵ���";
            base._Tooltip = "���������ͬ���߶��м��Ƿ��жϵ�";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "��α�ڵ���";
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

            //ִ����α�ڵ���
            FrmMathematicsCheck mFrmMathematicsCheck = new FrmMathematicsCheck(_AppHk, enumErrorType.�ߴ���α�ڵ� );
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

            //    //ִ����α�ڵ���
            //    DataCheckClass dataCheckCls = new DataCheckClass(_AppHk);
            //    dataCheckCls.OrdinaryTopoCheck( pFeaDataset, esriGeometryType.esriGeometryPolyline, esriTopologyRuleType.esriTRTLineNoPseudos, out eError);
            //    if (eError != null)
            //    {
            //        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��α�ڵ���ʧ�ܣ�"+eError.Message );
            //        return;
            //    }
            //}

            //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��α�ڵ������!");
        }


       public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGISRef;
            if (_AppHk.MapControl == null) return;
        }
    }
}
