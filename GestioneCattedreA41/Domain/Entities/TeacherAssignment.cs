using System;

namespace GestioneCattedreA41.Domain.Entities
{
    /// <summary>
    /// Rappresenta l'abbinamento fra un docente e una necessità oraria:
    /// la "cattedra" assegnata al docente per una specifica classe/disciplina.
    /// </summary>
    internal sealed class TeacherAssignment
    {
        public TeachingRequirement Requirement { get; }
        public Teacher Teacher { get; }

        public TeacherAssignment(TeachingRequirement requirement, Teacher teacher)
        {
            Requirement = requirement ?? throw new ArgumentNullException(nameof(requirement));
            Teacher = teacher ?? throw new ArgumentNullException(nameof(teacher));
        }

        public override string ToString() =>
            $"{Requirement.SchoolClass.Name} {Requirement.Subject.Name} {Requirement.Hours} {Teacher.Name}";
    }
}
