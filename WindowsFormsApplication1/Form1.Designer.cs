﻿namespace WindowsFormsApplication1
{
    partial class Form1
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
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.ultravioletPanel1 = new TwistedLogik.Ultraviolet.WindowsForms.UltravioletPanel();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Right;
            this.propertyGrid1.Location = new System.Drawing.Point(49, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(235, 262);
            this.propertyGrid1.TabIndex = 0;
            // 
            // ultravioletPanel1
            // 
            this.ultravioletPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultravioletPanel1.Location = new System.Drawing.Point(0, 0);
            this.ultravioletPanel1.Name = "ultravioletPanel1";
            this.ultravioletPanel1.Size = new System.Drawing.Size(49, 262);
            this.ultravioletPanel1.TabIndex = 1;
            this.ultravioletPanel1.Drawing += new System.EventHandler(this.ultravioletPanel1_Drawing);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.ultravioletPanel1);
            this.Controls.Add(this.propertyGrid1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private TwistedLogik.Ultraviolet.WindowsForms.UltravioletPanel ultravioletPanel1;
    }
}

