﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.1
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace KinecTh.Properties {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("KinecTh.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   厳密に型指定されたこのリソース クラスを使用して、すべての検索リソースに対し、
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
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
        
        /// <summary>
        ///   &lt;OpenNI&gt;
        ///	&lt;Licenses&gt;
        ///		&lt;!-- Add licenses here 
        ///		&lt;License vendor=&quot;vendor&quot; key=&quot;key&quot;/&gt;
        ///		--&gt;
        ///	&lt;/Licenses&gt;
        ///	&lt;Log writeToConsole=&quot;false&quot; writeToFile=&quot;false&quot;&gt;
        ///		&lt;!-- 0 - Verbose, 1 - Info, 2 - Warning, 3 - Error (default) --&gt;
        ///		&lt;LogLevel value=&quot;3&quot;/&gt;
        ///		&lt;Masks&gt;
        ///			&lt;Mask name=&quot;ALL&quot; on=&quot;true&quot;/&gt;
        ///		&lt;/Masks&gt;
        ///		&lt;Dumps&gt;
        ///		&lt;/Dumps&gt;
        ///	&lt;/Log&gt;
        ///	&lt;ProductionNodes&gt;
        ///		&lt;Node type=&quot;Image&quot; name=&quot;Image1&quot;&gt;
        ///			&lt;Configuration&gt;
        ///				&lt;MapOutputMode xRes=&quot;640&quot; yRes=&quot;480&quot; FPS=&quot;30&quot;/&gt;
        ///				&lt;Mirror on=&quot;true&quot;/&gt;
        ///			&lt;/Configura [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string KinecThConfig {
            get {
                return ResourceManager.GetString("KinecThConfig", resourceCulture);
            }
        }
        
        internal static System.Drawing.Bitmap momiji {
            get {
                object obj = ResourceManager.GetObject("momiji", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static byte[] reimu3 {
            get {
                object obj = ResourceManager.GetObject("reimu3", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}