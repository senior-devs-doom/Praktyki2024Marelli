Public Class OdbiorForm
    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim sequel As New RapidConnection()
        sequel.SetCommand("select IdKlienta,CONCAT(Kod,'  ',Nazwa) from praktyka.tblKlienci")
        sequel.RunDataQuery()
        For i As Integer = 0 To sequel.SQLDT.Rows.Count - 1
            KlienciID.Add(sequel.SQLDT.Rows.Item(i).Item(0)) 'list id index = index of klient in combobox
            ComboBox1.Items.Add(sequel.SQLDT.Rows.Item(i).Item(1))
        Next

        ComboBox3.SelectedItem = KolejkowanieZaładunku.locationButton.Text
        ComboBox4.Items.Add(KolejkowanieZaładunku.rampaLabel1.Text)
        ComboBox4.Items.Add(KolejkowanieZaładunku.rampaLabel2.Text)
        ComboBox4.Items.Add(KolejkowanieZaładunku.rampaLabel3.Text)
        ComboBox4.Items.Add(KolejkowanieZaładunku.rampaLabel4.Text)

        If KolejkowanieZaładunku.OdbiorRamp <> "" Then
            ComboBox4.SelectedItem = KolejkowanieZaładunku.OdbiorRamp
        End If
        If KolejkowanieZaładunku.OdbiorCzas <> "" Then
            MaskedTextBox1.Text = KolejkowanieZaładunku.OdbiorCzas
        End If
        If KolejkowanieZaładunku.OdbiorData <> "" Then
            DateTimePicker1.Text = KolejkowanieZaładunku.OdbiorData
        End If

        sequel.SetCommand("select IdPrzewoznika,CONCAT(Kod,'  ',Nazwa) from praktyka.tblPrzewoznicy where Aktywny=1")
        sequel.RunDataQuery()
        For i As Integer = 0 To sequel.SQLDT.Rows.Count - 1
            PrzewoznicyID.Add(sequel.SQLDT.Rows.Item(i).Item(0)) 'list id index = index of klient in combobox
            ComboBox2.Items.Add(sequel.SQLDT.Rows.Item(i).Item(1))
        Next
        DateTimePicker1.Format = DateTimePickerFormat.Custom
        DateTimePicker1.CustomFormat = "yyyy-MM-dd"


    End Sub
    ' Use KeyDown to restrict input to numeric values.
    Private Sub TextBox4_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox4.KeyPress
        If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub
    Private Sub TextBox5_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox5.KeyPress
        If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    ' Use TextChanged to update TextBox5 after TextBox4's value changes.
    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged
        Dim ThisConversionHasAspergerFRFR As Integer = 0
        If Integer.TryParse(TextBox4.Text, ThisConversionHasAspergerFRFR) Then
            TextBox5.Text = PaletyToMin(ThisConversionHasAspergerFRFR).ToString()
        Else
            TextBox5.Text = "" ' Clear TextBox5 or set to some default value if TextBox4's content isn't a number
        End If
    End Sub
    Private Function PaletyToMin(palety As Int32) As Int32 'This function could approximate task time based on palet number. But I need data to create a mathematical function so it's simple now. 
        Return palety
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim sequel As New RapidConnection()
        Dim cmd As String
        ' Check if ComboBox1 has a selected item
        If ComboBox1.SelectedIndex = -1 OrElse ComboBox1.Text = String.Empty Then
            MessageBox.Show("Proszę wybraćKlienta!") ' Customize the message as needed
            Exit Sub
        End If

        ' Check if ComboBox2 has a selected item
        If ComboBox2.SelectedIndex = -1 OrElse ComboBox2.Text = String.Empty Then
            MessageBox.Show("Proszę wybrać Przewoźnika!") ' Customize the message as needed
            Exit Sub
        End If

        ' Check if MaskedTextBox1 is filled appropriately (assuming a specific format/input is required)
        If Not MaskedTextBox1.MaskCompleted Then ' Or use String.IsNullOrWhiteSpace(MaskedTextBox1.Text) for a simple check
            MessageBox.Show("Proszę wybrać godzinę!")
            Exit Sub
        End If

        ' Check if TextBox4 is empty
        If TextBox4.Text = "" Then
            MessageBox.Show("Palety nie mogą być puste!")
            Exit Sub
        End If

        ' Check if TextBox5 is empty
        If TextBox5.Text = "" Then
            MessageBox.Show("Czas nie może być pusty!")
            Exit Sub
        End If
        cmd = $"insert into praktyka.tblZlecenia(KlientId,PrzewoznikId,LokalizacjaId,RampaId,NrRejestracyjny,NrTransportu,NrWz,Data,Godzina,Palety,Czas,Uwagi) values (" & KlienciID(ComboBox1.SelectedIndex) & "," & PrzewoznicyID(ComboBox2.SelectedIndex) & "," & ComboBox3.SelectedIndex & "," & ComboBox4.SelectedIndex & ",'" & TextBox1.Text & "','" & TextBox2.Text & "','" & TextBox3.Text & "','" & DateTimePicker1.Value.Date & "','" & MaskedTextBox1.Text & ":00'," & TextBox4.Text & "," & TextBox5.Text & ",'" & TextBox6.Text & "')"
        sequel.SetCommand(cmd)
        sequel.RunQueryNoData()
        If sequel.QueryResult = True Then
            MessageBox.Show("Dodano Zlecenie!")
            Me.Close()
        End If
    End Sub

    Private KlienciID As New List(Of Integer)
    Private PrzewoznicyID As New List(Of Integer)

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Close()
    End Sub

End Class