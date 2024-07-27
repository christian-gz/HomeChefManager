using System;

namespace HomeChefManager.Utils;

public class RecordNotFoundException : Exception
{
    public RecordNotFoundException(string message) : base(message) { }

    public RecordNotFoundException(string message, Exception inner) : base(message, inner) { }
}