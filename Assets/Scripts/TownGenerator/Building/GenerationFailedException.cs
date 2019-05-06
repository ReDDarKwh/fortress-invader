using System;

public class GenerationFailedException : Exception
{
    public GenerationFailedException()
    {
    }

    public GenerationFailedException(string message)
        : base(message)
    {
    }
}