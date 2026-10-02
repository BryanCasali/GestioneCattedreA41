using GestioneCattedreA41.Domain.Entities;
using Google.OrTools.Sat;

namespace GestioneCattedreA41.Domain.Services
{
    internal class CpSatAssignmentSolver : IAssignmentSolver
    {
        private readonly int SatisfactionScale = 1000;
        private const double MaxSeconds =30.0;

        public bool TrySolve(IReadOnlyList<TeachingRequirement> requirements, IReadOnlyList<Teacher> teachers, out IReadOnlyList<TeacherAssignment> assignments)
        {
            ResetState(requirements, teachers);

            var m = teachers.Count;
            var n = requirements.Count;

            var model = new CpModel();

            var x = new BoolVar[m, n];

            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    // i = teachers; j = requirement
                    x[i, j] = model.NewBoolVar($"x_{i}_{j}");
                }
            }

            // vincolo: ogni necessità coperta da esattamente un docente
            for (var j = 0; j < n; j++)
            {
                var col = new List<BoolVar>(m);
                for (var i = 0; i < m; i++)
                {
                    col.Add(x[i, j]);
                }
                model.AddExactlyOne(col);
            }

            // vincolo: monte ore del docente non superato
            for (var i = 0; i < m; i++)
            {
                var row = new IntVar[n];
                var w = new long[n]; // wj => ore delle necessità
                for (var j = 0; j < n; j++)
                {
                    row[j] = x[i, j];
                    w[j] = requirements[j].Hours;
                }
                var bi = teachers[i].ContractHours;

                model.Add(LinearExpr.WeightedSum(row, w) <= bi);
            }

            //Obiettivo massimizzare la funzione obiettivo
            var allVariables = new List<IntVar>(m * n);
            var allCoef = new List<long>(m * n);
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    var sij = teachers[i].Preferences.SatisfactionFor(requirements[j].Subject.Name);
                    allVariables.Add(x[i, j]);
                    allCoef.Add((long)Math.Round((decimal)sij * SatisfactionScale));
                }
            }
            model.Maximize(LinearExpr.WeightedSum(allVariables, allCoef));

            //Risoluzione
            var solver = new CpSolver();
            solver.StringParameters = $"max_time_in_seconds: {MaxSeconds}";
            var status = solver.Solve(model);

            if(status != CpSolverStatus.Optimal && status != CpSolverStatus.Feasible)
            {
                assignments = Array.Empty<TeacherAssignment>();
                return false;
            }

            //Applichiamo la soluzione trovata agli oggetti del dominio
            var result = new List<TeacherAssignment>(n);
            for (var j = 0; j < n; j++)
            {
                for (var i = 0; i < m; i++)
                {
                    //se x_ij == 1 nella soluzione trovata -> allora la necessità j va al docente i
                    if (solver.Value(x[i, j]) == 1)
                    {
                        teachers[i].Commit(requirements[j].Hours);
                        requirements[j].MarkAsAssigned();
                        result.Add(new TeacherAssignment(requirements[j], teachers[i]));
                    }
                }
            }
            assignments = result;
            return true;
        }

        private void ResetState(IEnumerable<TeachingRequirement> requirements, IEnumerable<Teacher> teachers)
        {
            foreach (var requirement in requirements)
                requirement.Reset();
            foreach (var teacher in teachers)
                teacher.ResetRemainingHours();
        }
    }
}
