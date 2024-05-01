namespace HomelessAnimalsDiplom.Models
{
    public class Node
    {
        public string Name { get; set; }
        public Node Parent { get; set; }  // Добавлено поле для родителя
        public List<Node> Children { get; set; }

        public Node(string name)
        {
            Name = name;
            Children = new List<Node>();
        }
    }
}
