using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WowClient
{
   /// <summary>
   /// Interaction logic for FormLogin.xaml
   /// </summary>
   public partial class FormLogin : Window
   {
      // View controller interface
      IWowClientController wowClientController;
      List<string> regionStrings = new List<string>();

      public FormLogin(IWowClientController wowClientController)
      {
         InitializeComponent();

         this.SetLocalizedStrings();
         this.wowClientController = wowClientController;
         this.regionStrings = wowClientController.GetRegionList();

         // Databind the language picker, and select the active culture
         if(0 < this.regionStrings.Count)
         {
            this.cbLanguagePicker.ItemsSource = this.regionStrings;

            string currentCulture = Thread.CurrentThread.CurrentCulture.ToString();
            
            if(this.cbLanguagePicker.Items.Contains(currentCulture))
            {
               this.cbLanguagePicker.SelectedItem = currentCulture;
            }
         }

         this.tbAccountName.Focus();
      }

      /// <summary>
      /// Set the localized strings.
      /// </summary>
      private void SetLocalizedStrings()
      {
         try
         {
            ResourceManager resourceManager = new ResourceManager("WowClient.LocalizationResources", Assembly.GetExecutingAssembly());

            btnLogin.Content = resourceManager.GetString("BtnLoginText");
            btnCreateAccount.Content = resourceManager.GetString("BtnCreateAccountText");
            lblAccountName.Content = resourceManager.GetString("LblBattleNetAccountText");
            lblPassword.Content = resourceManager.GetString("LblPasswordText");
         }
         catch(ArgumentNullException)
         { /* Do error handling/reporting */  }
         catch(MissingManifestResourceException)
         { /* Do error handling/reporting */ }
      }

      /// <summary>
      /// Attempt to login the character.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnLogin_Click(object sender, RoutedEventArgs e)
      {
         wowClientController.Login(tbAccountName.Text, pwbPassword.Password);
      }

      /// <summary>
      /// Launch the interface to create a new account.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnCreateAccount_Click(object sender, RoutedEventArgs e)
      {
         this.wowClientController.ShowCreateAccount();
      }

      /// <summary>
      /// Select a different language localization option.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void cbLanguagePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         wowClientController.SetCultureInfo(this.cbLanguagePicker.SelectedItem.ToString());
         this.SetLocalizedStrings();
      }

      /// <summary>
      /// Change the text colour on the language picker.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void cbLanguagePicker_DropDownOpened(object sender, System.EventArgs e)
      {
         this.cbLanguagePicker.Foreground = Brushes.Black;
      }

      /// <summary>
      /// Change the text colour on the language picker.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void cbLanguagePicker_DropDownClosed(object sender, System.EventArgs e)
      {
         this.cbLanguagePicker.Foreground = Brushes.Gold;
      }
   }
}
