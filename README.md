# Cipher with Video Stream Application

<strong>Breif</strong>
<br/>The idea of the project (university project in 2012) was to develop an application that can cipher data using a video stream. That means, you have three entities: data to be cipher, clean input video stream, dirty output video stream with data ciphered inside.

<br/><strong>Sounds nice, now how?</strong>
<br/> Using 3 different methods:
<ol>
<li/><strong>Stream Encryption Frame By Frame</strong>: Encrypting data using the color components of the pixels contained in a video frame.
<li/><strong>Stream Encryption By Inner Frame</strong>: Ciphering using an additional frame containing the ciphered data. This cause some glittering in the output stream.
<li/><strong>Hybrid Stream Encrypting</strong>: A combination of both.
</ol>


<br/><strong>Parameters</strong>
<ul>
<li/>ByteEncryptionMode
<li/>TimeSpan: The duration to stop ciphering the video.
<li/>ColorComponent: Ciphering using one color component.
<li/>R,G,B Length: Ciphering on a number of bits on R or G or B or any combination of these. 
<li/>FramesStep: # of frames between ciphered frames.
<li/>Row, Col Area Step: Spatial ciphering on rows and cols.
</ul>

<br/><strong>Code and Libraries</strong>
<br/>C#, WPF and AForge Library to manipulate videos.

<br/><strong>UI</strong>
<br/>The system has a simple UI to show how everything is working: the input stream and the output stream with data ciphered. Here's some screen shots.

![alt tag](https://raw.githubusercontent.com/ZGTR/Video-Cipher-Application/master/ScreenShots/Screen Shot 2016-05-31 at 03.08.14.png)

![alt tag](https://raw.githubusercontent.com/ZGTR/Video-Cipher-Application/master/ScreenShots/Screen Shot 2016-05-31 at 03.08.24.png)

![alt tag](https://raw.githubusercontent.com/ZGTR/Video-Cipher-Application/master/ScreenShots/Screen Shot 2016-05-31 at 03.08.32.png)

![alt tag](https://raw.githubusercontent.com/ZGTR/Video-Cipher-Application/master/ScreenShots/Screen Shot 2016-05-31 at 03.08.39.png)
