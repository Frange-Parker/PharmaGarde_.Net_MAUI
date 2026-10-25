using System.Windows.Input;

namespace PharmaGarde.ViewModels;

// Une commande : une action que la Vue peut déclencher par binding (bouton, toucher...)
public class RelayCommand : ICommand
{
    private readonly Func<Task> _action; // l'action à exécuter

    public RelayCommand(Func<Task> action) { _action = action; } // reçoit l'action

    public event EventHandler? CanExecuteChanged { add { } remove { } } // la commande est toujours exécutable
    public bool CanExecute(object? parameter) => true;
    public async void Execute(object? parameter) => await _action(); // exécute l'action
}