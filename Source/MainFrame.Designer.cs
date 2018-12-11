namespace Dropoin {
	partial class MainFrame {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.label2 = new System.Windows.Forms.Label();
			this.m_picReady = new System.Windows.Forms.PictureBox();
			this.m_pictureBox1 = new System.Windows.Forms.PictureBox();
			this.contextMenuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.m_picReady)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(113, 26);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(112, 22);
			this.toolStripMenuItem1.Text = "終了(&E)";
			this.toolStripMenuItem1.Click += new System.EventHandler(this.OnMenuExit);
			// 
			// label2
			// 
			this.label2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label2.ForeColor = System.Drawing.Color.Black;
			this.label2.Location = new System.Drawing.Point(0, 71);
			this.label2.Margin = new System.Windows.Forms.Padding(0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(109, 19);
			this.label2.TabIndex = 2;
			this.label2.Text = "label2";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label2.Click += new System.EventHandler(this.label2_Click);
			// 
			// m_picReady
			// 
			this.m_picReady.Image = global::Dropoin.Properties.Resources.arrow_icon295;
			this.m_picReady.InitialImage = null;
			this.m_picReady.Location = new System.Drawing.Point(32, 4);
			this.m_picReady.Name = "m_picReady";
			this.m_picReady.Size = new System.Drawing.Size(44, 60);
			this.m_picReady.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.m_picReady.TabIndex = 3;
			this.m_picReady.TabStop = false;
			this.m_picReady.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
			this.m_picReady.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
			// 
			// m_pictureBox1
			// 
			this.m_pictureBox1.Image = global::Dropoin.Properties.Resources.ajax_loader;
			this.m_pictureBox1.Location = new System.Drawing.Point(21, 0);
			this.m_pictureBox1.Name = "m_pictureBox1";
			this.m_pictureBox1.Size = new System.Drawing.Size(64, 64);
			this.m_pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.m_pictureBox1.TabIndex = 4;
			this.m_pictureBox1.TabStop = false;
			// 
			// MainFrame
			// 
			this.AllowDrop = true;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(109, 90);
			this.ContextMenuStrip = this.contextMenuStrip1;
			this.ControlBox = false;
			this.Controls.Add(this.m_picReady);
			this.Controls.Add(this.m_pictureBox1);
			this.Controls.Add(this.label2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "MainFrame";
			this.Text = "ファイルをドロップ";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
			this.contextMenuStrip1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.m_picReady)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PictureBox m_picReady;
		private System.Windows.Forms.PictureBox m_pictureBox1;
	}
}