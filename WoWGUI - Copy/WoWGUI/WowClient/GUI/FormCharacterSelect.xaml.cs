using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Common;

namespace WowClient
{
   /// <summary>
   /// Interaction logic for the Character Selection GUI
   /// </summary>
   public partial class FormCharacterSelect : Window
   {
      private static ResourceManager resourceManager = new ResourceManager("WowClient.LocalizationResources", Assembly.GetExecutingAssembly());

      // View controller interface
      private IWowClientController wowClientController;

      // Variables for the character select pane
      private Character selectedCharacter;
      private TextBlock[] characterBlocks = new TextBlock[10];
      private Brush highlightBrush = Brushes.Goldenrod;

      /// <summary>
      /// Character Selection GUI constructor
      /// </summary>
      /// <param name="wowClientController">Interface to the View controller.</param>
      public FormCharacterSelect(IWowClientController wowClientController)
      {
         InitializeComponent();

         this.wowClientController = wowClientController;

         this.characterBlocks[0] = taChar0;
         this.characterBlocks[1] = taChar1;
         this.characterBlocks[2] = taChar2;
         this.characterBlocks[3] = taChar3;
         this.characterBlocks[4] = taChar4;
         this.characterBlocks[5] = taChar5;
         this.characterBlocks[6] = taChar6;
         this.characterBlocks[7] = taChar7;
         this.characterBlocks[8] = taChar8;
         this.characterBlocks[9] = taChar9;

         this.SetLocalizedStrings();
         this.UpdateControls();
      }

      /// <summary>
      /// Populate the Character description/selection UI elements from a character list.
      /// </summary>
      /// <param name="characters">List of characters to use to populate the character description/selection UI elements.</param>
      void PopulateCharacterDisplayList(List<Character> characters)
      {
         int index;

         // Reset any existing data
         for(index = 0; index < this.characterBlocks.Length; ++index)
         {
            this.characterBlocks[index].Tag = null;
            this.characterBlocks[index].Text = "";
         }

         // Populate with the new data
         for(index = 0; index < characters.Count; ++index)
         {
            this.characterBlocks[index].Tag = characters[index];
            this.characterBlocks[index].Text = characters[index].ToString();
         }
      }

      /// <summary>
      /// Set the localized strings.
      /// </summary>
      private void SetLocalizedStrings()
      {
         try
         {
            btnAdminTools.Content = resourceManager.GetString("BtnAdminTools");
            btnCreateChar.Content = resourceManager.GetString("BtnCreateChar");
            btnReactivateChar.Content = resourceManager.GetString("BtnReactivateChar");
            btnLogout.Content = resourceManager.GetString("BtnLogout");
         }
         catch(MissingManifestResourceException)
         { /* Do error handling/reporting */ }
      }

      /// <summary>
      /// Update the controls for the GUI.
      /// </summary>
      void UpdateControls()
      {
         btnAdminTools.Visibility = (Thread.CurrentPrincipal.IsInRole("admin")) ? Visibility.Visible : Visibility.Hidden;

         string btnDeleteString = resourceManager.GetString("BtnDeactivateChar");
         btnDeleteString = (this.selectedCharacter != null && !this.selectedCharacter.IsActive) ? resourceManager.GetString("BtnDeleteChar") : btnDeleteString;
         btnDeleteChar.Content = btnDeleteString;

         btnCreateChar.IsEnabled = wowClientController.CanAddCharacter();
         btnReactivateChar.IsEnabled = (this.selectedCharacter != null && !this.selectedCharacter.IsActive);
         btnDeleteChar.IsEnabled = (this.selectedCharacter != null);

         this.PopulateCharacterDisplayList(wowClientController.GetAccountCharacters());
         this.UpdateCharacterHighlight();
      }

      /// <summary>
      /// Update the highlighted character control based on what character is currently selected.
      /// </summary>
      private void UpdateCharacterHighlight()
      {
         Character character;

         for(int index = 0; index < this.characterBlocks.Length; ++index)
         {
            character = this.characterBlocks[index].Tag as Character;

            // Set the text colour to reflect character.isActive
            if(character != null)
            {
               this.characterBlocks[index].Foreground = (character.IsActive) ? Brushes.White : Brushes.Gray;
            }

            // If this is the selected character, highlight it
            if(character != null && character == this.selectedCharacter)
            {
               this.characterBlocks[index].Background = highlightBrush;
            }
            else
            {
               this.characterBlocks[index].Background = Brushes.Transparent;
            }
         }
      }

      /// <summary>
      /// Delete/Deactivate the currently selected character.
      /// </summary>
      private void DeleteSelectedCharacter()
      {
         if(this.selectedCharacter != null)
         {
            // If the character is active, set it to be inactive
            if(this.selectedCharacter.IsActive)
            {
               this.wowClientController.DeactivateCharacter(selectedCharacter);
            }
            // If the character is inactive, delete it permanently
            else
            {
               MessageBoxResult result = MessageBox.Show("Are you sure you want to delete " + this.selectedCharacter.Name + " permanently?", "Delete Character", MessageBoxButton.YesNo);

               if(result == MessageBoxResult.Yes)
               {
                  this.wowClientController.DeleteCharacter(selectedCharacter);
                  this.selectedCharacter = null;
               }
            }

            this.UpdateControls();
         }
      }

      private void Logout()
      {
         this.wowClientController.SaveAccountsCharacterData();
         this.wowClientController.Logout();
      }

      /// <summary>
      /// Delete/Deactivate the currently selected character.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnDeleteChar_Click(object sender, RoutedEventArgs e)
      {
         this.DeleteSelectedCharacter();
      }

      /// <summary>
      /// Reactivate the currently selected character.
      /// </summary>
      private void ReactivateSelectedCharacter()
      {
         if(this.selectedCharacter != null)
         {
            this.wowClientController.ReactivateCharacter(selectedCharacter);

            this.UpdateControls();
         }
      }

      /// <summary>
      /// Reactivate a deactivated character.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnReactivateChar_Click(object sender, RoutedEventArgs e)
      {
         this.ReactivateSelectedCharacter();
      }

      /// <summary>
      /// Logout the current account using the controller interface, and close this interface.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnLogOut_Click(object sender, RoutedEventArgs e)
      {
         this.Logout();
         this.Close();
      }

      /// <summary>
      /// Tell the controller interface that we want to add a new character (ie. display the character creation interface).
      /// If successful, close this interface.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnCreateChar_Click(object sender, RoutedEventArgs e)
      {
         if(this.wowClientController.ShowCharacterCreation())
         {
            this.Close();
         }
      }

      /// <summary>
      /// Update the currently selected Character
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void taChar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
      {
         this.selectedCharacter = null;

         TextBlock textBlock = sender as TextBlock;

         if(textBlock != null)
         {
            this.selectedCharacter = textBlock.Tag as Character;
         }

         this.UpdateControls();
      }

      /// <summary>
      /// Launch the admin options dialog
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnAdminOptions_Click(object sender, RoutedEventArgs e)
      {
         this.wowClientController.ShowAdminOptions();
         this.UpdateControls();
      }
   }
}
