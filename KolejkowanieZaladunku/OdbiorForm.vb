Public Class OdbiorForm
    Private Property UserId As Integer
    Private Property locationButtonText As String
    Private Property OdbiorRamp As String
    Private Property OdbiorCzas As String
    Private Property OdbiorData As String

    Private KlienciID As New List(Of Integer) ' list holding id's with indexes equivalent to indexes in ComboBox1
    Private KlienciPrzelicznik As New List(Of Integer) ' list holding PaletyToMin with indexes equivalent to indexes in ComboBox1
    Private PrzewoznicyID As New List(Of Integer) ' list holding id's with indexes equivalent to indexes in ComboBox2
    Private SOSRampy As New List(Of String)
    Private KATRampy As New List(Of String)

    Public Sub New(UserId As Integer,
               Optional locationButtonText As String = "",
               Optional OdbiorRamp As String = "",
               Optional OdbiorCzas As String = "",
               Optional OdbiorData As String = "")

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _UserId = UserId
        _locationButtonText = locationButtonText
        _OdbiorRamp = OdbiorRamp
        _OdbiorCzas = OdbiorCzas
        _OdbiorData = OdbiorData


    End Sub
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
            KATRampy.Add(sequel.SQLDT.Rows.Item(i).Item(1))
        Next
        'set rampy
        ComboBox3.SelectedItem = locationButtonText
        ComboBox3_SelectedIndexChanged()

        If OdbiorRamp <> "" Then
            ComboBox4.SelectedItem = OdbiorRamp
        End If
        If OdbiorCzas <> "" Then
            MaskedTextBox1.Text = OdbiorCzas
        End If
        If OdbiorData <> "" Then
            DateTimePicker1.Text = OdbiorData
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

        If ComboBox2.SelectedIndex = -1 OrElse ComboBox2.Text = String.Empty Then
            MessageBox.Show("Proszę wybrać Przewoźnika!") ' Customize the message as needed
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

        'check if date is atleast 1 hour into the future
        Dim ts As TimeSpan
        TimeSpan.TryParse(MaskedTextBox1.Text, ts)
        If DateTimePicker1.Value.Date.Add(ts) < Now.AddHours(1) Then
            MessageBox.Show("Wybrana data/czas jest zbyt wczesna! Zlecenie może być zlecone najwcześniej na godzine od chwili obecnej.")
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
       "WHERE tblZlecenia.Data = @Data AND lokalizacjaID = @LokalizacjaID AND rampaId = @RampaId " &
       "AND Godzina < @Godzina ORDER BY Godzina DESC"

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
            cmd = "SELECT TOP 1 tblZlecenia.Godzina, tblZlecenia.Czas, tblZlecenia.ZlecenieId " &
            "FROM praktyka.tblZlecenia " &
            "WHERE tblZlecenia.Data = @Data AND lokalizacjaID = @LokalizacjaID AND rampaId = @RampaId " &
            "AND (DATEPART(HOUR, Godzina) * 60 + DATEPART(MINUTE, Godzina) + Czas) > 1440 " &
            "ORDER BY Godzina"

            sequel.SetCommand(cmd)
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
        "WHERE tblZlecenia.Data = @Data AND lokalizacjaID = @LokalizacjaID AND rampaId = @RampaId " &
        "AND Godzina > @Godzina ORDER BY Godzina"

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
            "WHERE tblZlecenia.Data = @Data AND lokalizacjaID = @LokalizacjaID AND rampaId = @RampaId " &
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



        'input zlecenie
        cmd = "INSERT INTO praktyka.tblZlecenia(KlientId,PrzewoznikId,LokalizacjaId,RampaId,NrRejestracyjny,NrTransportu,NrWz,Data,Godzina,Palety,Czas,Uwagi) " &
       "OUTPUT INSERTED.ZlecenieId " &
       "VALUES (@KlientId, @PrzewoznikId, @LokalizacjaId, @RampaId, @NrRejestracyjny, @NrTransportu, @NrWz, @Data, @Godzina, @Palety, @Czas, @Uwagi)"

        sequel.SetCommand(cmd)
        sequel.AddParam("@KlientId", KlienciID(ComboBox1.SelectedIndex))
        sequel.AddParam("@PrzewoznikId", PrzewoznicyID(ComboBox2.SelectedIndex))
        sequel.AddParam("@LokalizacjaId", ComboBox3.SelectedIndex)
        sequel.AddParam("@RampaId", ComboBox4.SelectedIndex)
        sequel.AddParam("@NrRejestracyjny", TextBox1.Text)
        sequel.AddParam("@NrTransportu", TextBox2.Text)
        sequel.AddParam("@NrWz", TextBox3.Text)
        sequel.AddParam("@Data", DateTimePicker1.Value.Date)
        sequel.AddParam("@Godzina", MaskedTextBox1.Text & ":00")
        sequel.AddParam("@Palety", TextBox4.Text)
        sequel.AddParam("@Czas", TextBox5.Text)
        sequel.AddParam("@Uwagi", TextBox6.Text)

        sequel.RunDataQuery()

        If sequel.QueryResult = True Then
            'create odbiór for zlecenie
            createdId = sequel.SQLDT.Rows.Item(0).Item(0)
            cmd = "INSERT INTO praktyka.tblOdbiory(IdZlecenia, Utworzenie, UtworzeniePrac, UtworzenieCZAS) " &
            "VALUES (@IdZlecenia, 1, @UtworzeniePrac, @UtworzenieCZAS)"
            sequel.SetCommand(cmd)
            sequel.AddParam("@IdZlecenia", createdId) ' Assuming createdId is a variable holding the ID
            sequel.AddParam("@UtworzeniePrac", UserId) ' Assuming UserId is a variable holding the user ID
            sequel.AddParam("@UtworzenieCZAS", Now) ' Directly using Now() function to get current date and time
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


End Class