﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace XnewsAdapter.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5000")]
        public int scanInterval {
            get {
                return ((int)(this["scanInterval"]));
            }
            set {
                this["scanInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("MediaManagerAdapter")]
        public string AppTitle {
            get {
                return ((string)(this["AppTitle"]));
            }
            set {
                this["AppTitle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://10.27.137.127/api")]
        public string arcTranscodeAPI {
            get {
                return ((string)(this["arcTranscodeAPI"]));
            }
            set {
                this["arcTranscodeAPI"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("mxf.xml")]
        public string arcProfile {
            get {
                return ((string)(this["arcProfile"]));
            }
            set {
                this["arcProfile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("xmt")]
        public string arcappid {
            get {
                return ((string)(this["arcappid"]));
            }
            set {
                this["arcappid"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3")]
        public int errorRetryCounts {
            get {
                return ((int)(this["errorRetryCounts"]));
            }
            set {
                this["errorRetryCounts"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5000")]
        public int checkStatusInterval {
            get {
                return ((int)(this["checkStatusInterval"]));
            }
            set {
                this["checkStatusInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public int textclearLenth {
            get {
                return ((int)(this["textclearLenth"]));
            }
            set {
                this["textclearLenth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\10.26.124.101\\admin\\xw\\smstxt")]
        public string smspath {
            get {
                return ((string)(this["smspath"]));
            }
            set {
                this["smspath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10.27.137.24:16379,password=P@ssw0rdredis")]
        public string redisConnstring {
            get {
                return ((string)(this["redisConnstring"]));
            }
            set {
                this["redisConnstring"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>13917347195;丁鑫锋</string>\r\n  <string>15821225096;8990值班</string>\r\n</ArrayOf" +
            "String>")]
        public global::System.Collections.Specialized.StringCollection mobilePhones {
            get {
                return ((global::System.Collections.Specialized.StringCollection)(this["mobilePhones"]));
            }
            set {
                this["mobilePhones"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\10.26.124.101\\admin\\xw\\XnewsSearchToAvid\\script")]
        public string scanScriptPath {
            get {
                return ((string)(this["scanScriptPath"]));
            }
            set {
                this["scanScriptPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\\\\10.26.124.101\\admin\\xw\\XnewsSearchToAvid\\video")]
        public string scanVideoPath {
            get {
                return ((string)(this["scanVideoPath"]));
            }
            set {
                this["scanVideoPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("XDCAM HD422")]
        public string avidcoder {
            get {
                return ((string)(this["avidcoder"]));
            }
            set {
                this["avidcoder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("172.27.3.159")]
        public string localIP {
            get {
                return ((string)(this["localIP"]));
            }
            set {
                this["localIP"] = value;
            }
        }
    }
}
