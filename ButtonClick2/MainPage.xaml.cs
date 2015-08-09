using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using System.Diagnostics;     //allows me to use Write to Debug Output console via Debug.WriteLine("....");

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ButtonClick2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int BUTTONPINNBR = 16;
        private const int LEDPINNBR = 6;
        private GpioPin buttonPin, ledPin;
        private GpioPinValue buttonPinValCurrent, buttonPinValPrior, ledPinVal;
        public MainPage()
        {
            this.InitializeComponent();
            InitGPIO();

        }
        private void InitGPIO()
        {
            var mygpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (mygpio == null)
            {
                buttonPin = null;
                ledPin = null;
                return;
            }
            ledPin = mygpio.OpenPin(LEDPINNBR);
            ledPin.Write(GpioPinValue.Low); //initialize Led to On as wired in active Low config (+3.3-Led-GPIO)
            ledPin.SetDriveMode(GpioPinDriveMode.Output);


            buttonPin = mygpio.OpenPin(BUTTONPINNBR);
            buttonPin.Write(GpioPinValue.High);
            buttonPin.SetDriveMode(GpioPinDriveMode.Output);
            buttonPinValCurrent = buttonPin.Read();
            buttonPin.SetDriveMode(GpioPinDriveMode.Input);
            buttonPinValPrior = GpioPinValue.High;

            Debug.WriteLine("ButtonPin Value at Init: " + buttonPin.Read() + ",      with Pin ID = " + buttonPin.PinNumber);

            //buttonPinVal = buttonPin.Read();
            // Set a debounce timeout to filter out switch bounce noise from a button press
            buttonPin.DebounceTimeout = TimeSpan.FromMilliseconds(20);

            // Register for the ValueChanged event so our buttonPin_ValueChanged 
            // function is called when the button is pressed
            buttonPin.ValueChanged += buttonPressAction;

        }
        private void buttonPressAction(GpioPin mycallerPin, GpioPinValueChangedEventArgs myevent)
        {
            //Debug.WriteLine("Event handler detected ButtonPin Change : " + myevent.Edge);
            if (myevent.Edge == GpioPinEdge.RisingEdge) //RisingEdge)
            {
                Debug.WriteLine("Event handler detected RISING Edge");
                ledPinVal = (ledPinVal == GpioPinValue.Low) ?
                    GpioPinValue.High : GpioPinValue.Low;
                ledPin.Write(ledPinVal);
            }
            else
            {
                Debug.WriteLine("Event handler detected FALLING Edge");
            }

        }
    }
}
