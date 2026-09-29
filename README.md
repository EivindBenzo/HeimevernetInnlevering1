# Heimevernet beredskapsapplikasjon

Dette er gruppens ASP.NET Core MVC-prosjekt for å registrere kontaktinformasjon og plassere ressurser på et kart. Denne README-en beskriver det som faktisk ligger i main etter at arbeidet fra oliver-branch ble slått inn. Alle i gruppa jobbet felles i samme branch

## Innhold i leveransen

- Controller (`Heimevernet.Web/Controllers/BeredskapController.cs`)
- ViewModel (`Heimevernet.Web/Models/IncidentReportFormViewModel.cs`)
- Views (`Heimevernet.Web/Views/Beredskap/*`)
- Håndtering av GET og POST for skjema
- Kartintegrasjon (Leaflet/OpenStreetMap) med koordinatinnhenting
- Resultatside som viser innsendte data fra skjema og kart
- Docker-oppsett for kjøring av applikasjonen

## Drift

### Lokal kjøring
1. Gå til prosjektmappen:
   - `Heimevernet.Web`
2. Kjør:
   - `dotnet run`
3. Åpne URL som vises i terminalen.

### Kjøring i Docker
Fra rotmappen (`repository root`):

1. Bygg image:
   - `docker build -t heimevernet-web .`
2. Start container:
   - `docker run --rm -p 8080:8080 heimevernet-web`
3. Åpne:
   - `http://localhost:8080`

## Systemarkitektur

- **Presentasjonslag (MVC Views):** Razor views viser skjema, kart og resultat.
- **Applikasjonslag (Controller):** `BeredskapController` håndterer GET/POST, validering og flyt.
- **Modellag (ViewModels):** `IncidentReportFormViewModel` og `IncidentReportResultViewModel` holder data for UI.
- **Ekstern kartkilde:** Leaflet med OpenStreetMap tiles brukes i nettleser for valg av koordinater.

Flyt:
1. Bruker åpner `GET /Beredskap/MeldHendelse`.
2. Server returnerer dynamisk innhold (f.eks. servergenerert tidspunkt + hendelsestyper).
3. Bruker fyller skjema og klikker i kart.
4. `POST /Beredskap/MeldHendelse` validerer data.
5. Ved gyldig data vises `MeldingMottatt` med innsendte verdier.

## Testing – scenarier og resultater

### Scenario 1: Gyldig innsending
- **Steg:** Fyll inn alle felt, velg hendelsestype, klikk kart, send.
- **Forventet:** Melding mottatt-side med alle innsendte data.
- **Resultat:** OK.

### Scenario 2: Manglende skjema-felt
- **Steg:** Send uten navn/beskrivelse.
- **Forventet:** Valideringsfeil i samme side.
- **Resultat:** OK.

### Scenario 3: Manglende kartposisjon
- **Steg:** Send uten å klikke i kartet.
- **Forventet:** Valideringsfeil for koordinater.
- **Resultat:** OK.

### Teknisk verifikasjon
- `dotnet build` i `Heimevernet.Web`.
- Resultat: Build succeeded uten feil.

## Dokumentasjon i kode

Kjernelogikk er dokumentert med XML-kommentarer i controller og view models:
- `BeredskapController`
- `IncidentReportFormViewModel`
- `IncidentReportResultViewModel`

## Bruk av KI i prosjektet

Formål med KI-bruk:
- Strukturering av leveransekrav til konkrete utviklingsoppgaver.
- Forslag til oppsett av MVC-komponenter (controller/viewmodel/view).
- Kvalitetssikring av dokumentasjon og leveranseinnhold.

Verktøy brukt:
- GitHub Copilot (agentbasert arbeidsflyt)

Eksempler på prompt-kommandoer brukt i prosessen:
- «Lag en ASP.NET Core MVC-løsning med GET/POST for hendelsesskjema.»
- «Legg til kart (Leaflet) og send koordinater i samme skjema.»
- «Oppdater README med drift, arkitektur, testscenarier og KI-bruk.»

KI ble brukt som støtteverktøy for idé, struktur og kodeforslag. Gruppen evaluerte og tilpasset resultatet manuelt før ferdigstillelse.
