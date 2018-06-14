//Task form
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.DirectoryServices;
using Microsoft.Win32;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;

namespace AssignDirectlyToAnalystBasic
{
    public partial class AssignForm : Form
    {
        public AssignForm()
        {
            InitializeComponent();
        }

        public EnterpriseManagementGroup emg = null;
        public bool bShowTier = false;
        public string sTierGuid = "";

        private void AssignForm_Load(object sender, EventArgs e)
        {
            this.CancelButton = butCancel;          
            //Load the analysts from the AD group
            this.PopulateAnalysts(this.comboAnalysts,this.textDefault.Text);
        }

        private bool CheckAnalyst()
        {
            if (this.comboAnalysts.Text == "") return false;
            if (this.comboAnalysts.Text.IndexOf(this.textDefault.Text, 0) != -1) return false;
            return true;
        }

        private void CheckStates()
        {
            if (this.checkBypassComment.Checked == true)
            {
                this.textComment.Enabled = false;

                if (this.CheckAnalyst() == true)
                    this.butOK.Enabled = true;
                else
                    this.butOK.Enabled = false;
            }
            else
            {
                this.textComment.Enabled = true;

                if (this.textComment.Text != "" && this.CheckAnalyst() == true)
                    this.butOK.Enabled = true;
                else
                    this.butOK.Enabled = false;
            }
        }
  
        private void textComment_TextChanged(object sender, EventArgs e)
        {
            this.CheckStates();
        }

        private void comboAnalysts_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CheckStates();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Visible = false;
        }

        private void butOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Visible = false;
        }

        //Read analysts from the AD groups
        private bool PopulateAnalysts(
            ComboBox cbox,
            string sDefaultUser
            )
        {
            try
            {
                //Get the analyst settings class and MP
                ManagementPack mpSetting = emg.ManagementPacks.GetManagementPack(new Guid("56d5c2d6-7e19-59ff-7a81-ac8a331fcb3f"));
                ManagementPackClass classSettings = mpSetting.GetClass("AssignSettingsClass");

                //Get the emo for the settings
                EnterpriseManagementObject emoSettings = emg.EntityObjects.GetObject<EnterpriseManagementObject>(classSettings.Id, ObjectQueryOptions.Default);
                
                if (emoSettings[classSettings, "AssignDomain1"].Value != null &&
                    emoSettings[classSettings, "AssignGroup1"].Value != null &&
                    emoSettings[classSettings, "AssignDomain1"].Value.ToString() != "" &&
                    emoSettings[classSettings, "AssignGroup1"].Value.ToString() != "")
                {
                    AddUsers(emoSettings[classSettings, "AssignDomain1"].Value.ToString(),
                        emoSettings[classSettings, "AssignGroup1"].Value.ToString(),
                        classSettings, emoSettings, cbox);
                }

                if (emoSettings[classSettings, "AssignDomain2"].Value != null &&
                    emoSettings[classSettings, "AssignGroup2"].Value != null &&
                    emoSettings[classSettings, "AssignDomain2"].Value.ToString() != "" &&
                    emoSettings[classSettings, "AssignGroup2"].Value.ToString() != "")
                {
                    AddUsers(emoSettings[classSettings, "AssignDomain2"].Value.ToString(),
                        emoSettings[classSettings, "AssignGroup2"].Value.ToString(),
                        classSettings, emoSettings, cbox);
                }

                if (emoSettings[classSettings, "AssignDomain3"].Value != null &&
                    emoSettings[classSettings, "AssignGroup3"].Value != null &&
                    emoSettings[classSettings, "AssignDomain3"].Value.ToString() != "" &&
                    emoSettings[classSettings, "AssignGroup3"].Value.ToString() != "")
                {
                    AddUsers(emoSettings[classSettings, "AssignDomain3"].Value.ToString(),
                        emoSettings[classSettings, "AssignGroup3"].Value.ToString(),
                        classSettings, emoSettings, cbox);
                }          

                //Set currently assigned to user
                foreach (string s in cbox.Items)
                {
                    if (s.IndexOf(this.textDefault.Text) != -1) cbox.Text = s;
                } 
              
                //Check tier settings and populate/show if required
                if (emoSettings[classSettings, "AssignShowTier"].Value != null &&
                    emoSettings[classSettings, "AssignShowTier"].Value.ToString() == "1")
                {
                    this.comboTier.Visible = true;
                    this.labelTier.Visible = true;
                    this.bShowTier = true;
                    BuildTierList(this.comboTier, this.comboTierGuids);
                    
                    //Set default tier
                    if (this.sTierGuid != "") this.comboTier.SelectedIndex = this.comboTierGuids.FindStringExact(this.sTierGuid);                  
                }       
              
                return true;
            }
            catch
            {
                MessageBox.Show("Failed to read the assign settings, check configuration.", "Check Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        //Add tiers to the tier list
        private void BuildTierList(
         ComboBox cbox,
         ComboBox cboxguids
         )
        {
            try
            {
                //Tier enum
                string sTiers = "c3264527-a501-029f-6872-31300080b3bf";
                ManagementPackEnumerationCriteria mpenumCriteria =
                    new ManagementPackEnumerationCriteria("Parent = '" + sTiers + "'");

                IList<ManagementPackEnumeration> listEnumIncidentTier =
                    emg.EntityTypes.GetEnumerations(mpenumCriteria);

                //Sort by Ordinal
                IEnumerable<ManagementPackEnumeration> tiers = listEnumIncidentTier.OrderBy(x => x.Ordinal);

                foreach (ManagementPackEnumeration mpenumTier in tiers)
                {
                    string s = new String(' ', 250);
                    cbox.Items.Add(mpenumTier.DisplayName);
                    cboxguids.Items.Add(mpenumTier.Id.ToString());
                    AddTierChildren(mpenumTier, cbox, cboxguids, "    ", s);
                }
            }
            catch
            {
            }
        }
        //Get tier children for passed tier
        private void AddTierChildren(
          ManagementPackEnumeration mpe,
          ComboBox cbox,
          ComboBox cboxguids,
          string sSpacer,
          string s
          )
        {
            try
            {
                ManagementPackEnumerationCriteria mpenumCriteriaKids =
                  new ManagementPackEnumerationCriteria("Parent = '" + mpe.Id.ToString() + "'");
                IList<ManagementPackEnumeration> listEnumTierKids =
                emg.EntityTypes.GetEnumerations(mpenumCriteriaKids);

                //Sort by ordinal
                IEnumerable<ManagementPackEnumeration> tiers = listEnumTierKids.OrderBy(x => x.Ordinal);

                //Add children (we don't curently have any further levels)
                foreach (ManagementPackEnumeration mpenumTierKids in tiers)
                {
                    cbox.Items.Add(sSpacer + mpenumTierKids.DisplayName);
                    cboxguids.Items.Add(mpenumTierKids.Id.ToString());
                    AddTierChildren(mpenumTierKids, cbox, cboxguids, sSpacer + "  ", s);
                }
            }
            catch
            {            
            }
        }
        //Add users to the passed combo box for the passed group
        private static void AddUsers(
            string strDomainRoot,
            string strGroupPath,
            ManagementPackClass classSettings,
            EnterpriseManagementObject emoSettings,
            ComboBox cbox
            )
        {

            try
            {
                //Read each user object from the specified group
                DirectoryEntry de = new DirectoryEntry(strDomainRoot);
                DirectorySearcher ds = new DirectorySearcher(de, "(objectClass=person)");
                ds.Filter = "(memberOf=" + strGroupPath + ")";
                ds.PropertiesToLoad.Add("displayname");
                ds.PropertiesToLoad.Add("samaccountname");

                string s = "";
                if (emoSettings[classSettings, "AssignShowAccount"].Value != null &&
                    emoSettings[classSettings, "AssignShowAccount"].Value.ToString() == "1") s = new string(' ', 5);
                else if (emoSettings[classSettings, "AssignShowAccount"].Value != null &&
                    emoSettings[classSettings, "AssignShowAccount"].Value.ToString() == "0") s = new string(' ', 100);
                else s = new string(' ', 100);

                //Add each user to the list, with !samAccountName on the very far right (so it's hidden)
                foreach (SearchResult sr in ds.FindAll())
                {
                    if (cbox.FindStringExact(sr.Properties["displayname"][0].ToString() + s +
                          "(" + sr.Properties["samaccountname"][0].ToString() + ")") == -1)
                    {
                        int ci = cbox.Items.Add(sr.Properties["displayname"][0].ToString() + s +
                            "(" + sr.Properties["samaccountname"][0].ToString() + ")");
                    }
                }
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message + "\n\n" + e.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        private void checkBypassComment_CheckedChanged(object sender, EventArgs e)
        {
            this.CheckStates();
        }

        //Allows the tier to be removed with DEL key
        private void comboTier_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                comboTier.SelectedIndex = -1;
                comboTierGuids.SelectedIndex = -1;
            }       
        }             
    }   
 }
