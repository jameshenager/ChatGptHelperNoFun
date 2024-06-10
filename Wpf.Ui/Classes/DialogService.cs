using Microsoft.Win32;
using System.Windows.Forms;
using Ookii.Dialogs.WinForms;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Wpf.Ui.Common.Classes;

public class DialogService : IDialogService
{
    public bool ShowConfirmationDialog(string message, string title) => MessageBox.Show(message, title, MessageBoxButtons.YesNo) == DialogResult.Yes;

    public void ShowSaveFileDialog(string title, string filter, string defaultExtension, string fileName, out string? filePath, out string? fileExtension)
    {
        var saveFileDialog = new SaveFileDialog()
        {
            Title = title,
            Filter = filter,
            DefaultExt = defaultExtension,
            FileName = fileName,
        };
        var dialogResult = saveFileDialog.ShowDialog();
        filePath = dialogResult is true ? saveFileDialog.FileName : null;
        fileExtension = dialogResult is true ? saveFileDialog.DefaultExt : null;
    }

    public string? ShowOpenFileDialog(string title, string filter)
    {
        var openFileDialog = new OpenFileDialog { Filter = filter, Title = title, };
        var dialogResult = openFileDialog.ShowDialog();
        return dialogResult is true ? openFileDialog.FileName : null;
    }

    public string[] ShowOpenFilesDialog()
    {
        //ToDo: Add filter/Title parameters
        var openFileDialog = new OpenFileDialog
        {
            Filter = "PDF files (*.pdf)|*.pdf",
            Title = "Select a PDF file",
            Multiselect = true,
        };
        var dialogResult = openFileDialog.ShowDialog();
        return dialogResult is true ? openFileDialog.FileNames : [];
    }

    public string? ShowFolderBrowserDialog()
    {
        var folderDialog = new OpenFolderDialog { Title = "Select Folder", };
        return folderDialog.ShowDialog() == true ? folderDialog.FolderName : null;
    }

    public void ShowMessageBox(string message) => System.Windows.MessageBox.Show(message);
    public void Show(string messageBoxText, string caption, bool isError = true)
    {
        System.Windows.MessageBox.Show(messageBoxText, caption, MessageBoxButton.OK, isError ? MessageBoxImage.Error : MessageBoxImage.Information);
    }

    public string? GetUserString(string prompt)
    {
        var window = new InputDialog() { Content = prompt, };
        var result = window.ShowDialog();
        return result == DialogResult.OK ? window.Input : null;
    }

    public string? GetApiKey()
    {
        var window = new InputDialog() { Content = "Provide API string", };
        var result = window.ShowDialog();
        return result == DialogResult.OK ? window.Input : null;
    }

    public void OpenFile(string pdfPath)
    {
        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = pdfPath,
            UseShellExecute = true,
        };
        System.Diagnostics.Process.Start(psi);
    }
}