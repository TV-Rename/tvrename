namespace TVRename.Forms.Utilities
{
    partial class BrowserTest
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.webChrome = new CefSharp.WinForms.ChromiumWebBrowser();
            this.webEdge = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webEdge)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.webChrome);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.webEdge);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 398;
            this.splitContainer1.TabIndex = 0;
            // 
            // webChrome
            // 
            this.webChrome.ActivateBrowserOnCreation = false;
            this.webChrome.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webChrome.Location = new System.Drawing.Point(0, 0);
            this.webChrome.Name = "webChrome";
            this.webChrome.Size = new System.Drawing.Size(398, 450);
            this.webChrome.TabIndex = 0;
            // 
            // webEdge
            // 
            this.webEdge.CreationProperties = null;
            this.webEdge.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webEdge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webEdge.Location = new System.Drawing.Point(0, 0);
            this.webEdge.Name = "webEdge";
            this.webEdge.Size = new System.Drawing.Size(398, 450);
            this.webEdge.TabIndex = 0;
            this.webEdge.ZoomFactor = 1D;
            // 
            // BrowserTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Name = "BrowserTest";
            this.Text = "BrowserTest";
            this.Load += new System.EventHandler(this.BrowserTest_Load);
            this.Resize += new System.EventHandler(this.BrowserTest_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.webEdge)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private CefSharp.WinForms.ChromiumWebBrowser webChrome;
        private Microsoft.Web.WebView2.WinForms.WebView2 webEdge;
    }
}
