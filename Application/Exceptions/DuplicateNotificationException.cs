using System;

namespace Application.Exceptions
{
    public class DuplicateNotificationException : ConflictException
    {
        public DuplicateNotificationException(string message) : base(message)
        {
        }
    }
}