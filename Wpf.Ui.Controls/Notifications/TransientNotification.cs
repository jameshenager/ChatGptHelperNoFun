using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Wpf.Ui.Controls.Notifications;

public partial class NotificationManagerViewModel : ObservableObject
{
    private readonly INotificationService _notificationService;

    public ObservableCollection<TransientNotification> TransientNotifications { get; } = [];
    public ObservableCollection<PersistentNotification> PersistentNotifications { get; } = [];

    public NotificationManagerViewModel(INotificationService notificationService)
    {
        _notificationService = notificationService;

        _notificationService.NotificationAdded += NotificationService_NotificationAdded;
        _notificationService.NotificationRemoved += NotificationService_NotificationRemoved;

        WeakReferenceMessenger.Default.Register<RemoveNotificationMessage>(this, RemoveNotificationMessageHandler);

        //What's a better way of adding this for testing? 
        //_notificationService.RaiseNotification(new PersistentNotification("This is a persistent notification."));
    }

    private void RemoveNotificationMessageHandler(object recipient, RemoveNotificationMessage message) => RemoveNotification(message.Notification);

    private void NotificationService_NotificationAdded(object? sender, NotificationEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            switch (e.TransientNotification)
            {
                case TransientNotification transientNotification:
                    TransientNotifications.Add(transientNotification);
                    break;
                case PersistentNotification persistentNotification:
                    PersistentNotifications.Add(persistentNotification);
                    break;
            }
        });
    }

    private void NotificationService_NotificationRemoved(object? sender, NotificationEventArgs e)
    {
        if (Application.Current is null) { return; }
        Application.Current.Dispatcher.Invoke(() =>
        {
            switch (e.TransientNotification)
            {
                case TransientNotification transientNotification:
                    TransientNotifications.Remove(transientNotification);
                    break;
                case PersistentNotification persistentNotification:
                    PersistentNotifications.Remove(persistentNotification);
                    break;
            }
        });
    }

    [RelayCommand]
    public void RemoveNotification(NotificationBase? notification)
    {
        if (notification is null) { return; }
        _notificationService.RemoveNotification(notification);
    }
}

public abstract partial class NotificationBase(string message, NotificationType type, TimeSpan? duration = null)
    : ObservableObject
{
    public string Message { get; set; } = message;
    public NotificationType Type { get; set; } = type;
    public TimeSpan? Duration { get; set; } = duration;
}

public class PersistentNotification(string message) : NotificationBase(message, NotificationType.Persistent);

public enum NotificationType { Transient, Persistent, Error, } //These might not be useful. 

public partial class TransientNotification : NotificationBase, IDisposable
{
    public static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(3);

    [ObservableProperty] private bool _shouldStartFading;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public TransientNotification(string message, TimeSpan? duration) : base(message, NotificationType.Transient)
    {
        Duration ??= duration ?? DefaultDuration;

        var cancellationToken = _cancellationTokenSource.Token;
        Task.Delay(Duration.Value, cancellationToken).ContinueWith(_ =>
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                WeakReferenceMessenger.Default.Send(new RemoveNotificationMessage { Notification = this, });
            }
        }, cancellationToken);

        Task.Delay(Duration.Value - DefaultDuration, cancellationToken).ContinueWith(_ =>
        {
            if (!cancellationToken.IsCancellationRequested) { ShouldStartFading = true; }
        }, cancellationToken);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _cancellationTokenSource.Dispose();
    }
}

public interface INotificationService
{
    event EventHandler<NotificationEventArgs> NotificationAdded;
    event EventHandler<NotificationEventArgs> NotificationRemoved;
    void RaiseNotification(NotificationBase notification);
    void RemoveNotification(NotificationBase transientNotification);
}

public class NotificationService : INotificationService
{
    private readonly List<NotificationBase> _notifications = [];

    public event EventHandler<NotificationEventArgs>? NotificationAdded;
    public event EventHandler<NotificationEventArgs>? NotificationRemoved;

    public void RaiseNotification(NotificationBase notification)
    {
        _notifications.Add(notification);
        NotificationAdded?.Invoke(this, new NotificationEventArgs(notification));
    }

    public void RemoveNotification(NotificationBase transientNotification)
    {
        if (_notifications.Remove(transientNotification))
        {
            NotificationRemoved?.Invoke(this, new NotificationEventArgs(transientNotification));
        }
    }
}

public class NotificationEventArgs(NotificationBase transientNotification) : EventArgs
{
    public NotificationBase TransientNotification { get; } = transientNotification;
}

public class RemoveNotificationMessage //Later this will be for non-transient notifications as well.
{
    public required NotificationBase Notification { get; set; }
}