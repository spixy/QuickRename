﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoRename
{
	/// <summary>
	/// GUI edit type
	/// </summary>
	public enum EditType
	{
		FileName,
		UpperCase,
		Brackets,
		StartingNumber,
		Visual
	}

    public class MainViewModel : INotifyPropertyChanged
    {
	    private readonly FileNameProcessor fileNameProcessor;

		public MainViewModel(FileNameProcessor fileNameProcessor)
		{
			this.fileNameProcessor = fileNameProcessor;
		}

		public event PropertyChangedEventHandler PropertyChanged = (sender, args) =>
	    {
		    MainViewModel model = (MainViewModel) sender;

	        switch (args.PropertyName)
	        {
	            case "SelectedItem":
	                model.CheckGuiStates();
	                break;

	            case "StartWithUpperCase":
	                foreach (var row in model.DataGridRows)
	                {
	                    row.StartWithUpperCase = model.StartWithUpperCase;
	                }
	                break;

	            case "RemoveBrackets":
	                foreach (var row in model.DataGridRows)
	                {
	                    row.RemoveBrackets = model.RemoveBrackets;
	                }
	                break;

	            case "RemoveStartingNumber":
	                foreach (var row in model.DataGridRows)
	                {
	                    row.RemoveStartingNumber = model.RemoveStartingNumber;
	                }
	                break;

	            case "ShowFullPath":
	                foreach (var row in model.DataGridRows)
	                {
	                    row.ShowFullPath = model.showFullPath;
	                }
	                break;

	            case "ShowExtension":
	                foreach (var row in model.DataGridRows)
	                {
	                    row.ShowExtension = model.ShowExtension;
	                }
	                break;
	        }
	    };

	    private void OnPropertyChanged(string propertyName)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }

	    /// <summary>
	    /// DataGrid Rows
	    /// </summary>
	    private ObservableCollection<GridRowViewModel> dataGridRows = new ObservableCollection<GridRowViewModel>();
	    public ObservableCollection<GridRowViewModel> DataGridRows
	    {
		    get => dataGridRows;
		    set
		    {
			    dataGridRows = value;
			    OnPropertyChanged("DataGridRows");
		    }
	    }

	    /// <summary>
        /// Window title
        /// </summary>
        public string Title
        {
            get
            {
	            string name = Utility.CurrentApplication.Name;
	            Version version = Utility.CurrentApplication.Version;
	            return $"{name} {version.Major}{(version.Minor > 0 ? "." + version.Minor : "")}{(version.Build > 0 ? "." + version.Build : "")}";
            }
        }

	    /// <summary>
	    /// Website button caption
	    /// </summary>
	    private string websiteButton = Properties.Resources.WebsiteButtonVisit;
	    public string WebsiteButton
	    {
		    get => websiteButton;
		    set
			{
				websiteButton = value;
				OnPropertyChanged("WebsiteButton");
			}
	    }

	    /// <summary>
	    /// Selected row in DagaGrid
	    /// </summary>
	    private GridRowViewModel selectedItem;
	    public GridRowViewModel SelectedItem
	    {
		    get => selectedItem;
		    set
			{
				selectedItem = value;			
				OnPropertyChanged("SelectedItem");
			}
	    }

	    /// <summary>
	    /// Start wit upper case checkbox
	    /// </summary>
	    public bool StartWithUpperCase
	    {
			get => fileNameProcessor.StartWithUpperCase;
		    set
		    {
				fileNameProcessor.StartWithUpperCase = value;
				OnPropertyChanged("StartWithUpperCase");
		    }
	    }

	    /// <summary>
	    /// Remove brackets
		/// </summary>
	    public bool RemoveBrackets
	    {
			get => fileNameProcessor.RemoveBrackets;
		    set
			{
				fileNameProcessor.RemoveBrackets = value;
			    OnPropertyChanged("RemoveBrackets");

		    }
	    }

		public bool RemoveStartingNumber
		{
			get => fileNameProcessor.RemoveStartingNumber;
			set
			{
				fileNameProcessor.RemoveStartingNumber = value;
				OnPropertyChanged("RemoveStartingNumber");
			}
		}

        /// <summary>
        /// Show extension checkbox
        /// </summary>
        private bool showExtension;
        public bool ShowExtension
	    {
		    get => showExtension;
	        set
		    {
		        showExtension = value;
				OnPropertyChanged("ShowExtension");
		    }
	    }

        /// <summary>
        /// Show full path checkbox
        /// </summary>
        private bool showFullPath;
        public bool ShowFullPath
	    {
		    get => showFullPath;
	        set
		    {
		        showFullPath = value;
				OnPropertyChanged("ShowFullPath");
		    }
	    }

	    /// <summary>
	    /// Show full path checkbox
	    /// </summary>
	    private bool showGridLines;
	    public bool ShowGridLines
	    {
		    get => showGridLines;
		    set
		    {
		        showGridLines = value;
			    OnPropertyChanged("ShowGridLines");
		        OnPropertyChanged("GridLinesVisibility");
            }
	    }

        public DataGridGridLinesVisibility GridLinesVisibility => ShowGridLines ? DataGridGridLinesVisibility.All : DataGridGridLinesVisibility.None;

	    /// <summary>
        /// Show full path checkbox
        /// </summary>
        private bool exitAfterRename;
        public bool ExitAfterRename
        {
            get => exitAfterRename;
	        set
            {
                exitAfterRename = value;
                OnPropertyChanged("ExitAfterRename");
            }
        }

        /// <summary>
        /// Rename button
        /// </summary>
        private RenameCommand renameButtonCommand;
	    public ICommand RenameButtonClick => renameButtonCommand ?? (renameButtonCommand = new RenameCommand(this));

	    /// <summary>
	    /// Add file button
	    /// </summary>
	    private AddFileCommand addFileCommand;
	    public ICommand AddFileButtonClick => addFileCommand ?? (addFileCommand = new AddFileCommand(this));

	    /// <summary>
	    /// Website button
	    /// </summary>
	    private WebsiteCommand websiteCommand;
	    public ICommand WebsiteButtonClick => websiteCommand ?? (websiteCommand = new WebsiteCommand());

	    /// <summary>
	    /// Rename button Enabled / Disabled
	    /// </summary>
	    private bool renameButtonEnabled;
	    public bool RenameButtonEnabled
	    {
		    get => renameButtonEnabled;
		    set
		    {
			    renameButtonEnabled = value;
			    OnPropertyChanged("RenameButtonEnabled");
		    }
	    }

	    private bool contextMenuEnabled;
		public bool ContextMenuEnabled
	    {
			get => contextMenuEnabled;
			set
			{
				contextMenuEnabled = value;
				OnPropertyChanged("ContextMenuEnabled");
			}
		}

		private Visibility rowSettingsEnabled = Visibility.Collapsed;
		public Visibility RowSettingsEnabled
		{
			get => rowSettingsEnabled;
			set
			{
				rowSettingsEnabled = value;
				OnPropertyChanged("RowSettingsEnabled");
			}
		}

	    /// <summary>
	    /// Add files to table
	    /// </summary>
	    /// <param name="files">file collection</param>
	    public void AddFiles(IEnumerable<string> files)
	    {
		    foreach (string file in files)
		    {
			    AddFile(file);
		    }
	    }

	    /// <summary>
	    /// Add file to table
	    /// </summary>
	    /// <param name="file">file path</param>
	    public void AddFile(string file)
	    {
		    if (FileAlreadyLoaded(file))
		    {
			    return;
		    }

		    try
		    {
			    GridRowViewModel newRow = new GridRowViewModel(this, file, fileNameProcessor);
			    DataGridRows.Add(newRow);
			    CheckGuiStates();
		    }
		    catch
		    {
			    SystemSounds.Beep.Play();
		    }
	    }

		private void CheckGuiStates()
		{
			RenameButtonEnabled = ContextMenuEnabled = DataGridRows.Count > 0;
			RowSettingsEnabled = SelectedItem != null ? Visibility.Visible : Visibility.Collapsed;
		}

	    private bool FileAlreadyLoaded(string file)
	    {
		    foreach (GridRowViewModel item in DataGridRows)
		    {
			    if (item.OldFullPath == file)
			    {
				    return true;
			    }
		    }
		    return false;
	    }

	    public void RemoveSelected()
	    {
		    DataGridRows.Remove(SelectedItem);
		    CheckGuiStates();
	    }

	    public void RemoveAll()
	    {
			DataGridRows = new ObservableCollection<GridRowViewModel>();
		    CheckGuiStates();
	    }

	    public void RenameAll()
	    {
	        if (DataGridRows.Count == 0)
	        {
	            return;
	        }

		    bool success = true;

	        foreach (GridRowViewModel row in DataGridRows)
	        {
	            bool renamed = row.Rename();
	            if (renamed)
	            {
	                DataGridRows.Remove(row);
	            }
	            success &= renamed;
	        }

		    if (success)
		    {
		        if (ExitAfterRename)
		        {
		            Application.Current.Shutdown();
                }
		    }
		    else
		    {
			    SystemSounds.Beep.Play();
		    }
	    }
	}
}