namespace rail_link_sim
{
    partial class graph_editor_panel
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
            this.max_speed_box = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.operate_speed_text = new System.Windows.Forms.Label();
            this.max_speed_text = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.operate_speed_box = new System.Windows.Forms.NumericUpDown();
            this.apply_but = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.civil_text = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dewell_box = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.max_speed_box)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.operate_speed_box)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dewell_box)).BeginInit();
            this.SuspendLayout();
            // 
            // max_speed_box
            // 
            this.max_speed_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.max_speed_box.Location = new System.Drawing.Point(120, 33);
            this.max_speed_box.Margin = new System.Windows.Forms.Padding(2);
            this.max_speed_box.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.max_speed_box.Name = "max_speed_box";
            this.max_speed_box.Size = new System.Drawing.Size(70, 22);
            this.max_speed_box.TabIndex = 0;
            this.max_speed_box.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.max_speed_box.ValueChanged += new System.EventHandler(this.max_speed_box_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(198, 35);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "%";
            // 
            // operate_speed_text
            // 
            this.operate_speed_text.AutoSize = true;
            this.operate_speed_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.operate_speed_text.Location = new System.Drawing.Point(226, 62);
            this.operate_speed_text.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.operate_speed_text.Name = "operate_speed_text";
            this.operate_speed_text.Size = new System.Drawing.Size(37, 16);
            this.operate_speed_text.TabIndex = 3;
            this.operate_speed_text.Text = "(000)";
            // 
            // max_speed_text
            // 
            this.max_speed_text.AutoSize = true;
            this.max_speed_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.max_speed_text.Location = new System.Drawing.Point(226, 35);
            this.max_speed_text.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.max_speed_text.Name = "max_speed_text";
            this.max_speed_text.Size = new System.Drawing.Size(37, 16);
            this.max_speed_text.TabIndex = 4;
            this.max_speed_text.Text = "(000)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(198, 62);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "%";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 61);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 16);
            this.label5.TabIndex = 6;
            this.label5.Text = "Operate Speed :";
            // 
            // operate_speed_box
            // 
            this.operate_speed_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.operate_speed_box.Location = new System.Drawing.Point(120, 59);
            this.operate_speed_box.Margin = new System.Windows.Forms.Padding(2);
            this.operate_speed_box.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.operate_speed_box.Name = "operate_speed_box";
            this.operate_speed_box.Size = new System.Drawing.Size(70, 22);
            this.operate_speed_box.TabIndex = 5;
            this.operate_speed_box.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.operate_speed_box.ValueChanged += new System.EventHandler(this.operate_speed_box_ValueChanged);
            // 
            // apply_but
            // 
            this.apply_but.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.apply_but.Location = new System.Drawing.Point(15, 114);
            this.apply_but.Name = "apply_but";
            this.apply_but.Size = new System.Drawing.Size(244, 31);
            this.apply_but.TabIndex = 8;
            this.apply_but.Text = "Apply";
            this.apply_but.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 35);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Max Speed :";
            // 
            // civil_text
            // 
            this.civil_text.AutoSize = true;
            this.civil_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.civil_text.Location = new System.Drawing.Point(12, 10);
            this.civil_text.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.civil_text.Name = "civil_text";
            this.civil_text.Size = new System.Drawing.Size(111, 16);
            this.civil_text.TabIndex = 9;
            this.civil_text.Text = "Max Civil Speed :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 87);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 16);
            this.label3.TabIndex = 11;
            this.label3.Text = "Dewell Time :";
            // 
            // dewell_box
            // 
            this.dewell_box.Enabled = false;
            this.dewell_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dewell_box.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.dewell_box.Location = new System.Drawing.Point(120, 85);
            this.dewell_box.Margin = new System.Windows.Forms.Padding(2);
            this.dewell_box.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.dewell_box.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.dewell_box.Name = "dewell_box";
            this.dewell_box.Size = new System.Drawing.Size(98, 22);
            this.dewell_box.TabIndex = 10;
            this.dewell_box.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.dewell_box.ValueChanged += new System.EventHandler(this.dewell_box_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(229, 87);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 16);
            this.label6.TabIndex = 12;
            this.label6.Text = "sec";
            // 
            // graph_editor_panel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5F, 9F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 154);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dewell_box);
            this.Controls.Add(this.civil_text);
            this.Controls.Add(this.apply_but);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.operate_speed_box);
            this.Controls.Add(this.max_speed_text);
            this.Controls.Add(this.operate_speed_text);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.max_speed_box);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "graph_editor_panel";
            this.Text = "Graph Editor";
            ((System.ComponentModel.ISupportInitialize)(this.max_speed_box)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.operate_speed_box)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dewell_box)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button apply_but;
        public System.Windows.Forms.NumericUpDown max_speed_box;
        public System.Windows.Forms.Label operate_speed_text;
        public System.Windows.Forms.Label max_speed_text;
        public System.Windows.Forms.NumericUpDown operate_speed_box;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label civil_text;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown dewell_box;
        private System.Windows.Forms.Label label6;
    }
}