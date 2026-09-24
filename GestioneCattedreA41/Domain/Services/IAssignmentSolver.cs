using System.Collections.Generic;
using GestioneCattedreA41.Domain.Entities;

namespace GestioneCattedreA41.Domain.Services
{
    /// <summary>
    /// Servizio di dominio responsabile di trovare un abbinamento completo
    /// fra le necessità orarie e i docenti disponibili.
    /// Non appartiene a nessuna entità in particolare: coinvolge l'intero aggregato
    /// "necessità + docenti", perciò è modellato come Domain Service.
    /// </summary>
    internal interface IAssignmentSolver
    {
        /// <summary>
        /// Tenta di produrre un abbinamento che copra tutte le necessità indicate.
        /// Ritorna true e valorizza <paramref name="assignments"/> se ci riesce,
        /// false se non è stata trovata una soluzione valida.
        /// </summary>
        bool TrySolve(
            IReadOnlyList<TeachingRequirement> requirements,
            IReadOnlyList<Teacher> teachers,
            out IReadOnlyList<TeacherAssignment> assignments);
    }
}
