using System;
using System.Runtime.Serialization;

namespace Play.Identity.Service.Exceptions
{
    [Serializable]
    internal class InsufficientFundsException : Exception
    {
        public InsufficientFundsException(Guid userId, decimal gitToDebit) : base($"Not enough gil to debit {gitToDebit} from user '{userId}'")
        {
            this.UserId = userId;
            this.GilToDebit = gitToDebit;
        }


        public Guid UserId { get;}
        public Decimal GilToDebit { get; }
    }
}