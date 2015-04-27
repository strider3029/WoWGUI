using System;
using System.Windows;
using System.Reflection;
using System.Resources;

namespace WowClient
{
   /// <summary>
   /// Interaction logic for FormCreateAccount.xaml
   /// </summary>
   public partial class FormCreateAccount : Window
   {
      // View controller interface
      IWowClientController wowClientController;

      /// <summary>
      /// Interface for creating new accounts.
      /// </summary>
      /// <param name="wowClientController">Client controller interface.</param>
      public FormCreateAccount(IWowClientController wowClientController)
      {
         InitializeComponent();

         this.SetLocalizedStrings();
         this.wowClientController = wowClientController;
      }

      /// <summary>
      /// Set the localized strings.
      /// </summary>
      private void SetLocalizedStrings()
      {
         try
         {
            ResourceManager resourceManager = new ResourceManager("WowClient.LocalizationResources", Assembly.GetExecutingAssembly());
            
            btnCreateAccount.Content = resourceManager.GetString("BtnCreateAccountText");
            lblNewAccountName.Content = resourceManager.GetString("LblNewAccountNameText");
            chkIsAdmin.Content = resourceManager.GetString("ChkAccountIsAdmin");
            lblPassword.Content = resourceManager.GetString("LblPasswordText");
            btnCancel.Content = resourceManager.GetString("BtnCancel");
         }
         catch(ArgumentNullException)
         { /* Do error handling/reporting */  }
         catch(MissingManifestResourceException)
         { /* Do error handling/reporting */ }
      }

      /// <summary>
      /// Attempt to create a new account. If successful close this dialog.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnCreateAccount_Click(object sender, RoutedEventArgs e)
      {
         if(this.wowClientController.CreateAccount(tbAccountName.Text, pwbPassword.Password, (bool)chkIsAdmin.IsChecked))
         {
            this.Close();
         }
      }

      /// <summary>
      /// Cancel account creation.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnCancel_Click(object sender, RoutedEventArgs e)
      {
         this.Close();
      }
   }
}
