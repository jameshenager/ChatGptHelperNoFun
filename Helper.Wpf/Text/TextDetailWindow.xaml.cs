using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Helper.Wpf.Text;

public partial class TextDetailWindow
{
    public TextDetailWindow(TextDetailWindowViewModel questionAnswer)
    {
        DataContext = questionAnswer;
        InitializeComponent();
        questionAnswer.CloseWindow = CloseThisWindow;
    }
    public void CloseThisWindow() => Close();
}

public partial class TextDetailWindowViewModel(QuestionAnswerViewModel questionAnswerViewModel) : ObservableObject
{
    public QuestionAnswerViewModel QuestionAnswerViewModel { get; } = questionAnswerViewModel;
    public string Prompt => QuestionAnswerViewModel.Prompt;
    public string Answer => QuestionAnswerViewModel.Answer;
    public Action? CloseWindow { get; set; }
    [RelayCommand] public void Close() /* Ctrl+W */ => CloseWindow?.Invoke();
}