using System.Collections.Generic;
using GestioneCattedreA41.Domain.Entities;
using GestioneCattedreA41.Domain.Repositories;
using GestioneCattedreA41.Domain.Services;
using GestioneCattedreA41.Domain.ValueObjects;

namespace GestioneCattedreA41.Application
{
    /// <summary>
    /// Servizio applicativo che coordina il caso d'uso "genera l'abbinamento cattedre":
    /// carica classi, discipline e docenti dai repository, calcola le necessità orarie
    /// e delega al dominio la ricerca di un abbinamento valido.
    /// Non contiene regole di dominio: si limita a orchestrare le collaborazioni.
    /// </summary>
    internal sealed class TimetableBuildingService
    {
        private readonly IClassRepository _classRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ITeacherPreferenceRepository? _preferenceRepository;
        private readonly IAssignmentSolver _solver;

        public TimetableBuildingService(
            IClassRepository classRepository,
            ISubjectRepository subjectRepository,
            ITeacherRepository teacherRepository,
            IAssignmentSolver solver,
            ITeacherPreferenceRepository? preferenceRepository = null)
        {
            _classRepository = classRepository;
            _subjectRepository = subjectRepository;
            _teacherRepository = teacherRepository;
            _solver = solver;
            _preferenceRepository = preferenceRepository;
        }

        /// <summary>
        /// Esegue il caso d'uso completo: import dati, calcolo necessità, ricerca abbinamento.
        /// Restituisce null se non è stato possibile trovare un abbinamento valido.
        /// </summary>
        public TimetableResult? Build()
        {
            var classes = _classRepository.GetAll();
            var subjects = _subjectRepository.GetAll();
            var teachers = _teacherRepository.GetAll();

            AttachPreferences(teachers);

            var requirements = BuildRequirements(classes, subjects);

            if (!_solver.TrySolve(requirements, teachers, out var assignments))
                return null;

            return new TimetableResult(requirements, assignments, teachers);
        }

        /// <summary>
        /// Se è stato configurato un repository delle preferenze, le associa ai docenti
        /// corrispondenti (per nome). Un docente senza preferenze dichiarate resta "neutro"
        /// agli occhi del solver.
        /// </summary>
        private void AttachPreferences(IReadOnlyList<Teacher> teachers)
        {
            if (_preferenceRepository is null)
                return;

            var preferencesByTeacherName = _preferenceRepository.GetAll();

            foreach (var teacher in teachers)
            {
                if (preferencesByTeacherName.TryGetValue(teacher.Name, out var preferences))
                    teacher.SetPreferences(preferences);
            }
        }

        /// <summary>
        /// Calcola le necessità orarie incrociando ogni classe con le discipline previste
        /// per il suo anno di corso, aggiornando contestualmente il totale ore di ogni classe.
        /// Corrisponde alla logica originale CalcoloOre + ElaboraNecessita.
        /// </summary>
        private static List<TeachingRequirement> BuildRequirements(
            IReadOnlyList<SchoolClass> classes,
            IReadOnlyList<Subject> subjects)
        {
            var requirements = new List<TeachingRequirement>();

            foreach (var schoolClass in classes)
            {
                foreach (var subject in subjects)
                {
                    if (!schoolClass.BelongsToYear(subject.Year))
                        continue;

                    schoolClass.AddWeeklyHours(subject.WeeklyHours);
                    requirements.Add(new TeachingRequirement(schoolClass, subject, subject.WeeklyHours));
                }
            }

            return requirements;
        }
    }
}
