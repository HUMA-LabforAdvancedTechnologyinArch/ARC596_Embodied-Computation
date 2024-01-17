    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    namespace ApplicationInfo
    {
        [System.Serializable]
        public class ApplicationSettings
        {
            public string parentname {get; set;}
            public string storagename {get; set;}

            public bool objorientation {get; set;}
        }
    }