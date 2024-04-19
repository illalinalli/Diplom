namespace HomelessAnimalsDiplom.Models
{
    public abstract class Publication
    {
        public string Name { get; set; }
        public string[] Parents { get; set; }

        protected Publication()
        {
            Name = "";
            Parents = new string[0];
        }

        public override string ToString() => Name;
    }
}
