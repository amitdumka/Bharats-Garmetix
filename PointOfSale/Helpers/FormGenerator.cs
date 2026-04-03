using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace PointOfSale.Helpers
{
    // Converters used for binding between Entry.Text and numeric properties
    public class NumericConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            if (string.IsNullOrWhiteSpace(str))
            {
                if (targetType == typeof(int) || targetType == typeof(int?)) return 0;
                if (targetType == typeof(decimal) || targetType == typeof(decimal?)) return 0m;
                if (targetType == typeof(double) || targetType == typeof(double?)) return 0d;
                return null;
            }

            try
            {
                if (targetType == typeof(int) || targetType == typeof(int?)) return int.Parse(str);
                if (targetType == typeof(decimal) || targetType == typeof(decimal?)) return decimal.Parse(str);
                if (targetType == typeof(double) || targetType == typeof(double?)) return double.Parse(str);
            }
            catch
            {
                // fallthrough to default
            }

            return value;
        }
    }

    public static class FormGenerator
    {
        // Build a ContentPage with form controls for properties decorated with FormFieldAttribute.
        // Usage: var page = FormGenerator.BuildPage(myModel, "Edit Product", async m => await SaveAsync(m));
        public static ContentPage BuildPage<T>(T model, string title = null, Func<T, System.Threading.Tasks.Task>? onSaveAsync = null, bool includeBackButton = true)
        {
            var result = BuildLayout(model);

            var page = new ContentPage
            {
                Title = title ?? typeof(T).Name,
                Content = new ScrollView { Content = result.layout },
                BindingContext = model
            };

            // Wire buttons: Save, Clear, Cancel/Back
            // Find buttons by searching the layout tree (we created last three buttons in a HorizontalStackLayout)
            var buttons = FindButtons(result.layout).ToList();
            // Buttons appended in order in BuildLayout: Save, Clear, Cancel
            if (buttons.Count >= 1)
            {
                var save = buttons[0];
                save.Clicked += async (s, e) =>
                {
                    // run validation (per-field)
                    var errors = ValidateModel(model, result.controls);
                    if (errors.Any())
                    {
                        result.validationLabel.Text = string.Join('\n', errors);
                        result.validationLabel.IsVisible = true;
                        return;
                    }

                    result.validationLabel.Text = string.Empty;
                    result.validationLabel.IsVisible = false;

                    if (onSaveAsync != null)
                    {
                        try
                        {
                            await onSaveAsync(model);
                        }
                        catch (Exception ex)
                        {
                            // display error
                            result.validationLabel.Text = ex.Message;
                            result.validationLabel.IsVisible = true;
                            return;
                        }
                    }

                    if (page.Navigation?.NavigationStack?.Count > 0)
                        await page.Navigation.PopAsync();
                };
            }

            if (buttons.Count >= 2)
            {
                var clear = buttons[1];
                clear.Clicked += (s, e) =>
                {
                    // Clear to defaults
                    foreach (var entry in result.controls)
                    {
                        var prop = entry.prop;
                        var control = entry.control;
                        var meta = entry.meta;

                        var defaultVal = GetDefault(prop.PropertyType);
                        prop.SetValue(model, defaultVal);

                        // update UI control
                        if (control is Entry en)
                            en.Text = defaultVal?.ToString() ?? string.Empty;
                        else if (control is Editor ed)
                            ed.Text = defaultVal?.ToString() ?? string.Empty;
                        else if (control is DatePicker dp)
                            dp.Date = defaultVal is DateTime dt ? dt : DateTime.Now;
                        else if (control is Switch sw)
                            sw.IsToggled = defaultVal is bool b && b;
                        else if (control is Picker pk)
                        {
                            // if enum meta provided, clear selection
                            pk.SelectedIndex = -1;
                        }
                    }

                    result.validationLabel.Text = string.Empty;
                };
            }

            if (buttons.Count >= 3)
            {
                var cancel = buttons[2];
                cancel.Clicked += async (s, e) =>
                {
                    if (includeBackButton && page.Navigation?.NavigationStack?.Count > 0)
                    {
                        await page.Navigation.PopAsync();
                    }
                };
            }

            return page;
        }

        static IEnumerable<Button> FindButtons(Layout layout)
        {
            foreach (var child in layout.Children)
            {
                if (child is Button b) yield return b;
                if (child is Layout nested)
                {
                    foreach (var nb in FindButtons(nested)) yield return nb;
                }
            }
        }

        public static (StackLayout layout, List<(PropertyInfo prop, View control, object? meta)> controls, Dictionary<string, object?> snapshot) BuildLayout<T>(T model)
        {
            var stack = new StackLayout { Padding = new Thickness(12), Spacing = 8 };

            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new { Prop = p, Attr = p.GetCustomAttribute(typeof(PointOfSale.Models.FormFieldAttribute)) as PointOfSale.Models.FormFieldAttribute })
                .Where(x => x.Attr != null)
                .OrderBy(x => x.Attr.Order)
                .ToList();

            var controls = new List<(PropertyInfo prop, View control, object? meta)>();
            var snapshot = new Dictionary<string, object?>();

            foreach (var item in props)
            {
                var prop = item.Prop;
                var attr = item.Attr;

                // take snapshot
                snapshot[prop.Name] = prop.GetValue(model);

                var label = new Label { Text = attr.Label, FontAttributes = FontAttributes.Bold };
                stack.Children.Add(label);

                View control = null;
                object? meta = null;

                var controlType = (attr.ControlType ?? "Entry").ToLowerInvariant();

                switch (controlType)
                {
                    case "entry":
                        var entry = new Entry { Placeholder = attr.Placeholder };
                        entry.SetBinding(Entry.TextProperty, new Binding(prop.Name, BindingMode.TwoWay));
                        control = entry;
                        break;
                    case "numeric":
                        var num = new Entry { Keyboard = Keyboard.Numeric, Placeholder = attr.Placeholder };
                        num.SetBinding(Entry.TextProperty, new Binding(prop.Name, BindingMode.TwoWay, new NumericConverter(), null));
                        control = num;
                        break;
                    case "phone":
                        var phone = new Entry { Keyboard = Keyboard.Telephone, Placeholder = attr.Placeholder };
                        phone.SetBinding(Entry.TextProperty, new Binding(prop.Name, BindingMode.TwoWay));
                        control = phone;
                        break;
                    case "editor":
                        var editor = new Editor { Placeholder = attr.Placeholder, AutoSize = EditorAutoSizeOption.TextChanges };
                        editor.SetBinding(Editor.TextProperty, new Binding(prop.Name, BindingMode.TwoWay));
                        control = editor;
                        break;
                    case "datepicker":
                        var dp = new DatePicker();
                        dp.SetBinding(DatePicker.DateProperty, new Binding(prop.Name, BindingMode.TwoWay));
                        control = dp;
                        break;
                    case "switch":
                        var sw = new Switch();
                        sw.SetBinding(Switch.IsToggledProperty, new Binding(prop.Name, BindingMode.TwoWay));
                        control = sw;
                        break;
                    case "picker":
                        var picker = new Picker();

                        // If enum, populate items
                        var propType = prop.PropertyType;
                        var enumType = Nullable.GetUnderlyingType(propType) ?? propType;
                        if (enumType.IsEnum)
                        {
                            var values = Enum.GetValues(enumType).Cast<object>().ToList();
                            foreach (var v in values)
                            {
                                picker.Items.Add(v.ToString());
                            }

                            // Bind SelectedIndex by mapping enum value to index
                            picker.SelectedIndexChanged += (s, e) =>
                            {
                                if (picker.SelectedIndex >= 0)
                                {
                                    var val = values[picker.SelectedIndex];
                                    prop.SetValue(model, val);
                                }
                            };

                            // set initial index
                            var current = prop.GetValue(model);
                            if (current != null)
                            {
                                var idx = values.IndexOf(current);
                                picker.SelectedIndex = idx >= 0 ? idx : -1;
                            }

                            meta = values;
                        }

                        control = picker;
                        break;
                    default:
                        var def = new Entry { Placeholder = attr.Placeholder };
                        def.SetBinding(Entry.TextProperty, new Binding(prop.Name, BindingMode.TwoWay));
                        control = def;
                        break;
                }

                if (control != null)
                {
                    stack.Children.Add(control);
                    controls.Add((prop, control, meta));
                }
            }

            // Add Save, Clear and Cancel buttons
            var btnLayout = new HorizontalStackLayout { Spacing = 12 };
            var save = new Button { Text = "Save", HorizontalOptions = LayoutOptions.EndAndExpand };
            var clear = new Button { Text = "Clear", HorizontalOptions = LayoutOptions.Center };
            var cancel = new Button { Text = "Cancel", HorizontalOptions = LayoutOptions.Start };
            btnLayout.Children.Add(save);
            btnLayout.Children.Add(clear);
            btnLayout.Children.Add(cancel);
            stack.Children.Add(btnLayout);

            return (stack, controls, snapshot);
        }

        static object? GetDefault(Type t)
        {
            var nt = Nullable.GetUnderlyingType(t) ?? t;
            if (nt == typeof(string)) return string.Empty;
            if (nt.IsEnum) return null;
            if (nt == typeof(int)) return 0;
            if (nt == typeof(decimal)) return 0m;
            if (nt == typeof(double)) return 0d;
            if (nt == typeof(bool)) return false;
            if (nt == typeof(DateTime)) return DateTime.Now;
            if (nt == typeof(Guid)) return Guid.Empty;
            return null;
        }
    }
}
