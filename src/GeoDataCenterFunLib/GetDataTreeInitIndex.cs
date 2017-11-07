using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;
//========================================================
//���ܣ����Res\\Xml\\DataTreeInitIndex.xml���ж�ȡ������ֻ����
//���ߣ�wgf
//ʱ�䣺2011-01-25
//========================================================
namespace GeoDataCenterFunLib
{
    public class GetDataTreeInitIndex
    {
        //����xml�ļ�
        public    string m_strInitXmlPath = Application.StartupPath + "\\..\\Res\\Xml\\DataTreeInitIndex.xml";
  

        //����itemName��ȡtblName����
        public string GetTblNameByItemName(string strItemName)
        {
            string strTblName = "";
            if (strItemName.Equals(""))
                return strTblName;
         
            XmlDocument xmldoc = new XmlDocument();
            if (xmldoc != null)
            {
                if (File.Exists(m_strInitXmlPath))
                {
                    xmldoc.Load(m_strInitXmlPath);
                    string strSearch = "//Layer[@ItemName=" + "'" + strItemName + "'" + "]";
                 //   string strSearch = "//Layer[@ItemName=" + strItemName;
                    XmlNode xmlNode = xmldoc.SelectSingleNode(strSearch);
                    if (xmlNode != null)
                    {
                        XmlElement xmlElt = (XmlElement)xmlNode;
                        strTblName = xmlElt.GetAttribute("tblName");
                    }
                }
            }
            return strTblName;
        }

        //�������ļ��л�ȡ��Ϣ
        //strItemTag       ��ǩ
        //strItemName      Ԫ������
        //����             Ԫ��ֵ
        public string GetXmlElementValue(string strItemTag,string strItemName)
        {
            string strVaule = "";
            if (strItemName.Equals(""))
                return strVaule;
            XmlDocument xmldoc = new XmlDocument();
            if (xmldoc != null)
            {
                if (File.Exists(m_strInitXmlPath))
                {
                    xmldoc.Load(m_strInitXmlPath);
                    string strSearch = "//" + strItemTag;
                    XmlNode xmlNode = xmldoc.SelectSingleNode(strSearch);
                    if (xmlNode != null)
                    {
                        XmlElement xmlElt = (XmlElement)xmlNode;
                        strVaule = xmlElt.GetAttribute(strItemName);
                    }
                }
            }
            return strVaule;
        }

        //��ȡ���ݿ��¼��Ϣ
        public string GetDbInfo()
        {
            string strVaule = "";
            string strBuffer = "";
            XmlDocument xmldoc = new XmlDocument();
            if (xmldoc != null)
            {
                if (File.Exists(m_strInitXmlPath))
                {
                    xmldoc.Load(m_strInitXmlPath);
                    string strSearch = "//DbInfo";
                    XmlNode xmlNode = xmldoc.SelectSingleNode(strSearch);
                    if (xmlNode != null)
                    {
                        XmlElement xmlElt = (XmlElement)xmlNode;
                        strBuffer = xmlElt.GetAttribute("dbInfoPath");
                    }
                }
                strVaule = Application.StartupPath + strBuffer;
            }
            return strVaule;
        }

        //��ȡ��ǰ���ݿ�������Ϣ
        //strItemName  ��ӦdbType��dbServerPath��dbServerName��dbUser��dbPassword
        public string GetDbValue(string strItemName)
        {
            string strVaule = "";
            if (strItemName.Equals(""))
                return strVaule;
            XmlDocument xmldoc = new XmlDocument();
            if (xmldoc != null)
            {
                if (File.Exists(m_strInitXmlPath))
                {
                    xmldoc.Load(m_strInitXmlPath);
                    string strSearch = "//CurDbSet";
                    XmlNode xmlNode = xmldoc.SelectSingleNode(strSearch);
                    if (xmlNode != null)
                    {
                        XmlElement xmlElt = (XmlElement)xmlNode;
                        strVaule = xmlElt.GetAttribute(strItemName);
                    }
                }
            }
            return strVaule;
        }

        //�޸ĵ�ǰ�����ݿ�������Ϣ
        //strItemName  ��ӦdbType��dbServerPath��dbServerName��dbUser��dbPassword
        public void SetDbValue(string strItemName,string strVaule)
        {
            if (strItemName.Equals(""))
                return ;
            XmlDocument xmldoc = new XmlDocument();
            if (xmldoc != null)
            {
                if (File.Exists(m_strInitXmlPath))
                {
                    xmldoc.Load(m_strInitXmlPath);
                    string strSearch = "//CurDbSet";
                    XmlNode xmlNode = xmldoc.SelectSingleNode(strSearch);
                    if (xmlNode != null)
                    {
                        XmlElement xmlElt = (XmlElement)xmlNode;
                        xmlElt.SetAttribute(strItemName, strVaule);
                        xmldoc.Save(m_strInitXmlPath);
                    }
                }
            }
            return ;
        }
    }

   
}
