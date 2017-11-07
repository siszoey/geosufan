using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Carto;
using System.Xml;
using ESRI.ArcGIS.Display;
using stdole;

namespace SysCommon
{
    public static class ModuleLabel
    {
        public static void SetLayerInfoFromXml(IGeoFeatureLayer pGeaFeatureLayer)
        {
            IAnnotateLayerPropertiesCollection pAnnoLayerProperCol = pGeaFeatureLayer.AnnotationProperties;
            pAnnoLayerProperCol.Clear();

            ILabelEngineLayerProperties pLabelEngineLayerPro = new LabelEngineLayerPropertiesClass();
            IAnnotateLayerProperties pAnnoLayerProperties = pLabelEngineLayerPro as IAnnotateLayerProperties;

            //��ͼ��ڵ��XML�ж�ȡ��ע���ã��Ḳ�������������ý����
            try
            {
                XmlNode nodeLayerXml = GetSelectNodeXml(pGeaFeatureLayer);
                if (nodeLayerXml != null) ReadLabelConfigFromXML(ref pAnnoLayerProperties, nodeLayerXml, pGeaFeatureLayer.FeatureClass.ShapeType.ToString());
                pAnnoLayerProperCol.Add(pAnnoLayerProperties);
                
                //shduan 20110623 ���
                pGeaFeatureLayer.DisplayAnnotation = true;
            }
            catch
            {

            }
        }

        public static XmlNode GetSelectNodeXml(ILayer pLayer)
        {
            ILayerGeneralProperties pLayerGenPro = pLayer as ILayerGeneralProperties;

            string sValue = pLayerGenPro.LayerDescription;
            if (sValue.Equals(string.Empty)) return null;

            XmlDocument docXml = new XmlDocument();
            docXml.LoadXml(sValue);

            XmlNode nodeSelectXml = docXml.DocumentElement;

            return nodeSelectXml;
        }

        /// <summary>
        /// ��ͼ���Ӧ�ڵ��XML�ж�ȡ��ǰ��ı�ע����
        /// </summary>
        /// <param name="pAnnoLayerProper"></param>
        public static void ReadLabelConfigFromXML(ref IAnnotateLayerProperties pAnnoLayerProper, XmlNode nodeLayerXml, string pFeatureType)
        {
            //���ñ�ע�������Ϣ             ;
            ILabelEngineLayerProperties pLabelEngineLayerProperties = null;
            if (pAnnoLayerProper == null)
            {
                pLabelEngineLayerProperties = new LabelEngineLayerProperties() as ILabelEngineLayerProperties;
                pAnnoLayerProper = pLabelEngineLayerProperties as IAnnotateLayerProperties;
            }
            else
            {
                pLabelEngineLayerProperties = pAnnoLayerProper as ILabelEngineLayerProperties;
            }
            
            IAnnotationExpressionEngine pAnnoVBScriptEngine = new AnnotationVBScriptEngineClass();
            pLabelEngineLayerProperties.ExpressionParser = pAnnoVBScriptEngine;

            IBasicOverposterLayerProperties4 pBasicOverposterLayerProperties = pLabelEngineLayerProperties.BasicOverposterLayerProperties as IBasicOverposterLayerProperties4;
            
            switch (pFeatureType)
            {
                case "esriGeometryPoint":
                    pBasicOverposterLayerProperties.FeatureType = esriBasicOverposterFeatureType.esriOverposterPoint;
                    break;
                case "esriGeometryPolyline":
                    pBasicOverposterLayerProperties.FeatureType = esriBasicOverposterFeatureType.esriOverposterPolyline;
                    break;
                case "esriGeometryPolygon":
                    pBasicOverposterLayerProperties.FeatureType = esriBasicOverposterFeatureType.esriOverposterPolygon;
                    break;
            }

            ITextSymbol pTextSymbol = pLabelEngineLayerProperties.Symbol;
            IFontDisp pFont = pTextSymbol.Font;
            foreach (XmlNode nd in nodeLayerXml)
            {
                if (nd.Name == "AttrLabel")
                {
                    //��ȡ��ͬ�ı�ע������Ϣ
                    pFont.Name = nd.Attributes["FontName"].Value;
                    pFont.Size = Convert.ToDecimal(nd.Attributes["FontSize"].Value.ToLower());
                    if (nd.Attributes["FontBold"].Value != "")
                        pFont.Bold = Convert.ToBoolean(nd.Attributes["FontBold"].Value.ToLower());
                    if (nd.Attributes["FontItalic"].Value != "")
                        pFont.Italic = Convert.ToBoolean(nd.Attributes["FontItalic"].Value.ToLower());
                    if(nd.Attributes["FontUnderLine"].Value!="")
                        pFont.Underline = Convert.ToBoolean(nd.Attributes["FontUnderLine"].Value.ToLower());
                    pTextSymbol.Font = pFont;
                    IColor  pColor = new RgbColorClass();
                    if (nd.Attributes["FontBoldColor"].Value != "")
                        pColor.RGB = Convert.ToInt32(nd.Attributes["FontBoldColor"].Value);
                    else
                    {
                        pColor.RGB = 36054566;
                        //pColor.Green = 0;
                        //pColor.Blue = 0;
                        pTextSymbol.Color = pColor as IColor;
                    }
                    pTextSymbol.Color = pColor;


                    if (nd.Attributes["Expression"].Value != "")
                        pLabelEngineLayerProperties.Expression = "[" + nd.Attributes["Expression"].Value + "]"; 

                    if (nd.Attributes["MaxScale"].Value != "" && nd.Attributes["MaxScale"].Value != null)
                    {
                        pAnnoLayerProper.AnnotationMaximumScale = Convert.ToDouble(nd.Attributes["MaxScale"].Value);
                    }
                    if (nd.Attributes["MinScale"].Value != "" && nd.Attributes["MinScale"].Value != null)
                    {
                        pAnnoLayerProper.AnnotationMinimumScale = Convert.ToDouble(nd.Attributes["MinScale"].Value);
                    }

                    if (nd.Attributes["NumLabelsOption"].Value == "esriOneLabelPerName")
                        pBasicOverposterLayerProperties.NumLabelsOption = esriBasicNumLabelsOption.esriOneLabelPerName;
                    else if (nd.Attributes["NumLabelsOption"].Value == "esriOneLabelPerPart")
                        pBasicOverposterLayerProperties.NumLabelsOption = esriBasicNumLabelsOption.esriOneLabelPerPart;
                    else if (nd.Attributes["NumLabelsOption"].Value == "esriOneLabelPerShape")
                        pBasicOverposterLayerProperties.NumLabelsOption = esriBasicNumLabelsOption.esriOneLabelPerShape;
                    //���水��ͬҪ�����Ͷ�ȡ��ͬ�ı�ע������Ϣ
                    //��Ҫ��
                    if (pBasicOverposterLayerProperties.FeatureType == esriBasicOverposterFeatureType.esriOverposterPoint)
                    {
                        pBasicOverposterLayerProperties.PointPlacementOnTop = Convert.ToBoolean(nd.Attributes["PointPlacementOnTop"].Value.ToLower());
                        if (nd.Attributes["PointPlacementMethod"].Value == "esriAroundPoint")
                        {
                            pBasicOverposterLayerProperties.PointPlacementMethod = esriOverposterPointPlacementMethod.esriAroundPoint;

                            IPointPlacementPriorities pPointPlacementPriorities = pBasicOverposterLayerProperties.PointPlacementPriorities;
                            string strPlacement;
                            try
                            {
                                //��ȡ��עλ�õ�����
                                strPlacement = nd.Attributes["PointPlacementPriorities"].Value;
                                SetPlacementPriotities(pPointPlacementPriorities, strPlacement);
                                pBasicOverposterLayerProperties.PointPlacementPriorities = pPointPlacementPriorities;
                            }
                            catch
                            {
                                //MessageBox.Show("��ͼ��XML�в�����PointPlacementPriorities���ԣ�");
                            }
                        }
                        else if (nd.Attributes["PointPlacementMethod"].Value == "esriOnTopPoint")
                            pBasicOverposterLayerProperties.PointPlacementMethod = esriOverposterPointPlacementMethod.esriOnTopPoint;
                        else if (nd.Attributes["PointPlacementMethod"].Value == "esriSpecifiedAngles")
                        {
                            double[] dArray = new double[1];
                            string sAngle = nd.Attributes["PointPlacementAngles"].Value;
                            if (sAngle.Contains(",") == false)
                                dArray[0] = Convert.ToDouble(sAngle);
                            else
                            {
                                string[] sAngles = sAngle.Split(new char[] { ',' });
                                dArray = new double[sAngles.Length];
                                for (int i = 0; i < sAngles.Length; i++)
                                {
                                    dArray[i] = Convert.ToDouble(sAngles[i]);
                                }
                            }
                            pBasicOverposterLayerProperties.PointPlacementAngles = dArray;
                        }
                        else if (nd.Attributes["PointPlacementMethod"].Value == "esriRotationField")
                        {
                            pBasicOverposterLayerProperties.PointPlacementMethod = esriOverposterPointPlacementMethod.esriRotationField;
                            pBasicOverposterLayerProperties.RotationField = nd.Attributes["RotationField"].Value;
                            if (nd.Attributes["RotationType"].Value == "esriRotateLabelArithmetic")
                                pBasicOverposterLayerProperties.RotationType = esriLabelRotationType.esriRotateLabelArithmetic;
                            else if (nd.Attributes["RotationType"].Value == "esriRotateLabelGeographic")
                                pBasicOverposterLayerProperties.RotationType = esriLabelRotationType.esriRotateLabelGeographic;

                            pBasicOverposterLayerProperties.PerpendicularToAngle = Convert.ToBoolean(nd.Attributes["PerpendicularToAngle"].Value);
                        }
                    }
                    //��Ҫ��
                    else if (pBasicOverposterLayerProperties.FeatureType == esriBasicOverposterFeatureType.esriOverposterPolyline)
                    {

                        ILineLabelPosition pLineLabelPosition = pBasicOverposterLayerProperties.LineLabelPosition;
                        if (nd.Attributes["Above"] != null)
                            pLineLabelPosition.Above = Convert.ToBoolean(nd.Attributes["Above"].Value);
                        if (nd.Attributes["AtEnd"] != null)
                            pLineLabelPosition.AtEnd = Convert.ToBoolean(nd.Attributes["AtEnd"].Value);
                        if (nd.Attributes["AtStart"] != null)
                            pLineLabelPosition.AtStart = Convert.ToBoolean(nd.Attributes["AtStart"].Value);
                        if (nd.Attributes["Below"] != null)
                            pLineLabelPosition.Below = Convert.ToBoolean(nd.Attributes["Below"].Value);
                        if (nd.Attributes["Horizontal"] != null)
                            pLineLabelPosition.Horizontal = Convert.ToBoolean(nd.Attributes["Horizontal"].Value);
                        if (nd.Attributes["InLine"] != null)
                            pLineLabelPosition.InLine = Convert.ToBoolean(nd.Attributes["InLine"].Value);
                        if (nd.Attributes["Left"] != null)
                            pLineLabelPosition.Left = Convert.ToBoolean(nd.Attributes["Left"].Value);
                        if (nd.Attributes["Offset"] != null)
                            pLineLabelPosition.Offset = Convert.ToDouble(nd.Attributes["Offset"].Value);
                        if (nd.Attributes["OnTop"] != null)
                            pLineLabelPosition.OnTop = Convert.ToBoolean(nd.Attributes["OnTop"].Value);
                        if (nd.Attributes["Parallel"] != null)
                            pLineLabelPosition.Parallel = Convert.ToBoolean(nd.Attributes["Parallel"].Value);
                        if (nd.Attributes["Perpendicular"] != null)
                            pLineLabelPosition.Perpendicular = Convert.ToBoolean(nd.Attributes["Perpendicular"].Value);
                        if (nd.Attributes["ProduceCurvedLabels"] != null)
                            pLineLabelPosition.ProduceCurvedLabels = Convert.ToBoolean(nd.Attributes["ProduceCurvedLabels"].Value);
                        if (nd.Attributes["Right"] != null)
                            pLineLabelPosition.Right = Convert.ToBoolean(nd.Attributes["Right"].Value);
                    }
                    //��Ҫ��
                    else if (pBasicOverposterLayerProperties.FeatureType == esriBasicOverposterFeatureType.esriOverposterPolygon)
                    {
                        if (nd.Attributes["PolygonPlacementMethod"].Value == "esriAlwaysHorizontal")
                            pBasicOverposterLayerProperties.PolygonPlacementMethod = esriOverposterPolygonPlacementMethod.esriAlwaysHorizontal;
                        else if (nd.Attributes["PolygonPlacementMethod"].Value == "esriAlwaysStraight")
                            pBasicOverposterLayerProperties.PolygonPlacementMethod = esriOverposterPolygonPlacementMethod.esriAlwaysStraight;
                        else if (nd.Attributes["PolygonPlacementMethod"].Value == "esriMixedStrategy")
                            pBasicOverposterLayerProperties.PolygonPlacementMethod = esriOverposterPolygonPlacementMethod.esriMixedStrategy;

                        pBasicOverposterLayerProperties.PlaceOnlyInsidePolygon = Convert.ToBoolean(nd.Attributes["PlaceOnlyInsidePolygon"].Value);
                    }
                }
            }
        }

        private static int GetColorIntFromNode(string strNodeValue)
        {
            int iRGBColor;
            try
            {
                iRGBColor =Convert.ToInt32( strNodeValue.Substring(10, 3));
                return iRGBColor;
            }
            catch(Exception e)
            {
                return 255;
            }
        }

        /// <summary>
        /// �ж��Ƿ��Ǹ��ӱ��ʽ
        /// </summary>
        /// <param name="Expression"></param>
        /// <returns></returns>
        public static bool JudgeIsComplexExpression(string Expression)
        {
            //�������������ַ���ʱ��Ϊ�Ǹ��ӱ��ʽ
            if ((Expression.IndexOf("&") != -1) | (Expression.IndexOf("+") != -1) | (Expression.IndexOf("And") != -1)
                | (Expression.IndexOf("Or") != -1) | (Expression.IndexOf("=") != -1) | (Expression.IndexOf(">") != -1)
                | (Expression.IndexOf("<") != -1))
            {
                return true;
            }
            else
            {
                int i = Expression.IndexOf("[");
                if (i > 0)        //�����λ�ڵ�һ���Ļ���˵�����ʽ�а����к���
                    return true;
                else if (i == 0)
                {
                    i = Expression.LastIndexOf("[", 1);
                    if (i > 0)        //ʣ����ַ����л���[ʱ��˵���Ǹ��ӱ��ʽ
                        return true;
                }
            }
            return false;
            //if ((!Expression.StartsWith("[")) || (!Expression.EndsWith("]")))
            //    return true;
            //Expression = Expression.Substring(1, Expression.Length - 2);
            //for (int i = 0; i < cmbFieldName.Items.Count; i++)
            //{
            //    int index = cmbFieldName.Items[i].ToString().IndexOf('��');
            //    if (index >= 0)
            //    {
            //        if (Expression == cmbFieldName.Items[i].ToString().Substring(0, index))
            //            return false;
            //    }
            //}
            //return true;          
        }

        /// <summary>
        /// ��ȡ�ַ���������PointPlacementPriorities����
        /// </summary>
        /// <param name="pPointPlacementPriorities"></param>
        /// <param name="strPlacement">��"00100000"</param>
        public static void SetPlacementPriotities(IPointPlacementPriorities pPointPlacementPriorities, string strPlacement)
        {
            pPointPlacementPriorities.AboveLeft = Convert.ToInt16(strPlacement.Substring(0, 1));
            pPointPlacementPriorities.AboveCenter = Convert.ToInt16(strPlacement.Substring(1, 1));
            pPointPlacementPriorities.AboveRight = Convert.ToInt16(strPlacement.Substring(2, 1));
            pPointPlacementPriorities.CenterRight = Convert.ToInt16(strPlacement.Substring(3, 1));
            pPointPlacementPriorities.BelowRight = Convert.ToInt16(strPlacement.Substring(4, 1));
            pPointPlacementPriorities.BelowCenter = Convert.ToInt16(strPlacement.Substring(5, 1));
            pPointPlacementPriorities.BelowLeft = Convert.ToInt16(strPlacement.Substring(6, 1));
            pPointPlacementPriorities.CenterLeft = Convert.ToInt16(strPlacement.Substring(7, 1));
        }

        /// <summary>
        /// ��PointPlacementPriorities���Խ������ַ���
        /// </summary>
        /// <param name="pPointPlacementPriorities"></param>
        /// <returns>��"00100000"</returns>
        public static string GetPlacementPriorities(IPointPlacementPriorities pPointPlacementPriorities)
        {
            return pPointPlacementPriorities.AboveLeft.ToString() + pPointPlacementPriorities.AboveCenter.ToString() +
                    pPointPlacementPriorities.AboveRight.ToString() + pPointPlacementPriorities.CenterRight.ToString() +
                    pPointPlacementPriorities.BelowRight.ToString() + pPointPlacementPriorities.BelowCenter.ToString() +
                    pPointPlacementPriorities.BelowLeft.ToString() + pPointPlacementPriorities.CenterLeft.ToString();
        }

    }
}
