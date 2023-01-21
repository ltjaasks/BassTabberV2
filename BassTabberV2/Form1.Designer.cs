namespace BassTabberV2
{
    partial class BassTabber
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.start = new System.Windows.Forms.Button();
            this.stop = new System.Windows.Forms.Button();
            this.tab = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(234, 415);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(75, 23);
            this.start.TabIndex = 0;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.startRecording);
            // 
            // stop
            // 
            this.stop.Location = new System.Drawing.Point(315, 415);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(75, 23);
            this.stop.TabIndex = 1;
            this.stop.Text = "Stop";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Click += new System.EventHandler(this.stopRecording);
            // 
            // tab
            // 
            this.tab.Location = new System.Drawing.Point(12, 58);
            this.tab.Name = "tab";
            this.tab.Size = new System.Drawing.Size(616, 351);
            this.tab.TabIndex = 2;
            this.tab.Text = "Press start to record";
            // 
            // BassTabber
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 450);
            this.Controls.Add(this.tab);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.start);
            this.Name = "BassTabber";
            this.Text = "BassTabber";
            this.ResumeLayout(false);

        }

        #endregion

        private Button start;
        private Button stop;
        private RichTextBox tab;
    }
}