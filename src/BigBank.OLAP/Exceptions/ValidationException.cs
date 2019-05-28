namespace BigBank.OLAP.Exceptions
{
    public class ValidationException : BigBankException
    {
        public ValidationException(string message) : base(message)
        {
        }
    }
}