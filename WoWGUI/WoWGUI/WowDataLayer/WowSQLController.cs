//------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//  C# code sample
//
//  Author:  Bradley Gray
//  Email:   strider_3029@yahoo.com
//
//  This was a test application developed to allow the management and creation of World of WarCraft accounts, and the player characters on the account. 
//  When the application starts, a user is able to login and take on the role of a new player, able to add and remove characters from their 'account'.
//  The characters are persisted through application restarts via the use of an SQL database.  An administrative view is also part of the program, 
//  allowing admins to edit certain properties of the player's characters (note - this is not linked to whether the user is a Windows admin).
//
//  The test included a list of technical requirements, but I have chosen to leave these out as I am not the author of the requirements and do not
//  wish to include any information that could be considered confidential or sensitive.
//
//  Notes:
//  For the solution to work, visual studio needs to be run as an administrator in order for the WCF end points to be created (under normal windows security settings).
//  It will need SQL Express 2008 installed, and the user must be logged in on an account with SQL permissions to create and modify the database/tables.
//  The WCF endpoints are using port:8000 which should be safe from conflicts, I do not know of any common programs that use this port.
//  The SQL interface for account name and password is not case sensitive to make testing easier.
//  Modifying the character levels using admin tools is built into the datagrid control in the admin options dialog, by double clicking on the character level values.
//  Read ReadMe.txt for more information.
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using Common;

namespace WowDataLayer
{
   public static class WowSQLController
   {
      private static string databaseName = "BGrayBlizzTestDB";
      private static string tablePlayerData = "PlayerData";
      private static string tableCharacters = "Characters";

      /// <summary>
      /// Check the BGrayBlizzTestDB exists.
      /// </summary>
      /// <returns>
      /// True:  The database already exists.
      /// False: The database does not exist.
      /// </returns>
      public static bool CheckDatabaseExists()
      {
         bool retVal = false;
         
         string sqlValidateDBExists = string.Format("SELECT database_id FROM sys.databases WHERE Name = '{0}'", databaseName);

         using(SqlConnection sqlConn = new SqlConnection("server=(local)\\SQLEXPRESS;Trusted_Connection=yes"))
         {
            try
            {
               using(SqlCommand sqlCmd = new SqlCommand(sqlValidateDBExists, sqlConn))
               {
                  sqlConn.Open();

                  object sqlRetVal = sqlCmd.ExecuteScalar();

                  if(sqlRetVal != null)
                  {
                     int databaseID = (int)sqlRetVal;

                     retVal = (databaseID > 0);
                  }
               }
            }
            catch(Exception)
            {
               /* Do exception logging here */
               retVal = false;
            }
            finally
            {
               if(sqlConn.State == ConnectionState.Open)
               {
                  sqlConn.Close();
               }
            }
         }

         return retVal;
      }

      /// <summary>
      /// Create the BGrayBlizzTestDB database and PlayerData/Character tables.
      /// </summary>
      /// <returns>
      /// A string describing the error returned from SQLExpress.
      /// </returns>
      public static string CreateDatabaseNTables()
      {
         string errStr = "";

         string cmdString = "CREATE DATABASE " + databaseName;

         using(SqlConnection sqlConn = new SqlConnection("Server=(local)\\SQLEXPRESS;Integrated security=SSPI;database=master"))
         {
            try
            {
               using(SqlCommand sqlCmd = new SqlCommand(cmdString, sqlConn))
               {
                  sqlConn.Open();
                  sqlCmd.ExecuteNonQuery();
               }
            }
            catch(Exception ex)
            {
               errStr = "An exception occurred when trying to create the database:\n" + ex.Message;
            }
            finally
            {
               if(sqlConn.State == ConnectionState.Open)
               {
                  sqlConn.Close();
               }
            }
         }

         // If the database was created without error, create the tables.
         if(string.IsNullOrEmpty(errStr))
         {
            errStr = CreateTables();
         }

         return errStr;
      }

      /// <summary>
      /// Check the PlayerData and Character tables exist.
      /// </summary>
      /// <returns>
      /// True:  The tables already exist.
      /// False: The tables do not exist.
      /// </returns>
      public static bool CheckTableExists()
      {
         bool retVal = false;

         string sqlValidateDBExists = string.Format("Use {0}; SELECT COUNT(*) FROM sys.tables;", databaseName);

         using(SqlConnection sqlConn = new SqlConnection(string.Format("Integrated Security=SSPI;Initial Catalog={0};Data Source=(local)\\SQLEXPRESS;", databaseName)))
         {
            try
            {
               using(SqlCommand sqlCmd = new SqlCommand(sqlValidateDBExists, sqlConn))
               {
                  sqlConn.Open();
                  int tableCount = (int)sqlCmd.ExecuteScalar();

                  retVal = (0 < tableCount);
               }
            }
            catch(Exception)
            {
               /* Do exception logging here */
               retVal = false;
            }
            finally
            {
               if(sqlConn.State == ConnectionState.Open)
               {
                  sqlConn.Close();
               }
            }
         }

         return retVal;
      }

      /// <summary>
      /// Create the PlayerData and Character tables.
      /// </summary>
      /// <returns>
      /// A string describing the error returned from SQLExpress.
      /// </returns>
      public static string CreateTables()
      {
         string errStr = "";
         string cmdStringPlayerData = string.Format("CREATE TABLE {0} ([accountName] [nchar](30) NOT NULL PRIMARY KEY, [password] [nchar](30) NOT NULL, [isAdmin] [bit] NOT NULL)", tablePlayerData);
         string cmdStringCharacters = string.Format("CREATE TABLE {0}([accountName] [nchar](30) NOT NULL, [charName] [nchar](20) NOT NULL PRIMARY KEY, [level] [int] NOT NULL, [race] [int] NOT NULL, " +
            "[class] [int] NOT NULL, [isActive] [bit] NOT NULL, FOREIGN KEY(accountName) REFERENCES {1} (accountName) ON DELETE CASCADE)", tableCharacters, tablePlayerData);


         using(SqlConnection sqlConn = new SqlConnection(string.Format("Integrated Security=SSPI;Initial Catalog={0};Data Source=(local)\\SQLEXPRESS;", databaseName)))
         {
            SqlCommand cmdCreatePlayerData = new SqlCommand(cmdStringPlayerData, sqlConn);
            SqlCommand cmdCreateCharacters = new SqlCommand(cmdStringCharacters, sqlConn);

            try
            {
               sqlConn.Open();
               cmdCreatePlayerData.ExecuteNonQuery();
               cmdCreateCharacters.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
               errStr = "An exception occurred when trying to create the database tables:\n" + ex.Message;
            }
            finally
            {
               if(sqlConn.State == ConnectionState.Open)
               {
                  sqlConn.Close();
               }
            }
         }

         return errStr;
      }

      /// <summary>
      /// Create a new player account entry in the SQL database.
      /// </summary>
      /// <param name="accountName">Name of the account.</param>
      /// <param name="password">Password for the account.</param>
      /// <param name="isAdmin">The account is an administrator.</param>
      /// <returns>
      /// True:  A new player account was created successfully.
      /// False: The player account could not be created.
      /// </returns>
      public static bool CreateAccount(string accountName, string password, bool isAdmin)
      {
         bool retVal = true;

         using(SqlConnection sqlConn = new SqlConnection(string.Format("Integrated Security=SSPI;Initial Catalog={0};Data Source=(local)\\SQLEXPRESS;", databaseName)))
         {
            try
            {
               using(SqlCommand sqlCmd = sqlConn.CreateCommand())
               {
                  sqlConn.Open();

                  // Create INSERT statement with named parameters
                  sqlCmd.CommandText = string.Format("INSERT INTO {0} (accountName, password, isAdmin) VALUES (@accountName, @password, @isAdmin)", tablePlayerData);

                  // Add Parameters to Command Parameters collection
                  sqlCmd.Parameters.Add("@accountName", SqlDbType.VarChar, 30).Value = accountName;
                  sqlCmd.Parameters.Add("@password", SqlDbType.VarChar, 30).Value = password;
                  sqlCmd.Parameters.Add("@isAdmin", SqlDbType.Bit).Value = isAdmin;

                  // Prepare command
                  sqlCmd.Prepare();

                  sqlCmd.ExecuteNonQuery();
               }
            }
            catch(Exception)
            {
               /* Do exception logging here */
               retVal = false;
            }
            finally
            {
               if(sqlConn.State == ConnectionState.Open)
               {
                  sqlConn.Close();
               }
            }
         }

        return retVal;
      }

      /// <summary>
      /// Add a new character to the database.
      /// </summary>
      /// <param name="playerData">The account to add the character to.</param>
      /// <param name="character">The new character we are adding.</param>
      /// <returns>
      /// True:  The new character was added successfully.
      /// False: The new character was not added to the database. Likely due to a conflicting Character name.
      /// </returns>
      public static bool AddCharacter(PlayerData playerData, Character character)
      {
         bool retVal = true;
         bool charNameExists = false;

         using(SqlConnection sqlConn = new SqlConnection(string.Format("Integrated Security=SSPI;Initial Catalog={0};Data Source=(local)\\SQLEXPRESS;", databaseName)))
         {
            try
            {
               using(SqlCommand sqlCmd = sqlConn.CreateCommand())
               {
                  sqlConn.Open();

                  sqlCmd.CommandText = string.Format("SELECT * FROM {0} WHERE charName=@charName", tableCharacters);
                  sqlCmd.Parameters.Add("@charName", SqlDbType.VarChar, 30).Value = character.Name;

                  sqlCmd.Prepare();

                  using(SqlDataReader reader = sqlCmd.ExecuteReader())
                  {
                     if(reader.HasRows)
                     {
                        charNameExists = true;
                        retVal = false;
                     }
                  }

                  // If we didn't find the charName in the DB, insert the new entry
                  if(!charNameExists)
                  {
                     sqlCmd.CommandText = string.Format("INSERT INTO {0} VALUES (@accountName, @charName, @level, @race, @class, @isActive)", tableCharacters);

                     // Add Parameters to Command Parameters collection
                     sqlCmd.Parameters.Add("@accountName", SqlDbType.VarChar, 30).Value = playerData.AccountName;
                     sqlCmd.Parameters.Add("@level", SqlDbType.Int).Value = character.Level;
                     sqlCmd.Parameters.Add("@race", SqlDbType.Int).Value = character.Race;
                     sqlCmd.Parameters.Add("@class", SqlDbType.Int).Value = character.CharClass;
                     sqlCmd.Parameters.Add("@isActive", SqlDbType.Bit).Value = character.IsActive;

                     // Prepare command
                     sqlCmd.Prepare();

                     sqlCmd.ExecuteNonQuery();
                  }
               }
            }
            catch(Exception)
            {
               /* Do exception logging here */
               retVal = false;
            }
            finally
            {
               if(sqlConn.State == ConnectionState.Open)
               {
                  sqlConn.Close();
               }
            }
         }

         return retVal;
      }

      /// <summary>
      /// Uodate the accounts character data.
      /// </summary>
      /// <param name="playerData">The account to update.</param>
      /// <returns>
      /// True:  The accounts characters were successfully updated.
      /// False: The accounts characters were not updated.
      /// </returns>
      public static bool UpdateCharacters(PlayerData playerData)
      {
         bool retVal = true;

         using(SqlConnection sqlConn = new SqlConnection(string.Format("Integrated Security=SSPI;Initial Catalog={0};Data Source=(local)\\SQLEXPRESS;", databaseName)))
         {
            try
            {
               using(SqlCommand sqlCmd = sqlConn.CreateCommand())
               {
                  sqlConn.Open();

                  sqlCmd.CommandText = string.Format("UPDATE {0} SET accountName=@accountName, charName=@charName, level= @level, race=@race, class=@class, isActive=@isActive WHERE accountName='{1}' " +
                     "AND charName=@charName IF @@ROWCOUNT=0 INSERT INTO {0} VALUES (@accountName, @charName, @level, @race, @class, @isActive)", tableCharacters, playerData.AccountName);

                  // Add Parameters to Command Parameters collection
                  sqlCmd.Parameters.Add("@accountName", SqlDbType.VarChar, 30).Value = playerData.AccountName;
                  sqlCmd.Parameters.Add("@charName", SqlDbType.VarChar, 30);
                  sqlCmd.Parameters.Add("@level", SqlDbType.Int);
                  sqlCmd.Parameters.Add("@race", SqlDbType.Int);
                  sqlCmd.Parameters.Add("@class", SqlDbType.Int);
                  sqlCmd.Parameters.Add("@isActive", SqlDbType.Bit);

                  // Update each character in the account
                  foreach(Character character in playerData.Characters)
                  {
                     sqlCmd.Parameters["@charName"].Value = character.Name;
                     sqlCmd.Parameters["@level"].Value = character.Level ;
                     sqlCmd.Parameters["@race"].Value = character.Race;
                     sqlCmd.Parameters["@class"].Value = character.CharClass;
                     sqlCmd.Parameters["@isActive"].Value = character.IsActive;

                     // Prepare command for repeated execution
                     sqlCmd.Prepare();

                     sqlCmd.ExecuteNonQuery();
                  }
               }
            }
            catch(Exception)
            {
               /* Do exception logging here */
               retVal = false;
            }
            finally
            {
               if(sqlConn.State == ConnectionState.Open)
               {
                  sqlConn.Close();
               }
            }
         }

         return retVal;
      }

      /// <summary>
      /// Update the character levels for all the characters in the list.
      /// </summary>
      /// <param name="characters">List of characters with updated level values.</param>
      public static void UpdateCharacterLevels(List<Character> characters)
      {
         using(SqlConnection sqlConn = new SqlConnection(string.Format("Integrated Security=SSPI;Initial Catalog={0};Data Source=(local)\\SQLEXPRESS;", databaseName)))
         {
            try
            {
               using(SqlCommand sqlCmd = sqlConn.CreateCommand())
               {
                  sqlConn.Open();

                  sqlCmd.CommandText = string.Format("UPDATE {0} SET level=@level WHERE charName=@charName", tableCharacters);

                  // Add Parameters to Command Parameters collection
                  sqlCmd.Parameters.Add("@charName", SqlDbType.VarChar, 30);
                  sqlCmd.Parameters.Add("@level", SqlDbType.Int);

                  // Update each character
                  foreach(Character character in characters)
                  {
                     sqlCmd.Parameters["@charName"].Value = character.Name;
                     sqlCmd.Parameters["@level"].Value = character.Level;

                     // Prepare command for repeated execution
                     sqlCmd.Prepare();

                     sqlCmd.ExecuteNonQuery();
                  }
               }
            }
            catch(Exception)
            { /* Do exception logging here */ }
            finally
            {
               if(sqlConn.State == ConnectionState.Open)
               {
                  sqlConn.Close();
               }
            }
         }
      }

      /// <summary>
      /// Attempt to login the account, returning the matching playerData.
      /// </summary>
      /// <param name="accountName">Name of the account.</param>
      /// <param name="password">Login password.</param>
      /// <param name="isAdmin">Out value to return whether the account is an administrative login or not.</param>
      /// <returns>
      /// The accounts playerData and character data.
      /// </returns>
      public static PlayerData LoginAccount(string accountName, string password, out bool isAdmin)
      {
         PlayerData retVal = new PlayerData();
         bool hasLoggedIn = false;
         isAdmin = false;

         string cmdStringPlayerData = string.Format("SELECT * FROM {0} WHERE accountName = '{1}' AND password = '{2}'", tablePlayerData, accountName, password);
         string cmdStringCharacters = string.Format("SELECT * FROM {0} WHERE accountName = '{1}'", tableCharacters, accountName);

         using(SqlConnection sqlConn = new SqlConnection(string.Format("Integrated Security=SSPI;Initial Catalog={0};Data Source=(local)\\SQLEXPRESS;", databaseName)))
         {
            try
            {
               using(SqlCommand sqlCmd = new SqlCommand(cmdStringPlayerData, sqlConn))
               {
                  sqlConn.Open();

                  using(SqlDataReader reader = sqlCmd.ExecuteReader())
                  {
                     if(reader.HasRows)
                     {
                        reader.Read();
                        hasLoggedIn = true;
                        retVal.AccountName = reader.GetString(0).Trim();
                        isAdmin = reader.GetBoolean(2);
                     }

                     reader.Close();
                  }
               }

               // If the login was successful, retrieve the characters.
               if(hasLoggedIn)
               {
                  using(SqlCommand sqlCmd = new SqlCommand(cmdStringCharacters, sqlConn))
                  {
                     using(SqlDataReader reader = sqlCmd.ExecuteReader())
                     {
                        if(reader.HasRows)
                        {
                           while(reader.Read())
                           {
                              Character character = new Character(reader.GetString(1).Trim(), (Race)reader.GetInt32(3), (CharClass)reader.GetInt32(4), reader.GetInt32(2), reader.GetBoolean(5));
                              retVal.AddCharacter(character);
                           }
                        }

                        reader.Close();
                     }
                  }
               }
            }
            catch(Exception)
            {
               /* Do exception logging here */
               retVal = null;
            }
            finally
            {
               if(sqlConn.State == ConnectionState.Open)
               {
                  sqlConn.Close();
               }
            }
         }

         return retVal;
      }

      /// <summary>
      /// Retrieve the characters for the account.
      /// </summary>
      /// <param name="accountName">Account name.</param>
      /// <returns>
      /// List of characters associated with the account.
      /// </returns>
      public static List<Character> RetrieveAccountCharacters(string accountName)
      {
         List<Character> retVal = new List<Character>();

         string cmdStringCharacters = string.Format("SELECT * FROM {0} WHERE accountName = '{1}'", tableCharacters, accountName);

         using(SqlConnection sqlConn = new SqlConnection(string.Format("Integrated Security=SSPI;Initial Catalog={0};Data Source=(local)\\SQLEXPRESS;", databaseName)))
         {
            try
            {
               using(SqlCommand sqlCmd = new SqlCommand(cmdStringCharacters, sqlConn))
               {
                  sqlConn.Open();

                  using(SqlDataReader reader = sqlCmd.ExecuteReader())
                  {
                     if(reader.HasRows)
                     {
                        while(reader.Read())
                        {
                           Character character = new Character(reader.GetString(1).Trim(), (Race)reader.GetInt32(3), (CharClass)reader.GetInt32(4), reader.GetInt32(2), reader.GetBoolean(5));
                           retVal.Add(character);
                        }
                     }

                     reader.Close();
                  }
               }
            }
            catch(Exception)
            {
               /* Do exception logging here */
               retVal = null;
            }
            finally
            {
               if(sqlConn.State == ConnectionState.Open)
               {
                  sqlConn.Close();
               }
            }
         }

         return retVal;
      }

      /// <summary>
      /// Retrieve all the characters stored in the database.
      /// </summary>
      /// <returns>
      /// List of all characters in the database.
      /// </returns>
      public static List<Character> RetrieveAllCharacters()
      {
         List<Character> retVal = new List<Character>();

         string cmdStringCharacters = string.Format("SELECT * FROM {0} ORDER BY 'accountName'", tableCharacters);

         using(SqlConnection sqlConn = new SqlConnection(string.Format("Integrated Security=SSPI;Initial Catalog={0};Data Source=(local)\\SQLEXPRESS;", databaseName)))
         {
            try
            {
               using(SqlCommand sqlCmd = new SqlCommand(cmdStringCharacters, sqlConn))
               {
                  sqlConn.Open();

                  using(SqlDataReader reader = sqlCmd.ExecuteReader())
                  {
                     if(reader.HasRows)
                     {
                        while(reader.Read())
                        {
                           Character character = new Character(reader.GetString(1).Trim(), (Race)reader.GetInt32(3), (CharClass)reader.GetInt32(4), reader.GetInt32(2), reader.GetBoolean(5));
                           retVal.Add(character);
                        }
                     }

                     reader.Close();
                  }
               }
            }
            catch(Exception)
            {
               /* Do exception logging here */
               retVal = null;
            }
            finally
            {
               if(sqlConn.State == ConnectionState.Open)
               {
                  sqlConn.Close();
               }
            }
         }

         return retVal;
      }

      /// <summary>
      /// Delete an accounts character from the database.
      /// </summary>
      /// <param name="playerData">The account we are deleting a character from.</param>
      /// <param name="character">The character we are deleting.</param>
      public static void DeleteCharacter(PlayerData playerData, Character character)
      {
         using(SqlConnection sqlConn = new SqlConnection(string.Format("Integrated Security=SSPI;Initial Catalog={0};Data Source=(local)\\SQLEXPRESS;", databaseName)))
         {
            try
            {
               using(SqlCommand sqlCmd = sqlConn.CreateCommand())
               {
                  sqlConn.Open();

                  sqlCmd.CommandText = string.Format("DELETE FROM {0} WHERE accountName=@accountName AND charName=@charName", tableCharacters, playerData.AccountName);

                  // Add Parameters to Command Parameters collection
                  sqlCmd.Parameters.Add("@accountName", SqlDbType.VarChar, 30).Value = playerData.AccountName;
                  sqlCmd.Parameters.Add("@charName", SqlDbType.VarChar, 30).Value = character.Name;

                  // Prepare command
                  sqlCmd.Prepare();

                  sqlCmd.ExecuteNonQuery();
                  
               }
            }
            catch(Exception)
            { /* Do exception logging here */ }
            finally
            {
               if(sqlConn.State == ConnectionState.Open)
               {
                  sqlConn.Close();
               }
            }
         }
      }

   }// class
}// namespace
