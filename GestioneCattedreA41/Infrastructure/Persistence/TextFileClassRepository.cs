using System;
using System.Collections.Generic;
using System.Linq;
using GestioneCattedreA41.Domain.Entities;
using GestioneCattedreA41.Domain.Repositories;

namespace GestioneCattedreA41.Infrastructure.Persistence
{
    /// <summary>
    /// Legge l'elenco delle classi da un file di testo con una sola riga,
    /// contenente i nomi delle classi separati da ';' (formato invariato: Classi.txt).
    /// </summary>
    internal sealed class TextFileClassRepository : IClassRepository
    {
        private readonly string _filePath;

        public TextFileClassRepository(string filePath)
        {
            _filePath = filePath;
        }

        public IReadOnlyList<SchoolClass> GetAll()
        {
            var lines = TextFileReader.ReadLines(_filePath);
            var firstLine = lines.FirstOrDefault() ?? string.Empty;

            return firstLine
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(name => new SchoolClass(name))
                .ToList();
        }
    }
}
