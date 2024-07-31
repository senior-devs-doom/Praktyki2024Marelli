Public Class PrzewoznikForm
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim sequel As New RapidConnection()
        Dim bit As New Int16
        If TextBox1.Text = "" Then
            MessageBox.Show("Nazwa Przewoznika nie może być pusta!")
            Exit Sub
        End If
        If TextBox2.Text = "" Then
            MessageBox.Show("Kod Przewoznika nie może być pusty!")
            Exit Sub
        End If
        If CheckBox1.Checked Then
            bit = 1 'sql bit type accepts only 1 or 0
        Else
            bit = 0
        End If

        sequel.SetCommand($"insert into praktyka.tblPrzewoznicy(Nazwa,Kod,Aktywny) values ('" & TextBox1.Text & "'," & TextBox2.Text & "," & bit & ")")
        sequel.RunQueryNoData()
        If sequel.QueryResult = True Then
            MessageBox.Show("Dodano Przewoznika!")
            Me.Close()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim sequel As New RapidConnection()
        sequel.SetCommand($"DELETE From praktyka.tblPrzewoznicy Where (nazwa = '" & TextBox1.Text & "' AND nazwa <> '')OR (kod = '" & TextBox2.Text & "' AND kod <> '') ")
        sequel.RunQueryNoData()
        If sequel.QueryResult = True Then
            MessageBox.Show("Usunięto Przewoznika!")
            Me.Close()
        End If
    End Sub
End Class