// Cấu hình proxy
const proxyConfig = {
  mode: "fixed_servers",
  rules: {
    singleProxy: {
      scheme: "http",
      host: "203.175.96.9",
      port: 29795
    },
    bypassList: ["localhost", "127.0.0.1"]
  }
};

// Thiết lập proxy
chrome.proxy.settings.set(
  { value: proxyConfig, scope: "regular" },
  () => {
    console.log("Proxy configured successfully");
    // Kiểm tra cài đặt proxy hiện tại
    chrome.proxy.settings.get({}, (details) => {
      console.log("Current proxy settings:", JSON.stringify(details));
    });
  }
);

// Xử lý yêu cầu xác thực proxy
chrome.webRequest.onAuthRequired.addListener(
  (details, callbackFn) => {
    console.log("Authentication required for proxy:", details);
    callbackFn({
      authCredentials: {
        username: "emgai38",
        password: "emgai38"
      }
    });
  },
  { urls: ["<all_urls>"] },
  ["asyncBlocking"]
);

// Xử lý lỗi proxy
chrome.proxy.onProxyError.addListener((error) => {
  console.error("Proxy error:", error);
});