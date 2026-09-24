using System.Collections.Generic;
using GestioneCattedreA41.Domain.Entities;

namespace GestioneCattedreA41.Domain.Repositories
{
    /// <summary>
    /// Astrazione per il recupero delle classi scolastiche, indipendente dalla sorgente dati
    /// (file di testo, database, ecc.). Fa parte del dominio: solo la sua implementazione
    /// concreta appartiene all'infrastruttura.
    /// </summary>
    internal interface IClassRepository
    {
        /// <summary>Restituisce tutte le classi disponibili.</summary>
        IReadOnlyList<SchoolClass> GetAll();
    }
}
