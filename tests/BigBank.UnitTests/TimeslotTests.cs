using System;
using BigBank.OLAP.Exceptions;
using BigBank.OLAP.Models;
using FluentAssertions;
using Xunit;

namespace BigBank.UnitTests
{
    public class TimeSlotTests
    {
        [Fact]
        public void DateTimeToTimeSlot_ReturnsZeroTimeSlot_ForOrigin()
        {
            var dt = new DateTime(2018, 1, 1);
            var timeslot = TimeSlot.DateTimeToTimeSlot(dt);

            timeslot.Index.Should().Be(0);
            timeslot.Start.Should().Be(dt);
            timeslot.End.Should().Be(dt.AddSeconds(10_000));
        }

        [Fact]
        public void DateTimeToTimeSlot_ThrowsError_ForDateSmallerThanOrigin()
        {
            var dt = new DateTime(2017, 1, 1);

            Assert.Throws<ValidationException>(() => TimeSlot.DateTimeToTimeSlot(dt));
        }

        [Theory]
        [InlineData(2018, 3, 15, 17, 33, 40)]
        [InlineData(2018, 3, 15, 17, 34, 40)]
        public void DateTimeToTimeSlot_ReturnsCorrectTimeSlot(int year, int month, int day, int hour, int minute, int second)
        {
            var dt = new DateTime(year, month, day, hour, minute, second);
            var expectedTimeslotStart = new DateTime(2018, 3, 15, 17, 26, 40);
            var timeslot = TimeSlot.DateTimeToTimeSlot(dt);

            timeslot.Index.Should().Be(637);
            timeslot.Start.Should().Be(expectedTimeslotStart);
            timeslot.End.Should().Be(expectedTimeslotStart.AddSeconds(10_000));
        }
    }
}