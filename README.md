# XamarinArduinoBLE
<pre>
자마린(xamarin)을 사용하여
아두이노 BLE 데이터 통신을 구현하는 전체 코드입니다.

Xabre Bluetooth LE plugin for Xamarin을 사용합니다.

Xabre 플러그인은
Apache 2.0 라이센스를 사용하며

길이스타트업에서 배포하는 본 코드도
Apache 2.0 라이센스로 배포하므로

권한요청 없이 상업적 이용
및 수정 재배포가 가능합니다.


</pre>

https://github.com/xabre/xamarin-bluetooth-le
<br><br>

<pre>

관련 과정 글은 길이스타트업
Naver Blog 링크를 참고하시면 됩니다.

(3)과 (4) 글은 ESP32에 관한 내용으로
Xamarin과 무관한 내용이므로 링크하지 않습니다.

</pre>

https://blog.naver.com/gilitechmaster/221994280802 (1)
<br>
https://blog.naver.com/gilitechmaster/222017267997 (2)
<br>
https://blog.naver.com/gilitechmaster/222131564036 (5)
<br><br>


<pre>

소규모 사업자가 웨어러블 개발시
개발자 구하기가 어려워서 사업 진전이
안된다고 알고 있습니다.

창의적인 전자제품이 한국사회에
"대폭 증진"되기를 희망함에

공유드립니다. ^^

한국에 자마린으로 IoT 웨어러블
많이 만들어주세요 ^^.

Wemos ESP32와 우노 HM10은 통신성공했습니다.
클래식 블루투스는 안되는 "BLE" 코드입니다~~

아두이노는 오픈소스 하드웨어이기 때문에
센서모듈만 바꾸어서 공장에서 제작하면
쉽게 제품으로 구현이 가능합니다.

대신에, HM10 혹은 espressif 통신모듈은
현재 코드에 최적화되있기에 다른 모듈을
사용하려면 본 어플을 사용할 수 없습니다.

예를들면, HC06은 본 어플과 통신이 안됩니다.

즉, UNO - Nano는 하드웨어 도면이 상업적 이용
가능하므로 "통신모듈은 바꾸지 않고", 센서랑
코드만 독자적으로 임베디드 하여 IoT제품으로
출시하여 사용가능한 어플입니다.


</pre>

<pre>

Assets 폴더에 TTF폰트는 배민연성체입니다.

임베딩 및 상업적 이용이 가능하며
아래에 링크를 참고하시길 바랍니다.

</pre>

https://www.woowahan.com/#/fonts
<br><br>


</pre>

<img src="https://mblogthumb-phinf.pstatic.net/MjAyMDEwMzFfMjEx/MDAxNjA0MDc4MTUwMzk4.nmEYkiw9SRAtrnfN12wrNG07F8xUaNBXvXqdtUvqa_Ag.9jRyKElsdkA8dYGiDPaHSBkzSN4Nv1lT-FbqlVczz7Ag.GIF.gilitechmaster/%25EC%259E%2590%25EB%25A7%2588%25EB%25A6%25B0_%25EC%2595%2584%25EB%2591%2590%25EC%259D%25B4%25EB%2585%25B8_5_1.gif?type=w800">
<img src="https://mblogthumb-phinf.pstatic.net/MjAyMDEwMzFfMjI4/MDAxNjA0MDc4MzgzMDM5.Ne1qZ_7F7wvFcVTkIxe7vxTBAdX7PUnXtICm60D-Ryog.WeRSrncsrjYtzlIHDdY1kdsMS6AXmVkTqYgZRLOgQr0g.GIF.gilitechmaster/%25EC%259E%2590%25EB%25A7%2588%25EB%25A6%25B0_%25EC%2595%2584%25EB%2591%2590%25EC%259D%25B4%25EB%2585%25B8_5_2.gif?type=w800">
<img src="https://mblogthumb-phinf.pstatic.net/MjAyMDEwMzFfMjM3/MDAxNjA0MDc4NDU3ODg0.573Vtp17WCDKgT_vDaN-HSYYAGs-RETYus5BSUGcrKUg.9cfB8wY5imxaXB_mJWcUBV47az1rlqq5-gb51t0iyhEg.GIF.gilitechmaster/%25EC%259E%2590%25EB%25A7%2588%25EB%25A6%25B0_%25EC%2595%2584%25EB%2591%2590%25EC%259D%25B4%25EB%2585%25B8_5_3.gif?type=w800">
