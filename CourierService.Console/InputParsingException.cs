namespace CourierService.ConsoleApp;

public sealed class InputParsingException : Exception
{
    public InputParsingException(string message)
        : base(message)
    {
    }
}

