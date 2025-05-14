To use this project you need to have a database installed of your choice, the project is currently using postgre.
It is recommended to stay with postgre as the project uses PostGIS for coordinate conversions.


Steps to get the project up and running:
Install the required packages in your project if changing from postgre to any other database.
Copy and change the appsettings.Template.json to appsettings.Development.json or your deployment environment as required.
Change your connection string and Clientname in appsettings.Development.json to your database's connection sting.
  If switching to a different database then also update this in program.cs where it currently points to postgre.
After this is done, in Visual Studio open Tools -> NuGet Package Manager -> Package Manager Console 
In the console run the command: update-database
This will run all the migrations and create the tables.

Before running app for the first time:
In Visual Studio in Solution Explorer go to Services -> Identity and update the RoleSeeder.cs as required,
also update the UserSeeder with your own admin user or just check the current user for the login details.

Run app and login!

Additional Information:
The app uses a mail server in order to send invitations to new users, you will have to set this up with your own mail server in order for it to work.
App also uses pdftoppm to generate thumbnails for certain uploads, install this if you want that functionality.
