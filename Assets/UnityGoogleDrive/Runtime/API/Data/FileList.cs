using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class FileList : GoogleDriveData
    {
        /// <summary>
        /// Identifies what kind of resource this is. Value: the fixed string "drive#fileList"
        /// </summary>
        public override string Kind { get { return "drive#fileList"; } }

    }
}
