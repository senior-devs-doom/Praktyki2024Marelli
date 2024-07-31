Public Class DodajKlienta
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim sequel As New RapidConnection()
        If TextBox1.Text = "" Then
            MessageBox.Show("Nazwa Klienta nie może być pusta!")
            Exit Sub
        End If
        If TextBox2.Text = "" Then
            MessageBox.Show("Kod Klienta nie może być pusty!")
            Exit Sub
        End If
        sequel.SetCommand($"insert into praktyka.tblKlienci(Nazwa,Kod) values ('" & TextBox1.Text & "'," & TextBox2.Text & ")")
        sequel.RunQueryNoData()
        If sequel.QueryResult = True Then
            MessageBox.Show("Dodano Klienta!")
            Me.Close()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim sequel As New RapidConnection()
        sequel.SetCommand($"DELETE From praktyka.tblKlienci Where (nazwa = '" & TextBox1.Text & "' AND nazwa <> '')OR (kod = '" & TextBox2.Text & "' AND kod <> '') ")
        sequel.RunQueryNoData()
        If sequel.QueryResult = True Then
            MessageBox.Show("Usunięto Klienta!")
            Me.Close()
        End If
    End Sub
End Class