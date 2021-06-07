using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DMVCS.BusinessLogic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using DMVCS.Models;

namespace DMVCS.DataWorker
{
    public class CaseDetailsDataAccess
    {
        SqlConnection _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQL_db_DEV"].ConnectionString);

        /// <summary>
        /// Returns a list of CaseDetails based upon the customers DLN or ID
        /// </summary>
        /// <param name="SearchBy">DLN or Customer ID</param>
        /// <param name="Procedure">ByID or ByDLN</param>
        /// <returns>List of Case Details</returns>
        public List<CaseDetails> getCaseDetails(string searchBy, string procedure)
        {
            List<CaseDetails> result = new List<CaseDetails>();
            using (SqlCommand cmd = new SqlCommand("procCaseDetails", _conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (procedure.Equals("ByDLN")) { cmd.Parameters.AddWithValue("DLN", searchBy); }
                else if (procedure.Equals("ByID")){ cmd.Parameters.AddWithValue("customerID", searchBy); }
                cmd.Parameters.AddWithValue("procedure", procedure);
                _conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CaseDetails c = new CaseDetails();
                        {
                            c.dln = Convert.ToString(reader["DLN"]);
                            c.recordNumber = Convert.ToInt32(reader["RecordNumber"]);
                            c.received = Convert.ToString(reader["Received"]);
                            c.source = Convert.ToString(reader["Source"]);
                            c.reasonCode = Convert.ToString(reader["ReasonCode"]);
                            c.actionCode = Convert.ToString(reader["Original Action Codes"]);
                            c.dateEntered = Convert.ToString(reader["DateEntered"]);
                            c.docType = Convert.ToString(reader["Type of Document"]);
                            c.forwardTo = Convert.ToString(reader["ForwardTo"]);
                            c.reviewer = Convert.ToString(reader["Reviewer"]);
                            c.CustomerID = Convert.ToString(reader["CustomerID"]);
                            c.createdBy = Convert.ToString(reader["CreatedBy"]);
                        }
                        result.Add(c);
                    }
                }
                _conn.Close();
            }
            return result;
        }

        public CaseDetails getCaseDetailByRecord(string recordNumber, string procedure)
        {
            CaseDetails c = new CaseDetails();

            using (SqlCommand cmd = new SqlCommand("procCaseDetails", _conn))
            {

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("RecordNumber", recordNumber);
                cmd.Parameters.AddWithValue("procedure", procedure);
                _conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        c.dln = Convert.ToString(reader["DLN"]);
                        c.received =  Convert.ToString(Convert.ToDateTime(reader["Received"]).ToShortDateString());
                        c.source = Convert.ToString(reader["Source"]);
                        c.reasonCode = Convert.ToString(reader["ReasonCode"]);
                        c.actionCode = Convert.ToString(reader["Original Action Codes"]);
                        c.dateEntered = Convert.ToString(reader["DateEntered"]);
                        c.docType = Convert.ToString(reader["Type of Document"]);
                        c.forwardTo = Convert.ToString(reader["ForwardTo"]);
                        c.reviewer = Convert.ToString(reader["Reviewer"]);
                        c.recordNumber = Convert.ToInt32(reader["RecordNumber"]);
                        c.CustomerID = Convert.ToString(reader["CustomerID"]);
                        c.createdBy = Convert.ToString(reader["CreatedBy"]);
                        

                    }
                }
                _conn.Close();
            }
            return c;
        }
        public void InputUpdateCase(CaseDetailsLettersModel model, string procedure)
        {


            using (SqlCommand cmd = new SqlCommand("procCaseDetails", _conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                // Customer Information
                if (!string.IsNullOrWhiteSpace(model.customer.DLN)) { cmd.Parameters.AddWithValue("DLN", model.customer.DLN); }
                if (!string.IsNullOrWhiteSpace(model.customer.LName)) { cmd.Parameters.AddWithValue("LName", model.customer.LName); }
                if (!string.IsNullOrWhiteSpace(model.customer.FName)) { cmd.Parameters.AddWithValue("FName", model.customer.FName); }
                if (!string.IsNullOrWhiteSpace(model.customer.MName)) { cmd.Parameters.AddWithValue("MName", model.customer.MName); }
                if (!string.IsNullOrWhiteSpace(model.customer.DOB)) { cmd.Parameters.AddWithValue("DOB", model.customer.DOB); }
                if (!string.IsNullOrWhiteSpace(model.customer.Gender)) { cmd.Parameters.AddWithValue("Sex", model.customer.Gender); }
                if (!string.IsNullOrWhiteSpace(model.customer.Title)) { cmd.Parameters.AddWithValue("Title", model.customer.Title); }
                if (!string.IsNullOrWhiteSpace(model.customer.PhoneNumber)) { cmd.Parameters.AddWithValue("Cust_Phone", model.customer.PhoneNumber); }
                if (!string.IsNullOrWhiteSpace(model.customer.EmailAddress)) { cmd.Parameters.AddWithValue("Cust_Email", model.customer.EmailAddress); }
                if (!string.IsNullOrWhiteSpace(model.customer.MailingAddress)) { cmd.Parameters.AddWithValue("Cust_Mail_Street", model.customer.MailingAddress); }
                if (!string.IsNullOrWhiteSpace(model.customer.ResAddress)) { cmd.Parameters.AddWithValue("StreetAddress", model.customer.ResAddress); }
                if (!string.IsNullOrWhiteSpace(model.customer.Notes)) { cmd.Parameters.AddWithValue("Driver_Notes", model.customer.Notes); }
                cmd.Parameters.AddWithValue("customerID", model.customer.CustID);
              

                cmd.Parameters.AddWithValue("Mod_Date", DateTime.Now);
                if (procedure.Equals("Create")) { cmd.Parameters.AddWithValue("DateEntered", DateTime.Now.ToShortDateString()); }
                if (!string.IsNullOrWhiteSpace(model.caseDetail.userCode)) { cmd.Parameters.AddWithValue("user_Code", model.caseDetail.reviewer); }

                // Case detail information

                
                if (!string.IsNullOrWhiteSpace(model.caseDetail.dateEntered)) { cmd.Parameters.AddWithValue("DateEntered", model.caseDetail.dateEntered); }
                if (!string.IsNullOrWhiteSpace(model.hiddenDocType) && !model.hiddenDocType.Equals("Select")) { cmd.Parameters.AddWithValue("DocType", model.hiddenDocType); }
                else { cmd.Parameters.AddWithValue("DocType", model.caseDetail.docType); }
                if (!string.IsNullOrWhiteSpace(model.hiddenSourceCode) && !model.hiddenSourceCode.Equals("Select")) { cmd.Parameters.AddWithValue("source", model.hiddenSourceCode); }
                else  { cmd.Parameters.AddWithValue("source", model.caseDetail.source); }
                if (!string.IsNullOrWhiteSpace(model.hiddenReceived)) { cmd.Parameters.AddWithValue("Received", model.hiddenReceived); }
                else { cmd.Parameters.AddWithValue("Received", model.caseDetail.received); }
                if (!string.IsNullOrWhiteSpace(model.hiddenReasonCode) && !model.hiddenReasonCode.Equals("Select")) { cmd.Parameters.AddWithValue("ReasonCode", model.hiddenReasonCode); }
                else { cmd.Parameters.AddWithValue("ReasonCode", model.caseDetail.reasonCode); }
                if (!string.IsNullOrWhiteSpace(model.hiddenActionCode) && !model.hiddenActionCode.Equals("Select")) { cmd.Parameters.AddWithValue("ActionCode", model.hiddenActionCode); }
                else { cmd.Parameters.AddWithValue("ActionCode", model.caseDetail.actionCode); }
                if (!string.IsNullOrWhiteSpace(model.reviewerOfRecrod)) { cmd.Parameters.AddWithValue("reviewer", model.reviewerOfRecrod); }
                else { cmd.Parameters.AddWithValue("reviewer", model.caseDetail.reviewer);}
                if (string.IsNullOrWhiteSpace(model.caseDetail.createdBy)) { cmd.Parameters.AddWithValue("CreatedBy", model.hiddenReviewer); }
                else { cmd.Parameters.AddWithValue("CreatedBy", model.caseDetail.createdBy); } 
                if (!string.IsNullOrWhiteSpace(model.hiddenForwardTo) && !model.hiddenForwardTo.Equals("Select")) { cmd.Parameters.AddWithValue("ForwardTo", model.hiddenForwardTo); }
                else { cmd.Parameters.AddWithValue("ForwardTo", model.caseDetail.forwardTo); }
                if (model.caseDetail.recordNumber > 0) { cmd.Parameters.AddWithValue("RecordNumber", model.caseDetail.recordNumber); }
                cmd.Parameters.AddWithValue("procedure", procedure);



                _conn.Open();
                cmd.ExecuteScalar();
                _conn.Close();
            }
        }







    }


} 