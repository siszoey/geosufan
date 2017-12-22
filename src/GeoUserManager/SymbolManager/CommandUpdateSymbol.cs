using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace GeoUserManager
{
    public class CommandUpdateSymbol : Fan.Plugin.Interface.CommandRefBase
    {
        private Fan.Plugin.Application.IAppGisUpdateRef _AppHk;

        public CommandUpdateSymbol()
        {
            base._Name = "GeoUserManager.CommandUpdateSymbol";
            base._Caption = "���·���";
            base._Tooltip = "���·���";
            base._Checked = false;
            base._Visible = true;
            base._Enabled =false;
            base._Message = "���·���";
        }

        /// <summary>
        /// ͼ���д�������ʱ����״̬Ϊ����ʱ�ſ���
        /// </summary>
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
                Fan.Plugin.Application.IAppFormRef pAppFormRef = _AppHk as Fan.Plugin.Application.IAppFormRef;
                if (pAppFormRef != null)
                {
                    pAppFormRef.OperatorTips = base._Message;
                }
                return base._Message;
            }
        }

        public override void ClearMessage()
        {
            Fan.Plugin.Application.IAppFormRef pAppFormRef = _AppHk as Fan.Plugin.Application.IAppFormRef;
            if (pAppFormRef != null)
            {
                pAppFormRef.OperatorTips = string.Empty;
            }
        }

        public override void OnClick()
        {
            //��÷�����Ϣ ͨ��mxd
            System.Windows.Forms.OpenFileDialog dir = new System.Windows.Forms.OpenFileDialog();
            dir.FileName = "";
            dir.Filter = "ArcGis��ͼ�ĵ�(*.mxd)|*.mxd";
            dir.Multiselect = false;
            if (dir.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            string strMxdPath = dir.FileName;
            IMapDocument pMapDoc = new MapDocumentClass();
            if (!pMapDoc.get_IsMapDocument(strMxdPath)) return;

            pMapDoc.Open(strMxdPath, "");
            
            //������еķ�����Ϣ ���������л�
            if (SaveMxdSymbolToXML(pMapDoc, System.Windows.Forms.Application.StartupPath + "\\..\\Template\\SymbolInfo.xml"))
            {
                Fan.Common.Error.ErrorHandle.ShowInform("��ʾ", "������Ϣ���³ɹ���");
            }
            else
            {
                Fan.Common.Error.ErrorHandle.ShowInform("��ʾ", "�޷����·�����Ϣ��");
            }
            
        }

        public override void OnCreate(Fan.Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Fan.Plugin.Application.IAppGisUpdateRef;
        }

        private bool SaveMxdSymbolToXML(IMapDocument pMapDoc, string strXMLFile)
        {
            if (pMapDoc == null) return false;
            if (!System.IO.File.Exists(strXMLFile)) return false;

            //��xml�ĵ�
            try
            {
                System.Xml.XmlDocument pXmlDoc = new XmlDocument();
                pXmlDoc.Load(strXMLFile);

                IMap pMap = pMapDoc.get_Map(0);
                for (int i = 0; i < pMap.LayerCount; i++)
                {
                    ILayer pLyr = pMap.get_Layer(i);
                    if (pLyr is IGeoFeatureLayer)
                    {
                        IGeoFeatureLayer pGeoFeaLyr = pLyr as IGeoFeatureLayer;
                        string strLyrName = pLyr.Name;

                        //���������Դ ����Ҫ���������Ϊ׼ ���û�о���ͼ����Ϊ׼
                        if (pGeoFeaLyr.FeatureClass != null)
                        {
                            IDataset pDataset = pGeoFeaLyr.FeatureClass as IDataset;
                            strLyrName = pDataset.Name;
                        }

                        strLyrName = strLyrName.Substring(strLyrName.IndexOf('.') + 1);
                        IFeatureRenderer pFeaRender = pGeoFeaLyr.Renderer;
                        UpdateSymbolInfo(strLyrName, pXmlDoc, pFeaRender);
                    }
                }

                pXmlDoc.Save(strXMLFile);
            }
            catch
            {

                return false;
            }

            return true;
        }

        //����xml�ӵ�
        private void UpdateSymbolInfo(string strLyrName,XmlDocument pXmlDoc,IFeatureRenderer pFeaRender)
        {
            if (pXmlDoc == null) return;

            XmlElement pElement = pXmlDoc.SelectSingleNode("//" + strLyrName) as XmlElement;
            XmlElement pRoot = pXmlDoc.SelectSingleNode("//SYMBOLINFO") as XmlElement;
            if (pElement == null)
            {
                pElement = pXmlDoc.CreateElement(strLyrName);
                pRoot.AppendChild(pElement);
            }

            string strXml = "";
            Fan.Common.XML.XMLClass.XmlSerializer(pFeaRender, "", out strXml);
            pElement.RemoveAll();
            pElement.AppendChild(pXmlDoc.CreateTextNode(strXml));

        }

    }
}
