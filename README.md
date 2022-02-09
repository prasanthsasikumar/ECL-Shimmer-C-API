# Shimmer GUI with LSL Streaming 

Shimmer GUI application to collect sensor data and stream it to other applications using LSL. 


[![N|Solid](https://github.com/prasanthsasikumar/ECL-Shimmer-C-API/blob/master/Resources/StreamLog.png)](http://empathiccomputing.org/)


[![N|Solid](https://github.com/prasanthsasikumar/ECL-Shimmer-C-API/blob/master/Resources/config.png)](http://empathiccomputing.org/)

[![N|Solid](https://github.com/prasanthsasikumar/ECL-Shimmer-C-API/blob/master/Resources/LSL.png)](http://empathiccomputing.org/)

Working Video : https://youtu.be/J90ntmIAdNM


Developed physiological sensing use cases in remote collaboration applications using Unity. 

Thanks to Massey University for lending the shimmer device. 


LSL Sample Application for Unity: https://github.com/labstreaminglayer/LSL4Unity

The Shimmer C# API is used to control and stream data from both a Verisense Device and a Shimmer3 Bluetooth Device running LogAndStream firmware. 



## Shimmer 3
**First open the ShimmerAPI solution. The examples will be opened along with the ShimmerAPI project. Build the ShimmerAPI first before proceeding to using the examples as the examples references the ShimmerAPI dll**.

NOTE: Where required the dll for ShimmerClosedLibrary (algorithms) can be found in the libs folder within the ShimmerAPI project. Where required the following nuget packages are used:-
- MathNet.Numerics which is a ShimmerClosedLibrary dependency
- 32feet.NET which is required when using the 32 feet library (ShimmerLogAndStream32Feet.cs , etc)

NOTE: The ShimmerClosedLibrary provides 
- ECG to Heart Rate/Inter Beat Interval(IBI) algorithm
- PPG to Heart Rate/Inter Beat Interval(IBI) algorithm



# The Following Applies To All Code Provided in the repository
Copyright (c) 2014, Shimmer Research, Ltd.
 All rights reserved

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions are
 met:

     * Redistributions of source code must retain the above copyright
       notice, this list of conditions and the following disclaimer.
     * Redistributions in binary form must reproduce the above
       copyright notice, this list of conditions and the following
       disclaimer in the documentation and/or other materials provided
       with the distribution.
     * Neither the name of Shimmer Research, Ltd. nor the names of its
       contributors may be used to endorse or promote products derived
       from this software without specific prior written permission.

 THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
