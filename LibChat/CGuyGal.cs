using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace LibChat
{
    public class CGuyGal
    {
        
        public long ID { get; set; } = 0L;
        //private const string _CON_STRING = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=dbChattative;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private const string _CON_STRING = @"Data Source=SQL1002.site4now.net;Initial Catalog=db_ab214d_yy;User Id=db_ab214d_yy_admin;Password=Password1!";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FullName => $"{FirstName} {LastName}";
        public string SkinColour { get; set; } = "";
        public short Age { get; set; } = 0;
        public int Score { get; set; } = 0;
        public long CUserID { get; set; } = 0L;

        public override string ToString()
        {
            return $"Name: {FullName}, Age: {Age}, Score: {Score}";
        }

        public void Create(long userID) // Create a pet for this (userID) user.
        {
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = @"
                    INSERT INTO tblGuyGal
                    (FirstName, LastName, SkinColour, Age, Score, FkUserID)
                    VALUES (@FirstName, @LastName, @SkinColour, @Age, @Score, @userID)
                ";
                cmd.Parameters.AddWithValue("@FirstName", this.FirstName);
                cmd.Parameters.AddWithValue("@LastName", this.LastName);
                cmd.Parameters.AddWithValue("@SkinColour", this.SkinColour);
                cmd.Parameters.AddWithValue("@Age", this.Age);
                cmd.Parameters.AddWithValue("@Score", this.Score);
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.ExecuteNonQuery();
                
                /*using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Loop through the returned rows and add IDs to the list.
                    while (reader.Read()) // While there are more rows.
                    {
                        string petID = reader.GetInt64(0).ToString(); // Always first column of each row.
                        string fullName = reader.GetString(1);
                        string skinColour = reader.GetString(2);
                        usersPets.Add(petID);
                        usersPets.Add(fullName);
                        usersPets.Add(skinColour);
                    }
                }
                con.Close();
                return usersPets;*/
            }
            catch (Exception E)
            {
                throw E; // allow same character names for different users. 1 user cannot have duplicate names chatracters,... 
            }

        }

        public DataTable Search(long userID)
        {
            //List<string> usersPets = new List<string>(); // To hold query results.
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = @"
                    SELECT FullName, SkinColour, Age, Score FROM [tblGuyGal] 
                    WHERE FkUserID = @userID
                ";
                cmd.Parameters.AddWithValue("@userID", userID);

                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                return dt;
                //using (SqlDataReader reader = cmd.ExecuteReader())
                /*{
                    Loop through the returned rows and add IDs to the list.
                    while (reader.Read()) // While there are more rows.
                    {
                        string petID = reader.GetInt64(0).ToString(); // Always first column of each row.
                        string fullName = reader.GetString(1);
                        string skinColour = reader.GetString(2);
                        usersPets.Add(petID);
                        usersPets.Add(fullName);
                        usersPets.Add(skinColour);
                    }
                }
                con.Close();
                return usersPets;*/
            }
            catch (Exception E)
            {
                throw E;
            }

        }
    }
}
