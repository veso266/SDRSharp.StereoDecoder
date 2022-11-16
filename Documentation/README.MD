# Documentation
This is all the documentation I used when researching and developing this plugin (if you want to know how everything works, then I suggest you read it)

1. I started by finding the standard ``ITU std on FM -- R-REC-BS.450-3-200111-I!!PDF-E.pdf``
2. Then I asked a nice lady in University to help me translate this picture ``ris1.gif``, she translated it like this ``20210419_165218.jpg``
3. Then as she was not able to translate it all, I had to ask my aunt for help as well, so we chatted a bit and then figured out the remainings
4. Then I finaly had the whole picture on how this system works and how it differs from out CCIR Stereo System
 ![Polar Stereo Explanation](PolarStereoExplanation.jpg?raw=true "Polar Stereo Explanation")
5. Then I used GNURadio 3.7.14.0 to create the first prototype ``MPX OIRT Stereo Decoder.grc``, it still lacked the K-Notch filter so it only worked on a specific signal: ``PolarStereo.wav``
GNURadio is a great learning tool, because it allows you to see visualy how signal travel through blocks, you also don't need to care at all about buffers and most functions (not all) are already implemented 
as blocks there like FM Demodulation, AM Demodulation, Resampling, etc, so you don't realy need to deaply understand how all this works internaly to make something usefull with a signal
If you want to do anything more complex though, like [Bilinear transformation](https://www.math.hkust.edu.hk/~maykwok/courses/ma304/03_04/03MA304Conf.pdf), which in our case is used to transform the filter from S-Plane into Z-Plane, in whose form we could use it
GNURadio does not have that (ot at least I didn't find it there), so I used Matlab, is has [bilinear()](https://www.mathworks.com/help/signal/ref/bilinear.html) function
6. Then I decided to write my decoder in Matlab, since its DSP Toolbox already had a lot of usefull functions to deal with signal processing it seamed easier to write my K-Notch filter there
7. After I had my K-Notch filter working in Matlab, I decided to bite the bullet and made everything into a plugin
This which was harder then it first seamed, because if GNURadio and Matlab have a lot of functions to help you, plain C# has nothing like that
I had to write Bilinear Transformation from scratch there, I had to care for resampling myself, I had to create buffers, shuffle bits around without a magic box (yea, its true that SDR# Developer, aka @prog had implemented a lot of magic boxes already
but more complex things were still missing, SDR# is not Signal Processing Toolbox, thats GNURadio and Matlab's job. SDR# is a program that allows above average humans (average Joe will just buy a premade product that he can control from a confort of his couch and pay top $ for it) to listen to radio with an SDR, if you want to do complex things with a signal, you have to write complex functions from scratch

And that's the story on how this plugin was created, I hope it works and that it will be as usefull to other people as it is to me :)