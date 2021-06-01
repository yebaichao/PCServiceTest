namespace PCServiceTest
{
    partial class FormPCService
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button_Test = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_Test
            // 
            this.button_Test.Location = new System.Drawing.Point(165, 106);
            this.button_Test.Name = "button_Test";
            this.button_Test.Size = new System.Drawing.Size(151, 83);
            this.button_Test.TabIndex = 0;
            this.button_Test.Text = "Test";
            this.button_Test.UseVisualStyleBackColor = true;
            this.button_Test.Click += new System.EventHandler(this.button_Test_Click);
            // 
            // FormPCService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 314);
            this.Controls.Add(this.button_Test);
            this.Name = "FormPCService";
            this.Text = "FormPCService";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Test;
    }
}

