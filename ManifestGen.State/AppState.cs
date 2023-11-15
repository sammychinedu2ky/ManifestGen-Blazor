using System;
using System.Text.Json.Serialization;

namespace ManifestGen.State
{
    public class AppState
    {
        public event EventHandler? DataChanged;

        private string _name = "app name";
        [JsonPropertyName("name")]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;

                    OnDataChanged();
                }
            }
        }

        private string _shortName = "short name";
        [JsonPropertyName("short_name")]
        public string ShortName
        {
            get => _shortName;
            set
            {
                if (_shortName != value)
                {
                    _shortName = value;
                    OnDataChanged();
                }
            }
        }

        private string _themeColor = "#fff";
        [JsonPropertyName("theme_color")]
        public string ThemeColor
        {
            get => _themeColor;
            set
            {
                if (_themeColor != value)
                {
                    _themeColor = value;
                    OnDataChanged();
                }
            }
        }

        private string _backgroundColor = "#fff";
        [JsonPropertyName("background_color")]
        public string BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (_backgroundColor != value)
                {
                    _backgroundColor = value;
                    OnDataChanged();
                }
            }
        }

        private string _display = "browser";
        [JsonPropertyName("display")]
        public string Display
        {
            get => _display;
            set
            {
                if (_display != value)
                {
                    _display = value;
                    OnDataChanged();
                }
            }
        }

        private string _orientation = "portrait";
        [JsonPropertyName("orientation")]
        public string Orientation
        {
            get => _orientation;
            set
            {
                if (_orientation != value)
                {
                    _orientation = value;
                    OnDataChanged();
                }
            }
        }

        private string _scope = "/";
        [JsonPropertyName("scope")]
        public string Scope
        {
            get => _scope;
            set
            {
                if (_scope != value)
                {
                    _scope = value;
                    OnDataChanged();
                }
            }
        }

        private string _startUrl = "/";
        [JsonPropertyName("start_url")]
        public string StartUrl
        {
            get => _startUrl;
            set
            {
                if (_startUrl != value)
                {
                    _startUrl = value;
                    OnDataChanged();
                }
            }
        }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("icons")]
        public List<Dictionary<string, string>> Icons { get; set; } = default!;
        private void OnDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
