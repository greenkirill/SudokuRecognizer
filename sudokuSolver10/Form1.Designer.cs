namespace sudokuSolver10 {
    partial class Form1 {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent() {
            this.label1 = new System.Windows.Forms.Label();
            this.blackCheck = new System.Windows.Forms.CheckBox();
            this.startButton = new System.Windows.Forms.Button();
            this.whiteCheck = new System.Windows.Forms.CheckBox();
            this.whiteRectCheck = new System.Windows.Forms.CheckBox();
            this.sudokuLineCheck = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.rotateLineCheck = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.RotateCheck = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(18, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Отображение:";
            // 
            // blackCheck
            // 
            this.blackCheck.AutoSize = true;
            this.blackCheck.Location = new System.Drawing.Point(22, 39);
            this.blackCheck.Name = "blackCheck";
            this.blackCheck.Size = new System.Drawing.Size(111, 17);
            this.blackCheck.TabIndex = 1;
            this.blackCheck.Text = "Черные пиксели";
            this.blackCheck.UseVisualStyleBackColor = true;
            this.blackCheck.CheckedChanged += new System.EventHandler(this.blackCheck_CheckedChanged);
            // 
            // startButton
            // 
            this.startButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.startButton.Location = new System.Drawing.Point(9, 521);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(184, 32);
            this.startButton.TabIndex = 2;
            this.startButton.Text = "Старт";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // whiteCheck
            // 
            this.whiteCheck.AutoSize = true;
            this.whiteCheck.Location = new System.Drawing.Point(22, 63);
            this.whiteCheck.Name = "whiteCheck";
            this.whiteCheck.Size = new System.Drawing.Size(104, 17);
            this.whiteCheck.TabIndex = 3;
            this.whiteCheck.Text = "Белые пиксели";
            this.whiteCheck.UseVisualStyleBackColor = true;
            this.whiteCheck.CheckedChanged += new System.EventHandler(this.whiteCheck_CheckedChanged);
            // 
            // whiteRectCheck
            // 
            this.whiteRectCheck.AutoSize = true;
            this.whiteRectCheck.Location = new System.Drawing.Point(22, 87);
            this.whiteRectCheck.Name = "whiteRectCheck";
            this.whiteRectCheck.Size = new System.Drawing.Size(103, 17);
            this.whiteRectCheck.TabIndex = 4;
            this.whiteRectCheck.Text = "Белый квадрат";
            this.whiteRectCheck.UseVisualStyleBackColor = true;
            this.whiteRectCheck.CheckedChanged += new System.EventHandler(this.whiteRectCheck_CheckedChanged);
            // 
            // sudokuLineCheck
            // 
            this.sudokuLineCheck.AutoSize = true;
            this.sudokuLineCheck.Location = new System.Drawing.Point(22, 110);
            this.sudokuLineCheck.Name = "sudokuLineCheck";
            this.sudokuLineCheck.Size = new System.Drawing.Size(110, 17);
            this.sudokuLineCheck.TabIndex = 5;
            this.sudokuLineCheck.Text = "Линии на судоку";
            this.sudokuLineCheck.UseVisualStyleBackColor = true;
            this.sudokuLineCheck.CheckedChanged += new System.EventHandler(this.sudokuLineCheck_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(9, 486);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(184, 29);
            this.button1.TabIndex = 6;
            this.button1.Text = "Выбор папки";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // rotateLineCheck
            // 
            this.rotateLineCheck.AutoSize = true;
            this.rotateLineCheck.Location = new System.Drawing.Point(22, 133);
            this.rotateLineCheck.Name = "rotateLineCheck";
            this.rotateLineCheck.Size = new System.Drawing.Size(77, 17);
            this.rotateLineCheck.TabIndex = 7;
            this.rotateLineCheck.Text = "Rotate line";
            this.rotateLineCheck.UseVisualStyleBackColor = true;
            this.rotateLineCheck.CheckedChanged += new System.EventHandler(this.rotateLineCheck_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.RotateCheck);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.startButton);
            this.panel1.Controls.Add(this.rotateLineCheck);
            this.panel1.Controls.Add(this.blackCheck);
            this.panel1.Controls.Add(this.whiteCheck);
            this.panel1.Controls.Add(this.sudokuLineCheck);
            this.panel1.Controls.Add(this.whiteRectCheck);
            this.panel1.Location = new System.Drawing.Point(722, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 560);
            this.panel1.TabIndex = 8;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(22, 181);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(60, 17);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "цифры";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // RotateCheck
            // 
            this.RotateCheck.AutoSize = true;
            this.RotateCheck.Location = new System.Drawing.Point(22, 157);
            this.RotateCheck.Name = "RotateCheck";
            this.RotateCheck.Size = new System.Drawing.Size(151, 17);
            this.RotateCheck.TabIndex = 8;
            this.RotateCheck.Text = "Повернуть изображение";
            this.RotateCheck.UseVisualStyleBackColor = true;
            this.RotateCheck.CheckedChanged += new System.EventHandler(this.RotateCheck_CheckedChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(282, 552);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "<-";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(364, 552);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "->";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(95, 275);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(523, 23);
            this.progressBar1.TabIndex = 11;
            this.progressBar1.Visible = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 589);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Распознаватель судоку";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox blackCheck;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.CheckBox whiteCheck;
        private System.Windows.Forms.CheckBox whiteRectCheck;
        private System.Windows.Forms.CheckBox sudokuLineCheck;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox rotateLineCheck;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.CheckBox RotateCheck;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}

