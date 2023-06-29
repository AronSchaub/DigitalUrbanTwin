using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Godot;
using Godot.Collections;

namespace Leipzig3DGodot.Scripts.MVVM.View;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public partial class MVVMBean : Resource
{
    public string Binding { get; set; } = MVVMUtil.NONE;
    public MVVMDirection Direction { get; set; } = MVVMDirection.INOUT;
    public string Converter { get; set; }
    public string Param { get; set; } = string.Empty;

    public bool IsCommand()
    {
        return HasBinding() && Binding.EndsWith("Command");
    }

    public bool HasBinding()
    {
        return !string.IsNullOrEmpty(Binding) && !MVVMUtil.NONE.Equals(Binding);
    }

    public static bool BindingDictContains(string? propertyName, Dictionary<StringName, MVVMBean> bindingDict, out MVVMBean bean)
    {
        foreach (var bindingDictKey in bindingDict.Keys)
        {
            if (bindingDictKey + MVVMUtil.KEY_BINDING == propertyName || bindingDictKey + MVVMUtil.KEY_DIRECTION == propertyName || bindingDictKey + MVVMUtil.KEY_CONVERTER == propertyName || bindingDictKey + MVVMUtil.KEY_PARAM == propertyName)
            {
                bean = bindingDict[bindingDictKey];
                return true;
            }
        }

        bean = default;
        return false;
    }

    // public void SetNodeValue<T>([DisallowNull] T obj, object? sourceValue, object? targetValue, PropertyInfo? propertyInfo)
    // {
    //     propertyInfo?.SetValue(obj, Converter.Convert(sourceValue, Param));
    // }
    //
    // public void SetViewModelValue<T>([DisallowNull] T obj, object? sourceValue, object? targetValue, PropertyInfo? propertyInfo)
    // {
    //     propertyInfo?.SetValue(obj, Converter.ConvertBack(sourceValue, Param));
    // }

    // public void SetValue<T>([DisallowNull] T obj, object? sourceValue, object? targetValue, PropertyInfo? propertyInfo, Func<object?, string, object> converter)
    // {
    //     if (sourceValue?.GetType() == targetValue?.GetType())
    //         propertyInfo?.SetValue(obj, converter(sourceValue, Param));
    //     else if (typeof(string) == targetValue?.GetType())
    //         propertyInfo?.SetValue(obj, converter(sourceValue, Param));
    //     else if (typeof(int) == targetValue?.GetType())
    //         propertyInfo?.SetValue(obj, converter(sourceValue, Param));
    //     else if (typeof(float) == targetValue?.GetType())
    //         propertyInfo?.SetValue(obj, converter(sourceValue, Param));
    //     else if (typeof(double) == targetValue?.GetType())
    //         propertyInfo?.SetValue(obj, converter(sourceValue, Param));
    //     else if (typeof(bool) == targetValue?.GetType())
    //         propertyInfo?.SetValue(obj, converter(sourceValue, Param));
    //     else
    //         GD.PrintErr($"unsupported type conversion: {sourceValue?.GetType()} -> {targetValue?.GetType()}");
    // }
}