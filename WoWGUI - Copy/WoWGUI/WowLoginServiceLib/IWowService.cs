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

using System.Collections.Generic;
using System.ServiceModel;
using Common;

namespace WowServiceLib
{
   [ServiceContract(Namespace = "http://WowLoginServiceLib")]
   public interface IWowService
   {
      /// <summary>
      /// Login a player account.
      /// </summary>
      /// <param name="accountName">Players account name.</param>
      /// <param name="password">Account password.</param>
      /// <param name="isAdmin">Returns whether the account has administrator privileges.</param>
      /// <returns>
      /// Accounts playerData and characters
      /// </returns>
      [OperationContract]
      PlayerData Login(string accountName, string password, out bool isAdmin);

      /// <summary>
      /// Retrieve the characters for the account.
      /// </summary>
      /// <param name="accountName">Name of the players account.</param>
      /// <returns>
      /// The list of the accounts characters.
      /// </returns>
      [OperationContract]
      List<Character> RetrieveAccountCharacters(string accountName);

      /// <summary>
      /// Retrieve all of the characters in the database.
      /// </summary>
      /// <returns>
      /// List of all characters in the database.</returns>
      [OperationContract]
      List<Character> RetrieveAllCharacters();

      /// <summary>
      /// Create a new player account.
      /// </summary>
      /// <param name="accountName">Name of the player account.</param>
      /// <param name="password">Password for the account.</param>
      /// <param name="isAdmin">Account has administrator privileges.</param>
      /// <returns>
      /// True:  New account was successfully created.
      /// False: New account could not be created.
      /// </returns>
      [OperationContract]
      bool CreateAccount(string accountName, string password, bool isAdmin);

      /// <summary>
      /// Add a new character to the account.
      /// </summary>
      /// <param name="playerData">Account we are adding a character to.</param>
      /// <param name="character">The new character we are adding.</param>
      /// <returns>
      /// True:  The new character was successfully added to the player account.
      /// False: The new character could not be added to the player account.
      /// </returns>
      [OperationContract]
      bool AddCharacterToAccount(PlayerData playerData, Character character);

      /// <summary>
      /// Update the accounts characters.
      /// </summary>
      /// <param name="playerData">The account whose characters are being updated.</param>
      [OperationContract]
      void UpdateCharactersForPlayer(PlayerData playerData);

      /// <summary>
      /// Update the levels of the characters passed in.
      /// </summary>
      /// <param name="characters">List of characters to update.</param>
      [OperationContract]
      void UpdateCharacterLevels(List<Character> characters);

      /// <summary>
      /// Delete a character from an account.
      /// </summary>
      /// <param name="playerData">Account we are deleting a character from.</param>
      /// <param name="character">Character to delete from the account.</param>
      [OperationContract]
      void DeleteCharacterFromAccount(PlayerData playerData, Character character);
   }
}
