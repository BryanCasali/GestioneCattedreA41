using System;

namespace GestioneCattedreA41.Domain.Entities
{
    /// <summary>
    /// Rappresenta una classe scolastica (es. "3E").
    /// È un'entità di dominio: la sua identità è definita dal nome.
    /// </summary>
    internal sealed class SchoolClass
    {
        /// <summary>Nome della classe (es. "3E").</summary>
        public string Name { get; }

        /// <summary>Totale ore settimanali di cattedra assegnate a questa classe.</summary>
        public int TotalWeeklyHours { get; private set; }

        public SchoolClass(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Il nome della classe non può essere vuoto.", nameof(name));

            Name = name;
            TotalWeeklyHours = 0;
        }

        /// <summary>
        /// Aggiunge ore di insegnamento al totale della classe.
        /// Viene invocato quando si individua una disciplina compatibile con l'anno della classe.
        /// </summary>
        public void AddWeeklyHours(int hours)
        {
            TotalWeeklyHours += hours;
        }

        /// <summary>
        /// Determina se la classe appartiene all'anno di corso indicato,
        /// ricavandolo dalla prima cifra del nome (es. "3E" -> anno 3).
        /// </summary>
        public bool BelongsToYear(int year)
        {
            return Name.StartsWith(year.ToString());
        }

        public override string ToString() => $"Classe: {Name} TotOre: {TotalWeeklyHours}";
    }
}
