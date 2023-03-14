[![Build status](https://ci.appveyor.com/api/projects/status/0l0bjmkcv1hihmq3?svg=true)](https://ci.appveyor.com/project/Yitzchok/zmanim)

This project is a port from the [Java zmanim-project](http://www.kosherjava.com/zmanim-project/) developed by Eliyahu Hershfeld.

The _Zmanim_ ("times" referring to the calculations of time that govern the start and end time of Jewish prayers and holidays)
project is a .NET API for generating zmanim from within .NET programs.
If you are a non programmer, this means that the software created by the project is a building block of code to allow other programmers to easily include zmanim in their programs.
The basis for most zmanim in this class are from the sefer Yisroel Vehazmanim by Rabbi Yisroel Dovid Harfenes.

The code available under the LGPL license.
Please note: due to atmospheric conditions (pressure, humidity and other conditions), calculating zmanim accurately is very complex.
The calculation of zmanim is dependant on Atmospheric refraction (refraction of sunlight through the atmosphere), and zmanim can be off by up to 2 minutes based on atmospheric conditions.
Inaccuracy is increased by elevation. It is not the intent of this API to provide any guarantee of accuracy.

Forked from Yitzchok/Zmanim
Primarily fixed bugs on DST transition in function AstrononicalCalendar.GetDateFromTime(). 'DateWithLocation.Location.TimeZone.UtcOffset(utcDateTime)' does not offset correctly if UTC time is before DST change but local time is in DST (e.g. getting sunrise in Australia on first day of transition) or vice versa (e.g. getting tzais in Los Angeles on last day of DST when it is already standard time in UTC). Also added date adjustment


TODO:
    * Make it Linq friendly.
    * Add examples how to use this project in a ASP.NET MVC site and WPF Application.
