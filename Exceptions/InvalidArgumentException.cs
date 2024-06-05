namespace AlignAPI.Exceptions
{
    public class InvalidArgumentException : ArgumentException
    {
        public InvalidArgumentException(string message) : base(message) { }
    }
}
