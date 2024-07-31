Public Class SzablonForm
    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        ' rampy
        ListBox1.Items.Add(KolejkowanieZaładunku.rampaLabel1.Text)
        ListBox1.Items.Add(KolejkowanieZaładunku.rampaLabel2.Text)
        ListBox1.Items.Add(KolejkowanieZaładunku.rampaLabel3.Text)
        ListBox1.Items.Add(KolejkowanieZaładunku.rampaLabel4.Text)
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim sequel As New RapidConnection()
        Dim command As String
        If TextBox1.Text = "" Then
            MessageBox.Show("Nazwa szablonu nie może być pusta!")
            Exit Sub
        End If
        If ListBox1.SelectedIndex = -1 Then
            MessageBox.Show("Wybierz rampę!")
            Exit Sub
        End If
        If ListBox2.SelectedIndex = -1 Then
            MessageBox.Show("Wybierz dzień tygodnia!")
            Exit Sub
        End If
        If MaskedTextBox1.Text = "" Then
            MessageBox.Show("Wybierz godzinę!")
            Exit Sub
        End If
        command = $"insert into praktyka.tblSzablony(nazwaSzablonu,RampaId,DzienTygodnia,Godzina) values ('" & TextBox1.Text & "(szablon)'," & ListBox1.SelectedIndex + 1 & "," & ListBox2.SelectedIndex + 1 & ",'" & MaskedTextBox1.Text & "')"
        Console.WriteLine(command)
        sequel.SetCommand(command)
        sequel.RunQueryNoData()
        If sequel.QueryResult = True Then
            MessageBox.Show("Dodano Szablon!")
            Me.Close()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim sequel As New RapidConnection()
        sequel.SetCommand($"DELETE From praktyka.tblSzablony Where nazwa = '" & TextBox1.Text & "' ")
        sequel.RunQueryNoData()
        If sequel.QueryResult = True Then
            MessageBox.Show("Usunięto Szablon!")
            Me.Close()
        End If
    End Sub
End Class