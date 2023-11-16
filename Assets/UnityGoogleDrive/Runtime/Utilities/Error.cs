using System;

namespace UnityGoogleDrive
{
    /// <summary>
    /// Exception thrown from the UnityGoogleDrive internal behaviour.
    /// </summary>
    public class Error : Exception
    {
        public Error (string message) : base(message) { }
    }
}
