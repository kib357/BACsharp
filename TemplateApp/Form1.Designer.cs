namespace TemplateApp
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.whoIsButton = new System.Windows.Forms.Button();
            this.readPropertyButton = new System.Windows.Forms.Button();
            this.writePropertyButton = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(127, 277);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // whoIsButton
            // 
            this.whoIsButton.Location = new System.Drawing.Point(12, 295);
            this.whoIsButton.Name = "whoIsButton";
            this.whoIsButton.Size = new System.Drawing.Size(75, 23);
            this.whoIsButton.TabIndex = 1;
            this.whoIsButton.Text = "Who Is";
            this.whoIsButton.UseVisualStyleBackColor = true;
            this.whoIsButton.Click += new System.EventHandler(this.whoIsButton_Click);
            // 
            // readPropertyButton
            // 
            this.readPropertyButton.Location = new System.Drawing.Point(93, 295);
            this.readPropertyButton.Name = "readPropertyButton";
            this.readPropertyButton.Size = new System.Drawing.Size(98, 23);
            this.readPropertyButton.TabIndex = 2;
            this.readPropertyButton.Text = "Read property";
            this.readPropertyButton.UseVisualStyleBackColor = true;
            this.readPropertyButton.Click += new System.EventHandler(this.readPropertyButton_Click);
            // 
            // writePropertyButton
            // 
            this.writePropertyButton.Location = new System.Drawing.Point(197, 295);
            this.writePropertyButton.Name = "writePropertyButton";
            this.writePropertyButton.Size = new System.Drawing.Size(88, 23);
            this.writePropertyButton.TabIndex = 3;
            this.writePropertyButton.Text = "Write property";
            this.writePropertyButton.UseVisualStyleBackColor = true;
            this.writePropertyButton.Click += new System.EventHandler(this.writePropertyButton_Click);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(145, 12);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(302, 277);
            this.listBox2.TabIndex = 4;
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(453, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 365);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.writePropertyButton);
            this.Controls.Add(this.readPropertyButton);
            this.Controls.Add(this.whoIsButton);
            this.Controls.Add(this.listBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button whoIsButton;
        private System.Windows.Forms.Button readPropertyButton;
        private System.Windows.Forms.Button writePropertyButton;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.TextBox textBox1;
    }
}

