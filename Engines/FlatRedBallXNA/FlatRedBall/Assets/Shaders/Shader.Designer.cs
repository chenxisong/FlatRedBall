﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.832
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FlatRedBall.Assets.Shaders {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Shader {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Shader() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("FlatRedBall.Assets.Shaders.Shader", typeof(Shader).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static byte[] Bloom {
            get {
                object obj = ResourceManager.GetObject("Bloom", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] Blur {
            get {
                object obj = ResourceManager.GetObject("Blur", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to common lineHeight=22 base=18 scaleW=256 scaleH=256 pages=1
        ///char id=32   x=0     y=0     width=1     height=0     xoffset=0     yoffset=22    xadvance=5     page=0 
        ///char id=33   x=252   y=82    width=2     height=14    xoffset=2     yoffset=4     xadvance=6     page=0 
        ///char id=34   x=174   y=108   width=5     height=5     xoffset=1     yoffset=4     xadvance=7     page=0 
        ///char id=35   x=244   y=37    width=11    height=14    xoffset=0     yoffset=4     xadvance=11    page=0 
        ///char id=36   x=234   y=19    [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string defaultFont {
            get {
                return ResourceManager.GetString("defaultFont", resourceCulture);
            }
        }
        
        internal static byte[] defaultText {
            get {
                object obj = ResourceManager.GetObject("defaultText", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] DirectionalBlur {
            get {
                object obj = ResourceManager.GetObject("DirectionalBlur", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] FlatRedBallModelShader {
            get {
                object obj = ResourceManager.GetObject("FlatRedBallModelShader", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] FlatRedBallShader {
            get {
                object obj = ResourceManager.GetObject("FlatRedBallShader", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] Pixellate {
            get {
                object obj = ResourceManager.GetObject("Pixellate", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] RadialBlur {
            get {
                object obj = ResourceManager.GetObject("RadialBlur", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] test_cone {
            get {
                object obj = ResourceManager.GetObject("test_cone", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] test_cube {
            get {
                object obj = ResourceManager.GetObject("test_cube", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] test_cylinder {
            get {
                object obj = ResourceManager.GetObject("test_cylinder", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        internal static byte[] test_sphere {
            get {
                object obj = ResourceManager.GetObject("test_sphere", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}
