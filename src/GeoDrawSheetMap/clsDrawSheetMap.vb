Imports ESRI.ArcGIS.Carto
Imports ESRI.ArcGIS.Controls
Imports ESRI.ArcGIS.DataSourcesFile
Imports ESRI.ArcGIS.DataSourcesRaster
Imports ESRI.ArcGIS.Display
Imports ESRI.ArcGIS.GeoDatabase
Imports ESRI.ArcGIS.Geometry
Imports ESRI.ArcGIS.esriSystem


Public Class clsDrawSheetMap
    Private m_vXoffset As Collection                       '��¼ģ���е������е������ƫ����X
    Private m_vYoffset As Collection                       '��¼ģ���е������е������ƫ����Y
    Public vScale As Long                                 '������
    Public pGeometry As IGeometry                         '��������ͼ����״
    Public vPageLayoutControl As IPageLayoutControl       '����������ͼ����
    Public m_strSheetNo As String                         'ͼ����
    Public m_intPntCount As Integer                       'Ҫ����ĵ���
    Public m_pPrjCoor As ISpatialReference      '���õ�ͶӰ����ϵͳ
    Public type_ZT As Integer                  'ר������0����ͼ1������״ͼ2�滮ͼ20110914 yjl add

    Private m_clsGetGeoInfo As clsGetGeoInfo              '����ͼ���ĸ������λ����Ϣ
    Private m_pPaperPointCol As IPointCollection           '�ĸ��ǵ��ͼֽ����Ϣ

    Private m_pMapInfo As MapInfo()                         '�õ������ܶ��ͼ����Ϣ


    '��ʱ
    Public g_pRecElement As IElement         '�ο����ο�
    Public g_MapFrameEle As IElement         '���ݿ�


    '��Ҫ����������  ��Ϊ����ģ�岢��ģ�������Ҫ�غ��ƶ�Ҫ��������

    'Public Sub DrawSheetMap()
    '    Try
    '        Dim vType As String

    '        ''On Error GoTo ErrH
    '        vType = CStr(vScale)

    '        '���ȸı���MAP�Ŀռ�ο�
    '        vPageLayoutControl.ActiveView.FocusMap.SpatialReference = m_pPrjCoor

    '        '�����ĸ����λ����Ϣ
    '        'm_clsGetGeoInfo = New clsGetGeoInfo
    '        'm_clsGetGeoInfo.m_lngMapScale = vScale
    '        'm_clsGetGeoInfo.m_strMapNO = m_strSheetNo
    '        'm_clsGetGeoInfo.m_intInsertCount = m_intPntCount
    '        'Dim tempspataialreference As IProjectedCoordinateSystem = New ProjectedCoordinateSystem()
    '        'Try
    '        '    tempspataialreference = m_pPrjCoor
    '        'Catch ex As Exception
    '        '    Dim factroy As ISpatialReferenceFactory = New SpatialReferenceEnvironment()
    '        '    tempspataialreference = factroy.CreateProjectedCoordinateSystem(102018)

    '        'End Try
    '        'm_clsGetGeoInfo.m_pPrjCoor = tempspataialreference
    '        'm_clsGetGeoInfo.ComputerAllGeoInfo()

    '        pGeometry = GISServices.Sheet.LargeScaleMapNO.GetPolygonFromLargeMapNO(m_strSheetNo, m_pPrjCoor)

    '        '�ı��ͼ�ĳߴ��С
    '        If vScale < 5000 Then
    '            Call SetPageLayoutSize(vPageLayoutControl, 80, 80, 50, 50)
    '            Call OpenTemplateElement(vType, pGeometry, vPageLayoutControl)
    '            m_vXoffset = New Collection
    '            m_vYoffset = New Collection
    '            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerLeft.X - g_pRecElement.Geometry.Envelope.LowerLeft.X)
    '            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerRight.X - g_pRecElement.Geometry.Envelope.LowerRight.X)
    '            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperRight.X - g_pRecElement.Geometry.Envelope.UpperRight.X)
    '            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperLeft.X - g_pRecElement.Geometry.Envelope.UpperLeft.X)
    '            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerLeft.Y - g_pRecElement.Geometry.Envelope.LowerLeft.Y)
    '            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerRight.Y - g_pRecElement.Geometry.Envelope.LowerRight.Y)
    '            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperRight.Y - g_pRecElement.Geometry.Envelope.UpperRight.Y)
    '            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperLeft.Y - g_pRecElement.Geometry.Envelope.UpperLeft.Y)
    '            Call MoveElementInBigScalePage(vPageLayoutControl.PageLayout, m_vXoffset, m_vYoffset)
    '        Else
    '            Select Case vType
    '                Case "5000"
    '                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 50, pGeometry.Envelope.Height / 50)
    '                Case "10000"
    '                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 100, pGeometry.Envelope.Height / 100)
    '                Case "25000"
    '                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 250, pGeometry.Envelope.Height / 250)
    '                Case "50000"
    '                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 500, pGeometry.Envelope.Height / 500)
    '                Case "250000"
    '                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 2500, pGeometry.Envelope.Height / 2500)
    '                Case Else
    '                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, 80, 80)
    '            End Select
    '            '���ģ�� ����������� ������ʾ
    '            Call OpenTemplateElement(vType, pGeometry, vPageLayoutControl)
    '            '�ƶ���ͼԪ�ص�λ��
    '            Call MoveElementInPage(vPageLayoutControl.PageLayout, m_vXoffset, m_vYoffset)
    '        End If


    '        'Exit Sub
    '        'ErrH:
    '    Catch ex As Exception

    '    End Try


    'End Sub
    Public Sub DrawSheetMap()
        Dim vType As String

        'On Error GoTo ErrH
        vType = CStr(vScale)

        '���ȸı���MAP�Ŀռ�ο�
        vPageLayoutControl.ActiveView.FocusMap.SpatialReference = m_pPrjCoor

        '�ı��ͼ�ĳߴ��С
        '�����ĸ����λ����Ϣ
        m_clsGetGeoInfo = New clsGetGeoInfo
        m_clsGetGeoInfo.m_lngMapScale = vScale
        m_clsGetGeoInfo.m_strMapNO = m_strSheetNo
        m_clsGetGeoInfo.m_intInsertCount = m_intPntCount
        m_clsGetGeoInfo.m_pPrjCoor = m_pPrjCoor
        m_clsGetGeoInfo.ComputerAllGeoInfo()

        pGeometry = m_clsGetGeoInfo.m_pSheetMapGeometry

        If vScale < 5000 Then
          
            Call SetPageLayoutSize(vPageLayoutControl, 80, 80, 50, 50)
            Call OpenTemplateElement(vType, pGeometry, vPageLayoutControl)
            m_vXoffset = New Collection
            m_vYoffset = New Collection
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerLeft.X - g_pRecElement.Geometry.Envelope.LowerLeft.X)
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerRight.X - g_pRecElement.Geometry.Envelope.LowerRight.X)
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperRight.X - g_pRecElement.Geometry.Envelope.UpperRight.X)
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperLeft.X - g_pRecElement.Geometry.Envelope.UpperLeft.X)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerLeft.Y - g_pRecElement.Geometry.Envelope.LowerLeft.Y)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerRight.Y - g_pRecElement.Geometry.Envelope.LowerRight.Y)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperRight.Y - g_pRecElement.Geometry.Envelope.UpperRight.Y)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperLeft.Y - g_pRecElement.Geometry.Envelope.UpperLeft.Y)
            Call MoveElementInBigScalePage(vPageLayoutControl.PageLayout, m_vXoffset, m_vYoffset)
        Else
            Select Case vType
                Case "5000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 50, pGeometry.Envelope.Height / 50)
                Case "10000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 100, pGeometry.Envelope.Height / 100)
                Case "25000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 250, pGeometry.Envelope.Height / 250)
                Case "50000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 500, pGeometry.Envelope.Height / 500)
                Case "250000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 2500, pGeometry.Envelope.Height / 2500)
                Case Else
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, 80, 80)
            End Select
            '���ģ�� ����������� ������ʾ
            Call OpenTemplateElement(vType, pGeometry, vPageLayoutControl)
            '�ƶ���ͼԪ�ص�λ��
            If type_ZT = 0 Then '����ͼ
                Call MoveElementInPage(vPageLayoutControl.PageLayout, m_vXoffset, m_vYoffset)
            ElseIf type_ZT = 1 Then 'ɭ����Դ��״�ַ�
                Call MoveElementInPageForTDLY(vPageLayoutControl.PageLayout, m_vXoffset, m_vYoffset)
            End If
            Dim noc As New Collection

            CalculateFigure(Me.m_strSheetNo, Me.vScale, noc, Me.vPageLayoutControl.PageLayout, pGeometry)
        End If



        ''�ı��ͼ�ĳߴ��С
        'If vScale < 5000 Then
        '    Call SetPageLayoutSize(vPageLayoutControl, 60, 60, 50, 50)
        'Else
        '    Select Case vType
        '        Case "5000"
        '            Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 50, pGeometry.Envelope.Height / 50)
        '        Case "10000"
        '            Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 100, pGeometry.Envelope.Height / 100)
        '        Case "25000"
        '            Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 250, pGeometry.Envelope.Height / 250)
        '        Case "50000"
        '            Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 500, pGeometry.Envelope.Height / 500)
        '        Case "250000"
        '            Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 2500, pGeometry.Envelope.Height / 2500)
        '        Case Else
        '            Call SetPageLayoutSize(vPageLayoutControl, 80, 80, 80, 80)
        '    End Select

        'End If

        ''���ģ�� ����������� ������ʾ
        'Call OpenTemplateElement(vType, pGeometry, vPageLayoutControl)
        ''�ƶ���ͼԪ�ص�λ��
        'Call MoveElementInPage(vPageLayoutControl.PageLayout, m_vXoffset, m_vYoffset)
        ''Exit Sub
        ''ErrH:

    End Sub

    Public Sub DrawSheetMap(ByVal pMapInfo As Object)
        Dim vType As String

        '2008.04.17 t���......
        m_pMapInfo = pMapInfo

        'On Error GoTo ErrH
        vType = CStr(vScale)

        '���ȸı���MAP�Ŀռ�ο�
        vPageLayoutControl.ActiveView.FocusMap.SpatialReference = m_pPrjCoor

        '�����ĸ����λ����Ϣ
        m_clsGetGeoInfo = New clsGetGeoInfo
        m_clsGetGeoInfo.m_lngMapScale = vScale
        m_clsGetGeoInfo.m_strMapNO = m_strSheetNo
        m_clsGetGeoInfo.m_intInsertCount = m_intPntCount
        m_clsGetGeoInfo.m_pPrjCoor = m_pPrjCoor
        m_clsGetGeoInfo.ComputerAllGeoInfo()

        pGeometry = m_clsGetGeoInfo.m_pSheetMapGeometry

        'For i As Integer = 0 To m_pMapInfo.GetLength(0) - 1
        '    If m_pMapInfo(i).Keys = m_strSheetNo Then
        '        pGeometry = m_pMapInfo(i).pGeometry
        '        Exit For
        '    End If
        'Next
        'pGeometry.Envelope.XMax

        'm_clsGetGeoInfo.m_pSheetMapGeometry

        '�ı��ͼ�ĳߴ��С
        If vScale < 5000 Then
            Call SetPageLayoutSize(vPageLayoutControl, 80, 80, 50, 50)
            Call OpenTemplateElement(vType, pGeometry, vPageLayoutControl)
            m_vXoffset = New Collection
            m_vYoffset = New Collection
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerLeft.X - g_pRecElement.Geometry.Envelope.LowerLeft.X)
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerRight.X - g_pRecElement.Geometry.Envelope.LowerRight.X)
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperRight.X - g_pRecElement.Geometry.Envelope.UpperRight.X)
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperLeft.X - g_pRecElement.Geometry.Envelope.UpperLeft.X)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerLeft.Y - g_pRecElement.Geometry.Envelope.LowerLeft.Y)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerRight.Y - g_pRecElement.Geometry.Envelope.LowerRight.Y)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperRight.Y - g_pRecElement.Geometry.Envelope.UpperRight.Y)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperLeft.Y - g_pRecElement.Geometry.Envelope.UpperLeft.Y)
            Call MoveElementInBigScalePage(vPageLayoutControl.PageLayout, m_vXoffset, m_vYoffset)
        Else
            Select Case vType
                Case "5000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 50, pGeometry.Envelope.Height / 50)
                Case "10000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 100, pGeometry.Envelope.Height / 100)
                Case "25000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 250, pGeometry.Envelope.Height / 250)
                Case "50000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 500, pGeometry.Envelope.Height / 500)
                Case "250000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 2500, pGeometry.Envelope.Height / 2500)
                Case Else
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, 80, 80)
            End Select
            '���ģ�� ����������� ������ʾ
            Call OpenTemplateElement(vType, pGeometry, vPageLayoutControl)
            '�ƶ���ͼԪ�ص�λ��
            Call MoveElementInPage(vPageLayoutControl.PageLayout, m_vXoffset, m_vYoffset)
        End If


        'Exit Sub
        'ErrH:

    End Sub



    '��Ҫ����������  ��Ϊ����ģ�岢��ģ�������Ҫ�غ��ƶ�Ҫ��������
    Public Sub DrawSheetMap(ByVal pPageWidth As Double, ByVal pPageHeight As Double, ByVal pMapWidth As Double, ByVal pMapHeight As Double)
        Dim vType As String

        'On Error GoTo ErrH
        vType = CStr(vScale)

        '���ȸı���MAP�Ŀռ�ο�
        vPageLayoutControl.ActiveView.FocusMap.SpatialReference = m_pPrjCoor

        '�����ĸ����λ����Ϣ
        m_clsGetGeoInfo = New clsGetGeoInfo
        m_clsGetGeoInfo.m_lngMapScale = vScale
        m_clsGetGeoInfo.m_strMapNO = m_strSheetNo
        m_clsGetGeoInfo.m_intInsertCount = m_intPntCount
        m_clsGetGeoInfo.m_pPrjCoor = m_pPrjCoor
        m_clsGetGeoInfo.ComputerAllGeoInfo()

        pGeometry = m_clsGetGeoInfo.m_pSheetMapGeometry

        '�ı��ͼ�ĳߴ��С
        If vScale < 5000 Then
            Call SetPageLayoutSize(vPageLayoutControl, pPageWidth, pPageHeight, pMapWidth, pMapHeight)
            Call OpenTemplateElement(vType, pGeometry, vPageLayoutControl)
            m_vXoffset = New Collection
            m_vYoffset = New Collection
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerLeft.X - g_pRecElement.Geometry.Envelope.LowerLeft.X)
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerRight.X - g_pRecElement.Geometry.Envelope.LowerRight.X)
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperRight.X - g_pRecElement.Geometry.Envelope.UpperRight.X)
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperLeft.X - g_pRecElement.Geometry.Envelope.UpperLeft.X)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerLeft.Y - g_pRecElement.Geometry.Envelope.LowerLeft.Y)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerRight.Y - g_pRecElement.Geometry.Envelope.LowerRight.Y)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperRight.Y - g_pRecElement.Geometry.Envelope.UpperRight.Y)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperLeft.Y - g_pRecElement.Geometry.Envelope.UpperLeft.Y)
            Call MoveElementInBigScalePage(vPageLayoutControl.PageLayout, m_vXoffset, m_vYoffset)
        Else
            Select Case vType
                Case "5000"
                    Call SetPageLayoutSize(vPageLayoutControl, pPageWidth, pPageHeight, pGeometry.Envelope.Width / 50, pGeometry.Envelope.Height / 50)
                Case "10000"
                    Call SetPageLayoutSize(vPageLayoutControl, pPageWidth, pPageHeight, pGeometry.Envelope.Width / 100, pGeometry.Envelope.Height / 100)
                Case "25000"
                    Call SetPageLayoutSize(vPageLayoutControl, pPageWidth, pPageHeight, pGeometry.Envelope.Width / 250, pGeometry.Envelope.Height / 250)
                Case "50000"
                    Call SetPageLayoutSize(vPageLayoutControl, pPageWidth, pPageHeight, pGeometry.Envelope.Width / 500, pGeometry.Envelope.Height / 500)
                Case "250000"
                    Call SetPageLayoutSize(vPageLayoutControl, pPageWidth, pPageHeight, pGeometry.Envelope.Width / 2500, pGeometry.Envelope.Height / 2500)
                Case Else
                    Call SetPageLayoutSize(vPageLayoutControl, pPageWidth, pPageHeight, pMapWidth, pMapHeight)
            End Select
            '���ģ�� ����������� ������ʾ
            Call OpenTemplateElement(vType, pGeometry, vPageLayoutControl)
            '�ƶ���ͼԪ�ص�λ��
            Call MoveElementInPage(vPageLayoutControl.PageLayout, m_vXoffset, m_vYoffset)
        End If


        'Exit Sub
        'ErrH:

    End Sub




    Public Sub DrawSheetMapByMultiMap(ByRef pSourceGeo As IGeometry)
        Dim vType As String

        'On Error GoTo ErrH
        vType = CStr(vScale)

        '���ȸı���MAP�Ŀռ�ο�
        'vPageLayoutControl.ActiveView.FocusMap.SpatialReference = m_pPrjCoor

        '�����ĸ����λ����Ϣ
        m_clsGetGeoInfo = New clsGetGeoInfo
        m_clsGetGeoInfo.m_lngMapScale = vScale
        m_clsGetGeoInfo.m_strMapNO = m_strSheetNo
        m_clsGetGeoInfo.m_intInsertCount = m_intPntCount
        m_clsGetGeoInfo.m_pPrjCoor = m_pPrjCoor
        m_clsGetGeoInfo.ComputerAllGeoInfo()

        pGeometry = m_clsGetGeoInfo.m_pSheetMapGeometry

        '2008.04.18
        pSourceGeo = pGeometry

        '�ı��ͼ�ĳߴ��С
        If vScale < 5000 Then
            Call SetPageLayoutSize(vPageLayoutControl, 80, 80, 50, 50)
            Call OpenTemplateElement(vType, pGeometry, vPageLayoutControl)
            m_vXoffset = New Collection
            m_vYoffset = New Collection
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerLeft.X - g_pRecElement.Geometry.Envelope.LowerLeft.X)
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerRight.X - g_pRecElement.Geometry.Envelope.LowerRight.X)
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperRight.X - g_pRecElement.Geometry.Envelope.UpperRight.X)
            m_vXoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperLeft.X - g_pRecElement.Geometry.Envelope.UpperLeft.X)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerLeft.Y - g_pRecElement.Geometry.Envelope.LowerLeft.Y)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.LowerRight.Y - g_pRecElement.Geometry.Envelope.LowerRight.Y)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperRight.Y - g_pRecElement.Geometry.Envelope.UpperRight.Y)
            m_vYoffset.Add(g_MapFrameEle.Geometry.Envelope.UpperLeft.Y - g_pRecElement.Geometry.Envelope.UpperLeft.Y)
            Call MoveElementInBigScalePage(vPageLayoutControl.PageLayout, m_vXoffset, m_vYoffset)
        Else
            Select Case vType
                Case "5000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 50, pGeometry.Envelope.Height / 50)
                Case "10000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 100, pGeometry.Envelope.Height / 100)
                Case "25000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 250, pGeometry.Envelope.Height / 250)
                Case "50000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 500, pGeometry.Envelope.Height / 500)
                Case "250000"
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, pGeometry.Envelope.Width / 2500, pGeometry.Envelope.Height / 2500)
                Case Else
                    Call SetPageLayoutSize(vPageLayoutControl, 80, 80, 80, 80)
            End Select
            '���ģ�� ����������� ������ʾ
            Call OpenTemplateElement(vType, pGeometry, vPageLayoutControl)
            '�ƶ���ͼԪ�ص�λ��

            Call MoveElementInPage(vPageLayoutControl.PageLayout, m_vXoffset, m_vYoffset)
        End If


        'Exit Sub
        'ErrH:

    End Sub


    '�ƶ�Ҫ�ص�λ��
    Private Sub MoveElementInPage(ByVal pPagelayout As IPageLayout, ByVal vXoffset As Collection, ByVal vYoffset As Collection)
        Dim pTxtElement As ITextElement
        Dim pGraphicsContainer As IGraphicsContainer
        pGraphicsContainer = New PageLayout
        pGraphicsContainer = pPagelayout
        Dim pElement As IElement
        pGraphicsContainer.Reset()
        pElement = pGraphicsContainer.Next

        While Not pElement Is Nothing
            If TypeOf pElement Is ITextElement Then
                pTxtElement = pElement
                Select Case pTxtElement.Text

                    Case "�л����񹲺͹����������ߵ���ͼ"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���Ͻ�ͼ��
                    Case "G50G004004"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                        '���Ϸ�ͼ��
                    Case "G50G004005"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                        '���Ͻ�ͼ��
                    Case "G50G004006"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                        '���ͼ��
                    Case "G50G005004"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                    Case "ͼ��1"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                    Case "ͼ��2"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "ͼ��3"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                    Case "ͼ��4"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        'Case "ͼ��3"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(4) + vXoffset(1)) / 2, (vYoffset(4) + vYoffset(1)) / 2)
                        '    End If
                        'Case "ͼ��4"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(1) + vXoffset(2)) / 2, (vYoffset(1) + vYoffset(2)) / 2)
                        '    End If
                        'Case "ͼ��5"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(2) + vXoffset(3)) / 2, (vYoffset(2) + vYoffset(3)) / 2)
                        '    End If
                        'Case "ͼ��6"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(3) + vXoffset(4)) / 2, (vYoffset(3) + vYoffset(4)) / 2)
                        '    End If
                        '��ǰͼ��
                    Case "G50G005005"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, (vXoffset(3) + vXoffset(4)) / 2, (vYoffset(3) + vYoffset(4)) / 2)
                        End If
                    Case "ͼ��1"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                    Case "ͼ��2"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "ͼ��3"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                    Case "ͼ��4"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '�ұ�ͼ��
                    Case "G50G005006"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                        '���½�ͼ��
                    Case "G50G006004"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                        '���·�ͼ��
                    Case "G50G006005"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                        '���½�ͼ��
                    Case "G50G006006"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If

                        '��ǰ������
                    Case "������"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, (vXoffset(1) + vXoffset(2)) / 2, (vYoffset(1) + vYoffset(2)) / 2)
                        End If
                    Case "������2"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                    Case "ͼ��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, (vXoffset(3) + vXoffset(4)) / 2, (vYoffset(3) + vYoffset(4)) / 2)
                        End If
                        'Case "����"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(3) + vXoffset(4)) / 2, (vYoffset(3) + vYoffset(4)) / 2)
                        '    End If

                    Case "�ܼ�"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                    Case "��ע��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "����ϵ"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "������ȫ��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                        'Case "���Ա"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        '    End If
                        'Case "��ͼԱ"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        '    End If
                        'Case "����Ա"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        '    End If
                    Case "1985���Ҹ̻߳�׼��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "�ȸ߾�Ϊ1�ס�"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "1995��5��XXX��ͼ��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "1996���ͼʽ��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If

                        '��Ϊͼ��������û�����û��༭�����Ծ�ֱ��������ˣ����������ݿ�ȡ�ķ�����ͬ
                    Case "553.75����"
                        'FieldValue = "����ͼ���ǵ�Y����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                    Case "385.50����"
                        ' FieldValue = "����ͼ���ǵ�X����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                        '���Ͻ�y,x����
                    Case "554.00����"
                        'FieldValue = "����ͼ���ǵ�Y����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                    Case "385.50����"
                        'FieldValue = "����ͼ���ǵ�X����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���Ͻ�����
                    Case "554.00����"
                        ' FieldValue = "����ͼ���ǵ�Y����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                    Case "385.75����"
                        'FieldValue = "����ͼ���ǵ�X����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                        '���½�����
                    Case "553.75����"
                        ' FieldValue = "����ͼ���ǵ�Y����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "385.75����"
                        'FieldValue = "����ͼ���ǵ�X����"
                        'FigureChar pSheetEnvelope.xmax, vScaleType, pTxtElement
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case Else
                        Dim pElePro As IElementProperties
                        pElePro = pElement
                        If pElePro.Name.Contains("���½Ǳ�ע") Then
                            If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                                Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                            End If
                        End If
                        If pElePro.Name.Contains("���½Ǳ�ע") Then
                            If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                                Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                            End If
                        End If

                End Select
            ElseIf TypeOf pElement Is IGroupElement Then
                Dim pGroupElement As IGroupElement
                Dim pElePro As IElementProperties
                pElePro = pElement
                pGroupElement = pElement
                If pElePro.Name = "��ͼ��" Or pGroupElement.ElementCount = 2 Then '���ڹ涨�Ӻϱ��ELEMENT���� ��û���ҵ����������𷽷�
                    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                    End If
                Else
                    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        Call MovePageLyrOutlbl(pElement, (vXoffset(1) + vXoffset(2)) / 2, (vYoffset(1) + vYoffset(2)) / 2)
                    End If
                End If
            ElseIf TypeOf pElement Is IMapSurroundFrame Then
                Dim pMapSurroundFrame As IMapSurroundFrame
                pMapSurroundFrame = pElement
                If TypeOf pMapSurroundFrame.Object Is IScaleBar Then
                    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        Call MovePageLyrOutlbl(pElement, (vXoffset(1) + vXoffset(2)) / 2, (vYoffset(1) + vYoffset(2)) / 2)
                    End If
                End If
            End If
            pElement = pGraphicsContainer.Next
        End While
    End Sub
    '�ƶ�Ҫ�ص�λ��yjl 20110916 add for ɭ����Դ��״�ַ�
    Private Sub MoveElementInPageForTDLY(ByVal pPagelayout As IPageLayout, ByVal vXoffset As Collection, ByVal vYoffset As Collection)
        Dim pTxtElement As ITextElement
        Dim pGraphicsContainer As IGraphicsContainer
        pGraphicsContainer = New PageLayout
        pGraphicsContainer = pPagelayout
        Dim pElement As IElement
        pGraphicsContainer.Reset()
        pElement = pGraphicsContainer.Next

        While Not pElement Is Nothing
            If TypeOf pElement Is ITextElement Then
                pTxtElement = pElement
                Select Case pTxtElement.Text

                    Case "�л����񹲺͹����������ߵ���ͼ"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���Ͻ�ͼ��
                    Case "G50G004004"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���Ϸ�ͼ��
                    Case "G50G004005"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���Ͻ�ͼ��
                    Case "G50G004006"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���ͼ��
                    Case "G50G005004"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                    Case "ͼ��1"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                    Case "ͼ��2"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "ͼ��3"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                    Case "ͼ��4"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        'Case "ͼ��3"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(4) + vXoffset(1)) / 2, (vYoffset(4) + vYoffset(1)) / 2)
                        '    End If
                        'Case "ͼ��4"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(1) + vXoffset(2)) / 2, (vYoffset(1) + vYoffset(2)) / 2)
                        '    End If
                        'Case "ͼ��5"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(2) + vXoffset(3)) / 2, (vYoffset(2) + vYoffset(3)) / 2)
                        '    End If
                        'Case "ͼ��6"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(3) + vXoffset(4)) / 2, (vYoffset(3) + vYoffset(4)) / 2)
                        '    End If
                        '��ǰͼ��
                    Case "G50G005005"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, (vXoffset(3) + vXoffset(4)) / 2, (vYoffset(3) + vYoffset(4)) / 2)
                        End If
                    Case "ͼ��1"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                    Case "ͼ��2"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "ͼ��3"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                    Case "ͼ��4"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '�ұ�ͼ��
                    Case "G50G005006"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���½�ͼ��
                    Case "G50G006004"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���·�ͼ��
                    Case "G50G006005"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���½�ͼ��
                    Case "G50G006006"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If

                        '��ǰ������
                    Case "������"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, (vXoffset(1) + vXoffset(2)) / 2, (vYoffset(1) + vYoffset(2)) / 2)
                        End If
                    Case "������2"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                    Case "ͼ��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, (vXoffset(3) + vXoffset(4)) / 2, (vYoffset(3) + vYoffset(4)) / 2)
                        End If
                        'Case "����"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(3) + vXoffset(4)) / 2, (vYoffset(3) + vYoffset(4)) / 2)
                        '    End If

                    Case "�ܼ�"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                    Case "��ע��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "����ϵ"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                    Case "������ȫ��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                        'Case "���Ա"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        '    End If
                        'Case "��ͼԱ"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        '    End If
                        'Case "����Ա"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        '    End If
                    Case "1985���Ҹ̻߳�׼��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                    Case "�ȸ߾�Ϊ1�ס�"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                    Case "1995��5��XXX��ͼ��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                    Case "1996���ͼʽ��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If

                        '��Ϊͼ��������û�����û��༭�����Ծ�ֱ��������ˣ����������ݿ�ȡ�ķ�����ͬ
                    Case "553.75����"
                        'FieldValue = "����ͼ���ǵ�Y����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                    Case "385.50����"
                        ' FieldValue = "����ͼ���ǵ�X����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                        '���Ͻ�y,x����
                    Case "554.00����"
                        'FieldValue = "����ͼ���ǵ�Y����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                    Case "385.50����"
                        'FieldValue = "����ͼ���ǵ�X����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���Ͻ�����
                    Case "554.00����"
                        ' FieldValue = "����ͼ���ǵ�Y����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                    Case "385.75����"
                        'FieldValue = "����ͼ���ǵ�X����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                        '���½�����
                    Case "553.75����"
                        ' FieldValue = "����ͼ���ǵ�Y����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "385.75����"
                        'FieldValue = "����ͼ���ǵ�X����"
                        'FigureChar pSheetEnvelope.xmax, vScaleType, pTxtElement
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                End Select
            ElseIf TypeOf pElement Is IGroupElement Then
                Dim pGroupElement As IGroupElement
                Dim pElePro As IElementProperties
                pElePro = pElement
                pGroupElement = pElement
                If pElePro.Name = "��ͼ��" Or pGroupElement.ElementCount = 2 Then '���ڹ涨�Ӻϱ��ELEMENT���� ��û���ҵ����������𷽷�
                    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                    End If
                Else
                    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        Call MovePageLyrOutlbl(pElement, (vXoffset(1) + vXoffset(2)) / 2, (vYoffset(1) + vYoffset(2)) / 2)
                    End If
                End If
            ElseIf TypeOf pElement Is IMapSurroundFrame Then
                Dim pMapSurroundFrame As IMapSurroundFrame
                pMapSurroundFrame = pElement
                If TypeOf pMapSurroundFrame.Object Is IScaleBar Then
                    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        Call MovePageLyrOutlbl(pElement, (vXoffset(1) + vXoffset(2)) / 2, (vYoffset(1) + vYoffset(2)) / 2)
                    End If
                End If
            End If
            pElement = pGraphicsContainer.Next
        End While
    End Sub
    Private Sub MoveElementInBigScalePage(ByVal pPagelayout As IPageLayout, ByVal vXoffset As Collection, ByVal vYoffset As Collection)
        Dim pTxtElement As ITextElement
        Dim pGraphicsContainer As IGraphicsContainer
        pGraphicsContainer = New PageLayout
        pGraphicsContainer = pPagelayout
        Dim pElement As IElement
        pGraphicsContainer.Reset()
        pElement = pGraphicsContainer.Next

        While Not pElement Is Nothing
            If TypeOf pElement Is ITextElement Then
                pTxtElement = pElement
                Select Case pTxtElement.Text

                    Case "�л����񹲺͹������ߵ���ͼ"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���Ͻ�ͼ��
                    Case "G50G004004"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���Ϸ�ͼ��
                    Case "G50G004005"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���Ͻ�ͼ��
                    Case "G50G004006"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���ͼ��
                    Case "G50G005004"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        'Case "ͼ��1"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        '    End If
                        'Case "ͼ��2"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        '    End If
                        'Case "ͼ��3"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        '    End If
                        'Case "ͼ��4"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        '    End If
                        'Case "ͼ��3"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(4) + vXoffset(1)) / 2, (vYoffset(4) + vYoffset(1)) / 2)
                        '    End If
                        'Case "ͼ��4"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(1) + vXoffset(2)) / 2, (vYoffset(1) + vYoffset(2)) / 2)
                        '    End If
                        'Case "ͼ��5"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(2) + vXoffset(3)) / 2, (vYoffset(2) + vYoffset(3)) / 2)
                        '    End If
                        'Case "ͼ��6"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(3) + vXoffset(4)) / 2, (vYoffset(3) + vYoffset(4)) / 2)
                        '    End If
                        '��ǰͼ��
                    Case "G50G005005"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, (vXoffset(3) + vXoffset(4)) / 2, (vYoffset(3) + vYoffset(4)) / 2)
                        End If
                        'Case "ͼ��1"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        '    End If
                        'Case "ͼ��2"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        '    End If
                        'Case "ͼ��3"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        '    End If
                        'Case "ͼ��4"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        '    End If
                        '�ұ�ͼ��
                    Case "G50G005006"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���½�ͼ��
                    Case "G50G006004"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���·�ͼ��
                    Case "G50G006005"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���½�ͼ��
                    Case "G50G006006"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If

                        '��ǰ������
                    Case "������"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, (vXoffset(1) + vXoffset(2)) / 2, (vYoffset(1) + vYoffset(2)) / 2)
                        End If
                        'Case "������2"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        '    End If
                    Case "ͼ��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, (vXoffset(3) + vXoffset(4)) / 2, (vYoffset(3) + vYoffset(4)) / 2)
                        End If
                        'Case "����"
                        '    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        '        Call MovePageLyrOutlbl(pElement, (vXoffset(3) + vXoffset(4)) / 2, (vYoffset(3) + vYoffset(4)) / 2)
                        '    End If

                    Case "�ܼ�"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                    Case "��ע��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "����ϵ"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "������ȫ��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                    Case "���Ա"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "��ͼԱ"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "����Ա"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "1985���Ҹ̻߳�׼��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "�ȸ߾�Ϊ1�ס�"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "1995��5��XXX��ͼ��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "1996���ͼʽ��"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If

                        '��Ϊͼ��������û�����û��༭�����Ծ�ֱ��������ˣ����������ݿ�ȡ�ķ�����ͬ
                    Case "553.75����"
                        'FieldValue = "����ͼ���ǵ�Y����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                    Case "385.50����"
                        ' FieldValue = "����ͼ���ǵ�X����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(1), vYoffset(1))
                        End If
                        '���Ͻ�y,x����
                    Case "554.00����"
                        'FieldValue = "����ͼ���ǵ�Y����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                    Case "385.50����"
                        'FieldValue = "����ͼ���ǵ�X����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                        End If
                        '���Ͻ�����
                    Case "554.00����"
                        ' FieldValue = "����ͼ���ǵ�Y����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                    Case "385.75����"
                        'FieldValue = "����ͼ���ǵ�X����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(3), vYoffset(3))
                        End If
                        '���½�����
                    Case "553.75����"
                        ' FieldValue = "����ͼ���ǵ�Y����"
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case "385.75����"
                        'FieldValue = "����ͼ���ǵ�X����"
                        'FigureChar pSheetEnvelope.xmax, vScaleType, pTxtElement
                        If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                            Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                        End If
                    Case Else
                        Dim pElePro As IElementProperties
                        pElePro = pElement
                        If pElePro.Name.Contains("���½Ǳ�ע1") Then
                            If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                                Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2) + 1.190625)
                            End If
                        End If
                        If pElePro.Name.Contains("���½Ǳ�ע2") Then
                            If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                                Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                            End If
                        End If
                        If pElePro.Name.Contains("���½Ǳ�ע") Then
                            If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                                Call MovePageLyrOutlbl(pElement, vXoffset(2), vYoffset(2))
                            End If
                        End If
                End Select
            ElseIf TypeOf pElement Is IGroupElement Then
                Dim pGroupElement As IGroupElement
                pGroupElement = pElement
                If pGroupElement.ElementCount = 2 Then '���ڹ涨�Ӻϱ��ELEMENT���� ��û���ҵ����������𷽷�
                    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        Call MovePageLyrOutlbl(pElement, vXoffset(4), vYoffset(4))
                    End If
                Else
                    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        Call MovePageLyrOutlbl(pElement, (vXoffset(1) + vXoffset(2)) / 2, (vYoffset(1) + vYoffset(2)) / 2)
                    End If
                End If
            ElseIf TypeOf pElement Is IMapSurroundFrame Then
                Dim pMapSurroundFrame As IMapSurroundFrame
                pMapSurroundFrame = pElement
                If TypeOf pMapSurroundFrame.Object Is IScaleBar Then
                    If (Not vXoffset Is Nothing) And (Not vYoffset Is Nothing) Then
                        Call MovePageLyrOutlbl(pElement, (vXoffset(1) + vXoffset(2)) / 2, (vYoffset(1) + vYoffset(2)) / 2)
                    End If
                End If
            End If
            pElement = pGraphicsContainer.Next
        End While
    End Sub

    Private Sub OpenTemplateElement(ByVal vType As String, ByVal pGeometry As IGeometry, ByVal vPagelayout As IPageLayoutControl)
        Dim pMapDoc As IMapDocument
        Dim pElement As IElement
        Dim pGraphicsContainer As IGraphicsContainer
        Dim pLayOutGrapContainer As IGraphicsContainer

        pMapDoc = New MapDocument
        If type_ZT = 0 Then
            Select Case vType
                Case "500", "1000", "2000"
                    pMapDoc.Open(System.Windows.Forms.Application.StartupPath & "\..\Template\500Template.mxd")
                Case "5000"
                    pMapDoc.Open(System.Windows.Forms.Application.StartupPath & "\..\Template\5000Template.mxd")
                Case "10000"
                    pMapDoc.Open(System.Windows.Forms.Application.StartupPath & "\..\Template\10000Template.mxd")
                Case "25000"
                    pMapDoc.Open(System.Windows.Forms.Application.StartupPath & "\..\Template\10000Template.mxd")
                Case "50000"
                    pMapDoc.Open(System.Windows.Forms.Application.StartupPath & "\..\Template\50000Template.mxd")
                Case "250000"
                    pMapDoc.Open(System.Windows.Forms.Application.StartupPath & "\..\Template\250000Template.mxd")
            End Select
        ElseIf type_ZT = 1 Then '20110914 yjl add for tdly
            
            pMapDoc.Open(System.Windows.Forms.Application.StartupPath & "\..\Template\TDLY10000Template.mxd")
         
        End If

        '����ɾ��ԭ��Element
        pLayOutGrapContainer = vPagelayout.PageLayout
        pLayOutGrapContainer.Reset()
        pElement = pLayOutGrapContainer.Next
        While Not pElement Is Nothing
            If TypeOf pElement Is IMapFrame Then
                g_MapFrameEle = pElement
            End If
            If Not (TypeOf pElement Is IMapFrame) Then  'And Not (TypeOf pElement Is IFrameElement)
                pLayOutGrapContainer.DeleteElement(pElement)
                pLayOutGrapContainer.Reset()  '//ɾ����֮��Ҫreset����Ȼɾ������
            End If
            pElement = pLayOutGrapContainer.Next
        End While

        '�����ͼģ���е�Element
        pGraphicsContainer = pMapDoc.PageLayout
        pGraphicsContainer.Reset()
        pElement = pGraphicsContainer.Next
        While Not pElement Is Nothing
            'If CLng(vType) < 5000 Then
            '    If Not (TypeOf pElement Is ITextElement) Then   'And Not (TypeOf pElement Is IFrameElement)
            '        pLayOutGrapContainer.AddElement(pElement, 0)
            '    End If
            '    pElement = pGraphicsContainer.Next
            'Else
            If Not (TypeOf pElement Is IMapFrame) And Not (TypeOf pElement Is ITextElement) Then   'And Not (TypeOf pElement Is IFrameElement)
                pLayOutGrapContainer.AddElement(pElement, 0)
                If TypeOf pElement Is IRectangleElement Then
                    '�����ͼ�ľ��βο���
                    g_pRecElement = pElement
                End If
            End If
            pElement = pGraphicsContainer.Next
            'End If

        End While


        'copy����

        CopyToPageLayout(pGeometry, vPagelayout, CLng(vType))
        If CLng(vType) >= 5000 Then
            '���ݲ�ͬ�ı���������ͼ���������һ����ͼ��
            DrawMapBroderIn(CLng(vType), pGeometry, vPagelayout)
            '���������
            ' DrawKmGrid
        End If
        pGraphicsContainer.Reset()
        pElement = pGraphicsContainer.Next
        While Not pElement Is Nothing
            If (TypeOf pElement Is ITextElement) Then   'And Not (TypeOf pElement Is IFrameElement)
                pLayOutGrapContainer.AddElement(pElement, 0)

            End If
            pElement = pGraphicsContainer.Next
        End While
    End Sub

    '�Ե����� ���Ͻ�Ϊ��һ��(����ֻ��һ�������� �������ε� ��X Yƫ���̫��
    Private Sub OrderPointCol(ByVal pPointCol As IPointCollection)
        '���ھ�ȷ��Ϊ�ĸ���
        Dim pMinPointTemp As IPoint = Nothing
        Dim pMaxPointTemp As IPoint = Nothing
        Dim pLeftPointTemp As IPoint = Nothing
        Dim pRightPointTemp As IPoint = Nothing

        Dim dblA As Double
        Dim IntMin As Integer
        Dim IntMax As Integer
        Dim i As Integer

        '������½ǵ� ���Ͻǵ�
        For i = 0 To 3
            If dblA < pPointCol.Point(i).x + pPointCol.Point(i).y Then
                pMaxPointTemp = pPointCol.Point(i)
                IntMax = i
                dblA = pPointCol.Point(i).x + pPointCol.Point(i).y
            End If
        Next i
        dblA = 0
        For i = 0 To 3
            If pPointCol.Point(i).x + pPointCol.Point(i).y < dblA Or dblA = 0 Then
                pMinPointTemp = pPointCol.Point(i)
                IntMin = i
                dblA = pPointCol.Point(i).x + pPointCol.Point(i).y
            End If
        Next i

        For i = 0 To 3
            If i <> IntMin And i <> IntMax Then
                If Not pLeftPointTemp Is Nothing Then
                    If pLeftPointTemp.x > pPointCol.Point(i).x Then
                        pRightPointTemp = pLeftPointTemp
                        pLeftPointTemp = pPointCol.Point(i)
                        Exit For
                    Else
                        pRightPointTemp = pPointCol.Point(i)
                    End If
                Else
                    pLeftPointTemp = pPointCol.Point(i)
                End If
            End If
        Next i

        pPointCol = Nothing
        pPointCol = New Polygon
        pPointCol.AddPoint(pLeftPointTemp)
        pPointCol.AddPoint(pMinPointTemp)
        pPointCol.AddPoint(pRightPointTemp)
        pPointCol.AddPoint(pMaxPointTemp)
        pPointCol.AddPoint(pLeftPointTemp)

        pLeftPointTemp = Nothing
        pMinPointTemp = Nothing
        pRightPointTemp = Nothing
        pMaxPointTemp = Nothing

    End Sub

    '�õ�����ͼ���Ա����������ƽ�浽ͼֽ��ת�� ͬʱ�õ��ο����ο� ��õĲ������Թ��ñ�������ʽ����
    '�ڵõ�һЩ������ͬʱ �ֽ�����һЩ��Ҫ�Ĵ������
    Private Sub GetMapAndPagerEn(ByVal pPagelayout As IPageLayout)
        Dim pGrapContainer As IGraphicsContainer
        Dim pMapelement As IMapFrame
        Dim pElement As IElement
        Dim pBorderSym As ISymbolBorder
        Dim pLineSym As ISimpleLineSymbol
        Dim pAv As IActiveView

        pGrapContainer = pPagelayout
        pGrapContainer.Reset()
        pElement = pGrapContainer.Next

        Do While Not pElement Is Nothing

            '�����ͼ��
            If TypeOf pElement Is IMapFrame Then
                pMapelement = pElement
                pAv = pMapelement.Map

                '��ù��ñ�����Ϣ
                g_pMapFrame = pElement
                g_pMapEnvelope = pAv.Extent
                g_pPaperEnvelope = pElement.Geometry.Envelope

                '������ͼ���ݿ�ı��߿��Ϊ0
                On Error Resume Next
                If Not pMapelement.Border Is Nothing Then
                    pBorderSym = pMapelement.Border
                    pLineSym = pBorderSym.LineSymbol
                    pLineSym.Width = 0

                    pBorderSym.LineSymbol = pLineSym
                    pMapelement.Border = pBorderSym
                End If

            ElseIf TypeOf pElement Is IRectangleElement Then
                '�����ͼ�ľ��βο���
                g_pRecElement = pElement
            End If
            pElement = pGrapContainer.Next
        Loop

    End Sub

    '��ͼ��������ͼ��  ˼·���ȸ���ͼ���ļ�����״����ͼ�� �ٸ�����ͼ����ͼ�򣨱��ߣ�
    Private Sub DrawMapBroderIn(ByVal lngScale As Long, ByVal pGeometry As IGeometry, ByVal vPagelayout As IPageLayoutControl)
        Dim pFillElement As IFillShapeElement
        Dim pGrapContainer As IGraphicsContainer
        Dim pFillSymbol As IFillSymbol
        Dim pColor As IColor
        Dim pOutline As ILineSymbol
        Dim pLineColor As IColor
        Dim pElement As IElement
        Dim pPointCol As IPointCollection
        Dim pPolygon As IPolygon
        Dim i As Integer
        '''''''''''''''''''''''''''''''''''''''''''''''''''''
        Dim pInLinePolygon As IPolygon
        pInLinePolygon = New Polygon
        Dim pInLinePointCol As IPointCollection
        pInLinePointCol = New Polygon
        Dim pMap As IMap
        Dim pMapActive As IActiveView
        pMap = vPagelayout.ActiveView.FocusMap
        pMapActive = pMap

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''

        pFillElement = New PolygonElement
        pFillSymbol = New MarkerFillSymbol
        pColor = New RgbColor
        pLineColor = New RgbColor
        pOutline = New SimpleLineSymbol

        '��ͼ��ı�����ʽ
        pColor.NullColor = True
        pLineColor.RGB = RGB(0, 0, 0)
        pFillSymbol.Color = pColor
        pOutline.Color = pLineColor
        pOutline.Width = 1.5
        pFillSymbol.Outline = pOutline

        '�����ͼ����Ϣ
        Call GetMapAndPagerEn(vPagelayout.PageLayout)
        '��ò�������±�����Ϣ
        m_clsGetGeoInfo.ChangeMapToPaper()

        pGrapContainer = vPagelayout.PageLayout

        '���ĵ�����ת��Ϊͼֽ���� ���ŵ������� ���˳������Ϊ���Ͻ� ���½� ���½� ���Ͻ�
        pPointCol = New Polygon
        pPointCol.AddPoint(GetPaperPoint(g_pMapEnvelope.LowerLeft, g_pMapEnvelope.UpperRight, g_pPaperEnvelope.LowerLeft, g_pPaperEnvelope.UpperRight, m_clsGetGeoInfo.m_pPointLT1))
        pPointCol.AddPoint(GetPaperPoint(g_pMapEnvelope.LowerLeft, g_pMapEnvelope.UpperRight, g_pPaperEnvelope.LowerLeft, g_pPaperEnvelope.UpperRight, m_clsGetGeoInfo.m_pPointLB2))
        pPointCol.AddPoint(GetPaperPoint(g_pMapEnvelope.LowerLeft, g_pMapEnvelope.UpperRight, g_pPaperEnvelope.LowerLeft, g_pPaperEnvelope.UpperRight, m_clsGetGeoInfo.m_pPointRB3))
        pPointCol.AddPoint(GetPaperPoint(g_pMapEnvelope.LowerLeft, g_pMapEnvelope.UpperRight, g_pPaperEnvelope.LowerLeft, g_pPaperEnvelope.UpperRight, m_clsGetGeoInfo.m_pPointRT4))
        pPointCol.AddPoint(GetPaperPoint(g_pMapEnvelope.LowerLeft, g_pMapEnvelope.UpperRight, g_pPaperEnvelope.LowerLeft, g_pPaperEnvelope.UpperRight, m_clsGetGeoInfo.m_pPointLT1))

        '�ŵ�ģ�鼶�ı�����
        m_pPaperPointCol = pPointCol
        pPolygon = pPointCol

        '����
        Dim pSegmentCol As ISegmentCollection
        Dim pCurve As ICurve2
        Dim pPolyline As IPolyline
        Dim pFromPoint As IPoint
        Dim pToPoint As IPoint
        Dim pLineElement As ILineElement
        pSegmentCol = pPolygon

        '��ͼ���������
        Dim pConstructCurve As IConstructCurve
        Dim pColOutLine As Collection
        '��߿����������
        Dim pConstructOutin As IConstructCurve
        Dim pColOutInLine As Collection

        pColOutLine = New Collection
        pColOutInLine = New Collection

        For i = 0 To pSegmentCol.SegmentCount - 1
            pCurve = Nothing
            pFromPoint = Nothing
            pToPoint = Nothing
            pElement = Nothing

            pCurve = pSegmentCol.Segment(i)
            pPolyline = New Polyline
            pPolyline.FromPoint = pCurve.FromPoint
            pPolyline.ToPoint = pCurve.ToPoint

            '�����ǹ���ƽ����  Ϊ����ͼ����׼�� ��������ƽ���� һ������߿���ⲿ һ�����ڲ�
            Dim pConstructPolyline As IPolyline
            pConstructCurve = New Polyline
            Dim pConstructLine As IPolyline
            pConstructLine = New Polyline
            pConstructLine = pPolyline
            '������ͼ��������
            pConstructCurve.ConstructOffset(pConstructLine, 0.75, esriConstructOffsetEnum.esriConstructOffsetMitered + esriConstructOffsetEnum.esriConstructOffsetSimple)
            '������ͼ����ڱ���
            pConstructOutin = New Polyline
            pConstructOutin.ConstructOffset(pConstructLine, 0.6, esriConstructOffsetEnum.esriConstructOffsetMitered + esriConstructOffsetEnum.esriConstructOffsetSimple)

            pConstructPolyline = pConstructCurve
            pColOutLine.Add(pConstructPolyline)
            pConstructPolyline = pConstructOutin
            pColOutInLine.Add(pConstructPolyline)
        Next i

        pElement = New PolygonElement
        pFillElement.Symbol = pFillSymbol
        pElement = pFillElement
        pElement.Geometry = pPolygon
        'pElement.Locked = True
        pGrapContainer.AddElement(pElement, 0)

        'ͬʱ�ٻ���ͼ��
        Dim pOutPointCol As IPointCollection
        Dim pOutPoint As IPoint
        pOutPointCol = New Polygon

        'ȷ����ͼ�� ���ĸ�����
        pOutPoint = GetIntersection(pColOutLine.Item(1), pColOutLine.Item(4))
        pOutPointCol.AddPoint(pOutPoint)
        pOutPoint = GetIntersection(pColOutLine.Item(1), pColOutLine.Item(2))
        pOutPointCol.AddPoint(pOutPoint)
        pOutPoint = GetIntersection(pColOutLine.Item(2), pColOutLine.Item(3))
        pOutPointCol.AddPoint(pOutPoint)
        pOutPoint = GetIntersection(pColOutLine.Item(3), pColOutLine.Item(4))
        pOutPointCol.AddPoint(pOutPoint)
        pOutPoint = GetIntersection(pColOutLine.Item(1), pColOutLine.Item(4))
        pOutPointCol.AddPoint(pOutPoint)

        pPolygon = pOutPointCol
        '����ͼ�������
        pOutline.Width = 1.5
        pFillSymbol.Outline = pOutline
        pFillElement.Symbol = pFillSymbol
        pElement = pFillElement
        pElement.Geometry = pPolygon
        pElement.Locked = True
        pGrapContainer.AddElement(pElement, 0)

        Dim pPolygonOutIn As IPolygon = Nothing
        pOutPointCol = Nothing
        pOutPointCol = New Polygon

        '����Ǳ����ߴ��ڵ���10000������Ҫ��߿������
        If lngScale > 10000 Then
            '����ͼ����ڱ���
            pOutPoint = GetIntersection(pColOutInLine.Item(1), pColOutInLine.Item(4))
            pOutPointCol.AddPoint(pOutPoint)
            pOutPoint = GetIntersection(pColOutInLine.Item(1), pColOutInLine.Item(2))
            pOutPointCol.AddPoint(pOutPoint)
            pOutPoint = GetIntersection(pColOutInLine.Item(2), pColOutInLine.Item(3))
            pOutPointCol.AddPoint(pOutPoint)
            pOutPoint = GetIntersection(pColOutInLine.Item(3), pColOutInLine.Item(4))
            pOutPointCol.AddPoint(pOutPoint)
            pOutPoint = GetIntersection(pColOutInLine.Item(1), pColOutInLine.Item(4))
            pOutPointCol.AddPoint(pOutPoint)

            pPolygonOutIn = pOutPointCol
            '����ͼ����ڱ���
            pOutline.Width = 0.1
            pFillElement = New PolygonElement
            pFillSymbol.Outline = pOutline
            pFillElement.Symbol = pFillSymbol
            pElement = pFillElement
            pElement.Geometry = pPolygonOutIn
            pElement.Locked = True
            pGrapContainer.AddElement(pElement, 0)

        End If

        '�����ڻ���ͼ��
        Dim pSegmentOut As ISegmentCollection
        pSegmentOut = pPolygon

        '�Ȼ���γ�ȸ�����С����
        'Dim lngMinX As Double
        'Dim lngMinY As Double

        '    Dim lngCur As Long
        'If lngScale > 10000 Then
        '    '������Ͻ�����
        '    GetCoordinateFromNewCode(m_strSheetNo, lngMinX, lngMinY)

        '    DrawCoordinameGridLine(pOutPointCol.Point(0), pOutPointCol.Point(1), pSegmentOut.Segment(0), lngMinX, lngMinY, 1, lngScale, g_pMapFrame, pGrapContainer)
        '    DrawCoordinameGridLine(pOutPointCol.Point(1), pOutPointCol.Point(2), pSegmentOut.Segment(1), lngMinX, lngMinY, 2, lngScale, g_pMapFrame, pGrapContainer)
        '    DrawCoordinameGridLine(pOutPointCol.Point(2), pOutPointCol.Point(3), pSegmentOut.Segment(2), lngMinX, lngMinY, 3, lngScale, g_pMapFrame, pGrapContainer)
        '    DrawCoordinameGridLine(pOutPointCol.Point(3), pOutPointCol.Point(4), pSegmentOut.Segment(3), lngMinX, lngMinY, 4, lngScale, g_pMapFrame, pGrapContainer)
        'End If

        Dim pTempSegmentCol As ISegmentCollection
        '��ͼ����ʽ
        pOutline.Width = 0.1
        '��һ��
        pCurve = pSegmentCol.Segment(0)
        pFromPoint = GetIntersectionSeg(pSegmentCol.Segment(0), pSegmentOut.Segment(1), True)
        pToPoint = GetIntersectionSeg(pSegmentCol.Segment(0), pSegmentOut.Segment(3), True)
        pPolyline = New Polyline
        pPolyline.FromPoint = pFromPoint
        pPolyline.ToPoint = pToPoint

        pLineElement = New LineElement
        pLineElement.Symbol = pOutline
        pElement = pLineElement
        pElement.Geometry = pPolyline
        pElement.Locked = True
        pGrapContainer.AddElement(pElement, 0)

        '�ڶ���
        '�ڴ˴����в�����  ***************************88
        pCurve = pSegmentCol.Segment(1)
        pPolyline = New Polyline
        pPolyline = m_clsGetGeoInfo.m_pTopLineGeometry
        pTempSegmentCol = pPolyline

        pFromPoint = GetIntersectionSeg(pTempSegmentCol.Segment(0), pSegmentOut.Segment(0), True)
        pToPoint = GetIntersectionSeg(pTempSegmentCol.Segment(pTempSegmentCol.SegmentCount - 1), pSegmentOut.Segment(2), True)

        pPolyline.FromPoint = pFromPoint
        pPolyline.ToPoint = pToPoint

        pLineElement = New LineElement
        pLineElement.Symbol = pOutline
        pElement = pLineElement
        pElement.Geometry = pPolyline
        pElement.Locked = True
        pGrapContainer.AddElement(pElement, 0)

        '������
        pCurve = pSegmentCol.Segment(2)
        pFromPoint = GetIntersectionSeg(pSegmentCol.Segment(2), pSegmentOut.Segment(1), True)
        pToPoint = GetIntersectionSeg(pSegmentCol.Segment(2), pSegmentOut.Segment(3), True)
        pPolyline = New Polyline
        pPolyline.FromPoint = pFromPoint
        pPolyline.ToPoint = pToPoint

        pLineElement = New LineElement
        pLineElement.Symbol = pOutline
        pElement = pLineElement
        pElement.Geometry = pPolyline
        pElement.Locked = True
        pGrapContainer.AddElement(pElement, 0)

        '������
        pCurve = pSegmentCol.Segment(3)
        pPolyline = New Polyline
        pPolyline = m_clsGetGeoInfo.m_pBottomLineGeometry
        pTempSegmentCol = pPolyline

        pFromPoint = GetIntersectionSeg(pTempSegmentCol.Segment(0), pSegmentOut.Segment(0), True)
        pToPoint = GetIntersectionSeg(pTempSegmentCol.Segment(pTempSegmentCol.SegmentCount - 1), pSegmentOut.Segment(2), True)

        pPolyline.FromPoint = pFromPoint
        pPolyline.ToPoint = pToPoint

        pLineElement = New LineElement
        pLineElement.Symbol = pOutline
        pElement = pLineElement
        pElement.Geometry = pPolyline
        pElement.Locked = True
        pGrapContainer.AddElement(pElement, 0)

        Dim lngPaperIndex As Double
        Dim lngMinIndex As Long
        Dim lngMaxIndex As Long
        'Dim pGridLine As Polyline
        Dim pGridSegment As ISegment

        '����ĸ���ͼ�����ԭ����ͼ����ƫ��
        m_vXoffset = New Collection
        m_vYoffset = New Collection
        pElement = g_pRecElement

        m_vXoffset.Add(pSegmentOut.Segment(1).FromPoint.X - pElement.Geometry.Envelope.LowerLeft.X)
        m_vXoffset.Add(pSegmentOut.Segment(2).FromPoint.X - pElement.Geometry.Envelope.LowerRight.X)
        m_vXoffset.Add(pSegmentOut.Segment(3).FromPoint.X - pElement.Geometry.Envelope.UpperRight.X)
        m_vXoffset.Add(pSegmentOut.Segment(0).FromPoint.X - pElement.Geometry.Envelope.UpperLeft.X)

        m_vYoffset.Add(pSegmentOut.Segment(1).FromPoint.Y - pElement.Geometry.Envelope.LowerLeft.Y)
        m_vYoffset.Add(pSegmentOut.Segment(2).FromPoint.Y - pElement.Geometry.Envelope.LowerRight.Y)
        m_vYoffset.Add(pSegmentOut.Segment(3).FromPoint.Y - pElement.Geometry.Envelope.UpperRight.Y)
        m_vYoffset.Add(pSegmentOut.Segment(0).FromPoint.Y - pElement.Geometry.Envelope.UpperLeft.Y)

        '���������
        Dim IntGridLen As Integer
        IntGridLen = 1

        If lngScale > 50000 Then
            IntGridLen = 2
        End If

        lngMinIndex = Fix(pGeometry.Envelope.XMin / 1000)
        lngMaxIndex = Fix(pGeometry.Envelope.XMax / 1000)
        Dim strTemp As String

        '���������
        '����Ǳ����ߴ���10000�������ֱ��������ߵ���ͼ���ཻ
        If lngScale > 10000 Then
            pSegmentOut = pPolygonOutIn
        Else
            pSegmentOut = pPolygon
        End If

        strTemp = "0"

        For i = lngMinIndex + 1 To lngMaxIndex Step IntGridLen

            If i > lngMaxIndex Then Exit For
            lngPaperIndex = GetPaperXY(g_pMapEnvelope.LowerLeft, g_pMapEnvelope.UpperRight, g_pPaperEnvelope.LowerLeft, g_pPaperEnvelope.UpperRight, CDbl(i * 1000.0#), 0.0#)
            pGridSegment = New Line
            pFromPoint = New Point
            pToPoint = New Point
            pFromPoint.PutCoords(lngPaperIndex, 10)
            pToPoint.PutCoords(lngPaperIndex, 15)
            pGridSegment.FromPoint = pFromPoint
            pGridSegment.ToPoint = pToPoint



            '�����������ͼ��Ľ���
            pFromPoint = GetIntersectionSeg(pGridSegment, pSegmentOut.Segment(3), False) '��
            pToPoint = GetIntersectionSeg(pGridSegment, pSegmentOut.Segment(1), False) '��

            If (Not pFromPoint Is Nothing) And (Not pToPoint Is Nothing) Then

                '�жϽ��б��� ֻ�й��������ͼ�����ڲŽ��л���
                If (m_pPaperPointCol.Point(3).X - pFromPoint.X) * (pFromPoint.X - m_pPaperPointCol.Point(0).X) > 0 And _
                    (m_pPaperPointCol.Point(2).X - pToPoint.X) * (pToPoint.X - m_pPaperPointCol.Point(1).X) > 0 Then

                    pPolyline = New Polyline
                    pPolyline.FromPoint = pFromPoint
                    pPolyline.ToPoint = pToPoint
                    pLineElement = New LineElement
                    pLineElement.Symbol = pOutline
                    pElement = pLineElement
                    pElement.Geometry = pPolyline
                    pElement.Locked = True
                    pGrapContainer.AddElement(pElement, 0)

                    '�����С��ʾ       ˮƽ����
                    '���˵�����߿�̫�� �򲻽��л��� ��׼����ͼ����һ�����ײŽ��л���
                    If (pFromPoint.X - m_pPaperPointCol.Point(0).X) > 0.5 And (m_pPaperPointCol.Point(3).X - pFromPoint.X) > 0.5 Then
                        If CLng(strTemp) > CLng(Right(CStr(i), 2)) Or i = lngMinIndex + 1 Or i = lngMaxIndex Then
                            pFromPoint.X = pFromPoint.X '+ 0.23
                            pFromPoint.Y = (pFromPoint.Y + m_pPaperPointCol.Point(3).Y) / 2 '- 0.34


                            pElement = DrawGridText(pFromPoint, Right(CStr(i), 2), 12, esriTextHorizontalAlignment.esriTHALeft, esriTextVerticalAlignment.esriTVACenter)
                            'pElement.Locked = True
                            pGrapContainer.AddElement(pElement, 0)

                            pToPoint.X = pToPoint.X '+ 0.25
                            pToPoint.Y = (pToPoint.Y + m_pPaperPointCol.Point(2).Y) / 2
                            pElement = DrawGridText(pToPoint, Right(CStr(i), 2), 12, esriTextHorizontalAlignment.esriTHALeft, esriTextVerticalAlignment.esriTVACenter)
                            'pElement.Locked = True
                            pGrapContainer.AddElement(pElement, 0)

                            pFromPoint.X = pFromPoint.X ' - 0.35
                            pFromPoint.Y = pFromPoint.Y + 0.05
                            pElement = DrawGridText(pFromPoint, Left(CStr(i), Len(CStr(i)) - 2), 8, esriTextHorizontalAlignment.esriTHARight, esriTextVerticalAlignment.esriTVACenter)
                            'pElement.Locked = True
                            pGrapContainer.AddElement(pElement, 0)

                            pToPoint.X = pToPoint.X '- 0.35
                            pToPoint.Y = pToPoint.Y + 0.05
                            pElement = DrawGridText(pToPoint, Left(CStr(i), Len(CStr(i)) - 2), 8, esriTextHorizontalAlignment.esriTHARight, esriTextVerticalAlignment.esriTVACenter)
                            'pElement.Locked = True
                            pGrapContainer.AddElement(pElement, 0)
                        Else
                            'д�����ı�ʶ
                            pFromPoint.X = pFromPoint.X '+ 0.23
                            pFromPoint.Y = (pFromPoint.Y + m_pPaperPointCol.Point(3).Y) / 2 '- 0.34
                            pElement = DrawGridText(pFromPoint, Right(CStr(i), 2), 12, esriTextHorizontalAlignment.esriTHALeft, esriTextVerticalAlignment.esriTVACenter)
                            'pElement.Locked = True
                            pGrapContainer.AddElement(pElement, 0)

                            pToPoint.X = pToPoint.X '+ 0.25
                            pToPoint.Y = (pToPoint.Y + m_pPaperPointCol.Point(2).Y) / 2 '+ 0.02
                            pElement = DrawGridText(pToPoint, Right(CStr(i), 2), 12, esriTextHorizontalAlignment.esriTHALeft, esriTextVerticalAlignment.esriTVACenter)
                            'pElement.Locked = True
                            pGrapContainer.AddElement(pElement, 0)
                        End If
                    End If
                    '*********����Ϊ�������������
                End If
            End If

            strTemp = Right(CStr(i), 2)
        Next i

        lngMinIndex = Fix(pGeometry.Envelope.YMin / 1000)
        lngMaxIndex = Fix(pGeometry.Envelope.YMax / 1000)
        strTemp = "0"

        Try
            For i = lngMinIndex + 1 To lngMaxIndex Step IntGridLen

                If i > lngMaxIndex Then Exit For
                lngPaperIndex = GetPaperXY(g_pMapEnvelope.LowerLeft, g_pMapEnvelope.UpperRight, g_pPaperEnvelope.LowerLeft, g_pPaperEnvelope.UpperRight, 0, i * 1000.0#)
                pGridSegment = New Line
                pFromPoint = New Point
                pToPoint = New Point
                pFromPoint.PutCoords(10, lngPaperIndex)
                pToPoint.PutCoords(15, lngPaperIndex)
                pGridSegment.FromPoint = pFromPoint
                pGridSegment.ToPoint = pToPoint

                '�����������ͼ��Ľ���
                pFromPoint = GetIntersectionSeg(pGridSegment, pSegmentOut.Segment(2), False) '��
                pToPoint = GetIntersectionSeg(pGridSegment, pSegmentOut.Segment(0), False) '��

                If (Not pFromPoint Is Nothing) And (Not pToPoint Is Nothing) Then
                    If (m_pPaperPointCol.Point(0).Y - pToPoint.Y) * (pToPoint.Y - m_pPaperPointCol.Point(1).Y) > 0 And _
                        (m_pPaperPointCol.Point(3).Y - pFromPoint.Y) * (pFromPoint.Y - m_pPaperPointCol.Point(2).Y) > 0 Then

                        pPolyline = New Polyline
                        pPolyline.FromPoint = pFromPoint
                        pPolyline.ToPoint = pToPoint
                        pLineElement = New LineElement
                        pLineElement.Symbol = pOutline
                        pElement = pLineElement
                        pElement.Geometry = pPolyline
                        pElement.Locked = True
                        pGrapContainer.AddElement(pElement, 0)
                        '��ֱ����
                        '���˵�����߿�̫�� �򲻽��л��� ��׼����ͼ����һ�����ײŽ��л��� ����ֻ���ж���һ�����Ƿ����߿����0.5������
                        If (pFromPoint.Y - m_pPaperPointCol.Point(2).Y) > 0.5 And (m_pPaperPointCol.Point(3).Y - pFromPoint.Y) > 0.5 Then
                            If CLng(strTemp) > CLng(Right(CStr(i), 2)) Or i = lngMinIndex + 1 Or i = lngMaxIndex Then

                                pFromPoint.X = (m_pPaperPointCol.Point(2).X + pFromPoint.X) / 2 - 0.05 '- 0.18
                                pFromPoint.Y = pFromPoint.Y
                                pElement = DrawGridText(pFromPoint, Right(CStr(i), 2), 12, esriTextHorizontalAlignment.esriTHALeft, esriTextVerticalAlignment.esriTVABaseline)
                                'pElement.Locked = True
                                pGrapContainer.AddElement(pElement, 0)

                                pToPoint.X = (m_pPaperPointCol.Point(1).X + pToPoint.X) / 2  '+ 0.42
                                pToPoint.Y = pToPoint.Y
                                pElement = DrawGridText(pToPoint, Right(CStr(i), 2), 12, esriTextHorizontalAlignment.esriTHALeft, esriTextVerticalAlignment.esriTVABaseline)
                                'pElement.Locked = True
                                pGrapContainer.AddElement(pElement, 0)

                                'pFromPoint.X = pFromPoint.X ' - 0.3
                                pFromPoint.Y = pFromPoint.Y + 0.1

                                pElement = DrawGridText(pFromPoint, Left(CStr(i), Len(CStr(i)) - 2), 8, esriTextHorizontalAlignment.esriTHARight, esriTextVerticalAlignment.esriTVABaseline)
                                'pElement.Locked = True
                                pGrapContainer.AddElement(pElement, 0)

                                'pToPoint.X = pToPoint.X '- 0.3
                                pToPoint.Y = pToPoint.Y + 0.1
                                pElement = DrawGridText(pToPoint, Left(CStr(i), Len(CStr(i)) - 2), 8, esriTextHorizontalAlignment.esriTHARight, esriTextVerticalAlignment.esriTVABaseline)
                                'pElement.Locked = True
                                pGrapContainer.AddElement(pElement, 0)
                            Else
                                'д�����ı�ʶ
                                'pFromPoint.X = pFromPoint.X '- 0.25
                                'pFromPoint.Y = pFromPoint.Y
                                pElement = DrawGridText(pFromPoint, Right(CStr(i), 2), 12, esriTextHorizontalAlignment.esriTHARight, esriTextVerticalAlignment.esriTVABaseline)
                                'pElement.Locked = True
                                pGrapContainer.AddElement(pElement, 0)

                                'pToPoint.X = pToPoint.X '+ 0.24
                                'pToPoint.Y = pToPoint.Y
                                pElement = DrawGridText(pToPoint, Right(CStr(i), 2), 12, esriTextHorizontalAlignment.esriTHALeft, esriTextVerticalAlignment.esriTVABaseline)
                                'pElement.Locked = True
                                pGrapContainer.AddElement(pElement, 0)
                            End If
                        End If

                    End If
                    '****����Ϊ�������������
                    strTemp = Right(CStr(i), 2)
                End If

            Next i
        Catch ex As Exception
            Debug.Write(ex.Message)
        End Try

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        pInLinePointCol.AddPoint(m_clsGetGeoInfo.m_pPointLT1)
        pInLinePointCol.AddPoint(m_clsGetGeoInfo.m_pPointLB2)
        pInLinePointCol.AddPoint(m_clsGetGeoInfo.m_pPointRB3)
        pInLinePointCol.AddPoint(m_clsGetGeoInfo.m_pPointRT4)
        pInLinePointCol.AddPoint(m_clsGetGeoInfo.m_pPointLT1)

        pInLinePolygon = pInLinePointCol

        Dim pInLineGeo As IGeometry
        pInLinePolygon.SpatialReference = pMap.SpatialReference

        pInLineGeo = pInLinePolygon
        Dim pTopo As ITopologicalOperator4
        pTopo = pInLineGeo
        pTopo.IsKnownSimple_2 = False
        pTopo.Simplify()
        pMap.ClipGeometry = pInLineGeo
        pInLinePointCol = Nothing
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        pFillElement = Nothing
        pFillSymbol = Nothing
        pColor = Nothing
        pOutline = Nothing

    End Sub




    '����γ�ȸ�������
    Private Sub DrawCoordinameGridLine(ByVal pFromPoint As IPoint, ByVal pToPoint As IPoint, _
        ByVal pLine As ILine, ByVal lngMinX As Double, ByVal lngMinY As Double, _
        ByVal vType As Integer, ByVal lngScale As Long, ByVal pMapelement As IMapFrame, ByVal pGrapContainer As IGraphicsContainer)

        Dim lngFZ As Double
        Dim pBaseLine As ILine
        pBaseLine = New Line
        pBaseLine.FromPoint = pFromPoint
        pBaseLine.ToPoint = pToPoint

        '�ж���������������
        Dim dblWidth As Double
        Dim dblHeight As Double
        Dim blnWidth As Boolean
        dblWidth = pFromPoint.x - pToPoint.x
        dblHeight = pFromPoint.y - pToPoint.y
        If Math.Abs(dblWidth) > Math.Abs(dblHeight) Then
            blnWidth = True
        End If

        Dim IntLineCount As Integer
        Dim i As Integer
        Dim lngMin As Long
        Dim pNormalLine As ILine
        Select Case lngScale
            Case 50000
                If blnWidth Then
                    IntLineCount = 15
                    lngMin = lngMinX
                Else
                    IntLineCount = 10
                    lngMin = lngMinY
                End If
            Case 100000

            Case 250000
                If blnWidth Then
                    IntLineCount = 90
                    lngMin = lngMinX
                Else
                    IntLineCount = 60
                    lngMin = lngMinY
                End If
            Case 500000

            Case 1000000

            Case Else
                Exit Sub
        End Select

        '��òο���һ������ֵ
        Select Case vType
            Case 1
                lngFZ = lngMinX
            Case 2
                lngFZ = lngMinY
            Case 3
                lngFZ = lngMinX + IntLineCount * 60
            Case 4
                lngFZ = lngMinY + IntLineCount * 60
        End Select

        Dim dblMapx As Double
        Dim dblMapY As Double
        Dim lngMapX As Long
        Dim lngMapY As Long
        'Dim DblPaperX As Double
        'Dim DblPagerY As Double
        Dim pPoint1 As IPoint         '��ʵ�ľ�γ������ͼ�α����ϵ����
        Dim pPointNormal As IPoint    '����
        Dim pConstructPoint As IPoint '�����
        Dim pShortLine As IPolyline        '����С����
        'Dim pNormalLine As esriGeometry.ILine      '�ο�����
        'Dim pConstructPoint As IConstructPoint
        Dim pProximityOperator As IProximityOperator
        Dim pElement As IElement
        Dim pLineElement As ILineElement
        Dim pSimpleSymbol As ISimpleLineSymbol
        Dim pColor As IColor

        pColor = New RgbColor
        pColor.RGB = RGB(0, 0, 0)
        pSimpleSymbol = New SimpleLineSymbol
        pSimpleSymbol.Color = pColor
        pSimpleSymbol.Width = 0.1

        '�������ת������ز���
        Dim pMapEnvelope As IEnvelope
        Dim pPaperEnvelope As IEnvelope
        Dim pAv As IActiveView
        Dim pMap As IMap
        Dim pProjectReference As IProjectedCoordinateSystem
        pMap = pMapelement.Map
        pAv = pMap
        pMapEnvelope = pAv.Extent
        pElement = pMapelement
        pPaperEnvelope = pElement.Geometry.Envelope
        pProjectReference = pMap.SpatialReference

        For i = 1 To IntLineCount - 1
            '��þ�γ����
            If blnWidth = True Then
                lngMapX = lngMin + i * 60
                lngMapY = lngFZ
            Else
                lngMapY = lngMin + i * 60
                lngMapX = lngFZ
            End If

            '���ƽ������
            GetPanleByCoordinate(lngMapX / 3600, lngMapY / 3600, pProjectReference, dblMapx, dblMapY)
            pPoint1 = New Point
            pPoint1.X = dblMapx
            pPoint1.Y = dblMapY

            Dim pPaperPoint As IPoint
            '���ͼֽ����
            pPaperPoint = GetPaperPoint(pMapEnvelope.LowerLeft, pMapEnvelope.UpperRight, pPaperEnvelope.LowerLeft, pPaperEnvelope.UpperRight, pPoint1)

            '�ҵ�����õ������ֱ���ϵĵ� ���ҹ���һ����
            pProximityOperator = pBaseLine
            pPointNormal = pProximityOperator.ReturnNearestPoint(pPaperPoint, esriSegmentExtension.esriNoExtension)

            '����һ������
            pNormalLine = New Line
            pNormalLine.FromPoint = pPaperPoint
            pNormalLine.ToPoint = pPointNormal

            '��ý���
            pConstructPoint = GetIntersectionSeg(pLine, pNormalLine, True)

            '�õ�����С����
            pShortLine = New Polyline
            pShortLine.FromPoint = pConstructPoint
            pShortLine.ToPoint = pPointNormal

            '��ELEMENT
            pLineElement = New LineElement
            pLineElement.Symbol = pSimpleSymbol
            pElement = pLineElement
            pElement.Geometry = pShortLine
            pGrapContainer.AddElement(pElement, 0)
        Next i
    End Sub

    '���ƽ������
    Public Sub GetPanleByCoordinate(ByVal dblMapx As Double, ByVal dblMapY As Double, ByVal pProjectReference As IProjectedCoordinateSystem, ByVal dblX As Double, ByVal dblY As Double)
        Dim pPoint As WKSPoint
        If pProjectReference Is Nothing Then
            dblX = dblMapx
            dblY = dblMapY
        Else
            pPoint.x = dblMapx
            pPoint.y = dblMapY

            pProjectReference.Forward(1, pPoint)

            dblX = pPoint.x
            dblY = pPoint.y
        End If

    End Sub
    'д�����ı���
    Private Function DrawGridText(ByVal pPoint As IPoint, ByVal strText As String, ByVal dblTextSize As Double, ByVal h_alignment As esriTextHorizontalAlignment, ByVal v_alignment As esriTextVerticalAlignment) As ITextElement
        Dim pRGBColor As IRgbColor
        Dim pTextElement As ITextElement
        Dim pTextSymbol As ITextSymbol
        'Dim fnt As stdole.IFontDisp
        Dim pElement As IElement
        pRGBColor = New RgbColor
        pRGBColor.Blue = 0
        pRGBColor.Red = 0
        pRGBColor.Green = 0
        pTextElement = New TextElement
        pElement = pTextElement
        pElement.Geometry = pPoint
        Dim pFontDisp As stdole.IFontDisp
        pFontDisp = New stdole.StdFont
        If type_ZT = 0 Then
            pFontDisp.Name = "����"
        ElseIf type_ZT = 1 Then
            pFontDisp.Name = "����"     'yjl20110915 add
        End If

        pFontDisp.Bold = True
        'pFontDisp.Underline = True
        pTextSymbol = New TextSymbol
        pTextSymbol.Font = pFontDisp
        pTextSymbol.Color = pRGBColor
        pTextSymbol.Size = dblTextSize
        pTextSymbol.HorizontalAlignment = h_alignment
        pTextSymbol.VerticalAlignment = v_alignment
        pTextElement.Symbol = pTextSymbol
        pTextElement.Text = strText
        DrawGridText = pTextElement
    End Function


    '��MAP�ϵ�GEOMETRYת��ͼֽ��������
    Private Function GetPaperXY(ByVal pMapPoint1 As IPoint, ByVal pMapPoint2 As IPoint, _
        ByVal pPaperPoint1 As IPoint, ByVal pPaperPoint2 As IPoint, _
        ByVal dblX As Double, ByVal dblY As Double) As Double

        If dblY = 0 Then
            GetPaperXY = (pPaperPoint1.x - pPaperPoint2.x) * (dblX - pMapPoint1.X) / (pMapPoint1.X - pMapPoint2.X) + pPaperPoint1.x
        ElseIf dblX = 0 Then
            GetPaperXY = (pPaperPoint1.y - pPaperPoint2.y) * (dblY - pMapPoint1.Y) / (pMapPoint1.Y - pMapPoint2.Y) + pPaperPoint1.y
        Else
            Exit Function
        End If
    End Function

    '�õ�Ҫ�صĽ���
    Private Function GetIntersectionSeg(ByVal pSegment1 As ISegment, ByVal pSegment2 As ISegment, ByVal blnEx As Boolean) As IPoint
        Dim pTcoll As IPointCollection
        Dim pConstructMultipoint As IConstructMultipoint
        Dim i As Integer

        pConstructMultipoint = New Multipoint
        If blnEx = False Then
            pConstructMultipoint.ConstructIntersection(pSegment1, esriSegmentExtension.esriExtendEmbedded, pSegment2, esriSegmentExtension.esriNoExtension)
        Else
            pConstructMultipoint.ConstructIntersection(pSegment1, esriSegmentExtension.esriExtendEmbedded, pSegment2, esriSegmentExtension.esriExtendEmbedded)
        End If
        pTcoll = pConstructMultipoint
        For i = 0 To pTcoll.PointCount - 1
            GetIntersectionSeg = pTcoll.Point(i)
            Exit Function
        Next i
        GetIntersectionSeg = Nothing
    End Function
    '�õ�Ҫ�صĽ���
    Private Function GetIntersection(ByVal pPolyline1 As IPolyline, ByVal pPolyline2 As IPolyline) As IPoint
        Dim pTcoll As IPointCollection
        Dim pConstructMultipoint As IConstructMultipoint
        Dim pSegment1 As ISegment
        Dim pSegment2 As ISegment
        Dim pSegmentCol As ISegmentCollection
        GetIntersection = Nothing
        pSegmentCol = New Polyline

        pSegmentCol = pPolyline1
        pSegment1 = pSegmentCol.Segment(0)

        pSegmentCol = Nothing
        pSegmentCol = pPolyline2
        pSegment2 = pSegmentCol.Segment(0)

        Dim i As Integer
        pConstructMultipoint = New Multipoint
        pConstructMultipoint.ConstructIntersection(pSegment1, esriSegmentExtension.esriExtendEmbedded, pSegment2, esriSegmentExtension.esriExtendEmbedded)
        pTcoll = pConstructMultipoint
        For i = 0 To pTcoll.PointCount - 1
            GetIntersection = pTcoll.Point(i)
            Exit For
        Next i
        Return GetIntersection
    End Function

    Private Sub CopyToPageLayout(ByVal pGeometry As IGeometry, ByVal vPagelayout As IPageLayoutControl, ByVal lngScale As Long)
        '��pagelayerout��ͼ��
        Dim pMap As IMap

        '�õ� IActiveView �ӿ�
        Dim pActiveView As IActiveView
        Dim pSymbolBorder As ISymbolBorder
        Dim pSimpleLineSymbol As ISimpleLineSymbol

        pMap = vPagelayout.ActiveView.FocusMap

        pActiveView = pMap
        pActiveView.Extent = pGeometry.Envelope

        pMap.MapScale = lngScale


        pSimpleLineSymbol = New SimpleLineSymbol
        pSimpleLineSymbol.Width = 0
        pSymbolBorder = New SymbolBorder
        pSymbolBorder.LineSymbol = pSimpleLineSymbol
        pMap.ClipGeometry = pGeometry
        pMap.ClipBorder = pSymbolBorder


    End Sub

    '�ƶ�ͼ���ܱߵ�һЩ��ʾxinxi
    Private Sub MovePageLyrOutlbl(ByVal pElement As IElement, ByVal dblX As Double, ByVal dblY As Double)
        'Dim pEnvelope As IEnvelope
        'Dim pPoint As IPoint
        'If pElement.Geometry.GeometryType = esriGeometryType.esriGeometryPoint Then
        '    pPoint = pElement.Geometry
        '    pPoint.X = pPoint.X + dblX
        '    pPoint.Y = pPoint.Y + dblY

        '    pElement.Geometry = pPoint
        'Else
        '    pEnvelope = pElement.Geometry.Envelope
        '    pEnvelope.Offset(dblX, dblY)
        '    pElement.Geometry = pEnvelope
        'End If
        Dim pTran As ITransform2D
        pTran = pElement.Geometry
        pTran.Move(dblX, dblY)
        pElement.Geometry = pTran

    End Sub
End Class
