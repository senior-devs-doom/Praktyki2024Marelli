Public Class UpdateFormLogistyka
    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim sequel As New RapidConnection()
        Dim data As System.Data.DataRow

        '''''''''''''''' load zlecenie
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

        ''''''''''''load values in zlecenie
        sequel.SetCommand("select * from praktyka.tblZlecenia where ZlecenieId=" & KolejkowanieZaładunku.id)
        sequel.RunDataQuery()
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
        For i As Integer = 0 To sequel.SQLDT.Rows.Count - 1
            ' Create a new instance of CheckBox
            Dim checkBox As New CheckBox()

            ' Set properties of the checkbox
            checkBox.Text = sequel.SQLDT.Rows.Item(i).Item(0)
            Flagi.Add(sequel.SQLDT.Rows.Item(i).Item(0))
            checkBox.AutoSize = True ' Optional: To auto-size the checkbox according to its text
            checkBox.Dock = DockStyle.Fill

            ' Add the checkbox to the FlagPanel
            FlagPanel.Controls.Add(checkBox)
        Next
        ''''''''load values into flagi
        Dim FlagNamesString = String.Join(",", Flagi)
        Dim FalseString As String = String.Join(",", Enumerable.Repeat("0", Flagi.Count))
        sequel.SetCommand("SELECT " & FlagNamesString & " FROM praktyka.tblOdbiory  WHERE ZlecenieId=" & KolejkowanieZaładunku.id)
        sequel.RunDataQuery()
        If sequel.SQLDT.Rows.Count = 0 Then
            'Odbiór doesn't exist so we make it
            sequel.SetCommand("insert into praktyka.tblOdbiory(ZlecenieId," & FlagNamesString & ") values (" & KolejkowanieZaładunku.id & "," & FalseString & ")")
            sequel.RunDataQuery()
            For i As Integer = 0 To Flagi.Count - 1
                FlagiValue.Add(False)
            Next
        Else
            'Odbiór exists so we parse it into form
            For i As Integer = 0 To sequel.SQLDT.Rows.Item(0).ItemArray.Length - 1
                DirectCast(FlagPanel.Controls(i), CheckBox).Checked = sequel.SQLDT.Rows.Item(0).Item(i)
                FlagiValue.Add(sequel.SQLDT.Rows.Item(0).Item(i))
            Next
        End If
        'We now should have a list of database Flags and corresponding values
        'For i As Integer = 0 To Flagi.Count - 1
        '    Console.WriteLine("Flag: " & Flagi(i) & ", Value: " & FlagiValue(i))
        'Next
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
        Dim Queryresult As Boolean
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

        'update Zlecenie
        cmd = "UPDATE praktyka.tblZlecenia SET KlientId = " & KlienciID(ComboBox1.SelectedIndex) & ", PrzewoznikId = " & PrzewoznicyID(ComboBox2.SelectedIndex) & ", LokalizacjaId = " & ComboBox3.SelectedIndex & ", RampaId = " & ComboBox4.SelectedIndex & ", NrRejestracyjny = '" & TextBox1.Text & "', NrTransportu = '" & TextBox2.Text & "', NrWz = '" & TextBox3.Text & "', Data = '" & DateTimePicker1.Value.ToString("yyyy-MM-dd") & "', Godzina = '" & MaskedTextBox1.Text & ":00', Palety = " & TextBox4.Text & ", Czas = " & TextBox5.Text & ", Uwagi = '" & TextBox6.Text & "' where ZlecenieId=" & KolejkowanieZaładunku.id
        sequel.SetCommand(cmd)
        sequel.RunQueryNoData()
        Queryresult = sequel.QueryResult

        'Update Odbiory
        'Można zmienić dowolną flage, system rejestruje kto i kiedy zmienił daną flage ostatni
        'making a lot 2 lists cause it's the most striaghtforward and clear solution.

        Dim WorkerId = 1 ' DELETE, IN FINAL VERSION GET IF OFF RAPID
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
            stringBuilder.AppendFormat("{0} = {1}, {0}PRAC = {2}, {0}CZAS = '{3}'", FlagiChanged(i), FlagiValueChanged(i), WorkerId, Czas)
        Next

        cmd = "UPDATE praktyka.tblOdbiory SET " & stringBuilder.ToString() & " where ZlecenieId=" & KolejkowanieZaładunku.id
        Console.WriteLine(cmd)
        sequel.SetCommand(cmd)
        sequel.RunQueryNoData()
        'For i As Integer = 0 To FlagiChanged.Count - 1
        '    Console.WriteLine("Flag: " & FlagiChanged(i) & ", Value: " & FlagiValueChanged(i))
        'Next

        'if good then we good
        If sequel.QueryResult = True AndAlso Queryresult = True Then
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
            sequel.SetCommand("DELETE FROM praktyka.tblZlecenia WHERE ZlecenieId = " & KolejkowanieZaładunku.id)
            sequel.RunQueryNoData()
            If sequel.QueryResult = True Then
                MessageBox.Show("Usunięto Zlecenie!")
                Me.Close()
            End If
        End If
    End Sub

    Private KlienciID As New List(Of Integer)
    Private PrzewoznicyID As New List(Of Integer)
    'can't be asked to make this 2 dimensional list, list 1 holds checkbox names, list 2 holds their values. Spoilers, when we Update we make more lists.
    Private Flagi As New List(Of String)
    Private FlagiValue As New List(Of Boolean)

End Class