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
using System.ComponentModel.DataAnnotations;


namespace Common
{
   [Serializable]
   public class PlayerData
   {
      /// <summary>
      /// Maximum number of characters
      /// </summary>
      public static readonly int MAX_NUM_CHARACTERS = 10;

      /// <summary>
      /// Level requirement to create death knight characters
      /// </summary>
      public static readonly int DEATHKNIGHT_LEVEL_REQUIREMENT = 55;

      string accountName = "";
      List<Character> characters = new List<Character>(1);

      /// <summary>
      /// Get the account name for this account.
      /// </summary>
      [Required(ErrorMessage = "Account name is required.")]
      [StringLength(30, MinimumLength = 5, ErrorMessage = "Account name must be 5-30 characters long.")]
      [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Special characters are not allowed in the account name.")]
      public string AccountName
      {
         get { return accountName; }
         set
         {
            Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "AccountName" });
            accountName = value;
         }
      }

      /// <summary>
      /// Get the full list of characters for this player account.
      /// Use GetActiveCharacters to get a list of only the active characters.
      /// </summary>
      public List<Character> Characters
      { get { return characters; } set { characters = value; } }

      /// <summary>
      /// Default Constructor
      /// </summary>
      public PlayerData()
      {
      }

      /// <summary>
      /// Add a character to this players account.
      /// </summary>
      /// <param name="character"></param>
      public void AddCharacter(Character character)
      {
         characters.Add(character);
      }

      /// <summary>
      /// Check if we can add a character to the account.
      /// </summary>
      /// <returns>
      /// True:  We can add a character to the account.
      /// False: We cannot add a character to the account.
      /// </returns>
      public bool CanAddCharacter()
      {
         return (this.characters.Count < MAX_NUM_CHARACTERS);
      }

      /// <summary>
      /// Attempt to reactivate a character from the account.
      /// </summary>
      /// <param name="character">Character to deactivate.</param>
      public bool ReactivateCharacter(Character character)
      {
         bool retVal = false;
         Faction activeFaction = GetActiveFaction();
         if(activeFaction == character.Faction || activeFaction == Faction.Both)
         {
            retVal = true;
            character.IsActive = true;
         }

         return retVal;
      }

      /// <summary>
      /// Attempt to deactivate a character from the account.
      /// </summary>
      /// <param name="character">Character to deactivate.</param>
      public void DeactivateCharacter(Character character)
      {
         character.IsActive = false;
      }

      /// <summary>
      /// Attempt to delete a character from the account.
      /// </summary>
      /// <param name="character">Character to deactivate.</param>
      public void DeleteCharacter(Character character)
      {
         this.characters.Remove(character);
      }

      /// <summary>
      /// Gets a default character based on the accounts current data (faction, etc.).
      /// </summary>
      /// <returns></returns>
      public Character GetDefaultNewCharacter()
      {
         Character retVal;

         if(this.GetActiveFaction() == Faction.Horde)
         {
            retVal = new Character("", Race.Orc, CharClass.Warrior, 1, true);
         }
         else
         {
            retVal = new Character("", Race.Human, CharClass.Warrior, 1, true);
         }

         return retVal;
      }

      /// <summary>
      /// Has this user satisfied the requirements for creating a DeathKnight.
      /// </summary>
      /// <returns>
      /// True:  The user has a character with sufficient levels to unlock Death Knight creation.
      /// False: The user does not have a character with sufficient levels to unlock Death Knight creation.
      /// </returns>
      public bool CanCreateDeathKnights()
      {
         bool retVal = false;

         foreach(Character character in characters)
         {
            if(character.IsActive && (DEATHKNIGHT_LEVEL_REQUIREMENT <= character.Level))
            {
               retVal = true; 
               break;
            }
         }

            return retVal;
      }

      /// <summary>
      /// Get the active faction for this user, which limits what faction new characters can be created for.
      /// </summary>
      /// <returns>
      /// Alliance: Only Alliance characters can be created for this user.
      /// Horde:    Only Horde characters can be created for this user.
      /// Both:     Both Alliance and Horde characters can be created for this user.
      /// </returns>
      public Faction GetActiveFaction()
      {
         Faction retVal = Faction.Both;

         foreach(Character character in characters)
         {
            if(character.IsActive)
            {
               retVal = character.Faction;
               break;
            }
         }

         return retVal;
      }
   }
}
