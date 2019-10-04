using System;

namespace NaoMePerturbe.Classes.Exceptions
{
    class DomainException: ApplicationException
    {
        public DomainException(string message) : base(message)
        {

        }

    }
}
