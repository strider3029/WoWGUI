﻿#pragma checksum "..\..\..\..\GUI\FormLogin.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E3F2BECBE1CC109673E72CD52BF29A15"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace WowClient {
    
    
    /// <summary>
    /// FormLogin
    /// </summary>
    public partial class FormLogin : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\GUI\FormLogin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnLogin;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\..\..\GUI\FormLogin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tbAccountName;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\..\GUI\FormLogin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblAccountName;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\..\GUI\FormLogin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblPassword;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\..\GUI\FormLogin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCreateAccount;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\..\GUI\FormLogin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox pwbPassword;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\..\GUI\FormLogin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbLanguagePicker;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WowClient;component/gui/formlogin.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\GUI\FormLogin.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.btnLogin = ((System.Windows.Controls.Button)(target));
            
            #line 9 "..\..\..\..\GUI\FormLogin.xaml"
            this.btnLogin.Click += new System.Windows.RoutedEventHandler(this.btnLogin_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.tbAccountName = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.lblAccountName = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.lblPassword = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.btnCreateAccount = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\..\..\GUI\FormLogin.xaml"
            this.btnCreateAccount.Click += new System.Windows.RoutedEventHandler(this.btnCreateAccount_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.pwbPassword = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 7:
            this.cbLanguagePicker = ((System.Windows.Controls.ComboBox)(target));
            
            #line 16 "..\..\..\..\GUI\FormLogin.xaml"
            this.cbLanguagePicker.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.cbLanguagePicker_SelectionChanged);
            
            #line default
            #line hidden
            
            #line 16 "..\..\..\..\GUI\FormLogin.xaml"
            this.cbLanguagePicker.DropDownOpened += new System.EventHandler(this.cbLanguagePicker_DropDownOpened);
            
            #line default
            #line hidden
            
            #line 16 "..\..\..\..\GUI\FormLogin.xaml"
            this.cbLanguagePicker.DropDownClosed += new System.EventHandler(this.cbLanguagePicker_DropDownClosed);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

