C# code sample

Author:  Bradley Gray
Email:   strider_3029@yahoo.com

This was a test application developed to allow the management and creation of World of WarCraft accounts, and the player characters on the account. 
When the application starts, a user is able to login and take on the role of a new player, able to add and remove characters from their 'account'.
The characters are persisted through application restarts via the use of an SQL database.  An administrative view is also part of the program, 
allowing admins to edit certain properties of the player's characters (note - this is not linked to whether the user is a Windows admin).

The test included a list of technical requirements, but I have chosen to leave these out as I am not the author of the requirements and do not
wish to include any information that could be considered confidential or sensitive.

WoWGUI Solution is the main test solution.

TestDBCleanup Solution is a simple command line program that deletes the database created by my solution.

The test took me approximately 40 hours to complete.

A few notes on the solution:
- For the solution to work, visual studio needs to be run as an administrator in order for the WCF end points to be created (under normal windows security settings)
- It will need SQL Express 2008 installed, and the user must be logged in on an account with SQL permissions to create and modify the database/tables
- The WCF endpoints are using port:8000 which should be safe from conflicts, I do not know of any common programs that use this port
- The SQL interface for account name and password is not case sensitive to make testing easier
- Modifying the character levels using admin tools is built into the datagrid control in the admin options dialog, by double clicking on the character level values

One last note, I don't speak, read or write Spanish, so the localization strings for es-MX could be terrible.