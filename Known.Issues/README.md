# Known.Issues

TerminalManagerFixer Usage
--------------------------
After calling:
	var sesiuneCard = ManagerSesiuniCard.StartSesiuneNoua();
	
You should call:
	TerminalManagerFixer.FixUmOfflineStates( sesiuneCard );

Notes (ro)
-----
Fixul nu este adresat lucrului offline, ci permite lucrul cu cardul cu verificarea PIN-ului doar în chipset-ul smartcard-ului, mascând eroarea de autentificare primită de la UM. Deci tratează strict cazul în care există conexiune cu UM, dar UM răspunde cu eroare -5.
Pentru ca fixul să funcţioneze este necesar ca aplicaţia să fie online şi să existe fişierul TDX de înrolare a terminalului.
