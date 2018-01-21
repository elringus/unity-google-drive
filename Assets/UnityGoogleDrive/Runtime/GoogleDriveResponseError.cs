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

            [SerializeField] private List<ErrorDefinition> errors = null;
            [SerializeField] private int code = 0;
            [SerializeField] private string message = null;
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
        }

        public bool IsError { get { return Error != null; } }
        public ErrorDescription Error { get { return error; } }

        [SerializeField] private ErrorDescription error = null;
    }
}
