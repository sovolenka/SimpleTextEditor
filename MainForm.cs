namespace SimpleTextEditor;

public partial class MainForm : Form
{
    private bool _isChanged;
    private string _filePath;
    private readonly HashSet<string> _recentFiles;

    public MainForm()
    {
        InitializeComponent();
        _isChanged = false;
        _filePath = string.Empty;
        _recentFiles = new HashSet<string>();
    }

    private void AddRecentFile(string filePath)
    {
        if (_recentFiles.Contains(filePath))
        {
            _recentFiles.Remove(filePath);
        }
        else if (_recentFiles.Count >= 10)
        {
            _recentFiles.Remove(_recentFiles.First());
        }

        _recentFiles.Add(filePath);

        OnRecentFilesChanged();
    }

    private void OnRecentFilesChanged()
    {
        // delete previous recent files
        recentFilesToolStripMenuItem.DropDownItems.Clear();
        
        foreach (var filePath in _recentFiles)
        {
            var fileName = filePath.Split(@"\")[^1];
            ToolStripMenuItem recentFileMenuItem = new(fileName);
            recentFileMenuItem.Click += (sender, args) =>
            {
                Text = fileName;
                richTextBox1.Text = File.ReadAllText(filePath);
                _isChanged = false;
                _filePath = filePath;
            };
            
            recentFilesToolStripMenuItem.DropDownItems.Add(recentFileMenuItem);
        }
    }

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (_filePath != string.Empty)
        {
            AddRecentFile(_filePath);
        }
        
        // use file dialog
        FileDialog fileDialog = new OpenFileDialog();
        fileDialog.Filter = @"Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
        fileDialog.FilterIndex = 1;
        fileDialog.RestoreDirectory = true;
        fileDialog.Title = @"Open File";
        fileDialog.ShowDialog();

        // open file
        if (fileDialog.FileName == string.Empty) return;
        Text = fileDialog.FileName.Split(@"\")[^1];
        richTextBox1.Text = File.ReadAllText(fileDialog.FileName);
        _isChanged = false;
        _filePath = fileDialog.FileName;
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (_filePath != string.Empty)
        {
            File.WriteAllText(_filePath, richTextBox1.Text);
            _isChanged = false;
        }
        else
        {
            saveAsToolStripMenuItem_Click(sender, e);
        }
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (_filePath != string.Empty)
        {
            AddRecentFile(_filePath);
        }
        
        // use file dialog
        FileDialog fileDialog = new SaveFileDialog();
        fileDialog.Filter = @"Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
        fileDialog.FilterIndex = 1;
        fileDialog.RestoreDirectory = true;
        fileDialog.Title = @"Save File";
        fileDialog.ShowDialog();

        // save file
        if (fileDialog.FileName == string.Empty) return;
        File.WriteAllText(fileDialog.FileName, richTextBox1.Text);
        _isChanged = false;
        Text = fileDialog.FileName.Split(@"\")[^1];
        _filePath = fileDialog.FileName;
    }

    private void richTextBox1_TextChanged(object sender, EventArgs e)
    {
        _isChanged = true;
    }

    private void MainForm_OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!_isChanged) return;
        var dialogResult = MessageBox.Show(@"Do you want to save changes?", @"Save Changes",
            MessageBoxButtons.YesNoCancel);

        switch (dialogResult)
        {
            case DialogResult.Yes:
                // save
                if (sender != null) saveToolStripMenuItem_Click(sender, e);
                break;
            case DialogResult.No:
                // don't save
                break;
            case DialogResult.None:
            case DialogResult.OK:
            case DialogResult.Cancel:
            case DialogResult.Abort:
            case DialogResult.Retry:
            case DialogResult.Ignore:
            case DialogResult.TryAgain:
            case DialogResult.Continue:
            default:
                // cancel
                e.Cancel = true;
                break;
        }
    }

    private void authorToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var caption = "Author";
        var message = "Olena Stoliar";
        var buttonOk = MessageBoxButtons.OK;

        MessageBox.Show(message, caption, buttonOk);
    }
}
