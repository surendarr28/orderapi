namespace orderapis.Core
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using MySql.Data.MySqlClient;
    using Newtonsoft.Json;

    public static class DbAccess
    {
        private static string dbConnection = ConfigurationManager.ConnectionStrings["connect"].ConnectionString;
        public static List<Dictionary<string, object>> DbASelects(string tableName, object objItem)
        {
            try
            {
                var fieldName = "";
                var whereCondition = "";
                string query = "select " + fieldName + " from " + tableName + " where " + whereCondition;
                DataTable dt = new DataTable();

                using (MySqlConnection mySqlConnection = new MySqlConnection(dbConnection))
                {
                    using (MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection))
                    {
                        mySqlConnection.Open();

                        using (MySqlDataAdapter da = new MySqlDataAdapter(mySqlCommand))
                        {
                            da.Fill(dt);
                        }

                        return DictionaryData(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public static List<Dictionary<string, object>> DbASelect(string query)
        {
            try
            {
                DataTable dt = new DataTable();

                using (MySqlConnection mySqlConnection = new MySqlConnection(dbConnection))
                {
                    using (MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection))
                    {
                        mySqlConnection.Open();

                        using (MySqlDataAdapter da = new MySqlDataAdapter(mySqlCommand))
                        {
                            da.Fill(dt);
                        }

                        return DictionaryData(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public static string DbAUpdate(string tableName, object objItem)
        {
            try
            {
                var updateValue = "";
                var whereCondition = "";
                string query = "update  " + tableName + " SET " + updateValue + " where " + whereCondition;
                DataTable dt = new DataTable();

                using (MySqlConnection objDbConnection = new MySqlConnection(dbConnection))
                {
                    using (MySqlCommand DBcommand = new MySqlCommand(query, objDbConnection))
                    {
                        objDbConnection.Open();

                        int i = DBcommand.ExecuteNonQuery();

                        if (i == 1)
                        {
                            return "success";
                        }
                        else
                        {
                            return "no updated";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        public static string DbAUpdatecommon(string query)
        {
            try
            {
                DataTable dt = new DataTable();

                using (MySqlConnection objDbConnection = new MySqlConnection(dbConnection))
                {
                    using (MySqlCommand DBcommand = new MySqlCommand(query, objDbConnection))
                    {
                        objDbConnection.Open();

                        int i = DBcommand.ExecuteNonQuery();

                        if (i == 1)
                        {
                            return "success";
                        }
                        else
                        {
                            return "error";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }




        public static string DbAInsert(string tableName, object objItem)
        {
            try
            {

                var beforeValue = "";
                var afterValue = "";
                foreach (PropertyInfo key in objItem.GetType().GetProperties())
                {
                    if (key.GetValue(objItem, null) != null)
                    {

                        beforeValue += beforeValue == "" ? key.Name.ToString() : ("," + key.Name.ToString());
                        afterValue += afterValue == "" ? ("'" + key.GetValue(objItem, null) + "'").ToString() : (",'" + key.GetValue(objItem, null) + "'");
                    }
                }
                string query = "insert into " + tableName + " (" + beforeValue + ") values(" + afterValue + ")";
                using (MySqlConnection objDbConnection = new MySqlConnection(dbConnection))
                {
                    using (MySqlCommand DBcommand = new MySqlCommand(query, objDbConnection))
                    {
                        objDbConnection.Open();

                        int i = DBcommand.ExecuteNonQuery();

                        if (i == 1)
                        {
                            return "success";
                        }
                        else
                        {
                            return "no added";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public static string DbAQueryInsert(string query)
        {
            try
            {

                 using (MySqlConnection objDbConnection = new MySqlConnection(dbConnection))
                {
                    using (MySqlCommand DBcommand = new MySqlCommand(query, objDbConnection))
                    {
                        objDbConnection.Open();

                        int i = DBcommand.ExecuteNonQuery();

                        if (i == 1)
                        {
                            return DBcommand.LastInsertedId.ToString();
                        }
                        else
                        {
                            return "no added";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public static string DbAInsertGeneral(string tableName, Dictionary<string, object> objItem)
        {
            try
            {
                var beforeValue = "";
                var afterValue = "";
                foreach (var item in objItem)
                {
                    if (item.Key != "table")
                    {
                        beforeValue += beforeValue == "" ? item.Key : ("," + item.Key);

                        if (item.Key == "vcPassword")
                        {
                            afterValue += afterValue == "" ? ("'" + Encrypt(item.Value.ToString()) + "'").ToString() : (",'" + Encrypt(item.Value.ToString()) + "'");
                        }
                        else
                        {
                            afterValue += afterValue == "" ? ("'" + item.Value + "'").ToString() : (",'" + item.Value + "'");
                        }

                    }
                }
                string query = "insert into " + tableName + " (" + beforeValue + ") values(" + afterValue + ")";
                using (MySqlConnection objDbConnection = new MySqlConnection(dbConnection))
                {
                    using (MySqlCommand DBcommand = new MySqlCommand(query, objDbConnection))
                    {
                        objDbConnection.Open();

                        int i = DBcommand.ExecuteNonQuery();

                        if (i == 1)
                        {
                            return "success";
                        }
                        else
                        {
                            return "no added";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }

        public static List<Dictionary<string, object>> DictionaryData(DataTable dt)
        {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }

            return rows;
        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }



    }

    public class Query
    {
    public string query
        {
        get;
        set;
        }
    }

    public class General
    {
        public string tableName
        {
            get;
            set;
        }

        public int? id
        {
            get;
            set;
        }

        public bool? isActive
        {
            get;
            set;
        }
        public DateTime? createdAt
        {
            get;
            set;
        }

        public DateTime? updateAt
        {
            get;
            set;
        }

        public DateTime? deletedAt
        {
            get;
            set;
        }
    }

}