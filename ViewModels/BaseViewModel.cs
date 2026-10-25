using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PharmaGarde.ViewModels;

// Base de tous les ViewModels : prévient la Vue quand une propriété change (INotifyPropertyChanged)
public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged; // la Vue s'abonne à cet événement

    // change la valeur d'un champ et prévient la Vue ; renvoie vrai si la valeur a vraiment changé
    protected bool SetProperty<T>(ref T champ, T valeur, [CallerMemberName] string? nom = null)
    {
        if (EqualityComparer<T>.Default.Equals(champ, valeur)) return false; // rien n'a changé
        champ = valeur;                                                       // enregistre la nouvelle valeur
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nom));     // prévient la Vue
        return true;
    }
}