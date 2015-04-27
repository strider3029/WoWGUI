Brad Gray
---------------------------------------------------------
C# code sample


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