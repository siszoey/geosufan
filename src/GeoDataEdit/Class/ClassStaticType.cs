using System;
using System.Collections.Generic;
using System.Text;

namespace GeoDataEdit
{
    /// <summary>
    /// ��ǰ��������
    /// </summary>
    public enum DrawTypeConstant
    {
        /// <summary>
        /// ��ͨ��
        /// </summary>
        CommonPoint ,

        /// <summary>
        /// ��ͨ��
        /// </summary>
        CommonLine ,

        /// <summary>
        /// ��ͨ��
        /// </summary>
        CommonPolygon ,

        /// <summary>
        /// ƽ����
        /// </summary>
        ParallelLine ,

        /// <summary>
        /// ��ӽڵ�
        /// </summary>
        AddVertex ,

        /// <summary>
        /// ɾ���ڵ�
        /// </summary>
        DeleteVertex ,

        /// <summary>
        /// �ƶ��ڵ�
        /// </summary>
        MoveVertex ,

        /// <summary>
        /// ɾ��Ҫ��
        /// </summary>
        DeleteSketch ,

        /// <summary>
        /// �ƶ�Ҫ��
        /// </summary>
        MoveSketch ,

        /// <summary>
        /// ��֪�ߵ�ƽ����(�̶�ƽ����)
        /// </summary>
        AnchorParallelLine ,
        /// <summary>
        /// ������
        /// </summary>
        ExtendLine ,
        /// <summary>
        /// �����
        /// </summary>
        BreakLine ,
        /// <summary>
        /// �ֽ��ߡ���
        /// </summary>
        ExplodeLine ,
        /// <summary>
        /// �ı��ߵķ���
        /// </summary>
        ChangeLineDirection ,

        /// <summary>
        /// ����������
        /// </summary>
        VerticalLineFromPoint ,

        /// <summary>
        /// ��������
        /// </summary>
        ScaleZoom ,

        /// <summary>
        /// ��תҪ��
        /// </summary>
        RotateFeature ,

        /// <summary>
        /// ����Ҫ��
        /// </summary>
        MirrorFeature ,

        /// <summary>
        /// �޸�Ҫ������
        /// </summary>
        ChangeFeatureEttri ,

        /// <summary>
        /// ѡ��Ҫ��
        /// </summary>
        SelectFeature ,

        /// <summary>
        /// �ϲ���
        /// </summary>
        UnionLine ,

        /// <summary>
        /// �ϲ���
        /// </summary>
        UnionPolygon ,

        /// <summary>
        /// �ָ���
        /// </summary>
        SplitLine ,

        /// <summary>
        /// �ָ���
        /// </summary>
        SplitPolygon ,

        /// <summary>
        /// ��Բ��
        /// </summary>
        InRoundAngle
    }

    public static class OperationParam
    {
        //�����������
        /// <summary>
        /// �˳�
        /// </summary>
        public const string KeyEsc = "ESC";
        /// <summary>
        /// ���
        /// </summary>
        public const string KeyEnter = "ENT";
        /// <summary>
        /// ����
        /// </summary>
        public const string KeyUndo = "U";
        /// <summary>
        /// ��һ��·��
        /// </summary>
        public const string KeyNextPart = "N";
        /// <summary>
        /// ����
        /// </summary>
        public const string KeyLength = "L";
        /// <summary>
        /// ���
        /// </summary>
        public const string KeyClose = "C";
        /// <summary>
        /// ���
        /// </summary>
        public const string KeyBreak = "B";
        /// <summary>
        /// ȷ��
        /// </summary>
        public const string KeyOK = "Y";
        /// <summary>
        /// ��
        /// </summary>
        public const string KeyYes = "Y";
        /// <summary>
        /// ��
        /// </summary>
        public const string KeyNo = "N";

        //�Ŵ����ų���
        public const string KeyZoomOut = "ZOOMOUT";
        public const string KeyZoomIn = "ZOOMIN";
        public const string KeyPan = "PAN";

        //������������

        /// <summary>
        /// ����
        /// </summary>
        public const string CheckCommand = "Command";                     //
        /// <summary>
        /// �༶��������
        /// </summary>
        public const string CheckPara = "Param";                          //
        /// <summary>
        /// �ؼ���
        /// </summary>
        public const string CheckChar = "Char";                           //
        /// <summary>
        /// ������
        /// </summary>
        public const string CheckPoint = "Point";                         //
        /// <summary>
        /// ����
        /// </summary>
        public const string CheckLength = "Length";                       //
        /// <summary>
        /// ����
        /// </summary>
        public const string CheckLong = "Long";                           //
        /// <summary>
        /// ����
        /// </summary>
        public const string CheckDouble = "Double";                       //
        /// <summary>
        /// ����������
        /// </summary>
        public const string CheckNothing = "Nothing";                     // 
        /// <summary>
        /// �ַ���
        /// </summary>
        public const string CheckString = "String";                       //
        /// <summary>
        /// �Ƕ�
        /// </summary>
        public const string CheckAngle = "Angle";                         //
        /// <summary>
        /// ����������������
        /// </summary>
        public const string CheckPointChar = "PC";                        //
        /// <summary>
        /// ��������߳���
        /// </summary>
        public const string CheckPointLength = "PL";                      //
        /// <summary>
        /// ��������߽Ƕ�
        /// </summary>
        public const string CheckPointAngle = "PA";                      //
        /// <summary>
        /// ���Ȼ����������
        /// </summary>
        public const string CheckLengthChar = "LC";                       //
        /// <summary>
        /// �˳�ѡ��
        /// </summary>
        public const string CheckEscToSelect = "EscToSelect";             //
        /// <summary>
        /// ѡ��ʼ
        /// </summary>
        public const string CheckSelectBegin = "SelectBegin";              //
        /// <summary>
        /// ѡ�����
        /// </summary>
        public const string CheckSelectEnd = "SelectEnd";                 //
        /// <summary>
        /// ѡ����ȷ
        /// </summary>
        public const string CheckEndSelectOK = "EndSelectOK";             //
        /// <summary>
        /// ���˵ȵĸ��µ�ǰ������
        /// </summary>
        public const string CheckUpdate = "Update";                       // 
        /// <summary>
        /// mousedown�ȵĸ��µ�ǰ������
        /// </summary>
        public const string CheckUpOut = "UpOut";                         //
        /// <summary>
        /// �����ȵĸ��µ�ǰ������
        /// </summary>
        public const string CheckEmpty = "Empty";                         //
        /// <summary>
        /// �����ʼ��
        /// </summary>
        public const string CheckInit = "Init";                           //
        /// <summary>
        /// �˳���ǰ����
        /// </summary>
        public const string CheckESC = "ESC";                             //
        /// <summary>
        /// ������Ϣת��
        /// </summary>
        public const string CheckKEYDOWN = "KEYDOWN";                     //
        /// <summary>
        /// �˳���ǰ����
        /// </summary>
        public const string CheckSubBegin = "SubBegin";                   //
        /// <summary>
        /// ������Ϣת��
        /// </summary>
        public const string CheckSubEnd = "SubEnd";                       //
        /// <summary>
        /// �����ʼ
        /// </summary>
        public const string CheckTempCommandBegin = "TempCommandBegin";   //
        /// <summary>
        /// �˳�������
        /// </summary>
        public const string CheckTempCommandEnd = "TempCommandEnd";       //
        public const string CheckSelectTool = "SELECTFEATURETOOL";         //

        //ѡ������
        public const string SelectPoint = "SD";
        public const string SelectPolyline = "SX";
        public const string SelectPolygon = "SM";
        public const string SelectAll = "SA";
        public const string SelectCur = "SC";

        //�Ҽ��˵�����
        public const string MenuCompelte = "���";
        public const string MenuUndo = "����";
        public const string MenuCancel = "����";
        public const string MenuPan = "ƽ��";
        public const string MenuZoomIn = "�Ŵ�";
        public const string MenuZoomOut = "��С";


        //������ʾ����
        public const string TipUndoInvalid = "#������Ч��";
        public const string TipUndoOK = "#���˳ɹ���";


        public const string TipPolylineEnd = "#�����Ҫ�ػ�����ɣ�";
        public const string TipPolylineFail = "#�����Ҫ�ػ���ʧ�ܣ�";




    }

    //1Ϊ��ͨ�㣬2��ͨ�ߣ�3Ϊ��ͨ�棬4Ϊƽ���ߣ�5��ӽڵ㣬6ɾ���ڵ㣬7�ƶ��ڵ㣬8ɾ��Ҫ��,9�ƶ�Ҫ��,
    //10����֪�ߵ�ƽ����,11�ı��ߵķ���,12����������,13��������,14Ҫ��ת��,15Ҫ�������޸�,16�ϲ���,
    //17�ϲ���,18�ָ���,19�ָ���,20��ע��,21�޸�ע��,22ѡ��
    //23��Բ�ǣ�24��ֱ�ǣ�25�����ߣ�26�޼��ߣ�27����28ͶӰ,29�ֽ���,30�����

    /// <summary>
    /// ��ǰ�����ִ�н��
    /// </summary>
    /// <param name="strMsg"></param>
    public delegate void RefreshCommandHandler ( string strMsg );

    ///// <summary>
    ///// �²���������ʾ����
    ///// </summary>
    ///// <param name="sCommandType"></param>
    ///// <param name="sTipInfo"></param>
    ///// <param name="sKeyword"></param>
    //public delegate void GetCommandTipEventHandler(string sCommandType, string sTipInfo, string sKeyword);

    ///// <summary>
    ///// ʹ�����пؼ���ý���
    ///// </summary>
    //public delegate void SetFocusEventHandler();

    ///// <summary>
    ///// ������ǰTool���Ҽ��˵�
    ///// </summary>
    ///// <param name="ToolRightMenu"></param>
    //public delegate void PopupMenuEventHandler(Dictionary<string, bool> ToolRightMenu);

    /// <summary>
    /// ���ʼ�ʱ����ˢ��Toolbar
    /// </summary>
    public delegate void RefreshTBEventHandler ();
}
