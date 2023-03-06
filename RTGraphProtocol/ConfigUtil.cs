using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace RTGraphProtocol
{
    public class ConfigUtil
    {
        private string prefix = null;
        private Configuration config;
        private KeyValueConfigurationCollection cfgCollection;

        public ConfigUtil(string prefix = null)
        {
            this.config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            this.cfgCollection = config.AppSettings.Settings;
            this.prefix = prefix;
        }

        public void Save()
        {
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }

        public void SetValue(string key, string value)
        {
            if (prefix != null) key = prefix + "." + key;
            cfgCollection.Remove(key);
            cfgCollection.Add(key, value);
        }

        public string GetValue(string key)
        {
            if (prefix != null) key = prefix + "." + key;
            return cfgCollection[key]?.Value;
        }

        public bool TryGetValue(string key, out string value)
        {
            if (prefix != null) key = prefix + "." + key;
            var pair = cfgCollection[key];
            if (pair != null)
            {
                value = pair.Value;
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public void SetAllValues(object obj)
        {
            var props = obj.GetType().GetProperties();
            foreach(var prop in props) 
            {
                var val = prop.GetValue(obj);
                SetValue(prop.Name, val.ToString());
            };
        }

        public void GetAllValues(object obj)
        {
            var props = obj.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (TryGetValue(prop.Name, out var val))
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        prop.SetValue(obj, val);
                    }
                    else if (prop.PropertyType == typeof(Enum))
                    {
                        prop.SetValue(obj, Enum.Parse(prop.PropertyType, val));
                    }
                    else if (prop.PropertyType == typeof(int))
                    {
                        prop.SetValue(obj, int.Parse(val));
                    }
                    else if (prop.PropertyType == typeof(byte))
                    {
                        prop.SetValue(obj, byte.Parse(val));
                    }
                    else if (prop.PropertyType == typeof(short))
                    {
                        prop.SetValue(obj, short.Parse(val));
                    }
                }
            };
        }
    }
}
