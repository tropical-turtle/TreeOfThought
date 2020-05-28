using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tree_Of_Thought
{
    public partial class ArrangeChildNodesForm : Form
    {
        bool mouseDown;
        Point oldLocation;

        List<Label> labels;
        TreeNode mainNode;
        FileState fileState;

        public ArrangeChildNodesForm()
        {
            InitializeComponent();
        }

        public ArrangeChildNodesForm(TreeNode mainNode_, FileState fileState_)
        {
            if(mainNode_ != null)
            {
                this.mainNode = mainNode_;
                InitializeComponent();
                AddLabels(mainNode_);

                SetButtonLocations();
                this.fileState = fileState_;
            }

        }

        private void label_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            Cursor.Current = Cursors.Hand;
        }

        private void label_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            Cursor.Current = Cursors.Default;
        }

        private void label_MouseMove(object sender, MouseEventArgs e)
        {
            // let's use the change of absolute location of the cursor. and we can add this change to the location of the label

            Point cursor_newLocation = new Point(Cursor.Position.X, Cursor.Position.Y);
            Size movement_Size = locationDifference(oldLocation, cursor_newLocation);  // cursor movement size

            if (mouseDown )
            {               
                Label label = ((Label)sender);
                label.Location = MovingPointByMovementSize(label.Location, movement_Size);
            }
            oldLocation = cursor_newLocation;
            fileState.IsFileChanged = true;
        }


        private void AddLabels(TreeNode node)
        {
            labels = new List<Label>();

            Point location = new Point(100, 50);

            var childNodes= node.Nodes;

            foreach(TreeNode childNode in childNodes)
            {
                var label=AddOneLable(ref location, childNode.Text);
                labels.Add(label);               
            }


            // make window to fit its content
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowOnly;

        }

        private Label AddOneLable(ref Point location, string text)
        {


            Label label = new Label();
            label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label.AutoSize = true;

            label.Location = location;
            label.Name = text;
            label.Size = new System.Drawing.Size(50, 20);
            label.Text = text;

            label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label_MouseDown);
            label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_MouseMove);
            label.MouseUp += new System.Windows.Forms.MouseEventHandler(this.label_MouseUp);


            Point future_Location = new Point();
            future_Location.X = label.Location.X;
            future_Location.Y = location.Y+ label.Size.Height +50;

            location = future_Location;
            //this.Controls.Add(label);  // adding the label to the current form ("this" refers to the current class, which is the form)

            splitContainer1.Panel1.Controls.Add(label);


            return label;

        }

        Size locationDifference(Point oldLocation, Point newLocation)
        {
            Size locationDifference = new Size();

            locationDifference.Width = newLocation.X - oldLocation.X;

            locationDifference.Height = newLocation.Y - oldLocation.Y;


            return locationDifference;
        }

        Point MovingPointByMovementSize(Point oldLocation, Size movementSize)
        {
            Point newLocation = new Point();
            newLocation.X = oldLocation.X + movementSize.Width ;
            newLocation.Y = oldLocation.Y + movementSize.Height ;

            return newLocation;
        }


        private void ArrangeChildNodesForm_SizeChanged(object sender, EventArgs e)
        {
            SetButtonLocations();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            TreeNode  nodeCopy=(TreeNode) this.mainNode.Clone();
            var ordered_lables = new List<Label>();
            ordered_lables = labels.OrderBy(order => order.Location.Y).ToList();
            mainNode.Nodes.Clear();

            foreach (Label label in ordered_lables)
            {
                var rightNode=FindTreeNodeByName(nodeCopy, label.Text);
                mainNode.Nodes.Add(rightNode);
            }

            this.Close();

        }

        TreeNode FindTreeNodeByName(TreeNode parentNode, string givenText)
        {
            TreeNode node=null;

            foreach (TreeNode childNode in parentNode.Nodes)
            {
                if(childNode.Text== givenText)
                {
                    node = (TreeNode)childNode.Clone();
                    break;
                }
            }

            return node;
        }


        private void SetButtonLocations()
        {
            var rectangle_Size = splitContainer1.Panel2.DisplayRectangle;
            buttonSave.Location = new Point(rectangle_Size.Width - 100, rectangle_Size.Height - 100);

            buttonCancel.Location= new Point(rectangle_Size.Width - 200, rectangle_Size.Height - 100);
            buttonSort.Location = new Point(rectangle_Size.Width - 200, rectangle_Size.Height - 150);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonSort_Click(object sender, EventArgs e)
        {
            var sorted_lables=  labels.OrderBy(x => x.Name).ToList();
            splitContainer1.Panel1.Controls.Clear();


            var updated_lables = new List<Label>();
            Point location = new Point(100, 50);

            foreach(var lable in sorted_lables)
            {
                var label_=AddOneLable(ref location, lable.Text);
                updated_lables.Add(label_);
            }

            labels = updated_lables;
            fileState.IsFileChanged = true;
        }
    }
}
