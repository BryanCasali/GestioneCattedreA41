using System.Collections.Generic;
using GestioneCattedreA41.Domain.Entities;

namespace GestioneCattedreA41.Application
{
    /// <summary>
    /// Esito del caso d'uso "genera l'abbinamento cattedre": raccoglie le necessità
    /// calcolate, gli abbinamenti trovati e i docenti coinvolti, così che possano
    /// essere presentati.
    /// </summary>
    internal sealed record TimetableResult(
        IReadOnlyList<TeachingRequirement> Requirements,
        IReadOnlyList<TeacherAssignment> Assignments,
        IReadOnlyList<Teacher> Teachers);
}
