using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace LibChat
{
    public class CUser
    {
        public long ID { get; set; } = 0L; // ID is 0 by default. If they fail login, then it stays 0.
        //private const string _CON_STRING = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=dbChattative;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private const string _CON_STRING = @"Data Source=SQL1002.site4now.net;Initial Catalog=db_ab214d_yy;User Id=db_ab214d_yy_admin;Password=Password1!";
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string BorderColour { get; set; } = "";
        public string Colour { get; set; } = "";
        public string BGroundColour { get; set; } = "";
        public string BorderWidth { get; set; } = "";
        public string FontStyle { get; set; } = "";
        public string FontColour { get; set; } = "";
        public string Padding { get; set; } = "";
        public List<string> Interests { get; set; }
        public class MatchingAlgorithm
        {
            public List<Tuple<CUser, CUser, int>> FindMatches(List<CUser> users) // AI generated
            {
                var matches = new List<Tuple<CUser, CUser, int>>();

                foreach (var userX in users)
                {
                    foreach (var userY in users.Where(u => u.ID != userX.ID))
                    {
                        var commonInterests = userX.Interests.Intersect(userY.Interests).Count();
                        matches.Add(new Tuple<CUser, CUser, int>(userX, userY, commonInterests));
                    }
                }
                return matches.OrderByDescending(m => m.Item3).ToList();
            }
        }

        public bool Create()
        {
            try
            {
                // Set up the telephone line...
                SqlConnection con = new SqlConnection();

                // Key in the telephone number...
                con.ConnectionString = _CON_STRING;

                // Dial the number...
                con.Open();

                // Set up a two-way conversation through this connection...
                //SqlCommand cmd = conn.CreateCommand();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                // Set up a message from us to the database...
                cmd.CommandText = @"
                    SELECT COUNT(*)
                    FROM [tblUser]
                    WHERE UserName = @UserName;
                "; // Checking if the user already exists, as we can't have duplicates.

                cmd.Parameters.AddWithValue(@"UserName", UserName);
                int userCount = Convert.ToInt32(cmd.ExecuteScalar());
                if (userCount > 0)
                { // The same UserName user exists. Don't create a new one.
                    con.Close();
                    return false;
                }

                // Had to create another SqlCommand because using the same named parameter
                // 'UserName' and Password' was not allowed on the same SqlCommand.
                SqlCommand cmd2 = new SqlCommand();
                cmd2.Connection = con;
                cmd2.CommandText = @"
                    insert into [tblUser]
                    (
	                    [UserName],
	                    [Password]
                    )
                    values
                    (
	                    @UserName, @Password
                    );
                ";
                cmd2.Parameters.AddWithValue(@"UserName", UserName);
                cmd2.Parameters.AddWithValue(@"Password", Password);

                // Send message.
                cmd2.ExecuteNonQuery();

                // Close connection.
                con.Close();

                return true; // We are not logging in yet.
            }
            catch (Exception E)
            {
                throw (E);
            }
        }

        public long Login()
        {
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = @"
                    SELECT * FROM [tblUser] 
                    WHERE UserName = @UserName
                    AND Password = @Password
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
                ";
                cmd.Parameters.AddWithValue("@UserName", this.UserName);
                cmd.Parameters.AddWithValue("@Password", this.Password);
                object objId = cmd.ExecuteScalar();
                con.Close();

                if (objId != null) // Successful login, UN and PW matched, so ID of account is returned.
                {
                    this.ID = (long)objId; // ID wont be 0.
                    return ID;
                }
                else
                {
                    return 0L;
                }
                // contact CGuyGal to retrieve user's guys / gals... connected via ID...
                //ID = id;
            }
            catch (Exception E)
            {
                throw (E);
            }
        }

        public void Send(string receiverName, string message)
        {
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                cmd.CommandText = @"
                    SELECT [ID]
                    FROM [tblUser]
                    WHERE UserName = @UserName;
                ";
                cmd.Parameters.AddWithValue(@"UserName", receiverName);
                long receiverId = (long) cmd.ExecuteScalar();

                cmd.CommandText = @"
                    INSERT INTO [tblMessage]
                    (
                        [Message], [FkSenderID], [FkReceiverID]
                    )
                    values
                    (
                        @message, @fkSenderId, @receiverId
                    );
                ";
                cmd.Parameters.AddWithValue("@message", message);
                cmd.Parameters.AddWithValue("@fkSenderId", this.ID);
                cmd.Parameters.AddWithValue("@receiverId", receiverId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception E)
            {
                throw (E);
            }
        }
        public void SendFriend(string receiverName)
        {
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                cmd.CommandText = @"
                    SELECT [ID]
                    FROM [tblUser]
                    WHERE UserName = @UserName;
                ";
                cmd.Parameters.AddWithValue(@"UserName", receiverName);
                long receiverId = (long)cmd.ExecuteScalar();

                cmd.CommandText = @"
                    INSERT INTO [tblFriends]
                    (
                        [FkUserID1], [FkUserID2]
                    )
                    values
                    (
                        @fkSenderId, @receiverId
                    );
                ";
                cmd.Parameters.AddWithValue("@fkSenderId", this.ID);
                cmd.Parameters.AddWithValue("@receiverId", receiverId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception E)
            {
                throw (E);
            }
        }
        public List<string[]> SaveCustom(bool isSave, string BorderColour, string BorderWidth, string BGroundColour, string FontStyle, string FontColour, string Padding)
        {
            List<string[]> customs = new List<string[]>();
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                if (isSave) // Save the post's theme...
                {
                    cmd.CommandText = @"
                        SELECT COUNT(*)
                        FROM [tblPosts]
                        WHERE FkUserID = @FkUserID AND IsDefault = 1
                    ;";
                    cmd.Parameters.AddWithValue(@"FkUserID", ID);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0) // No default theme saved...
                    {
                        cmd.CommandText = @"
                        INSERT INTO [tblPosts]
                        (
                            [FkUserID], [BorderWidth], [BorderColour], [BGroundColour],
                            [FontStyle], [FontColour], [Padding], [IsDefault]
                        )
                        VALUES
                        (
                            @FkUserID, @BorderWidth, @BorderColour, @BGroundColour,
                            @FontStyle, @FontColour, @Padding, 1
                        )
                        ;";
                        cmd.Parameters.AddWithValue(@"FkUserID", ID);
                        cmd.Parameters.AddWithValue(@"BorderColour", BorderColour);
                        cmd.Parameters.AddWithValue(@"BorderWidth", BorderWidth);
                        cmd.Parameters.AddWithValue(@"BGroundColour", BGroundColour);
                        cmd.Parameters.AddWithValue(@"FontStyle", FontStyle);
                        cmd.Parameters.AddWithValue(@"FontColour", FontColour);
                        cmd.Parameters.AddWithValue(@"Padding", Padding);

                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    return null;
                }
                else // Retrieve the post's theme...
                {
                    cmd.CommandText = @"
                        SELECT 
                            [BorderWidth], [BorderColour], [BGroundColour], 
                            [FontStyle], [FontColour], [Padding]
                        FROM tblPosts
                        WHERE [FkUserID] = @FkUserID AND [IsDefault] = 1
                    ;";
                    cmd.Parameters.AddWithValue(@"BorderWidth", BorderWidth);
                    cmd.Parameters.AddWithValue(@"FkUserID", ID);
                    cmd.Parameters.AddWithValue(@"BorderColour", BorderColour);
                    cmd.Parameters.AddWithValue(@"BGroundColour", BGroundColour);
                    cmd.Parameters.AddWithValue(@"FontStyle", FontStyle);
                    cmd.Parameters.AddWithValue(@"FontColour", FontColour);
                    cmd.Parameters.AddWithValue(@"Padding", Padding);

                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string borWidth = reader.GetString(0);
                        string borColour = reader.GetString(1);
                        string bGColour = reader.GetString(2);
                        string fontStyle = reader.GetString(3);
                        string fontColour = reader.GetString(4);
                        string padding = reader.GetString(5);
                        customs.Add(new string[] { borWidth, borColour, bGColour, fontStyle, fontColour, padding });  
                    }
                    con.Close();
                    return customs;
                }
                
            }
            catch (Exception E)
            {
                throw (E);
            }
        }
        public List<string[]> LoadPosts(bool isUserPosts) // Load user's posts or friend's posts.
        {
            List<string[]> posts = new List<string[]>();
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                // Use an array to store the ID's of user's friends.
                // In SQL, use 'WHERE fkuserid IN (1, 2, 5, 77)' and use parameter
                if (isUserPosts) // Retrieve user's posts...
                {
                    cmd.CommandText = @"
                    SELECT 
                        p.BorderWidth, p.BorderColour, p.BGroundColour, p.FontStyle,
                        p.FontColour, p.Padding, p.PostText, u.UserName
                    FROM [tblPosts] p INNER JOIN [tblUser] u on p.FkUserID = u.ID
                    WHERE p.FkUserID = @FkUserID AND p.IsDefault = 0
                    ;";
                    cmd.Parameters.AddWithValue(@"FkUserID", ID);
                    cmd.ExecuteNonQuery();
                }
                else // Retrieve friend's posts...
                {
                    cmd.CommandText = @"
                    SELECT 
                        p.BorderWidth, p.BorderColour, p.BGroundColour, p.FontStyle,
                        p.FontColour, p.Padding, p.PostText, u.UserName
                    FROM [tblPosts] p INNER JOIN [tblUser] u on p.FkUserID = u.ID
                    WHERE p.FkUserID IN (
                        SELECT FkUserID1
                        FROM [tblFriends]
                        WHERE FkUserID2 = @FkUserID
                        UNION
                        SELECT FkUserID2
                        FROM [tblFriends]
                        WHERE FkUserID1 = @FkUserID
                        )   
                    AND p.IsDefault = 0
                    ORDER BY p.PostDate DESC
                    ;";
                    cmd.Parameters.AddWithValue(@"FkUserID", ID);
                    cmd.ExecuteNonQuery();
                }
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string borWidth = reader.GetString(0);
                    string borColour = reader.GetString(1);
                    string bGColour = reader.GetString(2);
                    string fontStyle = reader.GetString(3);
                    string fontColour = reader.GetString(4);
                    string padding = reader.GetString(5);
                    string postText = reader.GetString(6);
                    string userName = reader.GetString(7);
                    posts.Add(new string[] {  borWidth, borColour, bGColour, fontStyle,
                                             fontColour, padding, postText, userName});
                }
                con.Close();
                return posts;
            }
            catch (Exception E)
            {
                throw (E);
            }
        }
        public void PublishPost(string BorderColour, string BorderWidth, string BGroundColour, string FontStyle, string FontColour, string Padding, string PostText)
        {
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = @"
                    INSERT INTO [tblPosts]
                    (
                        [FkUserID], [BorderWidth], [BorderColour], [BGroundColour],
                        [FontStyle], [FontColour], [Padding], [IsDefault], [PostText]
                    )
                    VALUES
                    (
                        @FkUserID, @BorderWidth, @BorderColour, @BGroundColour,
                        @FontStyle, @FontColour, @Padding, 0, @PostText
                    )
                ;";
                cmd.Parameters.AddWithValue(@"FkUserID", ID);
                cmd.Parameters.AddWithValue(@"BorderColour", BorderColour);
                cmd.Parameters.AddWithValue(@"BorderWidth", BorderWidth);
                cmd.Parameters.AddWithValue(@"BGroundColour", BGroundColour);
                cmd.Parameters.AddWithValue(@"FontStyle", FontStyle);
                cmd.Parameters.AddWithValue(@"FontColour", FontColour);
                cmd.Parameters.AddWithValue(@"Padding", Padding);
                cmd.Parameters.AddWithValue(@"PostText", PostText);

                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception E)
            {
                throw (E);
            }
        }
        public List<string[]> GetMessages() //Read
        {
            List<string[]> messages = new List<string[]>();
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = @"
                    SELECT Message, SendTime
                    FROM [tblMessage]
                    WHERE FkReceiverID = @receiverId
                    ORDER BY SendTime Desc
                ";
                cmd.Parameters.AddWithValue("@receiverId", this.ID);
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string msg = reader.GetString(0); // Message
                    DateTime dateTime = reader.GetDateTime(1); // The .GetDateTime() allows us to
                    string date = dateTime.ToString();         // expect the data type from the 
                    messages.Add(new string[] { msg, date });  // database as DateTime.
                }
                con.Close();
                return messages;
            }
            catch (Exception E)
            {
                throw (E);
            }
        }
        public List<string[]> SearchUser(string searchQuery)
        {
            List<string[]> matches = new List<string[]>();
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = @"
                    SELECT ID, UserName
                    FROM [tblUser]
                    WHERE LOWER(UserName) LIKE @searchQuery
                ";
                cmd.Parameters.AddWithValue("@searchQuery", "%" + searchQuery.ToLower() + "%");
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    long longId = (long)reader.GetSqlInt64(0);
                    string username = reader.GetString(1);      
                    matches.Add(new string[] {longId.ToString(), username});  
                }
                con.Close();
                return matches;
            }
            catch (Exception E)
            {
                throw (E);
            }
        }
        public void AddFriend(long friendID)
        {
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                cmd.CommandText = @"
                    INSERT INTO [tblFriends]
                    (FkUserID1, FkUserID2)
                    VALUES
                        ( @userID, @friendID );  
                ";
                cmd.Parameters.AddWithValue("@friendID", friendID);
                cmd.Parameters.AddWithValue("@userID", ID);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception E)
            {
                throw (E);
            }
        }
        public List <string[]> FriendsList()
        {
            List<string[]> friends = new List<string[]>();
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = @"
                    SELECT u.ID, u.UserName
                    FROM [tblUser] u INNER JOIN [tblFriends] f 
                    ON (u.ID = f.FkUserID1 OR u.ID = f.FkUserID2)  
                    WHERE f.FkUserID1 = @userID OR f.FkUserID2 = @userID
                ";
                cmd.Parameters.AddWithValue("@userID", ID);
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    long id = (long)reader.GetSqlInt64(0);
                    string friendID = id.ToString();
                    string friendName = reader.GetString(1);
                    friends.Add(new string[] {friendID, friendName}); 
                }
                con.Close();
                return friends;
            }
            catch (Exception E)
            {
                throw (E);
            }
        }

        public List<string[]> GetThemes()
        {
            List<string[]> unlocked_themes = new List<string[]>();
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = @"
                    SELECT tblCustom.CustomName, tblCustom.ID, tblCustom.CustomType
                    FROM [tblCustom]
                    INNER JOIN tblUnlocks ON tblUnlocks.FkCustomID = tblCustom.ID
                    WHERE tblUnlocks.FkUserID = @userID
                ";
                cmd.Parameters.AddWithValue("@userID", this.ID);
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string theme = reader.GetString(0);
                    long themeIDLong = reader.GetInt64(1);
                    string themeID = Convert.ToString(themeIDLong);
                    string customType = reader.GetString(2);
                    unlocked_themes.Add(new string[] {theme, themeID, customType});  
                }
                return unlocked_themes;
            }
            catch (Exception E)
            {
                throw (E);
            }
        }

        public List<string[]> GetLockedThemes()
        {
            List<string[]> locked_themes = new List<string[]>();
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = @"
                    SELECT 
                        custom.CustomName, custom.ID
                    FROM 
                        tblCustom custom
                    LEFT JOIN 
                        tblUnlocks unlock 
                    ON 
                        custom.ID = unlock.FkCustomID 
                    AND 
                        unlock.FkUserID = @userID
                    WHERE 
                        unlock.FkUserID IS NULL;
                ";
                cmd.Parameters.AddWithValue("@userID", this.ID);
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string theme = reader.GetString(0);
                    long themeIDLong = reader.GetInt64(1);
                    string themeID = Convert.ToString(themeIDLong);
                    locked_themes.Add(new string[] {theme, themeID});
                }
                return locked_themes;
            }
            catch (Exception E)
            {
                throw (E);
            }
        }

        public void BuyCustom(long customID)
        {
            try
            {
                SqlConnection con = new SqlConnection();
                con.ConnectionString = _CON_STRING;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = @"
                    INSERT INTO tblUnlocks (FkUserID, FkCustomID)
                    VALUES (@userID, @FkCustomID)
                ";
                cmd.Parameters.AddWithValue("@userID", this.ID);
                cmd.Parameters.AddWithValue("@FkCustomID", customID);
                cmd.ExecuteNonQuery();
            }
            catch (Exception E)
            {
                throw (E);
            }
        }


    }
}
