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
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        //자세한 설명은 Xabre 깃허브 페이지 참고
        //https://github.com/xabre/xamarin-bluetooth-le

        IAdapter adapter;
        IBluetoothLE bluetoothBLE;
        ObservableCollection<IDevice> list;
        IDevice device;

        public MainPage()
        {

            //Xabre는 바닐라 혹은 Mvvmcross로 작업가능
            //바닐라는 Non Mvvmcross로써 순수 자마린
            //아래는 바닐라로 설정하는 코드입니다.

            InitializeComponent();
            bluetoothBLE = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            list = new ObservableCollection<IDevice>();
            DevicesList.ItemsSource = list;
        }
        
        // C# 비동기함수 async와 await
        //https://docs.microsoft.com/ko-kr/dotnet/csharp/programming-guide/concepts/async/

        
        private async void searchDevice(object sender, EventArgs e)
        {
            if (bluetoothBLE.State == BluetoothState.Off) //블루투스 상태확인
            {
                await DisplayAlert("알림", "블루투스가 켜져있지 않습니다.", "OK");
            }
            else
            {
                list.Clear();
                adapter.ScanTimeout = 3000; // 스캔 버튼 클릭시 반응속도
                adapter.ScanMode = ScanMode.Balanced;
                adapter.DeviceDiscovered += (obj, a) =>
                {
                    if (!list.Contains(a.Device))
                        list.Add(a.Device);
                };
                await adapter.StartScanningForDevicesAsync(); //스캔시작
            }
        }

        private async void DevicesList_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            device = DevicesList.SelectedItem as IDevice; // 자마린폼즈 ListView ItemSelected
            var result = await DisplayAlert("스캔", "이 기기에 연결하시겠습니까?", "연결", "취소");
            if (!result)
                return;
            await adapter.StopScanningForDevicesAsync(); //스캔종료
            try
            {
                var parameters = new ConnectParameters(forceBleTransport: true); // gattcallback 133 에러 방지
                await adapter.ConnectToDeviceAsync(device, parameters); // 장치연결 성공
                await DisplayAlert("연결", "상태 : " + device.State, "네");
            }
            catch (DeviceConnectionException ex) //장치연결 실패
            {
                await DisplayAlert("에러", ex.Message, "네");
            }
        }
        private async void Disconnect(object sender, EventArgs e)
        {
            try
            {
                await adapter.DisconnectDeviceAsync(device); //장치연결 해제
                await DisplayAlert("종료", "연결이 해제되었습니다.", "네");
            }
            catch
            {
            }
        }

        //블루투스는 서비스와 특성으로 데이터를 교환한다.
        //https://learn.adafruit.com/introduction-to-bluetooth-low-energy/gatt

        //Parse를 통해 문자열-숫자를 바꿉니다.
        //https://docs.microsoft.com/ko-kr/dotnet/csharp/programming-guide/types/how-to-convert-a-string-to-a-number

        //Guid구조체 MSDN 링크
        //https://docs.microsoft.com/ko-kr/dotnet/api/system.guid?view=net-5.0

        //Guid.Parse메서드 MSDN 링크
        //https://docs.microsoft.com/ko-kr/dotnet/api/system.guid.parse?view=net-5.0

        //UTF8Encoding 클래스 MSDN 링크
        //https://docs.microsoft.com/ko-kr/dotnet/api/system.text.utf8encoding?view=net-5.0

        //ReadAsync 비동기함수 MSDN 링크
        //MS에서 비동기 함수 구현에 많은 노력을 했다고 합니다.
        //그러므로 우리는 유용히 사용하도록 노력해봅니다.
        //https://docs.microsoft.com/ko-kr/dotnet/api/system.io.stream.readasync?view=net-5.0

        private async void ReadButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                //UUID와 Guid는 동일하며, 서비스의 고유 ID이다.
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("{beb5483e-36e1-4688-b7f5-ea07361b26a8}")); //ManufacturerName, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        var bytes = await characteristic.ReadAsync();
                        var str = Encoding.UTF8.GetString(bytes); //데이터를 읽을 때 값을 문자로 인코딩
                        readdata.Text = str;
                    }
                }
            }
            catch
            {
            }
        }
        
        //IsNullOrEmpty메서드는 널인지 공백인지 확인한다.
        //https://docs.microsoft.com/ko-kr/dotnet/api/system.string.isnullorempty?view=net-5.0

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
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
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
