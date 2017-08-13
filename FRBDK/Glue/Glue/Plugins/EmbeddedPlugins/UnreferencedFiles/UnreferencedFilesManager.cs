﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Glue.VSHelpers.Projects;
using FlatRedBall.IO;
using FlatRedBall.Glue.SaveClasses;
using System.IO;
using FlatRedBall.Glue.FormHelpers;
using System.Windows.Forms;
using FlatRedBall.Glue.Elements;
using FlatRedBall.Glue.IO;
using FlatRedBall.Glue.AutomatedGlue;
using System.Threading;
using FlatRedBall.Glue.Plugins.ExportedImplementations;
using Microsoft.Build.Evaluation;

namespace FlatRedBall.Glue.Managers
{
    public class UnreferencedFilesManager
    {
        #region Fields

        static List<ProjectSpecificFile> mLastAddedUnreferencedFiles = new List<ProjectSpecificFile>();
        static List<string> mListBeforeAddition = new List<string>();
        static List<ProjectSpecificFile> mUnreferencedFiles = new List<ProjectSpecificFile>();

        public bool mHasHadFailure = false;


        static UnreferencedFilesManager mSelf;



        #endregion

        #region Properties

        public static UnreferencedFilesManager Self
        {
            get
            {
                if (mSelf == null)
                {
                    mSelf = new UnreferencedFilesManager();
                }
                return mSelf;
            }
        }

        public static List<ProjectSpecificFile> LastAddedUnreferencedFiles
        {
            get
            {
                // Makes this getter thread-safe.
                List<ProjectSpecificFile> toReturn = new List<ProjectSpecificFile>();

                // This caused dead locks and I'm not sure we need it
                //lock (mLastAddedUnreferencedFiles)
                {
                    toReturn.AddRange(mLastAddedUnreferencedFiles);
                }

                return toReturn;
            }
        }

        public List<ProjectSpecificFile> UnreferencedFiles
        {
            get
            {
                return mUnreferencedFiles;
            }
        }

        bool mIsRefreshRequested;
        public bool IsRefreshRequested
        {
            get { return mIsRefreshRequested; }
            set 
            { 
                mIsRefreshRequested = value; 
            }
        }

        #endregion

        static bool alreadyShowedMessage = false;
        public void RefreshUnreferencedFiles(bool async)
        {
            if (async)
            {
                TaskManager.Self.AddAsyncTask(RefreshUnreferencedFilesInternal, "Refreshing unreferenced files");
            }
            else
            {
                RefreshUnreferencedFilesInternal();
            }
        }

        public void RefreshUnreferencedFilesInternal()
        {
            // This may be called sync and async at the same time
            // and this can cause weird results.  let's make sure that
            // this never happens...
            lock (mLastAddedUnreferencedFiles)
            {
                mLastAddedUnreferencedFiles.Clear();
                if (ProjectManager.ContentProject != null && !mHasHadFailure)
                {
                    // return after;

                    mListBeforeAddition.Clear();

                    // Gotta to-lower it 
                    // so we can do =='s checks later.
                    for (int i = 0; i < mUnreferencedFiles.Count; i++)
                    {
                        mListBeforeAddition.Add(mUnreferencedFiles[i].FilePath.ToLower());
                    }

                    mUnreferencedFiles.Clear();

                    string contentDirectory = ProjectManager.ContentProject.GetAbsoluteContentFolder();

                    List<string> referencedFiles = null;

                    try
                    {
                        referencedFiles = GlueCommands.Self.FileCommands.GetAllReferencedFileNames();

                        // to make debugging a little easier:
                        referencedFiles = referencedFiles
                            .Select(item => item.ToLower().Replace('\\', '/'))
                            .Distinct()
                            .OrderBy(item => item)
                            .ToList();
                    }
                    catch
                    {
                        if (!alreadyShowedMessage)
                        {
                            alreadyShowedMessage = true;

                            string message = "Glue was unable to track references to some files.  This means that Glue will be unable to automatically add files " +
                                "to your project, and also will be unable to remove files if necessary.  It is recommended that you fix the file reference errors and restart.";

                            GlueGui.ShowMessageBox(message);

                            mHasHadFailure = true;
                        }
                    }


                    if (referencedFiles != null)
                    {
                        foreach (var evaluatedItem in ProjectManager.ProjectBase.ContentProject.EvaluatedItems)
                        {
                            AddIfUnreferenced(evaluatedItem, ProjectManager.ProjectBase, referencedFiles, mUnreferencedFiles);
                        }

                        Application.DoEvents();

                        // copy the list to make it not lock
                        List<ProjectBase> syncedProjectCopy = new List<ProjectBase>();

                        lock (ProjectManager.SyncedProjects)
                        {
                            syncedProjectCopy.AddRange(ProjectManager.SyncedProjects);
                        }

                        foreach (var syncedProject in syncedProjectCopy)
                        {
                            var cloned = syncedProject.ContentProject.EvaluatedItems;
                            foreach (var evaluatedItem in cloned)
                            {
                                AddIfUnreferenced(evaluatedItem, syncedProject, referencedFiles, mUnreferencedFiles);
                                // Since we're in a lock, maybe we shouldn't
								// Allow the application to do its thing
								//Application.DoEvents();
                            }
                        }

                    }

                    mUnreferencedFiles = mUnreferencedFiles.OrderBy(item => item.FilePath.ToLowerInvariant()).ToList();
                }
            }
        }

        static bool GetIfIsUnreferenced(ProjectItem item, ProjectBase project, List<string> referencedFiles, out string nameToInclude)
        {
            bool isUnreferenced = false;

            var link = item.GetLink();

            string itemName;

            itemName =  item.UnevaluatedInclude.ToLower().Replace(@"\", @"/");
            nameToInclude = item.UnevaluatedInclude;

            // no extensions are unsupported.  What do we do with the content pipeline?
            if (!string.IsNullOrEmpty(FileManager.GetExtension(itemName)))
            {

                bool isContent = ProjectManager.IsContent(itemName);

                string absoluteFile =
                    FileManager.RemoveDotDotSlash(FileManager.GetDirectory(project.ContentProject.FullFileName) + nameToInclude);

                // the referencedFiles list is the list of files that are referenced relative to the main content project, but the project
                // may not be the content project. We want to get the name relative to the main content project before checking.
                if(isContent && project.ContentProject != GlueState.Self.CurrentMainContentProject)
                {
                    itemName = FileManager.MakeRelative(itemName, FileManager.GetDirectory(GlueState.Self.CurrentMainContentProject.FullFileName));
                }

                isUnreferenced = isContent &&
                    // Vic asks: This code would only consider a file unreferenced if it exists. Why?
                    // If a file doesn't exist but it's referenced by a VS project and it's not needed, shouldn't
                    // we consider it unreferenced?
                    //File.Exists(absoluteFile) &&
                    !referencedFiles.Contains(itemName);

            }
            return isUnreferenced;
        }

        private static void AddIfUnreferenced(ProjectItem item, ProjectBase project, List<string> referencedFiles, List<ProjectSpecificFile> unreferencedFiles)
        {
            string nameToInclude;
            bool isUnreferenced = GetIfIsUnreferenced(item, project, referencedFiles, out nameToInclude);

            if (isUnreferenced)
            {
                nameToInclude = ProjectManager.ContentProject.GetAbsoluteContentFolder() + nameToInclude;
                nameToInclude = nameToInclude.Replace(@"/", @"\");

                if (!mListBeforeAddition.Contains(nameToInclude.ToLower()))
                {
                    var projectSpecificFile = new ProjectSpecificFile()
                    {
                        FilePath = nameToInclude.ToLower(),
                        ProjectName = project.Name
                    };

                    lock (mLastAddedUnreferencedFiles)
                    {
                        mLastAddedUnreferencedFiles.Add(projectSpecificFile);
                    }
                }
                unreferencedFiles.Add(new ProjectSpecificFile()
                {
                    FilePath = nameToInclude,
                    ProjectName = project.Name
                });
            }
        }

        internal void RefreshAndRemoveNewlyAddedUnreferenced()
        {
            RefreshUnreferencedFiles(false);

            bool shouldRefreshAgainst = false;
            lock (FileWatchManager.LockObject)
            {
                // use the property to be thread-safe
                
                var lastAddedUnreferenced = UnreferencedFilesManager.LastAddedUnreferencedFiles;
                foreach (ProjectSpecificFile projectSpecificFile in lastAddedUnreferenced)
                {
                    if (File.Exists(ProjectManager.MakeAbsolute(projectSpecificFile.FilePath)))
                    {
                        DialogResult result =
                            System.Windows.Forms.MessageBox.Show(
                                "The following file is no longer referenced by the project\n\n" +
                                projectSpecificFile +
                                "\n\nRemove and delete this file?", "Remove unreferenced file?", MessageBoxButtons.YesNo);

                        if (result == DialogResult.Yes)
                        {
                            ProjectManager.GetProjectByName(projectSpecificFile.ProjectName).ContentProject.RemoveItem(
                                projectSpecificFile.FilePath);

                            FileHelper.DeleteFile(ProjectManager.MakeAbsolute(projectSpecificFile.FilePath));
                            shouldRefreshAgainst = true;
                        }
                    }
                }
            }

            if (shouldRefreshAgainst)
            {
                UnreferencedFilesManager.Self.RefreshUnreferencedFiles(false);
            }
        }


        public bool NeedsRefreshOfUnreferencedFiles { get; set; }
        public bool ProcessRefreshOfUnreferencedFiles()
        {
            if (!NeedsRefreshOfUnreferencedFiles) return false;

            RefreshUnreferencedFiles(false);

            var shouldRefreshAgainst = false;
            // makes this threadsafe
            var lastAddedUnreferencedFiles = LastAddedUnreferencedFiles;
            foreach (var projectSpecificFile in lastAddedUnreferencedFiles)
            {
                if (!File.Exists(ProjectManager.MakeAbsolute(projectSpecificFile.FilePath))) continue;

                DialogResult result =
                    System.Windows.Forms.MessageBox.Show(
                        "The following file is no longer referenced by the project\n\n" +
                        projectSpecificFile +
                        "\n\nRemove and delete this file?", "Remove unreferenced file?", MessageBoxButtons.YesNo);

                if (result != DialogResult.Yes) continue;

                ProjectManager.GetProjectByName(projectSpecificFile.ProjectName).ContentProject.RemoveItem(
                    projectSpecificFile.FilePath);

                FileHelper.DeleteFile(ProjectManager.MakeAbsolute(projectSpecificFile.FilePath));
                shouldRefreshAgainst = true;
            }

            if (shouldRefreshAgainst)
            {
                RefreshUnreferencedFiles(false);
            }

            NeedsRefreshOfUnreferencedFiles = false;
            return true;
        }

    }
    

}
