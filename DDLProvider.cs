using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DMVCS.DataWorker;
using System.Web.Mvc;
using System.Configuration;

namespace DMVCS.BusinessLogic
{
    public class DDLProvider
    {
        DDLDataAccess _ddl = new DDLDataAccess();
        DMVCSUserDataAccess _duda = new DMVCSUserDataAccess();
        string AgentInit = ConfigurationManager.AppSettings["agentInit"];
        string AgentUserName = ConfigurationManager.AppSettings["agentUsername"];
        string ManagerInit = ConfigurationManager.AppSettings["ManagerInit"];
        string ManagerUserName = ConfigurationManager.AppSettings["ManagerUsername"];
        public List<string> SearchBy()
        {
            List<string> result = new List<string>();
            result.Add("Name");
            result.Add("Drivers License Number");
            
           // result.Add("No Header");
            return result;
        }
        public List<string> GenderList()
        { 
            List<string> sex = new List<string>();
            sex.Add("Male");
            sex.Add("Female");
            return sex;
        }
        public List<string> CaseDetailsGridHeaders()
        {
            List<string> headers = new List<string>()
            {
                "Reviewer",
                "Date Received ",
                "Doc Type ",
                "Action Code ",
                "Forward To "
            };
            return headers;
        }
        public List<string> Display()
        {
            List<string> size = new List<string>()
            {
                "10",
                "25",
                "50",
            };
            return size;
        }
        public List<string> Copies()
        {
            List<string> size = new List<string>()
            {
                "1",
                "2"
            };
            return size;
        }
        public List<string> CustomerSearchGridHeaders()
        {
            List<string> headers = new List<string>()
            {
                "DLN ",
                "Last Name ",
                "First Name ",
                "Middle Name "
                
            };
            return headers;
        }
        public List<string> BatchHistoryHeaders()
        {
            List<string> headers = new List<string>()
            {
                "Select ",
                "Reviewer ",
                "Batch ID ",
                "Creation Date ",
                "# Letters "

            };
            return headers;
        }
        public List<string> DMVCSUserGridHeaders()
        {
            List<string> headers = new List<string>()
            {
                "Full Name",
                "User Name",
                "Display Name",
                "Admin"
            };
            return headers;
        }
        public List<string> LetterQueGridHeaders()
        {
            List<string> headers = new List<string>()
            {
                "Select ",
                "Reviewer",
                "Letter Code ",
                "Date Ordered "

            };
            return headers;
        }
        public List<string> RLMQueGridHeaders()
        {
            List<string> headers = new List<string>()
            {
                "Select ",
                "DLN",
                "Letter Code ",
                "Date Ordered "

            };
            return headers;
        }
        public List<string> BatchLettersGridHeaders()
        {
            List<string> headers = new List<string>()
            {
                "DLN ",
                "Letter Code ",
                "Date Ordered "

            };
            return headers;
        }
        public List<string> CustomerHistoryHeaders()
        {
            List<string> headers = new List<string>()
            {
                "Reviewer",
                "Letter Code ",
                "Date Ordered "

            };
            return headers;
        }

        
        public List<SelectListItem> reviewers(string user)
        {
            List<Reviewer> initials = _ddl.getReviewerInit(user);

            List<SelectListItem> results = new List<SelectListItem>();
            for (int i = 0; i < initials.Count; i++)
            {
                SelectListItem s = new SelectListItem();
                s.Text = initials[i].initials;
                s.Value = initials[i].userName;

                
                results.Add(s);
            }
            SelectListItem s2 = new SelectListItem();
            SelectListItem s3 = new SelectListItem();
            s2.Text = AgentInit;
            s2.Value = AgentUserName;
            s3.Text = ManagerInit;
            s3.Value = ManagerUserName;
            results.Add(s2);
            results.Add(s3);
            return results;
        }
        public List<SelectListItem> docType()
        {
            List<string> dt = _ddl.getDocType();
            dt.Insert(0, "Select");

            List<SelectListItem> docTypes = new List<SelectListItem>();
           
            for (int i = 0; i < dt.Count; i++)
            {
                SelectListItem s = new SelectListItem();
                s.Text = dt[i];
                s.Value = i.ToString();
                docTypes.Add(s);
            }
            return docTypes;

        }

        public List<SelectListItem> sourceCode()
        {
            List<SourceCode> sc = _ddl.getSourceCode();
            List<string> code = new List<string>();
            foreach (var sourceCode in sc)
            {
                string temp = sourceCode.code + " - " + sourceCode.description;
                code.Add(temp);
            }
            code.Insert(0,"Select");

            List<SelectListItem> results = new List<SelectListItem>();
            for (int i = 0; i < code.Count; i++)
            {
                SelectListItem s = new SelectListItem();
                s.Text = code[i];
                s.Value = i.ToString();
                results.Add(s);
            }
            return results;
           
        }
        public List<SelectListItem> reasonCode()
        {
            List<ReasonCode> rc = _ddl.getReasonCode();
            List<string> code = new List<string>();
            foreach (var reasonCode in rc)
            {
                string temp = reasonCode.reasonCode + " - " + reasonCode.Description;
                code.Add(temp);
            }
            code.Insert(0,"Select");

            List<SelectListItem> results = new List<SelectListItem>();
            for (int i = 0; i < code.Count; i++)
            {
                SelectListItem s = new SelectListItem();
                s.Text = code[i];
                s.Value = i.ToString();
                results.Add(s);
            }
            return results;
        }
        public List<SelectListItem> actionCodes()
        {
            List<ActionCode> ac = _ddl.getActionCode();
            List<string> code = new List<string>();
            foreach (var actionCode in ac)
            {
                string temp = actionCode.code + " - " + actionCode.description;
                code.Add(temp);
            }
            code.Insert(0,"Select");

            List<SelectListItem> results = new List<SelectListItem>();
            for (int i = 0; i < code.Count; i++)
            {
                SelectListItem s = new SelectListItem();
                s.Text = code[i];
                s.Value = i.ToString();
                results.Add(s);
            }
            return results;
        }
        public List<SelectListItem> forwardTo()
        {
            List<ForwardTo> ft = _ddl.getForwardTo();
            List<string> code = new List<string>();
            foreach (var forward in ft)
            {
                string temp = forward.Name + " - " + forward.description;
                code.Add(temp);
            }
            code.Insert(0,"Select");

            List<SelectListItem> results = new List<SelectListItem>();
            for (int i = 0; i < code.Count; i++)
            {
                SelectListItem s = new SelectListItem();
                s.Text = code[i];
                s.Value = i.ToString();
                results.Add(s);
            }
            return results;
        }
        public List<string> RolesList()
        {
            List<string> roles = new List<string>();
            roles.Add("Select User Role");
            roles.Add("Admin");
            roles.Add("User");
            roles.Add("Lookup");
            return roles;
        }
        public List<string> GetUserReportList()
        {
            List<string> reports = new List<string>();
            reports.Add("Select Report");
            reports.Add("Age Count");
            reports.Add("Fatals");
            reports.Add("Favorable");
            //            reports.Add("Monthly Statistical Report");
            reports.Add("Refer Any App");
            reports.Add("Refer to Reexam");
            reports.Add("Unfavorable");
            reports.Add("Letter Codes");
            reports.Add("Source Codes");
            reports.Add("Action Codes");
            reports.Add("Reason Codes");
            return reports;
        }
        public List<string> GetAdminReportList()
        {
            List<string> reports = new List<string>();
            reports.Add("Select Report");
            reports.Add("Age Count");
            reports.Add("Fatals");
            reports.Add("Favorable");
            reports.Add("Monthly Statistical Report");
            reports.Add("Refer Any App");
            reports.Add("Refer to Reexam");
            reports.Add("Unfavorable");
            reports.Add("Source Codes");
            reports.Add("Action Codes");
            reports.Add("Reason Codes");
            reports.Add("Letter Codes");
            return reports;
        }


    }
}
