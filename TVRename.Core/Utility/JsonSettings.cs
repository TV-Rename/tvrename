using System;
using Alphaleonis.Win32.Filesystem;
using Newtonsoft.Json;

namespace TVRename.Core.Utility
{
    /// <summary>
    /// Singleton lazy loaded settings serialized to JSON.
    /// </summary>
    /// <typeparam name="T">Settings class derived from <see cref="JsonSettings{T}"/> to instantiate.</typeparam>
    /// <example>
    /// <code>
    /// public class MySettings : JsonSettings&lt;MySettings&gt;
    ///	{
    ///		public bool Disabled { get; set; } = false;
    ///	}
    ///	
    ///	MySettings.FilePath = "mysettings.json";
    ///	
    ///	if (MySettings.Instance.Disabled) {
    ///		...
    ///	}
    ///	
    ///	MySettings.Instance.Disabled = false;
    ///	MySettings.Save();
    /// </code>
    /// </example>
    public abstract class JsonSettings<T> : ISettings<T> where T : class, ISettings<T>, new()
    {
        // ReSharper disable StaticMemberInGenericType

        private static T instance;

        /// <summary>
        /// Gets or sets the file path where the settings file will be read and written to.
        /// </summary>
        /// <value>
        /// The file path to the settings file.
        /// </value>
        public static string FilePath { get; set; } = AppDomain.CurrentDomain.BaseDirectory + @"\settings.json";

        /// <summary>
        /// <see cref="JsonSerializerSettings"/> to use when saving the settings.
        /// </summary>
        public static JsonSerializerSettings SerializerSettings => new JsonSerializerSettings();

        /// <summary>
        /// <see cref="SerializerFormatting"/> to use when saving the settings.
        /// </summary>
        public static Formatting SerializerFormatting => Formatting.Indented;

        /// <summary>
        /// Gets the singleton lazy loaded settings instace.
        /// Loads the settings file on first access, then maintains settings in memory.
        /// If the settings do not exist on disk, <see cref="Initialize"/> is called and the settings are saved to disk.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance != null) return instance;

                try
                {
                    instance = JsonConvert.DeserializeObject<T>(File.ReadAllText(FilePath));

                    if (instance == null) throw new NullReferenceException("Empty JSON file");
                }
                catch (Exception)
                {
                    instance = new T();
                    instance = instance.Initialize();
                    Save();
                }

                return instance;
            }
        }

        /// <summary>
        /// Virtual constructor to return a new instance of the settings type.
        /// 
        /// Allows for setting defaults when creating a new settings file.
        /// </summary>
        /// <returns>New instance of its own type with starting defaults.</returns>
        /// <example>
        /// <code>
        /// public class MySettings : JsonSettings&lt;MySettings&gt;
        ///	{
        ///		public List&lt;ShoppingItems&gt; Items { get; set; };
        ///
        ///		public override MySettings Initialize()
        ///		{
        ///			return new MySettings
        ///			{
        ///				Items = new List&lt;ShoppingItems&gt;
        ///				{
        ///					new ShoppingItems("Apples", 5),
        ///					new ShoppingItems("Oranges", 3)
        ///				}
        ///			};
        ///		}
        ///	}
        /// </code>
        /// </example>
        public virtual T Initialize()
        {
            instance = new T();

            return instance;
        }

        /// <summary>
        /// Saves the current settings instance to disk.
        /// </summary>
        /// <seealso cref="Path"/>
        public static void Save()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(Instance, SerializerFormatting, SerializerSettings));
        }
    }

    /// <summary>
    /// Base settings interface exposing virtual constructor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISettings<out T>
    {
        /// <summary>
        /// Virtual constructor to return a new instance of the settings type.
        /// </summary>
        /// <returns></returns>
        T Initialize();
    }
}
