﻿// * Zmanim .NET API
// * Copyright (C) 2004-2010 Eliyahu Hershfeld
// *
// * Converted to C# by AdminJew
// *
// * This file is part of Zmanim .NET API.
// *
// * Zmanim .NET API is free software: you can redistribute it and/or modify
// * it under the terms of the GNU Lesser General Public License as published by
// * the Free Software Foundation, either version 3 of the License, or
// * (at your option) any later version.
// *
// * Zmanim .NET API is distributed in the hope that it will be useful,
// * but WITHOUT ANY WARRANTY; without even the implied warranty of
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// * GNU Lesser General Public License for more details.
// *
// * You should have received a copy of the GNU Lesser General Public License
// * along with Zmanim.NET API.  If not, see <http://www.gnu.org/licenses/lgpl.html>.

using System;
using java.util;
using Math = java.lang.Math;

namespace net.sourceforge.zmanim.util
{
    /// <summary>
    ///   Implementation of sunrise and sunset methods to calculate astronomical times.
    ///   This calculator uses the Java algorithm written by <a href = "http://www.jstot.me.uk/jsuntimes/">Jonathan Stott</a> that is based on
    ///   the implementation by <a href = "http://noaa.gov">NOAA - National Oceanic and
    ///                           Atmospheric Administration</a>'s <a href = "http://www.srrb.noaa.gov/highlights/sunrise/sunrisehtml">Surface Radiation
    ///                                                              Research Branch</a>. NOAA's <a href = "http://www.srrb.noaa.gov/highlights/sunrise/solareqns.PDF">implementation</a>
    ///   is based on equations from <a href = "http://www.willbell.com/math/mc1.htm">Astronomical Algorithms</a> by
    ///   <a href = "http://en.wikipedia.org/wiki/Jean_Meeus">Jean Meeus</a>. Jonathan's
    ///   implementation was released under the GPL. Added to the algorithm is an
    ///   adjustment of the zenith to account for elevation.
    /// </summary>
    /// <seealso cref = "net.sourceforge.zmanim.util.NOAACalculator" />
    /// <author>Jonathan Stott</author>
    /// <author>Eliyahu Hershfeld</author>
    [Obsolete(
        "This class is based on the NOAA algorithm but does not return calculations that match the NOAA algorithm JavaScript implementation. The calculations are about 2 minutes off. This call has been replaced by the NOAACalculator class."
        )]
    public class JSuntimeCalculator : AstronomicalCalculator
    {
        private string calculatorName = "US National Oceanic and Atmospheric Administration Algorithm";

        ///<seealso cref = "net.sourceforge.zmanim.util.NOAACalculator.getCalculatorName()" />
        [Obsolete]
        public override string getCalculatorName()
        {
            return calculatorName;
        }

        ///<seealso cref = "net.sourceforge.zmanim.util.NOAACalculator.getUTCSunrise(AstronomicalCalendar, double, bool)" />
        ///<seealso cref = "net.sourceforge.zmanim.util.AstronomicalCalculator.getUTCSunrise(AstronomicalCalendar,double, bool)" />
        ///<exception cref = "ZmanimException">
        ///  if the year entered == 2000. This calculator can't properly
        ///  deal with the year 2000. It can properly calculate times for
        ///  years &gt; &lt; 2000. </exception>
        [Obsolete]
        public override double getUTCSunrise(AstronomicalCalendar astronomicalCalendar, double zenith,
                                             bool adjustForElevation)
        {
            //		if (astronomicalCalendar.getCalendar().get(Calendar.YEAR) == 2000) {
            //			throw new ZmanimException(
            //					"JSuntimeCalculator can not calculate times for the year 2000. Please try a date with a different year.");
            //		}

            if (adjustForElevation)
            {
                zenith = adjustZenith(zenith, astronomicalCalendar.getGeoLocation().getElevation());
            }
            else
            {
                zenith = adjustZenith(zenith, 0);
            }
            double timeMins = morningPhenomenon(dateToJulian(astronomicalCalendar.getCalendar()),
                                                astronomicalCalendar.getGeoLocation().getLatitude(),
                                                -astronomicalCalendar.getGeoLocation().getLongitude(), zenith);
            return timeMins/60;
        }

        ///<seealso cref = "net.sourceforge.zmanim.util.NOAACalculator.getUTCSunset(AstronomicalCalendar, double, bool)" />
        ///<seealso cref = "net.sourceforge.zmanim.util.AstronomicalCalculator.getUTCSunset(AstronomicalCalendar, double, bool)" />
        ///<exception cref = "ZmanimException">
        ///  if the year entered == 2000. This calculator can't properly
        ///  deal with the year 2000. It can properly calculate times for
        ///  years &gt; &lt; 2000. </exception>
        [Obsolete]
        public override double getUTCSunset(AstronomicalCalendar astronomicalCalendar, double zenith,
                                            bool adjustForElevation)
        {
            //		if (astronomicalCalendar.getCalendar().get(Calendar.YEAR) == 2000) {
            //			throw new ZmanimException(
            //					"JSuntimeCalculator can not calculate times for the year 2000. Please try a date with a different year.");
            //		}

            if (adjustForElevation)
            {
                zenith = adjustZenith(zenith, astronomicalCalendar.getGeoLocation().getElevation());
            }
            else
            {
                zenith = adjustZenith(zenith, 0);
            }
            double timeMins = eveningPhenomenon(dateToJulian(astronomicalCalendar.getCalendar()),
                                                astronomicalCalendar.getGeoLocation().getLatitude(),
                                                -astronomicalCalendar.getGeoLocation().getLongitude(), zenith);
            return timeMins/60;
        }

        ///<summary>
        ///  Calculate the UTC of a morning phenomenon for the given day at the given
        ///  latitude and longitude on Earth
        ///</summary>
        ///<param name = "julian">
        ///  Julian day </param>
        ///<param name = "latitude">
        ///  latitude of observer in degrees </param>
        ///<param name = "longitude">
        ///  longitude of observer in degrees </param>
        ///<param name = "zenithDistance">
        ///  one of Sun.SUNRISE_SUNSET_ZENITH_DISTANCE,
        ///  Sun.CIVIL_TWILIGHT_ZENITH_DISTANCE,
        ///  Sun.NAUTICAL_TWILIGHT_ZENITH_DISTANCE,
        ///  Sun.ASTRONOMICAL_TWILIGHT_ZENITH_DISTANCE. </param>
        ///<returns> time in minutes from zero Z </returns>
        private static double morningPhenomenon(double julian, double latitude, double longitude, double zenithDistance)
        {
            double t = julianDayToJulianCenturies(julian);
            double eqtime = equationOfTime(t);
            double solarDec = sunDeclination(t);
            double hourangle = hourAngleMorning(latitude, solarDec, zenithDistance);
            double delta = longitude - Math.toDegrees(hourangle);
            double timeDiff = 4*delta;
            double timeUTC = 720 + timeDiff - eqtime;

            // Second pass includes fractional julian day in gamma calc
            double newt = julianDayToJulianCenturies(julianCenturiesToJulianDay(t) + timeUTC/1440);
            eqtime = equationOfTime(newt);
            solarDec = sunDeclination(newt);
            hourangle = hourAngleMorning(latitude, solarDec, zenithDistance);
            delta = longitude - Math.toDegrees(hourangle);
            timeDiff = 4*delta;

            double morning = 720 + timeDiff - eqtime;
            return morning;
        }

        ///<summary>
        ///  Calculate the UTC of an evening phenomenon for the given day at the given
        ///  latitude and longitude on Earth
        ///</summary>
        ///<param name = "julian">
        ///  Julian day </param>
        ///<param name = "latitude">
        ///  latitude of observer in degrees </param>
        ///<param name = "longitude">
        ///  longitude of observer in degrees </param>
        ///<param name = "zenithDistance">
        ///  one of Sun.SUNRISE_SUNSET_ZENITH_DISTANCE,
        ///  Sun.CIVIL_TWILIGHT_ZENITH_DISTANCE,
        ///  Sun.NAUTICAL_TWILIGHT_ZENITH_DISTANCE,
        ///  Sun.ASTRONOMICAL_TWILIGHT_ZENITH_DISTANCE. </param>
        ///<returns> time in minutes from zero Z </returns>
        private static double eveningPhenomenon(double julian, double latitude, double longitude, double zenithDistance)
        {
            double t = julianDayToJulianCenturies(julian);

            // First calculates sunrise and approx length of day
            double eqtime = equationOfTime(t);
            double solarDec = sunDeclination(t);
            double hourangle = hourAngleEvening(latitude, solarDec, zenithDistance);

            double delta = longitude - Math.toDegrees(hourangle);
            double timeDiff = 4*delta;
            double timeUTC = 720 + timeDiff - eqtime;

            // first pass used to include fractional day in gamma calc
            double newt = julianDayToJulianCenturies(julianCenturiesToJulianDay(t) + timeUTC/1440);
            eqtime = equationOfTime(newt);
            solarDec = sunDeclination(newt);
            hourangle = hourAngleEvening(latitude, solarDec, zenithDistance);

            delta = longitude - Math.toDegrees(hourangle);
            timeDiff = 4*delta;

            double evening = 720 + timeDiff - eqtime;
            return evening;
        }

        private static double dateToJulian(Calendar date)
        {
            int year = date.get(Calendar.YEAR);
            int month = date.get(Calendar.MONTH) + 1;
            int day = date.get(Calendar.DAY_OF_MONTH);
            int hour = date.get(Calendar.HOUR_OF_DAY);
            int minute = date.get(Calendar.MINUTE);
            int second = date.get(Calendar.SECOND);

            double extra = (100.0*year) + month - 190002.5;
            double JD = (367.0*year) - (System.Math.Floor(7.0*(year + System.Math.Floor((month + 9.0)/12.0))/4.0)) +
                        System.Math.Floor((275.0*month)/9.0) + day + ((hour + ((minute + (second/60.0))/60.0))/24.0) +
                        1721013.5 - ((0.5*extra)/System.Math.Abs(extra)) + 0.5;
            return JD;
        }

        ///<summary>
        ///  Convert Julian Day to centuries since J2000.0
        ///</summary>
        ///<param name = "julian">
        ///  The Julian Day to convert </param>
        ///<returns> the value corresponding to the Julian Day </returns>
        private static double julianDayToJulianCenturies(double julian)
        {
            return (julian - 2451545)/36525;
        }

        ///<summary>
        ///  Convert centuries since J2000.0 to Julian Day
        ///</summary>
        ///<param name = "t">
        ///  Number of Julian centuries since J2000.0 </param>
        ///<returns> The Julian Day corresponding to the value of t </returns>
        private static double julianCenturiesToJulianDay(double t)
        {
            return (t*36525) + 2451545;
        }

        ///<summary>
        ///  Calculate the difference between true solar time and mean solar time
        ///</summary>
        ///<param name = "t">Number of Julian centuries since J2000.0</param>
        private static double equationOfTime(double t)
        {
            double epsilon = obliquityCorrection(t);
            double l0 = geomMeanLongSun(t);
            double e = eccentricityOfEarthsOrbit(t);
            double m = geometricMeanAnomalyOfSun(t);
            double y = System.Math.Pow((System.Math.Tan(Math.toRadians(epsilon)/2)), 2);

            double eTime = y*System.Math.Sin(2*Math.toRadians(l0)) - 2*e*System.Math.Sin(Math.toRadians(m)) +
                           4*e*y*System.Math.Sin(Math.toRadians(m))*System.Math.Cos(2*Math.toRadians(l0)) -
                           0.5*y*y*System.Math.Sin(4*Math.toRadians(l0)) - 1.25*e*e*System.Math.Sin(2*Math.toRadians(m));
            return Math.toDegrees(eTime)*4;
        }

        ///<summary>
        ///  Calculate the declination of the sun
        ///</summary>
        ///<param name = "t">
        ///  Number of Julian centuries since J2000.0 </param>
        ///<returns> The Sun's declination in degrees </returns>
        private static double sunDeclination(double t)
        {
            double e = obliquityCorrection(t);
            double lambda = sunsApparentLongitude(t);

            double sint = System.Math.Sin(Math.toRadians(e))*System.Math.Sin(Math.toRadians(lambda));
            return Math.toDegrees(System.Math.Asin(sint));
        }

        ///<summary>
        ///  calculate the hour angle of the sun for a morning phenomenon for the
        ///  given latitude
        ///</summary>
        ///<param name = "lat">
        ///  Latitude of the observer in degrees </param>
        ///<param name = "solarDec">
        ///  declination of the sun in degrees </param>
        ///<param name = "zenithDistance">
        ///  zenith distance of the sun in degrees </param>
        ///<returns> hour angle of sunrise in radians </returns>
        private static double hourAngleMorning(double lat, double solarDec, double zenithDistance)
        {
            return
                (System.Math.Acos(System.Math.Cos(Math.toRadians(zenithDistance))/
                                  (System.Math.Cos(Math.toRadians(lat))*System.Math.Cos(Math.toRadians(solarDec))) -
                                  System.Math.Tan(Math.toRadians(lat))*System.Math.Tan(Math.toRadians(solarDec))));
        }

        ///<summary>
        ///  Calculate the hour angle of the sun for an evening phenomenon for the
        ///  given latitude
        ///</summary>
        ///<param name = "lat">
        ///  Latitude of the observer in degrees </param>
        ///<param name = "solarDec">
        ///  declination of the Sun in degrees </param>
        ///<param name = "zenithDistance">
        ///  zenith distance of the sun in degrees </param>
        ///<returns> hour angle of sunset in radians </returns>
        private static double hourAngleEvening(double lat, double solarDec, double zenithDistance)
        {
            return -hourAngleMorning(lat, solarDec, zenithDistance);
        }

        ///<summary>
        ///  Calculate the corrected obliquity of the ecliptic
        ///</summary>
        ///<param name = "t">
        ///  Number of Julian centuries since J2000.0 </param>
        ///<returns> Corrected obliquity in degrees </returns>
        private static double obliquityCorrection(double t)
        {
            return meanObliquityOfEcliptic(t) + 0.00256*System.Math.Cos(Math.toRadians(125.04 - 1934.136*t));
        }

        ///<summary>
        ///  Calculate the mean obliquity of the ecliptic
        ///</summary>
        ///<param name = "t">
        ///  Number of Julian centuries since J2000.0 </param>
        ///<returns> Mean obliquity in degrees </returns>
        private static double meanObliquityOfEcliptic(double t)
        {
            return 23 + (26 + (21.448 - t*(46.815 + t*(0.00059 - t*(0.001813)))/60))/60;
        }

        ///<summary>
        ///  Calculate the geometric mean longitude of the sun
        ///</summary>
        ///<param name = "t">
        ///  number of Julian centuries since J2000.0 </param>
        ///<returns> the geometric mean longitude of the sun in degrees </returns>
        private static double geomMeanLongSun(double t)
        {
            double l0 = 280.46646 + t*(36000.76983 + 0.0003032*t);

            while ((l0 >= 0) && (l0 <= 360))
            {
                if (l0 > 360)
                {
                    l0 = l0 - 360;
                }

                if (l0 < 0)
                {
                    l0 = l0 + 360;
                }
            }
            return l0;
        }

        ///<summary>
        ///  Calculate the eccentricity of Earth's orbit
        ///</summary>
        ///<param name = "t">
        ///  Number of Julian centuries since J2000.0 </param>
        ///<returns> the eccentricity </returns>
        private static double eccentricityOfEarthsOrbit(double t)
        {
            return 0.016708634 - t*(0.000042037 + 0.0000001267*t);
        }

        ///<summary>
        ///  Calculate the geometric mean anomaly of the Sun
        ///</summary>
        ///<param name = "t">
        ///  Number of Julian centuries since J2000.0 </param>
        ///<returns> the geometric mean anomaly of the Sun in degrees </returns>
        private static double geometricMeanAnomalyOfSun(double t)
        {
            return 357.52911 + t*(35999.05029 - 0.0001537*t);
        }

        ///<summary>
        ///  Calculate the apparent longitude of the sun
        ///</summary>
        ///<param name = "t">
        ///  Number of Julian centuries since J2000.0 </param>
        ///<returns> The apparent longitude of the Sun in degrees </returns>
        private static double sunsApparentLongitude(double t)
        {
            return sunsTrueLongitude(t) - 0.00569 - 0.00478*System.Math.Sin(Math.toRadians(125.04 - 1934.136*t));
        }

        ///<summary>
        ///  Calculate the true longitude of the sun
        ///</summary>
        ///<param name = "t">
        ///  Number of Julian centuries since J2000.0 </param>
        ///<returns> The Sun's true longitude in degrees </returns>
        private static double sunsTrueLongitude(double t)
        {
            return geomMeanLongSun(t) + equationOfCentreForSun(t);
        }

        ///<summary>
        ///  Calculate the equation of centre for the Sun
        ///</summary>
        ///<param name = "centuries">
        ///  Number of Julian centuries since J2000.0 </param>
        ///<returns> The equation of centre for the Sun in degrees </returns>
        private static double equationOfCentreForSun(double t)
        {
            double m = geometricMeanAnomalyOfSun(t);

            return System.Math.Sin(Math.toRadians(m))*(1.914602 - t*(0.004817 + 0.000014*t)) +
                   System.Math.Sin(2*Math.toRadians(m))*(0.019993 - 0.000101*t) +
                   System.Math.Sin(3*Math.toRadians(m))*0.000289;
        }
    }
}