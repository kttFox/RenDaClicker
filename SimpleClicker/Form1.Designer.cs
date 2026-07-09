namespace SimpleClicker {
	partial class Form1 {
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing ) {
			if( disposing && ( components != null ) ) {
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.groupButton = new GroupBox();
			this.radioLeft = new RadioButton();
			this.radioRight = new RadioButton();
			this.groupMode = new GroupBox();
			this.radioHold = new RadioButton();
			this.radioToggle = new RadioButton();
			this.labelInterval = new Label();
			this.numInterval = new NumericUpDown();
			this.labelMs = new Label();
			this.labelHotkey = new Label();
			this.textHotkey = new TextBox();
			this.buttonCapture = new Button();
			this.checkLockPos = new CheckBox();
			this.checkIndicator = new CheckBox();
			this.labelEmergencyNote = new Label();
			this.buttonSuspend = new Button();
			this.labelStatus = new Label();
			this.buttonStart = new Button();
			this.buttonStop = new Button();
			this.groupButton.SuspendLayout();
			this.groupMode.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize)this.numInterval ).BeginInit();
			this.SuspendLayout();
			// 
			// groupButton
			// 
			this.groupButton.Controls.Add( this.radioLeft );
			this.groupButton.Controls.Add( this.radioRight );
			this.groupButton.Location = new Point( 12, 12 );
			this.groupButton.Name = "groupButton";
			this.groupButton.Size = new Size( 130, 80 );
			this.groupButton.TabIndex = 0;
			this.groupButton.TabStop = false;
			this.groupButton.Text = "クリックボタン";
			// 
			// radioLeft
			// 
			this.radioLeft.Checked = true;
			this.radioLeft.Location = new Point( 15, 22 );
			this.radioLeft.Name = "radioLeft";
			this.radioLeft.Size = new Size( 100, 24 );
			this.radioLeft.TabIndex = 0;
			this.radioLeft.TabStop = true;
			this.radioLeft.Text = "左クリック";
			// 
			// radioRight
			// 
			this.radioRight.Location = new Point( 15, 48 );
			this.radioRight.Name = "radioRight";
			this.radioRight.Size = new Size( 100, 24 );
			this.radioRight.TabIndex = 1;
			this.radioRight.Text = "右クリック";
			// 
			// groupMode
			// 
			this.groupMode.Controls.Add( this.radioHold );
			this.groupMode.Controls.Add( this.radioToggle );
			this.groupMode.Location = new Point( 155, 12 );
			this.groupMode.Name = "groupMode";
			this.groupMode.Size = new Size( 150, 80 );
			this.groupMode.TabIndex = 1;
			this.groupMode.TabStop = false;
			this.groupMode.Text = "動作モード";
			// 
			// radioHold
			// 
			this.radioHold.Checked = true;
			this.radioHold.Location = new Point( 15, 22 );
			this.radioHold.Name = "radioHold";
			this.radioHold.Size = new Size( 125, 24 );
			this.radioHold.TabIndex = 0;
			this.radioHold.TabStop = true;
			this.radioHold.Text = "押している間";
			this.radioHold.CheckedChanged +=  this.RadioMode_CheckedChanged ;
			// 
			// radioToggle
			// 
			this.radioToggle.Location = new Point( 15, 48 );
			this.radioToggle.Name = "radioToggle";
			this.radioToggle.Size = new Size( 125, 24 );
			this.radioToggle.TabIndex = 1;
			this.radioToggle.Text = "トグル";
			this.radioToggle.CheckedChanged +=  this.RadioMode_CheckedChanged ;
			// 
			// labelInterval
			// 
			this.labelInterval.AutoSize = true;
			this.labelInterval.Location = new Point( 12, 108 );
			this.labelInterval.Name = "labelInterval";
			this.labelInterval.Size = new Size( 68, 15 );
			this.labelInterval.TabIndex = 2;
			this.labelInterval.Text = "クリック間隔:";
			// 
			// numInterval
			// 
			this.numInterval.Location = new Point( 100, 105 );
			this.numInterval.Maximum = new decimal( new int[] { 60000, 0, 0, 0 } );
			this.numInterval.Minimum = new decimal( new int[] { 10, 0, 0, 0 } );
			this.numInterval.Name = "numInterval";
			this.numInterval.Size = new Size( 80, 23 );
			this.numInterval.TabIndex = 3;
			this.numInterval.Value = new decimal( new int[] { 100, 0, 0, 0 } );
			// 
			// labelMs
			// 
			this.labelMs.AutoSize = true;
			this.labelMs.Location = new Point( 185, 108 );
			this.labelMs.Name = "labelMs";
			this.labelMs.Size = new Size( 22, 15 );
			this.labelMs.TabIndex = 4;
			this.labelMs.Text = "ms";
			// 
			// labelHotkey
			// 
			this.labelHotkey.AutoSize = true;
			this.labelHotkey.Location = new Point( 12, 226 );
			this.labelHotkey.Name = "labelHotkey";
			this.labelHotkey.Size = new Size( 53, 15 );
			this.labelHotkey.TabIndex = 5;
			this.labelHotkey.Text = "ホットキー:";
			// 
			// textHotkey
			// 
			this.textHotkey.Location = new Point( 100, 223 );
			this.textHotkey.Name = "textHotkey";
			this.textHotkey.ReadOnly = true;
			this.textHotkey.Size = new Size( 120, 23 );
			this.textHotkey.TabIndex = 6;
			this.textHotkey.TextAlign = HorizontalAlignment.Center;
			this.textHotkey.KeyDown +=  this.TextHotkey_KeyDown ;
			this.textHotkey.Leave +=  this.TextHotkey_Leave ;
			this.textHotkey.MouseDown +=  this.TextHotkey_MouseDown ;
			this.textHotkey.PreviewKeyDown +=  this.TextHotkey_PreviewKeyDown ;
			// 
			// buttonCapture
			// 
			this.buttonCapture.Location = new Point( 230, 222 );
			this.buttonCapture.Name = "buttonCapture";
			this.buttonCapture.Size = new Size( 75, 25 );
			this.buttonCapture.TabIndex = 10;
			this.buttonCapture.Text = "設定";
			this.buttonCapture.UseVisualStyleBackColor = true;
			this.buttonCapture.Click +=  this.ButtonCapture_Click ;
			//
			// buttonSuspend
			//
			this.buttonSuspend.Location = new Point( 12, 253 );
			this.buttonSuspend.Name = "buttonSuspend";
			this.buttonSuspend.Size = new Size( 293, 25 );
			this.buttonSuspend.TabIndex = 14;
			this.buttonSuspend.Text = "ホットキー受付を一時中止";
			this.buttonSuspend.UseVisualStyleBackColor = true;
			this.buttonSuspend.Click += this.ButtonSuspend_Click;
			// 
			// checkLockPos
			// 
			this.checkLockPos.AutoSize = true;
			this.checkLockPos.Location = new Point( 12, 134 );
			this.checkLockPos.Name = "checkLockPos";
			this.checkLockPos.Size = new Size( 208, 19 );
			this.checkLockPos.TabIndex = 11;
			this.checkLockPos.Text = "連打開始時のカーソル位置を保持する";
			this.checkLockPos.UseVisualStyleBackColor = true;
			// 
			// checkIndicator
			// 
			this.checkIndicator.AutoSize = true;
			this.checkIndicator.Checked = true;
			this.checkIndicator.CheckState = CheckState.Checked;
			this.checkIndicator.Location = new Point( 12, 192 );
			this.checkIndicator.Name = "checkIndicator";
			this.checkIndicator.Size = new Size( 221, 19 );
			this.checkIndicator.TabIndex = 12;
			this.checkIndicator.Text = "連打中にカーソルの近くに目印を表示する";
			this.checkIndicator.UseVisualStyleBackColor = true;
			//
			// labelEmergencyNote
			//
			this.labelEmergencyNote.AutoSize = true;
			this.labelEmergencyNote.ForeColor = SystemColors.GrayText;
			this.labelEmergencyNote.Location = new Point( 28, 155 );
			this.labelEmergencyNote.Name = "labelEmergencyNote";
			this.labelEmergencyNote.Size = new Size( 280, 30 );
			this.labelEmergencyNote.TabIndex = 13;
			this.labelEmergencyNote.Text = "※クリック先のウインドウが変わると緊急停止します。\r\n    再開するには一度停止操作を行ってください。";
			//
			// labelStatus
			// 
			this.labelStatus.AutoSize = true;
			this.labelStatus.Font = new Font( "Yu Gothic UI", 9F, FontStyle.Bold );
			this.labelStatus.Location = new Point( 12, 296 );
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new Size( 61, 15 );
			this.labelStatus.TabIndex = 7;
			this.labelStatus.Text = "状態: 停止";
			// 
			// buttonStart
			// 
			this.buttonStart.Location = new Point( 150, 290 );
			this.buttonStart.Name = "buttonStart";
			this.buttonStart.Size = new Size( 75, 27 );
			this.buttonStart.TabIndex = 8;
			this.buttonStart.Text = "開始";
			this.buttonStart.UseVisualStyleBackColor = true;
			this.buttonStart.Click +=  this.ButtonStart_Click ;
			// 
			// buttonStop
			// 
			this.buttonStop.Location = new Point( 230, 290 );
			this.buttonStop.Name = "buttonStop";
			this.buttonStop.Size = new Size( 75, 27 );
			this.buttonStop.TabIndex = 9;
			this.buttonStop.Text = "停止";
			this.buttonStop.UseVisualStyleBackColor = true;
			this.buttonStop.Click +=  this.ButtonStop_Click ;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new SizeF( 7F, 15F );
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size( 320, 328 );
			this.Controls.Add( this.groupButton );
			this.Controls.Add( this.groupMode );
			this.Controls.Add( this.labelInterval );
			this.Controls.Add( this.numInterval );
			this.Controls.Add( this.labelMs );
			this.Controls.Add( this.labelHotkey );
			this.Controls.Add( this.textHotkey );
			this.Controls.Add( this.buttonCapture );
			this.Controls.Add( this.checkLockPos );
			this.Controls.Add( this.checkIndicator );
			this.Controls.Add( this.labelEmergencyNote );
			this.Controls.Add( this.buttonSuspend );
			this.Controls.Add( this.labelStatus );
			this.Controls.Add( this.buttonStart );
			this.Controls.Add( this.buttonStop );
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "SimpleClicker";
			this.groupButton.ResumeLayout( false );
			this.groupMode.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize)this.numInterval ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();
		}

		#endregion

		private GroupBox groupButton;
		private RadioButton radioLeft;
		private RadioButton radioRight;
		private GroupBox groupMode;
		private RadioButton radioHold;
		private RadioButton radioToggle;
		private Label labelInterval;
		private NumericUpDown numInterval;
		private Label labelMs;
		private Label labelHotkey;
		private TextBox textHotkey;
		private Button buttonCapture;
		private CheckBox checkLockPos;
		private CheckBox checkIndicator;
		private Label labelEmergencyNote;
		private Button buttonSuspend;
		private Label labelStatus;
		private Button buttonStart;
		private Button buttonStop;
	}
}
