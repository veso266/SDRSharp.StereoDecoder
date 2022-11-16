using System;
using System.Windows.Forms;
using SDRSharp.Common;
using SDRSharp.Radio;

namespace SDRSharp.StereoDecoder
{
    public class StereoPlugin : ISharpPlugin
    {
        public string DisplayName
        {
            get
            {
                return "Stereo Decoder";
            }
        }

        public UserControl Gui
        {
            get
            {
                return this._guiControl;
            }
        }

        public void Initialize(ISharpControl control)
        {
            this._control = control;
            this._audioProcessor = new AudioProcessor();
            this._control.RegisterStreamHook(this._audioProcessor, ProcessorType.FMMPX);
            this._guiControl = new StereoPanel(this._audioProcessor, this._control);
        }

        public void Close()
        {
            this._guiControl.StoreSettings();
            this._guiControl.StopAux();
        }

        private ISharpControl _control;
        private AudioProcessor _audioProcessor;
        private StereoPanel _guiControl;
    }
}
