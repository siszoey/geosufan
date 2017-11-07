using System;
using System.Collections.Generic;
using System.Text;
//using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System.Xml;

namespace GeoUserManager
{
    public class CommandSaveToDatabase : Plugin.Interface.CommandRefBase
    {
        private Plugin.Application.IAppGisUpdateRef _AppHk;

        public CommandSaveToDatabase()
        {
            base._Name = "GeoUserManager.CommandSaveToDatabase";
            base._Caption = "�������(�ϴ�)";
            base._Tooltip = "�������(�ϴ�)";
            base._Checked = false;
            base._Visible = true;
            base._Enabled =false;
            base._Message = "�������(�ϴ�)";
        }

        /// <summary>
        /// ͼ���д�������ʱ����״̬Ϊ����ʱ�ſ���
        /// </summary>
        public override bool Enabled
        {
            get
            {
                //if (_AppHk.MapControl == null || _AppHk.TOCControl == null) return false;
                //if (_AppHk.MapControl.LayerCount == 0) return false;
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
                System.Xml.XmlDocument pXmlDoc = new XmlDocument();
                pXmlDoc.Load(System.Windows.Forms.Application.StartupPath + "\\..\\Template\\SymbolInfo.xml");
                if (pXmlDoc == null) return;

                //�ϴ������ſ���ȥ
                Exception Err;
                bool result = false;
                SysCommon.Gis.SysGisDB vGisdb = new SysCommon.Gis.SysGisDB();
                result = vGisdb.SetWorkspace(SdeConfig.Server, SdeConfig.Instance, SdeConfig.Database, SdeConfig.User, SdeConfig.Password, SdeConfig.Version, out Err);
                if (!result) return;

                IWorkspace pWks = vGisdb.WorkSpace;
                if (pWks == null) return;

                ESRI.ArcGIS.esriSystem.IMemoryBlobStream pBlobStream = new ESRI.ArcGIS.esriSystem.MemoryBlobStreamClass();
                byte[] bytes = Encoding.Default.GetBytes(pXmlDoc.OuterXml);
                pBlobStream.ImportFromMemory(ref bytes[0], (uint)bytes.GetLength(0));

                IFeatureWorkspace pFeaWks = pWks as IFeatureWorkspace;
                ITable pTable = pFeaWks.OpenTable("SYMBOLINFO");
                IQueryFilter pQueryFilter = new ESRI.ArcGIS.Geodatabase.QueryFilterClass();
                pQueryFilter.WhereClause = "SYMBOLNAME='ALLSYMBOL'";

                ICursor pCursor = pTable.Search(pQueryFilter, false);
                IRow pRow = pCursor.NextRow();
                if (pRow == null)
                {
                    pRow = pTable.CreateRow();
                }

                pRow.set_Value(pRow.Fields.FindField("SYMBOLNAME"), "ALLSYMBOL");
                pRow.set_Value(pRow.Fields.FindField("SYMBOL"), pBlobStream);
                pRow.set_Value(pRow.Fields.FindField("UPDATETIME"), System.DateTime.Now);

                pRow.Store();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);

                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������Ϣ�ϴ��������ɹ���");
            }
            catch (Exception ex)
            {

                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������Ϣ�ϴ����������ִ���" + ex.Message);
            }
        }

        public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGisUpdateRef;

        }
    }
}
