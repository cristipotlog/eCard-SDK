# Known.Issues

TerminalManagerFixer Usage
--------------------------
After calling:
```csharp
	var sesiuneCard = ManagerSesiuniCard.StartSesiuneNoua();
```
You should call:
```csharp
	TerminalManagerFixer.FixUmOfflineStates( sesiuneCard );
```

> Acest fix permite lucrul cu cardul prin verificarea PIN-ului doar în chipset-ul smartcard-ului, mascând eroarea de autentificare primită de la UM pentru a permite realizarea semnăturii digitale. Fixul tratează strict cazul în care există conexiune cu UM, dar UM răspunde cu eroare -5.


SmartCardStatesFixer Usage
--------------------------
After calling:
```csharp
	var response = sesiuneCard.ActiveazaCard( sesiuneCard.Token );
```
or after calling:
```csharp
	var response = sesiuneCard.CitesteDate( sesiuneCard.Token, ... );
```
You should call:
```csharp
	SmartCardStatesFixer.FixKnownCardIssues( sesiuneCard, ref response );
```

> Acest fix permite sincronizarea PIN-ului cu UM în cazul în care a fost modificat doar în chipset-ul smartcard-ului, mascând erorile de autentificare primite de la UM. Fixul tratează următoarele răspunsuri primite de la SDK: ERR_UM_STARE_CARD_INVALIDA, ERR_INVALID_CARD, ERR_INVALID_PIN, ERR_CARD_BLOCKED.

Notes (ro)
----------
Fixurile nu se adresează modului de lucru offline, ci permit lucrul cu cardul ocolind anumite situaţii netratate de Novensys.eCard.SDK.
Pentru ca fixurile să funcţioneze este necesar ca aplicaţia să fie online şi să existe fişierul TDX de înrolare a terminalului.