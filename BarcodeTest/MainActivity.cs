using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using ZXing.Mobile;
using System.Threading.Tasks;
using System;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace BarcodeTest
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Button scan;
        TextView barcode;
        TextView info;
        bool isKitkat = Build.VERSION.SdkInt == BuildVersionCodes.Kitkat;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);

            barcode = FindViewById<TextView>(Resource.Id.barcode);
            info = FindViewById<TextView>(Resource.Id.info);
            if (isKitkat)
            {
                info.Text = "KitKat device. Disabling Auto Focus";
            }
            else
            {
                info.Text = "Auto Focus Enabled";
            }
            scan = FindViewById<Button>(Resource.Id.scan);
            scan.Click += Scan_ClickAsync;

            RequestPermissions();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void RequestPermissions()
        {
            var status = CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera).Result;
            if (status != PermissionStatus.Granted)
            {
                if (CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera).Result)
                {
                    Toast.MakeText(this, "Please enable camera access", ToastLength.Short).Show();
                }

                var results = CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera).Result;
            }
        }

        private async void Scan_ClickAsync(object sender, EventArgs e)
        {
            var mobileBarcodeScanner = new MobileBarcodeScanner(this);
            mobileBarcodeScanner.UseCustomOverlay = false;
            mobileBarcodeScanner.TopText = "Hold the camera up to the barcode About 6 inches away";
            mobileBarcodeScanner.BottomText = "Wait for the barcode to automatically scan!";

            ZXing.Result result = await mobileBarcodeScanner.Scan(new MobileBarcodeScanningOptions()
            {
                DisableAutoFocus = isKitkat
            });

            barcode.Text = string.IsNullOrEmpty(result?.Text) ? "No barcode found" : result.Text;
        }
    }
}