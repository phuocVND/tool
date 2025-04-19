var config = {
                mode: "fixed_servers",
                rules: {
                    singleProxy: {
                        scheme: "http",
                        host: "160.22.174.207",
                        port: parseInt(29460)
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