using System;
using NUnit.Framework;
using Zmanim;
using Zmanim.TimeZone;
using Zmanim.TzDatebase;
using Zmanim.Utilities;

namespace ZmanimTests
{
    [TestFixture]
    public class TimeZoneTests
    {
        [Test]
        public void LocalTimeConversion_should_correctly_use_time_for_daylight_saving_end_Sunrise_Australia()
        {
            ITimeZone timeZone = new OlsonTimeZone("Australia/Sydney");
            var location = new GeoLocation("Sydney Australia", -33.86, 151, 0, timeZone);
            var czc = new ComplexZmanimCalendar(DateTime.Parse("2023-04-01"), location);
            var sunrise = czc.GetSunrise();
            Assert.That(sunrise.Value.Hour, Is.EqualTo(7));
            
            czc = new ComplexZmanimCalendar(DateTime.Parse("2023-04-02"), location);
            sunrise = czc.GetSunrise();
            Assert.That(sunrise.Value.Hour, Is.EqualTo(6));
        }

        [Test]
        public void LocalTimeConversion_should_correctly_use_time_for_daylight_saving_start_Sunrise_Australia()
        {
            ITimeZone timeZone = new OlsonTimeZone("Australia/Sydney");
            var location = new GeoLocation("Sydney Australia", -33.86, 151, 0, timeZone);
            var czc = new ComplexZmanimCalendar(DateTime.Parse("2023-09-30"), location);
            var sunrise = czc.GetSunrise();
            Assert.That(sunrise.Value.Hour, Is.EqualTo(5));

            czc = new ComplexZmanimCalendar(DateTime.Parse("2023-10-01"), location);
            sunrise = czc.GetSunrise();
            Assert.That(sunrise.Value.Hour, Is.EqualTo(6));
        }

        [Test]
        public void LocalTimeConversion_should_correctly_use_time_for_daylight_saving_end_Sunset_Australia()
        {
            ITimeZone timeZone = new OlsonTimeZone("Australia/Sydney");
            var location = new GeoLocation("Sydney Australia", -33.86, 151, 0, timeZone);
            var czc = new ComplexZmanimCalendar(DateTime.Parse("2023-04-01"), location);
            var sunset = czc.GetSunset();
            Assert.That(sunset.Value.Hour, Is.EqualTo(18));

            czc = new ComplexZmanimCalendar(DateTime.Parse("2023-04-02"), location);
            sunset = czc.GetSunset();
            Assert.That(sunset.Value.Hour, Is.EqualTo(17));
        }

        [Test]
        public void LocalTimeConversion_should_correctly_use_time_for_daylight_saving_start_Sunset_Australia()
        {
            ITimeZone timeZone = new OlsonTimeZone("Australia/Sydney");
            var location = new GeoLocation("Sydney Australia", -33.86, 151, 0, timeZone);
            var czc = new ComplexZmanimCalendar(DateTime.Parse("2023-09-30"), location);
            var sunset = czc.GetSunset();
            Assert.That(sunset.Value.Hour, Is.EqualTo(17));

            czc = new ComplexZmanimCalendar(DateTime.Parse("2023-10-01"), location);
            sunset = czc.GetSunset();
            Assert.That(sunset.Value.Hour, Is.EqualTo(18));
        }

        [Test]
        public void LocalTimeConversion_should_correctly_use_time_for_daylight_saving_start_Tzeis_WestBloomfield()
        {
            //ITimeZone timeZone = new OlsonTimeZone("America/Detroit");
            ITimeZone timeZone = new WindowsTimeZone(TimeZoneInfo.Local);
            var location = new GeoLocation("West Bloomfield", 42.542609, -83.355477, 0, timeZone);
            var czc = new ComplexZmanimCalendar(DateTime.Parse("2023-03-11"), location);
            var tzais = czc.getTzaisByDegrees(6);
            Assert.That(tzais.Value.Hour, Is.EqualTo(19));

            czc = new ComplexZmanimCalendar(DateTime.Parse("2023-03-12"), location);
            tzais = czc.getTzaisByDegrees(6);
            Assert.That(tzais.Value.Hour, Is.EqualTo(20));
        }

        [Test]
        public void LocalTimeConversion_should_correctly_use_time_for_daylight_saving_end_Tzeis_WestBloomfield()
        {
            ITimeZone timeZone = new OlsonTimeZone("America/Detroit");
            var location = new GeoLocation("West Bloomfield", 42.542609, -83.355477, 0, timeZone);
            var czc = new ComplexZmanimCalendar(DateTime.Parse("2023-11-04"), location);
            var tzais = czc.getTzaisByDegrees(6);
            Assert.That(tzais.Value.Hour, Is.EqualTo(18));

            czc = new ComplexZmanimCalendar(DateTime.Parse("2023-11-05"), location);
            tzais = czc.getTzaisByDegrees(6);
            Assert.That(tzais.Value.Hour, Is.EqualTo(17));
        }

        [Test]
        public void LocalTimeConversion_should_correctly_use_time_for_daylight_saving_start_Tzeis_LosAngeles()
        {
            ITimeZone timeZone = new OlsonTimeZone("America/Los_Angeles");
            var location = new GeoLocation("Los Angeles", 34.05527, -118.3991647, 0, timeZone);
            var czc = new ComplexZmanimCalendar(DateTime.Parse("2023-03-11"), location);
            var tzais = czc.getTzaisByDegrees(8.5);
            Assert.That(tzais.Value.Hour, Is.EqualTo(18));

            czc = new ComplexZmanimCalendar(DateTime.Parse("2023-03-12"), location);
            tzais = czc.getTzaisByDegrees(8.5);
            Assert.That(tzais.Value.Hour, Is.EqualTo(19));
        }

        [Test]
        public void LocalTimeConversion_should_correctly_use_time_for_daylight_saving_end_Tzeis_LosAngeles()
        {
            ITimeZone timeZone = new OlsonTimeZone("America/Los_Angeles");
            var location = new GeoLocation("Los Angeles", 34.05527, -118.3991647, 0, timeZone);
            var czc = new ComplexZmanimCalendar(DateTime.Parse("2023-11-04"), location);
            var tzais = czc.getTzaisByDegrees(8.5);
            Assert.That(tzais.Value.Hour, Is.EqualTo(18));

            czc = new ComplexZmanimCalendar(DateTime.Parse("2023-11-05"), location);
            tzais = czc.getTzaisByDegrees(8.5);
            Assert.That(tzais.Value.Hour, Is.EqualTo(17));
        }

        [Test]
        public void Check_is_offset_timezone_working()
        {
            String locationName = "Australia/Sydney";
            double latitude = -33.86; //Australia
            double longitude = 152; //Australlia
            double elevation = 0; //optional elevation
            var timeZone = new OlsonTimeZone(locationName); // OffsetTimeZone(new TimeSpan(0, 0, -14400));
            var location = new GeoLocation(locationName, latitude, longitude, elevation, timeZone);
            var czc = new ComplexZmanimCalendar(new DateTime(2010, 4, 2), location);

            var zman = czc.GetSunrise();

            Assert.That(zman, Is.EqualTo(
                    new DateTime(2010, 4, 2, 7, 4, 24, 834)
                ));
        }
    }
}