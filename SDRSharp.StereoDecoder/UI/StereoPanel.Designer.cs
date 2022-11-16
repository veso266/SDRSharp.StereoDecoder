using System;

namespace SDRSharp.StereoDecoder
{
    partial class StereoPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.volumeTrackBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.disbalanceLabel = new System.Windows.Forms.Label();
            this.displayTimer = new System.Windows.Forms.Timer(this.components);
            this.bufferProgressBar = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.StereoLight = new SDRSharp.StereoDecoder.LedBulb();
            this.DeEmphasisGroup = new System.Windows.Forms.GroupBox();
            this.rbDNone = new System.Windows.Forms.RadioButton();
            this.rbD75 = new System.Windows.Forms.RadioButton();
            this.rbD50 = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.forceMonoCheckBox = new System.Windows.Forms.CheckBox();
            this.SampleRateLbl = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.StereoSystemGroup = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.rbSPolar = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.rbSAuto = new System.Windows.Forms.RadioButton();
            this.rbSCCIR = new System.Windows.Forms.RadioButton();
            this.auxAudioEnableCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.audioDeviceComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.volumeTrackBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.DeEmphasisGroup.SuspendLayout();
            this.StereoSystemGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // volumeTrackBar
            // 
            this.volumeTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.volumeTrackBar.AutoSize = false;
            this.volumeTrackBar.Location = new System.Drawing.Point(5, 201);
            this.volumeTrackBar.Maximum = 100;
            this.volumeTrackBar.Name = "volumeTrackBar";
            this.volumeTrackBar.Size = new System.Drawing.Size(259, 35);
            this.volumeTrackBar.TabIndex = 5;
            this.volumeTrackBar.TickFrequency = 10;
            this.volumeTrackBar.Scroll += new System.EventHandler(this.volumeTrackBar_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 185);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Output level";
            // 
            // disbalanceLabel
            // 
            this.disbalanceLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.disbalanceLabel.AutoSize = true;
            this.disbalanceLabel.Location = new System.Drawing.Point(196, 248);
            this.disbalanceLabel.Name = "disbalanceLabel";
            this.disbalanceLabel.Size = new System.Drawing.Size(71, 13);
            this.disbalanceLabel.TabIndex = 7;
            this.disbalanceLabel.Text = "Lost buffers 0";
            // 
            // displayTimer
            // 
            this.displayTimer.Enabled = true;
            this.displayTimer.Interval = 500;
            this.displayTimer.Tick += new System.EventHandler(this.displayTimer_Tick);
            // 
            // bufferProgressBar
            // 
            this.bufferProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bufferProgressBar.Location = new System.Drawing.Point(68, 243);
            this.bufferProgressBar.Name = "bufferProgressBar";
            this.bufferProgressBar.Size = new System.Drawing.Size(122, 23);
            this.bufferProgressBar.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 248);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Use buffer";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.StereoLight);
            this.groupBox1.Controls.Add(this.DeEmphasisGroup);
            this.groupBox1.Controls.Add(this.forceMonoCheckBox);
            this.groupBox1.Controls.Add(this.SampleRateLbl);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.StereoSystemGroup);
            this.groupBox1.Controls.Add(this.auxAudioEnableCheckBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.audioDeviceComboBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.volumeTrackBar);
            this.groupBox1.Controls.Add(this.bufferProgressBar);
            this.groupBox1.Controls.Add(this.disbalanceLabel);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(271, 315);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // StereoLight
            // 
            this.StereoLight.Color = System.Drawing.Color.Blue;
            this.StereoLight.Location = new System.Drawing.Point(100, 141);
            this.StereoLight.Name = "StereoLight";
            this.StereoLight.On = true;
            this.StereoLight.Size = new System.Drawing.Size(35, 36);
            this.StereoLight.TabIndex = 11;
            this.StereoLight.Text = "ledBulb1";
            // 
            // DeEmphasisGroup
            // 
            this.DeEmphasisGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DeEmphasisGroup.Controls.Add(this.rbDNone);
            this.DeEmphasisGroup.Controls.Add(this.rbD75);
            this.DeEmphasisGroup.Controls.Add(this.rbD50);
            this.DeEmphasisGroup.Controls.Add(this.label8);
            this.DeEmphasisGroup.Controls.Add(this.label11);
            this.DeEmphasisGroup.Location = new System.Drawing.Point(5, 100);
            this.DeEmphasisGroup.Name = "DeEmphasisGroup";
            this.DeEmphasisGroup.Size = new System.Drawing.Size(260, 25);
            this.DeEmphasisGroup.TabIndex = 15;
            this.DeEmphasisGroup.TabStop = false;
            // 
            // rbDNone
            // 
            this.rbDNone.AutoSize = true;
            this.rbDNone.Location = new System.Drawing.Point(185, -1);
            this.rbDNone.Name = "rbDNone";
            this.rbDNone.Size = new System.Drawing.Size(51, 17);
            this.rbDNone.TabIndex = 23;
            this.rbDNone.Text = "None";
            this.rbDNone.UseVisualStyleBackColor = true;
            this.rbDNone.CheckedChanged += new System.EventHandler(this.SCAGroup_Buttons);
            // 
            // rbD75
            // 
            this.rbD75.AutoSize = true;
            this.rbD75.Location = new System.Drawing.Point(134, -1);
            this.rbD75.Name = "rbD75";
            this.rbD75.Size = new System.Drawing.Size(51, 17);
            this.rbD75.TabIndex = 22;
            this.rbD75.Text = "75 µs";
            this.rbD75.UseVisualStyleBackColor = true;
            this.rbD75.CheckedChanged += new System.EventHandler(this.SCAGroup_Buttons);
            // 
            // rbD50
            // 
            this.rbD50.AutoSize = true;
            this.rbD50.Location = new System.Drawing.Point(79, -1);
            this.rbD50.Name = "rbD50";
            this.rbD50.Size = new System.Drawing.Size(51, 17);
            this.rbD50.TabIndex = 21;
            this.rbD50.Text = "50 µs";
            this.rbD50.UseVisualStyleBackColor = true;
            this.rbD50.CheckedChanged += new System.EventHandler(this.SCAGroup_Buttons);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(74, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "De-emphasis: ";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 3);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(0, 13);
            this.label11.TabIndex = 15;
            // 
            // forceMonoCheckBox
            // 
            this.forceMonoCheckBox.AutoSize = true;
            this.forceMonoCheckBox.Location = new System.Drawing.Point(190, 0);
            this.forceMonoCheckBox.Name = "forceMonoCheckBox";
            this.forceMonoCheckBox.Size = new System.Drawing.Size(53, 17);
            this.forceMonoCheckBox.TabIndex = 15;
            this.forceMonoCheckBox.Text = "Mono";
            this.forceMonoCheckBox.UseVisualStyleBackColor = true;
            this.forceMonoCheckBox.CheckedChanged += new System.EventHandler(this.forceMonoCheckBox_CheckedChanged);
            // 
            // SampleRateLbl
            // 
            this.SampleRateLbl.AutoSize = true;
            this.SampleRateLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SampleRateLbl.Location = new System.Drawing.Point(6, 282);
            this.SampleRateLbl.Name = "SampleRateLbl";
            this.SampleRateLbl.Size = new System.Drawing.Size(123, 20);
            this.SampleRateLbl.TabIndex = 11;
            this.SampleRateLbl.Text = "Sample Rate: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Pilot Tone Light: ";
            // 
            // StereoSystemGroup
            // 
            this.StereoSystemGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StereoSystemGroup.Controls.Add(this.label6);
            this.StereoSystemGroup.Controls.Add(this.rbSPolar);
            this.StereoSystemGroup.Controls.Add(this.label4);
            this.StereoSystemGroup.Controls.Add(this.rbSAuto);
            this.StereoSystemGroup.Controls.Add(this.rbSCCIR);
            this.StereoSystemGroup.Location = new System.Drawing.Point(6, 76);
            this.StereoSystemGroup.Name = "StereoSystemGroup";
            this.StereoSystemGroup.Size = new System.Drawing.Size(259, 20);
            this.StereoSystemGroup.TabIndex = 14;
            this.StereoSystemGroup.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "Stereo System:";
            // 
            // rbSPolar
            // 
            this.rbSPolar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rbSPolar.AutoSize = true;
            this.rbSPolar.Location = new System.Drawing.Point(133, -1);
            this.rbSPolar.Name = "rbSPolar";
            this.rbSPolar.Size = new System.Drawing.Size(49, 17);
            this.rbSPolar.TabIndex = 18;
            this.rbSPolar.Text = "Polar";
            this.rbSPolar.UseVisualStyleBackColor = true;
            this.rbSPolar.CheckedChanged += new System.EventHandler(this.SCAGroup_Buttons);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 13);
            this.label4.TabIndex = 15;
            // 
            // rbSAuto
            // 
            this.rbSAuto.AutoSize = true;
            this.rbSAuto.Location = new System.Drawing.Point(78, -1);
            this.rbSAuto.Name = "rbSAuto";
            this.rbSAuto.Size = new System.Drawing.Size(47, 17);
            this.rbSAuto.TabIndex = 11;
            this.rbSAuto.Text = "Auto";
            this.rbSAuto.UseVisualStyleBackColor = true;
            this.rbSAuto.CheckedChanged += new System.EventHandler(this.SCAGroup_Buttons);
            // 
            // rbSCCIR
            // 
            this.rbSCCIR.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rbSCCIR.AutoSize = true;
            this.rbSCCIR.Location = new System.Drawing.Point(184, -1);
            this.rbSCCIR.Name = "rbSCCIR";
            this.rbSCCIR.Size = new System.Drawing.Size(50, 17);
            this.rbSCCIR.TabIndex = 12;
            this.rbSCCIR.Text = "CCIR";
            this.rbSCCIR.UseVisualStyleBackColor = true;
            this.rbSCCIR.CheckedChanged += new System.EventHandler(this.SCAGroup_Buttons);
            // 
            // auxAudioEnableCheckBox
            // 
            this.auxAudioEnableCheckBox.AutoSize = true;
            this.auxAudioEnableCheckBox.Location = new System.Drawing.Point(9, 0);
            this.auxAudioEnableCheckBox.Name = "auxAudioEnableCheckBox";
            this.auxAudioEnableCheckBox.Size = new System.Drawing.Size(59, 17);
            this.auxAudioEnableCheckBox.TabIndex = 3;
            this.auxAudioEnableCheckBox.Text = "Enable";
            this.auxAudioEnableCheckBox.UseVisualStyleBackColor = true;
            this.auxAudioEnableCheckBox.CheckedChanged += new System.EventHandler(this.auxAudioEnableCheckBox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Audio device";
            // 
            // audioDeviceComboBox
            // 
            this.audioDeviceComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.audioDeviceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.audioDeviceComboBox.DropDownWidth = 330;
            this.audioDeviceComboBox.FormattingEnabled = true;
            this.audioDeviceComboBox.Location = new System.Drawing.Point(5, 39);
            this.audioDeviceComboBox.Name = "audioDeviceComboBox";
            this.audioDeviceComboBox.Size = new System.Drawing.Size(259, 21);
            this.audioDeviceComboBox.TabIndex = 0;
            this.audioDeviceComboBox.SelectedIndexChanged += new System.EventHandler(this.audioDeviceComboBox_SelectedIndexChanged);
            // 
            // StereoPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "StereoPanel";
            this.Size = new System.Drawing.Size(275, 325);
            ((System.ComponentModel.ISupportInitialize)(this.volumeTrackBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.DeEmphasisGroup.ResumeLayout(false);
            this.DeEmphasisGroup.PerformLayout();
            this.StereoSystemGroup.ResumeLayout(false);
            this.StereoSystemGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TrackBar volumeTrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label disbalanceLabel;
        private System.Windows.Forms.Timer displayTimer;
        private System.Windows.Forms.ProgressBar bufferProgressBar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox auxAudioEnableCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox audioDeviceComboBox;
        private System.Windows.Forms.GroupBox StereoSystemGroup;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label SampleRateLbl;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbSAuto;
        private System.Windows.Forms.RadioButton rbSCCIR;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rbSPolar;
        private System.Windows.Forms.CheckBox forceMonoCheckBox;
        private System.Windows.Forms.GroupBox DeEmphasisGroup;
        private System.Windows.Forms.RadioButton rbDNone;
        private System.Windows.Forms.RadioButton rbD75;
        private System.Windows.Forms.RadioButton rbD50;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label11;
        private LedBulb StereoLight;
    }
}
