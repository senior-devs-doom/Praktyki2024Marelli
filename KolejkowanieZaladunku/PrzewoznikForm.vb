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

        sequel.SetCommand("INSERT INTO praktyka.tblPrzewoznicy(Nazwa, Kod, Aktywny) VALUES (@Nazwa, @Kod, @Aktywny)")
        sequel.AddParam("@Nazwa", TextBox1.Text)
        sequel.AddParam("@Kod", TextBox2.Text)
        sequel.AddParam("@Aktywny", bit)
        sequel.RunQueryNoData()
        If sequel.QueryResult = True Then
            MessageBox.Show("Dodano Przewoznika!")
            Me.Close()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim sequel As New RapidConnection()
        sequel.SetCommand("DELETE FROM praktyka.tblPrzewoznicy WHERE (nazwa = @Nazwa AND nazwa <> '') OR (kod = @Kod AND kod <> '')")
        sequel.AddParam("@Nazwa", TextBox1.Text)
        sequel.AddParam("@Kod", TextBox2.Text)
        sequel.RunQueryNoData()
        If sequel.QueryResult = True Then
            MessageBox.Show("Usunięto Przewoznika!")
            Me.Close()
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim sequel As New RapidConnection()
        If TextBox1.Text = "" Then
            MessageBox.Show("Nazwa Przewoźnika nie może być pusta!")
            Exit Sub
        End If
        sequel.SetCommand("UPDATE praktyka.tblPrzewoznicy SET Kod=@Kod, Aktywny=@Aktywny where Nazwa=@Nazwa")
        sequel.AddParam("@Nazwa", TextBox1.Text)
        sequel.AddParam("@Kod", TextBox2.Text)
        sequel.AddParam("@Aktywny", If(CheckBox1.Checked, 1, 0))
        sequel.RunQueryNoData()
        If sequel.QueryResult = True Then
            MessageBox.Show("zaktualizowano Przewoźnika!")
            Me.Close()
        End If
    End Sub
End Class