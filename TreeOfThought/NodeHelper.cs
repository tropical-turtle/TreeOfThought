using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Tree_Of_Thought;

namespace TreeOfThought
{
    public class NodeHelper
    {
        TreeView treeView;
        RichTextBox richTexBox;
        ColorDialog colorDialog;
        FontDialog fontDialog;
        Form mainForm;
        Color defaultColor = Color.FromArgb(0);
        FileState fileState;
        TreeNode nodeToCopy;


        #region-- Xml Document to Treeview

        public void PlotXmlOnTreeView(string xmlFilePath, TreeView treeView)
        {

            var rootNodes = XmlFileToTreeNodes(xmlFilePath);

            foreach (TreeNode treeNode in rootNodes)
            {
                treeView.Nodes.Add(treeNode);
            }
        }
        #endregion---


        #region-- Following part is for getting TreeNodes from Xml file or XmlNodes---

        public TreeNode XmlNodeToTreeNode(XmlNode xmlNode)
        {

            TreeNode treeNode = new TreeNode();

            // Text of an xml-Node is considered as a Child-Node
            if (xmlNode.FirstChild != null && xmlNode.FirstChild.NodeType == XmlNodeType.Text)
                treeNode.Text = xmlNode.FirstChild.Value;
            if (xmlNode.Attributes != null )
            {
                if(xmlNode.Attributes["Tip"] != null)
                {
                    treeNode.ToolTipText = xmlNode.Attributes["Tip"].Value;
                }

                if(xmlNode.Attributes["Rtf"] != null)
                {
                    NodeInfo nodeInfo = new NodeInfo();
                    nodeInfo.Rtf=xmlNode.Attributes["Rtf"].Value;
                    treeNode.Tag = nodeInfo;
                }  //Color

                if (xmlNode.Attributes["Color"] != null)
                {
                    string rgbValue= xmlNode.Attributes["Color"].Value;
                    Color node_foreColor = Color.FromArgb(Int32.Parse( rgbValue));

                    treeNode.ForeColor= node_foreColor;
                    NodeInfo nodeInfo = new NodeInfo();
                    nodeInfo.Rtf = xmlNode.Attributes["Rtf"].Value;
                    treeNode.Tag = nodeInfo;
                }  //Color

            }


            foreach (XmlNode childXmlNode in xmlNode.ChildNodes)
            {
                if (childXmlNode.NodeType == XmlNodeType.Element)
                {
                    var childTreeNode = XmlNodeToTreeNode(childXmlNode);
                    treeNode.Nodes.Add(childTreeNode);
                }
            }

            return treeNode;
        }


        public List<TreeNode> XmlFileToTreeNodes(string filePath)
        {
            var document = new XmlDocument();
            List<TreeNode> treeNodes = null;
            try
            {
                document.Load(filePath);
                treeNodes = new List<TreeNode>();


                foreach (XmlNode xmlNode in document.ChildNodes)
                {
                    var treeNode = XmlNodeToTreeNode(xmlNode);
                    treeNodes.Add(treeNode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured:  " + ex.Message);
            }

            return treeNodes;
        }

        #endregion--- end of XmlNode to TreeNodes---


        #region --TreeView /TreeNode to  XmlNode/XmlDocument

        public XmlNode TreeNodeToXmlNode(TreeNode treeNode, XmlDocument document)
        {
            var xmlElement = document.CreateElement("Element");
            var tipAttribute = document.CreateAttribute("Tip");
            var richTextAttribute = document.CreateAttribute("Rtf");


            xmlElement.InnerXml = treeNode.Text;  // let's keep it this way

            tipAttribute.Value = ((NodeInfo)treeNode.Tag).TipText;
            richTextAttribute.Value = ((NodeInfo)treeNode.Tag).Rtf;
            xmlElement.Attributes.Append(tipAttribute);
            xmlElement.Attributes.Append(richTextAttribute);

 
            if (treeNode.ForeColor.ToArgb() != defaultColor.ToArgb())
            {
                var colorAttribute = document.CreateAttribute("Color");
                colorAttribute.Value = treeNode.ForeColor.ToArgb().ToString();
                xmlElement.Attributes.Append(colorAttribute);
            }


            if (treeNode.Nodes.Count > 0)
            {
                foreach (TreeNode childTreeNode in treeNode.Nodes)
                {
                    var childXmlElement = TreeNodeToXmlNode(childTreeNode, document);
                    xmlElement.AppendChild(childXmlElement);
                }
            }

            return xmlElement;
        }

        public XmlDocument TreeViewToXmlDocument(TreeView treeView)
        {
            XmlDocument document = new XmlDocument();
            foreach (TreeNode treeNode in treeView.Nodes)
            {
                var xmlNode = TreeNodeToXmlNode(treeNode, document);
                document.AppendChild(xmlNode);
            }

            return document;
        }

        #endregion



        #region -- Context Menu

        #region -- TreeView Menu Initialization
        public void InitializeContextMenuForTreeView(TreeView treeView_ , ColorDialog colorDialog_, Form mainForm_, FileState fileState_)
        {
            this.treeView = treeView_;
            this.colorDialog = colorDialog_;
            this.mainForm = mainForm_;
            this.fileState = fileState_;
            this.nodeToCopy = null;


            MenuItem[] menuItem = new MenuItem[10];
            menuItem[0] = new MenuItem("Add New Node...", Add_New_Node);
            menuItem[1] = new MenuItem("Edit Node...", Edit_Node);
            menuItem[2] = new MenuItem("Set Color...", Set_Color);
            menuItem[3] = new MenuItem("Arrange Child Nodes...", Arrange_Child_Nodes);

            menuItem[4] = new MenuItem("-");
            menuItem[5] = new MenuItem("Copy Node", Copy_Node);
            menuItem[6] = new MenuItem("Paste As Child Node", Paste_Node);

            menuItem[7] = new MenuItem("DELETE  Node...", Delete_Node);
            menuItem[8] = new MenuItem("-");
            menuItem[9] = new MenuItem("Add Top Level Node...", Add_Top_Level_Node);

            ContextMenu contextMenu = new ContextMenu(menuItem);
            contextMenu.MenuItems[9].Enabled = false;  // temporarily dis-allowed. TODO: fix, currently xmlDocument does not allow multiple documentElement.
            // change your strategy for saving nodes.

            treeView.ContextMenu = contextMenu;



        }


        void Add_New_Node(object sender, EventArgs e)
        {

            var selectedNode = this.treeView.SelectedNode;
            NodeForm nodeForm = new NodeForm(mainForm, treeView, false, false);
            nodeForm.Show();
            nodeForm.Location= Cursor.Position;
            fileState.IsFileChanged = true;

        }


        void Add_Top_Level_Node(object sender, EventArgs e)
        {
            NodeForm nodeForm = new NodeForm(mainForm, treeView, false,true);
            nodeForm.Show();
            Point newFormLocation = new Point(Cursor.Position.X - 100, Cursor.Position.Y - 100);
            nodeForm.Location = newFormLocation;
            fileState.IsFileChanged = true;
        }


        void Set_Color(object sender, EventArgs e)
        {

            var selectedNode = treeView.SelectedNode;
            if (selectedNode != null)
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    treeView.SelectedNode.ForeColor = colorDialog.Color;
                    fileState.IsFileChanged = true;
                }
            }
            else
            {
                MessageBox.Show("There are no nodes created yet ");
            }


        }

        void Edit_Node(object sender, EventArgs e)
        {
            var selectedNode = treeView.SelectedNode;
            if(selectedNode !=null)
            {
                NodeForm nodeForm = new NodeForm(mainForm, treeView, true,false);
                nodeForm.Show();
                nodeForm.Location = Cursor.Position;
                fileState.IsFileChanged = true;
            }
            else
            {
                MessageBox.Show("There are no nodes created yet ");
            }


        }

        void Arrange_Child_Nodes(object sender, EventArgs e)
        {
            var selectedNode = treeView.SelectedNode;
            if(selectedNode != null)
            {
                var nodeForm = new ArrangeChildNodesForm(selectedNode, fileState);

                nodeForm.Show();
                nodeForm.Location = Cursor.Position;
            }
            else
            {
                MessageBox.Show("No node selected !");
            }

        }

        void Copy_Node(object sender, EventArgs e)
        {
            var selectedNode = treeView.SelectedNode;
            if(selectedNode != null)
            {
                nodeToCopy =(TreeNode) selectedNode.Clone();
            }
            else
            {
                MessageBox.Show("No node selected for copying");
            }

        }

        void Paste_Node(object sender, EventArgs e)
        {
            var selectedNode = treeView.SelectedNode;
            if(nodeToCopy != null)
            {
                selectedNode.Nodes.Add(nodeToCopy);
                treeView.SelectedNode = nodeToCopy;
                fileState.IsFileChanged = true;
            }
            else
            {
                MessageBox.Show("No node copied yet for pasting");
            }

        }

        void Delete_Node(object sender, EventArgs e)
        {
            if(treeView.SelectedNode != null)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to DELETE the node?", "Confirmation", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    treeView.Nodes.Remove(treeView.SelectedNode);
                    richTexBox.Rtf = null;
                    fileState.IsFileChanged = true;
                }
                else if (dialogResult == DialogResult.No)
                {       
                }
            }
            else
            {
                MessageBox.Show("No node selected for deleting");
            }

        }

        #endregion-- End of TreeView context menu initialization.-----


        #region-- Rich Textbox menu initialization---
        public void InitializeContextMenuForRichText(RichTextBox richTextbox_, ColorDialog colorDialog_, Form mainForm_ ,FontDialog fontDialog_, FileState fileState_)
        {
            richTexBox = richTextbox_;
            this.colorDialog = colorDialog_;
            this.fontDialog = fontDialog_;
            this.mainForm = mainForm_;
            this.fileState = fileState_;

            MenuItem[] menuItem = new MenuItem[11];
            menuItem[0] = new MenuItem("Set Font...", Set_Font_ForRichText);
            menuItem[1] = new MenuItem("Set Text Color...", Set_TextColor_ForRichText);
            menuItem[2] = new MenuItem("Highlight...", Set_Highlight_Color);

            menuItem[3] = new MenuItem("-", Set_Highlight_Color);

            menuItem[4] = new MenuItem("Align To Right", Align_To_Right_ForRichText);
            menuItem[5] = new MenuItem("Align To Left", Align_To_Left_ForRichText);
            menuItem[6] = new MenuItem("Align To Centre", Align_To_Centre_ForRichText);

            menuItem[7] = new MenuItem("-", Set_Highlight_Color);

            menuItem[8] = new MenuItem("Copy", Copy_ForRichText);
            menuItem[9] = new MenuItem("Paste", Paste_ForRichText);
            menuItem[10] = new MenuItem("Cut", Cut_ForRichText);


            ContextMenu contextMenu = new ContextMenu(menuItem);
            richTextbox_.ContextMenu = contextMenu;



        }


        void Set_TextColor_ForRichText(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                richTexBox.SelectionColor= colorDialog.Color;
                fileState.IsFileChanged = true;
            }
        }

        void Set_Highlight_Color(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                richTexBox.SelectionBackColor = colorDialog.Color;
                fileState.IsFileChanged = true;
            }
        }


        void Set_Font_ForRichText(object sender, EventArgs e)
        {
            if(fontDialog.ShowDialog()==DialogResult.OK)
            {
                richTexBox.SelectionFont = fontDialog.Font;
                fileState.IsFileChanged = true;
            }
        }

        void Copy_ForRichText(object sender, EventArgs e)
        {
            richTexBox.Copy();
        }

        void Paste_ForRichText(object sender, EventArgs e)
        {
            richTexBox.Paste();
            fileState.IsFileChanged = true;
        }

        void Cut_ForRichText(object sender, EventArgs e)
        {
            richTexBox.Cut();
            fileState.IsFileChanged = true;
        }

        void Align_To_Left_ForRichText(object sender, EventArgs e)
        {
            richTexBox.SelectionAlignment = HorizontalAlignment.Left;
            fileState.IsFileChanged = true;
        }
        void Align_To_Right_ForRichText(object sender, EventArgs e)
        {
            richTexBox.SelectionAlignment = HorizontalAlignment.Right;
            fileState.IsFileChanged = true;
        }
        void Align_To_Centre_ForRichText(object sender, EventArgs e)
        {
            richTexBox.SelectionAlignment = HorizontalAlignment.Center;
            fileState.IsFileChanged = true;
        }


        #endregion--- end of Rich Textbox menu initialization---


        

        #endregion-- end of Context Menu



        public void SaveFileDialog( ref string filePath, TreeView treeView_ ,Form mainForm_, FileState fileState_, bool isSaveAs )
        {
            if(isSaveAs )
            {
                filePath = null;
            }

            if (filePath == null)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "rexl files |*.rexl"; 
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.ShowDialog();

                if (saveFileDialog1.FileName != "")
                {
                    filePath = saveFileDialog1.FileName;
                    try
                    {
                        NodeHelper helper = new NodeHelper();
                        var xmlDocument = helper.TreeViewToXmlDocument(treeView_);
                        xmlDocument.Save(filePath);
                        mainForm_.Text = "Tree of Thought     " + Path.GetFileNameWithoutExtension(filePath);
                        fileState_.IsFileChanged = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else  // over write the currently openned file
            {
                try
                {
                    NodeHelper helper = new NodeHelper();
                    var xmlDocument = helper.TreeViewToXmlDocument(treeView_);
                    xmlDocument.Save(filePath);
                    fileState_.IsFileChanged = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            this.fileState = fileState_;
        }


    }


}

