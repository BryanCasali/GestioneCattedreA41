using System.Collections.Generic;
using System.Linq;
using GestioneCattedreA41.Domain.Repositories;
using GestioneCattedreA41.Domain.ValueObjects;

namespace GestioneCattedreA41.Infrastructure.Persistence
{
    /// <summary>
    /// Legge le preferenze dei docenti da un file di testo, una riga per docente, nel formato
    /// "NomeDocente;Disciplina1;Disciplina2;...;DisciplinaN" (Preferenze.txt), con le discipline
    /// elencate dalla più preferita alla meno preferita.
    /// </summary>
    internal sealed class TextFilePreferenceRepository : ITeacherPreferenceRepository
    {
        private readonly string _filePath;

        public TextFilePreferenceRepository(string filePath)
        {
            _filePath = filePath;
        }

        public IReadOnlyDictionary<string, TeacherPreferences> GetAll()
        {
            var lines = TextFileReader.ReadLines(_filePath);
            var preferencesByTeacherName = new Dictionary<string, TeacherPreferences>(lines.Count);

            foreach (var line in lines)
            {
                var fields = line.Split(';');
                var teacherName = fields[0];
                var orderedSubjectNames = fields.Skip(1).ToList();

                preferencesByTeacherName[teacherName] = new TeacherPreferences(orderedSubjectNames);
            }

            return preferencesByTeacherName;
        }
    }
}
