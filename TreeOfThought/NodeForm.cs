using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TreeOfThought
{
    public partial class NodeForm : Form
    {
        private string nodeText;
        public string nodeInfo;
        bool isEdit;
        bool isAddTopLevelNode;


        public Form mainForm;
        TreeNode selectedNode;
        TreeView treeView;
        public NodeForm()
        {
            InitializeComponent();
        }


        public NodeForm(Form mainForm_, TreeView treeView_, bool isEdit_, bool isAddTopLevelNode_) 
        {
            this.mainForm = mainForm_;
            this.treeView = treeView_;
            this.selectedNode = treeView.SelectedNode;
            InitializeComponent();

            this.isAddTopLevelNode = isAddTopLevelNode_;

            this.isEdit = isEdit_;
            if(isEdit )
            {
                textBoxItemText.Text= selectedNode.Text;
                textBoxTip.Text = selectedNode.Text;
                richTextBoxOnEdit.Rtf = ((NodeInfo)selectedNode.Tag).Rtf;
            
            }

            this.AcceptButton = buttonSave;
        }


        private void buttonSave_Click(object sender, EventArgs e)
        {
             NodeInfo nodeInfo = new NodeInfo();
             nodeInfo.Rtf= richTextBoxOnEdit.Rtf;
            

            if (isEdit)
            {
                selectedNode.Text = textBoxItemText.Text;
                selectedNode.ToolTipText = textBoxTip.Text;
                selectedNode.Tag = nodeInfo;
                treeView.SelectedNode = selectedNode;

            }
            else
            {

                if(isAddTopLevelNode)
                {
                    this.selectedNode = new TreeNode();
                    selectedNode.Text = textBoxItemText.Text;
                    selectedNode.ToolTipText = textBoxTip.Text;

                    selectedNode.Tag = nodeInfo;
                    treeView.Nodes.Add(selectedNode);
                    treeView.SelectedNode = selectedNode;

                }
                else
                {
                    if(selectedNode != null)
                    {
                        var node = new TreeNode();
                        node.Text= textBoxItemText.Text;
                        node.ToolTipText = textBoxTip.Text;

                        node.Tag = nodeInfo;
                        selectedNode.Nodes.Add(node);
                        treeView.SelectedNode = node;
                    }
                    else
                    {
                        this.selectedNode = new TreeNode();
                        selectedNode.Text = textBoxItemText.Text;
                        selectedNode.ToolTipText= textBoxTip.Text; 

                        selectedNode.Tag = nodeInfo;
                        treeView.Nodes.Add(selectedNode);
                        treeView.SelectedNode = selectedNode;

                    }
                }

            }


            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private Control FindRichTextBox(Form form)
        {

            return null;
        }

        private void NodeForm_SizeChanged(object sender, EventArgs e)
        {
            //Point save_button_location = new Point();
            //save_button_location.X = this.Size.Width - 180;
            //save_button_location.Y=this.Size.Height - 110;

            //Point cancel_button_location = new Point();

            //cancel_button_location.Y = save_button_location.Y;
            //cancel_button_location.X = save_button_location.X - 157;

            //buttonCancel.Location = cancel_button_location;
            //buttonSave.Location = save_button_location;

            //Size size = new Size();
            //size.Width=this.Size.Width - 254;
            //size.Height = this.Size.Height - 290;
            //richTextBoxOnEdit.Size = size;
        }
    }
}
