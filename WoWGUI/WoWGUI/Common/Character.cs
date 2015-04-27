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
using System.Runtime.Serialization;

namespace Common
{
   [DataContract]
   public class Character
   {
      /// <summary>
      /// Enum with all Horde races
      /// </summary>
      private static Race HordeRaces =    Race.BloodElf  | Race.Orc     | Race.Tauren;

      /// <summary>
      /// Enum with all Alliance races
      /// </summary>
      private static Race AllianceRaces = Race.Human     | Race.Gnome   | Race.Worgen;

      /// <summary>
      /// Dictionary of Illegal Race/Class combinations
      /// </summary>
      private static Dictionary<Race, List<CharClass>> RaceClassIllegalCombinations = new Dictionary<Race, List<CharClass>>()
      {
         { Race.Gnome,     new List<CharClass>() { CharClass.Druid } },
         { Race.Human,     new List<CharClass>() { CharClass.Druid } },
         { Race.Orc,       new List<CharClass>() { CharClass.Druid } },
         { Race.BloodElf,  new List<CharClass>() { CharClass.Druid, CharClass.Warrior } }
      };

      private string name = "";

      private Faction faction = Faction.Alliance; 
      private Race race = Race.Human;
      private bool isActive = true;

      private CharClass charClass = CharClass.Warrior;
      private int level = 1;

      /// <summary>
      /// Character Name
      /// </summary>
      [DataMember]
      [Required(ErrorMessage = "Character name is required.")]
      [StringLength(20, MinimumLength = 1, ErrorMessage = "Character names must be 1-20 characters long.")]
      [RegularExpression(@"^[a-zA-Z]*$", ErrorMessage = "Special characters and numbers are not allowed in character names.")]
      public string Name
      { get { return name; }
         set
         {
            Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "Name" });
            name = value;
         }
      }

      /// <summary>
      /// Character Faction
      /// </summary>
      [DataMember]
      public Faction Faction
      { get { return faction; } set { faction = value; } }

      /// <summary>
      /// Character Race
      /// </summary>
      [DataMember]
      public Race Race
      { get { return race; } set { this.SetRace(value); } }

      /// <summary>
      /// Character is Active (ie. not deleted)
      /// </summary>
      [DataMember]
      [Display(Name = "Is Active")]
      public bool IsActive
      { get { return isActive; } set { isActive = value; } }

      /// <summary>
      /// Character Class
      /// </summary>
      [DataMember]
      [Display(Name = "Class")]
      public CharClass CharClass
      { get { return charClass; } set { this.SetCharClass(value); } }

      /// <summary>
      /// Character Level
      /// </summary>
      [DataMember]
      [Range(1, 85, ErrorMessage = "Character level must be between 1 and 85.")]
      public int Level
      {
         get { return level; }
         set
         {
            Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "Level" });
            level = value;
         }
      }


      /// <summary>
      /// Character Constructor
      /// </summary>
      /// <param name="name">Character Name</param>
      /// <param name="race">Character Race</param>
      /// <param name="charClass"> Character Class</param>
      /// <param name="level">Character Level</param>
      public Character(string name, Race race, CharClass charClass, int level, bool isActive = true)
      {
         this.name = name;

         this.race = race;
         this.SetFactionFromRace();

         this.charClass = charClass;
         this.Level = level; // Use the Property accessor as it goes through the validation code

         this.isActive = isActive;

         if(!IsRaceInFaction(faction, race)) System.Diagnostics.Debug.WriteLine("Warning: A Character has been created with a race that does not belong to the faction.");
         if(!IsRaceNClassLegal(race, charClass)) System.Diagnostics.Debug.WriteLine("Warning: A Character has been created with a race/class combination that is not legal. Race: {0} Class: {1}", race.ToString(), charClass.ToString() );
      }

      /// <summary>
      /// Set the character class.
      /// </summary>
      /// <param name="newCharClass"></param>
      public void SetCharClass(CharClass newCharClass)
      {
         if(IsRaceNClassLegal(this.race, newCharClass))
         {
            this.charClass = newCharClass;
         }
      }

      /// <summary>
      /// Set the race of the character. If it causes an illegal Race/Class, also set a default class.
      /// </summary>
      /// <param name="newRace"></param>
      public void SetRace(Race newRace)
      {
         this.race = newRace;

         if(!IsRaceNClassLegal(this.race, this.charClass))
         {
            this.charClass = (this.race != Race.BloodElf) ? CharClass.Warrior : CharClass.Mage;
         }
      }

      /// <summary>
      /// Complete the new characters construction by setting the characters faction and starting level
      /// based on the characters properties.
      /// </summary>
      public void CompleteNewCharacterConstruction()
      {
         int startingCharLevel = (this.charClass == Common.CharClass.DeathKnight) ? 55 : 1;
         this.Level = startingCharLevel; // Use the property so that it uses the validation code
         this.SetFactionFromRace();
      }

      /// <summary>
      /// Set the faction of this character based on the currently selected race.
      /// </summary>
      public void SetFactionFromRace()
      {
         this.faction = ((AllianceRaces & this.race) != 0) ? Faction.Alliance : Faction.Horde;
      }

      /// <summary>
      /// Returns a value indicating if the <paramref name="race"/> passed in belongs to the <paramref name="faction"/> passed in.
      /// </summary>
      /// <param name="faction">Faction we are checking.</param>
      /// <param name="race">Race we are checking.</param>
      /// <returns>
      /// True =  The race is part of the faction.
      /// False = The race is not part of the faction.
      /// </returns>
      public static bool IsRaceInFaction(Faction faction, Race race)
      {
         bool retVal = true;

         switch(faction)
         {
            case Faction.Alliance:
               retVal = ((race & AllianceRaces) != 0);
               break;
            case Faction.Horde:
               retVal = ((race & HordeRaces) != 0);
               break;
            case Faction.Both:
               break;
            default:
               System.Diagnostics.Debug.WriteLine("Warning: A faction has been added to the Factions enum in Character.cs that is not handled by IsRaceInFaction().");
               break;
         }

         return retVal;
      }

      /// <summary>
      /// Is the <paramref name="race"/> and <paramref name="charClass"/> a legal combination.
      /// </summary>
      /// <param name="race">Race we are checking.</param>
      /// <param name="charClass">Character class we are checking.</param>
      /// <returns>      
      /// True =  The race and class is a legal combination.
      /// False = The race and class is not a legal combination.
      /// </returns>
      public static bool IsRaceNClassLegal(Race race, CharClass charClass)
      {
         bool retVal = true;

         if(RaceClassIllegalCombinations.ContainsKey(race))
         {
            retVal = !(RaceClassIllegalCombinations[race].Contains(charClass));
         }

         return retVal;
      }

      /// <summary>
      /// ToString override - 
      /// "Thrall  -  Orc \n
      /// Level ## Shaman"
      /// </summary>
      /// <returns></returns>
      public override string ToString()
      {
         return this.name + "  -  " + race.ToString() + "\nLevel " + level + " " + charClass.ToString();
      }
   }

   /// <summary>
   /// WoW Factions enum
   /// </summary>
   public enum Faction
   {
      Alliance,
      Horde,
      Both,
   }

   /// <summary>
   /// WoW Races enum
   /// </summary>
   [Flags]
   public enum Race
   {
      Human =        1, 
      Gnome =        2, 
      Worgen =       4,
      Orc =          8, 
      Tauren =       16, 
      BloodElf =     32, 
   }

   /// <summary>
   /// WoW Character Classes enum
   /// </summary>
   [Flags]
   public enum CharClass
   {
      Warrior =      1,
      Druid =        2,
      DeathKnight =  4,
      Mage =         8,
   }
}
