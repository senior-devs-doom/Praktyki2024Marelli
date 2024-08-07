Public Class UpdateOdbiorMagazyn
    Private Property UserId As Integer
    Private Property Id As Integer

    Private KlienciID As New List(Of Integer)
    Private PrzewoznicyID As New List(Of Integer)
    Private KlienciPrzelicznik As New List(Of Integer) ' list holding PaletyToMin with indexes equivalent to indexes in ComboBox1
    Private zlecenieInDataBase As DataTable
    Private SOSRampy As New List(Of String)
    Private KatRampy As New List(Of String)
    'can't be asked to make this 2 dimensional list, list 1 holds checkbox names, list 2 holds their values. Spoilers, when we Update we make more lists.
    Private Flagi As New List(Of String)
    Private FlagiValue As New List(Of Boolean)
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
        sequel.SetCommand("select * from praktyka.tblZlecenia where ZlecenieId= @Id")
        sequel.AddParam("@Id", Id, DbType.Int64)
        sequel.RunDataQuery()
        zlecenieInDataBase = sequel.SQLDT
        data = sequel.SQLDT.Rows.Item(0) 'KlienciID PrzewoznicyID
        ComboBox1.SelectedIndex = KlienciID.FindIndex(Function(x) x = data(1)) 'looks for clientId in client list and passes it to combobox
        ComboBox2.SelectedIndex = PrzewoznicyID.FindIndex(Function(x) x = data(2)) 'looks for przaewoznicid in przewoznik list and passes it to combobox
        ComboBox3.SelectedIndex = data(3)
        ComboBox3_SelectedIndexChanged()
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
        sequel.SetCommand("SELECT " & FlagNamesString & " FROM praktyka.tblOdbiory WHERE [IdZlecenia]=@Id")
        sequel.AddParam("@Id", Id)
        sequel.RunDataQuery()
        'parse odbiór into form
        For i = 0 To sequel.SQLDT.Rows.Item(0).ItemArray.Length - 1
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
        sequel.SetCommand("UPDATE praktyka.tblZlecenia SET Zmieniono = 0 WHERE [ZlecenieId] = @Id")
        sequel.AddParam("@Id", Id)
        sequel.RunDataQuery()

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
        End If

        'additionally if zlecenie ended
        'zlecenie closes if it's WYDANE and in the past. 
        'Note that all flags change to true but don't update in database till user presses 'aktualizuj'
        'this might get reported as a bug, might not matter, we'll see.
        If data(8).Add(data(9)).AddMinutes(data(10)) <= Now() AndAlso CType(FlagPanel.Controls(FlagPanel.Controls.Count - 1), CheckBox).Checked = True Then
            TextBox1.Enabled = False
            TextBox6.Enabled = False
            For Each ctrl As Control In FlagPanel.Controls
                If TypeOf ctrl Is CheckBox Then
                    CType(ctrl, CheckBox).Checked = True
                    CType(ctrl, CheckBox).Enabled = False
                End If
            Next
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
        Dim UPDTZlecenieResult = True ' if we do run zlecenie update save if it suceeded

        ' Check if ComboBox1 has a selected item
        If ComboBox1.SelectedIndex = -1 OrElse ComboBox1.Text = String.Empty Then
            MessageBox.Show("Proszę wybrać Klienta!") ' Customize the message as needed
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

        'check if date has been CHANGED to the past
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
        cmd = "select top 1 tblZlecenia.Godzina, tblZlecenia.Czas, tblZlecenia.ZlecenieId " &
        "from praktyka.tblZlecenia " &
        "where tblZlecenia.Data=@Data " &
        "and lokalizacjaID=@LokalizacjaID " &
        "and rampaId=@RampaID " &
        "and Godzina < @Godzina " &
        "order by Godzina desc"

        sequel.SetCommand(cmd)
        sequel.AddParam("@Data", DateTimePicker1.Value.Date)
        sequel.AddParam("@LokalizacjaID", ComboBox3.SelectedIndex)
        sequel.AddParam("@RampaID", ComboBox4.SelectedIndex)
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
            cmd = "select top 1 tblZlecenia.Godzina, tblZlecenia.Czas, tblZlecenia.ZlecenieId " &
            "from praktyka.tblZlecenia " &
            "where tblZlecenia.Data=@Data " &
            "and lokalizacjaID=@LokalizacjaID " &
            "and rampaId=@RampaID " &
            "AND (DATEPART(HOUR, Godzina) * 60 + DATEPART(MINUTE, Godzina) + Czas) > 1440 " &
            "order by Godzina"

            sequel.SetCommand(cmd)
            sequel.AddParam("@Data", DateTimePicker1.Value.Date.AddDays(-1))
            sequel.AddParam("@LokalizacjaID", ComboBox3.SelectedIndex)
            sequel.AddParam("@RampaID", ComboBox4.SelectedIndex)
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

        cmd = "select top 1 tblZlecenia.Godzina, tblZlecenia.Czas, tblZlecenia.ZlecenieId " &
        "from praktyka.tblZlecenia " &
        "where tblZlecenia.Data=@Data " &
        "and lokalizacjaID=@LokalizacjaID " &
        "and rampaId=@RampaID " &
        "and Godzina > @Godzina " &
        "order by Godzina"

        sequel.SetCommand(cmd)
        sequel.AddParam("@Data", DateTimePicker1.Value.Date)
        sequel.AddParam("@LokalizacjaID", ComboBox3.SelectedIndex)
        sequel.AddParam("@RampaID", ComboBox4.SelectedIndex)
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
            cmd = "select top 1 tblZlecenia.Godzina, tblZlecenia.Czas, tblZlecenia.ZlecenieId " &
            "from praktyka.tblZlecenia " &
            "where tblZlecenia.Data=@Data " &
            "and lokalizacjaID=@LokalizacjaID " &
            "and rampaId=@RampaID " &
            "order by Godzina desc"

            sequel.SetCommand(cmd)
            sequel.AddParam("@Data", DateTimePicker1.Value.AddDays(1).Date)
            sequel.AddParam("@LokalizacjaID", ComboBox3.SelectedIndex)
            sequel.AddParam("@RampaID", ComboBox4.SelectedIndex)
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
        Dim updateParts As New List(Of String)
        Dim params As New Dictionary(Of String, Object)

        If ComboBox1.SelectedIndex <> KlienciID.FindIndex(Function(x) x = zlecenieInDataBase.Rows.Item(0)(1)) Then
            updateParts.Add(zlecenieInDataBase.Columns(1).ColumnName & " = @KlienciID")
            params.Add("@KlienciID", KlienciID(ComboBox1.SelectedIndex))
        End If

        If ComboBox2.SelectedIndex <> PrzewoznicyID.FindIndex(Function(x) x = zlecenieInDataBase.Rows.Item(0)(2)) Then
            updateParts.Add(zlecenieInDataBase.Columns(2).ColumnName & " = @PrzewoznicyID")
            params.Add("@PrzewoznicyID", PrzewoznicyID(ComboBox2.SelectedIndex))
        End If

        If ComboBox3.SelectedIndex <> zlecenieInDataBase.Rows.Item(0)(3) Then
            updateParts.Add(zlecenieInDataBase.Columns(3).ColumnName & " = @LokalizacjaID")
            params.Add("@LokalizacjaID", ComboBox3.SelectedIndex)
        End If

        If ComboBox4.SelectedIndex <> zlecenieInDataBase.Rows.Item(0)(4) Then
            updateParts.Add(zlecenieInDataBase.Columns(4).ColumnName & " = @RampaID")
            params.Add("@RampaID", ComboBox4.SelectedIndex)
        End If

        If TextBox1.Text <> zlecenieInDataBase.Rows.Item(0)(5).ToString() Then
            updateParts.Add(zlecenieInDataBase.Columns(5).ColumnName & " = @TextBox1")
            params.Add("@TextBox1", TextBox1.Text)
        End If

        If DateTimePicker1.Value.ToString("yyyyMMdd") <> Convert.ToDateTime(zlecenieInDataBase.Rows.Item(0)(8)).ToString("yyyyMMdd") Then
            updateParts.Add(zlecenieInDataBase.Columns(8).ColumnName & " = @Data")
            params.Add("@Data", DateTimePicker1.Value.ToString("yyyy-MM-dd"))
        End If

        If MaskedTextBox1.Text & ":00" <> zlecenieInDataBase.Rows.Item(0)(9).ToString() Then
            updateParts.Add(zlecenieInDataBase.Columns(9).ColumnName & " = @Godzina")
            params.Add("@Godzina", MaskedTextBox1.Text & ":00")
        End If

        If TextBox4.Text <> zlecenieInDataBase.Rows.Item(0)(10).ToString() Then
            updateParts.Add(zlecenieInDataBase.Columns(10).ColumnName & " = @TextBox4")
            params.Add("@TextBox4", TextBox4.Text)
        End If

        If TextBox5.Text <> zlecenieInDataBase.Rows.Item(0)(11).ToString() Then
            updateParts.Add(zlecenieInDataBase.Columns(11).ColumnName & " = @TextBox5")
            params.Add("@TextBox5", TextBox5.Text)
        End If

        If TextBox6.Text <> zlecenieInDataBase.Rows.Item(0)(12).ToString() Then
            updateParts.Add(zlecenieInDataBase.Columns(12).ColumnName & " = @TextBox6")
            params.Add("@TextBox6", TextBox6.Text)
        End If

        If updateParts.Count > 0 Then
            cmd += String.Join(", ", updateParts) + " WHERE ZlecenieId = @ZlecenieId"
            params.Add("@ZlecenieId", Id)

            sequel.SetCommand(cmd)

            For Each param As KeyValuePair(Of String, Object) In params
                sequel.AddParam(param.Key, param.Value)
            Next

            sequel.RunQueryNoData()
            UPDTZlecenieResult = sequel.QueryResult
        Else
            UPDTZlecenieResult = False
        End If

        'Update Odbiory
        'system rejestruje kto i kiedy zmienił daną flage
        'making 2 extra lists cause it's the most striaghtforward and clear solution.
        Dim currentTime As String = Now.ToString("HH:mm:ss")
        Dim flagsChanged As New List(Of String)
        Dim flagValuesChanged As New List(Of Boolean)

        ' Loop through each checkbox to check for changes
        For i As Integer = 0 To Flagi.Count - 1
            Dim currentCheckBox As CheckBox = DirectCast(FlagPanel.Controls(i), CheckBox)
            If currentCheckBox.Checked <> FlagiValue(i) Then ' Detect change
                flagsChanged.Add(Flagi(i))
                flagValuesChanged.Add(currentCheckBox.Checked)
            End If
        Next

        ' Building SQL query
        Dim queryBuilder As New System.Text.StringBuilder()
        For i As Integer = 0 To flagsChanged.Count - 1
            If i > 0 Then
                queryBuilder.Append(", ") ' Separator between updates
            End If
            queryBuilder.AppendFormat("{0} = {1}, {0}PRAC = {2}, {0}CZAS = '{3}'", flagsChanged(i), If(flagValuesChanged(i), 1, 0), UserId, currentTime)
        Next

        ' Execute query if changes were made
        If queryBuilder.Length > 0 Then
            Dim sqlCommand As String = "UPDATE praktyka.tblOdbiory SET " & queryBuilder.ToString() & " WHERE IdZlecenia = " & Id
            sequel.SetCommand(sqlCommand)
            sequel.RunQueryNoData()
        ElseIf Not UPDTZlecenieResult Then
            MessageBox.Show("Nie wykryto zmiany żadnego pola!")
            Exit Sub
        End If

        If sequel.QueryResult Then
            MessageBox.Show("Zaktualizowano Zlecenie!")
            Me.Close()
        End If

    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Close()
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs)
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