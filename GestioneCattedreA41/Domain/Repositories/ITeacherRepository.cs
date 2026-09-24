using System.Collections.Generic;
using GestioneCattedreA41.Domain.Entities;

namespace GestioneCattedreA41.Domain.Repositories
{
    /// <summary>Astrazione per il recupero dei docenti e del relativo monte ore.</summary>
    internal interface ITeacherRepository
    {
        /// <summary>Restituisce tutti i docenti disponibili.</summary>
        IReadOnlyList<Teacher> GetAll();
    }
}
