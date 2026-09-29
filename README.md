# Heimevernet beredskapsapplikasjon

Dette er gruppens ASP.NET Core MVC-prosjekt for å registrere kontaktinformasjon og plassere ressurser på et kart. Denne README-en beskriver det som faktisk ligger i `main` etter at arbeidet fra `oliver-branch` ble slått inn. Alle i gruppa jobbet felles i samme branch.

## Innhold i leveransen

- Controller (`Controllers/HomeController.cs`)
- ViewModels (`Models/FormViewModels.cs` og `Models/DataPageViewModel.cs`)
- Databasemodeller (`Models/DatabaseModels.cs`) og databasekontekst (`Data/AppDbContext.cs`)
- Views (`Views/Home/*` og `Views/Shared/_Layout.cshtml`)
- Håndtering av GET og POST for skjema
- Kartintegrasjon (Leaflet/OpenStreetMap) med koordinatinnhenting, bysøk og ressursregistrering
- Lagring i database med Entity Framework Core og SQLite
- Resultatsider som viser innsendte data fra skjema og kart (`Data` og `Verifiser`)
- Docker-oppsett for kjøring av applikasjonen, med lagring av databasen i et Docker-volum

## Drift

### Lokal kjøring
1. Gå til prosjektmappen:
   - repository root (mappen med `HeimevernetInnlevering1.csproj`)
2. Kjør:
   - `dotnet run`
3. Åpne URL som vises i terminalen (typisk `http://localhost:5000`).

Databasen opprettes automatisk ved oppstart som `Data/heimevernet.db`.

> **Merk:** Applikasjonen bruker `EnsureCreated()`, som ikke oppdaterer en eksisterende database når modellene endres. Har du en eldre `heimevernet.db`, ta backup og slett den før du kjører på nytt:
> - `Copy-Item .\Data\heimevernet.db .\Data\heimevernet-backup.db`
> - `Remove-Item .\Data\heimevernet.db`

### Kjøring i Docker
Fra rotmappen (`repository root`):

**Anbefalt – Docker Compose (data beholdes mellom omstarter):**
1. Bygg og start:
   - `docker compose up --build`
2. Åpne:
   - `http://localhost:8080`

Compose monterer volumet `heimevernet-data` på `/app/Data`, slik at SQLite-databasen overlever at containeren stoppes. `docker compose down -v` sletter volumet og alle data.

**Alternativ – kun Docker (data forsvinner når containeren stoppes):**
1. Bygg image:
   - `docker build -t heimevernet-web .`
2. Start container:
   - `docker run --rm -p 8080:8080 heimevernet-web`
3. Åpne:
   - `http://localhost:8080`

## Systemarkitektur

- **Presentasjonslag (MVC Views):** Razor views viser skjema, kart, resultat, data og verifisering.
- **Applikasjonslag (Controller):** `HomeController` håndterer GET/POST, validering og flyt.
- **Modellag (ViewModels):** `FormSubmissionViewModel`, `MapSubmissionViewModel` og `DataPageViewModel` holder data for UI.
- **Datalag (Entity Framework Core):** `AppDbContext` med entitetene `FormSubmission` og `MapSubmission`, lagret i SQLite. Datalaget bygger videre på EF-oppsettet og ressursmodellen (`Ressurs`) fra Marius sitt arbeid, tilpasset SQLite og dette prosjektet.
- **Tilgangsflyt (session):** Et mellomledd i `Program.cs` sender brukere som ikke har fylt ut skjemaet i denne nettleserøkten til `/Home/Form`.
- **Ekstern kartkilde:** Leaflet med OpenStreetMap tiles brukes i nettleser for valg av koordinater. Bysøk bruker OpenStreetMap Nominatim.

Flyt:
1. Bruker åpner siden og blir sendt til `GET /Home/Form` hvis skjemaet ikke er fylt ut i denne økten.
2. Bruker fyller ut skjema (fornavn, etternavn, e-post, beskrivelse).
3. `POST /Home/Form` validerer data, lagrer i databasen og låser opp resten av siden.
4. Bruker åpner `GET /Home/Map`. Kartet starter i Kristiansand og viser alle lagrede ressurser som markører.
5. Bruker velger ressurstype (Kjøretøy, Maskin, Drone, Generator, Personell, Utstyr), skriver ressursnavn og klikker i kartet eller søker etter by.
6. `POST /Home/Map` validerer og lagrer ressursen. Kartet lastes på nytt med ny markør og informasjonsboks.
7. `GET /Home/Data` og `GET /Home/Verify` viser innsendte data hentet direkte fra databasen.

## Testing – scenarier og resultater

### Scenario 1: Gyldig innsending av skjema
- **Steg:** Fyll inn alle felt, send.
- **Forventet:** Resultatside med innsendte data, og en ny rad i `FormSubmissions`.
- **Resultat:** OK. Konsollen viste `INSERT INTO "FormSubmissions"`, og `Data/heimevernet.db` ble oppdatert.

### Scenario 2: Manglende skjema-felt
- **Steg:** Send uten navn/beskrivelse.
- **Forventet:** Valideringsfeil i samme side.
- **Resultat:** OK.

### Scenario 3: Registrering av ressurs på kart
- **Steg:** Velg ressurstype, skriv ressursnavn, klikk i kartet, trykk **Send inn**.
- **Forventet:** Kartet lastes på nytt med ny markør, informasjonsboks og ressurskort.
- **Resultat:** Fylles inn av gruppen etter test.

### Scenario 4: Manglende kartposisjon
- **Steg:** Send uten å klikke i kartet.
- **Forventet:** Ingen valideringsfeil – ressursen lagres på standardposisjonen (Kristiansand, 58.1467, 7.9956).
- **Resultat:** Fylles inn av gruppen etter test.

### Scenario 5: Tilgang uten utfylt skjema
- **Steg:** Åpne `/Home/Map` i en ny nettleserøkt.
- **Forventet:** Bruker sendes til `/Home/Form`.
- **Resultat:** Fylles inn av gruppen etter test.

### Scenario 6: Lagring overlever omstart
- **Steg:** Send inn data, stopp applikasjonen, start igjen og åpne `/Home/Verify`.
- **Forventet:** Samme rader med samme ID-er vises.
- **Resultat:** OK for lokal kjøring (data lå fortsatt i databasen etter flere omstarter).

### Teknisk verifikasjon
- `dotnet build` / `dotnet run` i repository root.
- Resultat: Bygger og kjører. Byggingen gir advarselen `NU1903` for pakken `SQLitePCLRaw.lib.e_sqlite3` 2.1.11 (kjent sårbarhet), som bør oppdateres før produksjonsbruk.
- `docker compose up --build`: første forsøk feilet med `NETSDK1064` fordi lokale `bin/`/`obj/`-mapper ble kopiert inn i imaget. Løst med `.dockerignore` og ved å fjerne `--no-restore` i `Dockerfile`.

## Kjente begrensninger

- Session-sperren er ikke innlogging. En ny nettleserøkt må fylle ut skjemaet på nytt.
- `Data` og `Verifiser` viser alle registreringer (inkludert navn og e-post) til alle som har fylt ut skjemaet.
- Ressurser på kartet er ikke knyttet til personen som registrerte dem via en relasjon i databasen.
- Endringer i modellene krever ny database, siden prosjektet ikke bruker EF-migrasjoner.

## Dokumentasjon i kode

Kjernelogikk er dokumentert med kommentarer i:
- `HomeController` (lagring, kartflyt og verifisering)
- `AppDbContext`
- `MapSubmission` (ressursmodell basert på Marius sitt arbeid)
- `Program.cs` (databaseoppsett og tilgangsflyt)

## Bruk av KI i prosjektet

Formål med KI-bruk:
- Strukturering av leveransekrav til konkrete utviklingsoppgaver.
- Forslag til oppsett av MVC-komponenter (controller/viewmodel/view).
- Sammenslåing av Entity Framework-oppsettet fra Marius-branchen med skjema- og kartdesignet.
- Feilsøking av bygg- og Docker-feil.
- Kvalitetssikring av dokumentasjon og leveranseinnhold.

Verktøy brukt:
- GitHub Copilot (agentbasert arbeidsflyt)

Eksempler på prompt-kommandoer brukt i prosessen:
- «Lag en ASP.NET Core MVC-løsning med GET/POST for hendelsesskjema.»
- «Se på Marius-branchen og kombiner Entity Framework med skjema- og kartdesignet, slik at informasjonen faktisk lagres.»
- «La kartet starte i Kristiansand og lag en side som viser lagret informasjon.»
- «Legg til ressurstype og ressursnavn fra Marius-branchen»

KI ble brukt som støtteverktøy for idé, struktur og kodeforslag. Gruppen evaluerte, testet og tilpasset resultatet manuelt før ferdigstillelse. Før større endringer ble det laget en backup-branch (`backup/oliver-before-resource-map-2026-09-28`) slik at endringene kunne rulles tilbake.
