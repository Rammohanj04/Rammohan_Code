using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMVCS.BusinessLogic
{
    public class CaseDetails
    {
        public int recordNumber { get; set; }
        public string dateEntered { get; set; }
        public string docType { get; set; }
        public string source { get; set; }
        public string received { get; set; }
        public string reasonCode { get; set; }
        public string actionCode { get; set; }
        public string actionCodeName { get; set; }
        public string reviewer { get; set; }
        public string forwardTo { get; set; }
        public string forwardToName { get; set; }
        public DateTime modDate { get; set; }
        public string userCode { get; set; }
        public string dln { get; set; }
        public string CustomerID { get; set; }
        public string createdBy { get; set; }

    }
}