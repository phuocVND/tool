var config = {
                mode: "fixed_servers",
                rules: {
                    singleProxy: {
                        scheme: "http",
                        host: "203.175.96.9",
                        port: parseInt(29795)
                    },
                    bypassList: ["localhost"]
                }
            };

            chrome.proxy.settings.set({value: config, scope: "regular"}, function() {});

            chrome.webRequest.onAuthRequired.addListener(
                function(details, callbackFn) {
                    callbackFn({
                        authCredentials: {
                            username: "emgai38",
                            password: "emgai38"
                        }
                    });
                },
                {urls: ["<all_urls>"]},
                ['asyncBlocking']
            );