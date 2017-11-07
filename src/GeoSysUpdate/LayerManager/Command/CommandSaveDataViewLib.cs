using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace GeoSysUpdate
{
    public class CommandSaveDataViewLib : Plugin.Interface.CommandRefBase
    {
          private Plugin.Application.IAppGisUpdateRef _AppHk;

        public CommandSaveDataViewLib()
        {
            base._Name = "GeoSysUpdate.CommandSaveDataViewLib";
            base._Caption = "����ͼ��";
            base._Tooltip = "����ͼ��";
            base._Checked = false;
            base._Visible = true;
            base._Enabled =false;
            base._Message = "����ͼ��";
        }

        /// <summary>
        /// ͼ���д�������ʱ����״̬Ϊ����ʱ�ſ���
        /// </summary>
        public override bool Enabled
        {
            get
            {
                //if (_AppHk.MapControl == null || _AppHk.TOCControl == null) return false;

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
            try
            {
                Plugin.Application.IAppFormRef pAppForm=_AppHk as Plugin.Application.IAppFormRef;
                if(pAppForm==null) return;

                System.Xml.XmlDocument pXmlDoc=ModData.v_DataViewXml;
                if (pXmlDoc == null) return;

                //�ϴ������ſ���ȥ
                IWorkspace pWks = pAppForm.TempWksInfo.Wks;
                if (pWks == null) return;

                ESRI.ArcGIS.esriSystem.IMemoryBlobStream pBlobStream = new ESRI.ArcGIS.esriSystem.MemoryBlobStreamClass();
                byte[] bytes = Encoding.Default.GetBytes(pXmlDoc.OuterXml);
                pBlobStream.ImportFromMemory(ref bytes[0], (uint)bytes.GetLength(0));

                IFeatureWorkspace pFeaWks = pWks as IFeatureWorkspace;
                ITable pTable = pFeaWks.OpenTable("SysSetting");
                IQueryFilter pQueryFilter = new ESRI.ArcGIS.Geodatabase.QueryFilterClass();
                pQueryFilter.WhereClause = "SettingName='DataViewXml'";

                ICursor pCursor = pTable.Search(pQueryFilter, false);
                IRow pRow = pCursor.NextRow();
                if (pRow == null)
                {
                    pRow = pTable.CreateRow();
                }

                pRow.set_Value(pRow.Fields.FindField("SettingName"), "DataViewXml");
                pRow.set_Value(pRow.Fields.FindField("SysSettingValue2"), pBlobStream);

                pRow.Store();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);

                SysCommon.Error.ErrorHandle.ShowInform("��ʾ", "ͼ����Ϣ���óɹ���");
            }
            catch (Exception ex)
            {

                SysCommon.Error.ErrorHandle.ShowInform("��ʾ", "ͼ����Ϣ���ó��ִ���" + ex.Message);
            }
            
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGisUpdateRef;

        }
    }
}
