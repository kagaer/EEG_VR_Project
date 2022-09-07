# EEG in VR Project

Conducting an EEG Study in VR Seminar @UOS summer term 2022

# Git SetUp & Workflows

## Installations

1.) Git
- [Download](https://git-scm.com/) (if not already installed, check for "Git Bash" in the windows search bar)

2.) Kdiff3 Merge Tool
- [Download](https://sourceforge.net/projects/kdiff3/) (note down the installed path, needed later for Git Extensions SetUp)

3.) Git GUI Application: Git Extensions 
- [Download](https://github.com/gitextensions/gitextensions/releases/tag/v3.5.4) (download the .msi file for installation)

4.) Unity Version 2020.3.23f1 
- Installing via Unity Hub > Installs

Everything the Git GUI Application does, can also be done in the Git Bash console.
For the corresponding git bash commands please refer to the following [cheatsheet](https://education.github.com/git-cheat-sheet-education.pdf).

## Set Up Git Environment

1.) Start Up Git Extensions and have a look into the checklist that should pop-up automatically the first time you start. 
Alternatively the checklist can be accessed under Tools > Settings > Git Extensions

![Git Extensions Checklist](documentation/images/git_extensions_1.png)

Make sure to that most of the checklist is green, except for the linux tools part. For fixing open issues click on the 
repair button next to the issue. For the mergetool and difftool give the path to the Kdiff3 tool we installed before.
Anyhow if at some point the application asks for a merge/diff tool, just give the Kdiff3 tool as reference.

While cloning works usually without having to sign in to your github account, for pushing one needs to
authenticate via ssh (or via https). Please follow the following guide for [ssh-authorization](https://git-extensions-documentation.readthedocs.io/en/release-3.4/remote_feature.html) (starting from the chapter "Create SSH key") if needed. 

## Git Workflows

### Cloning Repo 
Only needed to done once. For cloning this repo, copy the https or ssh link under Code and paste
it into the cloning dialogue of git extensions. Choose a target folder on your local device ("Destination").
The rest can left as default.
![Github Clone](documentation/images/Git_clone.png)
![Git Extension Clone](documentation/images/ge_clone.png)

After that the cloning will happen and the project should be findable under recent repositories, the
next git extensions is started again. For opening the repository, double click on the repo. If the repo 
is opened, it should look like this:

![Git Extensions Repo](documentation/images/repo_1.png)

In the center one can see all the (pushed) commits and at which commit every available branch
currently is at. `origin/...` indicates that the branch is a remote branch. 
The other branches are local branches and needs to be pushed to be accessible for others.

We will mostly work with the tools in the toolbar at the top:

![Toolbar](documentation/images/Toolbar.png)

1. Refresh
2. Branch (current branch, click to change local branch)
3. Pulling & Fetching
4. Pushing
5. Staging, committing & viewing current changes
6. Stashing (we probably need to use this, just for reference)
7. Open file explorer starting in the local repository directory
8. Open git bash starting in the local repository directory



### Init Git LFS (for large files)

Before we start with working Git Extensions, we need to initiate Git LFS in the Git bash.
Therefore open the Git Bash and move into the local repository directory (or just open the bash 
within git extensions icon no. 8). 

Execute the following command in the bash to initialize Git LFS

`git lfs install`

### Pull & Fetching

Fetching: get open changes from the remote branch (without merging)
Pull: essentially fetching + merging to get and integrate new changes

For pulling use icon no. 3, while fetching can be found in the dropdown menu and will only 
get the new commits (without merging them).

![Pulling/Fetching](documentation/images/pull.png)

A window will pop up to show the pull progress.

### Committing & Pushing

All changes (exceptions can be defined in the .gitignore) done within the repository will be tracked automatically and can be seen
under icon no. 5. The following window will appear.

![Commit](documentation/images/commit_push.png)

On the top left, all changes are listed, indicating whether something was added (plus), edited (pen),
or removed (minus). In order to commit something, one needs to select and stage the wanted changes from the top left (multiselection possible).
Double arrow for unstage/stage all. Only staged changes can be committed. Dont forget to include a meaningful commit message! :)
After committing some changes, the commit should show up in the commit history. 

To push the the changes use icon no. 4. Before pushing it is good practice to pull once before pushing 
to get potential pending changes.

The stage area can be also used to revert unwanted changes (right click).
BE CAREFUL AND COMMIT WANTED CHANGES BEFORE REVERTING!

### Branching & Merging

![Branches](documentation/images/branches.png)

All local and remote branches can be seen on the left side on git extension. For checkout a local branch
double click the wanted branch (the currently checked out branch will be displayed in bold font). You will automatically work in one branch when opening a repository. 

Please create a separate branch for each feature to work on (very small changes can be committed to the main directly). The main branch should only contain 
features/content that is working/finished. For creating the branch, select the commit to start of and
right click on it to create new branch (like in the picture).

After finishing work on one feature branch, the changes can be merged into the main branch by
checking out the main branch and merging the feature branch:

![Branches](documentation/images/Merging.png)

If auto merging fails, a merge-conflict will arises which we will need to solve manually using the KDiff3 tool.
After successful merging the changes into the main branch, the feature branch can be removed (both locally and remotely).

### General workflow

To summarize the general workflow assuming the environment is completely set up:

1. Create a new feature branch for the new feature/content
2. Check-out feature branch and push it
3. Perform changes in the new branch
4. Stages & Commit wanted changes
5. If needed: revert unwanted changes
6. Pull changes remotely (origin/feature_branch => feature_branch)
7. Push changes remotely (feature_branch => origin/feature_branch)
8. Checkout main branch
9. Pull main branch
10. [Optional: create a pull request on github and link the associated github issue(s) to it to automatically close the issue(s)]
11. Merge feature branch into main branch
12. If needed: solve merge-conflicts
13. Commit merging changes
14. Push merging changes
15. If needed: close associated github issue manually, if it was not closed automatically
16. Repeat 1. for new feature/content



