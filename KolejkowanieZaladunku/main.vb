Public Class main

    Property Admin As Boolean
    Public UserId As Integer 'ID pracownika
    Public UserRights As Integer '1-magazyn 2-logistyk 3-Display/Brama reszta-admin

    'global variables
    Private Zlecenia As New List(Of Panel) 'panels representing individual zlecenie
    Private SOSRampy As New List(Of String)
    Private KATRampy As New List(Of String)
    Public MinToPix As New Single()

    Private DateTimePicker1 As DateTimePicker
    Public WithEvents locationButton As New CheckBox()
    Public rampaLabel1 As Label
    Public rampaLabel2 As Label
    Public rampaLabel3 As Label
    Public rampaLabel4 As Label
    Friend WithEvents menuRight As TableLayoutPanel
    Dim timeRngBtn As Button

    Const IdApp As Integer = 81

    Public Sub New(Params As List(Of Object))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Admin = CBool(Params(0))
        UserId = CInt(Params(1))

    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        'UserId = 34226 'User rights 1 
        UserId = 34227 'User rights 2 
        'UserId = 999999 'User wrongs 0(display only) 
    End Sub


    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        UserRights = CheckRights()
        Dim sequel As New RapidConnection()
        sequel.SetCommand("select Nazwa from praktyka.tblRampySosnowiec")
        sequel.RunDataQuery()
        For i As Integer = 0 To sequel.SQLDT.Rows.Count - 1
            SOSRampy.Add(sequel.SQLDT.Rows.Item(i).Item(0)) 'get rampy in sosnowiec
        Next

        sequel = New RapidConnection()
        sequel.SetCommand("select Nazwa from praktyka.tblRampyKatowice")
        sequel.RunDataQuery()
        For i As Integer = 0 To sequel.SQLDT.Rows.Count - 1
            KATRampy.Add(sequel.SQLDT.Rows.Item(i).Item(0)) 'get rampy in kato
        Next

        MinToPix = 0.64166666 'How long in pixels is one minute on chart, it's also declared in timeRngBtn_click

        Me.SuspendLayout() 'visual studio is do-do trash unoptimized and reloads all elements everytime I add one, this command turns off rendering, I turn it on again at the end Me.ResumeLayout(True)

        Me.Location = New Point(-7, 0)

        With TableLayoutPanel1
            ' Set TableLayoutPanel to fill the form
            .BackColor = Color.Transparent
            .Padding = New Padding(0)
            .Margin = New Padding(0)
            ' Set width of table columns as a percentage of the table's total width
            .ColumnStyles(0).Width = 101
            For i As Integer = 1 To 4
                .ColumnStyles(i).Width = 453

            Next

            ' Set height of the first two rows explicitly
            .RowStyles(0).Height = 42
            .RowStyles(1).Height = 40

            ' Use a loop to set the height of rows 2 through 25 consistently
            For i As Integer = 2 To 25 '37 is too short, 38 is too long, it is what it is.
                If i Mod 2 = 0 Then
                    .RowStyles(i).Height = 37
                Else
                    .RowStyles(i).Height = 38
                End If
            Next
            'button menu is not part of this tablelayout!
        End With

        'fill table with labels
        InitializeTableLayoutPanelLabels() 'I refer to labels by their children index

        'fill first row with date
        InitializeDateRow()

        'fill last column
        InitializeMenu()

        'there's our boy
        InitializeMarelliLogo()

        'fill rist row with location
        Initializelocation()

        'makes sure tablelayout doesnt cover anything
        TableLayoutPanel1.SendToBack()

        'loadZlecenia()
        'load zlecenia fires on load from Initializelocation() -> Private Sub locationButton_CheckedChanged(sender As Object, e As EventArgs) Handles locationButton.CheckedChanged
        DisplayOnlyRefreshTimer.Stop()
        If UserRights = 0 Then
            'periodical refreshing of display
            'currently for display mode only
            'since it checks one display only button
            DisplayOnlyRefreshTimer.Start()
        End If

        Me.ResumeLayout(True)

    End Sub

    Private Function CheckRights() As Integer
        Dim RetVal As Integer
        Dim DbData As New RapidConnection

        DbData.SetCommand("SELECT [AccessGroup] 
                            FROM [tblApplicationAccess]
                            WHERE [IdUser] = @Id AND [IdApp] = @Ida")
        DbData.AddParam("@Id", UserId, DbType.Int64)
        DbData.AddParam("@Ida", IdApp, DbType.Int64)
        RetVal = DbData.RunQueryNoData("Sc")

        Return RetVal

    End Function

    Private Sub Initializelocation()
        ' Set the appearance to Button so it looks like a toggle button
        locationButton.Appearance = Appearance.Button
        locationButton.Dock = DockStyle.Fill
        locationButton.TextAlign = ContentAlignment.MiddleCenter
        locationButton.Margin = New Padding(0)
        ' Configure the toggle button text to show the current state
        locationButton.Checked = True ' starts as "Sosnowiec"

        ' Add the toggle button to TableLayoutPanel1 at cell (1, 0)
        TableLayoutPanel1.Controls.Add(locationButton, 1, 0)
    End Sub
    Private Sub InitializeDateRow()
        Dim buttonsPanel As TableLayoutPanel = CreateButtonRow()
        TableLayoutPanel1.Controls.Add(buttonsPanel, 2, 0)
        TableLayoutPanel1.SetColumnSpan(buttonsPanel, 2)
        If UserRights = 0 Then
            buttonsPanel.Visible = False
            timeRngBtn = New Button()
            With timeRngBtn
                .Text = "8 H"
                .Dock = DockStyle.Fill
                .Margin = New Padding(0)
            End With
            TableLayoutPanel1.Controls.Add(timeRngBtn, 2, 0)
            AddHandler timeRngBtn.Click, AddressOf timeRngBtn_Click
        End If
    End Sub
    Private Sub InitializeTableLayoutPanelLabels() ' Adding labels to the TableLayoutPanel1
        Dim label As Label
        For row As Integer = 2 To 25 ' For 23 rows
            label = New Label()
            With label
                .Margin = New Padding(1, 1, 0, 0)
                .Name = $"Label0_{row}" ' Corrected the string interpolation
                .Text = $"{row - 2}:00"
                .TextAlign = ContentAlignment.MiddleCenter
                .Font = New Font("Microsoft Sans Serif", 18, FontStyle.Bold)
                .AutoSize = False
                .Dock = DockStyle.Fill
            End With

            ' Add the label to the TableLayoutPanel
            TableLayoutPanel1.Controls.Add(label, 0, row)
        Next

        Me.rampaLabel1 = New Label()
        With Me.rampaLabel1
            .Text = SOSRampy(0)
            .Name = "rampaLabel1"
            .ForeColor = Color.White
            .BackColor = Color.FromArgb(0, 156, 222)
            .Margin = New Padding(0)
            .TextAlign = ContentAlignment.MiddleCenter
            .Font = New Font("Microsoft Sans Serif", 18, FontStyle.Bold)
            .AutoSize = False
            .Dock = DockStyle.Fill
        End With
        TableLayoutPanel1.Controls.Add(Me.rampaLabel1, 1, 1)

        Me.rampaLabel2 = New Label()
        With rampaLabel2
            .Text = SOSRampy(1)
            .Name = "rampaLabel2"
            .BackColor = Color.FromArgb(0, 156, 222)
            .ForeColor = Color.White
            .Margin = New Padding(0)
            .TextAlign = ContentAlignment.MiddleCenter
            .Font = New Font("Microsoft Sans Serif", 18, FontStyle.Bold)
            .AutoSize = False
            .Dock = DockStyle.Fill
        End With
        TableLayoutPanel1.Controls.Add(Me.rampaLabel2, 2, 1)

        Me.rampaLabel3 = New Label()
        With rampaLabel3
            .Text = SOSRampy(2)
            .Name = "rampaLabel3"
            .BackColor = Color.FromArgb(0, 156, 222)
            .ForeColor = Color.White
            .Margin = New Padding(0)
            .TextAlign = ContentAlignment.MiddleCenter
            .Font = New Font("Microsoft Sans Serif", 18, FontStyle.Bold)
            .AutoSize = False
            .Dock = DockStyle.Fill
        End With
        TableLayoutPanel1.Controls.Add(Me.rampaLabel3, 3, 1)

        Me.rampaLabel4 = New Label()
        With rampaLabel4
            .Text = SOSRampy(3)
            .Name = "rampaLabel4"
            .BackColor = Color.FromArgb(0, 156, 222)
            .ForeColor = Color.White
            .Margin = New Padding(0)
            .TextAlign = ContentAlignment.MiddleCenter
            .Font = New Font("Microsoft Sans Serif", 18, FontStyle.Bold)
            .AutoSize = False
            .Dock = DockStyle.Fill
        End With
        TableLayoutPanel1.Controls.Add(Me.rampaLabel4, 4, 1)
    End Sub
    Private Sub InitializeMenu()
        'I ain't got no clue how to make table layout dynamic, visual designer was changing things without showing me
        'I debuged this for 120 minutes, I'm tired and frustrated so this Sub might be dubious, width of kids defined at the bottom
        Me.menuRight = New System.Windows.Forms.TableLayoutPanel()
        Me.Controls.Add(Me.menuRight)
        With menuRight
            .ColumnCount = 1
            .RowCount = 13
            .Location = New Point(1873, 44) 'numbers here don't make sense, it's fine, don't think about it, everything's ffiiiinnneeeee
            .Width = 45
            .Height = 512
            .Name = "menu"
            .BackColor = Color.Transparent
            .Margin = New Padding(0)
            .Padding = New Padding(0)

            .RowStyles.Add(New RowStyle(SizeType.Absolute, 41))
            For i As Integer = 1 To 12
                .RowStyles.Add(New RowStyle(SizeType.Absolute, 39))
            Next
        End With

        ' Add the TableLayoutPanel to the form's controls
        Me.Controls.Add(menuRight)
        menuRight.BringToFront()

        ' Create and configure menuButton
        Dim menuButton As New Button()
        With menuButton
            .Name = "menuButton"
            .Text = "≡"
            .Font = New Font("Microsoft Sans Serif", 18, FontStyle.Bold)
            .BackColor = Color.FromArgb(0, 156, 222)
            .ForeColor = Color.White
            .Margin = New Padding(0, 0, 0, 1)
            .TextAlign = ContentAlignment.MiddleCenter
            .AutoSize = False
            .Dock = DockStyle.Fill
            .FlatStyle = FlatStyle.Flat
            .FlatAppearance.BorderColor = Color.Black
            .FlatAppearance.BorderSize = 2
        End With
        menuRight.Controls.Add(menuButton, 1, 0)
        AddHandler menuButton.Click, AddressOf menuButton_Click

        ' Create and configure refreshButton
        Dim refreshButton As New Button()
        With refreshButton
            .Text = "↻"
            .Name = "refreshButton"
            .Margin = New Padding(0, 0, 0, 1)
            .Dock = DockStyle.Fill
            .BackColor = Color.FromArgb(0, 156, 222)
            .ForeColor = Color.White
            .FlatStyle = FlatStyle.Flat
            .FlatAppearance.BorderColor = Color.Black
            .FlatAppearance.BorderSize = 2
        End With
        menuRight.Controls.Add(refreshButton, 1, 1)
        AddHandler refreshButton.Click, AddressOf refreshButton_Click


        Dim odbiorForm As New Button()
        With odbiorForm
            .Text = "📝"
            .Name = "odbiorForm"
            .Margin = New Padding(0, 0, 0, 1)
            .Dock = DockStyle.Fill
            .BackColor = Color.FromArgb(0, 156, 222)
            .ForeColor = Color.White
            .FlatStyle = FlatStyle.Flat
            .FlatAppearance.BorderColor = Color.Black
            .FlatAppearance.BorderSize = 2
        End With
        menuRight.Controls.Add(odbiorForm, 1, 2)
        AddHandler odbiorForm.Click, AddressOf odbiorButton_Click

        Dim KlientForm As New Button()
        With KlientForm
            .Text = "👤"
            .Name = "KllientForm"
            .Margin = New Padding(0, 0, 0, 1)
            .Dock = DockStyle.Fill
            .BackColor = Color.FromArgb(0, 156, 222)
            .ForeColor = Color.White
            .FlatStyle = FlatStyle.Flat
            .FlatAppearance.BorderColor = Color.Black
            .FlatAppearance.BorderSize = 2
        End With
        menuRight.Controls.Add(KlientForm, 1, 3)
        AddHandler KlientForm.Click, AddressOf klientButton_Click


        Dim przewoznikForm As New Button()
        With przewoznikForm
            .Text = "🚚"
            .Name = "przewoznikForm"
            .Margin = New Padding(0, 0, 0, 1)
            .Dock = DockStyle.Fill
            .BackColor = Color.FromArgb(0, 156, 222)
            .ForeColor = Color.White
            .FlatStyle = FlatStyle.Flat
            .FlatAppearance.BorderColor = Color.Black
            .FlatAppearance.BorderSize = 2
        End With
        menuRight.Controls.Add(przewoznikForm, 1, 4)
        AddHandler przewoznikForm.Click, AddressOf przewoznikButton_Click

        ' Other controls (legendLabel1, legendLabel2, legendLabel3, legendLabel4, legendlabel5, legendlabel6)
        ' can be adjusted in a similar manner if needed
        ' Creating and configuring legendLabel1
        Dim legendLabel1 As New Label()
        With legendLabel1
            .Text = "Legenda: "
            .Name = "legendLabel1"
            .Margin = New Padding(0)
            .Dock = DockStyle.Fill
            .Visible = False
        End With
        menuRight.Controls.Add(legendLabel1, 1, 6)

        ' Creating and configuring legendLabel2
        Dim legendLabel2 As New Label()
        With legendLabel2
            .Text = "Oczekujące"
            .Name = "legendLabel2"
            .Margin = New Padding(0)
            .Dock = DockStyle.Fill
            .BackColor = Color.FromArgb(252, 202, 3)
            .Visible = False
        End With
        menuRight.Controls.Add(legendLabel2, 1, 7)

        ' Creating and configuring legendLabel3
        Dim legendLabel3 As New Label()
        With legendLabel3
            .Text = "Wydane"
            .Name = "legendLabel3"
            .Margin = New Padding(0)
            .Dock = DockStyle.Fill
            .BackColor = Color.FromArgb(82, 166, 8)
            .Visible = False
        End With
        menuRight.Controls.Add(legendLabel3, 1, 8)

        ' Creating and configuring legendLabel4
        Dim legendLabel4 As New Label()
        With legendLabel4
            .Text = "Opóźnienie"
            .Name = "legendLabel4"
            .AutoSize = False
            .Margin = New Padding(0)
            .Dock = DockStyle.Fill
            .BackColor = Color.FromArgb(230, 77, 69)
            .Visible = False
        End With
        menuRight.Controls.Add(legendLabel4, 1, 9)

        ' Creating and configuring legendLabel4
        Dim legendLabel5 As New Label()
        With legendLabel5
            .Text = "Rozpoczęte"
            .Name = "legendLabel5"
            .AutoSize = False
            .Margin = New Padding(0)
            .Dock = DockStyle.Fill
            .BackColor = Color.FromArgb(52, 210, 235)
            .Visible = False
        End With
        menuRight.Controls.Add(legendLabel5, 1, 10)

        Dim legendLabel6 As New Label()
        With legendLabel6
            .Text = "             Zmiana"
            .Name = "legendLabel6"
            .AutoSize = False
            .Margin = New Padding(0)
            .Dock = DockStyle.Fill
            .BackColor = Color.White
            .Visible = False
            AddHandler .Paint, AddressOf DrawTriangleOnZlecenie
        End With
        menuRight.Controls.Add(legendLabel6, 1, 11)

        'width
        For Each kid As Control In menuRight.Controls
            kid.Width = 45
            kid.Padding = New Padding(0)
        Next

        'toggling Visibility based on UserRights
        If UserRights = 1 Then 'magazyn
            odbiorForm.Visible = False
            KlientForm.Visible = False
            przewoznikForm.Visible = False
        End If
        If UserRights = 2 Then ' logistyka

        End If
        If UserRights = 0 Then 'brama
            menuRight.Visible = False
            'we also modify creating datetime row
        End If

    End Sub
    Private Function CreateButtonRow() As TableLayoutPanel
        ' Create a new TableLayoutPanel
        Dim tableLayoutPanel As New TableLayoutPanel()
        With tableLayoutPanel
            .Dock = DockStyle.Fill
            .Margin = New Padding(0)
            .Padding = New Padding(0)
            .RowCount = 1 ' Single row
            .ColumnCount = 7 ' seven columns
            .ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 10.0F)) ' 15% width for the left button
            .ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 10.0F))
            .ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 10.0F))
            .ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 40.0F)) ' 70% width for the middle button
            .ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 10.0F)) '
            .ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 10.0F)) '
            .ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 10.0F)) ' 15% width for the rightest button
        End With

        ' Create left button
        Dim leftButton As New Button()
        leftButton.Text = "<"
        leftButton.Name = "leftButtonDate"
        leftButton.Margin = New Padding(0)
        leftButton.Dock = DockStyle.Fill ' Ensure the button fills its cell

        ' Create lefter button
        Dim lefterButton As New Button()
        lefterButton.Text = "<<"
        lefterButton.Name = "lefterButtonDate"
        lefterButton.Margin = New Padding(0)
        lefterButton.Dock = DockStyle.Fill ' Ensure the button fills its cell

        ' Create leftest button
        Dim leftestButton As New Button()
        leftestButton.Text = "<<<"
        leftestButton.Name = "leftestButtonDate"
        leftestButton.Margin = New Padding(0)
        leftestButton.Dock = DockStyle.Fill ' Ensure the button fills its cell

        ' Create middle datetimepicker
        'datetimepicker jest nowy więc jego właściwości stylu wogóle nie działają
        'micro soft please ;-;
        Me.DateTimePicker1 = New DateTimePicker()
        Me.DateTimePicker1.Format = DateTimePickerFormat.Custom
        Me.DateTimePicker1.CustomFormat = "yyyy-MM-dd"
        Me.DateTimePicker1.Value = Now()
        Me.DateTimePicker1.Name = "DateTimePicker1"
        Me.DateTimePicker1.Margin = New Padding(0, 1, 0, 0)
        Me.DateTimePicker1.Dock = DockStyle.Fill ' Ensure the button fills its cell
        Me.DateTimePicker1.Font = New Font("Microsoft Sans Serif", 22, FontStyle.Bold)


        ' Create right button
        Dim rightButton As New Button()
        rightButton.Text = ">"
        rightButton.Name = "rightButtonDate"
        rightButton.Margin = New Padding(0)
        rightButton.Dock = DockStyle.Fill ' Ensure the button fills its cell

        ' Create righter button
        Dim righterButton As New Button()
        righterButton.Text = ">>"
        righterButton.Name = "rightetButtonDate"
        righterButton.Margin = New Padding(0)
        righterButton.Dock = DockStyle.Fill ' Ensure the button fills its cell

        ' Create rightest button
        Dim rightestButton As New Button()
        rightestButton.Text = ">>>"
        rightestButton.Name = "rightestButtonDate"
        rightestButton.Margin = New Padding(0)
        rightestButton.Dock = DockStyle.Fill ' Ensure the button fills its cell

        ' Add the buttons to the TableLayoutPanel
        tableLayoutPanel.Controls.Add(leftButton, 2, 0)
        tableLayoutPanel.Controls.Add(lefterButton, 1, 0)
        tableLayoutPanel.Controls.Add(leftestButton, 0, 0) ' Add to the first column
        tableLayoutPanel.Controls.Add(Me.DateTimePicker1, 3, 0) ' Add to the fourth column
        tableLayoutPanel.Controls.Add(rightButton, 4, 0)
        tableLayoutPanel.Controls.Add(righterButton, 5, 0)
        tableLayoutPanel.Controls.Add(rightestButton, 6, 0)

        Me.DateTimePicker1.Parent.Height = Me.DateTimePicker1.Height 'has to be after assigning parent

        AddHandler leftButton.Click, AddressOf LeftButtonDate_Click
        AddHandler lefterButton.Click, AddressOf LefterButtonDate_Click
        AddHandler leftestButton.Click, AddressOf LeftestButtonDate_Click
        AddHandler rightButton.Click, AddressOf RightButtonDate_Click
        AddHandler righterButton.Click, AddressOf RighterButtonDate_Click
        AddHandler rightestButton.Click, AddressOf RightestButtonDate_Click
        AddHandler DateTimePicker1.ValueChanged, AddressOf DateTimePicker1_ValueChanged


        Return tableLayoutPanel
    End Function
    Private Sub InitializeMarelliLogo()

        ' Creating a new PictureBox
        Dim picbox As New PictureBox()
        picbox.Image = My.Resources.MarelliLogo
        picbox.Padding = New Padding(0)
        picbox.Margin = New Padding(0)
        picbox.Dock = DockStyle.Fill
        ' Set the SizeMode to StretchImage to ensure image fills the PictureBox while maintaining aspect ratio
        picbox.SizeMode = PictureBoxSizeMode.StretchImage

        TableLayoutPanel1.Controls.Add(picbox, 0, 0) ' Column and row indices are zero-based
        TableLayoutPanel1.SetRowSpan(picbox, 2) 'how does he find which cell picbox is in btw? freaky
    End Sub

    Private Sub LeftButtonDate_Click(sender As Object, e As EventArgs)
        Me.DateTimePicker1.Value = Me.DateTimePicker1.Value.AddDays(-1) 'Dates are immutable so overwrite, this just looks wrong to me man
    End Sub
    Private Sub LefterButtonDate_Click(sender As Object, e As EventArgs)
        Me.DateTimePicker1.Value = Me.DateTimePicker1.Value.AddMonths(-1) 'Dates are immutable so overwrite, this just looks wrong to me man
    End Sub
    Private Sub LeftestButtonDate_Click(sender As Object, e As EventArgs)
        Me.DateTimePicker1.Value = Me.DateTimePicker1.Value.AddYears(-1) 'Dates are immutable so overwrite, this just looks wrong to me man
    End Sub
    ' Handler for the Right Button click
    Private Sub RightButtonDate_Click(sender As Object, e As EventArgs)
        Me.DateTimePicker1.Value = Me.DateTimePicker1.Value.AddDays(1) 'Dates are immutable so overwrite, this just looks wrong to me man
    End Sub
    Private Sub RighterButtonDate_Click(sender As Object, e As EventArgs)
        Me.DateTimePicker1.Value = Me.DateTimePicker1.Value.AddMonths(1) 'Dates are immutable so overwrite, this just looks wrong to me man
    End Sub
    Private Sub RightestButtonDate_Click(sender As Object, e As EventArgs)
        Me.DateTimePicker1.Value = Me.DateTimePicker1.Value.AddYears(1) 'Dates are immutable so overwrite, this just looks wrong to me man
    End Sub
    Private Sub menuButton_Click(sender As Object, e As EventArgs)
        Me.SuspendLayout() 'visual studio is do-do trash unoptimized and reloads all elements everytime I add one, this command turns off rendering, I turn it on again at the end Me.ResumeLayout(True)

        If menuRight.Width = 220 Then

            Dim fonty As New Font("Microsoft Sans Serif", 18, FontStyle.Bold)
            menuRight.Location = New Point(1873, 44)
            menuRight.Width = 45
            For Each kid As Control In menuRight.Controls
                kid.Width = 45
                kid.Font = fonty
                If TypeOf kid Is Label Then
                    kid.Visible = False
                End If
            Next
            menuRight.Controls(0).Text = "≡"
            menuRight.Controls(1).Text = "↻"
            menuRight.Controls(2).Text = "📝"
            menuRight.Controls(3).Text = "👤"
            menuRight.Controls(4).Text = "🚚"

        Else

            Dim fonty As New Font("Microsoft Sans Serif", 12, FontStyle.Italic)
            menuRight.Location = New Point(1698, 44)
            menuRight.Width = 220
            For Each kid As Control In menuRight.Controls
                kid.Width = 220
                kid.Font = fonty
                If TypeOf kid Is Label Then
                    kid.Visible = True
                    kid.Font = New Font("Microsoft Sans Serif", 18, FontStyle.Bold)
                End If
            Next
            menuRight.Controls(0).Text = "≡"
            menuRight.Controls(1).Text = "Odświerz Arkusz"
            menuRight.Controls(2).Text = "Dodaj zamowienie"
            menuRight.Controls(3).Text = "Dodaj/Usuń Klienta"
            menuRight.Controls(4).Text = "Dodaj/Usuń Przewoźnika"

        End If
        menuRight.BringToFront()
        Me.ResumeLayout(True)
    End Sub
    Private Sub refreshButton_Click(sender As Object, e As EventArgs)
        loadZlecenia()
    End Sub
    Private Sub odbiorButton_Click(sender As Object, e As EventArgs)
        Dim newForm As New OdbiorForm(UserId)
        newForm.ShowDialog()
        loadZlecenia()
    End Sub
    Private Sub klientButton_Click(sender As Object, e As EventArgs)
        Dim newForm As New DodajKlienta()
        newForm.ShowDialog()
    End Sub
    Private Sub przewoznikButton_Click(sender As Object, e As EventArgs)
        Dim newForm As New PrzewoznikForm()
        newForm.ShowDialog()
    End Sub
    Private Sub loadZlecenia()
        Dim sequel As New RapidConnection()
        Dim cmd As String
        Dim data As String
        Dim lokalizacja As Integer

        ' before we load zlecenia we kill current zlecenia
        For Each panel As Panel In Zlecenia.ToList()
            If Not panel.IsDisposed Then
                panel.Dispose() ' This will remove the panel and dispose it
            End If
        Next
        Zlecenia.Clear() ' Clear the list after removing all panels

        data = DateTimePicker1.Value
        If locationButton.Text = "Sosnowiec" Then
            lokalizacja = 0
        ElseIf locationButton.Text = "Katowice" Then
            lokalizacja = 1
        End If

        'all zlecenia from relevant date and location + their completion status
        cmd = "SELECT z.Godzina, z.RampaId, z.Czas, z.Palety, z.Uwagi, k.Nazwa, z.ZlecenieId, o.Wydanie, z.NrRejestracyjny, o.Przygotowanie, z.Zmieniono, " &
        "DATEADD(MINUTE, z.Czas, CAST(z.Data AS DATETIME) + CAST(z.Godzina AS DATETIME)) " &
        "FROM praktyka.tblZlecenia AS z " &
        "INNER JOIN praktyka.tblKlienci AS k ON k.IdKlienta = z.KlientID " &
        "LEFT JOIN praktyka.tblOdbiory AS o ON o.IdZlecenia = z.ZlecenieId " &
        "WHERE z.Data = @data AND z.LokalizacjaId = @lokalizacja"
        sequel.SetCommand(cmd)
        sequel.AddParam("@data", data)
        sequel.AddParam("@lokalizacja", lokalizacja)
        sequel.RunDataQuery()
        For i As Integer = 0 To sequel.SQLDT.Rows.Count - 1
            AddZlecenie(sequel.SQLDT.Rows.Item(i)) 'one row at a time
        Next

        'Now adding zlecenia from previous night that run past midnight
        data = DateTimePicker1.Value.AddDays(-1)

        cmd = "SELECT z.Godzina, z.RampaId, z.Czas, z.Palety, z.Uwagi, k.Nazwa, z.ZlecenieId, o.Wydanie, z.NrRejestracyjny, o.Przygotowanie, z.Zmieniono, " &
        "DATEADD(MINUTE, z.Czas, CAST(z.Data AS DATETIME) + CAST(z.Godzina AS DATETIME)) " &
        "FROM praktyka.tblZlecenia AS z " &
        "INNER JOIN praktyka.tblKlienci AS k ON k.IdKlienta = z.KlientID " &
        "LEFT JOIN praktyka.tblOdbiory AS o ON o.IdZlecenia = z.ZlecenieId " &
        "WHERE z.Data = @data AND z.LokalizacjaId = @lokalizacja AND " &
        "(DATEPART(HOUR, z.Godzina) * 60 + DATEPART(MINUTE, z.Godzina) + z.Czas) > 1440"
        sequel.SetCommand(cmd)
        sequel.AddParam("@data", data)
        sequel.AddParam("@lokalizacja", lokalizacja)
        sequel.RunDataQuery()
        For i As Integer = 0 To sequel.SQLDT.Rows.Count - 1
            Dim row = sequel.SQLDT.Rows.Item(i)
            row(2) -= (TimeSpan.FromDays(1) - row(0)).TotalMinutes 'minuty = minuty - czas pomiędzy rozpoczęciem a północą
            row(0) = TimeSpan.Zero 'godzina = 0:00
            AddZlecenie(row)
        Next
        menuRight.BringToFront()

    End Sub
    Private Sub AddZlecenie(dataRow As System.Data.DataRow)
        'Row :
        'Godzina, RampaID, Czas, Palety, Uwagi, Nazwa(Klienta), ZlecenieID, Wydane(Flaga czy zlecenie zakończono), numer rejestracyjny, przygotowanie(flaga),zmieniono(flaga), predicted endtime
        ' Create a new instance of a label
        Dim NewZlecenie As New Panel()
        Dim NewLabel As New Label()
        Dim IdLabel As New Label()
        Dim startTime As New Integer
        startTime = dataRow(0).TotalMinutes 'minutes since 0:00

        With NewLabel
            ' Set properties of the label
            If dataRow(4) <> "" Then ' jeśli są jakieś uwagi
                .Text = "NR.R. " & dataRow(8) & Environment.NewLine & dataRow(5) & ", " & dataRow(3) & " Palet" & Environment.NewLine & "Uwagi:'" & dataRow(4) & "'"
            Else
                .Text = "NR.R. " & dataRow(8) & Environment.NewLine & dataRow(5) & ", " & dataRow(3) & " Palet"
            End If
            .Name = "zlecenie"

            .ForeColor = Color.White
            .Margin = New Padding(0)
            .Padding = New Padding(0)
            .Dock = DockStyle.Fill
            .TextAlign = ContentAlignment.MiddleCenter
            .Font = New Font("Microsoft Sans Serif", 18, FontStyle.Bold)

            'Draw triangle which signalizes change in top left
            If Not IsDBNull(dataRow(10)) AndAlso dataRow(10) = True Then
                ' Attach the new Paint event handler
                AddHandler .Paint, AddressOf DrawTriangleOnZlecenie
                ' Force the panel to repaint itself with the new handler
                .Invalidate()
            End If
        End With
        Dim labelToolTip As New ToolTip()
        labelToolTip.SetToolTip(NewLabel, NewLabel.Text)

        IdLabel.Visible = False
        IdLabel.Text = dataRow(6)

        NewZlecenie.Controls.Add(IdLabel)
        NewZlecenie.Controls.Add(NewLabel)
        NewZlecenie.Padding = New Padding(4)

        setZlecenieSize(NewZlecenie, startTime, dataRow(1), dataRow(2))

        'ensure text fits inside panel
        'Truns out Label is trash and you can't change the padding height in between top of label and begining of actual text
        'hell, this fake padding size is tied to text size but it's not a set percentage. Pain
        'maybe it can be fixed by setting label position few px above panel position, but sounds like hell to implement so no thanks
        'this looks so bad my lord...
        If NewZlecenie.Height < 32 Then
            NewLabel.Font = New Font("Microsoft Sans Serif", NewZlecenie.Height - 12, FontStyle.Bold)
        End If

        If Not IsDBNull(dataRow(7)) AndAlso dataRow(7) = True Then          '   GREEN
            NewZlecenie.BackColor = Color.FromArgb(104, 207, 14) 'panel(border)
            NewLabel.BackColor = Color.FromArgb(82, 166, 8) 'label(background)
        Else
            If Now() >= dataRow(11) Then                                     '   RED
                NewZlecenie.BackColor = Color.FromArgb(242, 28, 17)
                NewLabel.BackColor = Color.FromArgb(230, 77, 69)
            Else
                If Not IsDBNull(dataRow(9)) AndAlso dataRow(9) = True Then  '   Bluey
                    NewZlecenie.BackColor = Color.FromArgb(52, 210, 235)
                    NewLabel.BackColor = Color.FromArgb(20, 144, 163)
                Else                                                        '   YELLOW
                    NewZlecenie.BackColor = Color.FromArgb(255, 217, 64)
                    NewLabel.BackColor = Color.FromArgb(252, 202, 3)
                End If
            End If
        End If

        Me.Controls.Add(NewZlecenie)
        Zlecenia.Add(NewZlecenie)
        AddHandler NewLabel.Click, AddressOf Zlecenie_Click
        AddHandler NewZlecenie.Click, Sub(sender, e) Zlecenie_Click(NewLabel, e) ' if clicked on new zlecenie(border) then click label
        NewZlecenie.BringToFront() 'for some reason table layout takes priority if not specified

    End Sub
    Private Sub setZlecenieSize(NewOdbior As Panel, startCzas As Integer, Rampa As Integer, minuty As Integer)
        With NewOdbior
            Select Case Rampa
                Case 0
                    .Left = 103
                Case 1
                    .Left = 557
                Case 2
                    .Left = 1011
                Case 3
                    .Left = 1465
                Case Else
                    .Left = 110
                    MessageBox.Show("Nie rozpoznano rampy do załadunku, pewnie wina programisty", "OOPS", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Select

            .Top = 85 + startCzas * MinToPix
            .Width = 453
            .Height = minuty * MinToPix
            If .Height + .Top > 1009 Then 'maximum size
                .Height = 1009 - .Top
            End If
            If .Height < 24 Then 'minimum size
                .Height = 24
                .Padding = New Padding(2)
            End If
        End With
    End Sub
    Private Sub DrawTriangleOnZlecenie(sender As Object, e As PaintEventArgs)
        ' Create points that define triangle.
        Dim point1 As New Point(0, 0)  'top left
        Dim point2 As New Point(0, 50)  ' top right
        Dim point3 As New Point(100, 0) ' bottom left

        Dim curvePoints As Point() = {point1, point2, point3}

        ' Define brush and fill the triangle
        Using brush As New SolidBrush(Color.FromArgb(255, 172, 28))  ' Change color as needed
            e.Graphics.FillPolygon(brush, curvePoints)
        End Using

        ' Optionally, draw the outline of the triangle
        'Using pen As New Pen(Color.Black)  ' Change outline color as needed
        '    e.Graphics.DrawPolygon(pen, curvePoints)
        'End Using
    End Sub
    Private Sub loadZlecenia8H()
        'I got buried here in edge cases for a solid while.
        'ended up with better query then in main loadZlecenia()
        'so you know, refactor if you dare.

        Dim sequel As New RapidConnection()
        Dim cmd As String
        Dim lokalizacja As Integer
        Dim startdate As New DateTime
        Dim enddate As New DateTime

        ' before we load zlecenia we kill current zlecenia
        For Each panel As Panel In Zlecenia.ToList()
            If Not panel.IsDisposed Then
                panel.Dispose() ' This will remove the panel and dispose it
            End If
        Next
        Zlecenia.Clear() ' Clear the list after removing all panels

        If locationButton.Text = "Sosnowiec" Then
            lokalizacja = 0
        ElseIf locationButton.Text = "Katowice" Then
            lokalizacja = 1
        End If

        Dim adjustedTime As DateTime = DateTime.Now.AddHours(-8)
        startdate = New DateTime(adjustedTime.Year, adjustedTime.Month, adjustedTime.Day, adjustedTime.Hour, 0, 0) ' cut off minutes and seconds
        adjustedTime = DateTime.Now.AddHours(8)
        enddate = New DateTime(adjustedTime.Year, adjustedTime.Month, adjustedTime.Day, adjustedTime.Hour, 0, 0) ' cut off minutes and seconds

        cmd = "SELECT z.Godzina, z.RampaId, z.Czas, z.Palety, z.Uwagi, k.Nazwa, z.ZlecenieId, o.Wydanie, z.NrRejestracyjny, o.Przygotowanie, z.Zmieniono, " &
        "CAST(z.Data AS DATETIME) + CAST(z.Godzina AS DATETIME), " &
        "DATEADD(MINUTE, z.Czas, CAST(z.Data AS DATETIME) + CAST(z.Godzina AS DATETIME)) " &
        "FROM praktyka.tblZlecenia AS z " &
        "INNER JOIN praktyka.tblKlienci AS k ON k.IdKlienta = z.KlientID " &
        "LEFT JOIN praktyka.tblOdbiory AS o ON o.IdZlecenia = z.ZlecenieId " &
        "WHERE z.LokalizacjaId = @lokalizacja " &
        "AND DATEADD(MINUTE, z.Czas, CAST(z.Data AS DATETIME) + CAST(z.Godzina AS DATETIME)) > @startdate " &
        "AND CAST(z.Data AS DATETIME) + CAST(z.Godzina AS DATETIME) < @enddate " &
        "ORDER BY CAST(z.Data AS DATETIME) + CAST(z.Godzina AS DATETIME)"
        sequel.SetCommand(cmd)
        sequel.AddParam("@lokalizacja", lokalizacja)
        sequel.AddParam("@startdate", startdate)
        sequel.AddParam("@enddate", enddate)
        sequel.RunDataQuery()

        'Godzina, RampaId, Czas, Palety, Uwagi, Nazwa, ZlecenieId, Wydanie, NrRejestracyjny, Przygotowanie, Zmieniono,  datetimestart, datetimeend
        For i As Integer = 0 To sequel.SQLDT.Rows.Count - 1
            Dim row = sequel.SQLDT.Rows.Item(i)
            'get distance between startdate(starting height) and task in minutes
            Dim startmin = (DirectCast(row(11), DateTime) - startdate).TotalMinutes
            AddZlecenie8H(row, startmin) 'one row at a time
        Next


    End Sub
    Private Sub AddZlecenie8H(dataRow As System.Data.DataRow, startmin As Integer) 'AddZlecenie modified for 16 h display
        'Row :
        'Godzina, RampaID, Czas, Palety, Uwagi, Nazwa(Klienta), ZlecenieID, Wydane(Flaga czy zlecenie zakończono), numer rejestracyjny, przygotowanie(flaga),zmieniono(flaga)
        ' Create a new instance of a label
        Dim NewZlecenie As New Panel()
        Dim NewLabel As New Label()
        Dim IdLabel As New Label()

        With NewLabel
            ' Set properties of the label
            If dataRow(4) <> "" Then ' jeśli są jakieś uwagi
                .Text = "NR.R. " & dataRow(8) & Environment.NewLine & dataRow(5) & ", " & dataRow(3) & " Palet" & Environment.NewLine & "Uwagi:'" & dataRow(4) & "'"
            Else
                .Text = "NR.R. " & dataRow(8) & Environment.NewLine & dataRow(5) & ", " & dataRow(3) & " Palet"
            End If
            .Name = "zlecenie"

            .ForeColor = Color.White
            .Margin = New Padding(0)
            .Padding = New Padding(0)
            .Dock = DockStyle.Fill
            .TextAlign = ContentAlignment.MiddleCenter
            .Font = New Font("Microsoft Sans Serif", 18, FontStyle.Bold)

            'Draw triangle which signalizes change in top left
            If Not IsDBNull(dataRow(10)) AndAlso dataRow(10) = True Then
                ' Attach the new Paint event handler
                AddHandler .Paint, AddressOf DrawTriangleOnZlecenie
                ' Force the panel to repaint itself with the new handler
                .Invalidate()
            End If
        End With
        IdLabel.Visible = False
        IdLabel.Text = dataRow(6)

        NewZlecenie.Controls.Add(IdLabel)
        NewZlecenie.Controls.Add(NewLabel)
        NewZlecenie.Padding = New Padding(4)

        setZlecenieSize8H(NewZlecenie, startmin, dataRow(1), dataRow(2))

        'ensure text fits inside panel
        'Truns out Label is trash and you can't change the padding height in between top of label and begining of actual text
        'hell, this fake padding size is tied to text size but it's not a set percentage. Pain
        'maybe it can be fixed by setting label position few px above panel position, but sounds like hell to implement so no thanks
        'this looks so bad my lord...
        If NewZlecenie.Height < 32 Then
            NewLabel.Font = New Font("Microsoft Sans Serif", NewZlecenie.Height - 12, FontStyle.Bold)
        End If

        If Not IsDBNull(dataRow(7)) AndAlso dataRow(7) = True Then          '   GREEN
            NewZlecenie.BackColor = Color.FromArgb(104, 207, 14) 'panel(border)
            NewLabel.BackColor = Color.FromArgb(82, 166, 8) 'label(background)
        Else
            If Now() >= dataRow(12) Then                                    '   RED
                NewZlecenie.BackColor = Color.FromArgb(242, 28, 17)
                NewLabel.BackColor = Color.FromArgb(230, 77, 69)
            Else
                If Not IsDBNull(dataRow(9)) AndAlso dataRow(9) = True Then  '   Bluey
                    NewZlecenie.BackColor = Color.FromArgb(52, 210, 235)
                    NewLabel.BackColor = Color.FromArgb(20, 144, 163)
                Else                                                        '   YELLOW
                    NewZlecenie.BackColor = Color.FromArgb(255, 217, 64)
                    NewLabel.BackColor = Color.FromArgb(252, 202, 3)
                End If
            End If
        End If

        Me.Controls.Add(NewZlecenie)
        Zlecenia.Add(NewZlecenie)
        AddHandler NewLabel.Click, AddressOf Zlecenie_Click
        AddHandler NewZlecenie.Click, Sub(sender, e) Zlecenie_Click(NewLabel, e) ' if clicked on new zlecenie(border) then click label
        NewZlecenie.BringToFront() 'for some reason table layout takes priority if not specified

    End Sub
    Private Sub setZlecenieSize8H(NewOdbior As Panel, startCzas As Integer, Rampa As Integer, minuty As Integer)
        With NewOdbior
            Select Case Rampa
                Case 0
                    .Left = 103
                Case 1
                    .Left = 557
                Case 2
                    .Left = 1011
                Case 3
                    .Left = 1465
                Case Else
                    .Left = 110
                    MessageBox.Show("Nie rozpoznano rampy do załadunku, pewnie wina programisty", "OOPS", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Select


            .Top = 85 + startCzas * MinToPix
            .Height = minuty * MinToPix

            If .Top < 85 Then 'task start is before the display starts (Top is negative)
                .Height += .Top - 85 'absolutely scuffed but i forgot the logic and this works.
                .Top = 85
            End If

            .Width = 453
            If .Height + .Top > 1009 Then 'maximum size
                .Height = 1009 - .Top
            End If
            If .Height < 24 Then 'minimum size
                .Height = 24
                .Padding = New Padding(2)
            End If
        End With
    End Sub

    Private Sub Zlecenie_Click(sender As Object, e As EventArgs)
        If UserRights = 0 Then ' Wyświetlacz na Bramie
            Exit Sub
        End If

        Dim id As String
        id = DirectCast(sender, Control).Parent.Controls(0).Text ' keeping id a string cause like, eh, it's going into a string anyway

        Select Case UserRights
            Case 1
                Dim newForm As New UpdateOdbiorMagazyn(UserId, id)
                newForm.ShowDialog()
            Case 2
                Dim newForm As New UpdateOdbiorLogistyka(UserId, id)
                newForm.ShowDialog()
            Case Else
                Dim result As DialogResult = MessageBox.Show("YES-logistyka NO-Magazyn", "mfw bezpłatny staż +_+", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If result = DialogResult.Yes Then
                    Dim newForm As New UpdateOdbiorLogistyka(UserId, id)
                    newForm.ShowDialog()
                ElseIf result = DialogResult.No Then
                    Dim newForm As New UpdateOdbiorMagazyn(UserId, id)
                    newForm.ShowDialog()
                End If
        End Select

        loadZlecenia()
    End Sub
    Private Sub locationButton_CheckedChanged(sender As Object, e As EventArgs) Handles locationButton.CheckedChanged
        Me.SuspendLayout()
        ' Assuming unchecked state represents "Katowice" and checked state represents "Sosnowiec"
        If locationButton.Checked Then
            locationButton.Text = "Sosnowiec"
            Me.rampaLabel1.Text = SOSRampy(0)
            Me.rampaLabel2.Text = SOSRampy(1)
            Me.rampaLabel3.Text = SOSRampy(2)
            Me.rampaLabel4.Text = SOSRampy(3)
        Else
            locationButton.Text = "Katowice"
            Me.rampaLabel1.Text = KATRampy(0)
            Me.rampaLabel2.Text = KATRampy(1)
            Me.rampaLabel3.Text = KATRampy(2)
            Me.rampaLabel4.Text = KATRampy(3)
        End If
        loadZlecenia()
        Me.ResumeLayout(True)
    End Sub
    Private Sub TableLayoutPanel1_MouseDown(sender As Object, e As MouseEventArgs) Handles TableLayoutPanel1.MouseDown
        If UserRights = 1 Or UserRights = 0 Then ' no new forms for you >.<
            Exit Sub
        End If

        Dim hour As Integer
        Dim newForm As OdbiorForm
        Dim OdbiorRamp As String 'for passing to odbior form
        Dim OdbiorCzas As String 'for passing to odbior form
        Dim OdbiorData As String 'for passing to odbior form
        OdbiorRamp = "" ' this line just there so I don't see a pointless warning
        If e.Location.X > 1434 Then
            OdbiorRamp = rampaLabel4.Text
        ElseIf e.Location.X > 993 Then
            OdbiorRamp = rampaLabel3.Text
        ElseIf e.Location.X > 552 Then
            OdbiorRamp = rampaLabel2.Text
        ElseIf e.Location.X > 110 Then
            OdbiorRamp = rampaLabel1.Text
        Else
            MessageBox.Show("some error in click event, please blame IT guy :p")
        End If

        hour = Math.Floor((e.Location.Y - 85) / 39) ' pixel math, change if changing layout
        If hour < 10 Then
            OdbiorCzas = "0" & hour & "00"
        Else
            OdbiorCzas = hour & "00"
        End If
        OdbiorData = DateTimePicker1.Text

        newForm = New OdbiorForm(UserId, locationButton.Text, OdbiorRamp, OdbiorCzas, OdbiorData)
        newForm.ShowDialog()
        loadZlecenia()
    End Sub
    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As EventArgs)
        loadZlecenia()
    End Sub
    Private Sub timeRngBtn_Click(sender As Object, e As EventArgs)
        TableLayoutPanel1.SuspendLayout() 'visual studio is do-do trash unoptimized and reloads all elements everytime I add one, this command turns off rendering, I turn it on again at the end Me.ResumeLayout(True)

        If sender.Text = "8 H" Then
            Dim hours As New Integer
            hours = DateTime.Now.AddHours(-8).Hour ' hours in loadzlecenie 8h are seperate variables
            With TableLayoutPanel1
                ' Use a loop to set the height of rows 2 through 25 consistently
                For i As Integer = 2 To 17 '38 is too short, 39 is too long, it is what it is.
                    .RowStyles(i).Height = 56
                    TableLayoutPanel1.GetControlFromPosition(0, i).Text = hours & ":00"
                    hours += 1
                    If hours >= 24 Then
                        hours = 0
                    End If
                Next
                For i As Integer = 18 To 25 '38 is too short, 39 is too long, it is what it is.
                    .RowStyles(i).Height = 0
                Next
            End With

            MinToPix = 0.95
            loadZlecenia8H()
            sender.Text = "24 H"

        ElseIf sender.Text = "24 H" Then
            With TableLayoutPanel1
                ' Use a loop to set the height of rows 2 through 25 consistently
                For i As Integer = 2 To 25 '37 is too short, 38 is too long, it is what it is.
                    TableLayoutPanel1.GetControlFromPosition(0, i).Text = i - 2 & ":00"
                    If i Mod 2 = 0 Then
                        .RowStyles(i).Height = 37
                    Else
                        .RowStyles(i).Height = 38
                    End If
                Next
            End With

            MinToPix = 0.64166666
            loadZlecenia()
            sender.Text = "8 H"
        End If

        TableLayoutPanel1.ResumeLayout(True)
    End Sub
    Private Sub gateTimer_Tick(sender As Object, e As EventArgs) Handles DisplayOnlyRefreshTimer.Tick
        If timeRngBtn.Text = "8 H" Then
            loadZlecenia()
        Else
            loadZlecenia8H()
        End If
    End Sub
    Private Sub DebugMonkey_Click(sender As Object, e As EventArgs) Handles DebugMonkey.Click ' button for debugging
        Console.WriteLine("Mmm... Monke")
    End Sub



End Class
