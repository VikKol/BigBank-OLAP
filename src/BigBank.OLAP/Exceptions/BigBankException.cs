using System;

namespace BigBank.OLAP.Exceptions
{
    public class BigBankException : Exception
    {
        public BigBankException(string message) : base(message)
        {
        }
    }
}