1- Install Packages:

	A- Go to Extensions Tab:

		- Extensions → Manage Extensions

		- Search for the installer:

			- In the search box, type: Microsoft Visual Studio Installer Projects

		- Download and Install:

			- Click the Download button
			- Close Visual Studio when prompted to begin installation
			- Wait for the VSIX installer to complete

	B- Go to Tools Tab:
		- Go To NuGet Package Manager 
		- Go To Package Manager Console
		- In the console Write this -> Install-Package Microsoft.Web.WebView2

2- In the MainWindow.xaml.cs

	- replace the URL with the URL you want 

3- Make Desktop Shortcut:

	- Right-click the WebView project
	- Select "View" → "File System"
	- Right-click "Publish Items..." → "Create Shortcut"
	- Rename shortcut to your app name
	- Drag shortcut to "User's Desktop" folder

4- Make Start Menu Shortcut:

	- Right-click "User's Programs Menu"
	- Add new folder name it like you want
	- Drag another shortcut into this folder

5- Change Name Of App:

	- Right-click the WebView project
	- change the 'title', 'ProductName'