using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Common;

namespace WowClient
{
   /// <summary>
   /// Interaction logic for FormAccountOptions.xaml
   /// </summary>
   public partial class FormAdminOptions : Window
   {

      private static ResourceManager resourceManager = new ResourceManager("WowClient.LocalizationResources", Assembly.GetExecutingAssembly());

      // View controller interface
      private IWowClientController wowClientController;
      private List<Character> characters;

      /// <summary>
      /// Administration options interface.
      /// </summary>
      /// <param name="wowClientController">Contoller interface</param>
      /// <param name="characters">List of characters that can be edited.</param>
      public FormAdminOptions(IWowClientController wowClientController, List<Character> characters)
      {
         InitializeComponent();

         this.SetLocalizedStrings();

         this.characters = characters;
         this.wowClientController = wowClientController;

         this.dataGrid1.ItemsSource = this.characters;
         this.dataGrid1.RowValidationRules.Add(new LevelRule() { ValidationStep = ValidationStep.UpdatedValue });
      }

      /// <summary>
      /// Set the localization based strings.
      /// </summary>
      private void SetLocalizedStrings()
      {
         try
         {
            btnAccept.Content = resourceManager.GetString("BtnAccept");
            btnCancel.Content = resourceManager.GetString("BtnCancel");
         }
         catch(MissingManifestResourceException)
         { /* Do error handling/reporting */ }
      }

      /// <summary>
      /// When the datagrid has finished loading, set the columns that should not be editable to be read only.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void dataGrid1_Loaded(object sender, RoutedEventArgs e)
      {
         foreach(DataGridColumn column in this.dataGrid1.Columns)
         {
            if(string.Compare(column.Header.ToString(), "Level", true) == 0)
            {
               column.IsReadOnly = !(Thread.CurrentPrincipal.IsInRole("admin"));
            }
            else
            {
               column.IsReadOnly = true;
            }
         }
      }

      /// <summary>
      /// Get the datagrid row corresponding to the index.
      /// </summary>
      /// <param name="grid">The datagrid to retrieve the row from.</param>
      /// <param name="index">Index of the row.</param>
      /// <returns>
      /// The dataGrid row at the index.
      /// </returns>
      private static DataGridRow GetRow(DataGrid grid, int index)
      {
         return (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
      }

      /// <summary>
      /// Check that there are no errors in any of the rows, and then commit the changes and close the dialog.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnAccept_Click(object sender, RoutedEventArgs e)
      {
         bool hasErrors = false;

         for(int index = 0; index < this.dataGrid1.Items.Count; ++index)
         {
            DataGridRow row = GetRow(this.dataGrid1, index);
            if(row != null && Validation.GetHasError(row))
            {
               hasErrors = true;
               break;
            }
         }

         if(!hasErrors)
         {
            this.wowClientController.UpdateCharacterLevels(this.characters);
            this.Close();
         }
         else
         {
            MessageBox.Show("You must fix any errors before committing the character changes.", "Illegal Character Values Found");
         }
      }

      /// <summary>
      /// Custom validation rule to allow feedback to the admin when the character level restrictions are disobeyed.
      /// </summary>
      class LevelRule : ValidationRule
      {
         public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
         {
            int newLevel = 1;

            Character item;

            try
            {
               System.Windows.Data.BindingGroup bindingGroup = (System.Windows.Data.BindingGroup)value;

               // Get the source object.
               item = bindingGroup.Items[0] as Character;

               object intValue;

               // If accessing the binding doesn't succeed, the property was not found
               if(!bindingGroup.TryGetValue(item, "Level", out intValue))
               {
                  return new ValidationResult(false, "Level property was not found in editing object.");
               }

               newLevel = int.Parse(intValue.ToString());
            }
            catch(System.FormatException)
            {
               return new ValidationResult(false, "Value is not a valid level.");
            }

            if(item != null && item.CharClass == CharClass.DeathKnight && (newLevel < PlayerData.DEATHKNIGHT_LEVEL_REQUIREMENT || 85 < newLevel))
            {
               return new ValidationResult(false, "Death Knight Characters can only have a level range of 55-85.");
            }
            else if(newLevel < 1 || 85 < newLevel)
            {
               return new ValidationResult(false, "Characters can only have a level range of 1-85.");
            }
            else
            {
               return ValidationResult.ValidResult;
            }
         }
      }
   }
}
