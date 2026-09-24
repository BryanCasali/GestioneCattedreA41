using System.Collections.Generic;
using GestioneCattedreA41.Domain.ValueObjects;

namespace GestioneCattedreA41.Domain.Repositories
{
    /// <summary>
    /// Astrazione per il recupero delle preferenze dichiarate dai docenti sull'ordine
    /// delle discipline. È opzionale rispetto al resto del dominio: non tutti i docenti
    /// devono necessariamente aver espresso una preferenza.
    /// </summary>
    internal interface ITeacherPreferenceRepository
    {
        /// <summary>Restituisce le preferenze dichiarate, indicizzate per nome del docente.</summary>
        IReadOnlyDictionary<string, TeacherPreferences> GetAll();
    }
}
