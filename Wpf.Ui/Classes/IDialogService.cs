namespace Wpf.Ui.Common.Classes;

public interface IDialogService
{
    void Show(string messageBoxText, string caption, bool isError);
    string[] ShowOpenFilesDialog();
    string? ShowOpenFileDialog(string title, string filter);
    string? ShowFolderBrowserDialog();
    void ShowMessageBox(string message);
    string? GetApiKey(); //I should replace this with a more generic method, GetUserString
    string? GetUserString(string prompt);
    void OpenFile(string pdfPath);
    bool ShowConfirmationDialog(string message, string title);
    void ShowSaveFileDialog(string title, string filter, string defaultExtension, string fileName, out string? filePath, out string? fileExtension);
}