namespace Plugin.MailMessageQueue.UI
{
	partial class ConfigCtrl
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.splitMain = new System.Windows.Forms.SplitContainer();
			this.lvTimers = new System.Windows.Forms.ListView();
			this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.lblNoPlugins = new System.Windows.Forms.Label();
			this.pgData = new System.Windows.Forms.PropertyGrid();
			this.tsbnRemove = new System.Windows.Forms.ToolStripButton();
			this.tsbnExecute = new System.Windows.Forms.ToolStripButton();
			this.tsddlAdd = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
			this.splitMain.Panel1.SuspendLayout();
			this.splitMain.Panel2.SuspendLayout();
			this.splitMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsddlAdd,
            this.tsbnRemove,
            this.toolStripSeparator1,
            this.tsbnExecute});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
			this.toolStrip1.Size = new System.Drawing.Size(225, 28);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
			// 
			// splitMain
			// 
			this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitMain.Location = new System.Drawing.Point(0, 28);
			this.splitMain.Name = "splitMain";
			this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitMain.Panel1
			// 
			this.splitMain.Panel1.Controls.Add(this.lvTimers);
			this.splitMain.Panel1.Controls.Add(this.lblNoPlugins);
			// 
			// splitMain.Panel2
			// 
			this.splitMain.Panel2.Controls.Add(this.pgData);
			this.splitMain.Size = new System.Drawing.Size(225, 221);
			this.splitMain.SplitterDistance = 98;
			this.splitMain.SplitterWidth = 5;
			this.splitMain.TabIndex = 1;
			// 
			// lvTimers
			// 
			this.lvTimers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
			this.lvTimers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvTimers.FullRowSelect = true;
			this.lvTimers.Location = new System.Drawing.Point(0, 22);
			this.lvTimers.Name = "lvTimers";
			this.lvTimers.Size = new System.Drawing.Size(225, 76);
			this.lvTimers.TabIndex = 0;
			this.lvTimers.UseCompatibleStateImageBehavior = false;
			this.lvTimers.View = System.Windows.Forms.View.Details;
			this.lvTimers.SelectedIndexChanged += new System.EventHandler(this.lvTimers_SelectedIndexChanged);
			// 
			// colName
			// 
			this.colName.Text = "Name";
			// 
			// lblNoPlugins
			// 
			this.lblNoPlugins.BackColor = System.Drawing.SystemColors.Info;
			this.lblNoPlugins.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblNoPlugins.Location = new System.Drawing.Point(0, 0);
			this.lblNoPlugins.Name = "lblNoPlugins";
			this.lblNoPlugins.Size = new System.Drawing.Size(225, 22);
			this.lblNoPlugins.TabIndex = 1;
			this.lblNoPlugins.Visible = false;
			// 
			// pgData
			// 
			this.pgData.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgData.Location = new System.Drawing.Point(0, 0);
			this.pgData.Name = "pgData";
			this.pgData.Size = new System.Drawing.Size(225, 118);
			this.pgData.TabIndex = 0;
			this.pgData.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgData_PropertyValueChanged);
			// 
			// tsbnRemove
			// 
			this.tsbnRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnRemove.Enabled = false;
			this.tsbnRemove.Image = global::Plugin.MailMessageQueue.Properties.Resources.iconDelete;
			this.tsbnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnRemove.Name = "tsbnRemove";
			this.tsbnRemove.Size = new System.Drawing.Size(24, 25);
			this.tsbnRemove.ToolTipText = "Remove item...";
			this.tsbnRemove.Click += new System.EventHandler(this.tsbnRemove_Click);
			// 
			// tsbnExecute
			// 
			this.tsbnExecute.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnExecute.Enabled = false;
			this.tsbnExecute.Image = global::Plugin.MailMessageQueue.Properties.Resources.iconDebug;
			this.tsbnExecute.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnExecute.Name = "tsbnExecute";
			this.tsbnExecute.Size = new System.Drawing.Size(24, 25);
			this.tsbnExecute.Text = "Execute";
			this.tsbnExecute.ToolTipText = "Execute method";
			this.tsbnExecute.Click += new System.EventHandler(this.tsbnExecute_Click);
			// 
			// tsddlAdd
			// 
			this.tsddlAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsddlAdd.Image = global::Plugin.MailMessageQueue.Properties.Resources.FileNew;
			this.tsddlAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddlAdd.Name = "tsddlAdd";
			this.tsddlAdd.Size = new System.Drawing.Size(38, 25);
			this.tsddlAdd.Text = "toolStripDropDownButton1";
			this.tsddlAdd.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsddlAdd_DropDownItemClicked);
			// 
			// ConfigCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitMain);
			this.Controls.Add(this.toolStrip1);
			this.Name = "ConfigCtrl";
			this.Size = new System.Drawing.Size(225, 249);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.splitMain.Panel1.ResumeLayout(false);
			this.splitMain.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
			this.splitMain.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.SplitContainer splitMain;
		private System.Windows.Forms.ListView lvTimers;
		private System.Windows.Forms.ColumnHeader colName;
		private System.Windows.Forms.PropertyGrid pgData;
		private System.Windows.Forms.ToolStripButton tsbnRemove;
		private System.Windows.Forms.Label lblNoPlugins;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton tsbnExecute;
		private System.Windows.Forms.ToolStripDropDownButton tsddlAdd;
	}
}
