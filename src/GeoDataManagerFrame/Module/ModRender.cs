//*********************************************************************************
//** �ļ�����ModRender.cs
//** CopyRight (c) 2000-2007 �人������Ϣ���̼������޹�˾���̲�
//** �����ˣ�chulili
//** ��  �ڣ�2011-03-15
//** �޸��ˣ�
//** ��  �ڣ�
//** ��  ��������ͼ����Ż� 
//**
//** ��  ����1.0
//*********************************************************************************
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
using GeoDataCenterFunLib;
namespace GeoDataManagerFrame
{
    public class ModRender
    {   
        //��������
        public static void SetRenderByXML(ILayer pLayer)
        {
            SetRenderByXML( pLayer,null);
        }
        //added by chulili
        //�������ܣ�����xml�����ļ�Ϊͼ�����÷���(xml�����ļ���ʱ����ָ��ȫ·��)
        //���������ͼ�� ��־�ļ�  �����������
        public static void SetRenderByXML(ILayer pLayer,LogFile Log)
        {
            string serverstylename;     //���ſ�����

            XmlDocument xmldoc = new XmlDocument();
            string strCurFile = Application.StartupPath + "\\..\\Template\\Render.xml";
            //���������ļ��й��ڸ�ͼ����Ϣ
            string strSearchExp = "//GisLayer [@ItemName='" + pLayer.Name  + "']";
            XmlNode xmlRenderNode;
            if (File.Exists(strCurFile) == false)
            {
                if (Log != null) Log.Writelog("���������ļ�������");
                return;
            }
            if (Log!=null) Log.Writelog("���ط��������ļ�");
            //��ȡxml�����ļ�
            xmldoc.Load(strCurFile);
            xmlRenderNode = xmldoc.SelectSingleNode(strSearchExp);
            if (xmlRenderNode == null)
            {
                if (Log != null) Log.Writelog("���������ļ��в�����ͼ��ڵ�");
                return;
            }
            if (Log != null) Log.Writelog("��ȡ���������ļ���Ϣ");
            XmlElement xmlElent = (XmlElement)xmlRenderNode;
            string strStyle, ColName,defaultname,defaultLab;
            //��ȡͼ��ڵ�����ֵ
            strStyle = xmlElent.GetAttribute("sTyle");
            ColName = xmlElent.GetAttribute("ColName");         //���Ż����ݵ��ֶ�����
            defaultname = xmlElent.GetAttribute("DefaultSymbol");//Ĭ�Ϸ���
            defaultLab = xmlElent.GetAttribute("DefaultLabel");  //Ĭ�Ϸ��ŵı�ǩ
            IFeatureLayer pFLayer = pLayer as IFeatureLayer;
            IFeatureClass fcls = pFLayer.FeatureClass;
            IUniqueValueRenderer ptmpRender;
            ptmpRender = new UniqueValueRendererClass();
            //�̶�����һ���ֶν��з��Ż������÷��Ż��ֶ�
            ptmpRender.FieldCount = 1;
            ptmpRender.set_Field(0, ColName);
            ptmpRender.RemoveAllValues();
            //��ȡ���ſ⣨��ʱʹ�ù̶��ķ��ſ⣬�Ժ�Ӧ�ÿ����ã�
            string stylefileFullname = Application.StartupPath + @"\..\Styles\testStyle.ServerStyle";
            string colvalue, symbolname, labelname;
            if (Log != null) Log.Writelog("��ȡ���ſ�,Ϊͼ�����÷���");
            foreach (XmlNode xmlchild in xmlRenderNode.ChildNodes)//����xml����ÿ��symbol
            {
                if (xmlchild.NodeType.ToString().Equals("Element"))
                {   
                    //��ȡ���Žڵ������
                    xmlElent = (XmlElement)xmlchild;
                    colvalue = xmlElent.GetAttribute("ColValue");   //�ֶ�ֵ
                    symbolname = xmlElent.GetAttribute("Symbol");   //������
                    labelname = xmlElent.GetAttribute("Label");     //���ű�ǩ
                    //���ݷ��ſ�ȫ·����������𣬷�������ȡ����
                    ISymbol pSymbol = GetSymbol(stylefileFullname, strStyle, symbolname);
                    //��ӷ���
                    ptmpRender.AddValue(colvalue, "", pSymbol);
                    //���÷��ű�ǩ
                    ptmpRender.set_Label(colvalue, labelname);
                }
            }
            //����Ĭ�Ϸ��ţ���������ֶ�ֵ������������Ⱦ����������£�������õķ��ţ�
            ISymbol pDefaultSym = GetSymbol(stylefileFullname, strStyle, defaultname);
            ptmpRender.DefaultSymbol = pDefaultSym;
            ptmpRender.DefaultLabel = defaultLab;
            if (ptmpRender.DefaultSymbol!=null)
                ptmpRender.UseDefaultSymbol=true;
            (pFLayer as IGeoFeatureLayer).Renderer = ptmpRender as IFeatureRenderer;
            if (Log != null) Log.Writelog("ͼ������������");

        }
        //added by chulili 
        //�������ܣ���ȡ���ſ��еķ���
        //������������ſ�ȫ·�� �������  ������
        //�������������
        //������Դ�����ͬ�´���
        private static ISymbol GetSymbol(string stylefileFullname, string styleClassName, string sSymbolName)
        {


            ISymbol pSymbol = null;
            IStyleGallery pStyleGallery = new ServerStyleGalleryClass();
            IStyleGalleryStorage pStyleGalleryStorage;
            IEnumStyleGalleryItem pEnumStyleGalleryItem = new EnumServerStyleGalleryItemClass();
            pStyleGalleryStorage = pStyleGallery as IStyleGalleryStorage;
            pStyleGalleryStorage.AddFile(stylefileFullname);

            pEnumStyleGalleryItem = pStyleGallery.get_Items(styleClassName, stylefileFullname, "");
            pEnumStyleGalleryItem.Reset();
            IStyleGalleryItem pEnumItem;
            pEnumItem = pEnumStyleGalleryItem.Next();
            while (pEnumItem != null)
            {
                if (pEnumItem.Name == sSymbolName)
                {
                    pSymbol = pEnumItem.Item as ISymbol;

                    break;
                }
                pEnumItem = pEnumStyleGalleryItem.Next();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pEnumStyleGalleryItem);
            return pSymbol;
        }
    }
}
