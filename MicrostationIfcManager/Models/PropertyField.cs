using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;


public enum EditorType
{
    String,
    Double,
    Int,
    Bool,
    Combo
}

public class PropertyField : INotifyPropertyChanged
{
    public string Name { get; }
    public string DataType { get; }

    public EditorType EditorType { get; }

    public ObservableCollection<string> LookupValues { get; set; } = new ObservableCollection<string>();
    public ObservableCollection<string> SourceLookupValues { get; }

    private object _value;
    public object Value
    {
        get => _value;
        set
        {
            _value = value;
            Changed = true;
            OnPropertyChanged();
        }
    }

    private bool changed;

    public bool Changed
    {
        get { return changed; }
        set
        {
            changed = value;
            OnPropertyChanged();
        }
    }

    private bool canBeEdited;

    public bool CanBeEdited
    {
        get { return canBeEdited; }
        set
        {
            canBeEdited = value;
            OnPropertyChanged();
        }
    }

    private bool isReadOnly;

    public bool IsReadOnly
    {
        get { return isReadOnly; }
        set
        {
            isReadOnly = value;
            OnPropertyChanged();
        }
    }

    public PropertyField(
        string name,
        string dataType,
        IEnumerable<string> lookupValues = null)
    {
        Name = name;
        DataType = dataType;

        LookupValues = lookupValues != null
            ? new ObservableCollection<string>(lookupValues)
            : null;

        SourceLookupValues = lookupValues != null
            ? new ObservableCollection<string>(lookupValues)
            : null;

        EditorType = ResolveEditorType(dataType, lookupValues);

        CanBeEdited = true;
        IsReadOnly = false;
    }

    private static EditorType ResolveEditorType(string dataType, IEnumerable<string> lookup)
    {
        if (lookup != null)
            return EditorType.Combo;

        switch (dataType.ToLower())
        {
            case "bool":
                return EditorType.Bool;
            case "int":
                return EditorType.Int;
                case "double":
                    return EditorType.Double;
                case "string":
                    return EditorType.String;
            default:
                break;
        }

        return EditorType.String;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
