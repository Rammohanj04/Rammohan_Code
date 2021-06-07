using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using DMVCS.BusinessLogic;

namespace DMVCS.DataWorker
{
    public class DataAccessLayer
    {
        //must change conn string name
        SqlConnection _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConNam"].ConnectionString);


        /// <summary>
        /// dual concept here.  One key for whole application in a Settings table or one key per record in db held in a seperate key holding related table.
        /// recordID can be empty or null
        /// </summary>
        /// <param name="recordID"></param>
        /// <returns></returns>
        public string GetEncryptionKey(string recordID)
        {
            string key = string.Empty;
            string storedPro = string.Empty;
            if (!String.IsNullOrEmpty(recordID))
            {

                storedPro = "GetKeyFromKeyDataTable";  //define table in stored pro
            }
            else
            {
                storedPro = "GetKeyFromSettingsTable";  //from settings table
            }
            using (SqlCommand cmd = new SqlCommand(storedPro, _conn))
            {
                if (!String.IsNullOrEmpty(recordID))
                {
                    cmd.Parameters.AddWithValue("Change this parm", recordID);
                }
                cmd.CommandType = CommandType.StoredProcedure;
                _conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        key = Convert.ToString(reader["SecretKey"]);
                    }
                }
                _conn.Close();
            }

            return key;
        }

        /// <summary>
        /// Used to upload files to sql attached to a specific room
        /// </summary>
        /// <param name="fileUpload"></param>
        public void UploadFileToSQL(List<FileUpload> fileUpload)
        {
            using (SqlCommand cmd = new SqlCommand("InsertFileUpload", _conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                _conn.Open();
                foreach (var fs in fileUpload)
                {
                    cmd.Parameters.AddWithValue("UploadName", fs.FileName);
                    cmd.Parameters.AddWithValue("ContentType", fs.ContentType);
                    cmd.Parameters.AddWithValue("UploadData", fs.FileBytes);
                }
                cmd.ExecuteScalar();
                _conn.Close();
            }
        }

        /// <summary>
        /// Used to update an uploaded file to sql for a specific room
        /// </summary>
        /// <param name="fileUpload"></param>
        public void UpdateFileToSQL(List<FileUpload> fileUpload)
        {
            using (SqlCommand cmd = new SqlCommand("UpdateFileUpload", _conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                _conn.Open();
                foreach (var fs in fileUpload)
                {
                    cmd.Parameters.AddWithValue("RoomID", fs.RoomID);
                    cmd.Parameters.AddWithValue("UploadName", fs.FileName);
                    cmd.Parameters.AddWithValue("ContentType", fs.ContentType);
                    cmd.Parameters.AddWithValue("UploadData", fs.FileBytes);
                }
                cmd.ExecuteScalar();
                _conn.Close();
            }
        }

        /// <summary>
        /// Get document upload data from sql
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public List<FileUpload> DownLoadFile(int roomId)
        {
            List<FileUpload> results = new List<FileUpload>();
            using (SqlCommand cmd = new SqlCommand("GetUploadedFileDataByRoomId", _conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("RoomID", roomId);
                _conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        FileUpload fs = new FileUpload();
                        {
                            fs.FileName = Convert.ToString(reader["UploadName"]);
                            fs.ContentType = Convert.ToString(reader["ContentType"]);
                            fs.FileBytes = (byte[])(reader["UploadData"]);
                        }
                        results.Add(fs);
                    }
                }
                _conn.Close();
            }

            return results;
        }

        /// <summary>
        /// data exports by year only
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public DataTable ExportForExcel(string year)
        {
            DataTable dt = new DataTable();

            using (SqlCommand cmd = new SqlCommand("ExportToExcelByYear", _conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TheYear", year);
                _conn.Open();
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = cmd;
                    da.Fill(dt);
                }
                _conn.Close();
            }
            return dt;
        }

        public List<string> GetCityStateZip()
        {
            List<string> result = new List<string>();
            result.Add("Select City");
            using (SqlCommand cmd = new SqlCommand("selectCityStateZip", _conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                _conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string Location = Convert.ToString(reader["CityStateZip"]);
                        result.Add(Location);
                    }
                }
            }
            _conn.Close();

            return result;
        }
    }
}