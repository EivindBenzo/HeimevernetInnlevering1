# Kriseberedskap – Heimevernet innlevering 1

Dette er gruppens ASP.NET Core MVC-prosjekt for å registrere kontaktinformasjon og plassere ressurser på et kart. Denne README-en beskriver det som faktisk ligger i `main` etter at arbeidet fra `oliver-branch` ble slått inn. Den er en beskrivelse av løsningen, ikke en gjengivelse av lærerens oppgavetekst: lenke eller ordlyd til lærerens repository var ikke tilgjengelig ved oppdateringen og er derfor ikke brukt som kilde til ekstra krav.

## Løsningen slik den er implementert

- **Skjema:** Fornavn, etternavn, e-post og beskrivelse valideres på serveren og lagres som `FormSubmission`. Etter vellykket innsending lagres innleveringens ID i brukerens ASP.NET-session.
- **Tilgangsflyt:** Forespørsler til `/Home/...` (unntatt skjema og feilside) omdirigeres til `/Home/Form` dersom session ikke har en registrerings-ID. Dette er en enkel per-nettleserøkt-sperre, **ikke** pålogging eller sikker tilgangskontroll. En ny/utløpt session må fylle ut skjemaet på nytt, selv om data allerede finnes i databasen. Rotruten `/` er ikke omfattet av denne sperren.
- **Kart og ressurser:** Leaflet med OpenStreetMap-fliser og søkeforslag fra Nominatim. Kartet starter ved Kristiansand (58.1467, 7.9956). Brukeren velger ressurstype (Kjøretøy, Maskin, Drone, Generator, Personell eller Utstyr), skriver ressursnavn og velger plassering ved bysøk eller klikk på kartet. `Send inn` lagrer ressursnavn, type, lokasjonsnavn og koordinater som `MapSubmission` og åpner kartet på nytt. Lagrede ressurser vises som markører med informasjonsbokser og som kort under kartet.
- **Data og Verifiser:** `/Home/Data` og `/Home/Verify` henter skjemainnleveringer og kartressurser fra databasen ved hver sidevisning. Verifiser viser i tillegg ID-er, antall rader og lagringstidspunkt. Sidene viser per nå alle registreringer, ikke bare dine egne.
- **Plassering og skjema:** Etter en kartinnsending lagres valgt plassering midlertidig i session og forhåndsutfylles ved **neste** besøk på Skjema. Dersom skjemaet deretter sendes inn, kan koordinatene lagres på den **nye** skjemainnleveringen. Ressursen knyttes **ikke automatisk til en tidligere** innsendt person/skjemarad; det finnes ingen fremmednøkkel mellom `FormSubmission` og `MapSubmission`.

Datamodellen bygger videre på EF Core-idéen i `Marius`-branchen (`AppDbContext` og ressursmodellen), men er tilpasset skjemaet, kartet og navnerommet i dette prosjektet. Prosjektet bruker **SQLite**, ikke MySQL-konfigurasjonen fra `Marius`. Datatilgang skjer gjennom `Data/AppDbContext.cs`, entitetene i `Models/DatabaseModels.cs` og asynkrone spørringer/lagringer i `Controllers/HomeController.cs`. UI-et ligger i `Views/Home/` og utformingen i `wwwroot/css/site.css`.

## Kjør lokalt

Krever .NET 10 SDK. Fra repository-roten:

```powershell
dotnet restore
dotnet run
```

Åpne adressen som `dotnet run` skriver ut (typisk `http://localhost:5000`). Databasetilkoblingen er konfigurert i `appsettings.json` som `Data Source=Data/heimevernet.db`. `Program.cs` oppretter katalog/databasen med `EnsureCreated()` ved oppstart; kjøring fra repository-roten gir filen `Data/heimevernet.db`. Ikke legg inn reelle personopplysninger i en offentlig testinstallasjon.

**Viktig ved modellendringer:** `EnsureCreated()` oppretter tabeller i en ny database, men oppdaterer **ikke** eksisterende tabeller. Repositoryet har ikke en EF-migrasjonsflyt for senere skjemaendringer. Har du en eldre database uten ressurskolonnene, ta først en sikkerhetskopi før du oppretter en ny utviklingsdatabase (dette fjerner eksisterende testdata):

```powershell
Copy-Item .\Data\heimevernet.db .\Data\heimevernet-backup.db
Remove-Item .\Data\heimevernet.db
dotnet run
```

Kopierings- og slettekommandoene brukes bare når filen allerede finnes. For å beholde eksisterende data må databaseskjemaet oppgraderes kontrollert; ikke slett den eneste kopien.

## Kjør med Docker Compose

Krever Docker med Compose:

```powershell
docker compose up --build
```

Åpne `http://localhost:8080`. `Dockerfile` bygger og publiserer .NET 10-appen. `.dockerignore` holder lokale `bin/`, `obj/` og SQLite-filer ute av byggekonteksten. Compose setter `ConnectionStrings__DefaultConnection=Data Source=/app/Data/heimevernet.db` og monterer det navngitte volumet `heimevernet-data` på `/app/Data`. Dermed ligger Docker-data i volumet, **adskilt fra den lokale** `Data/heimevernet.db`, og overlever at containeren stoppes eller gjenopprettes. `docker compose down -v` sletter volumet og dets data; ikke bruk `-v` dersom du vil beholde innleveringer.

## Verifiser lagring

1. Åpne `/Home/Form`, send inn gyldige testopplysninger og noter innleveringen.
2. Åpne `/Home/Map`, velg en ressurstype, skriv et ressursnavn, velg et punkt og trykk **Send inn**. Markøren og ressurskortet skal vises når kartet lastes på nytt.
3. Åpne `/Home/Verify` og kontroller ID, radantall, navn/type, sted og koordinater.
4. Stopp og start appen/Compose på nytt **uten å slette databasens fil eller Docker-volumet**. Ny session kan kreve ny skjemainnlevering før du får se Verifiser; sjekk at de opprinnelige radene fortsatt vises. En uendret filstørrelse betyr ikke at data mangler – SQLite kan gjenbruke ledig plass.

## Begrensninger og videre arbeid

- Session-sperren er ikke autentisering. Data- og Verifiser-sidene viser navn og e-post til enhver bruker som har sendt inn skjemaet; begrens tilgang før bruk med faktiske personopplysninger.
- Valgt ressurs på kartet kobles ikke automatisk til personen som først fylte ut skjemaet. Skal ressurser tilhøre en bruker, må en eksplisitt relasjon inn i modellen og databaseskjemaet.
- Skjemaendringer krever migrering eller manuell oppgradering av SQLite-databasen; `EnsureCreated()` utfører ingen migrasjoner.
- Kartfliser, Leaflet og bysøk bruker eksterne nettjenester og krever nettverkstilgang.
- En tidligere lokal `dotnet run` rapporterte NuGet-varselet `NU1903` for transitiv `SQLitePCLRaw.lib.e_sqlite3` 2.1.11. Avhengigheter bør gjennomgås og oppdateres før produksjonsbruk; et vellykket bygg er ikke dokumentasjon på at varselet er løst.
