// ------------------------------------------------------------------------------------------------------
// Copyright (c) 2012, Kevin Wang
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the 
// following conditions are met:
//
//  * Redistributions of source code must retain the above copyright notice, this list of conditions and 
//    the following disclaimer.
//  * Redistributions in binary form must reproduce the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ------------------------------------------------------------------------------------------------------

namespace Mirai.Social.Twitter.Commands
{
    using System.Globalization;

    public sealed class TwitterGeoCode
    {
        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public double Radius { get; private set; }

        public TwitterGeoCodeRadiusUnit RadiusUnit { get; private set; }


        public TwitterGeoCode(double latitude, double longitude, double radius, 
                              TwitterGeoCodeRadiusUnit radiusUnit = TwitterGeoCodeRadiusUnit.Miles)
        {
            this.Latitude   = latitude;
            this.Longitude  = longitude;
            this.Radius     = radius;
            this.RadiusUnit = radiusUnit;
        }

        public override string ToString()
        {
            return this.Latitude.ToString(CultureInfo.InvariantCulture) + "," +
                   this.Longitude.ToString(CultureInfo.InvariantCulture) + "," +
                   this.Radius.ToString(CultureInfo.InvariantCulture) + 
                   (RadiusUnit == TwitterGeoCodeRadiusUnit.Miles ? "mi" : "km");
        }
    }

    public enum TwitterGeoCodeRadiusUnit
    {
        Miles,
        Kilometers
    }
}