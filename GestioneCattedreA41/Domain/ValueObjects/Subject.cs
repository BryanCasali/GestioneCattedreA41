using System;

namespace GestioneCattedreA41.Domain.ValueObjects
{
    /// <summary>
    /// Rappresenta una disciplina scolastica del piano di studi (es. "Sistemi e Reti").
    /// È un value object: due discipline sono considerate uguali se hanno
    /// nome, anno e ore uguali (i "record" in C# forniscono questa uguaglianza per valore).
    /// </summary>
    internal sealed record Subject
    {
        /// <summary>Nome della disciplina.</summary>
        public string Name { get; }

        /// <summary>Anno di corso in cui viene insegnata (1..5).</summary>
        public int Year { get; }

        /// <summary>Ore settimanali previste dal piano di studi per quell'anno.</summary>
        public int WeeklyHours { get; }

        public Subject(string name, int year, int weeklyHours)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Il nome della disciplina non può essere vuoto.", nameof(name));
            if (weeklyHours <= 0)
                throw new ArgumentOutOfRangeException(nameof(weeklyHours), "Le ore settimanali devono essere positive.");

            Name = name;
            Year = year;
            WeeklyHours = weeklyHours;
        }

        public override string ToString() => $"Nome: {Name} Anno: {Year} Ore: {WeeklyHours}";
    }
}
