using System.Collections.Generic;

namespace UnityGoogleDrive.Data
{
    /// <summary>
    /// A notification channel used to watch for resource changes.
    /// </summary>
    public class Channel : ResourceData
    {
        /// <summary>
        /// Identifies this as a notification channel used to watch for changes to a resource.
        /// Value: the fixed string "api#channel".
        /// </summary>
        public override string Kind => "api#channel";
        /// <summary>
        /// The address where notifications are delivered for this channel.
        /// </summary>
        public virtual string Address { get; set; }
        /// <summary>
        /// Date and time of notification channel expiration, expressed as a Unix timestamp,
        /// in milliseconds. Optional.
        /// </summary>
        public virtual long? Expiration { get; set; }
        /// <summary>
        /// A UUID or similar unique string that identifies this channel.
        /// </summary>
        public virtual string Id { get; set; }
        /// <summary>
        /// Additional parameters controlling delivery channel behavior. Optional.
        /// </summary>
        public virtual Dictionary<string, string> Params { get; set; }
        /// <summary>
        /// A Boolean value to indicate whether payload is wanted. Optional.
        /// </summary>
        public virtual bool? Payload { get; set; }
        /// <summary>
        /// An opaque ID that identifies the resource being watched on this channel.
        /// Stable across different API versions.
        /// </summary>
        public virtual string ResourceId { get; set; }
        /// <summary>
        /// A version-specific identifier for the watched resource.
        /// </summary>
        public virtual string ResourceUri { get; set; }
        /// <summary>
        /// An arbitrary string delivered to the target address with each notification delivered
        /// over this channel. Optional.
        /// </summary>
        public virtual string Token { get; set; }
        /// <summary>
        /// The type of delivery mechanism used for this channel.
        /// </summary>
        public virtual string Type { get; set; }
    }
}
