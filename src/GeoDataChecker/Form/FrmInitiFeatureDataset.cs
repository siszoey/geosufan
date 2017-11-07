using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesGDB;
using System.Threading;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
namespace GeoDataChecker
{
    /// <summary>
    /// ��������Ҫ�����ݼ������û�оʹ���
    /// </summary>
    public partial class FrmInitiFeatureDataset : DevComponents.DotNetBar.Office2007RibbonForm
    {
        private Plugin.Application.IAppFormRef _AppHk;//�õ�Ҫ�õ��Ľ�����

        public FrmInitiFeatureDataset(Plugin.Application.IAppFormRef AppHk)
        {
            _AppHk = AppHk;
            InitializeComponent();
        }
        /// <summary>
        /// ��һ������Ҫ����������Դ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_org_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "�����ļ�|*.*|MDB�ļ�|*.mdb";
            open.InitialDirectory = @"d:\";
            DialogResult result = open.ShowDialog();//���ļ�
            if (result == DialogResult.OK)
            {
                txt_org.Text = open.FileName;
            }

        }
        /// <summary>
        /// ��һ�������ռ���յ�PRJ�ļ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_prj_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "PRJ�ļ�(*.prj)|*.prj";
            open.InitialDirectory = @"d:\";
            DialogResult result = open.ShowDialog();//���ļ�
            if (result == DialogResult.OK)
            {
                txt_prj.Text = open.FileName;
            }
        }

        /// <summary>
        /// ��ʼ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_enter_Click(object sender, EventArgs e)
        {
            if (txt_prj.Text != "" && txt_org.Text != "")
            {
                txt_prj.Visible = false;
                txt_org.Visible = false;
                btn_cancel.Visible = false;
                btn_enter.Visible = false;
                lab_org.Visible = false;
                labelX1.Visible = false;
                btn_org.Visible = false;
                btn_prj.Visible = false;
                pic_processbar.Visible = true;
                lab_show.Visible = true;
                Thread thread = new Thread(CreateFeatureClass);
                thread.Start();

            }
            else
            {
                SetCheckState.Message(_AppHk,"��ʾ", "��ѡ��·����");
            }
        }
        /// <summary>
        /// ����һ��Ҫ�ؼ�
        /// </summary>
        private void CreateFeatureClass()
        {
            string path = txt_org.Text;
            string PRJFile = txt_prj.Text;
            AccessWorkspaceFactory pAccessFact = new AccessWorkspaceFactoryClass();

            IFeatureWorkspace pFeatureWorkspace = pAccessFact.OpenFromFile(path, 0) as IFeatureWorkspace;
            ISpatialReferenceFactory2 Isp = new SpatialReferenceEnvironmentClass();//����һ���ռ���յĽӿڿռ�

            ISpatialReference spatial = Isp.CreateESRISpatialReferenceFromPRJFile(PRJFile);//����Ҫ�����PRJ�ļ�����
            ISpatialReferenceResolution pSRR = (ISpatialReferenceResolution)spatial;//���÷ֱ���
            pSRR.SetDefaultXYResolution();//����Ĭ��XYֵ

            ISpatialReferenceTolerance pSRT = (ISpatialReferenceTolerance)spatial;
            pSRT.SetDefaultXYTolerance();//����Ĭ���ݲ�ֵ
            IWorkspace space = pFeatureWorkspace as IWorkspace;

            IEnumDatasetName Dataset_name = space.get_DatasetNames(esriDatasetType.esriDTAny);//�õ��ж��ٸ�Ҫ�ؼ�������
            Dataset_name.Reset();
            IDatasetName Name_set = Dataset_name.Next();
            while (Name_set != null)
            {
                if (Name_set.Name == "Geo_Topo_ice")
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݼ��Ѵ���,���ô�����");
                    this.Close();//���Ҫ���������ݼ��Ѵ��ڣ��ͷ��أ����رմ���
                    return;
                }
                Name_set = Dataset_name.Next();
            }
            IFeatureDataset pfd = pFeatureWorkspace.CreateFeatureDataset("Geo_Topo_ice", spatial);//����һ��Ҫ�ؼ�

            IEnumDataset dst = space.get_Datasets(esriDatasetType.esriDTAny);//�õ����е�Ҫ�����һ������
            dst.Reset();
            IDataset det = dst.Next();
            _AppHk.OperatorTips = "��ʼ������Ӧ��Ҫ����...";
            while (det != null)
            {
                #region ��Ҫ�ؼ������յ�Ҫ����
                if (det.Type == esriDatasetType.esriDTFeatureClass)//�ж��ǲ���Ҫ����
                {

                    
                    string org_name = det.Name;//ԭʼ������
                    _AppHk.OperatorTips = "��ʼ����" + org_name + "Ҫ����...";
                    IFeatureClass f_class = pFeatureWorkspace.OpenFeatureClass(org_name);//��ԴҪ����
                    det.Rename(org_name + "_t");//��ԴҪ�������������
                    IFields Fieldset = new FieldsClass();//����һ���ֶμ�
                    IFieldsEdit sField = Fieldset as IFieldsEdit;//�ֶμ�
                    if (f_class.FeatureType != esriFeatureType.esriFTAnnotation)
                    {
                        //shape
                        IGeometryDefEdit d_edit;//����һ����������Ҫ�����SHAPE����
                        d_edit = new GeometryDefClass();//ʵ��һ��������
                        d_edit.GeometryType_2 = f_class.ShapeType;//��ԴҪ�����SHAPE��ֵ������Ҫ�����ļ�������
                        d_edit.SpatialReference_2 = spatial;//�ռ�ο�

                        string OID = f_class.OIDFieldName;//ODI����
                        string SHAPE = f_class.ShapeFieldName;//SHAPE����

                        //IFields Fieldset = new FieldsClass();//����һ���ֶμ�
                        //IFieldsEdit sField = Fieldset as IFieldsEdit;//�ֶμ�


                        //����Ҫ��������ֶ�
                        int count = f_class.Fields.FieldCount;//ȷ���ж��ٸ��ֶ�

                        #region �����ֶ�
                        for (int n = 0; n < count; n++)
                        {
                            IField f_ield = f_class.Fields.get_Field(n);

                            IFieldEdit fieldEdit = f_ield as IFieldEdit;
                            //Annotate
                            if (f_ield.Name == SHAPE)
                            {
                                //shape field
                                fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;//ȷ���ֶε�����
                                fieldEdit.GeometryDef_2 = d_edit;//�Ѽ������͸�ֵ����
                                fieldEdit.Name_2 = SHAPE;//�Ѽ�������SHPAE�����ָ�ֵ����
                                f_ield = fieldEdit as IField;
                                sField.AddField(f_ield);//����Ҫ�ؼ�
                            }
                            else if (f_ield.Name == OID)
                            {

                                //oid
                                fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;//OID��ʶ�ֶ�
                                fieldEdit.Name_2 = OID;//OID������
                                f_ield = fieldEdit as IField;
                                sField.AddField(f_ield);//����OID

                            }
                            else
                            {
                                //һ���ֶ�
                                fieldEdit.Name_2 = f_ield.Name;
                                fieldEdit.Type_2 = f_ield.Type;//�ֶε�����

                                f_ield = fieldEdit as IField;
                                sField.AddField(f_ield);
                            }

                        }
                        #endregion

                        Fieldset = sField as IFields;//���ɱ༭���ֶμ�ת��һ���ֶμ�
                        pfd.CreateFeatureClass(org_name, Fieldset, f_class.CLSID, null, esriFeatureType.esriFTSimple, SHAPE, "");//��Ҫ�ؼ��д���Ҫ����
                    }
                    else
                    {
                            
                            createAnnoFeatureClass(org_name, pfd, pfd.Workspace as IFeatureWorkspace, sField, 2000);
                    }

                    det = dst.Next();//���±�����һ��
                }
                else
                {
                    det = dst.Next();//���±�����һ��
                }
                #endregion

            }
            _AppHk.OperatorTips = "Ҫ�ؼ��ϴ����ɹ���";
            GetValue(pFeatureWorkspace);//����Ӧ��Ҫ���ཨ���ú󣬾Ϳ�ʼ����Ҫ���ำֵ
            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݴ�����ɣ�");
            this.Close();
            

        }

        /// <summary>
        /// ��ֵ�����´�����Ҫ�ؼ���
        /// </summary>
        private void GetValue(IFeatureWorkspace pFeatureWorkspace)
        {
            IFeatureDataset ds = pFeatureWorkspace.OpenFeatureDataset("Geo_Topo_ice");
            IEnumDataset dd = ds.Subsets;//�õ�Ҫ�����ݼ��µ�����Ҫ����ļ���
            dd.Reset();
            IDataset dt = dd.Next();
            if (dt == null) return;
            while (dt != null)
            {
                string des_name = dt.Name;//Ŀ��Ҫ���������
                string temp = des_name + "_t";//�õ���ӦԴ��CLASS 
                try
                {
                    _AppHk.OperatorTips = "��ʼ�Կյ�Ҫ�ؼ������" + des_name + "Ҫ������и�ֵ����...";
                    IFeatureClass org_class = pFeatureWorkspace.OpenFeatureClass(temp);//�õ�Դ��Ҫ����
                    IFeatureClass des_class = pFeatureWorkspace.OpenFeatureClass(des_name);//Ŀ��Ҫ����

                    #region ȷ��Դ��Ϊ�ս������в���
                    if (org_class != null)
                    {

                        IFeatureBuffer pFeaturebuffer = des_class.CreateFeatureBuffer();//����һ��BUFFER
                        IFeatureCursor pCursor = des_class.Insert(true);//����һ���α�

                        IFeatureCursor pCursor_y = org_class.Search(null, false);//�α꿪ʼ
                        IFeature Feat = pCursor_y.NextFeature();//������ʼ
                        ///������ֵ
                        #region ������ֵ
                        while (Feat != null)
                        {

                            int n = Feat.Fields.FieldCount;
                            for (int r = 0; r < n; r++)
                            {
                                //�����б�Ҫ�ص�����
                                if (Feat.Fields.get_Field(r).Type == esriFieldType.esriFieldTypeGeometry || Feat.Fields.get_Field(r).Editable == false)
                                {
                                    continue;
                                }
                                else
                                {
                                    int index = pFeaturebuffer.Fields.FindField(Feat.Fields.get_Field(r).Name);//��ԴҪ����Ŀ��Ҫ���ֶζ�Ӧ���������ֶ�����������
                                    if (index != -1)
                                    {
                                        pFeaturebuffer.set_Value(index, Feat.get_Value(r));//��Ŀ��Ҫ������ԴҪ�ض�Ӧ��ֵ
                                    }
                                }

                            }
                            pFeaturebuffer.Shape = Feat.ShapeCopy;//����ļ������Ը�Ŀ��
                            pCursor.InsertFeature(pFeaturebuffer);

                            Feat = pCursor_y.NextFeature();
                        }
                        #endregion

                        //�ͷ�cursor
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor_y);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);

                    }

                    IDataset dataset_temp = org_class as IDataset;//ת��DATASET����ʱ����ɾ��ʹ��
                    dataset_temp.Delete();//�����ǲ�����������������ݼ������Կɽ���COPY��Ҫ����ɾ��
                    #endregion
                }
                catch
                {
                    dt = dd.Next();//����������һ��
                    continue;
                }

                dt = dd.Next();//����������һ��
            }
            _AppHk.OperatorTips = "����Ҫ�ؼ��ϴ����ɹ���";
        }
        /// <summary>
        /// ������ע
        /// </summary>
        /// <param name="feaName"></param>
        /// <param name="featureDataset"></param>
        /// <param name="feaworkspace"></param>
        /// <param name="fsEditAnno"></param>
        /// <param name="intScale"></param>
        public void createAnnoFeatureClass(string feaName, IFeatureDataset featureDataset, IFeatureWorkspace feaworkspace, IFieldsEdit fsEditAnno, int intScale)
        {
            //����ע�ǵ������ֶ�
            try
            {
                //ע�ǵ�workSpace
                IFeatureWorkspaceAnno pFWSAnno = feaworkspace as IFeatureWorkspaceAnno;//��ע�����ռ�

                IGraphicsLayerScale pGLS = new GraphicsLayerScaleClass();//ͼ�α����ӿ�
                pGLS.Units = esriUnits.esriMeters;//ͼ�α����趨
                pGLS.ReferenceScale = Convert.ToDouble(intScale);//����ע�Ǳ���Ҫ���ñ�����

                IFormattedTextSymbol myTextSymbol = new TextSymbolClass();//�ı���ʽ�ӿ�
                ISymbol pSymbol = (ISymbol)myTextSymbol;//���
                //AnnoҪ��������е�ȱʡ����
                ISymbolCollection2 pSymbolColl = new SymbolCollectionClass();
                ISymbolIdentifier2 pSymID = new SymbolIdentifierClass();
                pSymbolColl.AddSymbol(pSymbol, "Default", out pSymID);

                //AnnoҪ����ı�Ҫ����
                IAnnotateLayerProperties pAnnoProps = new LabelEngineLayerPropertiesClass();
                pAnnoProps.CreateUnplacedElements = true;
                pAnnoProps.CreateUnplacedElements = true;
                pAnnoProps.DisplayAnnotation = true;
                pAnnoProps.UseOutput = true;

                ILabelEngineLayerProperties pLELayerProps = (ILabelEngineLayerProperties)pAnnoProps;
                pLELayerProps.Symbol = pSymbol as ITextSymbol;
                pLELayerProps.SymbolID = 0;
                pLELayerProps.IsExpressionSimple = true;
                pLELayerProps.Offset = 0;
                pLELayerProps.SymbolID = 0;

                IAnnotationExpressionEngine aAnnoVBScriptEngine = new AnnotationVBScriptEngineClass();
                pLELayerProps.ExpressionParser = aAnnoVBScriptEngine;
                pLELayerProps.Expression = "[DESCRIPTION]";
                IAnnotateLayerTransformationProperties pATP = (IAnnotateLayerTransformationProperties)pAnnoProps;
                pATP.ReferenceScale = pGLS.ReferenceScale;
                pATP.ScaleRatio = 1;

                IAnnotateLayerPropertiesCollection pAnnoPropsColl = new AnnotateLayerPropertiesCollectionClass();
                pAnnoPropsColl.Add(pAnnoProps);

                IObjectClassDescription pOCDesc = new AnnotationFeatureClassDescription();
                IFields fields = pOCDesc.RequiredFields;
                IFeatureClassDescription pFDesc = pOCDesc as IFeatureClassDescription;

                for (int j = 0; j < pOCDesc.RequiredFields.FieldCount; j++)
                {
                    fsEditAnno.AddField(pOCDesc.RequiredFields.get_Field(j));
                }
                fields = fsEditAnno as IFields;
                pFWSAnno.CreateAnnotationClass(feaName, fields, pOCDesc.InstanceCLSID, pOCDesc.ClassExtensionCLSID, pFDesc.ShapeFieldName, "", featureDataset, null, pAnnoPropsColl, pGLS, pSymbolColl, true);
            }
            catch
            {

            }
        }
        private void FrmInitiFeatureDataset_Load(object sender, EventArgs e)
        {

        }
    }
}