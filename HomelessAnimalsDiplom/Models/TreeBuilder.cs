using Accord.Collections;
using static HomelessAnimalsDiplom.Models.Item;
namespace HomelessAnimalsDiplom.Models
{
    public class TreeBuilder
    {
        public List<Item> GetCats(List<Item> items)
        {
            Item item_ = new();
            var allCats = item_.GetAllCats();
            List<Item> res = new();
            items.ForEach(item =>
            {
                allCats.ForEach(cat =>
                {
                    if (item.Id == cat.Id)
                    {
                        res.Add(item);
                    }
                });
            });
            return res;
        }
        public Node BuildTree(List<Item> items)
        {
            Node root = new Node("Животные");
            SetParentReferences(root, null);

            // Разделение на котов и собак
            Node catsNode = new Node("Кот");
            Node dogsNode = new Node("Собака");
            SetParentReferences(catsNode, root);
            SetParentReferences(dogsNode, root);
            root.Children.Add(catsNode);
            root.Children.Add(dogsNode);
            var allCats = GetCats(items);

            var allDogs = items.Except(allCats);

            foreach (var item in items)
            {
                var animalType = item.GetAnimalType();
                Node typeNode = animalType.Name == "Кот" ? catsNode : dogsNode;

                // Разделение по размеру
                Node sizeNode = FindOrCreateChild(typeNode, item.GetBreedSize().Name);
                SetParentReferences(sizeNode, typeNode);
                // Разделение по окрасу
                Node colorsNode = FindOrCreateChild(sizeNode, "Окрас");
                SetParentReferences(colorsNode, sizeNode);

                foreach (var color in item.Colors)
                {
                    Node colorNode = FindOrCreateChild(colorsNode, color.Name);
                    SetParentReferences(colorNode, colorsNode);
                    // Разделение по породе
                    Node breedNode = FindOrCreateChild(colorNode, item.GetBreed().Name);
                    SetParentReferences(breedNode, colorNode);
                    // Добавление элемента в соответствующий узел
                    breedNode.Children.Add(new Node(item.Title));
                }
            }

            return root;
        }
        private void SetParentReferences(Node node, Node parent)
        {
            node.Parent = parent;
            foreach (var child in node.Children)
            {
                SetParentReferences(child, node);
            }
        }
        private Node FindOrCreateChild(Node parent, string name)
        {
            var child = parent.Children.Find(c => c.Name == name);
            if (child == null)
            {
                child = new Node(name);
                parent.Children.Add(child);
            }
            return child;
        }
    }
}
