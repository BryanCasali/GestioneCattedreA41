using System.Linq;
using System.Text;

namespace GestioneCattedreA41.Application
{
    /// <summary>
    /// Trasforma l'esito del caso d'uso in un report testuale leggibile.
    /// Separata dal dominio e dal servizio applicativo per rispettare
    /// il principio di singola responsabilità (la formattazione non è logica di dominio).
    /// </summary>
    internal static class TimetableReportFormatter
    {
        public static string Format(TimetableResult result)
        {
            var report = new StringBuilder();

            report.AppendLine();
            report.AppendLine("Necessita:");
            foreach (var requirement in result.Requirements)
                report.AppendLine(requirement.ToString());

            report.AppendLine();
            report.AppendLine("Abbinamenti per Classe-Disciplina:");
            foreach (var assignment in result.Assignments)
                report.AppendLine(assignment.ToString());

            report.AppendLine();
            report.AppendLine("Abbinamenti per Docente:");
            foreach (var teacher in result.Teachers)
            {
                report.Append(teacher.Name).Append(": ");

                var teacherAssignments = result.Assignments
                    .Where(a => a.Teacher.Id == teacher.Id)
                    .ToList();

                foreach (var assignment in teacherAssignments)
                    report.Append($"{assignment.Requirement.SchoolClass.Name} ({assignment.Requirement.Subject.Name}) {assignment.Requirement.Hours} ore  ");

                var totalHours = teacherAssignments.Sum(a => a.Requirement.Hours);
                report.AppendLine($"Totale ore: {totalHours}");
            }

            report.AppendLine();
            report.AppendLine("Grado di soddisfazione delle preferenze (0% = mai la disciplina preferita, 100% = sempre):");
            foreach (var teacher in result.Teachers)
            {
                var teacherAssignments = result.Assignments.Where(a => a.Teacher.Id == teacher.Id).ToList();
                if (teacherAssignments.Count == 0)
                    continue;

                var averageSatisfaction = teacherAssignments.Average(a =>
                    teacher.Preferences?.SatisfactionFor(a.Requirement.Subject.Name) ?? 0.5);

                report.AppendLine($"{teacher.Name}: {averageSatisfaction:P0}");
            }

            if (result.Assignments.Count > 0)
            {
                var overallAverage = result.Assignments.Average(a =>
                    a.Teacher.Preferences?.SatisfactionFor(a.Requirement.Subject.Name) ?? 0.5);

                report.AppendLine($"Media complessiva: {overallAverage:P0}");
            }

            return report.ToString();
        }
    }
}
