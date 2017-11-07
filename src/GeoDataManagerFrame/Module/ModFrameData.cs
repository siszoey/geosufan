using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using SysCommon.Gis;
using System.Xml;
using System.IO;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;

namespace GeoDataManagerFrame
{
    public static class ModFrameData
    {
        public static Plugin.Application.AppGidUpdate v_AppGisUpdate;
        public static IFeatureWorkspace m_pFeatureWorkspace;
      //?  public static IFeatureClass v_CurMapSheetFeatureClass;
      //?  public static frmQueryOperationRecords v_QueryResult;

        //�����λ��
        public static ESRI.ArcGIS.Geometry.IPoint v_CurPoint;

        //������Ϣ
        public static string v_ConfigPath = Application.StartupPath + "\\conn.dat";
        public static string dbType = "";
        public static string Server = "";
        public static string Instance = "";
        public static string Database = "";
        public static string User = "";
        public static string Password = "";
        public static string Version = "DATACENTER.DEFAULT";

        //======================================================
        //ϵͳ������־  cyf 20110518
        public static SysCommon.Log.clsWriteSystemFunctionLog v_SysLog;
        public static string m_SysXmlPath = Application.StartupPath + "\\..\\Res\\Xml\\SysXml.Xml";                 //����ϵͳ����xml
       //ϵͳά����������Ϣ cyf 20110520
        public readonly static string v_AppDBConectXml = Application.StartupPath + "\\AppDBConectInfo.xml";////////////ϵͳά���������ַ��� 
        //end=======================================================
    }

    public enum enumLayType
    {
        LEFT = 1,
        RIGHT = 2,
        TOP = 3,
        BOTTOM = 4,
        FILL = 5,
    }

    /// <summary>
    /// ���Ż�
    /// </summary>
    public class SymbolLyr
    {
        #region ���Ż�ͼ��
        XmlDocument m_vDoc = null;
        public void SymbolFeatrueLayer(IFeatureLayer pFealyr)
        {
            if (!(pFealyr is IGeoFeatureLayer)) return;

            try
            {
                string strXMLpath = System.Windows.Forms.Application.StartupPath + "\\..\\Template\\SymbolInfo.xml";

                string strLyrName = pFealyr.Name;
                if (pFealyr.FeatureClass != null)
                {
                    IDataset pDataset = pFealyr.FeatureClass as IDataset;
                    strLyrName = pDataset.Name;
                }

                strLyrName = strLyrName.Substring(strLyrName.IndexOf('.') + 1);

                if (m_vDoc == null)
                {
                    IFeatureWorkspace pFeaWks = ModFrameData.m_pFeatureWorkspace;
                    ITable pTable = pFeaWks.OpenTable("SYMBOLINFO");
                    IQueryFilter pQueryFilter = new ESRI.ArcGIS.Geodatabase.QueryFilterClass();
                    pQueryFilter.WhereClause = "SYMBOLNAME='ALLSYMBOL'";

                    ICursor pCursor = pTable.Search(pQueryFilter, false);
                    IRow pRow = pCursor.NextRow();
                    if (pRow == null) return;

                    IMemoryBlobStreamVariant var = pRow.get_Value(pRow.Fields.FindField("SYMBOL")) as IMemoryBlobStreamVariant;
                    object tempObj = null;
                    if (var == null) return;

                    var.ExportToVariant(out tempObj);
                    XmlDocument doc = new XmlDocument();
                    byte[] btyes = (byte[])tempObj;
                    string xml = Encoding.Default.GetString(btyes);
                    doc.LoadXml(xml);

                    DateTime updateTime = (DateTime)pRow.get_Value(pRow.Fields.FindField("UPDATETIME"));
                    DateTime Nowtime;

                    bool blnUpdate = false;

                    //��һ����־����
                    string strTimeLog = System.Windows.Forms.Application.StartupPath + "\\..\\Template\\UpdateTime.txt";
                    if (System.IO.File.Exists(strTimeLog))
                    {
                        StreamReader sr = new StreamReader(strTimeLog);
                        string strTime = sr.ReadLine();
                        sr.Close();
                        if (!DateTime.TryParse(strTime, out Nowtime))
                        {
                            blnUpdate = true;
                        }

                        if (updateTime > Nowtime)
                        {
                            if (SysCommon.Error.ErrorHandle.ShowFrmInformation("��", "��", "�������·�����Ϣ���Ƿ���Ҫ���أ�"))
                            {
                                blnUpdate = true;
                            }
                        }
                    }
                    else
                    {
                        blnUpdate = true;
                    }

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
                    pCursor = null;

                    //�����ж��Ƿ���Ҫ���ط�����Ϣ
                    if (System.IO.File.Exists(strXMLpath))
                    {
                        if (blnUpdate)
                        {
                            doc.Save(strXMLpath);
                            StreamWriter sw1 = new StreamWriter(strTimeLog);
                            sw1.Write(updateTime.ToString());
                            sw1.Close();
                        }
                    }
                    else
                    {
                        doc.Save(strXMLpath);
                        StreamWriter sw = new StreamWriter(strTimeLog);
                        sw.Write(updateTime.ToString());
                        sw.Close();
                    }

                    m_vDoc = new XmlDocument();
                    m_vDoc.Load(strXMLpath);
                }
                else
                {
                }

                XmlElement pElement = m_vDoc.SelectSingleNode("//" + strLyrName) as XmlElement;
                if (pElement == null) return;

                IFeatureRenderer pFeaRender = SysCommon.XML.XMLClass.XmlDeSerializer2(pElement.FirstChild.Value) as IFeatureRenderer;
                IGeoFeatureLayer pGeoLyr = pFealyr as IGeoFeatureLayer;
                pGeoLyr.Renderer = pFeaRender;
            }
            catch
            {

            }
        }
        #endregion
    }
}
