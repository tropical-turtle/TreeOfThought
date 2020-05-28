using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tree_Of_Thought;

namespace TreeOfThought
{
    public partial class mainForm : Form
    {
        string filePath = null;
        TreeNode selectedNode;
        FileState fileState;
        float original_zoomFactor;
        float zoomFactor_smallesValue = 0.015625F;
        int trackbar_mid_value = 10;
        public mainForm()
        {
            InitializeComponent();
            fileState = new FileState();

            NodeHelper helper = new NodeHelper();
            helper.InitializeContextMenuForTreeView(treeView, colorDialog1, this, fileState);

            helper.InitializeContextMenuForRichText(richTextBoxNote, colorDialog1, this, fontDialog1, fileState);

            // enableAutoDragDop property is disabled. do not use RichTextBox's drag drop.
            //It has an inherant problem, when the file being dragged is a bit bigger it get trouble.
            // We should use TreeView's drag drop property.
            this.richTextBoxNote.DragDrop += new System.Windows.Forms.DragEventHandler(this.richTextBoxNote_DragDrop);
            treeView.AllowDrop = true;

            //To make treeView drag drop well, we need to some work at Drag eneter event.
            treeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView_DragEnter);
            treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);


            //


            original_zoomFactor = richTextBoxNote.ZoomFactor;

            int tracbar_x = this.Width - trackBar1.Width;
            var tracbarCoordinate = new Point(tracbar_x, 2);
            trackBar1.Location = tracbarCoordinate;

            trackBar1.Value = trackbar_mid_value;
        }

        private void richTextBoxNote_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if(fileNames != null)
            {
                string filePath = fileNames[0];

                treeView.Nodes.Clear();


                if (filePath != "")
                {
                    NodeHelper helper = new NodeHelper();

                    try
                    {
                        helper.PlotXmlOnTreeView(filePath, treeView);
                        this.Text = "Tree of Thought     " + Path.GetFileNameWithoutExtension(filePath);
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message + "\r\n" +
                                         ex.InnerException;
                        MessageBox.Show(message);
                    }
                }
                //richTextBoxNote.Clear();
            }

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "rexl files |*.rexl"; 
            fileDialog.ShowDialog();
            filePath = fileDialog.FileName;

            treeView.Nodes.Clear();

            if (filePath != "")
            {
                NodeHelper helper = new NodeHelper();

                try
                {
                    helper.PlotXmlOnTreeView(filePath, treeView);
                    this.Text = "Tree of Thought     " + Path.GetFileNameWithoutExtension(filePath);
                }
                catch (Exception ex)
                {
                    string message = ex.Message + "\r\n" +
                                     ex.InnerException;
                    MessageBox.Show(message);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // save the content of richtextbox 
            if (selectedNode != null)
            {
                var curruntNodeInfo = (NodeInfo)selectedNode.Tag;
                curruntNodeInfo.Rtf = richTextBoxNote.Rtf;
                treeView.SelectedNode.Tag = curruntNodeInfo;
            }

            if (treeView.Nodes.Count > 0)
            {
                NodeHelper helper = new NodeHelper();
                helper.SaveFileDialog(ref filePath, treeView, this, fileState, false);
            }
            else
            {
                MessageBox.Show("There is nothing to save ! ");
            }

            //if (filePath == null)
            //{
            //    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //    saveFileDialog1.Filter = "rexl files |*.rexl"; ;
            //    saveFileDialog1.FilterIndex = 1;
            //    saveFileDialog1.RestoreDirectory = true;
            //    saveFileDialog1.ShowDialog();

            //    if (saveFileDialog1.FileName !="")
            //    {
            //        try
            //        {
            //            NodeHelper helper = new NodeHelper();
            //            var xmlDocument = helper.TreeViewToXmlDocument(treeView);
            //            xmlDocument.Save(filePath);
            //            this.Text = "Tree of Thought     " + Path.GetFileNameWithoutExtension(filePath);
            //            fileState.IsFileChanged = false;
            //        }
            //        catch(Exception ex)
            //        {
            //            MessageBox.Show(ex.Message);
            //        }
            //    }
            //}
            //else  // over write the currently openned file
            //{
            //    NodeHelper helper = new NodeHelper();
            //    var xmlDocument=helper.TreeViewToXmlDocument(treeView);
            //    xmlDocument.Save(filePath);
            //}
        }


        private void NewMenuItem_Click(object sender, EventArgs e)
        {
            treeView.Nodes.Clear();
            richTextBoxNote.Clear();
            richTextBoxNote.ReadOnly = false;
            this.Text = "Tree of Thought";
            filePath = null;
            fileState.IsFileChanged = true; 
        }


        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                if( selectedNode !=null) 
                    selectedNode.BackColor = Color.FromArgb(-31);
                if (treeView.SelectedNode.Tag != null)
                {
                    richTextBoxNote.Rtf = ((NodeInfo)treeView.SelectedNode.Tag).Rtf;
                }
                selectedNode = treeView.SelectedNode;

                selectedNode.BackColor = Color.RoyalBlue;

                trackBar1.Value = 10; //resets trackbar value to the middle
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // save the content of richtextbox 
            if (selectedNode != null)
            {
                var curruntNodeInfo = (NodeInfo)selectedNode.Tag;
                curruntNodeInfo.Rtf = richTextBoxNote.Rtf;
                treeView.SelectedNode.Tag = curruntNodeInfo;
            }

            if (treeView.Nodes.Count> 0)
            {
                NodeHelper helper = new NodeHelper();
                helper.SaveFileDialog(ref filePath, treeView, this, fileState, true);
            }
            else
            {
                MessageBox.Show("There is nothing to save ! ");
            }



            //SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //saveFileDialog1.Filter = "rexl files |*.rexl";
            //saveFileDialog1.FilterIndex = 1;
            //saveFileDialog1.RestoreDirectory = true;
            //saveFileDialog1.ShowDialog();

            //if (saveFileDialog1.FileName != "")
            //{
            //    filePath = saveFileDialog1.FileName;
            //    try
            //    {
            //        NodeHelper helper = new NodeHelper();
            //        var xmlDocument = helper.TreeViewToXmlDocument(treeView);
            //        xmlDocument.Save(filePath);
            //        this.Text = "Tree of Thought     " + Path.GetFileNameWithoutExtension(filePath);
            //        fileState.IsFileChanged = false;
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}
        }

        private void richTextBoxNote_Leave(object sender, EventArgs e)
        {
            var selectedNode = treeView.SelectedNode;

            if(selectedNode != null)
            {
                var curruntNodeInfo =(NodeInfo) selectedNode.Tag;
                curruntNodeInfo.Rtf=richTextBoxNote.Rtf;
                treeView.SelectedNode.Tag = curruntNodeInfo;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm helpForm = new HelpForm();
            helpForm.Show();
            helpForm.Location = Cursor.Position;

        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.Show();
            aboutForm.Location = Cursor.Position;
        }



        #region---Test Context Menu
        private void richTextBoxNote_KeyPress(object sender, KeyPressEventArgs e)
        {
            fileState.IsFileChanged = true;
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(fileState.IsFileChanged && treeView.Nodes.Count>0)
            {

                DialogResult result = MessageBox.Show("Do you want to save your file?", "Confirm Changes ",MessageBoxButtons.YesNo);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    NodeHelper helper = new NodeHelper();
                    helper.SaveFileDialog(ref filePath, treeView, this, fileState, false);
                }
            }
        }

        private void richTextBoxNote_MouseDown(object sender, MouseEventArgs e)
        {
            // only checking for the right click of mouse down
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var currentSelection = richTextBoxNote.SelectedRtf;

                if (richTextBoxNote.SelectedText == "")
                {
                    DisableFontRelatedMenus();
                }


                if (richTextBoxNote.SelectedText == "" && SelectionDoesNotContainsImage())
                {  // nothing is selected , so we should dis-allow copy and cut
                    DisableCopyAndCut();

                    // we also need to dis-allow alignment related menus
                    DisableAlignmentRelatedMenus();
                }

                if(IsClipboardEmpty())
                {
                    DisableMenuItem(9);
                }

            }

        }

        private void richTextBoxNote_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                EnableRichTextMenuItems();
            }
            
        }

        private void EnableRichTextMenuItems()
        {
            foreach (MenuItem item in richTextBoxNote.ContextMenu.MenuItems)
            {
                item.Enabled = true;
            }

        }


        private void DisableCopyAndCut()
        {
            DisableMenuItem(8);
            DisableMenuItem(10);
        }

        private void DisableFontRelatedMenus()
        {
            DisableMenuItem(0);
            DisableMenuItem(1);
            DisableMenuItem(2);
        }

        private void DisableAlignmentRelatedMenus()
        {
            DisableMenuItem(4);
            DisableMenuItem(5);
            DisableMenuItem(6);
        }

        private void DisableMenuItem(int item_order)
        {
            richTextBoxNote.ContextMenu.MenuItems[item_order].Enabled = false;
        }

        private bool SelectionDoesNotContainsImage()
        {
            bool doesNotContainsImage = true;

            if (richTextBoxNote.SelectedRtf.Contains("{\\pict\\"))
            {
                doesNotContainsImage = false;
            }
            return doesNotContainsImage;
        }

       

        private bool IsClipboardEmpty()
        {
            bool isEmpty = true;
            var clipboard_data = Clipboard.GetDataObject();
            var data_formats = clipboard_data.GetFormats();
            var number_of_data_formats = data_formats.Count();

            if(number_of_data_formats > 0)
            {
                isEmpty = false;
            }

            return isEmpty;
        }


        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBoxNote.ZoomFactor = richTextBoxNote.ZoomFactor + 1;
            richTextBoxNote.WordWrap = false;
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(richTextBoxNote.ZoomFactor > 1)
            {
                richTextBoxNote.ZoomFactor = richTextBoxNote.ZoomFactor - 1;
            }
            else
            {
                richTextBoxNote.ZoomFactor = (richTextBoxNote.ZoomFactor) / 2;
            }
            richTextBoxNote.WordWrap = true;
        }

        private void fitActualSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBoxNote.ZoomFactor = original_zoomFactor;
            richTextBoxNote.WordWrap = true;
            trackBar1.Value = trackbar_mid_value;
        }

        private void mainForm_SizeChanged(object sender, EventArgs e)
        {
            int tracbar_x = (this.Width - trackBar1.Width)-20;
            var tracbarCoordinate = new Point(tracbar_x, 2);
            trackBar1.Location = tracbarCoordinate;
        }

        private void zoomWithZoomingBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(trackBar1.Visible)
            {
                trackBar1.Visible = false;
            }
            else
            {
                trackBar1.Visible = true;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

            var bar_value=trackBar1.Value;

            if(bar_value >10)
            {
                richTextBoxNote.ZoomFactor = richTextBoxNote.ZoomFactor + 1;
                richTextBoxNote.WordWrap = false;
            }
            else if(bar_value<10)
            {
                richTextBoxNote.WordWrap = true;
                if (richTextBoxNote.ZoomFactor > 1)
                {
                    richTextBoxNote.ZoomFactor = richTextBoxNote.ZoomFactor - 1;
                }
                else if(richTextBoxNote.ZoomFactor > zoomFactor_smallesValue *2)
                {
                    richTextBoxNote.ZoomFactor = (richTextBoxNote.ZoomFactor) / 2;
                }
            }
            else
            {
                richTextBoxNote.ZoomFactor = original_zoomFactor;
                richTextBoxNote.WordWrap = true;

            }


        }

        private void treeView_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (fileNames != null)
            {
                string filePath = fileNames[0];

                treeView.Nodes.Clear();


                if (filePath != "")
                {
                    NodeHelper helper = new NodeHelper();

                    try
                    {
                        helper.PlotXmlOnTreeView(filePath, treeView);
                        this.Text = "Tree of Thought     " + Path.GetFileNameWithoutExtension(filePath);
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message + "\r\n" +
                                         ex.InnerException;
                        MessageBox.Show(message);
                    }
                }
                //richTextBoxNote.Clear();
            }


        }

        private void treeView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }
    }



    #endregion

}

