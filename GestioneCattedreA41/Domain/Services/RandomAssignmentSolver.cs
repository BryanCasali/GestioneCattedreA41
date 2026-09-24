using System;
using System.Collections.Generic;
using System.Linq;
using GestioneCattedreA41.Domain.Entities;

namespace GestioneCattedreA41.Domain.Services
{
    /// <summary>
    /// Implementazione "a tentativi ripetuti" del solver di abbinamento:
    /// ad ogni tentativo scorre le necessità in ordine e, per ciascuna, sceglie
    /// casualmente un docente fra quelli con ore residue sufficienti, finché non
    /// trova una soluzione che copre tutte le necessità oppure supera il numero
    /// massimo di tentativi consentiti.
    /// </summary>
    internal sealed class RandomAssignmentSolver : IAssignmentSolver
    {
        /// <summary>Numero massimo di tentativi prima di arrendersi.</summary>
        private const long MaxAttempts = 10_000_000;

        private readonly Random _random;

        public RandomAssignmentSolver(Random? random = null)
        {
            _random = random ?? new Random();
        }

        public bool TrySolve(
           IReadOnlyList<TeachingRequirement> requirements,
           IReadOnlyList<Teacher> teachers,
           out IReadOnlyList<TeacherAssignment> assignments)
        {
            for (long attempt = 0; attempt < MaxAttempts; attempt++)
            {
                ResetState(requirements, teachers);

                if (TryAssignAll(requirements, teachers, out var result))
                {
                    assignments = result;
                    return true;
                }
            }

            assignments = Array.Empty<TeacherAssignment>();
            return false;
        }

        /// <summary>Riporta necessità e docenti allo stato iniziale prima di un nuovo tentativo.</summary>
        private static void ResetState(IReadOnlyList<TeachingRequirement> requirements, IReadOnlyList<Teacher> teachers)
        {
            foreach (var requirement in requirements)
                requirement.Reset();

            foreach (var teacher in teachers)
                teacher.ResetRemainingHours();
        }

        /// <summary>Prova a coprire, in un singolo tentativo, tutte le necessità nell'ordine dato.</summary>
        private bool TryAssignAll(
            IReadOnlyList<TeachingRequirement> requirements,
            IReadOnlyList<Teacher> teachers,
            out IReadOnlyList<TeacherAssignment> assignments)
        {
            var result = new List<TeacherAssignment>(requirements.Count);

            foreach (var requirement in requirements)
            {
                var teacher = PickRandomAvailableTeacher(teachers, requirement.Hours);
                if (teacher is null)
                {
                    // Nessun docente ha ore residue sufficienti: il tentativo corrente fallisce.
                    assignments = Array.Empty<TeacherAssignment>();
                    return false;
                }

                teacher.Commit(requirement.Hours);
                requirement.MarkAsAssigned();
                result.Add(new TeacherAssignment(requirement, teacher));
            }

            assignments = result;
            return true;
        }

        /// <summary>Sceglie casualmente un docente fra quelli con ore residue sufficienti a coprire le ore richieste.</summary>
        private Teacher? PickRandomAvailableTeacher(IReadOnlyList<Teacher> teachers, int requiredHours)
        {
            var candidates = teachers.Where(t => t.CanCover(requiredHours)).ToList();
            if (candidates.Count == 0)
                return null;

            return candidates[_random.Next(candidates.Count)];
        }

    }
}
