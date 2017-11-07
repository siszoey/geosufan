using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;

namespace GeoHistory
{
    public class ControlsAddHistoryDataBase : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGISRef _AppHk;
        public ControlsAddHistoryDataBase()
        {
            base._Name = "GeoHistory.ControlsAddHistoryDataBase";
            base._Caption = "������ʷ������";
            base._Tooltip = "������ʷ������";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "���ط���������ʷ������";
            //base._Image = "";
            //base._Category = "";
        }

        public override bool Enabled
        {
            get
            {
                if (_AppHk == null) return false;
                bool bres = true;
                for (int i = 0; i < _AppHk.MapControl.LayerCount; i++)
                {
                    ILayer mLayer = _AppHk.MapControl.get_Layer(i);
                    if (mLayer is IGroupLayer)
                    {
                        if (mLayer.Name == "��ʷ������")
                        {
                            bres = false;
                            break;
                        }
                    }
                }
                return bres;
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
            //��ǰ���з�Χ�棬��������������ʾ��Χ
            IFeatureLayer pRangeFeatLay = null;
            IFeatureLayer pFeatureLayer = null;
            IFeatureClass pFeatureClass = null;
            Exception exError = null;

            //�����ʷ������groupͼ���Ѿ����� �Ͳ����ټ���
            for (int i = 0; i < _AppHk.MapControl.LayerCount; i++)
            {
                ILayer mLayer = _AppHk.MapControl.get_Layer(i);
                if (mLayer is IGroupLayer)
                {
                    if (mLayer.Name == "��ʷ������")
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ʷ������ͼ���Ѿ����ڡ�");
                        return;
                    }
                }
            }

            Plugin.Application.IAppFormRef pArrForm = _AppHk as Plugin.Application.IAppFormRef;
            pArrForm.MainForm.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            //��ȡ���¿������ݼ�
            if (_AppHk.DBXmlDocument == null)
            {
                pArrForm.MainForm.Cursor = System.Windows.Forms.Cursors.Default;
                return;
            }
            XmlNode DBNode = _AppHk.DBXmlDocument.SelectSingleNode(".//��Ŀ����");
            if (DBNode == null)
            {
                return;
            }
            XmlElement DBElement = DBNode as XmlElement;
            XmlElement objNode = DBNode.SelectSingleNode(".//��ʷ��������") as XmlElement;//yjl20120510 ���Ϊѡ����ʷ��������

            SysCommon.Gis.SysGisDataSet pObjSysGisDataSet = new SysCommon.Gis.SysGisDataSet();
            pObjSysGisDataSet.SetWorkspace(objNode.GetAttribute("������"), objNode.GetAttribute("������"), objNode.GetAttribute("���ݿ�"), objNode.GetAttribute("�û�"), objNode.GetAttribute("����"), objNode.GetAttribute("�汾"), out exError);
            if (exError != null)
            {
                pArrForm.MainForm.Cursor = System.Windows.Forms.Cursors.Default;
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ʷ����������ʧ��,��ȷ��");
                return;
            }
            //��ȡ������ʾ��Χ
            IGeometry pGeometry = null;
            pRangeFeatLay = ModDBOperator.GetMapFrameLayer("zone", _AppHk.MapControl, "ʾ��ͼ") as IFeatureLayer;
            if (pRangeFeatLay == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�޷���ȡ������ʾ��Χ��");
                return;
            }
            //�жϵ�ǰ�Ƿ���ʾ�ύ�ķ�Χ��
            IFeatureCursor pFeatureCursor = pRangeFeatLay.Search(null, false);
            if (pFeatureCursor != null)
            {
                IFeature pFeature = pFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    if (pGeometry == null)
                    {
                        pGeometry = pFeature.Shape;
                    }
                    else
                    {
                        pGeometry = (pGeometry as ITopologicalOperator).Union(pFeature.Shape);
                    }
                    pFeature = pFeatureCursor.NextFeature();
                }
            }
            if (pGeometry == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�޷���ȡ������ʾ��Χ��");
                return;
            }
            //��ȡ���и������ݵ�FeatureClass
            IFeatureDataset pFeaDataset = pObjSysGisDataSet.GetFeatureDataset("h_njtdt", out exError);// ����ط�Ҫ�ؼ���������д���� ��ʱû�а취��ù��������Ϣ ��Ҫ�޸� ����ΰ 20091211
            if (pFeaDataset == null)
            {
                pArrForm.MainForm.Cursor = System.Windows.Forms.Cursors.Default;
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�޷����ָ�������ƿ����ݡ�H_NJTDT����");//yjl20120503 
                return;
            }

            //���Ż���
            //SymbolLyr symLyr = new SymbolLyr();   //@@@

            List<IDataset> listFC = pObjSysGisDataSet.GetFeatureClass(pFeaDataset);
            IGroupLayer pLayer = new GroupLayerClass();
            pLayer.Name = "��ʷ������";

            foreach (IDataset pDataset in listFC)
            {
                pFeatureClass = pDataset as IFeatureClass;
                if (pFeatureClass == null) continue;
                pFeatureLayer = new FeatureLayerClass();
                if (pFeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                {
                    pFeatureLayer = new FDOGraphicsLayerClass();
                }

                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pDataset.Name;
                pFeatureLayer.ScaleSymbols = false;
                //pFeatureLayer = ModDBOperator.GetSelectionLayer(pTempFeatureLayer, pGeometry);
                //if (pFeatureLayer == null) return;

                //���Ż�ͼ��
                //symLyr.SymbolFeatrueLayer(pFeatureLayer);  //@@@

                //����ͼ��
                if (pFeatureClass.FeatureType == esriFeatureType.esriFTSimple)
                {
                    ModDBOperator.ExpandLegend(pFeatureLayer as ILayer, false);
                }
                pLayer.Add(pFeatureLayer as ILayer);
            }
            _AppHk.MapControl.Map.AddLayer(pLayer);
            pObjSysGisDataSet.CloseWorkspace(true);

            //��ͼ���������
            SysCommon.Gis.ModGisPub.LayersCompose(pLayer);

            //��ͼ����ϱ����ڵײ�
            //ModDBOperator.MoveMapFrameLayer(_AppHk.MapControl);
            _AppHk.TOCControl.Update();
            _AppHk.MapControl.Map.ClipGeometry = pGeometry;
            _AppHk.MapControl.ActiveView.Refresh();

            pArrForm.MainForm.Cursor = System.Windows.Forms.Cursors.Default;
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGISRef;
        }
    }
}
