using System;
using System.Runtime.Serialization;

[Serializable]
internal class TweenCanceledException : Exception
{
    public TweenCanceledException()
    {
    }

    public TweenCanceledException(string message) : base(message)
    {
    }

    public TweenCanceledException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected TweenCanceledException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}