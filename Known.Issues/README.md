# Known.Issues

TerminalManagerFixer.cs

Usage
-----
After calling:
	var sesiuneCard = ManagerSesiuniCard.StartSesiuneNoua( this.terminalName );
	
You should call:
	TerminalManagerFixer.FixUmOfflineStates( sesiuneCard );

