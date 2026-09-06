namespace RenDaClicker {
	partial class ProfileEditForm {
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
			this.labelName = new Label();
			this.textName = new TextBox();
			this.groupButton = new GroupBox();
			this.radioLeft = new RadioButton();
			this.radioRight = new RadioButton();
			this.groupMode = new GroupBox();
			this.radioHold = new RadioButton();
			this.radioToggle = new RadioButton();
			this.labelInterval = new Label();
			this.numInterval = new NumericUpDown();
			this.labelMs = new Label();
			this.labelHoldTime = new Label();
			this.numHold = new NumericUpDown();
			this.labelHoldMs = new Label();
			this.labelHotkey = new Label();
			this.textHotkey = new TextBox();
			this.buttonCapture = new Button();
			this.checkLockPos = new CheckBox();
			this.buttonOk = new Button();
			this.buttonCancel = new Button();
			this.groupButton.SuspendLayout();
			this.groupMode.SuspendLayout();
			( (System.ComponentModel.ISupportInitialize)this.numInterval ).BeginInit();
			( (System.ComponentModel.ISupportInitialize)this.numHold ).BeginInit();
			this.SuspendLayout();
			//
			// labelName
			//
			this.labelName.AutoSize = true;
			this.labelName.Location = new Point( 12, 15 );
			this.labelName.Name = "labelName";
			this.labelName.Size = new Size( 35, 15 );
			this.labelName.TabIndex = 0;
			this.labelName.Text = "名前:";
			//
			// textName
			//
			this.textName.Location = new Point( 100, 12 );
			this.textName.Name = "textName";
			this.textName.Size = new Size( 205, 23 );
			this.textName.TabIndex = 1;
			//
			// groupButton
			//
			this.groupButton.Controls.Add( this.radioLeft );
			this.groupButton.Controls.Add( this.radioRight );
			this.groupButton.Location = new Point( 12, 45 );
			this.groupButton.Name = "groupButton";
			this.groupButton.Size = new Size( 130, 80 );
			this.groupButton.TabIndex = 2;
			this.groupButton.TabStop = false;
			this.groupButton.Text = "クリックボタン";
			//
			// radioLeft
			//
			this.radioLeft.Location = new Point( 15, 22 );
			this.radioLeft.Name = "radioLeft";
			this.radioLeft.Size = new Size( 100, 24 );
			this.radioLeft.TabIndex = 0;
			this.radioLeft.TabStop = true;
			this.radioLeft.Text = "左クリック";
			this.radioLeft.UseVisualStyleBackColor = true;
			//
			// radioRight
			//
			this.radioRight.Location = new Point( 15, 48 );
			this.radioRight.Name = "radioRight";
			this.radioRight.Size = new Size( 100, 24 );
			this.radioRight.TabIndex = 1;
			this.radioRight.Text = "右クリック";
			this.radioRight.UseVisualStyleBackColor = true;
			//
			// groupMode
			//
			this.groupMode.Controls.Add( this.radioHold );
			this.groupMode.Controls.Add( this.radioToggle );
			this.groupMode.Location = new Point( 155, 45 );
			this.groupMode.Name = "groupMode";
			this.groupMode.Size = new Size( 150, 80 );
			this.groupMode.TabIndex = 3;
			this.groupMode.TabStop = false;
			this.groupMode.Text = "動作モード";
			//
			// radioHold
			//
			this.radioHold.Location = new Point( 15, 22 );
			this.radioHold.Name = "radioHold";
			this.radioHold.Size = new Size( 125, 24 );
			this.radioHold.TabIndex = 0;
			this.radioHold.TabStop = true;
			this.radioHold.Text = "押している間";
			this.radioHold.UseVisualStyleBackColor = true;
			//
			// radioToggle
			//
			this.radioToggle.Location = new Point( 15, 48 );
			this.radioToggle.Name = "radioToggle";
			this.radioToggle.Size = new Size( 125, 24 );
			this.radioToggle.TabIndex = 1;
			this.radioToggle.Text = "トグル";
			this.radioToggle.UseVisualStyleBackColor = true;
			//
			// labelInterval
			//
			this.labelInterval.AutoSize = true;
			this.labelInterval.Location = new Point( 12, 138 );
			this.labelInterval.Name = "labelInterval";
			this.labelInterval.Size = new Size( 68, 15 );
			this.labelInterval.TabIndex = 4;
			this.labelInterval.Text = "クリック間隔:";
			//
			// numInterval
			//
			this.numInterval.Location = new Point( 100, 135 );
			this.numInterval.Maximum = new decimal( new int[] { 60000, 0, 0, 0 } );
			this.numInterval.Minimum = new decimal( new int[] { 10, 0, 0, 0 } );
			this.numInterval.Name = "numInterval";
			this.numInterval.Size = new Size( 80, 23 );
			this.numInterval.TabIndex = 5;
			this.numInterval.Value = new decimal( new int[] { 100, 0, 0, 0 } );
			//
			// labelMs
			//
			this.labelMs.AutoSize = true;
			this.labelMs.Location = new Point( 185, 138 );
			this.labelMs.Name = "labelMs";
			this.labelMs.Size = new Size( 22, 15 );
			this.labelMs.TabIndex = 6;
			this.labelMs.Text = "ms";
			//
			// labelHoldTime
			//
			this.labelHoldTime.AutoSize = true;
			this.labelHoldTime.Location = new Point( 12, 166 );
			this.labelHoldTime.Name = "labelHoldTime";
			this.labelHoldTime.Size = new Size( 68, 15 );
			this.labelHoldTime.TabIndex = 7;
			this.labelHoldTime.Text = "押下時間:";
			//
			// numHold
			//
			this.numHold.Location = new Point( 100, 163 );
			this.numHold.Maximum = new decimal( new int[] { 60000, 0, 0, 0 } );
			this.numHold.Minimum = new decimal( new int[] { 1, 0, 0, 0 } );
			this.numHold.Name = "numHold";
			this.numHold.Size = new Size( 80, 23 );
			this.numHold.TabIndex = 8;
			this.numHold.Value = new decimal( new int[] { 50, 0, 0, 0 } );
			//
			// labelHoldMs
			//
			this.labelHoldMs.AutoSize = true;
			this.labelHoldMs.Location = new Point( 185, 166 );
			this.labelHoldMs.Name = "labelHoldMs";
			this.labelHoldMs.Size = new Size( 22, 15 );
			this.labelHoldMs.TabIndex = 9;
			this.labelHoldMs.Text = "ms";
			//
			// labelHotkey
			//
			this.labelHotkey.AutoSize = true;
			this.labelHotkey.Location = new Point( 12, 194 );
			this.labelHotkey.Name = "labelHotkey";
			this.labelHotkey.Size = new Size( 53, 15 );
			this.labelHotkey.TabIndex = 10;
			this.labelHotkey.Text = "ホットキー:";
			//
			// textHotkey
			//
			this.textHotkey.Location = new Point( 100, 191 );
			this.textHotkey.Name = "textHotkey";
			this.textHotkey.ReadOnly = true;
			this.textHotkey.Size = new Size( 120, 23 );
			this.textHotkey.TabIndex = 11;
			this.textHotkey.TextAlign = HorizontalAlignment.Center;
			this.textHotkey.KeyDown += this.TextHotkey_KeyDown;
			this.textHotkey.KeyUp += this.TextHotkey_KeyUp;
			this.textHotkey.Leave += this.TextHotkey_Leave;
			this.textHotkey.MouseDown += this.TextHotkey_MouseDown;
			this.textHotkey.PreviewKeyDown += this.TextHotkey_PreviewKeyDown;
			//
			// buttonCapture
			//
			this.buttonCapture.Location = new Point( 230, 190 );
			this.buttonCapture.Name = "buttonCapture";
			this.buttonCapture.Size = new Size( 75, 25 );
			this.buttonCapture.TabIndex = 12;
			this.buttonCapture.Text = "設定";
			this.buttonCapture.UseVisualStyleBackColor = true;
			this.buttonCapture.Click += this.ButtonCapture_Click;
			//
			// checkLockPos
			//
			this.checkLockPos.AutoSize = true;
			this.checkLockPos.Location = new Point( 12, 222 );
			this.checkLockPos.Name = "checkLockPos";
			this.checkLockPos.Size = new Size( 208, 19 );
			this.checkLockPos.TabIndex = 13;
			this.checkLockPos.Text = "連打開始時のカーソル位置を保持する";
			this.checkLockPos.UseVisualStyleBackColor = true;
			//
			// buttonOk
			//
			this.buttonOk.Location = new Point( 150, 255 );
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new Size( 75, 27 );
			this.buttonOk.TabIndex = 14;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.Click += this.ButtonOk_Click;
			//
			// buttonCancel
			//
			this.buttonCancel.DialogResult = DialogResult.Cancel;
			this.buttonCancel.Location = new Point( 230, 255 );
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new Size( 75, 27 );
			this.buttonCancel.TabIndex = 15;
			this.buttonCancel.Text = "キャンセル";
			this.buttonCancel.UseVisualStyleBackColor = true;
			//
			// ProfileEditForm
			//
			this.AutoScaleDimensions = new SizeF( 7F, 15F );
			this.AutoScaleMode = AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new Size( 320, 295 );
			this.Controls.Add( this.labelName );
			this.Controls.Add( this.textName );
			this.Controls.Add( this.groupButton );
			this.Controls.Add( this.groupMode );
			this.Controls.Add( this.labelInterval );
			this.Controls.Add( this.numInterval );
			this.Controls.Add( this.labelMs );
			this.Controls.Add( this.labelHoldTime );
			this.Controls.Add( this.numHold );
			this.Controls.Add( this.labelHoldMs );
			this.Controls.Add( this.labelHotkey );
			this.Controls.Add( this.textHotkey );
			this.Controls.Add( this.buttonCapture );
			this.Controls.Add( this.checkLockPos );
			this.Controls.Add( this.buttonOk );
			this.Controls.Add( this.buttonCancel );
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProfileEditForm";
			this.StartPosition = FormStartPosition.CenterParent;
			this.Text = "プロファイルの設定";
			this.groupButton.ResumeLayout( false );
			this.groupMode.ResumeLayout( false );
			( (System.ComponentModel.ISupportInitialize)this.numInterval ).EndInit();
			( (System.ComponentModel.ISupportInitialize)this.numHold ).EndInit();
			this.ResumeLayout( false );
			this.PerformLayout();
		}

		#endregion

		private Label labelName;
		private TextBox textName;
		private GroupBox groupButton;
		private RadioButton radioLeft;
		private RadioButton radioRight;
		private GroupBox groupMode;
		private RadioButton radioHold;
		private RadioButton radioToggle;
		private Label labelInterval;
		private NumericUpDown numInterval;
		private Label labelMs;
		private Label labelHoldTime;
		private NumericUpDown numHold;
		private Label labelHoldMs;
		private Label labelHotkey;
		private TextBox textHotkey;
		private Button buttonCapture;
		private CheckBox checkLockPos;
		private Button buttonOk;
		private Button buttonCancel;
	}
}
