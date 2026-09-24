using GestioneCattedreA41.Domain.Entities;

namespace GestioneCattedreA41.Domain.Services
{
    public class PartitionSolver : IAssignmentSolver
    {
        bool IAssignmentSolver.TrySolve(IReadOnlyList<TeachingRequirement> requirements, IReadOnlyList<Teacher> teachers, out IReadOnlyList<TeacherAssignment> assignments)
        {
            
        }

        private bool Try(IReadOnlyList<TeachingRequirement> requirements, IReadOnlyList<Teacher> teachers, out IReadOnlyList<TeacherAssignment> assignments)
        {
            foreach (Teacher teacher in teachers)
            {
                if (teacher.ContractHours < 18)
                {

                }
            }
        }


    }
}
