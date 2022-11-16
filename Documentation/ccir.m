clear all;
clear sound;
[y,Fs_IN] = audioread('CCIRStereo.wav'); 
y = y(:,1) + sqrt(-1)*y(:,2);                      % [I,Q] --> complex [I+j*Q]

Fs_OUT = 48e3;
disp(Fs_IN);

scope  = dsp.SpectrumAnalyzer('SampleRate',Fs_IN,'Name',"MPX", 'PlotAsTwoSidedSpectrum', false);

% DECODING MONO L+R SIGNAL
y = FMDemodulate(y);
y = resample(y, 256e3, Fs_IN);
stereo = StereoDecoder(y, Fs_IN, scope);
sound(stereo, 32000)

function result = FMDemodulate(z) %According to here: https://www.reddit.com/r/DSP/comments/ckmhyk/comment/evqylcg/?utm_source=share&utm_medium=web2x&context=3
    dy = (z(2:end).*conj(z(1:end-1)));      % Calculation of instantenious frequency
    kot = angle(dy);                        % Getting angle
    result = kot;
end

function interleavedStereo = StereoDecoder(mpx, Fs, scope)
    %scope(mpx);
    %ym = nizko_prepustni_filter(mpx, 17e3, Fs);           % low-pass filtration of L+R (mono) signal

    %will be used later
    l_minur_R = decimate(mpx, 2, 'fir');   %Decimate to 128khz

    PT_128 = decimate(mpx, 2, 'fir');   %Decimate to 128khz

    %Bandpass filter
    fl = 18900/64000;
    fh = 19100/64000;
    [b,a] = butter(6, [fl, fh]);

    %Filter signal now
    pilot_tone = filter(b,a,PT_128);

    %Multiplication in time is convulution in frequency
    squared_PT = pilot_tone.^2;

    %high pass filter just to get rid of DC signal
    Wn = 10000/64000; %Normalized cut-off frequency
    [b1,a1] = butter(6, Wn, 'high');
    spt_no_DC = filter(b1,a1, squared_PT);

    %Multiply by original signal to bring L-R copies to baseband
    freq_conv_sig = spt_no_DC.* l_minur_R;

    %Get rid of PILOT TONE
    %freq_conv_sig = lowpass(freq_conv_sig,17e3,128e3);
    freq_conv_sig = nizko_prepustni_filter(freq_conv_sig, 17e3, 128e3);

    freq_conv_sig_dec = decimate(freq_conv_sig, 4, 'fir');  %Decimate to 32khz
    
    %Normalize audio
    freq_conv_sig_dec_sound = freq_conv_sig_dec / max(abs(freq_conv_sig_dec));

    %Get rid of PILOT TONE on original signal
    %mpx = lowpass(mpx,17e3,Fs);
    mpx = nizko_prepustni_filter(mpx, 19e3, Fs);

    %Decimate original from modulated signal to 32khz
    dfd = decimate(mpx, 8, 'fir');
    dfdd = dfd / max(abs(dfd));

    added_signal = freq_conv_sig_dec_sound + dfdd;
    substracted_signal = freq_conv_sig_dec_sound - dfdd;

    %Deamphasize signal to 50us (for Europe)
    left_audio = do_deemphasis(added_signal, 50, Fs);
    right_audio = do_deemphasis(substracted_signal, 50, Fs);

    stereo = [left_audio right_audio];

    %resampled = resample(mpx, 32e3, 256e3);

    interleavedStereo = stereo;
end

function result = nizko_prepustni_filter(signal, cutoff_freq, Fs)
    % Low-pass (LP) filter for removal of Pilot tone signal
    Wn = cutoff_freq/Fs/2; %Normalized cut-off frequency
    [b1,a1] = butter(6, Wn, 'low');
    result = filter(b1,a1, signal);
end

function result = do_deemphasis(signal, deemphasisTime, Fs)

    % FM systems use a pre-emphasis for the audio signals. 
    % That means, all audio  frequencies above a corner frequency are emphasized. 

    % The corner frequencies are:
    %   fc = 2.1221 kHz for region 2 (the Americas)
    %   fc = 3.1831 kHz for Region 1 (Europe and Asia)

    % This gives the time constants:
    %   τ = 75 μsec for region 2
    %   τ = 50 μsec for region 1

    % If European set receives an US  transmitter:  trebble boost of approx. 3.5 dB
    % If US set receives an European transmitter:  bass boost of approx. 3.5 dB

    % Pre-emphasis and de-emphasis filters
    if deemphasisTime == 75
        f1 = 21221; 
    elseif deemphasisTime == 50
        f1 = 31831;
    end
    tau1 = 1/(2*pi*f1); w1 = tan(1/(2*Fs*tau1));
    b_de = [w1/(1+w1), w1/(1+w1)]; 
    a_de = [1, (w1-1)/(w1+1)];
    result = filter(b_de,a_de,signal);
end