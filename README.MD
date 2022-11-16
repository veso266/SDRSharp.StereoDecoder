# SDRSharp.StereoDecoder
This plugin allows you to listen to Stereo in [OIRT FM Band](https://en.wikipedia.org/wiki/FM_broadcast_band#OIRT_bandplan) (sometimes called Polar Stereo)  

Ever since I started to DX, I heard stories about a special stereo system that countries in USSR are using that our normal radios are unable to decode, that 
So you needed a special receiver that was usualy only uvailable on USSR to decode this system
List of radios I know of

[Romantika-104-stereo](http://rr20.ddns.net/Item.aspx?ItemId=f33817ff-f3c9-4093-8f57-e7a5826c6355&Lang=En)
[Laspi-001-stereo](http://rr20.ddns.net/Item.aspx?ItemId=6388e8ea-f706-424c-a25f-1a278317b772&Lang=En)
[Takt-001-stereo](http://rr20.ddns.net/Item.aspx?ItemId=e77ba04e-fa82-49ac-a639-4418c96c0a2c&Lang=En)
[Rondo 102 Stereo](https://www.radiomuseum.org/r/taganrogwo_102_rondo_102_stereo.html) 
[Korvet 104 stereo](http://rr20.ddns.net/Item.aspx?ItemId=1480685a-e920-4638-8104-4d0373d3f83f&Lang=En)


The system is called Polar Stereo (at least I call it that way, since its using a process called [Polar Modulation](https://en.wikipedia.org/wiki/Polar_modulation) to encode Stereo Difference



in 02.07.2017 I even got Avtoradio on 68.00Mhz, but as I sadly only had my Sangean ATS909X as a reciever (which only supports CCIR FM Stereo) back then I could never properly record that  
sadly Avtoradio does not broadcast in Polar stereo anymore as their transmitter was dismantled in 2021

If you want to see what stations broadcast in Polar stereo, visit [this page](https://www.fmlist.org/oirt.php) 

For me one day I hope I will be able to receive a real station broadcasting in Polar Stereo
To remind myself what I could expect, I will write stations I could receive at my location here

RUS	72.920 Mhz	Radio Radonezh
RUS	66.980 Mhz	Radio Mayak
RUS	67.130 Mhz	Radio Rossii 	
UKR	70.970 MHz	UR 2 Radio Promin
 
# How to install
Download [dll](https://github.com/veso266/SDRSharp.StereoDecoder/releases/download/1.0/SDRSharp.StereoDecoder.dll) from release page
## For latest SDR#
You just copy dll into Plugins directory and enjoy
## For SDR# 1785 and below 
You copy the plugin DLL into sdrsharp program directory and then edit plugins.xml and add the following line to it (people used to call this  MagicLine.txt)
 ```xml
<add key="StereoDecoder" value="SDRSharp.StereoDecoder.StereoDecoderPlugin,SDRSharp.StereoDecoder" />
 ```
 
 Exmplanation of MagicLine(if you want to write your own plugin someday)
 ```xml
 <add key="Whatever-you-want-that-is-unique" value="NameSpace.EntryPoint,DLLName" />
 ```
 
 ![Polar Stereo](IMGS/PolarDEMO.PNG?raw=true "Polar Stereo") ![CCIR Stereo](IMGS/CCIRDEMO.PNG?raw=true "CCIR Stereo")

[Demonstration](https://youtu.be/Fu50GpB_aPk)
