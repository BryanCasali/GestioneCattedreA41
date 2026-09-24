using System;
using System.Collections.Generic;

namespace GestioneCattedreA41.Domain.ValueObjects
{
    /// <summary>
    /// Rappresenta l'ordine di preferenza con cui un docente ha indicato le discipline,
    /// dalla più gradita alla meno gradita. È un value object: due preferenze sono uguali
    /// se contengono la stessa sequenza di nomi di discipline.
    /// </summary>
    internal sealed record TeacherPreferences
    {
        /// <summary>Nomi delle discipline in ordine di preferenza (indice 0 = disciplina preferita).</summary>
        public IReadOnlyList<string> OrderedSubjectNames { get; }

        public TeacherPreferences(IReadOnlyList<string> orderedSubjectNames)
        {
            OrderedSubjectNames = orderedSubjectNames ?? throw new ArgumentNullException(nameof(orderedSubjectNames));
        }

        /// <summary>
        /// Calcola il grado di gradimento della disciplina indicata, normalizzato fra 0 e 1:
        /// 1.0 per la disciplina preferita, valori via via più bassi per le successive,
        /// 0.0 per una disciplina non presente nell'elenco (nessuna preferenza espressa su di essa).
        /// </summary>
        public double SatisfactionFor(string subjectName)
        {
            var rank = -1;
            for (var i = 0; i < OrderedSubjectNames.Count; i++)
            {
                if (OrderedSubjectNames[i] == subjectName)
                {
                    rank = i;
                    break;
                }
            }

            if (rank < 0)
                return 0.0;

            if (OrderedSubjectNames.Count <= 1)
                return 1.0;

            return 1.0 - (double)rank / (OrderedSubjectNames.Count - 1);
        }
    }
}
