namespace Leipzig3DGodot.Scripts.MVVM.View;

class SelectionFilterConverter : MVVMConverter
{
    public ISelectable? Reference { get; set; }

    public override object? Convert(object? sourceValue, object? targetValue, string param)
    {
        return Reference == null || Reference.IsSelected() ? base.Convert(sourceValue, targetValue, param) : null;
    }

    public override object? ConvertBack(object? sourceValue, object? targetValue, string param)
    {
        return Reference == null || Reference.IsSelected() ? base.ConvertBack(sourceValue, targetValue, param) : null;
    }
}