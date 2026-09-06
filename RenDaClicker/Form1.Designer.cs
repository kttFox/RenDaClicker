namespace RenDaClicker {
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
			this.components = new System.ComponentModel.Container();
			this.toolTip = new ToolTip( this.components );
			this.panelMarkPreviewLeft = new Panel();
			this.panelMarkPreviewRight = new Panel();
			this.listProfiles = new ListViewEx();
			this.columnName = new ColumnHeader();
			this.columnHotkey = new ColumnHeader();
			this.columnButton = new ColumnHeader();
			this.columnMode = new ColumnHeader();
			this.columnInterval = new ColumnHeader();
			this.columnLockPos = new ColumnHeader();
			this.buttonAdd = new Button();
			this.buttonEdit = new Button();
			this.buttonDelete = new Button();
			this.buttonUp = new Button();
			this.buttonDown = new Button();
			this.checkIndicator = new CheckBox();
			this.checkTargetWindow = new CheckBox();
			this.comboWindows = new ComboBox();
			this.buttonRefreshWindows = new Button();
			this.labelTargetNote = new Label();
			this.buttonSuspend = new Button();
			this.SuspendLayout();
			// 
			// panelMarkPreviewLeft
			// 
			this.panelMarkPreviewLeft.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Left ;
			this.panelMarkPreviewLeft.Location = new Point( 239, 210 );
			this.panelMarkPreviewLeft.Name = "panelMarkPreviewLeft";
			this.panelMarkPreviewLeft.Size = new Size( 16, 16 );
			this.panelMarkPreviewLeft.TabIndex = 5;
			this.toolTip.SetToolTip( this.panelMarkPreviewLeft, "左クリックの連打中に表示される目印です" );
			// 
			// panelMarkPreviewRight
			// 
			this.panelMarkPreviewRight.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Left ;
			this.panelMarkPreviewRight.Location = new Point( 256, 210 );
			this.panelMarkPreviewRight.Name = "panelMarkPreviewRight";
			this.panelMarkPreviewRight.Size = new Size( 16, 16 );
			this.panelMarkPreviewRight.TabIndex = 5;
			this.toolTip.SetToolTip( this.panelMarkPreviewRight, "右クリックの連打中に表示される目印です" );
			// 
			// listProfiles
			// 
			this.listProfiles.Anchor =    AnchorStyles.Top  |  AnchorStyles.Bottom   |  AnchorStyles.Left   |  AnchorStyles.Right ;
			this.listProfiles.Columns.AddRange( new ColumnHeader[] { this.columnName, this.columnHotkey, this.columnButton, this.columnMode, this.columnInterval, this.columnLockPos } );
			this.listProfiles.FullRowSelect = true;
			this.listProfiles.Location = new Point( 12, 12 );
			this.listProfiles.MultiSelect = false;
			this.listProfiles.Name = "listProfiles";
			this.listProfiles.Size = new Size( 370, 157 );
			this.listProfiles.TabIndex = 0;
			this.listProfiles.UseCompatibleStateImageBehavior = false;
			this.listProfiles.View = View.Details;
			// 
			// columnName
			// 
			this.columnName.Text = "名前";
			this.columnName.Width = 50;
			// 
			// columnHotkey
			// 
			this.columnHotkey.Text = "ホットキー";
			this.columnHotkey.Width = 95;
			// 
			// columnButton
			// 
			this.columnButton.Text = "ボタン";
			this.columnButton.TextAlign = HorizontalAlignment.Center;
			this.columnButton.Width = 45;
			// 
			// columnMode
			// 
			this.columnMode.Text = "モード";
			this.columnMode.TextAlign = HorizontalAlignment.Center;
			this.columnMode.Width = 55;
			// 
			// columnInterval
			// 
			this.columnInterval.Text = "間隔";
			this.columnInterval.TextAlign = HorizontalAlignment.Right;
			this.columnInterval.Width = 55;
			// 
			// columnLockPos
			// 
			this.columnLockPos.Text = "位置";
			this.columnLockPos.TextAlign = HorizontalAlignment.Center;
			this.columnLockPos.Width = 45;
			// 
			// buttonAdd
			// 
			this.buttonAdd.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Left ;
			this.buttonAdd.Location = new Point( 12, 175 );
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new Size( 75, 25 );
			this.buttonAdd.TabIndex = 1;
			this.buttonAdd.Text = "追加...";
			this.buttonAdd.UseVisualStyleBackColor = true;
			// 
			// buttonEdit
			// 
			this.buttonEdit.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Left ;
			this.buttonEdit.Location = new Point( 93, 175 );
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.Size = new Size( 75, 25 );
			this.buttonEdit.TabIndex = 2;
			this.buttonEdit.Text = "変更...";
			this.buttonEdit.UseVisualStyleBackColor = true;
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Left ;
			this.buttonDelete.Location = new Point( 174, 175 );
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new Size( 75, 25 );
			this.buttonDelete.TabIndex = 3;
			this.buttonDelete.Text = "削除...";
			this.buttonDelete.UseVisualStyleBackColor = true;
			//
			// buttonUp
			//
			this.buttonUp.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Right ;
			this.buttonUp.Location = new Point( 312, 175 );
			this.buttonUp.Name = "buttonUp";
			this.buttonUp.Size = new Size( 33, 25 );
			this.buttonUp.TabIndex = 4;
			this.buttonUp.Text = "▲";
			this.buttonUp.UseVisualStyleBackColor = true;
			this.toolTip.SetToolTip( this.buttonUp, "選択中のプロファイルを上へ移動します(上にあるほど優先)" );
			//
			// buttonDown
			//
			this.buttonDown.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Right ;
			this.buttonDown.Location = new Point( 349, 175 );
			this.buttonDown.Name = "buttonDown";
			this.buttonDown.Size = new Size( 33, 25 );
			this.buttonDown.TabIndex = 5;
			this.buttonDown.Text = "▼";
			this.buttonDown.UseVisualStyleBackColor = true;
			this.toolTip.SetToolTip( this.buttonDown, "選択中のプロファイルを下へ移動します" );
			// 
			// checkIndicator
			// 
			this.checkIndicator.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Left ;
			this.checkIndicator.AutoSize = true;
			this.checkIndicator.Checked = true;
			this.checkIndicator.CheckState = CheckState.Checked;
			this.checkIndicator.Location = new Point( 12, 212 );
			this.checkIndicator.Name = "checkIndicator";
			this.checkIndicator.Size = new Size( 221, 19 );
			this.checkIndicator.TabIndex = 4;
			this.checkIndicator.Text = "連打中にカーソルの近くに目印を表示する";
			this.checkIndicator.UseVisualStyleBackColor = true;
			// 
			// checkTargetWindow
			// 
			this.checkTargetWindow.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Left ;
			this.checkTargetWindow.AutoSize = true;
			this.checkTargetWindow.Location = new Point( 12, 237 );
			this.checkTargetWindow.Name = "checkTargetWindow";
			this.checkTargetWindow.Size = new Size( 197, 19 );
			this.checkTargetWindow.TabIndex = 5;
			this.checkTargetWindow.Text = "指定したウインドウ上でのみ連打する";
			this.checkTargetWindow.UseVisualStyleBackColor = true;
			// 
			// comboWindows
			// 
			this.comboWindows.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Left ;
			this.comboWindows.DropDownStyle = ComboBoxStyle.DropDownList;
			this.comboWindows.Location = new Point( 28, 260 );
			this.comboWindows.Name = "comboWindows";
			this.comboWindows.Size = new Size( 258, 23 );
			this.comboWindows.TabIndex = 6;
			// 
			// buttonRefreshWindows
			// 
			this.buttonRefreshWindows.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Left ;
			this.buttonRefreshWindows.Location = new Point( 292, 259 );
			this.buttonRefreshWindows.Name = "buttonRefreshWindows";
			this.buttonRefreshWindows.Size = new Size( 90, 25 );
			this.buttonRefreshWindows.TabIndex = 7;
			this.buttonRefreshWindows.Text = "一覧更新";
			this.buttonRefreshWindows.UseVisualStyleBackColor = true;
			// 
			// labelTargetNote
			// 
			this.labelTargetNote.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Left ;
			this.labelTargetNote.AutoSize = true;
			this.labelTargetNote.ForeColor = SystemColors.GrayText;
			this.labelTargetNote.Location = new Point( 28, 290 );
			this.labelTargetNote.Name = "labelTargetNote";
			this.labelTargetNote.Size = new Size( 252, 30 );
			this.labelTargetNote.TabIndex = 8;
			this.labelTargetNote.Text = "※クリック先のウインドウが変わると緊急停止します。\r\n    再開するにはホットキーをもう一度押してください。";
			// 
			// buttonSuspend
			// 
			this.buttonSuspend.Anchor =  AnchorStyles.Bottom  |  AnchorStyles.Left ;
			this.buttonSuspend.Location = new Point( 12, 335 );
			this.buttonSuspend.Name = "buttonSuspend";
			this.buttonSuspend.Size = new Size( 370, 42 );
			this.buttonSuspend.TabIndex = 9;
			this.buttonSuspend.Text = "ホットキー受付を一時中止";
			this.buttonSuspend.UseVisualStyleBackColor = true;
			this.buttonSuspend.Click +=  this.ButtonSuspend_Click ;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new SizeF( 96F, 96F );
			this.AutoScaleMode = AutoScaleMode.Dpi;
			this.ClientSize = new Size( 394, 392 );
			this.Controls.Add( this.listProfiles );
			this.Controls.Add( this.buttonAdd );
			this.Controls.Add( this.buttonEdit );
			this.Controls.Add( this.buttonDelete );
			this.Controls.Add( this.buttonUp );
			this.Controls.Add( this.buttonDown );
			this.Controls.Add( this.checkIndicator );
			this.Controls.Add( this.panelMarkPreviewRight );
			this.Controls.Add( this.panelMarkPreviewLeft );
			this.Controls.Add( this.checkTargetWindow );
			this.Controls.Add( this.comboWindows );
			this.Controls.Add( this.buttonRefreshWindows );
			this.Controls.Add( this.labelTargetNote );
			this.Controls.Add( this.buttonSuspend );
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "RenDaClicker";
			this.ResumeLayout( false );
			this.PerformLayout();
		}

		#endregion

		private ListViewEx listProfiles;
		private ColumnHeader columnName;
		private ColumnHeader columnHotkey;
		private ColumnHeader columnButton;
		private ColumnHeader columnMode;
		private ColumnHeader columnInterval;
		private ColumnHeader columnLockPos;
		private Button buttonAdd;
		private Button buttonEdit;
		private Button buttonDelete;
		private Button buttonUp;
		private Button buttonDown;
		private CheckBox checkIndicator;
		private ToolTip toolTip;
		private Panel panelMarkPreviewLeft;
		private CheckBox checkTargetWindow;
		private ComboBox comboWindows;
		private Button buttonRefreshWindows;
		private Label labelTargetNote;
		private Panel panelMarkPreviewRight;
		private Button buttonSuspend;
	}
}
