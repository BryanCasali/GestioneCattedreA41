using GestioneCattedreA41.Application;
using GestioneCattedreA41.Domain.Repositories;
using GestioneCattedreA41.Domain.Services;
using GestioneCattedreA41.Infrastructure.Persistence;
using GestioneCattedreA41.Legacy;
using System;

namespace GestioneCattedreA41
{
    /// <summary>
    /// Entry point dell'applicazione. Fa da "composition root": assembla le implementazioni
    /// concrete (repository su file di testo, solver basato sul metodo scelto) e le inietta
    /// nel servizio applicativo, senza contenere alcuna logica di dominio.
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {

            //IClassRepository classRepository = new TextFileClassRepository("Classi.txt");
            //ISubjectRepository subjectRepository = new TextFileSubjectRepository("Discipline.txt");
            //ITeacherRepository teacherRepository = new TextFileTeacherRepository("Docenti.txt");
            //ITeacherPreferenceRepository preferenceRepository = new TextFilePreferenceRepository("Preferenze.txt");
            //IAssignmentSolver solver = new RandomAssignmentSolver();

            //var timetableService = new TimetableBuildingService(
            //    classRepository, subjectRepository, teacherRepository, solver, preferenceRepository);

            //var result = timetableService.Build();

            //if (result is not null)
            //    Console.WriteLine(TimetableReportFormatter.Format(result));
            //else
            //    Console.WriteLine("Non è stato possibile trovare un abbinamento valido entro il numero massimo di tentativi.");

            List<UInt32> vals = new List<UInt32>() {
           498,497,483,479,391,390,386,376,369,367,364,362,361,359,298,283,255,248,220,183,156,149,133,131,118,106,87,61,60,50
           };
            effPartition p = new effPartition(vals, 4);
            p.Sort();
            p.Print();
            // sample of partition output
            Console.WriteLine("inaccuracy: {0:d} ({1:f}%)", p.Inacc, p.RelInacc);
            // using indexer
            for (UInt16 i = 0; i < p.SubsetCount; i++)
            {
                Console.Write("subset {0:d}\t", p[i].ID);
                foreach (UInt16 iID in p[i].NumbIDs)
                    Console.Write("{0:d} ", iID);
                Console.WriteLine();
            }
        }
    }
}
