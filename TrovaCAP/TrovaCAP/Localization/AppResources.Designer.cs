﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TrovaCAP.Localization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class AppResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AppResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TrovaCAP.Localization.AppResources", typeof(AppResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Italian ZIP Codes.
        /// </summary>
        public static string ApplicationName {
            get {
                return ResourceManager.GetString("ApplicationName", resourceCulture);
            }
        }
        
        public static byte[] Bollo {
            get {
                object obj = ResourceManager.GetObject("Bollo", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ZIP Code.
        /// </summary>
        public static string CAP {
            get {
                return ResourceManager.GetString("CAP", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to City.
        /// </summary>
        public static string Comune {
            get {
                return ResourceManager.GetString("Comune", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to District.
        /// </summary>
        public static string FrazioneQuartiere {
            get {
                return ResourceManager.GetString("FrazioneQuartiere", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Get Other Apps from WPME!.
        /// </summary>
        public static string GetOtherApps {
            get {
                return ResourceManager.GetString("GetOtherApps", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Address.
        /// </summary>
        public static string Indirizzo {
            get {
                return ResourceManager.GetString("Indirizzo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loading....
        /// </summary>
        public static string Loading {
            get {
                return ResourceManager.GetString("Loading", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Too many results, keep writing....
        /// </summary>
        public static string TooManyResults {
            get {
                return ResourceManager.GetString("TooManyResults", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Trial Mode, click here and buy the full version to see the full Zip Code.
        /// </summary>
        public static string TrialMode {
            get {
                return ResourceManager.GetString("TrialMode", resourceCulture);
            }
        }
    }
}
