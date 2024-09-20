using System.Xml; 

namespace Ncaa14MixmatchViewer
{
    public partial class Ncaa14MixmatchViewer : Form
    {
        public Ncaa14MixmatchViewer()
        {
            InitializeComponent();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleOpenFile();
        }

        private void AddParts(List<XmlNode> parts, DataGridView dataGrid, bool isJersey = false)
        {
            foreach (XmlNode part in parts)
            {
                string name = part.Attributes["name"].Value;
                XmlNodeList childNodes = part.ChildNodes;
                List<string> presetsUsedIn = new List<string>();
                string bigfile = "";
                string scene = "";
                // vars for special jersey props
                string kerningTable, fontColorTexture, numberColorTexture, nameScaleAtMin, nameMinWidth, nameMaxWidth,
                    numberSpacingOne, uvSet, decalUv, hasBaseLayer;
                kerningTable = fontColorTexture = numberColorTexture = nameScaleAtMin = nameMinWidth = nameMaxWidth =
                    numberSpacingOne = uvSet = decalUv = hasBaseLayer = "";

                // special var for glove animation
                string museAnimation = "";

                foreach (XmlNode child in childNodes)
                {
                    switch (child.Name)
                    {
                        case "bigfile":
                            bigfile = child.Attributes["name"].Value;
                            break;
                        case "scene":
                            scene = child.Attributes["name"].Value;
                            break;
                        case "official":
                            presetsUsedIn.Add(child.Attributes["name"].Value);
                            break;
                        case "kerningTable":
                            kerningTable = child.Attributes["name"].Value;
                            break;
                        case "fontColorTexture":
                            fontColorTexture = child.Attributes["name"].Value;
                            break;
                        case "numberColorTexture":
                            numberColorTexture = child.Attributes["name"].Value;
                            break;
                        case "nameScaleAtMin":
                            nameScaleAtMin = child.Attributes["value"].Value;
                            break;
                        case "nameMinWidth":
                            nameMinWidth = child.Attributes["value"].Value;
                            break;
                        case "nameMaxWidth":
                            nameMaxWidth = child.Attributes["value"].Value;
                            break;
                        case "numberSpacingOne":
                            numberSpacingOne = child.Attributes["value"].Value;
                            break;
                        case "uvSet":
                            uvSet = child.Attributes["value"].Value;
                            break;
                        case "decalUV":
                            decalUv = child.Attributes["value"].Value;
                            break;
                        case "hasBaseLayer":
                            hasBaseLayer = child.Attributes["value"].Value;
                            break;
                        case "MUSEAnimation":
                            museAnimation = child.Attributes["value"].Value;
                            break;
                        default:
                            continue;
                    }
                }
                if (isJersey)
                {

                    dataGrid.Rows.Add(
                        name,
                        bigfile,
                        scene,
                        String.Join(',', presetsUsedIn),
                        part.Attributes["shade"].Value,
                        kerningTable,
                        fontColorTexture,
                        numberColorTexture,
                        nameScaleAtMin,
                        nameMinWidth,
                        nameMaxWidth,
                        numberSpacingOne,
                        uvSet,
                        decalUv,
                        hasBaseLayer
                    );
                    continue;
                } else if (museAnimation.Length > 0)
                {
                    dataGrid.Rows.Add(name, bigfile, scene, String.Join(',', presetsUsedIn), museAnimation);
                    continue;
                }

                dataGrid.Rows.Add(name, bigfile, scene, String.Join(',', presetsUsedIn));
            }
        }

        private Boolean WriteToXml(string fileName)
        {
            // Making the XML doc object
            XmlDocument newXmlDoc = new XmlDocument();
            // Creating the initial xml node
            XmlProcessingInstruction instruction = newXmlDoc.CreateProcessingInstruction("xml", "version=\"1.0\"");
            newXmlDoc.AppendChild(instruction);
            // Creating the root element
            XmlElement rootElement = newXmlDoc.CreateElement("root");
            string[] partNames = new string[] { "jersey", "helmet", "pants", "socks", "shoes", "gloves" };
            // Set the part types at the beginning of the file.
            foreach (string partName in partNames) {
                XmlElement partTypeElement = newXmlDoc.CreateElement("partType");
                partTypeElement.SetAttribute("name", partName);
                rootElement.AppendChild(partTypeElement);
            }
            // Set the presets (officialTypes) next.
            foreach (DataGridViewRow item in presetsDataGrid.Rows)
            {
                if (item.Cells[0].Value == null || item.Cells[0].Value == "") { continue; }
                string presetName = item.Cells[0].Value.ToString();
                XmlElement node = newXmlDoc.CreateElement("officialType");
                node.SetAttribute("name", presetName);
                rootElement.AppendChild(node);
            }
            string[] partTypes = new string[] { "helmet", "jersey", "pants", "socks", "shoes", "gloves" };
            DataGridView[] partsDataGrids = new[]{
                    helmetsDataGrid, jerseysDataGrid, pantsDataGrid, socksDataGrid, shoesDataGrid, glovesDataGrid
            };
            // For each part, add a part node with proper child nodes.
            // Order: helmets, jerseys, pants, shoes, socks, gloves
            int partTypeIndex = 0;
            foreach (DataGridView dataGridView in partsDataGrids)
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    XmlElement partNode = newXmlDoc.CreateElement("part");
                    if (row.Cells[0].Value == null || row.Cells[0].Value == "") { continue; }

                    partNode.SetAttribute("name", row.Cells[0].Value.ToString());
                    if (partTypes[partTypeIndex] == "jersey")
                    {
                        // Add the shade to the jersey
                        // This has to be done before setting the type for some unknown reason
                        partNode.SetAttribute("shade", row.Cells[4].Value.ToString());
                    }
                    partNode.SetAttribute("type", partTypes[partTypeIndex]);

                    if (row.Cells[1].Value == null || row.Cells[1].Value == "")
                    {
                        MessageBox.Show(
                            "Bigfile is a required field, but is missing for one of your entries.",
                            "Invalid Data",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Stop
                        );
                        return false;
                    }
                    if (row.Cells[2].Value == null || row.Cells[2].Value == "")
                    {
                        MessageBox.Show(
                            "Scene is a required field, but is missing for one of your entries.",
                            "Invalid Data",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Stop
                        );
                        return false;
                    }
                    XmlElement bigfileNode = newXmlDoc.CreateElement("bigfile");
                    bigfileNode.SetAttribute("name", row.Cells[1].Value.ToString());
                    partNode.AppendChild(bigfileNode);

                    XmlElement sceneNode = newXmlDoc.CreateElement("scene");
                    string sceneName = row.Cells[2].Value.ToString();
                    sceneNode.SetAttribute("name", sceneName);
                    partNode.AppendChild(sceneNode);

                    // We have to add special fields for jerseys.
                    if (partTypes[partTypeIndex] == "jersey")
                    {
                        List<object> requiredJerseyValues = new List<object>()
                        {
                            row.Cells[5].Value,
                            row.Cells[6].Value,
                            row.Cells[7].Value,
                            row.Cells[8].Value,
                            row.Cells[9].Value,
                            row.Cells[10].Value,
                            row.Cells[11].Value,
                            row.Cells[12].Value,
                            row.Cells[13].Value
                        };

                        foreach (object value in requiredJerseyValues)
                        {
                            if (value == null || value.ToString().Length <= 0)
                            {
                                MessageBox.Show(
                                    "Missing required jersey params. All of the advanced params except baseLayer are required.",
                                    "Invalid Data",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Stop
                                );
                                return false;
                            }
                        }
                        XmlElement kerningTableElement = newXmlDoc.CreateElement("kerningTable");
                        kerningTableElement.SetAttribute("name", row.Cells[5].Value.ToString());
                        partNode.AppendChild(kerningTableElement);

                        XmlElement fontColorTextureElement = newXmlDoc.CreateElement("fontColorTexture");
                        fontColorTextureElement.SetAttribute("name", row.Cells[6].Value.ToString());
                        partNode.AppendChild(fontColorTextureElement);

                        XmlElement numberColorTextureElement = newXmlDoc.CreateElement("numberColorTexture");
                        numberColorTextureElement.SetAttribute("name", row.Cells[7].Value.ToString());
                        partNode.AppendChild(numberColorTextureElement);

                        XmlElement numberSpacingOneElement = newXmlDoc.CreateElement("numberSpacingOne");
                        numberSpacingOneElement.SetAttribute("value", row.Cells[11].Value.ToString());
                        partNode.AppendChild(numberSpacingOneElement);

                        XmlElement nameScaleAtMinElement = newXmlDoc.CreateElement("nameScaleAtMin");
                        nameScaleAtMinElement.SetAttribute("value", row.Cells[8].Value.ToString());
                        partNode.AppendChild(nameScaleAtMinElement);

                        XmlElement nameMinWidthElement = newXmlDoc.CreateElement("nameMinWidth");
                        nameMinWidthElement.SetAttribute("value", row.Cells[9].Value.ToString());
                        partNode.AppendChild(nameMinWidthElement);

                        XmlElement nameMaxWidthElement = newXmlDoc.CreateElement("nameMaxWidth");
                        nameMaxWidthElement.SetAttribute("value", row.Cells[10].Value.ToString());
                        partNode.AppendChild(nameMaxWidthElement);

                        object hasBaseLayer = row.Cells[14].Value;
                        if (hasBaseLayer != null && hasBaseLayer.ToString().Length > 0)
                        {
                            XmlElement hasBaseLayerElement = newXmlDoc.CreateElement("hasBaseLayer");
                            hasBaseLayerElement.SetAttribute("value", hasBaseLayer.ToString());
                            partNode.AppendChild(hasBaseLayerElement);
                        }

                        XmlElement uvSetElement = newXmlDoc.CreateElement("uvSet");
                        uvSetElement.SetAttribute("value", row.Cells[12].Value.ToString());
                        partNode.AppendChild(uvSetElement);

                        // Add a duplicate with shade light, so we can use for away games.
                        // rootElement.AppendChild(partNode);
                        // partNode.SetAttribute("shade", "light");
                        // Don't add the part here, it will get added outside the block.
                    }
                    object presetsUsedInValue = row.Cells[3].Value;
                    if (presetsUsedInValue != null)
                    {
                        string[] presetsUsedIn = presetsUsedInValue.ToString().Split(',');
                        foreach (string preset in presetsUsedIn)
                        {
                            XmlElement officialNode = newXmlDoc.CreateElement("official");
                            if (preset.Length <= 0) { continue; }
                            officialNode.SetAttribute("name", preset);
                            partNode.AppendChild(officialNode);
                        }
                    }
                    if (partTypes[partTypeIndex] == "jersey")
                    {
                        XmlElement decalUvElement = newXmlDoc.CreateElement("decalUV");
                        decalUvElement.SetAttribute("value", row.Cells[13].Value.ToString());
                        partNode.AppendChild(decalUvElement);
                    } else if (partTypes[partTypeIndex] == "gloves" && row.Cells[4].Value != null &&
                        row.Cells[4].Value.ToString().Length > 0)
                    {

                        XmlElement museAnimationElement = newXmlDoc.CreateElement("MUSEAnimation");
                        museAnimationElement.SetAttribute("value", row.Cells[4].Value.ToString());
                        partNode.AppendChild(museAnimationElement);
                    }


                    rootElement.AppendChild(partNode);
                }
                partTypeIndex++;
            }
            newXmlDoc.AppendChild(rootElement);
            newXmlDoc.Save(fileName);
            return true;
        }

        private void presetsDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView[] partsDataGrids = new[]{
                helmetsDataGrid, jerseysDataGrid, pantsDataGrid, shoesDataGrid, socksDataGrid, glovesDataGrid
            };
            foreach (DataGridView dataGrid in partsDataGrids)
            {
                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    row.Visible = true;
                }
            }

            if (presetsDataGrid.SelectedCells.Count < 1) { return; }
            if (presetsDataGrid.SelectedCells[0].Value == null) { return; }
            string presetName = presetsDataGrid.SelectedCells[0].Value.ToString();

            foreach (DataGridView dataGrid in partsDataGrids) {
                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    if (row.Cells[3].Value == null) { continue; }
                    String[] presetNames = row.Cells[3].Value.ToString().Split(',');
                    if (!presetNames.Contains(presetName)) {
                        row.Visible = false;
                    }
                }
            }
        }

        private void HandleOpenFile()
        {
            if (openDialogMixmatch.ShowDialog() == DialogResult.OK)
            {
                fileOpenTxtBox.Text = openDialogMixmatch.FileName;
                fileSavedTxtBox.Text = "";
                DataGridView[] partsDataGrids = new[]{
                    helmetsDataGrid, jerseysDataGrid, pantsDataGrid, shoesDataGrid, socksDataGrid, glovesDataGrid
                };
                foreach (DataGridView dataGrid in partsDataGrids)
                {
                    dataGrid.Rows.Clear();
                }
                presetsDataGrid.Rows.Clear();

                // openDialogMixmatch.FileName
                XmlDocument xmlDoc = new();
                xmlDoc.Load(openDialogMixmatch.FileName);

                // Get all of the preset names
                string[] presetNames = new string[] { };
                XmlNodeList presets = xmlDoc.GetElementsByTagName("officialType");
                foreach (XmlNode preset in presets)
                {
                    string presetName = preset.Attributes["name"].Value;
                    presetNames.Append(preset.Attributes["name"].Value);
                    presetsDataGrid.Rows.Add(presetName);
                }

                // Get all of the parts
                XmlNodeList parts = xmlDoc.GetElementsByTagName("part");
                List<XmlNode> helmets = new List<XmlNode>();
                List<XmlNode> jerseys = new List<XmlNode>();
                List<XmlNode> pants = new List<XmlNode>();
                List<XmlNode> socks = new List<XmlNode>();
                List<XmlNode> shoes = new List<XmlNode>();
                List<XmlNode> gloves = new List<XmlNode>();

                foreach (XmlNode part in parts)
                {
                    if (part.Attributes["type"] == null) { continue; }
                    switch (part.Attributes["type"].Value)
                    {
                        case "helmet":
                            helmets.Add(part);
                            break;
                        case "jersey":
                            jerseys.Add(part);
                            break;
                        case "pants":
                            pants.Add(part);
                            break;
                        case "socks":
                            socks.Add(part);
                            break;
                        case "shoes":
                            shoes.Add(part);
                            break;
                        case "gloves":
                            gloves.Add(part);
                            break;
                    }
                }

                AddParts(helmets, helmetsDataGrid);
                AddParts(jerseys, jerseysDataGrid, true);
                AddParts(pants, pantsDataGrid);
                AddParts(socks, socksDataGrid);
                AddParts(shoes, shoesDataGrid);
                AddParts(gloves, glovesDataGrid);
            }
            else
            {
                MessageBox.Show("Could not open file");
            }
        }

        private void HandleSaveFile()
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) { return; }

            if (!WriteToXml(saveFileDialog1.FileName))
            {
                MessageBox.Show("Error saving file");
                return;
            }
            fileSavedTxtBox.Text = saveFileDialog1.FileName;
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleSaveFile();
        }

        private void toggleJerseyAdvanced_Click(object sender, EventArgs e)
        {
            for (int i = 5; i < jerseysDataGrid.Columns.Count; i++)
            {
                jerseysDataGrid.Columns[i].Visible = !jerseysDataGrid.Columns[i].Visible;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteCells(helmetsDataGrid);
        }

        private void DeleteCells(DataGridView dataGrid)
        {
            foreach (DataGridViewCell cell in dataGrid.SelectedCells)
            {
                cell.Value = "";
            }
        }

        private void HandlePasteCells(DataGridView dataGrid)
        {
            string s = Clipboard.GetText();
            string[] lines = s.Split('\n');
            int row = dataGrid.CurrentCell.RowIndex;
            int col = dataGrid.CurrentCell.ColumnIndex;
            string[] cells = lines[0].Split('\t');
            int cellsSelected = cells.Length;
            for (int i = 0; i < cellsSelected; i++)
            {
                if (col >= dataGrid.Columns.Count) { break; }
                dataGrid[col, row].Value = cells[i];
                col++;
            }
            if (row >= dataGrid.Rows.Count-1)
            {
                dataGrid.NotifyCurrentCellDirty(true);
                dataGrid.EndEdit();
                dataGrid.NotifyCurrentCellDirty(false);
            }
        }

        private void HandleKeyDownDataGrid(KeyEventArgs e, DataGridView dataGrid)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                HandlePasteCells(dataGrid);
            }
            else if (e.Control && (e.KeyCode == Keys.D) || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                DeleteCells(dataGrid);
            } else if (e.Control && e.KeyCode == Keys.R)
            {
                foreach (DataGridViewCell cell in dataGrid.Rows[dataGrid.SelectedCells[0].RowIndex].Cells)
                {
                    cell.Selected = true;
                }
            } else if (e.Control && e.KeyCode == Keys.O)
            {
                HandleOpenFile();
            } else if (e.Control && e.KeyCode == Keys.S)
            {
                HandleSaveFile();
            }
        }

        private void helmetsDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyDownDataGrid(e, helmetsDataGrid);
        }

        private void presetsDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyDownDataGrid(e, presetsDataGrid);
        }

        private void jerseysDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyDownDataGrid(e, jerseysDataGrid);
        }

        private void pantsDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyDownDataGrid(e, pantsDataGrid);
        }

        private void shoesDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyDownDataGrid(e, shoesDataGrid);
        }

        private void socksDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyDownDataGrid(e, socksDataGrid);
        }

        private void glovesDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyDownDataGrid(e, glovesDataGrid);
        }
    }
}