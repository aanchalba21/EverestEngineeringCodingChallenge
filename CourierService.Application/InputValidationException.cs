namespace CourierService.Application;

public sealed class InputValidationException : Exception
{
    public InputValidationException(string message)
        : base(message)
    {
    }
}

