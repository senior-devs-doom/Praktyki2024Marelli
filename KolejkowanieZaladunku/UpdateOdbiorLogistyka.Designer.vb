<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UpdateFormLogistyka
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UpdateFormLogistyka))
        Me.ComboBox4 = New System.Windows.Forms.ComboBox()
        Me.ComboBox3 = New System.Windows.Forms.ComboBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TextBox6 = New System.Windows.Forms.TextBox()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.TextBox5 = New System.Windows.Forms.TextBox()
        Me.TextBox4 = New System.Windows.Forms.TextBox()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.MaskedTextBox1 = New System.Windows.Forms.MaskedTextBox()
        Me.DateTimePicker1 = New System.Windows.Forms.DateTimePicker()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.ComboBox2 = New System.Windows.Forms.ComboBox()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.FlagPanel = New System.Windows.Forms.FlowLayoutPanel()
        Me.SuspendLayout()
        '
        'ComboBox4
        '
        Me.ComboBox4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.ComboBox4.FormattingEnabled = True
        Me.ComboBox4.Location = New System.Drawing.Point(370, 365)
        Me.ComboBox4.Name = "ComboBox4"
        Me.ComboBox4.Size = New System.Drawing.Size(285, 37)
        Me.ComboBox4.TabIndex = 52
        '
        'ComboBox3
        '
        Me.ComboBox3.AutoCompleteCustomSource.AddRange(New String() {"Sosnowiec", "Katowice"})
        Me.ComboBox3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.ComboBox3.FormattingEnabled = True
        Me.ComboBox3.Items.AddRange(New Object() {"Sosnowiec", "Katowice"})
        Me.ComboBox3.Location = New System.Drawing.Point(370, 322)
        Me.ComboBox3.Name = "ComboBox3"
        Me.ComboBox3.Size = New System.Drawing.Size(285, 37)
        Me.ComboBox3.TabIndex = 51
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(239, 368)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(100, 29)
        Me.Label12.TabIndex = 50
        Me.Label12.Text = "Rampa*"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(206, 325)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(142, 29)
        Me.Label11.TabIndex = 49
        Me.Label11.Text = "Lokalizacja*"
        '
        'TextBox6
        '
        Me.TextBox6.Location = New System.Drawing.Point(370, 501)
        Me.TextBox6.Multiline = True
        Me.TextBox6.Name = "TextBox6"
        Me.TextBox6.Size = New System.Drawing.Size(285, 86)
        Me.TextBox6.TabIndex = 48
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(370, 604)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(89, 43)
        Me.Button3.TabIndex = 47
        Me.Button3.Text = "Anuluj"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(211, 604)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(137, 43)
        Me.Button1.TabIndex = 46
        Me.Button1.Text = "Aktualizuj"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'TextBox5
        '
        Me.TextBox5.Location = New System.Drawing.Point(370, 456)
        Me.TextBox5.Name = "TextBox5"
        Me.TextBox5.Size = New System.Drawing.Size(59, 35)
        Me.TextBox5.TabIndex = 45
        '
        'TextBox4
        '
        Me.TextBox4.Location = New System.Drawing.Point(370, 415)
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.Size = New System.Drawing.Size(59, 35)
        Me.TextBox4.TabIndex = 44
        '
        'TextBox3
        '
        Me.TextBox3.Location = New System.Drawing.Point(370, 199)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(285, 35)
        Me.TextBox3.TabIndex = 43
        '
        'TextBox2
        '
        Me.TextBox2.Location = New System.Drawing.Point(370, 156)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(285, 35)
        Me.TextBox2.TabIndex = 42
        '
        'MaskedTextBox1
        '
        Me.MaskedTextBox1.Location = New System.Drawing.Point(370, 281)
        Me.MaskedTextBox1.Mask = "00:00"
        Me.MaskedTextBox1.Name = "MaskedTextBox1"
        Me.MaskedTextBox1.Size = New System.Drawing.Size(71, 35)
        Me.MaskedTextBox1.TabIndex = 41
        '
        'DateTimePicker1
        '
        Me.DateTimePicker1.Location = New System.Drawing.Point(370, 240)
        Me.DateTimePicker1.Name = "DateTimePicker1"
        Me.DateTimePicker1.Size = New System.Drawing.Size(165, 35)
        Me.DateTimePicker1.TabIndex = 40
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(370, 105)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(285, 35)
        Me.TextBox1.TabIndex = 39
        '
        'ComboBox2
        '
        Me.ComboBox2.FormattingEnabled = True
        Me.ComboBox2.Location = New System.Drawing.Point(370, 54)
        Me.ComboBox2.Name = "ComboBox2"
        Me.ComboBox2.Size = New System.Drawing.Size(285, 37)
        Me.ComboBox2.TabIndex = 38
        '
        'ComboBox1
        '
        Me.ComboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(370, 6)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(285, 37)
        Me.ComboBox1.TabIndex = 37
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(258, 504)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(81, 29)
        Me.Label10.TabIndex = 36
        Me.Label10.Text = "Uwagi"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(217, 459)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(131, 29)
        Me.Label9.TabIndex = 35
        Me.Label9.Text = "Czas(min)*"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(4, 108)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(344, 29)
        Me.Label8.TabIndex = 34
        Me.Label8.Text = "Nr. Rejestracyjny(przewoźnika)"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(250, 415)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(89, 29)
        Me.Label7.TabIndex = 33
        Me.Label7.Text = "Palety*"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(255, 202)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(84, 29)
        Me.Label6.TabIndex = 32
        Me.Label6.Text = "Nr. Wz"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(171, 159)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(168, 29)
        Me.Label5.TabIndex = 31
        Me.Label5.Text = "Nr. Transportu"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(227, 284)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(112, 29)
        Me.Label4.TabIndex = 30
        Me.Label4.Text = "Godzina*"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(267, 245)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(72, 29)
        Me.Label3.TabIndex = 29
        Me.Label3.Text = "Data*"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(193, 57)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(146, 29)
        Me.Label2.TabIndex = 28
        Me.Label2.Text = "Przewoźnik*"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(255, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(84, 29)
        Me.Label1.TabIndex = 27
        Me.Label1.Text = "Klient*"
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(479, 604)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(89, 43)
        Me.Button2.TabIndex = 53
        Me.Button2.Text = "Usuń"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'FlagPanel
        '
        Me.FlagPanel.Location = New System.Drawing.Point(1, 202)
        Me.FlagPanel.Name = "FlagPanel"
        Me.FlagPanel.Size = New System.Drawing.Size(211, 385)
        Me.FlagPanel.TabIndex = 54
        '
        'UpdateFormLogistyka
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(14.0!, 29.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.ClientSize = New System.Drawing.Size(725, 657)
        Me.Controls.Add(Me.FlagPanel)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.ComboBox4)
        Me.Controls.Add(Me.ComboBox3)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.TextBox6)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.TextBox5)
        Me.Controls.Add(Me.TextBox4)
        Me.Controls.Add(Me.TextBox3)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.MaskedTextBox1)
        Me.Controls.Add(Me.DateTimePicker1)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.ComboBox2)
        Me.Controls.Add(Me.ComboBox1)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(7)
        Me.Name = "UpdateFormLogistyka"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Aktualizacja Zlecenia"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ComboBox4 As ComboBox
    Friend WithEvents ComboBox3 As ComboBox
    Friend WithEvents Label12 As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents TextBox6 As TextBox
    Friend WithEvents Button3 As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents TextBox5 As TextBox
    Friend WithEvents TextBox4 As TextBox
    Friend WithEvents TextBox3 As TextBox
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents MaskedTextBox1 As MaskedTextBox
    Friend WithEvents DateTimePicker1 As DateTimePicker
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents ComboBox2 As ComboBox
    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents Label10 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Button2 As Button
    Friend WithEvents FlagPanel As FlowLayoutPanel
End Class
