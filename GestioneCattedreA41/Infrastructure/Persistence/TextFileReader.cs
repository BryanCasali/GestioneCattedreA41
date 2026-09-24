using System;
using System.Collections.Generic;
using System.IO;

namespace GestioneCattedreA41.Infrastructure.Persistence
{
    /// <summary>
    /// Utility per la lettura di file di testo riga per riga, condivisa dai repository.
    /// Isola i dettagli tecnici di I/O (in origine duplicati in Gestione.LeggiFile)
    /// dal resto dell'infrastruttura.
    /// </summary>
    internal static class TextFileReader
    {
        /// <summary>
        /// Legge tutte le righe del file indicato.
        /// A differenza della versione originale (che restituiva silenziosamente null
        /// in caso di errore), solleva un'eccezione esplicita: un file dati mancante
        /// o illeggibile è una condizione anomala che non va nascosta.
        /// </summary>
        public static List<string> ReadLines(string filePath)
        {
            try
            {
                return new List<string>(File.ReadAllLines(filePath));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                throw new InvalidOperationException($"Impossibile leggere il file dati '{filePath}'.", ex);
            }
        }
    }
}
