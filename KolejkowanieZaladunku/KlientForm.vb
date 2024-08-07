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
        If TextBox3.Text = "" Then
            MessageBox.Show("Proszę wypełnić pole które jest przelicznikiem średniego czasu potrzebnego na przygotowanie zlecenia w zależności od ilości zamówionych palet™, jest ono zależnie głównie od typu palet.")
            Exit Sub
        End If
        sequel.SetCommand("INSERT INTO praktyka.tblKlienci(Nazwa, Kod, PaletToMin) VALUES (@Nazwa, @Kod, @PaletToMin)")
        sequel.AddParam("@Nazwa", TextBox1.Text)
        sequel.AddParam("@Kod", TextBox2.Text)
        sequel.AddParam("@PaletToMin", TextBox3.Text)
        sequel.RunQueryNoData()
        If sequel.QueryResult = True Then
            MessageBox.Show("Dodano Klienta!")
            Me.Close()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim sequel As New RapidConnection()
        sequel.SetCommand("DELETE FROM praktyka.tblKlienci WHERE (nazwa = @Nazwa AND nazwa <> '') OR (kod = @Kod AND kod <> '')")
        sequel.AddParam("@Nazwa", TextBox1.Text)
        sequel.AddParam("@Kod", TextBox2.Text)
        sequel.RunQueryNoData()
        If sequel.QueryResult = True Then
            MessageBox.Show("Usunięto Klienta!")
            Me.Close()
        End If
    End Sub
    Private Sub TextBox3_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox3.KeyPress
        ' Check if the character is not a digit, a control character, or a period (.)
        If e.KeyChar = ","c Then
            e.KeyChar = "."c
        End If
        Console.WriteLine(e.KeyChar)
        If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) AndAlso e.KeyChar <> "."c Then
            ' Ignore the character by setting Handled to True
            e.Handled = True
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim sequel As New RapidConnection()
        If TextBox1.Text = "" Then
            MessageBox.Show("Nazwa Klienta nie może być pusta!")
            Exit Sub
        End If
        sequel.SetCommand("UPDATE praktyka.tblKlienci SET Kod=@Kod, PaletToMin=@PaletToMin where Nazwa=@Nazwa")
        sequel.AddParam("@Nazwa", TextBox1.Text)
        sequel.AddParam("@Kod", TextBox2.Text)
        sequel.AddParam("@PaletToMin", TextBox3.Text)
        sequel.RunQueryNoData()
        If sequel.QueryResult = True Then
            MessageBox.Show("zaktualizowano Klienta!")
            Me.Close()
        End If
    End Sub
End Class