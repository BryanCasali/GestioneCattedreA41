using System.Collections.Generic;
using GestioneCattedreA41.Domain.ValueObjects;

namespace GestioneCattedreA41.Domain.Repositories
{
    /// <summary>Astrazione per il recupero delle discipline del piano di studi.</summary>
    internal interface ISubjectRepository
    {
        /// <summary>Restituisce tutte le discipline disponibili.</summary>
        IReadOnlyList<Subject> GetAll();
    }
}
