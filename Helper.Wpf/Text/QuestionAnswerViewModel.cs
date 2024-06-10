using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Helper.ServiceGateways.Models;
using Helper.Wpf.Messaging;

namespace Helper.Wpf.Text;

public partial class QuestionAnswerViewModel : ObservableObject
{
    private readonly Query _q;

    private int QueryId => _q.QueryId;
    public string Prompt => _q.Text;
    public int PromptTokens => _q.TokenCount;
    public string ModelUsed => _q.Response?.ModelUsed ?? "?";
    public string Answer => string.Join(Environment.NewLine, _q.Response?.Answers.Select(a => a.Text) ?? []);
    public int AnswerTokens => _q.Response?.TokenCount ?? 0;
    [ObservableProperty] private int? _selectedCategoryId;
    [ObservableProperty] private bool _isExpanded;

    partial void OnSelectedCategoryIdChanged(int? value) => WeakReferenceMessenger.Default.Send(new CategoryChangedMessage { QueryId = QueryId, NewCategoryId = value, });

    public QuestionAnswerViewModel(Query q) { _q = q; SelectedCategoryId = q.CategoryId; }
    public void ToggleExpansion() => IsExpanded = !IsExpanded;
}