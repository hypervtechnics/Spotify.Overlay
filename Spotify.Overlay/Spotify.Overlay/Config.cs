using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Spotify.Overlay
{
    public class Config
    {
        private static string filename = "Spotify.Overlay.Configuration.xml";

        private static ConfigHolder holder = null;
        private static ConfigHolder Holder
        {
            get
            {
                if (holder == null) LoadConfig();

                return holder;
            }
            set
            {
                holder = value;
            }
        }

        [Serializable]
        public class ConfigHolder
        {
            public double normalOpacity = 0.7;
            public bool autoConnectAtStart = true;
            public bool autoLowerOpacityWhenPaused = true;
            public bool enableDoubleClickRealign = true;
            public bool useAnimationWhenRealign = true;
            public bool useAnimationsAtStartup = true;
            public bool useAnimationWhenLoweringOpacity = true;
            public bool startIfNotRunning = true;
        }

        public static double NormalOpacity
        {
            get
            {
                return Holder.normalOpacity;
            }
            set
            {
                if(value > 0.15 && value <= 1) Holder.normalOpacity = value;
            }
        }
        public static bool AutoConnectAtStart
        {
            get
            {
                return Holder.autoConnectAtStart;
            }
            set
            {
                Holder.autoConnectAtStart = value;
            }
        }
        public static bool AutoLowerOpacityWhenPaused
        {
            get
            {
                return Holder.autoLowerOpacityWhenPaused;
            }
            set
            {
                Holder.autoLowerOpacityWhenPaused = value;
            }
        }
        public static bool EnableDoubleClickRealign
        {
            get
            {
                return Holder.enableDoubleClickRealign;
            }
            set
            {
                Holder.enableDoubleClickRealign = value;
            }
        }
        public static bool UseAnimationWhenRealign
        {
            get
            {
                return Holder.useAnimationWhenRealign;
            }
            set
            {
                Holder.useAnimationWhenRealign = value;
            }
        }
        public static bool UseAnimationsAtStartup
        {
            get
            {
                return Holder.useAnimationsAtStartup;
            }
            set
            {
                Holder.useAnimationsAtStartup = value;
            }
        }
        public static bool UseAnimationWhenLoweringOpacity
        {
            get
            {
                return Holder.useAnimationWhenLoweringOpacity;
            }
            set
            {
                Holder.useAnimationWhenLoweringOpacity = value;
            }
        }
        public static bool StartIfNotRunning
        {
            get
            {
                return Holder.startIfNotRunning;
            }
            set
            {
                Holder.startIfNotRunning = value;
            }
        }

        public static void LoadConfig()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigHolder));

                using (FileStream stream = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    holder = (ConfigHolder) serializer.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                holder = new ConfigHolder();
            }
        }
        public static void SaveConfig()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ConfigHolder));

            using (FileStream stream = new FileStream(filename, FileMode.OpenOrCreate))
            {
                serializer.Serialize(stream, holder);
            }
        }
    }
}
