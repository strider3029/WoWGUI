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
using Common;

namespace WowClient
{
   /// <summary>
   /// Interface to the WoW login service client and view controller.
   /// Provides the connection to the WoWServiceHost and controls the GUI views.
   /// </summary>
   public interface IWowClientController
   {
      /// <summary>
      /// Create a new account.
      /// </summary>
      /// <param name="accountName">Account name.</param>
      /// <param name="password">Password.</param>
      /// <param name="isAdmin">Account is an administrator.</param>
      /// <returns>
      /// True:  If account was created successfully.
      /// False: Account was not created.
      /// </returns>
      bool CreateAccount(string accountName, string password, bool isAdmin);

      /// <summary>
      /// Attempt to 'Login' a player account, and retrieve their playerData from the WowServiceHost.
      /// </summary>
      /// <param name="accountName">Account name we are logging in.</param>
      /// <param name="password">Password for the account we are logging in.</param>
      void Login(string accountName, string password);

      /// <summary>
      /// 'Logout' the currently active player account and redisplay the login interface.
      /// </summary>
      void Logout();

      /// <summary>
      /// Save this accounts character data.
      /// </summary>
      void SaveAccountsCharacterData();

      /// <summary>
      /// Update the levels of all the characters in the list.
      /// </summary>
      void UpdateCharacterLevels(List<Character> characters);

      /// <summary>
      /// Show the administration options dialog.
      /// </summary>
      void ShowAdminOptions();

      /// <summary>
      /// Display the Account Creation interface.
      /// </summary>
      void ShowCreateAccount();

      /// <summary>
      /// Display the Character Creation screen if the current account can create more characters.
      /// </summary>
      /// <returns>
      /// True:  If the logged in account can still create characters.
      /// False: If the logged in account has reached the maximum number of characters, or noone is logged in.
      /// </returns>
      bool ShowCharacterCreation();

      /// <summary>
      /// Display the Character Select interface.
      /// Closes the login interface if it is still active.
      /// </summary>
      void ShowCharacterSelect();

      /// <summary>
      /// Set the Current Threads Current Culture to the region described by the string.
      /// </summary>
      /// <param name="region">Region string.</param>
      void SetCultureInfo(string region);

      /// <summary>
      /// Get the List of region codes supported by our localization.
      /// </summary>
      /// <returns>
      /// List of strings representing the region codes supported by our localization.
      /// </returns>
      List<string> GetRegionList();

      /// <summary>
      /// Get the characters of the logged in account.
      /// </summary>
      /// <returns>
      /// The list of characters for the active account.
      /// If noone is logged in, returns an empty character list.
      /// </returns>
      List<Character> GetAccountCharacters();

      /// <summary>
      /// Get all the characters stored in the database.
      /// </summary>
      /// <returns>
      /// All the characters saved to the database.
      /// </returns>
      List<Character> GetAllCharacters();

      /// <summary>
      /// Attempt to add a character to the account.
      /// </summary>
      /// <param name="character">Character to add.</param>
      /// <returns>The <paramref name="character"/> was successfully added to the account.</returns>
      bool AddCharacter(Character character);

      /// <summary>
      /// Check if we can add a character to the account.
      /// </summary>
      /// <returns>
      /// True:  We can add a character to the account.
      /// False: We cannot add a character to the account.
      /// </returns>
      bool CanAddCharacter();

      void DeleteCharacter(Character character);

      /// <summary>
      /// Attempt to deactivate a character from the account.
      /// </summary>
      /// <param name="character">Character to delete.</param>
      void DeactivateCharacter(Character character);

      /// <summary>
      /// Attempt to reactivate a character on the account.
      /// </summary>
      /// <param name="character">Character to reactivate.</param>
      void ReactivateCharacter(Character character);
   }
}
