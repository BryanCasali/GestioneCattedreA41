using System;
using GestioneCattedreA41.Domain.ValueObjects;

namespace GestioneCattedreA41.Domain.Entities
{
    /// <summary>
    /// Rappresenta un docente con un monte ore di cattedra da coprire.
    /// È un'entità: la sua identità è l'Id, non i valori dei suoi attributi.
    /// </summary>
    internal sealed class Teacher
    {
        /// <summary>Identificativo univoco del docente.</summary>
        public int Id { get; }

        public string Name { get; }

        /// <summary>Monte ore contrattuale del docente.</summary>
        public int ContractHours { get; }

        /// <summary>Ore ancora disponibili: si riducono ad ogni abbinamento con una necessità.</summary>
        public int RemainingHours { get; private set; }

        /// <summary>
        /// Preferenze dichiarate dal docente sull'ordine delle discipline (facoltative:
        /// un docente per cui non è stata importata alcuna preferenza ha questa proprietà a null).
        /// </summary>
        public TeacherPreferences? Preferences { get; private set; }

        public Teacher(int id, string name, int contractHours)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Il nome del docente non può essere vuoto.", nameof(name));
            if (contractHours <= 0)
                throw new ArgumentOutOfRangeException(nameof(contractHours), "Le ore di contratto devono essere positive.");

            Id = id;
            Name = name;
            ContractHours = contractHours;
            RemainingHours = contractHours;
        }

        /// <summary>Vero se il docente ha ancora ore residue sufficienti per coprire la quantità richiesta.</summary>
        public bool CanCover(int hours) => RemainingHours >= hours;

        /// <summary>Impegna un certo numero di ore residue del docente (lo abbina a una necessità).</summary>
        public void Commit(int hours)
        {
            if (!CanCover(hours))
                throw new InvalidOperationException($"Il docente {Name} non ha abbastanza ore residue per coprire {hours} ore.");

            RemainingHours -= hours;
        }

        /// <summary>Ripristina le ore residue al valore contrattuale (usato ad ogni nuovo tentativo di abbinamento).</summary>
        public void ResetRemainingHours()
        {
            RemainingHours = ContractHours;
        }

        /// <summary>Assegna al docente le preferenze dichiarate sull'ordine delle discipline.</summary>
        public void SetPreferences(TeacherPreferences preferences)
        {
            Preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        }

        public override string ToString() => $"Nome: {Name} Ore: {ContractHours}";
    }
}
