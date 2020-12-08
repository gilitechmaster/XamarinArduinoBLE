using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE.Abstractions;
namespace BLE_Test9
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        IAdapter adapter;
        IBluetoothLE bluetoothBLE;
        ObservableCollection<IDevice> list;
        IDevice device;
        //private readonly IDevice device;
        public MainPage()
        {
            InitializeComponent();
            bluetoothBLE = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            list = new ObservableCollection<IDevice>();
            DevicesList.ItemsSource = list;
        }
        private async void searchDevice(object sender, EventArgs e)
        {
            if (bluetoothBLE.State == BluetoothState.Off)
            {
                await DisplayAlert("알림", "블루투스가 켜져있지 않습니다.", "OK");
            }
            else
            {
                list.Clear();
                adapter.ScanTimeout = 3000; // 스캔 클릭시 반응속도
                adapter.ScanMode = ScanMode.Balanced;
                adapter.DeviceDiscovered += (obj, a) =>
                {
                    if (!list.Contains(a.Device))
                        list.Add(a.Device);
                };
                await adapter.StartScanningForDevicesAsync();
            }
        }

        private async void DevicesList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            device = DevicesList.SelectedItem as IDevice;
            var result = await DisplayAlert("스캔", "이 기기에 연결하시겠습니까?", "연결", "취소");
            if (!result)
                return;
            await adapter.StopScanningForDevicesAsync();
            try
            {
                var parameters = new ConnectParameters(forceBleTransport: true); // gattcallback 133 에러 방지
                await adapter.ConnectToDeviceAsync(device, parameters);
                await DisplayAlert("연결", "상태 : " + device.State, "네");
            }
            catch (DeviceConnectionException ex)
            {
                await DisplayAlert("에러", ex.Message, "네");
            }
        }
        private async void Disconnect(object sender, EventArgs e)
        {
            try
            {
                await adapter.DisconnectDeviceAsync(device);
                await DisplayAlert("종료", "연결이 해제되었습니다.", "네");
            }
            catch
            {
            }
        }

        private async void ReadButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("{beb5483e-36e1-4688-b7f5-ea07361b26a8}")); //ManufacturerName, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        var bytes = await characteristic.ReadAsync();
                        var str = Encoding.UTF8.GetString(bytes);
                        readdata.Text = str;
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("{beb5483e-36e1-4688-b7f5-ea07361b26a8}")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata.Text) ? "전송" : writedata.Text);
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
    }
}