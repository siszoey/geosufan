using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;


namespace GeoDataChecker
{
    /// <summary>
    /// ��ѧ������ȷ�Լ��
    /// </summary>
    public class ControlsMathematicsCheck:Plugin.Interface.CommandRefBase
    {
       private Plugin.Application.IAppGISRef _AppHk;

        public ControlsMathematicsCheck()
        {
            base._Name = "GeoDataChecker.ControlsMathematicsCheck";
            base._Caption = "��ѧ������ȷ�Լ��";
            base._Tooltip = "�������ϵ�Ƿ���ϱ�׼�涨";
            base._Checked = false;
            base._Visible = true;
            base._Enabled = true;
            base._Message = "��ѧ������ȷ�Լ��";
        }

        /// <summary>
        /// ͼ���д�������ʱ����״̬Ϊ����ʱ�ſ���
        /// </summary>
        public override bool Enabled
        {
            get
            {
                if (_AppHk == null) return false;
                if (_AppHk.MapControl == null) return false;
                if (_AppHk.MapControl.LayerCount == 0) return false;
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

            //FrmMathematicsCheck mFrmMathematicsCheck = new FrmMathematicsCheck(_AppHk, enumErrorType.��ѧ������ȷ��);
            //mFrmMathematicsCheck.ShowDialog();

          

            FrmMathCheck pFrmMathCheck = new FrmMathCheck();
            if(pFrmMathCheck.ShowDialog()==DialogResult.OK)
            {
                #region ������־����
                //������־������Ϣ
                string logPath = TopologyCheckClass.GeoLogPath+"Log"+ System.DateTime.Today.Year.ToString() + System.DateTime.Today.Month.ToString() + System.DateTime.Today.Day.ToString() + System.DateTime.Now.Hour.ToString() + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString() + ".xls";
                if (File.Exists(logPath))
                {
                    if (SysCommon.Error.ErrorHandle.ShowFrmInformation("��", "��", "��־�ļ�\n'" + logPath + "'\n�Ѿ�����,�Ƿ��滻?"))
                    {
                        File.Delete(logPath);
                    }
                    else
                    {
                        return;
                    }
                }
                //������־��ϢEXCEL��ʽ
                SysCommon.DataBase.SysDataBase pSysLog = new SysCommon.DataBase.SysDataBase();
                pSysLog.SetDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + logPath + "; Extended Properties=Excel 8.0;", SysCommon.enumDBConType.OLEDB, SysCommon.enumDBType.ACCESS, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��־��Ϣ������ʧ�ܣ�");
                    return;
                }
                string strCreateTableSQL = @" CREATE TABLE ";
                strCreateTableSQL += @" ������־ ";
                strCreateTableSQL += @" ( ";
                strCreateTableSQL += @" ��鹦���� VARCHAR, ";
                strCreateTableSQL += @" �������� VARCHAR, ";
                strCreateTableSQL += @" �������� VARCHAR, ";
                strCreateTableSQL += @" ����ͼ��1 VARCHAR, ";
                strCreateTableSQL += @" ����OID1 VARCHAR, ";
                strCreateTableSQL += @" ����ͼ��2 VARCHAR, ";
                strCreateTableSQL += @" ����OID2 VARCHAR, ";
                strCreateTableSQL += @" ��λ��X VARCHAR , ";
                strCreateTableSQL += @" ��λ��Y VARCHAR , ";
                strCreateTableSQL += @" ���ʱ�� VARCHAR ,";
                strCreateTableSQL += @" �����ļ�·�� VARCHAR ";
                strCreateTableSQL += @" ) ";

                pSysLog.UpdateTable(strCreateTableSQL, out eError);
                if (eError != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ͷ����");
                    pSysLog.CloseDbConnection();
                    return;
                }

                DataCheckClass dataCheckCls = new DataCheckClass(_AppHk);
                //����־��������Ϣ�ͱ�����������
                dataCheckCls.ErrDbCon = pSysLog.DbConn;
                dataCheckCls.ErrTableName = "������־";
                #endregion

                #region ���Ҫ����IFeatureClass�ļ���
                //��Mapcontrol�����е�ͼ������������
                List<IFeatureClass> LstFeaClass = new List<IFeatureClass>();
                for (int i = 0; i < _AppHk.MapControl.LayerCount; i++)
                {
                    ILayer player = _AppHk.MapControl.get_Layer(i);
                    if (player is IGroupLayer)
                    {
                       
                        ICompositeLayer pComLayer = player as ICompositeLayer;
                        for (int j = 0; j < pComLayer.Count; j++)
                        {
                            ILayer mLayer = pComLayer.get_Layer(j);
                            IFeatureLayer mfeatureLayer = mLayer as IFeatureLayer;
                            if (mfeatureLayer == null) continue;
                            IFeatureClass pfeaCls = mfeatureLayer.FeatureClass;
                            if (!LstFeaClass.Contains(pfeaCls))
                            {
                                LstFeaClass.Add(pfeaCls);
                            }
                        }
                    }
                    else
                    {
                        IFeatureLayer pfeatureLayer = player as IFeatureLayer;
                        if (pfeatureLayer == null) continue;
                        IFeatureClass mFeaCls = pfeatureLayer.FeatureClass;
                        if (!LstFeaClass.Contains(mFeaCls))
                        {
                            LstFeaClass.Add(mFeaCls);
                        }
                    }
                }
                #endregion

                if (_AppHk.DataTree == null) return;
                _AppHk.DataTree.Nodes.Clear();
                //����������ͼ
                IntialTree(_AppHk.DataTree);
                //�������ڵ���ɫ
                setNodeColor(_AppHk.DataTree);
                _AppHk.DataTree.Tag = false;

                string prjStr = pFrmMathCheck.PRJFNAME;
                if(prjStr=="")
                {
                    return;
                }
                try
                {
                    ISpatialReferenceFactory spatialRefFac = new SpatialReferenceEnvironmentClass();
                    ISpatialReference standardSpatialRef = spatialRefFac.CreateESRISpatialReferenceFromPRJFile(prjStr);

                    
                    for (int i = 0; i < LstFeaClass.Count; i++)
                    {
                        IFeatureClass pFeatureClass = LstFeaClass[i];
                        string pFeaClsNameStr = "";//ͼ����
                        pFeaClsNameStr = (pFeatureClass as IDataset).Name.Trim();

                        //������ͼ�ڵ�(��ͼ������Ϊ�����)
                        DevComponents.AdvTree.Node pNode = new DevComponents.AdvTree.Node();
                        pNode = (DevComponents.AdvTree.Node)CreateAdvTreeNode(_AppHk.DataTree.Nodes, pFeaClsNameStr, pFeaClsNameStr, _AppHk.DataTree.ImageList.Images[6], true);//ͼ�����ڵ�
                        //��ʾ������
                        ShowProgressBar(true);

                        int tempValue = 0;
                        ChangeProgressBar((_AppHk as Plugin.Application.IAppFormRef).ProgressBar, 0, 1, tempValue);

                        dataCheckCls.MathematicsCheck(pFeatureClass, standardSpatialRef, out eError);
                        if (eError != null)
                        {
                            SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѧ��������ȷ���ʧ�ܡ�" + eError.Message);
                            pSysLog.CloseDbConnection();
                            return;
                        }

                        tempValue += 1;//��������ֵ��1
                        ChangeProgressBar((_AppHk as Plugin.Application.IAppFormRef).ProgressBar, -1, -1, tempValue);

                        //�ı���ͼ����״̬
                        ChangeTreeSelectNode(pNode, "���ͼ��"+pFeaClsNameStr+"�����ݻ�������ȷ���", false);
                    }
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ݼ�����!");
                    pSysLog.CloseDbConnection();
                    //���ؽ�����
                    ShowProgressBar(false);
                }
                catch(Exception ex)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ",ex.Message);
                    pSysLog.CloseDbConnection();
                    return;
                }
                
            }
        }


       public override void OnCreate(Plugin.Application.IApplicationRef hook)
        {
            if (hook == null) return;
            _AppHk = hook as Plugin.Application.IAppGISRef;
            if (_AppHk.MapControl == null) return;
        }



        #region ������ͼ��غ���
        //����������ͼ
        private void IntialTree(DevComponents.AdvTree.AdvTree aTree)
        {
            DevComponents.AdvTree.ColumnHeader aColumnHeader;
            aColumnHeader = new DevComponents.AdvTree.ColumnHeader();
            aColumnHeader.Name = "FCName";
            aColumnHeader.Text = "ͼ����";
            aColumnHeader.Width.Relative = 50;
            aTree.Columns.Add(aColumnHeader);

            aColumnHeader = new DevComponents.AdvTree.ColumnHeader();
            aColumnHeader.Name = "NodeRes";
            aColumnHeader.Text = "���";
            aColumnHeader.Width.Relative = 45;
            aTree.Columns.Add(aColumnHeader);
        }
        //����ѡ�����ڵ���ɫ
        private void setNodeColor(DevComponents.AdvTree.AdvTree aTree)
        {
            DevComponents.DotNetBar.ElementStyle elementStyle = new DevComponents.DotNetBar.ElementStyle();
            elementStyle.BackColor = Color.FromArgb(255, 244, 213);
            elementStyle.BackColor2 = Color.FromArgb(255, 216, 105);
            elementStyle.BackColorGradientAngle = 90;
            elementStyle.Border = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderBottomWidth = 1;
            elementStyle.BorderColor = Color.DarkGray;
            elementStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderLeftWidth = 1;
            elementStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderRightWidth = 1;
            elementStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            elementStyle.BorderTopWidth = 1;
            elementStyle.BorderWidth = 1;
            elementStyle.CornerDiameter = 4;
            elementStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            aTree.NodeStyleSelected = elementStyle;
            aTree.DragDropEnabled = false;
        }
        //������ͼ�ڵ�
        private DevComponents.AdvTree.Node CreateAdvTreeNode(DevComponents.AdvTree.NodeCollection nodeCol, string strText, string strName, Image pImage, bool bExpand)
        {

            DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node();
            node.Text = strText;
            node.Image = pImage;
            if (strName != null)
            {
                node.Name = strName;
            }

            if (bExpand == true)
            {
                node.Expand();
            }
            //�����ͼ�нڵ�
            DevComponents.AdvTree.Cell aCell = new DevComponents.AdvTree.Cell();
            aCell.Images.Image = null;
            node.Cells.Add(aCell);
            nodeCol.Add(node);
            return node;
        }

        //�����ͼ�ڵ���
        private DevComponents.AdvTree.Cell CreateAdvTreeCell(DevComponents.AdvTree.Node aNode, string strText, Image pImage)
        {
            DevComponents.AdvTree.Cell aCell = new DevComponents.AdvTree.Cell(strText);
            aCell.Images.Image = pImage;
            aNode.Cells.Add(aCell);

            return aCell;
        }

        //Ϊ���ݴ�����ͼ�ڵ���Ӵ�����״̬
        private void ChangeTreeSelectNode(DevComponents.AdvTree.Node aNode, string strRes, bool bClear)
        {
            if (aNode == null)
            {
                _AppHk.DataTree.SelectedNode = null;
                _AppHk.DataTree.Refresh();
                return;
            }

            _AppHk.DataTree.SelectedNode = aNode;
            if (bClear)
            {
                _AppHk.DataTree.SelectedNode.Nodes.Clear();
            }
            _AppHk.DataTree.SelectedNode.Cells[1].Text = strRes;
            _AppHk.DataTree.Refresh();
        }
        #endregion

        #region ��������ʾ
        //���ƽ�������ʾ
        private void ShowProgressBar(bool bVisible)
        {
            if (bVisible == true)
            {
                (_AppHk as Plugin.Application.IAppFormRef).ProgressBar.Visible = true;
            }
            else
            {
                (_AppHk as Plugin.Application.IAppFormRef).ProgressBar.Visible = false;
            }
        }
        //�޸Ľ�����
        private void ChangeProgressBar(DevComponents.DotNetBar.ProgressBarItem pProgressBar, int min, int max, int value)
        {
            if (min != -1)
            {
                pProgressBar.Minimum = min;
            }
            if (max != -1)
            {
                pProgressBar.Maximum = max;
            }
            pProgressBar.Value = value;
            pProgressBar.Refresh();
        }


        //�ı�״̬����ʾ����
        private void ShowStatusTips(string strText)
        {
            (_AppHk as Plugin.Application.IAppFormRef).OperatorTips = strText;
        }
        #endregion
    }
}
