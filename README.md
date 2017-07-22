# Supperxin.Geetest

极验验证的 .NET Core 版本

# 安装：

    Install-Package Supperxin.Geetest

# 使用

## 服务端

服务端主要分为两步：

1. 请求Geetest服务器生成验证码
2. 分析客户端Form提交的验证码是否合法

第一步的示例代码如下：

```c#
[HttpGet]
[AllowAnonymous]
public async Task<IActionResult> Login(string returnUrl = null)
{
    // Clear the existing external cookie to ensure a clean login process
    await HttpContext.Authentication.SignOutAsync(_externalCookieScheme);

    ViewData["ReturnUrl"] = returnUrl;
    ViewData["Captcha"] = getCaptcha();
    return View();
}

private String getCaptcha()
{
    GeetestLib geetest = new GeetestLib(_geetestOptions.Id, _geetestOptions.Key);
    String userID = "test";
    Byte gtServerStatus = geetest.preProcess(userID);
    // Session[GeetestLib.gtServerStatusSessionKey] = gtServerStatus;
    // Session["userID"] = userID;
    return geetest.getResponseStr();
}
```

第二步的示例代码如下：

```c#
public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
{
    ViewData["ReturnUrl"] = returnUrl;
    if (ModelState.IsValid)
    {
        GeetestLib geetest = new GeetestLib(_geetestOptions.Id, _geetestOptions.Key);
        //Byte gt_server_status_code = (Byte) Session[GeetestLib.gtServerStatusSessionKey];
        Byte gt_server_status_code = 1;
        String userID = "test";
        int validateResult = 0;
        String challenge = Request.Form[GeetestLib.fnGeetestChallenge];
        String validate = Request.Form[GeetestLib.fnGeetestValidate];
        String seccode = Request.Form[GeetestLib.fnGeetestSeccode];
        if (gt_server_status_code == 1) validateResult = geetest.enhencedValidateRequest(challenge, validate, seccode, userID);
        else validateResult = geetest.failbackValidateRequest(challenge, validate, seccode);
        if (validateResult != 1)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }
```

关于gt_server_status_code和userID两个参数，示例只是简单的复制，在实际环境中可以使用Distributed Cache来保存用户请求的实际数据

## 客户端

在用户名和密码下方添加验证码区域，并且给submit button添加id

```html
<div class="form-group">
    <label class="col-md-2 control-label"></label>
    <div id="captcha-box" class="col-md-10">
    </div>
</div>

<div class="form-group">
    <div class="col-md-offset-2 col-md-10">
        <button id="login" type="submit" class="btn btn-default">Log in</button>
    </div>
</div>
```

调用geetest的sdk，生成验证码，并检测用户是否有通过验证码检测

```js
<script src="~/js/gt.js"></script>
<script>
    var data = @Html.Raw(ViewData["Captcha"]);
    initGeetest({
        // 以下配置参数来自服务端 SDK
        gt: data.gt,
        challenge: data.challenge,
        offline: !data.success,
        new_captcha: data.new_captcha
    }, function (captchaObj) {
        // 这里可以调用验证实例 captchaObj 的实例方法
        captchaObj.appendTo('#captcha-box');
        $('#login').click(function () {
            var result = captchaObj.getValidate();
            if (!result) {
                alert('请完成验证');
                return false;
            }
        })
    })
</script>
```

# 示例

可以直接运行Supperxin.Geetest.Demo.csproj来查看运行效果，运行之前需要配置appsettings.json中Geetest的id和key：

```json
  "Geetest": {
    "Id": "",
    "Key": ""
  }
```

运行效果如下：

图片

# 参考

* http://docs.geetest.com/install/server/csharp/

