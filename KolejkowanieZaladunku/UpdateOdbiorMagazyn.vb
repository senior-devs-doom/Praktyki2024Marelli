Public Class UpdateOdbiorMagazyn
    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim sequel As New RapidConnection()
        Dim data As System.Data.DataRow

        'load klienci
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

        'load przewoźnik
        sequel.SetCommand("select IdPrzewoznika,CONCAT(Kod,'  ',Nazwa) from praktyka.tblPrzewoznicy where Aktywny=1")
        sequel.RunDataQuery()
        For i As Integer = 0 To sequel.SQLDT.Rows.Count - 1
            PrzewoznicyID.Add(sequel.SQLDT.Rows.Item(i).Item(0)) 'list id index = index of klient in combobox
            ComboBox2.Items.Add(sequel.SQLDT.Rows.Item(i).Item(1))
        Next
        DateTimePicker1.Format = DateTimePickerFormat.Custom
        DateTimePicker1.CustomFormat = "yyyy-MM-dd"

        ''''''''''''load values in zlecenie
        sequel.SetCommand("select * from praktyka.tblZlecenia where ZlecenieId=" & KolejkowanieZaładunku.id)
        sequel.RunDataQuery()
        zlecenieInDataBase = sequel.SQLDT
        data = sequel.SQLDT.Rows.Item(0) 'KlienciID PrzewoznicyID
        ComboBox1.SelectedIndex = KlienciID.FindIndex(Function(x) x = data(1)) 'looks for clientId in client list and passes it to combobox
        ComboBox2.SelectedIndex = PrzewoznicyID.FindIndex(Function(x) x = data(2)) 'looks for przaewoznicid in przewoznik list and passes it to combobox
        ComboBox3.SelectedIndex = data(3)
        ComboBox4.SelectedIndex = data(4)
        TextBox1.Text = data(5)
        TextBox2.Text = data(6)
        TextBox3.Text = data(7)
        DateTimePicker1.Text = data(8).ToString()
        MaskedTextBox1.Text = data(9).ToString()
        TextBox4.Text = data(10)
        TextBox5.Text = data(11)
        TextBox6.Text = data(12)

        ''''''''''''''''''load flagi
        sequel.SetCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'praktyka' AND TABLE_NAME = 'tblOdbiory' AND DATA_TYPE = 'bit';")
        sequel.RunDataQuery()
        For i As Integer = 1 To sequel.SQLDT.Rows.Count - 1
            ' Create a new instance of CheckBox

            Dim checkBox As New CheckBox()

            ' Set properties of the checkbox
            checkBox.Text = sequel.SQLDT.Rows.Item(i).Item(0)
            Flagi.Add(sequel.SQLDT.Rows.Item(i).Item(0))
            checkBox.AutoSize = True ' Optional: To auto-size the checkbox according to its text
            checkBox.Dock = DockStyle.Fill

            'add event that enables box only if previous box is checked
            'i LOGIC IS BIT SCUFFED HERE SINCE I STARTED ITERATING FROM 1
            If i > 1 Then
                checkBox.Enabled = False ' Initially disable all checkboxes except the first one
                Dim previousCheckBox As CheckBox = CType(FlagPanel.Controls(i - 2), CheckBox)

                ' Add CheckedChanged event handler for the previous checkbox that enables/disables the current one
                AddHandler previousCheckBox.CheckedChanged, Sub(lambdaSender, LambdaEvent)
                                                                ' Ensure control exists and avoid cross-thread operations
                                                                If Not checkBox.IsDisposed AndAlso checkBox.InvokeRequired Then
                                                                    checkBox.Invoke(Sub() checkBox.Enabled = previousCheckBox.Checked)
                                                                Else
                                                                    checkBox.Enabled = previousCheckBox.Checked
                                                                End If
                                                            End Sub
            End If

            ' Add the checkbox to the FlagPanel
            FlagPanel.Controls.Add(checkBox)
        Next
        ''''''''load values into flagi
        Dim FlagNamesString = String.Join(",", Flagi)
        Dim FalseString As String = String.Join(",", Enumerable.Repeat("0", Flagi.Count))
        sequel.SetCommand("SELECT " & FlagNamesString & " FROM praktyka.tblOdbiory  WHERE [IdZlecenia]=" & KolejkowanieZaładunku.id)
        sequel.RunDataQuery()
        'parse odbiór into form
        For i As Integer = 0 To sequel.SQLDT.Rows.Item(0).ItemArray.Length - 1
            If sequel.SQLDT.Rows.Item(0).Item(i) Is DBNull.Value Then
                DirectCast(FlagPanel.Controls(i), CheckBox).Checked = False
                FlagiValue.Add(False)
            Else
                DirectCast(FlagPanel.Controls(i), CheckBox).Checked = sequel.SQLDT.Rows.Item(0).Item(i)
                FlagiValue.Add(sequel.SQLDT.Rows.Item(0).Item(i))
            End If
        Next
        'We now should have a list of database Flags and corresponding values
        'For i As Integer = 0 To Flagi.Count - 1
        '    Console.WriteLine("Flag: " & Flagi(i) & ", Value: " & FlagiValue(i))
        'Next

        'if magazyn loaded form we assume they know about change from ligostyka and flag dissapears
        sequel.SetCommand("update praktyka.tblZlecenia set Zmieniono = 0 WHERE [ZlecenieId] = " & KolejkowanieZaładunku.id)
        sequel.RunDataQuery()
    End Sub
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
        Dim UPDTZlecenie = True 'if no changes are detected in zlecenie dont update it (change to false)
        Dim UPDTZlecenieResult = True ' if we do run zlecenie update save if it suceeded

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

        'detecting changes, creating input and column strings
        cmd = "UPDATE praktyka.tblZlecenia SET"
        If ComboBox1.SelectedIndex <> KlienciID.FindIndex(Function(x) x = zlecenieInDataBase.Rows.Item(0)(1)) Then
            cmd += " " & zlecenieInDataBase.Columns(1).ColumnName & " = " & KlienciID(ComboBox1.SelectedIndex) & ","
        End If

        If ComboBox2.SelectedIndex <> PrzewoznicyID.FindIndex(Function(x) x = zlecenieInDataBase.Rows.Item(0)(2)) Then
            cmd += " " & zlecenieInDataBase.Columns(2).ColumnName & " = " & PrzewoznicyID(ComboBox2.SelectedIndex) & ","
        End If

        If ComboBox3.SelectedIndex <> zlecenieInDataBase.Rows.Item(0)(3) Then
            cmd += " " & zlecenieInDataBase.Columns(3).ColumnName & " = " & ComboBox3.SelectedIndex & ","
        End If

        If ComboBox4.SelectedIndex <> zlecenieInDataBase.Rows.Item(0)(4) Then
            cmd += " " & zlecenieInDataBase.Columns(4).ColumnName & " = " & ComboBox4.SelectedIndex & ","
        End If

        If TextBox1.Text <> zlecenieInDataBase.Rows.Item(0)(5).ToString() Then
            cmd += " " & zlecenieInDataBase.Columns(5).ColumnName & " = '" & TextBox1.Text & "',"
        End If

        If TextBox2.Text <> zlecenieInDataBase.Rows.Item(0)(6).ToString() Then
            cmd += " " & zlecenieInDataBase.Columns(6).ColumnName & " = '" & TextBox2.Text & "',"
        End If

        If TextBox3.Text <> zlecenieInDataBase.Rows.Item(0)(7).ToString() Then
            cmd += " " & zlecenieInDataBase.Columns(7).ColumnName & " = '" & TextBox3.Text & "',"
        End If

        If DateTimePicker1.Value.ToString("yyyyMMdd") <> Convert.ToDateTime(zlecenieInDataBase.Rows.Item(0)(8)).ToString("yyyyMMdd") Then
            cmd += " " & zlecenieInDataBase.Columns(8).ColumnName & " = '" & DateTimePicker1.Value.ToString("yyyy-MM-dd") & "',"
        End If

        If MaskedTextBox1.Text & ":00" <> zlecenieInDataBase.Rows.Item(0)(9).ToString() Then
            cmd += " " & zlecenieInDataBase.Columns(9).ColumnName & " = '" & MaskedTextBox1.Text & ":00',"
        End If

        If TextBox4.Text <> zlecenieInDataBase.Rows.Item(0)(10).ToString() Then
            cmd += " " & zlecenieInDataBase.Columns(10).ColumnName & " = '" & TextBox4.Text & "',"
        End If

        If TextBox5.Text <> zlecenieInDataBase.Rows.Item(0)(11).ToString() Then
            cmd += " " & zlecenieInDataBase.Columns(11).ColumnName & " = '" & TextBox5.Text & "',"
        End If

        If TextBox6.Text <> zlecenieInDataBase.Rows.Item(0)(12).ToString() Then
            cmd += " " & zlecenieInDataBase.Columns(12).ColumnName & " = '" & TextBox6.Text & "',"
        End If

        If cmd.EndsWith(",") Then
            cmd = cmd.TrimEnd(New Char() {","c})
        Else
            UPDTZlecenie = False
        End If

        cmd += " WHERE ZlecenieId = " & KolejkowanieZaładunku.id

        If UPDTZlecenie Then
            sequel.SetCommand(cmd)
            sequel.RunQueryNoData()
            sequel.QueryResult = UPDTZlecenieResult
        End If

        'Update Odbiory
        'system rejestruje kto i kiedy zmienił daną flage

        'making 2 extra lists cause it's the most striaghtforward and clear solution.
        Dim Czas As String = Now.ToString("HH:mm:ss")
        Dim FlagiChanged As New List(Of String)
        Dim FlagiValueChanged As New List(Of Boolean)
        For i As Integer = 0 To Flagi.Count - 1
            If DirectCast(FlagPanel.Controls(i), CheckBox).Checked <> FlagiValue(i) Then ' value changed
                FlagiChanged.Add(Flagi(i))
                FlagiValueChanged.Add(DirectCast(FlagPanel.Controls(i), CheckBox).Checked)
            End If
        Next
        'creating query
        Dim stringBuilder As New System.Text.StringBuilder()
        For i As Integer = 0 To FlagiChanged.Count - 1
            If i > 0 Then
                stringBuilder.Append(", ") ' Add a separator between items
            End If
            stringBuilder.AppendFormat("{0} = {1}, {0}PRAC = {2}, {0}CZAS = '{3}'", FlagiChanged(i), If(FlagiValueChanged(i), 1, 0), KolejkowanieZaładunku.UserId, Czas) 'True/False need converting to 1/0
        Next
        If stringBuilder.ToString <> "" Then
            cmd = "UPDATE praktyka.tblOdbiory SET " & stringBuilder.ToString() & " where IdZlecenia=" & KolejkowanieZaładunku.id
            sequel.SetCommand(cmd)
            sequel.RunQueryNoData()
            'For i As Integer = 0 To FlagiChanged.Count - 1
            '    Console.WriteLine("Flag: " & FlagiChanged(i) & ", Value: " & FlagiValueChanged(i))
            'Next
        End If

        'if no changes detected
        If stringBuilder.ToString = "" AndAlso Not UPDTZlecenie Then
            MessageBox.Show("Nie wykryto zmiany żadnego pola!")
            Exit Sub
        End If

        'if good then we good
        If sequel.QueryResult Then
            MessageBox.Show("Zaktualizowano Zlecenie!")
            If UPDTZlecenieResult = False Then
                MessageBox.Show("Wystąpił błąd przy aktualizacji pól z literami")
            End If
            Me.Close()
        End If

    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Close()
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If MessageBox.Show("Czy na pewno chcesz usunąć zlecenie?", "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
            ' User clicked "No", so break or exit the current subroutine/function
            Exit Sub
        Else
            ' User clicked "Yes", continue with the deletion
            Dim sequel As New RapidConnection()
            sequel.SetCommand("DELETE FROM praktyka.tblZlecenia WHERE ZlecenieId = " & KolejkowanieZaładunku.id)
            sequel.RunQueryNoData()
            If sequel.QueryResult Then
                sequel.SetCommand("DELETE FROM praktyka.tblOdbiory WHERE IdZlecenia = " & KolejkowanieZaładunku.id)
                sequel.RunQueryNoData()
                If sequel.QueryResult Then
                    MessageBox.Show("Usunięto Zlecenie!")
                    Me.Close()
                End If
            End If

        End If
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
            For Each item As String In KatRampy
                ComboBox4.Items.Add(item)
            Next
        End If
        ComboBox4.Text = String.Empty ' Clears the text
    End Sub

    Private KlienciID As New List(Of Integer)
    Private PrzewoznicyID As New List(Of Integer)
    Private KlienciPrzelicznik As New List(Of Integer) ' list holding PaletyToMin with indexes equivalent to indexes in ComboBox1
    Private zlecenieInDataBase As DataTable
    Private SOSRampy As New List(Of String)
    Private KatRampy As New List(Of String)
    'can't be asked to make this 2 dimensional list, list 1 holds checkbox names, list 2 holds their values. Spoilers, when we Update we make more lists.
    Private Flagi As New List(Of String)
    Private FlagiValue As New List(Of Boolean)

End Class