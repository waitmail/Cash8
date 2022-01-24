using System;
using System.Collections.Generic;
using System.Text;

namespace Cash8
{               
    class PreInspectionResponse
    {
        //Ответ на проверку КМ с ожиданием ответа от сервера
        public DriverError driverError { get; set; }
        public ItemInfoCheckResult itemInfoCheckResult { get; set; }
        public OfflineValidation offlineValidation { get; set; }
        public OnlineValidation onlineValidation { get; set; }
        public bool sentImcRequest { get; set; }


        public class DriverError
        {
            public int code { get; set; }
        }

        public class ItemInfoCheckResult
        {
            public bool ecrStandAloneFlag { get; set; }
            public bool imcCheckFlag { get; set; }
            public bool imcCheckResult { get; set; }
            public bool imcEstimatedStatusCorrect { get; set; }
            public bool imcStatusInfo { get; set; }
        }

        public class OfflineValidation
        {
            public bool fmCheck { get; set; }
            public string fmCheckErrorReason { get; set; }
            public bool fmCheckResult { get; set; }
        }

        public class MarkOperatorResponse
        {
            public bool itemStatusCheck { get; set; }
            public bool responseStatus { get; set; }
        }

        public class OnlineValidation
        {
            public string imcType { get; set; }
            public ItemInfoCheckResult itemInfoCheckResult { get; set; }
            public string markOperatorItemStatus { get; set; }
            public MarkOperatorResponse markOperatorResponse { get; set; }
            public string markOperatorResponseResult { get; set; }
        }
    }
}
