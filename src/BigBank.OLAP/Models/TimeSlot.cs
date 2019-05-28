using System;
using System.Collections.Generic;
using BigBank.OLAP.Exceptions;
using BigBank.OLAP.Extensions;

namespace BigBank.OLAP.Models
{
    public class TimeSlot : IEquatable<TimeSlot>
    {
        public const int DurationSeconds = 10_000;
        public static readonly DateTime Origin = new DateTime(2018, 1, 1);

        /// <summary>
        /// Timeslot index (number)
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Start of the timeslot
        /// </summary>
        public DateTime Start { get; }

        /// <summary>
        /// End of the timelsot: Start + Dureation(10_000 sec)
        /// </summary>
        public DateTime End { get; }

        public TimeSlot(int index, DateTime start, DateTime end)
        {
            Index = index;
            Start = start;
            End = end;
        }

        public DateTime GetTimeSlotStartDate() => Start;

        public static TimeSlot DateTimeToTimeSlot(DateTime dateTime)
        {
            if (dateTime < Origin)
                throw new ValidationException($"Cannot create TimeSlot for date: '{dateTime.ToIsoDateTimeString()}' smaller than Origin: '{Origin.ToIsoDateTimeString()}'");

            var index = (int)(dateTime - Origin).TotalSeconds / DurationSeconds;
            var start = Origin.AddSeconds(index * DurationSeconds);
            var end = start.AddSeconds(DurationSeconds);

            return new TimeSlot(index, start, end);
        }

        public bool Equals(TimeSlot other) => other != null && Index == other.Index;
        public override bool Equals(object obj) => Equals(obj as TimeSlot);
        public override int GetHashCode() => Index.GetHashCode();
        public static bool operator ==(TimeSlot x, TimeSlot y) => EqualityComparer<TimeSlot>.Default.Equals(x, y);
        public static bool operator !=(TimeSlot x, TimeSlot y) => !(x == y);
    }
}