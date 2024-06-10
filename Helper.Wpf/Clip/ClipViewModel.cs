using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Helper.Core;

namespace Helper.Wpf.Clip;

public partial class ClipViewModel() { }

public partial class Annotation : ObservableObject
{
    public Guid Id { get; init; }
    public List<AnnotationPoint> AnnotationPoints { get; set; } = [];
    [ObservableProperty] private string _text = null!;
}
public class AnnotationsSaveFile
{
    public List<Annotation> Annotations { get; init; } = [];
    public required string FilePath { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public double Width { get; init; }
    public double Height { get; init; }
}

public class VideoMemeInfo
{
    public required string OriginalVideoPath { get; init; }
    public required string TrimmedVideoPath { get; init; }
    public required string AnnotationsPath { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public List<string> Tags { get; init; } = [];
    public required string Title { get; init; }
}