Public Class OdbiorForm
    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim sequel As New RapidConnection()
        sequel.SetCommand("select IdKlienta,CONCAT(Kod,'  ',Nazwa),PaletToMin from praktyka.tblKlienci")
        sequel.RunDataQuery()
        For i As Integer = 0 To sequel.SQLDT.Rows.Count - 1
            KlienciID.Add(sequel.SQLDT.Rows.Item(i).Item(0)) 'list id index = index of klient in combobox
            ComboBox1.Items.Add(sequel.SQLDT.Rows.Item(i).Item(1))
            KlienciPrzelicznik.Add(sequel.SQLDT.Rows.Item(i).Item(2))
        Next

        'get rampy
        sequel.SetCommand("select * from praktyka.tblRampySosnowiec")
        sequel.RunDataQuery()
        For i As Integer = 0 To sequel.SQLDT.Rows.Count - 1
            SOSRampy.Add(sequel.SQLDT.Rows.Item(i).Item(1))
        Next

        sequel.SetCommand("select * from praktyka.tblRampyKatowice")
        sequel.RunDataQuery()
        For i As Integer = 0 To sequel.SQLDT.Rows.Count - 1
            KatRampy.Add(sequel.SQLDT.Rows.Item(i).Item(1))
        Next
        'set rampy
        ComboBox3.SelectedItem = KolejkowanieZaładunku.locationButton.Text
        ComboBox3_SelectedIndexChanged()

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
            If ComboBox1.SelectedIndex = -1 Then 'Client is not selected, assumed conversion 1 to 1
                TextBox5.Text = ThisConversionHasAspergerFRFR
            Else
                TextBox5.Text = ThisConversionHasAspergerFRFR * KlienciPrzelicznik(ComboBox1.SelectedIndex)
            End If
        Else
            TextBox5.Text = "" ' Clear TextBox5 or set to some default value if TextBox4's content isn't a number
        End If
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim sequel As New RapidConnection()
        Dim cmd As String
        Dim createdId As Integer
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
        'Check if Zlecenie is not overlapping with already existing Zlecenia
        'previous
        cmd = "select top 1 tblZlecenia.Godzina, tblZlecenia.Czas, tblZlecenia.ZlecenieId from praktyka.tblZlecenia where tblZlecenia.Data='" & DateTimePicker1.Value.Date & "' and lokalizacjaID=" & ComboBox3.SelectedIndex & " and rampaId=" & ComboBox4.SelectedIndex & " and Godzina < '" & MaskedTextBox1.Text & ":00' order by Godzina desc"
        sequel.SetCommand(cmd)
        sequel.RunDataQuery()
        Dim startMinuteCurrent = CInt(MaskedTextBox1.Text.Split(":"c)(0)) * 60 + CInt(MaskedTextBox1.Text.Split(":"c)(1))
        If sequel.SQLDT.Rows.Count <> 0 Then 'zlecenie exists -> checking previous zlecenie from current day
            Dim endMinutePrevious = sequel.SQLDT.Rows.Item(0).Item(0).TotalMinutes + sequel.SQLDT.Rows.Item(0).Item(1)
            If endMinutePrevious > startMinuteCurrent Then
                MessageBox.Show("Podana godzina nachodzi na poprzednie zlecenie!")
                Exit Sub
            End If
        Else 'checking last zlecenie from previous day
            cmd = "select top 1 tblZlecenia.Godzina, tblZlecenia.Czas, tblZlecenia.ZlecenieId from praktyka.tblZlecenia where tblZlecenia.Data='" & DateTimePicker1.Value.Date.AddDays(-1) & "' and lokalizacjaID=" & ComboBox3.SelectedIndex & " and rampaId=" & ComboBox4.SelectedIndex & " AND (DATEPART(HOUR, Godzina) * 60 + DATEPART(MINUTE, Godzina) + Czas) > 1440 order by Godzina "
            Console.WriteLine(cmd)
            sequel.SetCommand(cmd)
            sequel.RunDataQuery()
            If sequel.SQLDT.Rows.Count <> 0 Then 'there was zlecenie yesterday that ran past midnight
                Dim endMinutePrevious = sequel.SQLDT.Rows.Item(0)(1) - (TimeSpan.FromDays(1) - sequel.SQLDT.Rows.Item(0)(0)).TotalMinutes 'minuty = minuty - czas pomiędzy rozpoczęciem a północą
                If endMinutePrevious > startMinuteCurrent Then
                    MessageBox.Show("Podana godzina nachodzi na wczorajsze zlecenie!")
                    Exit Sub
                End If
            End If
        End If
        'next
        cmd = "select top 1 tblZlecenia.Godzina, tblZlecenia.Czas, tblZlecenia.ZlecenieId from praktyka.tblZlecenia where tblZlecenia.Data='" & DateTimePicker1.Value.Date & "' and lokalizacjaID=" & ComboBox3.SelectedIndex & " and rampaId=" & ComboBox4.SelectedIndex & "  and Godzina >'" & MaskedTextBox1.Text & ":00' order by Godzina"
        sequel.SetCommand(cmd)
        sequel.RunDataQuery()
        Dim endMinuteCurrent = CInt(MaskedTextBox1.Text.Split(":"c)(0)) * 60 + CInt(MaskedTextBox1.Text.Split(":"c)(1)) + CInt(TextBox5.Text)
        If sequel.SQLDT.Rows.Count <> 0 Then 'zlecenie exists -> checking next zlecenie from current day
            Dim startMinuteNext = sequel.SQLDT.Rows.Item(0).Item(0).TotalMinutes
            If endMinuteCurrent > startMinuteNext Then
                MessageBox.Show("Podana godzina nachodzi na następne zlecenie!")
                Exit Sub
            End If
        Else 'checking first zlecenie from next day
            cmd = "select top 1 tblZlecenia.Godzina, tblZlecenia.Czas, tblZlecenia.ZlecenieId from praktyka.tblZlecenia where tblZlecenia.Data='" & DateTimePicker1.Value.AddDays(1).Date & "' and lokalizacjaID=" & ComboBox3.SelectedIndex & " and rampaId=" & ComboBox4.SelectedIndex & " order by Godzina desc"
            sequel.SetCommand(cmd)
            sequel.RunDataQuery()
            If endMinuteCurrent > 1440 AndAlso sequel.SQLDT.Rows.Count <> 0 Then 'current zlecenie will run past midnight and there is zlecenie tommorow
                endMinuteCurrent -= 1440 'remaining minutes
                Dim startMinuteNext = sequel.SQLDT.Rows.Item(0).Item(0).TotalMinutes
                If endMinuteCurrent > startMinuteNext Then
                    MessageBox.Show("Podana godzina nachodzi na jutrzejsze zlecenie!")
                    Exit Sub
                End If
            End If
        End If



        'input zlecenie
        cmd = $"insert into praktyka.tblZlecenia(KlientId,PrzewoznikId,LokalizacjaId,RampaId,NrRejestracyjny,NrTransportu,NrWz,Data,Godzina,Palety,Czas,Uwagi)    OUTPUT INSERTED.ZlecenieId    values (" & KlienciID(ComboBox1.SelectedIndex) & "," & PrzewoznicyID(ComboBox2.SelectedIndex) & "," & ComboBox3.SelectedIndex & "," & ComboBox4.SelectedIndex & ",'" & TextBox1.Text & "','" & TextBox2.Text & "','" & TextBox3.Text & "','" & DateTimePicker1.Value.Date & "','" & MaskedTextBox1.Text & ":00'," & TextBox4.Text & "," & TextBox5.Text & ",'" & TextBox6.Text & "')"
        sequel.SetCommand(cmd)
        sequel.RunDataQuery()

        If sequel.QueryResult = True Then
            'create odbiór for zlecenie
            createdId = sequel.SQLDT.Rows.Item(0).Item(0)
            cmd = $"insert into praktyka.tblOdbiory(IdZlecenia,Utworzenie,UtworzeniePrac,UtworzenieCZAS)  values (" & createdId & ", 1 ," & KolejkowanieZaładunku.UserId & ",'" & Now() & "')"
            sequel.SetCommand(cmd)
            sequel.RunQueryNoData()
            If sequel.QueryResult = True Then
                MessageBox.Show("Dodano Zlecenie!")
                Me.Close()
            End If
        End If
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Close()
    End Sub
    Private Sub ComboBox3_SelectedIndexChanged() Handles ComboBox3.SelectedIndexChanged
        If ComboBox3.SelectedIndex = 0 Then
            ' Clear existing items from the ComboBox
            ComboBox4.Items.Clear()

            ' Add all items from SOSRampy to the ComboBox
            For Each item As String In SOSRampy
                ComboBox4.Items.Add(item)
            Next
        End If
        If ComboBox3.SelectedIndex = 1 Then
            ' Clear existing items from the ComboBox
            ComboBox4.Items.Clear()

            ' Add all items from SOSRampy to the ComboBox
            For Each item As String In KATRampy
                ComboBox4.Items.Add(item)
            Next
        End If
        ComboBox4.Text = String.Empty ' Clears the text
    End Sub

    Private KlienciID As New List(Of Integer) ' list holding id's with indexes equivalent to indexes in ComboBox1
    Private KlienciPrzelicznik As New List(Of Integer) ' list holding PaletyToMin with indexes equivalent to indexes in ComboBox1
    Private PrzewoznicyID As New List(Of Integer) ' list holding id's with indexes equivalent to indexes in ComboBox2
    Private SOSRampy As New List(Of String)
    Private KATRampy As New List(Of String)

End Class