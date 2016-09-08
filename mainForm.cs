using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CPE415_Project
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

     

        Tree _tree = new Tree();

        private void Form1_Load(object sender, EventArgs e)
        {
            /*
                Sorry for lazy code,
                I haven't got time to write optimized algorithms 
                for tree population on the component.
            */
            var nodes = _tree.ReadTree();

            List<TreeNode> tnodes = new List<TreeNode>();

            foreach(var asd in nodes)
            {
                var x = new TreeNode(asd.Name);
                x.Tag = asd.HeruisticValue;
                tnodes.Add(x);
            }


            foreach(var xnode in nodes)
            {

                foreach(var tnode in tnodes)
                {
                    if(tnode.Text == xnode.Name)
                    {
                        foreach(var subxnode in xnode.Nodes)
                        {
                            foreach(var renode in tnodes)
                            {
                                if (subxnode.Name == renode.Text)
                                    tnode.Nodes.Add(renode);
                            }
                        }
                    }
                }
            
            }

            foreach (var asd in tnodes)
                asd.Text += "(" + asd.Tag.ToString() + ")";


            treeView.Nodes.Add(tnodes[0]);
            treeView.ExpandAll();

        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            Tree t = _tree.Clone();
            t.GreedySearch(textBox1.Text.ToUpper());
            tbVisited.Text = t.VisitedNodes;
            tbPathCost.Text = t.TotalPathCost.ToString().ToUpper();
            tbResult.Text = t.Path;
        }
    }
}
