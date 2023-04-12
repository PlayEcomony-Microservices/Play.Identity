using System;
using System.Runtime.Serialization;

namespace Play.Identity.Service.Exceptions
{
    [Serializable]
    internal class UnknownUserException : Exception
    {
        public UnknownUserException(Guid userId) : base($"Unknow user '{userId}'")
        {
            this.UserId = userId;
        }

        public Guid UserId { get; }
       
    }
}