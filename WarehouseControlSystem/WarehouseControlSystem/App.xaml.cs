﻿// ----------------------------------------------------------------------------------
// Copyright © 2018, Oleg Lobakov, Contacts: <oleg.lobakov@gmail.com>
// Licensed under the GNU GENERAL PUBLIC LICENSE, Version 3.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// https://github.com/OlegLobakov/WarehouseControlSystem/blob/master/LICENSE
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------
using Xamarin.Forms.Xaml;
using Xamarin.Forms;


[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace WarehouseControlSystem
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                // determine the correct, supported .NET culture
                System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(Settings.CurrentLocalization);
                Resx.AppResources.Culture = ci; // set the RESX for resource localization
                DependencyService.Get<ILocalize>().SetLocale(ci); // set the Thread for locale-aware methods
            }

            Global.Init();
            Global.SetCurrentConnection();
            Global.MainPage = new MainPage();
            MainPage = Global.MainPage;
        }

        protected override void OnStart()
        {
            Global.CompliantPlug = "OnStart";
        }

        protected override void OnSleep()
        {
            Global.SaveParameters();
        }

        protected override void OnResume()
        {
            Global.CompliantPlug = "OnResume";
        }
    }
}
