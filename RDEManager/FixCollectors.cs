using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RDEManager
{
    public partial class FixCollectors : Form
    {
        public FixCollectors(DataTable records, Dictionary<string, List<string>> nonExistentAgents, DataTable masterAgents)
        {

            InitializeComponent();
            this.records = records;
            this.masterAgents = masterAgents.AsEnumerable();

            this.originalAgents = nonExistentAgents;

            List<string> temp = new List<string>();
            foreach (string agent in this.originalAgents.Keys)
            {
                temp.Add(agent);
            }
            temp.Sort();

            foreach (string agent in temp)
            {
                lbAgents.Items.Add(agent);
            }
            

            updatedAgents = new Dictionary<string, string>();

            lookupNames = new List<string>();

        }

        private void lbCollectors_SelectedIndexChanged(object sender, EventArgs e)
        {

            txtAgentName.Text = null;
            txtNumRecords.Text = null;
            txtCorrectedName.Text = null;
            lstCorrectedNameOptions.DataSource = null;


            string selected = lbAgents.SelectedItem.ToString();

            if (String.IsNullOrEmpty(selected))
            {
                return;
            }

            txtAgentName.Text = selected;
            txtNumRecords.Text = originalAgents[selected].Count.ToString();

            if (updatedAgents.Keys.Contains(selected))
            {
                txtCorrectedName.Text = updatedAgents[selected];
            }
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            List<string> notUpdated = new List<string>();
            foreach (string originalAgent in originalAgents.Keys)
            {
                if (!updatedAgents.Keys.Contains(originalAgent) || String.IsNullOrEmpty(updatedAgents[originalAgent]))
                {
                    notUpdated.Add(originalAgent);
                }
            }
            if (notUpdated.Count > 0)
            {
                string message = "The following collector names have not been updated. Are you sure you want to proceed?" +
                    Environment.NewLine + Environment.NewLine +
                    String.Join("; ", notUpdated.ToArray());
                DialogResult dr = MessageBox.Show(message, "Confirm update", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                if (dr == DialogResult.Yes)
                {
                    saveChanges();
                }
            }
            else
            {
                saveChanges();
            }
        }

        private void saveChanges()
        {
            var recordsEnum = records.AsEnumerable();

            int counter = 0;

            foreach (string agentName in updatedAgents.Keys)
            {
                foreach (string barcode in originalAgents[agentName])
                {
                    DataRow affectedRow = recordsEnum.Where(row => row["barcode"].ToString().Trim() == barcode).FirstOrDefault();
                    if (affectedRow != null) //should never be the case
                    {
                        if (affectedRow["detby"].ToString().Trim() == agentName)
                        {
                            affectedRow["detby"] = updatedAgents[agentName];
                            counter++;
                        }

                        if (affectedRow["collector"].ToString().Trim() == agentName)
                        {
                            affectedRow["collector"] = updatedAgents[agentName];
                            counter++;
                        }

                        string additional = affectedRow["addcoll"].ToString().Trim();
                        if (!String.IsNullOrEmpty(additional))
                        {
                            bool changed = false;
                            List<string> addCollectors = additional.Split(';').Select(x => x.Trim()).Where(x => !String.IsNullOrEmpty(x)).ToList();
                            for (int i = 0; i < addCollectors.Count; i++)
                            {
                                if (addCollectors[i] == agentName)
                                {
                                    addCollectors[i] = updatedAgents[agentName];
                                    counter++;
                                    changed = true;
                                }
                            }
                            if (changed)
                            {
                                affectedRow["addcoll"] = String.Join("; ", addCollectors.ToArray());
                            }
                        }
                    }
                }
            }
            MessageBox.Show($"{counter} name/s updated in dataset");
            this.Close();
        }

        private void txtCorrectedName_KeyUp(object sender, KeyEventArgs e)
        {
            tmrUpdateNamesList.Stop();
            tmrUpdateNamesList.Start();
        }

        private void tmrUpdateNamesList_Tick(object sender, EventArgs e)
        {
            tmrUpdateNamesList.Stop();
            lstCorrectedNameOptions.DataSource = null;
            lookupNames.Clear();

            List<DataRow> results = masterAgents.Where(row =>
            {
                return row["surname"].ToString().Trim().ToLower().StartsWith(txtCorrectedName.Text.ToLower()) ||
                row["initials"].ToString().Trim().ToLower().Contains(txtCorrectedName.Text.ToLower()) ||
                row["first"].ToString().Trim().ToLower().Contains(txtCorrectedName.Text.ToLower());
            }).ToList();

            lookupNames.Add("");
            foreach (DataRow row in results)
            {
                string last = row["surname"].ToString().Trim();
                string first = row["first"].ToString().Trim();
                string initials = row["initials"].ToString().Trim();

                if (String.IsNullOrEmpty(last))
                {
                    if (!String.IsNullOrEmpty(initials))
                    {
                        lookupNames.Add(initials);
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(initials))
                    {
                        lookupNames.Add(last);
                    }
                    else
                    {
                        lookupNames.Add(last + ", " + initials);
                    }
                }
            }
            lookupNames.Sort();
            lstCorrectedNameOptions.DataSource = lookupNames;
        }

        private void lstCorrectedNameOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lstCorrectedNameOptions.SelectedItem == null || lstCorrectedNameOptions.SelectedItem.ToString() == "")
            {
                return;
            }
            string selected = lstCorrectedNameOptions.SelectedItem.ToString();
            txtCorrectedName.Text = selected;
            if (updatedAgents.Keys.Contains(txtAgentName.Text))
            {
                updatedAgents[txtAgentName.Text] = selected;
            }
            else
            {
                updatedAgents.Add(txtAgentName.Text, selected);
            }
        }

        DataTable records { get; set; }
        
        EnumerableRowCollection<DataRow> masterAgents { get; set; }

        Dictionary<string, List<string>> originalAgents { get; set; }

        Dictionary<string, string> updatedAgents { get; set; }

        List<string> lookupNames { get; set; }

        
    }
}
