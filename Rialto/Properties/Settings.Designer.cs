﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Rialto.Properties {
    
    
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
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string LastOpenDbName {
            get {
                return ((string)(this["LastOpenDbName"]));
            }
            set {
                this["LastOpenDbName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ThumbnailImageDirectory {
            get {
                return ((string)(this["ThumbnailImageDirectory"]));
            }
            set {
                this["ThumbnailImageDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ImgDataDirectory {
            get {
                return ((string)(this["ImgDataDirectory"]));
            }
            set {
                this["ImgDataDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"
          <WINDOWPLACEMENT xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
            <length>0</length>
            <flags>0</flags>
            <showCmd>HIDE</showCmd>
            <minPosition>
              <X>0</X>
              <Y>0</Y>
            </minPosition>
            <maxPosition>
              <X>0</X>
              <Y>0</Y>
            </maxPosition>
            <normalPosition>
              <Left>0</Left>
              <Top>0</Top>
              <Right>0</Right>
              <Bottom>0</Bottom>
            </normalPosition>
          </WINDOWPLACEMENT>
        ")]
        public global::Rialto.Components.Views.WINDOWPLACEMENT MainWindowPlacements {
            get {
                return ((global::Rialto.Components.Views.WINDOWPLACEMENT)(this["MainWindowPlacements"]));
            }
            set {
                this["MainWindowPlacements"] = value;
            }
        }
    }
}
