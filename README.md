This is a small shortcut key app that can detect which window is on top and show an appropriate html document inside.

There is only one shortcut key for the which is Windows-K, to show hide the main window.

For the app to work the following files must exists in c:\khebbiekeys
- khebbiekeys.xml which defines the binding between the title window and the html document with the shortcut keys.
- a number of html files refered to in khebbiekeys.xml.

The layout of khebbiekeys.xml should be as follows:
	<khebbiekey>
		<sk>
			<key>outlook</key>
			<value>
				c:\khebbiekey\test.html
			</value>
		</sk>
		
		<sk>
			<key>sql server</key>
			<value>
				c:\khebbiekey\sql.html
			</value>
		<key>visual studio</key>
			<value>
				c:\khebbiekey\devenv.html
			</value>

		</sk>
	</khebbiekey>

The key is any part of the string in the title window, and the value is a html file.