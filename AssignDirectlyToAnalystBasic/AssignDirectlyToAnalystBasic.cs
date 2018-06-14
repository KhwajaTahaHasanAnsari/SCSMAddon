/*
 * Custom SCSM 2010 task to Assign incident directly to Analyst via a list of users read from an AD group

 * Uses IDataItem and not tested on 2012

 * Thanks to Anton Gritsenko (FreemanRU) for help on using IDataItem to update form data and for IServiceContainer method of connection
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;                              //ConsoleCommand
using Microsoft.EnterpriseManagement.UI.SdkDataAccess.DataAdapters;                 //EnterpriseManagementObjectDataType
using Microsoft.EnterpriseManagement.ConsoleFramework;                              //FrameworkServices
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using System.DirectoryServices.AccountManagement;
using Microsoft.EnterpriseManagement.UI.DataModel;                                  //IDataItem
using System.ComponentModel.Design;                                                 //IServiceContainer
using Microsoft.EnterpriseManagement.ServiceManager.Application.Common;             //ConsoleContextHelper

namespace AssignDirectlyToAnalystBasic
{
    //Admin settings task
    public class AssignDirectlyToAnalystSettings : ConsoleCommand
    {
        public AssignDirectlyToAnalystSettings()
        {
        }

        public override void ExecuteCommand(IList<NavigationModelNodeBase> nodes, NavigationModelNodeTask task, ICollection<string> parameters)
        {
            try
            {
                //Connect to MG
                IServiceContainer isContainer = (IServiceContainer)FrameworkServices.GetService(typeof(IServiceContainer));
                IManagementGroupSession imgSession = (IManagementGroupSession)isContainer.GetService(typeof(IManagementGroupSession));
                if (imgSession == null)
                {
                    MessageBox.Show("Failed to connect to the current session", "Assign Incident Settings", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                EnterpriseManagementGroup emg = imgSession.ManagementGroup;

                //Get the analyst settings class and MP
                ManagementPack mpSetting = emg.ManagementPacks.GetManagementPack(new Guid("56d5c2d6-7e19-59ff-7a81-ac8a331fcb3f"));
                ManagementPackClass classSettings = mpSetting.GetClass("AssignSettingsClass");

                //Get the emo for the settings
                EnterpriseManagementObject emoSettings = emg.EntityObjects.GetObject<EnterpriseManagementObject>(classSettings.Id, ObjectQueryOptions.Default);

                //Setup form
                AdminForm af = new AdminForm();
                if (emoSettings[classSettings, "AssignDomain1"].Value != null)
                    af.textD1.Text = emoSettings[classSettings, "AssignDomain1"].Value.ToString();
                if (emoSettings[classSettings, "AssignDomain2"].Value != null)
                    af.textD2.Text = emoSettings[classSettings, "AssignDomain2"].Value.ToString();
                if (emoSettings[classSettings, "AssignDomain3"].Value != null)
                    af.textD3.Text = emoSettings[classSettings, "AssignDomain3"].Value.ToString();
                if (emoSettings[classSettings, "AssignGroup1"].Value != null)
                    af.textG1.Text = emoSettings[classSettings, "AssignGroup1"].Value.ToString();
                if (emoSettings[classSettings, "AssignGroup2"].Value != null)
                    af.textG2.Text = emoSettings[classSettings, "AssignGroup2"].Value.ToString();
                if (emoSettings[classSettings, "AssignGroup2"].Value != null)
                    af.textG3.Text = emoSettings[classSettings, "AssignGroup3"].Value.ToString();
                if (emoSettings[classSettings, "AssignShowAccount"].Value != null)
                    if (emoSettings[classSettings, "AssignShowAccount"].Value.ToString() == "1") af.checkShow.Checked = true;
                if (emoSettings[classSettings, "AssignShowTier"].Value != null)
                    if (emoSettings[classSettings, "AssignShowTier"].Value.ToString() == "1") af.checkShowTier.Checked = true;
                if (emoSettings[classSettings, "AssignedUserAlias"].Value != null) af.textAssignedUserName.Text = emoSettings[classSettings, "AssignedUserAlias"].Value.ToString();
                    else af.textAssignedUserName.Text = "AssignedUser";                
                if (emoSettings[classSettings, "ActionLogAlias"].Value != null) af.textActionLogName.Text = emoSettings[classSettings, "ActionLogAlias"].Value.ToString();
                    else af.textActionLogName.Text = "ActionLogs";

                DialogResult dr = af.ShowDialog();

                if (dr != DialogResult.Cancel)
                {
                    //save settings
                    emoSettings[classSettings, "AssignDomain1"].Value = af.textD1.Text;
                    emoSettings[classSettings, "AssignDomain2"].Value = af.textD2.Text;
                    emoSettings[classSettings, "AssignDomain3"].Value = af.textD3.Text;
                    emoSettings[classSettings, "AssignGroup1"].Value = af.textG1.Text;
                    emoSettings[classSettings, "AssignGroup2"].Value = af.textG2.Text;
                    emoSettings[classSettings, "AssignGroup3"].Value = af.textG3.Text;
                    if (af.checkShow.Checked) emoSettings[classSettings, "AssignShowAccount"].Value = "1";
                    else emoSettings[classSettings, "AssignShowAccount"].Value = "0";
                    if (af.checkShowTier.Checked) emoSettings[classSettings, "AssignShowTier"].Value = "1";
                    else emoSettings[classSettings, "AssignShowTier"].Value = "0";
                    emoSettings[classSettings, "AssignedUserAlias"].Value = af.textAssignedUserName.Text;
                    emoSettings[classSettings, "ActionLogAlias"].Value = af.textActionLogName.Text;
                    emoSettings.Commit();
                }
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }
    }

    //Console task
    public class AssignDirectlyToAnalyst : ConsoleCommand
    {
        public AssignDirectlyToAnalyst()
        {
        }

        public override void ExecuteCommand(IList<NavigationModelNodeBase> nodes, NavigationModelNodeTask task, ICollection<string> parameters)
        {

            //Used for idataitem (form mode, 1=new form, 2=edit form);
            int iMode = 0;

            //Set title for messageboxes
            string sAppTitle = "Assign Incident Directly To Analyst";

            //Connect to MG
            IServiceContainer isContainer = (IServiceContainer)FrameworkServices.GetService(typeof(IServiceContainer));
            IManagementGroupSession imgSession = (IManagementGroupSession)isContainer.GetService(typeof(IManagementGroupSession));
            if (imgSession == null)
            {
                MessageBox.Show("Failed to connect to the current session", sAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            EnterpriseManagementGroup emg = imgSession.ManagementGroup;

            //Get the incident class (System.WorkItem.Incident)
            ManagementPackClass classIncident = emg.EntityTypes.GetClass(new Guid("A604B942-4C7B-2FB2-28DC-61DC6F465C68"));

            //Microsoft.Windows.Library
            ManagementPack mpWindows =
                emg.ManagementPacks.GetManagementPack(new Guid("545131F0-58DE-1914-3A82-4FCAC9100A33"));

            //Get the Microsoft.AD.User class
            ManagementPackClass mpcADUser = emg.EntityTypes.GetClass("Microsoft.AD.User", mpWindows);

            //Return the currently selected incident reference
            NavigationModelNodeBase selincident = nodes[0];

            //Form mode (not new)?
            if (selincident.Location.AbsoluteUri.IndexOf("FormDisplay", 0) > 0) iMode = 2;

            //Get objects
            EnterpriseManagementObject emoSelIncident = null;
            IDataItem i = ConsoleContextHelper.Instance.GetFormDataContext(nodes[0]);

            //Get the analyst settings class and MP
            ManagementPack mpSetting = emg.ManagementPacks.GetManagementPack(new Guid("56d5c2d6-7e19-59ff-7a81-ac8a331fcb3f"));
            ManagementPackClass classSettings = mpSetting.GetClass("AssignSettingsClass");

            //Get the emo for the settings
            EnterpriseManagementObject emoSettings = emg.EntityObjects.GetObject<EnterpriseManagementObject>(classSettings.Id, ObjectQueryOptions.Default);
            
            //TP names
            string sAssignedTo = "AssignedUser";
            string sActionLog = "ActionLogs";
            if (emoSettings[classSettings, "AssignedUserAlias"].Value != null) sAssignedTo = emoSettings[classSettings, "AssignedUserAlias"].Value.ToString();
            if (emoSettings[classSettings, "ActionLogAlias"].Value != null) sActionLog = emoSettings[classSettings, "ActionLogAlias"].Value.ToString();

            //Check if new
            if (!(bool)i["$IsNew$"])
            {
                //Now get the guid of the selected workitem. Depending on the view type, the return will be different, so take it after the last "."
                //There are lots of ways of doing this, this is not the best way but it was the first I learnt and it works
                string strGuid = selincident.GetId().Substring(selincident.GetId().LastIndexOf('.') + 1);

                //Get the emo of the workitem via it's guid
                emoSelIncident = emg.EntityObjects.GetObject<EnterpriseManagementObject>(new Guid(strGuid), ObjectQueryOptions.Default);
            }
            //Creating new incident
            else iMode = 1;

            //Was task was run from an workitem opened for editing, as opposed to a list or view?            
            if (selincident.Location.AbsoluteUri.IndexOf("FormDisplay", 0) != -1) iMode = 2;

            //Get the status guid
            Guid gStatus = Guid.NewGuid();
            try
            {
                //"New" status will throw an exception to must catch here
                if (i["Status"] != null) gStatus = (Guid)(i["Status"] as IDataItem)["Id"];
            }
            catch
            {
            }

            //Get the incident class (System.WorkItem.Incident)
            ManagementPackClass mpcIncident = emg.EntityTypes.GetClass(new Guid("a604b942-4c7b-2fb2-28dc-61dc6f465c68"));

            //Check if the incident is closed              
            if (gStatus == new Guid("bd0ae7c4-3315-2eb3-7933-82dfc482dbaf"))
            {
                MessageBox.Show("This incident cannot be reassigned as it has been closed.", sAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //Note - we are alllowing resolved incidents to be re-assigned, to prevent this, uncomment the following block:
            /*else if (gStatus == new Guid("2b8830b6-59f0-f574-9c2a-f4b4682f1681"))
            {
                MessageBox.Show("This incident cannot be reassigned as it has been resolved.", sAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
             */         

            //Current assignee and Id
            string sId = "";
            string sUser = "";         
            string sTierGuid = "";
            if (iMode == 0)
            {
                //View mode
                sUser = this.GetProperty(ref emg, emoSelIncident, ref mpcADUser, "DisplayName");
                sId = emoSelIncident[mpcIncident, "Id"].Value.ToString();
                if (emoSelIncident[mpcIncident, "TierQueue"].Value != null) sTierGuid = ((ManagementPackEnumeration)emoSelIncident[mpcIncident, "TierQueue"].Value).Id.ToString();
            }
            else
            {
                //New or edit form mode  
                try
                {
                    //Get display name of current assignee
                    if (i[sAssignedTo] == null) sUser = "no-one";
                    else sUser = (string)(i[sAssignedTo] as IDataItem)["DisplayName"];
                }
                catch
                {
                    //Set no assignee
                    sUser = "no-one";
                }
                sId = (string)i["Id"];
                try
                {
                    //Check current tier queue enum value
                    if (i["TierQueue"] != null) sTierGuid = ((Guid)(i["TierQueue"] as IDataItem)["Id"]).ToString();
                }
                catch
                {
                }
            }

            //Create a new instance of the form and set it up
            AssignForm af = new AssignForm();
            af.sTierGuid = sTierGuid;
            af.Text = "Assign incident " + sId + " directly to Analyst - currently assigned to " + sUser;
            af.textDefault.Text = sUser;
            af.emg = emg;         

            //Show the analyst/tier selection form
            DialogResult dr = af.ShowDialog();

            if (dr != DialogResult.Cancel)
            {
                //Get the samaccountname from the right hand part of the combobox.text after the !
                string sADUserName = af.comboAnalysts.Text.Substring(af.comboAnalysts.Text.LastIndexOf("(") + 1);
                //Remove last )
                sADUserName = sADUserName.Substring(0, sADUserName.Length - 1);

                //Format to get the display name only from the left part - this is used for the actionlog entry
                string sADUserDisplayName = af.comboAnalysts.Text.Substring(0, af.comboAnalysts.Text.LastIndexOf("(")).Trim();

                try
                {
                    //Set the query for the user - note - usernames are assumed unique across configured domains
                    //If this is not the case, you need to customise these criteria to include a domain
                    string sADUserCriteria = String.Format(@"
                        <Criteria xmlns=""http://Microsoft.EnterpriseManagement.Core.Criteria/"">
                        <Reference Id=""Microsoft.Windows.Library"" PublicKeyToken=""{0}"" Version=""{1}"" Alias=""MSWinLib"" />
                        <Expression>
                        <SimpleExpression>
                        <ValueExpressionLeft>
                        <Property>$Target/Property[Type='MSWinLib!Microsoft.AD.User']/UserName$</Property>
                        </ValueExpressionLeft>
                        <Operator>Equal</Operator>
                        <ValueExpressionRight>
                        <Value>" + sADUserName + @"</Value>
                        </ValueExpressionRight>
                        </SimpleExpression>
                        </Expression>
                        </Criteria>
                        ", mpWindows.KeyToken, mpWindows.Version.ToString());

                    //Object query options
                    ObjectQueryOptions objQueryOpts = new ObjectQueryOptions();
                    objQueryOpts.ObjectRetrievalMode = ObjectRetrievalOptions.Buffered;
                    objQueryOpts.DefaultPropertyRetrievalBehavior = ObjectPropertyRetrievalBehavior.All;
                    //We are searching via samAccountName so there will be only 1 item
                    objQueryOpts.MaxResultCount = 1;

                    //Get the AD User CI object       
                    EnterpriseManagementObjectCriteria emocADUser =
                        new EnterpriseManagementObjectCriteria(sADUserCriteria, mpcADUser, emg);
                    IObjectReader<EnterpriseManagementObject> orADUser = emg.EntityObjects.GetObjectReader<EnterpriseManagementObject>(emocADUser, objQueryOpts);
                    EnterpriseManagementObject emoAssignToUser = orADUser.ElementAt(0);

                    if (iMode == 0)
                    {
                        //View mode - create a new assigned to user relationship
                        ManagementPackRelationship relAssignedToUser =
                        emg.EntityTypes.GetRelationshipClass(new Guid("15e577a3-6bf9-6713-4eac-ba5a5b7c4722"));
                        CreatableEnterpriseManagementRelationshipObject cemroAssignedToUser =
                            new CreatableEnterpriseManagementRelationshipObject(emg, relAssignedToUser);

                        //Set the source and target...
                        cemroAssignedToUser.SetSource(emoSelIncident);
                        cemroAssignedToUser.SetTarget(emoAssignToUser);

                        //Save 
                        cemroAssignedToUser.Commit();

                        //Add a new comment
                        this.AddActionLogEntry(emg, emoSelIncident, "Incident was assigned to " + sADUserDisplayName, af.textComment.Text);

                        //Check tier
                        if (af.bShowTier)
                        {
                            if (af.comboTier.Text == "")
                                emoSelIncident[classIncident, "TierQueue"].Value = null;
                            else
                            {
                                ManagementPackEnumeration mpeTier = emg.EntityTypes.GetEnumeration(new Guid(af.comboTierGuids.Items[af.comboTier.SelectedIndex].ToString()));
                                emoSelIncident[classIncident, "TierQueue"].Value = mpeTier;
                            }
                            emoSelIncident.Commit();
                        }

                        //Refresh the current incident view
                        this.RequestViewRefresh();
                    }
                    else
                    {
                        //Note - IDataItem property names depend on the type projection being used and may differ from these

                        //Form mode, create a proxy to the emo user object to set on the form                
                        EnterpriseManagementObjectDataType dataType = new EnterpriseManagementObjectDataType(mpcADUser);
                        IDataItem iUser = dataType.CreateProxyInstance(emoAssignToUser);
                        i[sAssignedTo] = iUser;

                        //Check tier
                        if (af.bShowTier)
                        {
                            if (af.comboTier.Text == "")
                                //Remove tier
                                i["TierQueue"] = null;
                            else
                            {
                                //Set tier, get enum first
                                ManagementPackEnumeration mpeTier = emg.EntityTypes.GetEnumeration(new Guid(af.comboTierGuids.Items[af.comboTier.SelectedIndex].ToString()));
                                i["TierQueue"] = mpeTier;
                            }
                        }
                        //Uncommenting this will cause the IDataItem to be saved and thus update the actual object in the database, normally you don't want to do this
                        //as you want the user to click OK or cancel on the form instead                        
                        //EnterpriseManagementObjectProjectionDataType.UpdateDataItem(i);   

                        //IDataItem Action log
                        //
                        //Get the System.WorkItem.Library mp
                        ManagementPack mpWorkItemLibrary = emg.ManagementPacks.GetManagementPack(new Guid("405D5590-B45F-1C97-024F-24338290453E"));
                        //Get the actionlog class
                        ManagementPackClass mpcActionLog =
                            emg.EntityTypes.GetClass("System.WorkItem.TroubleTicket.ActionLog", mpWorkItemLibrary);

                        //Create a new action log entry as an idataitem
                        CreatableEnterpriseManagementObject cemoActionLog =
                            new CreatableEnterpriseManagementObject(emg, mpcActionLog);
                        EnterpriseManagementObjectDataType dataTypeLog = new EnterpriseManagementObjectDataType(mpcActionLog);
                        IDataItem iLog = dataTypeLog.CreateProxyInstance(cemoActionLog);  

                        //Setup the new action log entry
                        iLog["Id"] = Guid.NewGuid().ToString();
                        iLog["Description"] = af.textComment.Text;
                        iLog["Title"] = "Reassignment Comment";
                        iLog["EnteredBy"] = UserPrincipal.Current.DisplayName;
                        iLog["EnteredDate"] = DateTime.Now.ToUniversalTime();

                        //Set action type (this also adds the icon and is required)
                        ManagementPackEnumeration enumActionLog =
                            mpWorkItemLibrary.GetEnumerations().GetItem("System.WorkItem.ActionLogEnum.TaskExecuted");
                        iLog["ActionType"] = enumActionLog;

                        //This adds the new idataitem log entry to the entries displayed on the form, it does not over-write the existing entries
                        i[sActionLog] = iLog;
                    }
                }
                catch (System.Exception e)
                {
                    //Oops
                    MessageBox.Show(e.Message + "\n\n" + e.StackTrace, sAppTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
        }

        //Get property for passed emoUser
        public string GetProperty(
          ref EnterpriseManagementGroup emg,
          EnterpriseManagementObject emoIncident,
          ref ManagementPackClass mpcADUser,
          string sProperty
          )
        {
            try
            {
                //Assigned to user relationship Guid
                Guid rel = new Guid("15e577a3-6bf9-6713-4eac-ba5a5b7c4722");

                EnterpriseManagementObject emoAssignedToUser = null;

                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> obj in
                  emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(emoIncident.Id, TraversalDepth.OneLevel, ObjectQueryOptions.Default))
                {
                    if (obj.RelationshipId == rel)
                        emoAssignedToUser = obj.TargetObject;
                }
                return "" + emoAssignedToUser[mpcADUser, sProperty].Value;
            }
            catch
            {
                return "no-one";
            }
        }

        //Add action log relationship to passed emo
        private bool AddActionLogEntry(
           EnterpriseManagementGroup emg,
           EnterpriseManagementObject emoIncident,
           string strTitle,
           string strDescription
           )
        {
            try
            {
                //Get the System.WorkItem.Library mp
                ManagementPack mpWorkItemLibrary = emg.ManagementPacks.GetManagementPack(new Guid("405D5590-B45F-1C97-024F-24338290453E"));

                //Get the actionlog class
                ManagementPackClass typeActionLog =
                    emg.EntityTypes.GetClass("System.WorkItem.TroubleTicket.ActionLog", mpWorkItemLibrary);

                //Create a new action log entry
                CreatableEnterpriseManagementObject objectActionLog =
                    new CreatableEnterpriseManagementObject(emg, typeActionLog);

                //Setup the action log entry
                objectActionLog[typeActionLog, "Id"].Value = Guid.NewGuid().ToString();
                objectActionLog[typeActionLog, "Description"].Value = strDescription + "\n";
                objectActionLog[typeActionLog, "Title"].Value = strTitle;
                objectActionLog[typeActionLog, "EnteredBy"].Value = UserPrincipal.Current.DisplayName;
                objectActionLog[typeActionLog, "EnteredDate"].Value = DateTime.Now.ToUniversalTime();

                //Get the enumeration and relationship for the actionlog entry
                ManagementPackEnumeration enumActionLog =
                   mpWorkItemLibrary.GetEnumerations().GetItem("System.WorkItem.ActionLogEnum.TaskExecuted");
                objectActionLog[typeActionLog, "ActionType"].Value = enumActionLog;
                ManagementPackRelationship relActionLog =
                    emg.EntityTypes.GetRelationshipClass("System.WorkItem.TroubleTicketHasActionLog", mpWorkItemLibrary);

                //Get the projection for the incident from the emo
                EnterpriseManagementObjectProjection emopIncident = new EnterpriseManagementObjectProjection(emoIncident);

                //Add relationship and save
                emopIncident.Add(objectActionLog, relActionLog.Target);
                emopIncident.Commit();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}




