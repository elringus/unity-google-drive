using System;
using System.Xml;

namespace Data
{
    /// <summary>
    /// Base class for a Google Drive resource data representation.
    /// </summary>
    public abstract class ResourceData
    {
        /// <summary>
        /// Identifies what kind of resource this is.
        /// </summary>
        public abstract string Kind { get; }

        protected DateTime? Rfc3339ToDateTime (string rfc3339)
        {
            if (string.IsNullOrEmpty(rfc3339)) return null;
            return XmlConvert.ToDateTime(rfc3339, XmlDateTimeSerializationMode.Utc);
        }

        protected string DateTimeToRfc3339 (DateTime? dateTime)
        {
            if (!dateTime.HasValue) return null;
            return XmlConvert.ToString(dateTime.Value, XmlDateTimeSerializationMode.Utc);
        }
    }
}
