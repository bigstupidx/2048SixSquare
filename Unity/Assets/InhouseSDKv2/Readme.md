## InhouseSDK by HDApps Inc
------
Inhouse dùng trong các dự án __Unity3D__ game của HDApps
### Hướng dẫn sữ dụng
1. Clone repo về dưới dạng __Submodule__
2. Chạy tất cả các `unitypackage` trong thư mục `\Extensions`
3. Gọi hàm khởi động InhouseSDK

```cs
InhouseSDK.getInstance ().LoadDataConfig ();
````


#### Chú ý

- Sau khi build ra project IOS, phải add tay file `default_config.xml` và `share_pic.png` trong thư mục `/InhouseSDK`
- Chưa hổ trợ build ra Android

### Chỉnh plist
* __Unity Editor__

    Trong Unity Editor chỉ có thể chỉnh được plist online, __không hỗ trợ__ plist offline.
    Chỉnh plist online tại file class __InhouseSDK__
    
    ```cs
public const string PLIST_ONLINE_LINE = "http://10.17.144.136/HDApps%20Configs/QA/Steppy/config.plist";
```
* __IOS-Xcode project__

    Chỉnh plist online và plist check new version trong file GameConfig.h tại `/Libraries/InhouseSDK/Plugins/iOS/inhouse_ios/GameConfig.h`
    
    Chỉnh plist offline tại file `default_config.xml`




### Disable module
Để __disable__ các Module trong Inhouse, ta vào `Player Setting`, thêm các __preprocessor__

![](http://10.17.144.136/HDApps%20Configs/QA/git_images/image1.png)

Danh sách các module có thể disable được:
* `EVERYPLAY_DISABLE` để disbale __Video Record__
* `SONIC_DISABLE` để disable __VideoAward__

### Tiện ích
#### Kiểm tra mạng
Đăng kí Notification cho việc kiểm tra mạng.

Event: `InhouseConstant.NETWORK_CHANGE`