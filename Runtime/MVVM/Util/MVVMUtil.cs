using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using CommunityToolkit.Mvvm.Input;
using Godot;
using Godot.Collections;
using Leipzig3DGodot.Scripts.MVVM.ViewModel;

namespace Leipzig3DGodot.Scripts.MVVM.View;

///<author email="aron.schaub@stud.htwk-leipzig.de$">Aron Schaub</author>
public static class MVVMUtil
{
    //TODO: make this a MVVMComponent
    // -> make components like MVVMLabel to behave more like MVVMLantern
    // -> remove the property duplication and instead access the properties on reference component directly
    public const string KEY_BINDING = "_binding";
    public const string KEY_DIRECTION = "_direction";
    public const string KEY_CONVERTER = "_converter";
    public const string KEY_PARAM = "_param";
    public const string NONE = "-None-";

    public static Array<Dictionary> PropertyListFor<T>(T viewModel, Array<string> bindingFields)
    {
        var arr = new Array<Dictionary>();
        if (viewModel != null)
        {
            string properties = NONE + "," + string.Join(',', viewModel.GetType()
                .GetProperties()
                .Where(p => p.GetCustomAttribute(typeof(GeneratedCodeAttribute)) != null || p.GetCustomAttribute(typeof(ExportAttribute)) != null)
                .Select(p => p.Name)
            );
            string converters = string.Join(',', MVVMConverter.Converters.Keys);

            foreach (string bindingField in bindingFields)
            {
                arr.Add(new Dictionary
                {
                    { "name", bindingField },
                    { "prefix", bindingField },
                    { "type", Variant.From(Variant.Type.Nil) },
                    { "usage", Variant.From(PropertyUsageFlags.Group | PropertyUsageFlags.Editor) }
                });
                arr.Add(new Dictionary
                {
                    { "name", bindingField + KEY_BINDING },
                    { "prefix", bindingField },
                    { "type", Variant.From(Variant.Type.String) },
                    { "hint", Variant.From(PropertyHint.Enum) },
                    { "hint_string", properties },
                    { "usage", Variant.From(PropertyUsageFlags.Editor | PropertyUsageFlags.Storage) }
                });
                arr.Add(new Dictionary
                {
                    { "name", bindingField + KEY_DIRECTION },
                    { "prefix", bindingField },
                    { "type", Variant.From(Variant.Type.Int) },
                    { "hint", Variant.From(PropertyHint.Enum) },
                    { "hint_string", string.Join(",", Enum.GetNames<MVVMDirection>()) },
                    { "usage", Variant.From(PropertyUsageFlags.Editor | PropertyUsageFlags.Storage) }
                });
                arr.Add(new Dictionary
                {
                    { "name", bindingField + KEY_CONVERTER },
                    { "prefix", bindingField },
                    { "type", Variant.From(Variant.Type.String) },
                    { "hint", Variant.From(PropertyHint.Enum) },
                    { "hint_string", converters },
                    { "usage", Variant.From(PropertyUsageFlags.Editor | PropertyUsageFlags.Storage) }
                });
                arr.Add(new Dictionary
                {
                    { "name", bindingField + KEY_PARAM },
                    { "prefix", bindingField },
                    { "type", Variant.From(Variant.Type.String) },
                    { "usage", Variant.From(PropertyUsageFlags.Editor | PropertyUsageFlags.Storage) }
                });
            }
        }

        return arr;
    }

    public static bool GetProperty(string property, out Variant s, IEnumerable<string> bindingFields, Godot.Collections.Dictionary<StringName, MVVMBean> bindingDict)
    {
        if (bindingDict.Any(entry => CleanKey(property).Equals(entry.Key)))
        {
            KeyValuePair<StringName, MVVMBean> entry = bindingDict.FirstOrDefault(entry => CleanKey(property).Equals(entry.Key));
            if (property.Contains(KEY_BINDING))
            {
                s = string.IsNullOrEmpty(entry.Value.Binding) ? NONE : entry.Value.Binding;
            }
            else if (property.Contains(KEY_DIRECTION))
                s = Variant.From(entry.Value.Direction);
            else if (property.Contains(KEY_CONVERTER))
                s = entry.Value.Converter;
            else if (property.Contains(KEY_PARAM))
                s = entry.Value.Param;
            else
            {
                s = default;
                return false;
            }

            return true;
        }

        s = default;
        return false;
    }

    public static bool SetProperty(string property, Variant value, IEnumerable<string> bindingFields, Godot.Collections.Dictionary<StringName, MVVMBean> bindingDict)
    {
        foreach (string field in bindingFields.Where(s => s.Equals(CleanKey(property))))
        {
            if (!bindingDict.ContainsKey(field))
            {
                bindingDict.Add(field, new MVVMBean());
            }

            // if (value.Obj is MVVMBean bean)
            // {
            //     bindingDict[field] = bean;
            //     return true;
            // }

            if (property.EndsWith(KEY_BINDING))
                bindingDict[field].Binding = value.AsString();
            else if (property.EndsWith(KEY_DIRECTION))
                bindingDict[field].Direction = value.As<MVVMDirection>();
            else if (property.EndsWith(KEY_CONVERTER))
                bindingDict[field].Converter = value.AsString();
            else if (property.EndsWith(KEY_PARAM))
                bindingDict[field].Param = value.AsString();
            else
                continue;

            return true;
        }

        return false;
    }

    private static string CleanKey(string key)
    {
        return key.Replace(KEY_PARAM, string.Empty).Replace(KEY_BINDING, string.Empty).Replace(KEY_CONVERTER, string.Empty).Replace(KEY_DIRECTION, string.Empty);
    }

    public static void OnValueChanged<T>(T? viewModel, Node node, string? key, Godot.Collections.Dictionary<StringName, MVVMBean> bindingDict)
    {
        if (string.IsNullOrEmpty(key))
            return;
        var nodePropertyInfo = node.GetType().GetProperty(key);
        object? sourceValue = nodePropertyInfo?.GetValue(node);
        OnValueChanged(viewModel, node, key, sourceValue, bindingDict);
    }

    public static void OnValueChanged<T>(T? viewModel, Node node, string? key, object sourceValue, Godot.Collections.Dictionary<StringName, MVVMBean> bindingDict)
    {
        if (viewModel == null || !MVVMBean.BindingDictContains(key, bindingDict, out var bean)) return;
        if (!bean.HasBinding()) return;
        if (bean.IsCommand())
        {
            HandleCommand(viewModel, node, key, sourceValue, bean);
        }
        else
        {
            HandleValues(viewModel, key, sourceValue, bean);
        }
    }

    private static void HandleValues<T>([DisallowNull] T viewModel, string key, object sourceValue, MVVMBean bean)
    {
        try
        {
            var viewModelPropertyInfo = viewModel.GetType().GetProperty(bean.Binding);
            object? targetValue = viewModelPropertyInfo?.GetValue(viewModel);
            try
            {
                var conv = MVVMConverter.Default;
                if (MVVMConverter.Converters.ContainsKey(bean.Converter))
                    conv = MVVMConverter.Converters[bean.Converter];
                object? convertBack = conv.ConvertBack(sourceValue, targetValue, bean.Param);
                if (convertBack != null)
                    viewModelPropertyInfo?.SetValue(viewModel, convertBack);
            }
            catch (Exception exception)
            {
                GD.PrintErr($"{MethodBase.GetCurrentMethod()?.Name}: {exception.GetType()} of {key}->{bean.Binding} from {sourceValue} ({sourceValue.GetType()}) to {targetValue} ({targetValue?.GetType()})");
            }
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
        }
    }


    private static void HandleCommand<T>(T? viewModel, Node node, string? key, object sourceValue, MVVMBean bean)
    {
        var viewModelPropertyInfo = viewModel.GetType().GetProperty(bean.Binding);
        object? targetValue = viewModelPropertyInfo?.GetValue(viewModel);
        try
        {
            switch (targetValue)
            {
                case RelayCommand<bool> baseCommand when sourceValue is bool && baseCommand.CanExecute(sourceValue):
                    baseCommand.Execute(sourceValue);
                    break;
                case RelayCommand<string> baseCommand when sourceValue is string && baseCommand.CanExecute(sourceValue):
                    baseCommand.Execute(sourceValue);
                    break;
                case RelayCommand<string> baseCommand when baseCommand.CanExecute(bean.Param):
                    baseCommand.Execute(bean.Param);
                    break;
                case RelayCommand<Node> baseCommand when baseCommand.CanExecute(node):
                    baseCommand.Execute(node);
                    break;
                case RelayCommand baseCommand when baseCommand.CanExecute(sourceValue):
                    baseCommand.Execute(sourceValue);
                    break;
            }
        }
        catch (Exception exception)
        {
            GD.PrintErr($"{MethodBase.GetCurrentMethod()?.Name}: {exception.GetType()} of {viewModel?.GetType().Name}.{key}->{node.Name}.{bean.Binding} from {sourceValue} ({sourceValue.GetType()}) to {targetValue} ({targetValue?.GetType()})");
        }
    }

    public static void OnPropertyChange<T>(T? viewModel, Node node, string? propertyName, Godot.Collections.Dictionary<StringName, MVVMBean> bindingDict)
    {
        try
        {
            if (viewModel != null && !string.IsNullOrEmpty(propertyName) && !NONE.Equals(propertyName))
            {
                KeyValuePair<StringName?, MVVMBean> entry = bindingDict.FirstOrDefault(entry => entry.Value.Binding == propertyName)!;
                if (!string.IsNullOrEmpty(propertyName) && entry.Key != null)
                {
                    object? sourceValue = viewModel.GetType().GetProperty(propertyName)?.GetValue(viewModel);
                    var nodePropertyInfo = node.GetType().GetProperty(entry.Key);
                    object? targetValue = nodePropertyInfo?.GetValue(node);

                    try
                    {
                        var conv = MVVMConverter.Default;
                        if (MVVMConverter.Converters.ContainsKey(entry.Value.Converter))
                            conv = MVVMConverter.Converters[entry.Value.Converter];
                        if (conv is SelectionFilterConverter converter && node.GetParent() is ISelectable selectable)
                            converter.Reference = selectable;
                        object? convert = conv.Convert(sourceValue, targetValue, entry.Value.Param);
                        if (convert != null)
                            nodePropertyInfo?.SetValue(node, convert);
                    }
                    catch (Exception exception)
                    {
                        GD.PrintErr($"{MethodBase.GetCurrentMethod()?.Name}: {exception.GetType()} of {viewModel?.GetType().Name}.{propertyName}->{node.Name}.{entry.Key} from {sourceValue} ({sourceValue?.GetType()}) to {targetValue} ({targetValue?.GetType()})");
                    }
                }
            }
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
        }
    }

    public static void OnReady<T>(T? viewModel, Node node, Godot.Collections.Dictionary<StringName, MVVMBean> bindingDict)
    {
        try
        {
            foreach ((string key, var value) in bindingDict)
            {
                if (!value.IsCommand())
                    OnPropertyChange(viewModel, node, value.Binding, bindingDict);
            }
        }
        catch (InvalidCastException e)
        {
            GD.PrintErr(e);
            bindingDict.Clear();
        }
    }
}