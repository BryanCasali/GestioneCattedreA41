using System.Collections.Generic;
using System.Globalization;
using GestioneCattedreA41.Domain.Repositories;
using GestioneCattedreA41.Domain.ValueObjects;

namespace GestioneCattedreA41.Infrastructure.Persistence
{
    /// <summary>
    /// Legge l'elenco delle discipline da un file di testo, una riga per disciplina,
    /// nel formato "nome;anno;ore" (formato invariato: Discipline.txt).
    /// </summary>
    internal sealed class TextFileSubjectRepository : ISubjectRepository
    {
        private readonly string _filePath;

        public TextFileSubjectRepository(string filePath)
        {
            _filePath = filePath;
        }

        public IReadOnlyList<Subject> GetAll()
        {
            var lines = TextFileReader.ReadLines(_filePath);
            var subjects = new List<Subject>(lines.Count);

            foreach (var line in lines)
            {
                var fields = line.Split(';');
                var name = fields[0];
                var year = int.Parse(fields[1], CultureInfo.InvariantCulture);
                var weeklyHours = int.Parse(fields[2], CultureInfo.InvariantCulture);

                subjects.Add(new Subject(name, year, weeklyHours));
            }

            return subjects;
        }
    }
}
