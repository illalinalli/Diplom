namespace HomelessAnimalsDiplom.Models
{
    public class Node
    {
        public string Name { get; set; }
        public Item Item { get; set; }
        public Node Parent { get; set; }  // Добавлено поле для родителя
        public List<Node> Children { get; set; }

        public Node(string name)
        {
            Name = name;
            Children = new List<Node>();
        }
        public Node() { }
        public Node(Item item, string name, Node parent)
        {
            Name = name;
            Item = item;
            Children = new List<Node>();
            Parent = parent;
        }
    }
}
