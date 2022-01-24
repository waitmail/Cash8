using System;
using System.Collections.Generic;
using System.Text;

namespace Cash8
{
    class AddingKmArrayToTableTested
    {
        public string type { get; set; }
        public int timeout { get; set; }
        public List<Param> @params { get; set; }       

        public class Param
        {
            public string imcType { get; set; }
            public string imc { get; set; }
            public string itemEstimatedStatus { get; set; }          
            public int imcModeProcessing { get; set; }            
        }
    }
}
