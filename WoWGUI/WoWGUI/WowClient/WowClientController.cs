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
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using Common;
using WowClient.WowServiceReference;

namespace WowClient
{
   /// <summary>
   /// WoW login service client and view controller.
   /// Provides the connection to the WoWServiceHost and controls the GUI views.
   /// </summary>
   class WowClientController : Application, IWowClientController
   {
      private FormLogin formLogin;
      private PlayerData playerData;
      private WowServiceClient client;
      private List<string> supportedRegions = new List<string>() { "en-US", "en-AU", "es-MX" };

      /// <summary>
      /// WowClientController Constructor.
      /// </summary>
      /// <param name="region">Initial region to set as our active culture for localization.</param>
      public WowClientController(string region = "")
      {
         this.SetCultureInfo(region);

         this.client = new WowServiceClient();

         this.formLogin = new FormLogin(this);
      }

      /// <summary>
      /// Set the Current Threads Current Culture to the region described by the string.
      /// </summary>
      /// <param name="region">Region string.</param>
      public void SetCultureInfo(string region)
      {
         if(!string.IsNullOrEmpty(region) && supportedRegions.Contains(region))
         {
            CultureInfo cultureInfo = new CultureInfo(region);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
         }
      }

      /// <summary>
      /// Get the List of region codes supported by our localization.
      /// </summary>
      /// <returns>
      /// List of strings representing the region codes supported by our localization.
      /// </returns>
      public List<string> GetRegionList()
      {
         return supportedRegions;
      }

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
      public bool CreateAccount(string accountName, string password, bool isAdmin)
      {
         bool retVal = false;

         try
         {
            NewAccountVars newAccount = new NewAccountVars() { AccountName = accountName, Password = password, IsAdmin = isAdmin };

            retVal = this.client.CreateAccount(accountName, password, isAdmin);
            if(!retVal)
            {
               MessageBox.Show("Could not create the new account.", "Account Creation Failed");
            }         
         }
         catch(ValidationException ex)
         {
            string errorMsg = string.Format("Could not create the new account.\n\n{0}", ex.ValidationResult.ErrorMessage);

            MessageBox.Show(errorMsg, "Account Creation Failed");
         }

         return retVal;
      }
      
      /// <summary>
      /// Save this accounts character data.
      /// </summary>
      //[PrincipalPermission(SecurityAction.Demand, Role = "user")]
      public void SaveAccountsCharacterData()
      {
         if(Thread.CurrentPrincipal.IsInRole("admin") || Thread.CurrentPrincipal.IsInRole("user"))
         {
            if(this.playerData != null)
            {
               this.client.UpdateCharactersForPlayer(this.playerData);
            }
         }
         else
         {
            MessageBox.Show("You do not have permission to save characters to the account.", "Security Failed");
         }
      }

      /// <summary>
      /// Update the levels of all the characters in the list.
      /// </summary>
      public void UpdateCharacterLevels(List<Character> characters)
      {
         if(Thread.CurrentPrincipal.IsInRole("admin") || Thread.CurrentPrincipal.IsInRole("user"))
         {
            this.client.UpdateCharacterLevels(characters);
            this.playerData.Characters = this.client.RetrieveAccountCharacters(playerData.AccountName);
         }
         else
         {
            MessageBox.Show("You do not have permission to update character levels.", "Security Failed");
         }
      }

      /// <summary>
      /// Attempt to 'Login' a player account, and retrieve their playerData from the WowServiceHost.
      /// </summary>
      /// <param name="accountName">Account name we are logging in.</param>
      /// <param name="password">Password for the account we are logging in.</param>
      public void Login(string accountName, string password)
      {
         bool isAdmin;
         this.playerData = client.Login(out isAdmin, accountName, password);

         if(this.playerData != null && !string.IsNullOrEmpty(this.playerData.AccountName))
         {
            string[] roles = (isAdmin) ? new string[] { "admin", "user" } : new string[] { "user" }; //currentPrincipalRoles[0] : currentPrincipalRoles[1];
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(accountName), roles);

            if(this.playerData.Characters.Count == 0)
            {
               this.ShowCharacterCreation();
            }
            else
            {
               this.ShowCharacterSelect();
            }
         }
         else
         {
            MessageBox.Show("Login attempt failed, please try again.", "Login Failed");
         }
      }

      /// <summary>
      /// 'Logout' the currently active player account and redisplay the login interface.
      /// </summary>
      public void Logout()
      {
         Thread.CurrentPrincipal = null;
         this.playerData = null;

         this.formLogin = new FormLogin(this);
         this.formLogin.Show();
      }

      /// <summary>
      /// Display the Character Creation screen if the current account can create more characters.
      /// </summary>
      /// <returns>
      /// True:  If the logged in account can still create characters.
      /// False: If the logged in account has reached the maximum number of characters, or noone is logged in.
      /// </returns>
      public bool ShowCharacterCreation()
      {
         bool retVal = false;

         if(Thread.CurrentPrincipal.IsInRole("admin") || Thread.CurrentPrincipal.IsInRole("user"))
         {
            retVal = playerData.CanAddCharacter();

            if(retVal)
            {
               FormCharCreation dialog = new FormCharCreation(this, this.playerData.GetDefaultNewCharacter(), this.playerData.GetActiveFaction(), this.playerData.CanCreateDeathKnights());
               dialog.Show();
               this.formLogin.Close();
            }
         }

         return retVal;
      }

      /// <summary>
      /// Show the administration options dialog.
      /// </summary>
      public void ShowAdminOptions()
      {
         if(Thread.CurrentPrincipal.IsInRole("admin"))
         {
            this.SaveAccountsCharacterData(); // Save any newly created characters to the database before displaying the admin options

            FormAdminOptions dialog = new FormAdminOptions(this, this.GetAllCharacters());
            dialog.ShowDialog();
         }
      }

      /// <summary>
      /// Display the Account Creation interface.
      /// </summary>
      public void ShowCreateAccount()
      {
         FormCreateAccount dialog = new FormCreateAccount(this);
         dialog.ShowDialog();
      }

      /// <summary>
      /// Display the Character Select interface.
      /// Closes the login interface if it is still active.
      /// </summary>
      public void ShowCharacterSelect()
      {
         if(Thread.CurrentPrincipal.IsInRole("admin") || Thread.CurrentPrincipal.IsInRole("user"))
         {
            FormCharacterSelect dialog = new FormCharacterSelect(this);
            dialog.Show();

            if(this.formLogin != null)
            {
               this.formLogin.Close();
            }
         }
      }

      /// <summary>
      /// Get the characters of the logged in account.
      /// </summary>
      /// <returns>
      /// The list of characters for the active account.
      /// If noone is logged in, returns an empty character list.
      /// </returns>
      public List<Character> GetAccountCharacters()
      {
         List<Character> retVal = new List<Character>();

         if(Thread.CurrentPrincipal.IsInRole("admin") || Thread.CurrentPrincipal.IsInRole("user"))
         {
            if(this.playerData != null)
            {
               retVal = this.playerData.Characters;
            }
         }
         else
         {
            MessageBox.Show("You do not have permission to get the accounts characters.", "Security Failed");
         }

         return retVal;
      }

      /// <summary>
      /// Get all the characters stored in the database.
      /// </summary>
      /// <returns>
      /// All the characters saved to the database.
      /// </returns>
      public List<Character> GetAllCharacters()
      {
         List<Character> retVal = new List<Character>();
         
         if(this.playerData != null)
         {
            if(Thread.CurrentPrincipal.IsInRole("admin"))
            {
               retVal = this.client.RetrieveAllCharacters();
            }
            else
            {
               MessageBox.Show("You do not have permission to access character data.", "Security Failed");
            }
         }

         return retVal;
      }

      /// <summary>
      /// Attempt to add a character to the account.
      /// </summary>
      /// <param name="character">Character to add.</param>
      /// <returns>The <paramref name="character"/> was successfully added to the account.</returns>
      public bool AddCharacter(Character character)
      {
         bool retVal = false;
         if(Thread.CurrentPrincipal.IsInRole("admin") || Thread.CurrentPrincipal.IsInRole("user"))
         {
            retVal = this.client.AddCharacterToAccount(this.playerData, character);

            if(retVal)
            {
               this.playerData.AddCharacter(character);
            }
            else
            {
               MessageBox.Show("A character with that name already exists on this server.\n\nPlease try a different name.", "Character Name In Use");
            }
         }
         else
         {
            MessageBox.Show("You do not have permission to add characters to the account.", "Security Failed");
         }

         return retVal;
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
         bool retVal = false;
         if(Thread.CurrentPrincipal.IsInRole("admin") || Thread.CurrentPrincipal.IsInRole("user"))
         {
            retVal = this.playerData.CanAddCharacter();
         }
         else
         {
            MessageBox.Show("You do not have permission to add characters to the account.", "Security Failed");
         }

         return retVal;
      }

      /// <summary>
      /// Attempt to reactivate a character on the account.
      /// </summary>
      /// <param name="character">Character to reactivate.</param>
      public void ReactivateCharacter(Character character)
      {
         if(Thread.CurrentPrincipal.IsInRole("admin") || Thread.CurrentPrincipal.IsInRole("user"))
         {
            if(!playerData.ReactivateCharacter(character))
            {
               MessageBox.Show("Could not reactivate this character, as it is not a " + playerData.GetActiveFaction() + " character.", "Character Activation Failed");
            }
         }
         else
         {
            MessageBox.Show("You do not have permission to reactivate characters on the account.", "Security Failed");
         }
      }

      /// <summary>
      /// Attempt to deactivate a character from the account.
      /// </summary>
      /// <param name="character">Character to delete.</param>
      public void DeactivateCharacter(Character character)
      {
         if(Thread.CurrentPrincipal.IsInRole("admin") || Thread.CurrentPrincipal.IsInRole("user"))
         {
            if(this.playerData != null)
            {
               this.playerData.DeactivateCharacter(character);
            }
         }
         else
         {
            MessageBox.Show("You do not have permission to deactivate characters on the account.", "Security Failed");
         }
      }

      /// <summary>
      /// Attempt to delete a character from the account.
      /// </summary>
      /// <param name="character">Character to delete.</param>
      public void DeleteCharacter(Character character)
      {
         if(Thread.CurrentPrincipal.IsInRole("admin") || Thread.CurrentPrincipal.IsInRole("user"))
         {
            if(this.playerData != null)
            {
               this.playerData.DeleteCharacter(character);
               this.client.DeleteCharacterFromAccount(this.playerData, character);
            }
         }
         else
         {
            MessageBox.Show("You do not have permission to deactivate characters on the account.", "Security Failed");
         }
      }

      /// <summary>
      /// Run the Window base startup event, and show our initial interface.
      /// </summary>
      /// <param name="e"></param>
      protected override void OnStartup(StartupEventArgs e)
      {
         base.OnStartup(e);

         if(this.formLogin != null)
         {
            this.formLogin.Show();
         }
      }

   }

   /// <summary>
   /// Since we do not store the password in the PlayerData, when creating a new account,
   /// temporarily create this object to perform data validation on the accountName and password.
   /// </summary>
   class NewAccountVars
   {
      private string accountName, password;
      private bool isAdmin;

      [Required(ErrorMessage = "Account name is required.")]
      [StringLength(30, MinimumLength = 5, ErrorMessage = "Account name must be 5-30 characters long.")]
      [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Special characters  and spaces are not allowed in the account name.")]
      public string AccountName
      {
         get { return accountName; }
         set
         {
            Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "AccountName" });
            accountName = value;
         }
      }

      [Required(ErrorMessage = "A password is required.")]
      [StringLength(30, MinimumLength = 5, ErrorMessage = "Password must be 5-30 characters long.")]
      [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Special characters and spaces are not allowed in your password.")]
      public string Password
      {
         get { return password; }
         set
         {
            Validator.ValidateProperty(value, new ValidationContext(this, null, null) { MemberName = "Password" }); 
            password = value;
         }
      }

      public bool IsAdmin
      { get { return isAdmin; } set { isAdmin = value; } }
   }
}
