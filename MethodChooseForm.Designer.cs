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
            label1 = new Label();
            label2 = new Label();
            radioButton4 = new RadioButton();
            radioButton5 = new RadioButton();
            radioButton6 = new RadioButton();
            SuspendLayout();
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(12, 32);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(136, 24);
            radioButton1.TabIndex = 0;
            radioButton1.TabStop = true;
            radioButton1.Text = "Метод Фаркаса";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(12, 62);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(102, 24);
            radioButton2.TabIndex = 1;
            radioButton2.Text = "TSS-метод";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(118, 135);
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
            radioButton3.Location = new Point(12, 92);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(141, 24);
            radioButton3.TabIndex = 3;
            radioButton3.TabStop = true;
            radioButton3.Text = "Метод Алаівана";
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(126, 20);
            label1.TabIndex = 4;
            label1.Text = "Класичні методи";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(181, 9);
            label2.Name = "label2";
            label2.Size = new Size(151, 20);
            label2.TabIndex = 5;
            label2.Text = "Скориговані методи";
            // 
            // radioButton4
            // 
            radioButton4.AutoSize = true;
            radioButton4.Location = new Point(181, 32);
            radioButton4.Name = "radioButton4";
            radioButton4.Size = new Size(147, 24);
            radioButton4.TabIndex = 6;
            radioButton4.TabStop = true;
            radioButton4.Text = "TSS-метод (мод.)";
            radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton5
            // 
            radioButton5.AutoSize = true;
            radioButton5.Location = new Point(181, 62);
            radioButton5.Name = "radioButton5";
            radioButton5.Size = new Size(143, 24);
            radioButton5.TabIndex = 7;
            radioButton5.TabStop = true;
            radioButton5.Text = "TSS-метод (опт.)";
            radioButton5.UseVisualStyleBackColor = true;
            // 
            // radioButton6
            // 
            radioButton6.AutoSize = true;
            radioButton6.Location = new Point(181, 92);
            radioButton6.Name = "radioButton6";
            radioButton6.Size = new Size(182, 24);
            radioButton6.TabIndex = 8;
            radioButton6.TabStop = true;
            radioButton6.Text = "Метод Алаівана (опт.)";
            radioButton6.UseVisualStyleBackColor = true;
            // 
            // MethodChooseForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(360, 176);
            Controls.Add(radioButton6);
            Controls.Add(radioButton5);
            Controls.Add(radioButton4);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(radioButton1);
            Controls.Add(radioButton3);
            Controls.Add(radioButton2);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "MethodChooseForm";
            Text = "Виберіть метод аналізу";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private Button button1;
        private RadioButton radioButton3;
        private Label label1;
        private Label label2;
        private RadioButton radioButton4;
        private RadioButton radioButton5;
        private RadioButton radioButton6;
    }
}