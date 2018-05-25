using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGoogleDrive
{
    [Serializable]
    public class GoogleDriveResponseError
    {
        [Serializable]
        public class ErrorDescription
        {
            public List<ErrorDefinition> Errors { get { return errors; } }
            public int Code { get { return code; } }
            public string Message { get { return message; } }

            [SerializeField] private List<ErrorDefinition> errors = new List<ErrorDefinition>();
            [SerializeField] private int code = 0;
            [SerializeField] private string message = null;

            public override string ToString ()
            {
                var output = string.Format("Google Drive API Error Description: Code '{0}' Message: '{1}'", Code, Message);
                foreach (var error in Errors)
                    output += Environment.NewLine + " - " + error.ToString();
                return output;
            }
        }

        [Serializable]
        public class ErrorDefinition
        {
            public string Domain { get { return domain; } }
            public string Reason { get { return reason; } }
            public string Message { get { return message; } }
            public string LocationType { get { return locationType; } }
            public string Location { get { return location; } }

            [SerializeField] private string domain = null;
            [SerializeField] private string reason = null;
            [SerializeField] private string message = null;
            [SerializeField] private string locationType = null;
            [SerializeField] private string location = null;

            public override string ToString ()
            {
                return string.Format("Domain: '{0}' Reason: '{1}' Message: '{2}' LocationType: '{3}' Location: '{4}'", 
                    Domain, Reason, Message, LocationType, Location);
            }
        }

        public bool IsError { get { return Error != null && Error.Code != 0; } }
        public ErrorDescription Error { get { return error; } }

        public override string ToString ()
        {
            if (!IsError) return null;
            return Error.ToString();
        }

        [SerializeField] private ErrorDescription error = null;
    }
}
