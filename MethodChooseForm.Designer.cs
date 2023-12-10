namespace CourseWork
{
    partial class MethodChooseForm
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
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            button1 = new Button();
            radioButton3 = new RadioButton();
            SuspendLayout();
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(12, 12);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(144, 24);
            radioButton1.TabIndex = 0;
            radioButton1.TabStop = true;
            radioButton1.Text = "Параметричний";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(12, 42);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(102, 24);
            radioButton2.TabIndex = 1;
            radioButton2.Text = "TSS-метод";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(12, 102);
            button1.Name = "button1";
            button1.Size = new Size(108, 29);
            button1.TabIndex = 2;
            button1.Text = "Підтвердити";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(12, 72);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(131, 24);
            radioButton3.TabIndex = 3;
            radioButton3.TabStop = true;
            radioButton3.Text = "Алаівана (опт)";
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // MethodChooseForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(202, 143);
            Controls.Add(radioButton3);
            Controls.Add(button1);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "MethodChooseForm";
            Text = "Виберіть спосіб аналізу";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private Button button1;
        private RadioButton radioButton3;
    }
}