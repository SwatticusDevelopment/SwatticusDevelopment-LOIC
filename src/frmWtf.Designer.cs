namespace LOIC
{
	partial class frmWtf
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.SuspendLayout();
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = global::LOIC.Properties.Resources.WTF;
			this.ClientSize = new System.Drawing.Size(416, 300);
			this.ControlBox = false;
			this.Icon = global::LOIC.Properties.Resources.LOIC_ICO;
			this.Name = "frmWtf";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmWtf_FormClosed);
			this.Click += new System.EventHandler(this.frmWtf_Click);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmWtf_KeyDown);
			this.ResumeLayout(false);

		}

		#endregion
	}
}