using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using SDRSharp.Common;
using SDRSharp.Radio;
using SDRSharp.Radio.PortAudio;

namespace SDRSharp.StereoDecoder
{
    public partial class StereoPanel : UserControl
    {
        private AudioProcessor _audioProcessor;
        private ISharpControl _control;
        private AudioPlayer _player;
        private bool _playerIsStarted;
        public StereoPanel(AudioProcessor audioProcessor, ISharpControl control)
        {
            InitializeComponent();

            this._audioProcessor = audioProcessor;
            this._control = control;
            this._control.PropertyChanged += this.PropertyChangedHandler;
            this._player = new AudioPlayer(control, this._audioProcessor);
            this.AudioDeviceGet();
            this.audioDeviceComboBox_SelectedIndexChanged(null, null);
            this.volumeTrackBar.Value = Utils.GetIntSetting("StereoDecoderGain", 50);
            this.volumeTrackBar_Scroll(null, null);
            this.auxAudioEnableCheckBox.Checked = Utils.GetBooleanSetting("StereoDecoderEnable");

            this.forceMonoCheckBox.Checked = Utils.GetBooleanSetting("StereoDecoderForceMono");

            //Trying to remember what SCA was selected
            //Yea I know not the nicest code but if you know any better way of doing, please submit PR

            this.rbSAuto.Checked = Utils.GetBooleanSetting("StereoDecoderSAuto", true);
            this.rbSCCIR.Checked = Utils.GetBooleanSetting("StereoDecoderSCCIR", false);
            this.rbSPolar.Checked = Utils.GetBooleanSetting("StereoDecoderSPolar", false);

            //Get De-emphasis settings
            float DeemphasisTime = (float)Utils.GetDoubleSetting("deemphasisTime", 50);
            if (DeemphasisTime == 50f)
            {
                this.rbD50.Checked = true;
                this.rbD75.Checked = false;
            }
            else if (DeemphasisTime == 75f)
            {
                this.rbD75.Checked = true;
                this.rbD50.Checked = false;
            }

            //Hack to hide the groupbox border
            DeEmphasisGroup.Paint += (sender, e) =>
            {
                System.Drawing.Graphics gfx = e.Graphics;
                var pp = sender as GroupBox;
                gfx.Clear(pp.BackColor);
                gfx.DrawString(pp.Text, pp.Font, new System.Drawing.SolidBrush(pp.ForeColor), new System.Drawing.PointF(7, 0));
            };
            StereoSystemGroup.Paint += (sender, e) =>
            {
                System.Drawing.Graphics gfx = e.Graphics;
                var pp = sender as GroupBox;
                gfx.Clear(pp.BackColor);
                gfx.DrawString(pp.Text, pp.Font, new System.Drawing.SolidBrush(pp.ForeColor), new System.Drawing.PointF(7, 0));
            };
            
            this.EnableControls();
        }

        private void EnableControls()
        {
            bool isPlaying = this._control.IsPlaying;
            this.auxAudioEnableCheckBox.Enabled = isPlaying;
            this.audioDeviceComboBox.Enabled = !this._playerIsStarted;
        }

        public void StartAux()
        {
            if (this._playerIsStarted)
            {
                return;
            }
            this._player.Start();
            this._playerIsStarted = true;
            _control.AudioIsMuted = true;
            this.EnableControls();
        }

        public void StopAux()
        {
            if (!this._playerIsStarted)
            {
                return;
            }
            this._player.Stop();
            this._playerIsStarted = false;
            _control.AudioIsMuted = false;
            this.StereoLight.On = false;
            this.EnableControls();
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            string propertyName = e.PropertyName;
            if (!(propertyName == "StartRadio"))
            {
                if (!(propertyName == "StopRadio"))
                {
                    return;
                }
                this.EnableControls(); 
                this.StopAux();
            }
            else
            {
                this.EnableControls();
                if (this.auxAudioEnableCheckBox.Checked)
                {
                    this.StartAux();
                    return;
                }
            }
        }
        private void AudioDeviceGet()
        {
            int num = 0;
            int num2 = -1;
            List<AudioDevice> devices = AudioDevice.GetDevices(DeviceDirection.Output);
            string stringSetting = Utils.GetStringSetting("StereoDecoderDevice", string.Empty);
            for (int i = 0; i < devices.Count; i++)
            {
                this.audioDeviceComboBox.Items.Add(devices[i]);
                if (devices[i].IsDefault)
                {
                    num = i;
                }
                if (devices[i].ToString() == stringSetting)
                {
                    num2 = i;
                }
            }
            if (this.audioDeviceComboBox.Items.Count > 0)
            {
                this.audioDeviceComboBox.SelectedIndex = ((num2 >= 0) ? num2 : num);
            }
        }

        public void StoreSettings()
        {
            Utils.SaveSetting("StereoDecoderEnable", this.auxAudioEnableCheckBox.Checked);
            Utils.SaveSetting("StereoDecoderForceMono", this.forceMonoCheckBox.Checked);
            Utils.SaveSetting("StereoDecoderDevice", this.audioDeviceComboBox.SelectedItem);
            Utils.SaveSetting("StereoDecoderGain", this.volumeTrackBar.Value);

            //Trying to remember what Stereo System was selected
            //Yea I know not the nicest code but if you know any better way of doing, please submit PR
            if (this.rbSAuto.Checked)
            {
                Utils.SaveSetting("StereoDecoderSAuto", this.rbSAuto.Checked);
                Utils.SaveSetting("StereoDecoderSPolar", false);
                Utils.SaveSetting("StereoDecoderSCCIR", false);
            }
            else if (this.rbSPolar.Checked)
            {
                Utils.SaveSetting("StereoDecoderSAuto", false);
                Utils.SaveSetting("StereoDecoderSPolar", this.rbSPolar.Checked);
                Utils.SaveSetting("StereoDecoderSCCIR", false);
            }
            else if (this.rbSCCIR.Checked)
            {
                Utils.SaveSetting("StereoDecoderSAuto", false);
                Utils.SaveSetting("StereoDecoderSPolar", false);
                Utils.SaveSetting("StereoDecoderSCCIR", this.rbSCCIR.Checked);
            }

            //Store De-emphasis settings
            if (this.rbD50.Checked)
                Utils.SaveSetting("deemphasisTime", 50);
            else if (this.rbD75.Checked)
                Utils.SaveSetting("deemphasisTime", 75);
        }

        private void volumeTrackBar_Scroll(object sender, EventArgs e)
        {
            this._player.Gain = (float)Math.Pow((double)this.volumeTrackBar.Value, 3.0);
        }

        private void displayTimer_Tick(object sender, EventArgs e)
        {
            this.disbalanceLabel.Text = string.Format("Lost buffers {0:f0}", this._player.LostBuffers);
            this.bufferProgressBar.Value = this._player.BufferSize;

            //Show user what internal sapmle rate we have
            this.SampleRateLbl.Text = String.Format("Sample rate: {0} kHz", this._player.InternalSampleRate);

            //Show what Stereo System is in use
            if (this._player.StereoSystem == "Polar")
                this.rbSPolar.Checked = true;
            else if (this._player.StereoSystem == "CCIR")
                this.rbSCCIR.Checked = true;

            this.StereoLight.On = (this._player.IsPllLocked && !this._player.ForceMono);
            //this.forceMonoCheckBox.Checked = !this._player.IsPllLocked;
        }

        private void audioDeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AudioDevice audioDevice = (AudioDevice)this.audioDeviceComboBox.SelectedItem;
            this._player.DeviceIndex = audioDevice.Index;
        }

        private void auxAudioEnableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!this._control.IsPlaying)
            {
                return;
            }
            if (this.auxAudioEnableCheckBox.Checked)
            {
                this.StartAux();
                return;
            }
            if (!this.auxAudioEnableCheckBox.Checked)
            {
                this.StopAux();
            }
        }

        private void forceMonoCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this._player.ForceMono = !this._player.ForceMono;
        }

        //Determine which Radio Button was pressed
        private void SCAGroup_Buttons(object sender, EventArgs e)
        {
            System.Windows.Forms.RadioButton radioButton = sender as System.Windows.Forms.RadioButton;
            if (radioButton.Checked == true)
            {
                switch (radioButton.Text)
                {
                    //FM De-emphasis
                    case "50 µs":
                        _player.DeemphasisTime = 50;
                        break;
                    case "75 µs":
                        _player.DeemphasisTime = 75;
                        break;
                    case "None":
                        _player.DeemphasisTime = 0;
                        break;

                    //Stereo System
                    case "Auto":
                        _player.StereoSystem = "Auto";
                        break;
                    case "Polar":
                        _player.StereoSystem = "Polar";
                        break;
                    case "CCIR":
                        _player.StereoSystem = "CCIR";
                        break;
                }
            }
        }
    }
}
