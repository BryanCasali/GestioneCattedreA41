using System;
using GestioneCattedreA41.Domain.ValueObjects;

namespace GestioneCattedreA41.Domain.Entities
{
    /// <summary>
    /// Rappresenta la necessità di copertura oraria di una disciplina in una classe,
    /// cioè "la classe X ha bisogno di N ore di disciplina Y".
    /// </summary>
    internal sealed class TeachingRequirement
    {
        public SchoolClass SchoolClass { get; }
        public Subject Subject { get; }

        /// <summary>Numero di ore richieste per soddisfare la necessità.</summary>
        public int Hours { get; }

        /// <summary>Vero se la necessità è già stata coperta da un docente.</summary>
        public bool IsAssigned { get; private set; }

        public TeachingRequirement(SchoolClass schoolClass, Subject subject, int hours)
        {
            SchoolClass = schoolClass ?? throw new ArgumentNullException(nameof(schoolClass));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Hours = hours;
        }

        /// <summary>Segna la necessità come coperta da un docente.</summary>
        public void MarkAsAssigned() => IsAssigned = true;

        /// <summary>Riporta la necessità allo stato "non assegnata" (usato ad ogni nuovo tentativo di abbinamento).</summary>
        public void Reset() => IsAssigned = false;

        public override string ToString() =>
            $"Classe: {SchoolClass.Name}, Disciplina: {Subject.Name}, Ore: {Hours}";
    }
}
