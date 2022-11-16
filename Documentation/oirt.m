clear all;
clear sound;
[y,Fs_IN] = audioread('PolarStereo.wav'); 
y = y(:,1) + sqrt(-1)*y(:,2);                      % [I,Q] --> complex [I+j*Q]

Fs_OUT = 48e3;
fprintf("Sample Rate IN: %d\n", Fs_IN);
fprintf("Sample Rate OUT: %d\n", Fs_OUT);

scope  = dsp.SpectrumAnalyzer('SampleRate',Fs_IN,'Name',"MPX", 'PlotAsTwoSidedSpectrum', false);

% Demodulate FM signal
y = FMDemodulate(y);
y = resample(y, 256e3, Fs_IN);
stereo = StereoDecoder(y, Fs_IN, scope);
disp("Playing sound");
sound(stereo, Fs_OUT)

function result = FMDemodulate(z) %According to here: https://www.reddit.com/r/DSP/comments/ckmhyk/comment/evqylcg/?utm_source=share&utm_medium=web2x&context=3
    dy = (z(2:end).*conj(z(1:end-1)));      % Calculation of instantenious frequency
    kot = angle(dy);                        % Getting angle
    result = kot;
end

function interleavedStereo = StereoDecoder(mpx, Fs, scope)
    %scope(mpx);
    %ym = nizko_prepustni_filter(mpx, 17e3, Fs);           % low-pass filtration of L+R (mono) signal

    %will be used later
    mpx_128 = decimate(mpx, 2, 'fir');   %Decimate to 128khz

    PT_128 = decimate(mpx, 2, 'fir');   %Decimate to 128khz

    %Bandpass filter
    %Bandpass filter (Pilot tone is on 31.25 kHz in this Polar system)
    fl = 31200/64000; %We just cut 5 Hz up and 5Hz down (64e3 = 128khz (Fs) / 2)
    fh = 31300/64000; %We just cut 5 Hz up and 5Hz down
    [b,a] = butter(6, [fl, fh]);

    %Filter signal now
    pilot_tone = filter(b,a,PT_128);

    %See in frequency domain
    ft = [0:32000];
    PT_Plot = abs(fftshift(fft(pilot_tone, 64000)));
    plot(ft, PT_Plot(32000 + ft));

    pilot_tone = pilot_tone.*14; %Boost Pilot tone by 14dB since transmitter is reducing it by 14dB

    %Multiply by original signal to bring L-R copies to baseband
    freq_conv_sig = pilot_tone.* mpx_128;

    %Get rid of PILOT TONE
    freq_conv_sig = nizko_prepustni_filter(freq_conv_sig, 17e3, 128e3);

    freq_conv_sig_dec = decimate(freq_conv_sig, 4, 'fir');  %Decimate to 32khz
    
    %Normalize audio
    l_minus_r = freq_conv_sig_dec / max(abs(freq_conv_sig_dec));

    %Get rid of PILOT TONE on original signal
    %mpx = nizko_prepustni_filter(mpx, 32.25e3, Fs);

    %Decimate original from modulated signal to 32khz
    dfd = decimate(mpx, 8, 'fir');
    l_plus_r = dfd / max(abs(dfd));

    %Left channel is on positive phase, right is on negative
    %Deamphasis stay the same as in rest of europe 50us
    l_plus_r_d = do_deemphasis(l_plus_r, 50, Fs);
    l_minus_r_d = do_deemphasis(l_minus_r, 50, Fs);

    left_audio = l_plus_r_d + k_notch(l_minus_r_d, 'plus', 32e3);
    right_audio = l_plus_r_d + k_notch(l_minus_r_d, 'minus', 32e3);

    stereo = [left_audio right_audio];

    resampled = resample(stereo, 48e3, 32e3);

    interleavedStereo = resampled;
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

function result = k_notch(signal, phase, Fs)
    disp("Applying Notch filter for: " + phase);

    %One-pole, one-zero high-pass filter: http://msp.ucsd.edu/techniques/v0.11/book-html/node141.html

    num = [6.4/(2*pi)   1];
    den =  [6.4/(2*pi)   5];

    %Discrete this
    [ad, bd] = bilinear(num, den, Fs);

    %Filter signal
    signalF = filter(ad,bd,signal);
    %signalF = 0.2;


    %signalF = signal.*0.8; %Sweat spot

    %disp((1+1i*6.4*0)/(5+1i*6.4*0));
    %result = (1+1i*6.4*0)/(5+1i*6.4*0);

    %T = 1/Fs;             % Sampling period       
    %L = length(signal);      % Length of signal
    %t = (0:L-1)*T;        % Time vector

    %Y = fft(signal);
    %signalF = (1+1i*6.4*Y)/(5+1i*6.4*Y);

    %P2 = abs(Y/L);
    %P1 = P2(1:L/2+1);
    %P1(2:end-1) = 2*P1(2:end-1);

    %f = Fs*(0:(L/2))/L;
    %plot(f,P1); 
    %title('Single-Sided Amplitude Spectrum of X(t)');
    %xlabel('f (Hz)');
    %ylabel('|P1(f)|');

    if strcmp(phase, 'plus')
        result = signalF.*1;
    elseif strcmp(phase,'minus')
        result = signalF.*-1;
    end
end