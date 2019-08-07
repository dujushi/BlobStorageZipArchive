using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BlobStorageZipArchive.Exceptions
{
    [Serializable]
    public class BlobStorageZipArchiveException : Exception
    {
        public BlobStorageZipArchiveException() { }

        public BlobStorageZipArchiveException(string message)
            : base(message) { }

        public BlobStorageZipArchiveException(string message, Exception inner)
            : base(message, inner) { }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected BlobStorageZipArchiveException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
