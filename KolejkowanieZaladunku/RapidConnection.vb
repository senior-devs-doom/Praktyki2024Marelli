Imports System.Data.SqlClient

Public Class RapidConnection
    Implements IDisposable

    Private ReadOnly SQLCon As New SqlConnection With {.ConnectionString = "Server=10.129.206.48;Initial Catalog=Rapid_Test;Connection Timeout=30;INTEGRATED SECURITY=SSPI"}
    Private SQLCmd As SqlCommand
    Private SQLDA As SqlDataAdapter
    Public SQLDT As DataTable
    Public QueryResult As Boolean = False
    Private disposedValue As Boolean

    Public Sub New()
        Try
            SQLCon.Open()
        Catch ex As Exception
            If SQLCon.State = ConnectionState.Open Then SQLCon.Close()
            MsgBox(ex.Message) ' & vbNewLine & "Query:" & Query)
        End Try
    End Sub

    Public Sub SetCommand(Qry As String)
        Try
            SQLCmd = New SqlCommand
            With SQLCmd
                .Connection = SQLCon
                .CommandText = Qry
            End With
        Catch ex As Exception
            If SQLCon.State = ConnectionState.Open Then SQLCon.Close()
            MsgBox("Error creating SQL command", vbOKOnly + vbCritical, "HFM reporting")
        End Try
    End Sub

    Public Sub RunDataQuery()
        Try
            If SQLCon.State = ConnectionState.Closed Then
                SQLCon.Open()
            End If
            'FILL DATATABLE
            SQLDA = New SqlDataAdapter(SQLCmd)
            SQLDT = New DataTable
            SQLDA.Fill(SQLDT)

            QueryResult = True
        Catch ex As Exception
            If SQLCon.State = ConnectionState.Open Then SQLCon.Close()
            MsgBox(ex.Message) ' & vbNewLine & "Query:" & Query)            
        End Try

    End Sub


    ''' <summary>
    ''' Runs previously set query without filling data to datatable.  
    ''' </summary>
    ''' <param name="ExecuteType">Options 'Nq' (default) for Insert / Update / Delete queries, 'Sc' for Select query </param>
    ''' <returns>Number of rows affected by Insert / Update / Delete query for 'Nq' exceution type or 
    ''' <br>Value of first row and first column in Select query for 'Sc' execution type</br>     
    ''' </returns>
    Public Function RunQueryNoData(Optional ExecuteType As String = "Nq") As Integer
        Dim RetVal As Integer = -1
        Try
            If SQLCon.State = ConnectionState.Closed Then
                SQLCon.Open()
            End If
            If ExecuteType = "Nq" Then
                RetVal = SQLCmd.ExecuteNonQuery()
            Else
                RetVal = SQLCmd.ExecuteScalar
            End If

            QueryResult = True
        Catch ex As Exception
            If SQLCon.State = ConnectionState.Open Then SQLCon.Close()
            MsgBox(ex.Message) ' & vbNewLine & "Query:" & Query)         
        End Try

        Return RetVal
    End Function

    Public Sub AddParam(Name As String, Optional DataType As DbType = DbType.String)
        Dim newParam As New SqlParameter With {.ParameterName = Name, .DbType = DataType}
        SQLCmd.Parameters.Add(newParam)
    End Sub

    Public Sub AddParam(Name As String, Value As Object, Optional DataType As DbType = DbType.String)
        Dim newParam As New SqlParameter With {.ParameterName = Name, .Value = Value, .DbType = DataType}
        SQLCmd.Parameters.Add(newParam)
    End Sub

    Public Sub SetParamValue(Name As String, Value As Object)
        SQLCmd.Parameters(Name).Value = Value
    End Sub

    Public Sub ResetCommand()
        SQLCmd = New SqlCommand
        QueryResult = False
        SQLDT = New DataTable
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: Wyczyścić stan zarządzany (obiekty zarządzane)
                SQLCon.Dispose()
                SQLCmd.Dispose()
                SQLDA.Dispose()
                SQLDT.Dispose()
            End If

            ' TODO: Zwolnić niezarządzane zasoby (niezarządzane obiekty) i przesłonić finalizator
            ' TODO: Ustawić wartość null dla dużych pól
            disposedValue = True
        End If
    End Sub

    ' ' TODO: Przesłonić finalizator tylko w sytuacji, gdy powyższa metoda „Dispose(disposing As Boolean)” zawiera kod służący do zwalniania niezarządzanych zasobów
    ' Protected Overrides Sub Finalize()
    '     ' Nie zmieniaj tego kodu. Umieść kod czyszczący w metodzie „Dispose(disposing As Boolean)”.
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Nie zmieniaj tego kodu. Umieść kod czyszczący w metodzie „Dispose(disposing As Boolean)”.
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class