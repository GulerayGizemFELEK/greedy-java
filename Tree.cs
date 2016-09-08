using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CPE415_Project
{
    [Serializable]
    public class Node
    {
        public List<Node> Nodes = new List<Node>();
        public string Name { get; private set; }
        public void AddNode(Node p) {
            Nodes.Add(p);
            p.SetParent(this);
        }
        public void SetParent(Node p) { Parent = p; }
        public Node Parent { get; private set; }

        /* Between parent and this */
        public int PathCost { get; set; }
        public int HeruisticValue { get; private set; }
        public Node(string name,int heruisticValue)
        {
            Name = name;
            HeruisticValue = heruisticValue;
        }


        public bool Visited = false;
        public bool VisitedAllNodes = false;

    }
    [Serializable]
    public class Tree
    {
        public String Path { get; set; }
        public String VisitedNodes { get; set; }

        public int TotalPathCost { get; set; }
        public Tree Clone()
        {
            object clonedObject = null;
            BinaryFormatter formatter = new BinaryFormatter();
            using (Stream stream = new MemoryStream())
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                clonedObject = formatter.Deserialize(stream);
            }

            return (Tree)clonedObject;
        }
        public Node Root { get; set; }

        public List<Node> ReadTree()
        {
            
            List<Node> nodlar = new List<Node>();
            using (FileStream fs = new FileStream("heuristic.txt", FileMode.Open))
            {
                if (fs.CanRead)
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string okunan;
                        while ((okunan = sr.ReadLine()) != null)
                        {
                            if (okunan == string.Empty)
                                continue;
                            string[] ayrilan = okunan.Split(' ');
                            int heuristic = Convert.ToInt32(ayrilan[1]);

                            Node yeniNod = new Node(ayrilan[0], heuristic);
                            nodlar.Add(yeniNod);
                        }
                    }
                }
            }

            using (FileStream fs = new FileStream("tree.txt", FileMode.Open))
            {
                if (fs.CanRead)
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string okunan;
                        while ((okunan = sr.ReadLine()) != null)
                        {
                            if (okunan == string.Empty)
                                continue;
                            string[] ayrilan = okunan.Split(' ');
                            string parent, child,cost;
                            parent = ayrilan[0];
                            child = ayrilan[1];
                            int icost = Convert.ToInt32(ayrilan[2]);
                            /*Name = A , Heuristic = 60
                              Name =  B 25
                                Q 20
                                C 30
                                D 50
                                E 35
                                F 20
                                H 15
                                K 10
                                G 0*/
                            foreach (Node n in nodlar)
                            {
                                if(n.Name == parent)
                                {
                                    foreach(Node c in nodlar)
                                    {
                                        if(c.Name == child)
                                        {
                                            n.AddNode(c);
                                            c.PathCost = icost;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }

            Root = nodlar[0];

            foreach(Node n in nodlar)
            {
                Trace.WriteLine(string.Format("Node {0}, Heruistic value is {1}", n.Name, n.HeruisticValue));
                Trace.WriteLine(string.Format("Has {0} child(ren)", n.Nodes.Count));
                foreach(Node c in n.Nodes)
                {
                    Trace.WriteLine("\t Child node name {0}", c.Name);

                }
            }

            return nodlar;
        }


   


        public void GreedySearch(string destinationName)
        {
            /// Şuan bulunduğumuz nodu ağacın kök noduna eşitle (başlangıç noktası)
            Node currentNode = Root;
            /// Hedef noktasına giden yolu tutacak stack
            var destinationPath = new Stack<Node>();
            /// Bütün gezilen yolları tutacak stack
            var fullPath = new Stack<Node>();
            for (;;)
            {
                /// Şuanki bulunduğumuz nodu yola ekle
                destinationPath.Push(currentNode);
                parent_return:;
                /// Şuanki bulunduğumuz nodu gezilen nodlar stackına ekle
                fullPath.Push(currentNode);
                currentNode.Visited = true;

                Trace.WriteLine("node : {0}", currentNode.Name);

                /// Hedef noda varıldı mı?
                if(currentNode.Name.CompareTo(destinationName) == 0)
                {
                    Trace.WriteLine("Destination found");
                
                    break;
                }
                /// Şuan bulunduğumuz nodun alt nodları arasından seçilen nod
                Node selectedNode = null;
                /// Bütün alt nodlar için
                foreach(Node n in currentNode.Nodes)
                {
                    /// Eğer alt nodun altında kalan nodlar gezildiyse
                    if (n.VisitedAllNodes)
                        continue;
                    /// Seçilen nod boşsa, şuankine eşitle
                    if (selectedNode == null)
                        selectedNode = n;
                    else
                    {
                        /// Değilse, heuristic değerlerini karşılaştır ve küçük olanı seç
                        if (selectedNode.HeruisticValue > n.HeruisticValue)
                            selectedNode = n;
                    }
                }
                /// Eğer herhangi bir nod seçilmediyse
                if (selectedNode == null)
                {
                    /// Şuanki bulunduğumuz nodun bütün nodlarını gezildi olarak işaretle
                    currentNode.VisitedAllNodes = true;
                    /// Bir üst noda dön
                    currentNode = currentNode.Parent;
                    destinationPath.Pop();
                    goto parent_return;
                }
                else
                {
                    currentNode = selectedNode;
                    continue;
                }
            } /// (break) döngünün bitişi

            /// Sonucu değişkenlere aktar
            /// 
            while (fullPath.Count > 0)
                VisitedNodes += "," + fullPath.Pop().Name;

            char[] chars = VisitedNodes.ToCharArray();
            Array.Reverse(chars);
            VisitedNodes = new string(chars);


            int totalpathcost = 0;
            while (destinationPath.Count > 0)
            {
                var point = destinationPath.Pop();
                Path += "," + point.Name;
                totalpathcost += point.PathCost;
            }

            TotalPathCost = totalpathcost;

            Trace.WriteLine("{0}", totalpathcost.ToString());

            char[] charss = Path.ToCharArray();
            Array.Reverse(charss);
            Path = new string(charss);



        }

    }
}
