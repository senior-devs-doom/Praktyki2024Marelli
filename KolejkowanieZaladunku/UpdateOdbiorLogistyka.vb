Public Class UpdateOdbiorLogistyka
    Private Property UserId As Integer
    Private Property Id As Integer

    Private KlienciID As New List(Of Integer)
    Private PrzewoznicyID As New List(Of Integer)
    Private KlienciPrzelicznik As New List(Of Integer) ' list holding PaletyToMin with indexes equivalent to indexes in ComboBox1
    Private zlecenieInDataBase As DataTable
    Private SOSRampy As New List(Of String)
    Private KatRampy As New List(Of String)

    Public Sub New(UserId As Integer, Id As Integer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _UserId = UserId
        _Id = Id


    End Sub
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
        '[KlientId] [PrzewoznikId] [LokalizacjaId] [RampaId] [NrRejestracyjny] [NrTransportu] [NrWz] [Data] [Godzina] [Palety] [Czas] [Uwagi] [Zmieniono] [Opozniony]
        sequel.SetCommand("SELECT * FROM praktyka.tblZlecenia WHERE ZlecenieId = @ZlecenieId")
        sequel.AddParam("@ZlecenieId", Id)
        sequel.RunDataQuery()
        zlecenieInDataBase = sequel.SQLDT
        data = sequel.SQLDT.Rows.Item(0)
        'ZlecenieID, klientID, Przewoznikid, LokalizacjaId, RampaId, NrRejestracyjny, NrTransportu, NrWz, Data, godzina, Palety, Czas, Uwagi, zmieniono, Opozniony
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

        'If hour before task started or less, cut off editing privilages
        If data(8).Add(data(9)).subtract(Now()).totalHours <= 1 Then
            ComboBox1.Enabled = False
            ComboBox2.Enabled = False
            ComboBox3.Enabled = False
            ComboBox4.Enabled = False
            'TextBox1 NR REJ
            TextBox2.Enabled = False
            TextBox3.Enabled = False
            DateTimePicker1.Enabled = False
            MaskedTextBox1.Enabled = False
            TextBox4.Enabled = False
            TextBox5.Enabled = False
            'TextBox6 UWAGI
            Button2.Enabled = False
        End If
        'additionally if zlecenie ended
        If data(8).Add(data(9)).AddMinutes(data(10)) <= Now() Then
            TextBox1.Enabled = False
            TextBox6.Enabled = False
        End If
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
        Dim ZmianyStr As String
        Dim ZmienionoUwagi = False 'since ZmianyStr always goes into uwagi we handle it a little diffrently
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

        ' Check if TextBox1 IS EMPTY
        If TextBox1.Text = "" Then
            MessageBox.Show("Proszę wprowadzić numer rejestracyjny!") ' Customize the message as needed
            Exit Sub
        End If

        ' Check if TextBox1 is too short
        If TextBox1.Text.Length < 5 Then
            MessageBox.Show("numer rejestracyjny jest za krótki.") ' Customize the message as needed
            Exit Sub
        End If

        If ComboBox3.SelectedIndex = -1 OrElse ComboBox3.Text = String.Empty Then
            MessageBox.Show("Proszę wybrać Lokalizację!") ' Customize the message as needed
            Exit Sub
        End If

        If ComboBox4.SelectedIndex = -1 OrElse ComboBox4.Text = String.Empty Then
            MessageBox.Show("Proszę wybrać Rampę!") ' Customize the message as needed
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

        'check if date has been changed to past
        Dim ts As TimeSpan
        TimeSpan.TryParse(MaskedTextBox1.Text, ts)
        If DateTimePicker1.Value.Date.Add(ts) < Now AndAlso DateTimePicker1.Value.ToString("yyyyMMdd") <> Convert.ToDateTime(zlecenieInDataBase.Rows.Item(0)(8)).ToString("yyyyMMdd") Then
            MessageBox.Show("Wybrana data/czas jest zbyt wczesna! Zlecenie nie może być przesunięte w przeszłość.")
            Exit Sub
        End If

        'maximum palety in transport specjalny = 15
        If Convert.ToInt32(TextBox4.Text) > 15 AndAlso ComboBox3.SelectedIndex = 0 AndAlso ComboBox4.SelectedIndex = 3 Then
            MessageBox.Show("Maksymalna liczba palet dla 'Sosnowiec Transport specjalny' to 15")
            Exit Sub
        End If

        'Check if Zlecenie is not overlapping with already existing Zlecenia
        'previous
        cmd = "SELECT TOP 1 tblZlecenia.Godzina, tblZlecenia.Czas, tblZlecenia.ZlecenieId " &
                           "FROM praktyka.tblZlecenia " &
                           "WHERE tblZlecenia.Data = @Data " &
                           "AND lokalizacjaID = @LokalizacjaID " &
                           "AND rampaId = @RampaId " &
                           "AND Godzina < @Godzina " &
                           "ORDER BY Godzina DESC"
        sequel.SetCommand(cmd)
        sequel.AddParam("@Data", DateTimePicker1.Value.Date)
        sequel.AddParam("@LokalizacjaID", ComboBox3.SelectedIndex)
        sequel.AddParam("@RampaId", ComboBox4.SelectedIndex)
        sequel.AddParam("@Godzina", MaskedTextBox1.Text & ":00")
        sequel.RunDataQuery()

        Dim startMinuteCurrent = CInt(MaskedTextBox1.Text.Split(":"c)(0)) * 60 + CInt(MaskedTextBox1.Text.Split(":"c)(1))
        If sequel.SQLDT.Rows.Count <> 0 Then 'zlecenie exists -> checking previous zlecenie from current day
            Dim endMinutePrevious = sequel.SQLDT.Rows.Item(0).Item(0).TotalMinutes + sequel.SQLDT.Rows.Item(0).Item(1)
            If endMinutePrevious > startMinuteCurrent Then
                MessageBox.Show("Podana godzina nachodzi na poprzednie zlecenie!")
                Exit Sub
            End If
        Else 'checking last zlecenie from previous day
            Dim sqlCommand As String = "SELECT TOP 1 tblZlecenia.Godzina, tblZlecenia.Czas, tblZlecenia.ZlecenieId " &
                           "FROM praktyka.tblZlecenia " &
                           "WHERE tblZlecenia.Data = @Data " &
                           "AND lokalizacjaID = @LokalizacjaID " &
                           "AND rampaId = @RampaId " &
                           "AND (DATEPART(HOUR, Godzina) * 60 + DATEPART(MINUTE, Godzina) + Czas) > 1440 " &
                           "ORDER BY Godzina"
            sequel.SetCommand(sqlCommand)
            sequel.AddParam("@Data", DateTimePicker1.Value.Date.AddDays(-1))
            sequel.AddParam("@LokalizacjaID", ComboBox3.SelectedIndex)
            sequel.AddParam("@RampaId", ComboBox4.SelectedIndex)
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
        cmd = "SELECT TOP 1 tblZlecenia.Godzina, tblZlecenia.Czas, tblZlecenia.ZlecenieId " &
                           "FROM praktyka.tblZlecenia " &
                           "WHERE tblZlecenia.Data = @Data " &
                           "AND lokalizacjaID = @LokalizacjaID " &
                           "AND rampaId = @RampaId " &
                           "AND Godzina > @Godzina " &
                           "ORDER BY Godzina"

        sequel.SetCommand(cmd)
        sequel.AddParam("@Data", DateTimePicker1.Value.Date)
        sequel.AddParam("@LokalizacjaID", ComboBox3.SelectedIndex)
        sequel.AddParam("@RampaId", ComboBox4.SelectedIndex)
        sequel.AddParam("@Godzina", MaskedTextBox1.Text & ":00")
        sequel.RunDataQuery()
        Dim endMinuteCurrent = CInt(MaskedTextBox1.Text.Split(":"c)(0)) * 60 + CInt(MaskedTextBox1.Text.Split(":"c)(1)) + CInt(TextBox5.Text)
        If sequel.SQLDT.Rows.Count <> 0 Then 'zlecenie exists -> checking next zlecenie from current day
            Dim startMinuteNext = sequel.SQLDT.Rows.Item(0).Item(0).TotalMinutes
            If endMinuteCurrent > startMinuteNext Then
                MessageBox.Show("Podana godzina nachodzi na następne zlecenie!")
                Exit Sub
            End If
        Else 'checking first zlecenie from next day
            cmd = "SELECT TOP 1 tblZlecenia.Godzina, tblZlecenia.Czas, tblZlecenia.ZlecenieId " &
                           "FROM praktyka.tblZlecenia " &
                           "WHERE tblZlecenia.Data = @Data " &
                           "AND lokalizacjaID = @LokalizacjaID " &
                           "AND rampaId = @RampaId " &
                           "ORDER BY Godzina DESC"

            sequel.SetCommand(cmd)
            sequel.AddParam("@Data", DateTimePicker1.Value.AddDays(1).Date)
            sequel.AddParam("@LokalizacjaID", ComboBox3.SelectedIndex)
            sequel.AddParam("@RampaId", ComboBox4.SelectedIndex)
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
        cmd = "UPDATE praktyka.tblZlecenia SET "
        ZmianyStr = "Zmieniono:{"
        Dim updateParts As New List(Of String)
        Dim params As New Dictionary(Of String, Object)
        ZmienionoUwagi = False

        ' Check for changes and prepare SQL command
        If ComboBox2.SelectedIndex <> PrzewoznicyID.FindIndex(Function(x) x = zlecenieInDataBase.Rows.Item(0)(2)) Then
            updateParts.Add(zlecenieInDataBase.Columns(2).ColumnName & " = @PrzewoznicyID")
            params.Add("@PrzewoznicyID", PrzewoznicyID(ComboBox2.SelectedIndex))
            ZmianyStr += zlecenieInDataBase.Columns(2).ColumnName & ","
        End If

        If ComboBox3.SelectedIndex <> zlecenieInDataBase.Rows.Item(0)(3) Then
            updateParts.Add(zlecenieInDataBase.Columns(3).ColumnName & " = @LokalizacjaID")
            params.Add("@LokalizacjaID", ComboBox3.SelectedIndex)
            ZmianyStr += zlecenieInDataBase.Columns(3).ColumnName & ","
        End If

        If ComboBox4.SelectedIndex <> zlecenieInDataBase.Rows.Item(0)(4) Then
            updateParts.Add(zlecenieInDataBase.Columns(4).ColumnName & " = @RampaID")
            params.Add("@RampaID", ComboBox4.SelectedIndex)
            ZmianyStr += zlecenieInDataBase.Columns(4).ColumnName & ","
        End If

        If TextBox1.Text <> zlecenieInDataBase.Rows.Item(0)(5).ToString() Then
            updateParts.Add(zlecenieInDataBase.Columns(5).ColumnName & " = @TextBox1")
            params.Add("@TextBox1", TextBox1.Text)
            ZmianyStr += zlecenieInDataBase.Columns(5).ColumnName & ","
        End If

        If DateTimePicker1.Value.ToString("yyyyMMdd") <> Convert.ToDateTime(zlecenieInDataBase.Rows.Item(0)(8)).ToString("yyyyMMdd") Then
            updateParts.Add(zlecenieInDataBase.Columns(8).ColumnName & " = @Data")
            params.Add("@Data", DateTimePicker1.Value.ToString("yyyy-MM-dd"))
            ZmianyStr += zlecenieInDataBase.Columns(8).ColumnName & ","
        End If

        If MaskedTextBox1.Text & ":00" <> zlecenieInDataBase.Rows.Item(0)(9).ToString() Then
            updateParts.Add(zlecenieInDataBase.Columns(9).ColumnName & " = @Godzina")
            params.Add("@Godzina", MaskedTextBox1.Text & ":00")
            ZmianyStr += zlecenieInDataBase.Columns(9).ColumnName & ","
        End If

        If TextBox4.Text <> zlecenieInDataBase.Rows.Item(0)(10).ToString() Then
            updateParts.Add(zlecenieInDataBase.Columns(10).ColumnName & " = @TextBox4")
            params.Add("@TextBox4", TextBox4.Text)
            ZmianyStr += zlecenieInDataBase.Columns(10).ColumnName & ","
        End If

        If TextBox5.Text <> zlecenieInDataBase.Rows.Item(0)(11).ToString() Then
            updateParts.Add(zlecenieInDataBase.Columns(11).ColumnName & " = @TextBox5")
            params.Add("@TextBox5", TextBox5.Text)
            ZmianyStr += zlecenieInDataBase.Columns(11).ColumnName & ","
        End If

        If TextBox6.Text <> zlecenieInDataBase.Rows.Item(0)(12).ToString() Then
            ZmienionoUwagi = True
            ZmianyStr += zlecenieInDataBase.Columns(12).ColumnName & ","
        End If

        ' Finalize command string
        If updateParts.Count > 0 Or ZmienionoUwagi Then
            ZmianyStr = ZmianyStr.TrimEnd(","c)
            ZmianyStr += "}"
            updateParts.Add(zlecenieInDataBase.Columns(12).ColumnName & " = @TextBox6")
            params.Add("@TextBox6", TextBox6.Text & " " & ZmianyStr)
            updateParts.Add("Zmieniono = 1")
        Else
            MessageBox.Show("Nie wykryto zmiany żadnego pola!")
            Exit Sub
        End If

        cmd += String.Join(", ", updateParts) & " WHERE ZlecenieId = @ZlecenieId"
        params.Add("@ZlecenieId", Id)

        ' Execute the query
        sequel.SetCommand(cmd)
        For Each param As KeyValuePair(Of String, Object) In params
            sequel.AddParam(param.Key, param.Value)
        Next

        sequel.RunQueryNoData()
        If sequel.QueryResult Then
            MessageBox.Show("Zaktualizowano Zlecenie!")
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
            sequel.SetCommand("DELETE FROM praktyka.tblZlecenia WHERE ZlecenieId = @ZlecenieId")
            sequel.AddParam("@ZlecenieId", Id)
            sequel.RunQueryNoData()
            If sequel.QueryResult Then
                sequel.SetCommand("DELETE FROM praktyka.tblOdbiory WHERE IdZlecenia = @IdZlecenia")
                sequel.AddParam("@IdZlecenia", Id)
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

End Class