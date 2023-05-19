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
            public List<ErrorDefinition> Errors => errors;
            public int Code => code;
            public string Message => message;

            [SerializeField] private List<ErrorDefinition> errors = new List<ErrorDefinition>();
            [SerializeField] private int code;
            [SerializeField] private string message;

            public override string ToString ()
            {
                var output = $"Google Drive API Error Description: Code '{Code}' Message: '{Message}'";
                foreach (var error in Errors)
                    output += Environment.NewLine + " - " + error;
                return output;
            }
        }

        [Serializable]
        public class ErrorDefinition
        {
            public string Domain => domain;
            public string Reason => reason;
            public string Message => message;
            public string LocationType => locationType;
            public string Location => location;

            [SerializeField] private string domain;
            [SerializeField] private string reason;
            [SerializeField] private string message;
            [SerializeField] private string locationType;
            [SerializeField] private string location;

            public override string ToString ()
            {
                return $"Domain: '{Domain}' Reason: '{Reason}' Message: '{Message}' LocationType: '{LocationType}' Location: '{Location}'";
            }
        }

        public bool IsError => Error != null && Error.Code != 0;
        public ErrorDescription Error => error;

        [SerializeField] private ErrorDescription error;

        public override string ToString ()
        {
            if (!IsError) return null;
            return Error.ToString();
        }
    }
}
