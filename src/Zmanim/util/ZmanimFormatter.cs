// * Zmanim .NET API
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using java.text;

namespace net.sourceforge.zmanim.util
{
    /// <summary>
    ///   A class used to format non <see cref = "java.util.Date" /> times generated by the
    ///   Zmanim package. For example the
    ///   <see cref = "net.sourceforge.zmanim.AstronomicalCalendar.getTemporalHour()" /> returns
    ///   the length of the hour in milliseconds. This class can format this time.
    /// </summary>
    /// <author>Eliyahu Hershfeld</author>
    public class ZmanimFormatter
    {
        // private DecimalFormat decimalNF;

        ///<summary>
        ///  Format using hours, minutes, seconds and milliseconds using the xsd:time
        ///  format. This format will return 00.00.00.0 when formatting 0.
        ///</summary>
        public const int SEXAGESIMAL_XSD_FORMAT = 0;

        ///<summary>
        ///  Format using standard decimal format with 5 positions after the decimal.
        ///</summary>
        public const int DECIMAL_FORMAT = 1;

        /// <summary>
        ///   Format using hours and minutes.
        /// </summary>
        public const int SEXAGESIMAL_FORMAT = 2;

        /// <summary>
        ///   Format using hours, minutes and seconds.
        /// </summary>
        public const int SEXAGESIMAL_SECONDS_FORMAT = 3;

        /// <summary>
        ///   Format using hours, minutes, seconds and milliseconds.
        /// </summary>
        public const int SEXAGESIMAL_MILLIS_FORMAT = 4;

        /// <summary>
        ///   constant for milliseconds in a minute (60,000)
        /// </summary>
        internal const long MINUTE_MILLIS = 60 * 1000;

        /// <summary>
        ///   constant for milliseconds in an hour (3,600,000)
        /// </summary>
        public const long HOUR_MILLIS = MINUTE_MILLIS * 60;

        ///<summary>
        ///  Format using the XSD Duration format. This is in the format of
        ///  PT1H6M7.869S (P for period (duration), T for time, H, M and S indicate
        ///  hours, minutes and seconds.
        ///</summary>
        public const int XSD_DURATION_FORMAT = 5;

        private static readonly DecimalFormat minuteSecondNF = new DecimalFormat("00");
        private static readonly DecimalFormat milliNF = new DecimalFormat("000");
        private readonly DecimalFormat hourNF;
        private SimpleDateFormat dateFormat;
        private bool prependZeroHours;
        private int timeFormat = SEXAGESIMAL_XSD_FORMAT;
        internal bool useDecimal;
        private bool useMillis;
        private bool useSeconds;

        /// <summary>
        /// </summary>
        public ZmanimFormatter()
            : this(0, new SimpleDateFormat("h:mm:ss"))
        {
        }

        ///<summary>
        ///  ZmanimFormatter constructor using a formatter
        ///</summary>
        ///<param name = "format">
        ///  int The formatting style to use. Using
        ///  ZmanimFormatter.SEXAGESIMAL_SECONDS_FORMAT will format the
        ///  time time of 90*60*1000 + 1 as 1:30:00 </param>
        ///<param name = "dateFormat">The date format.</param>
        public ZmanimFormatter(int format, SimpleDateFormat dateFormat)
        {
            string hourFormat = "0";
            if (prependZeroHours)
            {
                hourFormat = "00";
            }
            hourNF = new DecimalFormat(hourFormat);
            // decimalNF = new DecimalFormat("0.0####");
            setTimeFormat(format);
            setDateFormat(dateFormat);
        }

        ///<summary>
        ///  Sets the format to use for formatting.
        ///</summary>
        ///<param name = "format">
        ///  int the format constant to use. </param>
        public virtual void setTimeFormat(int format)
        {
            timeFormat = format;
            switch (format)
            {
                case SEXAGESIMAL_XSD_FORMAT:
                    setSettings(true, true, true);
                    break;
                case SEXAGESIMAL_FORMAT:
                    setSettings(false, false, false);
                    break;
                case SEXAGESIMAL_SECONDS_FORMAT:
                    setSettings(false, true, false);
                    break;
                case SEXAGESIMAL_MILLIS_FORMAT:
                    setSettings(false, true, true);
                    break;
                case DECIMAL_FORMAT:
                default:
                    useDecimal = true;
                    break;
            }
        }

        /// <summary>
        ///   Sets the date format.
        /// </summary>
        /// <param name = "sdf"></param>
        public virtual void setDateFormat(SimpleDateFormat sdf)
        {
            dateFormat = sdf;
        }

        /// <summary>
        ///   Gets the date format.
        /// </summary>
        /// <returns></returns>
        public virtual SimpleDateFormat getDateFormat()
        {
            return dateFormat;
        }

        private void setSettings(bool prependZeroHours, bool useSeconds, bool useMillis)
        {
            this.prependZeroHours = prependZeroHours;
            this.useSeconds = useSeconds;
            this.useMillis = useMillis;
        }

        ///<summary>
        ///  A method that formats milliseconds into a time format.
        ///</summary>
        ///<param name = "milliseconds">
        ///  The time in milliseconds. </param>
        ///<returns> String The formatted <c>String</c> </returns>
        public virtual string format(double milliseconds)
        {
            return format((int)milliseconds);
        }

        ///<summary>
        ///  A method that formats milliseconds into a time format.
        ///</summary>
        ///<param name = "millis">
        ///  The time in milliseconds. </param>
        ///<returns> String The formatted <c>String</c> </returns>
        public virtual string format(int millis)
        {
            return format(new Time(millis));
        }

        ///<summary>
        ///  A method that formats <see cref = "Time" />objects.
        ///</summary>
        ///<param name = "time">
        ///  The time <c>Object</c> to be formatted. </param>
        ///<returns> String The formatted <c>String</c> </returns>
        public virtual string format(Time time)
        {
            if (timeFormat == XSD_DURATION_FORMAT)
            {
                return formatXSDDurationTime(time);
            }
            var sb = new StringBuilder();
            sb.Append(hourNF.format(time.getHours()));
            sb.Append(":");
            sb.Append(minuteSecondNF.format(time.getMinutes()));
            if (useSeconds)
            {
                sb.Append(":");
                sb.Append(minuteSecondNF.format(time.getSeconds()));
            }
            if (useMillis)
            {
                sb.Append(".");
                sb.Append(milliNF.format(time.getMilliseconds()));
            }
            return sb.ToString();
        }

        ///<summary>
        ///  Formats a date using this classe's <see cref = "getDateFormat()">date format</see>.
        ///</summary>
        ///<param name = "Date">
        ///  the date to format </param>
        ///<param name = "timeZoneDateTime">
        ///  the <see cref = "java.util.Calendar">Calendar</see> used to help format
        ///  based on the Calendar's DST and other settings. </param>
        ///<returns> the formatted string </returns>
        public virtual string formatDate(DateTime Date, ITimeZoneDateTime timeZoneDateTime)
        {
            //dateFormat.setCalendar(new GregorianCalendar(
            //    timeZoneDateTime.Date.Year, timeZoneDateTime.Date.Month, timeZoneDateTime.Date.Day,
            //    timeZoneDateTime.Date.Hour, timeZoneDateTime.Date.Minute, timeZoneDateTime.Date.Second));

            if (dateFormat.toPattern().Equals("yyyy-MM-dd'T'HH:mm:ss"))
            {
                return getXSDate(Date, timeZoneDateTime);
            }
            else
            {
                return Date.ToString(dateFormat.toPattern());
                return dateFormat.format(Date);
            }
        }

        ///<summary>
        ///  The date:date-time function returns the current date and time as a
        ///  date/time string. The date/time string that's returned must be a string
        ///  in the format defined as the lexical representation of xs:Date in
        ///  <a href = "http://www.w3.org/TR/xmlschema11-2/#Date">[3.3.8 Date]</a>
        ///  of <a href = "http://www.w3.org/TR/xmlschema11-2/">[XML Schema 1.1 Part 2:
        ///       Datatypes]</a>. The date/time format is basically CCYY-MM-DDThh:mm:ss,
        ///  although implementers should consult <a href = "http://www.w3.org/TR/xmlschema11-2/">[XML Schema 1.1 Part 2:
        ///                                         Datatypes]</a> and <a href = "http://www.iso.ch/markete/8601.pdf">[ISO
        ///                                                              8601]</a> for details. The date/time string format must include a time
        ///  zone, either a Z to indicate Coordinated Universal Time or a + or -
        ///  followed by the difference between the difference from UTC represented as
        ///  hh:mm.
        ///</summary>
        public virtual string getXSDate(DateTime Date, ITimeZoneDateTime cal)
        {
            string xsdDateFormat = "yyyy-MM-dd'T'HH:mm:ss";
            //        
            //		 * if (xmlDateFormat == null || xmlDateFormat.trim().equals("")) {
            //		 * xmlDateFormat = xsdDateFormat; }
            //		 
            var dateFormat = new SimpleDateFormat(xsdDateFormat);

            var buff = new StringBuilder(dateFormat.format(Date));
            // Must also include offset from UTF.
            // Get the offset (in milliseconds).
            //int offset = cal.get(Calendar.ZONE_OFFSET) + cal.get(Calendar.DST_OFFSET);
            int offset = cal.TimeZone.getRawOffset() + cal.TimeZone.getDSTSavings();
            // If there is no offset, we have "Coordinated
            // Universal Time."
            if (offset == 0)
                buff.Append("Z");
            else
            {
                // Convert milliseconds to hours and minutes
                int hrs = offset / (60 * 60 * 1000);
                // In a few cases, the time zone may be +/-hh:30.
                int min = offset % (60 * 60 * 1000);
                char posneg = hrs < 0 ? '-' : '+';
                buff.Append(posneg + formatDigits(hrs) + ':' + formatDigits(min));
            }
            return buff.ToString();
        }

        ///<summary>
        ///  Represent the hours and minutes with two-digit strings.
        ///</summary>
        ///<param name = "digits"> hours or minutes. </param>
        ///<returns> two-digit String representation of hrs or minutes. </returns>
        private static string formatDigits(int digits)
        {
            string dd = Convert.ToString(Math.Abs(digits));
            return dd.Length == 1 ? '0' + dd : dd;
        }

        ///<summary>
        ///  This returns the xml representation of an xsd:duration object.
        ///</summary>
        ///<param name = "millis"> the duration in milliseconds </param>
        ///<returns> the xsd:duration formatted String </returns>
        public virtual string formatXSDDurationTime(long millis)
        {
            return formatXSDDurationTime(new Time(millis));
        }

        ///<summary>
        ///  This returns the xml representation of an xsd:duration object.
        ///</summary>
        ///<param name = "time"> the duration as a Time object  </param>
        ///<returns> the xsd:duration formatted String </returns>
        public virtual string formatXSDDurationTime(Time time)
        {
            var duration = new StringBuilder();

            duration.Append("P");

            if (time.getHours() != 0 || time.getMinutes() != 0 || time.getSeconds() != 0 || time.getMilliseconds() != 0)
            {
                duration.Append("T");

                if (time.getHours() != 0)
                    duration.Append(time.getHours() + "H");

                if (time.getMinutes() != 0)
                    duration.Append(time.getMinutes() + "M");

                if (time.getSeconds() != 0 || time.getMilliseconds() != 0)
                {
                    duration.Append(time.getSeconds() + "." + milliNF.format(time.getMilliseconds()));
                    duration.Append("S");
                }
                if (duration.Length == 1) // zero seconds
                    duration.Append("T0S");
                if (time.IsNegative())
                    duration.Insert(0, "-");
            }
            return duration.ToString();
        }

        ///<summary>
        ///  A method that returns an XML formatted <c>String</c> representing
        ///  the serialized <c>Object</c>. The format used is:
        ///	
        ///  <code>
        ///    &lt;AstronomicalTimes date=&quot;1969-02-08&quot; type=&quot;net.sourceforge.zmanim.AstronomicalCalendar algorithm=&quot;US Naval Almanac Algorithm&quot; location=&quot;Lakewood, NJ&quot; latitude=&quot;40.095965&quot; longitude=&quot;-74.22213&quot; elevation=&quot;25.4&quot; timeZoneName=&quot;Eastern Standard Time&quot; timeZoneID=&quot;America/New_York&quot; timeZoneOffset=&quot;-5&quot;&gt;
        ///    &lt;Sunrise&gt;2007-02-18T06:45:27-05:00&lt;/Sunrise&gt;
        ///    &lt;TemporalHour&gt;PT54M17.529S&lt;/TemporalHour&gt;
        ///    ...
        ///    &lt;/AstronomicalTimes&gt;
        ///  </code>
        ///	
        ///  Note that the output uses the <a href = "http://www.w3.org/TR/xmlschema11-2/#Date">xsd:Date</a>
        ///  format for times such as sunrise, and <a href = "http://www.w3.org/TR/xmlschema11-2/#duration">xsd:duration</a>
        ///  format for times that are a duration such as the length of a
        ///  <see cref = "net.sourceforge.zmanim.AstronomicalCalendar.getTemporalHour()">temporal hour</see>.
        ///  The output of this method is returned by the <see cref = "AstronomicalCalendar.ToString" /> }.
        ///</summary>
        ///<returns> The XML formatted <c>String</c>. The format will be:
        ///	
        ///  <code>
        ///    &lt;AstronomicalTimes date=&quot;1969-02-08&quot; type=&quot;net.sourceforge.zmanim.AstronomicalCalendar algorithm=&quot;US Naval Almanac Algorithm&quot; location=&quot;Lakewood, NJ&quot; latitude=&quot;40.095965&quot; longitude=&quot;-74.22213&quot; elevation=&quot;25.4&quot; timeZoneName=&quot;Eastern Standard Time&quot; timeZoneID=&quot;America/New_York&quot; timeZoneOffset=&quot;-5&quot;&gt;
        ///    &lt;Sunrise&gt;2007-02-18T06:45:27-05:00&lt;/Sunrise&gt;
        ///    &lt;TemporalHour&gt;PT54M17.529S&lt;/TemporalHour&gt;
        ///    ...
        ///    &lt;/AstronomicalTimes&gt;
        ///  </code>
        ///</returns>
        public static string toXML(AstronomicalCalendar ac)
        {
            var formatter = new ZmanimFormatter(XSD_DURATION_FORMAT, new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss"));
            DateFormat df = new SimpleDateFormat("yyyy-MM-dd");
            var output = new StringBuilder("<");

            if (ac.GetType().Name.EndsWith("AstronomicalCalendar"))
            {
                output.Append("AstronomicalTimes");
            }
            else if (ac.GetType().Name.EndsWith("ZmanimCalendar"))
            {
                output.Append("Zmanim");
            }
            output.Append(" date=\"" + df.format(ac.getCalendar().Date) + "\"");
            output.Append(" type=\"" + ac.GetType().Name + "\"");
            output.Append(" algorithm=\"" + ac.getAstronomicalCalculator().getCalculatorName() + "\"");
            output.Append(" location=\"" + ac.getGeoLocation().getLocationName() + "\"");
            output.Append(" latitude=\"" + ac.getGeoLocation().getLatitude() + "\"");
            output.Append(" longitude=\"" + ac.getGeoLocation().getLongitude() + "\"");
            output.Append(" elevation=\"" + ac.getGeoLocation().getElevation() + "\"");
            output.Append(" timeZoneName=\"" + ac.getGeoLocation().getTimeZone().getDisplayName() + "\"");
            output.Append(" timeZoneID=\"" + ac.getGeoLocation().getTimeZone().getID() + "\"");
            output.Append(" timeZoneOffset=\"" +
                          (ac.getGeoLocation().getTimeZone().getOffset(ac.getCalendar().Date.ToFileTime()) /
                           ((double)HOUR_MILLIS)) + "\"");

            output.Append(">\n");

            MethodInfo[] theMethods = ac.GetType().GetMethods();
            string tagName = "";
            object @value = null;
            IList<Zman> dateList = new List<Zman>();
            IList<Zman> durationList = new List<Zman>();
            IList<string> otherList = new List<string>();
            for (int i = 0; i < theMethods.Length; i++)
            {
                if (includeMethod(theMethods[i]))
                {
                    tagName = theMethods[i].Name.Substring(3);
                    //String returnType = theMethods[i].getReturnType().getName();
                    try
                    {
                        @value = theMethods[i].Invoke(ac, null);
                        if (@value == null) //FIXME: use reflection to determine what the return type is, not the value
                        {
                            otherList.Add("<" + tagName + ">N/A</" + tagName + ">");
                        }
                        else if (@value is DateTime)
                        {
                            dateList.Add(new Zman((DateTime)@value, tagName));
                        } // shaah zmanis
                        else if (@value is long?)
                        {
                            durationList.Add(new Zman((int)((long?)@value), tagName));
                        } // will probably never enter this block, but is
                        else
                        {
                            // present to be future proof
                            otherList.Add("<" + tagName + ">" + @value + "</" + tagName + ">");
                        }
                    }
                    catch (Exception e)
                    {
                        output.Append(e.StackTrace);
                        throw;
                    }
                }
            }

            foreach (Zman zman in dateList.OrderBy(x => x.getZman()))
            {
                output.Append("\t<" + zman.getZmanLabel());
                output.Append(">");
                output.Append(formatter.formatDate(zman.getZman(), ac.getCalendar())
                              + "</" + zman.getZmanLabel() + ">\n");
            }

            foreach (Zman zman in durationList.OrderBy(x => x.getDuration()))
            {
                output.Append("\t<" + zman.getZmanLabel());
                output.Append(">");
                output.Append(formatter.format((int)zman.getDuration()) + "</"
                              + zman.getZmanLabel() + ">\n");
            }

            foreach (string t in otherList)
            {
                // will probably never enter this block
                output.Append("\t" + t + "\n");
            }

            if (ac.GetType().Name.EndsWith("AstronomicalCalendar"))
            {
                output.Append("</AstronomicalTimes>");
            }
            else if (ac.GetType().Name.EndsWith("ZmanimCalendar"))
            {
                output.Append("</Zmanim>");
            }

            return output.ToString();
        }

        ///<summary>
        ///  Determines if a method should be output by the <see cref = "toXML(AstronomicalCalendar)" />
        ///</summary>
        ///<param name = "method"> Should this method be inculeded. </param>
        private static bool includeMethod(MethodInfo method)
        {
            IList<string> methodWhiteList = new List<string>();
            // methodWhiteList.add("getName");

            IList<string> methodBlackList = new List<string>();
            // methodBlackList.add("getGregorianChange");

            if (methodWhiteList.Contains(method.Name))
                return true;
            if (methodBlackList.Contains(method.Name))
                return false;

            if (method.GetParameters().Length > 0)
                return false; // Skip get methods with parameters since we do not
            // know what value to pass

            if (!method.Name.StartsWith("get"))
                return false;

            if (method.ReturnType == typeof(DateTime)
                || method.ReturnType == typeof(long))
            {
                return true;
            }
            return false;
        }
    }
}