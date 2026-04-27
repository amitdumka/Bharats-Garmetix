using Syncfusion.Maui.DataForm;

namespace Garmetix.Core.Views.Customs.Forms
{
    public partial class EntryFormView : ContentView
    {
        public SfDataForm DataForm => dataForm;

        public EntryFormView()
        {
            ValidationMode = "PropertyChanged";
            Title = "EntryForm";
            SideImage = "thearvindstore005.jpg";
            ColumnCount = 3;
            InitializeComponent();
        }

        #region Properties

        public string ValidationMode
        {
            get => (string)GetValue(ValidationModeProperty);
            set => SetValue(ValidationModeProperty, value);
        }

        public static readonly BindableProperty ValidationModeProperty = BindableProperty.Create(nameof(ValidationMode), typeof(string), typeof(EntryFormView), "PropertyChanged");

        public string LayoutType
        {
            get => (string)GetValue(LayoutTypeProperty);
            set => SetValue(LayoutTypeProperty, value);
        }

        public static readonly BindableProperty LayoutTypeProperty = BindableProperty.Create(nameof(LayoutType), typeof(string), typeof(EntryFormView), "TextInputLayout");

        public int ColumnCount
        {
            get => (int)GetValue(ColumnCountProperty);
            set => SetValue(ColumnCountProperty, value);
        }

        public static readonly BindableProperty ColumnCountProperty = BindableProperty.Create(nameof(ColumnCount), typeof(int), typeof(EntryFormView), 2);

        public string SideImage
        {
            get => (string)GetValue(SideImageProperty);
            set => SetValue(SideImageProperty, value);
        }

        public static readonly BindableProperty SideImageProperty = BindableProperty.Create(nameof(SideImage), typeof(string), typeof(EntryFormView), "thearvindstore015.jpg");

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(EntryFormView), "Entry");

        #endregion Properties
    }
}