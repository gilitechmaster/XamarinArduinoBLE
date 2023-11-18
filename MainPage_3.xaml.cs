//MainPage_3.xaml과 연결되며 값을 입력하는 방식이 아닌 버튼으로 값을 전송하는 방식이다.
//버튼 36개로 36가지 색상을 제공한다.

using System;
//using System.Collections.Generic;
using System.ComponentModel;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;

//Nuget패키지에서 Plugin.BLE 설치
//https://github.com/xabre/xamarin-bluetooth-le

using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE.Abstractions;
namespace BLE_Test9
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        IAdapter adapter;
        IBluetoothLE bluetoothBLE;
        ObservableCollection<IDevice> list;
        IDevice device;

        public MainPage()
        {

            //Xabre는 바닐라 혹은 Mvvmcross로 작업가능
            //MvvM은 데이터를 바인딩해서 GUI 뷰하는 모델
            //https://ko.wikipedia.org/wiki/%EB%AA%A8%EB%8D%B8-%EB%B7%B0-%EB%B7%B0%EB%AA%A8%EB%8D%B8

            //바닐라는 Non Mvvmcross로써 순수 자마린
            //아래는 바닐라로 설정하는 코드입니다.

            InitializeComponent();
            bluetoothBLE = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            list = new ObservableCollection<IDevice>();
            DevicesList.ItemsSource = list;

            //스캔목록list은 ListView로 구현
            //ListView는 스크롤이 가능한 데이터 목록이다.
            //https://docs.microsoft.com/ko-kr/xamarin/xamarin-forms/user-interface/listview/

            //자마린 폼즈 ListView ItemSource
            //ObservableCollection 인스턴스가 데이터로 목록에 채워짐 
            //https://docs.microsoft.com/ko-kr/xamarin/xamarin-forms/user-interface/listview/data-and-databinding

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

        //IsNullOrEmpty메서드는 널인지 공백인지 확인한다.
        //https://docs.microsoft.com/ko-kr/dotnet/api/system.string.isnullorempty?view=net-5.0

        private async void WriteButton_Clicked_1(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata1.Text) ? "전송" : "1");
                        //버튼 값 1 ESP32로 보내기
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_2(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata2.Text) ? "전송" : "11");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_3(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata3.Text) ? "전송" : "111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_4(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata4.Text) ? "전송" : "1111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_5(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata5.Text) ? "전송" : "11111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_6(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata6.Text) ? "전송" : "111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_7(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata7.Text) ? "전송" : "1111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_8(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata8.Text) ? "전송" : "11111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_9(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata9.Text) ? "전송" : "111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_10(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata10.Text) ? "전송" : "1111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_11(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata11.Text) ? "전송" : "11111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_12(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata12.Text) ? "전송" : "111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_13(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata13.Text) ? "전송" : "1111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_14(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata14.Text) ? "전송" : "11111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_15(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata15.Text) ? "전송" : "111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_16(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata16.Text) ? "전송" : "1111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_17(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata17.Text) ? "전송" : "11111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_18(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata18.Text) ? "전송" : "111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_19(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata19.Text) ? "전송" : "1111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_20(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata20.Text) ? "전송" : "11111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_21(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata21.Text) ? "전송" : "111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_22(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata22.Text) ? "전송" : "1111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_23(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata23.Text) ? "전송" : "11111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_24(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata24.Text) ? "전송" : "111111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_25(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata25.Text) ? "전송" : "1111111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_26(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata26.Text) ? "전송" : "11111111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_27(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata27.Text) ? "전송" : "111111111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_28(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata28.Text) ? "전송" : "1111111111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_29(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata29.Text) ? "전송" : "11111111111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_30(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata30.Text) ? "전송" : "111111111111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_31(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata31.Text) ? "전송" : "1111111111111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_32(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata32.Text) ? "전송" : "11111111111111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_33(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata33.Text) ? "전송" : "111111111111111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_34(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata34.Text) ? "전송" : "1111111111111111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_35(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata35.Text) ? "전송" : "11111111111111111111111111111111111");
                        //데이터를 쓰기할 때 문자를 값으로 인코딩
                        var bytes = await characteristic.WriteAsync(senddata);
                    }
                }
            }
            catch
            {
            }
        }
        private async void WriteButton_Clicked_36(object sender, EventArgs e)
        {
            try
            {
                var service = await device.GetServiceAsync(Guid.Parse("4fafc201-1fb5-459e-8fcc-c5c9c331914b")); //ServiceId, ESP32 UUID 적용
                if (service != null)
                {
                    var characteristic = await service.GetCharacteristicAsync(Guid.Parse("beb5483e-36e1-4688-b7f5-ea07361b26a8")); //Dataexchange, ESP32 UUID 적용
                    if (characteristic != null)
                    {
                        byte[] senddata = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(writedata36.Text) ? "전송" : "111111111111111111111111111111111111");
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
