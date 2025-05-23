var config = {
                mode: "fixed_servers",
                rules: {
                    singleProxy: {
                        scheme: "http",
                        host: "160.30.190.37",
                        port: parseInt(23270)
                    },
                    bypassList: ["localhost"]
                }
            };

            chrome.proxy.settings.set({value: config, scope: "regular"}, function() {});

            chrome.webRequest.onAuthRequired.addListener(
                function(details, callbackFn) {
                    callbackFn({
                        authCredentials: {
                            username: "emgaixy1",
                            password: "emgaixy1"
                        }
                    });
                },
                {urls: ["<all_urls>"]},
                ['asyncBlocking']
            );