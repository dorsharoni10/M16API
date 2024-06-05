namespace AlignAPI.Exceptions
{
    public class InvalidDateException : FormatException
    {
        public InvalidDateException(string message) : base(message) { }
    }
}
