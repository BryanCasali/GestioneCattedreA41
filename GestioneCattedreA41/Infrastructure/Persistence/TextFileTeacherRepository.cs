using System.Collections.Generic;
using System.Globalization;
using GestioneCattedreA41.Domain.Entities;
using GestioneCattedreA41.Domain.Repositories;

namespace GestioneCattedreA41.Infrastructure.Persistence
{
    /// <summary>
    /// Legge l'elenco dei docenti da un file di testo, una riga per docente,
    /// nel formato "nome;ore" (formato invariato: Docenti.txt).
    /// L'Id viene assegnato in base all'ordine di lettura, come nella versione originale.
    /// </summary>
    internal sealed class TextFileTeacherRepository : ITeacherRepository
    {
        private readonly string _filePath;

        public TextFileTeacherRepository(string filePath)
        {
            _filePath = filePath;
        }

        public IReadOnlyList<Teacher> GetAll()
        {
            var lines = TextFileReader.ReadLines(_filePath);
            var teachers = new List<Teacher>(lines.Count);
            var id = 0;

            foreach (var line in lines)
            {
                var fields = line.Split(';');
                var name = fields[0];
                var contractHours = int.Parse(fields[1], CultureInfo.InvariantCulture);

                teachers.Add(new Teacher(id++, name, contractHours));
            }

            return teachers;
        }
    }
}
