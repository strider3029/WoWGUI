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
using System.Linq;
using System.Text;
using System.ServiceModel;
using WowClient.WowServiceReference;
using System.Windows;
using Common;

namespace WowClient
{
   class WowClient
   {
      [STAThread]
      static void Main(string[] args)
      {
         string region = "";

         //
         // Process command line arguments
         //
         if(args.Length > 0)
         {
            foreach(string arg in args)
            {
               if((arg.ToLower() == "en-us") || (arg.ToLower() == "-en-us") || (arg.ToLower() == "/en-us"))
               {
                  region = "en-US";
               }
               else if((arg.ToLower() == "es-au") || (arg.ToLower() == "-es-au") || (arg.ToLower() == "/es-au"))
               {
                  region = "es-AU";
               }
               else if((arg.ToLower() == "es-mx") || (arg.ToLower() == "-es-mx") || (arg.ToLower() == "/es-mx"))
               {
                  region = "es-MX";
               }
               else if((arg.ToUpper() == "/H") || (arg.ToUpper() == "/HELP") || (arg.ToUpper() == "/?"))
               {
                  string mess = "To set a localization region for the program on startup, run with command line argument of [region code], /[region code] or -[region code].  Ie: en-us, /en-us or -en-us.";
                  MessageBox.Show(mess);
                  return;
               }
            }
         }

         WowClientController wowClientController = new WowClientController(region);
         wowClientController.Run();
      }
   }
}
