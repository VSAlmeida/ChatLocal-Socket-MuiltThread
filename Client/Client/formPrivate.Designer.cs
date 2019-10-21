namespace Client
{
    partial class formPrivate
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
            this.inputPrivate = new System.Windows.Forms.TextBox();
            this.history = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // inputPrivate
            // 
            this.inputPrivate.Location = new System.Drawing.Point(12, 168);
            this.inputPrivate.Name = "inputPrivate";
            this.inputPrivate.Size = new System.Drawing.Size(161, 20);
            this.inputPrivate.TabIndex = 6;
            this.inputPrivate.Enter += new System.EventHandler(this.btnSend_Click);
            // 
            // history
            // 
            this.history.Location = new System.Drawing.Point(12, 12);
            this.history.Multiline = true;
            this.history.Name = "history";
            this.history.ReadOnly = true;
            this.history.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.history.Size = new System.Drawing.Size(231, 150);
            this.history.TabIndex = 5;
            this.history.TextChanged += new System.EventHandler(this.history_TextChanged);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(179, 168);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(64, 23);
            this.btnSend.TabIndex = 7;
            this.btnSend.Text = "Enviar";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            this.btnSend.Enter += new System.EventHandler(this.btnSend_Click);
            // 
            // formPrivate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 204);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.inputPrivate);
            this.Controls.Add(this.history);
            this.Name = "formPrivate";
            this.Text = "formPrivate";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputPrivate;
        private System.Windows.Forms.TextBox history;
        private System.Windows.Forms.Button btnSend;
    }
}