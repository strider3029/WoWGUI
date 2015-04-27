using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;

namespace WowClient
{
   /// <summary>
   /// Interaction logic for FormCharCreation.xaml
   /// </summary>
   public partial class FormCharCreation : Window
   {
      static ResourceManager resourceManager = new ResourceManager("WowClient.LocalizationResources", Assembly.GetExecutingAssembly());

      Faction activeFaction;
      Character newCharacter;
      bool deathKnightEligible;

      // View controller interface
      IWowClientController wowClientController;

      // Button data for race/class buttons (tooltips, normal/highlight/disabled images)
      Dictionary<Race, ButtonData> raceButtonData = new Dictionary<Race, ButtonData>(6);
      Dictionary<CharClass, ButtonData> classButtonData = new Dictionary<CharClass, ButtonData>(6);

      /// <summary>
      /// Character creation interface
      /// </summary>
      /// <param name="wowClientController">Contoller interface</param>
      /// <param name="defaultCharacter">Default character</param>
      /// <param name="accountActiveFaction">The current active faction for the account (Alliance/Horde/Both)</param>
      /// <param name="deathKnightEligible">Can create a Death Knight character</param>
      public FormCharCreation(IWowClientController wowClientController, Character defaultCharacter, Faction accountActiveFaction, bool deathKnightEligible)
      {
         InitializeComponent();
         InitializeButtonData();

         this.newCharacter = defaultCharacter;
         this.activeFaction = accountActiveFaction;
         this.deathKnightEligible = deathKnightEligible;
         this.wowClientController = wowClientController;

         this.tbCharName.Focus();

         this.SetLocalizedStrings();
         this.UpdateControls();
      }

      /// <summary>
      /// Set the localized strings.
      /// </summary>
      private void SetLocalizedStrings()
      {
         try
         {
            lblName.Content = resourceManager.GetString("LblName");

            btnAccept.Content = resourceManager.GetString("BtnAccept");
            btnCancel.Content = resourceManager.GetString("BtnCancel");
         }
         catch(ArgumentNullException)
         { /* Do error handling/reporting */  }
         catch(MissingManifestResourceException)
         { /* Do error handling/reporting */ }
      }

      /// <summary>
      /// Update the UI controls based on the current data
      /// </summary>
      private void UpdateControls()
      {
         btnRaceHuman.Tag = Race.Human;
         btnRaceWorgen.Tag = Race.Worgen;
         btnRaceGnome.Tag = Race.Gnome;
         
         btnRaceOrc.Tag = Race.Orc;
         btnRaceTauren.Tag = Race.Tauren;
         btnRaceBloodElf.Tag = Race.BloodElf;

         btnClassWarrior.Tag = CharClass.Warrior;
         btnClassMage.Tag = CharClass.Mage;
         btnClassDruid.Tag = CharClass.Druid;
         btnClassDeathKnight.Tag = CharClass.DeathKnight;

         // Set the button images and tooltips for Alliance themed controls
         if(activeFaction == Faction.Alliance || activeFaction == Faction.Both)
         {
            btnRaceHuman.Background = (this.newCharacter.Race == Race.Human) ? raceButtonData[Race.Human].Highlight : raceButtonData[Race.Human].Normal;
            btnRaceGnome.Background = (this.newCharacter.Race == Race.Gnome) ? raceButtonData[Race.Gnome].Highlight : raceButtonData[Race.Gnome].Normal;
            btnRaceWorgen.Background = (this.newCharacter.Race == Race.Worgen) ? raceButtonData[Race.Worgen].Highlight : raceButtonData[Race.Worgen].Normal;

            btnRaceHuman.ToolTip = raceButtonData[Race.Human].ToolTip;
            btnRaceGnome.ToolTip = raceButtonData[Race.Gnome].ToolTip;
            btnRaceWorgen.ToolTip = raceButtonData[Race.Worgen].ToolTip;
         }
         else
         {
            btnRaceHuman.Background = raceButtonData[Race.Human].Disabled;
            btnRaceGnome.Background = raceButtonData[Race.Gnome].Disabled;
            btnRaceWorgen.Background = raceButtonData[Race.Worgen].Disabled;

            btnRaceHuman.ToolTip = raceButtonData[Race.Human].DisabledToolTip;
            btnRaceGnome.ToolTip = raceButtonData[Race.Gnome].DisabledToolTip;
            btnRaceWorgen.ToolTip = raceButtonData[Race.Worgen].DisabledToolTip;
         }

         // Set the button images and tooltips for Horde themed controls
         if(activeFaction == Faction.Horde || activeFaction == Faction.Both)
         {
            btnRaceOrc.Background = (this.newCharacter.Race == Race.Orc) ? raceButtonData[Race.Orc].Highlight : raceButtonData[Race.Orc].Normal;
            btnRaceTauren.Background = (this.newCharacter.Race == Race.Tauren) ? raceButtonData[Race.Tauren].Highlight : raceButtonData[Race.Tauren].Normal;
            btnRaceBloodElf.Background = (this.newCharacter.Race == Race.BloodElf) ? raceButtonData[Race.BloodElf].Highlight : raceButtonData[Race.BloodElf].Normal;

            btnRaceOrc.ToolTip = raceButtonData[Race.Orc].ToolTip;
            btnRaceTauren.ToolTip = raceButtonData[Race.Tauren].ToolTip;
            btnRaceBloodElf.ToolTip = raceButtonData[Race.BloodElf].ToolTip;
         }
         else
         {
            btnRaceOrc.Background = raceButtonData[Race.Orc].Disabled;
            btnRaceTauren.Background = raceButtonData[Race.Tauren].Disabled;
            btnRaceBloodElf.Background = raceButtonData[Race.BloodElf].Disabled;

            btnRaceOrc.ToolTip = raceButtonData[Race.Orc].DisabledToolTip;
            btnRaceTauren.ToolTip = raceButtonData[Race.Tauren].DisabledToolTip;
            btnRaceBloodElf.ToolTip = raceButtonData[Race.BloodElf].DisabledToolTip;
         }

         bool legalRaceClassCombo = false;

         // Set the image and tooltip for the Warrior button
         if(this.newCharacter.CharClass == CharClass.Warrior)
         {
            btnClassWarrior.Background = classButtonData[CharClass.Warrior].Highlight;
            btnClassWarrior.ToolTip = classButtonData[CharClass.Warrior].ToolTip;
         }
         else
         {
            legalRaceClassCombo = Character.IsRaceNClassLegal(this.newCharacter.Race, CharClass.Warrior);

            btnClassWarrior.Background = (legalRaceClassCombo) ? classButtonData[CharClass.Warrior].Normal : classButtonData[CharClass.Warrior].Disabled;
            btnClassWarrior.ToolTip = (legalRaceClassCombo) ? classButtonData[CharClass.Warrior].ToolTip : classButtonData[CharClass.Warrior].DisabledToolTip;
         }

         // Set the image and tooltip for the Mage button
         if(this.newCharacter.CharClass == CharClass.Mage)
         {
            btnClassMage.Background = classButtonData[CharClass.Mage].Highlight;
            btnClassMage.ToolTip = classButtonData[CharClass.Mage].ToolTip;
         }
         else
         {
            legalRaceClassCombo = Character.IsRaceNClassLegal(this.newCharacter.Race, CharClass.Mage);

            btnClassMage.Background = (legalRaceClassCombo) ? classButtonData[CharClass.Mage].Normal : classButtonData[CharClass.Mage].Disabled;
            btnClassMage.ToolTip = (legalRaceClassCombo) ? classButtonData[CharClass.Mage].ToolTip : classButtonData[CharClass.Mage].DisabledToolTip;
         }

         // Set the image and tooltip for the Druid button
         if(this.newCharacter.CharClass == CharClass.Druid)
         {
            btnClassDruid.Background = classButtonData[CharClass.Druid].Highlight;
            btnClassDruid.ToolTip = classButtonData[CharClass.Druid].ToolTip;
         }
         else
         {
            legalRaceClassCombo = Character.IsRaceNClassLegal(this.newCharacter.Race, CharClass.Druid);

            btnClassDruid.Background = (legalRaceClassCombo) ? classButtonData[CharClass.Druid].Normal : classButtonData[CharClass.Druid].Disabled;
            btnClassDruid.ToolTip = (legalRaceClassCombo) ? classButtonData[CharClass.Druid].ToolTip : classButtonData[CharClass.Druid].DisabledToolTip;
         }

         // Set the image and tooltip for the DeathKnight button
         if(this.newCharacter.CharClass == CharClass.DeathKnight)
         {
            btnClassDeathKnight.Background = classButtonData[CharClass.DeathKnight].Highlight;
            btnClassDeathKnight.ToolTip = classButtonData[CharClass.DeathKnight].ToolTip;
         }
         else
         {
            legalRaceClassCombo = (deathKnightEligible && Character.IsRaceNClassLegal(this.newCharacter.Race, CharClass.DeathKnight));

            btnClassDeathKnight.Background = (legalRaceClassCombo) ? classButtonData[CharClass.DeathKnight].Normal : classButtonData[CharClass.DeathKnight].Disabled;
            btnClassDeathKnight.ToolTip = (legalRaceClassCombo) ? classButtonData[CharClass.DeathKnight].ToolTip : classButtonData[CharClass.DeathKnight].DisabledToolTip;
         }
      }

      /// <summary>
      /// Populate the ButtonData dictionary and structs
      /// </summary>
      private void InitializeButtonData()
      {
         ButtonData btnDataHuman = new ButtonData();
         btnDataHuman.ToolTip = resourceManager.GetString("ToolTipHumanEnabled");
         btnDataHuman.DisabledToolTip = resourceManager.GetString("ToolTipAllianceDisabled");
         btnDataHuman.Normal = CreateWPFResourceImage("Race_Human.png");
         btnDataHuman.Disabled = CreateWPFResourceImage("Race_Human_Disabled.png");
         btnDataHuman.Highlight = CreateWPFResourceImage("Race_Human_Highlight.png");

         raceButtonData.Add(Race.Human, btnDataHuman);

         ButtonData btnDataGnome = new ButtonData();
         btnDataGnome.ToolTip = resourceManager.GetString("ToolTipGnomeEnabled");
         btnDataGnome.DisabledToolTip = resourceManager.GetString("ToolTipAllianceDisabled");
         btnDataGnome.Normal = CreateWPFResourceImage("Race_Gnome.png");
         btnDataGnome.Disabled = CreateWPFResourceImage("Race_Gnome_Disabled.png");
         btnDataGnome.Highlight = CreateWPFResourceImage("Race_Gnome_Highlight.png");

         raceButtonData.Add(Race.Gnome, btnDataGnome);

         ButtonData btnDataWorgen = new ButtonData();
         btnDataWorgen.ToolTip = resourceManager.GetString("ToolTipWorgenEnabled");
         btnDataWorgen.DisabledToolTip = resourceManager.GetString("ToolTipAllianceDisabled");
         btnDataWorgen.Normal = CreateWPFResourceImage("Race_Worgen.png");
         btnDataWorgen.Disabled = CreateWPFResourceImage("Race_Worgen_Disabled.png");
         btnDataWorgen.Highlight = CreateWPFResourceImage("Race_Worgen_Highlight.png");

         raceButtonData.Add(Race.Worgen, btnDataWorgen);

         ButtonData btnDataOrc = new ButtonData();
         btnDataOrc.ToolTip = resourceManager.GetString("ToolTipOrcEnabled");
         btnDataOrc.DisabledToolTip = resourceManager.GetString("ToolTipHordeDisabled");
         btnDataOrc.Normal = CreateWPFResourceImage("Race_Orc.png");
         btnDataOrc.Disabled = CreateWPFResourceImage("Race_Orc_Disabled.png");
         btnDataOrc.Highlight = CreateWPFResourceImage("Race_Orc_Highlight.png");

         raceButtonData.Add(Race.Orc, btnDataOrc);

         ButtonData btnDataTauren = new ButtonData();
         btnDataTauren.ToolTip = resourceManager.GetString("ToolTipTaurenEnabled");
         btnDataTauren.DisabledToolTip = resourceManager.GetString("ToolTipHordeDisabled");
         btnDataTauren.Normal = CreateWPFResourceImage("Race_Tauren.png");
         btnDataTauren.Disabled = CreateWPFResourceImage("Race_Tauren_Disabled.png");
         btnDataTauren.Highlight = CreateWPFResourceImage("Race_Tauren_Highlight.png");

         raceButtonData.Add(Race.Tauren, btnDataTauren);

         ButtonData btnDataBloodElf = new ButtonData();
         btnDataBloodElf.ToolTip = resourceManager.GetString("ToolTipBloodElfEnabled");
         btnDataBloodElf.DisabledToolTip = resourceManager.GetString("ToolTipHordeDisabled");
         btnDataBloodElf.Normal = CreateWPFResourceImage("Race_BloodElf.png");
         btnDataBloodElf.Disabled = CreateWPFResourceImage("Race_BloodElf_Disabled.png");
         btnDataBloodElf.Highlight = CreateWPFResourceImage("Race_BloodElf_Highlight.png");

         raceButtonData.Add(Race.BloodElf, btnDataBloodElf);

         ButtonData btnDataWarrior = new ButtonData();
         btnDataWarrior.ToolTip = resourceManager.GetString("ToolTipWarriorEnabled");
         btnDataWarrior.DisabledToolTip = resourceManager.GetString("ToolTipWarriorDisabled");
         btnDataWarrior.Normal = CreateWPFResourceImage("Class_Warrior.png");
         btnDataWarrior.Disabled = CreateWPFResourceImage("Class_Warrior_Disabled.png");
         btnDataWarrior.Highlight = CreateWPFResourceImage("Class_Warrior_Highlight.png");

         classButtonData.Add(CharClass.Warrior, btnDataWarrior);

         ButtonData btnDataMage = new ButtonData();
         btnDataMage.ToolTip = resourceManager.GetString("ToolTipMageEnabled");
         btnDataMage.DisabledToolTip = resourceManager.GetString("ToolTipMageDisabled");
         btnDataMage.Normal = CreateWPFResourceImage("Class_Mage.png");
         btnDataMage.Disabled = CreateWPFResourceImage("Class_Mage_Disabled.png");
         btnDataMage.Highlight = CreateWPFResourceImage("Class_Mage_Highlight.png");

         classButtonData.Add(CharClass.Mage, btnDataMage);

         ButtonData btnDataDruid = new ButtonData();
         btnDataDruid.ToolTip = resourceManager.GetString("ToolTipDruidEnabled");
         btnDataDruid.DisabledToolTip = resourceManager.GetString("ToolTipDruidDisabled");
         btnDataDruid.Normal = CreateWPFResourceImage("Class_Druid.png");
         btnDataDruid.Disabled = CreateWPFResourceImage("Class_Druid_Disabled.png");
         btnDataDruid.Highlight = CreateWPFResourceImage("Class_Druid_Highlight.png");

         classButtonData.Add(CharClass.Druid, btnDataDruid);

         ButtonData btnDataDeathKnight = new ButtonData();
         btnDataDeathKnight.ToolTip = resourceManager.GetString("ToolTipDeathKnightEnabled");
         btnDataDeathKnight.DisabledToolTip = resourceManager.GetString("ToolTipDeathKnightDisabled");
         btnDataDeathKnight.Normal = CreateWPFResourceImage("Class_DeathKnight.png");
         btnDataDeathKnight.Disabled = CreateWPFResourceImage("Class_DeathKnight_Disabled.png");
         btnDataDeathKnight.Highlight = CreateWPFResourceImage("Class_DeathKnight_Highlight.png");

         classButtonData.Add(CharClass.DeathKnight, btnDataDeathKnight);
      }

      /// <summary>
      /// Create an ImageBrush from an image file in the projects Resources folder.
      /// </summary>
      /// <param name="fileName"></param>
      /// <returns></returns>
      private static ImageBrush CreateWPFResourceImage(string fileName)
      {
         BitmapImage bi = new BitmapImage();
         bi.BeginInit();

         try
         {
             bi.UriSource = new Uri(string.Format("pack://application:,,,/WowClient;component/resources/{0}", fileName), UriKind.Absolute);
         }
         catch(ArgumentException)
         { /* Do error handling here */ }
         catch(UriFormatException)
         { /* Do error handling here */ }

         bi.EndInit();
         ImageBrush brush = new ImageBrush();
         brush.ImageSource = bi;

         return brush;
      }

      /// <summary>
      /// Attempt to add the character to the current account, and if successful finalise the characters construction.
      /// </summary>
      private void FinaliseAndAddCharacter()
      {
         try
         {
            this.newCharacter.Name = tbCharName.Text;
            this.newCharacter.CompleteNewCharacterConstruction();

            // Tell the controller to add a character to the account, and finalize its construction
            if(this.wowClientController.AddCharacter(this.newCharacter))
            {
               this.wowClientController.ShowCharacterSelect();
               this.Close();
            }
         }
         catch(System.ComponentModel.DataAnnotations.ValidationException ex)
         {
            string errorMsg = string.Format("Could not create the new character.\n\n{0}", ex.ValidationResult.ErrorMessage);

            MessageBox.Show(errorMsg, "Character Creation Failed");
         }
      }

      /// <summary>
      /// Cancel creating a new character.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnCancel_Click(object sender, RoutedEventArgs e)
      {
         this.wowClientController.ShowCharacterSelect();
         this.Close();
      }

      /// <summary>
      /// Change race (if it is valid for your active faction)
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnRace_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            Race newCharRace = (Race)((System.Windows.Controls.Button)sender).Tag;

            if(Character.IsRaceInFaction(activeFaction, newCharRace))
            {
               this.newCharacter.SetRace(newCharRace);
            }
         }
         catch(InvalidCastException)
         { /* Do error handling here */ }

         this.UpdateControls();
      }

      /// <summary>
      /// Change class (if the class is valid for the race)
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnClass_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            CharClass newCharClass = (CharClass)((System.Windows.Controls.Button)sender).Tag;

            if(newCharClass != CharClass.DeathKnight || deathKnightEligible)
            {
               this.newCharacter.SetCharClass(newCharClass);
            }
         }
         catch(InvalidCastException)
         { /* Do error handling here */ }

         this.UpdateControls();
      }

      /// <summary>
      /// Accept/Add the character that has been created.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnAccept_Click(object sender, RoutedEventArgs e)
      {
         this.FinaliseAndAddCharacter();
      }
   }

   /// <summary>
   /// Data structure to hold information for a button with a graphic.
   /// Tooltip/Disabled Tooltip, and ImageBrush for Normal/Highlighted/Disabled button backgrounds.
   /// </summary>
   struct ButtonData
   {
      public string ToolTip, DisabledToolTip;
      public ImageBrush Normal, Highlight, Disabled;
   }
}