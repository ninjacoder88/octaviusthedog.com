using System;

namespace OctaviusTheDog.Models
{
    public class HomeIndexModel
    {
        public double AgeInMonths => Math.Round((Today.Subtract(Birthdate).TotalDays) * 1.0 / 30.416, 1);

        public DateTime Birthdate => new DateTime(2020, 10, 08);

        public DateTime Today => DateTime.Today;

        public string BirthdateString => Birthdate.ToString("d");

        public int CurrentWeight => 63;
    }
}
