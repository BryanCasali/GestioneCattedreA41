# GestioneCattedreA41 — refactoring DDD

Codice in inglese, commenti in italiano. Comportamento invariato rispetto
all'originale (stessi file dati `Classi.txt`, `Discipline.txt`, `Docenti.txt`,
stesso algoritmo di abbinamento a tentativi casuali).

## Struttura

```
GestioneCattedreA41/
  Domain/
    Entities/
      SchoolClass.cs          (ex Classe)
      Teacher.cs               (ex Docente)
      TeachingRequirement.cs   (ex Necessita)
      TeacherAssignment.cs     (ex Abbinamento)
    ValueObjects/
      Subject.cs                (ex Disciplina — ora un record immutabile)
    Services/
      IAssignmentSolver.cs
      RandomAssignmentSolver.cs    (logica di CalcolaAbbinamento/CercaNuovoAbbinamento)
      NeuralAssignmentSolver.cs    (rete neurale + REINFORCE, solver attivo in Program.cs)
      PartitionAssignmentSolver.cs (ex CercaNuovoAbbinamentoPart, disponibile ma non attivo)
      Neural/
        PreferenceScoringNetwork.cs
    Repositories/
      IClassRepository.cs
      ISubjectRepository.cs
      ITeacherRepository.cs
      ITeacherPreferenceRepository.cs
  Application/
    TimetableBuildingService.cs  (orchestrazione: ex Gestione.ImportaDati + CalcolaAbbinamento)
    TimetableResult.cs
    TimetableReportFormatter.cs  (ex Gestione.ToString)
  Infrastructure/
    Persistence/
      TextFileReader.cs
      TextFileClassRepository.cs
      TextFileSubjectRepository.cs
      TextFileTeacherRepository.cs
      TextFilePreferenceRepository.cs   (legge Preferenze.txt)
  Legacy/
    EffPartition.cs   (algoritmo di terze parti, usato da PartitionAssignmentSolver)
  Program.cs           (composition root)
  Preferenze.txt        (quarto file dati: preferenze docenti su disciplina, di esempio)
```

## Principali scelte di design

- **Domain**: entità (`SchoolClass`, `Teacher`, `TeachingRequirement`,
  `TeacherAssignment`) con identità e comportamento incapsulato (es. `Teacher`
  non permette di scendere sotto zero ore residue), value object immutabile
  per `Subject`. La logica di ricerca dell'abbinamento è isolata in un
  *domain service* (`IAssignmentSolver`), così da poter sostituire in futuro
  l'algoritmo randomico con uno più efficiente senza toccare il resto.
- **Application**: `TimetableBuildingService` orchestra il caso d'uso senza
  contenere regole di dominio; la formattazione del report è stata separata
  in `TimetableReportFormatter` (nella versione originale viveva dentro
  `Gestione.ToString`, mescolata con lo stato).
- **Infrastructure**: i repository su file di testo implementano le
  interfacce di dominio; il formato dei file `.txt` non è cambiato. A
  differenza dell'originale, un file dati mancante o illeggibile ora solleva
  un'eccezione esplicita invece di fallire silenziosamente.
- **Program.cs**: è il solo punto in cui si scelgono le implementazioni
  concrete (composition root), seguendo l'inversione delle dipendenze.

## Preferenze dei docenti e solver neurale

- **Quarto file dati** (`Preferenze.txt`): una riga per docente, formato
  `NomeDocente;Disciplina1;Disciplina2;...;DisciplinaN`, discipline elencate
  dalla più preferita alla meno preferita. È letto da
  `TextFilePreferenceRepository` (implementa `ITeacherPreferenceRepository`,
  interfaccia di dominio) e le preferenze vengono associate ai docenti
  corrispondenti da `TimetableBuildingService.AttachPreferences`. Un docente
  senza riga in `Preferenze.txt` resta "neutro" (né favorito né penalizzato).
- **`Domain/ValueObjects/TeacherPreferences.cs`**: value object che, data una
  disciplina, restituisce un grado di soddisfazione normalizzato fra 0 e 1
  (1.0 = disciplina preferita, valori decrescenti per le successive, 0.0 se
  non presente in elenco).
- **`Domain/Services/Neural/PreferenceScoringNetwork.cs`**: rete neurale
  feed-forward scritta da zero (2 ingressi → 6 nodi nascosti con `tanh` → 1
  uscita lineare), senza librerie di ML esterne — forward pass e backward
  pass (discesa/salita del gradiente) sono espliciti e commentati, per
  restare coerenti con lo scopo didattico del progetto. Gli ingressi sono il
  grado di soddisfazione del docente per la disciplina e la quota di ore
  ancora disponibili sul suo monte ore.
- **`Domain/Services/NeuralAssignmentSolver.cs`**: nuova implementazione di
  `IAssignmentSolver` (sostituisce `RandomAssignmentSolver` in `Program.cs`,
  che resta comunque disponibile) che addestra la rete con una tecnica di
  *policy gradient* (REINFORCE):
  1. Ad ogni episodio di addestramento costruisce un abbinamento completo
     campionando, per ciascuna necessità, un docente fra quelli disponibili
     secondo una distribuzione softmax sui punteggi della rete.
  2. Se l'abbinamento copre tutte le necessità, calcola la ricompensa come
     soddisfazione media dei docenti coinvolti e aggiorna i pesi per rendere
     più probabili le scelte fatte negli episodi con ricompensa superiore a
     una media mobile (la "baseline"), meno probabili quelle sotto media.
  3. Dopo l'addestramento (4000 episodi), esegue fino a 3000 tentativi con la
     policy appresa e restituisce quello con la soddisfazione media più alta
     fra tutti gli abbinamenti validi trovati.

  Il report finale (`TimetableReportFormatter`) mostra, in coda, il grado di
  soddisfazione ottenuto da ciascun docente e la media complessiva: è la
  grandezza che la rete cerca di massimizzare.

  **Nota**: l'aggiornamento dei pesi è fatto candidato per candidato invece
  che accumulando il gradiente su tutto l'episodio prima di applicarlo — una
  semplificazione scelta per chiarezza del codice, che in pratica funziona
  bene su un problema di queste dimensioni ma è una scorciatoia rispetto alla
  formulazione "da manuale" di REINFORCE (utile saperlo se lo si usa a scopo
  didattico).

## Solver a partizionamento (il tuo codice originale, tradotto)

`Domain/Services/PartitionAssignmentSolver.cs` è la traduzione fedele in
inglese del metodo `CercaNuovoAbbinamentoPart` (con i suoi due metodi di
supporto `TrovaDivisoreMax` e `TrovaPrimaNecessita`), presente nel progetto
originale ma mai attivato perché la sua chiamata in
`Gestione.CalcolaAbbinamento` era commentata. La logica non è cambiata:

1. **Docenti sotto cattedra piena** (`ContractHours < 18`): per ciascuno si
   cerca il divisore massimo del suo monte ore che corrisponde a un blocco
   orario effettivamente presente fra le necessità (`FindMaxDivisor`, ex
   `TrovaDivisoreMax`), e gli si abbinano tante necessità di quel blocco
   quante il monte ore ne contiene (`FindFirstUnassignedRequirement`, ex
   `TrovaPrimaNecessita`).
2. **Docenti a cattedra piena** (`ContractHours >= 18`): le necessità
   rimaste vengono ripartite fra loro con l'algoritmo di partizione di terze
   parti `effPartition` (ora richiamato da `Legacy/EffPartition.cs`), che
   minimizza lo squilibrio fra le somme dei sottoinsiemi; ogni sottoinsieme
   trovato viene poi abbinato al docente nella stessa posizione.

Come nell'originale, non ci sono tentativi ripetuti: se alla fine restano
necessità scoperte, `TrySolve` restituisce `false`. Ho mantenuto anche un
dettaglio "grezzo" dell'originale — due totali (`totalUnassignedHours`,
`totalAssignedHours`) calcolati nel secondo passo ma mai più utilizzati — per
fedeltà alla logica di partenza, con un commento che lo segnala.

Il solver **non è collegato di default** in `Program.cs` (esattamente come
nell'originale, dov'era scritto ma disattivato): quello attivo resta
`NeuralAssignmentSolver`. Per provarlo basta sostituire in `Program.cs`:
```csharp
IAssignmentSolver solver = new PartitionAssignmentSolver();
```
Nota: questo solver ignora le preferenze dei docenti (`Teacher.Preferences`)
e il numero massimo di ore è fissato a 18 tramite la costante
`FullTimeHours`, proprio come nel codice originale.

## Codice rimosso o messo da parte

- `Models/Partion.cs`: codice morto (mai richiamato da nessun metodo,
  nemmeno da `CercaNuovoAbbinamentoPart`), rimosso.

## Nota

Non è stato possibile compilare il progetto in questo ambiente (SDK .NET non
disponibile): il codice è stato scritto e rivisto con attenzione, ma si
consiglia una build/run in Visual Studio prima dell'uso didattico.
