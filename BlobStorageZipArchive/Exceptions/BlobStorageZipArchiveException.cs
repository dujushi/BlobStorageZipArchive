using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BlobStorageZipArchive.Exceptions
{
    [Serializable]
    public class InvalidConfigurationException : Exception
    {
        public InvalidConfigurationException() { }

        public InvalidConfigurationException(string message)
            : base(message) { }

        public InvalidConfigurationException(string message, Exception inner)
            : base(message, inner) { }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected InvalidConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
