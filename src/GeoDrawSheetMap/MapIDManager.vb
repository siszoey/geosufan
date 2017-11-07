Imports ESRI.ArcGIS.Geometry
'''<summary>
''' ����ͼ�Ž���������ͼ����ת�����ж�
'''ͼ������:��׼�ַ���ͼ����.500����84-80-��-��-��,2k����84-80-��
'''ͼ��ID:��ͼ�����ƶ�Ӧ�������ֹ��ڳ���������ķ��Ŵ�,��84-80-��-��-���Ӧ��ͼ��ID��84-80-2-2-2
'''</summary>

Public Class MapIDManager
    'CMapStringToString m_NametoID
    Private m_strConst As String
    Private m_IDtoName(4) As String

    Public Sub New()
        m_strConst = "�����"
        m_IDtoName(0) = "��"
        m_IDtoName(1) = "��"
        m_IDtoName(2) = "��"
        m_IDtoName(3) = "��"
    End Sub
    '��Ҫ���� : ����ͼ���Ż�ȡ��ͼ������
    '������� : strMapID��ͼ�����ַ�������84-80-2-2-2��84-80-2��
    Public Function GetScaleByMapID(ByVal strMapID As String) As Integer
        Dim iScale As Integer
        Dim iLength As Integer

        iScale = 0
        iLength = strMapID.Length
        Select Case iLength
            Case 10
                If MapID500IsCorrect(strMapID) = True Then
                    iScale = 500
                End If
            Case 8
                If MapID10KIsCorrect(strMapID) = True Then
                    iScale = 1000
                End If
            Case 4
                If MapID2KIsCorrect(strMapID) = True Then
                    iScale = 2000
                End If
                'Case 9
                '    If strMapID.IndexOf("-") > -1 Then
                '        If MapID2KIsCorrect(strMapID) = True Then
                '            iScale = 2000
                '        ElseIf MapID1KIsCorrect(strMapID) = True Then
                '            iScale = 1000
                '        End If
                '    End If


        End Select

        GetScaleByMapID = iScale
    End Function
    '����ͼ�����ƻ�ȡ������
    Public Function GetScaleByMapName(ByVal strMapName As String) As Integer
        Dim iScale As Integer
        Dim iLength As Integer

        iScale = 0
        iLength = strMapName.Length
        Select Case iLength
            Case 14, 16
                If MapName500IsCorrect(strMapName) = True Then
                    iScale = 500
                End If
            Case 8, 10
                If MapName2KIsCorrect(strMapName) = True Then
                    iScale = 2000
                End If
            Case 9
                If MapName10KIsCorrect(strMapName) = True Then
                    iScale = 1000
                End If
        End Select

        GetScaleByMapName = iScale
    End Function
    '��Ҫ����:   ����ͼ���Ż�ȡͼ������,������84-80-2-2-2�򷵻�84-80-��-��-��,1�������ֱ�ӷ�������ͼ����
    Public Function GetMapNameString(ByVal strMapID As String) As String
        Dim iScale As Integer
        Dim strMapName As String

        iScale = GetScaleByMapID(strMapID)
        strMapName = ""
        Select Case iScale
            Case 500
                strMapName = Get500MapName(strMapID)
            Case 2000
                strMapName = Get2KMapName(strMapID)
            Case 10000
                strMapName = Get10KMapName(strMapID)
        End Select

        GetMapNameString = strMapName
    End Function
    Public Function GetMapID(ByVal strMapName As String) As String
        Dim iScale As Integer
        Dim strMapID As String

        iScale = GetScaleByMapName(strMapName)
        strMapID = ""
        Select Case iScale
            Case 500
                strMapID = Get500MapIDByName(strMapName)
            Case 2000
                strMapID = Get2KMapIDByName(strMapName)
            Case 10000
                strMapID = Get10KMapIDByName(strMapName)
        End Select

        GetMapID = strMapID
    End Function
    '��ȡ500ͼ�������ڵ�2000ͼ���ŵ�����
    Public Function Get2KMapIDBy500ID(ByVal str500MapID As String) As String
        If GetScaleByMapID(str500MapID) <> 500 Then
            Get2KMapIDBy500ID = ""
        Else
            Get2KMapIDBy500ID = str500MapID.Substring(0, str500MapID.Length - 4)
        End If
    End Function
    '��ȡ2000ͼ���Ű�����500ͼ��������
    Public Function Get500MapIDArray(ByVal str2KMapID As String, ByRef strArray500ID() As String) As Boolean
        '�����СҪ����16
        If UBound(strArray500ID) <> 16 Then
            Get500MapIDArray = False
            Exit Function
        End If

        '��ͼͼ����Ҫ��������
        If GetScaleByMapID(str2KMapID) <> 2000 Then
            Get500MapIDArray = False
            Exit Function
        End If

        '��ֵ
        Dim i As Integer
        Dim j As Integer

        For i = 1 To 4
            For j = 1 To 4
                Dim iIndex As Integer

                iIndex = (i - 1) * 4 + (j - 1)
                strArray500ID(iIndex) = String.Format("%s-%d-%d", str2KMapID, i, j)
            Next
        Next

        Get500MapIDArray = True
    End Function
    Private Sub ComputerSubSheetOrigin(ByRef XOrigin As Integer, ByRef YOrigin As Integer, ByVal iSubSheet As Integer, ByVal iScale As Integer)
        If iScale <> 2000 And iScale <> 1000 And iScale <> 500 Then
            Throw New Exception("δ֧�ֵı�����.")
        End If

        If iSubSheet < 1 Or iSubSheet > 4 Then
            Throw New Exception("δ֧�ֵ���ͼ�����.")
        End If

        Dim iSheetWidth, iSheetHeight As Integer
        Select Case iScale
            Case 500
                iSheetWidth = 250
                iSheetHeight = 250
            Case 1000
                iSheetWidth = 500
                iSheetHeight = 500
            Case 2000
                iSheetWidth = 1000
                iSheetHeight = 1000
        End Select

        Select Case iSubSheet
            Case 1
                YOrigin = YOrigin + iSheetHeight
            Case 2
                XOrigin = XOrigin + iSheetWidth
                YOrigin = YOrigin + iSheetHeight
            Case 4
                XOrigin = XOrigin + iSheetWidth
        End Select

    End Sub
    '����ͼ��ID(��86-67-1-1-3,106-084-1-1-2��)��ȡ����������(500,2000)��ͼ����Χ
    Public Function GetGeometryByBigMapSheet(ByVal strSheetNo As String, Optional ByVal ipSpatial As ISpatialReference = Nothing) As IGeometry
        Dim iScale = GetScaleByMapID(strSheetNo)
        If (iScale <> 500 And iScale <> 1000 And iScale <> 2000 And iScale <> 10000) Then
            GetGeometryByBigMapSheet = Nothing
            Exit Function
        End If

        Dim iFirst, iLast As Integer
        Dim XOrigin, YOrigin As Integer

        iFirst = 0
        iLast = strSheetNo.IndexOf("-")

        '''''''''''''''''''''''''''''''''''''''''yuki'''''''''''''''''''''''''''''''''''''''''''
        'x��y���귴���Ѹ�
        YOrigin = Integer.Parse(strSheetNo.Substring(iFirst, iLast)) * 1000
        iFirst = iLast + 1
        iLast = strSheetNo.IndexOf("-", iFirst)
        XOrigin = Integer.Parse(strSheetNo.Substring(iFirst, iLast - iFirst)) * 1000
        iFirst = iLast + 1

        Dim iSubSheet As Integer = Integer.Parse(strSheetNo.Substring(iFirst, 1))
        Try
            ComputerSubSheetOrigin(XOrigin, YOrigin, iSubSheet, 2000)
            If (iScale = 500) Or (iScale = 1000) Then
                iLast = strSheetNo.IndexOf("-", iFirst)
                iFirst = iLast + 1
                iSubSheet = Integer.Parse(strSheetNo.Substring(iFirst, 1))
                ComputerSubSheetOrigin(XOrigin, YOrigin, iSubSheet, 1000)
                If (iScale = 500) Then

                    ''''''''''''''''''''''''''''''''''yuki'''''''''''''''''''''''''''''''''''
                    'iLastû�����¼��㣬�����������һ�����
                    iLast = strSheetNo.IndexOf("-", iFirst)
                    ''''''''''''''''''''''''''''''''''yuki'''''''''''''''''''''''''''''''''''

                    iFirst = iLast + 1
                    iSubSheet = Integer.Parse(strSheetNo.Substring(iFirst, 1))
                    ComputerSubSheetOrigin(XOrigin, YOrigin, iSubSheet, 500)
                End If
                'Else

            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            GetGeometryByBigMapSheet = Nothing
            Exit Function
        End Try

        Dim pPointOrigin As IPoint = New Point
        Dim pPointRight As IPoint = New Point
        Dim pPointRightUp As IPoint = New Point
        Dim pPointUp As IPoint = New Point
        Dim pPointCol As IPointCollection = New Polygon
        Dim dbXOrigin As Double = Convert.ToDouble(XOrigin)
        Dim dbYOrigin As Double = Convert.ToDouble(YOrigin)
        Dim dbSheetWidth As Double
        Dim dbSheetHeight As Double

        If iScale = 500 Then
            dbSheetWidth = 250.0
            dbSheetHeight = 250.0
        ElseIf iScale = 1000 Then
            dbSheetWidth = 500.0
            dbSheetHeight = 500.0
        Else
            dbSheetWidth = 1000.0
            dbSheetHeight = 1000.0
        End If

        pPointOrigin.PutCoords(dbXOrigin, dbYOrigin)
        pPointRight.PutCoords(dbXOrigin + dbSheetWidth, dbYOrigin)
        pPointRightUp.PutCoords(dbXOrigin + dbSheetWidth, dbYOrigin + dbSheetHeight)
        pPointUp.PutCoords(dbXOrigin, dbYOrigin + dbSheetHeight)
        'pPointOrigin.PutCoords(dbYOrigin, dbXOrigin)
        'pPointRight.PutCoords(dbYOrigin, dbXOrigin + dbSheetWidth)
        'pPointRightUp.PutCoords(dbYOrigin + dbSheetHeight, dbXOrigin + dbSheetWidth)
        'pPointUp.PutCoords(dbYOrigin + dbSheetHeight, dbXOrigin)

        'pPointCol.AddPoint(pPointOrigin)
        'pPointCol.AddPoint(pPointRight)
        'pPointCol.AddPoint(pPointRightUp)
        'pPointCol.AddPoint(pPointUp)
        'pPointCol.AddPoint(pPointOrigin)

        pPointCol.AddPoint(pPointOrigin)
        pPointCol.AddPoint(pPointUp)
        pPointCol.AddPoint(pPointRightUp)
        pPointCol.AddPoint(pPointRight)
        pPointCol.AddPoint(pPointOrigin)


        Dim pPolygon As IPolygon = pPointCol ' CType(pPointCol, IPolygon)
        If ipSpatial IsNot Nothing Then
            pPolygon.SpatialReference = ipSpatial
        End If

        Dim pTopo As ITopologicalOperator
        pTopo = pPolygon
        pTopo.Simplify()

        GetGeometryByBigMapSheet = CType(pPolygon, IGeometry)
    End Function
    Public Function GetGeometryByBigMapSheetFor500(ByVal strSheetNo As String, Optional ByVal ipSpatial As ISpatialReference = Nothing) As IGeometry
        Dim iScale = GetScaleByMapID(strSheetNo)
        If (iScale <> 500 And iScale <> 1000 And iScale <> 2000 And iScale <> 10000) Then
            Return Nothing
            Exit Function
        End If

        Dim iFirst, iLast As Integer
        Dim XOrigin, YOrigin As Integer

        iFirst = 0
        iLast = strSheetNo.IndexOf("-")

        '''''''''''''''''''''''''''''''''''''''''yuki'''''''''''''''''''''''''''''''''''''''''''
        'x��y���귴���Ѹ�
        '
        XOrigin = Integer.Parse(strSheetNo.Substring(iFirst, iLast)) * 1000
        iFirst = iLast + 1
        iLast = strSheetNo.IndexOf("-", iFirst)

        YOrigin = Integer.Parse(strSheetNo.Substring(iFirst, iLast - iFirst)) * 1000
        iFirst = iLast + 1

        Dim iSubSheet As Integer = Integer.Parse(strSheetNo.Substring(iFirst, 1))
        Try
            ComputerSubSheetOrigin(XOrigin, YOrigin, iSubSheet, 2000)
            If (iScale = 500) Or (iScale = 1000) Then
                iLast = strSheetNo.IndexOf("-", iFirst)
                iFirst = iLast + 1
                iSubSheet = Integer.Parse(strSheetNo.Substring(iFirst, 1))
                ComputerSubSheetOrigin(XOrigin, YOrigin, iSubSheet, 1000)
                If (iScale = 500) Then

                    ''''''''''''''''''''''''''''''''''yuki'''''''''''''''''''''''''''''''''''
                    'iLastû�����¼��㣬�����������һ�����
                    iLast = strSheetNo.IndexOf("-", iFirst)
                    ''''''''''''''''''''''''''''''''''yuki'''''''''''''''''''''''''''''''''''

                    iFirst = iLast + 1
                    iSubSheet = Integer.Parse(strSheetNo.Substring(iFirst, 1))
                    ComputerSubSheetOrigin(XOrigin, YOrigin, iSubSheet, 500)
                End If
                'Else

            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            Return Nothing
            Exit Function
        End Try

        Dim pPointOrigin As IPoint = New Point
        Dim pPointRight As IPoint = New Point
        Dim pPointRightUp As IPoint = New Point
        Dim pPointUp As IPoint = New Point
        Dim pPointCol As IPointCollection = New Polygon
        Dim dbXOrigin As Double = Convert.ToDouble(XOrigin)
        Dim dbYOrigin As Double = Convert.ToDouble(YOrigin)
        Dim dbSheetWidth As Double
        Dim dbSheetHeight As Double

        If iScale = 500 Then
            dbSheetWidth = 250.0
            dbSheetHeight = 250.0
        ElseIf iScale = 1000 Then
            dbSheetWidth = 500.0
            dbSheetHeight = 500.0
        Else
            dbSheetWidth = 1000.0
            dbSheetHeight = 1000.0
        End If

        pPointOrigin.PutCoords(dbXOrigin, dbYOrigin)
        pPointRight.PutCoords(dbXOrigin + dbSheetWidth, dbYOrigin)
        pPointRightUp.PutCoords(dbXOrigin + dbSheetWidth, dbYOrigin + dbSheetHeight)
        pPointUp.PutCoords(dbXOrigin, dbYOrigin + dbSheetHeight)
        'pPointOrigin.PutCoords(dbYOrigin, dbXOrigin)
        'pPointRight.PutCoords(dbYOrigin, dbXOrigin + dbSheetWidth)
        'pPointRightUp.PutCoords(dbYOrigin + dbSheetHeight, dbXOrigin + dbSheetWidth)
        'pPointUp.PutCoords(dbYOrigin + dbSheetHeight, dbXOrigin)

        'pPointCol.AddPoint(pPointOrigin)
        'pPointCol.AddPoint(pPointRight)
        'pPointCol.AddPoint(pPointRightUp)
        'pPointCol.AddPoint(pPointUp)
        'pPointCol.AddPoint(pPointOrigin)

        pPointCol.AddPoint(pPointOrigin)
        pPointCol.AddPoint(pPointUp)
        pPointCol.AddPoint(pPointRightUp)
        pPointCol.AddPoint(pPointRight)
        pPointCol.AddPoint(pPointOrigin)


        Dim pPolygon As IPolygon = pPointCol ' CType(pPointCol, IPolygon)
        If ipSpatial IsNot Nothing Then
            pPolygon.SpatialReference = ipSpatial
        End If

        Dim pTopo As ITopologicalOperator
        pTopo = pPolygon
        pTopo.Simplify()

        Return CType(pPolygon, IGeometry)
    End Function
    Public Function GetPolygonByBigMapSheet(ByVal strSheetNo As String, Optional ByVal ipSpatial As ISpatialReference = Nothing) As IPolygon
        Dim iScale = GetScaleByMapID(strSheetNo)
        If (iScale <> 500 And iScale <> 1000 And iScale <> 2000) Then
            Return Nothing
            Exit Function
        End If

        Dim iFirst, iLast As Integer
        Dim XOrigin, YOrigin As Integer

        iFirst = 0
        iLast = strSheetNo.IndexOf("-")

        '''''''''''''''''''''''''''''''''''''''''yuki'''''''''''''''''''''''''''''''''''''''''''
        'x��y���귴���Ѹ�
        YOrigin = Integer.Parse(strSheetNo.Substring(iFirst, iLast)) * 1000
        iFirst = iLast + 1
        iLast = strSheetNo.IndexOf("-", iFirst)
        XOrigin = Integer.Parse(strSheetNo.Substring(iFirst, iLast - iFirst)) * 1000
        iFirst = iLast + 1

        Dim iSubSheet As Integer = Integer.Parse(strSheetNo.Substring(iFirst, 1))
        Try
            ComputerSubSheetOrigin(XOrigin, YOrigin, iSubSheet, 2000)
            If (iScale = 500) Or (iScale = 1000) Then
                iLast = strSheetNo.IndexOf("-", iFirst)
                iFirst = iLast + 1
                iSubSheet = Integer.Parse(strSheetNo.Substring(iFirst, 1))
                ComputerSubSheetOrigin(XOrigin, YOrigin, iSubSheet, 1000)
                If (iScale = 500) Then

                    ''''''''''''''''''''''''''''''''''yuki'''''''''''''''''''''''''''''''''''
                    'iLastû�����¼��㣬�����������һ�����
                    iLast = strSheetNo.IndexOf("-", iFirst)
                    ''''''''''''''''''''''''''''''''''yuki'''''''''''''''''''''''''''''''''''

                    iFirst = iLast + 1
                    iSubSheet = Integer.Parse(strSheetNo.Substring(iFirst, 1))
                    ComputerSubSheetOrigin(XOrigin, YOrigin, iSubSheet, 500)
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            Return Nothing
            Exit Function
        End Try

        Dim pPointOrigin As IPoint = New Point
        Dim pPointRight As IPoint = New Point
        Dim pPointRightUp As IPoint = New Point
        Dim pPointUp As IPoint = New Point
        Dim pPointCol As IPointCollection = New Polygon
        Dim dbXOrigin As Double = Convert.ToDouble(XOrigin)
        Dim dbYOrigin As Double = Convert.ToDouble(YOrigin)
        Dim dbSheetWidth As Double
        Dim dbSheetHeight As Double

        If iScale = 500 Then
            dbSheetWidth = 250.0
            dbSheetHeight = 250.0
        ElseIf iScale = 1000 Then
            dbSheetWidth = 500.0
            dbSheetHeight = 500.0
        Else
            dbSheetWidth = 1000.0
            dbSheetHeight = 1000.0
        End If

        pPointOrigin.PutCoords(dbXOrigin, dbYOrigin)
        pPointRight.PutCoords(dbXOrigin + dbSheetWidth, dbYOrigin)
        pPointRightUp.PutCoords(dbXOrigin + dbSheetWidth, dbYOrigin + dbSheetHeight)
        pPointUp.PutCoords(dbXOrigin, dbYOrigin + dbSheetHeight)
        'pPointOrigin.PutCoords(dbYOrigin, dbXOrigin)
        'pPointRight.PutCoords(dbYOrigin, dbXOrigin + dbSheetWidth)
        'pPointRightUp.PutCoords(dbYOrigin + dbSheetHeight, dbXOrigin + dbSheetWidth)
        'pPointUp.PutCoords(dbYOrigin + dbSheetHeight, dbXOrigin)

        'pPointCol.AddPoint(pPointOrigin)
        'pPointCol.AddPoint(pPointRight)
        'pPointCol.AddPoint(pPointRightUp)
        'pPointCol.AddPoint(pPointUp)
        'pPointCol.AddPoint(pPointOrigin)

        pPointCol.AddPoint(pPointOrigin)
        pPointCol.AddPoint(pPointUp)
        pPointCol.AddPoint(pPointRightUp)
        pPointCol.AddPoint(pPointRight)
        pPointCol.AddPoint(pPointOrigin)


        Dim pPolygon As IPolygon = pPointCol ' CType(pPointCol, IPolygon)
        If ipSpatial IsNot Nothing Then
            pPolygon.SpatialReference = ipSpatial
        End If

        Dim pTopo As ITopologicalOperator
        pTopo = pPolygon
        pTopo.Simplify()

        Return pPolygon ' CType(pPolygon, IGeometry)
    End Function
    '�ж�500ͼ�����Ƿ���ȷ����ʽ��78-84-1-2-3��106-064-1-2-3����ʱ���ж�107-065-1-2-3���Ʋ��淶������
    Public Function MapID500IsCorrect(ByVal strMapID500 As String) As Boolean
        Dim strMapID As String
        Dim myString As CustomString
        Dim No_1 As Integer
        Dim No_2 As Integer

        myString = New CustomString(strMapID500)
        strMapID = myString.SpanIncluding("0123456789-")

        '�ж��Ƿ������ֺͼ������-�����
        If strMapID.Length <> 11 And strMapID.Length <> 13 Then
            MapID500IsCorrect = False
            Exit Function
        End If

        '�жϼ������-���Ƿ�����Ӧ��λ��
        If strMapID.Length = 11 Then
            If strMapID(2) <> "-" Or strMapID(5) <> "-" Or strMapID(7) <> "-" Or strMapID(9) <> "-" Then
                MapID500IsCorrect = False
                Exit Function
            End If
        ElseIf strMapID.Length = 13 Then
            If strMapID(3) <> "-" Or strMapID(7) <> "-" Or strMapID(9) <> "-" Or strMapID(11) <> "-" Then
                MapID500IsCorrect = False
                Exit Function
            End If
        End If

        '�ж�����λ�����Ƿ�ȫ������
        strMapID = strMapID500.Replace("-", "")
        If strMapID.Length <> 7 And strMapID.Length <> 9 Then
            MapID500IsCorrect = False
            Exit Function
        End If

        ''''''''''''''''''''''''''''''''''''''''''yuki''''''''''''''''''''''''''''''''''
        '�ж�ǰ����λ�Ƿ�Ϊż��
        If strMapID.Length = 7 Then

            No_1 = CType(strMapID.Substring(0, 2), Integer)
            No_2 = CType(strMapID.Substring(2, 2), Integer)

            If No_1 Mod 2 <> 0 Or No_2 Mod 2 <> 0 Then
                MapID500IsCorrect = False
                Exit Function
            End If
        ElseIf strMapID.Length = 9 Then
            No_1 = CType(strMapID.Substring(0, 3), Integer)
            No_2 = CType(strMapID.Substring(3, 3), Integer)

            If No_1 Mod 2 <> 0 Or No_2 Mod 2 <> 0 Then
                MapID500IsCorrect = False
                Exit Function
            End If

            '�ж�078-064-1���Ʋ��淶������
            If No_1 < 100 And No_2 < 100 Then
                MapID500IsCorrect = False
                Exit Function
            End If
        End If
        ''''''''''''''''''''''''''''''''''''''''''yuki'''''''''''''''''''''''''''''''''

        '�ж����3λ�Ƿ�������1234����֮һ
        myString.TextString = strMapID.Substring(strMapID.Length - 3)
        strMapID = myString.SpanIncluding("1234")
        If (strMapID.Length <> 3) Then
            MapID500IsCorrect = False
        Else
            MapID500IsCorrect = True
        End If
    End Function
    '�ж�1000ͼ�����Ƿ���ȷ����ʽ��78-84-1-1��106-064-1-1����ʱ���ж�078-065-1-1���Ʋ��淶������
    Public Function MapID1KIsCorrect(ByVal strMapID1K As String) As Boolean
        Dim strMapID As String
        Dim myString As CustomString
        Dim No_1 As Integer
        Dim No_2 As Integer

        myString = New CustomString(strMapID1K)
        strMapID = myString.SpanIncluding("0123456789-")

        '�ж��Ƿ������ֺͼ������-�����
        If strMapID.Length <> 9 And strMapID.Length <> 11 Then
            MapID1KIsCorrect = False
            Exit Function
        End If

        '�жϼ������-���Ƿ�����Ӧ��λ��
        If strMapID.Length = 9 Then
            If strMapID(2) <> "-" Or strMapID(5) <> "-" Or strMapID(7) <> "-" Then
                MapID1KIsCorrect = False
                Exit Function
            End If
        ElseIf strMapID.Length = 11 Then
            If strMapID(3) <> "-" Or strMapID(7) <> "-" Or strMapID(9) <> "-" Then
                MapID1KIsCorrect = False
                Exit Function
            End If
        End If

        '�ж�����λ�����Ƿ�ȫ������
        strMapID = strMapID1K.Replace("-", "")
        If strMapID.Length <> 6 And strMapID.Length <> 8 Then
            MapID1KIsCorrect = False
            Exit Function
        End If

        ''''''''''''''''''''''''''''''''''''''''''yuki''''''''''''''''''''''''''''''''''
        '�ж�ǰ����λ�Ƿ�Ϊż��
        If strMapID.Length = 7 Then

            No_1 = CType(strMapID.Substring(0, 2), Integer)
            No_2 = CType(strMapID.Substring(2, 2), Integer)

            If No_1 Mod 2 <> 0 Or No_2 Mod 2 <> 0 Then
                MapID1KIsCorrect = False
                Exit Function
            End If
        ElseIf strMapID.Length = 9 Then
            No_1 = CType(strMapID.Substring(0, 3), Integer)
            No_2 = CType(strMapID.Substring(3, 3), Integer)

            If No_1 Mod 2 <> 0 Or No_2 Mod 2 <> 0 Then
                MapID1KIsCorrect = False
                Exit Function
            End If

            '�ж�078-064-1���Ʋ��淶������
            If No_1 < 100 And No_2 < 100 Then
                MapID1KIsCorrect = False
                Exit Function
            End If
        End If
        ''''''''''''''''''''''''''''''''''''''''''yuki'''''''''''''''''''''''''''''''''

        '�ж����һλ�Ƿ�������1234����֮һ
        myString.TextString = strMapID.Substring(strMapID.Length - 1)
        strMapID = myString.SpanIncluding("1234")
        If (strMapID.Length <> 1) Then
            MapID1KIsCorrect = False
        Else
            MapID1KIsCorrect = True
        End If

    End Function
    '�ж�2000ͼ�����Ƿ���ȷ����ʽ��78-84-1��106-064-1����ʱ���ж�078-065-1���Ʋ��淶������
    Public Function MapID2KIsCorrect(ByVal strMapID2K As String) As Boolean
        Dim strMapID As String
        Dim myString As CustomString
        Dim No_1 As Integer
        Dim No_2 As Integer

        myString = New CustomString(strMapID2K)
        strMapID = myString.SpanIncluding("0123456789-")

        '�ж��Ƿ������ֺͼ������-�����
        If strMapID.Length = 4 Then
            MapID2KIsCorrect = True
        Else
            MapID2KIsCorrect = False
            Exit Function
        End If

        ''�жϼ������-���Ƿ�����Ӧ��λ��
        'If strMapID.Length = 7 Then
        '    If strMapID(2) <> "-" Or strMapID(5) <> "-" Then
        '        MapID2KIsCorrect = False
        '        Exit Function
        '    End If
        'ElseIf strMapID.Length = 9 Then
        '    If strMapID(3) <> "-" Or strMapID(7) <> "-" Then
        '        MapID2KIsCorrect = False
        '        Exit Function
        '    End If
        'End If

        ''�ж�����λ�����Ƿ�ȫ������
        'strMapID = strMapID2K.Replace("-", "")
        'If strMapID.Length <> 5 And strMapID.Length <> 7 Then
        '    MapID2KIsCorrect = False
        '    Exit Function
        'End If

        ''''''''''''''''''''''''''''''''''''''''''yuki''''''''''''''''''''''''''''''''''
        '�ж�ǰ����λ�Ƿ�Ϊż��
        'If strMapID.Length = 7 Then

        '    No_1 = CType(strMapID.Substring(0, 2), Integer)
        '    No_2 = CType(strMapID.Substring(2, 2), Integer)

        '    If No_1 Mod 2 <> 0 Or No_2 Mod 2 <> 0 Then
        '        MapID2KIsCorrect = False
        '        Exit Function
        '    End If
        'ElseIf strMapID.Length = 9 Then
        '    No_1 = CType(strMapID.Substring(0, 3), Integer)
        '    No_2 = CType(strMapID.Substring(3, 3), Integer)

        '    If No_1 Mod 2 <> 0 Or No_2 Mod 2 <> 0 Then
        '        MapID2KIsCorrect = False
        '        Exit Function
        '    End If

        '    '�ж�078-064-1���Ʋ��淶������
        '    If No_1 < 100 And No_2 < 100 Then
        '        MapID2KIsCorrect = False
        '        Exit Function
        '    End If
        'End If
        ''''''''''''''''''''''''''''''''''''''''''yuki'''''''''''''''''''''''''''''''''

        '�ж����һλ�Ƿ�������1234����֮һ
        'myString.TextString = strMapID.Substring(strMapID.Length - 1)
        'strMapID = myString.SpanIncluding("1234")
        'If (strMapID.Length <> 1) Then
        '    MapID2KIsCorrect = False
        'Else
        '    MapID2KIsCorrect = True
        'End If

    End Function
    '�ж�1��ͼ�����Ƿ���ȷ:��ʽ��H48g001053��I48g096053
    Public Function MapID10KIsCorrect(ByVal strMapID10K As String) As Boolean
        Dim strMapID As String
        Dim myString As CustomString

        myString = New CustomString(strMapID10K)

        strMapID = myString.SpanIncluding("HIhig0123456789")
        If strMapID.Length <> 10 Then
            MapID10KIsCorrect = False
            Exit Function
        End If

        strMapID = strMapID10K(0)
        myString.TextString = strMapID
        strMapID = myString.SpanIncluding("HIhig")
        If strMapID.Length <> 1 Then
            MapID10KIsCorrect = False
        Else
            MapID10KIsCorrect = True
        End If
    End Function
    '���500��ͼ�������Ƿ���ȷ
    Private Function MapName500IsCorrect(ByVal strMapName500 As String) As Boolean
        Dim strMapID500 As String

        strMapID500 = Get500MapIDByName(strMapName500)
        MapName500IsCorrect = MapID500IsCorrect(strMapID500)
    End Function
    '���2K��ͼ�������Ƿ���ȷ
    Private Function MapName2KIsCorrect(ByVal strMapName2K As String) As Boolean
        Dim strMapID2K As String

        strMapID2K = Get10KMapIDByName(strMapName2K)
        MapName2KIsCorrect = MapID10KIsCorrect(strMapID2K)
    End Function
    '���10K��ͼ�������Ƿ���ȷ
    Private Function MapName10KIsCorrect(ByVal strMapName10K As String) As Boolean
        Dim strMapID10K As String

        strMapID10K = Get10KMapIDByName(strMapName10K)
        MapName10KIsCorrect = MapID10KIsCorrect(strMapID10K)
    End Function
    '����500��ͼ��ID��ȡͼ������
    Private Function Get500MapName(ByVal strMapID500 As String) As String
        Get500MapName = GetMapName(strMapID500)
    End Function
    '����2K��ͼ��ID��ȡͼ������
    Private Function Get2KMapName(ByVal strMapID2K As String) As String
        Get2KMapName = GetMapName(strMapID2K)
    End Function
    '����10K��ͼ��ID��ȡͼ������,Ŀǰ����Ҫת��
    Private Function Get10KMapName(ByVal strMapID10K As String) As String
        Get10KMapName = strMapID10K
    End Function
    '����500��ͼ�����ƻ�ȡͼ��ID
    Private Function Get500MapIDByName(ByVal strMapName500 As String) As String
        Get500MapIDByName = GetMapIDByName(strMapName500)
    End Function
    '����2K��ͼ�����ƻ�ȡͼ��ID
    Private Function Get2KMapIDByName(ByVal strMapName2K As String) As String
        Get2KMapIDByName = GetMapIDByName(strMapName2K)
    End Function
    '����10K��ͼ�����ƻ�ȡͼ��ID
    Private Function Get10KMapIDByName(ByVal strMapName10K As String) As String
        Get10KMapIDByName = strMapName10K
    End Function
    '����ͼ�����ƻ�ȡͼ��ID,����500��2Kͼ����,�滻��־ͼ��λ�õİ���������Ϊ"��"��ϣ������
    Private Function GetMapIDByName(ByVal strMapName As String) As String
        Dim strMapID As String
        strMapID = strMapName
        strMapID.Replace(m_IDtoName(0), "1")
        strMapID.Replace(m_IDtoName(1), "1")
        strMapID.Replace(m_IDtoName(2), "2")
        strMapID.Replace(m_IDtoName(3), "3")
        GetMapIDByName = strMapID
    End Function
    '����ͼ��ID��ȡͼ������,����500��2Kͼ����,�滻"��"��ϣ������Ϊ����������
    Private Function GetMapName(ByVal strMapID As String) As String
        Dim iIndex As Integer
        Dim strMapName As String
        Try
            strMapName = strMapID
            iIndex = strMapName.IndexOf("-")
            iIndex = strMapName.IndexOf("-", iIndex) '�ڶ��������
            If iIndex > 0 Then
                Dim strMapNameLeft As String
                Dim strMapNameRight As String

                strMapNameLeft = strMapName.Substring(0, iIndex)
                strMapNameRight = strMapName.Substring(iIndex)
                strMapNameRight.Replace("1", m_IDtoName(0))
                strMapNameRight.Replace("2", m_IDtoName(1))
                strMapNameRight.Replace("3", m_IDtoName(2))
                strMapNameRight.Replace("4", m_IDtoName(3))

                GetMapName = strMapNameLeft + strMapNameRight
            Else
                GetMapName = ""
            End If
        Catch ex As Exception
            Return ""
        End Try

    End Function
End Class
''' <summary>
''' �Զ����࣬Ϊ��ʵ���ַ���������VC��CString���ĳЩ�����Ĳ������ر�����SpanIncluding��SpanExcluding���� 
''' </summary>
''' <remarks></remarks>
Public Class CustomString
    Private m_strText As String

    Public Sub New()
        m_strText = ""
    End Sub

    Public Sub New(ByVal strText As String)
        m_strText = strText
    End Sub
    Public Property TextString() As String
        Get
            TextString = m_strText
        End Get
        Set(ByVal value As String)
            m_strText = value
        End Set
    End Property
    '��VC��CString���SpanIncluding����
    Public Function SpanIncluding(ByVal strText As String) As String
        Dim strIncluding As String

        If strText.Length = 0 Then
            strIncluding = strText
        ElseIf m_strText.Length = 0 Then
            strIncluding = ""
        Else
            Dim iIndex As Integer

            strIncluding = ""
            For iIndex = 0 To m_strText.Length - 1
                If strText.IndexOf(m_strText(iIndex)) > -1 Then
                    strIncluding += m_strText(iIndex)
                Else
                    Exit For
                End If
            Next
        End If

        SpanIncluding = strIncluding
    End Function
    '��VC��CString���SpanExcluding����
    Public Function SpanExcluding(ByVal strText As String) As String
        Dim strExcluding As String

        If strText.Length = 0 Then
            strExcluding = strText
        ElseIf m_strText.Length = 0 Then
            strExcluding = ""
        Else
            Dim iIndex As Integer

            strExcluding = ""
            For iIndex = 0 To m_strText.Length - 1
                If strText.IndexOf(m_strText(iIndex)) > -1 Then
                    Exit For
                Else
                    strExcluding += m_strText(iIndex)
                End If
            Next
        End If

        SpanExcluding = strExcluding
    End Function
    '��VC��CString���MakeVerse����,��String�ַ�������
    Public Function Verse() As String
        Dim strVerse As String

        If m_strText.Length = 0 Then
            strVerse = ""
        Else
            Dim iIndex As Integer

            strVerse = ""
            For iIndex = m_strText.Length - 1 To 0
                strVerse += m_strText(iIndex)
            Next
        End If

        Verse = strVerse
    End Function
End Class
