using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Nfc;
using BR.Com.Gertec.Gedi;
using BR.Com.Gertec.Gedi.Structs;
using BR.Com.Gertec.Gedi.Interfaces;
using Android.Content;
using System.Threading;

namespace ExemploNfcGedi
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TextView txtLeitura;
        private NfcAdapter nfcAdapter;
        private static int Contador = 0;
        private Tag tag;

        ICL icl = null;
        GEDI_CL_st_ISO_PollingInfo pollingInfo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            startGedi();
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            txtLeitura = FindViewById<TextView>(Resource.Id.txtLeitura);
            nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnResume()
        {
            base.OnResume();
            //Check if we have an NFC adapter.
            if (nfcAdapter != null && nfcAdapter.IsEnabled)
            {

                IntentFilter tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
                IntentFilter ndefDetected = new IntentFilter(NfcAdapter.ActionNdefDiscovered);
                IntentFilter techDetected = new IntentFilter(NfcAdapter.ActionTechDiscovered);
                IntentFilter idDetected = new IntentFilter((NfcAdapter.ExtraId));
                IntentFilter idExtraTag = new IntentFilter((NfcAdapter.ExtraTag));

                IntentFilter[] nfcIntentFilter = new IntentFilter[] { techDetected, tagDetected, ndefDetected, idDetected, idExtraTag };

                //Enable the foreground dispatch.
                nfcAdapter.EnableForegroundDispatch
                (
                    this,
                    PendingIntent.GetActivity(this, 0, new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop), 0),
                    nfcIntentFilter,
                    new string[][] { new string[] { "android.nfc.tech.Ndef", "android.nfc.action.NDEF_DISCOVERED" } }
                );
            }

        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            
            icl = GEDI.Instance.CL;
            pollingInfo = new GEDI_CL_st_ISO_PollingInfo();
            pollingInfo = icl.ISO_Polling(100);
            LerCard();

        }

        public void startGedi()
        {
            new Thread(new ThreadStart(() =>
            {
                GEDI.GetInstance(this);
            })).Start();
        }


        public void LerCard()
        {
            long result = 0;

            if (pollingInfo.AbUID == null)
            {
                txtLeitura.Text = "Erro ao ler cartão";
            }
            else
            {
                for (int i = pollingInfo.AbUID.Count - 1; i >= 0; --i)
                {
                    result <<= 8;
                    result |= pollingInfo.AbUID[i] & 0x0FF;
                }

                Contador += 1;
                txtLeitura.Text = "Contador: " + Contador.ToString() + "\nID Cartão: " + result.ToString();
            }

        }

    }
}