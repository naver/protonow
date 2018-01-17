protoNow
---------------------

# How to build?
* Visual Studio 2015 or newer    

* Window 7 or newer     

* Microsoft .NET Framework 4.5 & Microsoft .NET Framework 3.5 .  Library SharpVectors(for SVG) work on .NET 3.5, you can target to .NET 4.5, the solution will build successfully, but SVG will work abnormal(SVG mainly used in protoNow application -> Libraries -> Icon ).   

* Visual Studio will restore NuGet packages automatically，if it failed, please reinstall it manually from: Tools > NuGet Package Manager > Package Manager Console    

# How to make package?
There are two ways.
* 1)Run autobuild.cmd under root path , you will get a package named "ProtoNow1.9.3-Setup.exe" under "Install" folder. (You need to install Nsis but no need to install Visual Studio)

* 2)We use NSIS to make package, if you want to compile NSIS Script yourself, please following this:
1. Install latest NSIS, and build solution in release mode using Visual studio.
2. Please get these plugins from [nsis](http://nsis.sourceforge.net/Main_Page) and copy these files to the same path of your NSIS install folder(eg: C:\Program Files (x86)\NSIS).  
   Tools\NSIS\Include\FileAssociation.nsh  
   Tools\NSIS\Include\nsProcess.nsh    
   Tools\NSIS\Plugins\x86-ansi\nsProcess.dll   
   Tools\NSIS\Plugins\x86-unicode\nsProcess.dll   
 3. Run Install\protoNow.nsi   
 
 # Third party libraries
 * SharpVectors : used to dispay and edit SVG.    
  https://sharpvectors.codeplex.com/   
  

 * AvalonDock : layout of UI.   
  https://avalondock.codeplex.com/    
   
 
  
 * RibbonControlsLibrary : part of toolbar.       
 https://msdn.microsoft.com/en-us/library/ff799534.aspx   
 
 
 * Nlog   
  http://nlog-project.org/    
 
 
 # Bug Report
 If you find a bug, please report to us posting [issues](https://github.com/naver/protonow/issues) on GitHub.
 
 # License
 Licensed under the LGPL v2.1, see [LICENSE](https://github.com/naver/protonow/blob/master/LICENSE) for details.   
 Copyright 2018 NAVER Corp. see [COPYING](https://github.com/naver/protonow/blob/master/COPYING) for details.  
 Third parties licenses, see [NOTICE](https://github.com/naver/protonow/blob/master/NOTICE) for details.   
 
