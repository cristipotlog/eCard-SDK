# Work.Offline

Notes (ro)
-----
Pentru a funcţiona OFFLINE este necesar să existe un fişier TDX corespunzător generat pentru CUI/CAS/Nr.Contract/DataContract în directorul de lucru al SDK-ului. Acest fişier poate fi obţinut doar online, dar se poate copia cu uşurinţă pe staţiile de lucru offline.

Mai există o mică particularitate a SDK-ului, care dacă primeşte un URL pentru conexiunea la UM încearcă să-l rezolve în DNS pentru a obţine IP-ul, şi fiind offline obţine o eroare. Problema poate fi ocolită prin transmiterea adresei de conexiune direct în format IP.

Se recomandă setarea adresei prin apelul metodei şi nu prim fişierul de configurare.

Exemplu:
```csharp
	Novensys.eCard.SDK.ManagerSesiuniCard.SetAdresaUnitateManagement( "213.177.18.123", 433 );
```

